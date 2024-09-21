using Microsoft.AspNetCore.Mvc;

namespace App.Areas.DashBoard
{
    [Area("DashBoard")]
    public class DashBoard : Controller
    {
        [Route("/")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
