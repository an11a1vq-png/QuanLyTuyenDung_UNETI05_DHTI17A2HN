// ==============================================================================
// Họ và tên: Hoàng Dương
// Mã sinh viên: 23103100117
// Module 5: Lịch phỏng vấn, Kết quả tuyển dụng, Dashboard và Thống kê
// Nội dung thực hiện: Xây dựng KetQuaTuyenDungController, phân quyền Session, 
//                     kiểm soát chỉ tiêu bằng LINQ, ghi nhận và chỉnh sửa kết quả tuyển dụng.
// ==============================================================================

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyTuyenDung_UNETI05_DHTI17A2HN.Data;
using QuanLyTuyenDung_UNETI05_DHTI17A2HN.Helpers;
using QuanLyTuyenDung_UNETI05_DHTI17A2HN.Models;
using System;
using System.Threading.Tasks;

namespace QuanLyTuyenDung_UNETI05_DHTI17A2HN.Controllers
{
    public class KetQuaTuyenDungController : Controller
    {
        private readonly ApplicationDbContext _context;

        public KetQuaTuyenDungController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ==============================================================================
        // 1. DANH SÁCH KẾT QUẢ TUYỂN DỤNG (/KetQuaTuyenDung/Index)
        // ==============================================================================
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // Kiểm tra phân quyền: Chỉ Admin và Nhân sự mới được xem/quản lý kết quả tuyển dụng (Mục 5.4 đề tài)
            var vaiTro = HttpContext.Session.GetVaiTro();
            if (vaiTro != "Admin" && vaiTro != "Nhân sự")
            {
                return RedirectToAction("AccessDenied", "TaiKhoan");
            }

            var results = await _context.KetQuaTuyenDung
                .Include(k => k.HoSoUngTuyen)
                    .ThenInclude(h => h.UngVien)
                .Include(k => k.HoSoUngTuyen)
                    .ThenInclude(h => h.ViTriTuyenDung)
                .OrderByDescending(k => k.NgayCapNhat)
                .ToListAsync();

            return View(results);
        }

        // ==============================================================================
        // 2. GET: FORM GHI NHẬN KẾT QUẢ (/KetQuaTuyenDung/CapNhatKetQua?maHoSo=...)
        // ==============================================================================
        [HttpGet]
        public async Task<IActionResult> CapNhatKetQua(int? maHoSo)
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

            // Kiểm tra nghiệp vụ: Chỉ hồ sơ Đã phỏng vấn mới được ghi nhận kết quả cuối cùng (Mục 9.4)
            if (hoSo == null || hoSo.TrangThai != "Đã phỏng vấn")
            {
                TempData["Error"] = "Chỉ những hồ sơ ở trạng thái 'Đã phỏng vấn' mới được nhập điểm và ghi nhận kết quả.";
                return RedirectToAction("Index", "LichPhongVan");
            }

            // Nếu đã có kết quả từ trước thì chuyển sang form Sửa kết quả
            var existingResult = await _context.KetQuaTuyenDung.FirstOrDefaultAsync(k => k.MaHoSo == maHoSo);
            if (existingResult != null)
            {
                return RedirectToAction(nameof(Edit), new { id = existingResult.MaKetQua });
            }

