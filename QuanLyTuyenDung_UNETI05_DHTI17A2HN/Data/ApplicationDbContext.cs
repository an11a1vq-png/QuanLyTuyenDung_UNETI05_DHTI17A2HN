// ==============================================================================
// Họ và tên: Đỗ Thiện An
// Mã sinh viên: 23103100082
// Module: 1 - Tài khoản, Đăng nhập, Phân quyền và Phòng ban
// Nội dung thực hiện: Cấu hình DbContext, khai báo 7 DbSet và quan hệ Entity Framework Core
// ==============================================================================

using Microsoft.EntityFrameworkCore;
using QuanLyTuyenDung_UNETI05_DHTI17A2HN.Models;

namespace QuanLyTuyenDung_UNETI05_DHTI17A2HN.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<TaiKhoan> TaiKhoan { get; set; } = null!;
        public DbSet<PhongBan> PhongBan { get; set; } = null!;
        public DbSet<ViTriTuyenDung> ViTriTuyenDung { get; set; } = null!;
        public DbSet<UngVien> UngVien { get; set; } = null!;
        public DbSet<HoSoUngTuyen> HoSoUngTuyen { get; set; } = null!;
        public DbSet<LichPhongVan> LichPhongVan { get; set; } = null!;
        public DbSet<KetQuaTuyenDung> KetQuaTuyenDung { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Cấu hình Unique Index
            modelBuilder.Entity<TaiKhoan>()
                .HasIndex(t => t.TenDangNhap)
                .IsUnique();

            modelBuilder.Entity<PhongBan>()
                .HasIndex(p => p.TenPhongBan)
                .IsUnique();

            // Quan hệ TaiKhoan - UngVien (1 - 0..1)
            modelBuilder.Entity<TaiKhoan>()
                .HasOne(t => t.UngVien)
                .WithOne(u => u.TaiKhoan)
                .HasForeignKey<UngVien>(u => u.MaTaiKhoan)
                .OnDelete(DeleteBehavior.Cascade);

            // Quan hệ PhongBan - ViTriTuyenDung (1 - n) (Restrict để không xóa mất dữ liệu khi còn vị trí)
            modelBuilder.Entity<ViTriTuyenDung>()
                .HasOne(v => v.PhongBan)
                .WithMany(p => p.DanhSachViTri)
                .HasForeignKey(v => v.MaPhongBan)
                .OnDelete(DeleteBehavior.Restrict);

            // Quan hệ UngVien - HoSoUngTuyen (1 - n)
            modelBuilder.Entity<HoSoUngTuyen>()
                .HasOne(h => h.UngVien)
                .WithMany(u => u.HoSoUngTuyens)
                .HasForeignKey(h => h.MaUngVien)
                .OnDelete(DeleteBehavior.Restrict);

            // Quan hệ ViTriTuyenDung - HoSoUngTuyen (1 - n)
            modelBuilder.Entity<HoSoUngTuyen>()
                .HasOne(h => h.ViTriTuyenDung)
                .WithMany(v => v.HoSoUngTuyens)
                .HasForeignKey(h => h.MaViTri)
                .OnDelete(DeleteBehavior.Restrict);

            // Quan hệ HoSoUngTuyen - LichPhongVan (1 - n)
            modelBuilder.Entity<LichPhongVan>()
                .HasOne(l => l.HoSoUngTuyen)
                .WithMany(h => h.LichPhongVans)
                .HasForeignKey(l => l.MaHoSo)
                .OnDelete(DeleteBehavior.Cascade);

            // Quan hệ HoSoUngTuyen - KetQuaTuyenDung (1 - 0..1)
            modelBuilder.Entity<HoSoUngTuyen>()
                .HasOne(h => h.KetQuaTuyenDung)
                .WithOne(k => k.HoSoUngTuyen)
                .HasForeignKey<KetQuaTuyenDung>(k => k.MaHoSo)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.SeedData();
        }
    }
}
