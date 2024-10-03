using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accommodation_Room_Project_Offical.Migrations
{
    /// <inheritdoc />
    public partial class V02 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notification_CategoryNotification_CategoryNoID",
                table: "Notification");

            migrationBuilder.DropForeignKey(
                name: "FK_Notification_Users_CreatorUserId",
                table: "Notification");

            migrationBuilder.DropForeignKey(
                name: "FK_OwnAsset_Assets_AssetID",
                table: "OwnAsset");

            migrationBuilder.DropForeignKey(
                name: "FK_OwnAsset_Room_RoomID",
                table: "OwnAsset");

            migrationBuilder.DropForeignKey(
                name: "FK_OwnNotification_Notification_NotificationId",
                table: "OwnNotification");

            migrationBuilder.DropForeignKey(
                name: "FK_OwnNotification_Users_UserId",
                table: "OwnNotification");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OwnNotification",
                table: "OwnNotification");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OwnAsset",
                table: "OwnAsset");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Notification",
                table: "Notification");

            migrationBuilder.RenameTable(
                name: "OwnNotification",
                newName: "OwnNotifications");

            migrationBuilder.RenameTable(
                name: "OwnAsset",
                newName: "OwnAssets");

            migrationBuilder.RenameTable(
                name: "Notification",
                newName: "Notifications");

            migrationBuilder.RenameIndex(
                name: "IX_OwnNotification_UserId",
                table: "OwnNotifications",
                newName: "IX_OwnNotifications_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_OwnAsset_RoomID",
                table: "OwnAssets",
                newName: "IX_OwnAssets_RoomID");

            migrationBuilder.RenameIndex(
                name: "IX_OwnAsset_AssetID",
                table: "OwnAssets",
                newName: "IX_OwnAssets_AssetID");

            migrationBuilder.RenameIndex(
                name: "IX_Notification_CreatorUserId",
                table: "Notifications",
                newName: "IX_Notifications_CreatorUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Notification_CategoryNoID",
                table: "Notifications",
                newName: "IX_Notifications_CategoryNoID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OwnNotifications",
                table: "OwnNotifications",
                columns: new[] { "NotificationId", "UserId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_OwnAssets",
                table: "OwnAssets",
                columns: new[] { "OwnAssetID", "AssetID", "RoomID" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Notifications",
                table: "Notifications",
                column: "NotificationId");

            migrationBuilder.CreateTable(
                name: "Invoices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ElectricityUsage = table.Column<double>(type: "float", nullable: false),
                    WaterUsage = table.Column<double>(type: "float", nullable: false),
                    ServiceDetails = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AdditionalServiceFee = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DateInvoice = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    QRCodeImage = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoomId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invoices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Invoices_Room_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Room",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_QRCodeImage",
                table: "Invoices",
                column: "QRCodeImage",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_RoomId",
                table: "Invoices",
                column: "RoomId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_CategoryNotification_CategoryNoID",
                table: "Notifications",
                column: "CategoryNoID",
                principalTable: "CategoryNotification",
                principalColumn: "CategoryNoID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Users_CreatorUserId",
                table: "Notifications",
                column: "CreatorUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OwnAssets_Assets_AssetID",
                table: "OwnAssets",
                column: "AssetID",
                principalTable: "Assets",
                principalColumn: "AssetID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OwnAssets_Room_RoomID",
                table: "OwnAssets",
                column: "RoomID",
                principalTable: "Room",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OwnNotifications_Notifications_NotificationId",
                table: "OwnNotifications",
                column: "NotificationId",
                principalTable: "Notifications",
                principalColumn: "NotificationId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OwnNotifications_Users_UserId",
                table: "OwnNotifications",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_CategoryNotification_CategoryNoID",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Users_CreatorUserId",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_OwnAssets_Assets_AssetID",
                table: "OwnAssets");

            migrationBuilder.DropForeignKey(
                name: "FK_OwnAssets_Room_RoomID",
                table: "OwnAssets");

            migrationBuilder.DropForeignKey(
                name: "FK_OwnNotifications_Notifications_NotificationId",
                table: "OwnNotifications");

            migrationBuilder.DropForeignKey(
                name: "FK_OwnNotifications_Users_UserId",
                table: "OwnNotifications");

            migrationBuilder.DropTable(
                name: "Invoices");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OwnNotifications",
                table: "OwnNotifications");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OwnAssets",
                table: "OwnAssets");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Notifications",
                table: "Notifications");

            migrationBuilder.RenameTable(
                name: "OwnNotifications",
                newName: "OwnNotification");

            migrationBuilder.RenameTable(
                name: "OwnAssets",
                newName: "OwnAsset");

            migrationBuilder.RenameTable(
                name: "Notifications",
                newName: "Notification");

            migrationBuilder.RenameIndex(
                name: "IX_OwnNotifications_UserId",
                table: "OwnNotification",
                newName: "IX_OwnNotification_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_OwnAssets_RoomID",
                table: "OwnAsset",
                newName: "IX_OwnAsset_RoomID");

            migrationBuilder.RenameIndex(
                name: "IX_OwnAssets_AssetID",
                table: "OwnAsset",
                newName: "IX_OwnAsset_AssetID");

            migrationBuilder.RenameIndex(
                name: "IX_Notifications_CreatorUserId",
                table: "Notification",
                newName: "IX_Notification_CreatorUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Notifications_CategoryNoID",
                table: "Notification",
                newName: "IX_Notification_CategoryNoID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OwnNotification",
                table: "OwnNotification",
                columns: new[] { "NotificationId", "UserId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_OwnAsset",
                table: "OwnAsset",
                columns: new[] { "OwnAssetID", "AssetID", "RoomID" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Notification",
                table: "Notification",
                column: "NotificationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notification_CategoryNotification_CategoryNoID",
                table: "Notification",
                column: "CategoryNoID",
                principalTable: "CategoryNotification",
                principalColumn: "CategoryNoID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Notification_Users_CreatorUserId",
                table: "Notification",
                column: "CreatorUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OwnAsset_Assets_AssetID",
                table: "OwnAsset",
                column: "AssetID",
                principalTable: "Assets",
                principalColumn: "AssetID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OwnAsset_Room_RoomID",
                table: "OwnAsset",
                column: "RoomID",
                principalTable: "Room",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OwnNotification_Notification_NotificationId",
                table: "OwnNotification",
                column: "NotificationId",
                principalTable: "Notification",
                principalColumn: "NotificationId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OwnNotification_Users_UserId",
                table: "OwnNotification",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
