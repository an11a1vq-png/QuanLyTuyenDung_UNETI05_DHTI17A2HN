
// Họ và tên: Nguyễn Tấn Dũng
// Mã sinh viên: 23103100070
// Nội dung: Module 3 - Nộp hồ sơ và theo dõi ứng tuyển
using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using QuanLyTuyenDung_UNETI05_DHTI17A2HN.Data;
using QuanLyTuyenDung_UNETI05_DHTI17A2HN.Models;
namespace QuanLyTuyenDung_UNETI05_DHTI17A2HN.Controllers
{
    public class HoSoUngTuyenController : Controller
    {
        private readonly ApplicationDbContext _context;
        public HoSoUngTuyenController(ApplicationDbContext context)
        {
            _context = context;
        }
        // GET: Mở form nộp hồ sơ ứng tuyển
        [HttpGet]
        public async Task<IActionResult> Apply(int id)
        {
            var maTaiKhoanStr = HttpContext.Session.GetString("MaTaiKhoan");
            if (string.IsNullOrEmpty(maTaiKhoanStr) || !int.TryParse(maTaiKhoanStr, out int maTaiKhoan))
            {
                TempData["ErrorMessage"] = "Vui lòng đăng nhập với tài khoản Ứng viên để nộp hồ sơ.";
                return RedirectToAction("Login", "TaiKhoan", new { returnUrl = Url.Action("Apply", "HoSoUngTuyen", new { id }) });
            }
            var ungVien = await _context.UngVien.FirstOrDefaultAsync(u => u.MaTaiKhoan == maTaiKhoan);
            if (ungVien == null)
            {
                TempData["ErrorMessage"] = "Bạn cần cập nhật thông tin cá nhân trước khi nộp hồ sơ.";
                return RedirectToAction("EditProfile", "UngVien");
            }
            var viTri = await _context.ViTriTuyenDung
                .Include(v => v.PhongBan)
                .FirstOrDefaultAsync(v => v.MaViTri == id);
            if (viTri == null)
            {
                TempData["ErrorMessage"] = "Vị trí tuyển dụng không tồn tại.";
                return RedirectToAction("Index", "ViTriTuyenDung");
            }
            var now = DateTime.Now;
            if (viTri.TrangThai != "Đang tuyển" || now < viTri.NgayBatDauNhanHoSo || now > viTri.HanNopHoSo)
            {
                TempData["ErrorMessage"] = "Vị trí này hiện không nhận hồ sơ (hết hạn hoặc chưa mở).";
                return RedirectToAction("Details", "ViTriTuyenDung", new { id });
            }
            bool daNop = await _context.HoSoUngTuyen.AnyAsync(h =>
                h.MaUngVien == ungVien.MaUngVien &&
                h.MaViTri == id &&
                h.TrangThai != "Đã hủy" &&
                h.TrangThai != "Không đạt sơ tuyển" &&
                h.TrangThai != "Không trúng tuyển");
            if (daNop)
            {
                TempData["ErrorMessage"] = "Bạn đã có hồ sơ đang xử lý cho vị trí này.";
                return RedirectToAction("MyApplications");
            }
            ViewBag.ViTri = viTri;
            ViewBag.UngVien = ungVien;
            var hoSo = new HoSoUngTuyen
            {
                MaViTri = id,
                MaUngVien = ungVien.MaUngVien
            };
            return View(hoSo);
        }
        // POST: Xử lý lưu hồ sơ nộp
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Apply(HoSoUngTuyen model)
        {
            var maTaiKhoanStr = HttpContext.Session.GetString("MaTaiKhoan");
            if (string.IsNullOrEmpty(maTaiKhoanStr) || !int.TryParse(maTaiKhoanStr, out int maTaiKhoan))
            {
                return RedirectToAction("Login", "TaiKhoan");
            }
            var ungVien = await _context.UngVien.FirstOrDefaultAsync(u => u.MaTaiKhoan == maTaiKhoan);
            if (ungVien == null)
            {
                return RedirectToAction("EditProfile", "UngVien");
            }
            model.MaUngVien = ungVien.MaUngVien;
            model.NgayNop = DateTime.Now;
            model.TrangThai = "Chờ duyệt";
            model.NgayXuLy = null;
            model.NhanXetSoTuyen = null;
            _context.HoSoUngTuyen.Add(model);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Nộp hồ sơ ứng tuyển thành công!";
            return RedirectToAction("MyApplications");
        }
        // GET: Danh sách các hồ sơ ứng viên đã nộp
        [HttpGet]
        public async Task<IActionResult> MyApplications()
        {
            var maTaiKhoanStr = HttpContext.Session.GetString("MaTaiKhoan");
            if (string.IsNullOrEmpty(maTaiKhoanStr) || !int.TryParse(maTaiKhoanStr, out int maTaiKhoan))
            {
                TempData["ErrorMessage"] = "Vui lòng đăng nhập để xem danh sách hồ sơ.";
                return RedirectToAction("Login", "TaiKhoan");
            }
            var ungVien = await _context.UngVien.FirstOrDefaultAsync(u => u.MaTaiKhoan == maTaiKhoan);
            if (ungVien == null)
            {
                return RedirectToAction("EditProfile", "UngVien");
            }
            var dsHoSo = await _context.HoSoUngTuyen
                .Include(h => h.ViTriTuyenDung)
                    .ThenInclude(v => v.PhongBan)
                .Include(h => h.LichPhongVans)
                .Include(h => h.KetQuaTuyenDung)
                .Where(h => h.MaUngVien == ungVien.MaUngVien)
                .OrderByDescending(h => h.NgayNop)
                .ToListAsync();
            return View(dsHoSo);
        }
        // POST: Hủy hồ sơ ứng tuyển
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelApplication(int id)
        {
            var maTaiKhoanStr = HttpContext.Session.GetString("MaTaiKhoan");
            if (string.IsNullOrEmpty(maTaiKhoanStr) || !int.TryParse(maTaiKhoanStr, out int maTaiKhoan))
            {
                return RedirectToAction("Login", "TaiKhoan");
            }
            var ungVien = await _context.UngVien.FirstOrDefaultAsync(u => u.MaTaiKhoan == maTaiKhoan);
            if (ungVien == null)
            {
                return RedirectToAction("Login", "TaiKhoan");
            }
            var hoSo = await _context.HoSoUngTuyen
                .Include(h => h.LichPhongVans)
                .FirstOrDefaultAsync(h => h.MaHoSo == id && h.MaUngVien == ungVien.MaUngVien);
            if (hoSo == null)
            {
                TempData["ErrorMessage"] = "Hồ sơ không tồn tại hoặc bạn không có quyền thao tác.";
                return RedirectToAction("MyApplications");
            }
            bool coLichPV = hoSo.LichPhongVans != null && hoSo.LichPhongVans.Any(l => l.TrangThai != "Đã hủy");
            if (hoSo.TrangThai == "Chờ duyệt" || (hoSo.TrangThai == "Đạt sơ tuyển" && !coLichPV))
            {
                hoSo.TrangThai = "Đã hủy";
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Đã hủy hồ sơ ứng tuyển thành công.";
            }
            else
            {
                TempData["ErrorMessage"] = "Không thể hủy hồ sơ đã được lên lịch phỏng vấn hoặc đã có kết quả.";
            }
            return RedirectToAction("MyApplications");
        }
    }
}