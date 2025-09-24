using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Loujico.Migrations
{
    /// <inheritdoc />
    public partial class ExtendContactToSupportEmployees : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "Co_CompanyEmployees");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "Co_CompanyEmployees");

            migrationBuilder.AddColumn<int>(
                name: "EmployeeId",
                table: "Co_Contacts",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Co_Contacts_EmployeeId",
                table: "Co_Contacts",
                column: "EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Co_Contacts_Co_CompanyEmployees_EmployeeId",
                table: "Co_Contacts",
                column: "EmployeeId",
                principalTable: "Co_CompanyEmployees",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Co_Contacts_Co_CompanyEmployees_EmployeeId",
                table: "Co_Contacts");

            migrationBuilder.DropIndex(
                name: "IX_Co_Contacts_EmployeeId",
                table: "Co_Contacts");

            migrationBuilder.DropColumn(
                name: "EmployeeId",
                table: "Co_Contacts");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Co_CompanyEmployees",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "Co_CompanyEmployees",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);
        }
    }
}
