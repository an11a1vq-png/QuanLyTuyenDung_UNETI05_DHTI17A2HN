using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyTuyenDung_UNETI05_DHTI17A2HN.Helpers;
using QuanLyTuyenDung_UNETI05_DHTI17A2HN.Models;
using QuanLyTuyenDung_UNETI05_DHTI17A2HN.ViewModels;
namespace QuanLyTuyenDung_UNETI05_DHTI17A2HN.Controllers;

public partial class ViTriTuyenDungController
{
    private bool LaQuanLy => HttpContext.Session.GetVaiTro() is "Admin" or "Nhân sự";

    private async Task<string?> KiemTraMoTuyenAsync(ViTriTuyenDung v, DateTime han, int chiTieu)
    {
        if (han.Date < DateTime.Today || han.Date < v.NgayBatDauNhanHoSo.Date)
            return "Hạn nộp phải từ hôm nay và không trước ngày bắt đầu nhận hồ sơ.";
        if (v.NgayBatDauNhanHoSo.Date > DateTime.Today)
            return "Chưa đến ngày nhận hồ sơ. Hãy cập nhật ngày bắt đầu trước khi mở lại.";
        if (!await _context.PhongBan.AnyAsync(p => p.MaPhongBan == v.MaPhongBan && p.TrangThai))
            return "Phòng ban đang ngưng hoạt động, không thể mở tuyển.";
        var daTuyen = await _context.HoSoUngTuyen.CountAsync(h => h.MaViTri == v.MaViTri && h.TrangThai == TrangThaiTuyenDung.TrungTuyen);
        if (chiTieu <= daTuyen) return $"Đã tuyển {daTuyen} người. Chỉ tiêu mới phải lớn hơn số đã tuyển.";
        return null;
    }

    private IActionResult KetQuaDoiTrangThai(bool thanhCong, string thongBao)
    {
        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            return Json(new { success = thanhCong, message = thongBao });
        TempData[thanhCong ? "SuccessMessage" : "ErrorMessage"] = thongBao;
        return RedirectToAction(nameof(DanhSachViTri));
    }

