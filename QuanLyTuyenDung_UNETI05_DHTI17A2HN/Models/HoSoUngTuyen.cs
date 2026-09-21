using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyTuyenDung_UNETI05_DHTI17A2HN.Models
{
    public class HoSoUngTuyen
    {
        [Key]
        [Display(Name = "Mã hồ sơ")]
        public int MaHoSo { get; set; }
        [Required(ErrorMessage = "Mã ứng viên không được để trống")]
        [Display(Name = "Mã ứng viên")]
        public int MaUngVien { get; set; }
        [Required(ErrorMessage = "Mã vị trí tuyển dụng không được để trống")]
        [Display(Name = "Mã vị trí tuyển dụng")]
        public int MaViTri { get; set; }
        [Required]
        [DataType(DataType.DateTime)]
        [Display(Name = "Ngày nộp hồ sơ")]
        public DateTime NgayNop { get; set; } = DateTime.Now;
        [Display(Name = "Thư giới thiệu / Đơn ứng tuyển")]
        public string? ThuGioiThieu { get; set; }
        [Display(Name = "Ghi chú ứng viên")]
        public string? GhiChu { get; set; }
        [Required(ErrorMessage = "Trạng thái hồ sơ không được để trống")]
        [StringLength(50)]
        [Display(Name = "Trạng thái hồ sơ")]
        public string TrangThai { get; set; } = "Chờ duyệt";
        [DataType(DataType.DateTime)]
        [Display(Name = "Ngày xử lý sơ tuyển")]
        public DateTime? NgayXuLy { get; set; }
        [Display(Name = "Nhận xét sơ tuyển")]
        public string? NhanXetSoTuyen { get; set; }
        // --- NAVIGATION PROPERTIES (QUAN HỆ BẢNG) ---
        [ForeignKey("MaUngVien")]
        public virtual UngVien? UngVien { get; set; }
        [ForeignKey("MaViTri")]
        public virtual ViTriTuyenDung? ViTriTuyenDung { get; set; }
        public virtual ICollection<LichPhongVan> LichPhongVans { get; set; } = new List<LichPhongVan>();
        public virtual KetQuaTuyenDung? KetQuaTuyenDung { get; set; }
    }
}
