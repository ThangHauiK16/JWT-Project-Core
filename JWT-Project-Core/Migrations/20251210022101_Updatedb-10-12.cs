using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JWT_Project_Core.Migrations
{
    /// <inheritdoc />
    public partial class Updatedb1012 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Order_Book",
                columns: table => new
                {
                    MaHoaDon = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MaSach = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SoLuong = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Order_Book", x => new { x.MaHoaDon, x.MaSach });
                    table.ForeignKey(
                        name: "FK_Order_Book_Books_MaSach",
                        column: x => x.MaSach,
                        principalTable: "Books",
                        principalColumn: "MaSach",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Order_Book_Orders_MaHoaDon",
                        column: x => x.MaHoaDon,
                        principalTable: "Orders",
                        principalColumn: "MaHoaDon",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Order_Book_MaSach",
                table: "Order_Book",
                column: "MaSach");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Order_Book");
        }
    }
}
