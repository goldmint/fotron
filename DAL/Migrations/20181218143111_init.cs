using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Fotron.DAL.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ft_add_token_request",
                columns: table => new
                {
                    id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    company_name = table.Column<string>(maxLength: 128, nullable: false),
                    contact_email = table.Column<string>(maxLength: 128, nullable: false),
                    is_deleted = table.Column<bool>(nullable: false),
                    is_enabled = table.Column<bool>(nullable: false),
                    start_price_eth = table.Column<decimal>(type: "decimal(38, 18)", nullable: false),
                    time_created = table.Column<DateTime>(nullable: false),
                    token_contract_address = table.Column<string>(maxLength: 43, nullable: true),
                    token_ticker = table.Column<string>(maxLength: 128, nullable: false),
                    total_supply = table.Column<long>(nullable: false),
                    website_url = table.Column<string>(maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ft_add_token_request", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "ft_mutex",
                columns: table => new
                {
                    id = table.Column<string>(maxLength: 64, nullable: false),
                    expires = table.Column<DateTime>(nullable: false),
                    locker = table.Column<string>(maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ft_mutex", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "ft_role",
                columns: table => new
                {
                    id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    concurrency_stamp = table.Column<string>(maxLength: 64, nullable: true),
                    name = table.Column<string>(maxLength: 256, nullable: true),
                    normalized_name = table.Column<string>(maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ft_role", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "ft_settings",
                columns: table => new
                {
                    id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    concurrency_stamp = table.Column<string>(maxLength: 64, nullable: true),
                    is_deleted = table.Column<bool>(nullable: false),
                    is_enabled = table.Column<bool>(nullable: false),
                    key = table.Column<string>(maxLength: 64, nullable: false),
                    value = table.Column<string>(maxLength: 16384, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ft_settings", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "ft_token",
                columns: table => new
                {
                    id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    current_price_eth = table.Column<decimal>(type: "decimal(38, 18)", maxLength: 1024, nullable: false),
                    description = table.Column<string>(maxLength: 1024, nullable: false),
                    fotron_contract_address = table.Column<string>(maxLength: 43, nullable: false),
                    full_name = table.Column<string>(maxLength: 128, nullable: false),
                    is_deleted = table.Column<bool>(nullable: false),
                    is_enabled = table.Column<bool>(nullable: false),
                    logo_url = table.Column<string>(maxLength: 1024, nullable: false),
                    start_price_eth = table.Column<decimal>(type: "decimal(38, 18)", maxLength: 1024, nullable: false),
                    ticker = table.Column<string>(maxLength: 16, nullable: false),
                    time_created = table.Column<DateTime>(nullable: false),
                    time_updated = table.Column<DateTime>(nullable: false),
                    token_contract_address = table.Column<string>(maxLength: 43, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ft_token", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "ft_user",
                columns: table => new
                {
                    id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    access_failed_count = table.Column<int>(nullable: false),
                    access_rights = table.Column<long>(nullable: false),
                    concurrency_stamp = table.Column<string>(maxLength: 64, nullable: true),
                    email = table.Column<string>(maxLength: 256, nullable: true),
                    email_confirmed = table.Column<bool>(nullable: false),
                    jwt_salt_cab = table.Column<string>(maxLength: 64, nullable: false),
                    jwt_salt_dbr = table.Column<string>(maxLength: 64, nullable: false),
                    lockout_enabled = table.Column<bool>(nullable: false),
                    lockout_end = table.Column<DateTimeOffset>(nullable: true),
                    normalized_email = table.Column<string>(maxLength: 256, nullable: true),
                    normalized_username = table.Column<string>(maxLength: 256, nullable: true),
                    password_hash = table.Column<string>(maxLength: 512, nullable: true),
                    phone_number = table.Column<string>(maxLength: 64, nullable: true),
                    phone_number_confirmed = table.Column<bool>(nullable: false),
                    security_stamp = table.Column<string>(maxLength: 64, nullable: true),
                    tfa_secret = table.Column<string>(maxLength: 32, nullable: false),
                    time_registered = table.Column<DateTime>(nullable: false),
                    tfa_enabled = table.Column<bool>(nullable: false),
                    username = table.Column<string>(maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ft_user", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "ft_role_claim",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    claim_type = table.Column<string>(maxLength: 256, nullable: true),
                    claim_value = table.Column<string>(maxLength: 256, nullable: true),
                    role_id = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ft_role_claim", x => x.id);
                    table.ForeignKey(
                        name: "FK_ft_role_claim_ft_role_role_id",
                        column: x => x.role_id,
                        principalTable: "ft_role",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ft_token_statistics",
                columns: table => new
                {
                    id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    block_num = table.Column<long>(nullable: false),
                    buy_count = table.Column<long>(nullable: false),
                    date = table.Column<DateTime>(nullable: false),
                    is_deleted = table.Column<bool>(nullable: false),
                    is_enabled = table.Column<bool>(nullable: false),
                    price_eth = table.Column<decimal>(type: "decimal(38, 18)", nullable: false),
                    sell_count = table.Column<long>(nullable: false),
                    share_reward = table.Column<decimal>(type: "decimal(38, 18)", nullable: false),
                    token_id = table.Column<long>(nullable: false),
                    volume_eth = table.Column<decimal>(type: "decimal(38, 18)", nullable: false),
                    volume_token = table.Column<decimal>(type: "decimal(38, 18)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ft_token_statistics", x => x.id);
                    table.ForeignKey(
                        name: "FK_ft_token_statistics_ft_token_token_id",
                        column: x => x.token_id,
                        principalTable: "ft_token",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ft_banned_country",
                columns: table => new
                {
                    id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    code = table.Column<string>(maxLength: 3, nullable: false),
                    comment = table.Column<string>(maxLength: 512, nullable: false),
                    is_deleted = table.Column<bool>(nullable: false),
                    is_enabled = table.Column<bool>(nullable: false),
                    time_created = table.Column<DateTime>(nullable: false),
                    user_id = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ft_banned_country", x => x.id);
                    table.ForeignKey(
                        name: "FK_ft_banned_country_ft_user_user_id",
                        column: x => x.user_id,
                        principalTable: "ft_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ft_kyc_shuftipro_ticket",
                columns: table => new
                {
                    id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    callback_message = table.Column<string>(maxLength: 128, nullable: true),
                    callback_status_code = table.Column<string>(maxLength: 32, nullable: true),
                    country_code = table.Column<string>(maxLength: 2, nullable: false),
                    dob = table.Column<DateTime>(nullable: false),
                    first_name = table.Column<string>(maxLength: 64, nullable: false),
                    is_deleted = table.Column<bool>(nullable: false),
                    is_enabled = table.Column<bool>(nullable: false),
                    is_verified = table.Column<bool>(nullable: false),
                    last_name = table.Column<string>(maxLength: 64, nullable: false),
                    method = table.Column<string>(maxLength: 32, nullable: false),
                    phone_number = table.Column<string>(maxLength: 32, nullable: false),
                    reference_id = table.Column<string>(maxLength: 32, nullable: false),
                    time_created = table.Column<DateTime>(nullable: false),
                    time_responed = table.Column<DateTime>(nullable: true),
                    user_id = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ft_kyc_shuftipro_ticket", x => x.id);
                    table.ForeignKey(
                        name: "FK_ft_kyc_shuftipro_ticket_ft_user_user_id",
                        column: x => x.user_id,
                        principalTable: "ft_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ft_signed_document",
                columns: table => new
                {
                    id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    callback_event_type = table.Column<string>(maxLength: 64, nullable: true),
                    callback_status = table.Column<string>(maxLength: 16, nullable: true),
                    is_deleted = table.Column<bool>(nullable: false),
                    is_enabled = table.Column<bool>(nullable: false),
                    is_signed = table.Column<bool>(nullable: false),
                    reference_id = table.Column<string>(maxLength: 64, nullable: false),
                    secret = table.Column<string>(maxLength: 64, nullable: false),
                    time_completed = table.Column<DateTime>(nullable: true),
                    time_created = table.Column<DateTime>(nullable: false),
                    type = table.Column<int>(nullable: false),
                    user_id = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ft_signed_document", x => x.id);
                    table.ForeignKey(
                        name: "FK_ft_signed_document_ft_user_user_id",
                        column: x => x.user_id,
                        principalTable: "ft_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ft_user_activity",
                columns: table => new
                {
                    id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    agent = table.Column<string>(maxLength: 128, nullable: false),
                    comment = table.Column<string>(maxLength: 512, nullable: false),
                    ip = table.Column<string>(maxLength: 15, nullable: false),
                    is_deleted = table.Column<bool>(nullable: false),
                    is_enabled = table.Column<bool>(nullable: false),
                    locale = table.Column<int>(nullable: true),
                    time_created = table.Column<DateTime>(nullable: false),
                    type = table.Column<string>(maxLength: 32, nullable: false),
                    user_id = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ft_user_activity", x => x.id);
                    table.ForeignKey(
                        name: "FK_ft_user_activity_ft_user_user_id",
                        column: x => x.user_id,
                        principalTable: "ft_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ft_user_claim",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    claim_type = table.Column<string>(maxLength: 256, nullable: true),
                    claim_value = table.Column<string>(maxLength: 256, nullable: true),
                    user_id = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ft_user_claim", x => x.id);
                    table.ForeignKey(
                        name: "FK_ft_user_claim_ft_user_user_id",
                        column: x => x.user_id,
                        principalTable: "ft_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ft_user_limits",
                columns: table => new
                {
                    id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    eth_deposited = table.Column<decimal>(type: "decimal(38, 18)", nullable: false),
                    eth_withdrawn = table.Column<decimal>(type: "decimal(38, 18)", nullable: false),
                    fiat_deposited = table.Column<long>(nullable: false),
                    fiat_withdrawn = table.Column<long>(nullable: false),
                    is_deleted = table.Column<bool>(nullable: false),
                    is_enabled = table.Column<bool>(nullable: false),
                    time_created = table.Column<DateTime>(nullable: false),
                    user_id = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ft_user_limits", x => x.id);
                    table.ForeignKey(
                        name: "FK_ft_user_limits_ft_user_user_id",
                        column: x => x.user_id,
                        principalTable: "ft_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ft_user_login",
                columns: table => new
                {
                    login_provider = table.Column<string>(maxLength: 128, nullable: false),
                    provider_key = table.Column<string>(maxLength: 128, nullable: false),
                    provider_display_name = table.Column<string>(maxLength: 64, nullable: true),
                    user_id = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ft_user_login", x => new { x.login_provider, x.provider_key });
                    table.ForeignKey(
                        name: "FK_ft_user_login_ft_user_user_id",
                        column: x => x.user_id,
                        principalTable: "ft_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ft_user_oplog",
                columns: table => new
                {
                    id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    concurrency_stamp = table.Column<string>(maxLength: 64, nullable: true),
                    is_deleted = table.Column<bool>(nullable: false),
                    is_enabled = table.Column<bool>(nullable: false),
                    message = table.Column<string>(maxLength: 512, nullable: false),
                    ref_id = table.Column<long>(nullable: true),
                    status = table.Column<int>(nullable: false),
                    time_created = table.Column<DateTime>(nullable: false),
                    user_id = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ft_user_oplog", x => x.id);
                    table.ForeignKey(
                        name: "FK_ft_user_oplog_ft_user_oplog_ref_id",
                        column: x => x.ref_id,
                        principalTable: "ft_user_oplog",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ft_user_oplog_ft_user_user_id",
                        column: x => x.user_id,
                        principalTable: "ft_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ft_user_options",
                columns: table => new
                {
                    id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    init_tfa_quest = table.Column<bool>(nullable: false),
                    is_deleted = table.Column<bool>(nullable: false),
                    is_enabled = table.Column<bool>(nullable: false),
                    user_id = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ft_user_options", x => x.id);
                    table.ForeignKey(
                        name: "FK_ft_user_options_ft_user_user_id",
                        column: x => x.user_id,
                        principalTable: "ft_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ft_user_role",
                columns: table => new
                {
                    user_id = table.Column<long>(nullable: false),
                    role_id = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ft_user_role", x => new { x.user_id, x.role_id });
                    table.ForeignKey(
                        name: "FK_ft_user_role_ft_role_role_id",
                        column: x => x.role_id,
                        principalTable: "ft_role",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ft_user_role_ft_user_user_id",
                        column: x => x.user_id,
                        principalTable: "ft_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ft_user_token",
                columns: table => new
                {
                    user_id = table.Column<long>(nullable: false),
                    login_provider = table.Column<string>(maxLength: 64, nullable: false),
                    name = table.Column<string>(maxLength: 128, nullable: false),
                    value = table.Column<string>(maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ft_user_token", x => new { x.user_id, x.login_provider, x.name });
                    table.ForeignKey(
                        name: "FK_ft_user_token_ft_user_user_id",
                        column: x => x.user_id,
                        principalTable: "ft_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ft_user_verification",
                columns: table => new
                {
                    id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    tos_agreed = table.Column<bool>(nullable: true),
                    apartment = table.Column<string>(maxLength: 128, nullable: true),
                    city = table.Column<string>(maxLength: 256, nullable: true),
                    country = table.Column<string>(maxLength: 64, nullable: true),
                    country_code = table.Column<string>(maxLength: 2, nullable: true),
                    dob = table.Column<DateTime>(nullable: true),
                    first_name = table.Column<string>(maxLength: 64, nullable: true),
                    is_deleted = table.Column<bool>(nullable: false),
                    is_enabled = table.Column<bool>(nullable: false),
                    last_kyc_ticket_id = table.Column<long>(nullable: true),
                    last_name = table.Column<string>(maxLength: 64, nullable: true),
                    middle_name = table.Column<string>(maxLength: 64, nullable: true),
                    phone_number = table.Column<string>(maxLength: 32, nullable: true),
                    postal_code = table.Column<string>(maxLength: 16, nullable: true),
                    proved_residence = table.Column<bool>(nullable: true),
                    proved_residence_comment = table.Column<string>(maxLength: 512, nullable: true),
                    state = table.Column<string>(maxLength: 256, nullable: true),
                    street = table.Column<string>(maxLength: 256, nullable: true),
                    time_user_changed = table.Column<DateTime>(nullable: true),
                    user_id = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ft_user_verification", x => x.id);
                    table.ForeignKey(
                        name: "FK_ft_user_verification_ft_kyc_shuftipro_ticket_last_kyc_ticket_id",
                        column: x => x.last_kyc_ticket_id,
                        principalTable: "ft_kyc_shuftipro_ticket",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ft_user_verification_ft_user_user_id",
                        column: x => x.user_id,
                        principalTable: "ft_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ft_banned_country_user_id",
                table: "ft_banned_country",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_ft_kyc_shuftipro_ticket_user_id",
                table: "ft_kyc_shuftipro_ticket",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "ft_role",
                column: "normalized_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ft_role_claim_role_id",
                table: "ft_role_claim",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "IX_ft_signed_document_user_id",
                table: "ft_signed_document",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_ft_token_statistics_token_id",
                table: "ft_token_statistics",
                column: "token_id");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "ft_user",
                column: "normalized_email");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "ft_user",
                column: "normalized_username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ft_user_activity_user_id",
                table: "ft_user_activity",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_ft_user_claim_user_id",
                table: "ft_user_claim",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_ft_user_limits_user_id",
                table: "ft_user_limits",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_ft_user_login_user_id",
                table: "ft_user_login",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_ft_user_oplog_ref_id",
                table: "ft_user_oplog",
                column: "ref_id");

            migrationBuilder.CreateIndex(
                name: "IX_ft_user_oplog_user_id",
                table: "ft_user_oplog",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_ft_user_options_user_id",
                table: "ft_user_options",
                column: "user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ft_user_role_role_id",
                table: "ft_user_role",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "IX_ft_user_verification_last_kyc_ticket_id",
                table: "ft_user_verification",
                column: "last_kyc_ticket_id");

            migrationBuilder.CreateIndex(
                name: "IX_ft_user_verification_user_id",
                table: "ft_user_verification",
                column: "user_id",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ft_add_token_request");

            migrationBuilder.DropTable(
                name: "ft_banned_country");

            migrationBuilder.DropTable(
                name: "ft_mutex");

            migrationBuilder.DropTable(
                name: "ft_role_claim");

            migrationBuilder.DropTable(
                name: "ft_settings");

            migrationBuilder.DropTable(
                name: "ft_signed_document");

            migrationBuilder.DropTable(
                name: "ft_token_statistics");

            migrationBuilder.DropTable(
                name: "ft_user_activity");

            migrationBuilder.DropTable(
                name: "ft_user_claim");

            migrationBuilder.DropTable(
                name: "ft_user_limits");

            migrationBuilder.DropTable(
                name: "ft_user_login");

            migrationBuilder.DropTable(
                name: "ft_user_oplog");

            migrationBuilder.DropTable(
                name: "ft_user_options");

            migrationBuilder.DropTable(
                name: "ft_user_role");

            migrationBuilder.DropTable(
                name: "ft_user_token");

            migrationBuilder.DropTable(
                name: "ft_user_verification");

            migrationBuilder.DropTable(
                name: "ft_token");

            migrationBuilder.DropTable(
                name: "ft_role");

            migrationBuilder.DropTable(
                name: "ft_kyc_shuftipro_ticket");

            migrationBuilder.DropTable(
                name: "ft_user");
        }
    }
}
