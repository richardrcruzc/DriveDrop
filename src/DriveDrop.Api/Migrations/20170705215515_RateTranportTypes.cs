using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace DriveDrop.Api.Migrations
{
    public partial class RateTranportTypes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                name: "IX_RateTranportTypes_RateId",
                table: "RateTranportTypes",
                column: "RateId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RateTranportTypes");
        }
    }
}
