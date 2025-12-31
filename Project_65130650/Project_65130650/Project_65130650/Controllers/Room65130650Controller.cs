using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using Project_65130650.Models;
using Project_65130650.Models.ViewModels;

namespace Project_65130650.Controllers
{
    public class Room65130650Controller : Controller
    {
        // Khởi tạo context database
        private readonly Model65130650DbContext _db = new Model65130650DbContext();

        /// <summary>
        /// GET: Index - Trang danh sách phòng với chức năng tìm kiếm, lọc và phân trang
        /// </summary>
        public ActionResult Index(string search, string loaiPhong, decimal? minPrice, decimal? maxPrice, int? minCapacity, string sortOrder, string status, string checkIn, string checkOut, int page = 1)
        {
            int pageSize = 9; 

            // Chuyển đổi ngày tháng một cách an toàn
            DateTime? dIn = null;
            DateTime? dOut = null;

            if (!string.IsNullOrEmpty(checkIn) && DateTime.TryParse(checkIn, out DateTime parsedIn)) dIn = parsedIn.Date;
            if (!string.IsNullOrEmpty(checkOut) && DateTime.TryParse(checkOut, out DateTime parsedOut)) dOut = parsedOut.Date;

            // Đảm bảo logic ngày hợp lý
            if (dIn.HasValue && dOut.HasValue && dIn >= dOut)
            {
                dOut = dIn.Value.AddDays(1);
            }

            // 1. Xác định danh sách mã phòng không khả dụng nếu có chọn ít nhất ngày nhận phòng
            List<string> unavailableRoomIds = new List<string>();
            if (dIn.HasValue)
            {
                // Nếu khách chỉ chọn ngày nhận, hệ thống giả định khách muốn ở ít nhất 1 đêm 
                // để kiểm tra xem hôm đó phòng có trống hay không.
                DateTime effectiveOut = dOut ?? dIn.Value.AddDays(1);

                // Các trạng thái được coi là đang giữ phòng (Theo Database Project_65130650.sql)
                var busyStatuses = new[] { "Đã xác nhận", "Đã nhận phòng", "Chờ xác nhận" };

                // Logic Overlap: (Ngày nhận cũ < Ngày trả mới) AND (Ngày trả cũ > Ngày nhận mới)
                unavailableRoomIds = _db.DatPhongs
                    .Where(dp => busyStatuses.Contains(dp.trangThaiDatPhong))
                    .Where(dp => DbFunctions.TruncateTime(dp.ngayNhanPhong) < effectiveOut && 
                                 DbFunctions.TruncateTime(dp.ngayTraPhong) > dIn.Value)
                    .Select(dp => dp.maPhong)
                    .Distinct()
                    .ToList();
            }

            // 2. Query lấy thông tin loại phòng và tính toán số lượng phòng trống theo điều kiện ngày
            var query = from lp in _db.LoaiPhongs
                        where lp.trangThaiHoatDong == true || lp.trangThaiHoatDong == null
                        select new
                        {
                            LoaiPhong = lp,
                            // Đếm số phòng còn trống (Kích hoạt khi có ít nhất ngày nhận phòng dIn)
                            SoPhongConTrong = _db.Phongs.Count(p =>
                                p.maLoaiPhong == lp.maLoaiPhong &&
                                (p.trangThaiHoatDong == true || p.trangThaiHoatDong == null) &&
                                p.trangThai != "Bảo trì" && 
                                (dIn.HasValue 
                                    ? !unavailableRoomIds.Contains(p.maPhong) 
                                    : p.trangThai == "Còn trống")),
                            
                            // Đếm số phòng bảo trì
                            SoPhongBaoTri = _db.Phongs.Count(p =>
                                p.maLoaiPhong == lp.maLoaiPhong &&
                                p.trangThai == "Bảo trì" &&
                                (p.trangThaiHoatDong == true || p.trangThaiHoatDong == null)),

                            // Đếm tổng số phòng hoạt động của loại này
                            TongSoPhong = _db.Phongs.Count(p =>
                                p.maLoaiPhong == lp.maLoaiPhong &&
                                (p.trangThaiHoatDong == true || p.trangThaiHoatDong == null))
                        };

            // Lọc theo trạng thái "available/soldout" dựa trên số phòng trống đã tính ở trên
            if (!string.IsNullOrWhiteSpace(status))
            {
                status = status.Trim();
                if (status == "available")
                {
                    query = query.Where(x => x.SoPhongConTrong > 0);
                }
                else if (status == "soldout")
                {
                    query = query.Where(x => x.SoPhongConTrong == 0);
                }
            }

            // Áp dụng bộ lọc tìm kiếm
            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim().ToLower();
                query = query.Where(x =>
                    x.LoaiPhong.tenLoaiPhong.ToLower().Contains(search) ||
                    (x.LoaiPhong.moTa != null && x.LoaiPhong.moTa.ToLower().Contains(search)) ||
                    (x.LoaiPhong.tienNghi != null && x.LoaiPhong.tienNghi.ToLower().Contains(search)));
            }

