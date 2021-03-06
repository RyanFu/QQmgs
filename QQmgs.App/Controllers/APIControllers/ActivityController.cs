﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Security;
using Microsoft.AspNet.Identity;
using Twitter.App.BusinessLogic;
using Twitter.App.Common;
using Twitter.App.DataContracts;
using Twitter.App.Filters;
using Twitter.App.Models.BindingModel;
using Twitter.App.Models.BindingModels;
using Twitter.App.Models.ViewModels;
using Twitter.App.Provider;
using Twitter.Data.UnitOfWork;
using Twitter.Models;
using Twitter.Models.ActivityModels;
using Twitter.Models.Interfaces;
using Twitter.Models.PhotoModels;

namespace Twitter.App.Controllers.APIControllers
{
    [RoutePrefix("api/activity")]
    [Authorize]
    public class ActivityController : TwitterApiController
    {
        public ActivityController()
            : base(new QQmgsData())
        {
        }

        [HttpGet]
        [Route("{activityId:int}")]
        public HttpResponseMessage Get([FromUri] int activityId)
        {
            Guard.ArgumentNotNull(activityId, nameof(activityId));

            var activity = Data.Activity.All()
                .Where(t => t.Id == activityId)
                .Select(ViewModelsHelper.AsActivictyViewModel)
                .FirstOrDefault();

            if (activity == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, $"Cannot find activity for activity ID {activityId}");
            }

            // assign avatar image full url & activity avatar image
            activity.AvatarImage = activity.AvatarImage.RetrievePhotoThumnails();
            activity.CreatorAvatarImage = activity.CreatorAvatarImage.RetrievePhotoThumnails();

            foreach (var participation in activity.Participations)
            {
                participation.AvatarImage =
                    participation.AvatarImage.RetrievePhotoThumnails(participation.HasAvatarImage);
            }

            return Request.CreateResponse(HttpStatusCode.OK, activity);
        }

