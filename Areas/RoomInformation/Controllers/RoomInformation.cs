using Microsoft.AspNetCore.Mvc;

namespace App.Areas.RoomInformation
{
    [Area("RoomInformation")]
    public class RoomInformation : Controller
    {
        [Route("/room-information")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
