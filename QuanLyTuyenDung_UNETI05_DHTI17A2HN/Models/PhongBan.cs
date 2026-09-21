using System.ComponentModel.DataAnnotations;

namespace QuanLyTuyenDung_UNETI05_DHTI17A2HN.Models
{
    public class PhongBan
    {
        [Key]
        public int MaPhongBan { get; set; }

        [Required(ErrorMessage = "Tên phòng ban không được để trống")]
        [StringLength(100)]
        [Display(Name = "Tên phòng ban")]
        public string TenPhongBan { get; set; } = string.Empty;

        [Display(Name = "Mô tả")]
        public string? MoTa { get; set; }

        [EmailAddress(ErrorMessage = "Email liên hệ không đúng định dạng")]
        [StringLength(100)]
        [Display(Name = "Email liên hệ")]
        public string? EmailLienHe { get; set; }

        [Display(Name = "Trạng thái")]
        public bool TrangThai { get; set; } = true; 

        public virtual ICollection<ViTriTuyenDung> DanhSachViTri { get; set; } = new List<ViTriTuyenDung>();
    }
}
