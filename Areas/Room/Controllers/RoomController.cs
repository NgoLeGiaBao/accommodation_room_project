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
                // Create a new UserRentalProperty entity to establish the relationship
                var userRentalProperty = new UserRentalProperty
                {
                    AppUserId = user.Id, // User ID from the current user
                    RentalPropertyId = model.Id // RentalPropertyId will be assigned after saving
                };

                // Check validation
                if (ModelState.IsValid)
                {
                    model.StartDate = model.StartDate.ToUniversalTime();
                    model.IsActive = true;

                    // Add the rental property first
                    _appDbContext.RentalProperties.Add(model);
                    await _appDbContext.SaveChangesAsync(); // Save changes to get the generated ID

                    // Now add the user-rental relationship
                    userRentalProperty.RentalPropertyId = model.Id; // Set RentalPropertyId after saving
                    _appDbContext.UserRentalProperties.Add(userRentalProperty);
                    await _appDbContext.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Rental property created successfully.";
                    return RedirectToAction("CreateHome");
                }
            }

            // Data is not validated
            TempData["FailureMessage"] = "Rental property creation failed, please try again.";
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
                // return RedirectToAction("CreateRoom");
                return View(room);
            }
            TempData["FailureMessage"] = "Room created failurely, please try again.";
            return View(room);
        }

        [Route("/edit-room")]
        [HttpGet]
        public async Task<IActionResult> EditRoom(int rentalPropertyId, int roomId)
        {
            if (rentalPropertyId == 0 || roomId == 0)
                return NotFound();

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return NotFound();


            var room = await _appDbContext.Rooms
                .Include(r => r.RentalProperty)
                .ThenInclude(rp => rp.UserRentalProperties)
                .FirstOrDefaultAsync(r => r.Id == roomId &&
                                           r.RentalPropertyId == rentalPropertyId &&
                                           r.RentalProperty.UserRentalProperties.Any(urp => urp.AppUserId == user.Id));

            if (room == null)
                return NotFound();

            return View(room);
        }

        [Route("/edit-room")]
        [HttpPost]
        public async Task<IActionResult> EditRoom(App.Models.Room room, int rentalPropertyId, int roomId)
        {
            if (ModelState.IsValid)
            {
                var existingRoom = await _appDbContext.Rooms.FindAsync(roomId);
                if (existingRoom == null)
                    return NotFound();

                existingRoom.RoomName = room.RoomName;
                existingRoom.Description = room.Description;
                if (existingRoom.RentalPropertyId != rentalPropertyId)
                    existingRoom.RentalPropertyId = rentalPropertyId;

                // Save the changes asynchronously
                await _appDbContext.SaveChangesAsync();
                TempData["SuccessMessage"] = "Room updated successfully.";
                return View(existingRoom);
            }
            TempData["FailureMessage"] = "Room update failed, please try again.";
            return View(room);
        }

        [Route("/get-list-rental-property")]
        [HttpPost]
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

                // Get rental properties associated with the user through the UserRentalProperty relationship
                var rentalProperties = await _appDbContext.UserRentalProperties
                    .Where(urp => urp.AppUserId == user.Id) // Check User ID directly
                    .Select(urp => new
                    {
                        RentalPropertyId = urp.RentalPropertyId,
                        PropertyName = urp.RentalProperty.PropertyName // Ensure you include the property name
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

        [Route("/get-list-room/{homeId}")]
        // [HttpPost]
        public async Task<IActionResult> GetListRoom(int homeId, int pageNumber = 1, int pageSize = 8)
        {
            try
            {
                var currentUserId = _userManager.GetUserId(User);

                // Count total rooms belonging to the user's rental property
                var totalCount = await _appDbContext.UserRentalProperties
                    .Where(ur => ur.AppUserId == currentUserId && ur.RentalPropertyId == homeId)
                    .SelectMany(ur => ur.RentalProperty.Rooms) // Lấy danh sách các phòng từ RentalProperty
                    .CountAsync();

                // Fetch the correct page of rooms
                var rooms = await _appDbContext.UserRentalProperties
                    .Where(ur => ur.AppUserId == currentUserId && ur.RentalPropertyId == homeId)
                    .SelectMany(ur => ur.RentalProperty.Rooms) // Lấy danh sách các phòng từ RentalProperty
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(r => new
                    {
                        r.Id,
                        r.RoomName,
                        r.Price,
                        r.Status
                    })
                    .ToListAsync();

                return Json(new { totalCount, rooms });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = 0, message = "Internal server error.", error = ex.Message });
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
