using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JWT_Project_Core.Migrations
{
    /// <inheritdoc />
    public partial class UpdateV4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Username",
                table: "HoaDons",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_HoaDons_Username",
                table: "HoaDons",
                column: "Username");

            migrationBuilder.AddForeignKey(
                name: "FK_HoaDons_Users_Username",
                table: "HoaDons",
                column: "Username",
                principalTable: "Users",
                principalColumn: "Username",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HoaDons_Users_Username",
                table: "HoaDons");

            migrationBuilder.DropIndex(
                name: "IX_HoaDons_Username",
                table: "HoaDons");

            migrationBuilder.DropColumn(
                name: "Username",
                table: "HoaDons");
        }
    }
}
