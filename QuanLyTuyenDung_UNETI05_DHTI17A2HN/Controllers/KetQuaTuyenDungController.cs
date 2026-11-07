// ==============================================================================
// Họ và tên: Hoàng Dương
// Mã sinh viên: 23103100117
// Module 5: Lịch phỏng vấn, Kết quả tuyển dụng, Dashboard và Thống kê
// Nội dung thực hiện: Xây dựng KetQuaTuyenDungController, phân quyền Session, 
//                     kiểm soát chỉ tiêu bằng LINQ, ghi nhận, chỉnh sửa và xuất file Excel kết quả.
// ==============================================================================

using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyTuyenDung_UNETI05_DHTI17A2HN.Data;
using QuanLyTuyenDung_UNETI05_DHTI17A2HN.Helpers;
using QuanLyTuyenDung_UNETI05_DHTI17A2HN.Models;
using System;
using System.IO;
using System.Threading.Tasks;

namespace QuanLyTuyenDung_UNETI05_DHTI17A2HN.Controllers
{
    public class KetQuaTuyenDungController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public KetQuaTuyenDungController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // ==============================================================================
        // 1. DANH SÁCH KẾT QUẢ TUYỂN DỤNG (/KetQuaTuyenDung/Index)
        // ==============================================================================
        [HttpGet]
        public async Task<IActionResult> Index(string? ketQua, string? tenViTri, string? tenUngVien, DateTime? tuNgay, DateTime? denNgay)
        {
            // Kiểm tra phân quyền: Chỉ Admin và Nhân sự mới được xem/quản lý kết quả tuyển dụng (Mục 5.4 đề tài)
            var vaiTro = HttpContext.Session.GetVaiTro();
            if (vaiTro != "Admin" && vaiTro != "Nhân sự")
            {
                return RedirectToAction("AccessDenied", "TaiKhoan");
            }

            var query = _context.KetQuaTuyenDung
                .Include(k => k.HoSoUngTuyen)
                    .ThenInclude(h => h.UngVien)
                .Include(k => k.HoSoUngTuyen)
                    .ThenInclude(h => h.ViTriTuyenDung)
                .AsQueryable();

            // Lọc theo kết quả (Trúng tuyển / Không trúng tuyển)
            if (!string.IsNullOrWhiteSpace(ketQua))
                query = query.Where(k => k.KetQua == ketQua);

            // Lọc theo tên vị trí tuyển dụng
            if (!string.IsNullOrWhiteSpace(tenViTri))
                query = query.Where(k => k.HoSoUngTuyen.ViTriTuyenDung.TenViTri.Contains(tenViTri));

            // Tìm kiếm theo tên ứng viên
            if (!string.IsNullOrWhiteSpace(tenUngVien))
                query = query.Where(k => k.HoSoUngTuyen.UngVien.HoTen.Contains(tenUngVien));

            // Lọc theo ngày đánh giá
            if (tuNgay.HasValue)
                query = query.Where(k => k.NgayCapNhat.Date >= tuNgay.Value.Date);
            if (denNgay.HasValue)
                query = query.Where(k => k.NgayCapNhat.Date <= denNgay.Value.Date);

            var results = await query
                .OrderByDescending(k => k.NgayCapNhat)
                .ToListAsync();

            // Truyền lại giá trị bộ lọc để View giữ nguyên trên form
            ViewBag.KetQuaFilter = ketQua;
            ViewBag.TenViTriFilter = tenViTri;
            ViewBag.TenUngVienFilter = tenUngVien;
            ViewBag.TuNgayFilter = tuNgay?.ToString("yyyy-MM-dd");
            ViewBag.DenNgayFilter = denNgay?.ToString("yyyy-MM-dd");

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
                // Gửi email thông báo kết quả tuyển dụng cho ứng viên (Trúng tuyển / Không trúng tuyển)
                var emailUV = hoSo.UngVien?.Email;
                if (!string.IsNullOrWhiteSpace(emailUV))
                {
                    var tenUV = hoSo.UngVien?.HoTen ?? "Ứng viên";
                    var tenViTri = hoSo.ViTriTuyenDung?.TenViTri ?? "";
                    _ = Task.Run(() => EmailHelper.SendThongBaoKetQuaPhongVanAsync(
                        emailUV, tenUV, ketQuaTuyenDung.KetQua, tenViTri, _configuration));
                }
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

        // ==============================================================================
        // 6. XUẤT DANH SÁCH KẾT QUẢ TUYỂN DỤNG RA FILE EXCEL (.XLSX)
        // ==============================================================================
        [HttpGet]
        public async Task<IActionResult> XuatExcel(string? ketQua, string? tenViTri, string? tenUngVien)
        {
            var vaiTro = HttpContext.Session.GetVaiTro();
            if (vaiTro != "Admin" && vaiTro != "Nhân sự")
            {
                return RedirectToAction("AccessDenied", "TaiKhoan");
            }

            var query = _context.KetQuaTuyenDung
                .Include(k => k.HoSoUngTuyen)
                    .ThenInclude(h => h.UngVien)
                .Include(k => k.HoSoUngTuyen)
                    .ThenInclude(h => h.ViTriTuyenDung)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(ketQua))
                query = query.Where(k => k.KetQua == ketQua);

            if (!string.IsNullOrWhiteSpace(tenViTri))
                query = query.Where(k => k.HoSoUngTuyen.ViTriTuyenDung.TenViTri.Contains(tenViTri));

            if (!string.IsNullOrWhiteSpace(tenUngVien))
                query = query.Where(k => k.HoSoUngTuyen.UngVien.HoTen.Contains(tenUngVien));

            var dsKetQua = await query.OrderByDescending(k => k.NgayCapNhat).ToListAsync();

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("KetQuaTuyenDung");

            // 1. Dòng tiêu đề bảng
            worksheet.Cell(1, 1).Value = "STT";
            worksheet.Cell(1, 2).Value = "Mã HS";
            worksheet.Cell(1, 3).Value = "Họ và tên ứng viên";
            worksheet.Cell(1, 4).Value = "Vị trí ứng tuyển";
            worksheet.Cell(1, 5).Value = "Điểm đánh giá";
            worksheet.Cell(1, 6).Value = "Kết quả tuyển dụng";
            worksheet.Cell(1, 7).Value = "Ngày công bố";
            worksheet.Cell(1, 8).Value = "Nhận xét của Hội đồng";

            // Định dạng tiêu đề: In đậm + nền màu xanh + chữ trắng
            var headerRow = worksheet.Row(1);
            headerRow.Style.Font.Bold = true;
            headerRow.Style.Fill.BackgroundColor = XLColor.FromHtml("#0D6EFD");
            headerRow.Style.Font.FontColor = XLColor.White;

            // 2. Đổ dữ liệu từng dòng
            int row = 2;
            for (int i = 0; i < dsKetQua.Count; i++)
            {
                var item = dsKetQua[i];
                worksheet.Cell(row, 1).Value = i + 1;
                worksheet.Cell(row, 2).Value = $"#{item.MaHoSo}";
                worksheet.Cell(row, 3).Value = item.HoSoUngTuyen?.UngVien?.HoTen;
                worksheet.Cell(row, 4).Value = item.HoSoUngTuyen?.ViTriTuyenDung?.TenViTri;
                worksheet.Cell(row, 5).Value = item.DiemDanhGia;
                worksheet.Cell(row, 6).Value = item.KetQua;
                worksheet.Cell(row, 7).Value = item.NgayCapNhat.ToString("dd/MM/yyyy");
                worksheet.Cell(row, 8).Value = item.NhanXet;
                row++;
            }

            // Tự động căn chỉnh độ rộng các cột
            worksheet.Columns().AdjustToContents();

            // 3. Trả file về trình duyệt tải xuống
            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return File(stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"DanhSach_KetQuaTuyenDung_{DateTime.Now:yyyyMMdd_HHmm}.xlsx");
        }
    }
}
