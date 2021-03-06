﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using Twitter.App.DataContracts;
using Twitter.App.Models.ViewModels;
using Twitter.Models;
using Twitter.Models.ActivityModels;
using Twitter.Models.CourseReviewModels;
using Twitter.Models.GroupModels;
using Twitter.Models.PhotoModels;
using Twitter.Models.TraceModels;
using Twitter.Models.UserModels;

namespace Twitter.App.BusinessLogic
{
    public static class ViewModelsHelper
    {
        #region Select Expression

        public static readonly Expression<Func<Group, GroupVieModels>> AsGroupViewModel =
            g => new GroupVieModels
            {
                Id = g.Id,
                CreatedTime = g.CreatedTime,
                Name = g.Name,
                HasImageOverview = g.HasImageOverview,
                ImageOverview = g.ImageOverview,
                Description = g.Description,
                TweetsCount = g.Tweets.Count,
                LastTweetUpdateTime = g.LastTweetUpdateTime,
                IsDisplay = g.IsDisplay,
                IsPrivate = g.IsPrivate,
                CreaterId = g.CreaterId,
                GroupPhotos = g.GroupPhotos.Select(photo => new GroupPhotoViewModel
                {
                    Id = photo.Id,
                    DatePosted = photo.DatePosted,
                    Name = photo.Name,
                    Description = photo.Description,
                    AuthorId = photo.AuthorId,
                    IsSoftDelete = photo.IsSoftDelete
                })
            };

        public static readonly Expression<Func<Tweet, TweetViewModel>> AsTweetViewModel =
            t => new TweetViewModel
            {
                Id = t.Id,
                Author = t.Author.RealName,
                AuthorStatus = t.Author.Status,
                AuthorPhoneNumber = t.Author.UserName,
                IsSoftDeleted = t.IsSoftDeleted,
                Text = t.Text,
                UsersFavouriteCount = t.UsersFavourite.Count,
                RepliesCount = t.Reply.Count,
                RetweetsCount = t.Retweets.Count,
                DatePosted = t.DatePosted,
                GroupId = t.GroupId,
                HasAvatarImage = t.Author.HasAvatarImage,
                AvatarImageName =
                    t.Author.HasAvatarImage
                        ? Constants.Constants.WebHostPrefix + "/" + Constants.Constants.ImageThumbnailsPrefix + "/" +
                          t.Author.AvatarImageName
                        : null,
                ReplyList = t.Reply.Select(reply => new ReplyViewModel
                {
                    Text = reply.Content,
                    Id = reply.Id,
                    PublishTime = reply.PublishTime,
                    Author = reply.Author.RealName
                }).ToList()
            };

        public static readonly Expression<Func<CourseReview, CourseReviewVideModel>> AsReviewModel =
            r => new CourseReviewVideModel
            {
                Id = r.Id,
                Comment = r.Comment,
                Course = r.Course,
                Teacher = r.Teacher,
                DatePosted = r.DatePosted,
                Author = r.Author.RealName
            };

        public static readonly Expression<Func<User, UserViewModel>> AsUserViewModel =
            u => new UserViewModel
            {
                RealName = u.RealName,
                Class = u.Class,
                Status = u.Status,
                UserId = u.Id,
                HasAvatarImage = u.HasAvatarImage,
                AvatarImageName = u.AvatarImageName,
                PhoneNumber = u.UserName,
                RegisteredTime = u.RegisteredTime
            };

        public static readonly Expression<Func<Image, PhotoViewModel>> AsPhotoViewModel =
            p => new PhotoViewModel
            {
                Description = p.Description,
                Author = p.Author.RealName,
                Name = p.Name,
                Height = p.Height,
                Width = p.Width,
                Id = p.Id,
                HasAvatarImage = p.Author.HasAvatarImage,
                AvatarImageName = p.Author.AvatarImageName
            };

        public static readonly Expression<Func<UserLogTrace, UserLoginTraceViewModel>> AsUserLoginTraceVideModel =
            u => new UserLoginTraceViewModel
            {
                DatePosted = u.DatePosted,
                Id = u.TraceId,
                IpAddress = u.IpAddress,
                IsLoggedSucceeded = u.IsLoggedSucceeded,
                LoggedUserPhoneNumber = u.PhoneNumber
            };

