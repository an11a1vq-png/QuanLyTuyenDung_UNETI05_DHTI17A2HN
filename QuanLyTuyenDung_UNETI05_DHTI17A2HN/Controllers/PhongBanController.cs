// ==============================================================================
// Họ và tên: Đỗ Thiện An
// Mã sinh viên: 23103100082
// Module: 1 - Tài khoản, Đăng nhập, Phân quyền và Phòng ban
// Nội dung thực hiện: Quản lý danh sách, Thêm mới, Cập nhật, Chi tiết và Xóa phòng ban (chuẩn hóa tên Action nghiệp vụ)
// ==============================================================================

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyTuyenDung_UNETI05_DHTI17A2HN.Data;
using QuanLyTuyenDung_UNETI05_DHTI17A2HN.Helpers;
using QuanLyTuyenDung_UNETI05_DHTI17A2HN.Models;

namespace QuanLyTuyenDung_UNETI05_DHTI17A2HN.Controllers
{
    public class PhongBanController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PhongBanController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ==============================================================================
        // 1. DANH SÁCH PHÒNG BAN (/PhongBan/DanhSachPhongBan hoặc /PhongBan/Index)
        // ==============================================================================
        [HttpGet]
        [Route("PhongBan/DanhSachPhongBan")]
        [Route("PhongBan/Index")]
        [Route("PhongBan")]
        public async Task<IActionResult> DanhSachPhongBan(string? searchString, bool? trangThai)
        {
            // Kiểm tra quyền: Admin hoặc Nhân sự được xem phòng ban
            var vaiTro = HttpContext.Session.GetVaiTro();
            if (vaiTro != "Admin" && vaiTro != "Nhân sự")
            {
                return RedirectToAction("AccessDenied", "TaiKhoan");
            }

            var query = _context.PhongBan
                .Include(p => p.DanhSachViTri)
                .AsQueryable();

            // Tìm kiếm bằng LINQ theo tên phòng ban
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                var term = searchString.Trim();
                query = query.Where(p => p.TenPhongBan.Contains(term) ||
                                         (p.MoTa != null && p.MoTa.Contains(term)));
            }

            // Lọc theo trạng thái hoạt động
            if (trangThai.HasValue)
            {
                query = query.Where(p => p.TrangThai == trangThai.Value);
            }

            ViewBag.CurrentSearch = searchString;
            ViewBag.CurrentTrangThai = trangThai;

            var danhSach = await query
                .OrderBy(p => p.TenPhongBan)
                .ToListAsync();

