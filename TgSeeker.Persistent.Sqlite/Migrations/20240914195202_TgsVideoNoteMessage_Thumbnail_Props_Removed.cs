using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TgSeeker.Persistent.Sqlite.Migrations
{
    /// <inheritdoc />
    public partial class TgsVideoNoteMessage_Thumbnail_Props_Removed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MinithumbnailData",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "MinithumbnailHeight",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "MinithumbnailWidth",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "ThumbnailFormat",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "ThumbnailHeight",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "ThumbnailLocalFileId",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "ThumbnailWidth",
                table: "Messages");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "MinithumbnailData",
                table: "Messages",
                type: "BLOB",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MinithumbnailHeight",
                table: "Messages",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MinithumbnailWidth",
                table: "Messages",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ThumbnailFormat",
                table: "Messages",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ThumbnailHeight",
                table: "Messages",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ThumbnailLocalFileId",
                table: "Messages",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ThumbnailWidth",
                table: "Messages",
                type: "INTEGER",
                nullable: true);
        }
    }
}
