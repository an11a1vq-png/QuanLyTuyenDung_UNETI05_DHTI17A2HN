// Họ và tên: Nguyễn Tấn Dũng
// Mã sinh viên: 23103100070
// Nội dung: Module 3 - Quản lý hồ sơ cá nhân ứng viên
using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using QuanLyTuyenDung_UNETI05_DHTI17A2HN.Data;
using QuanLyTuyenDung_UNETI05_DHTI17A2HN.Models;
namespace QuanLyTuyenDung_UNETI05_DHTI17A2HN.Controllers
{
    public class UngVienController : Controller
    {
        private readonly ApplicationDbContext _context;
        public UngVienController(ApplicationDbContext context)
        {
            _context = context;
        }
        // GET: Xem chi tiết thông tin hồ sơ ứng viên
        [HttpGet]
        public async Task<IActionResult> Details()
        {
            var maTaiKhoanStr = HttpContext.Session.GetString("MaTaiKhoan");
            if (string.IsNullOrEmpty(maTaiKhoanStr) || !int.TryParse(maTaiKhoanStr, out int maTaiKhoan))
            {
                TempData["ErrorMessage"] = "Vui lòng đăng nhập để xem thông tin.";
                return RedirectToAction("Login", "TaiKhoan");
            }
            var ungVien = await _context.UngVien
                .Include(u => u.TaiKhoan)
                .FirstOrDefaultAsync(u => u.MaTaiKhoan == maTaiKhoan);
            if (ungVien == null)
            {
                return RedirectToAction("EditProfile");
            }
            return View(ungVien);
        }
        // GET: Giao diện cập nhật hồ sơ
        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            var maTaiKhoanStr = HttpContext.Session.GetString("MaTaiKhoan");
            if (string.IsNullOrEmpty(maTaiKhoanStr) || !int.TryParse(maTaiKhoanStr, out int maTaiKhoan))
            {
                TempData["ErrorMessage"] = "Vui lòng đăng nhập để cập nhật hồ sơ.";
                return RedirectToAction("Login", "TaiKhoan");
            }
            var ungVien = await _context.UngVien.FirstOrDefaultAsync(u => u.MaTaiKhoan == maTaiKhoan);
            if (ungVien == null)
            {
                var taiKhoan = await _context.TaiKhoan.FindAsync(maTaiKhoan);
                ungVien = new UngVien
                {
                    MaTaiKhoan = maTaiKhoan,
                    HoTen = taiKhoan?.HoTen ?? "",
                    Email = taiKhoan?.Email ?? "",
                    TrangThai = true
                };
            }
            return View(ungVien);
        }
        // POST: Lưu thông tin hồ sơ cập nhật
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(UngVien model)
        {
            var maTaiKhoanStr = HttpContext.Session.GetString("MaTaiKhoan");
            if (string.IsNullOrEmpty(maTaiKhoanStr) || !int.TryParse(maTaiKhoanStr, out int maTaiKhoan))
            {
                return RedirectToAction("Login", "TaiKhoan");
            }
            model.MaTaiKhoan = maTaiKhoan;
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            if (model.SoNamKinhNghiem < 0)
            {
                ModelState.AddModelError("SoNamKinhNghiem", "Số năm kinh nghiệm phải >= 0.");
                return View(model);
            }
            var ungVienExist = await _context.UngVien.FirstOrDefaultAsync(u => u.MaTaiKhoan == maTaiKhoan);
            if (ungVienExist == null)
            {
                _context.UngVien.Add(model);
            }
            else
            {
                ungVienExist.HoTen = model.HoTen;
                ungVienExist.NgaySinh = model.NgaySinh;
                ungVienExist.GioiTinh = model.GioiTinh;
                ungVienExist.SoDienThoai = model.SoDienThoai;
                ungVienExist.Email = model.Email;
                ungVienExist.DiaChi = model.DiaChi;
                ungVienExist.TrinhDoHocVan = model.TrinhDoHocVan;
                ungVienExist.ChuyenNganh = model.ChuyenNganh;
                ungVienExist.SoNamKinhNghiem = model.SoNamKinhNghiem;
                ungVienExist.KyNang = model.KyNang;
                _context.UngVien.Update(ungVienExist);
            }
            await _context.SaveChangesAsync();
            var uv = await _context.UngVien.FirstOrDefaultAsync(u => u.MaTaiKhoan == maTaiKhoan);
            if (uv != null)
            {
                HttpContext.Session.SetString("MaUngVien", uv.MaUngVien.ToString());
            }
            TempData["SuccessMessage"] = "Cập nhật hồ sơ cá nhân thành công!";
            return RedirectToAction("Details");
        }
    }
}