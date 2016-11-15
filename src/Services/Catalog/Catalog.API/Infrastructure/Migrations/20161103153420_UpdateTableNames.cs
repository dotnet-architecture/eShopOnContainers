using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Catalog.API.Infrastructure.Migrations
{
    public partial class UpdateTableNames : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_catalog_catalogbrand_CatalogBrandId",
                table: "catalog");

            migrationBuilder.DropForeignKey(
                name: "FK_catalog_CatalogTypes_CatalogTypeId",
                table: "catalog");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CatalogTypes",
                table: "CatalogTypes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_catalog",
                table: "catalog");

            migrationBuilder.DropPrimaryKey(
                name: "PK_catalogbrand",
                table: "catalogbrand");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CatalogType",
                table: "CatalogTypes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Catalog",
                table: "catalog",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CatalogBrand",
                table: "catalogbrand",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Catalog_CatalogBrand_CatalogBrandId",
                table: "catalog",
                column: "CatalogBrandId",
                principalTable: "catalogbrand",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Catalog_CatalogType_CatalogTypeId",
                table: "catalog",
                column: "CatalogTypeId",
                principalTable: "CatalogTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.RenameIndex(
                name: "IX_catalog_CatalogTypeId",
                table: "catalog",
                newName: "IX_Catalog_CatalogTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_catalog_CatalogBrandId",
                table: "catalog",
                newName: "IX_Catalog_CatalogBrandId");

            migrationBuilder.RenameTable(
                name: "CatalogTypes",
                newName: "CatalogType");

            migrationBuilder.RenameTable(
                name: "catalog",
                newName: "Catalog");

            migrationBuilder.RenameTable(
                name: "catalogbrand",
                newName: "CatalogBrand");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Catalog_CatalogBrand_CatalogBrandId",
                table: "Catalog");

            migrationBuilder.DropForeignKey(
                name: "FK_Catalog_CatalogType_CatalogTypeId",
                table: "Catalog");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CatalogType",
                table: "CatalogType");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Catalog",
                table: "Catalog");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CatalogBrand",
                table: "CatalogBrand");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CatalogTypes",
                table: "CatalogType",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_catalog",
                table: "Catalog",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_catalogbrand",
                table: "CatalogBrand",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_catalog_catalogbrand_CatalogBrandId",
                table: "Catalog",
                column: "CatalogBrandId",
                principalTable: "CatalogBrand",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_catalog_CatalogTypes_CatalogTypeId",
                table: "Catalog",
                column: "CatalogTypeId",
                principalTable: "CatalogType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.RenameIndex(
                name: "IX_Catalog_CatalogTypeId",
                table: "Catalog",
                newName: "IX_catalog_CatalogTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_Catalog_CatalogBrandId",
                table: "Catalog",
                newName: "IX_catalog_CatalogBrandId");

            migrationBuilder.RenameTable(
                name: "CatalogType",
                newName: "CatalogTypes");

            migrationBuilder.RenameTable(
                name: "Catalog",
                newName: "catalog");

            migrationBuilder.RenameTable(
                name: "CatalogBrand",
                newName: "catalogbrand");
        }
    }
}
