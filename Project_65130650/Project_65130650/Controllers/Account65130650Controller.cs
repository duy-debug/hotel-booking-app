using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Project_65130650.Models;
using Project_65130650.Models.Forms;

namespace Project_65130650.Controllers
{
    public class Account65130650Controller : Controller
    {
        private Model65130650DbContext db = new Model65130650DbContext();

        // GET: Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // POST: Login (Form thường)
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginForm model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Tìm user trong database
            var user = db.NguoiDungs.FirstOrDefault(u => 
                u.email == model.Email && 
                u.matKhau == model.MatKhau);

            if (user == null)
            {
                ModelState.AddModelError("", "Email hoặc mật khẩu không chính xác");
                return View(model);
            }

            // Kiểm tra tài khoản có bị vô hiệu hóa không
            if (user.trangThaiHoatDong == false)
            {
                ModelState.AddModelError("", "Tài khoản của bạn đã bị vô hiệu hóa. Vui lòng liên hệ admin.");
                return View(model);
            }

            // Tạo authentication ticket
            var ticket = new FormsAuthenticationTicket(
                1,
                user.email,
                DateTime.Now,
                DateTime.Now.AddMinutes(30),
                model.RememberMe,
                user.vaiTro,
                FormsAuthentication.FormsCookiePath
            );

            // Encrypt ticket
            string encryptedTicket = FormsAuthentication.Encrypt(ticket);

            // Create cookie
            var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
            if (model.RememberMe)
            {
                authCookie.Expires = ticket.Expiration;
            }
            Response.Cookies.Add(authCookie);

            // Lưu thông tin vào Session
            Session["UserId"] = user.maNguoiDung;
            Session["UserName"] = user.hoTen;
            Session["UserEmail"] = user.email;
            Session["UserRole"] = user.vaiTro;

            // Redirect theo vai trò
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            if (user.vaiTro == "Quản trị")
            {
                return RedirectToAction("Index", "Home65130650", new { area = "Admin" });
            }
            else
            {
                return RedirectToAction("Index", "Home65130650", new { area = "Customer" });
            }
        }

        // POST: Login AJAX (Cho modal - trả về JSON để hiển thị lỗi trong modal)
        [HttpPost]
        [AllowAnonymous]
        public ActionResult LoginAjax(LoginForm model)
        {
            // Kiểm tra ModelState
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                return Json(new { success = false, errors = errors });
            }

