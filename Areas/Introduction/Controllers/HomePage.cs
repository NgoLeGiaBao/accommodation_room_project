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

        [Route("/video-review")]
        public IActionResult VideoReview()
        {
            return View();
        }

        [Route("/room-service")]
        public IActionResult RoomService()
        {
            return View();
        }

        [Route("/detail-room-service/{id?}")]
        public IActionResult DetailRoomService(string id)
        {
            return View();
        }
        [Route("/news-detail/{id}")]
        public async Task<ActionResult> Single(string id)
        {
            var news = await _appDbContext.ContentNewses
                .Include(c => c.Author)
                .Where(c => c.Published == true && c.Id == id)
                .FirstOrDefaultAsync();

            if (news == null)
            {
                return NotFound();
            }
            return View(news);
        }

        [Route("/get-news-pagination")]
        [HttpGet]
        public async Task<IActionResult> GetAllNews(int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber < 1 || pageSize < 1)
                return BadRequest("Invalid pageNumber or pageSize");

            var options = new Supabase.SupabaseOptions { AutoConnectRealtime = true };
            var supabase = new Supabase.Client(_supabaseSettings.SupabaseUrl, _supabaseSettings.SupabaseAnonKey, options);
            await supabase.InitializeAsync();

            var totalCount = await _appDbContext.ContentNewses.CountAsync();

            if (totalCount == 0)
                return NotFound("No news found.");

            var news = await _appDbContext.ContentNewses
                .Include(c => c.Author) // Include tác giả
                .OrderByDescending(c => c.Published)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = new
            {
                TotalCount = totalCount,
                PageSize = pageSize,
                PageNumber = pageNumber,
                Items = news.Select(c => new
                {
                    c.Id,
                    c.GeneralTitle,
                    c.Content,
                    DateUpdated = c.DateUpdated.ToString("dd/MM/yyyy"),
                    AuthorName = c.Author != null ? c.Author.FullName : "Unknown Author", // Lấy tên tác giả
                    ImageUrl = string.IsNullOrEmpty(c.ImageId)
                        ? null
                        : supabase.Storage.From("news_img").GetPublicUrl(c.ImageId)
                }).ToList()
            };

            return Ok(result);
        }
    }
}