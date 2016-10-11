﻿namespace Twitter.App
{
    using System.Web.Mvc;

    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            
            // TODO: apply HTTPS strictly
            //filters.Add(new RequireHttpsAttribute());
        }
    }
}