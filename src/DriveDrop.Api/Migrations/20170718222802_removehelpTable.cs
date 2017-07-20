using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace DriveDrop.Api.Migrations
{
    public partial class removehelpTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payments_HelperTable_CardTypeId",
                table: "Payments");

            migrationBuilder.DropTable(
                name: "HelperTable");

            migrationBuilder.RenameColumn(
                name: "vehicleInfo",
                schema: "shippings",
                table: "customer",
                newName: "VehicleInfo");

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_cardtypes_CardTypeId",
                table: "Payments",
                column: "CardTypeId",
                principalSchema: "shippings",
                principalTable: "cardtypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payments_cardtypes_CardTypeId",
                table: "Payments");

            migrationBuilder.RenameColumn(
                name: "VehicleInfo",
                schema: "shippings",
                table: "customer",
                newName: "vehicleInfo");

            migrationBuilder.CreateTable(
                name: "HelperTable",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    HelperType = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HelperTable", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_HelperTable_CardTypeId",
                table: "Payments",
                column: "CardTypeId",
                principalTable: "HelperTable",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
