using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Fotron.DAL.Migrations
{
    public partial class tokenstat : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "er_token",
                columns: table => new
                {
                    id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    contract_address = table.Column<string>(maxLength: 43, nullable: false),
                    description = table.Column<string>(maxLength: 1024, nullable: false),
                    user_id = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_er_token", x => x.id);
                    table.ForeignKey(
                        name: "FK_er_token_er_user_user_id",
                        column: x => x.user_id,
                        principalTable: "er_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "er_token_statistics",
                columns: table => new
                {
                    id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    date = table.Column<DateTime>(nullable: false),
                    token_id = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_er_token_statistics", x => x.id);
                    table.ForeignKey(
                        name: "FK_er_token_statistics_er_token_token_id",
                        column: x => x.token_id,
                        principalTable: "er_token",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_er_token_user_id",
                table: "er_token",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_er_token_statistics_token_id",
                table: "er_token_statistics",
                column: "token_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "er_token_statistics");

            migrationBuilder.DropTable(
                name: "er_token");
        }
    }
}
