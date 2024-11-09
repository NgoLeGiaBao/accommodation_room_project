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
        public async Task<IActionResult> Index()
        {
            // Get rental properties by user
            var user = await _userManager.GetUserAsync(User);
            var rentalProperties = _appDbContext.UserRentalProperties
                                .Where(r => r.AppUserId == user.Id)
                                .ToList();

            ViewBag.HasRentalProperties = false;
            if (rentalProperties != null && rentalProperties.Any())
                ViewBag.HasRentalProperties = true;
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
                    AppUserId = user.Id,
                    RentalPropertyId = model.Id
                };


                if (ModelState.IsValid)
                {
                    model.StartDate = model.StartDate.ToUniversalTime();
                    model.IsActive = true;

                    // Add the rental property first
                    _appDbContext.RentalProperties.Add(model);
                    await _appDbContext.SaveChangesAsync(); // Save changes to get the generated ID

                    // Create a new room and link it to the created rental property
                    App.Models.Room room = new Models.Room();
                    room.RoomName = "Warehouse";
                    room.Description = "Warehouse is used to store items.";
                    room.RentalPropertyId = model.Id; // Link the room to the rental property

                    // Now add the user-rental relationship
                    userRentalProperty.RentalPropertyId = model.Id; // Set RentalPropertyId after saving
                    _appDbContext.Rooms.Add(room);
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
        public async Task<IActionResult> EditHome(string id)
        {
            var rentalProperty = await _appDbContext.RentalProperties
                .FirstOrDefaultAsync(rp => rp.Id == id);
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

        [Route("/create-room/{homeId}")]
        [HttpGet]
        public IActionResult CreateRoom()
        {
            return View();
        }

        [Route("/create-room/{homeId}")]
        [HttpPost]
        public async Task<IActionResult> CreateRoomAsync(App.Models.Room room, string homeId)
        {
            // Find home by Id
            var home = await _appDbContext.RentalProperties.FindAsync(homeId);
            if (home == null)
                return NotFound();

            // Check model is valid
            if (ModelState.IsValid)
            {
                room.RentalPropertyId = home.Id;
                room.IsActive = true;
                room.Status = "Available";
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
        public async Task<IActionResult> EditRoom(string rentalPropertyId, string roomId)
        {
            if (rentalPropertyId == null || roomId == null)
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
        public async Task<IActionResult> EditRoom(App.Models.Room room, string rentalPropertyId, string roomId)
        {
            if (ModelState.IsValid)
            {
                var existingRoom = await _appDbContext.Rooms
                    .FirstOrDefaultAsync(r => r.Id == roomId);
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
                // Get the currently logged-in user
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return NotFound(new { success = 0, message = "User not found" });
                }

                // Get rental properties associated with the user through the UserRentalProperty relationship
                var rentalProperties = await _appDbContext.UserRentalProperties
                    .Where(urp => urp.AppUserId == user.Id)
                    .Select(urp => new
                    {
                        RentalPropertyId = urp.RentalPropertyId,
                        PropertyName = urp.RentalProperty.PropertyName
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
        public async Task<IActionResult> GetListRoom(string homeId, int pageNumber = 1, int pageSize = 8, string selectedStatus = null)
        {
            try
            {
                // Get the currently logged-in user
                var currentUserId = _userManager.GetUserId(User);
                var userRentalRoomsQuery = _appDbContext.UserRentalProperties
                    .Where(ur => ur.AppUserId == currentUserId && ur.RentalPropertyId == homeId)
                    .SelectMany(ur => ur.RentalProperty.Rooms)
                    .Where(r => r.IsActive == true)
                    .OrderBy(r => r.RoomName);

                // Count total rooms belonging to the user's rental property
                var totalCount = await userRentalRoomsQuery.CountAsync();

                // Count total rooms belonging to the user's rental property
                var roomStatusCounts = await userRentalRoomsQuery
                    .GroupBy(r => r.Status)
                    .Select(group => new
                    {
                        Status = group.Key,
                        Count = group.Count()
                    })
                    .ToListAsync();

                // Map the counts to the respective statuses
                var availableCount = roomStatusCounts.FirstOrDefault(c => c.Status == "Available")?.Count ?? 0;
                var aboutToExpireCount = roomStatusCounts.FirstOrDefault(c => c.Status == "About to expire")?.Count ?? 0;
                var rentedCount = roomStatusCounts.FirstOrDefault(c => c.Status == "Rented")?.Count ?? 0;

                // Fetch the correct page of rooms
                if (selectedStatus == null || selectedStatus == "All")
                {
                    var rooms = await _appDbContext.UserRentalProperties
                                    .Where(ur => ur.AppUserId == currentUserId && ur.RentalPropertyId == homeId)
                                    .SelectMany(ur => ur.RentalProperty.Rooms)
                                    .Where(r => r.IsActive == true)
                                    .OrderBy(r => r.RoomName)
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
                    return Json(new { totalCount, rooms, availableCount, aboutToExpireCount, rentedCount });
                }
                else
                {
                    var rooms = await _appDbContext.UserRentalProperties
                                    .Where(ur => ur.AppUserId == currentUserId && ur.RentalPropertyId == homeId)
                                    .SelectMany(ur => ur.RentalProperty.Rooms)
                                    .Where(r => r.IsActive == true && r.Status == selectedStatus)
                                    .OrderBy(r => r.RoomName)
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
                    totalCount = rooms.Count;

                    return Json(new { totalCount, rooms, availableCount, aboutToExpireCount, rentedCount });
                }
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
        public async Task<IActionResult> ViewRoom(string rentalPropertyId, string roomId)
        {
            if (string.IsNullOrEmpty(rentalPropertyId) || string.IsNullOrEmpty(roomId))
                return NotFound();

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return NotFound();

            // Fetch the room with rental contracts and include AppUser
            var room = await _appDbContext.Rooms
                .Include(r => r.RentalProperty)
                    .ThenInclude(rp => rp.UserRentalProperties)
                .Include(r => r.RentalContracts)
                    .ThenInclude(rc => rc.AppUser)
                .Include(r => r.OwnAssets)
                    .ThenInclude(oa => oa.Asset)
                .FirstOrDefaultAsync(r => r.Id == roomId &&
                               r.RentalPropertyId == rentalPropertyId &&
                               r.RentalProperty.UserRentalProperties.Any(urp => urp.AppUserId == user.Id));

            if (room == null)
                return NotFound();

            // Get the active tenants
            var currentDate = DateTime.Now;
            var activeTenants = (room.RentalContracts ?? Enumerable.Empty<RentalContract>())
                .Where(rc => rc.StartedDate <= currentDate && rc.EndupDate >= currentDate)
                .Select(rc => rc.AppUser)
                .Distinct()
                .Where(tenant => tenant != null)
                .ToList();
            // Time contract active
            var activeContract = room.RentalContracts?
                            .FirstOrDefault(rc => rc.StartedDate <= currentDate && rc.EndupDate >= currentDate);

            ViewBag.ContractStartDate = activeContract?.StartedDate.ToString("dd/MM/yyyy");
            ViewBag.ContractEndDate = activeContract?.EndupDate.ToString("dd/MM/yyyy");

            var assets = room.OwnAssets.Select(oa => oa.Asset).ToList() ?? new List<Asset>();
            ViewBag.ActiveTenants = activeTenants;
            ViewBag.Assets = assets;
            return View(room);
        }
    }
}
