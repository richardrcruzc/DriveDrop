using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DriveDrop.Api.Migrations
{
    public partial class extrachargeAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "ExtraCharge",
                schema: "shippings",
                table: "shipment",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "ExtraChargeNote",
                schema: "shippings",
                table: "shipment",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExtraCharge",
                schema: "shippings",
                table: "shipment");

            migrationBuilder.DropColumn(
                name: "ExtraChargeNote",
                schema: "shippings",
                table: "shipment");
        }
    }
}
