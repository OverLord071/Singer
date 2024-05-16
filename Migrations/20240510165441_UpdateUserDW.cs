using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Singer.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserDW : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Token",
                table: "UsersDw",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "TokenExpiration",
                table: "UsersDw",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Token",
                table: "UsersDw");

            migrationBuilder.DropColumn(
                name: "TokenExpiration",
                table: "UsersDw");
        }
    }
}
