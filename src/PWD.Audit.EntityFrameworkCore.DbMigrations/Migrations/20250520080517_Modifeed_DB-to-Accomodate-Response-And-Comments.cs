using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PWD.Audit.Migrations
{
    public partial class Modifeed_DBtoAccomodateResponseAndComments : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ResponseHistory_Objections_ObjectionId",
                table: "ResponseHistory");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ResponseHistory",
                table: "ResponseHistory");

            migrationBuilder.RenameTable(
                name: "ResponseHistory",
                newName: "ResponseHistories");

            migrationBuilder.RenameIndex(
                name: "IX_ResponseHistory_ObjectionId",
                table: "ResponseHistories",
                newName: "IX_ResponseHistories_ObjectionId");

            migrationBuilder.AddColumn<string>(
                name: "CurrentOffice",
                table: "Objections",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Recommendation",
                table: "ResponseHistories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ResponseHistories",
                table: "ResponseHistories",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "ResponseComments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ResponseHistoryId = table.Column<int>(type: "int", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Office = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    User = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PostingId = table.Column<int>(type: "int", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResponseComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResponseComments_ResponseHistories_ResponseHistoryId",
                        column: x => x.ResponseHistoryId,
                        principalTable: "ResponseHistories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ResponseStates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ResponseHistoryId = table.Column<int>(type: "int", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsModified = table.Column<bool>(type: "bit", nullable: false),
                    Office = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    User = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PostingId = table.Column<int>(type: "int", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResponseStates", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ResponseComments_ResponseHistoryId",
                table: "ResponseComments",
                column: "ResponseHistoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_ResponseHistories_Objections_ObjectionId",
                table: "ResponseHistories",
                column: "ObjectionId",
                principalTable: "Objections",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ResponseHistories_Objections_ObjectionId",
                table: "ResponseHistories");

            migrationBuilder.DropTable(
                name: "ResponseComments");

            migrationBuilder.DropTable(
                name: "ResponseStates");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ResponseHistories",
                table: "ResponseHistories");

            migrationBuilder.DropColumn(
                name: "CurrentOffice",
                table: "Objections");

            migrationBuilder.DropColumn(
                name: "Recommendation",
                table: "ResponseHistories");

            migrationBuilder.RenameTable(
                name: "ResponseHistories",
                newName: "ResponseHistory");

            migrationBuilder.RenameIndex(
                name: "IX_ResponseHistories_ObjectionId",
                table: "ResponseHistory",
                newName: "IX_ResponseHistory_ObjectionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ResponseHistory",
                table: "ResponseHistory",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ResponseHistory_Objections_ObjectionId",
                table: "ResponseHistory",
                column: "ObjectionId",
                principalTable: "Objections",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
