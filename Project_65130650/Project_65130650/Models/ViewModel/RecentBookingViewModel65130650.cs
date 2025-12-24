using System;

namespace Project_65130650.Models.ViewModel
{
    /// <summary>
    /// ViewModel cho hiển thị thông tin đặt phòng gần đây trên Dashboard
    /// </summary>
    public class RecentBookingViewModel65130650
    {
        public string MaDatPhong { get; set; }
        public string MaKhachHang { get; set; }
        public NguoiDung KhachHang { get; set; }
        public string MaPhong { get; set; }
        public Phong Phong { get; set; }
        public DateTime NgayNhanPhong { get; set; }
        public DateTime NgayTraPhong { get; set; }
        public string TrangThaiDatPhong { get; set; }
        public DateTime? NgayDat { get; set; }
    }
}
