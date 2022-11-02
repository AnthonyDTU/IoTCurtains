using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartPlatformBackendAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedDeviceDescriptorFieldName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DeviceType",
                table: "DeviceDecriptors",
                newName: "DeviceModel");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DeviceModel",
                table: "DeviceDecriptors",
                newName: "DeviceType");
        }
    }
}
