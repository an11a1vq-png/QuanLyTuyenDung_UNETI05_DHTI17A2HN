
// Họ và tên: Nguyễn Văn Khánh
// Mã sinh viên: 23103100101
// Nội dung thực hiện: Module 4 - Tiếp nhận hồ sơ, sơ tuyển và quản lý trạng thái tuyển dụng.

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyTuyenDung_UNETI05_DHTI17A2HN.Data;
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

        // ============================================================
        // KIỂM TRA QUYỀN
        // Module này dành cho Admin và Nhân sự
        // ============================================================
        private bool CoQuyenNhanSu()
        {
            var vaiTro = HttpContext.Session.GetString("VaiTro");

            return vaiTro == "Admin" || vaiTro == "NhanSu";
        }

        // ============================================================
        // INDEX
        // Danh sách hồ sơ + tìm kiếm + lọc + sắp xếp
        // ============================================================
        [HttpGet]
        public async Task<IActionResult> Index(
            string? keyword,
            int? maPhongBan,
            string? trangThai,
            DateTime? tuNgay,
            DateTime? denNgay,
            string? sort)
        {
            // Kiểm tra quyền
            if (!CoQuyenNhanSu())
            {
                return RedirectToAction("AccessDenied", "TaiKhoan");
            }

            // Lấy hồ sơ kèm ứng viên, vị trí và phòng ban
            IQueryable<HoSoUngTuyen> query = _context.HoSoUngTuyen
                .Include(h => h.UngVien)
                .Include(h => h.ViTriTuyenDung)
                    .ThenInclude(v => v.PhongBan);

            // --------------------------------------------------------
            // TÌM KIẾM
            // Tìm theo tên ứng viên hoặc tên vị trí
            // --------------------------------------------------------
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.Trim();

                query = query.Where(h =>
                    (h.UngVien != null &&
                     h.UngVien.HoTen.Contains(keyword))
                    ||
                    (h.ViTriTuyenDung != null &&
                     h.ViTriTuyenDung.TenViTri.Contains(keyword))
                );
            }

            // --------------------------------------------------------
            // LỌC THEO PHÒNG BAN
            // --------------------------------------------------------
            if (maPhongBan.HasValue)
            {
                query = query.Where(h =>
                    h.ViTriTuyenDung != null &&
                    h.ViTriTuyenDung.MaPhongBan == maPhongBan.Value);
            }

            // --------------------------------------------------------
            // LỌC THEO TRẠNG THÁI
            // --------------------------------------------------------
            if (!string.IsNullOrWhiteSpace(trangThai))
            {
                query = query.Where(h =>
                    h.TrangThai == trangThai);
            }

            // --------------------------------------------------------
            // LỌC TỪ NGÀY
            // --------------------------------------------------------
            if (tuNgay.HasValue)
            {
                var ngayBatDau = tuNgay.Value.Date;

                query = query.Where(h =>
                    h.NgayNop >= ngayBatDau);
            }

            // --------------------------------------------------------
            // LỌC ĐẾN NGÀY
            // --------------------------------------------------------
            if (denNgay.HasValue)
            {
                // Lấy đến hết ngày được chọn
                var ngayKetThuc = denNgay.Value.Date.AddDays(1);

                query = query.Where(h =>
                    h.NgayNop < ngayKetThuc);
            }

            // --------------------------------------------------------
            // SẮP XẾP
            // --------------------------------------------------------
            switch (sort)
            {
                case "ngay-cu":
                    query = query.OrderBy(h => h.NgayNop);
                    break;

                case "ngay-moi":
                    query = query.OrderByDescending(h => h.NgayNop);
                    break;

                case "ten-az":
                    query = query
                        .OrderBy(h => h.UngVien != null
                            ? h.UngVien.HoTen
                            : "");
                    break;

                case "ten-za":
                    query = query
                        .OrderByDescending(h => h.UngVien != null
                            ? h.UngVien.HoTen
                            : "");
                    break;

                default:
                    // Mặc định hồ sơ mới nhất lên trước
                    query = query.OrderByDescending(h => h.NgayNop);
                    break;
            }

            var danhSachHoSo = await query.ToListAsync();

            // Lấy danh sách phòng ban để View tạo bộ lọc
            ViewBag.PhongBans = await _context.PhongBan
                .Where(p => p.TrangThai)
                .OrderBy(p => p.TenPhongBan)
                .ToListAsync();

            // Giữ lại điều kiện tìm kiếm/lọc để View sử dụng
            ViewBag.Keyword = keyword;
            ViewBag.MaPhongBan = maPhongBan;
            ViewBag.TrangThai = trangThai;
            ViewBag.TuNgay = tuNgay;
            ViewBag.DenNgay = denNgay;
            ViewBag.Sort = sort;

            return View(danhSachHoSo);
        }

        // ============================================================
        // DETAILS
        // Xem chi tiết hồ sơ
        // ============================================================
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (!CoQuyenNhanSu())
            {
                return RedirectToAction("AccessDenied", "TaiKhoan");
            }

            if (!id.HasValue)
            {
                return NotFound();
            }

            var hoSo = await _context.HoSoUngTuyen
                .Include(h => h.UngVien)
                .Include(h => h.ViTriTuyenDung)
                    .ThenInclude(v => v.PhongBan)
                .FirstOrDefaultAsync(h => h.MaHoSo == id.Value);

            if (hoSo == null)
            {
                return NotFound();
            }

            return View(hoSo);
        }

        // ============================================================
        // Mở màn hình sơ tuyển
        // ============================================================
        [HttpGet]
        public async Task<IActionResult> SoTuyen(int? id)
        {
            if (!CoQuyenNhanSu())
            {
                return RedirectToAction("AccessDenied", "TaiKhoan");
            }

            if (!id.HasValue)
            {
                return NotFound();
            }

            var hoSo = await _context.HoSoUngTuyen
                .Include(h => h.UngVien)
                .Include(h => h.ViTriTuyenDung)
                    .ThenInclude(v => v.PhongBan)
                .FirstOrDefaultAsync(h => h.MaHoSo == id.Value);

            if (hoSo == null)
            {
                return NotFound();
            }

            // Chỉ hồ sơ Chờ duyệt mới được sơ tuyển
            if (hoSo.TrangThai != "Chờ duyệt")
            {
                TempData["ErrorMessage"] =
                    "Hồ sơ này đã được xử lý, không thể sơ tuyển lại.";

                return RedirectToAction(nameof(Index));
            }

            // Kiểm tra ứng viên
            if (hoSo.UngVien == null)
            {
                TempData["ErrorMessage"] =
                    "Không tìm thấy thông tin ứng viên.";

                return RedirectToAction(nameof(Index));
            }

            // Kiểm tra ứng viên còn hoạt động
            if (!hoSo.UngVien.TrangThai)
            {
                TempData["ErrorMessage"] =
                    "Ứng viên hiện đang bị khóa hoặc không còn hoạt động.";

                return RedirectToAction(nameof(Index));
            }

            // Kiểm tra vị trí tuyển dụng
            if (hoSo.ViTriTuyenDung == null)
            {
                TempData["ErrorMessage"] =
                    "Không tìm thấy vị trí tuyển dụng của hồ sơ.";

                return RedirectToAction(nameof(Index));
            }

            return View(hoSo);
        }

        // ============================================================
        // SO TUYỂN - POST
        // Xử lý Đạt sơ tuyển / Không đạt sơ tuyển
        // ============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SoTuyen(
            int id,
            string ketQua,
            string? nhanXet)
        {
            if (!CoQuyenNhanSu())
            {
                return RedirectToAction("AccessDenied", "TaiKhoan");
            }

            // --------------------------------------------------------
            // Kiểm tra kết quả được gửi lên
            // --------------------------------------------------------
            if (ketQua != "Đạt sơ tuyển" &&
                ketQua != "Không đạt sơ tuyển")
            {
                TempData["ErrorMessage"] =
                    "Kết quả sơ tuyển không hợp lệ.";

                return RedirectToAction(nameof(Index));
            }

            // --------------------------------------------------------
            // Lấy hồ sơ
            // --------------------------------------------------------
            var hoSo = await _context.HoSoUngTuyen
                .Include(h => h.UngVien)
                .Include(h => h.ViTriTuyenDung)
                    .ThenInclude(v => v.PhongBan)
                .FirstOrDefaultAsync(h => h.MaHoSo == id);

            if (hoSo == null)
            {
                TempData["ErrorMessage"] =
                    "Không tìm thấy hồ sơ ứng tuyển.";

                return RedirectToAction(nameof(Index));
            }

            // --------------------------------------------------------
            // Chỉ hồ sơ Chờ duyệt mới được sơ tuyển
            // --------------------------------------------------------
            if (hoSo.TrangThai != "Chờ duyệt")
            {
                TempData["ErrorMessage"] =
                    "Chỉ hồ sơ đang Chờ duyệt mới được sơ tuyển.";

                return RedirectToAction(nameof(Index));
            }

            // --------------------------------------------------------
            // Kiểm tra hồ sơ chưa bị hủy
            // --------------------------------------------------------
            if (hoSo.TrangThai == "Đã hủy")
            {
                TempData["ErrorMessage"] =
                    "Hồ sơ đã bị hủy, không thể sơ tuyển.";

                return RedirectToAction(nameof(Index));
            }

            // --------------------------------------------------------
            // Kiểm tra ứng viên
            // --------------------------------------------------------
            if (hoSo.UngVien == null)
            {
                TempData["ErrorMessage"] =
                    "Không tìm thấy thông tin ứng viên.";

                return RedirectToAction(nameof(Index));
            }

            if (!hoSo.UngVien.TrangThai)
            {
                TempData["ErrorMessage"] =
                    "Ứng viên không còn hoạt động.";

                return RedirectToAction(nameof(Index));
            }

            // --------------------------------------------------------
            // Kiểm tra vị trí
            // --------------------------------------------------------
            if (hoSo.ViTriTuyenDung == null)
            {
                TempData["ErrorMessage"] =
                    "Không tìm thấy vị trí tuyển dụng.";

                return RedirectToAction(nameof(Index));
            }

            // --------------------------------------------------------
            // Kiểm tra đã từng xử lý sơ tuyển chưa
            // --------------------------------------------------------
            if (hoSo.NgayXuLy.HasValue)
            {
                TempData["ErrorMessage"] =
                    "Hồ sơ đã có ngày xử lý sơ tuyển.";

                return RedirectToAction(nameof(Index));
            }

            // --------------------------------------------------------
            // Cập nhật kết quả sơ tuyển
            // --------------------------------------------------------
            hoSo.TrangThai = ketQua;
            hoSo.NgayXuLy = DateTime.Now;
            hoSo.NhanXetSoTuyen = nhanXet?.Trim();

            await _context.SaveChangesAsync();

            // --------------------------------------------------------
            // Thông báo kết quả
            // --------------------------------------------------------
            if (ketQua == "Đạt sơ tuyển")
            {
                TempData["SuccessMessage"] =
                    "Đã cập nhật hồ sơ đạt sơ tuyển thành công.";
            }
            else
            {
                TempData["SuccessMessage"] =
                    "Đã cập nhật hồ sơ không đạt sơ tuyển.";
            }

            return RedirectToAction(nameof(Index));
        }

        // ============================================================
        // HỦY HỒ SƠ
        // Không xóa bản ghi, chỉ chuyển trạng thái thành Đã hủy
        // ============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Huy(int id)
        {
            if (!CoQuyenNhanSu())
            {
                return RedirectToAction("AccessDenied", "TaiKhoan");
            }

            var hoSo = await _context.HoSoUngTuyen
                .FirstOrDefaultAsync(h => h.MaHoSo == id);

            if (hoSo == null)
            {
                TempData["ErrorMessage"] =
                    "Không tìm thấy hồ sơ.";

                return RedirectToAction(nameof(Index));
            }

            // Chỉ cho phép hủy hồ sơ ở giai đoạn đầu
            if (hoSo.TrangThai != "Chờ duyệt" &&
                hoSo.TrangThai != "Đạt sơ tuyển")
            {
                TempData["ErrorMessage"] =
                    "Hồ sơ ở trạng thái hiện tại không thể hủy.";

                return RedirectToAction(nameof(Index));
            }

            hoSo.TrangThai = "Đã hủy";
            hoSo.NgayXuLy = DateTime.Now;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Đã hủy hồ sơ ứng tuyển.";

            return RedirectToAction(nameof(Index));
        }

        // ============================================================
        // KIỂM TRA SỐ LƯỢNG ỨNG VIÊN ĐÃ TRÚNG TUYỂN
        // Dùng LINQ CountAsync()
        // ============================================================
        private async Task<bool> DaDuSoLuongTuyen(int maViTri)
        {
            var viTri = await _context.ViTriTuyenDung
                .FirstOrDefaultAsync(v => v.MaViTri == maViTri);

            if (viTri == null)
            {
                return false;
            }

            var soNguoiTrungTuyen = await _context.HoSoUngTuyen
                .CountAsync(h =>
                    h.MaViTri == maViTri &&
                    h.TrangThai == "Trúng tuyển");

            return soNguoiTrungTuyen >= viTri.SoLuongCanTuyen;
        }
    }
}
