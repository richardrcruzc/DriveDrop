using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace DriveDrop.Api.Migrations
{
    public partial class updaterateSchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RateDetails_Rates_RateId",
                table: "RateDetails");

            migrationBuilder.DropTable(
                name: "RatePackageSizes");

            migrationBuilder.DropTable(
                name: "RateTranportTypes");

            migrationBuilder.DropIndex(
                name: "IX_RateDetails_RateId",
                table: "RateDetails");

            migrationBuilder.DropColumn(
                name: "RateId",
                table: "RateDetails");

            migrationBuilder.DropColumn(
                name: "Active",
                table: "Rates");

            migrationBuilder.DropColumn(
                name: "ChargePerItem",
                table: "Rates");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "Rates");

            migrationBuilder.DropColumn(
                name: "FixChargePerShipping",
                table: "Rates");

            migrationBuilder.DropColumn(
                name: "FixChargePercentage",
                table: "Rates");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "Rates");

            migrationBuilder.RenameColumn(
                name: "PriorityId",
                table: "RatePriorities",
                newName: "PriorityTypeId");

            migrationBuilder.RenameColumn(
                name: "Tax",
                table: "Rates",
                newName: "OverHead");

            migrationBuilder.AddColumn<int>(
                name: "PackageSizeId",
                table: "Rates",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RatePriorities_PriorityTypeId",
                table: "RatePriorities",
                column: "PriorityTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Rates_PackageSizeId",
                table: "Rates",
                column: "PackageSizeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Rates_packageSizes_PackageSizeId",
                table: "Rates",
                column: "PackageSizeId",
                principalSchema: "shippings",
                principalTable: "packageSizes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RatePriorities_priorityType_PriorityTypeId",
                table: "RatePriorities",
                column: "PriorityTypeId",
                principalSchema: "shippings",
                principalTable: "priorityType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rates_packageSizes_PackageSizeId",
                table: "Rates");

            migrationBuilder.DropForeignKey(
                name: "FK_RatePriorities_priorityType_PriorityTypeId",
                table: "RatePriorities");

            migrationBuilder.DropIndex(
                name: "IX_RatePriorities_PriorityTypeId",
                table: "RatePriorities");

            migrationBuilder.DropIndex(
                name: "IX_Rates_PackageSizeId",
                table: "Rates");

            migrationBuilder.DropColumn(
                name: "PackageSizeId",
                table: "Rates");

            migrationBuilder.RenameColumn(
                name: "PriorityTypeId",
                table: "RatePriorities",
                newName: "PriorityId");

            migrationBuilder.RenameColumn(
                name: "OverHead",
                table: "Rates",
                newName: "Tax");

            migrationBuilder.AddColumn<int>(
                name: "RateId",
                table: "RateDetails",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "Active",
                table: "Rates",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "ChargePerItem",
                table: "Rates",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "Rates",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<decimal>(
                name: "FixChargePerShipping",
                table: "Rates",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "FixChargePercentage",
                table: "Rates",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "Rates",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

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

            migrationBuilder.CreateTable(
                name: "RateTranportTypes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Charge = table.Column<decimal>(nullable: false),
                    ChargePercentage = table.Column<bool>(nullable: false),
                    RateId = table.Column<int>(nullable: false),
                    TranportTypeId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RateTranportTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RateTranportTypes_Rates_RateId",
                        column: x => x.RateId,
                        principalTable: "Rates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RateDetails_RateId",
                table: "RateDetails",
                column: "RateId");

            migrationBuilder.CreateIndex(
                name: "IX_RatePackageSizes_RateId",
                table: "RatePackageSizes",
                column: "RateId");

            migrationBuilder.CreateIndex(
                name: "IX_RateTranportTypes_RateId",
                table: "RateTranportTypes",
                column: "RateId");

            migrationBuilder.AddForeignKey(
                name: "FK_RateDetails_Rates_RateId",
                table: "RateDetails",
                column: "RateId",
                principalTable: "Rates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
