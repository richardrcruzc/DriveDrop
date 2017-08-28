using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DriveDrop.Api.Migrations
{
    public partial class Reviewed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ReviwingTo",
                table: "Reviews",
                newName: "Reviewed");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Reviewed",
                table: "Reviews",
                newName: "ReviwingTo");
        }
    }
}
