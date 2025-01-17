using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Singer.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserDW2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Certificate",
                table: "UsersDw");

            migrationBuilder.DropColumn(
                name: "PinCertificate",
                table: "UsersDw");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Certificate",
                table: "UsersDw",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PinCertificate",
                table: "UsersDw",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
