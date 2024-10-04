using Microsoft.AspNetCore.Mvc;

namespace App.Areas.DashBoard
{
    [Area("DashBoard")]
    public class DashBoardController : Controller
    {
        [Route("/dashboard")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