        public static readonly Expression<Func<ActivityPhoto, ActivityPhotoViewModel>> AsActivictyPhotoViewModel =
            p => new ActivityPhotoViewModel
            {
                Name = p.Name,
                DatePosted = p.DatePosted,
                ActivityPhotoType = p.ActivityPhotoType
            };

        public static readonly Expression<Func<Activity, ActivityViewModel>> AsActivictyViewModel =
            a => new ActivityViewModel
            {
                Id = a.Id,
                Name = a.Name,
                Classficiation = a.Classficiation.ToString(),
                AvatarImage = a.ActivityPhotos.Any()
                    ? a.ActivityPhotos.Select(photo => new ActivityPhotoViewModel
                    {
                        Name = photo.Name,
                        DatePosted = photo.DatePosted,
                        ActivityPhotoType = photo.ActivityPhotoType
                    })
                        .Where(photo => photo.ActivityPhotoType == ActivityPhotoType.OverView)
                        .OrderByDescending(photo => photo.DatePosted)
                        .FirstOrDefault()
                        .Name
                    : string.Empty,
                CreatorId = a.CreatorId,
                Description = a.Description,
                EndTime = a.EndTime,
                StartTime = a.StartTime,
                Place = a.Place,
                PublishTime = a.PublishTime,
                Creator = a.Creator.RealName,
                Participations = a.Participents.Select(participant => new ParticipationViewModel
                {
                    Id = participant.Id,
                    Name = participant.RealName,
                    AvatarImage = participant.UserProfilePhotos.Any()
                        ? participant.UserProfilePhotos.Select(image => new UserProfilePhotoViewModel
                        {
                            Name = image.Name,
                            DatePosted = image.DatePosted,
                            UserProfilePhotoType = image.UserProfilePhotoType
                        })
                            .Where(photo => photo.UserProfilePhotoType == UserProfilePhotoType.AvatarImage)
                            .OrderByDescending(photo => photo.DatePosted)
                            .FirstOrDefault()
                            .Name
                        : string.Empty,
                    HasAvatarImage = participant.HasAvatarImage
                }),
                CreatorAvatarImage = a.Creator.UserProfilePhotos.Any()
                    ? a.Creator.UserProfilePhotos.Select(image => new UserProfilePhotoViewModel
                    {
                        Name = image.Name,
                        DatePosted = image.DatePosted,
                        UserProfilePhotoType = image.UserProfilePhotoType
                    })
                        .Where(photo => photo.UserProfilePhotoType == UserProfilePhotoType.AvatarImage)
                        .OrderByDescending(photo => photo.DatePosted)
                        .FirstOrDefault()
                        .Name
                    : string.Empty,
                HasCreatorAvatarImage = a.Creator.UserProfilePhotos.Any(),
                IsDisplay = a.IsDisplay,
                Organizer = a.Organizer,
                CreatorStatus = a.Creator.Status
            };

        public static readonly Expression<Func<HighAccLocationByIpResult, UserLoginTraceViewModel>>
            AsUserLoginTraceViewModel = u => new UserLoginTraceViewModel
            {
                DatePosted = u.DatePosted,
                Id = u.Id,
                IpAddress = u.IpAddress,
                IsLoggedSucceeded = u.IsLoggedSucceeded,
                LoggedUserName = u.LoggedUserName,
                LoggedUserPhoneNumber = u.LoggedUserPhoneNumber,
                HighAccIpLocation = new HighAccIpLocation
                {
                    Content = new Content
                    {
                        FormattedAddress = u.FormattedAddress,
                        Confidence = u.Confidence ?? 0.0,
                        AddressComponent = new AddressComponent
                        {
                            AdminAreaCode = u.AdminAreaCode ?? 0,
                            City = u.City,
                            Country = u.Country,
                            District = u.District,
                            Province = u.Province,
                            Street = u.Street,
                            StreetNumber = u.StreetNumber
                        },
                        Business = u.Business,
                        LocId = u.LocId,
                        Location = new Location
                        {
                            Lat = u.Lat ?? 0.0,
                            Lng = u.Lng ?? 0.0
                        },
                        Radius = u.Radius ?? 0.0
                    },
                    Result = new Result
                    {
                        Error = u.Error ?? 0,
                        LocalTime = u.LocalTime
                    }
                }
            };

