using System.Web.Mvc;

namespace Project_65130650.Areas.Admin
{
    public class AdminAreaRegistration : AreaRegistration 
    {
        /// <summary>
        /// Tên định danh của Area dành cho Quản trị viên
        /// </summary>
        public override string AreaName 
        {
            get 
            {
                return "Admin";
            }
        }

        /// <summary>
        /// Đăng ký các quy tắc định tuyến (Routing) riêng cho khu vực Admin
        /// </summary>
        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Admin_default", // Tên route
                "Admin/{controller}/{action}/{id}", // Cấu trúc URL (VD: /Admin/Home/Index)
                new { controller = "Home65130650", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}