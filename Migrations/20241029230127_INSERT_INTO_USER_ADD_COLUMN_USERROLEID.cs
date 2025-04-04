using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JurMaps.Migrations
{
    /// <inheritdoc />
    public partial class INSERT_INTO_USER_ADD_COLUMN_USERROLEID : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Roles_UserRoleRoleId",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "UserRoleRoleId",
                table: "Users",
                newName: "UserRoleId");

            migrationBuilder.RenameIndex(
                name: "IX_Users_UserRoleRoleId",
                table: "Users",
                newName: "IX_Users_UserRoleId");

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "ResetPasswordToken", "ResetPasswordTokenExpiration", "UserCreatedDate", "UserEmail", "UserName", "UserPassword", "UserProfilePicture", "UserRoleId" },
                values: new object[] { 1, null, null, null, "admin@admin.pl", "admin", "$2a$13$n7CWz1yTGsLnIBWaoCLrDuRENYG37WWCslR.2IZwRFJvoMU9o/fA2", null, 2 });

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Roles_UserRoleId",
                table: "Users",
                column: "UserRoleId",
                principalTable: "Roles",
                principalColumn: "RoleId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Roles_UserRoleId",
                table: "Users");

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1);

            migrationBuilder.RenameColumn(
                name: "UserRoleId",
                table: "Users",
                newName: "UserRoleRoleId");

            migrationBuilder.RenameIndex(
                name: "IX_Users_UserRoleId",
                table: "Users",
                newName: "IX_Users_UserRoleRoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Roles_UserRoleRoleId",
                table: "Users",
                column: "UserRoleRoleId",
                principalTable: "Roles",
                principalColumn: "RoleId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