        #endregion

        #region Tweet View Model

        public static TweetViewModel ToTweetViewModel(this Tweet t)
        {
            return new TweetViewModel
            {
                Id = t.Id,
                Author = t.Author.RealName,
                AuthorStatus = t.Author.Status,
                AuthorPhoneNumber = t.Author.UserName,
                IsSoftDeleted = t.IsSoftDeleted,
                Text = t.Text,
                UsersFavouriteCount = t.UsersFavourite.Count,
                RepliesCount = t.Reply.Count,
                RetweetsCount = t.Retweets.Count,
                DatePosted = t.DatePosted,
                GroupId = t.GroupId,
                HasAvatarImage = t.Author.UserProfilePhotos.Any(image => image.UserProfilePhotoType == UserProfilePhotoType.AvatarImage),
                AvatarImageName = t.Author.UserProfilePhotos.Any()
                    ? t.Author.UserProfilePhotos.Select(image => new UserProfilePhotoViewModel
                    {
                        Name = image.Name,
                        DatePosted = image.DatePosted,
                        UserProfilePhotoType = image.UserProfilePhotoType
                    })
                        .Where(photo => photo.UserProfilePhotoType == UserProfilePhotoType.AvatarImage)
                        .OrderByDescending(photo => photo.DatePosted)
                        .FirstOrDefault()?.Name
                    : string.Empty,
                ReplyList = t.Reply.Select(reply => new ReplyViewModel
                {
                    Text = reply.Content,
                    Id = reply.Id,
                    PublishTime = reply.PublishTime,
                    Author = reply.Author.RealName,
                    AvatarImageName =
                        reply.Author.UserProfilePhotos.Any()
                            ? reply.Author.UserProfilePhotos.Select(image => new UserProfilePhotoViewModel
                            {
                                Name = image.Name,
                                DatePosted = image.DatePosted,
                                UserProfilePhotoType = image.UserProfilePhotoType
                            })
                                .Where(photo => photo.UserProfilePhotoType == UserProfilePhotoType.AvatarImage)
                                .OrderByDescending(photo => photo.DatePosted)
                                .FirstOrDefault()?.Name
                            : string.Empty,
                    HasAvatarImage = reply.Author.UserProfilePhotos.Any(image => image.UserProfilePhotoType == UserProfilePhotoType.AvatarImage)
                })
            };
        }

        #endregion

        #region Activity View Model

        public static ActivityViewModel ToActivityViewModel(this Activity a)
        {
            return new ActivityViewModel
            {
                Id = a.Id,
                Name = a.Name,
                Classficiation = a.Classficiation.ToString(),
                AvatarImage = a.ActivityImage.RetrievePhotoThumnails(),
                CreatorId = a.CreatorId,
                Description = a.Description,
                EndTime = a.EndTime,
                StartTime = a.StartTime,
                Place = a.Place,
                PublishTime = a.PublishTime,
                Creator = a.Creator.RealName,
                Participations = a.Participents.Select(participant => new ParticipationViewModel
                {
                    Id = participant.Id,
                    Name = participant.RealName,
                    AvatarImage = participant.AvatarImageName.RetrievePhotoThumnails(participant.HasAvatarImage),
                    HasAvatarImage = participant.HasAvatarImage
                }).ToList(),
                CreatorAvatarImage = a.Creator.AvatarImageName,
                HasCreatorAvatarImage = a.Creator.HasAvatarImage,
                IsDisplay = a.IsDisplay,
                Organizer = a.Organizer,
                CreatorStatus = a.Creator.Status
            };
        }

        #endregion

        #region Group View Model

