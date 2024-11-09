using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accommodation_Room_Project_Offical.Migrations
{
    /// <inheritdoc />
    public partial class V08 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_CategoryNotification_CategoryNoID",
                table: "Notifications");

            migrationBuilder.AlterColumn<string>(
                name: "CategoryNoID",
                table: "Notifications",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "NotificationContent",
                table: "Notifications",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NotificationTitle",
                table: "Notifications",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_CategoryNotification_CategoryNoID",
                table: "Notifications",
                column: "CategoryNoID",
                principalTable: "CategoryNotification",
                principalColumn: "CategoryNoID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_CategoryNotification_CategoryNoID",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "NotificationContent",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "NotificationTitle",
                table: "Notifications");

            migrationBuilder.AlterColumn<string>(
                name: "CategoryNoID",
                table: "Notifications",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_CategoryNotification_CategoryNoID",
                table: "Notifications",
                column: "CategoryNoID",
                principalTable: "CategoryNotification",
                principalColumn: "CategoryNoID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
