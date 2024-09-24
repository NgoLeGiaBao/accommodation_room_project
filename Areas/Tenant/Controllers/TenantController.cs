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
        
        [Route("/tenant-edit/{id?}")]
        [HttpGet]
        public IActionResult Edit(int id)
        {
            return Content(id.ToString());
        }

        [Route("/tenant-delete/{id?}")]
        [HttpGet]
        public IActionResult Delete(int id)
        {
            return Content(id.ToString());
        }

        [Route("/tenant-create")]
        public IActionResult Create()
        {
            return View();
        }
    }
}
