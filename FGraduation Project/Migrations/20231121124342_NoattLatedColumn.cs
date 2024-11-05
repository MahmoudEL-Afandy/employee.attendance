using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FGraduation_Project.Migrations
{
    /// <inheritdoc />
    public partial class NoattLatedColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsLated",
                table: "Historys",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "NoAttended",
                table: "Historys",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsLated",
                table: "Historys");

            migrationBuilder.DropColumn(
                name: "NoAttended",
                table: "Historys");
        }
    }
}
