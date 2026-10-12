// ==============================================================================
// Họ và tên: Nguyễn Tấn Dũng
// Mã sinh viên: 23103100070
// Module: 3 - Quản lý ứng viên và nộp hồ sơ ứng tuyển
// Nội dung thực hiện: ViewModel đóng gói dữ liệu cho màn hình cập nhật thông tin cá nhân ứng viên (UngVienController)
// ==============================================================================

using System;
using System.ComponentModel.DataAnnotations;

namespace QuanLyTuyenDung_UNETI05_DHTI17A2HN.ViewModels
{
    // VIEWMODEL: DÙNG CHO FORM CẬP NHẬT HỒ SƠ CÁ NHÂN 
    public class CapNhatHoSoViewModel
    {
        public int MaUngVien { get; set; }

        public int MaTaiKhoan { get; set; }

        [Required(ErrorMessage = "Họ và tên ứng viên không được để trống")]
        [StringLength(100, ErrorMessage = "Họ và tên không quá 100 ký tự")]
        [Display(Name = "Họ và tên")]
        public string HoTen { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        [Display(Name = "Ngày sinh")]
        public DateTime? NgaySinh { get; set; }

        [Display(Name = "Giới tính")]
        public string? GioiTinh { get; set; }

        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        [Phone(ErrorMessage = "Số điện thoại không đúng định dạng")]
        [StringLength(15, ErrorMessage = "Số điện thoại không quá 15 ký tự")]
        [Display(Name = "Số điện thoại")]
        public string SoDienThoai { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        [StringLength(100, ErrorMessage = "Email không quá 100 ký tự")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [StringLength(250, ErrorMessage = "Địa chỉ không quá 250 ký tự")]
        [Display(Name = "Địa chỉ")]
        public string? DiaChi { get; set; }

        [StringLength(100, ErrorMessage = "Trình độ học vấn không quá 100 ký tự")]
        [Display(Name = "Trình độ học vấn")]
        public string? TrinhDoHocVan { get; set; }

        [StringLength(100, ErrorMessage = "Chuyên ngành không quá 100 ký tự")]
        [Display(Name = "Chuyên ngành")]
        public string? ChuyenNganh { get; set; }

        [Range(0, 50, ErrorMessage = "Số năm kinh nghiệm phải từ 0 đến 50 năm")]
        [Display(Name = "Số năm kinh nghiệm")]
        public int SoNamKinhNghiem { get; set; } = 0;

        [Display(Name = "Kỹ năng chuyên môn")]
        public string? KyNang { get; set; }
    }
}
