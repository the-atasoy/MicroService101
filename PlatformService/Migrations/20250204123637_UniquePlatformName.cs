using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlatformService.Migrations
{
    /// <inheritdoc />
    public partial class UniquePlatformName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Platform_Name",
                table: "Platform",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Platform_Name",
                table: "Platform");
        }
    }
}
