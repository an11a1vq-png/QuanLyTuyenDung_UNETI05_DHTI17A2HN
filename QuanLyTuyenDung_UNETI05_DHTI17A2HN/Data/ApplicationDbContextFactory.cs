// ==============================================================================
// Họ và tên: Đỗ Thiện An
// Mã sinh viên: 23103100082
// Module: 1 - Tài khoản, Đăng nhập, Phân quyền và Phòng ban
// Nội dung thực hiện: Factory hỗ trợ Design-time Migration kết nối tương đối App_Data
// ==============================================================================

using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace QuanLyTuyenDung_UNETI05_DHTI17A2HN.Data
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var projectDir = Directory.GetCurrentDirectory();
            var appDataDir = Path.Combine(projectDir, "App_Data");
            if (!Directory.Exists(appDataDir))
            {
                Directory.CreateDirectory(appDataDir);
            }

            var mdfPath = Path.Combine(appDataDir, "QuanLyTuyenDung_UNETI05_DHTI17A2HN.mdf");
            var connStr = $@"Server=(localdb)\mssqllocaldb;AttachDbFilename={mdfPath};Database=QuanLyTuyenDung_UNETI05_DHTI17A2HN;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True";

            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer(connStr);

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}
