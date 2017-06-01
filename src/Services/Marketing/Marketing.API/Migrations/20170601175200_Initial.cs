using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Microsoft.eShopOnContainers.Services.Marketing.API.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence(
                name: "campaign_hilo",
                incrementBy: 10);

            migrationBuilder.CreateTable(
                name: "Campaign",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    Description = table.Column<string>(nullable: false),
                    From = table.Column<DateTime>(nullable: false),
                    To = table.Column<DateTime>(nullable: false),
                    Url = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Campaign", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Rules",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CampaignId = table.Column<int>(nullable: false),
                    Description = table.Column<string>(nullable: false),
                    RuleTypeId = table.Column<int>(nullable: false),
                    LocationId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rules_Campaign_CampaignId",
                        column: x => x.CampaignId,
                        principalTable: "Campaign",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Rules_CampaignId",
                table: "Rules",
                column: "CampaignId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Rules");

            migrationBuilder.DropTable(
                name: "Campaign");

            migrationBuilder.DropSequence(
                name: "campaign_hilo");
        }
    }
}
