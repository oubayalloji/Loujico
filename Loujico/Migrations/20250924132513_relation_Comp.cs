using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Loujico.Migrations
{
    /// <inheritdoc />
    public partial class relation_Comp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Co_CompanyLegals");

         

            migrationBuilder.AddColumn<int>(
                name: "LegalId",
                table: "Co_Companies",
                type: "int",
                nullable: true);

          

            migrationBuilder.CreateIndex(
                name: "IX_Co_Companies_LegalId",
                table: "Co_Companies",
                column: "LegalId");

            migrationBuilder.AddForeignKey(
                name: "FK_Co_Companies_Co_Legals_LegalId",
                table: "Co_Companies",
                column: "LegalId",
                principalTable: "Co_Legals",
                principalColumn: "Id");

       
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Co_Companies_Co_Legals_LegalId",
                table: "Co_Companies");

     

     

            migrationBuilder.DropIndex(
                name: "IX_Co_Companies_LegalId",
                table: "Co_Companies");

        

            migrationBuilder.DropColumn(
                name: "LegalId",
                table: "Co_Companies");

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
    }
}
