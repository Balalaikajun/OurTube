using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OurTube.Infrastructure.Data.Migrations.ApplicationDb
{
    /// <inheritdoc />
    public partial class AnotherChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Playlists");

            migrationBuilder.AlterColumn<int>(
                name: "Count",
                table: "Playlists",
                type: "integer",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "Count",
                table: "Playlists",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Playlists",
                type: "character varying(5000)",
                maxLength: 5000,
                nullable: false,
                defaultValue: "");
        }
    }
}
