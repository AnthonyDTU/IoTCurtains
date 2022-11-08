using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartPlatformBackendAPI.Migrations
{
    /// <inheritdoc />
    public partial class removedBackendURLFromDevice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "backendUri",
                table: "DeviceDecriptors");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "backendUri",
                table: "DeviceDecriptors",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
