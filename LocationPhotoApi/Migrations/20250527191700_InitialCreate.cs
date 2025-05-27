using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LocationPhotoApi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Data",
                table: "QRCodeDatas",
                newName: "QrCode");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "QRCodeDatas",
                newName: "Timestamp");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Timestamp",
                table: "QRCodeDatas",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "QrCode",
                table: "QRCodeDatas",
                newName: "Data");
        }
    }
}
