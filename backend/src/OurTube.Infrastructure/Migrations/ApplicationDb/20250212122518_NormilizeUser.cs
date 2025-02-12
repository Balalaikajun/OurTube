using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OurTube.Infrastructure.Migrations.ApplicationDb
{
    /// <inheritdoc />
    public partial class NormilizeUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccessFailedCount",
                table: "ApplicationUser");

            migrationBuilder.DropColumn(
                name: "ConcurrencyStamp",
                table: "ApplicationUser");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "ApplicationUser");

            migrationBuilder.DropColumn(
                name: "EmailConfirmed",
                table: "ApplicationUser");

            migrationBuilder.DropColumn(
                name: "LockoutEnabled",
                table: "ApplicationUser");

            migrationBuilder.DropColumn(
                name: "LockoutEnd",
                table: "ApplicationUser");

            migrationBuilder.DropColumn(
                name: "NormalizedEmail",
                table: "ApplicationUser");

            migrationBuilder.DropColumn(
                name: "NormalizedUserName",
                table: "ApplicationUser");

            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "ApplicationUser");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "ApplicationUser");

            migrationBuilder.DropColumn(
                name: "PhoneNumberConfirmed",
                table: "ApplicationUser");

            migrationBuilder.DropColumn(
                name: "SecurityStamp",
                table: "ApplicationUser");

            migrationBuilder.DropColumn(
                name: "TwoFactorEnabled",
                table: "ApplicationUser");

            migrationBuilder.DropColumn(
                name: "UserName",
                table: "ApplicationUser");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AccessFailedCount",
                table: "ApplicationUser",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ConcurrencyStamp",
                table: "ApplicationUser",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "ApplicationUser",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EmailConfirmed",
                table: "ApplicationUser",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "LockoutEnabled",
                table: "ApplicationUser",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LockoutEnd",
                table: "ApplicationUser",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NormalizedEmail",
                table: "ApplicationUser",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NormalizedUserName",
                table: "ApplicationUser",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PasswordHash",
                table: "ApplicationUser",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "ApplicationUser",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "PhoneNumberConfirmed",
                table: "ApplicationUser",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "SecurityStamp",
                table: "ApplicationUser",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "TwoFactorEnabled",
                table: "ApplicationUser",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "ApplicationUser",
                type: "text",
                nullable: true);
        }
    }
}
