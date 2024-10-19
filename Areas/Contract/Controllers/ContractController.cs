using Microsoft.AspNetCore.Mvc;
using App.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
namespace App.Areas.Contract
{
    [Authorize]
    [Area("Contract")]
    public class ContractController : Controller
    {
        private readonly AppDbContext _appDbContext;
        private readonly UserManager<AppUser> _userManager;

        public ContractController(AppDbContext appDbContext, UserManager<AppUser> userManager)
        {
            _appDbContext = appDbContext;
            _userManager = userManager;
        }
        [Route("/contract")]
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

        // Get information and equipment room
        [Route("/get-room-information/{roomId}")]
        public IActionResult GetRoomInformation(string roomId)
        {
            // Fetch the room details
            var room = _appDbContext.Rooms
                .Where(r => r.Id == roomId)
                .Select(r => new
                {
                    r.RoomName,
                    r.Area,
                    r.MaximumNumberOfPeople,
                    r.Price,
                    r.Description,
                    r.RentalPropertyId

                })
                .FirstOrDefault();

            if (room == null)
            {
                return NotFound(new { success = false, message = "Room not found." });
            }

            // Fetch the assets associated with this room
            var assets = (from ownAsset in _appDbContext.OwnAssets
                          join asset in _appDbContext.Assets on ownAsset.AssetID equals asset.AssetID
                          where ownAsset.RoomID == roomId
                          select new
                          {
                              asset.AssetName,
                              asset.CategoryAssetID,
                              asset.PurchaseDate,
                              asset.Cost,
                              asset.Condition,
                              asset.Location,
                              asset.NextMaintenanceDueDate
                          }).ToList();

            // Return room and asset information
            return Ok(new
            {
                success = true,
                room,
                assets
            });
        }

        [Route("/create-contract/{homeId}")]
        [HttpGet]
        public IActionResult CreateContract(string homeId)
        {
            // Get LandloardInformation 
            var landloardInformation = _appDbContext.RentalProperties.Find(homeId);
            if (landloardInformation == null)
            {
                return NotFound();
            }

            // Get All Room from rental property
            var rooms = _appDbContext.Rooms
                .Where(r => r.RentalPropertyId == homeId && r.IsActive == true)
                .Select(r => new SelectListItem
                {
                    Value = r.Id.ToString(),
                    Text = r.RoomName
                }).ToList();


            ViewData["LandloardInformation"] = landloardInformation;
            ViewData["Rooms"] = rooms;

            return View();
        }

