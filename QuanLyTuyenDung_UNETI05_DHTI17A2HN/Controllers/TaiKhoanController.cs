// ==============================================================================
// Họ và tên: Đỗ Thiện An
// Mã sinh viên: 23103100082
// Module: 1 - Tài khoản, Đăng nhập, Phân quyền và Phòng ban
// Nội dung thực hiện: Nghiệp vụ Đăng nhập, Đăng ký, Đăng xuất, Phân quyền Session,
// Quản lý danh sách tài khoản, Băm mật khẩu BCrypt, Chống Brute-force & Quên mật khẩu qua OTP
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
        // Áp dụng: Băm mật khẩu BCrypt, Chống tấn công Brute-force (Lockout 15 phút)
        // ==============================================================================
        [HttpGet]
        [Route("TaiKhoan/DangNhap")]
        [Route("TaiKhoan/Login")]
        public IActionResult DangNhap(string? returnUrl = null)
        {
            if (HttpContext.Session.IsLoggedIn())
            {
                return RedirectToAction("Index", "Home");
            }

            var model = new LoginViewModel
            {
                ReturnUrl = returnUrl
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("TaiKhoan/DangNhap")]
        [Route("TaiKhoan/Login")]
        public async Task<IActionResult> DangNhap(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // 1. Tìm tài khoản theo Tên đăng nhập (không phân biệt hoa/thường)
            var taiKhoan = await _context.TaiKhoan
                .Include(t => t.UngVien)
                .FirstOrDefaultAsync(t => t.TenDangNhap.ToLower() == model.TenDangNhap.Trim().ToLower());

            if (taiKhoan == null)
            {
                ModelState.AddModelError(string.Empty, "Tên đăng nhập hoặc mật khẩu không chính xác.");
                return View(model);
            }

            // 2. Kiểm tra trạng thái hoạt động vĩnh viễn (Khóa bởi Admin)
            if (!taiKhoan.TrangThai)
            {
                ModelState.AddModelError(string.Empty, "Tài khoản của bạn hiện đang bị khóa bởi Quản trị viên. Vui lòng liên hệ hỗ trợ.");
                return View(model);
            }

            // 3. Cơ chế chống Brute-force: Kiểm tra xem tài khoản có đang bị tạm khóa không
            if (taiKhoan.KhoaDenThoiDiem.HasValue)
            {
                if (taiKhoan.KhoaDenThoiDiem.Value > DateTime.Now)
                {
                    var phutConLai = Math.Ceiling((taiKhoan.KhoaDenThoiDiem.Value - DateTime.Now).TotalMinutes);
                    ModelState.AddModelError(string.Empty, 
                        $"Tài khoản tạm thời bị khóa do nhập sai mật khẩu 5 lần. Vui lòng thử lại sau {phutConLai} phút hoặc dùng chức năng 'Quên mật khẩu'.");
                    return View(model);
                }
                else
                {
                    // Thời hạn khóa đã kết thúc -> Tự động giải phóng khóa
                    taiKhoan.KhoaDenThoiDiem = null;
                    taiKhoan.SoLanDangNhapSai = 0;
                }
            }

            // 4. So khớp mật khẩu qua PasswordHelper (BCrypt Salted Hash / tương thích ngược)
            bool isPasswordCorrect = PasswordHelper.VerifyPassword(model.MatKhau, taiKhoan.MatKhau);
            if (!isPasswordCorrect)
            {
                taiKhoan.SoLanDangNhapSai++;
                if (taiKhoan.SoLanDangNhapSai >= 5)
                {
                    // Khóa tài khoản tạm thời trong 15 phút
                    taiKhoan.KhoaDenThoiDiem = DateTime.Now.AddMinutes(15);
                    await _context.SaveChangesAsync();

                    ModelState.AddModelError(string.Empty, 
                        "Cảnh báo bảo mật: Bạn đã nhập sai mật khẩu 5 lần liên tiếp. Tài khoản đã bị tạm khóa trong 15 phút.");
                    return View(model);
                }
                else
                {
                    await _context.SaveChangesAsync();
                    int soLanConLai = 5 - taiKhoan.SoLanDangNhapSai;
                    ModelState.AddModelError(string.Empty, 
                        $"Tên đăng nhập hoặc mật khẩu không chính xác. Bạn còn {soLanConLai} lần thử trước khi tài khoản bị khóa tạm thời.");
                    return View(model);
                }
            }

            // 5. Đăng nhập đúng: Reset bộ đếm số lần sai và thời điểm khóa
            taiKhoan.SoLanDangNhapSai = 0;
            taiKhoan.KhoaDenThoiDiem = null;

            // Tự động nâng cấp mã băm BCrypt nếu mật khẩu cũ vẫn đang lưu dạng plaintext
            if (!taiKhoan.MatKhau.StartsWith("$2a$") && !taiKhoan.MatKhau.StartsWith("$2b$") && !taiKhoan.MatKhau.StartsWith("$2y$"))
            {
                taiKhoan.MatKhau = PasswordHelper.HashPassword(model.MatKhau);
            }
            await _context.SaveChangesAsync();

            // 6. Lưu thông tin người dùng vào Session
            HttpContext.Session.SetUserSession(
                taiKhoan.MaTaiKhoan,
                taiKhoan.HoTen,
                taiKhoan.VaiTro,
                taiKhoan.UngVien?.MaUngVien
            );

            TempData["SuccessMessage"] = $"Đăng nhập thành công! Xin chào {taiKhoan.HoTen} ({taiKhoan.VaiTro}).";

            if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
            {
                return Redirect(model.ReturnUrl);
            }

            // Điều hướng theo vai trò nghiệp vụ
            if (taiKhoan.VaiTro == "Admin")
            {
                return RedirectToAction("DanhSachTaiKhoan", "TaiKhoan");
            }
            if (taiKhoan.VaiTro == "Nhân sự")
            {
                return RedirectToAction("DanhSachPhongBan", "PhongBan");
            }

            return RedirectToAction("Index", "Home");
        }

        // ==============================================================================
        // 2. ĐĂNG KÝ TÀI KHOẢN ỨNG VIÊN (/TaiKhoan/DangKy hoặc /TaiKhoan/Register)
        // Áp dụng: Mã hóa băm mật khẩu một chiều BCrypt khi lưu CSDL
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
            return View(new RegisterViewModel());
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

            // 1. Kiểm tra trùng tên đăng nhập bằng LINQ
            var isUsernameExists = await _context.TaiKhoan.AnyAsync(t => t.TenDangNhap.ToLower() == model.TenDangNhap.Trim().ToLower());
            if (isUsernameExists)
            {
                ModelState.AddModelError("TenDangNhap", "Tên đăng nhập này đã tồn tại trong hệ thống. Vui lòng chọn tên khác.");
                return View(model);
            }

            // 2. Kiểm tra trùng email bằng LINQ
            var isEmailExists = await _context.TaiKhoan.AnyAsync(t => t.Email.ToLower() == model.Email.Trim().ToLower());
            if (isEmailExists)
            {
                ModelState.AddModelError("Email", "Email này đã được sử dụng. Vui lòng sử dụng địa chỉ email khác.");
                return View(model);
            }

            // 3. Tạo tài khoản người dùng với vai trò "Ứng viên" và mật khẩu được băm BCrypt
            var taiKhoan = new TaiKhoan
            {
                TenDangNhap = model.TenDangNhap.Trim(),
                MatKhau = PasswordHelper.HashPassword(model.MatKhau),
                HoTen = model.HoTen.Trim(),
                Email = model.Email.Trim(),
                VaiTro = "Ứng viên",
                TrangThai = true,
                SoLanDangNhapSai = 0,
                KhoaDenThoiDiem = null
            };

            _context.TaiKhoan.Add(taiKhoan);
            await _context.SaveChangesAsync();

            // 4. Tạo hồ sơ Ứng viên rỗng tương ứng liên kết với MaTaiKhoan
            var ungVien = new UngVien
            {
                MaTaiKhoan = taiKhoan.MaTaiKhoan,
                HoTen = model.HoTen.Trim(),
                Email = model.Email.Trim(),
                SoDienThoai = model.SoDienThoai.Trim(),
                TrangThai = true
            };

            _context.UngVien.Add(ungVien);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Đăng ký tài khoản ứng viên thành công! Vui lòng đăng nhập để bắt đầu ứng tuyển.";
            return RedirectToAction("DangNhap", "TaiKhoan");
        }

        // ==============================================================================
        // 3. ĐỔI MẬT KHẨU TÀI KHOẢN (/TaiKhoan/DoiMatKhau)
        // Dành cho mọi người dùng đã đăng nhập (Admin, Nhân sự, Ứng viên)
        // ==============================================================================
        [HttpGet]
        [Route("TaiKhoan/DoiMatKhau")]
        public IActionResult DoiMatKhau()
        {
            if (!HttpContext.Session.IsLoggedIn())
            {
                return RedirectToAction("DangNhap", new { returnUrl = "/TaiKhoan/DoiMatKhau" });
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
                return RedirectToAction("DangXuat");
            }

            var taiKhoan = await _context.TaiKhoan.FindAsync(maTaiKhoan.Value);
            if (taiKhoan == null)
            {
                return RedirectToAction("DangXuat");
            }

            // 1. Kiểm tra mật khẩu hiện tại
            if (!PasswordHelper.VerifyPassword(model.MatKhauHienTai, taiKhoan.MatKhau))
            {
                ModelState.AddModelError("MatKhauHienTai", "Mật khẩu hiện tại không chính xác.");
                return View(model);
            }

            // 2. Cập nhật mật khẩu mới băm BCrypt
            taiKhoan.MatKhau = PasswordHelper.HashPassword(model.MatKhauMoi);
            taiKhoan.SoLanDangNhapSai = 0;
            taiKhoan.KhoaDenThoiDiem = null;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Đổi mật khẩu thành công! Mật khẩu mới đã được cập nhật.";
            return RedirectToAction("Index", "Home");
        }

        // ==============================================================================
        // 4. QUÊN MẬT KHẨU QUA MÃ OTP (BƯỚC 1: NHẬP EMAIL / TÊN ĐĂNG NHẬP)
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
            taiKhoan.MaXacThucKhoiPhuc = otp;
            taiKhoan.ThoiHanMaXacThuc = DateTime.Now.AddMinutes(15);
            await _context.SaveChangesAsync();

            // Gửi email chứa mã OTP
            await EmailHelper.SendPasswordResetOtpAsync(taiKhoan.Email, taiKhoan.HoTen, otp, _configuration);

            TempData["SuccessMessage"] = $"Mã xác thực OTP 6 số đã được gửi tới email '{taiKhoan.Email}'. Mã có hiệu lực trong 15 phút.";
            TempData["OtpDemo"] = otp; // Phục vụ kiểm thử trực tiếp trên phòng máy bảo vệ đồ án

            return RedirectToAction("DatLaiMatKhau", new { email = taiKhoan.Email });
        }

        // ==============================================================================
        // 5. ĐẶT LẠI MẬT KHẨU (BƯỚC 2: NHẬP MÃ OTP VÀ MẬT KHẨU MỚI)
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
            var taiKhoan = await _context.TaiKhoan.FirstOrDefaultAsync(t => t.Email.ToLower() == emailTrim);
            if (taiKhoan == null)
            {
                ModelState.AddModelError("Email", "Không tìm thấy tài khoản tương ứng với email này.");
                return View(model);
            }

            // 1. Kiểm tra thời hạn mã OTP
            if (!taiKhoan.ThoiHanMaXacThuc.HasValue || taiKhoan.ThoiHanMaXacThuc.Value < DateTime.Now)
            {
                ModelState.AddModelError("MaOtp", "Mã xác thực OTP đã hết hiệu lực (quá 15 phút). Vui lòng yêu cầu mã mới.");
                return View(model);
            }

            // 2. Kiểm tra tính chính xác của mã OTP
            if (string.IsNullOrWhiteSpace(taiKhoan.MaXacThucKhoiPhuc) || taiKhoan.MaXacThucKhoiPhuc != model.MaOtp.Trim())
            {
                ModelState.AddModelError("MaOtp", "Mã xác thực OTP không chính xác. Vui lòng kiểm tra lại.");
                return View(model);
            }

            // 3. Cập nhật mật khẩu mới đã băm BCrypt, xóa OTP và mở khóa tài khoản
            taiKhoan.MatKhau = PasswordHelper.HashPassword(model.MatKhauMoi);
            taiKhoan.MaXacThucKhoiPhuc = null;
            taiKhoan.ThoiHanMaXacThuc = null;
            taiKhoan.SoLanDangNhapSai = 0;
            taiKhoan.KhoaDenThoiDiem = null;
            await _context.SaveChangesAsync();

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
                taiKhoan.SoLanDangNhapSai = 0;
                taiKhoan.KhoaDenThoiDiem = null;
            }
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Đã {(taiKhoan.TrangThai ? "mở khóa" : "khóa")} tài khoản '{taiKhoan.TenDangNhap}' thành công.";
            return RedirectToAction("DanhSachTaiKhoan");
        }

        // ==============================================================================
        // 9. MỞ KHÓA BRUTE-FORCE TẠM THỜI (BỞI ADMIN)
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

            taiKhoan.SoLanDangNhapSai = 0;
            taiKhoan.KhoaDenThoiDiem = null;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Đã mở khóa đăng nhập tạm thời cho tài khoản '{taiKhoan.TenDangNhap}'.";
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
