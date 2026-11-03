// ==============================================================================
// Họ và tên: Đỗ Thiện An
// Mã sinh viên: 23103100082
// Module: 1 - Tài khoản, Đăng nhập, Phân quyền và Phòng ban
// Chức năng: Lưu trữ và quản lý trạng thái bảo mật tạm thời (Chống Brute-force và OTP Quên mật khẩu)
// ĐẶC ĐIỂM: HOÀN TOÀN LƯU TRỮ TRÊN BỘ NHỚ RAM CỦA SERVER (IN-MEMORY CACHE), 
//            KHÔNG TẠO CỘT MỚI TRONG DATABASE, KHÔNG GÂY CONFLICT DATABASE VỚI CÁC THÀNH VIÊN KHÁC.
// ==============================================================================

using System;
using System.Collections.Concurrent;

namespace QuanLyTuyenDung_UNETI05_DHTI17A2HN.Helpers
{
    public static class AccountSecurityMemoryService
    {
        // ==========================================================================
        // 1. CƠ CHẾ CHỐNG BRUTE-FORCE TRÊN RAM SERVER
        // ==========================================================================

        /// <summary>
        /// Bộ đếm số lần nhập sai mật khẩu liên tiếp được lưu trên RAM.
        /// Key: TenDangNhap (chữ thường), Value: Số lần nhập sai.
        /// </summary>
        private static readonly ConcurrentDictionary<string, int> _failedAttempts = new(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Lưu thời điểm tài khoản hết hạn tạm khóa (ví dụ: khóa 15 phút) trên RAM.
        /// Key: TenDangNhap (chữ thường), Value: Thời điểm (DateTime) mở khóa.
        /// </summary>
        private static readonly ConcurrentDictionary<string, DateTime> _lockoutUntil = new(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Kiểm tra xem tài khoản có đang bị tạm khóa Brute-force trên RAM Server không.
        /// </summary>
        public static bool IsLockedOut(string tenDangNhap, out DateTime lockoutUntil)
        {
            if (string.IsNullOrWhiteSpace(tenDangNhap))
            {
                lockoutUntil = DateTime.MinValue;
                return false;
            }

            var key = tenDangNhap.Trim();
            if (_lockoutUntil.TryGetValue(key, out var expireTime))
            {
                if (expireTime > DateTime.Now)
                {
                    lockoutUntil = expireTime;
                    return true;
                }

                // Đã hết thời hạn khóa -> Tự động giải phóng khỏi RAM
                _lockoutUntil.TryRemove(key, out _);
                _failedAttempts.TryRemove(key, out _);
            }

            lockoutUntil = DateTime.MinValue;
            return false;
        }

        /// <summary>
        /// Ghi nhận 1 lần nhập sai mật khẩu vào RAM. 
        /// Nếu đạt ngưỡng maxAttempts (mặc định 5), tự động khóa tài khoản trong lockoutMinutes (mặc định 15 phút).
        /// </summary>
        public static int RecordFailedAttempt(string tenDangNhap, int maxAttempts = 5, int lockoutMinutes = 15)
        {
            if (string.IsNullOrWhiteSpace(tenDangNhap)) return 0;

            var key = tenDangNhap.Trim();
            var count = _failedAttempts.AddOrUpdate(key, 1, (_, current) => current + 1);

            if (count >= maxAttempts)
            {
                var lockoutTime = DateTime.Now.AddMinutes(lockoutMinutes);
                _lockoutUntil[key] = lockoutTime;
            }

            return count;
        }

        /// <summary>
        /// Lấy số lần nhập sai hiện tại trên RAM.
        /// </summary>
        public static int GetFailedAttempts(string tenDangNhap)
        {
            if (string.IsNullOrWhiteSpace(tenDangNhap)) return 0;
            var key = tenDangNhap.Trim();
            return _failedAttempts.TryGetValue(key, out var count) ? count : 0;
        }

        /// <summary>
        /// Lấy thời điểm khóa (nếu còn hiệu lực) từ RAM.
        /// </summary>
        public static DateTime? GetLockoutUntil(string tenDangNhap)
        {
            if (string.IsNullOrWhiteSpace(tenDangNhap)) return null;
            var key = tenDangNhap.Trim();
            if (_lockoutUntil.TryGetValue(key, out var expireTime) && expireTime > DateTime.Now)
            {
                return expireTime;
            }
            return null;
        }

        /// <summary>
        /// Xóa số lần nhập sai và gỡ bỏ khóa trên RAM Server (Dùng khi đăng nhập đúng hoặc Admin giải phóng khóa).
        /// </summary>
        public static void ResetLockout(string tenDangNhap)
        {
            if (string.IsNullOrWhiteSpace(tenDangNhap)) return;
            var key = tenDangNhap.Trim();
            _failedAttempts.TryRemove(key, out _);
            _lockoutUntil.TryRemove(key, out _);
        }

        // ==========================================================================
        // 2. CƠ CHẾ LƯU MÃ OTP KHÔI PHỤC MẬT KHẨU TRÊN RAM SERVER
        // ==========================================================================

        public class OtpRecord
        {
            public string Otp { get; set; } = string.Empty;
            public DateTime ExpireAt { get; set; }
            public string TenDangNhap { get; set; } = string.Empty;
        }

        /// <summary>
        /// Bộ nhớ RAM lưu trữ mã OTP khôi phục mật khẩu.
        /// Key: TenDangNhap hoặc Email (chữ thường), Value: OtpRecord (Mã OTP, Hạn dùng, Username).
        /// </summary>
        private static readonly ConcurrentDictionary<string, OtpRecord> _otpStore = new(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Lưu mã OTP vào RAM Server với thời hạn hiệu lực (mặc định 15 phút).
        /// </summary>
        public static void SetOtp(string keyInput, string tenDangNhap, string otp, int expiryMinutes = 15)
        {
            if (string.IsNullOrWhiteSpace(keyInput) || string.IsNullOrWhiteSpace(otp)) return;

            var record = new OtpRecord
            {
                Otp = otp.Trim(),
                ExpireAt = DateTime.Now.AddMinutes(expiryMinutes),
                TenDangNhap = tenDangNhap.Trim()
            };

            _otpStore[keyInput.Trim()] = record;
        }

        /// <summary>
        /// Xác thực mã OTP từ RAM Server. 
        /// Nếu hợp lệ và chưa hết hạn, trả về true kèm TenDangNhap tương ứng và tự động xóa OTP khỏi RAM.
        /// </summary>
        public static bool ValidateOtp(string keyInput, string otpInput, out string tenDangNhap)
        {
            tenDangNhap = string.Empty;
            if (string.IsNullOrWhiteSpace(keyInput) || string.IsNullOrWhiteSpace(otpInput)) return false;

            var key = keyInput.Trim();
            if (_otpStore.TryGetValue(key, out var record))
            {
                if (record.ExpireAt >= DateTime.Now && record.Otp.Equals(otpInput.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    tenDangNhap = record.TenDangNhap;
                    // Mã OTP chỉ được sử dụng 1 lần -> Xóa khỏi RAM ngay lập tức
                    _otpStore.TryRemove(key, out _);
                    return true;
                }

                // Nếu đã hết hạn -> Dọn dẹp khỏi RAM
                if (record.ExpireAt < DateTime.Now)
                {
                    _otpStore.TryRemove(key, out _);
                }
            }

            return false;
        }

        /// <summary>
        /// Lấy mã OTP còn hiệu lực từ RAM (Dùng để hiển thị trong khung Demo trên giao diện phục vụ chấm bài / thuyết trình).
        /// </summary>
        public static string? GetOtpForDemo(string keyInput)
        {
            if (string.IsNullOrWhiteSpace(keyInput)) return null;
            var key = keyInput.Trim();
            if (_otpStore.TryGetValue(key, out var record) && record.ExpireAt >= DateTime.Now)
            {
                return record.Otp;
            }
            return null;
        }

        /// <summary>
        /// Xóa OTP khỏi RAM Server.
        /// </summary>
        public static void RemoveOtp(string keyInput)
        {
            if (string.IsNullOrWhiteSpace(keyInput)) return;
            _otpStore.TryRemove(keyInput.Trim(), out _);
        }
    }
}
