using System;

namespace Project_65130650.Models.ViewModels
{
    /// <summary>
    /// ViewModel cho việc hiển thị thẻ phòng (Card) trên giao diện danh sách
    /// </summary>
    public class RoomCardViewModel65130650
    {
        public string MaPhong { get; set; }
        public string SoPhong { get; set; }
        public string TrangThai { get; set; }
        public int Tang { get; set; }
        public string TenLoaiPhong { get; set; }
        public decimal GiaCoBan { get; set; }
        public int SoNguoiToiDa { get; set; }
        public string LoaiGiuong { get; set; }
        public decimal? DienTichPhong { get; set; }
        public string TienNghi { get; set; }
        public string HinhAnh { get; set; }
        public string MaLoaiPhong { get; set; }
    }
}
