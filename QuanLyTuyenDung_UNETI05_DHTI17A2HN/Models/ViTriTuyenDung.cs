using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyTuyenDung_UNETI05_DHTI17A2HN.Models
{
    public class ViTriTuyenDung
    {
        [Key]
        [Display(Name = "Mã vị trí")]
        public int MaViTri { get; set; }

        [Required(ErrorMessage = "Tên vị trí không được để trống")]
        [StringLength(150)]
        [Display(Name = "Tên vị trí")]
        public string TenViTri { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng chọn phòng ban")]
        [Display(Name = "Phòng ban")]
        public int MaPhongBan { get; set; }

        [Required(ErrorMessage = "Số lượng cần tuyển không được để trống")]
        [Range(1, 1000, ErrorMessage = "Số lượng cần tuyển từ 1 đến 1000")]
        [Display(Name = "Số lượng cần tuyển")]
        public int SoLuongCanTuyen { get; set; } = 1;

        [StringLength(50)]
        [Display(Name = "Trình độ yêu cầu")]
        public string? TrinhDoYeuCau { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Số năm kinh nghiệm phải >= 0")]
        [Display(Name = "Kinh nghiệm tối thiểu (năm)")]
        public int SoNamKinhNghiemToiThieu { get; set; }

        [Required(ErrorMessage = "Ngày bắt đầu nhận hồ sơ không được để trống")]
        [DataType(DataType.Date)]
        [Column(TypeName = "date")]
        [Display(Name = "Ngày bắt đầu nhận hồ sơ")]
        public DateTime NgayBatDauNhanHoSo { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Hạn nộp hồ sơ không được để trống")]
        [DataType(DataType.Date)]
        [Column(TypeName = "date")]
        [Display(Name = "Hạn nộp hồ sơ")]
        public DateTime HanNopHoSo { get; set; } = DateTime.Today.AddDays(30);

        [Required(ErrorMessage = "Mô tả công việc không được để trống")]
        [Display(Name = "Mô tả công việc")]
        public string MoTaCongViec { get; set; } = string.Empty;

        [Required(ErrorMessage = "Yêu cầu ứng viên không được để trống")]
        [Display(Name = "Yêu cầu ứng viên")]
        public string YeuCauUngVien { get; set; } = string.Empty;

        [Required]
        [StringLength(30)]
        [Display(Name = "Trạng thái")]
        public string TrangThai { get; set; } = "Đang tuyển";

        [ForeignKey("MaPhongBan")]
        [Display(Name = "Phòng ban")]
        public virtual PhongBan? PhongBan { get; set; }

        public virtual ICollection<HoSoUngTuyen> HoSoUngTuyens { get; set; } = new List<HoSoUngTuyen>();

        public static readonly string[] DanhSachTrinhDo = { "Trung cấp", "Cao đẳng", "Đại học", "Thạc sĩ", "Tiến sĩ" };
        public static readonly string[] DanhSachTrangThai = { "Chưa mở", "Đang tuyển", "Tạm dừng", "Đã đóng" };
    }
}