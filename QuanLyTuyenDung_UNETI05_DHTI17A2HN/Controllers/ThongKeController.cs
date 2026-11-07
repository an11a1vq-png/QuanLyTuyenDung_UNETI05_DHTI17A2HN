// ==============================================================================
// Họ và tên: Hoàng Dương
// Mã sinh viên: 23103100117
// Module 5: Lịch phỏng vấn, Kết quả tuyển dụng, Dashboard và Thống kê
// Nội dung thực hiện: Xây dựng ThongKeController, phân quyền Session, 
//                     thực hiện Dashboard, 10 truy vấn thống kê bằng LINQ và xuất báo cáo Excel theo Đề tài 13.
// ==============================================================================

using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyTuyenDung_UNETI05_DHTI17A2HN.Data;
using QuanLyTuyenDung_UNETI05_DHTI17A2HN.Helpers;
using QuanLyTuyenDung_UNETI05_DHTI17A2HN.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace QuanLyTuyenDung_UNETI05_DHTI17A2HN.Controllers
{
    public class ThongKeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ThongKeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ==============================================================================
        // DASHBOARD & BÁO CÁO THỐNG KÊ LINQ (/ThongKe hoặc /ThongKe/Index hoặc /ThongKe/Dashboard)
        // ==============================================================================
        [HttpGet]
        [Route("ThongKe")]
        [Route("ThongKe/Index")]
        [Route("ThongKe/Dashboard")]
        public async Task<IActionResult> Index(int? nam, int? thang)
        {
            // Kiểm tra phân quyền: Chỉ Admin và Nhân sự mới được xem Dashboard & Thống kê (Mục 5.4)
            var vaiTro = HttpContext.Session.GetVaiTro();
            if (vaiTro != "Admin" && vaiTro != "Nhân sự")
            {
                return RedirectToAction("AccessDenied", "TaiKhoan");
            }

            var model = new ThongKeViewModel();

            // Truyền bộ lọc xuống View để giữ nguyên giá trị dropdown
            ViewBag.NamFilter = nam;
            ViewBag.ThangFilter = thang;
            ViewBag.DanhSachNam = Enumerable.Range(2020, DateTime.Now.Year - 2020 + 1).Reverse().ToList();

            // ----------------------------------------------------------------------
            // 1. TÍNH TOÁN CÁC CHỈ SỐ DASHBOARD TỔNG QUAN (Mục 9.5 Đề tài 13)
            // ----------------------------------------------------------------------
            model.TongSoPhongBan = await _context.PhongBan.CountAsync();
            model.TongSoViTri = await _context.ViTriTuyenDung.CountAsync();
            model.SoViTriDangTuyen = await _context.ViTriTuyenDung.CountAsync(v => v.TrangThai == "Đang tuyển");
            model.TongSoUngVien = await _context.UngVien.CountAsync();
            model.TongSoHoSo = await _context.HoSoUngTuyen.CountAsync();
            model.SoHoSoChoDuyet = await _context.HoSoUngTuyen.CountAsync(h => h.TrangThai == "Chờ duyệt");
            model.SoHoSoDatSoTuyen = await _context.HoSoUngTuyen.CountAsync(h => h.TrangThai == "Đạt sơ tuyển");
            model.SoLichPVSapToi = await _context.LichPhongVan.CountAsync(l => l.TrangThai == "Đã lên lịch" && l.ThoiGianBatDau >= DateTime.Now);
            model.SoUngVienTrungTuyen = await _context.HoSoUngTuyen.CountAsync(h => h.TrangThai == "Trúng tuyển");

            // ----------------------------------------------------------------------
            // 2. 10 BÁO CÁO THỐNG KÊ BẰNG TRUY VẤN LINQ (Mục 9.6 Đề tài 13)
            // ----------------------------------------------------------------------

            // (1) Số vị trí tuyển dụng theo phòng ban bằng LINQ GroupBy
            model.ViTriTheoPhongBan = await _context.ViTriTuyenDung
                .Include(v => v.PhongBan)
                .GroupBy(v => v.PhongBan.TenPhongBan)
                .Select(g => new ItemThongKe
                {
                    Ten = g.Key,
                    SoLuong = g.Count()
                })
                .OrderByDescending(x => x.SoLuong)
                .ToListAsync();

            // (2) Số hồ sơ theo từng vị trí tuyển dụng bằng LINQ GroupBy
            model.HoSoTheoViTri = await _context.HoSoUngTuyen
                .Include(h => h.ViTriTuyenDung)
                .GroupBy(h => h.ViTriTuyenDung.TenViTri)
                .Select(g => new ItemThongKe
                {
                    Ten = g.Key,
                    SoLuong = g.Count()
                })
                .OrderByDescending(x => x.SoLuong)
                .ToListAsync();

            // (3) Số hồ sơ theo trạng thái
            model.HoSoTheoTrangThai = await _context.HoSoUngTuyen
                .GroupBy(h => h.TrangThai)
                .Select(g => new ItemThongKe
                {
                    Ten = g.Key,
                    SoLuong = g.Count()
                })
                .OrderByDescending(x => x.SoLuong)
                .ToListAsync();

            // (4) Số hồ sơ đạt sơ tuyển theo vị trí
            model.DatSoTuyenTheoViTri = await _context.HoSoUngTuyen
                .Include(h => h.ViTriTuyenDung)
                .Where(h => h.TrangThai != "Chờ duyệt" && h.TrangThai != "Không đạt sơ tuyển" && h.TrangThai != "Đã hủy")
                .GroupBy(h => h.ViTriTuyenDung.TenViTri)
                .Select(g => new ItemThongKe
                {
                    Ten = g.Key,
                    SoLuong = g.Count()
                })
                .OrderByDescending(x => x.SoLuong)
                .ToListAsync();

            // (5) Vị trí nhận nhiều hồ sơ nhất bằng LINQ
            var topViTri = await _context.HoSoUngTuyen
                .Include(h => h.ViTriTuyenDung)
                .GroupBy(h => h.ViTriTuyenDung.TenViTri)
                .Select(g => new
                {
                    TenViTri = g.Key,
                    SoHoSo = g.Count()
                })
                .OrderByDescending(x => x.SoHoSo)
                .FirstOrDefaultAsync();

            if (topViTri != null)
            {
                model.ViTriNhieuHoSoNhat = topViTri.TenViTri;
                model.SoHoSoNhieuNhat = topViTri.SoHoSo;
            }

            // (6) Số ứng viên trúng tuyển theo phòng ban bằng LINQ
            model.TrungTuyenTheoPhongBan = await _context.HoSoUngTuyen
                .Include(h => h.ViTriTuyenDung)
                    .ThenInclude(v => v.PhongBan)
                .Where(h => h.TrangThai == "Trúng tuyển")
                .GroupBy(h => h.ViTriTuyenDung.PhongBan.TenPhongBan)
                .Select(g => new ItemThongKe
                {
                    Ten = g.Key,
                    SoLuong = g.Count()
                })
                .OrderByDescending(x => x.SoLuong)
                .ToListAsync();

            // (7) Số hồ sơ nộp theo tháng (lọc theo năm/tháng nếu có)
            var queryHoSoThang = _context.HoSoUngTuyen.AsQueryable();
            if (nam.HasValue) queryHoSoThang = queryHoSoThang.Where(h => h.NgayNop.Year == nam.Value);
            if (thang.HasValue) queryHoSoThang = queryHoSoThang.Where(h => h.NgayNop.Month == thang.Value);

            model.HoSoTheoThang = await queryHoSoThang
                .GroupBy(h => new { h.NgayNop.Year, h.NgayNop.Month })
                .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
                .Select(g => new ItemThongKe
                {
                    Ten = $"{g.Key.Month:D2}/{g.Key.Year}",
                    SoLuong = g.Count()
                })
                .ToListAsync();

            // (8) Số cuộc phỏng vấn theo tháng (lọc theo năm/tháng nếu có)
            var queryLichThang = _context.LichPhongVan.AsQueryable();
            if (nam.HasValue) queryLichThang = queryLichThang.Where(l => l.ThoiGianBatDau.Year == nam.Value);
            if (thang.HasValue) queryLichThang = queryLichThang.Where(l => l.ThoiGianBatDau.Month == thang.Value);

            model.PhongVanTheoThang = await queryLichThang
                .GroupBy(l => new { l.ThoiGianBatDau.Year, l.ThoiGianBatDau.Month })
                .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
                .Select(g => new ItemThongKe
                {
                    Ten = $"{g.Key.Month:D2}/{g.Key.Year}",
                    SoLuong = g.Count()
                })
                .ToListAsync();

            // (9) Tỷ lệ hồ sơ qua sơ tuyển theo từng vị trí
            var allHoSoList = await _context.HoSoUngTuyen
                .Include(h => h.ViTriTuyenDung)
                .ToListAsync();

            model.TyLeSoTuyenTheoViTri = allHoSoList
                .GroupBy(h => h.ViTriTuyenDung?.TenViTri ?? "Khác")
                .Select(g =>
                {
                    int tong = g.Count();
                    int dat = g.Count(h => h.TrangThai != "Chờ duyệt" && h.TrangThai != "Không đạt sơ tuyển" && h.TrangThai != "Đã hủy");
                    return new TyLeSoTuyenItem
                    {
                        TenViTri = g.Key,
                        TongSoHoSo = tong,
                        SoLuongQuaSoTuyen = dat,
                        TyLePhanTram = tong > 0 ? Math.Round((double)dat / tong * 100, 1) : 0
                    };
                })
                .OrderByDescending(x => x.TyLePhanTram)
                .ToList();

            // (10) Tỷ lệ trúng tuyển trên số hồ sơ đã phỏng vấn
            model.TongDaPhongVan = await _context.HoSoUngTuyen
                .CountAsync(h => h.TrangThai == "Đã phỏng vấn" || h.TrangThai == "Trúng tuyển" || h.TrangThai == "Không trúng tuyển");
            model.TongTrungTuyen = model.SoUngVienTrungTuyen;
            model.TyLeTrungTuyenTrenPhongVan = model.TongDaPhongVan > 0
                ? Math.Round((double)model.TongTrungTuyen / model.TongDaPhongVan * 100, 1)
                : 0;

            return View(model);
        }

        // ==============================================================================
        // 3. XUẤT BÁO CÁO THỐNG KÊ TUYỂN DỤNG RA FILE EXCEL (.XLSX)
        // ==============================================================================
        [HttpGet]
        public async Task<IActionResult> XuatBaoCaoThongKeExcel(int? nam, int? thang)
        {
            var vaiTro = HttpContext.Session.GetVaiTro();
            if (vaiTro != "Admin" && vaiTro != "Nhân sự")
            {
                return RedirectToAction("AccessDenied", "TaiKhoan");
            }

            using var workbook = new XLWorkbook();

            // -------------------------------------------------------------
            // SHEET 1: CHỈ SỐ DASHBOARD TỔNG QUAN
            // -------------------------------------------------------------
            var wsTongQuan = workbook.Worksheets.Add("ChiSoTongQuan");
            wsTongQuan.Cell(1, 1).Value = "CHỈ SỐ BÁO CÁO TUYỂN DỤNG";
            wsTongQuan.Cell(1, 2).Value = "SỐ LƯỢNG";

            var h1 = wsTongQuan.Row(1);
            h1.Style.Font.Bold = true;
            h1.Style.Fill.BackgroundColor = XLColor.FromHtml("#198754");
            h1.Style.Font.FontColor = XLColor.White;

            int tongPB = await _context.PhongBan.CountAsync();
            int tongViTri = await _context.ViTriTuyenDung.CountAsync();
            int viTriDangTuyen = await _context.ViTriTuyenDung.CountAsync(v => v.TrangThai == "Đang tuyển");
            int tongUngVien = await _context.UngVien.CountAsync();
            int tongHoSo = await _context.HoSoUngTuyen.CountAsync();
            int hoSoChoDuyet = await _context.HoSoUngTuyen.CountAsync(h => h.TrangThai == "Chờ duyệt");
            int hoSoDatSoTuyen = await _context.HoSoUngTuyen.CountAsync(h => h.TrangThai == "Đạt sơ tuyển");
            int ungVienTrungTuyen = await _context.HoSoUngTuyen.CountAsync(h => h.TrangThai == "Trúng tuyển");

            var stats = new List<(string Name, int Val)>
            {
                ("Tổng số phòng ban", tongPB),
                ("Tổng số vị trí tuyển dụng", tongViTri),
                ("Số vị trí đang mở nhận hồ sơ", viTriDangTuyen),
                ("Tổng số ứng viên trong hệ thống", tongUngVien),
                ("Tổng số hồ sơ ứng tuyển đã tiếp nhận", tongHoSo),
                ("Số hồ sơ đang chờ duyệt sơ tuyển", hoSoChoDuyet),
                ("Số hồ sơ đã đạt sơ tuyển", hoSoDatSoTuyen),
                ("Số ứng viên đã trúng tuyển chính thức", ungVienTrungTuyen)
            };

            for (int i = 0; i < stats.Count; i++)
            {
                wsTongQuan.Cell(i + 2, 1).Value = stats[i].Name;
                wsTongQuan.Cell(i + 2, 2).Value = stats[i].Val;
            }
            wsTongQuan.Columns().AdjustToContents();

            // -------------------------------------------------------------
            // SHEET 2: CHI TIẾT THEO VỊ TRÍ TUYỂN DỤNG
            // -------------------------------------------------------------
            var wsViTri = workbook.Worksheets.Add("ThongKeViTri");
            wsViTri.Cell(1, 1).Value = "STT";
            wsViTri.Cell(1, 2).Value = "Vị trí tuyển dụng";
            wsViTri.Cell(1, 3).Value = "Phòng ban";
            wsViTri.Cell(1, 4).Value = "Chỉ tiêu tuyển";
            wsViTri.Cell(1, 5).Value = "Hồ sơ tiếp nhận";
            wsViTri.Cell(1, 6).Value = "Trúng tuyển";

            var h2 = wsViTri.Row(1);
            h2.Style.Font.Bold = true;
            h2.Style.Fill.BackgroundColor = XLColor.FromHtml("#0D6EFD");
            h2.Style.Font.FontColor = XLColor.White;

            var dsViTri = await _context.ViTriTuyenDung
                .Include(v => v.PhongBan)
                .Include(v => v.HoSoUngTuyens)
                .ToListAsync();

            int r = 2;
            for (int i = 0; i < dsViTri.Count; i++)
            {
                var vt = dsViTri[i];
                int hsCount = vt.HoSoUngTuyens?.Count ?? 0;
                int ttCount = vt.HoSoUngTuyens?.Count(h => h.TrangThai == "Trúng tuyển") ?? 0;

                wsViTri.Cell(r, 1).Value = i + 1;
                wsViTri.Cell(r, 2).Value = vt.TenViTri;
                wsViTri.Cell(r, 3).Value = vt.PhongBan?.TenPhongBan;
                wsViTri.Cell(r, 4).Value = vt.SoLuongCanTuyen;
                wsViTri.Cell(r, 5).Value = hsCount;
                wsViTri.Cell(r, 6).Value = ttCount;
                r++;
            }
            wsViTri.Columns().AdjustToContents();

            // -------------------------------------------------------------
            // SHEET 3: THỐNG KÊ XU HƯỚNG THEO THÁNG & NĂM
            // -------------------------------------------------------------
            var wsThang = workbook.Worksheets.Add("ThongKeTheoThang");

            // Truy vấn dữ liệu theo tháng có áp dụng bộ lọc (nếu có)
            var qHoSo = _context.HoSoUngTuyen.AsQueryable();
            if (nam.HasValue) qHoSo = qHoSo.Where(h => h.NgayNop.Year == nam.Value);
            if (thang.HasValue) qHoSo = qHoSo.Where(h => h.NgayNop.Month == thang.Value);

            var dsHoSoThang = await qHoSo
                .GroupBy(h => new { h.NgayNop.Year, h.NgayNop.Month })
                .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
                .Select(g => new { ThangNam = $"{g.Key.Month:D2}/{g.Key.Year}", SoLuong = g.Count() })
                .ToListAsync();

            var qLich = _context.LichPhongVan.AsQueryable();
            if (nam.HasValue) qLich = qLich.Where(l => l.ThoiGianBatDau.Year == nam.Value);
            if (thang.HasValue) qLich = qLich.Where(l => l.ThoiGianBatDau.Month == thang.Value);

            var dsLichThang = await qLich
                .GroupBy(l => new { l.ThoiGianBatDau.Year, l.ThoiGianBatDau.Month })
                .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
                .Select(g => new { ThangNam = $"{g.Key.Month:D2}/{g.Key.Year}", SoLuong = g.Count() })
                .ToListAsync();

            // Tiêu đề Bảng 1: Số hồ sơ nộp theo tháng
            wsThang.Cell(1, 1).Value = "THÁNG / NĂM";
            wsThang.Cell(1, 2).Value = "SỐ HỒ SƠ NỘP";
            var th1 = wsThang.Range("A1:B1");
            th1.Style.Font.Bold = true;
            th1.Style.Fill.BackgroundColor = XLColor.FromHtml("#0D6EFD");
            th1.Style.Font.FontColor = XLColor.White;

            for (int i = 0; i < dsHoSoThang.Count; i++)
            {
                wsThang.Cell(i + 2, 1).Value = dsHoSoThang[i].ThangNam;
                wsThang.Cell(i + 2, 2).Value = dsHoSoThang[i].SoLuong;
            }

            // Tiêu đề Bảng 2: Số cuộc phỏng vấn theo tháng
            wsThang.Cell(1, 4).Value = "THÁNG / NĂM";
            wsThang.Cell(1, 5).Value = "SỐ CUỘC PHỎNG VẤN";
            var th2 = wsThang.Range("D1:E1");
            th2.Style.Font.Bold = true;
            th2.Style.Fill.BackgroundColor = XLColor.FromHtml("#198754");
            th2.Style.Font.FontColor = XLColor.White;

            for (int i = 0; i < dsLichThang.Count; i++)
            {
                wsThang.Cell(i + 2, 4).Value = dsLichThang[i].ThangNam;
                wsThang.Cell(i + 2, 5).Value = dsLichThang[i].SoLuong;
            }

            wsThang.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            string fileSuffix = (nam.HasValue || thang.HasValue) ? $"_T{thang}_N{nam}" : "";
            return File(stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"BaoCao_ThongKe_TuyenDung{fileSuffix}_{DateTime.Now:yyyyMMdd_HHmm}.xlsx");
        }
    }
}
