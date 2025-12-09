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

        // Danh sách hình ảnh (tách từ chuỗi HinhAnh)
        public List<string> DanhSachHinhAnh
        {
            get
            {
                if (string.IsNullOrEmpty(HinhAnh))
                    return new List<string> { "~/Images/phonghangsang.jpeg" }; // Default image

                var images = new List<string>(HinhAnh.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries));
                var normalizedImages = new List<string>();
                
                // Normalize each image path
                foreach (var img in images)
                {
                    var trimmedImg = img.Trim();
                    
                    // Nếu đường dẫn đã có ~/ thì giữ nguyên
                    if (trimmedImg.StartsWith("~/"))
                    {
                        normalizedImages.Add(trimmedImg);
                    }
                    // Nếu đường dẫn bắt đầu bằng Images/ thì thêm ~/
                    else if (trimmedImg.StartsWith("Images/"))
                    {
                        normalizedImages.Add("~/" + trimmedImg);
                    }
                    // Nếu chỉ có tên file thì thêm ~/Images/
                    else if (!trimmedImg.Contains("/") && !trimmedImg.Contains("\\"))
                    {
                        normalizedImages.Add("~/Images/" + trimmedImg);
                    }
                    // Trường hợp khác giữ nguyên
                    else
                    {
                        normalizedImages.Add(trimmedImg);
                    }
                }
                
                return normalizedImages.Count > 0 ? normalizedImages : new List<string> { "~/Images/phonghangsang.jpeg" };
            }
        }

        // Số phòng còn trống của loại phòng này
        public int SoPhongConTrong { get; set; }

        // Tổng số phòng của loại phòng này
        public int TongSoPhong { get; set; }
    }
}
