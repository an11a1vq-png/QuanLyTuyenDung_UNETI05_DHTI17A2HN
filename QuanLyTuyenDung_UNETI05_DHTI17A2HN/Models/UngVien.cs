using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyTuyenDung_UNETI05_DHTI17A2HN.Models
{
    public class UngVien
    {
        [Key]
        [Display(Name = "Mã ứng viên")]
        public int MaUngVien { get; set; }
        [Required(ErrorMessage = "Mã tài khoản không được để trống")]
        [Display(Name = "Mã tài khoản")]
        public int MaTaiKhoan { get; set; }
        [Required(ErrorMessage = "Họ và tên ứng viên không được để trống")]
        [StringLength(100, ErrorMessage = "Họ và tên không được vượt quá 100 ký tự")]
        [Display(Name = "Họ và tên")]
        public string HoTen { get; set; } = string.Empty;
        [DataType(DataType.Date)]
        [Display(Name = "Ngày sinh")]
        public DateTime? NgaySinh { get; set; }
        [StringLength(10, ErrorMessage = "Giới tính không quá 10 ký tự")]
        [Display(Name = "Giới tính")]
        public string? GioiTinh { get; set; }
        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        [Phone(ErrorMessage = "Số điện thoại không đúng định dạng")]
        [StringLength(15, ErrorMessage = "Số điện thoại không quá 15 ký tự")]
        [Display(Name = "Số điện thoại")]
        public string SoDienThoai { get; set; } = string.Empty;
        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        [StringLength(100, ErrorMessage = "Email không vượt quá 100 ký tự")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;
        [StringLength(250, ErrorMessage = "Địa chỉ không vượt quá 250 ký tự")]
        [Display(Name = "Địa chỉ")]
        public string? DiaChi { get; set; }
        [StringLength(100, ErrorMessage = "Trình độ học vấn không vượt quá 100 ký tự")]
        [Display(Name = "Trình độ học vấn")]
        public string? TrinhDoHocVan { get; set; }
        [StringLength(100, ErrorMessage = "Chuyên ngành không vượt quá 100 ký tự")]
        [Display(Name = "Chuyên ngành")]
        public string? ChuyenNganh { get; set; }
        [Range(0, 50, ErrorMessage = "Số năm kinh nghiệm phải từ 0 đến 50 năm")]
        [Display(Name = "Số năm kinh nghiệm")]
        public int SoNamKinhNghiem { get; set; } = 0;
        [Display(Name = "Kỹ năng / Ghi chú")]
        public string? KyNang { get; set; }
        [Display(Name = "Trạng thái hoạt động")]
        public bool TrangThai { get; set; } = true;
        // --- NAVIGATION PROPERTIES (QUAN HỆ BẢNG) ---
        [ForeignKey("MaTaiKhoan")]
        public virtual TaiKhoan? TaiKhoan { get; set; }
        public virtual ICollection<HoSoUngTuyen> HoSoUngTuyens { get; set; } = new List<HoSoUngTuyen>();
    }
}
