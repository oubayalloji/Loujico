using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Loujico.Migrations
{
    /// <inheritdoc />
    public partial class CompanyEmployees : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Co_CompanyEmployees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Position = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Department = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Co_CompanyEmployees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Co_CompanyEmployees_Co_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Co_Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Co_CompanyEmployees_CompanyId",
                table: "Co_CompanyEmployees",
                column: "CompanyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Co_CompanyEmployees");
        }
    }
}
