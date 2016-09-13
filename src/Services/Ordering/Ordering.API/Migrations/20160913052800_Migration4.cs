using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Ordering.API.Migrations
{
    public partial class Migration4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "shared");

            migrationBuilder.CreateSequence<int>(
                name: "OrderSequences",
                schema: "shared",
                startValue: 1001L);

            migrationBuilder.AddColumn<int>(
                name: "SequenceNumber",
                table: "Orders",
                nullable: false,
                defaultValueSql: "NEXT VALUE FOR shared.OrderSequences");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropSequence(
                name: "OrderSequences",
                schema: "shared");

            migrationBuilder.DropColumn(
                name: "SequenceNumber",
                table: "Orders");
        }
    }
}
