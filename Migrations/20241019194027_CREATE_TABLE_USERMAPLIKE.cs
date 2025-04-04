using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JurMaps.Migrations
{
    /// <inheritdoc />
    public partial class CREATE_TABLE_USERMAPLIKE : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Maps_Users_UserId",
                table: "Maps");

            migrationBuilder.DropIndex(
                name: "IX_Maps_UserId",
                table: "Maps");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Maps");

            migrationBuilder.AlterColumn<string>(
                name: "MapPublicationDate",
                table: "Maps",
                type: "text",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "UserMapLikes",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    MapId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserMapLikes", x => new { x.UserId, x.MapId });
                    table.ForeignKey(
                        name: "FK_UserMapLikes_Maps_MapId",
                        column: x => x.MapId,
                        principalTable: "Maps",
                        principalColumn: "MapId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserMapLikes_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserMapLikes_MapId",
                table: "UserMapLikes",
                column: "MapId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserMapLikes");

            migrationBuilder.AlterColumn<DateTime>(
                name: "MapPublicationDate",
                table: "Maps",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Maps",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Maps_UserId",
                table: "Maps",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Maps_Users_UserId",
                table: "Maps",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId");
        }
    }
}
