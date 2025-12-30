using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.Data.Entity.Validation;
using Project_65130650.Models;
using Project_65130650.Models.ViewModels;

namespace Project_65130650.Controllers
{
    public class HomeController : Controller
    {
        private readonly Model65130650DbContext _db = new Model65130650DbContext();

        public ActionResult Index()
        {
            var featuredRooms = GetFeaturedRooms();
            ViewBag.FeaturedRooms = featuredRooms;
            return View();
        }

        public ActionResult RoomsList(string search, string maLoaiPhong, string trangThai)
        {
            var rooms = GetRoomsData(search, maLoaiPhong, trangThai);
            return PartialView("RoomsList", rooms);
        }

        private List<RoomCardViewModel65130650> GetFeaturedRooms()
        {
            var query = from p in _db.Phongs
                        join lp in _db.LoaiPhongs on p.maLoaiPhong equals lp.maLoaiPhong
                        where (p.trangThaiHoatDong == true || p.trangThaiHoatDong == null)
                              && (lp.trangThaiHoatDong == true || lp.trangThaiHoatDong == null)
                              && p.trangThai == "Còn trống"
                        select new RoomCardViewModel65130650
                        {
                            MaPhong = p.maPhong,
                            SoPhong = p.soPhong,
                            TrangThai = p.trangThai,
                            Tang = p.tang,
                            TenLoaiPhong = lp.tenLoaiPhong,
                            GiaCoBan = lp.giaCoBan,
                            SoNguoiToiDa = lp.soNguoiToiDa,
                            LoaiGiuong = lp.loaiGiuong,
                            DienTichPhong = lp.dienTichPhong,
                            TienNghi = lp.tienNghi,
                            HinhAnh = lp.hinhAnh,
                            MaLoaiPhong = lp.maLoaiPhong
                        };

            return query
                .OrderByDescending(r => r.GiaCoBan)
                .ThenByDescending(r => r.DienTichPhong)
                .ThenByDescending(r => r.SoNguoiToiDa)
                .Take(4)
                .ToList();
        }

        private List<RoomCardViewModel65130650> GetRoomsData(string search, string maLoaiPhong, string trangThai)
        {
            var query = from p in _db.Phongs
                        join lp in _db.LoaiPhongs on p.maLoaiPhong equals lp.maLoaiPhong
                        where (p.trangThaiHoatDong == true || p.trangThaiHoatDong == null)
                              && (lp.trangThaiHoatDong == true || lp.trangThaiHoatDong == null)
                        select new RoomCardViewModel65130650
                        {
                            MaPhong = p.maPhong,
                            SoPhong = p.soPhong,
                            TrangThai = p.trangThai,
                            Tang = p.tang,
                            TenLoaiPhong = lp.tenLoaiPhong,
                            GiaCoBan = lp.giaCoBan,
                            SoNguoiToiDa = lp.soNguoiToiDa,
                            LoaiGiuong = lp.loaiGiuong,
                            DienTichPhong = lp.dienTichPhong,
                            TienNghi = lp.tienNghi,
                            HinhAnh = lp.hinhAnh,
                            MaLoaiPhong = lp.maLoaiPhong
                        };

            if (!string.IsNullOrWhiteSpace(maLoaiPhong))
            {
                query = query.Where(r => r.MaLoaiPhong == maLoaiPhong);
            }

            if (!string.IsNullOrWhiteSpace(trangThai))
            {
                query = query.Where(r => r.TrangThai == trangThai);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();
                query = query.Where(r =>
                    r.TenLoaiPhong.Contains(search) ||
                    r.SoPhong.Contains(search) ||
                    (r.TienNghi != null && r.TienNghi.Contains(search)));
            }

            return query
                .OrderBy(r => r.GiaCoBan)
                .ThenBy(r => r.SoPhong)
                .ToList();
        }

        public ActionResult RoomDetail(string id, string checkIn = null, string checkOut = null)
        {
            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction("Index");
            }

