using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Microsoft.eShopOnContainers.Services.Ordering.API.Infrastructure.Migrations
{
    public partial class RefactorBuyerWithIdentityGuid : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FullName",
                schema: "ordering",
                table: "buyers",
                newName: "IdentityGuid");

            migrationBuilder.RenameIndex(
                name: "IX_buyers_FullName",
                schema: "ordering",
                table: "buyers",
                newName: "IX_buyers_IdentityGuid");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IdentityGuid",
                schema: "ordering",
                table: "buyers",
                newName: "FullName");

            migrationBuilder.RenameIndex(
                name: "IX_buyers_IdentityGuid",
                schema: "ordering",
                table: "buyers",
                newName: "IX_buyers_FullName");
        }
    }
}
