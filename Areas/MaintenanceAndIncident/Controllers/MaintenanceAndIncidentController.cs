using Microsoft.AspNetCore.Mvc;
using App.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;


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
    }
}
