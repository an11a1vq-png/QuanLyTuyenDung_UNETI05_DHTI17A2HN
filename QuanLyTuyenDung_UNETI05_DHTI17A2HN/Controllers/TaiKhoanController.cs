// ==============================================================================
// Họ và tên: Đỗ Thiện An
// Mã sinh viên: 23103100082
// Module: 1 - Tài khoản, Đăng nhập, Phân quyền và Phòng ban
// Vai trò: Sinh viên 1 (Phụ trách Đăng nhập, Đăng xuất, Đăng ký, Quản lý tài khoản)
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

        // GET: /TaiKhoan/Login
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
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

        // POST: /TaiKhoan/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var taiKhoan = await _context.TaiKhoan
                .Include(t => t.UngVien)
                .FirstOrDefaultAsync(t => t.TenDangNhap == model.TenDangNhap && t.MatKhau == model.MatKhau);

            if (taiKhoan == null)
            {
                ModelState.AddModelError(string.Empty, "Tên đăng nhập hoặc mật khẩu không chính xác.");
                return View(model);
            }

            if (!taiKhoan.TrangThai)
            {
                ModelState.AddModelError(string.Empty, "Tài khoản của bạn đang bị khóa. Vui lòng liên hệ Quản trị viên.");
                return View(model);
            }

            // Lưu Session
            HttpContext.Session.SetUserSession(
                taiKhoan.MaTaiKhoan,
                taiKhoan.HoTen,
                taiKhoan.VaiTro,
                taiKhoan.UngVien?.MaUngVien
            );

            TempData["SuccessMessage"] = $"Đăng nhập thành công! Chào mừng {taiKhoan.HoTen} ({taiKhoan.VaiTro}).";

            if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
            {
                return Redirect(model.ReturnUrl);
            }

            return RedirectToAction("Index", "Home");
        }

        // GET: /TaiKhoan/Register (Đăng ký tài khoản Ứng viên mới)
        [HttpGet]
        public IActionResult Register()
        {
            if (HttpContext.Session.IsLoggedIn())
            {
                return RedirectToAction("Index", "Home");
            }
            return View(new RegisterViewModel());
        }

        // POST: /TaiKhoan/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // 1. Kiểm tra trùng tên đăng nhập
            var isUsernameExists = await _context.TaiKhoan.AnyAsync(t => t.TenDangNhap.ToLower() == model.TenDangNhap.Trim().ToLower());
            if (isUsernameExists)
            {
                ModelState.AddModelError("TenDangNhap", "Tên đăng nhập đã tồn tại trong hệ thống. Vui lòng chọn tên khác.");
                return View(model);
            }

            // 2. Kiểm tra trùng email
            var isEmailExists = await _context.TaiKhoan.AnyAsync(t => t.Email.ToLower() == model.Email.Trim().ToLower());
            if (isEmailExists)
            {
                ModelState.AddModelError("Email", "Email này đã được sử dụng. Vui lòng chọn email khác.");
                return View(model);
            }

            // 3. Tạo tài khoản với vai trò "Ứng viên"
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

            // 4. Tạo bản ghi hồ sơ Ứng viên rỗng tương ứng
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
            return RedirectToAction("Login", "TaiKhoan");
        }

        // GET: /TaiKhoan (Admin quản lý danh sách tài khoản)
        [HttpGet]
        public async Task<IActionResult> Index(string? searchString, string? vaiTro)
        {
            if (HttpContext.Session.GetVaiTro() != "Admin")
            {
                return RedirectToAction("AccessDenied", "TaiKhoan");
            }

            var query = _context.TaiKhoan.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                query = query.Where(t => t.TenDangNhap.Contains(searchString) || 
                                         t.HoTen.Contains(searchString) || 
                                         t.Email.Contains(searchString));
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

        // POST: /TaiKhoan/ToggleTrangThai (Admin kích hoạt / khóa tài khoản)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleTrangThai(int id)
        {
            if (HttpContext.Session.GetVaiTro() != "Admin")
            {
                return RedirectToAction("AccessDenied", "TaiKhoan");
            }

            var currentAdminId = HttpContext.Session.GetMaTaiKhoan();
            if (id == currentAdminId)
            {
                TempData["ErrorMessage"] = "Bạn không thể tự khóa tài khoản của chính mình.";
                return RedirectToAction("Index");
            }

            var taiKhoan = await _context.TaiKhoan.FindAsync(id);
            if (taiKhoan == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy tài khoản cần thao tác.";
                return RedirectToAction("Index");
            }

            taiKhoan.TrangThai = !taiKhoan.TrangThai;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Đã {(taiKhoan.TrangThai ? "mở khóa" : "khóa")} tài khoản '{taiKhoan.TenDangNhap}' thành công.";
            return RedirectToAction("Index");
        }

        // GET/POST: /TaiKhoan/Logout
        [HttpGet, HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.ClearUserSession();
            TempData["SuccessMessage"] = "Bạn đã đăng xuất thành công.";
            return RedirectToAction("Login", "TaiKhoan");
        }

        // GET: /TaiKhoan/AccessDenied
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