        [Route("/create-contract/{homeId}")]
        [HttpPost]
        public async Task<IActionResult> CreateContract(InputModel inputModel, string homeId)
        {
            // Check RentalProperty exist
            var landloardInformation = _appDbContext.RentalProperties.Find(homeId);
            if (landloardInformation == null) return NotFound();

            // Check room exist
            var roomExists = _appDbContext.Rooms.FirstOrDefault(r => r.Id == inputModel.RoomId);
            var rentalPropertyExists = _appDbContext.RentalProperties.FirstOrDefault(r => r.Id == homeId);
            if (roomExists == null || rentalPropertyExists == null)
            {
                // Process here
                // TempData["FailureMessage"] = "Create contract failure, please try again.";
                // return View(inputModel);
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // Check tenant exist based on IdentityCard
                var existingUser = _appDbContext.Users.FirstOrDefault(u => u.IdentityCard == inputModel.appUser.IdentityCard);
                bool wasOwned = true;
                if (existingUser == null)
                {
                    wasOwned = false;
                    existingUser = new AppUser
                    {
                        UserName = inputModel.appUser.IdentityCard,
                        Email = inputModel.appUser.Email,
                        IdentityCard = inputModel.appUser.IdentityCard,
                        FullName = inputModel.appUser.FullName,
                        Address = inputModel.appUser.Address,
                        Sex = inputModel.appUser.Sex,
                    };

                    // Create new user with default password
                    // Process here with random password
                    var result = await _userManager.CreateAsync(existingUser, "UserPassword123!");
                    if (!result.Succeeded)
                    {
                        // Create a user failure, process here
                        // var errors = result.Errors.Select(e => e.Description).ToList();
                        // return Content("Errors: " + string.Join(", ", errors));
                        TempData["FailureMessage"] = "Create contract failure, please try again.";
                        return View(inputModel);
                    }
                }
                else
                {
                    existingUser.Email = inputModel.appUser.Email;
                    existingUser.FullName = inputModel.appUser.FullName;
                    existingUser.Address = inputModel.appUser.Address;
                    existingUser.Sex = inputModel.appUser.Sex;

                    var updateResult = await _userManager.UpdateAsync(existingUser);
                    if (!updateResult.Succeeded)
                    {
                        // Process here to return view
                        // var errors = updateResult.Errors.Select(e => e.Description).ToList();
                        // return Content("Errors: " + string.Join(", ", errors));
                        TempData["FailureMessage"] = "Create contract failure, please try again.";
                        return View(inputModel);
                    }
                }

                // Create new rental contract
                var rentalContract = new RentalContract
                {
                    UserID = existingUser.Id,
                    RoomID = inputModel.RoomId,
                    StartedDate = inputModel.rentalContract.StartedDate.ToUniversalTime(),
                    EndupDate = inputModel.rentalContract.EndupDate.ToUniversalTime(),
                    PElectricityPerKw = landloardInformation.ElectricityPrice,
                    PWaterPerK = landloardInformation.WaterPrice,
                    PServicePerK = 1000,
                    PRentalRoomPerM = roomExists.Price,
                    Rules = inputModel.rentalContract.Rules,
                    PersonalSignContract = existingUser.Id
                };

                _appDbContext.RentalContracts.Add(rentalContract);

                // Create new user rental property
                if (!wasOwned)
                {
                    var userRentalProperties = new UserRentalProperty
                    {
                        AppUserId = existingUser.Id,
                        RentalPropertyId = homeId
                    };
                    _appDbContext.UserRentalProperties.Add(userRentalProperties);
                }
                await _appDbContext.SaveChangesAsync();
                TempData["SuccessMessage"] = "Contracrt created successfully.";
                return RedirectToAction("CreateContract");
            }
            TempData["FailureMessage"] = "Create contract failure, please try again.";
            return View(inputModel);
        }

        // Get list contract
        [Route("/get-list-contracts/{homeId}")]
        [HttpGet]
        public IActionResult GetListContracts(string homeId)
        {
            var currentDate = DateTime.Now;

            // Retrieve contracts based on the homeId
            var contracts = _appDbContext.RentalContracts
                .Include(c => c.Room)   // Ensure Room is included
                .Include(c => c.AppUser) // Include AppUser for tenant name retrieval
                .Where(c => c.Room.RentalPropertyId == homeId) // Adjust this to match the correct foreign key relationship
                .ToList();

            // Initialize counters
            int allContracts = contracts.Count;
            int pendingContracts = 0;
            int activeContracts = 0;
            int completedContracts = 0;

            // Categorize contracts based on the date
            foreach (var contract in contracts)
            {
                if (contract.StartedDate > currentDate)
                {
                    pendingContracts++;
                }
                else if (contract.StartedDate <= currentDate && contract.EndupDate >= currentDate)
                {
                    activeContracts++;
                }
                else if (contract.EndupDate < currentDate)
                {
                    completedContracts++;
                }
            }

            // Prepare the response
            var result = new
            {
                allContracts,
                pendingContracts,
                activeContracts,
                completedContracts,
                contracts = contracts.Select(c => new
                {
                    c.ContractID,
                    RoomName = c.Room.RoomName,
                    TenantName = _appDbContext.AppUsers.FirstOrDefault(u => u.Id == c.PersonalSignContract)?.FullName,
                    StartingDate = c.StartedDate.ToString("yyyy-MM-dd"),
                    ExpirationDate = c.EndupDate.ToString("yyyy-MM-dd")
                })
            };

            return Json(result);
        }
    }
}
