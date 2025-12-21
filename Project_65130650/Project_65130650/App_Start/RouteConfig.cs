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
        /// <summary>
        /// Đăng ký và cấu hình các đường dẫn (URL) cho toàn bộ ứng dụng web
        /// </summary>
        public static void RegisterRoutes(RouteCollection routes)
        {
            // Bỏ qua các tài nguyên tĩnh như file log, cấu hình axd
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            /** 
             * CẤU HÌNH URL ĐẸP CHO CÁC CHỨC NĂNG TÀI KHOẢN
             * Giúp URL trông chuyên nghiệp hơn (VD: /Login thay vì /Account65130650/Login)
             */

            // Đường dẫn Đăng nhập
            routes.MapRoute(
                name: "Login",
                url: "Login",
                defaults: new { controller = "Account65130650", action = "Login" }
            );

            // Đường dẫn Đăng ký
            routes.MapRoute(
                name: "Register",
                url: "Register",
                defaults: new { controller = "Account65130650", action = "Register" }
            );

            // Đường dẫn Quên mật khẩu
            routes.MapRoute(
                name: "ForgotPassword",
                url: "ForgotPassword",
                defaults: new { controller = "Account65130650", action = "ForgotPassword" }
            );

            // Đường dẫn Đặt lại mật khẩu (Dùng mã xác nhận)
            routes.MapRoute(
                name: "ResetPassword",
                url: "ResetPassword",
                defaults: new { controller = "Account65130650", action = "ResetPassword" }
            );

            // Đường dẫn Đăng xuất
            routes.MapRoute(
                name: "Logout",
                url: "Logout",
                defaults: new { controller = "Account65130650", action = "Logout" }
            );

            /**
             * CẤU HÌNH TRANG CHỦ MẶC ĐỊNH
             * Đảm bảo khi truy cập vào địa chỉ gốc (/) thì trang chủ sẽ hiện ra
             */
            routes.MapRoute(
                name: "Home",
                url: "",
                defaults: new { controller = "Home", action = "Index" }
            );

            /**
             * CẤU HÌNH ĐƯỜNG DẪN MẶC ĐỊNH (DEFAULT ROUTE)
             * Xử lý các trường hợp URL theo cấu trúc chuẩn {controller}/{action}/{id}
             */
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