            return View(danhSach);
        }

        // ==============================================================================
        // 2. CHI TIẾT PHÒNG BAN (/PhongBan/ChiTietPhongBan/{id} hoặc /PhongBan/Details/{id})
        // ==============================================================================
        [HttpGet]
        [Route("PhongBan/ChiTietPhongBan/{id:int}")]
        [Route("PhongBan/Details/{id:int}")]
        public async Task<IActionResult> ChiTietPhongBan(int? id)
        {
            var vaiTro = HttpContext.Session.GetVaiTro();
            if (vaiTro != "Admin" && vaiTro != "Nhân sự")
            {
                return RedirectToAction("AccessDenied", "TaiKhoan");
            }

            if (!id.HasValue)
            {
                TempData["ErrorMessage"] = "Không tìm thấy mã phòng ban.";
                return RedirectToAction(nameof(DanhSachPhongBan));
            }

            var phongBan = await _context.PhongBan
                .Include(p => p.DanhSachViTri)
                .FirstOrDefaultAsync(p => p.MaPhongBan == id.Value);

            if (phongBan == null)
            {
                TempData["ErrorMessage"] = "Phòng ban không tồn tại trong hệ thống.";
                return RedirectToAction(nameof(DanhSachPhongBan));
            }

            return View(phongBan);
        }

        // ==============================================================================
        // 3. THÊM MỚI PHÒNG BAN (/PhongBan/ThemMoiPhongBan hoặc /PhongBan/Create)
        // ==============================================================================
        [HttpGet]
        [Route("PhongBan/ThemMoiPhongBan")]
        [Route("PhongBan/Create")]
        public IActionResult ThemMoiPhongBan()
        {
            // Chỉ Admin mới có quyền thêm mới phòng ban
            if (HttpContext.Session.GetVaiTro() != "Admin")
            {
                return RedirectToAction("AccessDenied", "TaiKhoan");
            }

            var model = new PhongBan
            {
                TrangThai = true
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("PhongBan/ThemMoiPhongBan")]
        [Route("PhongBan/Create")]
        public async Task<IActionResult> ThemMoiPhongBan([Bind("TenPhongBan,MoTa,EmailLienHe,TrangThai")] PhongBan phongBan)
        {
            if (HttpContext.Session.GetVaiTro() != "Admin")
            {
                return RedirectToAction("AccessDenied", "TaiKhoan");
            }

            // Kiểm tra trùng tên phòng ban bằng LINQ (Mục 5.5: Tên phòng ban không được trùng)
            if (!string.IsNullOrWhiteSpace(phongBan.TenPhongBan))
            {
                var tenTrim = phongBan.TenPhongBan.Trim().ToLower();
                var isDuplicate = await _context.PhongBan.AnyAsync(p => p.TenPhongBan.ToLower() == tenTrim);
                if (isDuplicate)
                {
                    ModelState.AddModelError("TenPhongBan", "Tên phòng ban này đã tồn tại trong hệ thống. Vui lòng đặt tên khác.");
                }
            }

            if (!ModelState.IsValid)
            {
                return View(phongBan);
            }

            phongBan.TenPhongBan = phongBan.TenPhongBan.Trim();
            phongBan.MoTa = phongBan.MoTa?.Trim();
            phongBan.EmailLienHe = phongBan.EmailLienHe?.Trim();

            _context.PhongBan.Add(phongBan);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Thêm mới phòng ban '{phongBan.TenPhongBan}' thành công.";
            return RedirectToAction(nameof(DanhSachPhongBan));
        }

        // ==============================================================================
        // 4. CẬP NHẬT PHÒNG BAN (/PhongBan/CapNhatPhongBan/{id} hoặc /PhongBan/Edit/{id})
        // ==============================================================================
        [HttpGet]
        [Route("PhongBan/CapNhatPhongBan/{id:int}")]
        [Route("PhongBan/Edit/{id:int}")]
        public async Task<IActionResult> CapNhatPhongBan(int? id)
        {
            if (HttpContext.Session.GetVaiTro() != "Admin")
            {
                return RedirectToAction("AccessDenied", "TaiKhoan");
            }

            if (!id.HasValue)
            {
                TempData["ErrorMessage"] = "Không tìm thấy mã phòng ban cần chỉnh sửa.";
                return RedirectToAction(nameof(DanhSachPhongBan));
            }

            var phongBan = await _context.PhongBan.FindAsync(id.Value);
            if (phongBan == null)
            {
                TempData["ErrorMessage"] = "Phòng ban không tồn tại trong hệ thống.";
                return RedirectToAction(nameof(DanhSachPhongBan));
            }

            return View(phongBan);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("PhongBan/CapNhatPhongBan/{id:int}")]
        [Route("PhongBan/Edit/{id:int}")]
        public async Task<IActionResult> CapNhatPhongBan(int id, [Bind("MaPhongBan,TenPhongBan,MoTa,EmailLienHe,TrangThai")] PhongBan phongBan)
        {
            if (HttpContext.Session.GetVaiTro() != "Admin")
            {
                return RedirectToAction("AccessDenied", "TaiKhoan");
            }

            if (id != phongBan.MaPhongBan)
            {
                TempData["ErrorMessage"] = "Mã phòng ban không hợp lệ.";
                return RedirectToAction(nameof(DanhSachPhongBan));
            }

            // Kiểm tra trùng tên phòng ban với phòng ban khác bằng LINQ
            if (!string.IsNullOrWhiteSpace(phongBan.TenPhongBan))
            {
                var tenTrim = phongBan.TenPhongBan.Trim().ToLower();
                var isDuplicate = await _context.PhongBan.AnyAsync(p => p.MaPhongBan != id && p.TenPhongBan.ToLower() == tenTrim);
                if (isDuplicate)
                {
                    ModelState.AddModelError("TenPhongBan", "Tên phòng ban này đã trùng với một phòng ban khác trong hệ thống.");
                }
            }

            if (!ModelState.IsValid)
            {
                return View(phongBan);
            }

            try
            {
                var existing = await _context.PhongBan.FindAsync(id);
                if (existing == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy phòng ban để cập nhật.";
                    return RedirectToAction(nameof(DanhSachPhongBan));
                }

                existing.TenPhongBan = phongBan.TenPhongBan.Trim();
                existing.MoTa = phongBan.MoTa?.Trim();
                existing.EmailLienHe = phongBan.EmailLienHe?.Trim();
                existing.TrangThai = phongBan.TrangThai;

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Cập nhật phòng ban '{existing.TenPhongBan}' thành công.";
                return RedirectToAction(nameof(DanhSachPhongBan));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.PhongBan.AnyAsync(p => p.MaPhongBan == id))
                {
                    TempData["ErrorMessage"] = "Phòng ban không còn tồn tại.";
                    return RedirectToAction(nameof(DanhSachPhongBan));
                }
                throw;
            }
        }

        // ==============================================================================
        // 5. XÓA PHÒNG BAN (/PhongBan/XoaPhongBan/{id} hoặc /PhongBan/Delete/{id})
        // ==============================================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("PhongBan/XoaPhongBan/{id:int}")]
        [Route("PhongBan/Delete/{id:int}")]
        public async Task<IActionResult> XoaPhongBan(int id)
        {
            if (HttpContext.Session.GetVaiTro() != "Admin")
            {
                return RedirectToAction("AccessDenied", "TaiKhoan");
            }

            var phongBan = await _context.PhongBan
                .Include(p => p.DanhSachViTri)
                .FirstOrDefaultAsync(p => p.MaPhongBan == id);

            if (phongBan == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy phòng ban cần xóa.";
                return RedirectToAction(nameof(DanhSachPhongBan));
            }

            // Kiểm tra ràng buộc nghiệp vụ (Mục 5.5: Không được xóa phòng ban nếu làm các vị trí tuyển dụng liên quan không còn hợp lệ)
            var soLuongViTri = await _context.ViTriTuyenDung.CountAsync(v => v.MaPhongBan == id);
            if (soLuongViTri > 0)
            {
                TempData["ErrorMessage"] = $"Không thể xóa phòng ban '{phongBan.TenPhongBan}' vì đang có {soLuongViTri} vị trí tuyển dụng liên kết. Bạn có thể cập nhật trạng thái sang 'Ngưng hoạt động'.";
                return RedirectToAction(nameof(DanhSachPhongBan));
            }

            _context.PhongBan.Remove(phongBan);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Đã xóa phòng ban '{phongBan.TenPhongBan}' thành công.";
            return RedirectToAction(nameof(DanhSachPhongBan));
        }

        // ==============================================================================
        // 6. BẬT / TẮT TRẠNG THÁI HOẠT ĐỘNG (/PhongBan/ToggleTrangThai/{id})
        // ==============================================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("PhongBan/ToggleTrangThai/{id:int}")]
        public async Task<IActionResult> ToggleTrangThai(int id)
        {
            if (HttpContext.Session.GetVaiTro() != "Admin")
            {
                return RedirectToAction("AccessDenied", "TaiKhoan");
            }

            var phongBan = await _context.PhongBan.FindAsync(id);
            if (phongBan == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy phòng ban.";
                return RedirectToAction(nameof(DanhSachPhongBan));
            }

            phongBan.TrangThai = !phongBan.TrangThai;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Đã đổi trạng thái phòng ban '{phongBan.TenPhongBan}' sang {(phongBan.TrangThai ? "Hoạt động" : "Ngưng hoạt động")}.";
            return RedirectToAction(nameof(DanhSachPhongBan));
        }
    }
}
