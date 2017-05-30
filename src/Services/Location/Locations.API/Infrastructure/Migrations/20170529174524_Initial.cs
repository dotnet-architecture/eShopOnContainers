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
                name: "frontier_hilo",
                incrementBy: 10);

            migrationBuilder.CreateSequence(
                name: "locations_hilo",
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

            migrationBuilder.CreateIndex(
                name: "IX_FrontierPoints_LocationId",
                table: "FrontierPoints",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Locations_ParentId",
                table: "Locations",
                column: "ParentId");

            migrationBuilder.Sql(CreateGetDistanceFunction());
            migrationBuilder.Sql(CreateLocationsNearSP());
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FrontierPoints");

            migrationBuilder.DropTable(
                name: "Locations");

            migrationBuilder.DropSequence(
                name: "frontier_hilo");

            migrationBuilder.DropSequence(
                name: "locations_hilo");

            migrationBuilder.Sql(@"DROP PROCEDURE IF EXISTS pLocationsNear");
            migrationBuilder.Sql(@"DROP FUNCTION IF EXISTS dbo.GetDistanceFromLocation");
        }

        private string CreateLocationsNearSP()
        {
            var sb = new StringBuilder();
            sb.AppendLine(@"CREATE PROCEDURE [dbo].[pLocationsNear]");
            sb.AppendLine(@"@latitude float,");
            sb.AppendLine(@"@longitude float,");
            sb.AppendLine(@"@size int = 500");
            sb.AppendLine(@"AS");
            sb.AppendLine(@"BEGIN");
            sb.AppendLine(@"SELECT TOP( @size) location.*");
            sb.AppendLine(@"FROM [dbo].[Locations] AS location");
            sb.AppendLine(@"ORDER BY dbo.[GetDistanceFromLocation](location.Latitude, location.Longitude, @latitude, @longitude)");
            sb.AppendLine(@"END");
            return sb.ToString();
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
