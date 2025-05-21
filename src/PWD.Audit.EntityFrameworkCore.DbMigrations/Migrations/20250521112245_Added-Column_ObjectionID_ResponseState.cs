using Microsoft.EntityFrameworkCore.Migrations;

namespace PWD.Audit.Migrations
{
    public partial class AddedColumn_ObjectionID_ResponseState : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ResponseStates_ResponseHistories_ResponseHistoryId",
                table: "ResponseStates");

            migrationBuilder.AlterColumn<int>(
                name: "ResponseHistoryId",
                table: "ResponseStates",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "ObjectionId",
                table: "ResponseStates",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_ResponseStates_ResponseHistories_ResponseHistoryId",
                table: "ResponseStates",
                column: "ResponseHistoryId",
                principalTable: "ResponseHistories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ResponseStates_ResponseHistories_ResponseHistoryId",
                table: "ResponseStates");

            migrationBuilder.DropColumn(
                name: "ObjectionId",
                table: "ResponseStates");

            migrationBuilder.AlterColumn<int>(
                name: "ResponseHistoryId",
                table: "ResponseStates",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ResponseStates_ResponseHistories_ResponseHistoryId",
                table: "ResponseStates",
                column: "ResponseHistoryId",
                principalTable: "ResponseHistories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
