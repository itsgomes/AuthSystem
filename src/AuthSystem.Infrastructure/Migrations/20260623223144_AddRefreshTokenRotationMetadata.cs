using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRefreshTokenRotationMetadata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ReplacedByTokenId",
                table: "RefreshTokens",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RevokedReason",
                table: "RefreshTokens",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReplacedByTokenId",
                table: "RefreshTokens");

            migrationBuilder.DropColumn(
                name: "RevokedReason",
                table: "RefreshTokens");
        }
    }
}
