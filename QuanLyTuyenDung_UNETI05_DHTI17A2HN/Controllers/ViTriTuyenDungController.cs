// ==============================================================================
// Họ và tên: Lê Gia Bảo
// Mã sinh viên: 23103100100
// Module: 2 - Quản lý vị trí tuyển dụng
// Nội dung thực hiện: Controller xử lý CRUD vị trí tuyển dụng, tìm kiếm, lọc, phân trang, đổi trạng thái
// ==============================================================================

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyTuyenDung_UNETI05_DHTI17A2HN.Data;
using QuanLyTuyenDung_UNETI05_DHTI17A2HN.Helpers;
using QuanLyTuyenDung_UNETI05_DHTI17A2HN.Models;
using QuanLyTuyenDung_UNETI05_DHTI17A2HN.ViewModels;

namespace QuanLyTuyenDung_UNETI05_DHTI17A2HN.Controllers
{
    public partial class ViTriTuyenDungController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ViTriTuyenDungController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =========================================================================
        // QUẢN LÝ (ADMIN / NHÂN SỰ): DANH SÁCH VỊ TRÍ CÓ LỌC VÀ PHÂN TRANG
        // =========================================================================
        [HttpGet]
        public async Task<IActionResult> DanhSachViTri(
            string? searchString,
            int? maPhongBan,
            string? trangThai,
            string? trinhDo,
            string? sapXep,
            int page = 1)
        {
            // Kiểm tra quyền: chỉ Admin hoặc Nhân sự mới xem được
            var vaiTro = HttpContext.Session.GetVaiTro();
            if (vaiTro != "Admin" && vaiTro != "Nhân sự")
            {
                TempData["ErrorMessage"] = "Bạn không có quyền truy cập chức năng này.";
                return RedirectToAction("Index", "Home");
            }

            await CapNhatTrangThaiTuDongAsync();
            var model = new DanhSachViTriViewModel
            {
                SearchString = searchString,
                MaPhongBan = maPhongBan,
                TrangThai = trangThai,
                TrinhDo = trinhDo,
                SapXep = sapXep,
                SoBanGhiMoiTrang = 10
            };

            // Truy vấn cơ bản, Include PhongBan và HoSoUngTuyens để tính toán
            var query = _context.ViTriTuyenDung
                .Include(v => v.PhongBan)
                .AsQueryable();

            // Áp dụng bộ lọc và sắp xếp
            query = model.ApDungBoLoc(query, chiLayDangTuyen: false);

            // Đếm tổng số bản ghi sau khi lọc
            model.TongSoBanGhi = await query.CountAsync();

            // Chuẩn hóa trang hiện tại
            model.ChuanHoaTrang(page);

            // Lấy dữ liệu trang hiện tại: chiếu sang ViewModel với COUNT chạy dưới SQL
            model.Items = await query
                .Skip(model.BoQua)
                .Take(model.SoBanGhiMoiTrang)
                .Select(ViTriTuyenDungItemViewModel.Projection)
                .ToListAsync();

            // Tính thống kê từ toàn bộ danh sách (không phân trang)
            var tatCa = await query
                .Select(ViTriTuyenDungItemViewModel.Projection)
                .ToListAsync();
            model.ThongKe = ThongKeViTriViewModel.TuDanhSach(tatCa);

            // Nạp dropdown cho form lọc
            model.NapDanhMucCoDinh();
            model.DanhSachPhongBan = await _context.PhongBan
                .Where(p => p.TrangThai)
                .OrderBy(p => p.TenPhongBan)
                .Select(p => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Value = p.MaPhongBan.ToString(),
                    Text = p.TenPhongBan,
                    Selected = p.MaPhongBan == maPhongBan
                })
                .ToListAsync();

