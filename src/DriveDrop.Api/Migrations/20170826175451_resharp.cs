using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace DriveDrop.Api.Migrations
{
    public partial class resharp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Values",
                table: "Reviews");

            migrationBuilder.CreateTable(
                name: "ReviewQuestion",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Description = table.Column<string>(nullable: true),
                    ReviewId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReviewQuestion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReviewQuestion_Reviews_ReviewId",
                        column: x => x.ReviewId,
                        principalTable: "Reviews",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ReviewDetails",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ReviewId = table.Column<int>(nullable: true),
                    ReviewQuestionId = table.Column<int>(nullable: true),
                    Values = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReviewDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReviewDetails_Reviews_ReviewId",
                        column: x => x.ReviewId,
                        principalTable: "Reviews",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReviewDetails_ReviewQuestion_ReviewQuestionId",
                        column: x => x.ReviewQuestionId,
                        principalTable: "ReviewQuestion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReviewDetails_ReviewId",
                table: "ReviewDetails",
                column: "ReviewId");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewDetails_ReviewQuestionId",
                table: "ReviewDetails",
                column: "ReviewQuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewQuestion_ReviewId",
                table: "ReviewQuestion",
                column: "ReviewId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReviewDetails");

            migrationBuilder.DropTable(
                name: "ReviewQuestion");

            migrationBuilder.AddColumn<string>(
                name: "Values",
                table: "Reviews",
                nullable: true);
        }
    }
}
