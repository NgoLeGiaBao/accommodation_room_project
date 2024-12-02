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
        public IActionResult CreateServiceNews(ServicesBlog servicesBlog, List<IFormFile> files)
        {
            if (files != null && files.Any())
            {
                var name = "";
                foreach (var file in files)
                {
                    if (file.Length > 0)
                    {
                        name = name + "/" + file.FileName;
                    }
                }
                return Content(name);
            }
            // return Content(servicesBlog.Title + "/" + servicesBlog.Province + "/" + servicesBlog.District + "/" + servicesBlog.Town);
            // return Content(servicesBlog.Rooms[0].Desk.ToString());
            return Content(servicesBlog.ContentDescription + "/" + servicesBlog.Rooms[0].Description);

        }
    }
}