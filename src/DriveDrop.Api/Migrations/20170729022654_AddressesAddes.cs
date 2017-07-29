using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DriveDrop.Api.Migrations
{
    public partial class AddressesAddes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CustomerId",
                schema: "shippings",
                table: "address",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_address_CustomerId",
                schema: "shippings",
                table: "address",
                column: "CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_address_customer_CustomerId",
                schema: "shippings",
                table: "address",
                column: "CustomerId",
                principalSchema: "shippings",
                principalTable: "customer",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_address_customer_CustomerId",
                schema: "shippings",
                table: "address");

            migrationBuilder.DropIndex(
                name: "IX_address_CustomerId",
                schema: "shippings",
                table: "address");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                schema: "shippings",
                table: "address");
        }
    }
}
