// Họ và tên: Hoàng Dương
// Mã sinh viên: [MãSV của bạn]
// Nội dung thực hiện: Xây dựng KetQuaTuyenDungController quản lý điểm đánh giá và kết quả cuối cùng của ứng viên.

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyTuyenDung_UNETI05_DHTI17A2HN.Models;
using QuanLyTuyenDung_UNETI05_DHTI17A2HN.Data;
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

        // 1. Danh sách kết quả tuyển dụng
        public async Task<IActionResult> Index()
        {
            var results = await _context.KetQuaTuyenDung
                .Include(k => k.HoSoUngTuyen)
                .ThenInclude(h => h.UngVien)
                .Include(k => k.HoSoUngTuyen)
                .ThenInclude(h => h.ViTriTuyenDung)
                .ToListAsync();
            return View(results);
        }

        // 2. GET: Nhập kết quả tuyển dụng cho hồ sơ
        public async Task<IActionResult> Create(int? maHoSo)
        {
            if (maHoSo == null)
            {
                return NotFound();
            }

            // Kiểm tra điều kiện: Chỉ hồ sơ "Đã phỏng vấn" mới được ghi nhận kết quả cuối cùng
            var hoSo = await _context.HoSoUngTuyen
                .Include(h => h.UngVien)
                .FirstOrDefaultAsync(h => h.MaHoSo == maHoSo);

            if (hoSo == null || hoSo.TrangThai != "Đã phỏng vấn")
            {
                TempData["Error"] = "Chỉ hồ sơ đã hoàn thành phỏng vấn mới được ghi nhận kết quả cuối cùng.";
                return RedirectToAction("Index", "HoSoUngTuyen");
            }

            // Kiểm tra nếu đã có kết quả trước đó thì chuyển sang trang Sửa
            var existingResult = await _context.KetQuaTuyenDung.FirstOrDefaultAsync(k => k.MaHoSo == maHoSo);
            if (existingResult != null)
            {
                return RedirectToAction(nameof(Edit), new { id = existingResult.MaKetQua });
            }

            ViewBag.MaHoSo = maHoSo;
            ViewBag.ThongTinHoSo = $"{hoSo.UngVien.HoTen} - Vị trí ID: {hoSo.MaViTri}";
            return View();
        }

        // 3. POST: Lưu kết quả tuyển dụng
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaHoSo,DiemDanhGia,NhanXet,KetQua")] KetQuaTuyenDung ketQuaTuyenDung)
        {
            bool hasResult = await _context.KetQuaTuyenDung.AnyAsync(k => k.MaHoSo == ketQuaTuyenDung.MaHoSo);
            if (hasResult)
            {
                ModelState.AddModelError("", "Hồ sơ này đã có kết quả tuyển dụng.");
            }

            if (ModelState.IsValid)
            {
                ketQuaTuyenDung.NgayCapNhat = DateTime.Now;
                _context.Add(ketQuaTuyenDung);

                // Cập nhật trạng thái cho Hồ sơ ứng tuyển (Trúng tuyển hoặc Không trúng tuyển)
                var hoSo = await _context.HoSoUngTuyen.FindAsync(ketQuaTuyenDung.MaHoSo);
                if (hoSo != null)
                {
                    hoSo.TrangThai = ketQuaTuyenDung.KetQua;
                    _context.Update(hoSo);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(ketQuaTuyenDung);
        }

        // 4. GET: Chỉnh sửa kết quả
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var ketQuaTuyenDung = await _context.KetQuaTuyenDung
                .Include(k => k.HoSoUngTuyen)
                .ThenInclude(h => h.UngVien)
                .FirstOrDefaultAsync(k => k.MaKetQua == id);

            if (ketQuaTuyenDung == null) return NotFound();

            return View(ketQuaTuyenDung);
        }

        // 5. POST: Lưu chỉnh sửa kết quả
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaKetQua,MaHoSo,DiemDanhGia,NhanXet,KetQua")] KetQuaTuyenDung ketQuaTuyenDung)
        {
            if (id != ketQuaTuyenDung.MaKetQua) return NotFound();

            if (ModelState.IsValid)
            {
                ketQuaTuyenDung.NgayCapNhat = DateTime.Now;
                _context.Update(ketQuaTuyenDung);

                // Đồng bộ lại trạng thái hồ sơ
                var hoSo = await _context.HoSoUngTuyen.FindAsync(ketQuaTuyenDung.MaHoSo);
                if (hoSo != null)
                {
                    hoSo.TrangThai = ketQuaTuyenDung.KetQua;
                    _context.Update(hoSo);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(ketQuaTuyenDung);
        }
    }
}