        public static GroupVieModels ToGroupVieModel(this Group g)
        {
            return new GroupVieModels
            {
                Id = g.Id,
                CreatedTime = g.CreatedTime,
                Name = g.Name,
                HasImageOverview = g.HasImageOverview,
                ImageOverview = g.ImageOverview,
                Description = g.Description,
                TweetsCount = g.Tweets.Count,
                LastTweetUpdateTime = g.LastTweetUpdateTime,
                IsDisplay = g.IsDisplay,
                IsPrivate = g.IsPrivate,
                CreaterId = g.CreaterId,
                GroupPhotos = g.GroupPhotos.Select(photo => new GroupPhotoViewModel
                {
                    Id = photo.Id,
                    DatePosted = photo.DatePosted,
                    Name = photo.Name,
                    Description = photo.Description,
                    AuthorId = photo.AuthorId,
                    IsSoftDelete = photo.IsSoftDelete
                })
            };
        }

        #endregion

        #region User View Model

        public static UserViewModel ToUserViewModel(this User user)
        {
            return new UserViewModel
            {
                RealName = user.RealName,
                Class = user.Class,
                Status = user.Status,
                UserId = user.Id,
                PhoneNumber = user.UserName,
                HasAvatarImage = user.HasAvatarImage,
                AvatarImageName = user.AvatarImageName.RetrievePhotoThumnails(user.HasAvatarImage),
                JoinedGroups = user.Groups.Select(group => new GroupVieModels
                {
                    Id = group.Id,
                    CreaterId = group.CreaterId,
                    CreatedTime = group.CreatedTime,
                    Name = group.Name,
                    HasImageOverview = group.HasImageOverview,
                    ImageOverview = group.ImageOverview.RetrievePhotoThumnails(group.HasImageOverview),
                    Description = group.Description,
                    TweetsCount = group.Tweets.Count,
                    LastTweetUpdateTime = group.LastTweetUpdateTime,
                    IsDisplay = group.IsDisplay,
                    IsPrivate = group.IsPrivate
                }),
                PostedTweets = user.Tweets.Select(tweet => new TweetViewModel
                {
                    Id = tweet.Id,
                    Author = tweet.Author.RealName,
                    AuthorStatus = tweet.Author.Status,
                    AuthorPhoneNumber = tweet.Author.UserName,
                    IsSoftDeleted = tweet.IsSoftDeleted,
                    Text = tweet.Text,
                    UsersFavouriteCount = tweet.UsersFavourite.Count,
                    RepliesCount = tweet.Reply.Count,
                    RetweetsCount = tweet.Retweets.Count,
                    DatePosted = tweet.DatePosted,
                    GroupId = tweet.GroupId,
                    HasAvatarImage = tweet.Author.HasAvatarImage,
                    AvatarImageName = tweet.Author.AvatarImageName.RetrievePhotoThumnails(tweet.Author.HasAvatarImage),
                    ReplyList = tweet.Reply.Select(reply => new ReplyViewModel
                    {
                        Text = reply.Content,
                        Id = reply.Id,
                        PublishTime = reply.PublishTime,
                        Author = reply.Author.RealName,
                        HasAvatarImage = reply.Author.HasAvatarImage,
                        AvatarImageName =
                            reply.Author.AvatarImageName.RetrievePhotoThumnails(reply.Author.HasAvatarImage)
                    }).ToList()
                }),
                JoinedActivities = user.JoinedActivities.Select(activity => new ActivityViewModel
                {
                    Id = activity.Id,
                    Name = activity.Name,
                    Classficiation = activity.Classficiation.ToString(),
                    AvatarImage = activity.ActivityImage.RetrievePhotoThumnails(),
                    CreatorId = activity.CreatorId,
                    Description = activity.Description,
                    EndTime = activity.EndTime,
                    StartTime = activity.StartTime,
                    Place = activity.Place,
                    PublishTime = activity.PublishTime,
                    Creator = activity.Creator.RealName,
                    Participations = activity.Participents.Select(participant => new ParticipationViewModel
                    {
                        Id = participant.Id,
                        Name = participant.RealName,
                        AvatarImage = participant.AvatarImageName.RetrievePhotoThumnails(participant.HasAvatarImage),
                        HasAvatarImage = participant.HasAvatarImage
                    }).ToList(),
                    CreatorAvatarImage =
                        activity.Creator.AvatarImageName.RetrievePhotoThumnails(activity.Creator.HasAvatarImage),
                    HasCreatorAvatarImage = activity.Creator.HasAvatarImage,
                    IsDisplay = activity.IsDisplay,
                    Organizer = activity.Organizer,
                    CreatorStatus = activity.Creator.Status
                }),
                CreatedActivities = user.CreatedActivities.Select(activity => new ActivityViewModel
                {
                    Id = activity.Id,
                    Name = activity.Name,
                    Classficiation = activity.Classficiation.ToString(),
                    AvatarImage = activity.ActivityImage ?? string.Empty,
                    CreatorId = activity.CreatorId,
                    Description = activity.Description,
                    EndTime = activity.EndTime,
                    StartTime = activity.StartTime,
                    Place = activity.Place,
                    PublishTime = activity.PublishTime,
                    Creator = activity.Creator.RealName,
                    Participations = activity.Participents.Select(participant => new ParticipationViewModel
                    {
                        Id = participant.Id,
                        Name = participant.RealName,
                        AvatarImage = participant.AvatarImageName.RetrievePhotoThumnails(participant.HasAvatarImage),
                        HasAvatarImage = participant.HasAvatarImage
                    }).ToList(),
                    CreatorAvatarImage =
                        activity.Creator.AvatarImageName.RetrievePhotoThumnails(activity.Creator.HasAvatarImage),
                    HasCreatorAvatarImage = activity.Creator.HasAvatarImage,
                    IsDisplay = activity.IsDisplay,
                    Organizer = activity.Organizer,
                    CreatorStatus = activity.Creator.Status
                }),
                PostedPhotos = user.Photos.Select(photo => new PhotoViewModel
                {
                    Description = photo.Description,
                    Author = photo.Author.RealName,
                    Name = photo.Name,
                    Height = photo.Height,
                    Width = photo.Width,
                    Id = photo.Id,
                    HasAvatarImage = photo.Author.HasAvatarImage,
                    AvatarImageName = photo.Author.AvatarImageName.RetrievePhotoThumnails(photo.Author.HasAvatarImage),
                    DatePosted = photo.DatePosted.ToString(CultureInfo.InvariantCulture)
                }),
                CreatedGroups = new List<GroupVieModels>()
            };
        }

