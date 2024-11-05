using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FGraduation_Project.Migrations
{
    /// <inheritdoc />
    public partial class ConnectHistoryWithAppUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Historys",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Historys_UserId",
                table: "Historys",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Historys_AspNetUsers_UserId",
                table: "Historys",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Historys_AspNetUsers_UserId",
                table: "Historys");

            migrationBuilder.DropIndex(
                name: "IX_Historys_UserId",
                table: "Historys");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Historys");
        }
    }
}
