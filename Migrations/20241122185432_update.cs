using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JurMaps.Migrations
{
    /// <inheritdoc />
    public partial class update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Roles_UserRoleId",
                table: "Users");

            migrationBuilder.AlterColumn<int>(
                name: "UserRoleId",
                table: "Users",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "UserPassword",
                value: "$2a$13$0vEP3cinvuNf/K4uJOoZIufW0z9ZOjwK5hoUvPwFormi1kZx0gPf6");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Roles_UserRoleId",
                table: "Users",
                column: "UserRoleId",
                principalTable: "Roles",
                principalColumn: "RoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Roles_UserRoleId",
                table: "Users");

            migrationBuilder.AlterColumn<int>(
                name: "UserRoleId",
                table: "Users",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "UserPassword",
                value: "$2a$13$n7CWz1yTGsLnIBWaoCLrDuRENYG37WWCslR.2IZwRFJvoMU9o/fA2");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Roles_UserRoleId",
                table: "Users",
                column: "UserRoleId",
                principalTable: "Roles",
                principalColumn: "RoleId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
