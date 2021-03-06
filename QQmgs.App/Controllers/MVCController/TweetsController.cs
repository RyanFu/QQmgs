﻿using Microsoft.AspNet.Identity;
using PagedList;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Web.Mvc;

using Twitter.App.Models.BindingModel;
using Twitter.App.Models.ViewModels;
using Twitter.Data.UnitOfWork;
using Twitter.Models;
using Twitter.App.BusinessLogic;
using Twitter.Models.GroupModels;
using Constants = Twitter.App.Constants.Constants;

namespace Twitter.App.Controllers
{
    [Authorize]
    [RoutePrefix("tweets")]
    public class TweetsController : TwitterBaseController
    {
        public TweetsController()
            : base(new QQmgsData())
        {
        }

        // GET /tweets/create
        //[HttpGet]
        //[Route("create")]
        //public ActionResult NewTweetForm()
        //{
        //    return this.PartialView("_NewTweetModal");
        //}

        // GET /tweets/MyTweets
        [HttpGet]
        [Route("MyTweets")]
        public ActionResult GetTweetBySpecifiedUser(int p = 1)
        {
            var loggedUserId = this.User.Identity.GetUserId();

            var recentTweets = Data.Tweets.All()
                .OrderByDescending(t => t.DatePosted)
                .Where(t => t.Author.Id == loggedUserId)
                .Select(ViewModelsHelper.AsTweetViewModel).ToList();

            var pagedTweets = recentTweets.ToPagedList(pageNumber: p, pageSize: 6);

            return this.View(pagedTweets);
        }

        // GET /tweets/HotTweets
        [HttpGet]
        [Route("HotTweets")]
        public ActionResult GetHotTweets(int p = 1)
        {
            var recentTweets = Data.Tweets.All()
                .OrderByDescending(t => t.UsersFavourite.Count)
                .Select(ViewModelsHelper.AsTweetViewModel)
                .Take(12).ToList();

            var pagedTweets = recentTweets.ToPagedList(pageNumber: p, pageSize: 6);

            return this.View(pagedTweets);
        }

        // GET /tweets?tweetId={tweetId}
        [HttpGet]
        [Route("{tweetId:int}/details")]
        public ActionResult GetTweet(int tweetId)
        {
            var tweet = Data.Tweets.All()
                .OrderByDescending(t => t.DatePosted)
                .Where(t => t.Id == tweetId)
                .Select(ViewModelsHelper.AsTweetViewModel)
                .FirstOrDefault();

            if (tweet == null)
            {
                return HttpNotFound($"Tweet with id {tweetId} not found");
            }

            return this.View(tweet);
        }

        [HttpGet]
        [Route("add")]
        public ActionResult InsertTweet()
        {
            return View();
        }

        [HttpPost]
        [Route("add")]
        public ActionResult InsertTweet(CreateTweetBindingModel model, bool directToHome = false)
        {
            if (!this.ModelState.IsValid)
            {
                this.Response.StatusCode = 400;
                return this.Json(this.ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage));
            }

            var loggedUserId = this.User.Identity.GetUserId();
            var loggedUserUsername = this.User.Identity.GetUserName();

            // for test
            model.GroupId = 1;
            var group = Data.Group.Find(model.GroupId);
            if (group == null)
            {
                this.Response.StatusCode = 400;
                return this.Json(this.ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage));
            }

            var tweet = new Tweet
            {
                Text = model.Text,
                AuthorId = loggedUserId,
                DatePosted = DateTime.Now,
                IsSoftDeleted = false,
                GroupId = model.GroupId
            };

            this.Data.Tweets.Add(tweet);
            this.Data.SaveChanges();

            if (directToHome)
            {
                return RedirectToAction("Index", "Home");
            }

