using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace support.Migrations
{
    /// <inheritdoc />
    public partial class updatesysadmins1123 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SentBy",
                table: "Messages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SentBy",
                table: "Messages");
        }
    }
}
