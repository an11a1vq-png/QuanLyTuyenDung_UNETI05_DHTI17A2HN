// ==============================================================================
// Họ và tên: Đỗ Thiện An
// Mã sinh viên: 23103100082
// Module: 1 - Tài khoản, Đăng nhập, Phân quyền và Phòng ban
// Nội dung thực hiện: Khởi tạo dữ liệu mẫu (Seed Data) và tự động áp dụng Migration vào SQL Server
// ==============================================================================

using Microsoft.EntityFrameworkCore;


namespace QuanLyTuyenDung_UNETI05_DHTI17A2HN.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            // Tự động kiểm tra và chạy Migration / tạo bảng nếu chưa có
            context.Database.Migrate();
        }
    }
}
