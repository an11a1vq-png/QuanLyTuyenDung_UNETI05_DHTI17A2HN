using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyTuyenDung_UNETI05_DHTI17A2HN.Migrations
{
    /// <inheritdoc />
    public partial class RemoveSecurityColumnsFromTaiKhoanModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "KhoaDenThoiDiem",
                table: "TaiKhoan");

            migrationBuilder.DropColumn(
                name: "MaXacThucKhoiPhuc",
                table: "TaiKhoan");

            migrationBuilder.DropColumn(
                name: "SoLanDangNhapSai",
                table: "TaiKhoan");

            migrationBuilder.DropColumn(
                name: "ThoiHanMaXacThuc",
                table: "TaiKhoan");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "KhoaDenThoiDiem",
                table: "TaiKhoan",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MaXacThucKhoiPhuc",
                table: "TaiKhoan",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SoLanDangNhapSai",
                table: "TaiKhoan",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "ThoiHanMaXacThuc",
                table: "TaiKhoan",
                type: "datetime2",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "TaiKhoan",
                keyColumn: "MaTaiKhoan",
                keyValue: 1,
                columns: new[] { "KhoaDenThoiDiem", "MaXacThucKhoiPhuc", "SoLanDangNhapSai", "ThoiHanMaXacThuc" },
                values: new object[] { null, null, 0, null });

            migrationBuilder.UpdateData(
                table: "TaiKhoan",
                keyColumn: "MaTaiKhoan",
                keyValue: 2,
                columns: new[] { "KhoaDenThoiDiem", "MaXacThucKhoiPhuc", "SoLanDangNhapSai", "ThoiHanMaXacThuc" },
                values: new object[] { null, null, 0, null });

            migrationBuilder.UpdateData(
                table: "TaiKhoan",
                keyColumn: "MaTaiKhoan",
                keyValue: 3,
                columns: new[] { "KhoaDenThoiDiem", "MaXacThucKhoiPhuc", "SoLanDangNhapSai", "ThoiHanMaXacThuc" },
                values: new object[] { null, null, 0, null });

            migrationBuilder.UpdateData(
                table: "TaiKhoan",
                keyColumn: "MaTaiKhoan",
                keyValue: 4,
                columns: new[] { "KhoaDenThoiDiem", "MaXacThucKhoiPhuc", "SoLanDangNhapSai", "ThoiHanMaXacThuc" },
                values: new object[] { null, null, 0, null });

            migrationBuilder.UpdateData(
                table: "TaiKhoan",
                keyColumn: "MaTaiKhoan",
                keyValue: 5,
                columns: new[] { "KhoaDenThoiDiem", "MaXacThucKhoiPhuc", "SoLanDangNhapSai", "ThoiHanMaXacThuc" },
                values: new object[] { null, null, 0, null });

            migrationBuilder.UpdateData(
                table: "TaiKhoan",
                keyColumn: "MaTaiKhoan",
                keyValue: 6,
                columns: new[] { "KhoaDenThoiDiem", "MaXacThucKhoiPhuc", "SoLanDangNhapSai", "ThoiHanMaXacThuc" },
                values: new object[] { null, null, 0, null });

            migrationBuilder.UpdateData(
                table: "TaiKhoan",
                keyColumn: "MaTaiKhoan",
                keyValue: 7,
                columns: new[] { "KhoaDenThoiDiem", "MaXacThucKhoiPhuc", "SoLanDangNhapSai", "ThoiHanMaXacThuc" },
                values: new object[] { null, null, 0, null });

            migrationBuilder.UpdateData(
                table: "TaiKhoan",
                keyColumn: "MaTaiKhoan",
                keyValue: 8,
                columns: new[] { "KhoaDenThoiDiem", "MaXacThucKhoiPhuc", "SoLanDangNhapSai", "ThoiHanMaXacThuc" },
                values: new object[] { null, null, 0, null });

            migrationBuilder.UpdateData(
                table: "TaiKhoan",
                keyColumn: "MaTaiKhoan",
                keyValue: 9,
                columns: new[] { "KhoaDenThoiDiem", "MaXacThucKhoiPhuc", "SoLanDangNhapSai", "ThoiHanMaXacThuc" },
                values: new object[] { null, null, 0, null });

            migrationBuilder.UpdateData(
                table: "TaiKhoan",
                keyColumn: "MaTaiKhoan",
                keyValue: 10,
                columns: new[] { "KhoaDenThoiDiem", "MaXacThucKhoiPhuc", "SoLanDangNhapSai", "ThoiHanMaXacThuc" },
                values: new object[] { null, null, 0, null });

            migrationBuilder.UpdateData(
                table: "TaiKhoan",
                keyColumn: "MaTaiKhoan",
                keyValue: 11,
                columns: new[] { "KhoaDenThoiDiem", "MaXacThucKhoiPhuc", "SoLanDangNhapSai", "ThoiHanMaXacThuc" },
                values: new object[] { null, null, 0, null });

            migrationBuilder.UpdateData(
                table: "TaiKhoan",
                keyColumn: "MaTaiKhoan",
                keyValue: 12,
                columns: new[] { "KhoaDenThoiDiem", "MaXacThucKhoiPhuc", "SoLanDangNhapSai", "ThoiHanMaXacThuc" },
                values: new object[] { null, null, 0, null });

            migrationBuilder.UpdateData(
                table: "TaiKhoan",
                keyColumn: "MaTaiKhoan",
                keyValue: 13,
                columns: new[] { "KhoaDenThoiDiem", "MaXacThucKhoiPhuc", "SoLanDangNhapSai", "ThoiHanMaXacThuc" },
                values: new object[] { null, null, 0, null });

            migrationBuilder.UpdateData(
                table: "TaiKhoan",
                keyColumn: "MaTaiKhoan",
                keyValue: 14,
                columns: new[] { "KhoaDenThoiDiem", "MaXacThucKhoiPhuc", "SoLanDangNhapSai", "ThoiHanMaXacThuc" },
                values: new object[] { null, null, 0, null });

            migrationBuilder.UpdateData(
                table: "TaiKhoan",
                keyColumn: "MaTaiKhoan",
                keyValue: 15,
                columns: new[] { "KhoaDenThoiDiem", "MaXacThucKhoiPhuc", "SoLanDangNhapSai", "ThoiHanMaXacThuc" },
                values: new object[] { null, null, 0, null });

            migrationBuilder.UpdateData(
                table: "TaiKhoan",
                keyColumn: "MaTaiKhoan",
                keyValue: 16,
                columns: new[] { "KhoaDenThoiDiem", "MaXacThucKhoiPhuc", "SoLanDangNhapSai", "ThoiHanMaXacThuc" },
                values: new object[] { null, null, 0, null });

            migrationBuilder.UpdateData(
                table: "TaiKhoan",
                keyColumn: "MaTaiKhoan",
                keyValue: 17,
                columns: new[] { "KhoaDenThoiDiem", "MaXacThucKhoiPhuc", "SoLanDangNhapSai", "ThoiHanMaXacThuc" },
                values: new object[] { null, null, 0, null });

            migrationBuilder.UpdateData(
                table: "TaiKhoan",
                keyColumn: "MaTaiKhoan",
                keyValue: 18,
                columns: new[] { "KhoaDenThoiDiem", "MaXacThucKhoiPhuc", "SoLanDangNhapSai", "ThoiHanMaXacThuc" },
                values: new object[] { null, null, 0, null });

            migrationBuilder.UpdateData(
                table: "TaiKhoan",
                keyColumn: "MaTaiKhoan",
                keyValue: 19,
                columns: new[] { "KhoaDenThoiDiem", "MaXacThucKhoiPhuc", "SoLanDangNhapSai", "ThoiHanMaXacThuc" },
                values: new object[] { null, null, 0, null });

            migrationBuilder.UpdateData(
                table: "TaiKhoan",
                keyColumn: "MaTaiKhoan",
                keyValue: 20,
                columns: new[] { "KhoaDenThoiDiem", "MaXacThucKhoiPhuc", "SoLanDangNhapSai", "ThoiHanMaXacThuc" },
                values: new object[] { null, null, 0, null });

            migrationBuilder.UpdateData(
                table: "TaiKhoan",
                keyColumn: "MaTaiKhoan",
                keyValue: 21,
                columns: new[] { "KhoaDenThoiDiem", "MaXacThucKhoiPhuc", "SoLanDangNhapSai", "ThoiHanMaXacThuc" },
                values: new object[] { null, null, 0, null });

            migrationBuilder.UpdateData(
                table: "TaiKhoan",
                keyColumn: "MaTaiKhoan",
                keyValue: 22,
                columns: new[] { "KhoaDenThoiDiem", "MaXacThucKhoiPhuc", "SoLanDangNhapSai", "ThoiHanMaXacThuc" },
                values: new object[] { null, null, 0, null });

            migrationBuilder.UpdateData(
                table: "TaiKhoan",
                keyColumn: "MaTaiKhoan",
                keyValue: 23,
                columns: new[] { "KhoaDenThoiDiem", "MaXacThucKhoiPhuc", "SoLanDangNhapSai", "ThoiHanMaXacThuc" },
                values: new object[] { null, null, 0, null });

            migrationBuilder.UpdateData(
                table: "TaiKhoan",
                keyColumn: "MaTaiKhoan",
                keyValue: 24,
                columns: new[] { "KhoaDenThoiDiem", "MaXacThucKhoiPhuc", "SoLanDangNhapSai", "ThoiHanMaXacThuc" },
                values: new object[] { null, null, 0, null });

            migrationBuilder.UpdateData(
                table: "TaiKhoan",
                keyColumn: "MaTaiKhoan",
                keyValue: 25,
                columns: new[] { "KhoaDenThoiDiem", "MaXacThucKhoiPhuc", "SoLanDangNhapSai", "ThoiHanMaXacThuc" },
                values: new object[] { null, null, 0, null });

            migrationBuilder.UpdateData(
                table: "TaiKhoan",
                keyColumn: "MaTaiKhoan",
                keyValue: 26,
                columns: new[] { "KhoaDenThoiDiem", "MaXacThucKhoiPhuc", "SoLanDangNhapSai", "ThoiHanMaXacThuc" },
                values: new object[] { null, null, 0, null });

            migrationBuilder.UpdateData(
                table: "TaiKhoan",
                keyColumn: "MaTaiKhoan",
                keyValue: 27,
                columns: new[] { "KhoaDenThoiDiem", "MaXacThucKhoiPhuc", "SoLanDangNhapSai", "ThoiHanMaXacThuc" },
                values: new object[] { null, null, 0, null });

            migrationBuilder.UpdateData(
                table: "TaiKhoan",
                keyColumn: "MaTaiKhoan",
                keyValue: 28,
                columns: new[] { "KhoaDenThoiDiem", "MaXacThucKhoiPhuc", "SoLanDangNhapSai", "ThoiHanMaXacThuc" },
                values: new object[] { null, null, 0, null });

            migrationBuilder.UpdateData(
                table: "TaiKhoan",
                keyColumn: "MaTaiKhoan",
                keyValue: 29,
                columns: new[] { "KhoaDenThoiDiem", "MaXacThucKhoiPhuc", "SoLanDangNhapSai", "ThoiHanMaXacThuc" },
                values: new object[] { null, null, 0, null });

            migrationBuilder.UpdateData(
                table: "TaiKhoan",
                keyColumn: "MaTaiKhoan",
                keyValue: 30,
                columns: new[] { "KhoaDenThoiDiem", "MaXacThucKhoiPhuc", "SoLanDangNhapSai", "ThoiHanMaXacThuc" },
                values: new object[] { null, null, 0, null });

            migrationBuilder.UpdateData(
                table: "TaiKhoan",
                keyColumn: "MaTaiKhoan",
                keyValue: 31,
                columns: new[] { "KhoaDenThoiDiem", "MaXacThucKhoiPhuc", "SoLanDangNhapSai", "ThoiHanMaXacThuc" },
                values: new object[] { null, null, 0, null });

            migrationBuilder.UpdateData(
                table: "TaiKhoan",
                keyColumn: "MaTaiKhoan",
                keyValue: 32,
                columns: new[] { "KhoaDenThoiDiem", "MaXacThucKhoiPhuc", "SoLanDangNhapSai", "ThoiHanMaXacThuc" },
                values: new object[] { null, null, 0, null });

            migrationBuilder.UpdateData(
                table: "TaiKhoan",
                keyColumn: "MaTaiKhoan",
                keyValue: 33,
                columns: new[] { "KhoaDenThoiDiem", "MaXacThucKhoiPhuc", "SoLanDangNhapSai", "ThoiHanMaXacThuc" },
                values: new object[] { null, null, 0, null });

            migrationBuilder.UpdateData(
                table: "TaiKhoan",
                keyColumn: "MaTaiKhoan",
                keyValue: 34,
                columns: new[] { "KhoaDenThoiDiem", "MaXacThucKhoiPhuc", "SoLanDangNhapSai", "ThoiHanMaXacThuc" },
                values: new object[] { null, null, 0, null });

            migrationBuilder.UpdateData(
                table: "TaiKhoan",
                keyColumn: "MaTaiKhoan",
                keyValue: 35,
                columns: new[] { "KhoaDenThoiDiem", "MaXacThucKhoiPhuc", "SoLanDangNhapSai", "ThoiHanMaXacThuc" },
                values: new object[] { null, null, 0, null });
        }
    }
}
