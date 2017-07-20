using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DriveDrop.Api.Migrations
{
    public partial class packagesizeAddes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PackageSizeId",
                schema: "shippings",
                table: "shipment",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "packageSizes",
                schema: "shippings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false, defaultValue: 1),
                    Name = table.Column<string>(maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_packageSizes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_shipment_PackageSizeId",
                schema: "shippings",
                table: "shipment",
                column: "PackageSizeId");

            migrationBuilder.AddForeignKey(
                name: "FK_shipment_packageSizes_PackageSizeId",
                schema: "shippings",
                table: "shipment",
                column: "PackageSizeId",
                principalSchema: "shippings",
                principalTable: "packageSizes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_shipment_packageSizes_PackageSizeId",
                schema: "shippings",
                table: "shipment");

            migrationBuilder.DropTable(
                name: "packageSizes",
                schema: "shippings");

            migrationBuilder.DropIndex(
                name: "IX_shipment_PackageSizeId",
                schema: "shippings",
                table: "shipment");

            migrationBuilder.DropColumn(
                name: "PackageSizeId",
                schema: "shippings",
                table: "shipment");
        }
    }
}
