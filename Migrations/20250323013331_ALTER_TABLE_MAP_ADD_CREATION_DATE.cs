using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JurMaps.Migrations
{
    /// <inheritdoc />
    public partial class ALTER_TABLE_MAP_ADD_CREATION_DATE : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MapCreationDate",
                table: "Maps",
                type: "text",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "UserPassword",
                value: "$2a$13$0cs/MSYkJFviJC39vzGJoOzX/CPbwSgdjmGAiwuqxoNOs.MwzuqI6");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 2,
                column: "UserPassword",
                value: "$2a$13$98MI226Pq.HME6a5w64dm.SPnU5PzEixkOHykR.JYBFLzmuaANsxa");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MapCreationDate",
                table: "Maps");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "UserPassword",
                value: "$2a$13$67i973c80ICTfz0USin3CuxejEmVdjT7h6scWbF/VgNhqkSz2YOPi");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 2,
                column: "UserPassword",
                value: "$2a$13$4EPXckeLfiV8AUKpt2vRye74ZBE0clKOUZykvy2RQ5eZI2pYZOqIm");
        }
    }
}
