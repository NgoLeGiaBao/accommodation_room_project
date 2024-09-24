using App.ExtendMethods;
using App.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace App
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.
			builder.Services.AddControllersWithViews();

			// Configure to connect database
			builder.Services.AddDbContext<AppDbContext>(options =>
			{
				string connectionString = builder.Configuration.GetConnectionString("AppMvcConnectionString");
				options.UseSqlServer(connectionString);
			});

			// Register Identity without UI
			builder.Services.AddIdentity<AppUser, IdentityRole>()
						.AddEntityFrameworkStores<AppDbContext>()
						.AddDefaultTokenProviders();

			var app = builder.Build();
			// Configure the HTTP request pipeline.
			if (!app.Environment.IsDevelopment())
			{
				app.UseExceptionHandler("/Home/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}
			app.AddStatusCode();
			app.UseHttpsRedirection();
			app.UseStaticFiles();

			app.UseRouting();

			app.UseAuthorization();

			app.MapControllerRoute(
				name: "default",
				pattern: "{Area= DashBoard }/{controller=DashBoard}/{action=Index}");

			app.Run();
		}
	}
}
