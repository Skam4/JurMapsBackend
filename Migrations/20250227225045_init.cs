using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JurMaps.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                column: "UserPassword",
                value: "$2a$13$swFbex.c9yVZyXtkoJE/U.AZO6lMeTozRtRTlSeKVhmTx5dOi8mDi");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 2,
                column: "UserPassword",
                value: "$2a$13$tvmaM5PUPQ1L60ZvUmK16.aYH0UU6Lxj16NIhdqI5wmfZEHEFOKD.");
        }
    }
}
