// ==============================================================================
// Họ và tên: Đỗ Thiện An
// Mã sinh viên: 23103100082
// Module: 1 - Tài khoản, Đăng nhập, Phân quyền và Phòng ban
// Nội dung thực hiện: Migration khởi tạo cơ sở dữ liệu và nạp dữ liệu mẫu nhom5
// ==============================================================================

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace QuanLyTuyenDung_UNETI05_DHTI17A2HN.Migrations
{

    public partial class nhom5 : Migration
    {

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PhongBan",
                columns: table => new
                {
                    MaPhongBan = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenPhongBan = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmailLienHe = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhongBan", x => x.MaPhongBan);
                });

            migrationBuilder.CreateTable(
                name: "TaiKhoan",
                columns: table => new
                {
                    MaTaiKhoan = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenDangNhap = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MatKhau = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    HoTen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    VaiTro = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaiKhoan", x => x.MaTaiKhoan);
                });

            migrationBuilder.CreateTable(
                name: "ViTriTuyenDung",
                columns: table => new
                {
                    MaViTri = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenViTri = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    MaPhongBan = table.Column<int>(type: "int", nullable: false),
                    SoLuongCanTuyen = table.Column<int>(type: "int", nullable: false),
                    TrinhDoYeuCau = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SoNamKinhNghiemToiThieu = table.Column<int>(type: "int", nullable: false),
                    NgayBatDauNhanHoSo = table.Column<DateTime>(type: "date", nullable: false),
                    HanNopHoSo = table.Column<DateTime>(type: "date", nullable: false),
                    MoTaCongViec = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    YeuCauUngVien = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ViTriTuyenDung", x => x.MaViTri);
                    table.ForeignKey(
                        name: "FK_ViTriTuyenDung_PhongBan_MaPhongBan",
                        column: x => x.MaPhongBan,
                        principalTable: "PhongBan",
                        principalColumn: "MaPhongBan",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UngVien",
                columns: table => new
                {
                    MaUngVien = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaTaiKhoan = table.Column<int>(type: "int", nullable: false),
                    HoTen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NgaySinh = table.Column<DateTime>(type: "datetime2", nullable: true),
                    GioiTinh = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    SoDienThoai = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DiaChi = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    TrinhDoHocVan = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ChuyenNganh = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SoNamKinhNghiem = table.Column<int>(type: "int", nullable: false),
                    KyNang = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UngVien", x => x.MaUngVien);
                    table.ForeignKey(
                        name: "FK_UngVien_TaiKhoan_MaTaiKhoan",
                        column: x => x.MaTaiKhoan,
                        principalTable: "TaiKhoan",
                        principalColumn: "MaTaiKhoan",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HoSoUngTuyen",
                columns: table => new
                {
                    MaHoSo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaUngVien = table.Column<int>(type: "int", nullable: false),
                    MaViTri = table.Column<int>(type: "int", nullable: false),
                    NgayNop = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ThuGioiThieu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NgayXuLy = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NhanXetSoTuyen = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HoSoUngTuyen", x => x.MaHoSo);
                    table.ForeignKey(
                        name: "FK_HoSoUngTuyen_UngVien_MaUngVien",
                        column: x => x.MaUngVien,
                        principalTable: "UngVien",
                        principalColumn: "MaUngVien",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HoSoUngTuyen_ViTriTuyenDung_MaViTri",
                        column: x => x.MaViTri,
                        principalTable: "ViTriTuyenDung",
                        principalColumn: "MaViTri",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "KetQuaTuyenDung",
                columns: table => new
                {
                    MaKetQua = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaHoSo = table.Column<int>(type: "int", nullable: false),
                    DiemDanhGia = table.Column<double>(type: "float", nullable: false),
                    NhanXet = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    KetQua = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NgayCapNhat = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KetQuaTuyenDung", x => x.MaKetQua);
                    table.ForeignKey(
                        name: "FK_KetQuaTuyenDung_HoSoUngTuyen_MaHoSo",
                        column: x => x.MaHoSo,
                        principalTable: "HoSoUngTuyen",
                        principalColumn: "MaHoSo",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LichPhongVan",
                columns: table => new
                {
                    MaLichPhongVan = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaHoSo = table.Column<int>(type: "int", nullable: false),
                    ThoiGianBatDau = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ThoiGianKetThuc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HinhThucPhongVan = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DiaDiemHoacLienKet = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    NguoiPhongVan = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LichPhongVan", x => x.MaLichPhongVan);
                    table.ForeignKey(
                        name: "FK_LichPhongVan_HoSoUngTuyen_MaHoSo",
                        column: x => x.MaHoSo,
                        principalTable: "HoSoUngTuyen",
                        principalColumn: "MaHoSo",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "PhongBan",
                columns: new[] { "MaPhongBan", "EmailLienHe", "MoTa", "TenPhongBan", "TrangThai" },
                values: new object[,]
                {
                    { 1, "cntt@uneti.edu.vn", "Phòng kỹ thuật và phát triển phần mềm hệ thống", "Công nghệ thông tin", true },
                    { 2, "nhansu@uneti.edu.vn", "Phòng quản trị nguồn nhân lực, tuyển dụng và chế độ đãi ngộ", "Nhân sự", true },
                    { 3, "ketoan@uneti.edu.vn", "Phòng quản lý tài chính kế toán và ngân sách doanh nghiệp", "Kế toán", true },
                    { 4, "marketing@uneti.edu.vn", "Phòng truyền thông thương hiệu, quảng bá và phát triển thị trường", "Marketing", true },
                    { 5, "kinhdoanh@uneti.edu.vn", "Phòng phát triển khách hàng và kinh doanh giải pháp công nghệ", "Kinh doanh", true }
                });

            migrationBuilder.InsertData(
                table: "TaiKhoan",
                columns: new[] { "MaTaiKhoan", "Email", "HoTen", "MatKhau", "TenDangNhap", "TrangThai", "VaiTro" },
                values: new object[,]
                {
                    { 1, "admin@uneti.edu.vn", "Quản trị viên Hệ thống", "123", "admin", true, "Admin" },
                    { 2, "admin2@uneti.edu.vn", "Quản trị viên Dự phòng", "123", "admin2", true, "Admin" },
                    { 3, "ha.hr@uneti.edu.vn", "Nguyễn Thị Thu Hà", "123", "hr1", true, "Nhân sự" },
                    { 4, "minh.hr@uneti.edu.vn", "Trần Văn Minh", "123", "hr2", true, "Nhân sự" },
                    { 5, "chi.hr@uneti.edu.vn", "Lê Mai Chi", "123", "hr3", true, "Nhân sự" },
                    { 6, "ungvien1@gmail.com", "Nguyễn Văn An", "123", "ungvien1", true, "Ứng viên" },
                    { 7, "ungvien2@gmail.com", "Trần Thị Bích", "123", "ungvien2", true, "Ứng viên" },
                    { 8, "ungvien3@gmail.com", "Lê Hoàng Nam", "123", "ungvien3", true, "Ứng viên" },
                    { 9, "ungvien4@gmail.com", "Phạm Quốc Cường", "123", "ungvien4", true, "Ứng viên" },
                    { 10, "ungvien5@gmail.com", "Vũ Thị Duyên", "123", "ungvien5", true, "Ứng viên" },
                    { 11, "ungvien6@gmail.com", "Hoàng Văn Giang", "123", "ungvien6", true, "Ứng viên" },
                    { 12, "ungvien7@gmail.com", "Đặng Thùy Linh", "123", "ungvien7", true, "Ứng viên" },
                    { 13, "ungvien8@gmail.com", "Bùi Văn Hùng", "123", "ungvien8", true, "Ứng viên" },
                    { 14, "ungvien9@gmail.com", "Đỗ Mai Hương", "123", "ungvien9", true, "Ứng viên" },
                    { 15, "ungvien10@gmail.com", "Ngô Quang Khải", "123", "ungvien10", true, "Ứng viên" },
                    { 16, "ungvien11@gmail.com", "Dương Thị Lan", "123", "ungvien11", true, "Ứng viên" },
                    { 17, "ungvien12@gmail.com", "Lý Thành Long", "123", "ungvien12", true, "Ứng viên" },
                    { 18, "ungvien13@gmail.com", "Chu Minh Ngọc", "123", "ungvien13", true, "Ứng viên" },
                    { 19, "ungvien14@gmail.com", "Hà Thị Phương", "123", "ungvien14", true, "Ứng viên" },
                    { 20, "ungvien15@gmail.com", "Phan Văn Quân", "123", "ungvien15", true, "Ứng viên" },
                    { 21, "ungvien16@gmail.com", "Tạ Thị Quỳnh", "123", "ungvien16", true, "Ứng viên" },
                    { 22, "ungvien17@gmail.com", "Võ Hoài Sơn", "123", "ungvien17", true, "Ứng viên" },
                    { 23, "ungvien18@gmail.com", "Trịnh Thị Thanh", "123", "ungvien18", true, "Ứng viên" },
                    { 24, "ungvien19@gmail.com", "Lương Tuấn Tú", "123", "ungvien19", true, "Ứng viên" },
                    { 25, "ungvien20@gmail.com", "Cao Thị Uyên", "123", "ungvien20", true, "Ứng viên" },
                    { 26, "ungvien21@gmail.com", "Mai Văn Vũ", "123", "ungvien21", true, "Ứng viên" },
                    { 27, "ungvien22@gmail.com", "Đinh Thị Xuân", "123", "ungvien22", true, "Ứng viên" },
                    { 28, "ungvien23@gmail.com", "Đoàn Hữu Yên", "123", "ungvien23", true, "Ứng viên" },
                    { 29, "ungvien24@gmail.com", "Trương Gia Bảo", "123", "ungvien24", true, "Ứng viên" },
                    { 30, "ungvien25@gmail.com", "Vương Thảo Chi", "123", "ungvien25", true, "Ứng viên" },
                    { 31, "ungvien26@gmail.com", "Tô Đình Đạt", "123", "ungvien26", true, "Ứng viên" },
                    { 32, "ungvien27@gmail.com", "Lâm Khánh Huyền", "123", "ungvien27", true, "Ứng viên" },
                    { 33, "ungvien28@gmail.com", "Nghiêm Đức Khoa", "123", "ungvien28", true, "Ứng viên" },
                    { 34, "ungvien29@gmail.com", "Quách Mỹ Loan", "123", "ungvien29", true, "Ứng viên" },
                    { 35, "ungvien30@gmail.com", "Bạch Tiến Dũng", "123", "ungvien30", true, "Ứng viên" }
                });

            migrationBuilder.InsertData(
                table: "UngVien",
                columns: new[] { "MaUngVien", "ChuyenNganh", "DiaChi", "Email", "GioiTinh", "HoTen", "KyNang", "MaTaiKhoan", "NgaySinh", "SoDienThoai", "SoNamKinhNghiem", "TrangThai", "TrinhDoHocVan" },
                values: new object[,]
                {
                    { 1, "Công nghệ thông tin", "Số 12, Đường Giải Phóng, Quận Hoàng Mai, Hà Nội", "ungvien1@gmail.com", "Nam", "Nguyễn Văn An", "C#, ASP.NET Core, SQL Server, Git", 6, new DateTime(1997, 2, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), "0981000001", 2, true, "Đại học" },
                    { 2, "Quản trị nhân lực", "Số 24, Đường Giải Phóng, Quận Hoàng Mai, Hà Nội", "ungvien2@gmail.com", "Nữ", "Trần Thị Bích", "Sàng lọc CV, Phỏng vấn STAR, Luật Lao động", 7, new DateTime(1998, 3, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), "0981000002", 1, true, "Đại học" },
                    { 3, "Kế toán", "Số 36, Đường Giải Phóng, Quận Hoàng Mai, Hà Nội", "ungvien3@gmail.com", "Nam", "Lê Hoàng Nam", "Phần mềm MISA, Báo cáo thuế, Excel tài chính", 8, new DateTime(1999, 4, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), "0981000003", 3, true, "Cao đẳng" },
                    { 4, "Marketing", "Số 48, Đường Giải Phóng, Quận Hoàng Mai, Hà Nội", "ungvien4@gmail.com", "Nam", "Phạm Quốc Cường", "SEO, Facebook Ads, Google Analytics, Content", 9, new DateTime(2000, 5, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "0981000004", 2, true, "Đại học" },
                    { 5, "Quản trị kinh doanh", "Số 60, Đường Giải Phóng, Quận Hoàng Mai, Hà Nội", "ungvien5@gmail.com", "Nữ", "Vũ Thị Duyên", "Giao tiếp B2B, Thuyết trình, Kỹ năng đàm phán hợp đồng", 10, new DateTime(2001, 6, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), "0981000005", 4, true, "Thạc sĩ" },
                    { 6, "Khoa học máy tính", "Số 72, Đường Giải Phóng, Quận Hoàng Mai, Hà Nội", "ungvien6@gmail.com", "Nam", "Hoàng Văn Giang", "Java, Spring Boot, MySQL, Docker", 11, new DateTime(2002, 7, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), "0981000006", 3, true, "Đại học" },
                    { 7, "Truyền thông đa phương tiện", "Số 84, Đường Giải Phóng, Quận Hoàng Mai, Hà Nội", "ungvien7@gmail.com", "Nữ", "Đặng Thùy Linh", "Photoshop, Illustrator, Premiere, Sáng tạo nội dung", 12, new DateTime(2003, 8, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "0981000007", 1, true, "Đại học" },
                    { 8, "Tài chính ngân hàng", "Số 96, Đường Giải Phóng, Quận Hoàng Mai, Hà Nội", "ungvien8@gmail.com", "Nam", "Bùi Văn Hùng", "Kê khai thuế điện tử, Quyết toán hóa đơn VAT", 13, new DateTime(1996, 9, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), "0981000008", 2, true, "Cao đẳng" },
                    { 9, "Kỹ thuật phần mềm", "Số 108, Đường Giải Phóng, Quận Hoàng Mai, Hà Nội", "ungvien9@gmail.com", "Nữ", "Đỗ Mai Hương", "Python, Machine Learning, Django, REST API", 14, new DateTime(1997, 10, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "0981000009", 5, true, "Đại học" },
                    { 10, "Hệ thống thông tin", "Số 120, Đường Giải Phóng, Quận Hoàng Mai, Hà Nội", "ungvien10@gmail.com", "Nam", "Ngô Quang Khải", "ReactJS, TypeScript, TailwindCSS, NextJS", 15, new DateTime(1998, 11, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), "0981000010", 2, true, "Đại học" },
                    { 11, "Kinh doanh quốc tế", "Số 132, Đường Giải Phóng, Quận Hoàng Mai, Hà Nội", "ungvien11@gmail.com", "Nữ", "Dương Thị Lan", "Telesales, Chăm sóc khách hàng, CRM Salesforce", 16, new DateTime(1999, 12, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "0981000011", 1, true, "Cao đẳng" },
                    { 12, "An toàn thông tin", "Số 144, Đường Giải Phóng, Quận Hoàng Mai, Hà Nội", "ungvien12@gmail.com", "Nam", "Lý Thành Long", "Kiểm thử tự động Selenium, Postman, JMeter", 17, new DateTime(2000, 1, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), "0981000012", 3, true, "Đại học" },
                    { 13, "Quản trị nhân sự", "Số 156, Đường Giải Phóng, Quận Hoàng Mai, Hà Nội", "ungvien13@gmail.com", "Nữ", "Chu Minh Ngọc", "Đào tạo hội nhập, Xây dựng KPI, Đánh giá 360", 18, new DateTime(2001, 2, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "0981000013", 2, true, "Đại học" },
                    { 14, "Kiểm toán", "Số 168, Đường Giải Phóng, Quận Hoàng Mai, Hà Nội", "ungvien14@gmail.com", "Nữ", "Hà Thị Phương", "Phân tích báo cáo tài chính, Kiểm toán nội bộ", 19, new DateTime(2002, 3, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "0981000014", 4, true, "Đại học" },
                    { 15, "Thương mại điện tử", "Số 180, Đường Giải Phóng, Quận Hoàng Mai, Hà Nội", "ungvien15@gmail.com", "Nam", "Phan Văn Quân", "Digital Marketing, Email Marketing, TikTok Ads", 20, new DateTime(2003, 4, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "0981000015", 2, true, "Đại học" },
                    { 16, "Thiết kế đồ họa", "Số 192, Đường Giải Phóng, Quận Hoàng Mai, Hà Nội", "ungvien16@gmail.com", "Nữ", "Tạ Thị Quỳnh", "Canva, UI/UX cơ bản, Quay dựng video ngắn", 21, new DateTime(1996, 5, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "0981000016", 1, true, "Cao đẳng" },
                    { 17, "Mạng máy tính", "Số 204, Đường Giải Phóng, Quận Hoàng Mai, Hà Nội", "ungvien17@gmail.com", "Nam", "Võ Hoài Sơn", "Quản trị mạng CCNA, Cài đặt máy chủ Linux, Windows Server", 22, new DateTime(1997, 6, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "0981000017", 3, true, "Đại học" },
                    { 18, "Kế toán doanh nghiệp", "Số 216, Đường Giải Phóng, Quận Hoàng Mai, Hà Nội", "ungvien18@gmail.com", "Nữ", "Trịnh Thị Thanh", "Kế toán tiền lương, Chấm công, Bảo hiểm xã hội", 23, new DateTime(1998, 7, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "0981000018", 2, true, "Đại học" },
                    { 19, "Marketing số", "Số 228, Đường Giải Phóng, Quận Hoàng Mai, Hà Nội", "ungvien19@gmail.com", "Nam", "Lương Tuấn Tú", "Google Ads, Content Marketing, Viết bài chuẩn SEO", 24, new DateTime(1999, 8, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "0981000019", 2, true, "Đại học" },
                    { 20, "Quản trị kinh doanh", "Số 240, Đường Giải Phóng, Quận Hoàng Mai, Hà Nội", "ungvien20@gmail.com", "Nữ", "Cao Thị Uyên", "Thuyết phục khách hàng, Xử lý từ chối, Quản lý đơn hàng", 25, new DateTime(2000, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "0981000020", 1, true, "Cao đẳng" },
                    { 21, "Công nghệ thông tin", "Số 252, Đường Giải Phóng, Quận Hoàng Mai, Hà Nội", "ungvien21@gmail.com", "Nam", "Mai Văn Vũ", ".NET Web API, Microservices, Redis, RabbitMQ", 26, new DateTime(2001, 10, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "0981000021", 3, true, "Đại học" },
                    { 22, "Quản lý công nghiệp", "Số 264, Đường Giải Phóng, Quận Hoàng Mai, Hà Nội", "ungvien22@gmail.com", "Nữ", "Đinh Thị Xuân", "Lập kế hoạch đào tạo, Kỹ năng thuyết trình trước đám đông", 27, new DateTime(2002, 11, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), "0981000022", 2, true, "Đại học" },
                    { 23, "Khoa học dữ liệu", "Số 276, Đường Giải Phóng, Quận Hoàng Mai, Hà Nội", "ungvien23@gmail.com", "Nam", "Đoàn Hữu Yên", "SQL nâng cao, PowerBI, Phân tích dữ liệu kinh doanh", 28, new DateTime(2003, 12, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), "0981000023", 1, true, "Đại học" },
                    { 24, "Kỹ thuật phần mềm", "Số 288, Đường Giải Phóng, Quận Hoàng Mai, Hà Nội", "ungvien24@gmail.com", "Nam", "Trương Gia Bảo", "Fullstack C# + React, Kiến trúc phần mềm Clean Architecture", 29, new DateTime(1996, 1, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "0981000024", 5, true, "Thạc sĩ" },
                    { 25, "Tổ chức sự kiện", "Số 300, Đường Giải Phóng, Quận Hoàng Mai, Hà Nội", "ungvien25@gmail.com", "Nữ", "Vương Thảo Chi", "Quản lý fanpage, Tổ chức sự kiện tuyển dụng", 30, new DateTime(1997, 2, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "0981000025", 2, true, "Đại học" },
                    { 26, "Kinh tế đầu tư", "Số 312, Đường Giải Phóng, Quận Hoàng Mai, Hà Nội", "ungvien26@gmail.com", "Nam", "Tô Đình Đạt", "Tư vấn bán hàng, Chăm sóc đại lý phân phối", 31, new DateTime(1998, 3, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), "0981000026", 1, true, "Cao đẳng" },
                    { 27, "Tài chính doanh nghiệp", "Số 324, Đường Giải Phóng, Quận Hoàng Mai, Hà Nội", "ungvien27@gmail.com", "Nữ", "Lâm Khánh Huyền", "Lập bảng cân đối kế toán, Luân chuyển dòng tiền", 32, new DateTime(1999, 4, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), "0981000027", 3, true, "Đại học" },
                    { 28, "Truyền thông", "Số 336, Đường Giải Phóng, Quận Hoàng Mai, Hà Nội", "ungvien28@gmail.com", "Nam", "Nghiêm Đức Khoa", "Quản lý truyền thông nội bộ, Viết thông cáo báo chí", 33, new DateTime(2000, 5, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), "0981000028", 2, true, "Đại học" },
                    { 29, "Tin học ứng dụng", "Số 348, Đường Giải Phóng, Quận Hoàng Mai, Hà Nội", "ungvien29@gmail.com", "Nữ", "Quách Mỹ Loan", "Cài đặt phần cứng máy tính, Hỗ trợ người dùng Helpdesk", 34, new DateTime(2001, 6, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "0981000029", 1, true, "Cao đẳng" },
                    { 30, "Khoa học máy tính", "Số 360, Đường Giải Phóng, Quận Hoàng Mai, Hà Nội", "ungvien30@gmail.com", "Nam", "Bạch Tiến Dũng", "Kiến trúc hệ thống hướng dịch vụ, Cloud Azure, Docker", 35, new DateTime(2002, 7, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), "0981000030", 4, true, "Đại học" }
                });

            migrationBuilder.InsertData(
                table: "ViTriTuyenDung",
                columns: new[] { "MaViTri", "HanNopHoSo", "MaPhongBan", "MoTaCongViec", "NgayBatDauNhanHoSo", "SoLuongCanTuyen", "SoNamKinhNghiemToiThieu", "TenViTri", "TrangThai", "TrinhDoYeuCau", "YeuCauUngVien" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 11, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "Phát triển backend web API và hệ thống quản trị bằng ASP.NET Core MVC.", new DateTime(2026, 9, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, 2, "Lập trình viên .NET Core", "Đang tuyển", "Đại học", "Có kinh nghiệm C#, Entity Framework Core, SQL Server tối thiểu 2 năm." },
                    { 2, new DateTime(2026, 11, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "Dịch tài liệu kỹ thuật, họp trao đổi yêu cầu với đối tác Nhật Bản.", new DateTime(2026, 9, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 1, "Kỹ sư cầu nối IT Comtor", "Đang tuyển", "Đại học", "Tiếng Nhật N2 trở lên, hiểu biết cơ bản về quy trình phần mềm." },
                    { 3, new DateTime(2026, 11, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, "Tìm kiếm, sàng lọc hồ sơ ứng viên và điều phối phỏng vấn nhân sự.", new DateTime(2026, 9, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 1, "Chuyên viên Tuyển dụng IT", "Đang tuyển", "Đại học", "Tốt nghiệp chuyên ngành Quản trị nhân lực hoặc liên quan, giao tiếp tốt." },
                    { 4, new DateTime(2026, 11, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, "Lập báo cáo tài chính, quyết toán thuế và theo dõi công nợ công ty.", new DateTime(2026, 9, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 3, "Kế toán tổng hợp", "Đang tuyển", "Đại học", "Sử dụng thành thạo phần mềm MISA, Excel nâng cao, chứng chỉ kế toán." },
                    { 5, new DateTime(2026, 11, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 4, "Chạy chiến dịch quảng cáo Facebook Ads, Google Ads và tối ưu SEO.", new DateTime(2026, 9, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 1, "Chuyên viên Digital Marketing", "Đang tuyển", "Cao đẳng", "Kinh nghiệm tối thiểu 1 năm quản lý ngân sách quảng cáo digital." },
                    { 6, new DateTime(2026, 11, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), 5, "Tìm kiếm khách hàng doanh nghiệp, tư vấn giải pháp phần mềm doanh nghiệp.", new DateTime(2026, 9, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), 4, 1, "Nhân viên Kinh doanh B2B", "Đang tuyển", "Đại học", "Đam mê kinh doanh, kỹ năng đàm phán và thuyết trình tốt." },
                    { 7, new DateTime(2026, 12, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "Xây dựng giao diện Single Page App (SPA) với ReactJS và TailwindCSS.", new DateTime(2026, 9, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 2, "Frontend ReactJS Developer", "Đang tuyển", "Đại học", "Thành thạo ReactJS, TypeScript, HTML5/CSS3 và giao tiếp RESTful API." },
                    { 8, new DateTime(2026, 11, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, "Xây dựng giáo trình và kế hoạch đào tạo văn hóa, kỹ năng cho nhân viên.", new DateTime(2026, 9, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, "Chuyên viên Đào tạo nội bộ", "Đang tuyển", "Đại học", "Kỹ năng sư phạm, năng động, nhiệt huyết và có kinh nghiệm đào tạo." },
                    { 9, new DateTime(2026, 12, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, "Kê khai thuế VAT, thuế TNCN, TNDN và chuẩn bị hồ sơ kiểm toán.", new DateTime(2026, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 2, "Kế toán thuế nội bộ", "Đang tuyển", "Cao đẳng", "Nắm vững luật thuế hiện hành, cẩn thận và tỉ mỉ trong xử lý số liệu." },
                    { 10, new DateTime(2026, 12, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 4, "Sáng tạo nội dung bài viết, kịch bản video TikTok và bài đăng mạng xã hội.", new DateTime(2026, 9, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 1, "Content Creator & Copywriter", "Đang tuyển", "Cao đẳng", "Khả năng viết lách tốt, bắt trend nhanh, có tư duy hình ảnh thẩm mỹ." },
                    { 11, new DateTime(2026, 12, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), 5, "Quản lý nhóm kinh doanh, lập kế hoạch doanh số và tiếp cận khách VIP.", new DateTime(2026, 9, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 4, "Trưởng nhóm Kinh doanh Dự án", "Đang tuyển", "Đại học", "Kinh nghiệm quản lý team kinh doanh ít nhất 2 năm, chịu được áp lực cao." },
                    { 12, new DateTime(2026, 12, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "Viết test script tự động hóa với Selenium/Playwright và kiểm thử API.", new DateTime(2026, 9, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 2, "Chuyên viên QA/QC Automation", "Đang tuyển", "Đại học", "Có kinh nghiệm kiểm thử phần mềm, tư duy logic tốt, cẩn thận." },
                    { 13, new DateTime(2026, 8, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "Phát triển vi dịch vụ backend cho hệ thống ngân hàng tài chính.", new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 3, "Lập trình viên Java Spring Boot", "Đã đóng", "Đại học", "Nắm chắc Java Core, Spring Boot, Microservices và cơ sở dữ liệu." },
                    { 14, new DateTime(2026, 8, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, "Tính lương thưởng, bảo hiểm xã hội và phúc lợi cho toàn thể cán bộ.", new DateTime(2026, 7, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 2, "Chuyên viên Tiền lương và Chế độ C&B", "Đã đóng", "Đại học", "Hiểu rõ Luật Lao động, BHXH, sử dụng Excel nâng cao thành thạo." },
                    { 15, new DateTime(2026, 8, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "Hỗ trợ kỹ thuật mạng nội bộ, cài đặt phần mềm máy tính cho công ty.", new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, 0, "Thực tập sinh IT Helpdesk", "Hết hạn nộp", "Cao đẳng", "Nhanh nhẹn, ham học hỏi, yêu thích công nghệ thông tin." }
                });

            migrationBuilder.InsertData(
                table: "HoSoUngTuyen",
                columns: new[] { "MaHoSo", "GhiChu", "MaUngVien", "MaViTri", "NgayNop", "NgayXuLy", "NhanXetSoTuyen", "ThuGioiThieu", "TrangThai" },
                values: new object[,]
                {
                    { 1, "Ứng tuyển qua cổng thông tin tuyển dụng UNETI.", 1, 1, new DateTime(2026, 9, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Tôi rất mong muốn được đóng góp năng lực và gắn bó lâu dài tại quý công ty với vị trí này.", "Chờ duyệt" },
                    { 2, "Ứng tuyển qua cổng thông tin tuyển dụng UNETI.", 2, 2, new DateTime(2026, 9, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Tôi rất mong muốn được đóng góp năng lực và gắn bó lâu dài tại quý công ty với vị trí này.", "Chờ duyệt" },
                    { 3, "Ứng tuyển qua cổng thông tin tuyển dụng UNETI.", 3, 3, new DateTime(2026, 9, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Tôi rất mong muốn được đóng góp năng lực và gắn bó lâu dài tại quý công ty với vị trí này.", "Chờ duyệt" },
                    { 4, "Ứng tuyển qua cổng thông tin tuyển dụng UNETI.", 4, 4, new DateTime(2026, 9, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Tôi rất mong muốn được đóng góp năng lực và gắn bó lâu dài tại quý công ty với vị trí này.", "Chờ duyệt" },
                    { 5, "Ứng tuyển qua cổng thông tin tuyển dụng UNETI.", 5, 5, new DateTime(2026, 9, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Tôi rất mong muốn được đóng góp năng lực và gắn bó lâu dài tại quý công ty với vị trí này.", "Chờ duyệt" },
                    { 6, "Ứng tuyển qua cổng thông tin tuyển dụng UNETI.", 6, 6, new DateTime(2026, 9, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Tôi rất mong muốn được đóng góp năng lực và gắn bó lâu dài tại quý công ty với vị trí này.", "Chờ duyệt" },
                    { 7, "Ứng tuyển qua cổng thông tin tuyển dụng UNETI.", 7, 7, new DateTime(2026, 9, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Tôi rất mong muốn được đóng góp năng lực và gắn bó lâu dài tại quý công ty với vị trí này.", "Chờ duyệt" },
                    { 8, "Ứng tuyển qua cổng thông tin tuyển dụng UNETI.", 8, 8, new DateTime(2026, 9, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Tôi rất mong muốn được đóng góp năng lực và gắn bó lâu dài tại quý công ty với vị trí này.", "Chờ duyệt" },
                    { 9, "Ứng tuyển qua cổng thông tin tuyển dụng UNETI.", 9, 9, new DateTime(2026, 9, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Tôi rất mong muốn được đóng góp năng lực và gắn bó lâu dài tại quý công ty với vị trí này.", "Chờ duyệt" },
                    { 10, "Ứng tuyển qua cổng thông tin tuyển dụng UNETI.", 10, 10, new DateTime(2026, 9, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Tôi rất mong muốn được đóng góp năng lực và gắn bó lâu dài tại quý công ty với vị trí này.", "Chờ duyệt" },
                    { 11, "Ứng tuyển qua cổng thông tin tuyển dụng UNETI.", 11, 11, new DateTime(2026, 9, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 9, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "Hồ sơ đầy đủ, kinh nghiệm và kỹ năng phù hợp yêu cầu vị trí.", "Tôi rất mong muốn được đóng góp năng lực và gắn bó lâu dài tại quý công ty với vị trí này.", "Đạt sơ tuyển" },
                    { 12, "Ứng tuyển qua cổng thông tin tuyển dụng UNETI.", 12, 12, new DateTime(2026, 9, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 9, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "Hồ sơ đầy đủ, kinh nghiệm và kỹ năng phù hợp yêu cầu vị trí.", "Tôi rất mong muốn được đóng góp năng lực và gắn bó lâu dài tại quý công ty với vị trí này.", "Đạt sơ tuyển" },
                    { 13, "Ứng tuyển qua cổng thông tin tuyển dụng UNETI.", 13, 13, new DateTime(2026, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 9, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "Hồ sơ đầy đủ, kinh nghiệm và kỹ năng phù hợp yêu cầu vị trí.", "Tôi rất mong muốn được đóng góp năng lực và gắn bó lâu dài tại quý công ty với vị trí này.", "Đạt sơ tuyển" },
                    { 14, "Ứng tuyển qua cổng thông tin tuyển dụng UNETI.", 14, 14, new DateTime(2026, 9, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 9, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "Hồ sơ đầy đủ, kinh nghiệm và kỹ năng phù hợp yêu cầu vị trí.", "Tôi rất mong muốn được đóng góp năng lực và gắn bó lâu dài tại quý công ty với vị trí này.", "Đạt sơ tuyển" },
                    { 15, "Ứng tuyển qua cổng thông tin tuyển dụng UNETI.", 15, 15, new DateTime(2026, 9, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 9, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "Hồ sơ đầy đủ, kinh nghiệm và kỹ năng phù hợp yêu cầu vị trí.", "Tôi rất mong muốn được đóng góp năng lực và gắn bó lâu dài tại quý công ty với vị trí này.", "Đạt sơ tuyển" },
                    { 16, "Ứng tuyển qua cổng thông tin tuyển dụng UNETI.", 16, 1, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 9, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "Hồ sơ đầy đủ, kinh nghiệm và kỹ năng phù hợp yêu cầu vị trí.", "Tôi rất mong muốn được đóng góp năng lực và gắn bó lâu dài tại quý công ty với vị trí này.", "Đạt sơ tuyển" },
                    { 17, "Ứng tuyển qua cổng thông tin tuyển dụng UNETI.", 17, 2, new DateTime(2026, 9, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 9, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "Hồ sơ đầy đủ, kinh nghiệm và kỹ năng phù hợp yêu cầu vị trí.", "Tôi rất mong muốn được đóng góp năng lực và gắn bó lâu dài tại quý công ty với vị trí này.", "Đạt sơ tuyển" },
                    { 18, "Ứng tuyển qua cổng thông tin tuyển dụng UNETI.", 18, 3, new DateTime(2026, 9, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 9, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "Hồ sơ đầy đủ, kinh nghiệm và kỹ năng phù hợp yêu cầu vị trí.", "Tôi rất mong muốn được đóng góp năng lực và gắn bó lâu dài tại quý công ty với vị trí này.", "Đạt sơ tuyển" },
                    { 19, "Ứng tuyển qua cổng thông tin tuyển dụng UNETI.", 19, 4, new DateTime(2026, 9, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 9, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), "Chưa đáp ứng đủ số năm kinh nghiệm tối thiểu của vị trí tuyển dụng.", "Tôi rất mong muốn được đóng góp năng lực và gắn bó lâu dài tại quý công ty với vị trí này.", "Không đạt sơ tuyển" },
                    { 20, "Ứng tuyển qua cổng thông tin tuyển dụng UNETI.", 20, 5, new DateTime(2026, 9, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 9, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), "Chưa đáp ứng đủ số năm kinh nghiệm tối thiểu của vị trí tuyển dụng.", "Tôi rất mong muốn được đóng góp năng lực và gắn bó lâu dài tại quý công ty với vị trí này.", "Không đạt sơ tuyển" },
                    { 21, "Ứng tuyển qua cổng thông tin tuyển dụng UNETI.", 21, 6, new DateTime(2026, 9, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 9, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), "Chưa đáp ứng đủ số năm kinh nghiệm tối thiểu của vị trí tuyển dụng.", "Tôi rất mong muốn được đóng góp năng lực và gắn bó lâu dài tại quý công ty với vị trí này.", "Không đạt sơ tuyển" },
                    { 22, "Ứng tuyển qua cổng thông tin tuyển dụng UNETI.", 22, 7, new DateTime(2026, 9, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 9, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), "Chưa đáp ứng đủ số năm kinh nghiệm tối thiểu của vị trí tuyển dụng.", "Tôi rất mong muốn được đóng góp năng lực và gắn bó lâu dài tại quý công ty với vị trí này.", "Không đạt sơ tuyển" },
                    { 23, "Ứng tuyển qua cổng thông tin tuyển dụng UNETI.", 23, 8, new DateTime(2026, 9, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 9, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), "Chưa đáp ứng đủ số năm kinh nghiệm tối thiểu của vị trí tuyển dụng.", "Tôi rất mong muốn được đóng góp năng lực và gắn bó lâu dài tại quý công ty với vị trí này.", "Không đạt sơ tuyển" },
                    { 24, "Ứng tuyển qua cổng thông tin tuyển dụng UNETI.", 24, 9, new DateTime(2026, 9, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 9, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), "Chưa đáp ứng đủ số năm kinh nghiệm tối thiểu của vị trí tuyển dụng.", "Tôi rất mong muốn được đóng góp năng lực và gắn bó lâu dài tại quý công ty với vị trí này.", "Không đạt sơ tuyển" },
                    { 25, "Ứng tuyển qua cổng thông tin tuyển dụng UNETI.", 25, 10, new DateTime(2026, 9, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 9, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "Hồ sơ đầy đủ, kinh nghiệm và kỹ năng phù hợp yêu cầu vị trí.", "Tôi rất mong muốn được đóng góp năng lực và gắn bó lâu dài tại quý công ty với vị trí này.", "Chờ phỏng vấn" },
                    { 26, "Ứng tuyển qua cổng thông tin tuyển dụng UNETI.", 26, 11, new DateTime(2026, 9, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 9, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "Hồ sơ đầy đủ, kinh nghiệm và kỹ năng phù hợp yêu cầu vị trí.", "Tôi rất mong muốn được đóng góp năng lực và gắn bó lâu dài tại quý công ty với vị trí này.", "Chờ phỏng vấn" },
                    { 27, "Ứng tuyển qua cổng thông tin tuyển dụng UNETI.", 27, 12, new DateTime(2026, 9, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 9, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "Hồ sơ đầy đủ, kinh nghiệm và kỹ năng phù hợp yêu cầu vị trí.", "Tôi rất mong muốn được đóng góp năng lực và gắn bó lâu dài tại quý công ty với vị trí này.", "Chờ phỏng vấn" },
                    { 28, "Ứng tuyển qua cổng thông tin tuyển dụng UNETI.", 28, 13, new DateTime(2026, 9, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 9, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "Hồ sơ đầy đủ, kinh nghiệm và kỹ năng phù hợp yêu cầu vị trí.", "Tôi rất mong muốn được đóng góp năng lực và gắn bó lâu dài tại quý công ty với vị trí này.", "Chờ phỏng vấn" },
                    { 29, "Ứng tuyển qua cổng thông tin tuyển dụng UNETI.", 29, 14, new DateTime(2026, 9, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 9, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "Hồ sơ đầy đủ, kinh nghiệm và kỹ năng phù hợp yêu cầu vị trí.", "Tôi rất mong muốn được đóng góp năng lực và gắn bó lâu dài tại quý công ty với vị trí này.", "Chờ phỏng vấn" },
                    { 30, "Ứng tuyển qua cổng thông tin tuyển dụng UNETI.", 30, 15, new DateTime(2026, 9, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 9, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "Hồ sơ đầy đủ, kinh nghiệm và kỹ năng phù hợp yêu cầu vị trí.", "Tôi rất mong muốn được đóng góp năng lực và gắn bó lâu dài tại quý công ty với vị trí này.", "Chờ phỏng vấn" },
                    { 31, "Ứng tuyển qua cổng thông tin tuyển dụng UNETI.", 1, 1, new DateTime(2026, 9, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 9, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "Hồ sơ đầy đủ, kinh nghiệm và kỹ năng phù hợp yêu cầu vị trí.", "Tôi rất mong muốn được đóng góp năng lực và gắn bó lâu dài tại quý công ty với vị trí này.", "Chờ phỏng vấn" },
                    { 32, "Ứng tuyển qua cổng thông tin tuyển dụng UNETI.", 2, 2, new DateTime(2026, 9, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 9, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "Hồ sơ đầy đủ, kinh nghiệm và kỹ năng phù hợp yêu cầu vị trí.", "Tôi rất mong muốn được đóng góp năng lực và gắn bó lâu dài tại quý công ty với vị trí này.", "Đã phỏng vấn" },
                    { 33, "Ứng tuyển qua cổng thông tin tuyển dụng UNETI.", 3, 3, new DateTime(2026, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 9, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "Hồ sơ đầy đủ, kinh nghiệm và kỹ năng phù hợp yêu cầu vị trí.", "Tôi rất mong muốn được đóng góp năng lực và gắn bó lâu dài tại quý công ty với vị trí này.", "Đã phỏng vấn" },
                    { 34, "Ứng tuyển qua cổng thông tin tuyển dụng UNETI.", 4, 4, new DateTime(2026, 9, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 9, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "Hồ sơ đầy đủ, kinh nghiệm và kỹ năng phù hợp yêu cầu vị trí.", "Tôi rất mong muốn được đóng góp năng lực và gắn bó lâu dài tại quý công ty với vị trí này.", "Đã phỏng vấn" },
                    { 35, "Ứng tuyển qua cổng thông tin tuyển dụng UNETI.", 5, 5, new DateTime(2026, 9, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 9, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "Hồ sơ đầy đủ, kinh nghiệm và kỹ năng phù hợp yêu cầu vị trí.", "Tôi rất mong muốn được đóng góp năng lực và gắn bó lâu dài tại quý công ty với vị trí này.", "Đã phỏng vấn" },
                    { 36, "Ứng tuyển qua cổng thông tin tuyển dụng UNETI.", 6, 6, new DateTime(2026, 9, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 9, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "Hồ sơ đầy đủ, kinh nghiệm và kỹ năng phù hợp yêu cầu vị trí.", "Tôi rất mong muốn được đóng góp năng lực và gắn bó lâu dài tại quý công ty với vị trí này.", "Đã phỏng vấn" },
                    { 37, "Ứng tuyển qua cổng thông tin tuyển dụng UNETI.", 7, 7, new DateTime(2026, 9, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 9, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "Hồ sơ đầy đủ, kinh nghiệm và kỹ năng phù hợp yêu cầu vị trí.", "Tôi rất mong muốn được đóng góp năng lực và gắn bó lâu dài tại quý công ty với vị trí này.", "Đã phỏng vấn" },
                    { 38, "Ứng tuyển qua cổng thông tin tuyển dụng UNETI.", 8, 8, new DateTime(2026, 9, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 9, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "Hồ sơ đầy đủ, kinh nghiệm và kỹ năng phù hợp yêu cầu vị trí.", "Tôi rất mong muốn được đóng góp năng lực và gắn bó lâu dài tại quý công ty với vị trí này.", "Trúng tuyển" },
                    { 39, "Ứng tuyển qua cổng thông tin tuyển dụng UNETI.", 9, 9, new DateTime(2026, 9, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 9, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "Hồ sơ đầy đủ, kinh nghiệm và kỹ năng phù hợp yêu cầu vị trí.", "Tôi rất mong muốn được đóng góp năng lực và gắn bó lâu dài tại quý công ty với vị trí này.", "Trúng tuyển" },
                    { 40, "Ứng tuyển qua cổng thông tin tuyển dụng UNETI.", 10, 10, new DateTime(2026, 9, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 9, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "Hồ sơ đầy đủ, kinh nghiệm và kỹ năng phù hợp yêu cầu vị trí.", "Tôi rất mong muốn được đóng góp năng lực và gắn bó lâu dài tại quý công ty với vị trí này.", "Trúng tuyển" },
                    { 41, "Ứng tuyển qua cổng thông tin tuyển dụng UNETI.", 11, 11, new DateTime(2026, 9, 4, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 9, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "Hồ sơ đầy đủ, kinh nghiệm và kỹ năng phù hợp yêu cầu vị trí.", "Tôi rất mong muốn được đóng góp năng lực và gắn bó lâu dài tại quý công ty với vị trí này.", "Trúng tuyển" },
                    { 42, "Ứng tuyển qua cổng thông tin tuyển dụng UNETI.", 12, 12, new DateTime(2026, 9, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 9, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "Hồ sơ đầy đủ, kinh nghiệm và kỹ năng phù hợp yêu cầu vị trí.", "Tôi rất mong muốn được đóng góp năng lực và gắn bó lâu dài tại quý công ty với vị trí này.", "Trúng tuyển" },
                    { 43, "Ứng tuyển qua cổng thông tin tuyển dụng UNETI.", 13, 13, new DateTime(2026, 9, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 9, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "Hồ sơ đầy đủ, kinh nghiệm và kỹ năng phù hợp yêu cầu vị trí.", "Tôi rất mong muốn được đóng góp năng lực và gắn bó lâu dài tại quý công ty với vị trí này.", "Không trúng tuyển" },
                    { 44, "Ứng tuyển qua cổng thông tin tuyển dụng UNETI.", 14, 14, new DateTime(2026, 9, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 9, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "Hồ sơ đầy đủ, kinh nghiệm và kỹ năng phù hợp yêu cầu vị trí.", "Tôi rất mong muốn được đóng góp năng lực và gắn bó lâu dài tại quý công ty với vị trí này.", "Không trúng tuyển" },
                    { 45, "Ứng tuyển qua cổng thông tin tuyển dụng UNETI.", 15, 15, new DateTime(2026, 9, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 9, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "Hồ sơ đầy đủ, kinh nghiệm và kỹ năng phù hợp yêu cầu vị trí.", "Tôi rất mong muốn được đóng góp năng lực và gắn bó lâu dài tại quý công ty với vị trí này.", "Không trúng tuyển" },
                    { 46, "Ứng tuyển qua cổng thông tin tuyển dụng UNETI.", 16, 1, new DateTime(2026, 9, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 9, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "Hồ sơ đầy đủ, kinh nghiệm và kỹ năng phù hợp yêu cầu vị trí.", "Tôi rất mong muốn được đóng góp năng lực và gắn bó lâu dài tại quý công ty với vị trí này.", "Không trúng tuyển" },
                    { 47, "Ứng tuyển qua cổng thông tin tuyển dụng UNETI.", 17, 2, new DateTime(2026, 9, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 9, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "Ứng viên chủ động rút/hủy hồ sơ ứng tuyển.", "Tôi rất mong muốn được đóng góp năng lực và gắn bó lâu dài tại quý công ty với vị trí này.", "Đã hủy" },
                    { 48, "Ứng tuyển qua cổng thông tin tuyển dụng UNETI.", 18, 3, new DateTime(2026, 9, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 9, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "Ứng viên chủ động rút/hủy hồ sơ ứng tuyển.", "Tôi rất mong muốn được đóng góp năng lực và gắn bó lâu dài tại quý công ty với vị trí này.", "Đã hủy" },
                    { 49, "Ứng tuyển qua cổng thông tin tuyển dụng UNETI.", 19, 4, new DateTime(2026, 9, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 9, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "Ứng viên chủ động rút/hủy hồ sơ ứng tuyển.", "Tôi rất mong muốn được đóng góp năng lực và gắn bó lâu dài tại quý công ty với vị trí này.", "Đã hủy" },
                    { 50, "Ứng tuyển qua cổng thông tin tuyển dụng UNETI.", 20, 5, new DateTime(2026, 9, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 9, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "Ứng viên chủ động rút/hủy hồ sơ ứng tuyển.", "Tôi rất mong muốn được đóng góp năng lực và gắn bó lâu dài tại quý công ty với vị trí này.", "Đã hủy" }
                });

            migrationBuilder.InsertData(
                table: "KetQuaTuyenDung",
                columns: new[] { "MaKetQua", "DiemDanhGia", "KetQua", "MaHoSo", "NgayCapNhat", "NhanXet" },
                values: new object[,]
                {
                    { 1, 88.0, "Trúng tuyển", 38, new DateTime(2026, 9, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), "Ứng viên trả lời xuất sắc câu hỏi chuyên môn, tác phong chuyên nghiệp, phù hợp với vị trí." },
                    { 2, 89.0, "Trúng tuyển", 39, new DateTime(2026, 9, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), "Ứng viên trả lời xuất sắc câu hỏi chuyên môn, tác phong chuyên nghiệp, phù hợp với vị trí." },
                    { 3, 90.0, "Trúng tuyển", 40, new DateTime(2026, 9, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), "Ứng viên trả lời xuất sắc câu hỏi chuyên môn, tác phong chuyên nghiệp, phù hợp với vị trí." },
                    { 4, 91.0, "Trúng tuyển", 41, new DateTime(2026, 9, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), "Ứng viên trả lời xuất sắc câu hỏi chuyên môn, tác phong chuyên nghiệp, phù hợp với vị trí." },
                    { 5, 92.0, "Trúng tuyển", 42, new DateTime(2026, 9, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), "Ứng viên trả lời xuất sắc câu hỏi chuyên môn, tác phong chuyên nghiệp, phù hợp với vị trí." },
                    { 6, 58.0, "Không trúng tuyển", 43, new DateTime(2026, 9, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), "Kỹ năng giao tiếp và xử lý tình huống chưa đạt yêu cầu của vị trí." },
                    { 7, 59.0, "Không trúng tuyển", 44, new DateTime(2026, 9, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), "Kỹ năng giao tiếp và xử lý tình huống chưa đạt yêu cầu của vị trí." },
                    { 8, 60.0, "Không trúng tuyển", 45, new DateTime(2026, 9, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), "Kỹ năng giao tiếp và xử lý tình huống chưa đạt yêu cầu của vị trí." },
                    { 9, 61.0, "Không trúng tuyển", 46, new DateTime(2026, 9, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), "Kỹ năng giao tiếp và xử lý tình huống chưa đạt yêu cầu của vị trí." }
                });

            migrationBuilder.InsertData(
                table: "LichPhongVan",
                columns: new[] { "MaLichPhongVan", "DiaDiemHoacLienKet", "GhiChu", "HinhThucPhongVan", "MaHoSo", "NguoiPhongVan", "ThoiGianBatDau", "ThoiGianKetThuc", "TrangThai" },
                values: new object[,]
                {
                    { 1, "Phòng họp 302, Tòa nhà UNETI", "Vòng 1 - Phỏng vấn chuyên môn và trao đổi văn hóa công ty.", "Trực tiếp", 25, "Trần Văn Minh (Trưởng nhóm Phỏng vấn)", new DateTime(2026, 10, 11, 9, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 11, 10, 0, 0, 0, DateTimeKind.Unspecified), "Đã lên lịch" },
                    { 2, "https://meet.google.com/une-ti05-rec", "Vòng 1 - Phỏng vấn chuyên môn và trao đổi văn hóa công ty.", "Trực tuyến Google Meet", 26, "Nguyễn Thị Thu Hà (HR Manager)", new DateTime(2026, 10, 12, 9, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 12, 10, 0, 0, 0, DateTimeKind.Unspecified), "Đã lên lịch" },
                    { 3, "Phòng họp 302, Tòa nhà UNETI", "Vòng 1 - Phỏng vấn chuyên môn và trao đổi văn hóa công ty.", "Trực tiếp", 27, "Lê Mai Chi (Chuyên viên Tuyển dụng)", new DateTime(2026, 10, 13, 9, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 13, 10, 0, 0, 0, DateTimeKind.Unspecified), "Đã lên lịch" },
                    { 4, "https://meet.google.com/une-ti05-rec", "Vòng 1 - Phỏng vấn chuyên môn và trao đổi văn hóa công ty.", "Trực tuyến MS Teams", 28, "Đỗ Thiện An (Tech Lead)", new DateTime(2026, 10, 14, 9, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 14, 10, 0, 0, 0, DateTimeKind.Unspecified), "Đã lên lịch" },
                    { 5, "Phòng họp 302, Tòa nhà UNETI", "Vòng 1 - Phỏng vấn chuyên môn và trao đổi văn hóa công ty.", "Trực tiếp", 29, "Trần Văn Minh (Trưởng nhóm Phỏng vấn)", new DateTime(2026, 10, 15, 9, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 15, 10, 0, 0, 0, DateTimeKind.Unspecified), "Đã lên lịch" },
                    { 6, "https://meet.google.com/une-ti05-rec", "Vòng 1 - Phỏng vấn chuyên môn và trao đổi văn hóa công ty.", "Trực tuyến Google Meet", 30, "Nguyễn Thị Thu Hà (HR Manager)", new DateTime(2026, 10, 16, 9, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 16, 10, 0, 0, 0, DateTimeKind.Unspecified), "Đã lên lịch" },
                    { 7, "Phòng họp 302, Tòa nhà UNETI", "Vòng 1 - Phỏng vấn chuyên môn và trao đổi văn hóa công ty.", "Trực tiếp", 31, "Lê Mai Chi (Chuyên viên Tuyển dụng)", new DateTime(2026, 10, 17, 9, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 17, 10, 0, 0, 0, DateTimeKind.Unspecified), "Đã lên lịch" },
                    { 8, "https://meet.google.com/une-ti05-rec", "Vòng 1 - Phỏng vấn chuyên môn và trao đổi văn hóa công ty.", "Trực tuyến MS Teams", 32, "Đỗ Thiện An (Tech Lead)", new DateTime(2026, 10, 18, 9, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 10, 18, 10, 0, 0, 0, DateTimeKind.Unspecified), "Đã lên lịch" },
                    { 9, "Phòng họp 302, Tòa nhà UNETI", "Vòng 1 - Phỏng vấn chuyên môn và trao đổi văn hóa công ty.", "Trực tiếp", 33, "Trần Văn Minh (Trưởng nhóm Phỏng vấn)", new DateTime(2026, 9, 27, 14, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 9, 27, 15, 0, 0, 0, DateTimeKind.Unspecified), "Đã hoàn thành" },
                    { 10, "https://meet.google.com/une-ti05-rec", "Vòng 1 - Phỏng vấn chuyên môn và trao đổi văn hóa công ty.", "Trực tuyến Google Meet", 34, "Nguyễn Thị Thu Hà (HR Manager)", new DateTime(2026, 9, 28, 14, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 9, 28, 15, 0, 0, 0, DateTimeKind.Unspecified), "Đã hoàn thành" },
                    { 11, "Phòng họp 302, Tòa nhà UNETI", "Vòng 1 - Phỏng vấn chuyên môn và trao đổi văn hóa công ty.", "Trực tiếp", 35, "Lê Mai Chi (Chuyên viên Tuyển dụng)", new DateTime(2026, 9, 29, 14, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 9, 29, 15, 0, 0, 0, DateTimeKind.Unspecified), "Đã hoàn thành" },
                    { 12, "https://meet.google.com/une-ti05-rec", "Vòng 1 - Phỏng vấn chuyên môn và trao đổi văn hóa công ty.", "Trực tuyến MS Teams", 36, "Đỗ Thiện An (Tech Lead)", new DateTime(2026, 9, 26, 14, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 9, 26, 15, 0, 0, 0, DateTimeKind.Unspecified), "Đã hoàn thành" },
                    { 13, "Phòng họp 302, Tòa nhà UNETI", "Vòng 1 - Phỏng vấn chuyên môn và trao đổi văn hóa công ty.", "Trực tiếp", 37, "Trần Văn Minh (Trưởng nhóm Phỏng vấn)", new DateTime(2026, 9, 27, 14, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 9, 27, 15, 0, 0, 0, DateTimeKind.Unspecified), "Đã hoàn thành" },
                    { 14, "https://meet.google.com/une-ti05-rec", "Vòng 1 - Phỏng vấn chuyên môn và trao đổi văn hóa công ty.", "Trực tuyến Google Meet", 38, "Nguyễn Thị Thu Hà (HR Manager)", new DateTime(2026, 9, 28, 14, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 9, 28, 15, 0, 0, 0, DateTimeKind.Unspecified), "Đã hoàn thành" },
                    { 15, "Phòng họp 302, Tòa nhà UNETI", "Vòng 1 - Phỏng vấn chuyên môn và trao đổi văn hóa công ty.", "Trực tiếp", 39, "Lê Mai Chi (Chuyên viên Tuyển dụng)", new DateTime(2026, 9, 29, 14, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 9, 29, 15, 0, 0, 0, DateTimeKind.Unspecified), "Đã hoàn thành" },
                    { 16, "https://meet.google.com/une-ti05-rec", "Vòng 1 - Phỏng vấn chuyên môn và trao đổi văn hóa công ty.", "Trực tuyến MS Teams", 40, "Đỗ Thiện An (Tech Lead)", new DateTime(2026, 9, 26, 14, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 9, 26, 15, 0, 0, 0, DateTimeKind.Unspecified), "Đã hoàn thành" },
                    { 17, "Phòng họp 302, Tòa nhà UNETI", "Vòng 1 - Phỏng vấn chuyên môn và trao đổi văn hóa công ty.", "Trực tiếp", 41, "Trần Văn Minh (Trưởng nhóm Phỏng vấn)", new DateTime(2026, 9, 27, 14, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 9, 27, 15, 0, 0, 0, DateTimeKind.Unspecified), "Đã hoàn thành" },
                    { 18, "https://meet.google.com/une-ti05-rec", "Vòng 1 - Phỏng vấn chuyên môn và trao đổi văn hóa công ty.", "Trực tuyến Google Meet", 42, "Nguyễn Thị Thu Hà (HR Manager)", new DateTime(2026, 9, 28, 10, 30, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 9, 28, 11, 30, 0, 0, DateTimeKind.Unspecified), "Đã hủy" },
                    { 19, "Phòng họp 302, Tòa nhà UNETI", "Vòng 1 - Phỏng vấn chuyên môn và trao đổi văn hóa công ty.", "Trực tiếp", 43, "Lê Mai Chi (Chuyên viên Tuyển dụng)", new DateTime(2026, 9, 28, 10, 30, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 9, 28, 11, 30, 0, 0, DateTimeKind.Unspecified), "Đã hủy" },
                    { 20, "https://meet.google.com/une-ti05-rec", "Vòng 1 - Phỏng vấn chuyên môn và trao đổi văn hóa công ty.", "Trực tuyến MS Teams", 44, "Đỗ Thiện An (Tech Lead)", new DateTime(2026, 9, 28, 10, 30, 0, 0, DateTimeKind.Unspecified), new DateTime(2026, 9, 28, 11, 30, 0, 0, DateTimeKind.Unspecified), "Đã hủy" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_HoSoUngTuyen_MaUngVien",
                table: "HoSoUngTuyen",
                column: "MaUngVien");

            migrationBuilder.CreateIndex(
                name: "IX_HoSoUngTuyen_MaViTri",
                table: "HoSoUngTuyen",
                column: "MaViTri");

            migrationBuilder.CreateIndex(
                name: "IX_KetQuaTuyenDung_MaHoSo",
                table: "KetQuaTuyenDung",
                column: "MaHoSo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LichPhongVan_MaHoSo",
                table: "LichPhongVan",
                column: "MaHoSo");

            migrationBuilder.CreateIndex(
                name: "IX_PhongBan_TenPhongBan",
                table: "PhongBan",
                column: "TenPhongBan",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TaiKhoan_TenDangNhap",
                table: "TaiKhoan",
                column: "TenDangNhap",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UngVien_MaTaiKhoan",
                table: "UngVien",
                column: "MaTaiKhoan",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ViTriTuyenDung_MaPhongBan",
                table: "ViTriTuyenDung",
                column: "MaPhongBan");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "KetQuaTuyenDung");

            migrationBuilder.DropTable(
                name: "LichPhongVan");

            migrationBuilder.DropTable(
                name: "HoSoUngTuyen");

            migrationBuilder.DropTable(
                name: "UngVien");

            migrationBuilder.DropTable(
                name: "ViTriTuyenDung");

            migrationBuilder.DropTable(
                name: "TaiKhoan");

            migrationBuilder.DropTable(
                name: "PhongBan");
        }
    }
}
