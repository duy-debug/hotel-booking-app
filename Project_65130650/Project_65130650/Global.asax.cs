using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;

namespace Project_65130650
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        /// <summary>
        /// Xử lý authentication ticket và thiết lập Principal với Role
        /// </summary>
        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            if (HttpContext.Current.User != null && HttpContext.Current.User.Identity.IsAuthenticated)
            {
                if (HttpContext.Current.User.Identity is FormsIdentity formsIdentity)
                {
                    FormsAuthenticationTicket ticket = formsIdentity.Ticket;

                    // Lấy role từ UserData của ticket (đã lưu khi login)
                    string role = ticket.UserData;

                    // Tạo GenericPrincipal với role
                    string[] roles = string.IsNullOrEmpty(role) ? new string[0] : new string[] { role };
                    GenericPrincipal principal = new GenericPrincipal(formsIdentity, roles);

                    // Gán principal cho context
                    HttpContext.Current.User = principal;
                }
            }
        }
    }
}
