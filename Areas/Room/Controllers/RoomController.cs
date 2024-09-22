using Microsoft.AspNetCore.Mvc;

namespace App.Areas.Room
{
    [Area("Room")]
    public class RoomController : Controller
    {
        [Route("/room-information")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
