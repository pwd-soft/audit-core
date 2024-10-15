using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PWD.Audit.Migrations
{
    public partial class AddPostingTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Type",
                table: "Objections",
                newName: "Description");

            migrationBuilder.AddColumn<string>(
                name: "Attachments",
                table: "Objections",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DirectorateType",
                table: "Objections",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ObjectionType",
                table: "Objections",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Postings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PostingId = table.Column<int>(type: "int", nullable: false),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NameBn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Post = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Designation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DesignationBn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Office = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OfficeBn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrgUniId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Postings", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Postings");

            migrationBuilder.DropColumn(
                name: "Attachments",
                table: "Objections");

            migrationBuilder.DropColumn(
                name: "DirectorateType",
                table: "Objections");

            migrationBuilder.DropColumn(
                name: "ObjectionType",
                table: "Objections");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Objections",
                newName: "Type");
        }
    }
}
