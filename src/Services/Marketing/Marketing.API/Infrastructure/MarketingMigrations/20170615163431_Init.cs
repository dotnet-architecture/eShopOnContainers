using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Microsoft.eShopOnContainers.Services.Marketing.API.Infrastructure.MarketingMigrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence(
                name: "campaign_hilo",
                incrementBy: 10);

            migrationBuilder.CreateSequence(
                name: "rule_hilo",
                incrementBy: 10);

            migrationBuilder.CreateTable(
                name: "Campaign",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    Description = table.Column<string>(nullable: false),
                    From = table.Column<DateTime>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    PictureUri = table.Column<string>(nullable: false),
                    To = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Campaign", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Rule",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    CampaignId = table.Column<int>(nullable: false),
                    Description = table.Column<string>(nullable: false),
                    RuleTypeId = table.Column<int>(nullable: false),
                    LocationId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rule", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rule_Campaign_CampaignId",
                        column: x => x.CampaignId,
                        principalTable: "Campaign",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Rule_CampaignId",
                table: "Rule",
                column: "CampaignId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Rule");

            migrationBuilder.DropTable(
                name: "Campaign");

            migrationBuilder.DropSequence(
                name: "campaign_hilo");

            migrationBuilder.DropSequence(
                name: "rule_hilo");
        }
    }
}
