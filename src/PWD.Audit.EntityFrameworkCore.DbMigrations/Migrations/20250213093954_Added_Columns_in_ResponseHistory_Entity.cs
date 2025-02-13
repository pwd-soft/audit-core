using Microsoft.EntityFrameworkCore.Migrations;

namespace PWD.Audit.Migrations
{
    public partial class Added_Columns_in_ResponseHistory_Entity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "LockStatus",
                table: "ResponseHistory",
                type: "bit",
                nullable: false,
                defaultValue: false);

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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LockStatus",
                table: "ResponseHistory");

            migrationBuilder.DropColumn(
                name: "MonitorComment",
                table: "ResponseHistory");

            migrationBuilder.DropColumn(
                name: "MonitorUsername",
                table: "ResponseHistory");
        }
    }
}
