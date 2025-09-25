using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Loujico.Migrations
{
    /// <inheritdoc />
    public partial class realtion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IndustryId",
                table: "Co_CompanyActivities",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Co_CompanyActivities_IndustryId",
                table: "Co_CompanyActivities",
                column: "IndustryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Co_CompanyActivities_Co_Industries_IndustryId",
                table: "Co_CompanyActivities",
                column: "IndustryId",
                principalTable: "Co_Industries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Co_CompanyActivities_Co_Industries_IndustryId",
                table: "Co_CompanyActivities");

            migrationBuilder.DropIndex(
                name: "IX_Co_CompanyActivities_IndustryId",
                table: "Co_CompanyActivities");

            migrationBuilder.DropColumn(
                name: "IndustryId",
                table: "Co_CompanyActivities");
        }
    }
}
