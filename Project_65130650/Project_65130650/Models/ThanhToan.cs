namespace Project_65130650.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ThanhToan")]
    public partial class ThanhToan
    {
        [Key]
        [StringLength(5)]
        public string maThanhToan { get; set; }

        [Required]
        [StringLength(5)]
        public string maDatPhong { get; set; }

        public DateTime? ngayThanhToan { get; set; }

        public decimal soTien { get; set; }

        public decimal? tienPhong { get; set; }

        public decimal? tienDichVu { get; set; }

        public decimal? giamGia { get; set; }

        [Required]
        [StringLength(50)]
        public string phuongThucThanhToan { get; set; }

        [StringLength(20)]
        public string trangThaiThanhToan { get; set; }

        [StringLength(100)]
        public string maGiaoDich { get; set; }

        [StringLength(500)]
        public string ghiChu { get; set; }

        [StringLength(5)]
        public string nguoiXuLy { get; set; }

        public virtual DatPhong DatPhong { get; set; }

        public virtual NguoiDung NguoiDung { get; set; }
    }
}
