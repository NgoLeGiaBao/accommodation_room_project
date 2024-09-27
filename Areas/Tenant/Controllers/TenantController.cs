using Microsoft.AspNetCore.Mvc;

namespace App.Areas.Tenant
{
    [Area("Tenant")]
    public class TenantController : Controller
    {
        [Route("tenant-information")]
        public IActionResult Index()
        {
            return View();
        }

        [Route("edit-tenant/{id:int?}")]
        [HttpGet]
        public IActionResult EditTenant(int? id)
        {
            // Kiểm tra id nếu cần
            return View();
        }
        [Route("/tenant-delete/{id?}")]
        [HttpGet]
        public IActionResult Delete(int id)
        {
            return Content(id.ToString());
        }

        [Route("/create-tenant")]
        public IActionResult CreateTenant()
        {
            return View();
        }
    }
}
