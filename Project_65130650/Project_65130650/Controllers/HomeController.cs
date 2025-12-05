using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Project_65130650.Models;
using Project_65130650.Models.ViewModels;

namespace Project_65130650.Controllers
{
    public class HomeController : Controller
    {
        private readonly Model65130650DbContext _db = new Model65130650DbContext();

        public ActionResult Index()
        {
            // Lấy danh sách phòng nổi bật (Featured Rooms)
            // Ưu tiên phòng còn trống, sắp xếp theo giá cao nhất, lấy 4 phòng
            var featuredRooms = GetFeaturedRooms();

            // Truyền dữ liệu qua ViewBag
            ViewBag.FeaturedRooms = featuredRooms;

            return View();
        }

        // Action trả về Partial View cho AJAX
        public ActionResult RoomsList(string search, string maLoaiPhong, string trangThai)
        {
            var rooms = GetRoomsData(search, maLoaiPhong, trangThai);
            return PartialView("RoomsList", rooms);
        }

        // Hàm lấy 4 phòng tốt nhất (cao cấp nhất) cho trang chủ
        private List<RoomCardViewModel> GetFeaturedRooms()
        {
            // Query lấy tất cả phòng CÒN TRỐNG đang hoạt động từ database
            var query = from p in _db.Phongs
                        join lp in _db.LoaiPhongs on p.maLoaiPhong equals lp.maLoaiPhong
                        where (p.trangThaiHoatDong == true || p.trangThaiHoatDong == null)
                              && (lp.trangThaiHoatDong == true || lp.trangThaiHoatDong == null)
                              && p.trangThai == "Còn trống"  // Chỉ lấy phòng còn trống
                        select new RoomCardViewModel
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

            // Lấy 4 phòng TỐT NHẤT (cao cấp nhất) CÒN TRỐNG từ database:
            // 1. Sắp xếp theo giá cao nhất (phòng cao cấp nhất đứng đầu)
            // 2. Nếu cùng giá, ưu tiên phòng có diện tích lớn hơn
            // 3. Nếu cùng diện tích, ưu tiên phòng có sức chứa nhiều hơn
            // 4. Lấy 4 phòng đầu tiên
            return query
                .OrderByDescending(r => r.GiaCoBan)                    // Giá cao nhất = phòng tốt nhất
                .ThenByDescending(r => r.DienTichPhong)                // Diện tích lớn nhất
                .ThenByDescending(r => r.SoNguoiToiDa)                 // Sức chứa nhiều hơn
                .Take(4)                                               // Lấy 4 phòng tốt nhất
                .ToList();
        }

        // Hàm logic lọc dữ liệu dùng chung (cho trang danh sách phòng)
        private List<RoomCardViewModel> GetRoomsData(string search, string maLoaiPhong, string trangThai)
        {
            var query = from p in _db.Phongs
                        join lp in _db.LoaiPhongs on p.maLoaiPhong equals lp.maLoaiPhong
                        where (p.trangThaiHoatDong == true || p.trangThaiHoatDong == null)
                              && (lp.trangThaiHoatDong == true || lp.trangThaiHoatDong == null)
                        select new RoomCardViewModel
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

        // Action hiển thị chi tiết phòng theo mã loại phòng
        public ActionResult RoomDetail(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction("Index");
            }

            // Lấy thông tin loại phòng
            var loaiPhong = _db.LoaiPhongs.FirstOrDefault(lp => 
                lp.maLoaiPhong == id && 
                (lp.trangThaiHoatDong == true || lp.trangThaiHoatDong == null));

            if (loaiPhong == null)
            {
                return HttpNotFound("Không tìm thấy loại phòng.");
            }

            // Đếm số phòng còn trống của loại phòng này
            var soPhongConTrong = _db.Phongs.Count(p => 
                p.maLoaiPhong == id && 
                p.trangThai == "Còn trống" &&
                (p.trangThaiHoatDong == true || p.trangThaiHoatDong == null));

            // Đếm tổng số phòng của loại phòng này
            var tongSoPhong = _db.Phongs.Count(p => 
                p.maLoaiPhong == id &&
                (p.trangThaiHoatDong == true || p.trangThaiHoatDong == null));

            // Tạo ViewModel
            var viewModel = new RoomDetailViewModel
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
                TongSoPhong = tongSoPhong
            };

            return View(viewModel);
        }
    }
}
