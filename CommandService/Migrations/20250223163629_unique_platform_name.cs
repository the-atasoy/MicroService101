using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CommandService.Migrations
{
    /// <inheritdoc />
    public partial class unique_platform_name : Migration
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
