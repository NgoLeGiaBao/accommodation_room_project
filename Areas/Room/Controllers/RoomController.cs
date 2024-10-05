using Microsoft.AspNetCore.Mvc;
using App.Models;
using System.ComponentModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace App.Areas.Room
{
    [Authorize]
    [Area("Room")]
    public class RoomController : Controller
    {
        private readonly AppDbContext _appDbContext;
        private readonly UserManager<AppUser> _userManager;


        public RoomController(AppDbContext appDbContext, UserManager<AppUser> userManager)
        {
            _appDbContext = appDbContext;
            _userManager = userManager;
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
        public async Task<IActionResult> CreateHome(RentalProperty model, IFormFile[]? propertyImage)
        {
            // Get user logged in
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                model.AppUserId = user.Id.ToString();
            }

            // Check validation
            if (ModelState.IsValid)
            {
                model.StartDate = model.StartDate.ToUniversalTime();
                model.IsActive = true;
                _appDbContext.RentalProperties.Add(model);
                await _appDbContext.SaveChangesAsync();
                TempData["SuccessMessage"] = "Rental property created successfully.";
                return RedirectToAction("CreateHome");
            }

            // Data is not validated
            TempData["FailureMessage"] = "Rental property created successfully, please try again.";
            return View(model);
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
