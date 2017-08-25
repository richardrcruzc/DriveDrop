using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DriveDrop.Api.Migrations
{
    public partial class paypalExtraData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PaymentNotes",
                schema: "shippings",
                table: "shipment",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentReceipt",
                schema: "shippings",
                table: "shipment",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentReceived",
                schema: "shippings",
                table: "shipment",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentReceivedDate",
                schema: "shippings",
                table: "shipment",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentNotes",
                schema: "shippings",
                table: "shipment");

            migrationBuilder.DropColumn(
                name: "PaymentReceipt",
                schema: "shippings",
                table: "shipment");

            migrationBuilder.DropColumn(
                name: "PaymentReceived",
                schema: "shippings",
                table: "shipment");

            migrationBuilder.DropColumn(
                name: "PaymentReceivedDate",
                schema: "shippings",
                table: "shipment");
        }
    }
}
