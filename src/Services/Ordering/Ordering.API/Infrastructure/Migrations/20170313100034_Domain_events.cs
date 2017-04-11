using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Ordering.API.Migrations
{
    public partial class Domain_events : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_orders_buyers_BuyerId",
                schema: "ordering",
                table: "orders");

            migrationBuilder.AlterColumn<int>(
                name: "PaymentMethodId",
                schema: "ordering",
                table: "orders",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "BuyerId",
                schema: "ordering",
                table: "orders",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_orders_buyers_BuyerId",
                schema: "ordering",
                table: "orders",
                column: "BuyerId",
                principalSchema: "ordering",
                principalTable: "buyers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_orders_buyers_BuyerId",
                schema: "ordering",
                table: "orders");

            migrationBuilder.AlterColumn<int>(
                name: "PaymentMethodId",
                schema: "ordering",
                table: "orders",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "BuyerId",
                schema: "ordering",
                table: "orders",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_orders_buyers_BuyerId",
                schema: "ordering",
                table: "orders",
                column: "BuyerId",
                principalSchema: "ordering",
                principalTable: "buyers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
