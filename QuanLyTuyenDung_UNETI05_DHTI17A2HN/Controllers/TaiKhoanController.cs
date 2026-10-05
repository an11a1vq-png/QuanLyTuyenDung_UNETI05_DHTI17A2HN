// ==============================================================================
// Họ và tên: Đỗ Thiện An
// Mã sinh viên: 23103100082
// Module: 1 - Tài khoản, Đăng nhập, Phân quyền và Phòng ban
// Nội dung thực hiện: Nghiệp vụ Đăng nhập, Đăng ký, Đăng xuất, Phân quyền Session và Quản lý danh sách tài khoản
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

        public TaiKhoanController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ==============================================================================
        // 1. ĐĂNG NHẬP HỆ THỐNG (/TaiKhoan/DangNhap hoặc /TaiKhoan/Login)
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

            // Kiểm tra thông tin tài khoản bằng LINQ
            var taiKhoan = await _context.TaiKhoan
                .Include(t => t.UngVien)
                .FirstOrDefaultAsync(t => t.TenDangNhap == model.TenDangNhap && t.MatKhau == model.MatKhau);

            if (taiKhoan == null)
            {
                ModelState.AddModelError(string.Empty, "Tên đăng nhập hoặc mật khẩu không chính xác.");
                return View(model);
            }

            // Kiểm tra trạng thái hoạt động
            if (!taiKhoan.TrangThai)
            {
                ModelState.AddModelError(string.Empty, "Tài khoản của bạn hiện đang bị khóa. Vui lòng liên hệ Quản trị viên.");
                return View(model);
            }

            // Lưu thông tin người dùng vào Session
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

            // 3. Tạo tài khoản người dùng với vai trò "Ứng viên"
            var taiKhoan = new TaiKhoan
            {
                TenDangNhap = model.TenDangNhap.Trim(),
                MatKhau = model.MatKhau,
                HoTen = model.HoTen.Trim(),
                Email = model.Email.Trim(),
                VaiTro = "Ứng viên",
                TrangThai = true
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
        // 3. ĐĂNG XUẤT HỆ THỐNG (/TaiKhoan/DangXuat hoặc /TaiKhoan/Logout)
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
        // 4. QUẢN TRỊ DANH SÁCH TÀI KHOẢN (/TaiKhoan/DanhSachTaiKhoan hoặc /TaiKhoan/Index)
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
        // 5. KHÓA / MỞ KHÓA TÀI KHOẢN (/TaiKhoan/KhoaTaiKhoan hoặc /TaiKhoan/ToggleTrangThai)
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
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Đã {(taiKhoan.TrangThai ? "mở khóa" : "khóa")} tài khoản '{taiKhoan.TenDangNhap}' thành công.";
            return RedirectToAction("DanhSachTaiKhoan");
        }

        // ==============================================================================
        // 6. TRANG TỪ CHỐI TRUY CẬP (/TaiKhoan/AccessDenied)
        // ==============================================================================
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
