using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Project_65130650
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            // Account Routes (Clean URLs)
            routes.MapRoute(
                name: "Login",
                url: "Login",
                defaults: new { controller = "Account65130650", action = "Login" }
            );

            routes.MapRoute(
                name: "Register",
                url: "Register",
                defaults: new { controller = "Account65130650", action = "Register" }
            );

            routes.MapRoute(
                name: "Logout",
                url: "Logout",
                defaults: new { controller = "Account65130650", action = "Logout" }
            );

            // Ensure Root URL leads to Home/Index
            routes.MapRoute(
                name: "Home",
                url: "",
                defaults: new { controller = "Home", action = "Index" }
            );

            // Default Route
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
