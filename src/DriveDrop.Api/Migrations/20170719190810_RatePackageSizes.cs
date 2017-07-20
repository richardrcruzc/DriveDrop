using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace DriveDrop.Api.Migrations
{
    public partial class RatePackageSizes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RateId",
                schema: "shippings",
                table: "packageSizes",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "RatePackageSizes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Charge = table.Column<decimal>(nullable: false),
                    ChargePercentage = table.Column<bool>(nullable: false),
                    PackageSizeId = table.Column<int>(nullable: false),
                    RateId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RatePackageSizes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RatePackageSizes_Rates_RateId",
                        column: x => x.RateId,
                        principalTable: "Rates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_packageSizes_RateId",
                schema: "shippings",
                table: "packageSizes",
                column: "RateId");

            migrationBuilder.CreateIndex(
                name: "IX_RatePackageSizes_RateId",
                table: "RatePackageSizes",
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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_packageSizes_Rates_RateId",
                schema: "shippings",
                table: "packageSizes");

            migrationBuilder.DropTable(
                name: "RatePackageSizes");

            migrationBuilder.DropIndex(
                name: "IX_packageSizes_RateId",
                schema: "shippings",
                table: "packageSizes");

            migrationBuilder.DropColumn(
                name: "RateId",
                schema: "shippings",
                table: "packageSizes");
        }
    }
}
