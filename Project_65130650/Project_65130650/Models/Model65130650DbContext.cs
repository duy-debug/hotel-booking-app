using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace Project_65130650.Models
{
    public partial class Model65130650DbContext : DbContext
    {
        public Model65130650DbContext()
            : base("name=Model65130650DbContext")
        {
        }

        public virtual DbSet<DatPhong> DatPhongs { get; set; }
        public virtual DbSet<DichVu> DichVus { get; set; }
        public virtual DbSet<DichVuDatPhong> DichVuDatPhongs { get; set; }
        public virtual DbSet<LoaiPhong> LoaiPhongs { get; set; }
        public virtual DbSet<NguoiDung> NguoiDungs { get; set; }
        public virtual DbSet<Phong> Phongs { get; set; }
        public virtual DbSet<ThanhToan> ThanhToans { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DatPhong>()
                .Property(e => e.maDatPhong)
                .IsUnicode(false);

            modelBuilder.Entity<DatPhong>()
                .Property(e => e.maKhachHang)
                .IsUnicode(false);

            modelBuilder.Entity<DatPhong>()
                .Property(e => e.maPhong)
                .IsUnicode(false);

            modelBuilder.Entity<DatPhong>()
                .Property(e => e.nguoiTao)
                .IsUnicode(false);

            modelBuilder.Entity<DatPhong>()
                .HasMany(e => e.DichVuDatPhongs)
                .WithRequired(e => e.DatPhong)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<DatPhong>()
                .HasMany(e => e.ThanhToans)
                .WithRequired(e => e.DatPhong)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<DichVu>()
                .Property(e => e.maDichVu)
                .IsUnicode(false);

            modelBuilder.Entity<DichVu>()
                .HasMany(e => e.DichVuDatPhongs)
                .WithRequired(e => e.DichVu)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<DichVuDatPhong>()
                .Property(e => e.maDichVuDatPhong)
                .IsUnicode(false);

            modelBuilder.Entity<DichVuDatPhong>()
                .Property(e => e.maDatPhong)
                .IsUnicode(false);

            modelBuilder.Entity<DichVuDatPhong>()
                .Property(e => e.maDichVu)
                .IsUnicode(false);

            modelBuilder.Entity<LoaiPhong>()
                .Property(e => e.maLoaiPhong)
                .IsUnicode(false);

            modelBuilder.Entity<LoaiPhong>()
                .Property(e => e.dienTichPhong)
                .HasPrecision(10, 2);

            modelBuilder.Entity<LoaiPhong>()
                .HasMany(e => e.Phongs)
                .WithRequired(e => e.LoaiPhong)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<NguoiDung>()
                .Property(e => e.maNguoiDung)
                .IsUnicode(false);

            modelBuilder.Entity<NguoiDung>()
                .HasMany(e => e.DatPhongs)
                .WithRequired(e => e.NguoiDung)
                .HasForeignKey(e => e.maKhachHang)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<NguoiDung>()
                .HasMany(e => e.DatPhongs1)
                .WithOptional(e => e.NguoiDung1)
                .HasForeignKey(e => e.nguoiTao);

            modelBuilder.Entity<NguoiDung>()
                .HasMany(e => e.ThanhToans)
                .WithOptional(e => e.NguoiDung)
                .HasForeignKey(e => e.nguoiXuLy);

            modelBuilder.Entity<Phong>()
                .Property(e => e.maPhong)
                .IsUnicode(false);

            modelBuilder.Entity<Phong>()
                .Property(e => e.maLoaiPhong)
                .IsUnicode(false);

            modelBuilder.Entity<Phong>()
                .HasMany(e => e.DatPhongs)
                .WithRequired(e => e.Phong)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ThanhToan>()
                .Property(e => e.maThanhToan)
                .IsUnicode(false);

            modelBuilder.Entity<ThanhToan>()
                .Property(e => e.maDatPhong)
                .IsUnicode(false);

            modelBuilder.Entity<ThanhToan>()
                .Property(e => e.nguoiXuLy)
                .IsUnicode(false);
        }
    }
}
