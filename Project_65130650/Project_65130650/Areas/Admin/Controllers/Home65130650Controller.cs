using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project_65130650.Areas.Admin.Controllers
{
    /// <summary>
    /// Controller cho Admin Area
    /// Chỉ user có vai trò "Quản trị" mới được truy cập
    /// </summary>
    [Authorize(Roles = "Quản trị")]
    public class Home65130650Controller : Controller
    {
        // GET: Admin/Home65130650
        public ActionResult Index()
        {
            ViewBag.UserName = Session["UserName"];
            ViewBag.UserRole = Session["UserRole"];
            ViewBag.UserId = Session["UserId"];
            return View();
        }
    }
}