using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace AcraData.Migrations.Trigger
{
    public partial class InvoiceChange : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "TriggerPersons",
                type: "datetime",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "TriggerPersons",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "TriggerPersons",
                type: "int(11)",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SourceId",
                table: "DicTriggerReportReasons",
                type: "int(11)",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Status",
                table: "TriggerPersons",
                column: "Status");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Status",
                table: "TriggerPersons");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "TriggerPersons");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "TriggerPersons");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "TriggerPersons");

            migrationBuilder.DropColumn(
                name: "SourceId",
                table: "DicTriggerReportReasons");
        }
    }
}
