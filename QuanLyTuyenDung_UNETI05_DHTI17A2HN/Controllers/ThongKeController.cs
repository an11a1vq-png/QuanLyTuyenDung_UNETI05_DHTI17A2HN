// ==============================================================================
// Họ và tên: Hoàng Dương
// Mã sinh viên: 23103100117
// Module 5: Lịch phỏng vấn, Kết quả tuyển dụng, Dashboard và Thống kê
// Nội dung thực hiện: Xây dựng ThongKeController, phân quyền Session, 
//                     thực hiện Dashboard và 10 truy vấn thống kê bằng LINQ theo Đề tài 13.
// ==============================================================================

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyTuyenDung_UNETI05_DHTI17A2HN.Data;
using QuanLyTuyenDung_UNETI05_DHTI17A2HN.Helpers;
using QuanLyTuyenDung_UNETI05_DHTI17A2HN.ViewModels;
using System;
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
        public async Task<IActionResult> Index()
        {
            // Kiểm tra phân quyền: Chỉ Admin và Nhân sự mới được xem Dashboard & Thống kê (Mục 5.4)
            var vaiTro = HttpContext.Session.GetVaiTro();
            if (vaiTro != "Admin" && vaiTro != "Nhân sự")
            {
                return RedirectToAction("AccessDenied", "TaiKhoan");
            }

            var model = new ThongKeViewModel();

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

            // (7) Số hồ sơ nộp theo tháng bằng LINQ
            model.HoSoTheoThang = await _context.HoSoUngTuyen
                .GroupBy(h => new { h.NgayNop.Year, h.NgayNop.Month })
                .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
                .Select(g => new ItemThongKe
                {
                    Ten = $"{g.Key.Month:D2}/{g.Key.Year}",
                    SoLuong = g.Count()
                })
                .ToListAsync();

            // (8) Số cuộc phỏng vấn theo tháng bằng LINQ
            model.PhongVanTheoThang = await _context.LichPhongVan
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
    }
}
