// ==============================================================================
// Họ và tên: Đỗ Thiện An
// Mã sinh viên: 23103100082
// Module: 1 - Tài khoản, Đăng nhập, Phân quyền và Phòng ban
// Nội dung thực hiện: Helper băm mật khẩu một chiều (BCrypt with Salt), kiểm tra mật khẩu và sinh mã OTP bảo mật
// ==============================================================================

using System.Security.Cryptography;

namespace QuanLyTuyenDung_UNETI05_DHTI17A2HN.Helpers
{
    public static class PasswordHelper
    {
        /// <summary>
        /// Băm mật khẩu bằng thuật toán BCrypt kèm Salt ngẫu nhiên một chiều
        /// </summary>
        public static string HashPassword(string plainPassword)
        {
            if (string.IsNullOrEmpty(plainPassword))
                return string.Empty;

            // Sử dụng BCrypt.Net với workFactor 11 (2^11 vòng tính toán)
            return BCrypt.Net.BCrypt.HashPassword(plainPassword, workFactor: 11);
        }

        /// <summary>
        /// So khớp mật khẩu nhập vào với chuỗi băm trong CSDL.
        /// Hỗ trợ tương thích ngược với mật khẩu chuỗi thuần chưa mã hóa (legacy plaintext)
        /// </summary>
        public static bool VerifyPassword(string inputPassword, string storedHash)
        {
            if (string.IsNullOrEmpty(storedHash) || string.IsNullOrEmpty(inputPassword))
                return false;

            // Nếu chuỗi lưu trữ là định dạng băm chuẩn của BCrypt ($2a$, $2b$, hoặc $2y$)
            if (storedHash.StartsWith("$2a$") || storedHash.StartsWith("$2b$") || storedHash.StartsWith("$2y$"))
            {
                try
                {
                    return BCrypt.Net.BCrypt.Verify(inputPassword, storedHash);
                }
                catch
                {
                    return false;
                }
            }

            // Tương thích ngược: nếu CSDL cũ lưu chuỗi thuần (ví dụ "123" từ Seed Data)
            return inputPassword == storedHash;
        }

        /// <summary>
        /// Sinh mã số xác thực OTP ngẫu nhiên bằng bộ sinh số ngẫu nhiên mật mã (CSPRNG)
        /// </summary>
        public static string GenerateOtp(int length = 6)
        {
            int min = (int)Math.Pow(10, length - 1);
            int max = (int)Math.Pow(10, length) - 1;
            return RandomNumberGenerator.GetInt32(min, max + 1).ToString();
        }
    }
}
