using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JurMaps.Migrations
{
    /// <inheritdoc />
    public partial class INSERT_INTO_USER_VERIFICATIONTOKEN : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsVerified",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastVerificationEmailSent",
                table: "Users",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VerificationToken",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "VerificationTokenExpiration",
                table: "Users",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                columns: new[] { "IsVerified", "LastVerificationEmailSent", "UserPassword", "VerificationToken", "VerificationTokenExpiration" },
                values: new object[] { false, null, "$2a$13$k79UR2Chenjhjv5L4z5AEuJjtfNglRCSdSu7v.eR5Jc87UIypmxkK", null, null });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "IsVerified", "LastVerificationEmailSent", "ResetPasswordToken", "ResetPasswordTokenExpiration", "UserCreatedDate", "UserEmail", "UserName", "UserPassword", "UserProfilePicture", "UserRoleId", "VerificationToken", "VerificationTokenExpiration" },
                values: new object[] { 2, false, null, null, null, null, "qwerty@qwerty.pl", "qwerty", "$2a$13$1bfLdwKld.6cyqxMC6RPiOiEAoP1ZyX/GlNaAiR5uWFeKwH2gqD7e", null, 1, null, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 2);

            migrationBuilder.DropColumn(
                name: "IsVerified",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LastVerificationEmailSent",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "VerificationToken",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "VerificationTokenExpiration",
                table: "Users");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "UserPassword",
                value: "$2a$13$rmLFn8kDYKQHa6N4KxWTf.2q0/e9BWySKmZHhpewXMQeSgPU0LULy");
        }
    }
}
