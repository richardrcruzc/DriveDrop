﻿//using Hangfire.Annotations;
//using Hangfire.Dashboard;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace DriveDrop.Api.Filters
//{
//    public class CustomAuthorizeFilter : IDashboardAuthorizationFilter
//    {
//        public bool Authorize([NotNull] DashboardContext context)
//        {
//            var httpcontext = context.GetHttpContext();
//            return httpcontext.User.Identity.IsAuthenticated;
//        }
//    }
//}
