using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DriveDrop.Api.Migrations
{
    public partial class dropcommentpict : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DropComment",
                schema: "shippings",
                table: "shipment",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DropPictureUri",
                schema: "shippings",
                table: "shipment",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DropComment",
                schema: "shippings",
                table: "shipment");

            migrationBuilder.DropColumn(
                name: "DropPictureUri",
                schema: "shippings",
                table: "shipment");
        }
    }
}
