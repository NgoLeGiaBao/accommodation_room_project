using App.Models.NewsModel;
using App.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using App.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;


namespace App.Areas.Blog
{
    [Area("Blog")]
    [Authorize]
    public class NewsController : Controller
    {
        private readonly SupabaseSettings _supabaseSettings;
        private readonly AppDbContext _appDbContext;
        private readonly UserManager<AppUser> _userManager;

        public NewsController(IOptions<SupabaseSettings> supabaseSettings, AppDbContext appDbContext, UserManager<AppUser> userManager)
        {
            _supabaseSettings = supabaseSettings.Value;
            _appDbContext = appDbContext;
            _userManager = userManager;
        }

        [Route("/news-blog")]
        public IActionResult Index()
        {
            return View();
        }

        [Route("/create-news-blog")]
        [HttpGet]
        public IActionResult CreateNews()
        {
            return View(new ContentNews());
        }

        [Route("/create-news-blog")]
        [HttpPost]
        public async Task<IActionResult> CreateNews(ContentNews contentNews, IFormFile? ImageId)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);

                contentNews.DateCreated = DateTime.Now.ToUniversalTime();
                contentNews.DateUpdated = DateTime.Now.ToUniversalTime();
                contentNews.AuthorId = user.Id;
                contentNews.Published = true;

                // Process the uploaded image if it exists
                // Initialize Supabase
                var options = new Supabase.SupabaseOptions { AutoConnectRealtime = true };
                var supabase = new Supabase.Client(_supabaseSettings.SupabaseUrl, _supabaseSettings.SupabaseAnonKey, options);
                await supabase.InitializeAsync();

