using Microsoft.AspNetCore.Mvc;

namespace App.Areas.Room
{
    [Area("Room")]
    public class RoomController : Controller
    {
        [Route("/information-room")]
        public IActionResult Index()
        {
            return View();
        }

        [Route("/edit-room/{id?}")]
        [HttpGet]
        public IActionResult EditRoom(int id)
        {
            return View();
        }

        [Route("/edit-home/{id?}")]
        [HttpGet]
        public IActionResult EditHome(int id)
        {
            return View();
        }


        [Route("/room-delete/{id?}")]
        [HttpGet]
        public IActionResult Delete(int id)
        {
            return Content(id.ToString());
        }

        [Route("/create-home")]
        public IActionResult CreateHome()
        {
            return View();
        }

        [Route("/create-room")]
        public IActionResult CreateRoom()
        {
            return View();
        }

        [Route("/view-room")]
        public IActionResult ViewRoom()
        {
            return View();
        }
    }
}
