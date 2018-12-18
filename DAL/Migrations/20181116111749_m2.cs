using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Fotron.DAL.Migrations
{
    public partial class m2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            /*
            migrationBuilder.DropForeignKey(
                name: "FK_er_token_er_user_user_id",
                table: "er_token");
               
            migrationBuilder.DropIndex(
                name: "IX_er_token_user_id",
                table: "er_token");
                 */
            migrationBuilder.DropColumn(
                name: "user_id",
                table: "er_token");

            migrationBuilder.RenameColumn(
                name: "contract_address",
                table: "er_token",
                newName: "fotron_contract_address");

            migrationBuilder.AddColumn<string>(
                name: "erc20_contract_address",
                table: "er_token",
                maxLength: 43,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "erc20_contract_address",
                table: "er_token");

            migrationBuilder.RenameColumn(
                name: "fotron_contract_address",
                table: "er_token",
                newName: "contract_address");

            migrationBuilder.AddColumn<long>(
                name: "user_id",
                table: "er_token",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_er_token_user_id",
                table: "er_token",
                column: "user_id");

            migrationBuilder.AddForeignKey(
                name: "FK_er_token_er_user_user_id",
                table: "er_token",
                column: "user_id",
                principalTable: "er_user",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
