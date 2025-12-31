namespace Project_65130650.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("LoaiPhong")]
    public partial class LoaiPhong
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public LoaiPhong()
        {
            Phongs = new HashSet<Phong>();
        }

        [Key]
        [StringLength(5)]
        public string maLoaiPhong { get; set; }

        [Required]
        [StringLength(50)]
        public string tenLoaiPhong { get; set; }

        [StringLength(500)]
        public string moTa { get; set; }

        public decimal giaCoBan { get; set; }

        public int soNguoiToiDa { get; set; }

        [StringLength(50)]
        public string loaiGiuong { get; set; }

        public decimal? dienTichPhong { get; set; }

        [StringLength(1000)]
        public string tienNghi { get; set; }

        [StringLength(255)]
        public string hinhAnh { get; set; }

        public bool? trangThaiHoatDong { get; set; }

        public DateTime? ngayTao { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Phong> Phongs { get; set; }
    }
}
