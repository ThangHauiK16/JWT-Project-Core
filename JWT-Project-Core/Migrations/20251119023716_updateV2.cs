using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JWT_Project_Core.Migrations
{
    /// <inheritdoc />
    public partial class updateV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HoaDons",
                columns: table => new
                {
                    MaHoaDon = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HoaDons", x => x.MaHoaDon);
                });

            migrationBuilder.CreateTable(
                name: "Saches",
                columns: table => new
                {
                    MaSach = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TenSach = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TheLoai = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GiaNhap = table.Column<double>(type: "float", nullable: false),
                    GiaBan = table.Column<double>(type: "float", nullable: false),
                    TenTacGia = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NoiDungSach = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Saches", x => x.MaSach);
                });

            migrationBuilder.CreateTable(
                name: "HoaDon_Saches",
                columns: table => new
                {
                    MaHoaDon = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaSach = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SoLuong = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HoaDon_Saches", x => new { x.MaHoaDon, x.MaSach });
                    table.ForeignKey(
                        name: "FK_HoaDon_Saches_HoaDons_MaHoaDon",
                        column: x => x.MaHoaDon,
                        principalTable: "HoaDons",
                        principalColumn: "MaHoaDon",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HoaDon_Saches_Saches_MaSach",
                        column: x => x.MaSach,
                        principalTable: "Saches",
                        principalColumn: "MaSach",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HoaDon_Saches_MaSach",
                table: "HoaDon_Saches",
                column: "MaSach");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HoaDon_Saches");

            migrationBuilder.DropTable(
                name: "HoaDons");

            migrationBuilder.DropTable(
                name: "Saches");
        }
    }
}
