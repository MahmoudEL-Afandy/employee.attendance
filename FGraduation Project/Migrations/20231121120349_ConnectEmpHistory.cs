using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FGraduation_Project.Migrations
{
    /// <inheritdoc />
    public partial class ConnectEmpHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EmployeeId",
                table: "Historys",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Historys_EmployeeId",
                table: "Historys",
                column: "EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Historys_Employees_EmployeeId",
                table: "Historys",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Historys_Employees_EmployeeId",
                table: "Historys");

            migrationBuilder.DropIndex(
                name: "IX_Historys_EmployeeId",
                table: "Historys");

            migrationBuilder.DropColumn(
                name: "EmployeeId",
                table: "Historys");
        }
    }
}
