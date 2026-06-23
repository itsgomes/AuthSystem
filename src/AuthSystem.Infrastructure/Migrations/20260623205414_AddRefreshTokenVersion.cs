using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRefreshTokenVersion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "RefreshTokens",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Version",
                table: "RefreshTokens");
        }
    }
}
