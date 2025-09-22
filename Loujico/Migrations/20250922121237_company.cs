using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Loujico.Migrations
{
    /// <inheritdoc />
    public partial class company : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Co_Companies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Comm_No = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Tax_No = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Found_Date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompanyDescription = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastVisit = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Co_Companies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Co_Industries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Co_Industries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TbContact",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TbContact", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TbCountries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TbCountries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Co_Legals",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    LegalInfo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Co_Legals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Co_Legals_Co_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Co_Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Co_Activities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    IndustryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Co_Activities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Co_Activities_Co_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Co_Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Co_Activities_Co_Industries_IndustryId",
                        column: x => x.IndustryId,
                        principalTable: "Co_Industries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Co_Contacts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    ContactTypeId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Co_Contacts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Co_Contacts_Co_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Co_Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Co_Contacts_TbContact_ContactTypeId",
                        column: x => x.ContactTypeId,
                        principalTable: "TbContact",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TbStates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CountrId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TbStates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TbStates_TbCountries_CountrId",
                        column: x => x.CountrId,
                        principalTable: "TbCountries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TbCities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    StateId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TbCities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TbCities_TbStates_StateId",
                        column: x => x.StateId,
                        principalTable: "TbStates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Co_Address",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    CountryId = table.Column<int>(type: "int", nullable: false),
                    StateId = table.Column<int>(type: "int", nullable: false),
                    CityId = table.Column<int>(type: "int", nullable: false),
                    AddressLine = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Co_Address", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Co_Address_Co_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Co_Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Co_Address_TbCities_CityId",
                        column: x => x.CityId,
                        principalTable: "TbCities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Co_Address_TbCountries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "TbCountries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Co_Address_TbStates_StateId",
                        column: x => x.StateId,
                        principalTable: "TbStates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Co_Activities_CompanyId",
                table: "Co_Activities",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Co_Activities_IndustryId",
                table: "Co_Activities",
                column: "IndustryId");

            migrationBuilder.CreateIndex(
                name: "IX_Co_Address_CityId",
                table: "Co_Address",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_Co_Address_CompanyId",
                table: "Co_Address",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Co_Address_CountryId",
                table: "Co_Address",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Co_Address_StateId",
                table: "Co_Address",
                column: "StateId");

            migrationBuilder.CreateIndex(
                name: "IX_Co_Contacts_CompanyId",
                table: "Co_Contacts",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Co_Contacts_ContactTypeId",
                table: "Co_Contacts",
                column: "ContactTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Co_Legals_CompanyId",
                table: "Co_Legals",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_TbCities_StateId",
                table: "TbCities",
                column: "StateId");

            migrationBuilder.CreateIndex(
                name: "IX_TbStates_CountrId",
                table: "TbStates",
                column: "CountrId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Co_Activities");

            migrationBuilder.DropTable(
                name: "Co_Address");

            migrationBuilder.DropTable(
                name: "Co_Contacts");

            migrationBuilder.DropTable(
                name: "Co_Legals");

            migrationBuilder.DropTable(
                name: "Co_Industries");

            migrationBuilder.DropTable(
                name: "TbCities");

            migrationBuilder.DropTable(
                name: "TbContact");

            migrationBuilder.DropTable(
                name: "Co_Companies");

            migrationBuilder.DropTable(
                name: "TbStates");

            migrationBuilder.DropTable(
                name: "TbCountries");
        }
    }
}
