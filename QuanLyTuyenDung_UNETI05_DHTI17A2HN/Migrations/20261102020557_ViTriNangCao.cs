using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyTuyenDung_UNETI05_DHTI17A2HN.Migrations
{
    /// <inheritdoc />
    public partial class ViTriNangCao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LyDoDong",
                table: "ViTriTuyenDung",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "LichSuViTri",
                columns: table => new
                {
                    MaLichSu = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaViTri = table.Column<int>(type: "int", nullable: true),
                    TenViTri = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    MaTaiKhoan = table.Column<int>(type: "int", nullable: true),
                    NguoiThucHien = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    HanhDong = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NoiDung = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ThoiGian = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LichSuViTri", x => x.MaLichSu);
                    table.ForeignKey(
                        name: "FK_LichSuViTri_ViTriTuyenDung_MaViTri",
                        column: x => x.MaViTri,
                        principalTable: "ViTriTuyenDung",
                        principalColumn: "MaViTri",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "ViecLamYeuThich",
                columns: table => new
                {
                    MaTaiKhoan = table.Column<int>(type: "int", nullable: false),
                    MaViTri = table.Column<int>(type: "int", nullable: false),
                    NgayLuu = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ViecLamYeuThich", x => new { x.MaTaiKhoan, x.MaViTri });
                    table.ForeignKey(
                        name: "FK_ViecLamYeuThich_TaiKhoan_MaTaiKhoan",
                        column: x => x.MaTaiKhoan,
                        principalTable: "TaiKhoan",
                        principalColumn: "MaTaiKhoan",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ViecLamYeuThich_ViTriTuyenDung_MaViTri",
                        column: x => x.MaViTri,
                        principalTable: "ViTriTuyenDung",
                        principalColumn: "MaViTri",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "ViTriTuyenDung",
                keyColumn: "MaViTri",
                keyValue: 1,
                column: "LyDoDong",
                value: null);

            migrationBuilder.UpdateData(
                table: "ViTriTuyenDung",
                keyColumn: "MaViTri",
                keyValue: 2,
                column: "LyDoDong",
                value: null);

            migrationBuilder.UpdateData(
                table: "ViTriTuyenDung",
                keyColumn: "MaViTri",
                keyValue: 3,
                column: "LyDoDong",
                value: null);

            migrationBuilder.UpdateData(
                table: "ViTriTuyenDung",
                keyColumn: "MaViTri",
                keyValue: 4,
                column: "LyDoDong",
                value: null);

            migrationBuilder.UpdateData(
                table: "ViTriTuyenDung",
                keyColumn: "MaViTri",
                keyValue: 5,
                column: "LyDoDong",
                value: null);

            migrationBuilder.UpdateData(
                table: "ViTriTuyenDung",
                keyColumn: "MaViTri",
                keyValue: 6,
                column: "LyDoDong",
                value: null);

            migrationBuilder.UpdateData(
                table: "ViTriTuyenDung",
                keyColumn: "MaViTri",
                keyValue: 7,
                column: "LyDoDong",
                value: null);

            migrationBuilder.UpdateData(
                table: "ViTriTuyenDung",
                keyColumn: "MaViTri",
                keyValue: 8,
                column: "LyDoDong",
                value: null);

            migrationBuilder.UpdateData(
                table: "ViTriTuyenDung",
                keyColumn: "MaViTri",
                keyValue: 9,
                column: "LyDoDong",
                value: null);

            migrationBuilder.UpdateData(
                table: "ViTriTuyenDung",
                keyColumn: "MaViTri",
                keyValue: 10,
                column: "LyDoDong",
                value: null);

            migrationBuilder.UpdateData(
                table: "ViTriTuyenDung",
                keyColumn: "MaViTri",
                keyValue: 11,
                column: "LyDoDong",
                value: null);

            migrationBuilder.UpdateData(
                table: "ViTriTuyenDung",
                keyColumn: "MaViTri",
                keyValue: 12,
                column: "LyDoDong",
                value: null);

            migrationBuilder.UpdateData(
                table: "ViTriTuyenDung",
                keyColumn: "MaViTri",
                keyValue: 13,
                column: "LyDoDong",
                value: null);

            migrationBuilder.UpdateData(
                table: "ViTriTuyenDung",
                keyColumn: "MaViTri",
                keyValue: 14,
                column: "LyDoDong",
                value: null);

            migrationBuilder.UpdateData(
                table: "ViTriTuyenDung",
                keyColumn: "MaViTri",
                keyValue: 15,
                column: "LyDoDong",
                value: null);

            migrationBuilder.Sql(@"UPDATE ViTriTuyenDung SET LyDoDong = N'Đã đóng trước khi bật chức năng lưu lý do' WHERE TrangThai = N'Đã đóng' AND LyDoDong IS NULL;
UPDATE ViTriTuyenDung SET TrangThai = N'Đã đóng', LyDoDong = N'Đã hết hạn nộp hồ sơ' WHERE TrangThai = N'Hết hạn nộp';");

            migrationBuilder.CreateIndex(
                name: "IX_LichSuViTri_MaViTri_ThoiGian",
                table: "LichSuViTri",
                columns: new[] { "MaViTri", "ThoiGian" });

            migrationBuilder.CreateIndex(
                name: "IX_ViecLamYeuThich_MaViTri",
                table: "ViecLamYeuThich",
                column: "MaViTri");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LichSuViTri");

            migrationBuilder.DropTable(
                name: "ViecLamYeuThich");

            migrationBuilder.DropColumn(
                name: "LyDoDong",
                table: "ViTriTuyenDung");
        }
    }
}
