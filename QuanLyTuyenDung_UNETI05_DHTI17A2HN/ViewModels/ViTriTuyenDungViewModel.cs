// ==============================================================================
// Họ và tên: Lê Gia Bảo
// Mã sinh viên: 23103100100
// Module: 2 - Quản lý vị trí tuyển dụng
// Nội dung thực hiện: ViewModel đóng gói dữ liệu cho các màn hình vị trí tuyển dụng (ViTriTuyenDungController)
// ==============================================================================

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using QuanLyTuyenDung_UNETI05_DHTI17A2HN.Models;

namespace QuanLyTuyenDung_UNETI05_DHTI17A2HN.ViewModels
{
    // HẰNG SỐ TRẠNG THÁI DÙNG CHUNG (tránh gõ sai chuỗi ở nhiều nơi)
    public static class TrangThaiTuyenDung
    {
        // Trạng thái vị trí (khớp ViTriTuyenDung.DanhSachTrangThai)
        public const string ChuaMo = "Chưa mở";
        public const string DangTuyen = "Đang tuyển";
        public const string TamDung = "Tạm dừng";
        public const string DaDong = "Đã đóng";

        // Trạng thái hồ sơ (khớp HoSoUngTuyenController)
        public const string ChoDuyet = "Chờ duyệt";
        public const string DatSoTuyen = "Đạt sơ tuyển";
        public const string KhongDatSoTuyen = "Không đạt sơ tuyển";
        public const string DaPhongVan = "Đã phỏng vấn";
        public const string TrungTuyen = "Trúng tuyển";
        public const string KhongTrungTuyen = "Không trúng tuyển";
        public const string DaHuy = "Đã hủy";

        // Hồ sơ ở các trạng thái này coi như đã kết thúc, ứng viên được nộp lại
        public static readonly string[] HoSoDaKetThuc = { DaHuy, KhongDatSoTuyen, KhongTrungTuyen };

        // Số ngày còn lại để coi là "sắp hết hạn"
        public const int SoNgaySapHetHan = 7;
    }

    // VIEWMODEL: DÙNG CHO FORM THÊM MỚI / CẬP NHẬT VỊ TRÍ TUYỂN DỤNG
    public class ViTriTuyenDungFormViewModel : IValidatableObject
    {
        // = 0 khi thêm mới, > 0 khi cập nhật
        public int MaViTri { get; set; }

