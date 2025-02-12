using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OurTube.Infrastructure.Migrations.ApplicationDb
{
    /// <inheritdoc />
    public partial class UpdateSubscribes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Subscription_ApplicationUser_SubscribedToId1",
                table: "Subscription");

            migrationBuilder.DropForeignKey(
                name: "FK_Subscription_ApplicationUser_SubscriberId1",
                table: "Subscription");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Subscription",
                table: "Subscription");

            migrationBuilder.DropIndex(
                name: "IX_Subscription_SubscribedToId1",
                table: "Subscription");

            migrationBuilder.DropIndex(
                name: "IX_Subscription_SubscriberId",
                table: "Subscription");

            migrationBuilder.DropIndex(
                name: "IX_Subscription_SubscriberId1",
                table: "Subscription");

            migrationBuilder.DropColumn(
                name: "SubscribedToId1",
                table: "Subscription");

            migrationBuilder.DropColumn(
                name: "SubscriberId1",
                table: "Subscription");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Subscription",
                table: "Subscription",
                columns: new[] { "SubscriberId", "SubscribedToId" });

            migrationBuilder.CreateIndex(
                name: "IX_Subscription_SubscribedToId",
                table: "Subscription",
                column: "SubscribedToId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Subscription",
                table: "Subscription");

            migrationBuilder.DropIndex(
                name: "IX_Subscription_SubscribedToId",
                table: "Subscription");

            migrationBuilder.AddColumn<string>(
                name: "SubscribedToId1",
                table: "Subscription",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SubscriberId1",
                table: "Subscription",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Subscription",
                table: "Subscription",
                columns: new[] { "SubscribedToId", "SubscriberId" });

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

            migrationBuilder.AddForeignKey(
                name: "FK_Subscription_ApplicationUser_SubscribedToId1",
                table: "Subscription",
                column: "SubscribedToId1",
                principalTable: "ApplicationUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Subscription_ApplicationUser_SubscriberId1",
                table: "Subscription",
                column: "SubscriberId1",
                principalTable: "ApplicationUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
