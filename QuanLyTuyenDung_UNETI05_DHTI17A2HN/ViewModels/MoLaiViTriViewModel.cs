using System.ComponentModel.DataAnnotations;
namespace QuanLyTuyenDung_UNETI05_DHTI17A2HN.ViewModels;
public class MoLaiViTriViewModel
{
    public int MaViTri { get; set; }
    public string? TenViTri { get; set; }
    [Required, DataType(DataType.Date), Display(Name = "Hạn nộp mới")]
    public DateTime HanNopMoi { get; set; } = DateTime.Today.AddDays(30);
    [Range(1,1000), Display(Name = "Chỉ tiêu tuyển dụng")]
    public int SoLuongCanTuyen { get; set; }
}
