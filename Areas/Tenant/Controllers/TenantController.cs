using App.Models;
using Microsoft.AspNetCore.Mvc;

namespace App.Areas.Tenant
{

    [Area("Tenant")]
    public class TenantController : Controller
    {
        private readonly AppDbContext _appDbContext;

        public TenantController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        [Route("information-tenant")]
        public IActionResult Index()
        {
            return View();
        }

        [Route("/create-tenant/{rentalPropertyId:int?}")]
        [HttpGet]
        public IActionResult CreateTenant()
        {
            AppUser appUser = new AppUser();
            return View(appUser);
        }

        [Route("/create-tenant/{rentalPropertyId:int?}")]
        [HttpPost]
        public async Task<IActionResult> CreateTenant(AppUser appUser, int rentalPropertyId)
        {
            // var rentalProperty = await _appDbContext.RentalProperties.FindAsync(rentalPropertyId);
            // if (rentalProperty != null && ModelState.IsValid)
            // {
            //     appUser.RentalProperties. = rentalProperty.Id;
            //     _appDbContext.AppUsers.Add(appUser);
            //     await _appDbContext.SaveChangesAsync();
            //     TempData["SuccessMessage"] = "Tenant created successfully";
            //     return RedirectToAction("CreateTenant");
            // }
            // TempData["FailureMessage"] = "Tenant created failurely, please try again";
            // return View(appUser);
            return NotFound();
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


    }
}
