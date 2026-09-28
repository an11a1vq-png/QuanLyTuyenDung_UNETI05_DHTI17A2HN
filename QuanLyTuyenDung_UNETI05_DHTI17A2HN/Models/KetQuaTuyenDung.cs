using QuanLyTuyenDung_UNETI05_DHTI17A2HN.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class KetQuaTuyenDung
{
    [Key]
    public int MaKetQua { get; set; }

    [Required(ErrorMessage = "Mã hồ sơ không được để trống")]
    public int MaHoSo { get; set; }

    [ForeignKey("MaHoSo")]
    public virtual HoSoUngTuyen HoSoUngTuyen { get; set; }

    [Range(0, 100, ErrorMessage = "Điểm đánh giá phải từ 0 đến 100")]
    public double DiemDanhGia { get; set; }

    public string NhanXet { get; set; }

    [Required(ErrorMessage = "Kết quả tuyển dụng không được để trống")]
    [StringLength(50)]
    public string KetQua { get; set; }

    [DataType(DataType.Date)]
    public DateTime NgayCapNhat { get; set; } = DateTime.Now;
}