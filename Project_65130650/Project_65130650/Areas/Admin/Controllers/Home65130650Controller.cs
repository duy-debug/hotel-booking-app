using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity.Validation;
using System.Data.Entity.Infrastructure;
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
        public ActionResult Users(int page = 1, int pageSize = 10, string search = "", string role = "", string status = "")
        {
            ViewBag.Title = "Quản lý Người dùng";
            ViewBag.UserName = Session["UserName"];

            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;

            // Get all users for statistics
            var allUsers = _db.NguoiDungs.ToList();
            
            // Apply filters
            var filteredUsers = allUsers.AsQueryable();
            
            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                filteredUsers = filteredUsers.Where(u => 
                    (u.hoTen != null && u.hoTen.ToLower().Contains(search)) ||
                    (u.email != null && u.email.ToLower().Contains(search)) ||
                    (u.soDienThoai != null && u.soDienThoai.Contains(search)));
            }
            
            if (!string.IsNullOrEmpty(role))
            {
                string roleName = role == "Quản trị" ? "Quản trị" : "Khách hàng";
                filteredUsers = filteredUsers.Where(u => u.vaiTro == roleName);
            }
            
            if (!string.IsNullOrEmpty(status))
            {
                if (status == "active")
                {
                    filteredUsers = filteredUsers.Where(u => u.trangThaiHoatDong == true || u.trangThaiHoatDong == null);
                }
                else if (status == "inactive")
                {
                    filteredUsers = filteredUsers.Where(u => u.trangThaiHoatDong == false);
                }
            }
            
            var filteredList = filteredUsers.ToList();
            
            // Calculate statistics from all users (not filtered)
            ViewBag.TotalUsers = allUsers.Count;
            ViewBag.TotalAdmins = allUsers.Count(u => u.vaiTro == "Quản trị");
            ViewBag.TotalCustomers = allUsers.Count(u => u.vaiTro == "Khách hàng");
            ViewBag.TotalActive = allUsers.Count(u => u.trangThaiHoatDong == true || u.trangThaiHoatDong == null);
            ViewBag.TotalInactive = allUsers.Count(u => u.trangThaiHoatDong == false);
            
            // Preserve search parameters for view
            ViewBag.Search = search;
            ViewBag.Role = role;
            ViewBag.Status = status;

            // Pagination
            var totalUsers = filteredList.Count;
            var totalPages = (int)Math.Ceiling(totalUsers / (double)pageSize);
            if (page > totalPages && totalPages > 0) page = totalPages;

            var users = filteredList
                .OrderByDescending(kh => kh.ngayTao)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.FilteredCount = totalUsers;

            return View(users);
        }

        // GET: Admin/Home65130650/GetUserDetails
        [HttpGet]
        public JsonResult GetUserDetails(string id)
        {
            try
            {
                var user = _db.NguoiDungs.Find(id);
                if (user == null)
                    return Json(new { success = false, error = "Không tìm thấy người dùng" }, JsonRequestBehavior.AllowGet);

                // Map 'vaiTro' text back to code for dropdown
                string maVaiTro = user.vaiTro == "Quản trị" ? "QT" : "KH";

                var userCode = (user.maNguoiDung ?? string.Empty).Trim();
                // Lấy danh sách đặt phòng theo mã KH, bỏ qua phân biệt hoa/thường và khoảng trắng
                var bookingHistory = _db.DatPhongs
                    .AsEnumerable()
                    .Where(dp => string.Equals((dp.maKhachHang ?? string.Empty).Trim(), userCode, StringComparison.OrdinalIgnoreCase))
                    .OrderByDescending(dp => dp.ngayDat ?? dp.ngayNhanPhong) // nếu ngayDat null thì ưu tiên ngày nhận
                    .Take(20)
                    .Select(dp => new
                    {
                        dp.maDatPhong,
                        dp.maPhong,
                        dp.trangThaiDatPhong,
                        ngayDat = dp.ngayDat,
                        ngayNhan = dp.ngayNhanPhong,
                        ngayTra = dp.ngayTraPhong
                    })
                    .ToList();

                return Json(new
                {
                    success = true,
                    data = new
                    {
                        user.maNguoiDung,
                        user.hoTen,
                        user.email,
                        user.soDienThoai,
                        user.diaChi,
                        user.gioiTinh,
                        ngaySinh = user.ngaySinh.HasValue ? user.ngaySinh.Value.ToString("yyyy-MM-dd") : null,
                        maVaiTro = maVaiTro,
                        vaiTro = user.vaiTro,
                        user.trangThaiHoatDong,
                        ngayTao = user.ngayTao.HasValue ? user.ngayTao.Value.ToString("dd/MM/yyyy HH:mm") : "",
                        ngayCapNhat = user.ngayCapNhat.HasValue ? user.ngayCapNhat.Value.ToString("dd/MM/yyyy HH:mm") : "",
                        bookings = bookingHistory.Select(b => new
                        {
                            b.maDatPhong,
                            b.maPhong,
                            b.trangThaiDatPhong,
                            ngayDat = b.ngayDat.HasValue ? b.ngayDat.Value.ToString("dd/MM/yyyy") : "",
                            ngayNhan = b.ngayNhan.ToString("dd/MM/yyyy"),
                            ngayTra = b.ngayTra.ToString("dd/MM/yyyy")
                        })
                    }
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // Returns 500 status code implicitly if exceptions occur outside of this block, 
                // but here we catch and return 200 with error message
                return Json(new { success = false, error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // POST: Admin/Home65130650/CreateUser
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult CreateUser(string hoTen, string email, string soDienThoai, string matKhau,
                                    string diaChi, string gioiTinh, DateTime? ngaySinh, string maVaiTro)
        {
            try
            {
                // Kiểm tra email trùng
                if (_db.NguoiDungs.Any(u => u.email == email))
                    return Json(new { success = false, error = "Email đã tồn tại trong hệ thống" });

                // Kiểm tra SĐT trùng
                if (!string.IsNullOrEmpty(soDienThoai) && _db.NguoiDungs.Any(u => u.soDienThoai == soDienThoai))
                    return Json(new { success = false, error = "Số điện thoại đã tồn tại trong hệ thống" });

                // Tạo mã người dùng tự động
                string roleName = maVaiTro == "QT" ? "Quản trị" : "Khách hàng";
                string maNguoiDung = GenerateUserId(maVaiTro);

                var newUser = new NguoiDung
                {
                    maNguoiDung = maNguoiDung,
                    hoTen = hoTen,
                    email = email,
                    soDienThoai = soDienThoai,
                    matKhau = matKhau, // Plain text (nên hash trong thực tế)
                    diaChi = diaChi,
                    gioiTinh = gioiTinh,
                    ngaySinh = ngaySinh,
                    vaiTro = roleName,
                    trangThaiHoatDong = true,
                    ngayTao = DateTime.Now
                };

                _db.NguoiDungs.Add(newUser);

                // Tự kiểm tra validation trước khi SaveChanges để lấy lỗi chi tiết
                var preValidation = _db.GetValidationErrors().ToList();
                if (preValidation.Any())
                {
                    return Json(new { success = false, error = FormatValidationErrors(preValidation) });
                }

                _db.SaveChanges();

                return Json(new { success = true, message = "Thêm người dùng thành công!" });
            }
            catch (DbEntityValidationException ex)
            {
                return Json(new { success = false, error = BuildValidationErrorMessage(ex) });
            }
            catch (DbUpdateException ex)
            {
                return Json(new { success = false, error = "Lỗi DB: " + GetInnermostMessage(ex) });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = "Lỗi: " + GetInnermostMessage(ex) });
            }
        }

        // POST: Admin/Home65130650/UpdateUser
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult UpdateUser(string maNguoiDung, string hoTen, string email, string soDienThoai,
                                    string diaChi, string gioiTinh, DateTime? ngaySinh, string maVaiTro)
        {
            try
            {
                var user = _db.NguoiDungs.Find(maNguoiDung);
                if (user == null)
                    return Json(new { success = false, error = "Không tìm thấy người dùng" });

                // Kiểm tra email trùng (trừ chính user này)
                if (_db.NguoiDungs.Any(u => u.email == email && u.maNguoiDung != maNguoiDung))
                    return Json(new { success = false, error = "Email đã tồn tại trong hệ thống" });

                // Kiểm tra SĐT trùng
                if (!string.IsNullOrEmpty(soDienThoai) && _db.NguoiDungs.Any(u => u.soDienThoai == soDienThoai && u.maNguoiDung != maNguoiDung))
                    return Json(new { success = false, error = "Số điện thoại đã tồn tại trong hệ thống" });

                string roleName = maVaiTro == "QT" ? "Quản trị" : "Khách hàng";

                user.hoTen = hoTen;
                user.email = email;
                user.soDienThoai = soDienThoai;
                user.diaChi = diaChi;
                user.gioiTinh = gioiTinh;
                user.ngaySinh = ngaySinh;
                user.vaiTro = roleName;
                user.ngayCapNhat = DateTime.Now;

                // Tự kiểm tra validation trước khi SaveChanges để lấy lỗi chi tiết
                var preValidation = _db.GetValidationErrors().ToList();
                if (preValidation.Any())
                {
                    return Json(new { success = false, error = FormatValidationErrors(preValidation) });
                }

                _db.SaveChanges();

                return Json(new { success = true, message = "Cập nhật thông tin thành công!" });
            }
            catch (DbEntityValidationException ex)
            {
                return Json(new { success = false, error = BuildValidationErrorMessage(ex) });
            }
            catch (DbUpdateException ex)
            {
                return Json(new { success = false, error = "Lỗi DB: " + GetInnermostMessage(ex) });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = "Lỗi: " + GetInnermostMessage(ex) });
            }
        }

        // POST: Admin/Home65130650/ToggleUserStatus
        [HttpPost]
        public JsonResult ToggleUserStatus(string id)
        {
            try
            {
                var user = _db.NguoiDungs.Find(id);
                if (user == null)
                    return Json(new { success = false, error = "Không tìm thấy người dùng" });

                // Không cho phép tự vô hiệu hóa chính mình
                var currentUserId = Session["UserId"]?.ToString();
                if (user.maNguoiDung == currentUserId)
                    return Json(new { success = false, error = "Bạn không thể vô hiệu hóa tài khoản của chính mình" });

                // Toggle trạng thái
                user.trangThaiHoatDong = !(user.trangThaiHoatDong ?? true);
                user.ngayCapNhat = DateTime.Now;

                _db.SaveChanges();

                var newStatus = user.trangThaiHoatDong == true ? "kích hoạt" : "vô hiệu hóa";
                return Json(new { success = true, message = $"Đã {newStatus} tài khoản thành công!", isActive = user.trangThaiHoatDong });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        // POST: Admin/Home65130650/ResetUserPassword
        [HttpPost]
        public JsonResult ResetUserPassword(string id, string newPassword)
        {
            try
            {
                var user = _db.NguoiDungs.Find(id);
                if (user == null)
                    return Json(new { success = false, error = "Không tìm thấy người dùng" });

                // Validate password đơn giản
                if (string.IsNullOrEmpty(newPassword) || newPassword.Length < 8)
                    return Json(new { success = false, error = "Mật khẩu mới phải có ít nhất 8 ký tự" });

                user.matKhau = newPassword; // Plain text (nên hash trong thực tế)
                user.ngayCapNhat = DateTime.Now;

                _db.SaveChanges();

                return Json(new { success = true, message = "Đặt lại mật khẩu thành công!" });
            }
            catch (DbEntityValidationException ex)
            {
                return Json(new { success = false, error = BuildValidationErrorMessage(ex) });
            }
            catch (DbUpdateException ex)
            {
                return Json(new { success = false, error = "Lỗi DB: " + GetInnermostMessage(ex) });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = GetInnermostMessage(ex) });
            }
        }

        // GET: Admin/Home65130650/GetRoomDetails
        [HttpGet]
        public JsonResult GetRoomDetails(string id)
        {
            try
            {
                var room = _db.Phongs.Find(id);
                if (room == null)
                    return Json(new { success = false, error = "Không tìm thấy phòng" }, JsonRequestBehavior.AllowGet);

                return Json(new
                {
                    success = true,
                    data = new
                    {
                        room.maPhong,
                        room.soPhong,
                        room.maLoaiPhong,
                        tenLoaiPhong = room.LoaiPhong != null ? room.LoaiPhong.tenLoaiPhong : "",
                        room.tang,
                        room.trangThai,
                        room.trangThaiHoatDong,

                        giaCoBan = room.LoaiPhong != null ? room.LoaiPhong.giaCoBan : 0,
                        room.moTa
                    }
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // POST: Admin/Home65130650/CreateRoom
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult CreateRoom(Phong model)
        {
            try
            {
                if (_db.Phongs.Any(p => p.soPhong == model.soPhong))
                    return Json(new { success = false, error = "Số phòng đã tồn tại" });

                model.maPhong = GenerateRoomId();
                model.trangThai = model.trangThai ?? "Còn trống";
                model.trangThaiHoatDong = true;
                model.ngayTao = DateTime.Now;
                model.ngayCapNhat = DateTime.Now;

                _db.Phongs.Add(model);
                _db.SaveChanges();

                return Json(new { success = true, message = "Thêm phòng thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        // POST: Admin/Home65130650/UpdateRoom
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult UpdateRoom(Phong model)
        {
            try
            {
                var room = _db.Phongs.Find(model.maPhong);
                if (room == null)
                    return Json(new { success = false, error = "Không tìm thấy phòng" });

                // Check duplicate SoPhong but exclude current room
                if (_db.Phongs.Any(p => p.soPhong == model.soPhong && p.maPhong != model.maPhong))
                    return Json(new { success = false, error = "Số phòng đã tồn tại" });

                room.soPhong = model.soPhong;
                room.maLoaiPhong = model.maLoaiPhong;
                room.tang = model.tang;
                room.trangThai = model.trangThai;
                room.trangThaiHoatDong = model.trangThaiHoatDong;
                room.moTa = model.moTa;
                room.ngayCapNhat = DateTime.Now;

                _db.SaveChanges();

                return Json(new { success = true, message = "Cập nhật phòng thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        // POST: Admin/Home65130650/ToggleRoomStatus
        [HttpPost]
        public JsonResult ToggleRoomStatus(string id)
        {
            try
            {
                var room = _db.Phongs.Find(id);
                if (room == null)
                    return Json(new { success = false, error = "Không tìm thấy phòng" });

                room.trangThaiHoatDong = !(room.trangThaiHoatDong ?? true);
                _db.SaveChanges();

                var status = room.trangThaiHoatDong == true ? "kích hoạt" : "vô hiệu hóa";
                return Json(new { success = true, message = $"Đã {status} phòng thành công!", isActive = room.trangThaiHoatDong });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }



        private string GenerateRoomId()
        {
            // Fetch all existing IDs to find the max value.
            // Expected format: P0001, P0002... P0030
            var ids = _db.Phongs
                .Where(p => p.maPhong.StartsWith("P"))
                .Select(p => p.maPhong)
                .ToList();

            if (!ids.Any()) return "P0001";

            int maxVal = 0;
            foreach (var id in ids)
            {
                // Remove 'P' and parse the number
                // Using Trim() to be safe.
                var cleanId = id.Trim();
                if (cleanId.Length > 1) 
                {
                    string numPart = cleanId.Substring(1);
                    if (int.TryParse(numPart, out int num))
                    {
                        if (num > maxVal) maxVal = num;
                    }
                }
            }

            // Generate next ID with 4-digit padding (e.g., 30 -> 31 -> P0031)
            return "P" + (maxVal + 1).ToString("D4");
        }

            // GET: Admin/Home65130650/RoomTypes
            // GET: Admin/Home65130650/RoomTypes
            public ActionResult RoomTypes(int page = 1, string search = "", string availability = "")
            {
                ViewBag.Title = "Quản lý Loại phòng";
                ViewBag.UserName = Session["UserName"];

                int pageSize = 10;
                var query = _db.LoaiPhongs.AsQueryable();

                if (!string.IsNullOrEmpty(search))
                {
                    // search = search.ToLower(); // Remove to preserve case and rely on SQL collation
                    query = query.Where(l => l.tenLoaiPhong.Contains(search) || l.maLoaiPhong.Contains(search));
                }

                if (!string.IsNullOrEmpty(availability))
                {
                    if (availability == "active")
                        query = query.Where(l => l.trangThaiHoatDong == true || l.trangThaiHoatDong == null);
                    else if (availability == "inactive")
                        query = query.Where(l => l.trangThaiHoatDong == false);
                }

                ViewBag.TotalRoomTypes = query.Count();
                
                var totalItems = query.Count();
                var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

                if (page < 1) page = 1;
                if (page > totalPages && totalPages > 0) page = totalPages;

                var roomTypes = query
                    .OrderBy(lp => lp.tenLoaiPhong)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                ViewBag.CurrentPage = page;
                ViewBag.TotalPages = totalPages;
                ViewBag.Search = search;
                ViewBag.Availability = availability;

                return View(roomTypes);
            }

        // GET: Admin/Home65130650/GetRoomTypeDetails
        [HttpGet]
        public JsonResult GetRoomTypeDetails(string id)
        {
            try
            {
                var type = _db.LoaiPhongs.Find(id);
                if (type == null)
                    return Json(new { success = false, error = "Không tìm thấy loại phòng" }, JsonRequestBehavior.AllowGet);

                return Json(new
                {
                    success = true,
                    data = new
                    {
                        type.maLoaiPhong,
                        type.tenLoaiPhong,
                        type.giaCoBan,
                        type.soNguoiToiDa,
                        type.dienTichPhong,
                        type.moTa,
                        hinhAnh = !string.IsNullOrEmpty(type.hinhAnh) && !type.hinhAnh.StartsWith("/") && !type.hinhAnh.StartsWith("http") ? "/Images/" + type.hinhAnh : type.hinhAnh,
                        type.trangThaiHoatDong,
                        type.loaiGiuong,
                        type.tienNghi
                    }
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // POST: Admin/Home65130650/CreateRoomType
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult CreateRoomType(LoaiPhong model, System.Web.HttpPostedFileBase imageFile)
        {
            try
            {
                if (_db.LoaiPhongs.Any(lp => lp.tenLoaiPhong == model.tenLoaiPhong))
                     return Json(new { success = false, error = "Tên loại phòng đã tồn tại" });

                // Generate ID: LP001...
                model.maLoaiPhong = GenerateRoomTypeId();
                model.trangThaiHoatDong = true;
                if (model.dienTichPhong == null) model.dienTichPhong = 0;
                model.ngayTao = DateTime.Now;

                // Handle Image Upload
                if (imageFile != null && imageFile.ContentLength > 0)
                {
                    string fileName = System.IO.Path.GetFileName(imageFile.FileName);
                    var serverSavePath = Server.MapPath("~/Images");
                    
                    // Save to user requested "Images" folder
                    string path = System.IO.Path.Combine(serverSavePath, fileName);
                    
                    // Create directory if not exists
                    if (!System.IO.Directory.Exists(serverSavePath))
                    {
                        System.IO.Directory.CreateDirectory(serverSavePath);
                    }

                    imageFile.SaveAs(path);
                    model.hinhAnh = fileName; 
                }

                _db.LoaiPhongs.Add(model);
                _db.SaveChanges();

                return Json(new { success = true, message = "Thêm loại phòng thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        // POST: Admin/Home65130650/UpdateRoomType
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult UpdateRoomType(LoaiPhong model, System.Web.HttpPostedFileBase imageFile)
        {
            try
            {
                var type = _db.LoaiPhongs.Find(model.maLoaiPhong);
                if (type == null)
                    return Json(new { success = false, error = "Không tìm thấy loại phòng" });

                type.tenLoaiPhong = model.tenLoaiPhong;
                type.soNguoiToiDa = model.soNguoiToiDa;
                type.dienTichPhong = model.dienTichPhong;
                type.giaCoBan = model.giaCoBan;
                type.moTa = model.moTa;
                type.trangThaiHoatDong = model.trangThaiHoatDong;
                type.loaiGiuong = model.loaiGiuong;
                type.tienNghi = model.tienNghi;

                // Handle Image Upload
                if (imageFile != null && imageFile.ContentLength > 0)
                {
                    // Delete old image if exists
                    if (!string.IsNullOrEmpty(type.hinhAnh))
                    {
                         // Use GetFileName to avoid issues if DB has relative paths like /Images/file.jpg
                         string oldFileName = System.IO.Path.GetFileName(type.hinhAnh);
                         string oldPath = System.IO.Path.Combine(Server.MapPath("~/Images"), oldFileName);
                         if (System.IO.File.Exists(oldPath))
                         {
                             // Attempt to delete. If file is locked, this might throw, which is better than silent fail given user feedback.
                             System.IO.File.Delete(oldPath);
                         }
                    }

                    string fileName = System.IO.Path.GetFileName(imageFile.FileName);
                    var serverSavePath = Server.MapPath("~/Images");
                    
                    // Save to user requested "Images" folder
                    string path = System.IO.Path.Combine(serverSavePath, fileName);
                    
                    // Create directory if not exists
                    if (!System.IO.Directory.Exists(serverSavePath))
                    {
                        System.IO.Directory.CreateDirectory(serverSavePath);
                    }

                    imageFile.SaveAs(path);
                    type.hinhAnh = fileName;
                }

                _db.SaveChanges();

                return Json(new { success = true, message = "Cập nhật loại phòng thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        // POST: Admin/Home65130650/ToggleRoomTypeStatus
        [HttpPost]
        public JsonResult ToggleRoomTypeStatus(string id)
        {
            try
            {
                var type = _db.LoaiPhongs.Find(id);
                if (type == null)
                    return Json(new { success = false, error = "Không tìm thấy loại phòng" });

                bool currentStatus = type.trangThaiHoatDong ?? false;
                type.trangThaiHoatDong = !currentStatus;
                _db.SaveChanges();

                return Json(new { success = true, message = "Đã thay đổi trạng thái thành công" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        private string GenerateRoomTypeId()
        {
            // LP001, LP002
            var ids = _db.LoaiPhongs
                .Where(lp => lp.maLoaiPhong.StartsWith("LP"))
                .Select(lp => lp.maLoaiPhong)
                .ToList();

             if (!ids.Any()) return "LP001";

            int maxVal = 0;
            foreach (var id in ids)
            {
                var cleanId = id.Trim();
                if (cleanId.Length > 2 && int.TryParse(cleanId.Substring(2), out int num))
                {
                    if (num > maxVal) maxVal = num;
                }
            }

            return "LP" + (maxVal + 1).ToString("D3");
        }

        // GET: Admin/Home65130650/Rooms
        public ActionResult Rooms(int page = 1, string search = "", string status = "", string floor = "", string availability = "")
        {
            ViewBag.Title = "Quản lý Phòng";
            ViewBag.UserName = Session["UserName"];

            int pageSize = 10;
            // Original query should allow inactive items to be filtered later, 
            // BUT previous code restricted to active only (Where(p => p.trangThaiHoatDong == true)).
            // We must remove that initial restriction to allow filtering by "inactive".
            var roomsQuery = _db.Phongs.AsQueryable(); 

            // If no availability filter is selected, maybe default to showing ALL or just Active?
            // User request implies they want option to see Active OR Inactive.
            // Let's assume default shows ALL or handle below.
            // Usually default View might only show Active, but if we have a filter "All Status", "Active", "Inactive".
            // Let's start with ALL queryable.

            // Calculate overall stats (unfiltered)
            ViewBag.TotalRooms = roomsQuery.Count();
            ViewBag.AvailableRooms = roomsQuery.Count(p => p.trangThai == "Còn trống");
            ViewBag.BookedRooms = roomsQuery.Count(p => p.trangThai == "Đã đặt");
            ViewBag.OccupiedRooms = roomsQuery.Count(p => p.trangThai == "Đang sử dụng");
            ViewBag.MaintenanceRooms = roomsQuery.Count(p => p.trangThai == "Bảo trì");

            // Apply filters
            if (!string.IsNullOrEmpty(search))
            {
                 // search = search.ToLower();
                 // Search by Room Number OR Room Type Name
                 roomsQuery = roomsQuery.Where(p => p.soPhong.Contains(search) || (p.LoaiPhong != null && p.LoaiPhong.tenLoaiPhong.Contains(search)));
            }

            if (!string.IsNullOrEmpty(status))
            {
                roomsQuery = roomsQuery.Where(p => p.trangThai == status);
            }

            if (!string.IsNullOrEmpty(floor))
            {
                int floorNum = int.Parse(floor);
                roomsQuery = roomsQuery.Where(p => p.tang == floorNum);
            }

            if (!string.IsNullOrEmpty(availability))
            {
                if (availability == "active")
                {
                    roomsQuery = roomsQuery.Where(p => p.trangThaiHoatDong == true || p.trangThaiHoatDong == null);
                }
                else if (availability == "inactive")
                {
                    roomsQuery = roomsQuery.Where(p => p.trangThaiHoatDong == false);
                }
            }

            var totalRooms = roomsQuery.Count();
            var totalPages = (int)Math.Ceiling((double)totalRooms / pageSize);

            if (page < 1) page = 1;
            if (page > totalPages && totalPages > 0) page = totalPages;

            var rooms = roomsQuery
                .OrderBy(p => p.tang)
                .ThenBy(p => p.soPhong)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.RoomTypes = _db.LoaiPhongs
                .Where(lp => lp.trangThaiHoatDong == true || lp.trangThaiHoatDong == null)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.Search = search;
            ViewBag.Status = status;
            ViewBag.Floor = floor;
            ViewBag.Availability = availability;
            ViewBag.Floors = _db.Phongs.Select(p => p.tang).Distinct().OrderBy(t => t).ToList();

            return View(rooms);
        }

        // GET: Admin/Home65130650/Services
        // GET: Admin/Home65130650/Services
        public ActionResult Services(int page = 1, string search = "", string availability = "", string serviceType = "")
        {
            ViewBag.Title = "Quản lý Dịch vụ";
            ViewBag.UserName = Session["UserName"];

            int pageSize = 10;
            var query = _db.DichVus.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(dv => dv.tenDichVu.Contains(search) || dv.maDichVu.Contains(search) || dv.loaiDichVu.Contains(search));
            }

            if (!string.IsNullOrEmpty(serviceType))
            {
                query = query.Where(dv => dv.loaiDichVu == serviceType);
            }

            if (!string.IsNullOrEmpty(availability))
            {
                if (availability == "active")
                    query = query.Where(dv => dv.trangThaiHoatDong == true || dv.trangThaiHoatDong == null);
                else if (availability == "inactive")
                    query = query.Where(dv => dv.trangThaiHoatDong == false);
            }

            ViewBag.TotalServices = query.Count();

            var totalItems = query.Count();
            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            if (page < 1) page = 1;
            if (page > totalPages && totalPages > 0) page = totalPages;

            var services = query
                .OrderBy(dv => dv.tenDichVu)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.Search = search;
            ViewBag.Availability = availability;
            ViewBag.ServiceType = serviceType;

            // Get distinct service types for dropdown
            ViewBag.ServiceTypes = _db.DichVus
                                    .Select(dv => dv.loaiDichVu)
                                    .Distinct()
                                    .Where(t => !string.IsNullOrEmpty(t))
                                    .OrderBy(t => t)
                                    .ToList();

            return View(services);
        }

        // GET: Admin/Home65130650/GetServiceDetails
        [HttpGet]
        public JsonResult GetServiceDetails(string id)
        {
            try
            {
                var service = _db.DichVus.Find(id);
                if (service == null)
                    return Json(new { success = false, error = "Không tìm thấy dịch vụ" }, JsonRequestBehavior.AllowGet);

                return Json(new
                {
                    success = true,
                    data = new
                    {
                        service.maDichVu,
                        service.tenDichVu,
                        service.moTa,
                        service.giaDichVu,
                        service.loaiDichVu,
                        service.trangThaiHoatDong,
                        hinhAnh = !string.IsNullOrEmpty(service.hinhAnh) && !service.hinhAnh.StartsWith("/") && !service.hinhAnh.StartsWith("http") ? "/Images/" + service.hinhAnh : service.hinhAnh
                    }
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // POST: Admin/Home65130650/CreateService
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult CreateService(DichVu model, System.Web.HttpPostedFileBase imageFile)
        {
            try
            {
                if (_db.DichVus.Any(dv => dv.tenDichVu == model.tenDichVu))
                    return Json(new { success = false, error = "Tên dịch vụ đã tồn tại" });

                model.maDichVu = GenerateServiceId();
                model.trangThaiHoatDong = true;
                model.ngayTao = DateTime.Now;

                if (imageFile != null && imageFile.ContentLength > 0)
                {
                    string fileName = System.IO.Path.GetFileName(imageFile.FileName);
                    var serverSavePath = Server.MapPath("~/Images");
                    string path = System.IO.Path.Combine(serverSavePath, fileName);
                    
                    if (!System.IO.Directory.Exists(serverSavePath))
                    {
                        System.IO.Directory.CreateDirectory(serverSavePath);
                    }

                    imageFile.SaveAs(path);
                    model.hinhAnh = fileName;
                }

                _db.DichVus.Add(model);
                _db.SaveChanges();

                return Json(new { success = true, message = "Thêm dịch vụ thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        // POST: Admin/Home65130650/UpdateService
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult UpdateService(DichVu model, System.Web.HttpPostedFileBase imageFile)
        {
            try
            {
                var service = _db.DichVus.Find(model.maDichVu);
                if (service == null)
                    return Json(new { success = false, error = "Không tìm thấy dịch vụ" });

                service.tenDichVu = model.tenDichVu;
                service.moTa = model.moTa;
                service.giaDichVu = model.giaDichVu;
                service.loaiDichVu = model.loaiDichVu;
                service.trangThaiHoatDong = model.trangThaiHoatDong;

                if (imageFile != null && imageFile.ContentLength > 0)
                {
                     // Delete old image if exists
                    if (!string.IsNullOrEmpty(service.hinhAnh))
                    {
                         string oldFileName = System.IO.Path.GetFileName(service.hinhAnh);
                         string oldPath = System.IO.Path.Combine(Server.MapPath("~/Images"), oldFileName);
                         if (System.IO.File.Exists(oldPath))
                         {
                             System.IO.File.Delete(oldPath);
                         }
                    }

                    string fileName = System.IO.Path.GetFileName(imageFile.FileName);
                    var serverSavePath = Server.MapPath("~/Images");
                    string path = System.IO.Path.Combine(serverSavePath, fileName);
                    
                    if (!System.IO.Directory.Exists(serverSavePath))
                    {
                        System.IO.Directory.CreateDirectory(serverSavePath);
                    }

                    imageFile.SaveAs(path);
                    service.hinhAnh = fileName;
                }

                _db.SaveChanges();

                return Json(new { success = true, message = "Cập nhật dịch vụ thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        // POST: Admin/Home65130650/ToggleServiceStatus
        [HttpPost]
        public JsonResult ToggleServiceStatus(string id)
        {
            try
            {
                var service = _db.DichVus.Find(id);
                if (service == null)
                    return Json(new { success = false, error = "Không tìm thấy dịch vụ" });

                service.trangThaiHoatDong = !(service.trangThaiHoatDong ?? true);
                _db.SaveChanges();

                return Json(new { success = true, message = "Đã thay đổi trạng thái thành công" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        private string GenerateServiceId()
        {
            var ids = _db.DichVus
                .Where(dv => dv.maDichVu.StartsWith("DV"))
                .Select(dv => dv.maDichVu)
                .ToList();

             if (!ids.Any()) return "DV001";

            int maxVal = 0;
            foreach (var id in ids)
            {
                var cleanId = id.Trim();
                if (cleanId.Length > 2 && int.TryParse(cleanId.Substring(2), out int num))
                {
                    if (num > maxVal) maxVal = num;
                }
            }

            return "DV" + (maxVal + 1).ToString("D3");
        }

        // GET: Admin/Home65130650/Payments
        public ActionResult Payments(int page = 1, string search = "", string status = "", string method = "", string fromDate = "", string toDate = "")
        {
            ViewBag.Title = "Quản lý Thanh toán";
            ViewBag.UserName = Session["UserName"];

            int pageSize = 10;
            var query = _db.ThanhToans.AsQueryable();

            // Filter by Search (Payment ID, Booking ID, Customer Name)
            if (!string.IsNullOrEmpty(search))
            {
                // search = search.ToLower();
                query = query.Where(tt => 
                    tt.maThanhToan.Contains(search) || 
                    tt.maDatPhong.Contains(search) ||
                    (tt.NguoiDung != null && tt.NguoiDung.hoTen.Contains(search)) ||
                    (tt.DatPhong != null && tt.DatPhong.NguoiDung != null && tt.DatPhong.NguoiDung.hoTen.Contains(search)));
            }

            // Filter by Status
            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(tt => tt.trangThaiThanhToan == status);
            }

            // Filter by Method
            if (!string.IsNullOrEmpty(method))
            {
                query = query.Where(tt => tt.phuongThucThanhToan == method);
            }

            // Filter by Date
            if (!string.IsNullOrEmpty(fromDate) && DateTime.TryParse(fromDate, out DateTime start))
            {
                 query = query.Where(tt => tt.ngayThanhToan >= start);
            }
            if (!string.IsNullOrEmpty(toDate) && DateTime.TryParse(toDate, out DateTime end))
            {
                 // Include the end date fully
                 end = end.AddDays(1).AddTicks(-1);
                 query = query.Where(tt => tt.ngayThanhToan <= end);
            }

            var totalItems = query.Count();
            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            if (page < 1) page = 1;
            if (page > totalPages && totalPages > 0) page = totalPages;

            var payments = query
                .OrderByDescending(tt => tt.ngayThanhToan)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.Search = search;
            ViewBag.Status = status;
            ViewBag.Method = method;
            ViewBag.FromDate = fromDate;
            ViewBag.ToDate = toDate;

            // Stats for the view (All, not filtered, or maybe filtered? Let's use all for global stats)
            // Or use filtered stats? User request G.1 doesn't specify stats behavior but usually stats are global.
            // But preserving the existing simple stats is fine.
            // Let's keep global stats.
            ViewBag.TotalRevenue = _db.ThanhToans.Sum(t => (decimal?)t.soTien) ?? 0;
            ViewBag.TotalTrans = _db.ThanhToans.Count();
            ViewBag.TransferCount = _db.ThanhToans.Count(t => t.phuongThucThanhToan == "Chuyển khoản");
            ViewBag.CashCount = _db.ThanhToans.Count(t => t.phuongThucThanhToan == "Tiền mặt");

            // Getting pending bookings for the "Create Payment" modal dropdown
            ViewBag.PendingBookings = _db.DatPhongs
                .Where(dp => dp.trangThaiDatPhong == "Đã xác nhận" && !dp.ThanhToans.Any(tt => tt.trangThaiThanhToan == "Đã thanh toán" || tt.trangThaiThanhToan == "Thành công"))
                .ToList()
                .Select(dp => new SelectListItem
                {
                    Value = dp.maDatPhong,
                    Text = dp.maDatPhong + " - " + (dp.NguoiDung != null ? dp.NguoiDung.hoTen : "")
                })
                .ToList();

            // if (Request.IsAjaxRequest())
            // {
            //    return PartialView("_PaymentsTable", payments); 
            // }

            return View(payments);
        }

        // GET: Admin/Home65130650/GetPaymentDetails
        [HttpGet]
        public JsonResult GetPaymentDetails(string id)
        {
            try
            {
                var payment = _db.ThanhToans.Find(id);
                if (payment == null)
                    return Json(new { success = false, error = "Không tìm thấy giao dịch" }, JsonRequestBehavior.AllowGet);

                return Json(new
                {
                    success = true,
                    data = new
                    {
                        payment.maThanhToan,
                        payment.maDatPhong,
                        ngayThanhToan = payment.ngayThanhToan.HasValue ? payment.ngayThanhToan.Value.ToString("dd/MM/yyyy HH:mm") : "",
                        payment.soTien,
                        payment.phuongThucThanhToan,
                        payment.trangThaiThanhToan,
                        payment.ghiChu,
                        nguoiXuLy = payment.nguoiXuLy, // Maybe fetching handler name would be better?
                        handlerName = GetUserName(payment.nguoiXuLy),
                        customerName = payment.DatPhong != null && payment.DatPhong.NguoiDung != null ? payment.DatPhong.NguoiDung.hoTen : ""
                    }
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        private string GetUserName(string userId)
        {
            if (string.IsNullOrEmpty(userId)) return "";
            var u = _db.NguoiDungs.Find(userId);
            return u != null ? u.hoTen : userId;
        }

        // POST: Admin/Home65130650/CreatePayment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult CreatePayment(string maDatPhong, decimal soTien, string phuongThucThanhToan, string ghiChu)
        {
            try
            {
                if (string.IsNullOrEmpty(maDatPhong))
                    return Json(new { success = false, error = "Vui lòng chọn mã đặt phòng" });

                if (soTien <= 0)
                    return Json(new { success = false, error = "Số tiền phải lớn hơn 0" });

                var adminId = Session["UserId"]?.ToString();
                
                var payment = new ThanhToan
                {
                    maThanhToan = GeneratePaymentId(),
                    maDatPhong = maDatPhong,
                    ngayThanhToan = DateTime.Now,
                    soTien = soTien,
                    phuongThucThanhToan = phuongThucThanhToan,
                    ghiChu = ghiChu,
                    nguoiXuLy = adminId,
                    trangThaiThanhToan = (phuongThucThanhToan == "Tiền mặt") ? "Thành công" : "Chờ xử lý" // Cash is instant, Transfer might be pending? Or manual entry implies verified?
                    // Request G.2 says: "Update payment status". It implies we set it.
                    // If creating manually, it's usually "Success" (Collected money).
                    // Let's assume manual creation = "Thành công" or "Đã thanh toán" unless specified. 
                    // Let's default to "Thành công" for manual entry.
                };
                
                if (payment.trangThaiThanhToan == null) payment.trangThaiThanhToan = "Thành công";

                _db.ThanhToans.Add(payment);
                _db.SaveChanges();

                return Json(new { success = true, message = "Tạo thanh toán thành công!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        // POST: Admin/Home65130650/UpdatePaymentStatus
        [HttpPost]
        public JsonResult UpdatePaymentStatus(string id, string status, string reason = "")
        {
            try
            {
                var payment = _db.ThanhToans.Find(id);
                if (payment == null)
                    return Json(new { success = false, error = "Không tìm thấy giao dịch" });

                payment.trangThaiThanhToan = status;
                
                if (!string.IsNullOrEmpty(reason))
                {
                    payment.ghiChu = (payment.ghiChu ?? "") + " | Lý do: " + reason; 
                }
                
                var adminId = Session["UserId"]?.ToString();
                payment.nguoiXuLy = adminId; // Update handler

                _db.SaveChanges();

                return Json(new { success = true, message = "Cập nhật trạng thái thành công" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        private string GeneratePaymentId()
        {
            var ids = _db.ThanhToans
                .Where(t => t.maThanhToan.StartsWith("TT"))
                .Select(t => t.maThanhToan)
                .ToList();

            if (!ids.Any()) return "TT001";
            
            int maxVal = 0;
            foreach (var id in ids)
            {
                var cleanId = id.Trim();
                // TT001 -> len 5
                if (cleanId.Length > 2 && int.TryParse(cleanId.Substring(2), out int num))
                {
                    if (num > maxVal) maxVal = num;
                }
            }

            return "TT" + (maxVal + 1).ToString("D3");
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

                return Json(new
                {
                    success = true,
                    message = "Cập nhật thông tin thành công!",
                    updatedInfo = new
                    {
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

        // Sinh mã người dùng tối đa 5 ký tự (phù hợp StringLength 5)
        private string GenerateUserId(string maVaiTro)
        {
            // Yêu cầu mã 5 ký tự: ND### cho khách hàng, AD### cho quản trị
            var prefix = maVaiTro == "QT" ? "AD" : "ND";

            // Dùng AsEnumerable để xử lý parse trong memory, tránh lỗi expression tree
            var numbers = _db.NguoiDungs
                .Where(u => u.maNguoiDung.StartsWith(prefix) && u.maNguoiDung.Length == 5)
                .AsEnumerable()
                .Select(u =>
                {
                    int n;
                    return int.TryParse(u.maNguoiDung.Substring(prefix.Length), out n) ? (int?)n : null;
                })
                .Where(n => n.HasValue)
                .Select(n => n.Value);

            var last = numbers.Any() ? numbers.Max() : 0;
            var next = last + 1;
            if (next > 999) next = 1; // quay vòng nếu vượt quá 999

            var candidate = prefix + next.ToString("D3");

            // Nếu hi hữu bị trùng, thử tăng dần
            for (int i = 0; i < 5; i++)
            {
                if (!_db.NguoiDungs.Any(u => u.maNguoiDung == candidate))
                    return candidate;
                next++;
                if (next > 999) next = 1;
                candidate = prefix + next.ToString("D3");
            }

            return candidate; // trả về ứng viên cuối cùng
        }

        // Gom lỗi validation để trả về chi tiết
        private string BuildValidationErrorMessage(DbEntityValidationException ex)
        {
            var errors = ex.EntityValidationErrors
                .SelectMany(e => e.ValidationErrors)
                .Select(e => $"{e.PropertyName}: {e.ErrorMessage}")
                .ToList();
            return "Lỗi dữ liệu: " + string.Join("; ", errors);
        }

        // Helper: format validation errors from GetValidationErrors
        private string FormatValidationErrors(IEnumerable<DbEntityValidationResult> results)
        {
            var errors = results
                .SelectMany(e => e.ValidationErrors)
                .Select(e => $"{e.PropertyName}: {e.ErrorMessage}")
                .ToList();
            return "Lỗi dữ liệu: " + string.Join("; ", errors);
        }

        // Helper: lấy thông báo lỗi sâu nhất
        private string GetInnermostMessage(Exception ex)
        {
            while (ex.InnerException != null) ex = ex.InnerException;
            return ex.Message;
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