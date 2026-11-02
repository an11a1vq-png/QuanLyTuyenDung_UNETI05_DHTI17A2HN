namespace QuanLyTuyenDung_UNETI05_DHTI17A2HN.Models;
public class ViecLamYeuThich
{
    public int MaTaiKhoan { get; set; }
    public TaiKhoan TaiKhoan { get; set; } = null!;
    public int MaViTri { get; set; }
    public ViTriTuyenDung ViTri { get; set; } = null!;
    public DateTime NgayLuu { get; set; } = DateTime.Now;
}
