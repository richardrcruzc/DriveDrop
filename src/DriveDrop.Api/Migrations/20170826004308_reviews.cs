using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace DriveDrop.Api.Migrations
{
    public partial class reviews : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Reviews",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Comment = table.Column<string>(nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    DriverId = table.Column<int>(nullable: true),
                    Published = table.Column<bool>(nullable: false),
                    ReviwingTo = table.Column<string>(nullable: true),
                    SenderId = table.Column<int>(nullable: true),
                    ShippingId = table.Column<int>(nullable: true),
                    Values = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reviews_customer_DriverId",
                        column: x => x.DriverId,
                        principalSchema: "shippings",
                        principalTable: "customer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Reviews_customer_SenderId",
                        column: x => x.SenderId,
                        principalSchema: "shippings",
                        principalTable: "customer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Reviews_shipment_ShippingId",
                        column: x => x.ShippingId,
                        principalSchema: "shippings",
                        principalTable: "shipment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_DriverId",
                table: "Reviews",
                column: "DriverId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_SenderId",
                table: "Reviews",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_ShippingId",
                table: "Reviews",
                column: "ShippingId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Reviews");
        }
    }
}
