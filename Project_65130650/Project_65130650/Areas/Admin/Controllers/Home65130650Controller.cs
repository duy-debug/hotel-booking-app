using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Project_65130650.Models;
using Project_65130650.Models.ViewModel;

namespace Project_65130650.Areas.Admin.Controllers
{
    /// <summary>
    /// Controller cho Admin Area
    /// Chỉ user có vai trò "Quản trị" mới được truy cập
    /// </summary>
    [Authorize(Roles = "Quản trị")]
    public class Home65130650Controller : Controller
    {
        private readonly Model65130650DbContext _db = new Model65130650DbContext();

        // GET: Admin/Home65130650
        public ActionResult Index()
        {
            ViewBag.Title = "Index";
            ViewBag.UserName = Session["UserName"];
            ViewBag.UserRole = Session["UserRole"];
            ViewBag.UserId = Session["UserId"];

            // Thống kê tổng quan
            var today = DateTime.Today;
            var thisMonth = new DateTime(today.Year, today.Month, 1);
            var nextMonth = thisMonth.AddMonths(1);

            // Tổng số phòng
            var totalRooms = _db.Phongs.Count(p => p.trangThaiHoatDong == true || p.trangThaiHoatDong == null);
            ViewBag.TotalRooms = totalRooms;

            // Phòng đang sử dụng
            var occupiedRooms = _db.Phongs.Count(p => 
                p.trangThai == "Đang sử dụng" && 
                (p.trangThaiHoatDong == true || p.trangThaiHoatDong == null));
            ViewBag.OccupiedRooms = occupiedRooms;

            // Tỷ lệ lấp đầy
            ViewBag.OccupancyRate = totalRooms > 0 ? Math.Round((double)occupiedRooms / totalRooms * 100, 1) : 0;

            // Tổng số đặt phòng tháng này  
            var bookingsThisMonth = _db.DatPhongs.Count(dp => 
                dp.ngayDat >= thisMonth && 
                dp.ngayDat < nextMonth);
            ViewBag.BookingsThisMonth = bookingsThisMonth;

            // Doanh thu tháng này
            var revenueThisMonth = _db.ThanhToans
                .Where(tt => tt.ngayThanhToan >= thisMonth && tt.ngayThanhToan < nextMonth)
                .Sum(tt => (decimal?)tt.soTien) ?? 0;
            ViewBag.RevenueThisMonth = revenueThisMonth;

            // Số khách hàng mới tháng này
            var newCustomersThisMonth = _db.NguoiDungs.Count(kh => 
                kh.ngayTao >= thisMonth && 
                kh.ngayTao < nextMonth);
            ViewBag.NewCustomersThisMonth = newCustomersThisMonth;

            // Đặt phòng gần đây (10 booking mới nhất)
            var recentBookings = _db.DatPhongs
                .OrderByDescending(dp => dp.ngayDat)
                .Take(10)
                .ToList()
                .Select(dp => new RecentBookingViewModel
                {
                    MaDatPhong = dp.maDatPhong,
                    MaKhachHang = dp.maKhachHang,
                    KhachHang = _db.NguoiDungs.FirstOrDefault(kh => kh.maNguoiDung == dp.maKhachHang),
                    MaPhong = dp.maPhong,
                    Phong = _db.Phongs.FirstOrDefault(p => p.maPhong == dp.maPhong),
                    NgayNhanPhong = dp.ngayNhanPhong,
                    NgayTraPhong = dp.ngayTraPhong,
                    TrangThaiDatPhong = dp.trangThaiDatPhong,
                    NgayDat = dp.ngayDat
                })
                .ToList();
            ViewBag.RecentBookings = recentBookings;

            // Phòng sắp check-out hôm nay
            var tomorrow = today.AddDays(1);
            var checkoutsToday = _db.DatPhongs
                .Where(dp => dp.ngayTraPhong >= today && dp.ngayTraPhong < tomorrow && dp.trangThaiDatPhong == "Đã xác nhận")
                .Count();
            ViewBag.CheckoutsToday = checkoutsToday;

            // Phòng sắp check-in hôm nay
            var checkinsToday = _db.DatPhongs
                .Where(dp => dp.ngayNhanPhong >= today && dp.ngayNhanPhong < tomorrow && dp.trangThaiDatPhong == "Đã xác nhận")
                .Count();
            ViewBag.CheckinsToday = checkinsToday;

            return View();
        }

