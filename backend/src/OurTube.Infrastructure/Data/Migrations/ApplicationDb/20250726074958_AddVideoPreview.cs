using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OurTube.Infrastructure.Data.Migrations.ApplicationDb
{
    /// <inheritdoc />
    public partial class AddVideoPreview : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VideoPreview_Videos_VideoId",
                table: "VideoPreview");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VideoPreview",
                table: "VideoPreview");

            migrationBuilder.RenameTable(
                name: "VideoPreview",
                newName: "VideoPreviews");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VideoPreviews",
                table: "VideoPreviews",
                column: "VideoId");

            migrationBuilder.AddForeignKey(
                name: "FK_VideoPreviews_Videos_VideoId",
                table: "VideoPreviews",
                column: "VideoId",
                principalTable: "Videos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VideoPreviews_Videos_VideoId",
                table: "VideoPreviews");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VideoPreviews",
                table: "VideoPreviews");

            migrationBuilder.RenameTable(
                name: "VideoPreviews",
                newName: "VideoPreview");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VideoPreview",
                table: "VideoPreview",
                column: "VideoId");

            migrationBuilder.AddForeignKey(
                name: "FK_VideoPreview_Videos_VideoId",
                table: "VideoPreview",
                column: "VideoId",
                principalTable: "Videos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
