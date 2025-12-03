namespace Project_65130650.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Phong")]
    public partial class Phong
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Phong()
        {
            DatPhongs = new HashSet<DatPhong>();
        }

        [Key]
        [StringLength(5)]
        public string maPhong { get; set; }

        [Required]
        [StringLength(10)]
        public string soPhong { get; set; }

        [Required]
        [StringLength(5)]
        public string maLoaiPhong { get; set; }

        public int tang { get; set; }

        [Required]
        [StringLength(20)]
        public string trangThai { get; set; }

        [StringLength(500)]
        public string moTa { get; set; }

        public bool? trangThaiHoatDong { get; set; }

        public DateTime? ngayTao { get; set; }

        public DateTime? ngayCapNhat { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DatPhong> DatPhongs { get; set; }

        public virtual LoaiPhong LoaiPhong { get; set; }
    }
}
