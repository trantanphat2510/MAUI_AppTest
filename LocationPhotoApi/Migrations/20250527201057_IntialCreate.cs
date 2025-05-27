using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LocationPhotoApi.Migrations
{
    /// <inheritdoc />
    public partial class IntialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageData",
                table: "PhotoInfos");

            migrationBuilder.AddColumn<string>(
                name: "ImagePath",
                table: "PhotoInfos",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagePath",
                table: "PhotoInfos");

            migrationBuilder.AddColumn<byte[]>(
                name: "ImageData",
                table: "PhotoInfos",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);
        }
    }
}
