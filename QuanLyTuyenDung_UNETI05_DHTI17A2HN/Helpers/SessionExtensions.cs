// ==============================================================================
// Họ và tên: Đỗ Thiện An
// Mã sinh viên: 23103100082
// Module: 1 - Tài khoản, Đăng nhập, Phân quyền và Phòng ban
// Nội dung thực hiện: Helper quản lý Session và phân quyền người dùng
// ==============================================================================

using Microsoft.AspNetCore.Http;

namespace QuanLyTuyenDung_UNETI05_DHTI17A2HN.Helpers
{
    public static class SessionExtensions
    {
        public const string SessionKeyMaTaiKhoan = "MaTaiKhoan";
        public const string SessionKeyHoTen = "HoTen";
        public const string SessionKeyVaiTro = "VaiTro";
        public const string SessionKeyMaUngVien = "MaUngVien";

        public static void SetUserSession(this ISession session, int maTaiKhoan, string hoTen, string vaiTro, int? maUngVien = null)
        {
            session.SetInt32(SessionKeyMaTaiKhoan, maTaiKhoan);
            session.SetString(SessionKeyMaTaiKhoan, maTaiKhoan.ToString());
            session.SetString(SessionKeyHoTen, hoTen ?? string.Empty);
            session.SetString(SessionKeyVaiTro, vaiTro ?? string.Empty);
            if (maUngVien.HasValue)
            {
                session.SetInt32(SessionKeyMaUngVien, maUngVien.Value);
                session.SetString(SessionKeyMaUngVien, maUngVien.Value.ToString());
            }
        }

        public static void ClearUserSession(this ISession session)
        {
            session.Clear();
        }

        public static bool IsLoggedIn(this ISession session)
        {
            return session.GetInt32(SessionKeyMaTaiKhoan).HasValue ||
                   !string.IsNullOrEmpty(session.GetString(SessionKeyMaTaiKhoan));
        }

        public static int? GetMaTaiKhoan(this ISession session)
        {
            var val = session.GetInt32(SessionKeyMaTaiKhoan);
            if (val.HasValue) return val;

            var str = session.GetString(SessionKeyMaTaiKhoan);
            if (int.TryParse(str, out int res)) return res;
            return null;
        }

        public static string? GetHoTen(this ISession session)
        {
            return session.GetString(SessionKeyHoTen);
        }

        public static string? GetVaiTro(this ISession session)
        {
            return session.GetString(SessionKeyVaiTro);
        }

        public static int? GetMaUngVien(this ISession session)
        {
            var val = session.GetInt32(SessionKeyMaUngVien);
            if (val.HasValue) return val;

            var str = session.GetString(SessionKeyMaUngVien);
            if (int.TryParse(str, out int res)) return res;
            return null;
        }
    }
}
