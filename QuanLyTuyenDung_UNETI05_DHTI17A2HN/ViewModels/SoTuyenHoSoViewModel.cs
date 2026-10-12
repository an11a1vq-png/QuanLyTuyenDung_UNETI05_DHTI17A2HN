
// =========================================================
// HỌ VÀ TÊN: Nguyễn Văn Khánh
// MÃ SINH VIÊN: 23103100101
// MODULE 4: VIEWMODEL SƠ TUYỂN HỒ SƠ
// =========================================================

using System.ComponentModel.DataAnnotations;

namespace QuanLyTuyenDung_UNETI05_DHTI17A2HN.ViewModels
{
    public class SoTuyenHoSoViewModel
    {
        // =========================
        // THÔNG TIN HỒ SƠ
        // =========================

        public int MaHoSo { get; set; }

        public DateTime NgayNop { get; set; }

        public string TrangThai { get; set; } = string.Empty;

        // =========================
        // THÔNG TIN ỨNG VIÊN
        // =========================

        public int MaUngVien { get; set; }

        public string HoTen { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string SoDienThoai { get; set; } = string.Empty;

        public string? TrinhDoHocVan { get; set; }

        public string? ChuyenNganh { get; set; }

        public int SoNamKinhNghiem { get; set; }

        public string? KyNang { get; set; }

        // =========================
        // THÔNG TIN VỊ TRÍ TUYỂN DỤNG
        // =========================

        public int MaViTri { get; set; }

        public string TenViTri { get; set; } = string.Empty;

        public string? TenPhongBan { get; set; }

        public string? TrinhDoYeuCau { get; set; }

        public int SoNamKinhNghiemToiThieu { get; set; }

        public string YeuCauUngVien { get; set; } = string.Empty;

        // =========================
        // KẾT QUẢ XỬ LÝ SƠ TUYỂN
        // =========================

        [Required(ErrorMessage = "Vui lòng chọn kết quả sơ tuyển.")]
        public string KetQua { get; set; } = string.Empty;

        [StringLength(
            1000,
            ErrorMessage = "Nhận xét sơ tuyển không được vượt quá 1000 ký tự.")]
        [Display(Name = "Nhận xét sơ tuyển")]
        public string? NhanXetSoTuyen { get; set; }
    }
}
