﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using Twitter.App.Models.ViewModels;
using Twitter.Models;

namespace Twitter.App.BusinessLogic
{
    public class ViewModelsHelper
    {
        public static readonly Expression<Func<Group, GroupVieModels>> AsGroupViewModel =
            t => new GroupVieModels
            {
                Id = t.Id,
                CreatedTime = t.CreatedTime,
                Name = t.Name,
                TweetsCount = t.Tweets.Count
            };

        public static readonly Expression<Func<Tweet, TweetViewModel>> AsTweetViewModel =
            t => new TweetViewModel
            {
                Id = t.Id,
                Author = t.Author.UserName,
                AuthorStatus = t.Author.Status,
                IsEvent = t.IsEvent,
                Text = t.Text,
                UsersFavouriteCount = t.UsersFavourite.Count,
                RepliesCount = t.Reply.Count,
                RetweetsCount = t.Retweets.Count,
                DatePosted = t.DatePosted,
                GroupId = t.GroupId,
                ReplyList = t.Reply.Select(reply => new ReplyViewModel
                {
                    Text = reply.Content,
                    Id = reply.Id,
                    PublishTime = reply.PublishTime,
                    Author = reply.AuthorName
                }).ToList()
            };
    }
}