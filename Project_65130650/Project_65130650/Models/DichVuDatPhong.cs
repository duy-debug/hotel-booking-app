namespace Project_65130650.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("DichVuDatPhong")]
    public partial class DichVuDatPhong
    {
        [Key]
        [StringLength(5)]
        public string maDichVuDatPhong { get; set; }

        [Required]
        [StringLength(5)]
        public string maDatPhong { get; set; }

        [Required]
        [StringLength(5)]
        public string maDichVu { get; set; }

        public int? soLuong { get; set; }

        public decimal donGia { get; set; }

        public decimal thanhTien { get; set; }

        public DateTime? ngaySuDung { get; set; }

        [StringLength(500)]
        public string ghiChu { get; set; }

        public virtual DatPhong DatPhong { get; set; }

        public virtual DichVu DichVu { get; set; }
    }
}
