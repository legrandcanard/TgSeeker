using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TgSeeker.Persistent.Sqlite.Migrations
{
    /// <inheritdoc />
    public partial class TgsVideoNoteMessage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Length",
                table: "Messages",
                type: "INTEGER",
                nullable: true);

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

            migrationBuilder.AddColumn<int>(
                name: "TgsVoiceMessage_Duration",
                table: "Messages",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TgsVoiceMessage_LocalFileId",
                table: "Messages",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "TgsVoiceMessage_Waveform",
                table: "Messages",
                type: "BLOB",
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Length",
                table: "Messages");

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
                name: "TgsVoiceMessage_Duration",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "TgsVoiceMessage_LocalFileId",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "TgsVoiceMessage_Waveform",
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
    }
}
