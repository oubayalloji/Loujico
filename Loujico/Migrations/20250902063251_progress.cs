using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Loujico.Migrations
{
    /// <inheritdoc />
    public partial class progress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
     

            migrationBuilder.AddColumn<int>(
                name: "Progress",
                table: "TbProjects",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ProjectType",
                table: "TbProjects",
                type: "nvarchar(max)",
                nullable: true);

         
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
          

            migrationBuilder.DropColumn(
                name: "Progress",
                table: "TbProjects");

            migrationBuilder.DropColumn(
                name: "ProjectType",
                table: "TbProjects");

         
        }
    }
}
