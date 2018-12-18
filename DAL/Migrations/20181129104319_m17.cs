using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Fotron.DAL.Migrations
{
    public partial class m17 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "volume_eth",
                table: "er_token_statistics",
                type: "decimal(38, 18)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "volume_token",
                table: "er_token_statistics",
                type: "decimal(38, 18)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "volume_eth",
                table: "er_token_statistics");

            migrationBuilder.DropColumn(
                name: "volume_token",
                table: "er_token_statistics");
        }
    }
}
