using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace JurMaps.Migrations
{
    /// <inheritdoc />
    public partial class fresh : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Bounds",
                table: "Places");

            migrationBuilder.DropColumn(
                name: "PlaceRadius",
                table: "Places");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "UserPassword",
                value: "$2a$13$rmLFn8kDYKQHa6N4KxWTf.2q0/e9BWySKmZHhpewXMQeSgPU0LULy");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Polygon>(
                name: "Bounds",
                table: "Places",
                type: "geometry",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "PlaceRadius",
                table: "Places",
                type: "double precision",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "UserPassword",
                value: "$2a$13$ImefbTv9EPQnXNXsdd0rfeEryeAW5W5p2.Vlg/5qjdtZZ2OA7e1bS");
        }
    }
}
