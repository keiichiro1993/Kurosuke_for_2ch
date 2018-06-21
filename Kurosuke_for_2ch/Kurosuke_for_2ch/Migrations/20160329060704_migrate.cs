using System;
using System.Collections.Generic;
using Microsoft.Data.Entity.Migrations;

namespace Kurosuke_for_2ch.Migrations
{
    public partial class migrate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Category",
                columns: table => new
                {
                    CategoryId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Category", x => x.CategoryId);
                });
            migrationBuilder.CreateTable(
                name: "Ita",
                columns: table => new
                {
                    ItaId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CategoryId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Url = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ita", x => x.ItaId);
                    table.ForeignKey(
                        name: "FK_Ita_Category_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Category",
                        principalColumn: "CategoryId",
                        onDelete: ReferentialAction.Cascade);
                });
            migrationBuilder.CreateTable(
                name: "Thread",
                columns: table => new
                {
                    ThreadId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Count = table.Column<int>(nullable: false),
                    CurrentOffset = table.Column<float>(nullable: false),
                    ItaId = table.Column<int>(nullable: false),
                    ItaNumber = table.Column<int>(nullable: false),
                    MidokuCount = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Url = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Thread", x => x.ThreadId);
                    table.ForeignKey(
                        name: "FK_Thread_Ita_ItaId",
                        column: x => x.ItaId,
                        principalTable: "Ita",
                        principalColumn: "ItaId",
                        onDelete: ReferentialAction.Cascade);
                });
            migrationBuilder.CreateTable(
                name: "Post",
                columns: table => new
                {
                    PostId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DataDate = table.Column<string>(nullable: true),
                    DataId = table.Column<int>(nullable: true),
                    DataUserId = table.Column<string>(nullable: true),
                    Date = table.Column<string>(nullable: true),
                    Id = table.Column<int>(nullable: true),
                    MailTo = table.Column<string>(nullable: true),
                    Message = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Number = table.Column<string>(nullable: true),
                    ThreadId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Post", x => x.PostId);
                    table.ForeignKey(
                        name: "FK_Post_Thread_ThreadId",
                        column: x => x.ThreadId,
                        principalTable: "Thread",
                        principalColumn: "ThreadId",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable("Post");
            migrationBuilder.DropTable("Thread");
            migrationBuilder.DropTable("Ita");
            migrationBuilder.DropTable("Category");
        }
    }
}
