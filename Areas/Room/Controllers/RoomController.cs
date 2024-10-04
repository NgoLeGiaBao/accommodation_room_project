using Microsoft.AspNetCore.Mvc;
using App.Models;
using System.ComponentModel;
using Microsoft.AspNetCore.Authorization;

namespace App.Areas.Room
{
    [Authorize]
    [Area("Room")]
    public class RoomController : Controller
    {
        private readonly AppDbContext _appDbContext;
        public RoomController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }


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
        [HttpGet]
        public IActionResult CreateHome()
        {
            return View();
        }

        [Route("/create-home")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateHome(RentalProperty model, IFormFile propertyImage)
        {
            model.StartDate = model.StartDate.ToUniversalTime();
            model.IsActive = true;
            _appDbContext.RentalProperties.Add(model);
            await _appDbContext.SaveChangesAsync();
            return Content(model.PropertyName.ToString());
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