        // GET: Admin/Home65130650/Bookings
        public ActionResult Bookings()
        {
            ViewBag.Title = "Quản lý Đặt phòng";
            ViewBag.UserName = Session["UserName"];
            
            var bookings = _db.DatPhongs
                .OrderByDescending(dp => dp.ngayDat)
                .ToList();
            
            return View(bookings);
        }

        // GET: Admin/Home65130650/Users
        public ActionResult Users()
        {
            ViewBag.Title = "Quản lý Người dùng";
            ViewBag.UserName = Session["UserName"];
            
            var users = _db.NguoiDungs
                .OrderByDescending(kh => kh.ngayTao)
                .ToList();
            
            return View(users);
        }

        // GET: Admin/Home65130650/RoomTypes
        public ActionResult RoomTypes()
        {
            ViewBag.Title = "Quản lý Loại phòng";
            ViewBag.UserName = Session["UserName"];
            
            var roomTypes = _db.LoaiPhongs
                .Where(lp => lp.trangThaiHoatDong == true || lp.trangThaiHoatDong == null)
                .ToList();
            
            return View(roomTypes);
        }

        // GET: Admin/Home65130650/Rooms
        public ActionResult Rooms()
        {
            ViewBag.Title = "Quản lý Phòng";
            ViewBag.UserName = Session["UserName"];
            
            var rooms = _db.Phongs
                .Where(p => p.trangThaiHoatDong == true || p.trangThaiHoatDong == null)
                .OrderBy(p => p.tang)
                .ThenBy(p => p.soPhong)
                .ToList();
            
            return View(rooms);
        }

        // GET: Admin/Home65130650/Services
        public ActionResult Services()
        {
            ViewBag.Title = "Quản lý Dịch vụ";
            ViewBag.UserName = Session["UserName"];
            
            var services = _db.DichVus
                .Where(dv => dv.trangThaiHoatDong == true || dv.trangThaiHoatDong == null)
                .ToList();
            
            return View(services);
        }

        // GET: Admin/Home65130650/Payments
        public ActionResult Payments()
        {
            ViewBag.Title = "Quản lý Thanh toán";
            ViewBag.UserName = Session["UserName"];
            
            var payments = _db.ThanhToans
                .OrderByDescending(tt => tt.ngayThanhToan)
                .ToList();
            
            return View(payments);
        }

        // GET: Admin/Home65130650/Reports
        public ActionResult Reports()
        {
            ViewBag.Title = "Báo cáo & Thống kê";
            ViewBag.UserName = Session["UserName"];
            
            // Thống kê theo tháng (6 tháng gần nhất)
            var sixMonthsAgo = DateTime.Today.AddMonths(-6);
            var monthlyRevenue = _db.ThanhToans
                .Where(tt => tt.ngayThanhToan >= sixMonthsAgo)
                .GroupBy(tt => new { tt.ngayThanhToan.Value.Year, tt.ngayThanhToan.Value.Month })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Revenue = g.Sum(tt => tt.soTien),
                    Count = g.Count()
                })
                .OrderBy(x => x.Year)
                .ThenBy(x => x.Month)
                .ToList();
            
            ViewBag.MonthlyRevenue = monthlyRevenue;
            
