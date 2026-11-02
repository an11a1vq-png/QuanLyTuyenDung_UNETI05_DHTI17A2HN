using System.ComponentModel.DataAnnotations;
namespace QuanLyTuyenDung_UNETI05_DHTI17A2HN.Models;
public class LichSuViTri
{
    [Key] public int MaLichSu { get; set; }
    public int? MaViTri { get; set; }
    public ViTriTuyenDung? ViTri { get; set; }
    [MaxLength(150)] public string TenViTri { get; set; } = "";
    public int? MaTaiKhoan { get; set; }
    [MaxLength(100)] public string NguoiThucHien { get; set; } = "Hệ thống";
    [MaxLength(100)] public string HanhDong { get; set; } = "";
    public string NoiDung { get; set; } = "";
    public DateTime ThoiGian { get; set; } = DateTime.Now;
}
