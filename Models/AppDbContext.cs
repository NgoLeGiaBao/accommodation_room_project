using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace App.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options): base(options)
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        public DbSet<Menu> menu { set; get; }
    }
}
