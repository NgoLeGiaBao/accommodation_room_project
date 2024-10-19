using Microsoft.AspNetCore.Mvc;
using App.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;


namespace Areas.AssetAndEquipment
{
    [Authorize]
    [Area("AssetAndEquipment")]
    public class AssetAndEquipmentController : Controller
    {
        private readonly AppDbContext _appDbContext;
        private readonly UserManager<AppUser> _userManager;

        public AssetAndEquipmentController(AppDbContext appDbContext, UserManager<AppUser> userManager)
        {
            _appDbContext = appDbContext;
            _userManager = userManager;
        }

        [Route("/asset-equipment")]
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


        [Route("/create-asset-equipment/{homeId}")]
        [HttpGet]
        public IActionResult CreateAssetAndEquipment(string homeId)
        {
            var rooms = _appDbContext.Rooms
                .Where(r => r.RentalPropertyId == homeId)
                .Select(r => new SelectListItem
                {
                    Value = r.Id.ToString(),
                    Text = r.RoomName
                }).ToList();

            var categories = _appDbContext.CategoryAssets
                .Select(c => new SelectListItem
                {
                    Value = c.CategoryAssetID.ToString(),
                    Text = c.Name
                }).ToList();


            ViewData["Rooms"] = rooms;
            ViewData["Categories"] = categories;

            return View();
        }

        [Route("/create-asset-equipment/{homeId}")]
        [HttpPost]
        public async Task<IActionResult> CreateAssetAndEquipmentAsync(Asset assetAndEquipment, string homeId)
        {
            // Check if the rental property exists
            var rentalProperty = await _appDbContext.RentalProperties.FindAsync(homeId);
            if (rentalProperty == null)
                return NotFound();

            // Always populate categories for the dropdown
            ViewData["Categories"] = new SelectList(await _appDbContext.CategoryAssets.ToListAsync(), "CategoryAssetID", "Name");

            if (ModelState.IsValid)
            {
                assetAndEquipment.PurchaseDate = assetAndEquipment.PurchaseDate.ToUniversalTime();
                assetAndEquipment.NextMaintenanceDueDate = assetAndEquipment.NextMaintenanceDueDate?.ToUniversalTime();

                assetAndEquipment.Condition = "Using ";

                await _appDbContext.Assets.AddAsync(assetAndEquipment);
                await _appDbContext.SaveChangesAsync();

                var ownAsset = new OwnAsset
                {
                    CreatedDate = DateTime.UtcNow,
                    AssetID = assetAndEquipment.AssetID,
                    RoomID = assetAndEquipment.Location
                };


                await _appDbContext.OwnAssets.AddAsync(ownAsset);
                await _appDbContext.SaveChangesAsync();
                TempData["SuccessMessage"] = "Crated a new asset and equipment successfully.";
                return RedirectToAction("CreateAssetAndEquipment");
            }

            ViewData["Rooms"] = new SelectList(await _appDbContext.Rooms.Where(r => r.RentalPropertyId == homeId).ToListAsync(), "Id", "RoomName");
            TempData["FailureMessage"] = "Crate a assest and equipment failed, please try again.";
            return View(assetAndEquipment);
        }

        [Route("/get-list-assets/{homeId}")]
        // [HttpPost]
        public async Task<IActionResult> GetAssets(string homeId)
        {
            // var now = DateTime.UtcNow; // Ensure you're using UTC
            // var assets = await _appDbContext.Assets
            //     .Where(a => a.OwnAssets.Any(o => o.Room.RentalPropertyId == homeId))
            //     .Select(a => new
            //     {
            //         a.AssetID,
            //         a.AssetName,
            //         Category = a.CategoryAsset.Name,
            //         PurchaseDate = a.PurchaseDate,
            //         Cost = a.Cost,
            //         Condition = a.Condition,
            //         RoomName = a.OwnAssets.Select(o => o.Room.RoomName).FirstOrDefault(), // Get the room name
            //         NextMaintenanceDueDate = a.NextMaintenanceDueDate,
            //         DaysUntilDue = a.NextMaintenanceDueDate.HasValue
            //             ? (a.NextMaintenanceDueDate.Value - now).Days
            //             : (int?)null
            //     })
            //     .ToListAsync();

            // return Json(new { assets });

            var now = DateTime.UtcNow; // Sử dụng UTC cho ngày giờ hiện tại
            var maintenanceThreshold = now.AddDays(30); // Ngưỡng bảo dưỡng: 30 ngày từ hiện tại

            // Fetch các thiết bị cho ID phòng thuê (RentalPropertyId) được cung cấp
            var assets = await _appDbContext.Assets
                .Where(a => a.OwnAssets.Any(o => o.Room.RentalPropertyId == homeId))
                .Select(a => new
                {
                    a.AssetID,
                    a.AssetName,
                    Category = a.CategoryAsset.Name,
                    PurchaseDate = a.PurchaseDate,
                    Cost = a.Cost,
                    Condition = a.Condition,
                    RoomName = a.OwnAssets.Select(o => o.Room.RoomName).FirstOrDefault(),
                    NextMaintenanceDueDate = a.NextMaintenanceDueDate,
                    DaysUntilDue = a.NextMaintenanceDueDate.HasValue
                        ? (a.NextMaintenanceDueDate.Value - now).Days
                        : (int?)null,
                    IsNearMaintenance = a.NextMaintenanceDueDate.HasValue
                        && a.NextMaintenanceDueDate.Value <= maintenanceThreshold
                })
                .ToListAsync();

            foreach (var asset in assets)
            {
                if (asset.IsNearMaintenance && asset.Condition != "Maintenance")
                {
                    // Tìm thiết bị trong database và cập nhật
                    var assetToUpdate = await _appDbContext.Assets.FindAsync(asset.AssetID);
                    if (assetToUpdate != null)
                    {
                        assetToUpdate.Condition = "Maintenance"; // Cập nhật trạng thái thành "Maintenance"
                        _appDbContext.Assets.Update(assetToUpdate);
                    }
                }
            }

            await _appDbContext.SaveChangesAsync();

            var totalAssets = assets.Count;
            var assetsInMaintenance = assets.Count(a => a.Condition == "Maintenance").ToString();
            var assetsBroken = assets.Count(a => a.Condition.Trim() == "Broken").ToString();
            var assetsInUse = assets.Count(a => a.Condition.Trim() == "Using").ToString();

            return Json(new
            {
                totalAssets,
                assetsInMaintenance,
                assetsBroken,
                assetsInUse,
                assets
            });
        }

    }


}
