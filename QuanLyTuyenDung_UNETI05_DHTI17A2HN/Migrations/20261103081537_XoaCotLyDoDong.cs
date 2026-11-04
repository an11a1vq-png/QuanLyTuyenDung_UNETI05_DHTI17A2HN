using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyTuyenDung_UNETI05_DHTI17A2HN.Migrations
{
    /// <inheritdoc />
    public partial class XoaCotLyDoDong : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LyDoDong",
                table: "ViTriTuyenDung");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LyDoDong",
                table: "ViTriTuyenDung",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

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
        }
    }
}
