using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Project_65130650.Models;
using Project_65130650.Models.ViewModels;

namespace Project_65130650.Controllers
{
    public class Room65130650Controller : Controller
    {
        private readonly Model65130650DbContext _db = new Model65130650DbContext();

        // GET: Room
        public ActionResult Index(string search, string loaiPhong, decimal? minPrice, decimal? maxPrice, int? minCapacity, string sortOrder, string status, int page = 1)
        {
            int pageSize = 9; // Số phòng mỗi trang

            // Query lấy tất cả loại phòng đang hoạt động
            var query = from lp in _db.LoaiPhongs
                        where lp.trangThaiHoatDong == true || lp.trangThaiHoatDong == null
                        select new
                        {
                            LoaiPhong = lp,
                            // Đếm số phòng còn trống
                            SoPhongConTrong = _db.Phongs.Count(p =>
                                p.maLoaiPhong == lp.maLoaiPhong &&
                                p.trangThai == "Còn trống" &&
                                (p.trangThaiHoatDong == true || p.trangThaiHoatDong == null)),
                            
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

            // Lọc theo trạng thái
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
                    // Mặc định: hiển thị theo thứ tự tự nhiên trong database
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
                        // Nếu không còn phòng trống
                        // Kiểm tra xem có phải tất cả đều bảo trì không?
                        // Nếu TongSoPhong > 0 và SoPhongBaoTri == TongSoPhong -> Bảo trì
                        // Ngược lại (có phòng Đã đặt/Đang ở) -> Hết phòng
                        if (x.TongSoPhong > 0 && x.SoPhongBaoTri == x.TongSoPhong)
                        {
                            s = "Bảo trì";
                        }
                        else
                        {
                            s = "Hết phòng";
                        }
                    }

                    return new RoomTypeItemViewModel
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

            // Lấy danh sách loại phòng cho filter
            var loaiPhongs = (from lp in _db.LoaiPhongs
                              where lp.trangThaiHoatDong == true || lp.trangThaiHoatDong == null
                              let soPhongTrong = _db.Phongs.Count(p =>
                                  p.maLoaiPhong == lp.maLoaiPhong &&
                                  p.trangThai == "Còn trống" &&
                                  (p.trangThaiHoatDong == true || p.trangThaiHoatDong == null))
                              where soPhongTrong > 0
                              select new LoaiPhongFilterItem
                              {
                                  MaLoaiPhong = lp.maLoaiPhong,
                                  TenLoaiPhong = lp.tenLoaiPhong,
                                  SoPhongConTrong = soPhongTrong
                              }).ToList();

            // Tạo ViewModel
            var viewModel = new RoomListViewModel
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
                LoaiPhongs = loaiPhongs
            };

            return View(viewModel);
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
