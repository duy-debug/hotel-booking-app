namespace Project_65130650.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("DichVu")]
    public partial class DichVu
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DichVu()
        {
            DichVuDatPhongs = new HashSet<DichVuDatPhong>();
        }

        [Key]
        [StringLength(5)]
        public string maDichVu { get; set; }

        [Required]
        [StringLength(100)]
        public string tenDichVu { get; set; }

        [StringLength(500)]
        public string moTa { get; set; }

        public decimal giaDichVu { get; set; }

        [StringLength(50)]
        public string loaiDichVu { get; set; }

        public bool? trangThaiHoatDong { get; set; }

        [StringLength(255)]
        public string hinhAnh { get; set; }

        public DateTime? ngayTao { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DichVuDatPhong> DichVuDatPhongs { get; set; }
    }
}
