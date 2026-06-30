using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace AcraData.Migrations.Trigger
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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
                name: "DicTriggerReportReasons",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ReportReasonID = table.Column<int>(type: "int(11)", nullable: true),
                    ReportSubReasonID = table.Column<int>(type: "int(11)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DicTriggerReportReasons", x => x.ID);
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

            migrationBuilder.CreateTable(
                name: "TriggerSources",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Filter = table.Column<string>(type: "text", nullable: false),
                    SourceID = table.Column<int>(type: "int(11)", nullable: false),
                    Status = table.Column<int>(type: "int(11)", nullable: false, defaultValueSql: "'0'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TriggerSources", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "TriggerPersons",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PersonID = table.Column<int>(type: "int(11)", nullable: false),
                    PersonType = table.Column<int>(type: "int(11)", nullable: false),
                    SysDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    TSID = table.Column<int>(type: "int(11)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TriggerPersons", x => x.ID);
                    table.ForeignKey(
                        name: "fk_TSID",
                        column: x => x.TSID,
                        principalTable: "TriggerSources",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TriggerReports",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ActivityTime = table.Column<DateTime>(type: "datetime", nullable: false),
                    PersonId = table.Column<int>(type: "int(11)", nullable: false),
                    ReasonId = table.Column<int>(type: "int(11)", nullable: true),
                    ReportId = table.Column<int>(type: "int(11)", nullable: true),
                    ReportInfo = table.Column<string>(type: "text", nullable: true),
                    SourceId = table.Column<int>(type: "int(11)", nullable: false),
                    SubReasonId = table.Column<int>(type: "int(11)", nullable: true),
                    SysDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    TSID = table.Column<int>(type: "int(11)", nullable: true),
                    UserActivityId = table.Column<long>(type: "bigint(20)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TriggerReports", x => x.ID);
                    table.ForeignKey(
                        name: "fk_RepTSID",
                        column: x => x.TSID,
                        principalTable: "TriggerSources",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TriggerVolumes",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Date = table.Column<DateTime>(type: "date", nullable: false),
                    TSID = table.Column<int>(type: "int(11)", nullable: false),
                    Volume = table.Column<int>(type: "int(11)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TriggerVolumes", x => x.ID);
                    table.ForeignKey(
                        name: "fk_SourceVolume",
                        column: x => x.TSID,
                        principalTable: "TriggerSources",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TriggerPersonsDetails",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint(20)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PersonID = table.Column<int>(type: "int(11)", nullable: false),
                    SysDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    TPID = table.Column<long>(type: "bigint(20)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TriggerPersonsDetails", x => x.ID);
                    table.ForeignKey(
                        name: "fk_TPID",
                        column: x => x.TPID,
                        principalTable: "TriggerPersons",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ReportReasonID",
                table: "DicReportSubReasons",
                column: "ReportReasonID");

            migrationBuilder.CreateIndex(
                name: "CreditorTypeID",
                table: "Sources",
                column: "CreditorTypeID");

            migrationBuilder.CreateIndex(
                name: "fk_TSID",
                table: "TriggerPersons",
                column: "TSID");

            migrationBuilder.CreateIndex(
                name: "fk_TPID",
                table: "TriggerPersonsDetails",
                column: "TPID");

            migrationBuilder.CreateIndex(
                name: "fk_RepTSID",
                table: "TriggerReports",
                column: "TSID");

            migrationBuilder.CreateIndex(
                name: "fk_SourceVolume",
                table: "TriggerVolumes",
                column: "TSID");

            migrationBuilder.CreateIndex(
                name: "pk_Date",
                table: "TriggerVolumes",
                columns: new[] { "Date", "TSID" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DicReportReasons");

            migrationBuilder.DropTable(
                name: "DicReports");

            migrationBuilder.DropTable(
                name: "DicReportSubReasons");

            migrationBuilder.DropTable(
                name: "DicTriggerReportReasons");

            migrationBuilder.DropTable(
                name: "Sources");

            migrationBuilder.DropTable(
                name: "TriggerPersonsDetails");

            migrationBuilder.DropTable(
                name: "TriggerReports");

            migrationBuilder.DropTable(
                name: "TriggerVolumes");

            migrationBuilder.DropTable(
                name: "TriggerPersons");

            migrationBuilder.DropTable(
                name: "TriggerSources");
        }
    }
}
