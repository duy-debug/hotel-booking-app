using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Project_65130650.Models;
using Project_65130650.Models.Forms;
using Project_65130650.Helpers;
using System.Threading.Tasks;

namespace Project_65130650.Controllers
{
    public class Account65130650Controller : Controller
    {
        // Khởi tạo context database của dự án
        private Model65130650DbContext db = new Model65130650DbContext();

        /// <summary>
        /// GET: Login - Hiển thị trang đăng nhập
        /// </summary>
        /// <param name="returnUrl">Đường dẫn người dùng muốn truy cập trước khi bị chuyển hướng đăng nhập</param>
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            // Nếu người dùng đã đăng nhập rồi thì chuyển về trang chủ
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        /// <summary>
        /// POST: Login - Xử lý đăng nhập từ Form thường
        /// </summary>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginForm model, string returnUrl)
        {
            // Kiểm tra dữ liệu đầu vào theo định nghĩa trong LoginForm
            if (!ModelState.IsValid) return View(model);

            // Kiểm tra thông tin người dùng trong cơ sở dữ liệu
            var user = GetAuthenticatedUser(model.Email, model.MatKhau);
            if (user == null)
            {
                ModelState.AddModelError("", "Email hoặc mật khẩu không chính xác");
                return View(model);
            }

            // Kiểm tra xem tài khoản có đang bị khóa (vô hiệu hóa) không
            if (user.trangThaiHoatDong == false)
            {
                ModelState.AddModelError("", "Tài khoản của bạn đã bị vô hiệu hóa.");
                return View(model);
            }

            // Thực hiện các bước login (tạo cookie, session)
            SignInUser(user, model.RememberMe);

            // Chuyển hướng về trang trước đó hoặc trang mặc định theo vai trò
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToDefaultPage(user.vaiTro);
        }

        /// <summary>
        /// POST: Login AJAX - Xử lý đăng nhập thông qua AJAX (thường dùng cho các Modal popup)
        /// </summary>
        [HttpPost]
        [AllowAnonymous]
        public ActionResult LoginAjax(LoginForm model)
        {
            if (!ModelState.IsValid)
            {
                // Lấy danh sách các lỗi validation gửi về client
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return Json(new { success = false, errors = errors });
            }

            var user = GetAuthenticatedUser(model.Email, model.MatKhau);
            if (user == null)
                return Json(new { success = false, errors = new[] { "Email hoặc mật khẩu không chính xác" } });

            if (user.trangThaiHoatDong == false)
                return Json(new { success = false, errors = new[] { "Tài khoản bị vô hiệu hóa." } });

            // Đăng nhập thành công
            SignInUser(user, model.RememberMe);

            // Xác định URL chuyển hướng dựa trên vai trò người dùng
            string redirectUrl = user.vaiTro == "Quản trị"
                ? Url.Action("Index", "Home65130650", new { area = "Admin" })
                : Url.Action("Index", "Home65130650", new { area = "Customer" });

            return Json(new { success = true, redirectUrl = redirectUrl });
        }

        // --- HÀM DÙNG CHUNG ĐỂ TỐI ƯU CODE ---

        /// <summary>
        /// Truy vấn thông tin người dùng từ Email và Mật khẩu
        /// </summary>
        private NguoiDung GetAuthenticatedUser(string email, string password)
        {
            return db.NguoiDungs.FirstOrDefault(u => u.email == email && u.matKhau == password);
        }

