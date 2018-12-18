using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Fotron.DAL.Migrations
{
    public partial class m4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "price_eth",
                table: "er_token_statistics",
                type: "decimal(38, 18)",
                nullable: false,
                oldClrType: typeof(long));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "price_eth",
                table: "er_token_statistics",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(38, 18)");
        }
    }
}
