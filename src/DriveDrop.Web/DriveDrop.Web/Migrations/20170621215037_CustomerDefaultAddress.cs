using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DriveDrop.Web.Migrations
{
    public partial class CustomerDefaultAddress : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DefaultAddressId",
                schema: "shippings",
                table: "customer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_customer_DefaultAddressId",
                schema: "shippings",
                table: "customer",
                column: "DefaultAddressId");

            migrationBuilder.AddForeignKey(
                name: "FK_customer_address_DefaultAddressId",
                schema: "shippings",
                table: "customer",
                column: "DefaultAddressId",
                principalSchema: "shippings",
                principalTable: "address",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_customer_address_DefaultAddressId",
                schema: "shippings",
                table: "customer");

            migrationBuilder.DropIndex(
                name: "IX_customer_DefaultAddressId",
                schema: "shippings",
                table: "customer");

            migrationBuilder.DropColumn(
                name: "DefaultAddressId",
                schema: "shippings",
                table: "customer");
        }
    }
}
