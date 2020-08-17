using Microsoft.EntityFrameworkCore.Migrations;

namespace Microsoft.eShopOnContainers.Services.Marketing.API.Infrastructure.MarketingMigrations
{
    public partial class addedcampaigndetails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DetailsUri",
                table: "Campaign",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PictureName",
                table: "Campaign",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DetailsUri",
                table: "Campaign");

            migrationBuilder.DropColumn(
                name: "PictureName",
                table: "Campaign");
        }
    }
}