        public static UserViewModel ToUserPostedTweetViewModel(this User user)
        {
            return new UserViewModel
            {
                RealName = user.RealName,
                Class = user.Class,
                Status = user.Status,
                UserId = user.Id,
                PhoneNumber = user.UserName,
                HasAvatarImage = user.HasAvatarImage,
                AvatarImageName = user.AvatarImageName.RetrievePhotoThumnails(user.HasAvatarImage),
                PostedTweets = user.Tweets.Select(tweet => new TweetViewModel
                {
                    Id = tweet.Id,
                    Author = tweet.Author.RealName,
                    AuthorStatus = tweet.Author.Status,
                    AuthorPhoneNumber = tweet.Author.UserName,
                    IsSoftDeleted = tweet.IsSoftDeleted,
                    Text = tweet.Text,
                    UsersFavouriteCount = tweet.UsersFavourite.Count,
                    RepliesCount = tweet.Reply.Count,
                    RetweetsCount = tweet.Retweets.Count,
                    DatePosted = tweet.DatePosted,
                    GroupId = tweet.GroupId,
                    HasAvatarImage = tweet.Author.HasAvatarImage,
                    AvatarImageName = tweet.Author.AvatarImageName.RetrievePhotoThumnails(tweet.Author.HasAvatarImage)
                }),
                JoinedActivities = new List<ActivityViewModel>(),
                PostedPhotos = new List<PhotoViewModel>(),
                JoinedGroups = new List<GroupVieModels>(),
                CreatedActivities = new List<ActivityViewModel>(),
                CreatedGroups = new List<GroupVieModels>()
            };
        }

        public static UserViewModel ToUserPostedPhotosViewModel(this User user)
        {
            return new UserViewModel
            {
                RealName = user.RealName,
                Class = user.Class,
                Status = user.Status,
                UserId = user.Id,
                PhoneNumber = user.UserName,
                HasAvatarImage = user.HasAvatarImage,
                AvatarImageName = user.AvatarImageName.RetrievePhotoThumnails(user.HasAvatarImage),
                PostedPhotos = user.Photos.Select(photo => new PhotoViewModel
                {
                    Description = photo.Description,
                    Author = photo.Author.RealName,
                    Name = photo.Name,
                    Height = photo.Height,
                    Width = photo.Width,
                    Id = photo.Id,
                    HasAvatarImage = photo.Author.HasAvatarImage,
                    AvatarImageName = photo.Author.AvatarImageName
                }),
                JoinedActivities = new List<ActivityViewModel>(),
                JoinedGroups = new List<GroupVieModels>(),
                PostedTweets = new List<TweetViewModel>(),
                CreatedActivities = new List<ActivityViewModel>(),
                CreatedGroups = new List<GroupVieModels>()
            };
        }

