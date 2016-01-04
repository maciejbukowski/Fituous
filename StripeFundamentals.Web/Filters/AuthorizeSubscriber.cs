using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
 

namespace StripeFundamentals.Web.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public sealed class AuthorizeSubscriber : FilterAttribute, IAuthorizationFilter
    {

        public void OnAuthorization(AuthorizationContext filterContext)
        {
            var userManager = filterContext.HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var user = userManager.FindByName(filterContext.HttpContext.User.Identity.Name);

            if (user == null || DateTime.Now > user.ActiveUntil)
            {
                filterContext.Result = new HttpUnauthorizedResult();
            }
        }
    }
}