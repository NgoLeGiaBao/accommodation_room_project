using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accommodation_Room_Project_Offical.Migrations
{
    /// <inheritdoc />
    public partial class V01 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CategoryNotification",
                columns: table => new
                {
                    CategoryNoID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryNotification", x => x.CategoryNoID);
                });

            migrationBuilder.CreateTable(
                name: "Notification",
                columns: table => new
                {
                    NotificationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryNoID = table.Column<int>(type: "int", nullable: false),
                    StatusNotification = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorUserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notification", x => x.NotificationId);
                    table.ForeignKey(
                        name: "FK_Notification_CategoryNotification_CategoryNoID",
                        column: x => x.CategoryNoID,
                        principalTable: "CategoryNotification",
                        principalColumn: "CategoryNoID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Notification_Users_CreatorUserId",
                        column: x => x.CreatorUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OwnNotification",
                columns: table => new
                {
                    NotificationId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OwnNotification", x => new { x.NotificationId, x.UserId });
                    table.ForeignKey(
                        name: "FK_OwnNotification_Notification_NotificationId",
                        column: x => x.NotificationId,
                        principalTable: "Notification",
                        principalColumn: "NotificationId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OwnNotification_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Notification_CategoryNoID",
                table: "Notification",
                column: "CategoryNoID");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_CreatorUserId",
                table: "Notification",
                column: "CreatorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_OwnNotification_UserId",
                table: "OwnNotification",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OwnNotification");

            migrationBuilder.DropTable(
                name: "Notification");

            migrationBuilder.DropTable(
                name: "CategoryNotification");
        }
    }
}
