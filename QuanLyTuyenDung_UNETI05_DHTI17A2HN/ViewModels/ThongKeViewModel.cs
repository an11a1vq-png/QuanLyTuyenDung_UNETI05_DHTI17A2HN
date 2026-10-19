// ==============================================================================
// Họ và tên: Hoàng Dương
// Mã sinh viên: 23103100117
// Module 5: Lịch phỏng vấn, Kết quả tuyển dụng, Dashboard và Thống kê
// Nội dung thực hiện: ViewModel chứa các chỉ số Dashboard và kết quả thống kê bằng LINQ
//                     theo yêu cầu Mục 9.5 và Mục 9.6 Đề tài 13.
// ==============================================================================

using System.Collections.Generic;

namespace QuanLyTuyenDung_UNETI05_DHTI17A2HN.ViewModels
{
    public class ThongKeViewModel
    {
        // ==============================================================================
        // 1. CHỈ SỐ DASHBOARD TỔNG QUAN (Mục 9.5 Đề tài 13)
        // ==============================================================================
        public int TongSoPhongBan { get; set; }
        public int TongSoViTri { get; set; }
        public int SoViTriDangTuyen { get; set; }
        public int TongSoUngVien { get; set; }
        public int TongSoHoSo { get; set; }
        public int SoHoSoChoDuyet { get; set; }
        public int SoHoSoDatSoTuyen { get; set; }
        public int SoLichPVSapToi { get; set; }
        public int SoUngVienTrungTuyen { get; set; }

        // ==============================================================================
        // 2. KẾT QUẢ THỐNG KÊ BẰNG LINQ (Mục 9.6 Đề tài 13)
        // ==============================================================================
        // 1. Số vị trí tuyển dụng theo phòng ban
        public List<ItemThongKe> ViTriTheoPhongBan { get; set; } = new();

        // 2. Số hồ sơ theo từng vị trí tuyển dụng
        public List<ItemThongKe> HoSoTheoViTri { get; set; } = new();

        // 3. Số hồ sơ theo trạng thái
        public List<ItemThongKe> HoSoTheoTrangThai { get; set; } = new();

        // 4. Số hồ sơ đạt sơ tuyển theo vị trí
        public List<ItemThongKe> DatSoTuyenTheoViTri { get; set; } = new();

        // 5. Vị trí nhận nhiều hồ sơ nhất
        public string ViTriNhieuHoSoNhat { get; set; } = "Chưa có";
        public int SoHoSoNhieuNhat { get; set; } = 0;

        // 6. Số ứng viên trúng tuyển theo phòng ban
        public List<ItemThongKe> TrungTuyenTheoPhongBan { get; set; } = new();

        // 7. Số hồ sơ nộp theo tháng
        public List<ItemThongKe> HoSoTheoThang { get; set; } = new();

        // 8. Số cuộc phỏng vấn theo tháng
        public List<ItemThongKe> PhongVanTheoThang { get; set; } = new();

        // 9. Tỷ lệ hồ sơ qua sơ tuyển theo từng vị trí
        public List<TyLeSoTuyenItem> TyLeSoTuyenTheoViTri { get; set; } = new();

        // 10. Tỷ lệ trúng tuyển trên số hồ sơ đã phỏng vấn
        public int TongDaPhongVan { get; set; }
        public int TongTrungTuyen { get; set; }
        public double TyLeTrungTuyenTrenPhongVan { get; set; }
    }

    public class ItemThongKe
    {
        public string Ten { get; set; } = string.Empty;
        public int SoLuong { get; set; }
    }

    public class TyLeSoTuyenItem
    {
        public string TenViTri { get; set; } = string.Empty;
        public int TongSoHoSo { get; set; }
        public int SoLuongQuaSoTuyen { get; set; }
        public double TyLePhanTram { get; set; }
    }
}
