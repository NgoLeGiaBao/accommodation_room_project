using System.Security.Cryptography.Xml;
using App.ExtendMethods;
using App.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Newtonsoft.Json;
using App.Services;
using Microsoft.Extensions.Options;


namespace App
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.
			builder.Services.AddControllersWithViews()
				.AddNewtonsoftJson(options =>
				{
					options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

				});

			// Configure to connect database
			builder.Services.AddDbContext<AppDbContext>(options =>
			{
				string connectionString = builder.Configuration.GetConnectionString("AppMvcConnectionString");
				options.UseNpgsql(connectionString);
				// options.UseSqlServer(connectionString);
			});

			// Bind the Supabase settings
			builder.Services.Configure<SupabaseSettings>(builder.Configuration.GetSection("SupabaseSettings"));

			// Register the Supabase Client
			builder.Services.AddScoped<Supabase.Client>(provider =>
			{
				var settings = provider.GetRequiredService<IOptions<SupabaseSettings>>().Value;
				return new Supabase.Client(settings.SupabaseUrl, settings.SupabaseAnonKey);
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
				options.LoginPath = "/login";
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
				pattern: "{area=Introduction}/{controller=HomePage}/{action=Home}"
			);
			app.Run();
		}
	}
}
