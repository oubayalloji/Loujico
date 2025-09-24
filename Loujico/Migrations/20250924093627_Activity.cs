using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Loujico.Migrations
{
    /// <inheritdoc />
    public partial class Activity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Co_Activities_Co_Companies_CompanyId",
                table: "Co_Activities");

            migrationBuilder.DropIndex(
                name: "IX_Co_Activities_CompanyId",
                table: "Co_Activities");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "Co_Activities");

            migrationBuilder.CreateTable(
                name: "Co_CompanyActivities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    ActivityId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Co_CompanyActivities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Co_CompanyActivities_Co_Activities_ActivityId",
                        column: x => x.ActivityId,
                        principalTable: "Co_Activities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Co_CompanyActivities_Co_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Co_Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Co_CompanyActivities_ActivityId",
                table: "Co_CompanyActivities",
                column: "ActivityId");

            migrationBuilder.CreateIndex(
                name: "IX_Co_CompanyActivities_CompanyId",
                table: "Co_CompanyActivities",
                column: "CompanyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Co_CompanyActivities");

            migrationBuilder.AddColumn<int>(
                name: "CompanyId",
                table: "Co_Activities",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Co_Activities_CompanyId",
                table: "Co_Activities",
                column: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Co_Activities_Co_Companies_CompanyId",
                table: "Co_Activities",
                column: "CompanyId",
                principalTable: "Co_Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