            return View();
        }

        // GET: Admin/Home65130650/Settings
        public ActionResult Settings()
        {
            ViewBag.Title = "Cài đặt Hệ thống";
            ViewBag.UserName = Session["UserName"];
            
            return View();
        }

        // GET: Admin/Home65130650/Profile
        public new ActionResult Profile()
        {
            ViewBag.Title = "Hồ sơ của tôi";
            ViewBag.UserName = Session["UserName"];
            
            var userId = Session["UserId"]?.ToString();
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account65130650", new { area = "" });
            }
            
            var user = _db.NguoiDungs.Find(userId);
            if (user == null)
            {
                return HttpNotFound();
            }
            
            return View(user);
        }

        // POST: Admin/Home65130650/UpdateProfile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult UpdateProfile(NguoiDung model, string currentPassword, string newPassword, string confirmPassword)
        {
            try
            {
                var userId = Session["UserId"]?.ToString();
                if (string.IsNullOrEmpty(userId))
                {
                    return Json(new { success = false, error = "Phiên đăng nhập đã hết hạn" });
                }

                var user = _db.NguoiDungs.Find(userId);
                if (user == null)
                {
                    return Json(new { success = false, error = "Không tìm thấy thông tin người dùng" });
                }

                // Xử lý đổi mật khẩu nếu có
                if (!string.IsNullOrEmpty(currentPassword) || !string.IsNullOrEmpty(newPassword))
                {
                    // Kiểm tra mật khẩu hiện tại (so sánh plain text)
                    if (user.matKhau != currentPassword)
                    {
                        return Json(new { success = false, passwordError = "Mật khẩu hiện tại không đúng" });
                    }

                    // Validate mật khẩu mới
                    var passwordValidation = ValidatePassword(newPassword, confirmPassword);
                    if (!passwordValidation.IsValid)
                    {
                        return Json(new { success = false, passwordError = passwordValidation.ErrorMessage });
                    }

                    // Cập nhật mật khẩu mới (lưu plain text)
                    user.matKhau = newPassword;
                }

                // Cập nhật thông tin cơ bản
                user.hoTen = model.hoTen;
                user.soDienThoai = model.soDienThoai;
                user.diaChi = model.diaChi;
                user.gioiTinh = model.gioiTinh;
                user.ngaySinh = model.ngaySinh;
                user.ngayCapNhat = DateTime.Now;

                _db.SaveChanges();
                
                // Cập nhật lại session nếu tên thay đổi
                Session["UserName"] = user.hoTen;

                return Json(new { 
                    success = true, 
                    message = "Cập nhật thông tin thành công!",
                    updatedInfo = new {
                        hoTen = user.hoTen,
                        soDienThoai = user.soDienThoai ?? "Chưa cập nhật",
                        diaChi = user.diaChi ?? "Chưa cập nhật",
                        gioiTinh = user.gioiTinh ?? "Chưa cập nhật",
                        ngaySinh = user.ngaySinh.HasValue ? user.ngaySinh.Value.ToString("dd/MM/yyyy") : "Chưa cập nhật",
                        ngayCapNhat = user.ngayCapNhat.HasValue ? user.ngayCapNhat.Value.ToString("dd/MM/yyyy HH:mm") : "Chưa cập nhật"
                    }
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = "Có lỗi xảy ra: " + ex.Message });
            }
        }

        // Helper method: Validate password
        private (bool IsValid, string ErrorMessage) ValidatePassword(string newPassword, string confirmPassword)
        {
            if (string.IsNullOrEmpty(newPassword))
            {
                return (false, "Mật khẩu mới không được để trống");
            }

            if (newPassword.Length < 8)
            {
                return (false, "Mật khẩu phải có ít nhất 8 ký tự");
            }

            if (!newPassword.Any(char.IsUpper))
            {
                return (false, "Mật khẩu phải có ít nhất 1 ký tự viết hoa");
            }

            if (!newPassword.Any(char.IsDigit))
            {
                return (false, "Mật khẩu phải có ít nhất 1 chữ số");
            }

            if (!newPassword.Any(ch => !char.IsLetterOrDigit(ch)))
            {
                return (false, "Mật khẩu phải có ít nhất 1 ký tự đặc biệt");
            }

            if (newPassword != confirmPassword)
            {
                return (false, "Mật khẩu xác nhận không khớp");
            }

            return (true, string.Empty);
        }

        // POST: Admin/Home65130650/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Index", "Home", new { area = "" });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}