using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace AcraData.Migrations.Trigger
{
    public partial class MoveToAcra3DT : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DicReportReasons");

            migrationBuilder.DropTable(
                name: "DicReports");

            migrationBuilder.DropTable(
                name: "DicReportSubReasons");

            migrationBuilder.DropTable(
                name: "Sources");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DicReportReasons",
                columns: table => new
                {
                    ReportReasonID = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ReportReason = table.Column<string>(maxLength: 200, nullable: false, defaultValueSql: "''")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DicReportReasons", x => x.ReportReasonID);
                });

            migrationBuilder.CreateTable(
                name: "DicReports",
                columns: table => new
                {
                    ReportID = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Report = table.Column<string>(maxLength: 200, nullable: true),
                    ReportPrice = table.Column<int>(type: "int(11)", nullable: true),
                    ReportType = table.Column<int>(type: "int(11)", nullable: false, defaultValueSql: "'0'"),
                    ScoreReport = table.Column<int>(type: "int(11)", nullable: true, defaultValueSql: "'0'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DicReports", x => x.ReportID);
                });

            migrationBuilder.CreateTable(
                name: "DicReportSubReasons",
                columns: table => new
                {
                    ReportSubReasonID = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ReportReasonID = table.Column<int>(type: "int(11)", nullable: false, defaultValueSql: "'0'"),
                    ReportSubReason = table.Column<string>(maxLength: 200, nullable: false, defaultValueSql: "''")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DicReportSubReasons", x => x.ReportSubReasonID);
                });

            migrationBuilder.CreateTable(
                name: "Sources",
                columns: table => new
                {
                    SourceID = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    AccountNumber = table.Column<string>(maxLength: 100, nullable: true),
                    Accountant = table.Column<string>(maxLength: 100, nullable: true),
                    Address = table.Column<string>(maxLength: 200, nullable: true),
                    Bank = table.Column<string>(maxLength: 100, nullable: true),
                    ContractDate = table.Column<DateTime>(type: "date", nullable: true),
                    ContractId = table.Column<string>(maxLength: 100, nullable: true),
                    CreditorCode = table.Column<string>(maxLength: 50, nullable: true),
                    CreditorTypeID = table.Column<sbyte>(type: "tinyint(4)", nullable: false, defaultValueSql: "'1'"),
                    eMail = table.Column<string>(maxLength: 200, nullable: true),
                    Fax = table.Column<string>(maxLength: 100, nullable: true),
                    HomePage = table.Column<string>(maxLength: 100, nullable: true),
                    HVHH = table.Column<string>(maxLength: 100, nullable: true),
                    Manager = table.Column<string>(maxLength: 100, nullable: true),
                    Phone = table.Column<string>(maxLength: 100, nullable: true),
                    ShortName = table.Column<string>(maxLength: 6, nullable: true),
                    ShowInReport = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    SourceName = table.Column<string>(maxLength: 200, nullable: true),
                    SourceType = table.Column<int>(type: "int(11)", nullable: true),
                    SpecialDiscount = table.Column<int>(type: "int(11)", nullable: true, defaultValueSql: "'0'"),
                    XMLName = table.Column<string>(maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sources", x => x.SourceID);
                });

            migrationBuilder.CreateIndex(
                name: "ReportReasonID",
                table: "DicReportSubReasons",
                column: "ReportReasonID");

            migrationBuilder.CreateIndex(
                name: "CreditorTypeID",
                table: "Sources",
                column: "CreditorTypeID");
        }
    }
}
