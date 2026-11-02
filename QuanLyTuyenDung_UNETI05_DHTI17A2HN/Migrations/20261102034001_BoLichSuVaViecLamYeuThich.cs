using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyTuyenDung_UNETI05_DHTI17A2HN.Migrations
{
    /// <inheritdoc />
    public partial class BoLichSuVaViecLamYeuThich : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LichSuViTri");

            migrationBuilder.DropTable(
                name: "ViecLamYeuThich");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LichSuViTri",
                columns: table => new
                {
                    MaLichSu = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaViTri = table.Column<int>(type: "int", nullable: true),
                    HanhDong = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MaTaiKhoan = table.Column<int>(type: "int", nullable: true),
                    NguoiThucHien = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NoiDung = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TenViTri = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
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

            migrationBuilder.CreateIndex(
                name: "IX_LichSuViTri_MaViTri_ThoiGian",
                table: "LichSuViTri",
                columns: new[] { "MaViTri", "ThoiGian" });

            migrationBuilder.CreateIndex(
                name: "IX_ViecLamYeuThich_MaViTri",
                table: "ViecLamYeuThich",
                column: "MaViTri");
        }
    }
}
