using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project_65130650.Areas.Customer.Controllers
{
    /// <summary>
    /// Controller cho Customer Area
    /// Chỉ user có vai trò "Khách hàng" mới được truy cập
    /// </summary>
    [Authorize(Roles = "Khách hàng")]
    public class Home65130650Controller : Controller
    {
        // GET: Customer/Home65130650
        public ActionResult Index()
        {
            ViewBag.UserName = Session["UserName"];
            ViewBag.UserRole = Session["UserRole"];
            ViewBag.UserId = Session["UserId"];
            return View();
        }
    }
}