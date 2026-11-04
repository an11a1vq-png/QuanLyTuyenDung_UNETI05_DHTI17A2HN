// ==============================================================================
// Họ và tên: Đỗ Thiện An
// Mã sinh viên: 23103100082
// Module: 1 - Tài khoản, Đăng nhập, Phân quyền và Phòng ban
// Nội dung thực hiện: Controller quản lý Đăng nhập, Đăng ký, Đổi mật khẩu, Quên mật khẩu OTP,
// Phân quyền Session (Admin, Nhân sự, Ứng viên), bảo mật băm mật khẩu một chiều BCrypt và chống Brute-force trên RAM SERVER.
// Lưu ý kiến trúc: Các trạng thái tạm thời (Khóa Brute-force & OTP) được lưu hoàn toàn trên RAM SERVER (AccountSecurityMemoryService), KHÔNG sửa đổi Database Entity.
// ==============================================================================

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyTuyenDung_UNETI05_DHTI17A2HN.Data;
using QuanLyTuyenDung_UNETI05_DHTI17A2HN.Helpers;
using QuanLyTuyenDung_UNETI05_DHTI17A2HN.Models;
using QuanLyTuyenDung_UNETI05_DHTI17A2HN.ViewModels;

namespace QuanLyTuyenDung_UNETI05_DHTI17A2HN.Controllers
{
    public class TaiKhoanController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public TaiKhoanController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // ==============================================================================
        // 1. ĐĂNG NHẬP HỆ THỐNG (/TaiKhoan/DangNhap hoặc /TaiKhoan/Login)
        // ==============================================================================
        [HttpGet]
        [Route("TaiKhoan/DangNhap")]
        [Route("TaiKhoan/Login")]
        public IActionResult DangNhap(string? returnUrl)
        {
            if (HttpContext.Session.IsLoggedIn())
            {
                return RedirectToAction("Index", "Home");
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("TaiKhoan/DangNhap")]
        [Route("TaiKhoan/Login")]
        public async Task<IActionResult> DangNhap(LoginViewModel model, string? returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var tenDangNhapTrim = model.TenDangNhap.Trim();

            // 1. Tìm tài khoản theo Tên đăng nhập trong CSDL
            var taiKhoan = await _context.TaiKhoan
                .Include(t => t.UngVien)
                .FirstOrDefaultAsync(t => t.TenDangNhap.ToLower() == tenDangNhapTrim.ToLower());

            if (taiKhoan == null)
            {
                ModelState.AddModelError(string.Empty, "Tên đăng nhập hoặc mật khẩu không chính xác.");
                return View(model);
            }

            // 2. Kiểm tra trạng thái hoạt động vĩnh viễn (Khóa bởi Admin trong CSDL)
            if (!taiKhoan.TrangThai)
            {
                ModelState.AddModelError(string.Empty, "Tài khoản của bạn hiện đang bị khóa bởi Quản trị viên. Vui lòng liên hệ hỗ trợ.");
                return View(model);
            }

            // 3. Cơ chế chống Brute-force: Kiểm tra xem tài khoản có đang bị tạm khóa TRÊN RAM SERVER không
            if (AccountSecurityMemoryService.IsLockedOut(taiKhoan.TenDangNhap, out var lockoutUntil))
            {
                var phutConLai = Math.Ceiling((lockoutUntil - DateTime.Now).TotalMinutes);
                if (phutConLai < 1) phutConLai = 1;

                ModelState.AddModelError(string.Empty, 
                    $"Tài khoản tạm thời bị khóa do nhập sai mật khẩu 5 lần. Vui lòng thử lại sau {phutConLai} phút hoặc dùng chức năng 'Quên mật khẩu'.");
                return View(model);
            }

            // 4. So khớp mật khẩu qua PasswordHelper (BCrypt Salted Hash / tương thích ngược)
            bool isPasswordCorrect = PasswordHelper.VerifyPassword(model.MatKhau, taiKhoan.MatKhau);
            if (!isPasswordCorrect)
            {
                // Ghi nhận số lần nhập sai vào RAM SERVER
                var failedCount = AccountSecurityMemoryService.RecordFailedAttempt(taiKhoan.TenDangNhap, maxAttempts: 5, lockoutMinutes: 15);
                if (failedCount >= 5)
                {
                    ModelState.AddModelError(string.Empty, 
                        "Cảnh báo bảo mật: Bạn đã nhập sai mật khẩu 5 lần liên tiếp. Tài khoản đã bị tạm khóa trong 15 phút trên RAM máy chủ.");
                    return View(model);
                }
                else
                {
                    int soLanConLai = 5 - failedCount;
                    ModelState.AddModelError(string.Empty, 
                        $"Tên đăng nhập hoặc mật khẩu không chính xác. Bạn còn {soLanConLai} lần thử trước khi tài khoản bị khóa tạm thời 15 phút.");
                    return View(model);
                }
            }

            // 5. Đăng nhập đúng: Giải phóng khóa và reset bộ đếm sai trên RAM SERVER
            AccountSecurityMemoryService.ResetLockout(taiKhoan.TenDangNhap);

            // Tự động nâng cấp mã băm BCrypt nếu mật khẩu cũ vẫn đang lưu dạng plaintext
            if (!taiKhoan.MatKhau.StartsWith("$2a$") && !taiKhoan.MatKhau.StartsWith("$2b$") && !taiKhoan.MatKhau.StartsWith("$2y$"))
            {
                taiKhoan.MatKhau = PasswordHelper.HashPassword(model.MatKhau);
                await _context.SaveChangesAsync();
            }

            // 6. Lưu thông tin người dùng vào Session
            HttpContext.Session.SetUserSession(
                taiKhoan.MaTaiKhoan,
                taiKhoan.HoTen,
                taiKhoan.VaiTro,
                taiKhoan.UngVien?.MaUngVien
            );

            TempData["SuccessMessage"] = $"Đăng nhập thành công! Chào mừng {taiKhoan.HoTen} ({taiKhoan.VaiTro}).";

            // 7. Điều hướng ReturnUrl an toàn (Chống Open Redirect Attack)
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            // Phân quyền điều hướng theo vai trò người dùng
            if (taiKhoan.VaiTro == "Admin")
            {
                return RedirectToAction("Index", "ThongKe");
            }
            if (taiKhoan.VaiTro == "Nhân sự")
            {
                return RedirectToAction("DanhSachHoSo", "HoSoUngTuyen");
            }

            return RedirectToAction("Index", "Home");
        }

        // ==============================================================================
        // 2. ĐĂNG KÝ TÀI KHOẢN ỨNG VIÊN (/TaiKhoan/DangKy hoặc /TaiKhoan/Register)
        // ==============================================================================
        [HttpGet]
        [Route("TaiKhoan/DangKy")]
        [Route("TaiKhoan/Register")]
        public IActionResult DangKy()
        {
            if (HttpContext.Session.IsLoggedIn())
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("TaiKhoan/DangKy")]
        [Route("TaiKhoan/Register")]
        public async Task<IActionResult> DangKy(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var tenDangNhapTrim = model.TenDangNhap.Trim().ToLower();
            var emailTrim = model.Email.Trim().ToLower();

            // Kiểm tra trùng Tên đăng nhập bằng LINQ AnyAsync
            if (await _context.TaiKhoan.AnyAsync(t => t.TenDangNhap.ToLower() == tenDangNhapTrim))
            {
                ModelState.AddModelError("TenDangNhap", "Tên đăng nhập này đã được sử dụng. Vui lòng chọn tên khác.");
                return View(model);
            }

            // Kiểm tra trùng Email
            if (await _context.TaiKhoan.AnyAsync(t => t.Email.ToLower() == emailTrim))
            {
                ModelState.AddModelError("Email", "Địa chỉ email này đã được đăng ký tài khoản khác.");
                return View(model);
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Băm mật khẩu người dùng bằng BCrypt + Salt ngẫu nhiên
                var passwordHash = PasswordHelper.HashPassword(model.MatKhau);

                var taiKhoan = new TaiKhoan
                {
                    TenDangNhap = model.TenDangNhap.Trim(),
                    MatKhau = passwordHash,
                    HoTen = model.HoTen.Trim(),
                    Email = model.Email.Trim(),
                    VaiTro = "UngVien",
                    TrangThai = true
                };

                _context.TaiKhoan.Add(taiKhoan);
                await _context.SaveChangesAsync();

                // Tự động tạo bản ghi UngVien liên kết với Tài khoản
                var ungVien = new UngVien
                {
                    MaTaiKhoan = taiKhoan.MaTaiKhoan,
                    HoTen = model.HoTen.Trim(),
                    Email = model.Email.Trim(),
                    SoDienThoai = model.SoDienThoai.Trim()
                };

                _context.UngVien.Add(ungVien);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                TempData["SuccessMessage"] = "Đăng ký tài khoản ứng viên thành công! Vui lòng đăng nhập.";
                return RedirectToAction(nameof(DangNhap));
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                ModelState.AddModelError(string.Empty, "Đã xảy ra lỗi trong quá trình tạo tài khoản. Vui lòng thử lại.");
                return View(model);
            }
        }

        // ==============================================================================
        // 3. ĐỔI MẬT KHẨU CÁ NHÂN (/TaiKhoan/DoiMatKhau)
        // ==============================================================================
        [HttpGet]
        [Route("TaiKhoan/DoiMatKhau")]
        public IActionResult DoiMatKhau()
        {
            if (!HttpContext.Session.IsLoggedIn())
            {
                TempData["ErrorMessage"] = "Vui lòng đăng nhập trước khi thực hiện đổi mật khẩu.";
                return RedirectToAction("DangNhap", new { returnUrl = Url.Action("DoiMatKhau") });
            }

            return View(new DoiMatKhauViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("TaiKhoan/DoiMatKhau")]
        public async Task<IActionResult> DoiMatKhau(DoiMatKhauViewModel model)
        {
            if (!HttpContext.Session.IsLoggedIn())
            {
                return RedirectToAction("DangNhap");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var maTaiKhoan = HttpContext.Session.GetMaTaiKhoan();
            if (!maTaiKhoan.HasValue)
            {
                return RedirectToAction("DangNhap");
            }

            var taiKhoan = await _context.TaiKhoan.FindAsync(maTaiKhoan.Value);
            if (taiKhoan == null)
            {
                return RedirectToAction("DangNhap");
            }

            // Xác thực mật khẩu hiện tại
            if (!PasswordHelper.VerifyPassword(model.MatKhauHienTai, taiKhoan.MatKhau))
            {
                ModelState.AddModelError("MatKhauHienTai", "Mật khẩu hiện tại không chính xác.");
                return View(model);
            }

            // Băm mật khẩu mới bằng BCrypt và lưu vào CSDL
            taiKhoan.MatKhau = PasswordHelper.HashPassword(model.MatKhauMoi);
            await _context.SaveChangesAsync();

            // Reset bộ đếm trên RAM Server
            AccountSecurityMemoryService.ResetLockout(taiKhoan.TenDangNhap);

            TempData["SuccessMessage"] = "Đổi mật khẩu thành công! Mật khẩu mới của bạn đã có hiệu lực.";
            return RedirectToAction("Index", "Home");
        }

        // ==============================================================================
        // 4. QUÊN MẬT KHẨU (BƯỚC 1: GỬI MÃ OTP QUA EMAIL - LƯU TRÊN RAM SERVER)
        // ==============================================================================
        [HttpGet]
        [Route("TaiKhoan/QuenMatKhau")]
        public IActionResult QuenMatKhau()
        {
            if (HttpContext.Session.IsLoggedIn())
            {
                return RedirectToAction("Index", "Home");
            }
            return View(new QuenMatKhauViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("TaiKhoan/QuenMatKhau")]
        public async Task<IActionResult> QuenMatKhau(QuenMatKhauViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var term = model.EmailHoacTenDangNhap.Trim().ToLower();
            var taiKhoan = await _context.TaiKhoan.FirstOrDefaultAsync(t =>
                t.Email.ToLower() == term || t.TenDangNhap.ToLower() == term);

            if (taiKhoan == null)
            {
                ModelState.AddModelError("EmailHoacTenDangNhap", "Không tìm thấy tài khoản phù hợp với thông tin đã nhập.");
                return View(model);
            }

            if (!taiKhoan.TrangThai)
            {
                ModelState.AddModelError("EmailHoacTenDangNhap", "Tài khoản này hiện đang bị khóa bởi Quản trị viên.");
                return View(model);
            }

            // Sinh mã OTP 6 số bảo mật
            var otp = PasswordHelper.GenerateOtp(6);

            // Lưu mã OTP và hạn dùng 15 phút HOÀN TOÀN TRÊN RAM SERVER (Không đụng vào Database Model)
            AccountSecurityMemoryService.SetOtp(taiKhoan.Email, taiKhoan.TenDangNhap, otp, expiryMinutes: 15);
            AccountSecurityMemoryService.SetOtp(taiKhoan.TenDangNhap, taiKhoan.TenDangNhap, otp, expiryMinutes: 15);

            // Gửi email chứa mã OTP
            await EmailHelper.SendPasswordResetOtpAsync(taiKhoan.Email, taiKhoan.HoTen, otp, _configuration);

            TempData["SuccessMessage"] = $"Mã xác thực OTP 6 số đã được gửi tới email '{taiKhoan.Email}'. Mã có hiệu lực trong 15 phút.";
            TempData["OtpDemo"] = otp; // Phục vụ kiểm thử trực tiếp trên phòng máy bảo vệ đồ án

            return RedirectToAction("DatLaiMatKhau", new { email = taiKhoan.Email });
        }

        // ==============================================================================
        // 5. ĐẶT LẠI MẬT KHẨU (BƯỚC 2: XÁC THỰC MÃ OTP TỪ RAM SERVER VÀ ĐỔI MẬT KHẨU)
        // ==============================================================================
        [HttpGet]
        [Route("TaiKhoan/DatLaiMatKhau")]
        public IActionResult DatLaiMatKhau(string? email)
        {
            var model = new DatLaiMatKhauViewModel
            {
                Email = email ?? string.Empty
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("TaiKhoan/DatLaiMatKhau")]
        public async Task<IActionResult> DatLaiMatKhau(DatLaiMatKhauViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var emailTrim = model.Email.Trim().ToLower();

            // 1. Xác thực mã OTP từ RAM SERVER
            if (!AccountSecurityMemoryService.ValidateOtp(emailTrim, model.MaOtp, out var tenDangNhap))
            {
                ModelState.AddModelError("MaOtp", "Mã xác thực OTP không chính xác hoặc đã hết hiệu lực (15 phút). Vui lòng kiểm tra lại.");
                return View(model);
            }

            var taiKhoan = await _context.TaiKhoan.FirstOrDefaultAsync(t => 
                t.TenDangNhap.ToLower() == tenDangNhap.ToLower() || t.Email.ToLower() == emailTrim);

            if (taiKhoan == null)
            {
                ModelState.AddModelError("Email", "Không tìm thấy tài khoản tương ứng.");
                return View(model);
            }

            // 2. Cập nhật mật khẩu mới đã băm BCrypt vào CSDL
            taiKhoan.MatKhau = PasswordHelper.HashPassword(model.MatKhauMoi);
            await _context.SaveChangesAsync();

            // 3. Giải phóng tạm khóa và xóa OTP trên RAM SERVER
            AccountSecurityMemoryService.ResetLockout(taiKhoan.TenDangNhap);
            AccountSecurityMemoryService.RemoveOtp(taiKhoan.TenDangNhap);
            AccountSecurityMemoryService.RemoveOtp(taiKhoan.Email);

            TempData["SuccessMessage"] = "Khôi phục mật khẩu thành công! Bạn có thể đăng nhập ngay với mật khẩu mới.";
            return RedirectToAction("DangNhap");
        }

        // ==============================================================================
        // 6. ĐĂNG XUẤT HỆ THỐNG (/TaiKhoan/DangXuat hoặc /TaiKhoan/Logout)
        // ==============================================================================
        [HttpGet, HttpPost]
        [Route("TaiKhoan/DangXuat")]
        [Route("TaiKhoan/Logout")]
        public IActionResult DangXuat()
        {
            HttpContext.Session.ClearUserSession();
            TempData["SuccessMessage"] = "Bạn đã đăng xuất khỏi hệ thống thành công.";
            return RedirectToAction("DangNhap", "TaiKhoan");
        }

        // ==============================================================================
        // 7. QUẢN TRỊ DANH SÁCH TÀI KHOẢN (/TaiKhoan/DanhSachTaiKhoan hoặc /TaiKhoan/Index)
        // ==============================================================================
        [HttpGet]
        [Route("TaiKhoan/DanhSachTaiKhoan")]
        [Route("TaiKhoan/Index")]
        [Route("TaiKhoan")]
        public async Task<IActionResult> DanhSachTaiKhoan(string? searchString, string? vaiTro)
        {
            // Kiểm tra quyền Admin tại Controller
            if (HttpContext.Session.GetVaiTro() != "Admin")
            {
                return RedirectToAction("AccessDenied", "TaiKhoan");
            }

            var query = _context.TaiKhoan.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                var term = searchString.Trim();
                query = query.Where(t => t.TenDangNhap.Contains(term) ||
                                         t.HoTen.Contains(term) ||
                                         t.Email.Contains(term));
            }

            if (!string.IsNullOrWhiteSpace(vaiTro))
            {
                query = query.Where(t => t.VaiTro == vaiTro);
            }

            ViewBag.CurrentSearch = searchString;
            ViewBag.CurrentRole = vaiTro;
            ViewBag.DanhSachVaiTro = new[] { "Admin", "Nhân sự", "Ứng viên" };

            var list = await query.OrderByDescending(t => t.MaTaiKhoan).ToListAsync();
            return View(list);
        }

        // ==============================================================================
        // 8. KHÓA / MỞ KHÓA VĨNH VIỄN TÀI KHOẢN (BỞI ADMIN)
        // ==============================================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("TaiKhoan/KhoaTaiKhoan")]
        [Route("TaiKhoan/ToggleTrangThai")]
        public async Task<IActionResult> KhoaTaiKhoan(int id)
        {
            if (HttpContext.Session.GetVaiTro() != "Admin")
            {
                return RedirectToAction("AccessDenied", "TaiKhoan");
            }

            var currentAdminId = HttpContext.Session.GetMaTaiKhoan();
            if (id == currentAdminId)
            {
                TempData["ErrorMessage"] = "Bạn không thể tự khóa tài khoản Admin đang đăng nhập.";
                return RedirectToAction("DanhSachTaiKhoan");
            }

            var taiKhoan = await _context.TaiKhoan.FindAsync(id);
            if (taiKhoan == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy tài khoản cần thao tác.";
                return RedirectToAction("DanhSachTaiKhoan");
            }

            taiKhoan.TrangThai = !taiKhoan.TrangThai;
            if (taiKhoan.TrangThai)
            {
                // Mở khóa thì xóa luôn khóa RAM
                AccountSecurityMemoryService.ResetLockout(taiKhoan.TenDangNhap);
            }
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Đã {(taiKhoan.TrangThai ? "mở khóa" : "khóa")} tài khoản '{taiKhoan.TenDangNhap}' thành công.";
            return RedirectToAction("DanhSachTaiKhoan");
        }

        // ==============================================================================
        // 9. MỞ KHÓA BRUTE-FORCE TẠM THỜI TRÊN RAM SERVER (BỞI ADMIN)
        // ==============================================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("TaiKhoan/MoKhoaTamThoi")]
        public async Task<IActionResult> MoKhoaTamThoi(int id)
        {
            if (HttpContext.Session.GetVaiTro() != "Admin")
            {
                return RedirectToAction("AccessDenied", "TaiKhoan");
            }

            var taiKhoan = await _context.TaiKhoan.FindAsync(id);
            if (taiKhoan == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy tài khoản.";
                return RedirectToAction("DanhSachTaiKhoan");
            }

            // Giải phóng tạm khóa trên RAM SERVER
            AccountSecurityMemoryService.ResetLockout(taiKhoan.TenDangNhap);

            TempData["SuccessMessage"] = $"Đã mở khóa đăng nhập tạm thời trên RAM Server cho tài khoản '{taiKhoan.TenDangNhap}'.";
            return RedirectToAction("DanhSachTaiKhoan");
        }

        // ==============================================================================
        // 10. TRANG TỪ CHỐI TRUY CẬP (/TaiKhoan/AccessDenied)
        // ==============================================================================
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
