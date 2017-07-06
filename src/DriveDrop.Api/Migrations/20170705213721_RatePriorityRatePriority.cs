using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DriveDrop.Api.Migrations
{
    public partial class RatePriorityRatePriority : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "PriorityId",
                table: "RatePriorities",
                nullable: false,
                oldClrType: typeof(decimal));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "PriorityId",
                table: "RatePriorities",
                nullable: false,
                oldClrType: typeof(int));
        }
    }
}
