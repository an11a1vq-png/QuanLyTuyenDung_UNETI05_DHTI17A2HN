// Họ và tên: Nguyễn Tấn Dũng
// Mã sinh viên: 23103100070
// Nội dung: Module 3 - Quản lý hồ sơ cá nhân ứng viên
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyTuyenDung_UNETI05_DHTI17A2HN.Data;
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

        // ==========================================
        // BẮT ĐẦU TEST LOGIN (BỎ QUA ĐĂNG NHẬP ĐỂ TEST)
        // ==========================================
        [HttpGet]
        public IActionResult TestLogin()
        {
            // Tự động gán Session đóng vai tài khoản Ứng viên số 3
            HttpContext.Session.SetString("MaTaiKhoan", "3");
            HttpContext.Session.SetString("HoTen", "Ứng Viên Test UNETI");
            HttpContext.Session.SetString("VaiTro", "Ứng viên");

            TempData["SuccessMessage"] = "Đã giả lập đăng nhập thành công tài khoản Ứng viên #3!";
            return RedirectToAction(nameof(ThongTinCaNhan));
        }
        // ==========================================
        // KẾT THÚC TEST LOGIN 
        // ==========================================

        // GET: Xem chi tiết thông tin hồ sơ ứng viên
        [HttpGet]
        public async Task<IActionResult> ThongTinCaNhan()
        {
            var maTaiKhoanStr = HttpContext.Session.GetString("MaTaiKhoan");
            if (string.IsNullOrEmpty(maTaiKhoanStr) || !int.TryParse(maTaiKhoanStr, out int maTaiKhoan))
            {
                TempData["ErrorMessage"] = "Vui lòng đăng nhập để xem thông tin.";
                return RedirectToAction("DangNhap", "TaiKhoan");
            }

            var ungVien = await _context.UngVien
                .Include(u => u.TaiKhoan)
                .FirstOrDefaultAsync(u => u.MaTaiKhoan == maTaiKhoan);

            if (ungVien == null)
            {
                return RedirectToAction(nameof(CapNhatHoSo));
            }

            return View(ungVien);
        }
        // GET: Giao diện cập nhật hồ sơ
        [HttpGet]
        public async Task<IActionResult> CapNhatHoSo()
        {
            var maTaiKhoanStr = HttpContext.Session.GetString("MaTaiKhoan");
            if (string.IsNullOrEmpty(maTaiKhoanStr) || !int.TryParse(maTaiKhoanStr, out int maTaiKhoan))
            {
                TempData["ErrorMessage"] = "Vui lòng đăng nhập để cập nhật hồ sơ.";
                return RedirectToAction("DangNhap", "TaiKhoan");
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
        public async Task<IActionResult> CapNhatHoSo(UngVien model)
        {
            var maTaiKhoanStr = HttpContext.Session.GetString("MaTaiKhoan");
            if (string.IsNullOrEmpty(maTaiKhoanStr) || !int.TryParse(maTaiKhoanStr, out int maTaiKhoan))
            {
                return RedirectToAction("DangNhap", "TaiKhoan");
            }

            // KIỂM TRA VALIDATION DỮ LIỆU ĐẦU VÀO
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var ungVienExist = await _context.UngVien.FirstOrDefaultAsync(u => u.MaTaiKhoan == maTaiKhoan);

            if (ungVienExist == null)
            {
                model.MaTaiKhoan = maTaiKhoan;
                model.TrangThai = true;
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
            TempData["SuccessMessage"] = "Cập nhật hồ sơ cá nhân thành công!";
            return RedirectToAction(nameof(ThongTinCaNhan));
        }
    }
}