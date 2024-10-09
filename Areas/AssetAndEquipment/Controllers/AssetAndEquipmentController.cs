using Microsoft.AspNetCore.Mvc;
using App.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Areas.AssetAndEquipment
{
    [Area("AssetAndEquipment")]
    public class AssetAndEquipmentController : Controller
    {
        private readonly AppDbContext _appDbContext;

        public AssetAndEquipmentController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        [Route("/asset-equipment")]
        public IActionResult Index()
        {

            return View();
        }

        [Route("/create-asset-equipment/{homeId:int?}")]
        [HttpGet]
        public IActionResult CreateAssetAndEquipment(int homeId)
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

            var statuses = new List<SelectListItem>
            {
                new SelectListItem { Value = "Good", Text = "Good" },
                new SelectListItem { Value = "Bad", Text = "Bad" }
            };

            ViewData["Rooms"] = rooms;
            ViewData["Categories"] = categories;
            ViewData["Statuses"] = statuses;

            return View();
        }

        [Route("/create-asset-equipment/{homeId:int?}")]
        [HttpPost]
        public async Task<IActionResult> CreateAssetAndEquipmentAsync(Asset assetAndEquipment, int homeId)
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

                await _appDbContext.Assets.AddAsync(assetAndEquipment);
                await _appDbContext.SaveChangesAsync();

                var ownAsset = new OwnAsset
                {
                    CreatedDate = DateTime.UtcNow,
                    AssetID = assetAndEquipment.AssetID,
                    RoomID = Convert.ToInt32(assetAndEquipment.Location)
                };


                await _appDbContext.OwnAssets.AddAsync(ownAsset);
                await _appDbContext.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            ViewData["Rooms"] = new SelectList(await _appDbContext.Rooms.Where(r => r.RentalPropertyId == homeId).ToListAsync(), "Id", "RoomName");
            return View(assetAndEquipment);
        }

        [Route("/get-list-assets/{homeId:int}")]
        // [HttpPost]
        public async Task<IActionResult> GetAssets(int homeId)
        {
            var now = DateTime.UtcNow; // Ensure you're using UTC
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
                    RoomName = a.OwnAssets.Select(o => o.Room.RoomName).FirstOrDefault(), // Get the room name
                    NextMaintenanceDueDate = a.NextMaintenanceDueDate,
                    DaysUntilDue = a.NextMaintenanceDueDate.HasValue
                        ? (a.NextMaintenanceDueDate.Value - now).Days
                        : (int?)null
                })
                .ToListAsync();

            return Json(new { assets });
        }

    }


}
