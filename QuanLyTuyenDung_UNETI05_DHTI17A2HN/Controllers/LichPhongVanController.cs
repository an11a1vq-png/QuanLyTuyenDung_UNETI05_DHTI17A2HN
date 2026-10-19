// ==============================================================================
// Họ và tên: Hoàng Dương
// Mã sinh viên: 23103100117
// Module 5: Lịch phỏng vấn, Kết quả tuyển dụng, Dashboard và Thống kê
// Nội dung thực hiện: Xây dựng LichPhongVanController, kiểm tra phân quyền Session, 
//                     thuật toán chống trùng lịch bằng LINQ và quản lý vòng đời phỏng vấn.
// ==============================================================================

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyTuyenDung_UNETI05_DHTI17A2HN.Data;
using QuanLyTuyenDung_UNETI05_DHTI17A2HN.Helpers;
using QuanLyTuyenDung_UNETI05_DHTI17A2HN.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace QuanLyTuyenDung_UNETI05_DHTI17A2HN.Controllers
{
    public class LichPhongVanController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LichPhongVanController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ==============================================================================
        // 1. DANH SÁCH LỊCH PHỎNG VẤN (/LichPhongVan/Index)
        // ==============================================================================
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // Kiểm tra phân quyền: Chỉ Admin và Nhân sự mới được quản lý lịch phỏng vấn (Mục 5.4 đề tài)
            var vaiTro = HttpContext.Session.GetVaiTro();
            if (vaiTro != "Admin" && vaiTro != "Nhân sự")
            {
                return RedirectToAction("AccessDenied", "TaiKhoan");
            }

            var lichPhongVan = await _context.LichPhongVan
                .Include(l => l.HoSoUngTuyen)
                    .ThenInclude(h => h.UngVien)
                .Include(l => l.HoSoUngTuyen)
                    .ThenInclude(h => h.ViTriTuyenDung)
                .OrderByDescending(l => l.ThoiGianBatDau)
                .ToListAsync();

            return View(lichPhongVan);
        }

        // ==============================================================================
        // 2. GET: FORM LẬP LỊCH PHỎNG VẤN (/LichPhongVan/LapLichPhongVan?maHoSo=...)
        // ==============================================================================
        [HttpGet]
        public async Task<IActionResult> LapLichPhongVan(int? maHoSo)
        {
            var vaiTro = HttpContext.Session.GetVaiTro();
            if (vaiTro != "Admin" && vaiTro != "Nhân sự")
            {
                return RedirectToAction("AccessDenied", "TaiKhoan");
            }

            if (maHoSo == null)
            {
                return NotFound();
            }

            var hoSo = await _context.HoSoUngTuyen
                .Include(h => h.UngVien)
                .Include(h => h.ViTriTuyenDung)
                .FirstOrDefaultAsync(h => h.MaHoSo == maHoSo);

            // Kiểm tra nghiệp vụ: Chỉ hồ sơ Đạt sơ tuyển mới được lập lịch phỏng vấn (Mục 9.2)
            if (hoSo == null || hoSo.TrangThai != "Đạt sơ tuyển")
            {
                TempData["Error"] = "Chỉ những hồ sơ ở trạng thái 'Đạt sơ tuyển' mới được lập lịch phỏng vấn.";
                return RedirectToAction("QuanLyDanhSach", "HoSoUngTuyen");
            }

            // Kiểm tra xem hồ sơ này đã có lịch phỏng vấn đang hiệu lực chưa
            bool coLichHieuLuc = await _context.LichPhongVan
                .AnyAsync(l => l.MaHoSo == maHoSo && l.TrangThai != "Đã hủy");

            if (coLichHieuLuc)
            {
                TempData["Error"] = "Hồ sơ này đã có lịch phỏng vấn đang hiệu lực.";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.MaHoSo = maHoSo;
            ViewBag.ThongTinHoSo = $"{hoSo.UngVien?.HoTen} - Vị trí: {hoSo.ViTriTuyenDung?.TenViTri}";
            return View();
        }

        // ==============================================================================
        // 3. POST: LƯU LỊCH PHỎNG VẤN & THUẬT TOÁN CHỐNG TRÙNG LỊCH BẰNG LINQ
        // ==============================================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LapLichPhongVan([Bind("MaHoSo,ThoiGianBatDau,ThoiGianKetThuc,HinhThucPhongVan,DiaDiemHoacLienKet,NguoiPhongVan,GhiChu")] LichPhongVan lichPhongVan)
        {
            var vaiTro = HttpContext.Session.GetVaiTro();
            if (vaiTro != "Admin" && vaiTro != "Nhân sự")
            {
                return RedirectToAction("AccessDenied", "TaiKhoan");
            }

            // 1. Kiểm tra điều kiện thời gian cơ bản
            if (lichPhongVan.ThoiGianKetThuc <= lichPhongVan.ThoiGianBatDau)
            {
                ModelState.AddModelError("ThoiGianKetThuc", "Thời gian kết thúc phải diễn ra sau thời gian bắt đầu.");
            }

            if (lichPhongVan.ThoiGianBatDau < DateTime.Now)
            {
                ModelState.AddModelError("ThoiGianBatDau", "Thời gian bắt đầu phỏng vấn không được ở trong quá khứ.");
            }

            // 2. Lấy thông tin hồ sơ và ứng viên
            var hoSo = await _context.HoSoUngTuyen
                .Include(h => h.UngVien)
                .Include(h => h.ViTriTuyenDung)
                .FirstOrDefaultAsync(h => h.MaHoSo == lichPhongVan.MaHoSo);

            if (hoSo == null || hoSo.TrangThai != "Đạt sơ tuyển")
            {
                ModelState.AddModelError("", "Hồ sơ không tồn tại hoặc không ở trạng thái 'Đạt sơ tuyển'.");
            }

            // 3. Kiểm tra hồ sơ đã có lịch phỏng vấn hiệu lực khác chưa
            bool coLichHieuLuc = await _context.LichPhongVan
                .AnyAsync(l => l.MaHoSo == lichPhongVan.MaHoSo && l.TrangThai != "Đã hủy");

            if (coLichHieuLuc)
            {
                ModelState.AddModelError("", "Hồ sơ này đã có một lịch phỏng vấn đang hiệu lực khác.");
            }

            // 4. Thuật toán kiểm tra chống trùng lịch bằng LINQ theo đúng Đề tài 13 (Mục 9.2):
            // - Ứng viên không có lịch khác bị trùng khoảng thời gian (xét theo MaUngVien trên tất cả các hồ sơ chưa hủy)
            // - Người phỏng vấn không có lịch khác bị trùng khoảng thời gian
            if (hoSo != null)
            {
                var ungVienTrungLich = await _context.LichPhongVan
                    .Include(l => l.HoSoUngTuyen)
                    .Where(l => l.TrangThai != "Đã hủy")
                    .Where(l => l.HoSoUngTuyen.MaUngVien == hoSo.MaUngVien
                                && lichPhongVan.ThoiGianBatDau < l.ThoiGianKetThuc
                                && lichPhongVan.ThoiGianKetThuc > l.ThoiGianBatDau)
                    .AnyAsync();

                if (ungVienTrungLich)
                {
                    ModelState.AddModelError("", "Ứng viên này đã có một lịch phỏng vấn khác bị trùng trong khoảng thời gian đã chọn.");
                }

                var nguoiPvTrungLich = await _context.LichPhongVan
                    .Where(l => l.TrangThai != "Đã hủy")
                    .Where(l => l.NguoiPhongVan.ToLower() == lichPhongVan.NguoiPhongVan.Trim().ToLower()
                                && lichPhongVan.ThoiGianBatDau < l.ThoiGianKetThuc
                                && lichPhongVan.ThoiGianKetThuc > l.ThoiGianBatDau)
                    .AnyAsync();

                if (nguoiPvTrungLich)
                {
                    ModelState.AddModelError("NguoiPhongVan", "Người phỏng vấn này đã có lịch họp/phỏng vấn khác trùng trong khung giờ này.");
                }
            }

            if (ModelState.IsValid && hoSo != null)
            {
                lichPhongVan.TrangThai = "Đã lên lịch";
                _context.Add(lichPhongVan);

                // Cập nhật trạng thái hồ sơ sang Chờ phỏng vấn (Mục 25.3)
                hoSo.TrangThai = "Chờ phỏng vấn";
                _context.Update(hoSo);

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Lập lịch phỏng vấn thành công!";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.MaHoSo = lichPhongVan.MaHoSo;
            if (hoSo != null)
            {
                ViewBag.ThongTinHoSo = $"{hoSo.UngVien?.HoTen} - Vị trí: {hoSo.ViTriTuyenDung?.TenViTri}";
            }
            return View(lichPhongVan);
        }

        // ==============================================================================
        // 4. ĐÁNH DẤU HOÀN THÀNH PHỎNG VẤN (/LichPhongVan/HoanThanh/5)
        // ==============================================================================
        [HttpGet]
        public async Task<IActionResult> HoanThanh(int id)
        {
            var vaiTro = HttpContext.Session.GetVaiTro();
            if (vaiTro != "Admin" && vaiTro != "Nhân sự")
            {
                return RedirectToAction("AccessDenied", "TaiKhoan");
            }

            var lich = await _context.LichPhongVan
                .Include(l => l.HoSoUngTuyen)
                .FirstOrDefaultAsync(l => l.MaLichPhongVan == id);

            if (lich == null || lich.TrangThai == "Đã hủy")
            {
                TempData["Error"] = "Lịch phỏng vấn không tồn tại hoặc đã bị hủy.";
                return RedirectToAction(nameof(Index));
            }

            lich.TrangThai = "Đã hoàn thành";

            // Cập nhật trạng thái hồ sơ sang Đã phỏng vấn (Mục 9.3)
            if (lich.HoSoUngTuyen != null)
            {
                lich.HoSoUngTuyen.TrangThai = "Đã phỏng vấn";
                _context.Update(lich.HoSoUngTuyen);
            }

            _context.Update(lich);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Đã cập nhật trạng thái 'Đã hoàn thành'! Bạn có thể chuyển sang bước nhập điểm kết quả tuyển dụng.";
            return RedirectToAction(nameof(Index));
        }

        // ==============================================================================
        // 5. HỦY LỊCH PHỎNG VẤN (/LichPhongVan/HuyLich/5) - (Mục 22.5 Đề tài)
        // ==============================================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> HuyLich(int id)
        {
            var vaiTro = HttpContext.Session.GetVaiTro();
            if (vaiTro != "Admin" && vaiTro != "Nhân sự")
            {
                return RedirectToAction("AccessDenied", "TaiKhoan");
            }

            var lich = await _context.LichPhongVan
                .Include(l => l.HoSoUngTuyen)
                .FirstOrDefaultAsync(l => l.MaLichPhongVan == id);

            if (lich == null || lich.TrangThai != "Đã lên lịch")
            {
                TempData["Error"] = "Chỉ có thể hủy lịch phỏng vấn đang ở trạng thái 'Đã lên lịch'.";
                return RedirectToAction(nameof(Index));
            }

            lich.TrangThai = "Đã hủy";
            _context.Update(lich);

            // Hoàn lại trạng thái hồ sơ về "Đạt sơ tuyển" nếu không còn lịch hiệu lực khác
            if (lich.HoSoUngTuyen != null)
            {
                bool conLichKhac = await _context.LichPhongVan
                    .AnyAsync(l => l.MaHoSo == lich.MaHoSo && l.MaLichPhongVan != id && l.TrangThai != "Đã hủy");

                if (!conLichKhac)
                {
                    lich.HoSoUngTuyen.TrangThai = "Đạt sơ tuyển";
                    _context.Update(lich.HoSoUngTuyen);
                }
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Đã hủy lịch phỏng vấn thành công!";
            return RedirectToAction(nameof(Index));
        }
    }
}