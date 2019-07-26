using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Identity.API.Migrations.ConfigurationDb
{
    public partial class core3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                table: "IdentityResources",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "NonEditable",
                table: "IdentityResources",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "Updated",
                table: "IdentityResources",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "ClientSecrets",
                maxLength: 4000,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 2000);

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "ClientSecrets",
                maxLength: 250,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 250,
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                table: "ClientSecrets",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                table: "Clients",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "DeviceCodeLifetime",
                table: "Clients",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastAccessed",
                table: "Clients",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "NonEditable",
                table: "Clients",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "Updated",
                table: "Clients",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserCodeType",
                table: "Clients",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserSsoLifetime",
                table: "Clients",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "ApiSecrets",
                maxLength: 4000,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 2000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "ApiSecrets",
                maxLength: 250,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 250,
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                table: "ApiSecrets",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                table: "ApiResources",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastAccessed",
                table: "ApiResources",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "NonEditable",
                table: "ApiResources",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "Updated",
                table: "ApiResources",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ApiProperties",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Key = table.Column<string>(maxLength: 250, nullable: false),
                    Value = table.Column<string>(maxLength: 2000, nullable: false),
                    ApiResourceId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiProperties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApiProperties_ApiResources_ApiResourceId",
                        column: x => x.ApiResourceId,
                        principalTable: "ApiResources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IdentityProperties",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Key = table.Column<string>(maxLength: 250, nullable: false),
                    Value = table.Column<string>(maxLength: 2000, nullable: false),
                    IdentityResourceId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityProperties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IdentityProperties_IdentityResources_IdentityResourceId",
                        column: x => x.IdentityResourceId,
                        principalTable: "IdentityResources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApiProperties_ApiResourceId",
                table: "ApiProperties",
                column: "ApiResourceId");

            migrationBuilder.CreateIndex(
                name: "IX_IdentityProperties_IdentityResourceId",
                table: "IdentityProperties",
                column: "IdentityResourceId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApiProperties");

            migrationBuilder.DropTable(
                name: "IdentityProperties");

            migrationBuilder.DropColumn(
                name: "Created",
                table: "IdentityResources");

            migrationBuilder.DropColumn(
                name: "NonEditable",
                table: "IdentityResources");

            migrationBuilder.DropColumn(
                name: "Updated",
                table: "IdentityResources");

            migrationBuilder.DropColumn(
                name: "Created",
                table: "ClientSecrets");

            migrationBuilder.DropColumn(
                name: "Created",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "DeviceCodeLifetime",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "LastAccessed",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "NonEditable",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "Updated",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "UserCodeType",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "UserSsoLifetime",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "Created",
                table: "ApiSecrets");

            migrationBuilder.DropColumn(
                name: "Created",
                table: "ApiResources");

            migrationBuilder.DropColumn(
                name: "LastAccessed",
                table: "ApiResources");

            migrationBuilder.DropColumn(
                name: "NonEditable",
                table: "ApiResources");

            migrationBuilder.DropColumn(
                name: "Updated",
                table: "ApiResources");

            migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "ClientSecrets",
                maxLength: 2000,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 4000);

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "ClientSecrets",
                maxLength: 250,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 250);

            migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "ApiSecrets",
                maxLength: 2000,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 4000);

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "ApiSecrets",
                maxLength: 250,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 250);
        }
    }
}
