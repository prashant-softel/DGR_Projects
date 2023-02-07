using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using System.Linq;

namespace DGRA_V1.Filters
{
    public class SessionValidation : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
            var Session_userid = context.HttpContext.Session.Keys.ToList();
            var ControllerName = context.Controller;
            var ActionName = context.ActionDescriptor;

             string Controller = (string)context.RouteData.Values["Controller"];
             string Action = (string)context.RouteData.Values["Action"];
            string ControllerAction = Controller + Action;
            if (Session_userid.Count == 0 && ControllerAction != "HomeIndex")
            {
                //context.Result = new RedirectToRouteResult("SystemLogin", routeValues);
                
                context.Result = new RedirectToRouteResult(
                   new RouteValueDictionary(new { controller = "Home", action = "Index", Area="" })
                    );
                //context.Result = new RedirectResult("Index","Home", new {Area ="" });

            }

        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            //var Session_userid = context.HttpContext.Session.Keys.ToList();
            //if (Session_userid.Count == 0)
            //{
            //    //context.Result = new RedirectToRouteResult("SystemLogin", routeValues);

            //    context.Result = new RedirectToRouteResult(
            //       new RouteValueDictionary(new { controller = "Home", action = "Index" })
            //        );

            //}
        }
    }
}
