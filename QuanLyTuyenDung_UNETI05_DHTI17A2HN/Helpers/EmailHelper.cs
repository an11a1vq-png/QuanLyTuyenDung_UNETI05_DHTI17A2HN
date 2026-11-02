// ==============================================================================
// Họ và tên: Đỗ Thiện An
// Mã sinh viên: 23103100082
// Module: 1 - Tài khoản, Đăng nhập, Phân quyền và Phòng ban
// Nội dung thực hiện: Helper gửi email thông báo mã xác thực OTP / Khôi phục mật khẩu
// ==============================================================================

using System.Net;
using System.Net.Mail;

namespace QuanLyTuyenDung_UNETI05_DHTI17A2HN.Helpers
{
    public static class EmailHelper
    {
        public static async Task<bool> SendPasswordResetOtpAsync(string toEmail, string hoTen, string otp, IConfiguration configuration)
        {
            try
            {
                var smtpSection = configuration.GetSection("SmtpSettings");
                var host = smtpSection["Host"];
                var portStr = smtpSection["Port"];
                var username = smtpSection["Username"];
                var password = smtpSection["Password"];
                var fromEmail = smtpSection["FromEmail"] ?? "tuyendung.uneti05@gmail.com";
                var fromDisplayName = smtpSection["FromDisplayName"] ?? "Hệ thống Tuyển dụng UNETI - Nhóm 5";

                // Nếu có cấu hình SMTP thực tế, thực hiện gửi qua SMTP
                if (!string.IsNullOrWhiteSpace(host) && int.TryParse(portStr, out int port) && !string.IsNullOrWhiteSpace(username))
                {
                    using var message = new MailMessage
                    {
                        From = new MailAddress(fromEmail, fromDisplayName),
                        Subject = "Mã xác thực OTP đặt lại mật khẩu - Hệ thống Tuyển dụng UNETI",
                        IsBodyHtml = true,
                        Body = $@"
                            <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; border: 1px solid #e2e8f0; border-radius: 8px; overflow: hidden;'>
                                <div style='background-color: #1A56DB; padding: 20px; text-align: center; color: #ffffff;'>
                                    <h2 style='margin: 0;'>XÁC THỰC ĐẶT LẠI MẬT KHẨU</h2>
                                    <p style='margin: 5px 0 0 0; opacity: 0.8;'>Hệ thống Quản lý Tuyển dụng & Hồ sơ Ứng viên (UNETI - Đề tài 13)</p>
                                </div>
                                <div style='padding: 30px; background-color: #ffffff; color: #334155;'>
                                    <p>Xin chào <strong>{WebUtility.HtmlEncode(hoTen)}</strong>,</p>
                                    <p>Bạn vừa gửi yêu cầu khôi phục mật khẩu cho tài khoản liên kết với email này. Dưới đây là mã xác thực OTP của bạn:</p>
                                    <div style='text-align: center; margin: 25px 0;'>
                                        <span style='display: inline-block; font-size: 32px; font-weight: bold; letter-spacing: 6px; background-color: #EFF6FF; color: #1E40AF; padding: 12px 24px; border-radius: 8px; border: 1px dashed #BFDBFE;'>{otp}</span>
                                    </div>
                                    <p style='color: #dc2626; font-size: 14px;'><strong>Lưu ý:</strong> Mã OTP có hiệu lực trong vòng <strong>15 phút</strong>. Tuyệt đối không chia sẻ mã này cho bất kỳ ai khác.</p>
                                    <p>Nếu bạn không gửi yêu cầu này, vui lòng bỏ qua email hoặc thông báo cho Quản trị viên hệ thống.</p>
                                    <hr style='border: none; border-top: 1px solid #e2e8f0; margin: 20px 0;' />
                                    <p style='font-size: 12px; color: #64748B;'>Trường Đại học Kinh tế - Kỹ thuật Công nghiệp (UNETI) • Nhóm 5 - DHTI17A2HN</p>
                                </div>
                            </div>"
                    };
                    message.To.Add(toEmail);

                    using var smtp = new SmtpClient(host, port)
                    {
                        Credentials = new NetworkCredential(username, password),
                        EnableSsl = true,
                        Timeout = 10000
                    };

                    await smtp.SendMailAsync(message);
                    return true;
                }
            }
            catch (Exception ex)
            {
                // Ghi nhận lỗi nhưng không làm sập luồng nếu máy thi không có mạng ngoài
                Console.WriteLine($"[EmailHelper Error]: {ex.Message}");
            }

            return false;
        }
    }
}
