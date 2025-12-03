namespace Project_65130650.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("NguoiDung")]
    public partial class NguoiDung
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public NguoiDung()
        {
            DatPhongs = new HashSet<DatPhong>();
            DatPhongs1 = new HashSet<DatPhong>();
            ThanhToans = new HashSet<ThanhToan>();
        }

        [Key]
        [StringLength(5)]
        public string maNguoiDung { get; set; }

        [Required]
        [StringLength(100)]
        public string hoTen { get; set; }

        [Required]
        [StringLength(100)]
        public string email { get; set; }

        [StringLength(20)]
        public string soDienThoai { get; set; }

        [Required]
        [StringLength(255)]
        public string matKhau { get; set; }

        [Required]
        [StringLength(20)]
        public string vaiTro { get; set; }

        [StringLength(255)]
        public string diaChi { get; set; }

        [Column(TypeName = "date")]
        public DateTime? ngaySinh { get; set; }

        [StringLength(10)]
        public string gioiTinh { get; set; }

        public bool? trangThaiHoatDong { get; set; }

        public DateTime? ngayTao { get; set; }

        public DateTime? ngayCapNhat { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DatPhong> DatPhongs { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DatPhong> DatPhongs1 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ThanhToan> ThanhToans { get; set; }
    }
}