            // Lọc theo loại phòng
            if (!string.IsNullOrWhiteSpace(loaiPhong))
            {
                query = query.Where(x => x.LoaiPhong.maLoaiPhong == loaiPhong);
            }

            // Lọc theo giá
            if (minPrice.HasValue)
            {
                query = query.Where(x => x.LoaiPhong.giaCoBan >= minPrice.Value);
            }
            if (maxPrice.HasValue)
            {
                query = query.Where(x => x.LoaiPhong.giaCoBan <= maxPrice.Value);
            }

            // Lọc theo sức chứa
            if (minCapacity.HasValue)
            {
                query = query.Where(x => x.LoaiPhong.soNguoiToiDa >= minCapacity.Value);
            }

            // Đếm tổng số items
            int totalItems = query.Count();
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            // Đảm bảo page hợp lệ
            if (page < 1) page = 1;
            if (page > totalPages && totalPages > 0) page = totalPages;

            // Sắp xếp theo sortOrder
            switch (sortOrder)
            {
                case "price_asc":
                    query = query.OrderBy(x => x.LoaiPhong.giaCoBan);
                    break;
                case "price_desc":
                    query = query.OrderByDescending(x => x.LoaiPhong.giaCoBan);
                    break;
                default:
                    query = query.OrderBy(x => x.LoaiPhong.maLoaiPhong);
                    break;
            }

            // Lấy dữ liệu cho trang hiện tại
            var rooms = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList()
                .Select(x => {
                    string s = "Còn trống";
                    if (x.SoPhongConTrong > 0)
                    {
                        s = "Còn trống";
                    }
                    else
                    {
                        if (x.TongSoPhong > 0 && x.SoPhongBaoTri == x.TongSoPhong)
                        {
                            s = "Bảo trì";
                        }
                        else
                        {
                            s = "Hết phòng";
                        }
                    }

                    return new RoomTypeItemViewModel65130650
                    {
                        MaLoaiPhong = x.LoaiPhong.maLoaiPhong,
                        TenLoaiPhong = x.LoaiPhong.tenLoaiPhong,
                        MoTa = x.LoaiPhong.moTa,
                        GiaCoBan = x.LoaiPhong.giaCoBan,
                        SoNguoiToiDa = x.LoaiPhong.soNguoiToiDa,
                        LoaiGiuong = x.LoaiPhong.loaiGiuong,
                        DienTichPhong = x.LoaiPhong.dienTichPhong,
                        TienNghi = x.LoaiPhong.tienNghi,
                        HinhAnh = x.LoaiPhong.hinhAnh,
                        SoPhongConTrong = x.SoPhongConTrong,
                        TrangThaiHienThi = s
                    };
                })
                .ToList();

            // Lấy danh sách loại phòng cho filter (cũng áp dụng logic ngày nếu có)
            var loaiPhongs = (from lp in _db.LoaiPhongs
                              where lp.trangThaiHoatDong == true || lp.trangThaiHoatDong == null
                              let soPhongTrong = _db.Phongs.Count(p =>
                                  p.maLoaiPhong == lp.maLoaiPhong &&
                                  (p.trangThaiHoatDong == true || p.trangThaiHoatDong == null) &&
                                  p.trangThai != "Bảo trì" &&
                                  (dIn.HasValue ? !unavailableRoomIds.Contains(p.maPhong) : p.trangThai == "Còn trống"))
                              where soPhongTrong > 0
                              select new LoaiPhongFilterItem65130650
                              {
                                  MaLoaiPhong = lp.maLoaiPhong,
                                  TenLoaiPhong = lp.tenLoaiPhong,
                                  SoPhongConTrong = soPhongTrong
                              }).ToList();

            // Tạo ViewModel
            var viewModel = new RoomListViewModel65130650
            {
                Rooms = rooms,
                CurrentPage = page,
                TotalPages = totalPages,
                TotalItems = totalItems,
                PageSize = pageSize,
                SearchQuery = search,
                SelectedLoaiPhong = loaiPhong,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                MinCapacity = minCapacity,
                SortOrder = sortOrder,
                Status = status,
                CheckIn = dIn,
                CheckOut = dOut,
                LoaiPhongs = loaiPhongs
            };

            return View(viewModel);
        }

        /// <summary>
        /// Giải phóng tài nguyên database khi controller bị hủy
        /// </summary>
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
