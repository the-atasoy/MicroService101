using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CommandService.Migrations
{
    /// <inheritdoc />
    public partial class composite_unique_howto_platformid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Command_CommandLine_PlatformId",
                table: "Command",
                columns: new[] { "CommandLine", "PlatformId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Command_CommandLine_PlatformId",
                table: "Command");
        }
    }
}
