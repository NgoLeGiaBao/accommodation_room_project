﻿// <auto-generated />
using System;
using App.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Accommodation_Room_Project_Offical.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20241018150528_V02")]
    partial class V02
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("App.Models.AppUser", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("integer");

                    b.Property<string>("Address")
                        .HasMaxLength(512)
                        .HasColumnType("text");

                    b.Property<DateTime?>("Birthday")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("text");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("boolean");

                    b.Property<string>("FullName")
                        .HasMaxLength(100)
                        .HasColumnType("text");

                    b.Property<string>("IdentityCard")
                        .HasMaxLength(12)
                        .HasColumnType("character varying(12)");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("boolean");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("text");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("text");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("boolean");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("text");

                    b.Property<bool>("Sex")
                        .HasColumnType("boolean");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("boolean");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.HasKey("Id");

                    b.HasIndex("IdentityCard")
                        .IsUnique();

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex");

                    b.ToTable("AspNetUsers", (string)null);
                });

            modelBuilder.Entity("App.Models.Asset", b =>
                {
                    b.Property<string>("AssetID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("text");

                    b.Property<string>("AssetName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<string>("CategoryAssetID")
                        .HasColumnType("text");

                    b.Property<string>("Condition")
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<decimal>("Cost")
                        .HasColumnType("decimal(18, 2)");

                    b.Property<string>("ImagePath")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("Location")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<DateTime?>("NextMaintenanceDueDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("PurchaseDate")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("AssetID");

                    b.HasIndex("CategoryAssetID");

                    b.ToTable("Assets");
                });

            modelBuilder.Entity("App.Models.CategoryAsset", b =>
                {
                    b.Property<string>("CategoryAssetID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.HasKey("CategoryAssetID");

                    b.ToTable("CategoryAssets");
                });

            modelBuilder.Entity("App.Models.CategoryNotification", b =>
                {
                    b.Property<string>("CategoryNoID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.HasKey("CategoryNoID");

                    b.ToTable("CategoryNotification");
                });

            modelBuilder.Entity("App.Models.Invoice", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("text");

                    b.Property<decimal>("AdditionalServiceFee")
                        .HasColumnType("numeric");

                    b.Property<double>("ElectricityForBefore")
                        .HasColumnType("double precision");

                    b.Property<double>("ElectricityUsage")
                        .HasColumnType("double precision");

                    b.Property<DateTime>("InvoiceDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("PaymentDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("QRCodeImage")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("RoomId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ServiceDetails")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<decimal>("ServiceFee")
                        .HasColumnType("numeric");

                    b.Property<string>("StatusInvocie")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<decimal>("TotalMoney")
                        .HasColumnType("numeric");

                    b.Property<double>("WaterUsage")
                        .HasColumnType("double precision");

                    b.Property<double>("WaterUsageForBefore")
                        .HasColumnType("double precision");

                    b.HasKey("Id");

                    b.HasIndex("QRCodeImage")
                        .IsUnique();

                    b.HasIndex("RoomId");

                    b.ToTable("Invoices");
                });

            modelBuilder.Entity("App.Models.Notification", b =>
                {
                    b.Property<string>("NotificationId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("text");

                    b.Property<string>("CategoryNoID")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("CreatorUserId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("StatusNotification")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("UpdatedDate")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("NotificationId");

                    b.HasIndex("CategoryNoID");

                    b.HasIndex("CreatorUserId");

                    b.ToTable("Notifications");
                });

            modelBuilder.Entity("App.Models.OwnAsset", b =>
                {
                    b.Property<string>("OwnAssetID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("text");

                    b.Property<string>("AssetID")
                        .HasColumnType("text");

                    b.Property<string>("RoomID")
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("OwnAssetID", "AssetID", "RoomID");

                    b.HasIndex("AssetID");

                    b.HasIndex("RoomID");

                    b.ToTable("OwnAssets");
                });

            modelBuilder.Entity("App.Models.OwnNotification", b =>
                {
                    b.Property<string>("NotificationId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("text");

                    b.Property<string>("UserId")
                        .HasColumnType("text")
                        .HasColumnOrder(1);

                    b.HasKey("NotificationId", "UserId");

                    b.HasIndex("UserId");

                    b.ToTable("OwnNotifications");
                });

            modelBuilder.Entity("App.Models.RentalContract", b =>
                {
                    b.Property<string>("ContractID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("text");

                    b.Property<string>("RoomID")
                        .HasColumnType("text");

                    b.Property<string>("UserID")
                        .HasColumnType("text");

                    b.Property<decimal>("Deposit")
                        .HasColumnType("numeric");

                    b.Property<DateTime>("EndupDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("IsPaid")
                        .HasColumnType("boolean");

                    b.Property<decimal>("PElectricityPerKw")
                        .HasColumnType("numeric");

                    b.Property<decimal>("PRentalRoomPerM")
                        .HasColumnType("numeric");

                    b.Property<decimal>("PServicePerK")
                        .HasColumnType("numeric");

                    b.Property<decimal>("PWaterPerK")
                        .HasColumnType("numeric");

                    b.Property<string>("PersonalSignContract")
                        .HasColumnType("text");

                    b.Property<string>("Rules")
                        .HasColumnType("text");

                    b.Property<DateTime>("StartedDate")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("ContractID", "RoomID", "UserID");

                    b.HasIndex("RoomID");

                    b.HasIndex("UserID");

                    b.ToTable("RentalContract");
                });

            modelBuilder.Entity("App.Models.RentalProperty", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("text");

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasMaxLength(512)
                        .HasColumnType("character varying(512)");

                    b.Property<decimal>("ElectricityPrice")
                        .HasColumnType("numeric");

                    b.Property<string>("Facilities")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean");

                    b.Property<int>("NumberOfRooms")
                        .HasColumnType("integer");

                    b.Property<decimal>("OtherService")
                        .HasColumnType("numeric");

                    b.Property<string>("OwnerName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<string>("OwnerPhoneNumber")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("PropertyImage")
                        .HasColumnType("text");

                    b.Property<string>("PropertyName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<string>("PropertyType")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<double>("TotalArea")
                        .HasColumnType("double precision");

                    b.Property<decimal>("WaterPrice")
                        .HasColumnType("numeric");

                    b.HasKey("Id");

                    b.ToTable("RentalProperty");
                });

            modelBuilder.Entity("App.Models.Room", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("text");

                    b.Property<double>("Area")
                        .HasColumnType("double precision");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<double>("ElectricityPriceInit")
                        .HasColumnType("double precision");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean");

                    b.Property<int>("MaximumNumberOfPeople")
                        .HasColumnType("integer");

                    b.Property<decimal>("Price")
                        .HasColumnType("numeric");

                    b.Property<string>("RentalPropertyId")
                        .HasColumnType("text");

                    b.Property<string>("RoomName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<string>("Status")
                        .HasColumnType("text");

                    b.Property<double>("WaterPriceInit")
                        .HasColumnType("double precision");

                    b.HasKey("Id");

                    b.HasIndex("RentalPropertyId");

                    b.ToTable("Room");
                });

            modelBuilder.Entity("App.Models.UserRentalProperty", b =>
                {
                    b.Property<string>("AppUserId")
                        .HasColumnType("text")
                        .HasColumnOrder(1);

                    b.Property<string>("RentalPropertyId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("text")
                        .HasColumnOrder(2);

                    b.HasKey("AppUserId", "RentalPropertyId");

                    b.HasIndex("RentalPropertyId");

                    b.ToTable("UserRentalProperties");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex");

                    b.ToTable("AspNetRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("text");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("text");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("text");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("text");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("text");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("text");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("text");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("text");

                    b.Property<string>("RoleId")
                        .HasColumnType("text");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("text");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<string>("Value")
                        .HasColumnType("text");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens", (string)null);
                });

            modelBuilder.Entity("App.Models.Asset", b =>
                {
                    b.HasOne("App.Models.CategoryAsset", "CategoryAsset")
                        .WithMany("Assets")
                        .HasForeignKey("CategoryAssetID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("CategoryAsset");
                });

            modelBuilder.Entity("App.Models.Invoice", b =>
                {
                    b.HasOne("App.Models.Room", "Room")
                        .WithMany("Invoices")
                        .HasForeignKey("RoomId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Room");
                });

            modelBuilder.Entity("App.Models.Notification", b =>
                {
                    b.HasOne("App.Models.CategoryNotification", "CategoryNotification")
                        .WithMany("Notifications")
                        .HasForeignKey("CategoryNoID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("App.Models.AppUser", "Creator")
                        .WithMany("CreatedNotifications")
                        .HasForeignKey("CreatorUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("CategoryNotification");

                    b.Navigation("Creator");
                });

            modelBuilder.Entity("App.Models.OwnAsset", b =>
                {
                    b.HasOne("App.Models.Asset", "Asset")
                        .WithMany("OwnAssets")
                        .HasForeignKey("AssetID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("App.Models.Room", "Room")
                        .WithMany("OwnAssets")
                        .HasForeignKey("RoomID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Asset");

                    b.Navigation("Room");
                });

            modelBuilder.Entity("App.Models.OwnNotification", b =>
                {
                    b.HasOne("App.Models.Notification", "Notification")
                        .WithMany("OwnNotifications")
                        .HasForeignKey("NotificationId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("App.Models.AppUser", "AppUsers")
                        .WithMany("OwnNotifications")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("AppUsers");

                    b.Navigation("Notification");
                });

            modelBuilder.Entity("App.Models.RentalContract", b =>
                {
                    b.HasOne("App.Models.Room", "Room")
                        .WithMany("RentalContracts")
                        .HasForeignKey("RoomID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("App.Models.AppUser", "AppUser")
                        .WithMany("RentalContracts")
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AppUser");

                    b.Navigation("Room");
                });

            modelBuilder.Entity("App.Models.Room", b =>
                {
                    b.HasOne("App.Models.RentalProperty", "RentalProperty")
                        .WithMany("Rooms")
                        .HasForeignKey("RentalPropertyId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("RentalProperty");
                });

            modelBuilder.Entity("App.Models.UserRentalProperty", b =>
                {
                    b.HasOne("App.Models.AppUser", "AppUser")
                        .WithMany("UserRentalProperties")
                        .HasForeignKey("AppUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("App.Models.RentalProperty", "RentalProperty")
                        .WithMany("UserRentalProperties")
                        .HasForeignKey("RentalPropertyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AppUser");

                    b.Navigation("RentalProperty");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("App.Models.AppUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("App.Models.AppUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("App.Models.AppUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("App.Models.AppUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("App.Models.AppUser", b =>
                {
                    b.Navigation("CreatedNotifications");

                    b.Navigation("OwnNotifications");

                    b.Navigation("RentalContracts");

                    b.Navigation("UserRentalProperties");
                });

            modelBuilder.Entity("App.Models.Asset", b =>
                {
                    b.Navigation("OwnAssets");
                });

            modelBuilder.Entity("App.Models.CategoryAsset", b =>
                {
                    b.Navigation("Assets");
                });

            modelBuilder.Entity("App.Models.CategoryNotification", b =>
                {
                    b.Navigation("Notifications");
                });

            modelBuilder.Entity("App.Models.Notification", b =>
                {
                    b.Navigation("OwnNotifications");
                });

            modelBuilder.Entity("App.Models.RentalProperty", b =>
                {
                    b.Navigation("Rooms");

                    b.Navigation("UserRentalProperties");
                });

            modelBuilder.Entity("App.Models.Room", b =>
                {
                    b.Navigation("Invoices");

                    b.Navigation("OwnAssets");

                    b.Navigation("RentalContracts");
                });
#pragma warning restore 612, 618
        }
    }
}
