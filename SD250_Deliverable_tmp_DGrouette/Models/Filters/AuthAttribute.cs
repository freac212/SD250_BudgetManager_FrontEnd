using System;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Web;

using System.Web.Mvc;
using System.Web.Routing;

namespace SD220_Deliverable_1_DGrouette.Models.Filters
{
    internal class AuthAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            var token = filterContext.RequestContext.HttpContext.Request.Cookies["UserAuthCookie"];

            if (token is null)
            {
                //filterContext.Controller.TempData["ErrorMessage"] = "That data either doesn't exist or you don't have access to it.";
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary
                    {
                        { "controller", "Account" },
                        { "action", "LogIn" }
                    });
            }
        }
    }
}
