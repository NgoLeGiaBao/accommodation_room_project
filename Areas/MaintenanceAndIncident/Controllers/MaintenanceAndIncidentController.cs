using Microsoft.AspNetCore.Mvc;

namespace App.Areas.MaintenanceAndIncident
{
    [Area("MaintenanceAndIncident")]
    public class MaintenanceAndIncidentController : Controller
    {
        [Route("/maintenance-incident")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
