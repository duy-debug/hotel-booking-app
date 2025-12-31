namespace Project_65130650.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("DatPhong")]
    public partial class DatPhong
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DatPhong()
        {
            DichVuDatPhongs = new HashSet<DichVuDatPhong>();
            ThanhToans = new HashSet<ThanhToan>();
        }

        [Key]
        [StringLength(5)]
        public string maDatPhong { get; set; }

        [Required]
        [StringLength(5)]
        public string maKhachHang { get; set; }

        [Required]
        [StringLength(5)]
        public string maPhong { get; set; }

        public DateTime ngayNhanPhong { get; set; }

        public DateTime ngayTraPhong { get; set; }

        public int soKhach { get; set; }

        public decimal tienPhong { get; set; }

        public decimal? tienDatCoc { get; set; }

        [Required]
        [StringLength(20)]
        public string trangThaiDatPhong { get; set; }

        [StringLength(1000)]
        public string yeuCauDacBiet { get; set; }

        public DateTime? ngayDat { get; set; }

        [StringLength(5)]
        public string nguoiTao { get; set; }

        public DateTime? ngayCapNhat { get; set; }

        [StringLength(500)]
        public string lyDoHuy { get; set; }

        public DateTime? ngayHuy { get; set; }

        public virtual NguoiDung NguoiDung { get; set; }

        public virtual Phong Phong { get; set; }

        public virtual NguoiDung NguoiDung1 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DichVuDatPhong> DichVuDatPhongs { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ThanhToan> ThanhToans { get; set; }
    }
}
