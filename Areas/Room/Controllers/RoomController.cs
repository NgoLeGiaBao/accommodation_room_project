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

        [Route("/room-edit/{id?}")]
        [HttpGet]
        public IActionResult Edit(int id)
        {
            return Content(id.ToString());
        }

        [Route("/room-delete/{id?}")]
        [HttpGet]
        public IActionResult Delete(int id)
        {
            return Content(id.ToString());
        }

        [Route("/room-create")]
        public IActionResult Create ()
        {
            return View();
        }
    }
}
