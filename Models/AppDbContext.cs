using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace App.Models
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Bỏ đi tiền tố ASP.NET trong tên bảng
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
                .HasOne(rp => rp.AppUser)
                .WithMany(au => au.RentalProperties)
                .HasForeignKey(rp => rp.AppUserId)
                .OnDelete(DeleteBehavior.Restrict);

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
                .WithMany(r => r.RentalContracts) // Specify the collection in Room
                .HasForeignKey(rc => rc.RoomID)
                .OnDelete(DeleteBehavior.Cascade); // Allow cascading deletes for contracts

            // Establish the relationship between RentalContract and AppUser
            modelBuilder.Entity<RentalContract>()
                .HasOne(rc => rc.AppUser)
                .WithMany(au => au.RentalContracts) // Specify the collection in AppUser
                .HasForeignKey(rc => rc.UserID)
                .OnDelete(DeleteBehavior.Cascade); // Allow cascading deletes for contracts    

            // Define the composite primary key for CategoryAsset
            modelBuilder.Entity<CategoryAsset>()
                .HasMany(c => c.Assets)
                .WithOne(a => a.CategoryAsset)
                .HasForeignKey(a => a.CategoryAssetID);

            // Định nghĩa mối quan hệ một-nhiều giữa CategoryAsset và Asset
            modelBuilder.Entity<CategoryAsset>()
                .HasMany(c => c.Assets)
                .WithOne(a => a.CategoryAsset)
                .HasForeignKey(a => a.CategoryAssetID)
                .OnDelete(DeleteBehavior.Cascade); // Cho phép xóa đệ quy

            // Định nghĩa mối quan hệ nhiều-nhiều giữa Asset và Room qua OwnAsset
            modelBuilder.Entity<OwnAsset>()
                .HasKey(oa => new { oa.OwnAssetID, oa.AssetID, oa.RoomID }); // Khóa chính là sự kết hợp của AssetID và RoomID

            modelBuilder.Entity<OwnAsset>()
                .HasOne(oa => oa.Asset)
                .WithMany(a => a.OwnAssets)
                .HasForeignKey(oa => oa.AssetID)
                .OnDelete(DeleteBehavior.Cascade); // Cho phép xóa đệ quy khi xóa tài sản

            modelBuilder.Entity<OwnAsset>()
                .HasOne(oa => oa.Room)
                .WithMany(r => r.OwnAssets)
                .HasForeignKey(oa => oa.RoomID)
                .OnDelete(DeleteBehavior.Cascade); // Cho phép xóa đệ quy khi xóa phòng

            // Cấu hình mối quan hệ giữa User và Notification
            modelBuilder.Entity<Notification>()
                .HasOne(n => n.Creator)
                .WithMany(u => u.CreatedNotifications)
                .HasForeignKey(n => n.CreatorUserId)
                .OnDelete(DeleteBehavior.Cascade); // Xóa thông báo khi người tạo bị xóa

            // Cấu hình mối quan hệ giữa Notification và CategoryNotification
            modelBuilder.Entity<Notification>()
                .HasOne(n => n.CategoryNotification)
                .WithMany(c => c.Notifications)
                .HasForeignKey(n => n.CategoryNoID); // Khóa ngoại




            modelBuilder.Entity<Notification>()
                .HasKey(n => n.NotificationId);

            modelBuilder.Entity<OwnNotification>()
                .HasKey(on => new { on.NotificationId, on.UserId });

            modelBuilder.Entity<OwnNotification>()
                .HasOne(on => on.Notification)
                .WithMany(n => n.OwnNotifications)
                .HasForeignKey(on => on.NotificationId)
                .OnDelete(DeleteBehavior.Restrict); // Ngăn chặn xóa cascade

            modelBuilder.Entity<OwnNotification>()
                .HasOne(on => on.AppUsers)
                .WithMany(u => u.OwnNotifications)
                .HasForeignKey(on => on.UserId)
                .OnDelete(DeleteBehavior.Restrict); // Ngăn chặn xóa cascade
        }

        // Các DbSet cho các thực thể
        public DbSet<RentalProperty> RentalProperties { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<RentalContract> RentalContracts { get; set; }
        public DbSet<CategoryAsset> CategoryAssets { get; set; }
        public DbSet<Asset> Assets { get; set; }
    }
}
