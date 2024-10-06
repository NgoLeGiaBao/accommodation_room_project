using Microsoft.AspNetCore.Mvc;
using App.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


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

        [Route("/edit-home/{id?}")]
        [HttpGet]
        public IActionResult EditHome(int id)
        {
            var rentalProperty = _appDbContext.RentalProperties.Find(id);
            if (rentalProperty != null)
            {
                return View(rentalProperty);
            }
            return NotFound();
        }

        [Route("/edit-home/{id?}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditHome(RentalProperty model)
        {
            if (ModelState.IsValid)
            {
                model.StartDate = model.StartDate.ToUniversalTime();
                model.IsActive = true;
                _appDbContext.RentalProperties.Update(model);
                _appDbContext.SaveChanges();
                TempData["SuccessMessage"] = "Rental property updated successfully.";
                return RedirectToAction("EditHome");
            }
            TempData["FailureMessage"] = "Rental property updated failurely, please try again.";
            return View(model);
        }

        [Route("/create-room/{homeId:int?}")]
        [HttpGet]
        public IActionResult CreateRoom()
        {
            return View();
        }

        [Route("/create-room/{homeId:int?}")]
        [HttpPost]
        public async Task<IActionResult> CreateRoomAsync(App.Models.Room room, int homeId)
        {
            var home = await _appDbContext.RentalProperties.FindAsync(homeId);
            if (home != null && ModelState.IsValid)
            {
                room.RentalPropertyId = home.Id;
                _appDbContext.Rooms.Add(room);
                await _appDbContext.SaveChangesAsync();
                TempData["SuccessMessage"] = "Room created successfully.";
                return RedirectToAction("CreateRoom");
            }
            TempData["FailureMessage"] = "Room created failurely, please try again.";
            return View(room);
        }

        [Route("/edit-room")]
        [HttpGet]
        public async Task<IActionResult> EditRoom(int homeId, int roomId)
        {
            if (homeId == 0 || roomId == 0)
                return NotFound();

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return NotFound();

            var room = _appDbContext.Rooms
                        .Include(r => r.RentalProperty)
                        .Where(r => r.Id == roomId
                        && r.RentalPropertyId == homeId
                        && r.RentalProperty.AppUserId == user.Id.ToString())
                        .FirstOrDefault();
            if (room == null)
                return NotFound();
            return View(room);
        }

        [Route("/edit-room")]
        [HttpPost]
        public async Task<IActionResult> EditRoom(App.Models.Room room, int homeId, int roomId)
        {
            if (ModelState.IsValid)
            {
                var existingRoom = await _appDbContext.Rooms.FindAsync(roomId);
                if (existingRoom == null)
                    return NotFound();

                existingRoom.RoomName = room.RoomName;
                existingRoom.Description = room.Description;
                if (existingRoom.RentalPropertyId != homeId)
                    existingRoom.RentalPropertyId = homeId;

                // Save the changes asynchronously
                await _appDbContext.SaveChangesAsync();
                TempData["SuccessMessage"] = "Room updated successfully.";
                return View(existingRoom);
            }
            TempData["FailureMessage"] = "Room update failed, please try again.";
            return View(room);
        }

        // [HttpPost]
        [Route("/get-list-rental-property")]
        public async Task<IActionResult> GetListRentalProperty()
        {
            try
            {
                // Check if the user is authenticated
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return NotFound(new { success = 0, message = "User not found" });
                }

                // Get rental properties owned by user
                var rentalProperties = await _appDbContext.RentalProperties
                    .Where(rp => rp.AppUserId == user.Id.ToString())
                    .Select(rp => new
                    {
                        rp.Id,
                        rp.PropertyName
                    })
                    .ToListAsync();

                return Json(new
                {
                    success = 1,
                    rentalProperties
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = 0, message = "Internal server error", error = ex.Message });
            }
        }

        // [HttpPost]
        [Route("/get-list-room/{homeId}")]
        public async Task<IActionResult> GetListRoom(int homeId)
        {
            try
            {
                // Lấy ID của người dùng hiện tại
                var currentUserId = _userManager.GetUserId(User);
                // Truy vấn danh sách phòng theo homeId và người dùng hiện tại
                var rooms = await _appDbContext.Rooms
                    .Where(r => r.RentalPropertyId == homeId && r.RentalProperty.AppUserId == currentUserId)
                    .Select(r => new
                    {
                        r.Id,
                        r.RoomName,
                        r.Price,
                        r.Status
                    })
                    .ToListAsync();
                return Json(rooms);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error.");
            }
        }







        [Route("/room-delete/{id?}")]
        [HttpGet]
        public IActionResult Delete(int id)
        {
            return Content(id.ToString());
        }


        [Route("/view-room")]
        public IActionResult ViewRoom()
        {
            return View();
        }
    }
}
