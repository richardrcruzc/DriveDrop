using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DriveDrop.Api.Migrations
{
    public partial class ReviewQuestionAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReviewDetails_ReviewQuestion_ReviewQuestionId",
                table: "ReviewDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_ReviewQuestion_Reviews_ReviewId",
                table: "ReviewQuestion");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ReviewQuestion",
                table: "ReviewQuestion");

            migrationBuilder.RenameTable(
                name: "ReviewQuestion",
                newName: "ReviewQuestions");

            migrationBuilder.RenameIndex(
                name: "IX_ReviewQuestion_ReviewId",
                table: "ReviewQuestions",
                newName: "IX_ReviewQuestions_ReviewId");

            migrationBuilder.AddColumn<string>(
                name: "Group",
                table: "ReviewQuestions",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ReviewQuestions",
                table: "ReviewQuestions",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ReviewDetails_ReviewQuestions_ReviewQuestionId",
                table: "ReviewDetails",
                column: "ReviewQuestionId",
                principalTable: "ReviewQuestions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ReviewQuestions_Reviews_ReviewId",
                table: "ReviewQuestions",
                column: "ReviewId",
                principalTable: "Reviews",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReviewDetails_ReviewQuestions_ReviewQuestionId",
                table: "ReviewDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_ReviewQuestions_Reviews_ReviewId",
                table: "ReviewQuestions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ReviewQuestions",
                table: "ReviewQuestions");

            migrationBuilder.DropColumn(
                name: "Group",
                table: "ReviewQuestions");

            migrationBuilder.RenameTable(
                name: "ReviewQuestions",
                newName: "ReviewQuestion");

            migrationBuilder.RenameIndex(
                name: "IX_ReviewQuestions_ReviewId",
                table: "ReviewQuestion",
                newName: "IX_ReviewQuestion_ReviewId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ReviewQuestion",
                table: "ReviewQuestion",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ReviewDetails_ReviewQuestion_ReviewQuestionId",
                table: "ReviewDetails",
                column: "ReviewQuestionId",
                principalTable: "ReviewQuestion",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ReviewQuestion_Reviews_ReviewId",
                table: "ReviewQuestion",
                column: "ReviewId",
                principalTable: "Reviews",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
