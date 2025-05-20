using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PWD.Audit.Migrations
{
    public partial class Removed_Columns_From_ResponseHistory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Date",
                table: "ResponseHistory");

            migrationBuilder.DropColumn(
                name: "MonitorComment",
                table: "ResponseHistory");

            migrationBuilder.DropColumn(
                name: "MonitorUsername",
                table: "ResponseHistory");

            migrationBuilder.DropColumn(
                name: "ObjectionStatus",
                table: "ResponseHistory");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "ResponseHistory",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "MonitorComment",
                table: "ResponseHistory",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MonitorUsername",
                table: "ResponseHistory",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ObjectionStatus",
                table: "ResponseHistory",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