            try
            {
                // Tìm user trong database
                var user = db.NguoiDungs.FirstOrDefault(u =>
                    u.email == model.Email &&
                    u.matKhau == model.MatKhau);

                if (user == null)
                {
                    return Json(new { success = false, errors = new[] { "Email hoặc mật khẩu không chính xác" } });
                }

                // Kiểm tra tài khoản có bị vô hiệu hóa không
                if (user.trangThaiHoatDong == false)
                {
                    return Json(new { success = false, errors = new[] { "Tài khoản của bạn đã bị vô hiệu hóa. Vui lòng liên hệ admin." } });
                }

                // Tạo authentication ticket
                var ticket = new FormsAuthenticationTicket(
                    1,
                    user.email,
                    DateTime.Now,
                    DateTime.Now.AddMinutes(30),
                    model.RememberMe,
                    user.vaiTro,
                    FormsAuthentication.FormsCookiePath
                );

                // Encrypt ticket và tạo cookie
                string encryptedTicket = FormsAuthentication.Encrypt(ticket);
                var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
                if (model.RememberMe)
                {
                    authCookie.Expires = ticket.Expiration;
                }
                Response.Cookies.Add(authCookie);

                // Lưu thông tin vào Session
                Session["UserId"] = user.maNguoiDung;
                Session["UserName"] = user.hoTen;
                Session["UserEmail"] = user.email;
                Session["UserRole"] = user.vaiTro;

                // Xác định redirect URL theo vai trò
                string redirectUrl = user.vaiTro == "Quản trị"
                    ? Url.Action("Index", "Home65130650", new { area = "Admin" })
                    : Url.Action("Index", "Home65130650", new { area = "Customer" });

                return Json(new { success = true, redirectUrl = redirectUrl });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, errors = new[] { "Đã xảy ra lỗi: " + ex.Message } });
            }
        }

        // POST: Register AJAX (Cho modal - trả về JSON)
        [HttpPost]
        [AllowAnonymous]
        public ActionResult RegisterAjax(RegisterForm model)
        {
            // Kiểm tra ModelState
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                return Json(new { success = false, errors = errors });
            }

            // Kiểm tra email đã tồn tại chưa
            if (db.NguoiDungs.Any(u => u.email == model.Email))
            {
                return Json(new { success = false, errors = new[] { "Email này đã được đăng ký" } });
            }

            // Kiểm tra số điện thoại đã tồn tại chưa
            if (db.NguoiDungs.Any(u => u.soDienThoai == model.SoDienThoai))
            {
                return Json(new { success = false, errors = new[] { "Số điện thoại này đã được sử dụng" } });
            }

            try
            {
                // Tạo mã người dùng mới
                string newUserId = GenerateUserId();

                // Tạo đối tượng NguoiDung
                var newUser = new NguoiDung
                {
                    maNguoiDung = newUserId,
                    hoTen = model.HoTen,
                    email = model.Email,
                    soDienThoai = model.SoDienThoai,
                    matKhau = model.MatKhau,
                    vaiTro = "Khách hàng",
                    diaChi = model.DiaChi,
                    ngaySinh = model.NgaySinh,
                    gioiTinh = model.GioiTinh,
                    trangThaiHoatDong = true,
                    ngayTao = DateTime.Now,
                    ngayCapNhat = DateTime.Now
                };

                db.NguoiDungs.Add(newUser);
                db.SaveChanges();

                return Json(new { success = true, message = "Đăng ký thành công! Vui lòng đăng nhập." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, errors = new[] { "Đã xảy ra lỗi khi đăng ký: " + ex.Message } });
            }
        }

        // GET: Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        // POST: Register (Form thường)
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterForm model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Kiểm tra email đã tồn tại chưa
            if (db.NguoiDungs.Any(u => u.email == model.Email))
            {
                ModelState.AddModelError("Email", "Email này đã được đăng ký");
                return View(model);
            }

            // Kiểm tra số điện thoại đã tồn tại chưa
            if (db.NguoiDungs.Any(u => u.soDienThoai == model.SoDienThoai))
            {
                ModelState.AddModelError("SoDienThoai", "Số điện thoại này đã được sử dụng");
                return View(model);
            }

            try
            {
                // Tạo mã người dùng mới
                string newUserId = GenerateUserId();

                // Tạo đối tượng NguoiDung
                var newUser = new NguoiDung
                {
                    maNguoiDung = newUserId,
                    hoTen = model.HoTen,
                    email = model.Email,
                    soDienThoai = model.SoDienThoai,
                    matKhau = model.MatKhau,
                    vaiTro = "Khách hàng",
                    diaChi = model.DiaChi,
                    ngaySinh = model.NgaySinh,
                    gioiTinh = model.GioiTinh,
                    trangThaiHoatDong = true,
                    ngayTao = DateTime.Now,
                    ngayCapNhat = DateTime.Now
                };

                db.NguoiDungs.Add(newUser);
                db.SaveChanges();

                TempData["SuccessMessage"] = "Đăng ký thành công! Vui lòng đăng nhập.";
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Đã xảy ra lỗi khi đăng ký: " + ex.Message);
                return View(model);
            }
        }

        // Logout
        [Authorize]
        public ActionResult Logout()
        {
            // Clear Forms Authentication
            FormsAuthentication.SignOut();

            // Clear Session
            Session.Clear();
            Session.Abandon();

            // Clear authentication cookie
            if (Request.Cookies[FormsAuthentication.FormsCookieName] != null)
            {
                var cookie = new HttpCookie(FormsAuthentication.FormsCookieName)
                {
                    Expires = DateTime.Now.AddDays(-1)
                };
                Response.Cookies.Add(cookie);
            }

            return RedirectToAction("Login");
        }

        // Helper method: Generate User ID
        private string GenerateUserId()
        {
            // Lấy user ID lớn nhất hiện tại
            var lastUser = db.NguoiDungs
                .OrderByDescending(u => u.maNguoiDung)
                .FirstOrDefault();

            if (lastUser == null)
            {
                return "ND001";
            }

            // Parse số từ mã cuối cùng (VD: ND030 => 30)
            string lastId = lastUser.maNguoiDung;
            int number = int.Parse(lastId.Substring(2));
            number++;

            // Tạo mã mới (VD: ND031)
            return "ND" + number.ToString("D3");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}