        [Required(ErrorMessage = "Tên vị trí không được để trống")]
        [StringLength(150, ErrorMessage = "Tên vị trí tối đa 150 ký tự")]
        [Display(Name = "Tên vị trí")]
        public string TenViTri { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn phòng ban")]
        [Display(Name = "Phòng ban")]
        public int MaPhongBan { get; set; }

        [Range(1, 1000, ErrorMessage = "Số lượng cần tuyển từ 1 đến 1000")]
        [Display(Name = "Số lượng cần tuyển")]
        public int SoLuongCanTuyen { get; set; } = 1;

        [StringLength(50)]
        [Display(Name = "Trình độ yêu cầu")]
        public string? TrinhDoYeuCau { get; set; }

        [Range(0, 50, ErrorMessage = "Số năm kinh nghiệm từ 0 đến 50")]
        [Display(Name = "Kinh nghiệm tối thiểu (năm)")]
        public int SoNamKinhNghiemToiThieu { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Ngày bắt đầu nhận hồ sơ")]
        public DateTime NgayBatDauNhanHoSo { get; set; } = DateTime.Today;

        [DataType(DataType.Date)]
        [Display(Name = "Hạn nộp hồ sơ")]
        public DateTime HanNopHoSo { get; set; } = DateTime.Today.AddDays(30);

        [Required(ErrorMessage = "Mô tả công việc không được để trống")]
        [Display(Name = "Mô tả công việc")]
        public string MoTaCongViec { get; set; } = string.Empty;

        [Required(ErrorMessage = "Yêu cầu ứng viên không được để trống")]
        [Display(Name = "Yêu cầu ứng viên")]
        public string YeuCauUngVien { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng chọn trạng thái")]
        [StringLength(30)]
        [Display(Name = "Trạng thái")]
        public string TrangThai { get; set; } = TrangThaiTuyenDung.DangTuyen;

        // Số hồ sơ đã trúng tuyển: controller tự đếm từ CSDL,
        // [BindNever] để client không thể gửi giá trị giả lên qua form
        [BindNever]
        public int SoDaTrungTuyen { get; set; }

        public bool LaCapNhat => MaViTri > 0;

        // Dữ liệu cho các dropdown: không bind từ form, không validate
        [BindNever, ValidateNever]
        public IEnumerable<SelectListItem> DanhSachPhongBan { get; set; } = Enumerable.Empty<SelectListItem>();

        [BindNever, ValidateNever]
        public IEnumerable<SelectListItem> DanhSachTrinhDo { get; set; } = Enumerable.Empty<SelectListItem>();

        [BindNever, ValidateNever]
        public IEnumerable<SelectListItem> DanhSachTrangThai { get; set; } = Enumerable.Empty<SelectListItem>();

        // Kiểm tra ràng buộc giữa các trường và danh mục cố định
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (HanNopHoSo.Date < NgayBatDauNhanHoSo.Date)
            {
                yield return new ValidationResult(
                    "Hạn nộp hồ sơ phải sau hoặc bằng ngày bắt đầu nhận hồ sơ",
                    new[] { nameof(HanNopHoSo) });
            }

            if (!string.IsNullOrEmpty(TrinhDoYeuCau) && !ViTriTuyenDung.DanhSachTrinhDo.Contains(TrinhDoYeuCau))
            {
                yield return new ValidationResult("Trình độ không hợp lệ", new[] { nameof(TrinhDoYeuCau) });
            }

            if (!ViTriTuyenDung.DanhSachTrangThai.Contains(TrangThai))
            {
                yield return new ValidationResult("Trạng thái không hợp lệ", new[] { nameof(TrangThai) });
            }
        }

        // Validate() chạy lúc model binding, khi SoDaTrungTuyen chưa có giá trị.
        // Controller gán SoDaTrungTuyen (đếm từ CSDL) rồi gọi hàm này trước khi kiểm tra ModelState.IsValid
        public void KiemTraChiTieu(ModelStateDictionary modelState)
        {
            if (LaCapNhat && SoLuongCanTuyen < SoDaTrungTuyen)
            {
                modelState.AddModelError(nameof(SoLuongCanTuyen),
                    $"Số lượng cần tuyển không được nhỏ hơn số người đã trúng tuyển ({SoDaTrungTuyen})");
            }
        }

        // Nạp sẵn dropdown trình độ / trạng thái (phòng ban do controller truy vấn CSDL)
        public void NapDanhMucCoDinh()
        {
            DanhSachTrinhDo = new SelectList(ViTriTuyenDung.DanhSachTrinhDo, TrinhDoYeuCau);
            DanhSachTrangThai = new SelectList(ViTriTuyenDung.DanhSachTrangThai, TrangThai);
        }

        // Nạp dropdown phòng ban từ danh sách controller đã truy vấn.
        // Chỉ hiện phòng ban đang hoạt động, nhưng vẫn giữ phòng ban hiện tại khi cập nhật
        public void NapPhongBan(IEnumerable<PhongBan> dsPhongBan)
        {
            DanhSachPhongBan = dsPhongBan
                .Where(p => p.TrangThai || p.MaPhongBan == MaPhongBan)
                .OrderBy(p => p.TenPhongBan)
                .Select(p => new SelectListItem
                {
                    Value = p.MaPhongBan.ToString(),
                    Text = p.TrangThai ? p.TenPhongBan : $"{p.TenPhongBan} (ngưng hoạt động)",
                    Selected = p.MaPhongBan == MaPhongBan
                })
                .ToList();
        }

        // Kiểm tra phòng ban được chọn (controller truyền vào sau khi FindAsync)
        public void KiemTraPhongBan(PhongBan? phongBan, ModelStateDictionary modelState)
        {
            if (phongBan == null)
            {
                modelState.AddModelError(nameof(MaPhongBan), "Phòng ban không tồn tại");
            }
            else if (!phongBan.TrangThai && TrangThai == TrangThaiTuyenDung.DangTuyen)
            {
                modelState.AddModelError(nameof(MaPhongBan), "Phòng ban đang ngưng hoạt động, không thể mở tuyển");
            }
        }

        // Entity -> ViewModel (màn hình cập nhật)
        public static ViTriTuyenDungFormViewModel TuEntity(ViTriTuyenDung v, int soDaTrungTuyen = 0) => new()
        {
            MaViTri = v.MaViTri,
            TenViTri = v.TenViTri,
            MaPhongBan = v.MaPhongBan,
            SoLuongCanTuyen = v.SoLuongCanTuyen,
            TrinhDoYeuCau = v.TrinhDoYeuCau,
            SoNamKinhNghiemToiThieu = v.SoNamKinhNghiemToiThieu,
            NgayBatDauNhanHoSo = v.NgayBatDauNhanHoSo,
            HanNopHoSo = v.HanNopHoSo,
            MoTaCongViec = v.MoTaCongViec,
            YeuCauUngVien = v.YeuCauUngVien,
            TrangThai = v.TrangThai,
            SoDaTrungTuyen = soDaTrungTuyen
        };

        // ViewModel -> Entity (thêm mới và cập nhật). Không đụng tới MaViTri và navigation
        public void GanVaoEntity(ViTriTuyenDung v)
        {
            v.TenViTri = TenViTri.Trim();
            v.MaPhongBan = MaPhongBan;
            v.SoLuongCanTuyen = SoLuongCanTuyen;
            v.TrinhDoYeuCau = string.IsNullOrWhiteSpace(TrinhDoYeuCau) ? null : TrinhDoYeuCau;
            v.SoNamKinhNghiemToiThieu = SoNamKinhNghiemToiThieu;
            v.NgayBatDauNhanHoSo = NgayBatDauNhanHoSo.Date;
            v.HanNopHoSo = HanNopHoSo.Date;
            v.MoTaCongViec = MoTaCongViec.Trim();
            v.YeuCauUngVien = YeuCauUngVien.Trim();
            v.TrangThai = TrangThai;
        }
    }

    // LỚP CƠ SỞ: CÁC TRƯỜNG VÀ THUỘC TÍNH TÍNH TOÁN DÙNG CHUNG CHO DANH SÁCH VÀ CHI TIẾT
    public abstract class ViTriTomTatViewModel
    {
        public int MaViTri { get; set; }

        [Display(Name = "Tên vị trí")]
        public string TenViTri { get; set; } = string.Empty;

        [Display(Name = "Phòng ban")]
        public string TenPhongBan { get; set; } = string.Empty;

        [Display(Name = "Trình độ yêu cầu")]
        public string? TrinhDoYeuCau { get; set; }

        [Display(Name = "Kinh nghiệm tối thiểu (năm)")]
        public int SoNamKinhNghiemToiThieu { get; set; }

        [Display(Name = "Số lượng cần tuyển")]
        public int SoLuongCanTuyen { get; set; }

        [Display(Name = "Ngày bắt đầu nhận hồ sơ")]
        [DataType(DataType.Date)]
        public DateTime NgayBatDauNhanHoSo { get; set; }

        [Display(Name = "Hạn nộp hồ sơ")]
        [DataType(DataType.Date)]
        public DateTime HanNopHoSo { get; set; }

        [Display(Name = "Trạng thái")]
        public string TrangThai { get; set; } = string.Empty;

        [Display(Name = "Số hồ sơ đã nộp")]
        public int SoHoSoDaNop { get; set; }

        [Display(Name = "Đã trúng tuyển")]
        public int SoDaTrungTuyen { get; set; }

        // --- THUỘC TÍNH TÍNH TOÁN ---
        public bool ConHan => DateTime.Today >= NgayBatDauNhanHoSo.Date && DateTime.Today <= HanNopHoSo.Date;

        public bool DaDuChiTieu => SoDaTrungTuyen >= SoLuongCanTuyen;

        public bool CoTheUngTuyen => TrangThai == TrangThaiTuyenDung.DangTuyen && ConHan && !DaDuChiTieu;

        public int SoNgayConLai => Math.Max(0, (HanNopHoSo.Date - DateTime.Today).Days);

        public int SoLuongConLai => Math.Max(0, SoLuongCanTuyen - SoDaTrungTuyen);

        public bool SapHetHan => CoTheUngTuyen && SoNgayConLai <= TrangThaiTuyenDung.SoNgaySapHetHan;

        // Phần trăm chỉ tiêu đã tuyển được (0 - 100), dùng cho progress bar
        public int TyLeHoanThanh => SoLuongCanTuyen <= 0
            ? 0
            : Math.Min(100, SoDaTrungTuyen * 100 / SoLuongCanTuyen);

        // Class CSS badge Bootstrap theo trạng thái, để view không phải viết switch
        public string BadgeTrangThai => TrangThai switch
        {
            TrangThaiTuyenDung.DangTuyen => "bg-success",
            TrangThaiTuyenDung.TamDung => "bg-warning text-dark",
            TrangThaiTuyenDung.DaDong => "bg-secondary",
            _ => "bg-light text-dark"
        };
    }

    // VIEWMODEL: MỘT DÒNG TRONG DANH SÁCH VỊ TRÍ TUYỂN DỤNG
    public class ViTriTuyenDungItemViewModel : ViTriTomTatViewModel
    {
        // Biểu thức chiếu cho EF: dùng trong .Select(...) để COUNT chạy dưới SQL,
        // không phải Include toàn bộ hồ sơ về bộ nhớ
        public static readonly Expression<Func<ViTriTuyenDung, ViTriTuyenDungItemViewModel>> Projection = v => new ViTriTuyenDungItemViewModel
        {
            MaViTri = v.MaViTri,
            TenViTri = v.TenViTri,
            TenPhongBan = v.PhongBan != null ? v.PhongBan.TenPhongBan : string.Empty,
            TrinhDoYeuCau = v.TrinhDoYeuCau,
            SoNamKinhNghiemToiThieu = v.SoNamKinhNghiemToiThieu,
            SoLuongCanTuyen = v.SoLuongCanTuyen,
            NgayBatDauNhanHoSo = v.NgayBatDauNhanHoSo,
            HanNopHoSo = v.HanNopHoSo,
            TrangThai = v.TrangThai,
            SoHoSoDaNop = v.HoSoUngTuyens.Count(),
            SoDaTrungTuyen = v.HoSoUngTuyens.Count(h => h.TrangThai == TrangThaiTuyenDung.TrungTuyen)
        };
    }

    // VIEWMODEL: TRANG DANH SÁCH VỊ TRÍ (CÓ TÌM KIẾM, LỌC, PHÂN TRANG)
    public class DanhSachViTriViewModel
    {
        public const int SoBanGhiMacDinh = 10;

        public IReadOnlyList<ViTriTuyenDungItemViewModel> Items { get; set; } = Array.Empty<ViTriTuyenDungItemViewModel>();

        // Bộ lọc
        [Display(Name = "Tìm kiếm")]
        public string? SearchString { get; set; }

        [Display(Name = "Phòng ban")]
        public int? MaPhongBan { get; set; }

        [Display(Name = "Trạng thái")]
        public string? TrangThai { get; set; }

        [Display(Name = "Trình độ")]
        public string? TrinhDo { get; set; }

        // Thẻ thống kê đầu trang quản lý (null ở trang việc làm của ứng viên)
        public ThongKeViTriViewModel? ThongKe { get; set; }

        public bool DangLoc =>!string.IsNullOrWhiteSpace(SearchString) || MaPhongBan.HasValue
                               || !string.IsNullOrEmpty(TrangThai) || !string.IsNullOrEmpty(TrinhDo);

        // Dữ liệu cho các dropdown lọc
        public IEnumerable<SelectListItem> DanhSachPhongBan { get; set; } = Enumerable.Empty<SelectListItem>();
        public IEnumerable<SelectListItem> DanhSachTrangThai { get; set; } = Enumerable.Empty<SelectListItem>();
        public IEnumerable<SelectListItem> DanhSachTrinhDo { get; set; } = Enumerable.Empty<SelectListItem>();

        // Phân trang
        private int _soBanGhiMoiTrang = SoBanGhiMacDinh;
        public int SoBanGhiMoiTrang
        {
            get => _soBanGhiMoiTrang;
            set => _soBanGhiMoiTrang = value > 0 ? value : SoBanGhiMacDinh; // tránh chia cho 0
        }

        public int TrangHienTai { get; set; } = 1;
        public int TongSoBanGhi { get; set; }
        public int TongSoTrang => (TongSoBanGhi + SoBanGhiMoiTrang - 1) / SoBanGhiMoiTrang;
        public bool CoTrangTruoc => TrangHienTai > 1;
        public bool CoTrangSau => TrangHienTai < TongSoTrang;

        // Số bản ghi cần bỏ qua khi truy vấn: query.Skip(model.BoQua).Take(model.SoBanGhiMoiTrang)
        public int BoQua => (TrangHienTai - 1) * SoBanGhiMoiTrang;

        // Gọi sau khi đã gán TongSoBanGhi, để trang yêu cầu luôn nằm trong khoảng hợp lệ
        public void ChuanHoaTrang(int trangYeuCau)
        {
            TrangHienTai = Math.Clamp(trangYeuCau, 1, Math.Max(1, TongSoTrang));
        }

        // Nạp sẵn dropdown trình độ / trạng thái / sắp xếp (phòng ban do controller truy vấn CSDL)
        public void NapDanhMucCoDinh()
        {
            DanhSachTrangThai = new SelectList(ViTriTuyenDung.DanhSachTrangThai, TrangThai);
            DanhSachTrinhDo = new SelectList(ViTriTuyenDung.DanhSachTrinhDo, TrinhDo);
            DanhSachSapXep = new SelectList(CacKieuSapXep, "Key", "Value", SapXep);
        }

        // Sắp xếp
        [Display(Name = "Sắp xếp")]
        public string? SapXep { get; set; }

        public static readonly IReadOnlyDictionary<string, string> CacKieuSapXep = new Dictionary<string, string>
        {
            ["han_nop"] = "Hạn nộp gần nhất",
            ["moi_nhat"] = "Mới đăng",
            ["ten"] = "Tên vị trí (A-Z)",
            ["so_luong"] = "Số lượng cần tuyển"
        };

        public IEnumerable<SelectListItem> DanhSachSapXep { get; set; } = Enumerable.Empty<SelectListItem>();

        // Áp bộ lọc + sắp xếp lên truy vấn EF (chạy dưới SQL).
        // chiLayDangTuyen = true cho trang việc làm của ứng viên: bỏ qua lọc trạng thái,
        // chỉ lấy vị trí đang tuyển, đã mở và còn hạn
        public IQueryable<ViTriTuyenDung> ApDungBoLoc(IQueryable<ViTriTuyenDung> query, bool chiLayDangTuyen = false)
        {
            if (!string.IsNullOrWhiteSpace(SearchString))
            {
                var term = SearchString.Trim();
                query = query.Where(v => v.TenViTri.Contains(term) ||
                                         v.MoTaCongViec.Contains(term) ||
                                         (v.PhongBan != null && v.PhongBan.TenPhongBan.Contains(term)));
            }

            if (MaPhongBan.HasValue)
            {
                query = query.Where(v => v.MaPhongBan == MaPhongBan.Value);
            }

            if (!string.IsNullOrEmpty(TrinhDo))
            {
                query = query.Where(v => v.TrinhDoYeuCau == TrinhDo);
            }

            if (chiLayDangTuyen)
            {
                var homNay = DateTime.Today;
                query = query.Where(v => v.TrangThai == TrangThaiTuyenDung.DangTuyen &&
                                         v.NgayBatDauNhanHoSo <= homNay &&
                                         v.HanNopHoSo >= homNay);
            }
            else if (!string.IsNullOrEmpty(TrangThai))
            {
                query = query.Where(v => v.TrangThai == TrangThai);
            }

            return SapXep switch
            {
                "moi_nhat" => query.OrderByDescending(v => v.NgayBatDauNhanHoSo).ThenByDescending(v => v.MaViTri),
                "ten" => query.OrderBy(v => v.TenViTri),
                "so_luong" => query.OrderByDescending(v => v.SoLuongCanTuyen).ThenBy(v => v.HanNopHoSo),
                // Mặc định: vị trí đang tuyển lên trước, hạn nộp gần nhất trước
                _ => query.OrderByDescending(v => v.TrangThai == TrangThaiTuyenDung.DangTuyen).ThenBy(v => v.HanNopHoSo)
            };
        }
    }

    // VIEWMODEL: THẺ THỐNG KÊ TỔNG QUAN ĐẦU TRANG QUẢN LÝ VỊ TRÍ
    public class ThongKeViTriViewModel
    {
        [Display(Name = "Tổng số vị trí")]
        public int TongSoViTri { get; set; }

        [Display(Name = "Đang tuyển")]
        public int SoDangTuyen { get; set; }

        [Display(Name = "Sắp hết hạn")]
        public int SoSapHetHan { get; set; }

        [Display(Name = "Đã đóng")]
        public int SoDaDong { get; set; }

        [Display(Name = "Tổng chỉ tiêu")]
        public int TongChiTieu { get; set; }

        [Display(Name = "Đã trúng tuyển")]
        public int TongDaTrungTuyen { get; set; }

        [Display(Name = "Tổng hồ sơ")]
        public int TongHoSo { get; set; }

        public int TyLeHoanThanh => TongChiTieu <= 0 ? 0 : Math.Min(100, TongDaTrungTuyen * 100 / TongChiTieu);

        // Tính từ danh sách đã chiếu (dùng khi đã có sẵn Items, tránh query thêm)
        public static ThongKeViTriViewModel TuDanhSach(IEnumerable<ViTriTomTatViewModel> ds)
        {
            var list = ds as IList<ViTriTomTatViewModel> ?? ds.ToList();
            return new ThongKeViTriViewModel
            {
                TongSoViTri = list.Count,
                SoDangTuyen = list.Count(v => v.TrangThai == TrangThaiTuyenDung.DangTuyen),
                SoSapHetHan = list.Count(v => v.SapHetHan),
                SoDaDong = list.Count(v => v.TrangThai == TrangThaiTuyenDung.DaDong),
                TongChiTieu = list.Sum(v => v.SoLuongCanTuyen),
                TongDaTrungTuyen = list.Sum(v => v.SoDaTrungTuyen),
                TongHoSo = list.Sum(v => v.SoHoSoDaNop)
            };
        }
    }

    // VIEWMODEL: TRANG CHI TIẾT VỊ TRÍ TUYỂN DỤNG
    public class ChiTietViTriViewModel : ViTriTomTatViewModel
    {
        [Display(Name = "Mô tả công việc")]
        public string MoTaCongViec { get; set; } = string.Empty;

        [Display(Name = "Yêu cầu ứng viên")]
        public string YeuCauUngVien { get; set; } = string.Empty;

        // Thông tin phòng ban
        public int MaPhongBan { get; set; }

        [Display(Name = "Email liên hệ")]
        public string? EmailLienHe { get; set; }

        // Thống kê hồ sơ (dành cho Admin / Nhân sự)
        // Key: trạng thái hồ sơ (Chờ duyệt, Đạt sơ tuyển, ...), Value: số lượng
        public IReadOnlyDictionary<string, int> ThongKeTheoTrangThai { get; set; } = new Dictionary<string, int>();

        // Ứng viên đang đăng nhập đã có hồ sơ còn hiệu lực cho vị trí này chưa
        public bool DaUngTuyen { get; set; }

        // Hiển thị nút "Ứng tuyển" khi vị trí còn nhận hồ sơ và ứng viên chưa nộp
        public bool HienNutUngTuyen => CoTheUngTuyen && !DaUngTuyen;

        // Lý do không ứng tuyển được, để hiển thị thay cho nút (null = ứng tuyển được)
        public string? LyDoKhongTheUngTuyen
        {
            get
            {
                if (DaUngTuyen) return "Bạn đã nộp hồ sơ cho vị trí này.";
                if (TrangThai != TrangThaiTuyenDung.DangTuyen) return $"Vị trí đang ở trạng thái '{TrangThai}'.";
                if (DateTime.Today < NgayBatDauNhanHoSo.Date) return $"Bắt đầu nhận hồ sơ từ {NgayBatDauNhanHoSo:dd/MM/yyyy}.";
                if (DateTime.Today > HanNopHoSo.Date) return "Đã hết hạn nộp hồ sơ.";
                if (DaDuChiTieu) return "Vị trí đã tuyển đủ chỉ tiêu.";
                return null;
            }
        }

        // Lấy số hồ sơ của một trạng thái, trả 0 nếu chưa có
        public int SoHoSo(string trangThaiHoSo) =>
            ThongKeTheoTrangThai.TryGetValue(trangThaiHoSo, out var soLuong) ? soLuong : 0;

        // Hồ sơ đang chờ xử lý (chưa có kết quả cuối)
        public int SoHoSoDangXuLy => SoHoSo(TrangThaiTuyenDung.ChoDuyet)
                                   + SoHoSo(TrangThaiTuyenDung.DatSoTuyen)
                                   + SoHoSo(TrangThaiTuyenDung.DaPhongVan);

        // Biểu thức chiếu cho EF (giống ViTriTuyenDungItemViewModel.Projection)
        public static readonly Expression<Func<ViTriTuyenDung, ChiTietViTriViewModel>> Projection = v => new ChiTietViTriViewModel
        {
            MaViTri = v.MaViTri,
            TenViTri = v.TenViTri,
            TenPhongBan = v.PhongBan != null ? v.PhongBan.TenPhongBan : string.Empty,
            TrinhDoYeuCau = v.TrinhDoYeuCau,
            SoNamKinhNghiemToiThieu = v.SoNamKinhNghiemToiThieu,
            SoLuongCanTuyen = v.SoLuongCanTuyen,
            NgayBatDauNhanHoSo = v.NgayBatDauNhanHoSo,
            HanNopHoSo = v.HanNopHoSo,
            TrangThai = v.TrangThai,
            SoHoSoDaNop = v.HoSoUngTuyens.Count(),
            SoDaTrungTuyen = v.HoSoUngTuyens.Count(h => h.TrangThai == TrangThaiTuyenDung.TrungTuyen),
            MoTaCongViec = v.MoTaCongViec,
            YeuCauUngVien = v.YeuCauUngVien,
            MaPhongBan = v.MaPhongBan,
            EmailLienHe = v.PhongBan != null ? v.PhongBan.EmailLienHe : null
        };
    }
}