        /// <summary>
        /// Thực hiện lưu thông tin đăng nhập vào Cookie và Session
        /// </summary>
        private void SignInUser(NguoiDung user, bool rememberMe)
        {
            // 1. Tạo Authentication Ticket với thông tin vai trò (role)
            var ticket = new FormsAuthenticationTicket(
                1,
                user.email,
                DateTime.Now,
                DateTime.Now.AddMinutes(30), // Thời gian hết hạn login
                rememberMe,
                user.vaiTro, // Lưu vai trò vào userdata của ticket
                FormsAuthentication.FormsCookiePath
            );

            // 2. Mã hóa ticket và lưu vào Cookie của trình duyệt
            string encryptedTicket = FormsAuthentication.Encrypt(ticket);
            var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
            if (rememberMe) authCookie.Expires = ticket.Expiration;
            Response.Cookies.Add(authCookie);

            // 3. Lưu các thông tin cần thiết vào Session để truy xuất nhanh
            Session["UserId"] = user.maNguoiDung;
            Session["UserName"] = user.hoTen;
            Session["UserEmail"] = user.email;
            Session["UserRole"] = user.vaiTro;
        }

        /// <summary>
        /// Chuyển hướng người dùng về trang chủ tương ứng với vai trò của họ
        /// </summary>
        private ActionResult RedirectToDefaultPage(string role)
        {
            if (role == "Quản trị")
                return RedirectToAction("Index", "Home65130650", new { area = "Admin" });
            
            return RedirectToAction("Index", "Home65130650", new { area = "Customer" });
        }

        /// <summary>
        /// POST: Register AJAX - Xử lý đăng ký tài khoản qua AJAX
        /// </summary>
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

        /// <summary>
        /// POST: Register - Xử lý đăng ký tài khoản từ Form thường
        /// </summary>
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

        /// <summary>
        /// Kiểm tra xem thông tin đăng ký có hợp lệ và duy nhất không (Email, SĐT)
        /// </summary>
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

