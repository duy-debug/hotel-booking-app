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
            if (!ModelState.IsValid) return View(model);

            var user = GetAuthenticatedUser(model.Email, model.MatKhau);
            if (user == null)
            {
                ModelState.AddModelError("", "Email hoặc mật khẩu không chính xác");
                return View(model);
            }

            if (user.trangThaiHoatDong == false)
            {
                ModelState.AddModelError("", "Tài khoản của bạn đã bị vô hiệu hóa.");
                return View(model);
            }

            // Thực hiện đăng nhập
            SignInUser(user, model.RememberMe);

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToDefaultPage(user.vaiTro);
        }

        // POST: Login AJAX (Cho modal)
        [HttpPost]
        [AllowAnonymous]
        public ActionResult LoginAjax(LoginForm model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return Json(new { success = false, errors = errors });
            }

            var user = GetAuthenticatedUser(model.Email, model.MatKhau);
            if (user == null)
                return Json(new { success = false, errors = new[] { "Email hoặc mật khẩu không chính xác" } });

            if (user.trangThaiHoatDong == false)
                return Json(new { success = false, errors = new[] { "Tài khoản bị vô hiệu hóa." } });

            // Thực hiện đăng nhập
            SignInUser(user, model.RememberMe);

            string redirectUrl = user.vaiTro == "Quản trị"
                ? Url.Action("Index", "Home65130650", new { area = "Admin" })
                : Url.Action("Index", "Home65130650", new { area = "Customer" });

            return Json(new { success = true, redirectUrl = redirectUrl });
        }

        // --- HÀM DÙNG CHUNG ĐỂ TỐI ƯU CODE ---

        private NguoiDung GetAuthenticatedUser(string email, string password)
        {
            return db.NguoiDungs.FirstOrDefault(u => u.email == email && u.matKhau == password);
        }

        private void SignInUser(NguoiDung user, bool rememberMe)
        {
            // 1. Tạo Ticket (Thời gian bạn có thể chỉnh ở đây cho đồng nhất)
            var ticket = new FormsAuthenticationTicket(
                1,
                user.email,
                DateTime.Now,
                DateTime.Now.AddMinutes(30), // Cố định 30 phút ở cả 2 nơi
                rememberMe,
                user.vaiTro,
                FormsAuthentication.FormsCookiePath
            );

            // 2. Mã hóa và tạo Cookie
            string encryptedTicket = FormsAuthentication.Encrypt(ticket);
            var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
            if (rememberMe) authCookie.Expires = ticket.Expiration;
            Response.Cookies.Add(authCookie);

            // 3. Lưu Session
            Session["UserId"] = user.maNguoiDung;
            Session["UserName"] = user.hoTen;
            Session["UserEmail"] = user.email;
            Session["UserRole"] = user.vaiTro;
        }

        private ActionResult RedirectToDefaultPage(string role)
        {
            if (role == "Quản trị")
                return RedirectToAction("Index", "Home65130650", new { area = "Admin" });
            
            return RedirectToAction("Index", "Home65130650", new { area = "Customer" });
        }

        // POST: Register AJAX (Cho modal)
        [HttpPost]
        [AllowAnonymous]
        public ActionResult RegisterAjax(RegisterForm model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return Json(new { success = false, errors = errors });
            }

            string errorMessage;
            if (!IsRegistrationValid(model, out errorMessage))
                return Json(new { success = false, errors = new[] { errorMessage } });

            try
            {
                CreateNewUser(model);
                return Json(new { success = true, message = "Đăng ký thành công! Vui lòng đăng nhập." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, errors = new[] { "Lỗi: " + ex.Message } });
            }
        }

        // POST: Register (Form thường)
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterForm model)
        {
            if (!ModelState.IsValid) return View(model);

            string errorMessage;
            if (!IsRegistrationValid(model, out errorMessage))
            {
                ModelState.AddModelError("", errorMessage);
                return View(model);
            }

            try
            {
                CreateNewUser(model);
                TempData["SuccessMessage"] = "Đăng ký thành công! Vui lòng đăng nhập.";
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Lỗi: " + ex.Message);
                return View(model);
            }
        }

        // --- HÀM DÙNG CHUNG CHO ĐĂNG KÝ ---

        private bool IsRegistrationValid(RegisterForm model, out string errorMessage)
        {
            errorMessage = "";
            if (db.NguoiDungs.Any(u => u.email == model.Email))
            {
                errorMessage = "Email này đã được đăng ký";
                return false;
            }
            if (db.NguoiDungs.Any(u => u.soDienThoai == model.SoDienThoai))
            {
                errorMessage = "Số điện thoại này đã được sử dụng";
                return false;
            }
            return true;
        }

        private void CreateNewUser(RegisterForm model)
        {
            var newUser = new NguoiDung
            {
                maNguoiDung = GenerateUserId(),
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

            return RedirectToAction("Index", "Home");
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