                if (ImageId != null && ImageId.Length > 0)
                {
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + ImageId.FileName;
                    // Read the file into a MemoryStream
                    using var memoryStream = new MemoryStream();
                    await ImageId.CopyToAsync(memoryStream);
                    var fileData = memoryStream.ToArray();
                    // Upload to Supabase
                    var result = await supabase.Storage
                        .From("news_img")
                        .Upload(fileData, uniqueFileName, new Supabase.Storage.FileOptions
                        {
                            CacheControl = "3600",
                            Upsert = true
                        });

                    // Get the public URL of the uploaded file
                    var publicUrl = supabase.Storage.From("news_img").GetPublicUrl(uniqueFileName);
                    contentNews.ImageId = uniqueFileName;

                    _appDbContext.ContentNewses.Add(contentNews);
                    await _appDbContext.SaveChangesAsync();
                    return Redirect("/news-blog");
                }
            }
            var modelErrors = ModelState.Values.SelectMany(v => v.Errors)
                                                   .Select(e => e.ErrorMessage)
                                                   .ToList();
            return Content("ModelState Errors: " + string.Join(", ", modelErrors));
        }

        [Route("/get-all-news")]
        [HttpPost]
        public async Task<IActionResult> GetUserNews(string status)
        {
            // Initialize Supabase client
            var options = new Supabase.SupabaseOptions { AutoConnectRealtime = true };
            var supabase = new Supabase.Client(_supabaseSettings.SupabaseUrl, _supabaseSettings.SupabaseAnonKey, options);
            await supabase.InitializeAsync();

            // Retrieve the current user
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return NotFound(new { message = "User not found" });

            // Fetch all news articles for the current user
            var query = _appDbContext.ContentNewses.Where(c => c.AuthorId == user.Id); // Filter by user

            // Filter by 'status' (Published)
            if (status != "all")
            {
                bool isPublished = status == "public";
                query = query.Where(c => c.Published == isPublished);
            }

            var news = await query.ToListAsync(); // Execute query to get the list

            // Check if news is empty
            if (news == null || !news.Any())
                return NotFound(new { message = "No news found" });

            // Prepare the result to send to the client
            var result = news.Select(c => new
            {
                c.Id,
                c.GeneralTitle,
                c.Content,
                c.Published,
                ImageUrl = string.IsNullOrEmpty(c.ImageId)
                    ? null
                    : supabase.Storage.From("news_img").GetPublicUrl(c.ImageId)
            }).ToList();

            return Ok(result);
        }

        [Route("/update-news-status")]
        [HttpPost]
        public async Task<IActionResult> UpdateNewsStatus([FromBody] dynamic data)
        {
            string id = data.id;
            bool published = data.published;
            var news = await _appDbContext.ContentNewses
                            .Where(c => c.Id == id)
                            .FirstOrDefaultAsync();
            if (news != null)
            {
                news.Published = published;
                await _appDbContext.SaveChangesAsync();
            }
            return Ok();
        }

        // Update blog
        [Route("/edit-news/{id}")]
        [HttpGet]
        public async Task<IActionResult> EditNews(string id)
        {
            Console.WriteLine("ID: " + id);
            // Retrieve the current user
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return NotFound("User not found");
            var news = await _appDbContext.ContentNewses
                .FirstOrDefaultAsync(c => c.Id == id && c.AuthorId == user.Id);
            if (news == null)
            {
                return NotFound();
            }
            return View(news);
        }

        [Route("/edit-news/{id}")]
        [HttpPost]
        public async Task<IActionResult> EditNews(IFormFile? ImageId, ContentNews contentNews, string id)
        {
            if (ModelState.IsValid)
            {
                // Lấy bài viết từ cơ sở dữ liệu
                var existingContentNews = await _appDbContext.ContentNewses.FindAsync(id);

                if (existingContentNews == null)
                {
                    return NotFound("Bài viết không tồn tại.");
                }

                // Cập nhật thông tin bài viết
                existingContentNews.GeneralTitle = contentNews.GeneralTitle;
                existingContentNews.DetailTitle = contentNews.DetailTitle;
                existingContentNews.GeneralDescription = contentNews.GeneralDescription;
                existingContentNews.DetailDescription = contentNews.DetailDescription;
                existingContentNews.Content = contentNews.Content;
                existingContentNews.DateUpdated = DateTime.Now.ToUniversalTime();

                // Xử lý file ảnh nếu có
                if (ImageId != null && ImageId.Length > 0)
                {
                    // Khởi tạo Supabase
                    var options = new Supabase.SupabaseOptions { AutoConnectRealtime = true };
                    var supabase = new Supabase.Client(_supabaseSettings.SupabaseUrl, _supabaseSettings.SupabaseAnonKey, options);
                    await supabase.InitializeAsync();

                    // Tạo tên file duy nhất
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + ImageId.FileName;

                    // Đọc file vào MemoryStream
                    using var memoryStream = new MemoryStream();
                    await ImageId.CopyToAsync(memoryStream);
                    var fileData = memoryStream.ToArray();

                    // Upload lên Supabase
                    var result = await supabase.Storage
                        .From("news_img")
                        .Upload(fileData, uniqueFileName, new Supabase.Storage.FileOptions
                        {
                            CacheControl = "3600",
                            Upsert = true
                        });

                    // Xóa ảnh cũ nếu cần
                    if (!string.IsNullOrEmpty(existingContentNews.ImageId))
                    {
                        await supabase.Storage.From("news_img").Remove(new List<string> { existingContentNews.ImageId });
                    }

                    // Cập nhật URL ảnh mới
                    existingContentNews.ImageId = uniqueFileName;
                }

                // Lưu thay đổi
                _appDbContext.ContentNewses.Update(existingContentNews);
                await _appDbContext.SaveChangesAsync();

                return Redirect("/news-blog");
            }

            var modelErrors = ModelState.Values.SelectMany(v => v.Errors)
                                               .Select(e => e.ErrorMessage)
                                               .ToList();
            return Content("ModelState Errors: " + string.Join(", ", modelErrors));
        }

        [Route("/delete-news/{id}")]
        [HttpPost]
        public async Task<IActionResult> DeleteNews(string id)
        {
            // Find news
            var existingContentNews = await _appDbContext.ContentNewses.FindAsync(id);
            if (existingContentNews == null)
                return NotFound("News is not existence");

            // Initial supabase
            var options = new Supabase.SupabaseOptions { AutoConnectRealtime = true };
            var supabase = new Supabase.Client(_supabaseSettings.SupabaseUrl, _supabaseSettings.SupabaseAnonKey, options);
            await supabase.InitializeAsync();

            // Exist image on supabase, delete it
            if (!string.IsNullOrEmpty(existingContentNews.ImageId))
                await supabase.Storage.From("news_img").Remove(new List<string> { existingContentNews.ImageId });

            // Delete news on database
            _appDbContext.ContentNewses.Remove(existingContentNews);
            await _appDbContext.SaveChangesAsync();
            return Ok();
        }
    }
}
