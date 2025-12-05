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
        public ActionResult Index(string search, string loaiPhong, decimal? minPrice, decimal? maxPrice, int? minCapacity, string sortOrder, int page = 1)
        {
            int pageSize = 6; // Số phòng mỗi trang

            // Query lấy tất cả loại phòng đang hoạt động
            var query = from lp in _db.LoaiPhongs
                        where lp.trangThaiHoatDong == true || lp.trangThaiHoatDong == null
                        select new
                        {
                            LoaiPhong = lp,
                            // Đếm số phòng còn trống của loại phòng này
                            SoPhongConTrong = _db.Phongs.Count(p =>
                                p.maLoaiPhong == lp.maLoaiPhong &&
                                p.trangThai == "Còn trống" &&
                                (p.trangThaiHoatDong == true || p.trangThaiHoatDong == null))
                        };

            // Chỉ lấy loại phòng có ít nhất 1 phòng còn trống
            query = query.Where(x => x.SoPhongConTrong > 0);

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
            IOrderedQueryable<dynamic> orderedQuery;
            switch (sortOrder)
            {
                case "price_asc":
                    orderedQuery = query.OrderBy(x => x.LoaiPhong.giaCoBan);
                    break;
                case "price_desc":
                    orderedQuery = query.OrderByDescending(x => x.LoaiPhong.giaCoBan);
                    break;
                default:
                    orderedQuery = query.OrderByDescending(x => x.SoPhongConTrong)
                                        .ThenByDescending(x => x.LoaiPhong.giaCoBan);
                    break;
            }

            // Lấy dữ liệu cho trang hiện tại
            var rooms = orderedQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList()
                .Select(x => new RoomTypeItemViewModel
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
                    SoPhongConTrong = x.SoPhongConTrong
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
