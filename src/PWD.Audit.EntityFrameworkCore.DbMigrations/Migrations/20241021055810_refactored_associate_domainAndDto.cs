using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PWD.Audit.Migrations
{
    public partial class refactored_associate_domainAndDto : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Associates_Objections_ObjectionId",
                table: "Associates");

            migrationBuilder.DropColumn(
                name: "ComplainId",
                table: "Associates");

            migrationBuilder.DropColumn(
                name: "EmployeeId",
                table: "Associates");

            migrationBuilder.DropColumn(
                name: "EngagedFrom",
                table: "Associates");

            migrationBuilder.DropColumn(
                name: "EngagedTo",
                table: "Associates");

            migrationBuilder.DropColumn(
                name: "PostingId",
                table: "Associates");

            migrationBuilder.AlterColumn<int>(
                name: "ObjectionId",
                table: "Associates",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Associates_Objections_ObjectionId",
                table: "Associates",
                column: "ObjectionId",
                principalTable: "Objections",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Associates_Objections_ObjectionId",
                table: "Associates");

            migrationBuilder.AlterColumn<int>(
                name: "ObjectionId",
                table: "Associates",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "ComplainId",
                table: "Associates",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "EmployeeId",
                table: "Associates",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "EngagedFrom",
                table: "Associates",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "EngagedTo",
                table: "Associates",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "PostingId",
                table: "Associates",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_Associates_Objections_ObjectionId",
                table: "Associates",
                column: "ObjectionId",
                principalTable: "Objections",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
