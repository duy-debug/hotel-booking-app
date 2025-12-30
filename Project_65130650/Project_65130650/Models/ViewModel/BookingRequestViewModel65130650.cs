using System;
using System.Collections.Generic;

namespace Project_65130650.Models.ViewModels
{
    public class BookingRequestViewModel65130650
    {
        public string MaLoaiPhong { get; set; }
        public string MaPhong { get; set; }
        public DateTime NgayNhan { get; set; }
        public DateTime NgayTra { get; set; }
        public int SoKhach { get; set; }
        public string YeuCauDacBiet { get; set; }
        public decimal TienPhong { get; set; }
        public decimal TienDatCoc { get; set; }
        public List<ServiceSelectionViewModel65130650> SelectedServices { get; set; }
    }

    public class ServiceSelectionViewModel65130650
    {
        public string MaDichVu { get; set; }
        public int SoLuong { get; set; }
    }
}
