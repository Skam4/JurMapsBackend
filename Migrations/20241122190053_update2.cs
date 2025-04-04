using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JurMaps.Migrations
{
    /// <inheritdoc />
    public partial class update2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "UserPassword",
                value: "$2a$13$ImefbTv9EPQnXNXsdd0rfeEryeAW5W5p2.Vlg/5qjdtZZ2OA7e1bS");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "UserPassword",
                value: "$2a$13$0vEP3cinvuNf/K4uJOoZIufW0z9ZOjwK5hoUvPwFormi1kZx0gPf6");
        }
    }
}
