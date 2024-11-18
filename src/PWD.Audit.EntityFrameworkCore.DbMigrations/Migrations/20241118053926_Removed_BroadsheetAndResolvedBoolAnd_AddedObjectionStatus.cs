using Microsoft.EntityFrameworkCore.Migrations;

namespace PWD.Audit.Migrations
{
    public partial class Removed_BroadsheetAndResolvedBoolAnd_AddedObjectionStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsBroadSheet",
                table: "Objections");

            migrationBuilder.DropColumn(
                name: "IsResolved",
                table: "Objections");

            migrationBuilder.AddColumn<int>(
                name: "ObjectionStatus",
                table: "Objections",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ObjectionStatus",
                table: "Objections");

            migrationBuilder.AddColumn<bool>(
                name: "IsBroadSheet",
                table: "Objections",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsResolved",
                table: "Objections",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
