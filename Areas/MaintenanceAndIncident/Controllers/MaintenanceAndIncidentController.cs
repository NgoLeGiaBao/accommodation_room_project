using Microsoft.AspNetCore.Mvc;
using App.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Identity.Client;
using Microsoft.EntityFrameworkCore;


namespace App.Areas.MaintenanceAndIncident
{
    [Authorize]
    [Area("MaintenanceAndIncident")]
    public class MaintenanceAndIncidentController : Controller
    {
        private readonly AppDbContext _appDbContext;
        private readonly UserManager<AppUser> _userManager;

        public MaintenanceAndIncidentController(AppDbContext appDbContext, UserManager<AppUser> userManager)
        {
            _appDbContext = appDbContext;
            _userManager = userManager;
        }
        [Route("/maintenance-incident")]
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

        [Route("/create-maintenance-incident/{homeId}")]
        [HttpGet]
        public async Task<IActionResult> CreateMaintenanceAndIncident(string homeId)
        {
            var assest = _appDbContext.Assets.Where(a => a.OwnAssets.Any(o => o.Room.RentalPropertyId == homeId)).ToList();
            ViewData["Assets"] = assest.Select(a => new SelectListItem
            {
                Value = a.AssetID.ToString(),
                Text = a.AssetName
            }).ToList();
            return View();
        }

        [Route("/create-maintenance-incident/{homeId}")]
        [HttpPost]
        public async Task<IActionResult> CreateMaintenanceAndIncident(App.Models.MaintenanceAndIncident maintenanceAndIncident, string homeId)
        {
            // Get user information
            var user = await _userManager.GetUserAsync(User);

            // Get assest
            var assest = _appDbContext.Assets.Where(a => a.OwnAssets.Any(o => o.Room.RentalPropertyId == homeId)).ToList();
            ViewData["Assets"] = assest.Select(a => new SelectListItem
            {
                Value = a.AssetID.ToString(),
                Text = a.AssetName
            }).ToList();

            if (user != null || ModelState.IsValid)
            {

                maintenanceAndIncident.RentalPropertyId = homeId;
                maintenanceAndIncident.ReportedBy = user.Id;
                maintenanceAndIncident.ReportedDate = DateTime.Now.ToUniversalTime();
                maintenanceAndIncident.Status = "Requested";
                _appDbContext.MaintenanceAndIncidents.Add(maintenanceAndIncident);
                _appDbContext.SaveChanges();
                TempData["SuccessMessage"] = "Response report created successfully.";
                return View();

            }
            // var modelErrors = ModelState.Values.SelectMany(v => v.Errors)
            //                                        .Select(e => e.ErrorMessage)
            //                                        .ToList();
            // return Content("ModelState Errors: " + string.Join(", ", modelErrors));
            TempData["FailureMessage"] = "Rental property creation failed, please try again.";

            return View(maintenanceAndIncident);
        }

        [Route("/get-list-maintenance-incident/{homeId}")]
        public async Task<IActionResult> GetListMaintanceAndIncident(string homeId)
        {
            var rentalProperty = await _appDbContext.RentalProperties.FirstOrDefaultAsync(rp => rp.Id == homeId);
            if (rentalProperty == null)
                return NotFound();

            // Retrieve the list of maintenance and incidents, including room names via the Asset and OwnAsset relationship
            var maintenanceAndIncidents = await _appDbContext.MaintenanceAndIncidents
                .Where(m => m.RentalPropertyId == homeId)
                .Select(m => new
                {
                    m.IncidentId,
                    m.Description,
                    m.Status,
                    m.ReportedDate,
                    m.RentalPropertyId,
                    m.Asset.AssetName,
                    RoomName = _appDbContext.Assets
                                 .Where(a => a.AssetID == m.AssetID) // Assuming AssetID is part of MaintenanceAndIncidents
                                 .Select(a => a.OwnAssets
                                     .Select(oa => oa.Room.RoomName) // Navigate from OwnAssets to Room to get the Name
                                     .FirstOrDefault()) // Get the first related room's name
                                 .FirstOrDefault() // Get the first asset that matches
                })
                .ToListAsync();

            var total = maintenanceAndIncidents.Count();
            var requested = maintenanceAndIncidents.Where(m => m.Status == "Requested").Count();
            var processed = maintenanceAndIncidents.Where(m => m.Status == "Processed").Count();

            return Ok(new
            {
                MaintenanceAndIncidents = maintenanceAndIncidents,
                Total = total,
                Requested = requested,
                Processed = processed
            });
        }

    }
}
