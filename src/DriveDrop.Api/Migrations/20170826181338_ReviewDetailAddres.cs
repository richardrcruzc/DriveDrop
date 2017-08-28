using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DriveDrop.Api.Migrations
{
    public partial class ReviewDetailAddres : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReviewQuestions_Reviews_ReviewId",
                table: "ReviewQuestions");

            migrationBuilder.DropIndex(
                name: "IX_ReviewQuestions_ReviewId",
                table: "ReviewQuestions");

            migrationBuilder.DropColumn(
                name: "ReviewId",
                table: "ReviewQuestions");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ReviewId",
                table: "ReviewQuestions",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReviewQuestions_ReviewId",
                table: "ReviewQuestions",
                column: "ReviewId");

            migrationBuilder.AddForeignKey(
                name: "FK_ReviewQuestions_Reviews_ReviewId",
                table: "ReviewQuestions",
                column: "ReviewId",
                principalTable: "Reviews",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
