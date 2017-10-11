using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DriveDrop.Api.Migrations
{
    public partial class history : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PackageStatusHistories_shipment_ShipmentId",
                table: "PackageStatusHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_PackageStatusHistories_shippingStatus_ShippingStatusId",
                table: "PackageStatusHistories");

            migrationBuilder.DropIndex(
                name: "IX_PackageStatusHistories_ShippingStatusId",
                table: "PackageStatusHistories");

            migrationBuilder.DropColumn(
                name: "ShippingStatusId",
                table: "PackageStatusHistories");

            migrationBuilder.AddForeignKey(
                name: "FK_PackageStatusHistories_shipment_ShipmentId",
                table: "PackageStatusHistories",
                column: "ShipmentId",
                principalSchema: "shippings",
                principalTable: "shipment",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PackageStatusHistories_shippingStatus_ShipmentId",
                table: "PackageStatusHistories",
                column: "ShipmentId",
                principalSchema: "shippings",
                principalTable: "shippingStatus",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PackageStatusHistories_shipment_ShipmentId",
                table: "PackageStatusHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_PackageStatusHistories_shippingStatus_ShipmentId",
                table: "PackageStatusHistories");

            migrationBuilder.AddColumn<int>(
                name: "ShippingStatusId",
                table: "PackageStatusHistories",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PackageStatusHistories_ShippingStatusId",
                table: "PackageStatusHistories",
                column: "ShippingStatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_PackageStatusHistories_shipment_ShipmentId",
                table: "PackageStatusHistories",
                column: "ShipmentId",
                principalSchema: "shippings",
                principalTable: "shipment",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PackageStatusHistories_shippingStatus_ShippingStatusId",
                table: "PackageStatusHistories",
                column: "ShippingStatusId",
                principalSchema: "shippings",
                principalTable: "shippingStatus",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
