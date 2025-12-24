using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Project_65130650.Helpers
{
    public class RoleAuthorizeAttribute : AuthorizeAttribute
    {
        public string RedirectController { get; set; } = "Home";
        public string RedirectAction { get; set; } = "Index";
        public string RedirectArea { get; set; } = "";

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                // Người dùng đã đăng nhập nhưng sai vai trò
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                {
                    controller = RedirectController,
                    action = RedirectAction,
                    area = RedirectArea
                }));
            }
            else
            {
                // Người dùng chưa đăng nhập
                base.HandleUnauthorizedRequest(filterContext);
            }
        }
    }
}
