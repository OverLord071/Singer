using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Singer.Migrations
{
    /// <inheritdoc />
    public partial class AddedStatusDocument : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Reason",
                table: "Documents",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StatusDocument",
                table: "Documents",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Reason",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "StatusDocument",
                table: "Documents");
        }
    }
}