        public static UserViewModel ToUserJoinedGroupsViewModel(this User user)
        {
            return new UserViewModel
            {
                RealName = user.RealName,
                Class = user.Class,
                Status = user.Status,
                UserId = user.Id,
                PhoneNumber = user.UserName,
                HasAvatarImage = user.HasAvatarImage,
                AvatarImageName = user.AvatarImageName.RetrievePhotoThumnails(user.HasAvatarImage),
                JoinedGroups = user.Groups.Select(group => new GroupVieModels
                {
                    Id = group.Id,
                    CreatedTime = group.CreatedTime,
                    Name = group.Name,
                    HasImageOverview = group.HasImageOverview,
                    ImageOverview = group.ImageOverview,
                    Description = group.Description,
                    TweetsCount = group.Tweets.Count,
                    LastTweetUpdateTime = group.LastTweetUpdateTime,
                    IsDisplay = group.IsDisplay,
                    IsPrivate = group.IsPrivate
                }),
                JoinedActivities = new List<ActivityViewModel>(),
                PostedTweets = new List<TweetViewModel>(),
                PostedPhotos = new List<PhotoViewModel>(),
                CreatedActivities = new List<ActivityViewModel>(),
                CreatedGroups = new List<GroupVieModels>()
            };
        }

        public static UserViewModel ToUserCreatedGroupsViewModel(this User user)
        {
            return new UserViewModel
            {
                RealName = user.RealName,
                Class = user.Class,
                Status = user.Status,
                UserId = user.Id,
                PhoneNumber = user.UserName,
                HasAvatarImage = user.HasAvatarImage,
                AvatarImageName = user.AvatarImageName.RetrievePhotoThumnails(user.HasAvatarImage),
                CreatedGroups = user.Groups.Select(group => new GroupVieModels
                {
                    Id = group.Id,
                    CreatedTime = group.CreatedTime,
                    Name = group.Name,
                    HasImageOverview = group.HasImageOverview,
                    ImageOverview = group.ImageOverview,
                    Description = group.Description,
                    TweetsCount = group.Tweets.Count,
                    LastTweetUpdateTime = group.LastTweetUpdateTime,
                    IsDisplay = group.IsDisplay,
                    IsPrivate = group.IsPrivate
                }),
                JoinedActivities = new List<ActivityViewModel>(),
                PostedTweets = new List<TweetViewModel>(),
                PostedPhotos = new List<PhotoViewModel>(),
                CreatedActivities = new List<ActivityViewModel>(),
                JoinedGroups = new List<GroupVieModels>()
            };
        }

        public static UserViewModel ToUserJoinedActivitiesViewModel(this User user)
        {
            return new UserViewModel
            {
                RealName = user.RealName,
                Class = user.Class,
                Status = user.Status,
                UserId = user.Id,
                PhoneNumber = user.UserName,
                HasAvatarImage = user.HasAvatarImage,
                AvatarImageName = user.AvatarImageName.RetrievePhotoThumnails(user.HasAvatarImage),
                JoinedActivities = user.JoinedActivities.Select(activity => new ActivityViewModel
                {
                    Id = activity.Id,
                    Name = activity.Name,
                    Classficiation = activity.Classficiation.ToString(),
                    AvatarImage = activity.ActivityImage ?? string.Empty,
                    CreatorId = activity.CreatorId,
                    Description = activity.Description,
                    EndTime = activity.EndTime,
                    StartTime = activity.StartTime,
                    Place = activity.Place,
                    PublishTime = activity.PublishTime,
                    Creator = activity.Creator.RealName,
                    Participations = activity.Participents.Select(participant => new ParticipationViewModel
                    {
                        Id = participant.Id,
                        Name = participant.RealName,
                        AvatarImage = participant.AvatarImageName.RetrievePhotoThumnails(participant.HasAvatarImage),
                        HasAvatarImage = participant.HasAvatarImage
                    }).ToList(),
                    CreatorAvatarImage =
                        activity.Creator.AvatarImageName.RetrievePhotoThumnails(activity.Creator.HasAvatarImage),
                    HasCreatorAvatarImage = activity.Creator.HasAvatarImage,
                    IsDisplay = activity.IsDisplay,
                    Organizer = activity.Organizer,
                    CreatorStatus = activity.Creator.Status
                }),
                PostedTweets = new List<TweetViewModel>(),
                PostedPhotos = new List<PhotoViewModel>(),
                JoinedGroups = new List<GroupVieModels>(),
                CreatedActivities = new List<ActivityViewModel>(),
                CreatedGroups = new List<GroupVieModels>()
            };
        }

