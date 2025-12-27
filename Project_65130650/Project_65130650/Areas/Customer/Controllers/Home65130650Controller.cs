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
                    // Cập nhật mật khẩu mới (băm SHA256 trước khi lưu)
                    user.matKhau = PasswordHelper.HashPassword(NewPassword); 
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
        /// <param name="page">Số trang hiện tại</param>
        /// <param name="status">Lọc theo trạng thái đơn đặt phòng</param>
        public ActionResult MyBookings(int? page, string status)
        {
            var userId = Session["UserId"] as string;
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account65130650", new { area = "" });
            }

            // Cấu hình phân trang
            int pageSize = 5;
            int pageNumber = page ?? 1;

            // Lấy tất cả đơn đặt phòng của khách hàng
            var allBookings = db.DatPhongs
                .Where(b => b.maKhachHang == userId)
                .OrderByDescending(b => b.ngayDat);

            // Thống kê theo trạng thái (dùng toàn bộ dữ liệu)
            var allBookingsList = allBookings.ToList();
            ViewBag.TotalBookings = allBookingsList.Count;
            ViewBag.PendingCount = allBookingsList.Count(b => b.trangThaiDatPhong == "Chờ xác nhận");
            ViewBag.ConfirmedCount = allBookingsList.Count(b => b.trangThaiDatPhong == "Đã xác nhận");
            ViewBag.CheckedInCount = allBookingsList.Count(b => b.trangThaiDatPhong == "Đã nhận phòng");
            ViewBag.CheckedOutCount = allBookingsList.Count(b => b.trangThaiDatPhong == "Đã trả phòng");
            ViewBag.CancelledCount = allBookingsList.Count(b => b.trangThaiDatPhong == "Đã hủy");

            // Tính tổng tiền đã chi tiêu (các đơn đã hoàn thành)
            ViewBag.TotalSpent = allBookingsList
                .Where(b => b.trangThaiDatPhong == "Đã trả phòng")
                .Sum(b => (decimal?)b.tienPhong) ?? 0;

            // Lọc theo trạng thái nếu có
            IQueryable<DatPhong> filteredBookings = allBookings;
            if (!string.IsNullOrEmpty(status) && status != "all")
            {
                filteredBookings = filteredBookings.Where(b => b.trangThaiDatPhong == status);
            }

            // Tính toán phân trang
            int totalItems = filteredBookings.Count();
            var bookings = filteredBookings
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Thông tin phân trang
            ViewBag.CurrentPage = pageNumber;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            ViewBag.PageSize = pageSize;
            ViewBag.CurrentStatus = status ?? "all";
            ViewBag.UserName = Session["UserName"];

            return View(bookings);
        }

        /// <summary>
        /// GET: Customer/Home65130650/GetBookingDetails
        /// Lấy chi tiết đơn đặt phòng dưới dạng JSON (cho modal popup)
        /// </summary>
        [HttpGet]
        public ActionResult GetBookingDetails(string id)
        {
            var userId = Session["UserId"] as string;
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { success = false, message = "Vui lòng đăng nhập." }, JsonRequestBehavior.AllowGet);
            }

            var booking = db.DatPhongs
                .Where(b => b.maDatPhong == id && b.maKhachHang == userId)
                .Select(b => new
                {
                    b.maDatPhong,
                    b.ngayNhanPhong,
                    b.ngayTraPhong,
                    b.soKhach,
                    b.tienPhong,
                    b.tienDatCoc,
                    b.trangThaiDatPhong,
                    b.yeuCauDacBiet,
                    b.ngayDat,
                    b.lyDoHuy,
                    b.ngayHuy,
                    Phong = new
                    {
                        b.Phong.soPhong,
                        b.Phong.tang,
                        LoaiPhong = new
                        {
                            b.Phong.LoaiPhong.tenLoaiPhong,
                            b.Phong.LoaiPhong.giaCoBan,
                            b.Phong.LoaiPhong.soNguoiToiDa,
                            b.Phong.LoaiPhong.dienTichPhong,
                            b.Phong.LoaiPhong.tienNghi,
                            b.Phong.LoaiPhong.hinhAnh
                        }
                    },
                    DichVuDatPhongs = b.DichVuDatPhongs.Select(dv => new
                    {
                        dv.DichVu.tenDichVu,
                        dv.soLuong,
                        dv.donGia,
                        dv.thanhTien,
                        dv.ngaySuDung,
                        dv.ghiChu
                    }),
                    ThanhToans = b.ThanhToans.Select(tt => new
                    {
                        tt.maThanhToan,
                        tt.ngayThanhToan,
                        tt.soTien,
                        tt.phuongThucThanhToan,
                        tt.trangThaiThanhToan,
                        tt.ghiChu
                    }),
                    TongTienDichVu = b.DichVuDatPhongs.Sum(dv => (decimal?)dv.thanhTien) ?? 0,
                    TongDaThanhToan = b.ThanhToans
                        .Where(tt => tt.trangThaiThanhToan == "Thành công")
                        .Sum(tt => (decimal?)tt.soTien) ?? 0
                })
                .FirstOrDefault();

            if (booking == null)
            {
                return Json(new { success = false, message = "Không tìm thấy đơn đặt phòng." }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { success = true, data = booking }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// POST: Customer/Home65130650/CancelBooking
        /// Hủy đơn đặt phòng (chỉ được hủy khi đơn ở trạng thái "Chờ xác nhận")
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CancelBooking(string id, string lyDoHuy)
        {
            var userId = Session["UserId"] as string;
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { success = false, message = "Vui lòng đăng nhập." });
            }

            var booking = db.DatPhongs.FirstOrDefault(b => b.maDatPhong == id && b.maKhachHang == userId);
            if (booking == null)
            {
                return Json(new { success = false, message = "Không tìm thấy đơn đặt phòng." });
            }

            // Chỉ cho phép hủy khi đơn ở trạng thái "Chờ xác nhận"
            if (booking.trangThaiDatPhong != "Chờ xác nhận")
            {
                return Json(new { success = false, message = "Chỉ có thể hủy đơn ở trạng thái 'Chờ xác nhận'. Vui lòng liên hệ lễ tân để được hỗ trợ." });
            }

            try
            {
                booking.trangThaiDatPhong = "Đã hủy";
                booking.lyDoHuy = lyDoHuy ?? "Khách hàng tự hủy";
                booking.ngayHuy = DateTime.Now;
                booking.ngayCapNhat = DateTime.Now;

                db.Entry(booking).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                return Json(new { success = true, message = "Đã hủy đơn đặt phòng thành công." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi hệ thống: " + ex.Message });
            }
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