        /// <summary>
        /// Lưu người dùng mới vào database
        /// </summary>
        private void CreateNewUser(RegisterForm model)
        {
            var newUser = new NguoiDung
            {
                maNguoiDung = GenerateUserId(),
                hoTen = model.HoTen,
                email = model.Email,
                soDienThoai = model.SoDienThoai,
                matKhau = model.MatKhau, // Lưu ý: Trong thực tế nên hash mật khẩu
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

        /// <summary>
        /// GET: Logout - Đăng xuất người dùng khỏi hệ thống
        /// </summary>
        [Authorize]
        public ActionResult Logout()
        {
            // Xóa Authentication Ticket
            FormsAuthentication.SignOut();

            // Xóa bỏ tất cả dữ liệu Session
            Session.Clear();
            Session.Abandon();

            // Xóa cookie liên quan đến đăng nhập
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

        /// <summary>
        /// Tự động sinh mã người dùng mới dựa trên mã lớn nhất hiện có (VD: ND001 -> ND002)
        /// </summary>
        private string GenerateUserId()
        {
            var lastUser = db.NguoiDungs.OrderByDescending(u => u.maNguoiDung).FirstOrDefault();
            if (lastUser == null) return "ND001";
            int lastNum = int.Parse(lastUser.maNguoiDung.Substring(2));
            return "ND" + (lastNum + 1).ToString("D3");
        }

        /// <summary>
        /// GET: ForgotPassword - Hiển thị giao diện nhập email khôi phục mật khẩu
        /// </summary>
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        /// <summary>
        /// POST: ForgotPassword - Xử lý gửi mã xác nhận qua email
        /// </summary>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordForm model)
        {
            if (ModelState.IsValid)
            {
                // Tìm người dùng theo email nhập vào
                var user = db.NguoiDungs.FirstOrDefault(u => u.email == model.Email);
                if (user != null)
                {
                    // Tạo mã xác nhận ngẫu nhiên gồm 6 chữ số
                    string verificationCode = new Random().Next(100000, 999999).ToString();
                    
                    // Lưu mã xác nhận và thời gian hết hạn vào Session
                    Session["ResetEmail"] = model.Email;
                    Session["ResetCode"] = verificationCode;
                    Session["ResetCodeExpiry"] = DateTime.Now.AddMinutes(5); // Hết hạn sau 5 phút

                    // Soạn nội dung email (Sử dụng HTML để định dạng đẹp)
                    string subject = "Mã xác nhận khôi phục mật khẩu";
                    string body = $@"
                        <div style='font-family: Arial, sans-serif; padding: 20px; border: 1px solid #ddd; border-radius: 8px;'>
                            <h2 style='color: #0d4ea6;'>Khôi phục mật khẩu</h2>
                            <p>Chào <b>{user.hoTen}</b>,</p>
                            <p>Bạn đã yêu cầu khôi phục mật khẩu. Dưới đây là mã xác nhận của bạn:</p>
                            <div style='background-color: #f4f4f4; padding: 15px; text-align: center; font-size: 24px; font-weight: bold; letter-spacing: 5px; color: #d9534f; border-radius: 5px;'>
                                {verificationCode}
                            </div>
                            <p style='margin-top: 20px;'>Mã này sẽ hết hạn sau 5 phút.</p>
                            <p>Nếu bạn không yêu cầu thay đổi này, vui lòng bỏ qua email này.</p>
                            <hr style='border: 0; border-top: 1px solid #eee; margin: 20px 0;'>
                            <p style='font-size: 12px; color: #777;'>Đây là email tự động, vui lòng không phản hồi.</p>
                        </div>";

                    try
                    {
                        // Gửi email bất đồng bộ thông qua EmailHelper
                        await EmailHelper.SendEmailAsync(model.Email, subject, body);
                        TempData["SuccessMessage"] = "Mã xác nhận đã được gửi đến email của bạn.";
                        return RedirectToAction("ResetPassword", new { email = model.Email });
                    }
                    catch (Exception ex)
                    {
                        // Hiển thị thông báo lỗi chi tiết nếu gửi mail không thành công
                        ModelState.AddModelError("", "Lỗi gửi mail: " + (ex.InnerException?.Message ?? ex.Message));
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Email không tồn tại trong hệ thống.");
                }
            }
            return View(model);
        }

        /// <summary>
        /// GET: ResetPassword - Hiển thị giao diện nhập mã xác nhận và mật khẩu mới
        /// </summary>
        [AllowAnonymous]
        public ActionResult ResetPassword(string email)
        {
            if (string.IsNullOrEmpty(email)) return RedirectToAction("ForgotPassword");
            return View(new ResetPasswordForm { Email = email });
        }

        /// <summary>
        /// POST: ResetPassword - Xác thực mã và cập nhật mật khẩu mới vào database
        /// </summary>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(ResetPasswordForm model)
        {
            if (ModelState.IsValid)
            {
                // Lấy thông tin mã xác nhận đã lưu trong Session
                string sessionEmail = Session["ResetEmail"] as string;
                string sessionCode = Session["ResetCode"] as string;
                DateTime? expiry = Session["ResetCodeExpiry"] as DateTime?;

                // Kiểm tra mã xác nhận có trùng khớp và còn hiệu lực không
                if (sessionEmail == model.Email && sessionCode == model.VerificationCode && expiry > DateTime.Now)
                {
                    var user = db.NguoiDungs.FirstOrDefault(u => u.email == model.Email);
                    if (user != null)
                    {
                        // Cập nhật mật khẩu mới
                        user.matKhau = model.NewPassword; 
                        user.ngayCapNhat = DateTime.Now;
                        db.SaveChanges();

                        // Xóa dữ liệu khôi phục mật khẩu trong Session sau khi thành công
                        Session.Remove("ResetEmail");
                        Session.Remove("ResetCode");
                        Session.Remove("ResetCodeExpiry");

                        // Chuyển hướng về login và báo thành công
                        TempData["PasswordResetSuccess"] = "Mật khẩu đã được thay đổi thành công. Vui lòng đăng nhập lại.";
                        return RedirectToAction("Login");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Mã xác nhận không đúng hoặc đã hết hạn.");
                }
            }
            return View(model);
        }

        /// <summary>
        /// Giải phóng tài nguyên database khi không sử dụng
        /// </summary>
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