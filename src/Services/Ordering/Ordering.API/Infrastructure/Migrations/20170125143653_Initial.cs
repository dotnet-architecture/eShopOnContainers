using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Ordering.API.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "ordering");

            migrationBuilder.CreateSequence(
                name: "orderitemseq",
                incrementBy: 10);

            migrationBuilder.CreateSequence(
                name: "buyerseq",
                schema: "ordering",
                incrementBy: 10);

            migrationBuilder.CreateSequence(
                name: "orderseq",
                schema: "ordering",
                incrementBy: 10);

            migrationBuilder.CreateSequence(
                name: "paymentseq",
                schema: "ordering",
                incrementBy: 10);

            migrationBuilder.CreateTable(
                name: "buyers",
                schema: "ordering",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    FullName = table.Column<string>(maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_buyers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "cardtypes",
                schema: "ordering",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false, defaultValue: 1),
                    Name = table.Column<string>(maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cardtypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "orderstatus",
                schema: "ordering",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false, defaultValue: 1),
                    Name = table.Column<string>(maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_orderstatus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "payments",
                schema: "ordering",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    Alias = table.Column<string>(maxLength: 200, nullable: false),
                    BuyerId = table.Column<int>(nullable: false),
                    CardHolderName = table.Column<string>(maxLength: 200, nullable: false),
                    CardNumber = table.Column<string>(maxLength: 25, nullable: false),
                    CardTypeId = table.Column<int>(nullable: false),
                    Expiration = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_payments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_payments_buyers_BuyerId",
                        column: x => x.BuyerId,
                        principalSchema: "ordering",
                        principalTable: "buyers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_payments_cardtypes_CardTypeId",
                        column: x => x.CardTypeId,
                        principalSchema: "ordering",
                        principalTable: "cardtypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "orders",
                schema: "ordering",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    BuyerId = table.Column<int>(nullable: false),
                    City = table.Column<string>(nullable: false),
                    OrderDate = table.Column<DateTime>(nullable: false),
                    OrderStatusId = table.Column<int>(nullable: false),
                    PaymentId = table.Column<int>(nullable: false),
                    State = table.Column<string>(nullable: false),
                    Street = table.Column<string>(nullable: false),
                    ZipCode = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_orders_buyers_BuyerId",
                        column: x => x.BuyerId,
                        principalSchema: "ordering",
                        principalTable: "buyers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_orders_orderstatus_OrderStatusId",
                        column: x => x.OrderStatusId,
                        principalSchema: "ordering",
                        principalTable: "orderstatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_orders_payments_PaymentId",
                        column: x => x.PaymentId,
                        principalSchema: "ordering",
                        principalTable: "payments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "orderItems",
                schema: "ordering",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    Discount = table.Column<decimal>(nullable: false),
                    OrderId = table.Column<int>(nullable: false),
                    ProductId = table.Column<int>(nullable: false),
                    ProductName = table.Column<string>(nullable: false),
                    UnitPrice = table.Column<decimal>(nullable: false),
                    Units = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_orderItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_orderItems_orders_OrderId",
                        column: x => x.OrderId,
                        principalSchema: "ordering",
                        principalTable: "orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_buyers_FullName",
                schema: "ordering",
                table: "buyers",
                column: "FullName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_payments_BuyerId",
                schema: "ordering",
                table: "payments",
                column: "BuyerId");

            migrationBuilder.CreateIndex(
                name: "IX_payments_CardTypeId",
                schema: "ordering",
                table: "payments",
                column: "CardTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_orders_BuyerId",
                schema: "ordering",
                table: "orders",
                column: "BuyerId");

            migrationBuilder.CreateIndex(
                name: "IX_orders_OrderStatusId",
                schema: "ordering",
                table: "orders",
                column: "OrderStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_orders_PaymentId",
                schema: "ordering",
                table: "orders",
                column: "PaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_orderItems_OrderId",
                schema: "ordering",
                table: "orderItems",
                column: "OrderId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "orderItems",
                schema: "ordering");

            migrationBuilder.DropTable(
                name: "orders",
                schema: "ordering");

            migrationBuilder.DropTable(
                name: "orderstatus",
                schema: "ordering");

            migrationBuilder.DropTable(
                name: "payments",
                schema: "ordering");

            migrationBuilder.DropTable(
                name: "buyers",
                schema: "ordering");

            migrationBuilder.DropTable(
                name: "cardtypes",
                schema: "ordering");

            migrationBuilder.DropSequence(
                name: "orderitemseq");

            migrationBuilder.DropSequence(
                name: "buyerseq",
                schema: "ordering");

            migrationBuilder.DropSequence(
                name: "orderseq",
                schema: "ordering");

            migrationBuilder.DropSequence(
                name: "paymentseq",
                schema: "ordering");
        }
    }
}
