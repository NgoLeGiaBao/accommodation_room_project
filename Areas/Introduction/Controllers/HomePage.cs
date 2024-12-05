using App.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using App.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

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
            var news = _appDbContext.ContentNewses
                            .Include(c => c.Author)
                            .Where(c => c.Published == true)
                            .ToList();
            var blogNews = _appDbContext.ServicesBlogs
                            .Include(c => c.Rooms).ToList();

            ViewData["News"] = news;
            ViewData["BlogNews"] = blogNews;
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

        [Route("/get-filtered-services-blogs")]
        [HttpPost]
        public async Task<IActionResult> GetFilteredServicesBlogs(string province, string district, string town, decimal? minSize, decimal? maxSize, decimal? minPrice, decimal? maxPrice)
        {
            // Khởi tạo query cơ bản
            var query = _appDbContext.ServicesBlogs.AsQueryable();

            // Thêm các điều kiện lọc vào query nếu có
            if (!string.IsNullOrEmpty(province))
                query = query.Where(s => s.Province == province);

            if (!string.IsNullOrEmpty(district))
            {
                if (district != "0")
                {
                    query = query.Where(s => s.District == district);

                }
            }

            if (!string.IsNullOrEmpty(town))
            {
                if (town != "0")
                {
                    query = query.Where(s => s.Town == town);
                }
            }

            if (minSize.HasValue)
                query = query.Where(s => s.Size >= minSize.Value);

            if (maxSize.HasValue)
                query = query.Where(s => s.Size <= maxSize.Value);

            if (minPrice.HasValue)
                query = query.Where(s => s.RentalPrice >= minPrice.Value);

            if (maxPrice.HasValue)
                query = query.Where(s => s.RentalPrice <= maxPrice.Value);

            // Lấy dữ liệu đã lọc và trả về Json
            var filteredServicesBlogs = await query.ToListAsync();
            return Json(filteredServicesBlogs);
        }

        [Route("/detail-service-blog/{slug}")]
        [HttpGet]
        public async Task<IActionResult> DetailRoomService(string slug)
        {
            var serviceBlog = _appDbContext.ServicesBlogs
                                .Include(s => s.Rooms)
                                .FirstOrDefault(s => s.Slug == slug);

            if (serviceBlog == null)
                return NotFound();
            var user = await _appDbContext.AppUsers.Where(u => u.Id == serviceBlog.CreatedBy).FirstOrDefaultAsync();
            if (user == null)
                return NotFound();

            ViewData["UserName"] = user.FullName;
            return View(serviceBlog);
        }


    }
}