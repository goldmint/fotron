using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Fotron.DAL.Migrations
{
    public partial class m3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_deleted",
                table: "er_user_verification",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "is_enabled",
                table: "er_user_verification",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "is_deleted",
                table: "er_user_options",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "is_enabled",
                table: "er_user_options",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "is_deleted",
                table: "er_user_oplog",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "is_enabled",
                table: "er_user_oplog",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "is_deleted",
                table: "er_user_limits",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "is_enabled",
                table: "er_user_limits",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "is_deleted",
                table: "er_user_activity",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "is_enabled",
                table: "er_user_activity",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "is_deleted",
                table: "er_token_statistics",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "is_enabled",
                table: "er_token_statistics",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "is_deleted",
                table: "er_token",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "is_enabled",
                table: "er_token",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "time_created",
                table: "er_token",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "time_updated",
                table: "er_token",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "is_deleted",
                table: "er_signed_document",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "is_enabled",
                table: "er_signed_document",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "is_deleted",
                table: "er_settings",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "is_enabled",
                table: "er_settings",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "is_deleted",
                table: "er_kyc_shuftipro_ticket",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "is_enabled",
                table: "er_kyc_shuftipro_ticket",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "is_deleted",
                table: "er_banned_country",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "is_enabled",
                table: "er_banned_country",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_deleted",
                table: "er_user_verification");

            migrationBuilder.DropColumn(
                name: "is_enabled",
                table: "er_user_verification");

            migrationBuilder.DropColumn(
                name: "is_deleted",
                table: "er_user_options");

            migrationBuilder.DropColumn(
                name: "is_enabled",
                table: "er_user_options");

            migrationBuilder.DropColumn(
                name: "is_deleted",
                table: "er_user_oplog");

            migrationBuilder.DropColumn(
                name: "is_enabled",
                table: "er_user_oplog");

            migrationBuilder.DropColumn(
                name: "is_deleted",
                table: "er_user_limits");

            migrationBuilder.DropColumn(
                name: "is_enabled",
                table: "er_user_limits");

            migrationBuilder.DropColumn(
                name: "is_deleted",
                table: "er_user_activity");

            migrationBuilder.DropColumn(
                name: "is_enabled",
                table: "er_user_activity");

            migrationBuilder.DropColumn(
                name: "is_deleted",
                table: "er_token_statistics");

            migrationBuilder.DropColumn(
                name: "is_enabled",
                table: "er_token_statistics");

            migrationBuilder.DropColumn(
                name: "is_deleted",
                table: "er_token");

            migrationBuilder.DropColumn(
                name: "is_enabled",
                table: "er_token");

            migrationBuilder.DropColumn(
                name: "time_created",
                table: "er_token");

            migrationBuilder.DropColumn(
                name: "time_updated",
                table: "er_token");

            migrationBuilder.DropColumn(
                name: "is_deleted",
                table: "er_signed_document");

            migrationBuilder.DropColumn(
                name: "is_enabled",
                table: "er_signed_document");

            migrationBuilder.DropColumn(
                name: "is_deleted",
                table: "er_settings");

            migrationBuilder.DropColumn(
                name: "is_enabled",
                table: "er_settings");

            migrationBuilder.DropColumn(
                name: "is_deleted",
                table: "er_kyc_shuftipro_ticket");

            migrationBuilder.DropColumn(
                name: "is_enabled",
                table: "er_kyc_shuftipro_ticket");

            migrationBuilder.DropColumn(
                name: "is_deleted",
                table: "er_banned_country");

            migrationBuilder.DropColumn(
                name: "is_enabled",
                table: "er_banned_country");
        }
    }
}
