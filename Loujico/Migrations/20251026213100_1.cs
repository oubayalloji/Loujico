using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Loujico.Migrations
{
    /// <inheritdoc />
    public partial class _1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
    

            migrationBuilder.DropTable(
                name: "TbCustomers");

            migrationBuilder.RenameColumn(
                name: "customer_id",
                table: "TbCompanyProducts",
                newName: "CompanyId");



            migrationBuilder.AddForeignKey(
                name: "FK_TbCompanyProducts_CoCompanies_CompanyId",
                table: "TbCompanyProducts",
                column: "CompanyId",
                principalTable: "Co_Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TbCompanyProducts_TbProducts_ProductId",
                table: "TbCompanyProducts",
                column: "product_id",
                principalTable: "TbProducts",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TbCompanyProducts_CoCompanies_CompanyId",
                table: "TbCompanyProducts");

            migrationBuilder.DropForeignKey(
                name: "FK_TbCompanyProducts_TbProducts_ProductId",
                table: "TbCompanyProducts");

            migrationBuilder.RenameColumn(
                name: "CompanyId",
                table: "TbCompanyProducts",
                newName: "customer_id");

            migrationBuilder.RenameIndex(
                name: "IX_TbCompanyProducts_CompanyId",
                table: "TbCompanyProducts",
                newName: "IX_TbCompanyProducts_customer_id");

            migrationBuilder.CreateTable(
                name: "TbCustomers",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    company_description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(sysutcdatetime())"),
                    created_by = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    customerAddress = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    customerName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    industry = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    inquiry = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    is_deleted = table.Column<bool>(type: "bit", nullable: false),
                    last_visit = table.Column<DateTime>(type: "datetime2", nullable: true),
                    phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    service_provided = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    work_date = table.Column<DateOnly>(type: "date", nullable: true),
                    work_duration = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TbCustom__3213E83F06F3797B", x => x.id);
                });


        }
    }
}