            return View(model);
        }

        // =========================================================================
        // ỨNG VIÊN: DANH SÁCH VIỆC LÀM (CHỈ VỊ TRÍ ĐANG TUYỂN, ĐÃ MỞ, CÒN HẠN)
        // =========================================================================
        [HttpGet]
        public async Task<IActionResult> ViecLam(
            string? searchString,
            int? maPhongBan,
            string? trinhDo,
            string? sapXep,
            int page = 1, bool chiYeuThich = false)
        {
            var maTaiKhoan = HttpContext.Session.GetMaTaiKhoan();
            if (chiYeuThich && (HttpContext.Session.GetVaiTro() != "Ứng viên" || !maTaiKhoan.HasValue))
                return RedirectToAction("DangNhap", "TaiKhoan");
            await CapNhatTrangThaiTuDongAsync();

            var model = new DanhSachViTriViewModel
            {
                SearchString = searchString,
                MaPhongBan = maPhongBan,
                TrinhDo = trinhDo,
                SapXep = sapXep,
                ChiYeuThich = chiYeuThich,
                SoBanGhiMoiTrang = 12  // Hiển thị 12 thẻ mỗi trang
            };

            var query = _context.ViTriTuyenDung
                .Include(v => v.PhongBan)
                .AsQueryable();

            // Áp dụng bộ lọc (chỉ lấy vị trí đang tuyển, còn hạn)
            if (chiYeuThich)
                query = query.Where(v => _context.ViecLamYeuThich.Any(x => x.MaTaiKhoan == maTaiKhoan && x.MaViTri == v.MaViTri));
            // Danh sách đã lưu vẫn giữ vị trí đã đóng để ứng viên có thể bỏ lưu.
            query = model.ApDungBoLoc(query, chiLayDangTuyen: !chiYeuThich);
            if (maTaiKhoan.HasValue && HttpContext.Session.GetVaiTro() == "Ứng viên")
                model.MaViTriYeuThich = (await _context.ViecLamYeuThich
                    .Where(x => x.MaTaiKhoan == maTaiKhoan.Value).Select(x => x.MaViTri).ToListAsync()).ToHashSet();

            model.TongSoBanGhi = await query.CountAsync();
            model.ChuanHoaTrang(page);

            model.Items = await query
                .Skip(model.BoQua)
                .Take(model.SoBanGhiMoiTrang)
                .Select(ViTriTuyenDungItemViewModel.Projection)
                .ToListAsync();

            // Nạp dropdown (không có thống kê, không có lọc trạng thái)
            model.NapDanhMucCoDinh();
            model.DanhSachPhongBan = await _context.PhongBan
                .Where(p => p.TrangThai)
                .OrderBy(p => p.TenPhongBan)
                .Select(p => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Value = p.MaPhongBan.ToString(),
                    Text = p.TenPhongBan,
                    Selected = p.MaPhongBan == maPhongBan
                })
                .ToListAsync();

            return View(model);
        }

        // =========================================================================
        // CHI TIẾT VỊ TRÍ (CÔNG KHAI CHO TẤT CẢ)
        // =========================================================================
        [HttpGet]
        public async Task<IActionResult> ChiTietViTri(int id)
        {
            await CapNhatTrangThaiTuDongAsync();

            var model = await _context.ViTriTuyenDung
                .Where(v => v.MaViTri == id)
                .Select(ChiTietViTriViewModel.Projection)
                .FirstOrDefaultAsync();

            if (model == null)
            {
                TempData["ErrorMessage"] = "Vị trí tuyển dụng không tồn tại.";
                return RedirectToAction(nameof(ViecLam));
            }

            // Thống kê hồ sơ theo trạng thái (chỉ Admin / Nhân sự mới thấy)
            var vaiTro = HttpContext.Session.GetVaiTro();
            if (vaiTro == "Admin" || vaiTro == "Nhân sự")
            {
                model.LichSu = await _context.LichSuViTri.AsNoTracking().Where(x => x.MaViTri == id)
                    .OrderByDescending(x => x.ThoiGian).ThenByDescending(x => x.MaLichSu).ToListAsync();
                model.ThongKeTheoTrangThai = await _context.HoSoUngTuyen
                    .Where(h => h.MaViTri == id)
                    .GroupBy(h => h.TrangThai)
                    .Select(g => new { TrangThai = g.Key, SoLuong = g.Count() })
                    .ToDictionaryAsync(x => x.TrangThai, x => x.SoLuong);
            }

            var accountId = HttpContext.Session.GetMaTaiKhoan();
            if (accountId.HasValue && vaiTro == "Ứng viên")
                model.DaYeuThich = await _context.ViecLamYeuThich.AnyAsync(x => x.MaTaiKhoan == accountId.Value && x.MaViTri == id);

            // Kiểm tra ứng viên đã nộp hồ sơ chưa
            var maUngVien = HttpContext.Session.GetMaUngVien();
            if (maUngVien.HasValue)
            {
                model.DaUngTuyen = await _context.HoSoUngTuyen.AnyAsync(h =>
                    h.MaViTri == id &&
                    h.MaUngVien == maUngVien.Value &&
                    !TrangThaiTuyenDung.HoSoDaKetThuc.Contains(h.TrangThai));
            }

            return View(model);
        }

        // Tự động đóng vị trí đã hết hạn hoặc đã tuyển đủ chỉ tiêu.
        // Chỉ xử lý vị trí đang tuyển để không ghi đè các trạng thái thủ công.
        private async Task CapNhatTrangThaiTuDongAsync()
        {
            var homNay = DateTime.Today;

            var viTriCanDong = await _context.ViTriTuyenDung
                .Where(v => v.TrangThai == TrangThaiTuyenDung.DangTuyen)
                .Where(v => v.HanNopHoSo < homNay ||
                            v.HoSoUngTuyens.Count(h => h.TrangThai == TrangThaiTuyenDung.TrungTuyen) >= v.SoLuongCanTuyen)
                .ToListAsync();

            if (viTriCanDong.Count == 0)
            {
                return;
            }

            foreach (var viTri in viTriCanDong)
            {
                var duChiTieu = await _context.HoSoUngTuyen.CountAsync(h => h.MaViTri == viTri.MaViTri && h.TrangThai == TrangThaiTuyenDung.TrungTuyen) >= viTri.SoLuongCanTuyen;
                viTri.LyDoDong = duChiTieu ? "Đã tuyển đủ chỉ tiêu" : "Đã hết hạn nộp hồ sơ";
                viTri.TrangThai = TrangThaiTuyenDung.DaDong;
            }

            await LuuThayDoiAsync("Tự động đóng", heThong: true);
        }

        // =========================================================================
        // THÊM MỚI VỊ TRÍ (GET)
        // =========================================================================
        [HttpGet]
        public async Task<IActionResult> ThemMoiViTri()
        {
            var vaiTro = HttpContext.Session.GetVaiTro();
            if (vaiTro != "Admin" && vaiTro != "Nhân sự")
            {
                TempData["ErrorMessage"] = "Bạn không có quyền truy cập chức năng này.";
                return RedirectToAction("Index", "Home");
            }

            var model = new ViTriTuyenDungFormViewModel();
            model.NapDanhMucCoDinh();
            model.NapPhongBan(await _context.PhongBan.ToListAsync());

            return View(model);
        }

        // =========================================================================
        // THÊM MỚI VỊ TRÍ (POST)
        // =========================================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ThemMoiViTri(ViTriTuyenDungFormViewModel model)
        {
            var vaiTro = HttpContext.Session.GetVaiTro();
            if (vaiTro != "Admin" && vaiTro != "Nhân sự")
            {
                TempData["ErrorMessage"] = "Bạn không có quyền truy cập chức năng này.";
                return RedirectToAction("Index", "Home");
            }

            // Kiểm tra phòng ban
            var phongBan = await _context.PhongBan.FindAsync(model.MaPhongBan);
            model.KiemTraPhongBan(phongBan, ModelState);

            if (model.TrangThai == TrangThaiTuyenDung.DangTuyen)
            {
                if (model.HanNopHoSo.Date < DateTime.Today)
                    ModelState.AddModelError(nameof(model.HanNopHoSo), "Không thể mở tuyển khi đã hết hạn.");
                if (model.LaCapNhat && model.SoDaTrungTuyen >= model.SoLuongCanTuyen)
                    ModelState.AddModelError(nameof(model.SoLuongCanTuyen), "Đã đủ chỉ tiêu. Hãy tăng chỉ tiêu trước khi mở tuyển.");
            }
            if (!ModelState.IsValid)
            {
                model.NapDanhMucCoDinh();
                model.NapPhongBan(await _context.PhongBan.ToListAsync());
                return View(model);
            }

            var viTri = new ViTriTuyenDung();
            model.GanVaoEntity(viTri);

            _context.ViTriTuyenDung.Add(viTri);
            await LuuThayDoiAsync();

            TempData["SuccessMessage"] = $"Đã thêm vị trí '{viTri.TenViTri}' thành công.";
            return RedirectToAction(nameof(DanhSachViTri));
        }

        // =========================================================================
        // CẬP NHẬT VỊ TRÍ (GET)
        // =========================================================================
        [HttpGet]
        public async Task<IActionResult> CapNhatViTri(int id)
        {
            var vaiTro = HttpContext.Session.GetVaiTro();
            if (vaiTro != "Admin" && vaiTro != "Nhân sự")
            {
                TempData["ErrorMessage"] = "Bạn không có quyền truy cập chức năng này.";
                return RedirectToAction("Index", "Home");
            }

            var viTri = await _context.ViTriTuyenDung.FindAsync(id);
            if (viTri == null)
            {
                TempData["ErrorMessage"] = "Vị trí tuyển dụng không tồn tại.";
                return RedirectToAction(nameof(DanhSachViTri));
            }

            // Đếm số người đã trúng tuyển
            var soDaTrungTuyen = await _context.HoSoUngTuyen
                .CountAsync(h => h.MaViTri == id && h.TrangThai == TrangThaiTuyenDung.TrungTuyen);

            var model = ViTriTuyenDungFormViewModel.TuEntity(viTri, soDaTrungTuyen);
            model.NapDanhMucCoDinh();
            model.NapPhongBan(await _context.PhongBan.ToListAsync());

            return View(model);
        }

        // =========================================================================
        // CẬP NHẬT VỊ TRÍ (POST)
        // =========================================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CapNhatViTri(int id, ViTriTuyenDungFormViewModel model)
        {
            var vaiTro = HttpContext.Session.GetVaiTro();
            if (vaiTro != "Admin" && vaiTro != "Nhân sự")
            {
                TempData["ErrorMessage"] = "Bạn không có quyền truy cập chức năng này.";
                return RedirectToAction("Index", "Home");
            }

            if (id != model.MaViTri)
            {
                return BadRequest();
            }

            var viTri = await _context.ViTriTuyenDung.FindAsync(id);
            if (viTri == null)
            {
                TempData["ErrorMessage"] = "Vị trí tuyển dụng không tồn tại.";
                return RedirectToAction(nameof(DanhSachViTri));
            }

            // Đếm số người đã trúng tuyển và kiểm tra chỉ tiêu
            var soDaTrungTuyen = await _context.HoSoUngTuyen
                .CountAsync(h => h.MaViTri == id && h.TrangThai == TrangThaiTuyenDung.TrungTuyen);
            model.SoDaTrungTuyen = soDaTrungTuyen;
            model.KiemTraChiTieu(ModelState);

            // Kiểm tra phòng ban
            var phongBan = await _context.PhongBan.FindAsync(model.MaPhongBan);
            model.KiemTraPhongBan(phongBan, ModelState);

            if (model.TrangThai == TrangThaiTuyenDung.DangTuyen)
            {
                if (model.HanNopHoSo.Date < DateTime.Today)
                    ModelState.AddModelError(nameof(model.HanNopHoSo), "Không thể mở tuyển khi đã hết hạn.");
                if (soDaTrungTuyen >= model.SoLuongCanTuyen)
                    ModelState.AddModelError(nameof(model.SoLuongCanTuyen), "Đã đủ chỉ tiêu. Hãy tăng chỉ tiêu trước khi mở tuyển.");
            }
            if (!ModelState.IsValid)
            {
                model.NapDanhMucCoDinh();
                model.NapPhongBan(await _context.PhongBan.ToListAsync());
                return View(model);
            }

            model.GanVaoEntity(viTri);

            try
            {
                await LuuThayDoiAsync();
                TempData["SuccessMessage"] = $"Đã cập nhật vị trí '{viTri.TenViTri}' thành công.";
                return RedirectToAction(nameof(ChiTietViTri), new { id });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.ViTriTuyenDung.AnyAsync(v => v.MaViTri == id))
                {
                    TempData["ErrorMessage"] = "Vị trí đã bị xóa bởi người dùng khác.";
                    return RedirectToAction(nameof(DanhSachViTri));
                }
                throw;
            }
        }

        // =========================================================================
        // XÓA VỊ TRÍ (CHỈ ADMIN, CHỈ KHI CHƯA CÓ HỒ SƠ)
        // =========================================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> XoaViTri(int id)
        {
            var vaiTro = HttpContext.Session.GetVaiTro();
            if (vaiTro != "Admin")
            {
                TempData["ErrorMessage"] = "Chỉ Admin mới có quyền xóa vị trí tuyển dụng.";
                return RedirectToAction(nameof(DanhSachViTri));
            }

            var viTri = await _context.ViTriTuyenDung.FindAsync(id);
            if (viTri == null)
            {
                TempData["ErrorMessage"] = "Vị trí tuyển dụng không tồn tại.";
                return RedirectToAction(nameof(DanhSachViTri));
            }

            // Kiểm tra đã có hồ sơ nào chưa
            var coHoSo = await _context.HoSoUngTuyen.AnyAsync(h => h.MaViTri == id);
            if (coHoSo)
            {
                TempData["ErrorMessage"] = "Không thể xóa vị trí đã có hồ sơ ứng tuyển. Hãy đóng vị trí thay vì xóa.";
                return RedirectToAction(nameof(DanhSachViTri));
            }

            _context.ViTriTuyenDung.Remove(viTri);
            await LuuThayDoiAsync();

            TempData["SuccessMessage"] = $"Đã xóa vị trí '{viTri.TenViTri}' thành công.";
            return RedirectToAction(nameof(DanhSachViTri));
        }

        // =========================================================================
        // ĐỔI TRẠNG THÁI NHANH (ADMIN / NHÂN SỰ)
        // =========================================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DoiTrangThai(int id, string trangThaiMoi, string? lyDo)
        {
            var vaiTro = HttpContext.Session.GetVaiTro();
            if (vaiTro != "Admin" && vaiTro != "Nhân sự")
            {
                return KetQuaDoiTrangThai(false, "Bạn không có quyền thực hiện thao tác này.");
            }

            var viTri = await _context.ViTriTuyenDung.FindAsync(id);
            if (viTri == null)
            {
                return KetQuaDoiTrangThai(false, "Vị trí không tồn tại.");
            }

            if (!ViTriTuyenDung.DanhSachTrangThai.Contains(trangThaiMoi))
            {
                return KetQuaDoiTrangThai(false, "Trạng thái không hợp lệ.");
            }

            if (trangThaiMoi == TrangThaiTuyenDung.DangTuyen)
            {
                var loi = await KiemTraMoTuyenAsync(viTri, viTri.HanNopHoSo, viTri.SoLuongCanTuyen);
                if (loi != null) return KetQuaDoiTrangThai(false, loi);
            }
            if (lyDo?.Length > 500) return KetQuaDoiTrangThai(false, "Lý do tối đa 500 ký tự.");
            viTri.LyDoDong = trangThaiMoi == TrangThaiTuyenDung.DaDong
                ? (string.IsNullOrWhiteSpace(lyDo) ? "Đóng thủ công" : lyDo.Trim()) : null;
            viTri.TrangThai = trangThaiMoi;
            await LuuThayDoiAsync();

            return KetQuaDoiTrangThai(true, $"Đã đổi trạng thái thành '{trangThaiMoi}'.");
        }
    }
}

