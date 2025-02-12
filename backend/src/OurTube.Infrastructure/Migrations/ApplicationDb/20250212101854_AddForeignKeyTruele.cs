using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OurTube.Infrastructure.Migrations.ApplicationDb
{
    /// <inheritdoc />
    public partial class AddForeignKeyTruele : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Videos_Playlists_PlaylistId",
                table: "Videos");

            migrationBuilder.DropTable(
                name: "ApplicationUserApplicationUser");

            migrationBuilder.DropIndex(
                name: "IX_Videos_PlaylistId",
                table: "Videos");

            migrationBuilder.DropColumn(
                name: "PlaylistId",
                table: "Videos");

            migrationBuilder.CreateTable(
                name: "PlaylistElement",
                columns: table => new
                {
                    PlaylistId = table.Column<int>(type: "integer", nullable: false),
                    VideoId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlaylistElement", x => new { x.PlaylistId, x.VideoId });
                    table.ForeignKey(
                        name: "FK_PlaylistElement_Playlists_PlaylistId",
                        column: x => x.PlaylistId,
                        principalTable: "Playlists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlaylistElement_Videos_VideoId",
                        column: x => x.VideoId,
                        principalTable: "Videos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Subscription",
                columns: table => new
                {
                    SubscribedToId = table.Column<string>(type: "text", nullable: false),
                    SubscriberId = table.Column<string>(type: "text", nullable: false),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    SubscribedToId1 = table.Column<string>(type: "text", nullable: false),
                    SubscriberId1 = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subscription", x => new { x.SubscribedToId, x.SubscriberId });
                    table.ForeignKey(
                        name: "FK_Subscription_ApplicationUser_SubscribedToId",
                        column: x => x.SubscribedToId,
                        principalTable: "ApplicationUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Subscription_ApplicationUser_SubscribedToId1",
                        column: x => x.SubscribedToId1,
                        principalTable: "ApplicationUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Subscription_ApplicationUser_SubscriberId",
                        column: x => x.SubscriberId,
                        principalTable: "ApplicationUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Subscription_ApplicationUser_SubscriberId1",
                        column: x => x.SubscriberId1,
                        principalTable: "ApplicationUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlaylistElement_VideoId",
                table: "PlaylistElement",
                column: "VideoId");

            migrationBuilder.CreateIndex(
                name: "IX_Subscription_SubscribedToId1",
                table: "Subscription",
                column: "SubscribedToId1");

            migrationBuilder.CreateIndex(
                name: "IX_Subscription_SubscriberId",
                table: "Subscription",
                column: "SubscriberId");

            migrationBuilder.CreateIndex(
                name: "IX_Subscription_SubscriberId1",
                table: "Subscription",
                column: "SubscriberId1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlaylistElement");

            migrationBuilder.DropTable(
                name: "Subscription");

            migrationBuilder.AddColumn<int>(
                name: "PlaylistId",
                table: "Videos",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ApplicationUserApplicationUser",
                columns: table => new
                {
                    SubscribersId = table.Column<string>(type: "text", nullable: false),
                    SubscribеToId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationUserApplicationUser", x => new { x.SubscribersId, x.SubscribеToId });
                    table.ForeignKey(
                        name: "FK_ApplicationUserApplicationUser_ApplicationUser_SubscribersId",
                        column: x => x.SubscribersId,
                        principalTable: "ApplicationUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApplicationUserApplicationUser_ApplicationUser_SubscribеToId",
                        column: x => x.SubscribеToId,
                        principalTable: "ApplicationUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Videos_PlaylistId",
                table: "Videos",
                column: "PlaylistId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUserApplicationUser_SubscribеToId",
                table: "ApplicationUserApplicationUser",
                column: "SubscribеToId");

            migrationBuilder.AddForeignKey(
                name: "FK_Videos_Playlists_PlaylistId",
                table: "Videos",
                column: "PlaylistId",
                principalTable: "Playlists",
                principalColumn: "Id");
        }
    }
}
