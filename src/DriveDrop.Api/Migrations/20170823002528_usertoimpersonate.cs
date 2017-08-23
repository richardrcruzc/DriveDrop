using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DriveDrop.Api.Migrations
{
    public partial class usertoimpersonate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_customer_customerStatus_CustomerStatusId",
                schema: "shippings",
                table: "customer");

            migrationBuilder.DropForeignKey(
                name: "FK_customer_clientType_CustomerTypeId",
                schema: "shippings",
                table: "customer");

            migrationBuilder.AlterColumn<int>(
                name: "CustomerTypeId",
                schema: "shippings",
                table: "customer",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "CustomerStatusId",
                schema: "shippings",
                table: "customer",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<string>(
                name: "UserNameToImpersonate",
                schema: "shippings",
                table: "customer",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_customer_customerStatus_CustomerStatusId",
                schema: "shippings",
                table: "customer",
                column: "CustomerStatusId",
                principalSchema: "shippings",
                principalTable: "customerStatus",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_customer_clientType_CustomerTypeId",
                schema: "shippings",
                table: "customer",
                column: "CustomerTypeId",
                principalSchema: "shippings",
                principalTable: "clientType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_customer_customerStatus_CustomerStatusId",
                schema: "shippings",
                table: "customer");

            migrationBuilder.DropForeignKey(
                name: "FK_customer_clientType_CustomerTypeId",
                schema: "shippings",
                table: "customer");

            migrationBuilder.DropColumn(
                name: "UserNameToImpersonate",
                schema: "shippings",
                table: "customer");

            migrationBuilder.AlterColumn<int>(
                name: "CustomerTypeId",
                schema: "shippings",
                table: "customer",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CustomerStatusId",
                schema: "shippings",
                table: "customer",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_customer_customerStatus_CustomerStatusId",
                schema: "shippings",
                table: "customer",
                column: "CustomerStatusId",
                principalSchema: "shippings",
                principalTable: "customerStatus",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_customer_clientType_CustomerTypeId",
                schema: "shippings",
                table: "customer",
                column: "CustomerTypeId",
                principalSchema: "shippings",
                principalTable: "clientType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
