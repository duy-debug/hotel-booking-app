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
        } // Corrected closing brace for Application_AuthenticateRequest

        /// <summary>
        /// Chặn và xóa ReturnUrl khỏi Redirect URL trước khi gửi về client
        /// </summary>
        protected void Application_EndRequest(object sender, EventArgs e)
        {
            if (Response.StatusCode == 302)
            {
                var returnUrl = Request.QueryString["ReturnUrl"];
                if (!string.IsNullOrEmpty(returnUrl) || Response.RedirectLocation.Contains("ReturnUrl="))
                {
                    // Lấy địa chỉ redirect hiện tại
                    string redirectUrl = Response.RedirectLocation;

                    // Nếu địa chỉ redirect chứa ReturnUrl, ta cắt bỏ nó
                    if (redirectUrl.Contains("ReturnUrl="))
                    {
                        int index = redirectUrl.IndexOf("ReturnUrl=");
                        // Cắt lấy phần trước ReturnUrl (thường là dấu ? hoặc &)
                        string newUrl = redirectUrl.Substring(0, index).TrimEnd('?', '&');
                        
                        // Nếu sau khi cắt mà URL trống (do loginUrl="~/"), set về root
                        if (string.IsNullOrEmpty(newUrl) || newUrl == "~/" || newUrl == "/") {
                            newUrl = VirtualPathUtility.ToAbsolute("~/");
                        }
                        
                        Response.RedirectLocation = newUrl;
                    }
                }
            }
        }
    }
}
