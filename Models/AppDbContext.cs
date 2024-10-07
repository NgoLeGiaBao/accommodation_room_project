using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace App.Models
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //-- Bỏ đi tiền tố ASP NET trong model builder --//
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                var tableName = entity.GetTableName();
                if (tableName.StartsWith("AspNet"))
                {
                    entity.SetTableName(tableName.Substring(6));
                }
            }
            // Rename all tables that start with "AspNet"
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                var tableName = entity.GetTableName();
                if (tableName.StartsWith("AspNet"))
                {
                    entity.SetTableName(tableName.Substring(6));
                }
            }

            // Define Unique IdentityCard
            modelBuilder.Entity<AppUser>()
                .HasIndex(u => u.IdentityCard)
                .IsUnique();

            //  Define the composite primary key for Rental Property
            modelBuilder.Entity<RentalProperty>()
                .HasMany(rp => rp.Rooms)
                .WithOne(r => r.RentalProperty)
                .HasForeignKey(r => r.RentalPropertyId)
                .OnDelete(DeleteBehavior.Cascade);

            // Define the composite primary key for RentalContract
            modelBuilder.Entity<RentalContract>()
                .HasKey(rc => new { rc.ContractID, rc.RoomID, rc.UserID });

            // Establish the relationship between RentalContract and Room
            modelBuilder.Entity<RentalContract>()
                .HasOne(rc => rc.Room)
                .WithMany(r => r.RentalContracts)
                .HasForeignKey(rc => rc.RoomID)
                .OnDelete(DeleteBehavior.Cascade);

            // Establish the relationship between RentalContract and AppUser
            modelBuilder.Entity<RentalContract>()
                .HasOne(rc => rc.AppUser)
                .WithMany(au => au.RentalContracts)
                .HasForeignKey(rc => rc.UserID)
                .OnDelete(DeleteBehavior.Cascade);

            // Define the composite primary key for CategoryAsset
            modelBuilder.Entity<CategoryAsset>()
                .HasMany(c => c.Assets)
                .WithOne(a => a.CategoryAsset)
                .HasForeignKey(a => a.CategoryAssetID);

            // Define the one-to-many relationship between CategoryAsset and Asset
            modelBuilder.Entity<CategoryAsset>()
                .HasMany(c => c.Assets)
                .WithOne(a => a.CategoryAsset)
                .HasForeignKey(a => a.CategoryAssetID)
                .OnDelete(DeleteBehavior.Cascade);

            // Define the composite primary key for between Asset and Room through OwnAsset
            modelBuilder.Entity<OwnAsset>()
                .HasKey(oa => new { oa.OwnAssetID, oa.AssetID, oa.RoomID });

            modelBuilder.Entity<OwnAsset>()
                .HasOne(oa => oa.Asset)
                .WithMany(a => a.OwnAssets)
                .HasForeignKey(oa => oa.AssetID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OwnAsset>()
                .HasOne(oa => oa.Room)
                .WithMany(r => r.OwnAssets)
                .HasForeignKey(oa => oa.RoomID)
                .OnDelete(DeleteBehavior.Cascade);

            // Define the one-to-many relationship between User and Notification
            modelBuilder.Entity<Notification>()
                .HasOne(n => n.Creator)
                .WithMany(u => u.CreatedNotifications)
                .HasForeignKey(n => n.CreatorUserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Define the one-to-one relationship between Notification and CategoryNotification
            modelBuilder.Entity<Notification>()
                .HasOne(n => n.CategoryNotification)
                .WithMany(c => c.Notifications)
                .HasForeignKey(n => n.CategoryNoID);

            // Define the composite primary key for between Notification and AppUser through Own notification
            modelBuilder.Entity<Notification>()
                .HasKey(n => n.NotificationId);

            modelBuilder.Entity<OwnNotification>()
                .HasKey(on => new { on.NotificationId, on.UserId });

            modelBuilder.Entity<OwnNotification>()
                .HasOne(on => on.Notification)
                .WithMany(n => n.OwnNotifications)
                .HasForeignKey(on => on.NotificationId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OwnNotification>()
                .HasOne(on => on.AppUsers)
                .WithMany(u => u.OwnNotifications)
                .HasForeignKey(on => on.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Define the one-to-many relationship between Invoice and Room
            modelBuilder.Entity<Room>()
                .HasMany(r => r.Invoices)
                .WithOne(i => i.Room)
                .HasForeignKey(i => i.RoomId);

            modelBuilder.Entity<Invoice>()
                .HasIndex(i => i.QRCodeImage)
                .IsUnique();


            // Configure composite primary key for UserRentalProperty
            modelBuilder.Entity<UserRentalProperty>()
                .HasKey(ur => new { ur.AppUserId, ur.RentalPropertyId });

            modelBuilder.Entity<UserRentalProperty>()
                .HasOne(ur => ur.AppUser)
                .WithMany(u => u.UserRentalProperties)
                .HasForeignKey(ur => ur.AppUserId);

            modelBuilder.Entity<UserRentalProperty>()
                .HasOne(ur => ur.RentalProperty)
                .WithMany(rp => rp.UserRentalProperties)
                .HasForeignKey(ur => ur.RentalPropertyId);

            base.OnModelCreating(modelBuilder);
        }


        // Các DbSet cho các thực thể
        public DbSet<RentalProperty> RentalProperties { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<RentalContract> RentalContracts { get; set; }
        public DbSet<CategoryAsset> CategoryAssets { get; set; }
        public DbSet<Asset> Assets { get; set; }
        public DbSet<OwnAsset> OwnAssets { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<OwnNotification> OwnNotifications { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<UserRentalProperty> UserRentalProperties { get; set; }
        public DbSet<AppUser> AppUsers { get; set; }
    }
}
