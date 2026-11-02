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
        if (!ModelState.IsValid) return View(model);
        v.HanNopHoSo = model.HanNopMoi.Date;
        v.SoLuongCanTuyen = model.SoLuongCanTuyen;
        v.TrangThai = TrangThaiTuyenDung.DangTuyen;
        v.LyDoDong = null;
        await LuuThayDoiAsync();
        await transaction.CommitAsync();
        TempData["SuccessMessage"] = "Đã gia hạn và mở tuyển thành công.";
        return RedirectToAction(nameof(ChiTietViTri), new { id });
    }

    private async Task LuuThayDoiAsync()
    {
        var entries = _context.ChangeTracker.Entries<ViTriTuyenDung>()
            .Where(e => e.State is EntityState.Added or EntityState.Modified).ToList();
        foreach (var entry in entries)
        {
            var v = entry.Entity;
            v.LyDoDong = v.TrangThai == TrangThaiTuyenDung.DaDong
                ? (string.IsNullOrWhiteSpace(v.LyDoDong) ? "Đóng thủ công" : v.LyDoDong)
                : null;
        }
        await _context.SaveChangesAsync();
    }
}
