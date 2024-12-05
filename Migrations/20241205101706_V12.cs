using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accommodation_Room_Project_Offical.Migrations
{
    /// <inheritdoc />
    public partial class V12 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "ServicesBlogs",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ServicesBlogs_Slug",
                table: "ServicesBlogs",
                column: "Slug",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ServicesBlogs_Slug",
                table: "ServicesBlogs");

            migrationBuilder.DropColumn(
                name: "Slug",
                table: "ServicesBlogs");
        }
    }
}
