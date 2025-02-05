using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace support.Migrations
{
    /// <inheritdoc />
    public partial class updatesysadmins1126 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageAttachment",
                table: "Messages",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageAttachment",
                table: "Messages");
        }
    }
}
