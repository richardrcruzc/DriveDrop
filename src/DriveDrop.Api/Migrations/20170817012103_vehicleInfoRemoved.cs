using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DriveDrop.Api.Migrations
{
    public partial class vehicleInfoRemoved : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_packageSizes_Rates_RateId",
                schema: "shippings",
                table: "packageSizes");

            migrationBuilder.DropIndex(
                name: "IX_packageSizes_RateId",
                schema: "shippings",
                table: "packageSizes");

            migrationBuilder.DropColumn(
                name: "RateId",
                schema: "shippings",
                table: "packageSizes");

            migrationBuilder.RenameColumn(
                name: "VehicleInfo",
                schema: "shippings",
                table: "customer",
                newName: "VehicleYear");

            migrationBuilder.AddColumn<string>(
                name: "VehicleColor",
                schema: "shippings",
                table: "customer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VehicleMake",
                schema: "shippings",
                table: "customer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VehicleModel",
                schema: "shippings",
                table: "customer",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VehicleColor",
                schema: "shippings",
                table: "customer");

            migrationBuilder.DropColumn(
                name: "VehicleMake",
                schema: "shippings",
                table: "customer");

            migrationBuilder.DropColumn(
                name: "VehicleModel",
                schema: "shippings",
                table: "customer");

            migrationBuilder.RenameColumn(
                name: "VehicleYear",
                schema: "shippings",
                table: "customer",
                newName: "VehicleInfo");

            migrationBuilder.AddColumn<int>(
                name: "RateId",
                schema: "shippings",
                table: "packageSizes",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_packageSizes_RateId",
                schema: "shippings",
                table: "packageSizes",
                column: "RateId");

            migrationBuilder.AddForeignKey(
                name: "FK_packageSizes_Rates_RateId",
                schema: "shippings",
                table: "packageSizes",
                column: "RateId",
                principalTable: "Rates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
