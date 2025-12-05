using System;
using System.Collections.Generic;

namespace Project_65130650.Models.ViewModels
{
    public class RoomDetailViewModel
    {
        // Thông tin loại phòng
        public string MaLoaiPhong { get; set; }
        public string TenLoaiPhong { get; set; }
        public string MoTa { get; set; }
        public decimal GiaCoBan { get; set; }
        public int SoNguoiToiDa { get; set; }
        public string LoaiGiuong { get; set; }
        public decimal? DienTichPhong { get; set; }
        public string TienNghi { get; set; }
        public string HinhAnh { get; set; }

        // Danh sách tiện nghi (tách từ chuỗi TienNghi)
        public List<string> DanhSachTienNghi
        {
            get
            {
                if (string.IsNullOrEmpty(TienNghi))
                    return new List<string>();

                return new List<string>(TienNghi.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries));
            }
        }

        // Số phòng còn trống của loại phòng này
        public int SoPhongConTrong { get; set; }

        // Tổng số phòng của loại phòng này
        public int TongSoPhong { get; set; }
    }
}
