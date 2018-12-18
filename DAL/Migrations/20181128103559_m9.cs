using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Fotron.DAL.Migrations
{
    public partial class m9 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "er_add_token_request",
                columns: table => new
                {
                    id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    company_name = table.Column<string>(maxLength: 128, nullable: false),
                    contact_email = table.Column<string>(maxLength: 128, nullable: false),
                    is_deleted = table.Column<bool>(nullable: false),
                    is_enabled = table.Column<bool>(nullable: false),
                    start_price_eth = table.Column<decimal>(type: "decimal(38, 18)", nullable: false),
                    token_contract_address = table.Column<string>(maxLength: 43, nullable: true),
                    token_ticker = table.Column<string>(maxLength: 128, nullable: false),
                    total_supply = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_er_add_token_request", x => x.id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "er_add_token_request");
        }
    }
}
