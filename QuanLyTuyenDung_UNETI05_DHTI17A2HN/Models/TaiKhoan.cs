// ==============================================================================
// Họ và tên: Đỗ Thiện An
// Mã sinh viên: 23103100082
// Module: 1 - Tài khoản, Đăng nhập, Phân quyền và Phòng ban
// Nội dung thực hiện: Entity Tài khoản người dùng, phân quyền vai trò (Admin, Nhân sự, Ứng viên), bảo mật băm mật khẩu, chống Brute-force và mã xác thực OTP
// ==============================================================================

using System.ComponentModel.DataAnnotations;

namespace QuanLyTuyenDung_UNETI05_DHTI17A2HN.Models
{
    public class TaiKhoan
    {
        [Key]
        public int MaTaiKhoan { get; set; }

        [Required(ErrorMessage = "Tên đăng nhập không được để trống")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Tên đăng nhập từ 3 đến 50 ký tự")]
        [Display(Name = "Tên đăng nhập")]
        public string TenDangNhap { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [StringLength(255)]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string MatKhau { get; set; } = string.Empty;

        [Required(ErrorMessage = "Họ tên không được để trống")]
        [StringLength(100)]
        [Display(Name = "Họ và tên")]
        public string HoTen { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        [Display(Name = "Vai trò")]
        public string VaiTro { get; set; } = "UngVien";

        [Display(Name = "Trạng thái hoạt động")]
        public bool TrangThai { get; set; } = true;

        // --- CÁC THUỘC TÍNH BẢO MẬT NÂNG CAO (MODULE 1) ---
        [Display(Name = "Số lần đăng nhập sai")]
        public int SoLanDangNhapSai { get; set; } = 0;

        [Display(Name = "Khóa đến thời điểm")]
        public DateTime? KhoaDenThoiDiem { get; set; }

        [StringLength(100)]
        [Display(Name = "Mã xác thực khôi phục mật khẩu (OTP)")]
        public string? MaXacThucKhoiPhuc { get; set; }

        [Display(Name = "Thời hạn mã xác thực")]
        public DateTime? ThoiHanMaXacThuc { get; set; }

        public virtual UngVien? UngVien { get; set; }
    }
}
