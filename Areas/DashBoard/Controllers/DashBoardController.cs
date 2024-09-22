using Microsoft.AspNetCore.Mvc;

namespace App.Areas.DashBoard
{
    [Area("DashBoard")]
    public class DashBoardController : Controller
    {
        [Route("/")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
