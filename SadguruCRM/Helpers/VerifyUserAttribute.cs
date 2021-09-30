using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SadguruCRM.Helpers
{
    public class VerifyUserAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var user = filterContext.HttpContext.Session["UserID"];
            if (user == null)
                filterContext.Result = new RedirectResult(string.Format("/Login?targetUrl={0}", filterContext.HttpContext.Request.Url.AbsolutePath));
        }
    }
}