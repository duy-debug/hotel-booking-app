using System.Web.Mvc;

namespace Project_65130650.Areas.Customer
{
    public class CustomerAreaRegistration : AreaRegistration 
    {
        /// <summary>
        /// Tên định danh của Area dành cho Khách hàng
        /// </summary>
        public override string AreaName 
        {
            get 
            {
                return "Customer";
            }
        }

        /// <summary>
        /// Đăng ký các quy tắc định tuyến (Routing) riêng cho khu vực Customer
        /// </summary>
        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Customer_default", // Tên route
                "Customer/{controller}/{action}/{id}", // Cấu trúc URL (VD: /Customer/Home/Index)
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}