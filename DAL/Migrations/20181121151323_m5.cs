using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Fotron.DAL.Migrations
{
    public partial class m5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "buy_count",
                table: "er_token_statistics",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "sell_count",
                table: "er_token_statistics",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<decimal>(
                name: "share_reward",
                table: "er_token_statistics",
                type: "decimal(38, 18)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "buy_count",
                table: "er_token_statistics");

            migrationBuilder.DropColumn(
                name: "sell_count",
                table: "er_token_statistics");

            migrationBuilder.DropColumn(
                name: "share_reward",
                table: "er_token_statistics");
        }
    }
}
