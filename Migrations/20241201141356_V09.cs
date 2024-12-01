using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accommodation_Room_Project_Offical.Migrations
{
    /// <inheritdoc />
    public partial class V09 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "StatusNotification",
                table: "Notifications",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateTable(
                name: "ServicesBlogs",
                columns: table => new
                {
                    ServicesBlogId = table.Column<string>(type: "text", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: true),
                    SelfManagedAccommodation = table.Column<string>(type: "text", nullable: true),
                    Area = table.Column<string>(type: "text", nullable: true),
                    RentalPrice = table.Column<decimal>(type: "numeric", nullable: false),
                    ContentDescription = table.Column<string>(type: "text", nullable: true),
                    IsStudent = table.Column<bool>(type: "boolean", nullable: false),
                    IsWorker = table.Column<bool>(type: "boolean", nullable: false),
                    IsFamily = table.Column<bool>(type: "boolean", nullable: false),
                    IsCouple = table.Column<bool>(type: "boolean", nullable: false),
                    HasLoft = table.Column<bool>(type: "boolean", nullable: false),
                    HasWiFi = table.Column<bool>(type: "boolean", nullable: false),
                    HasIndoorCleaning = table.Column<bool>(type: "boolean", nullable: false),
                    HasBathroom = table.Column<bool>(type: "boolean", nullable: false),
                    HasWaterHeater = table.Column<bool>(type: "boolean", nullable: false),
                    HasKitchenShelf = table.Column<bool>(type: "boolean", nullable: false),
                    HasWashingMachine = table.Column<bool>(type: "boolean", nullable: false),
                    HasTV = table.Column<bool>(type: "boolean", nullable: false),
                    HasAirConditioner = table.Column<bool>(type: "boolean", nullable: false),
                    HasRefrigerator = table.Column<bool>(type: "boolean", nullable: false),
                    HasBedAndMattress = table.Column<bool>(type: "boolean", nullable: false),
                    HasWardrobe = table.Column<bool>(type: "boolean", nullable: false),
                    HasBalcony = table.Column<bool>(type: "boolean", nullable: false),
                    HasElevator = table.Column<bool>(type: "boolean", nullable: false),
                    HasPrivateParkingLot = table.Column<bool>(type: "boolean", nullable: false),
                    HasSecurityCamera = table.Column<bool>(type: "boolean", nullable: false),
                    HasSwimmingPool = table.Column<bool>(type: "boolean", nullable: false),
                    HasGarden = table.Column<bool>(type: "boolean", nullable: false),
                    NearMarket = table.Column<bool>(type: "boolean", nullable: false),
                    NearSupermarket = table.Column<bool>(type: "boolean", nullable: false),
                    NearHospital = table.Column<bool>(type: "boolean", nullable: false),
                    NearSchool = table.Column<bool>(type: "boolean", nullable: false),
                    NearPark = table.Column<bool>(type: "boolean", nullable: false),
                    NearBusStation = table.Column<bool>(type: "boolean", nullable: false),
                    NearGym = table.Column<bool>(type: "boolean", nullable: false),
                    Province = table.Column<string>(type: "text", nullable: true),
                    District = table.Column<string>(type: "text", nullable: true),
                    Town = table.Column<string>(type: "text", nullable: true),
                    Address = table.Column<string>(type: "text", nullable: true),
                    FullName = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    ZaloNumber = table.Column<string>(type: "text", nullable: true),
                    Images = table.Column<List<string>>(type: "text[]", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServicesBlogs", x => x.ServicesBlogId);
                });

            migrationBuilder.CreateTable(
                name: "RooomInServiceBlogs",
                columns: table => new
                {
                    RooomInServiceBlogId = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Area = table.Column<double>(type: "double precision", nullable: false),
                    RentalPrice = table.Column<decimal>(type: "numeric", nullable: false),
                    NumberOfRooms = table.Column<int>(type: "integer", nullable: false),
                    MaximumPerson = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Bed = table.Column<bool>(type: "boolean", nullable: false),
                    AirConditioner = table.Column<bool>(type: "boolean", nullable: false),
                    TV = table.Column<bool>(type: "boolean", nullable: false),
                    Socket = table.Column<bool>(type: "boolean", nullable: false),
                    Wardrobe = table.Column<bool>(type: "boolean", nullable: false),
                    Fan = table.Column<bool>(type: "boolean", nullable: false),
                    Refrigerator = table.Column<bool>(type: "boolean", nullable: false),
                    WaterHeater = table.Column<bool>(type: "boolean", nullable: false),
                    Desk = table.Column<bool>(type: "boolean", nullable: false),
                    Chair = table.Column<bool>(type: "boolean", nullable: false),
                    Curtains = table.Column<bool>(type: "boolean", nullable: false),
                    Lighting = table.Column<bool>(type: "boolean", nullable: false),
                    Wifi = table.Column<bool>(type: "boolean", nullable: false),
                    Window = table.Column<bool>(type: "boolean", nullable: false),
                    Mirror = table.Column<bool>(type: "boolean", nullable: false),
                    ImageUrls = table.Column<List<string>>(type: "text[]", nullable: false),
                    ServicesBlogId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RooomInServiceBlogs", x => x.RooomInServiceBlogId);
                    table.ForeignKey(
                        name: "FK_RooomInServiceBlogs_ServicesBlogs_ServicesBlogId",
                        column: x => x.ServicesBlogId,
                        principalTable: "ServicesBlogs",
                        principalColumn: "ServicesBlogId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RooomInServiceBlogs_ServicesBlogId",
                table: "RooomInServiceBlogs",
                column: "ServicesBlogId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RooomInServiceBlogs");

            migrationBuilder.DropTable(
                name: "ServicesBlogs");

            migrationBuilder.AlterColumn<string>(
                name: "StatusNotification",
                table: "Notifications",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }
    }
}
