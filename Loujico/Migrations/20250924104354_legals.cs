using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Loujico.Migrations
{
    /// <inheritdoc />
    public partial class legals : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Co_Legals_Co_Companies_CompanyId",
                table: "Co_Legals");

            migrationBuilder.DropIndex(
                name: "IX_Co_Legals_CompanyId",
                table: "Co_Legals");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "Co_Legals");

            migrationBuilder.CreateTable(
                name: "Co_CompanyLegals",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    LegalId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Co_CompanyLegals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Co_CompanyLegals_Co_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Co_Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Co_CompanyLegals_Co_Legals_LegalId",
                        column: x => x.LegalId,
                        principalTable: "Co_Legals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Co_CompanyLegals_CompanyId",
                table: "Co_CompanyLegals",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Co_CompanyLegals_LegalId",
                table: "Co_CompanyLegals",
                column: "LegalId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Co_CompanyLegals");

            migrationBuilder.AddColumn<int>(
                name: "CompanyId",
                table: "Co_Legals",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Co_Legals_CompanyId",
                table: "Co_Legals",
                column: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Co_Legals_Co_Companies_CompanyId",
                table: "Co_Legals",
                column: "CompanyId",
                principalTable: "Co_Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
