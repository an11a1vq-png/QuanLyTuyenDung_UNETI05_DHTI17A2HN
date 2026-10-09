// Họ và tên: Hoàng Dương
// Mã sinh viên: [MãSV của bạn]
// Nội dung thực hiện: Xây dựng LichPhongVanController, kiểm tra logic trùng lịch phỏng vấn bằng LINQ và cập nhật trạng thái.

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        // 1. Danh sách lịch phỏng vấn
        public async Task<IActionResult> Index()
        {
            var lichPhongVans = await _context.LichPhongVans
                .Include(l => l.HoSoUngTuyen)
                .ThenInclude(h => h.UngVien)
                .Include(l => l.HoSoUngTuyen)
                .ThenInclude(h => h.ViTriTuyenDung)
                .ToListAsync();
            return View(lichPhongVans);
        }

        // 2. GET: Tạo lịch phỏng vấn mới
        public async Task<IActionResult> Create(int? maHoSo)
        {
            if (maHoSo == null)
            {
                return NotFound();
            }

            // Kiểm tra hồ sơ có tồn tại và đạt sơ tuyển không
            var hoSo = await _context.HoSoUngTuyens
                .Include(h => h.UngVien)
                .FirstOrDefaultAsync(h => h.MaHoSo == maHoSo);

            if (hoSo == null || hoSo.TrangThai != "Đạt sơ tuyển")
            {
                TempData["Error"] = "Chỉ hồ sơ đạt sơ tuyển mới được lập lịch phỏng vấn.";
                return RedirectToAction("Index", "HoSoUngTuyen");
            }

            ViewBag.MaHoSo = maHoSo;
            ViewBag.ThongTinHoSo = $"{hoSo.UngVien.HoTen} - Vị trí ID: {hoSo.MaViTri}";
            return View();
        }

        // 3. POST: Xử lý tạo lịch phỏng vấn kèm kiểm tra nghiệp vụ
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaHoSo,ThoiGianBatDau,ThoiGianKetThuc,HinhThucPhongVan,DiaDiemHoacLienKet,NguoiPhongVan,GhiChu")] LichPhongVan lichPhongVan)
        {
            // Kiểm tra điều kiện thời gian cơ bản
            if (lichPhongVan.ThoiGianKetThuc <= lichPhongVan.ThoiGianBatDau)
            {
                ModelState.AddModelError("ThoiGianKetThuc", "Thời gian kết thúc phải sau thời gian bắt đầu.");
            }

            if (lichPhongVan.ThoiGianBatDau < DateTime.Now)
            {
                ModelState.AddModelError("ThoiGianBatDau", "Thời gian phỏng vấn không được nằm trong quá khứ.");
            }

            // Kiểm tra xem hồ sơ đã có lịch phỏng vấn đang hiệu lực nào chưa (trên các lịch chưa hủy)
            var existingScheduleForCandidate = await _context.LichPhongVans
                .AnyAsync(l => l.MaHoSo == lichPhongVan.MaHoSo && l.TrangThai != "Đã hủy");

            if (existingScheduleForCandidate)
            {
                ModelState.AddModelError("", "Hồ sơ này đã có một lịch phỏng vấn đang hiệu lực khác.");
            }

            // Kiểm tra trùng khoảng thời gian phỏng vấn của Ứng viên hoặc Người phỏng vấn (trên các lịch chưa hủy)
            var overlappingSchedule = await _context.LichPhongVans
                .Where(l => l.TrangThai != "Đã hủy")
                .Where(l => (l.MaHoSo == lichPhongVan.MaHoSo || l.NguoiPhongVan == lichPhongVan.NguoiPhongVan)
                            && lichPhongVan.ThoiGianBatDau < l.ThoiGianKetThuc
                            && lichPhongVan.ThoiGianKetThuc > l.ThoiGianBatDau)
                .AnyAsync();

            if (overlappingSchedule)
            {
                ModelState.AddModelError("", "Ứng viên hoặc Người phỏng vấn đã bị trùng lịch trong khoảng thời gian này.");
            }

            if (ModelState.IsValid)
            {
                lichPhongVan.TrangThai = "Đã lên lịch";
                _context.Add(lichPhongVan);

                // Cập nhật trạng thái hồ sơ thành Chờ phỏng vấn
                var hoSo = await _context.HoSoUngTuyens.FindAsync(lichPhongVan.MaHoSo);
                if (hoSo != null)
                {
                    hoSo.TrangThai = "Chờ phỏng vấn";
                    _context.Update(hoSo);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(lichPhongVan);
        }

        // 4. Cập nhật hoàn thành buổi phỏng vấn
        public async Task<IActionResult> HoanThanh(int id)
        {
            var lich = await _context.LichPhongVans
                .Include(l => l.HoSoUngTuyen)
                .FirstOrDefaultAsync(l => l.MaLichPhongVan == id);

            if (lich == null || lich.TrangThai == "Đã hủy")
            {
                TempData["Error"] = "Lịch phỏng vấn không tồn tại hoặc đã bị hủy.";
                return RedirectToAction(nameof(Index));
            }

            lich.TrangThai = "Đã hoàn thành";
            if (lich.HoSoUngTuyen != null)
            {
                lich.HoSoUngTuyen.TrangThai = "Đã phỏng vấn";
                _context.Update(lich.HoSoUngTuyen);
            }

            _context.Update(lich);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}