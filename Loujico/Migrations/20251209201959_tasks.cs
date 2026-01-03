using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Loujico.Migrations
{
    /// <inheritdoc />
    public partial class tasks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TbProjectTasks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProjectId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    EstimatedHours = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TbProjectTasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TbProjectTasks_TbProjects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "TbProjects",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TbProjectTaskEmployees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TaskId = table.Column<int>(type: "int", nullable: false),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    RoleOnTask = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AssignedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TbProjectTaskEmployees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TbProjectTaskEmployees_TbEmployees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "TbEmployees",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TbProjectTaskEmployees_TbProjectTasks_TaskId",
                        column: x => x.TaskId,
                        principalTable: "TbProjectTasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TbProjectTaskEmployees_EmployeeId",
                table: "TbProjectTaskEmployees",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_TbProjectTaskEmployees_TaskId",
                table: "TbProjectTaskEmployees",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_TbProjectTasks_ProjectId",
                table: "TbProjectTasks",
                column: "ProjectId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TbProjectTaskEmployees");

            migrationBuilder.DropTable(
                name: "TbProjectTasks");
        }
    }
}
