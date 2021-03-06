﻿using Twitter.App.Ambient;

namespace Twitter.App.Controllers
{
    using System.Linq;
    using System.Web.Mvc;

    using Twitter.App.Models.ViewModels;
    using Twitter.Data.UnitOfWork;

    using PagedList;

    [RoutePrefix("users")]
    public class UsersController : TwitterBaseController
    {
        public UsersController()
            : base(new QQmgsData())
        {
        }

        //[Route("search")]
        //public ActionResult SearchUser(string searchTerm, int p = 1)
        //{
        //    //ViewBag.searchTerm = searchTerm;

        //    //if (string.IsNullOrEmpty(searchTerm))
        //    //{
        //    //    return this.View();
        //    //}

        //    //var users =
        //    //    this.Data.Users.All()
        //    //        .Where(u => u.UserName.Contains(searchTerm))
        //    //        .OrderByDescending(u => u.UserName)
        //    //        .Select(u => new UserViewModel { Username = u.UserName, Email = u.Email });

        //    //var pagedUsers = users.ToPagedList(pageNumber: p, pageSize: 6);

        //    return !users.Any() ? this.View() : this.View(pagedUsers);
        //}
    }
}