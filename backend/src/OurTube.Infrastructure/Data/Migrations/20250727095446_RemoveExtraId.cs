using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OurTube.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveExtraId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_VideoSource",
                table: "VideoSource");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VideoPreviews",
                table: "VideoPreviews");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserAvatars",
                table: "UserAvatars");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VideoSource",
                table: "VideoSource",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VideoPreviews",
                table: "VideoPreviews",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserAvatars",
                table: "UserAvatars",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_VideoSource_VideoId",
                table: "VideoSource",
                column: "VideoId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VideoPreviews_VideoId",
                table: "VideoPreviews",
                column: "VideoId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserAvatars_UserId",
                table: "UserAvatars",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_VideoSource",
                table: "VideoSource");

            migrationBuilder.DropIndex(
                name: "IX_VideoSource_VideoId",
                table: "VideoSource");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VideoPreviews",
                table: "VideoPreviews");

            migrationBuilder.DropIndex(
                name: "IX_VideoPreviews_VideoId",
                table: "VideoPreviews");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserAvatars",
                table: "UserAvatars");

            migrationBuilder.DropIndex(
                name: "IX_UserAvatars_UserId",
                table: "UserAvatars");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VideoSource",
                table: "VideoSource",
                column: "VideoId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VideoPreviews",
                table: "VideoPreviews",
                column: "VideoId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserAvatars",
                table: "UserAvatars",
                column: "UserId");
        }
    }
}
