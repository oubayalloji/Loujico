using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Loujico.Migrations
{
    /// <inheritdoc />
    public partial class linkemployee : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
       

            migrationBuilder.CreateIndex(
                name: "IX_TbEmployees_UserId",
                table: "TbEmployees",
                column: "UserId",
                unique: true,
                filter: "[UserId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
           name: "FK_TbEmployees_AspNetUsers_UserId",
           table: "TbEmployees",
           column: "UserId",
           principalTable: "AspNetUsers",
           principalColumn: "Id",
           onDelete: ReferentialAction.SetNull);
        }


        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {


            migrationBuilder.DropIndex(
                name: "IX_TbEmployees_UserId",
                table: "TbEmployees");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "TbEmployees");
        }
    }
}
