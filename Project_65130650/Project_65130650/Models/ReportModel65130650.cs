using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project_65130650.Models
{
    public class RevenueReportModel
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public decimal Revenue { get; set; }
        public int Count { get; set; }
    }

    public class ServiceReportModel
    {
        public string ServiceId { get; set; }
        public decimal Revenue { get; set; }
        public int UsageCount { get; set; }
        public string ServiceName { get; set; }
    }

    public class RoomReportModel
    {
        public string RoomType { get; set; }
        public int BookingCount { get; set; }
    }

    public class PaymentReportModel
    {
        public string Method { get; set; }
        public decimal Total { get; set; }
        public int Count { get; set; }
    }
}
