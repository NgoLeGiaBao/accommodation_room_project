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
    }
}