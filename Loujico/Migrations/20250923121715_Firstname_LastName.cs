using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Loujico.Migrations
{
    /// <inheritdoc />
    public partial class Firstname_LastName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FullName",
                table: "Co_CompanyEmployees",
                newName: "LastName");

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "Co_CompanyEmployees",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "Co_CompanyEmployees");

            migrationBuilder.RenameColumn(
                name: "LastName",
                table: "Co_CompanyEmployees",
                newName: "FullName");
        }
    }
}