        public static UserViewModel ToUserCreatedActivitiesViewModel(this User user)
        {
            return new UserViewModel
            {
                RealName = user.RealName,
                Class = user.Class,
                Status = user.Status,
                UserId = user.Id,
                PhoneNumber = user.UserName,
                HasAvatarImage = user.HasAvatarImage,
                AvatarImageName = user.AvatarImageName.RetrievePhotoThumnails(user.HasAvatarImage),
                CreatedActivities = user.CreatedActivities.Select(activity => new ActivityViewModel
                {
                    Id = activity.Id,
                    Name = activity.Name,
                    Classficiation = activity.Classficiation.ToString(),
                    AvatarImage = activity.ActivityImage ?? string.Empty,
                    CreatorId = activity.CreatorId,
                    Description = activity.Description,
                    EndTime = activity.EndTime,
                    StartTime = activity.StartTime,
                    Place = activity.Place,
                    PublishTime = activity.PublishTime,
                    Creator = activity.Creator.RealName,
                    Participations = activity.Participents.Select(participant => new ParticipationViewModel
                    {
                        Id = participant.Id,
                        Name = participant.RealName,
                        AvatarImage = participant.AvatarImageName.RetrievePhotoThumnails(participant.HasAvatarImage),
                        HasAvatarImage = participant.HasAvatarImage
                    }).ToList(),
                    CreatorAvatarImage =
                        activity.Creator.AvatarImageName.RetrievePhotoThumnails(activity.Creator.HasAvatarImage),
                    HasCreatorAvatarImage = activity.Creator.HasAvatarImage,
                    IsDisplay = activity.IsDisplay,
                    Organizer = activity.Organizer,
                    CreatorStatus = activity.Creator.Status
                }),
                PostedTweets = new List<TweetViewModel>(),
                PostedPhotos = new List<PhotoViewModel>(),
                JoinedGroups = new List<GroupVieModels>(),
                JoinedActivities = new List<ActivityViewModel>(),
                CreatedGroups = new List<GroupVieModels>()
            };
        }

        public static UserViewModel ToUserBioViewModel(this User user)
        {
            return new UserViewModel
            {
                RealName = user.RealName,
                Class = user.Class,
                Status = user.Status,
                UserId = user.Id,
                PhoneNumber = user.UserName,
                HasAvatarImage = user.HasAvatarImage,
                AvatarImageName = user.AvatarImageName.RetrievePhotoThumnails(user.HasAvatarImage),
                JoinedActivities = new List<ActivityViewModel>(),
                PostedTweets = new List<TweetViewModel>(),
                PostedPhotos = new List<PhotoViewModel>(),
                JoinedGroups = new List<GroupVieModels>(),
                CreatedActivities = new List<ActivityViewModel>(),
                CreatedGroups = new List<GroupVieModels>()
            };
        }

        #endregion

        #region Group Plugins View Model

        public static GroupPluginViewModel ToGroupPluginViewModel(this GroupPlugin plugin)
        {
            return new GroupPluginViewModel
            {
                HasCheckInPad = plugin.HasCheckInPad,
                HasDisplayWall = plugin.HasDisplayWall,
                HasInfoFocus = plugin.HasInfoFocus
            };
        }

        #endregion

    }
}