            DateTime? dIn = null;
            DateTime? dOut = null;

            if (!string.IsNullOrEmpty(checkIn) && DateTime.TryParse(checkIn, out DateTime parsedIn)) dIn = parsedIn.Date;
            if (!string.IsNullOrEmpty(checkOut) && DateTime.TryParse(checkOut, out DateTime parsedOut)) dOut = parsedOut.Date;

            if (dIn.HasValue && dOut.HasValue && dIn >= dOut)
            {
                dOut = dIn.Value.AddDays(1);
            }

            var loaiPhong = _db.LoaiPhongs.FirstOrDefault(lp => 
                lp.maLoaiPhong == id && 
                (lp.trangThaiHoatDong == true || lp.trangThaiHoatDong == null));

            if (loaiPhong == null)
            {
                return HttpNotFound("Không tìm thấy loại phòng.");
            }

            int soPhongConTrong = 0;
            if (dIn.HasValue)
            {
                DateTime effectiveOut = dOut ?? dIn.Value.AddDays(1);
                var busyStatuses = new[] { "Đã xác nhận", "Đã nhận phòng", "Chờ xác nhận" };

                var unavailableRoomIds = _db.DatPhongs
                    .Where(dp => busyStatuses.Contains(dp.trangThaiDatPhong))
                    .Where(dp => DbFunctions.TruncateTime(dp.ngayNhanPhong) < effectiveOut && 
                                 DbFunctions.TruncateTime(dp.ngayTraPhong) > dIn.Value)
                    .Select(dp => dp.maPhong)
                    .Distinct()
                    .ToList();

                soPhongConTrong = _db.Phongs.Count(p =>
                    p.maLoaiPhong == loaiPhong.maLoaiPhong &&
                    (p.trangThaiHoatDong == true || p.trangThaiHoatDong == null) &&
                    p.trangThai != "Bảo trì" &&
                    !unavailableRoomIds.Contains(p.maPhong));
            }
            else
            {
                soPhongConTrong = _db.Phongs.Count(p => 
                    p.maLoaiPhong == loaiPhong.maLoaiPhong && 
                    p.trangThai == "Còn trống" &&
                    (p.trangThaiHoatDong == true || p.trangThaiHoatDong == null));
            }

            var tongSoPhong = _db.Phongs.Count(p => 
                p.maLoaiPhong == loaiPhong.maLoaiPhong &&
                (p.trangThaiHoatDong == true || p.trangThaiHoatDong == null));

            var viewModel = new RoomDetailViewModel65130650
            {
                MaLoaiPhong = loaiPhong.maLoaiPhong,
                TenLoaiPhong = loaiPhong.tenLoaiPhong,
                MoTa = loaiPhong.moTa,
                GiaCoBan = loaiPhong.giaCoBan,
                SoNguoiToiDa = loaiPhong.soNguoiToiDa,
                LoaiGiuong = loaiPhong.loaiGiuong,
                DienTichPhong = loaiPhong.dienTichPhong,
                TienNghi = loaiPhong.tienNghi,
                HinhAnh = loaiPhong.hinhAnh,
                SoPhongConTrong = soPhongConTrong,
                TongSoPhong = tongSoPhong,
                CheckIn = dIn,
                CheckOut = dOut
            };

            return View(viewModel);
        }

