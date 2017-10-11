using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace DriveDrop.Api.Migrations
{
    public partial class Statushistory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PackageStatusHistories",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DriverId = table.Column<int>(nullable: false),
                    ShipmentId = table.Column<int>(nullable: false),
                    ShippingStatusId = table.Column<int>(nullable: true),
                    StatusDate = table.Column<DateTime>(nullable: false),
                    StatusId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PackageStatusHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PackageStatusHistories_shipment_ShipmentId",
                        column: x => x.ShipmentId,
                        principalSchema: "shippings",
                        principalTable: "shipment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PackageStatusHistories_shippingStatus_ShippingStatusId",
                        column: x => x.ShippingStatusId,
                        principalSchema: "shippings",
                        principalTable: "shippingStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PackageStatusHistories_ShipmentId",
                table: "PackageStatusHistories",
                column: "ShipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_PackageStatusHistories_ShippingStatusId",
                table: "PackageStatusHistories",
                column: "ShippingStatusId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PackageStatusHistories");
        }
    }
}