            return this.PartialView(
                "_Tweet",
                new TweetViewModel
                {
                    Id = tweet.Id,
                    Author = loggedUserUsername,
                    DatePosted = tweet.DatePosted,
                    RetweetsCount = tweet.Retweets.Count,
                    RepliesCount = tweet.Reply.Count,
                    Text = tweet.Text,
                    UsersFavouriteCount = tweet.UsersFavourite.Count,
                    GroupId = tweet.GroupId
                });
        }

        [HttpGet]
        [Route("create")]
        public ActionResult CreateTweet(int groupId)
        {
            var group = Data.Group.Find(groupId);
            if (group == null)
            {
                return HttpNotFound($"Group with id {groupId} not found");
            }

            // pass Group info to view
            ViewData["GroupName"] = group.Name;
            ViewData["GroupId"] = groupId;

            return View();
        }

        [HttpPost]
        [Route("create")]
        public ActionResult CreateTweet(CreateTweetBindingModel model)
        {
            // pass Group info to view
            ViewData["GroupName"] = "";
            ViewData["GroupId"] = model.GroupId;

            if (!this.ModelState.IsValid)
            {
                return this.View(model);
            }

            var loggedUserId = this.User.Identity.GetUserId();

            var group = Data.Group.Find(model.GroupId);
            if (group == null)
            {
                this.Response.StatusCode = 400;
                return this.Json(this.ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage));
            }

            // update group last tweet update time
            group.LastTweetUpdateTime = DateTime.Now;
            this.Data.Group.Update(group);
            this.Data.SaveChanges();

            var tweet = new Tweet
            {
                Text = model.Text,
                AuthorId = loggedUserId,
                DatePosted = DateTime.Now,
                IsSoftDeleted = false,
                GroupId = model.GroupId
            };

            this.Data.Tweets.Add(tweet);
            this.Data.SaveChanges();

            return this.RedirectToAction("Get", "Group", new { groupId = model.GroupId, p = 1 });
        }

        [HttpPost]
        [Route("reply")]
        public ActionResult Reply(ReplyViewModel model)
        {
            var tweet = Data.Tweets.Find(model.Id);

            if (tweet == null)
            {
                return this.PartialView("_Tweet");
            }

            if (!this.ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(state => state.Errors).ToList();
                foreach (var error in errors)
                {
                    this.ModelState.AddModelError(string.Empty, error.ErrorMessage);
                }

                return RedirectToAction("GetPostDetail", "Group", new { groupId = tweet.GroupId, tweetId = tweet.Id });
            }

            var loggedUserId = this.User.Identity.GetUserId();

            var reply = new Reply
            {
                AuthorId = loggedUserId,
                Content = model.Text,
                PublishTime = DateTime.Now,
                TweetId = model.Id
            };

            try
            {
                this.Data.Reply.Add(reply);
                this.Data.SaveChanges();
            }
            catch (Exception)
            {
                return PartialView(
                    "Reply",
                    new ReplyViewModel
                    {
                        Text = "Reply should not be empty or less than 250 words."
                    });
            }

            var group = Data.Group.Find(tweet.GroupId);
            if (group == null)
            {
                this.Response.StatusCode = 400;
                return this.Json(this.ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage));
            }

            // update group last tweet update time
            group.LastTweetUpdateTime = DateTime.Now;
            this.Data.Group.Update(group);
            this.Data.SaveChanges();

            return RedirectToAction("GetPostDetail", "Group", new { groupId = tweet.GroupId, tweetId = tweet.Id });

            //return PartialView(
            //    "Reply",
            //    new ReplyViewModel
            //    {
            //        PublishTime = reply.PublishTime,
            //        Text = reply.Content,
            //        Author = reply.Author.RealName
            //    });
        }

        [HttpGet]
        public int Favourite(int tweetId)
        {
            var loggedUserId = this.User.Identity.GetUserId();
            var tweet = this.Data.Tweets.Find(tweetId);

            if (Data.Users.Find(loggedUserId).FavouriteTweets.Contains(tweet))
            {
                this.Data.Users.Find(loggedUserId).FavouriteTweets.Remove(tweet);
                this.Data.SaveChanges();
            }
            else
            {
                this.Data.Users.Find(loggedUserId).FavouriteTweets.Add(tweet);
                this.Data.SaveChanges();
            }

            return tweet.UsersFavourite.Count;
        }

        [HttpGet]
        public RedirectToRouteResult FavouriteTempRedirectToDetail(int groupId, int tweetId)
        {
            var loggedUserId = this.User.Identity.GetUserId();
            var tweet = this.Data.Tweets.Find(tweetId);

            if (Data.Users.Find(loggedUserId).FavouriteTweets.Contains(tweet))
            {
                this.Data.Users.Find(loggedUserId).FavouriteTweets.Remove(tweet);
                this.Data.SaveChanges();
            }
            else
            {
                this.Data.Users.Find(loggedUserId).FavouriteTweets.Add(tweet);
                this.Data.SaveChanges();
            }

            return RedirectToAction("GetPostDetail", "Group", new {groupId = groupId, tweetId = tweetId});
        }

        [HttpGet]
        [Route("{tweetId:int}/edit")]
        public ActionResult Edit(int? tweetId)
        {
            if (tweetId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Tweet tweet = Data.Tweets.Find(tweetId);
            if (tweet == null)
            {
                return HttpNotFound();
            }

            if (tweet.AuthorId != User.Identity.GetUserId())
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return View(tweet);
        }

        [HttpPost]
        [Route("{tweetId:int}/edit")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Tweet tweet)
        {
            var loggedUserId = this.User.Identity.GetUserId();
            if (tweet.AuthorId != loggedUserId)
            {
                this.Response.StatusCode = 400;
                return this.Json(this.ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage));
            }

            tweet.DatePosted = DateTime.Now;

            this.Data.Tweets.Update(tweet);
            this.Data.SaveChanges();

            return RedirectToAction("GetTweet", "Tweets", new { tweetId = tweet.Id });

            // return View(tweet);
        }

        [Route("delete")]
        public string Delete(int tweetId)
        {
            var loggedUserId = this.User.Identity.GetUserId();
            var tweet = this.Data.Tweets.Find(tweetId);

            var removedTweet = this.Data.Tweets.Remove(tweetId);

            this.Data.SaveChanges();

            return "Delete Successfully";
        }
    }
}