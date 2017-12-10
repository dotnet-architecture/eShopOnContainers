using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Ordering.API.Migrations
{
    public partial class AdressAsValueObject : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_orders_address_AddressId",
                schema: "ordering",
                table: "orders");

            migrationBuilder.DropForeignKey(
                name: "FK_orders_paymentmethods_PaymentMethodId",
                schema: "ordering",
                table: "orders");

            migrationBuilder.DropTable(
                name: "address",
                schema: "ordering");

            migrationBuilder.DropIndex(
                name: "IX_orders_AddressId",
                schema: "ordering",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "AddressId",
                schema: "ordering",
                table: "orders");

            migrationBuilder.AddColumn<string>(
                name: "Address_City",
                schema: "ordering",
                table: "orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Address_Country",
                schema: "ordering",
                table: "orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Address_State",
                schema: "ordering",
                table: "orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Address_Street",
                schema: "ordering",
                table: "orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Address_ZipCode",
                schema: "ordering",
                table: "orders",
                type: "nvarchar(max)",
                nullable: true);

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_orders_paymentmethods_PaymentMethodId",
                schema: "ordering",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "Address_City",
                schema: "ordering",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "Address_Country",
                schema: "ordering",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "Address_State",
                schema: "ordering",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "Address_Street",
                schema: "ordering",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "Address_ZipCode",
                schema: "ordering",
                table: "orders");

            migrationBuilder.AddColumn<int>(
                name: "AddressId",
                schema: "ordering",
                table: "orders",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "address",
                schema: "ordering",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    City = table.Column<string>(nullable: true),
                    Country = table.Column<string>(nullable: true),
                    State = table.Column<string>(nullable: true),
                    Street = table.Column<string>(nullable: true),
                    ZipCode = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_address", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_orders_AddressId",
                schema: "ordering",
                table: "orders",
                column: "AddressId");

            migrationBuilder.AddForeignKey(
                name: "FK_orders_address_AddressId",
                schema: "ordering",
                table: "orders",
                column: "AddressId",
                principalSchema: "ordering",
                principalTable: "address",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
