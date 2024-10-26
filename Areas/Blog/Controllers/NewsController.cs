using App.Models.NewsModel;
using App.Services;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using System.Text;

namespace App.Areas.Blog
{
    [Area("Blog")]
    public class NewsController : Controller
    {
        private readonly SupabaseSettings _supabaseSettings;

        public NewsController(IOptions<SupabaseSettings> supabaseSettings)
        {
            _supabaseSettings = supabaseSettings.Value;
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
                contentNews.ImageId = publicUrl; // Assuming ContentNews has an ImageId property
            }

            // Optionally save contentNews to your database here

            return Content("Success");
        }
    }
}
