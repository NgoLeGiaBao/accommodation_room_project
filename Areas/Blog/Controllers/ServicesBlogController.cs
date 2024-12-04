using App.Models;
using App.Models.ServicesModel;
using App.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;


namespace App.Areas.Blog
{
    [Area("Blog")]
    public class ServicesBlogController : Controller
    {
        private readonly SupabaseSettings _supabaseSettings;
        private readonly AppDbContext _appDbContext;
        private readonly UserManager<AppUser> _userManager;
        public ServicesBlogController(IOptions<SupabaseSettings> supabaseSettings, AppDbContext appDbContext, UserManager<AppUser> userManager)
        {
            _supabaseSettings = supabaseSettings.Value;
            _appDbContext = appDbContext;
            _userManager = userManager;
        }
        [Route("/services-blog")]
        public IActionResult Index()
        {
            return View();
        }

        [Route("/create-services-news")]
        [HttpGet]
        public IActionResult CreateServiceNews()
        {
            ServicesBlog servicesBlog = new ServicesBlog();
            servicesBlog.Rooms.Add(new RooomInServiceBlog());
            return View(servicesBlog);
        }

        [Route("/create-services-news")]
        [HttpPost]
        public async Task<IActionResult> CreateServiceNews(ServicesBlog servicesBlog, List<IFormFile> filesRoom, List<IFormFile> filesArea)
        {
            // Process the uploaded image if it exists
            // Initialize Supabase
            var options = new Supabase.SupabaseOptions { AutoConnectRealtime = true };
            var supabase = new Supabase.Client(_supabaseSettings.SupabaseUrl, _supabaseSettings.SupabaseAnonKey, options);
            await supabase.InitializeAsync();

            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                    return NotFound("User not found");
                // Insert data into Service Blog
                var roomInServiceBlog = servicesBlog.Rooms[0];

                // Upload images for areas
                List<string> areaImages = new List<string>();
                if (filesArea != null && filesArea.Count > 0)
                {
                    foreach (var file in filesArea)
                    {
                        var fileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                        using var memoryStream = new MemoryStream();
                        await file.CopyToAsync(memoryStream);
                        var fileData = memoryStream.ToArray();
                        await supabase.Storage
                            .From("home_img")
                            .Upload(fileData, fileName, new Supabase.Storage.FileOptions
                            {
                                CacheControl = "3600",
                                Upsert = true
                            });
                        areaImages.Add(fileName);
                    }
                }

                // Upload images for rooms
                List<string> roomImages = new List<string>();
                if (filesRoom != null && filesRoom.Count > 0)
                {
                    foreach (var file in filesRoom)
                    {
                        var fileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                        using var memoryStream = new MemoryStream();
                        await file.CopyToAsync(memoryStream);
                        var fileData = memoryStream.ToArray();
                        await supabase.Storage
                            .From("room_in_ser_img")
                            .Upload(fileData, fileName, new Supabase.Storage.FileOptions
                            {
                                CacheControl = "3600",
                                Upsert = true
                            });
                        roomImages.Add(fileName);
                    }
                }

                // Add data Service Blog
                servicesBlog.Images = areaImages;
                servicesBlog.CreatedBy = user.Id;
                servicesBlog.StatusBlog = "Active";
                _appDbContext.ServicesBlogs.Add(servicesBlog);

                // Insert data into Room In Service Blog
                var serviceBlogId = servicesBlog.ServicesBlogId;
                roomInServiceBlog.ServicesBlogId = serviceBlogId;
                roomInServiceBlog.ImageUrls = roomImages;
                _appDbContext.RooomInServiceBlogs.Add(roomInServiceBlog);

                await _appDbContext.SaveChangesAsync();
                TempData["SuccessMessage"] = "New Services Blog created successfully.";
                return RedirectToAction("CreateServiceNews");
            }

            TempData["FailureMessage"] = "New Services Blog created failed, please try again.";
            return View(servicesBlog);
        }

        [Route("/get-all-services-news")]
        [HttpGet]
        public async Task<IActionResult> GetAllServicesNews()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return NotFound("User not found");
            try
            {
                // Get service blog
                var servicesBlogs = await _appDbContext.ServicesBlogs
                    .Where(sb => sb.CreatedBy == user.Id)
                    .Include(sb => sb.Rooms)
                    .ToListAsync();

                // Chuyển dữ liệu sang dạng DTO để gửi về client
                var response = servicesBlogs.Select(blog => new
                {
                    blog.ServicesBlogId,
                    blog.Title,
                    blog.RentalPrice,
                    blog.Address,
                    blog.Province,
                    blog.District,
                    blog.Town,
                    blog.Images,
                    Rooms = blog.Rooms.Select(room => new
                    {
                        room.RooomInServiceBlogId,
                        room.Name,
                        room.RentalPrice,
                        room.ImageUrls
                    }).ToList()
                });

                // Trả về JSON response
                return Ok(response);
            }
            catch (Exception ex)
            {
                // Xử lý lỗi và trả về thông báo lỗi
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}