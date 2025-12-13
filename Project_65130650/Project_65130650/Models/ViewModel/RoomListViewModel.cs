using System;
using System.Collections.Generic;

namespace Project_65130650.Models.ViewModels
{
    public class RoomListViewModel
    {
        // Danh sách phòng (hiển thị theo loại phòng)
        public List<RoomTypeItemViewModel> Rooms { get; set; }

        // Thông tin phân trang
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int TotalItems { get; set; }
        public int PageSize { get; set; }

        // Thông tin tìm kiếm và lọc
        public string SearchQuery { get; set; }
        public string SelectedLoaiPhong { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int? MinCapacity { get; set; }
        public string SortOrder { get; set; } // "price_asc", "price_desc"
        public string Status { get; set; } // "available", "soldout"

        // Danh sách loại phòng cho dropdown filter
        public List<LoaiPhongFilterItem> LoaiPhongs { get; set; }

        public RoomListViewModel()
        {
            Rooms = new List<RoomTypeItemViewModel>();
            LoaiPhongs = new List<LoaiPhongFilterItem>();
            CurrentPage = 1;
            PageSize = 6;
        }

        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;
    }

    public class RoomTypeItemViewModel
    {
        public string MaLoaiPhong { get; set; }
        public string TenLoaiPhong { get; set; }
        public string MoTa { get; set; }
        public decimal GiaCoBan { get; set; }
        public int SoNguoiToiDa { get; set; }
        public string LoaiGiuong { get; set; }
        public decimal? DienTichPhong { get; set; }
        public string TienNghi { get; set; }
        public string HinhAnh { get; set; }

        // Số phòng còn trống
        public int SoPhongConTrong { get; set; }

        // Trạng thái hiển thị (Còn trống, Hết phòng, Bảo trì)
        public string TrangThaiHienThi { get; set; }
    }

    public class LoaiPhongFilterItem
    {
        public string MaLoaiPhong { get; set; }
        public string TenLoaiPhong { get; set; }
        public int SoPhongConTrong { get; set; }
    }
}