            ViewBag.MaHoSo = maHoSo;
            ViewBag.ThongTinHoSo = $"{hoSo.UngVien?.HoTen} - Vị trí: {hoSo.ViTriTuyenDung?.TenViTri}";
            ViewBag.ChiTieu = hoSo.ViTriTuyenDung?.SoLuongCanTuyen;
            return View();
        }

        // ==============================================================================
        // 3. POST: LƯU KẾT QUẢ TUYỂN DỤNG MỚI (KIỂM SOÁT CHỈ TIÊU BẰNG LINQ - Mục 8.5)
        // ==============================================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CapNhatKetQua([Bind("MaHoSo,DiemDanhGia,NhanXet,KetQua")] KetQuaTuyenDung ketQuaTuyenDung)
        {
            var vaiTro = HttpContext.Session.GetVaiTro();
            if (vaiTro != "Admin" && vaiTro != "Nhân sự")
            {
                return RedirectToAction("AccessDenied", "TaiKhoan");
            }

            var hoSo = await _context.HoSoUngTuyen
                .Include(h => h.UngVien)
                .Include(h => h.ViTriTuyenDung)
                .FirstOrDefaultAsync(h => h.MaHoSo == ketQuaTuyenDung.MaHoSo);

            if (hoSo == null || hoSo.TrangThai != "Đã phỏng vấn")
            {
                ModelState.AddModelError("", "Hồ sơ không tồn tại hoặc không ở trạng thái 'Đã phỏng vấn'.");
            }

            bool hasResult = await _context.KetQuaTuyenDung.AnyAsync(k => k.MaHoSo == ketQuaTuyenDung.MaHoSo);
            if (hasResult)
            {
                ModelState.AddModelError("", "Hồ sơ này đã có kết quả tuyển dụng từ trước.");
            }

            // Kiểm soát chỉ tiêu bằng LINQ (Mục 8.5 & 22.6):
            // Đếm số người trúng tuyển theo vị trí. Cảnh báo và chặn nếu đã đạt chỉ tiêu.
            if (ketQuaTuyenDung.KetQua == "Trúng tuyển" && hoSo?.ViTriTuyenDung != null)
            {
                int soNguoiTrungTuyen = await _context.HoSoUngTuyen
                    .CountAsync(h => h.MaViTri == hoSo.MaViTri && h.TrangThai == "Trúng tuyển");

                int chiTieu = hoSo.ViTriTuyenDung.SoLuongCanTuyen;
                if (soNguoiTrungTuyen >= chiTieu)
                {
                    ModelState.AddModelError("KetQua", $"Vị trí '{hoSo.ViTriTuyenDung.TenViTri}' đã tuyển đủ chỉ tiêu ({soNguoiTrungTuyen}/{chiTieu} người). Không thể chọn 'Trúng tuyển' thêm.");
                }
            }

            if (ModelState.IsValid && hoSo != null)
            {
                ketQuaTuyenDung.NgayCapNhat = DateTime.Now;
                _context.Add(ketQuaTuyenDung);

                // Cập nhật trạng thái cho Hồ sơ ứng tuyển (Trúng tuyển hoặc Không trúng tuyển)
                hoSo.TrangThai = ketQuaTuyenDung.KetQua;
                _context.Update(hoSo);

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Ghi nhận kết quả tuyển dụng thành công: Ứng viên '{hoSo.UngVien?.HoTen}' đã được cập nhật thành '{ketQuaTuyenDung.KetQua}'.";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.MaHoSo = ketQuaTuyenDung.MaHoSo;
            if (hoSo != null)
            {
                ViewBag.ThongTinHoSo = $"{hoSo.UngVien?.HoTen} - Vị trí: {hoSo.ViTriTuyenDung?.TenViTri}";
            }
            return View(ketQuaTuyenDung);
        }

        // ==============================================================================
        // 4. GET: SỬA KẾT QUẢ TUYỂN DỤNG (/KetQuaTuyenDung/Edit/5)
        // ==============================================================================
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            var vaiTro = HttpContext.Session.GetVaiTro();
            if (vaiTro != "Admin" && vaiTro != "Nhân sự")
            {
                return RedirectToAction("AccessDenied", "TaiKhoan");
            }

            if (id == null) return NotFound();

            var ketQuaTuyenDung = await _context.KetQuaTuyenDung
                .Include(k => k.HoSoUngTuyen)
                    .ThenInclude(h => h.UngVien)
                .Include(k => k.HoSoUngTuyen)
                    .ThenInclude(h => h.ViTriTuyenDung)
                .FirstOrDefaultAsync(k => k.MaKetQua == id);

            if (ketQuaTuyenDung == null) return NotFound();

            ViewBag.ThongTinHoSo = $"{ketQuaTuyenDung.HoSoUngTuyen?.UngVien?.HoTen} - Vị trí: {ketQuaTuyenDung.HoSoUngTuyen?.ViTriTuyenDung?.TenViTri}";
            return View(ketQuaTuyenDung);
        }

        // ==============================================================================
        // 5. POST: LƯU CHỈNH SỬA KẾT QUẢ TUYỂN DỤNG
        // ==============================================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaKetQua,MaHoSo,DiemDanhGia,NhanXet,KetQua")] KetQuaTuyenDung ketQuaTuyenDung)
        {
            var vaiTro = HttpContext.Session.GetVaiTro();
            if (vaiTro != "Admin" && vaiTro != "Nhân sự")
            {
                return RedirectToAction("AccessDenied", "TaiKhoan");
            }

            if (id != ketQuaTuyenDung.MaKetQua) return NotFound();

            var hoSo = await _context.HoSoUngTuyen
                .Include(h => h.ViTriTuyenDung)
                .Include(h => h.UngVien)
                .FirstOrDefaultAsync(h => h.MaHoSo == ketQuaTuyenDung.MaHoSo);

            // Kiểm tra chỉ tiêu nếu chỉnh sửa kết quả sang "Trúng tuyển" (trừ chính hồ sơ này)
            if (ketQuaTuyenDung.KetQua == "Trúng tuyển" && hoSo?.ViTriTuyenDung != null)
            {
                int soNguoiTrungTuyenKhac = await _context.HoSoUngTuyen
                    .CountAsync(h => h.MaViTri == hoSo.MaViTri && h.TrangThai == "Trúng tuyển" && h.MaHoSo != hoSo.MaHoSo);

                int chiTieu = hoSo.ViTriTuyenDung.SoLuongCanTuyen;
                if (soNguoiTrungTuyenKhac >= chiTieu)
                {
                    ModelState.AddModelError("KetQua", $"Vị trí '{hoSo.ViTriTuyenDung.TenViTri}' đã tuyển đủ chỉ tiêu ({chiTieu} người). Không thể chuyển sang 'Trúng tuyển'.");
                }
            }

            if (ModelState.IsValid && hoSo != null)
            {
                ketQuaTuyenDung.NgayCapNhat = DateTime.Now;
                _context.Update(ketQuaTuyenDung);

                // Đồng bộ lại trạng thái cho hồ sơ
                hoSo.TrangThai = ketQuaTuyenDung.KetQua;
                _context.Update(hoSo);

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Cập nhật kết quả tuyển dụng thành công!";
                return RedirectToAction(nameof(Index));
            }

            if (hoSo != null)
            {
                ViewBag.ThongTinHoSo = $"{hoSo.UngVien?.HoTen} - Vị trí: {hoSo.ViTriTuyenDung?.TenViTri}";
            }
            return View(ketQuaTuyenDung);
        }
    }
}