        [HttpGet]
        public ActionResult GetServices()
        {
            var services = _db.DichVus
                .Where(s => s.trangThaiHoatDong == true || s.trangThaiHoatDong == null)
                .Select(s => new
                {
                    s.maDichVu,
                    s.tenDichVu,
                    s.giaDichVu,
                    s.loaiDichVu,
                    s.hinhAnh
                })
                .ToList();
            return Json(services, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ConfirmBooking(BookingRequestViewModel65130650 model)
        {
            if (Session["UserId"] == null)
            {
                return Json(new { success = false, message = "Vui lòng đăng nhập để đặt phòng." });
            }

            string maKhachHang = Session["UserId"].ToString();

            var busyStatuses = new[] { "Đã xác nhận", "Đã nhận phòng", "Chờ xác nhận" };
            var unavailableRoomIds = _db.DatPhongs
                .Where(dp => busyStatuses.Contains(dp.trangThaiDatPhong))
                .Where(dp => DbFunctions.TruncateTime(dp.ngayNhanPhong) < model.NgayTra &&
                             DbFunctions.TruncateTime(dp.ngayTraPhong) > model.NgayNhan)
                .Select(dp => dp.maPhong)
                .Distinct()
                .ToList();

            var availableRoom = _db.Phongs
                .FirstOrDefault(p => p.maLoaiPhong == model.MaLoaiPhong &&
                                    (p.trangThaiHoatDong == true || p.trangThaiHoatDong == null) &&
                                    p.trangThai != "Bảo trì" &&
                                    !unavailableRoomIds.Contains(p.maPhong));

            if (availableRoom == null)
            {
                return Json(new { success = false, message = "Rất tiếc, loại phòng này vừa hết chỗ trong khoảng thời gian bạn chọn." });
            }

            var roomType = _db.LoaiPhongs.FirstOrDefault(lp => lp.maLoaiPhong == model.MaLoaiPhong);
            if (roomType != null && model.SoKhach > roomType.soNguoiToiDa)
            {
                return Json(new { success = false, message = "Số lượng khách vượt quá sức chứa tối đa của loại phòng này." });
            }

            using (var transaction = _db.Database.BeginTransaction())
            {
                try
                {
                    string maDatPhong = GenerateBookingId().Trim();
                    if (maDatPhong.Length > 5) maDatPhong = maDatPhong.Substring(0, 5);

                    var datPhong = new DatPhong
                    {
                        maDatPhong = maDatPhong,
                        maKhachHang = maKhachHang.Trim(),
                        maPhong = availableRoom.maPhong.Trim(),
                        ngayNhanPhong = model.NgayNhan,
                        ngayTraPhong = model.NgayTra,
                        soKhach = model.SoKhach,
                        tienPhong = model.TienPhong,
                        tienDatCoc = model.TienDatCoc,
                        trangThaiDatPhong = "Chờ xác nhận",
                        yeuCauDacBiet = model.YeuCauDacBiet,
                        ngayDat = DateTime.Now,
                        nguoiTao = maKhachHang.Trim(),
                        ngayCapNhat = DateTime.Now
                    };
                    _db.DatPhongs.Add(datPhong);

                    int svcsProcessed = 0;
                    if (model.SelectedServices != null && model.SelectedServices.Any())
                    {
                        int nextSeq = GetNextServiceSequence();
                        int svcIndex = 0;

                        foreach (var svc in model.SelectedServices)
                        {
                            string cleanSvcId = svc.MaDichVu?.Trim();
                            // Tìm kiếm linh hoạt hơn
                            var serviceInfo = _db.DichVus.FirstOrDefault(s => s.maDichVu.Trim() == cleanSvcId);
                            
                            if (serviceInfo != null)
                            {
                                int seqNum = nextSeq + svcIndex;
                                string maDVD = "DVD" + seqNum.ToString("D2");
                                
                                // Cắt ngắn nếu vượt quá 5 ký tự (DB giới hạn 5)
                                if (maDVD.Length > 5)
                                {
                                    maDVD = "D" + seqNum.ToString();
                                    if (maDVD.Length > 5) maDVD = maDVD.Substring(maDVD.Length - 5);
                                }

                                var dichVuDatPhong = new DichVuDatPhong
                                {
                                    maDichVuDatPhong = maDVD.Trim(),
                                    maDatPhong = maDatPhong,
                                    maDichVu = serviceInfo.maDichVu.Trim(), // Quan trọng: Trim() mã từ bảng gốc
                                    soLuong = svc.SoLuong,
                                    donGia = serviceInfo.giaDichVu,
                                    thanhTien = svc.SoLuong * serviceInfo.giaDichVu,
                                    ngaySuDung = DateTime.Now
                                };
                                _db.DichVuDatPhongs.Add(dichVuDatPhong);
                                svcIndex++;
                                svcsProcessed++;
                            }
                        }
                    }

                    var thanhToan = new ThanhToan
                    {
                        maThanhToan = GeneratePaymentId().Trim(),
                        maDatPhong = maDatPhong,
                        ngayThanhToan = DateTime.Now,
                        soTien = model.TienDatCoc,
                        tienPhong = 0,
                        tienDichVu = 0,
                        giamGia = 0,
                        phuongThucThanhToan = "Chuyển khoản",
                        trangThaiThanhToan = "Thành công",
                        maGiaoDich = "TXN" + DateTime.Now.Ticks,
                        ghiChu = "Thanh toán tiền đặt cọc 50%"
                    };
                    _db.ThanhToans.Add(thanhToan);

                    _db.SaveChanges();
                    transaction.Commit();
                    
                    string finalMsg = "Đặt phòng thành công!";
                    if (svcsProcessed > 0) finalMsg += $" Đã lưu {svcsProcessed} dịch vụ.";
                    
                    return Json(new { success = true, message = finalMsg, maDatPhong = maDatPhong });
                }
                catch (DbEntityValidationException ex)
                {
                    transaction.Rollback();
                    var errorMessages = ex.EntityValidationErrors
                        .SelectMany(x => x.ValidationErrors)
                        .Select(x => $"{x.Entry.Entity.GetType().Name}.{x.PropertyName}: {x.ErrorMessage}");
                    var fullErrorMessage = string.Join(" | ", errorMessages);
                    return Json(new { success = false, message = "Lỗi xác thực: " + fullErrorMessage });
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    string innerMsg = ex.InnerException != null ? " | Gốc: " + ex.InnerException.Message : "";
                    return Json(new { success = false, message = "Lỗi hệ thống: " + ex.Message + innerMsg });
                }
            }
        }

        private int GetNextServiceSequence()
        {
            var allSvcIds = _db.DichVuDatPhongs
                .Where(d => d.maDichVuDatPhong.StartsWith("DVD"))
                .Select(d => d.maDichVuDatPhong)
                .ToList();

            int maxNum = 0;
            foreach (var id in allSvcIds)
            {
                string numericPart = new string(id.Where(char.IsDigit).ToArray());
                if (int.TryParse(numericPart, out int num))
                {
                    if (num > maxNum) maxNum = num;
                }
            }
            
            if (maxNum == 0)
            {
                // Nếu chưa có DVD nào, lấy theo tổng số bản ghi hiện có để nối tiếp chuỗi cũ (STT trong DB)
                return _db.DichVuDatPhongs.Count() + 1;
            }

            return maxNum + 1;
        }

        private string GenerateBookingId()
        {
            var last = _db.DatPhongs.OrderByDescending(d => d.maDatPhong).FirstOrDefault();
            if (last == null) return "DP001";
            string numericPart = new string(last.maDatPhong.Where(char.IsDigit).ToArray());
            if (int.TryParse(numericPart, out int lastNum))
            {
                return "DP" + (lastNum + 1).ToString("D3");
            }
            return "DP" + DateTime.Now.ToString("HHmmss").Substring(0, 3);
        }

        private string GeneratePaymentId()
        {
            var last = _db.ThanhToans.OrderByDescending(t => t.maThanhToan).FirstOrDefault();
            if (last == null) return "TT001";
            string numericPart = new string(last.maThanhToan.Where(char.IsDigit).ToArray());
            if (int.TryParse(numericPart, out int lastNum))
            {
                return "TT" + (lastNum + 1).ToString("D3");
            }
            return "TT" + DateTime.Now.ToString("HHmmss").Substring(0, 3);
        }
    }
}
