using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PWD.Audit.Migrations
{
    public partial class updateuser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Post",
                table: "OfficeUsers");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "OfficeUsers",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "OfficeUsers");

            migrationBuilder.AddColumn<string>(
                name: "Post",
                table: "OfficeUsers",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
