using App.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using App.Models;
using Microsoft.EntityFrameworkCore;

namespace App.Areas.Introduction
{
    [Area("Introduction")]
    public class HomePage : Controller
    {
        private readonly SupabaseSettings _supabaseSettings;
        private readonly AppDbContext _appDbContext;

        public HomePage(IOptions<SupabaseSettings> supabaseSettings, AppDbContext appDbContext)
        {
            _supabaseSettings = supabaseSettings.Value;
            _appDbContext = appDbContext;
        }

        [Route("/")]
        public IActionResult Home()
        {
            return View();
        }

        [Route("/blog")]
        public IActionResult Blog()
        {
            return View();
        }

        [Route("/single/{id}")]
        public async Task<ActionResult> Single(string id)
        {
            var news = await _appDbContext.ContentNewses
                .Where(c => c.Published == true && c.Id == id)
                .FirstOrDefaultAsync();

            if (news == null)
            {
                return NotFound();
            }
            return View(news);
        }

        [Route("/get-all-news-intro")]
        [HttpGet]
        public async Task<IActionResult> GetUserNews()
        {
            // Initialize Supabase client
            var options = new Supabase.SupabaseOptions { AutoConnectRealtime = true };
            var supabase = new Supabase.Client(_supabaseSettings.SupabaseUrl, _supabaseSettings.SupabaseAnonKey, options);
            await supabase.InitializeAsync();

            // Fetch all news articles for the public
            var news = await _appDbContext.ContentNewses
                .Where(c => c.Published == true)
                .Select(c => new
                {
                    c.AuthorId,
                    c.Id,
                    c.DateUpdated,
                    c.GeneralTitle,
                    c.GeneralDescription,
                    ImageId = c.ImageId
                })
                .ToListAsync();

            if (news == null || !news.Any())
                return NotFound("No news found.");

            // Fetch authors based on unique AuthorIds in news
            var authorIds = news.Select(n => n.AuthorId).Distinct();
            var authors = await _appDbContext.AppUsers
                .Where(a => authorIds.Contains(a.Id))
                .ToDictionaryAsync(a => a.Id, a => a.FullName); // Adjust property names based on your Author model

            // Generate final result
            var result = news.Select(c => new
            {
                AuthorName = authors.ContainsKey(c.AuthorId) ? authors[c.AuthorId] : "Unknown",
                c.Id,
                c.DateUpdated,
                c.GeneralTitle,
                c.GeneralDescription,
                ImageUrl = string.IsNullOrEmpty(c.ImageId)
                    ? null
                    : supabase.Storage.From("news_img").GetPublicUrl(c.ImageId)
            }).ToList();

            return Ok(result);
        }
    }
}