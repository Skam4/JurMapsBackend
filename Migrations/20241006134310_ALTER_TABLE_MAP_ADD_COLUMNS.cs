using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace JurMaps.Migrations
{
    /// <inheritdoc />
    public partial class ALTER_TABLE_MAP_ADD_COLUMNS : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Photos");

            migrationBuilder.AddColumn<int>(
                name: "MapLikes",
                table: "Maps",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MapPlacesQuantity",
                table: "Maps",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "MapPublicationDate",
                table: "Maps",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MapThumbnail",
                table: "Maps",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MapLikes",
                table: "Maps");

            migrationBuilder.DropColumn(
                name: "MapPlacesQuantity",
                table: "Maps");

            migrationBuilder.DropColumn(
                name: "MapPublicationDate",
                table: "Maps");

            migrationBuilder.DropColumn(
                name: "MapThumbnail",
                table: "Maps");

            migrationBuilder.CreateTable(
                name: "Photos",
                columns: table => new
                {
                    PhotoId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PhotoMapThumbnailMapId = table.Column<int>(type: "integer", nullable: false),
                    PhotoSource = table.Column<string>(type: "text", nullable: false),
                    PhotoWhenTaken = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Photos", x => x.PhotoId);
                    table.ForeignKey(
                        name: "FK_Photos_Maps_PhotoMapThumbnailMapId",
                        column: x => x.PhotoMapThumbnailMapId,
                        principalTable: "Maps",
                        principalColumn: "MapId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Photos_PhotoMapThumbnailMapId",
                table: "Photos",
                column: "PhotoMapThumbnailMapId");
        }
    }
}
