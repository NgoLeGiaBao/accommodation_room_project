using Microsoft.AspNetCore.Mvc;

namespace App.Areas.Tenant
{
    [Area("Tenant")]
    public class TenantController : Controller
    {
        [Route("/tenant-information")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
