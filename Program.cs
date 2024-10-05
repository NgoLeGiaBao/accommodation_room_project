using App.ExtendMethods;
using App.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;


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
				options.UseNpgsql(connectionString);
				// options.UseSqlServer(connectionString);
			});

			builder.Services.AddSingleton<IFileProvider>(
				new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")));

			// Register Identity without UI
			builder.Services.AddIdentity<AppUser, IdentityRole>()
						.AddEntityFrameworkStores<AppDbContext>()
						.AddDefaultTokenProviders();
			// Configure authorize
			builder.Services.ConfigureApplicationCookie(options =>
			{
				options.LoginPath = "/";
				// options.LogoutPath = "/Identity/Account/Logout";
				// options.AccessDeniedPath = "/Identity/Account/AccessDenied";
			});

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
			app.UseAuthentication();
			app.UseAuthorization();

			app.MapControllerRoute(
				name: "default",
				pattern: "{area=Account}/{controller=Login}/{action=Index}"
			);
			app.Run();
		}
	}
}
