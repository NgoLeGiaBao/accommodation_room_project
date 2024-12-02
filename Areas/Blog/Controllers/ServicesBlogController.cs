using App.Models.ServicesModel;
using Microsoft.AspNetCore.Mvc;

namespace App.Areas.Blog
{
    [Area("Blog")]
    public class ServicesBlogController : Controller
    {
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
            servicesBlog.IsStudent = true;
            servicesBlog.HasBathroom = true;
            servicesBlog.NearGym = true;
            servicesBlog.Rooms.Add(new RooomInServiceBlog());
            return View(servicesBlog);
        }

        [Route("/create-services-news")]
        [HttpPost]
        public IActionResult CreateServiceNews(ServicesBlog servicesBlog, List<IFormFile> filesRoom, List<IFormFile> filesArea)
        {
            var results = "";

            // Xử lý các tệp trong 'filesRoom'
            if (filesRoom != null && filesRoom.Any())
            {
                // Lấy danh sách tên tệp hình ảnh từ 'filesRoom'
                var roomFileNames = filesRoom.Select(file => file.FileName).ToList();

                // Xử lý tệp (lưu trữ, hoặc xử lý thêm...)
                // Ví dụ: Lưu các tệp vào thư mục hoặc cơ sở dữ liệu
                results = $"Room files: {string.Join(", ", roomFileNames)}";
            }

            // Xử lý các tệp trong 'filesArea'
            if (filesArea != null && filesArea.Any())
            {
                var areaFileNames = filesArea.Select(file => file.FileName).ToList();

                // Xử lý tệp (lưu trữ, hoặc xử lý thêm...)
                results += $"\nArea files: {string.Join(", ", areaFileNames)}";
            }
            return Content(results);
        }
    }
}