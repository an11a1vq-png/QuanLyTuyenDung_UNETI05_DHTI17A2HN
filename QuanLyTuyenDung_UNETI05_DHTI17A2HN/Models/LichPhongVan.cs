using System.ComponentModel.DataAnnotations;


namespace QuanLyTuyenDung_UNETI05_DHTI17A2HN.Models;
using QuanLyTuyenDung_UNETI05_DHTI17A2HN.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class LichPhongVan
{
    [Key]
    public int MaLichPhongVan { get; set; }

    [Required(ErrorMessage = "Mã hồ sơ không được để trống")]
    public int MaHoSo { get; set; }

    [ForeignKey("MaHoSo")]
    public virtual HoSoUngTuyen HoSoUngTuyen { get; set; }

    [Required(ErrorMessage = "Thời gian bắt đầu là bắt buộc")]
    [DataType(DataType.DateTime)]
    public DateTime ThoiGianBatDau { get; set; }

    [Required(ErrorMessage = "Thời gian kết thúc là bắt buộc")]
    [DataType(DataType.DateTime)]
    public DateTime ThoiGianKetThuc { get; set; }

    [StringLength(100)]
    public string HinhThucPhongVan { get; set; }

    [StringLength(255)]
    public string DiaDiemHoacLienKet { get; set; }

    [Required(ErrorMessage = "Người phỏng vấn không được để trống")]
    [StringLength(100)]
    public string NguoiPhongVan { get; set; }

    public string GhiChu { get; set; }

    [Required]
    [StringLength(50)]
    public string TrangThai { get; set; } = "Đã lên lịch";
}