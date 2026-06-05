using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace web_luat.Models
{
    public class AdminAuthorize : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var controller = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            var action = filterContext.ActionDescriptor.ActionName;

            var isLogin = (HttpContext.Current.Session["UserId"] != null);

            // ❌ chưa login
            if (!isLogin)
            {
                // nếu không phải trang Login → đá về Login
                if (!(controller == "QuanTri" && action == "Login"))
                {
                    filterContext.Result = new RedirectToRouteResult(
                        new System.Web.Routing.RouteValueDictionary {
                    { "controller", "QuanTri" },
                    { "action", "Login" },
                    { "area", "Admin" }
                        });
                }
            }
            else
            {
                // ✅ đã login mà vẫn cố vào Login → đá về Index
                if (controller == "QuanTri" && action == "Login")
                {
                    filterContext.Result = new RedirectToRouteResult(
                        new System.Web.Routing.RouteValueDictionary {
                    { "controller", "QuanTri" },
                    { "action", "Index" },
                    { "area", "Admin" }
                        });
                }
            }
            var session = HttpContext.Current.Session;

            if (session["UserId"] == null)
            {
                RedirectLogin(filterContext);
                return;
            }


            base.OnActionExecuting(filterContext);
        }
        private void RedirectLogin(ActionExecutingContext filterContext)
        {
            filterContext.Result = new RedirectToRouteResult(
                new System.Web.Routing.RouteValueDictionary {
            { "controller", "QuanTri" },
            { "action", "Login" },
            { "area", "Admin" }
                });
        }
    }
}