using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FGraduation_Project.Migrations
{
    /// <inheritdoc />
    public partial class addMonthColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Month",
                table: "Historys",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Month",
                table: "Historys");
        }
    }
}
