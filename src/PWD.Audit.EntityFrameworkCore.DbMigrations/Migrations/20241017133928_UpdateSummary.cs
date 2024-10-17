using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PWD.Audit.Migrations
{
    public partial class UpdateSummary : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Date",
                table: "SummaryLines");

            migrationBuilder.DropColumn(
                name: "FinancialYear",
                table: "SummaryLines");

            migrationBuilder.DropColumn(
                name: "OfficeId",
                table: "SummaryLines");

            migrationBuilder.AlterColumn<int>(
                name: "Type",
                table: "SummaryLines",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Note",
                table: "SummaryLines",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "SummaryId",
                table: "SummaryLines",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_SummaryLines_SummaryId",
                table: "SummaryLines",
                column: "SummaryId");

            migrationBuilder.AddForeignKey(
                name: "FK_SummaryLines_Summaries_SummaryId",
                table: "SummaryLines",
                column: "SummaryId",
                principalTable: "Summaries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SummaryLines_Summaries_SummaryId",
                table: "SummaryLines");

            migrationBuilder.DropIndex(
                name: "IX_SummaryLines_SummaryId",
                table: "SummaryLines");

            migrationBuilder.DropColumn(
                name: "SummaryId",
                table: "SummaryLines");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "SummaryLines",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "Note",
                table: "SummaryLines",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "SummaryLines",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "FinancialYear",
                table: "SummaryLines",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "OfficeId",
                table: "SummaryLines",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }
    }
}
