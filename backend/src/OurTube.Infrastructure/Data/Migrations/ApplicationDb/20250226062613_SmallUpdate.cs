using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OurTube.Infrastructure.Data.Migrations.ApplicationDb
{
    /// <inheritdoc />
    public partial class SmallUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserAvatar_Bucket_BucketId",
                table: "UserAvatar");

            migrationBuilder.DropForeignKey(
                name: "FK_VideoPlaylist_Bucket_BucketId",
                table: "VideoPlaylist");

            migrationBuilder.DropForeignKey(
                name: "FK_VideoPreview_Bucket_BucketId",
                table: "VideoPreview");

            migrationBuilder.DropForeignKey(
                name: "FK_VideoSource_Bucket_BucketId",
                table: "VideoSource");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Bucket",
                table: "Bucket");

            migrationBuilder.DropColumn(
                name: "FileDirInStorage",
                table: "VideoSource");

            migrationBuilder.DropColumn(
                name: "FileDirInStorage",
                table: "VideoPreview");

            migrationBuilder.DropColumn(
                name: "FileDirInStorage",
                table: "VideoPlaylist");

            migrationBuilder.RenameTable(
                name: "Bucket",
                newName: "Buckets");

            migrationBuilder.AddColumn<int>(
                name: "SubscribedToCount",
                table: "ApplicationUser",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SubscribersCount",
                table: "ApplicationUser",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Buckets",
                table: "Buckets",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserAvatar_Buckets_BucketId",
                table: "UserAvatar",
                column: "BucketId",
                principalTable: "Buckets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VideoPlaylist_Buckets_BucketId",
                table: "VideoPlaylist",
                column: "BucketId",
                principalTable: "Buckets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VideoPreview_Buckets_BucketId",
                table: "VideoPreview",
                column: "BucketId",
                principalTable: "Buckets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VideoSource_Buckets_BucketId",
                table: "VideoSource",
                column: "BucketId",
                principalTable: "Buckets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserAvatar_Buckets_BucketId",
                table: "UserAvatar");

            migrationBuilder.DropForeignKey(
                name: "FK_VideoPlaylist_Buckets_BucketId",
                table: "VideoPlaylist");

            migrationBuilder.DropForeignKey(
                name: "FK_VideoPreview_Buckets_BucketId",
                table: "VideoPreview");

            migrationBuilder.DropForeignKey(
                name: "FK_VideoSource_Buckets_BucketId",
                table: "VideoSource");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Buckets",
                table: "Buckets");

            migrationBuilder.DropColumn(
                name: "SubscribedToCount",
                table: "ApplicationUser");

            migrationBuilder.DropColumn(
                name: "SubscribersCount",
                table: "ApplicationUser");

            migrationBuilder.RenameTable(
                name: "Buckets",
                newName: "Bucket");

            migrationBuilder.AddColumn<string>(
                name: "FileDirInStorage",
                table: "VideoSource",
                type: "character varying(125)",
                maxLength: 125,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FileDirInStorage",
                table: "VideoPreview",
                type: "character varying(125)",
                maxLength: 125,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FileDirInStorage",
                table: "VideoPlaylist",
                type: "character varying(125)",
                maxLength: 125,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Bucket",
                table: "Bucket",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserAvatar_Bucket_BucketId",
                table: "UserAvatar",
                column: "BucketId",
                principalTable: "Bucket",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VideoPlaylist_Bucket_BucketId",
                table: "VideoPlaylist",
                column: "BucketId",
                principalTable: "Bucket",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VideoPreview_Bucket_BucketId",
                table: "VideoPreview",
                column: "BucketId",
                principalTable: "Bucket",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VideoSource_Bucket_BucketId",
                table: "VideoSource",
                column: "BucketId",
                principalTable: "Bucket",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
