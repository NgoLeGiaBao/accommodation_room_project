using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accommodation_Room_Project_Offical.Migrations
{
    /// <inheritdoc />
    public partial class V06 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ContentNewses",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    GeneralTitle = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    DetailTitle = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    GeneralDescription = table.Column<string>(type: "text", nullable: false),
                    DetailDescription = table.Column<string>(type: "text", nullable: false),
                    Slug = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: true),
                    Content = table.Column<string>(type: "text", nullable: false),
                    Published = table.Column<bool>(type: "boolean", nullable: false),
                    AuthorId = table.Column<string>(type: "text", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateUpdated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ImageId = table.Column<string>(type: "text", nullable: true),
                    KeyWord = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentNewses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContentNewses_AspNetUsers_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CategoryNewses",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    ParentCategoryId = table.Column<string>(type: "text", nullable: true),
                    Title = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Slug = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ContentNewsId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryNewses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CategoryNewses_CategoryNewses_ParentCategoryId",
                        column: x => x.ParentCategoryId,
                        principalTable: "CategoryNewses",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CategoryNewses_ContentNewses_ContentNewsId",
                        column: x => x.ContentNewsId,
                        principalTable: "ContentNewses",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ContentAndCategoryNewses",
                columns: table => new
                {
                    categoryNewsID = table.Column<string>(type: "text", nullable: false),
                    contentNewsID = table.Column<string>(type: "text", nullable: false),
                    CategoryNewsID = table.Column<string>(type: "text", nullable: false),
                    ContentID = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentAndCategoryNewses", x => new { x.categoryNewsID, x.contentNewsID });
                    table.ForeignKey(
                        name: "FK_ContentAndCategoryNewses_CategoryNewses_categoryNewsID",
                        column: x => x.categoryNewsID,
                        principalTable: "CategoryNewses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ContentAndCategoryNewses_ContentNewses_contentNewsID",
                        column: x => x.contentNewsID,
                        principalTable: "ContentNewses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CategoryNewses_ContentNewsId",
                table: "CategoryNewses",
                column: "ContentNewsId");

            migrationBuilder.CreateIndex(
                name: "IX_CategoryNewses_ParentCategoryId",
                table: "CategoryNewses",
                column: "ParentCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_CategoryNewses_Slug",
                table: "CategoryNewses",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ContentAndCategoryNewses_contentNewsID",
                table: "ContentAndCategoryNewses",
                column: "contentNewsID");

            migrationBuilder.CreateIndex(
                name: "IX_ContentNewses_AuthorId",
                table: "ContentNewses",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_ContentNewses_Slug",
                table: "ContentNewses",
                column: "Slug",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContentAndCategoryNewses");

            migrationBuilder.DropTable(
                name: "CategoryNewses");

            migrationBuilder.DropTable(
                name: "ContentNewses");
        }
    }
}
