using Microsoft.AspNetCore.Mvc;
using App.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
namespace App.Areas.Contract
{
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
        public IActionResult Index()
        {
            return View();
        }

        // Get information and equipment room
        [Route("/get-room-information/{roomId:int}")]
        public IActionResult GetRoomInformation(int roomId)
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

        [Route("/create-contract/{homeId:int?}")]
        [HttpGet]
        public IActionResult CreateContract(int homeId)
        {
            // Get LandloardInformation 
            var landloardInformation = _appDbContext.RentalProperties.Find(homeId);
            if (landloardInformation == null)
            {
                return NotFound("Không tìm thấy thông tin về bất động sản này.");
            }

            // Get All Room from rental property
            var rooms = _appDbContext.Rooms
                .Where(r => r.RentalPropertyId == homeId)
                .Select(r => new SelectListItem
                {
                    Value = r.Id.ToString(),
                    Text = r.RoomName
                }).ToList();


            ViewData["LandloardInformation"] = landloardInformation;
            ViewData["Rooms"] = rooms;

            return View();
        }

        [Route("/create-contract/{homeId:int?}")]
        [HttpPost]
        public async Task<IActionResult> CreateContract(InputModel inputModel, int homeId)
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
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // Check tenant exist based on IdentityCard
                var existingUser = _appDbContext.Users.FirstOrDefault(u => u.IdentityCard == inputModel.appUser.IdentityCard);
                if (existingUser == null)
                {
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
                        var errors = result.Errors.Select(e => e.Description).ToList();
                        return Content("Errors: " + string.Join(", ", errors));
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
                        var errors = updateResult.Errors.Select(e => e.Description).ToList();
                        return Content("Errors: " + string.Join(", ", errors));
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
                await _appDbContext.SaveChangesAsync();

                // Chuyển hướng sau khi thành công
                return Content("Create contract successfully");
            }

            // Nếu ModelState không hợp lệ, hiển thị các lỗi
            var modelErrors = ModelState.Values.SelectMany(v => v.Errors)
                                               .Select(e => e.ErrorMessage)
                                               .ToList();
            return Content("ModelState Errors: " + string.Join(", ", modelErrors));
        }
    }
}