    [HttpGet]
    public async Task<IActionResult> MoLaiViTri(int id)
    {
        if (!LaQuanLy) return StatusCode(403);
        var v = await _context.ViTriTuyenDung.FindAsync(id);
        if (v == null) return NotFound();
        return View(new MoLaiViTriViewModel { MaViTri = id, TenViTri = v.TenViTri,
            HanNopMoi = v.HanNopHoSo.Date > DateTime.Today ? v.HanNopHoSo : DateTime.Today.AddDays(30),
            SoLuongCanTuyen = v.SoLuongCanTuyen });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> MoLaiViTri(int id, MoLaiViTriViewModel model)
    {
        if (!LaQuanLy) return StatusCode(403);
        if (id != model.MaViTri) return BadRequest();
        // Serialize quota validation with writes to applications for this position.
        await using var transaction = await _context.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);
        var v = await _context.ViTriTuyenDung.FindAsync(id);
        if (v == null) return NotFound();
        model.TenViTri = v.TenViTri;
        var loi = await KiemTraMoTuyenAsync(v, model.HanNopMoi, model.SoLuongCanTuyen);
        if (loi != null) ModelState.AddModelError("", loi);
        if (string.IsNullOrWhiteSpace(model.LyDo)) ModelState.AddModelError(nameof(model.LyDo), "Vui lòng nhập lý do.");
        if (!ModelState.IsValid) return View(model);
        v.HanNopHoSo = model.HanNopMoi.Date;
        v.SoLuongCanTuyen = model.SoLuongCanTuyen;
        v.TrangThai = TrangThaiTuyenDung.DangTuyen;
        v.LyDoDong = null;
        await LuuThayDoiAsync("Gia hạn / mở lại", model.LyDo.Trim(), v);
        await transaction.CommitAsync();
        TempData["SuccessMessage"] = "Đã gia hạn và mở tuyển thành công.";
        return RedirectToAction(nameof(ChiTietViTri), new { id });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> LuuYeuThich(int id, bool luu, string? returnUrl)
    {
        var account = HttpContext.Session.GetMaTaiKhoan();
        if (!account.HasValue || HttpContext.Session.GetVaiTro() != "Ứng viên")
            return RedirectToAction("DangNhap", "TaiKhoan");
        if (!await _context.ViTriTuyenDung.AnyAsync(v => v.MaViTri == id)) return NotFound();
        if (luu)
        {
            var existed = await _context.ViecLamYeuThich.FindAsync(account.Value, id);
            if (existed == null)
            {
                _context.ViecLamYeuThich.Add(new ViecLamYeuThich { MaTaiKhoan = account.Value, MaViTri = id });
                try { await _context.SaveChangesAsync(); }
                catch (DbUpdateException ex) when (ex.InnerException is Microsoft.Data.SqlClient.SqlException sql && sql.Number is 2601 or 2627)
                { _context.ChangeTracker.Clear(); }
            }
        }
        else await _context.ViecLamYeuThich.Where(x => x.MaTaiKhoan == account.Value && x.MaViTri == id).ExecuteDeleteAsync();
        TempData["SuccessMessage"] = luu ? "Đã lưu việc làm yêu thích." : "Đã bỏ lưu việc làm.";
        return Url.IsLocalUrl(returnUrl) ? LocalRedirect(returnUrl!) : RedirectToAction(nameof(ChiTietViTri), new { id });
    }

    private async Task LuuThayDoiAsync(string? hanhDong = null, string? lyDo = null, ViTriTuyenDung? viTri = null, bool heThong = false)
    {
        _context.ChangeTracker.DetectChanges();
        var entries = _context.ChangeTracker.Entries<ViTriTuyenDung>()
            .Where(e => e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted).ToList();
        foreach (var entry in entries)
        {
            var v = entry.Entity;
            if (entry.State != EntityState.Deleted)
            {
                v.LyDoDong = v.TrangThai == TrangThaiTuyenDung.DaDong
                    ? (string.IsNullOrWhiteSpace(v.LyDoDong) ? "Đóng thủ công" : v.LyDoDong) : null;
            }
            _context.ChangeTracker.DetectChanges();
            var changes = entry.Properties.Where(p => p.IsModified && !Equals(p.OriginalValue, p.CurrentValue))
                .Select(p => $"{typeof(ViTriTuyenDung).GetProperty(p.Metadata.Name)?.GetCustomAttribute<DisplayAttribute>()?.Name ?? p.Metadata.Name}: {HienThi(p.OriginalValue)} → {HienThi(p.CurrentValue)}").ToList();
            if (entry.State == EntityState.Modified && changes.Count == 0 && viTri != v) continue;
            _context.LichSuViTri.Add(new LichSuViTri
            {
                ViTri = entry.State == EntityState.Deleted ? null : v,
                TenViTri = v.TenViTri,
                MaTaiKhoan = heThong ? null : HttpContext.Session.GetMaTaiKhoan(),
                NguoiThucHien = heThong ? "Hệ thống" : HttpContext.Session.GetHoTen() ?? "Hệ thống",
                HanhDong = hanhDong ?? (entry.State == EntityState.Added ? "Thêm mới" : entry.State == EntityState.Deleted ? "Xóa vị trí" : "Cập nhật"),
                NoiDung = string.Join(Environment.NewLine, changes) + (lyDo == null ? "" : $"{Environment.NewLine}Lý do: {lyDo}")
            });
        }
        if (viTri != null && !entries.Any(e => e.Entity == viTri))
            _context.LichSuViTri.Add(new LichSuViTri { ViTri = viTri, TenViTri = viTri.TenViTri,
                MaTaiKhoan = heThong ? null : HttpContext.Session.GetMaTaiKhoan(), NguoiThucHien = heThong ? "Hệ thống" : HttpContext.Session.GetHoTen() ?? "Hệ thống",
                HanhDong = hanhDong ?? "Cập nhật", NoiDung = lyDo ?? "" });
        await _context.SaveChangesAsync();
    }
    private static string HienThi(object? value) => value is DateTime date ? date.ToString("dd/MM/yyyy") : value?.ToString() ?? "(trống)";
}