        [HttpGet]
        [Route("~/api/queries/activity")]
        public HttpResponseMessage GetAll([FromUri] string classcification = null, [FromUri] int pageNo = DefaultPageNo, [FromUri] int pageSize = DefaultPageSize)
        {
            if (pageNo <= 0 || pageSize <= 0)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "pageNo and pageSize should be both grater than 0.");
            }

            var activities = this.Data.Activity.All()
                .Select(ViewModelsHelper.AsActivictyViewModel)
                .Where(
                    model =>
                        model.Classficiation == classcification && classcification != null || classcification == null)
                .ToList();

            // retrieve creators data & activity avatar image
            foreach (var activity in activities)
            {
                activity.AvatarImage = activity.AvatarImage.RetrievePhotoThumnails();
                activity.CreatorAvatarImage = activity.CreatorAvatarImage.RetrievePhotoThumnails();

                foreach (var participation in activity.Participations)
                {
                    participation.AvatarImage =
                        participation.AvatarImage.RetrievePhotoThumnails(participation.HasAvatarImage);
                }
            }

            var pagedActivities = activities.GetPagedResult(t => t.PublishTime, pageNo, pageSize, SortDirection.Descending);

            return pagedActivities.Count == 0
                ? Request.CreateErrorResponse(HttpStatusCode.NoContent, $"No activity was found under classcification {classcification}")
                : Request.CreateResponse(HttpStatusCode.OK, new PaginationResult<ActivityViewModel>(pagedActivities, pageNo, pageSize, activities.Count));
        }

        [HttpGet]
        [Route("~/api/queries/hotActivity")]
        public HttpResponseMessage GetHotActivities([FromUri] string classcification = null, [FromUri] int pageNo = DefaultPageNo, [FromUri] int pageSize = DefaultPageSize)
        {
            if (pageNo <= 0 || pageSize <= 0)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "pageNo and pageSize should be both grater than 0.");
            }

            var activities = this.Data.Activity.All()
                .Select(ViewModelsHelper.AsActivictyViewModel)
                .Where(
                    model =>
                        model.Classficiation == classcification && classcification != null || classcification == null)
                .Take(2) // TODO: take top 2 activities if available
                .ToList();

            // retrieve creators data
            foreach (var activity in activities)
            {
                activity.CreatorAvatarImage = activity.CreatorAvatarImage.RetrievePhotoThumnails();
                foreach (var participation in activity.Participations)
                {
                    participation.AvatarImage =
                        participation.AvatarImage.RetrievePhotoThumnails(participation.HasAvatarImage);
                }
            }

            var pagedActivities = activities.GetPagedResult(t => t.PublishTime, pageNo, pageSize, SortDirection.Descending);

            return pagedActivities.Count == 0
                ? Request.CreateErrorResponse(HttpStatusCode.NoContent, $"No activity was found under classcification {classcification}")
                : Request.CreateResponse(HttpStatusCode.OK, new PaginationResult<ActivityViewModel>(pagedActivities, pageNo, pageSize, activities.Count));
        }

        [HttpPut]
        [Route("{activityId:int}")]
        public HttpResponseMessage Update([FromUri] int activityId, [FromBody] CreateActivityBindingModel model)
        {
            Guard.ArgumentNotNull(activityId, nameof(activityId));
            Guard.ArgumentNotNullOrEmpty(model.Name, $"{model}.{model.Name}");
            Guard.ArgumentNotNullOrEmpty(model.Description, $"{model}.{model.Description}");
            Guard.ArgumentNotNullOrEmpty(model.Place, $"{model}.{model.Place}");
            Guard.ArgumentNotNullOrEmpty(model.StartTime, $"{model}.{model.StartTime}");
            Guard.ArgumentNotNullOrEmpty(model.EndTime, $"{model}.{model.EndTime}");
            Guard.ArgumentNotNullOrEmpty(model.Classfication, $"{model}.{model.Classfication}");

            var activity = Data.Activity
                .All()
                .FirstOrDefault(t => t.Id == activityId);

            if (activity == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, $"Cannot find activity for activity ID {activityId}");
            }

            // Check ownership
            if (activity.CreatorId != this.User.Identity.GetUserId())
            {
                return Request.CreateErrorResponse(HttpStatusCode.Forbidden, $"Only creator {activity.CreatorId} can update the activity");
            }

            activity.Name = model.Name;
            activity.Description = model.Description;
            activity.Place = model.Place;
            activity.Classficiation = EnumUtils.Parse<ActivityClassficiation>(model.Classfication);
            activity.StartTime = DateTime.Parse(model.StartTime);
            activity.EndTime = DateTime.Parse(model.EndTime);
            activity.IsDisplay = model.IsDisplay;
            activity.Organizer = model.Organizer;

            this.Data.Activity.Update(activity);
            this.Data.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.OK, activity.ToActivityViewModel());
        }

        [HttpPost]
        [Route("")]
        public HttpResponseMessage Create([FromBody] CreateActivityBindingModel model)
        {
            Guard.ArgumentNotNullOrEmpty(model.Name, $"{model}.{model.Name}");
            Guard.ArgumentNotNullOrEmpty(model.Description, $"{model}.{model.Description}");
            Guard.ArgumentNotNullOrEmpty(model.Place, $"{model}.{model.Place}");
            Guard.ArgumentNotNullOrEmpty(model.StartTime, $"{model}.{model.StartTime}");
            Guard.ArgumentNotNullOrEmpty(model.EndTime, $"{model}.{model.EndTime}");
            Guard.ArgumentNotNullOrEmpty(model.Classfication, $"{model}.{model.Classfication}");

            var activity = new Activity
            {
                CreatorId = this.User.Identity.GetUserId(),
                Name = model.Name,
                Description = model.Description,
                PublishTime = DateTime.Now,
                Classficiation =
                    EnumUtils.Parse<ActivityClassficiation>(model.Classfication ??
                                                            ActivityClassficiation.Other.ToString()),
                Place = model.Place,
                StartTime = DateTime.Parse(model.StartTime),
                EndTime = DateTime.Parse(model.EndTime),
                IsDisplay = model.IsDisplay,
                Organizer = model.Organizer
            };

            this.Data.Activity.Add(activity);
            this.Data.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.OK, activity);
        }

        [HttpDelete]
        [Route("{activityId:int}")]
        public HttpResponseMessage Delete([FromUri] int activityId)
        {
            Guard.ArgumentNotNull(activityId, nameof(activityId));

            var activity = Data.Activity
                .All()
                .FirstOrDefault(t => t.Id == activityId);

            if (activity == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, $"Cannot find activity for activity ID {activityId}");
            }

            // Check ownership
            if (activity.CreatorId != this.User.Identity.GetUserId())
            {
                return Request.CreateErrorResponse(HttpStatusCode.Forbidden, $"Only creator {activity.CreatorId} can update the activity");
            }

            this.Data.Activity.Remove(activity);
            this.Data.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [HttpPost]
        [Route("{activityId:int}/activityImage")]
        public async Task<HttpResponseMessage> ActivityImage([FromUri] int activityId)
        {
            Guard.ArgumentNotNull(activityId, nameof(activityId));

            // Check if the request contains multipart/form-data.
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.UnsupportedMediaType,
                    "This request is not properly formatted"));
            }

            var activity = Data.Activity
                .All()
                .FirstOrDefault(t => t.Id == activityId);

            // Check activity existence
            if (activity == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, $"Cannot find activity for activity ID {activityId}");
            }

            // Check ownership
            var loggedUserId = this.User.Identity.GetUserId();
            if (activity.CreatorId != loggedUserId)
            {
                return Request.CreateErrorResponse(HttpStatusCode.Forbidden, $"Only creator {activity.CreatorId} can update the activity");
            }

            var root = HttpContext.Current.Server.MapPath(Constants.Constants.PhotoLocation);
            var provider = new CustomMultipartFormDataStreamProvider(root);

            try
            {
                // Retrieve the form data, and save the file on server
                await Request.Content.ReadAsMultipartAsync(provider);

                // Return no content if file data equal to zero
                var fileSize = provider.FileData.Count;
                if (fileSize == 0)
                {
                    return Request.CreateResponse(HttpStatusCode.NoContent);
                }

                foreach (var file in provider.FileData)
                {
                    // Check the user uploaded file name whether valiad
                    if (string.IsNullOrEmpty(file.Headers.ContentDisposition.FileName))
                    {
                        return Request.CreateResponse(HttpStatusCode.BadRequest, "This file name is null or empty");
                    }

                    var fileName = file.LocalFileName.Split(Path.DirectorySeparatorChar).Last();

                    var uploadedFile = FileUploadHelper.ResizeImage(Constants.Constants.DefaultResizeSize, Constants.Constants.DefaultResizeSize, file.LocalFileName, fileName, PhotoType.ActivityImage);

                    var activityOverView = new ActivityPhoto
                    {
                        AuthorId = loggedUserId,
                        DatePosted = DateTime.Now,
                        Name = fileName,
                        ActivityPhotoType = ActivityPhotoType.OverView,
                        IsSoftDelete = false,
                        Height = uploadedFile.Height,
                        Width = uploadedFile.Width
                    };
                    
                    activity.ActivityPhotos.Add(activityOverView);

                    this.Data.Activity.Update(activity);
                    this.Data.SaveChanges();

                    // Quit when upload succeeded
                    return Request.CreateResponse(HttpStatusCode.OK);
                }

                return Request.CreateResponse(HttpStatusCode.NoContent);
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        [HttpPost]
        [Route("{activityId:int}/Join")]
        public HttpResponseMessage JoinActivity([FromUri] int activityId)
        {
            Guard.ArgumentNotNull(activityId, nameof(activityId));

            var activity = Data.Activity
                .All()
                .FirstOrDefault(t => t.Id == activityId);

            if (activity == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, $"Cannot find activity for activity ID {activityId}");
            }

            var loggedUserId = this.User.Identity.GetUserId();
            var user = this.Data.Users.Find(loggedUserId);

            if (activity.Participents.Contains(user))
            {
                return Request.CreateResponse(HttpStatusCode.Created,
                    $"User {loggedUserId} already joined the activity {activityId}");
            }

            activity.Participents.Add(user);
            this.Data.Activity.Update(activity);
            this.Data.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [HttpDelete]
        [Route("{activityId:int}/Join")]
        public HttpResponseMessage LeaveActivity([FromUri] int activityId)
        {
            Guard.ArgumentNotNull(activityId, nameof(activityId));

            var activity = Data.Activity
                .All()
                .FirstOrDefault(t => t.Id == activityId);

            if (activity == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, $"Cannot find activity for activity ID {activityId}");
            }

            var loggedUserId = this.User.Identity.GetUserId();
            var user = this.Data.Users.Find(loggedUserId);

            if (!activity.Participents.Contains(user))
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest,
                    $"User {loggedUserId} haven't joined the activity {activityId}");
            }

            activity.Participents.Remove(user);
            this.Data.Activity.Update(activity);
            this.Data.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}