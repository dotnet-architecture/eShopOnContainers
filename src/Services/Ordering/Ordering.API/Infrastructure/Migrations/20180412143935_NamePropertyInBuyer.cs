using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Ordering.API.Infrastructure.Migrations
{
    public partial class NamePropertyInBuyer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_orderItems_orders_OrderId",
                schema: "ordering",
                table: "orderItems");                       

            migrationBuilder.AddColumn<string>(
                name: "Name",
                schema: "ordering",
                table: "buyers",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_orderItems_orders_OrderId",
                schema: "ordering",
                table: "orderItems",
                column: "OrderId",
                principalSchema: "ordering",
                principalTable: "orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_orderItems_orders_OrderId",
                schema: "ordering",
                table: "orderItems");

            migrationBuilder.DropColumn(
                name: "Name",
                schema: "ordering",
                table: "buyers");

            migrationBuilder.AddColumn<string>(
                name: "City",
                schema: "ordering",
                table: "orders",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Country",
                schema: "ordering",
                table: "orders",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "State",
                schema: "ordering",
                table: "orders",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Street",
                schema: "ordering",
                table: "orders",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ZipCode",
                schema: "ordering",
                table: "orders",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_orderItems_orders_OrderId",
                schema: "ordering",
                table: "orderItems",
                column: "OrderId",
                principalSchema: "ordering",
                principalTable: "orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
