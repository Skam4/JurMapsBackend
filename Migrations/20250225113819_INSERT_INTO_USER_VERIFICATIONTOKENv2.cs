using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JurMaps.Migrations
{
    /// <inheritdoc />
    public partial class INSERT_INTO_USER_VERIFICATIONTOKENv2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "UserPassword",
                value: "$2a$13$5JyJOBIkaKUNYuv42VJ7VuNrEVXzCUbtOYEQ5rqSbD7X/fO7AYbLm");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 2,
                columns: new[] { "IsVerified", "UserPassword" },
                values: new object[] { true, "$2a$13$8vH0gXb.nRBgayxP64OE4eZy9HZsMgdSsq1JEfNnt/R1IWsEFsEuW" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "UserPassword",
                value: "$2a$13$k79UR2Chenjhjv5L4z5AEuJjtfNglRCSdSu7v.eR5Jc87UIypmxkK");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 2,
                columns: new[] { "IsVerified", "UserPassword" },
                values: new object[] { false, "$2a$13$1bfLdwKld.6cyqxMC6RPiOiEAoP1ZyX/GlNaAiR5uWFeKwH2gqD7e" });
        }
    }
}
