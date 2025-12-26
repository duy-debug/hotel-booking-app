using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Project_65130650.Helpers;
using Project_65130650.Models;

namespace Project_65130650.Areas.Customer.Controllers
{
    /// <summary>
    /// Controller chính cho khu vực Khách hàng (Customer Area)
    /// Sử dụng Attribute [Authorize] để đảm bảo chỉ những người dùng có vai trò "Khách hàng" mới có thể truy cập
    /// </summary>
    [RoleAuthorize(Roles = "Khách hàng")]
    public class Home65130650Controller : Controller
    {
        private Model65130650DbContext db = new Model65130650DbContext();

        /// <summary>
        /// GET: Customer/Home65130650/Index
        /// Trang dashboard hoặc trang cá nhân dành riêng cho khách hàng sau khi đăng nhập thành công
        /// </summary>
        public ActionResult Index()
        {
            // Lấy thông tin từ Session để hiển thị lời chào hoặc thông tin cá nhân trên giao diện
            var userId = Session["UserId"] as string;
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account65130650", new { area = "" });
            }

            var user = db.NguoiDungs.Find(userId);
            if (user == null)
            {
                return HttpNotFound();
            }

            ViewBag.UserName = user.hoTen;
            ViewBag.UserRole = user.vaiTro;
            ViewBag.UserId = userId;
            
            return View(user);
        }

        /// <summary>
        /// GET: Customer/Home65130650/AccountManagement
        /// Quản lý tài khoản cá nhân: Cập nhật thông tin, đổi mật khẩu
        /// </summary>
        /// <summary>
        /// GET: Customer/Home65130650/AccountManagement
        /// Quản lý tài khoản cá nhân: Cập nhật thông tin
        /// </summary>
        public ActionResult AccountManagement()
        {
            var userId = Session["UserId"] as string;
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account65130650", new { area = "" });
            }

            var user = db.NguoiDungs.Find(userId);
            if (user == null)
            {
                return HttpNotFound();
            }

            return View(user);
        }

        /// <summary>
        /// POST: Customer/Home65130650/AccountManagement
        /// Xử lý cập nhật thông tin cá nhân
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AccountManagement(NguoiDung model, string NewPassword, string ConfirmPassword)
        {
            var userId = Session["UserId"] as string;
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account65130650", new { area = "" });
            }

            var user = db.NguoiDungs.Find(userId);
            if (user == null)
            {
                return HttpNotFound();
            }

            try 
            {
                // Cập nhật thông tin cơ bản (Không cập nhật Email và Vai trò để bảo mật)
                user.hoTen = model.hoTen;
                user.soDienThoai = model.soDienThoai;
                user.diaChi = model.diaChi;
                user.ngaySinh = model.ngaySinh;
                user.gioiTinh = model.gioiTinh;
                user.ngayCapNhat = DateTime.Now;

                // Xử lý đổi mật khẩu nếu người dùng nhập mật khẩu mới
                if (!string.IsNullOrEmpty(NewPassword))
                {
                    // Regex: Tối thiểu 8 ký tự, ít nhất 1 chữ hoa, 1 chữ thường, 1 số và 1 ký tự đặc biệt
                    var passwordRegex = new System.Text.RegularExpressions.Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$");
                    
                    if (!passwordRegex.IsMatch(NewPassword))
                    {
                        TempData["ErrorMessage"] = "Mật khẩu bảo mật phải có: ít nhất 8 ký tự, 1 chữ hoa, 1 chữ thường, 1 số và 1 ký tự đặc biệt";
                        return View(user);
                    }

                    if (NewPassword != ConfirmPassword)
                    {
                         TempData["ErrorMessage"] = "Mật khẩu xác nhận không khớp.";
                         return View(user);
                    }
                    // Cập nhật mật khẩu mới
                    user.matKhau = NewPassword; 
                }

                // Lưu thay đổi vào Database
                db.Entry(user).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                // Cập nhật lại session tên người dùng
                Session["UserName"] = user.hoTen;

                TempData["SuccessMessage"] = "Chúc mừng! Thông tin của bạn đã được cập nhật thành công.";
                return RedirectToAction("AccountManagement");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Lỗi hệ thống: " + ex.Message;
            }

            return View(user);
        }

        /// <summary>
        /// GET: Customer/Home65130650/MyBookings
        /// Quản lý đơn đặt phòng của tôi: Xem lịch sử, trạng thái đơn hàng
        /// </summary>
        public ActionResult MyBookings()
        {
            ViewBag.UserName = Session["UserName"];
            ViewBag.UserId = Session["UserId"];
            // TODO: Truy vấn danh sách booking từ DB theo UserId
            return View();
        }

        /// <summary>
        /// GET: Customer/Home65130650/Payment
        /// Trang thanh toán: Quản lý phương thức thanh toán, lịch sử giao dịch
        /// </summary>
        public ActionResult Payment(int? page)
        {
            var userId = Session["UserId"] as string;
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account65130650", new { area = "" });
            }

            // Cấu hình phân trang
            int pageSize = 5;
            int pageNumber = page ?? 1;

            // Lấy danh sách lịch sử thanh toán của khách hàng
            var allPayments = db.ThanhToans
                .Where(t => t.DatPhong.maKhachHang == userId)
                .OrderByDescending(t => t.ngayThanhToan);

            int totalItems = allPayments.Count();
            var paymentHistory = allPayments
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Tính toán thống kê chi tiêu (tổng hợp trên toàn bộ dữ liệu, không chỉ trang hiện tại)
            ViewBag.TotalSpent = allPayments
                .Where(p => p.trangThaiThanhToan != null && 
                           (p.trangThaiThanhToan.ToLower().Contains("thành công") || 
                            p.trangThaiThanhToan.ToLower().Contains("đã thanh toán")))
                .Sum(p => (decimal?)p.soTien) ?? 0;
            
            ViewBag.TransactionCount = totalItems;
            ViewBag.UserName = Session["UserName"];
            
            // Thông tin phân trang
            ViewBag.CurrentPage = pageNumber;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            ViewBag.PageSize = pageSize;

            return View(paymentHistory);
        }

        /// <summary>
        /// GET: Customer/Home65130650/Support
        /// Liên hệ hỗ trợ: Gửi yêu cầu hỗ trợ, FAQ
        /// </summary>
        public ActionResult Support()
        {
            ViewBag.UserName = Session["UserName"];
            return View();
        }
    }
}