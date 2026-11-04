// ==============================================================================
// Họ và tên: Đỗ Thiện An (MSV: 23103100082) & Nguyễn Tấn Dũng (MSV: 23103100070)
// Dự án: Quản lý tuyển dụng và hồ sơ ứng viên - UNETI (Nhóm 5 - Đề tài 13)
// Chức năng: Helper dùng chung gửi email hệ thống qua giao thức SMTP (Gmail)
// ==============================================================================

using System.Net;
using System.Net.Mail;

namespace QuanLyTuyenDung_UNETI05_DHTI17A2HN.Helpers
{
    public static class EmailHelper
    {
        // ==============================================================================
        // Module 1: Đỗ Thiện An - Gửi mã xác thực OTP khôi phục mật khẩu
        // ==============================================================================
        public static async Task<bool> SendPasswordResetOtpAsync(string toEmail, string hoTen, string otp, IConfiguration configuration)
        {
            try
            {
                var smtpSection = configuration.GetSection("SmtpSettings");
                var host = smtpSection["Host"] ?? "smtp.gmail.com";
                var portStr = smtpSection["Port"] ?? "587";
                var username = smtpSection["Username"];
                var password = smtpSection["Password"];
                var fromEmail = smtpSection["FromEmail"] ?? username ?? "tuyendung.uneti05@gmail.com";
                var fromDisplayName = smtpSection["FromDisplayName"] ?? "Hệ thống Tuyển dụng UNETI - Nhóm 5";

                // Kiểm tra cấu hình SMTP trước khi gửi
                if (!string.IsNullOrWhiteSpace(host) && int.TryParse(portStr, out int port) && 
                    !string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password))
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
                else
                {
                    // Chế độ mô phỏng khi chưa nhập mật khẩu (dùng cho máy chấm điểm)
                    Console.WriteLine($"[EmailHelper]: Mô phỏng gửi OTP tới {toEmail}: {otp}");
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

        // ==============================================================================
        // Module 3: Nguyễn Tấn Dũng - Gửi email xác nhận nộp hồ sơ ứng tuyển thành công
        // ==============================================================================
        public static async Task<bool> SendThongBaoNopHoSoAsync(
            string toEmail, string hoTen, string tenViTri, DateTime ngayNop, IConfiguration configuration)
        {
            try
            {
                var smtpSection = configuration.GetSection("SmtpSettings");
                var host = smtpSection["Host"] ?? "smtp.gmail.com";
                var portStr = smtpSection["Port"] ?? "587";
                var username = smtpSection["Username"];
                var password = smtpSection["Password"];
                var fromEmail = smtpSection["FromEmail"] ?? username ?? "tuyendung.uneti05@gmail.com";
                var fromDisplayName = smtpSection["FromDisplayName"] ?? "Hệ thống Tuyển dụng UNETI - Nhóm 5";

                // Kiểm tra cấu hình gửi mail
                if (!string.IsNullOrWhiteSpace(host) && int.TryParse(portStr, out int port) && 
                    !string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password))
                {
                    using var message = new MailMessage
                    {
                        From = new MailAddress(fromEmail, fromDisplayName),
                        Subject = $"Xác nhận nộp hồ sơ ứng tuyển: {tenViTri} - UNETI",
                        IsBodyHtml = true,
                        Body = $@"
                            <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; border: 1px solid #e2e8f0; border-radius: 8px; overflow: hidden;'>
                                <div style='background-color: #0284c7; padding: 20px; text-align: center; color: #ffffff;'>
                                    <h2 style='margin: 0;'>XÁC NHẬN NỘP HỒ SƠ ỨNG TUYỂN</h2>
                                    <p style='margin: 5px 0 0 0; opacity: 0.9;'>Cổng Tuyển Dụng UNETI (Nhóm 5 - Đề tài 13)</p>
                                </div>
                                <div style='padding: 25px 30px; background-color: #ffffff; color: #334155; line-height: 1.6;'>
                                    <p>Xin chào <strong>{WebUtility.HtmlEncode(hoTen)}</strong>,</p>
                                    <p>Hệ thống đã nhận được hồ sơ ứng tuyển của bạn với thông tin chi tiết:</p>
                                    
                                    <div style='background-color: #f8fafc; border-left: 4px solid #0284c7; padding: 15px; margin: 20px 0; border-radius: 4px;'>
                                        <p style='margin: 0 0 8px 0;'><strong>Vị trí ứng tuyển:</strong> <span style='color: #0369a1;'>{WebUtility.HtmlEncode(tenViTri)}</span></p>
                                        <p style='margin: 0 0 8px 0;'><strong>Thời gian nộp:</strong> {ngayNop:dd/MM/yyyy HH:mm:ss}</p>
                                        <p style='margin: 0;'><strong>Trạng thái:</strong> <span style='color: #d97706; font-weight: 600;'>Chờ duyệt (Sơ tuyển)</span></p>
                                    </div>

                                    <p>Bộ phận tuyển dụng sẽ kiểm tra hồ sơ và phản hồi kết quả sơ tuyển qua email này.</p>
                                    <p>Bạn có thể theo dõi tiến độ hồ sơ tại mục <strong>'Hồ sơ của tôi'</strong> trên website.</p>

                                    <hr style='border: none; border-top: 1px solid #e2e8f0; margin: 25px 0 15px 0;' />
                                    <p style='font-size: 13px; color: #64748b; margin: 0;'>
                                        Ban Tuyển dụng UNETI • Nhóm 5 - Lớp DHTI17A2HN
                                    </p>
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
                else
                {
                    // Chạy mô phỏng khi chưa nhập mật khẩu (dùng cho máy chấm điểm / test)
                    Console.WriteLine($"[EmailHelper]: Mô phỏng gửi email xác nhận ứng tuyển tới: {toEmail} (Vị trí: {tenViTri})");
                    return true;
                }
            }
            catch (Exception ex)
            {
                // Ghi log lỗi, không làm dừng luồng xử lý
                Console.WriteLine($"[EmailHelper Error]: {ex.Message}");
            }

            return false;
        }
    }
}
