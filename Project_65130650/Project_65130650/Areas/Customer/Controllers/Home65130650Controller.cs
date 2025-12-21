using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project_65130650.Areas.Customer.Controllers
{
    /// <summary>
    /// Controller chính cho khu vực Khách hàng (Customer Area)
    /// Sử dụng Attribute [Authorize] để đảm bảo chỉ những người dùng có vai trò "Khách hàng" mới có thể truy cập
    /// </summary>
    [Authorize(Roles = "Khách hàng")]
    public class Home65130650Controller : Controller
    {
        /// <summary>
        /// GET: Customer/Home65130650/Index
        /// Trang dashboard hoặc trang cá nhân dành riêng cho khách hàng sau khi đăng nhập thành công
        /// </summary>
        public ActionResult Index()
        {
            // Lấy thông tin từ Session để hiển thị lời chào hoặc thông tin cá nhân trên giao diện
            ViewBag.UserName = Session["UserName"];
            ViewBag.UserRole = Session["UserRole"];
            ViewBag.UserId = Session["UserId"];
            
            return View();
        }
    }
}