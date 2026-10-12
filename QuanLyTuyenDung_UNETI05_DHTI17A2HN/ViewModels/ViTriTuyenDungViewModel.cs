// ==============================================================================
// Họ và tên: [Họ và tên]
// Mã sinh viên: [Mã sinh viên]
// Module: 2 - Quản lý vị trí tuyển dụng
// Nội dung thực hiện: ViewModel đóng gói dữ liệu cho các màn hình vị trí tuyển dụng (ViTriTuyenDungController)
// ==============================================================================

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace QuanLyTuyenDung_UNETI05_DHTI17A2HN.ViewModels
{
    // VIEWMODEL: DÙNG CHO FORM THÊM MỚI / CẬP NHẬT VỊ TRÍ TUYỂN DỤNG
    public class ViTriTuyenDungFormViewModel : IValidatableObject
    {
        // = 0 khi thêm mới, > 0 khi cập nhật
        public int MaViTri { get; set; }

        [Required(ErrorMessage = "Tên vị trí không được để trống")]
        [StringLength(150, ErrorMessage = "Tên vị trí tối đa 150 ký tự")]
        [Display(Name = "Tên vị trí")]
        public string TenViTri { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng chọn phòng ban")]
        [Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn phòng ban")]
        [Display(Name = "Phòng ban")]
        public int MaPhongBan { get; set; }

        [Required(ErrorMessage = "Số lượng cần tuyển không được để trống")]
        [Range(1, 1000, ErrorMessage = "Số lượng cần tuyển từ 1 đến 1000")]
        [Display(Name = "Số lượng cần tuyển")]
        public int SoLuongCanTuyen { get; set; } = 1;

        [StringLength(50)]
        [Display(Name = "Trình độ yêu cầu")]
        public string? TrinhDoYeuCau { get; set; }

        [Required(ErrorMessage = "Số năm kinh nghiệm không được để trống")]
        [Range(0, 50, ErrorMessage = "Số năm kinh nghiệm từ 0 đến 50")]
        [Display(Name = "Kinh nghiệm tối thiểu (năm)")]
        public int SoNamKinhNghiemToiThieu { get; set; }

        [Required(ErrorMessage = "Ngày bắt đầu nhận hồ sơ không được để trống")]
        [DataType(DataType.Date)]
        [Display(Name = "Ngày bắt đầu nhận hồ sơ")]
        public DateTime NgayBatDauNhanHoSo { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Hạn nộp hồ sơ không được để trống")]
        [DataType(DataType.Date)]
        [Display(Name = "Hạn nộp hồ sơ")]
        public DateTime HanNopHoSo { get; set; } = DateTime.Today.AddDays(30);

        [Required(ErrorMessage = "Mô tả công việc không được để trống")]
        [Display(Name = "Mô tả công việc")]
        public string MoTaCongViec { get; set; } = string.Empty;

        [Required(ErrorMessage = "Yêu cầu ứng viên không được để trống")]
        [Display(Name = "Yêu cầu ứng viên")]
        public string YeuCauUngVien { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng chọn trạng thái")]
        [StringLength(30)]
        [Display(Name = "Trạng thái")]
        public string TrangThai { get; set; } = "Đang tuyển";

        // Số hồ sơ đã trúng tuyển (chỉ dùng khi cập nhật, để chặn giảm chỉ tiêu quá mức)
        public int SoDaTrungTuyen { get; set; }

        public bool LaCapNhat => MaViTri > 0;

        // Dữ liệu cho các dropdown (controller nạp lại khi trả view)
        public IEnumerable<SelectListItem> DanhSachPhongBan { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> DanhSachTrinhDo { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> DanhSachTrangThai { get; set; } = new List<SelectListItem>();

        // Kiểm tra ràng buộc giữa các trường
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (HanNopHoSo.Date < NgayBatDauNhanHoSo.Date)
            {
                yield return new ValidationResult(
                    "Hạn nộp hồ sơ phải sau hoặc bằng ngày bắt đầu nhận hồ sơ",
                    new[] { nameof(HanNopHoSo) });
            }

            if (LaCapNhat && SoLuongCanTuyen < SoDaTrungTuyen)
            {
                yield return new ValidationResult(
                    $"Số lượng cần tuyển không được nhỏ hơn số người đã trúng tuyển ({SoDaTrungTuyen})",
                    new[] { nameof(SoLuongCanTuyen) });
            }
        }
    }

    // VIEWMODEL: MỘT DÒNG TRONG DANH SÁCH VỊ TRÍ TUYỂN DỤNG
    public class ViTriTuyenDungItemViewModel
    {
        public int MaViTri { get; set; }

        [Display(Name = "Tên vị trí")]
        public string TenViTri { get; set; } = string.Empty;

        [Display(Name = "Phòng ban")]
        public string TenPhongBan { get; set; } = string.Empty;

        [Display(Name = "Trình độ")]
        public string? TrinhDoYeuCau { get; set; }

        [Display(Name = "Kinh nghiệm (năm)")]
        public int SoNamKinhNghiemToiThieu { get; set; }

        [Display(Name = "Số lượng cần tuyển")]
        public int SoLuongCanTuyen { get; set; }

        [Display(Name = "Ngày bắt đầu")]
        [DataType(DataType.Date)]
        public DateTime NgayBatDauNhanHoSo { get; set; }

        [Display(Name = "Hạn nộp hồ sơ")]
        [DataType(DataType.Date)]
        public DateTime HanNopHoSo { get; set; }

        [Display(Name = "Trạng thái")]
        public string TrangThai { get; set; } = string.Empty;

        [Display(Name = "Số hồ sơ đã nộp")]
        public int SoHoSoDaNop { get; set; }

        [Display(Name = "Đã trúng tuyển")]
        public int SoDaTrungTuyen { get; set; }

        // --- THUỘC TÍNH TÍNH TOÁN ---
        public bool ConHan => DateTime.Today >= NgayBatDauNhanHoSo.Date && DateTime.Today <= HanNopHoSo.Date;

        public bool DaDuChiTieu => SoDaTrungTuyen >= SoLuongCanTuyen;

        public bool CoTheUngTuyen => TrangThai == "Đang tuyển" && ConHan && !DaDuChiTieu;

        public int SoNgayConLai => Math.Max(0, (HanNopHoSo.Date - DateTime.Today).Days);
    }

    // VIEWMODEL: TRANG DANH SÁCH VỊ TRÍ (CÓ TÌM KIẾM, LỌC, PHÂN TRANG)
    public class DanhSachViTriViewModel
    {
        public List<ViTriTuyenDungItemViewModel> Items { get; set; } = new List<ViTriTuyenDungItemViewModel>();

        // Bộ lọc
        [Display(Name = "Tìm kiếm")]
        public string? SearchString { get; set; }

        [Display(Name = "Phòng ban")]
        public int? MaPhongBan { get; set; }

        [Display(Name = "Trạng thái")]
        public string? TrangThai { get; set; }

        [Display(Name = "Trình độ")]
        public string? TrinhDo { get; set; }

        // Dữ liệu cho các dropdown lọc
        public IEnumerable<SelectListItem> DanhSachPhongBan { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> DanhSachTrangThai { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> DanhSachTrinhDo { get; set; } = new List<SelectListItem>();

        // Phân trang
        public int TrangHienTai { get; set; } = 1;
        public int SoBanGhiMoiTrang { get; set; } = 10;
        public int TongSoBanGhi { get; set; }
        public int TongSoTrang => (int)Math.Ceiling((double)TongSoBanGhi / SoBanGhiMoiTrang);
        public bool CoTrangTruoc => TrangHienTai > 1;
        public bool CoTrangSau => TrangHienTai < TongSoTrang;
    }

    // VIEWMODEL: TRANG CHI TIẾT VỊ TRÍ TUYỂN DỤNG
    public class ChiTietViTriViewModel
    {
        public int MaViTri { get; set; }

        [Display(Name = "Tên vị trí")]
        public string TenViTri { get; set; } = string.Empty;

        [Display(Name = "Số lượng cần tuyển")]
        public int SoLuongCanTuyen { get; set; }

        [Display(Name = "Trình độ yêu cầu")]
        public string? TrinhDoYeuCau { get; set; }

        [Display(Name = "Kinh nghiệm tối thiểu (năm)")]
        public int SoNamKinhNghiemToiThieu { get; set; }

        [Display(Name = "Ngày bắt đầu nhận hồ sơ")]
        [DataType(DataType.Date)]
        public DateTime NgayBatDauNhanHoSo { get; set; }

        [Display(Name = "Hạn nộp hồ sơ")]
        [DataType(DataType.Date)]
        public DateTime HanNopHoSo { get; set; }

        [Display(Name = "Mô tả công việc")]
        public string MoTaCongViec { get; set; } = string.Empty;

        [Display(Name = "Yêu cầu ứng viên")]
        public string YeuCauUngVien { get; set; } = string.Empty;

        [Display(Name = "Trạng thái")]
        public string TrangThai { get; set; } = string.Empty;

        // Thông tin phòng ban
        public int MaPhongBan { get; set; }

        [Display(Name = "Phòng ban")]
        public string TenPhongBan { get; set; } = string.Empty;

        [Display(Name = "Email liên hệ")]
        public string? EmailLienHe { get; set; }

        // Thống kê hồ sơ (dành cho Admin / Nhân sự)
        [Display(Name = "Tổng số hồ sơ")]
        public int TongSoHoSo { get; set; }

        [Display(Name = "Đã trúng tuyển")]
        public int SoDaTrungTuyen { get; set; }

        // Key: trạng thái hồ sơ (Chờ duyệt, Đạt sơ tuyển, ...), Value: số lượng
        public Dictionary<string, int> ThongKeTheoTrangThai { get; set; } = new Dictionary<string, int>();

        // Thông tin cho ứng viên đang đăng nhập
        public bool DaUngTuyen { get; set; }

        // --- THUỘC TÍNH TÍNH TOÁN ---
        public bool ConHan => DateTime.Today >= NgayBatDauNhanHoSo.Date && DateTime.Today <= HanNopHoSo.Date;

        public bool DaDuChiTieu => SoDaTrungTuyen >= SoLuongCanTuyen;

        public bool CoTheUngTuyen => TrangThai == "Đang tuyển" && ConHan && !DaDuChiTieu;

        public int SoLuongConLai => Math.Max(0, SoLuongCanTuyen - SoDaTrungTuyen);
    }
}
