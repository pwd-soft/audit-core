using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PWD.Audit.Migrations
{
    public partial class Removed_OfficeId_From_AllEntities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OfficeId",
                table: "YearlyObjections");

            migrationBuilder.DropColumn(
                name: "OfficeId",
                table: "Summaries");

            migrationBuilder.DropColumn(
                name: "OfficeId",
                table: "ResponseHistory");

            migrationBuilder.DropColumn(
                name: "OfficeId",
                table: "OfficeUsers");

            migrationBuilder.DropColumn(
                name: "OfficeId",
                table: "Objections");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "OfficeId",
                table: "YearlyObjections",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "OfficeId",
                table: "Summaries",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "OfficeId",
                table: "ResponseHistory",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "OfficeId",
                table: "OfficeUsers",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "OfficeId",
                table: "Objections",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }
    }
}
