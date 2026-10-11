// ==============================================================================
// Họ và tên: Nguyễn Tấn Dũng
// Mã sinh viên: 23103100070
// Module: 3 - Quản lý ứng viên và nộp hồ sơ ứng tuyển
// Nội dung thực hiện: ViewModel đóng gói dữ liệu cho màn hình nộp hồ sơ ứng tuyển (HoSoUngTuyenController)
// ==============================================================================

using System;
using System.ComponentModel.DataAnnotations;

namespace QuanLyTuyenDung_UNETI05_DHTI17A2HN.ViewModels
{
    // VIEWMODEL: DÙNG CHO FORM NỘP HỒ SƠ ỨNG TUYỂN 
    public class NopHoSoViewModel
    {
        // Thông tin vị trí tuyển dụng (Hiển thị tóm tắt cho ứng viên xem)
        public int MaViTri { get; set; }

        [Display(Name = "Vị trí tuyển dụng")]
        public string TenViTri { get; set; } = string.Empty;

        [Display(Name = "Phòng ban")]
        public string TenPhongBan { get; set; } = string.Empty;

        [Display(Name = "Hạn nộp hồ sơ")]
        [DataType(DataType.Date)]
        public DateTime HanNopHoSo { get; set; }

        // Thông tin ứng viên nộp hồ sơ
        public int MaUngVien { get; set; }

        [Display(Name = "Họ và tên ứng viên")]
        public string HoTenUngVien { get; set; } = string.Empty;

        [Display(Name = "Email liên hệ")]
        public string EmailUngVien { get; set; } = string.Empty;

        [Display(Name = "Số điện thoại")]
        public string SoDienThoaiUngVien { get; set; } = string.Empty;

        // Dữ liệu ứng viên nhập để gửi đi ứng tuyển
        [Display(Name = "Thư giới thiệu / Đơn ứng tuyển")]
        [StringLength(2000, ErrorMessage = "Thư giới thiệu tối đa 2000 ký tự")]
        public string? ThuGioiThieu { get; set; }

        [Display(Name = "Ghi chú thêm")]
        [StringLength(500, ErrorMessage = "Ghi chú tối đa 500 ký tự")]
        public string? GhiChu { get; set; }
    }
}
