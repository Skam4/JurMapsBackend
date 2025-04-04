using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JurMaps.Migrations
{
    /// <inheritdoc />
    public partial class init41 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PlacePhoto1",
                table: "Places",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PlacePhoto2",
                table: "Places",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PlacePhoto3",
                table: "Places",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PlacePhoto1",
                table: "Places");

            migrationBuilder.DropColumn(
                name: "PlacePhoto2",
                table: "Places");

            migrationBuilder.DropColumn(
                name: "PlacePhoto3",
                table: "Places");
        }
    }
}
