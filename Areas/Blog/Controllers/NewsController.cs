using App.Models.NewsModel;
using App.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using App.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace App.Areas.Blog
{
    [Area("Blog")]
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
        [HttpGet]
        public async Task<IActionResult> GetUserNews()
        {
            // Initialize Supabase client
            var options = new Supabase.SupabaseOptions { AutoConnectRealtime = true };
            var supabase = new Supabase.Client(_supabaseSettings.SupabaseUrl, _supabaseSettings.SupabaseAnonKey, options);
            await supabase.InitializeAsync();

            // Retrieve the current user
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return NotFound("User not found");

            // Fetch all news articles for the current user
            var news = await _appDbContext.ContentNewses
                .Where(c => c.AuthorId == user.Id)
                .ToListAsync(); // Get the list first

            // Check if news is empty
            if (news == null || !news.Any())
                return NotFound("No news found.");


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
    }
}
