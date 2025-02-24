using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PWD.Audit.Migrations
{
    public partial class Addedobjectionmemonumberanddateaddedofficecodealongsideofficeid : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OfficeCode",
                table: "YearlyObjections",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OfficeCode",
                table: "Summaries",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OfficeCode",
                table: "OfficeUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ObjectionDate",
                table: "Objections",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "ObjectionMemoNumber",
                table: "Objections",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OfficeCode",
                table: "Objections",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OfficeCode",
                table: "YearlyObjections");

            migrationBuilder.DropColumn(
                name: "OfficeCode",
                table: "Summaries");

            migrationBuilder.DropColumn(
                name: "OfficeCode",
                table: "OfficeUsers");

            migrationBuilder.DropColumn(
                name: "ObjectionDate",
                table: "Objections");

            migrationBuilder.DropColumn(
                name: "ObjectionMemoNumber",
                table: "Objections");

            migrationBuilder.DropColumn(
                name: "OfficeCode",
                table: "Objections");
        }
    }
}
