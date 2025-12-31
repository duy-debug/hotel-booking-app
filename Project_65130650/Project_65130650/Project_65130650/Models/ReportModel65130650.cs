using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project_65130650.Models
{
    /// <summary>
    /// Model báo cáo doanh thu theo thời gian
    /// </summary>
    public class RevenueReportModel
    {
        public int Year { get; set; } // Năm
        public int Month { get; set; } // Tháng
        public decimal Revenue { get; set; } // Tổng doanh thu
        public int Count { get; set; } // Số lượng giao dịch
    }

    /// <summary>
    /// Model báo cáo hiệu quả sử dụng dịch vụ
    /// </summary>
    public class ServiceReportModel
    {
        public string ServiceId { get; set; } // Mã dịch vụ
        public decimal Revenue { get; set; } // Doanh thu từ dịch vụ
        public int UsageCount { get; set; } // Số lần sử dụng
        public string ServiceName { get; set; } // Tên dịch vụ
    }

    /// <summary>
    /// Model báo cáo thống kê phòng
    /// </summary>
    public class RoomReportModel
    {
        public string RoomType { get; set; } // Loại phòng
        public int BookingCount { get; set; } // Số lượt đặt
    }

    /// <summary>
    /// Model báo cáo phương thức thanh toán
    /// </summary>
    public class PaymentReportModel
    {
        public string Method { get; set; } // Phương thức thanh toán
        public decimal Total { get; set; } // Tổng tiền
        public int Count { get; set; } // Số lượng giao dịch
    }
}
