using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using System.Text;

namespace Microsoft.eShopOnContainers.Services.Locations.API.Infrastructure.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence(
                name: "frontier_seq",
                incrementBy: 10);

            migrationBuilder.CreateSequence(
                name: "locations_seq",
                incrementBy: 10);

            migrationBuilder.CreateSequence(
                name: "UserLocation_seq",
                incrementBy: 10);

            migrationBuilder.CreateTable(
                name: "Locations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    LocationCode = table.Column<string>(maxLength: 15, nullable: false),
                    Description = table.Column<string>(maxLength: 100, nullable: true),
                    Latitude = table.Column<double>(nullable: false),
                    Longitude = table.Column<double>(nullable: false),
                    ParentId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Locations_Locations_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FrontierPoints",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    Latitude = table.Column<double>(nullable: false),
                    LocationId = table.Column<int>(nullable: false),
                    Longitude = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FrontierPoints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FrontierPoints_Locations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserLocation",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    LocationId = table.Column<int>(nullable: true),
                    UserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLocation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserLocation_Locations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FrontierPoints_LocationId",
                table: "FrontierPoints",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Locations_ParentId",
                table: "Locations",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_UserLocation_LocationId",
                table: "UserLocation",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_UserLocation_UserId",
                table: "UserLocation",
                column: "UserId",
                unique: true);

            migrationBuilder.Sql(CreateGetDistanceFunction());
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FrontierPoints");

            migrationBuilder.DropTable(
                name: "UserLocation");

            migrationBuilder.DropTable(
                name: "Locations");

            migrationBuilder.DropSequence(
                name: "frontier_seq");

            migrationBuilder.DropSequence(
                name: "locations_seq");

            migrationBuilder.DropSequence(
                name: "UserLocation_seq");

            migrationBuilder.Sql(@"DROP FUNCTION IF EXISTS dbo.GetDistanceFromLocation");
        }

        private string CreateGetDistanceFunction()
        {
            var sb = new StringBuilder();
            sb.AppendLine(@"CREATE FUNCTION [dbo].[GetDistanceFromLocation](");
            sb.AppendLine(@"@CurrentLatitude float,");
            sb.AppendLine(@"@CurrentLongitude float,");
            sb.AppendLine(@"@latitude float,");
            sb.AppendLine(@"@longitude float)");
            sb.AppendLine(@"RETURNS int");
            sb.AppendLine(@"AS");
            sb.AppendLine(@"BEGIN");
            sb.AppendLine(@"DECLARE @geo1 geography = geography::Point(@CurrentLatitude, @CurrentLongitude, 4268),@geo2 geography = geography::Point(@latitude, @longitude, 4268)");
            sb.AppendLine(@"DECLARE @distance int");
            sb.AppendLine(@"SELECT @distance = @geo1.STDistance(@geo2)");
            sb.AppendLine(@"RETURN @distance");
            sb.AppendLine(@"END");
            return sb.ToString();
        }

    }
}
