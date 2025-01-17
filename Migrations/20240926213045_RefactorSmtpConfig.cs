using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Singer.Migrations
{
    /// <inheritdoc />
    public partial class RefactorSmtpConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "SmtpConfigs");

            migrationBuilder.RenameColumn(
                name: "SmtpClient",
                table: "SmtpConfigs",
                newName: "Password");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Password",
                table: "SmtpConfigs",
                newName: "SmtpClient");

            migrationBuilder.AddColumn<string>(
                name: "PasswordHash",
                table: "SmtpConfigs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
