
// Họ và tên: Nguyễn Tấn Dũng
// Mã sinh viên: 23103100070
// Nội dung: Module 3 - Nộp hồ sơ và theo dõi ứng tuyển
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


        // =========================================================
        // HỌ VÀ TÊN: Nguyễn Văn Khánh
        // MÃ SINH VIÊN: 23103100101
        // MODULE 4: TIẾP NHẬN HỒ SƠ - SƠ TUYỂN - QUẢN LÝ TRẠNG THÁI
        // =========================================================

        // GET: Nhân sự xem danh sách toàn bộ hồ sơ ứng tuyển
        [HttpGet]
        public async Task<IActionResult> QuanLyDanhSach(
            string? search,
            string? phongBan,
            string? trangThai)
        {
            var query = _context.HoSoUngTuyen
                .Include(h => h.UngVien)
                .Include(h => h.ViTriTuyenDung)
                    .ThenInclude(v => v.PhongBan)
                .AsQueryable();

            // Tìm kiếm theo tên ứng viên hoặc tên vị trí
            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();

                query = query.Where(h =>
                    h.UngVien.HoTen.Contains(search) ||
                    h.ViTriTuyenDung.TenViTri.Contains(search));
            }

            // Lọc theo phòng ban
            if (!string.IsNullOrWhiteSpace(phongBan))
            {
                query = query.Where(h =>
                    h.ViTriTuyenDung.PhongBan.TenPhongBan == phongBan);
            }

            // Lọc theo trạng thái hồ sơ
            if (!string.IsNullOrWhiteSpace(trangThai))
            {
                query = query.Where(h =>
                    h.TrangThai == trangThai);
            }

            // Sắp xếp hồ sơ mới nhất lên đầu
            var danhSach = await query
                .OrderByDescending(h => h.NgayNop)
                .ToListAsync();

            // Danh sách phòng ban phục vụ bộ lọc
            ViewBag.PhongBans = await _context.PhongBan
                .Where(p => p.TrangThai == "Hoạt động")
                .OrderBy(p => p.TenPhongBan)
                .ToListAsync();

            // Danh sách trạng thái
            ViewBag.TrangThai = new List<string>
            {
                "Chờ duyệt",
                "Đạt sơ tuyển",
                "Không đạt sơ tuyển",
                "Chờ phỏng vấn",
                "Đã phỏng vấn",
                "Trúng tuyển",
                "Không trúng tuyển",
                "Đã hủy"
            };

            ViewBag.Search = search;
            ViewBag.SelectedPhongBan = phongBan;
            ViewBag.SelectedTrangThai = trangThai;

            return View(danhSach);
        }


        // GET: Mở màn hình sơ tuyển hồ sơ
        [HttpGet]
        public async Task<IActionResult> SoTuyen(int id)
        {
            var hoSo = await _context.HoSoUngTuyen
                .Include(h => h.UngVien)
                .Include(h => h.ViTriTuyenDung)
                    .ThenInclude(v => v.PhongBan)
                .FirstOrDefaultAsync(h => h.MaHoSo == id);

            if (hoSo == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy hồ sơ ứng tuyển.";
                return RedirectToAction(nameof(QuanLyDanhSach));
            }

            // Chỉ cho phép sơ tuyển hồ sơ đang chờ duyệt
            if (hoSo.TrangThai != "Chờ duyệt")
            {
                TempData["ErrorMessage"] =
                    "Hồ sơ này không ở trạng thái Chờ duyệt nên không thể sơ tuyển.";

                return RedirectToAction(nameof(QuanLyDanhSach));
            }

            return View(hoSo);
        }


        // POST: Xử lý kết quả sơ tuyển
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SoTuyen(
            int id,
            string ketQua,
            string? nhanXetSoTuyen)
        {
            var hoSo = await _context.HoSoUngTuyen
                .Include(h => h.UngVien)
                .Include(h => h.ViTriTuyenDung)
                .FirstOrDefaultAsync(h => h.MaHoSo == id);

            if (hoSo == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy hồ sơ ứng tuyển.";
                return RedirectToAction(nameof(QuanLyDanhSach));
            }

            // Chỉ xử lý hồ sơ đang chờ duyệt
            if (hoSo.TrangThai != "Chờ duyệt")
            {
                TempData["ErrorMessage"] =
                    "Hồ sơ không còn ở trạng thái Chờ duyệt.";

                return RedirectToAction(nameof(QuanLyDanhSach));
            }

            // Kiểm tra kết quả sơ tuyển
            if (ketQua != "Đạt sơ tuyển" &&
                ketQua != "Không đạt sơ tuyển")
            {
                TempData["ErrorMessage"] =
                    "Kết quả sơ tuyển không hợp lệ.";

                return RedirectToAction(nameof(SoTuyen), new { id });
            }

            // Cập nhật trạng thái
            hoSo.TrangThai = ketQua;

            // Lưu thời gian xử lý
            hoSo.NgayXuLy = DateTime.Now;

            // Lưu nhận xét của nhân sự
            hoSo.NhanXetSoTuyen =
                string.IsNullOrWhiteSpace(nhanXetSoTuyen)
                    ? null
                    : nhanXetSoTuyen.Trim();

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                ketQua == "Đạt sơ tuyển"
                    ? "Đã duyệt hồ sơ đạt sơ tuyển."
                    : "Đã cập nhật hồ sơ không đạt sơ tuyển.";

            return RedirectToAction(nameof(QuanLyDanhSach));
        }


        // POST: Cập nhật trạng thái tuyển dụng
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CapNhatTrangThai(
            int id,
            string trangThaiMoi)
        {
            var hoSo = await _context.HoSoUngTuyen
                .Include(h => h.ViTriTuyenDung)
                .FirstOrDefaultAsync(h => h.MaHoSo == id);

            if (hoSo == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy hồ sơ.";
                return RedirectToAction(nameof(QuanLyDanhSach));
            }

            // Các trạng thái được phép sử dụng
            var trangThaiHopLe = new[]
            {
                "Chờ duyệt",
                "Đạt sơ tuyển",
                "Không đạt sơ tuyển",
                "Chờ phỏng vấn",
                "Đã phỏng vấn",
                "Trúng tuyển",
                "Không trúng tuyển",
                "Đã hủy"
            };

            if (!trangThaiHopLe.Contains(trangThaiMoi))
            {
                TempData["ErrorMessage"] =
                    "Trạng thái tuyển dụng không hợp lệ.";

                return RedirectToAction(nameof(QuanLyDanhSach));
            }

            // Không cho thay đổi hồ sơ đã hủy
            if (hoSo.TrangThai == "Đã hủy")
            {
                TempData["ErrorMessage"] =
                    "Hồ sơ đã bị hủy, không thể cập nhật trạng thái.";

                return RedirectToAction(nameof(QuanLyDanhSach));
            }

            // Nếu chuyển sang Trúng tuyển,
            // kiểm tra số lượng tuyển của vị trí
            if (trangThaiMoi == "Trúng tuyển" &&
                hoSo.TrangThai != "Trúng tuyển")
            {
                int soNguoiTrungTuyen = await _context.HoSoUngTuyen
                    .CountAsync(h =>
                        h.MaViTri == hoSo.MaViTri &&
                        h.TrangThai == "Trúng tuyển");

                int chiTieu = hoSo.ViTriTuyenDung.SoLuongCanTuyen;

                if (soNguoiTrungTuyen >= chiTieu)
                {
                    TempData["ErrorMessage"] =
                        $"Vị trí đã đủ chỉ tiêu tuyển dụng ({chiTieu} người).";

                    return RedirectToAction(nameof(QuanLyDanhSach));
                }
            }

            hoSo.TrangThai = trangThaiMoi;
            hoSo.NgayXuLy = DateTime.Now;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Cập nhật trạng thái hồ sơ thành công.";

            return RedirectToAction(nameof(QuanLyDanhSach));
        }
    }
}