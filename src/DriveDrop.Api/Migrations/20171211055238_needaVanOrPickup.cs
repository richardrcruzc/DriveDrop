using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace DriveDrop.Api.Migrations
{
    public partial class needaVanOrPickup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "NeedaVanOrPickup",
                schema: "shippings",
                table: "shipment",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NeedaVanOrPickup",
                schema: "shippings",
                table: "shipment");
        }
    }
}
