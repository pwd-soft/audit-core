using Microsoft.EntityFrameworkCore.Migrations;

namespace PWD.Audit.Migrations
{
    public partial class Added_LocktoResponseState : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LockStatus",
                table: "ResponseHistories");

            migrationBuilder.AddColumn<bool>(
                name: "IsLocked",
                table: "ResponseStates",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "User",
                table: "ResponseHistories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ResponseStates_ResponseHistoryId",
                table: "ResponseStates",
                column: "ResponseHistoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_ResponseStates_ResponseHistories_ResponseHistoryId",
                table: "ResponseStates",
                column: "ResponseHistoryId",
                principalTable: "ResponseHistories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ResponseStates_ResponseHistories_ResponseHistoryId",
                table: "ResponseStates");

            migrationBuilder.DropIndex(
                name: "IX_ResponseStates_ResponseHistoryId",
                table: "ResponseStates");

            migrationBuilder.DropColumn(
                name: "IsLocked",
                table: "ResponseStates");

            migrationBuilder.DropColumn(
                name: "User",
                table: "ResponseHistories");

            migrationBuilder.AddColumn<bool>(
                name: "LockStatus",
                table: "ResponseHistories",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
