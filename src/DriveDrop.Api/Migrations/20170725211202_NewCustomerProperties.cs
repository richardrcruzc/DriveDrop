using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DriveDrop.Api.Migrations
{
    public partial class NewCustomerProperties : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PaymentMethodId",
                schema: "shippings",
                table: "customer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PersonalPhotoUri",
                schema: "shippings",
                table: "customer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PrimaryPhone",
                schema: "shippings",
                table: "customer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VehiclePhotoUri",
                schema: "shippings",
                table: "customer",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentMethodId",
                schema: "shippings",
                table: "customer");

            migrationBuilder.DropColumn(
                name: "PersonalPhotoUri",
                schema: "shippings",
                table: "customer");

            migrationBuilder.DropColumn(
                name: "PrimaryPhone",
                schema: "shippings",
                table: "customer");

            migrationBuilder.DropColumn(
                name: "VehiclePhotoUri",
                schema: "shippings",
                table: "customer");
        }
    }
}
