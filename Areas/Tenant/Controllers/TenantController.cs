using App.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace App.Areas.Tenant
{

    [Area("Tenant")]
    public class TenantController : Controller
    {
        private readonly AppDbContext _appDbContext;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        public TenantController(AppDbContext appDbContext, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _appDbContext = appDbContext;
            _userManager = userManager;
            _signInManager = signInManager;
        }
        [Route("information-tenant")]
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

        [Route("/create-tenant/{rentalPropertyId}")]
        [HttpGet]
        public IActionResult CreateTenant()
        {
            AppUser appUser = new AppUser();
            return View(appUser);
        }
        [Route("/create-tenant/{rentalPropertyId}")]
        [HttpPost]
        public async Task<IActionResult> CreateTenant(AppUser appUser, string rentalPropertyId)
        {
            if (ModelState.IsValid)
            {
                var existingUser = await _userManager.Users
                    .FirstOrDefaultAsync(u => u.IdentityCard == appUser.IdentityCard);

                if (existingUser != null)
                {
                    // Update existing user information
                    existingUser.FullName = appUser.FullName;
                    existingUser.Email = appUser.Email;
                    existingUser.PhoneNumber = appUser.PhoneNumber;
                    existingUser.Birthday = appUser.Birthday?.ToUniversalTime();
                    existingUser.Address = appUser.Address;
                    existingUser.Sex = appUser.Sex;

                    // Update the user in the database
                    var updateResult = await _userManager.UpdateAsync(existingUser);
                    if (updateResult.Succeeded)
                    {
                        // Add to UserRentalProperty if not already present
                        var userRentalProperty = new UserRentalProperty
                        {
                            AppUserId = existingUser.Id,
                            RentalPropertyId = rentalPropertyId // Ensure rentalPropertyId has a value
                        };

                        // Check if the relationship already exists
                        var rentalPropertyExists = await _appDbContext.UserRentalProperties
                                .AnyAsync(urp => urp.AppUserId == existingUser.Id && urp.RentalPropertyId == rentalPropertyId);

                        if (!rentalPropertyExists)
                        {
                            await _appDbContext.UserRentalProperties.AddAsync(userRentalProperty);
                            await _appDbContext.SaveChangesAsync();
                        }

                        TempData["SuccessMessage"] = "User updated and rental property relationship added successfully.";
                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    // Create a new user
                    appUser.UserName = appUser.IdentityCard;
                    appUser.Birthday = appUser.Birthday?.ToUniversalTime();
                    var result = await _userManager.CreateAsync(appUser);
                    if (result.Succeeded)
                    {
                        // After creating the user, retrieve the newly created user's ID
                        var newUser = await _userManager.Users
                                            .FirstOrDefaultAsync(u => u.IdentityCard == appUser.IdentityCard);
                        // Add to UserRentalProperty
                        var userRentalProperty = new UserRentalProperty
                        {
                            AppUserId = newUser.Id, // Use the new user's ID
                            RentalPropertyId = rentalPropertyId
                        };

                        await _appDbContext.UserRentalProperties.AddAsync(userRentalProperty);
                        await _appDbContext.SaveChangesAsync();

                        TempData["SuccessMessage"] = "User created and rental property relationship added successfully.";
                        return RedirectToAction("Index"); // Redirect after successful creation
                    }
                }
            }

            // If we get to this postring, something went wrong, return to the view with the current appUser
            // var errors = ModelState.Values.SelectMany(v => v.Errors)
            //           .Select(e => e.ErrorMessage)
            //           .ToList();
            // return Content(string.Join(",", errors));
            return View(appUser);
        }

        [Route("/check-identity-card")]
        [HttpPost]
        public async Task<IActionResult> CheckIdentityCard(string identityCard)
        {
            var user = await _userManager.Users
                   .FirstOrDefaultAsync(u => u.IdentityCard == identityCard);

            if (user != null)
            {
                return Json(new
                {
                    exists = true,
                    fullName = user.FullName,
                    email = user.Email,
                    phoneNumber = user.PhoneNumber,
                    birthday = user.Birthday.HasValue ? user.Birthday.Value.ToString("yyyy-MM-dd") : null,
                    address = user.Address,
                    sex = user.Sex
                });
            }

            return Json(new { exists = false });
        }


        [Route("/get-list-user/{homeId}")]
        [HttpPost]
        public async Task<IActionResult> GetListUser(string homeId)
        {
            // Get the currently logged-in user
            var currentUser = await _signInManager.UserManager.GetUserAsync(User);

            if (currentUser == null)
                return Unauthorized(new { message = "User is not authenticated." });

            // Get the current date
            var currentDate = DateTime.UtcNow;

            // Count total users associated with the specified rental property, excluding the current user
            var totalUsers = await _appDbContext.UserRentalProperties
                .Where(rpu => rpu.RentalPropertyId == homeId && rpu.AppUserId != currentUser.Id) // Exclude current user
                .CountAsync();

            // Now retrieve user details if needed
            var users = await _appDbContext.UserRentalProperties
                .Where(rpu => rpu.RentalPropertyId == homeId && rpu.AppUserId != currentUser.Id) // Exclude current user
                .Include(rpu => rpu.AppUser)
                .Select(rpu => new
                {
                    User = rpu.AppUser,
                    // Count current rental contracts
                    CurrentContractsCount = _appDbContext.RentalContracts.Count(rc =>
                        rc.AppUser.Id == rpu.AppUserId &&
                        rc.Room.RentalPropertyId == homeId &&
                        rc.StartedDate <= currentDate && rc.EndupDate >= currentDate),

                    // Count past rental contracts
                    PastContractsCount = _appDbContext.RentalContracts.Count(rc =>
                        rc.AppUser.Id == rpu.AppUserId &&
                        rc.Room.RentalPropertyId == homeId &&
                        rc.EndupDate < currentDate)
                })
                .ToListAsync();

            return Ok(new { totalUsers, users });
        }


        [Route("edit-tenant/{id}")]
        [HttpGet]
        public IActionResult EditTenant(string? id)
        {
            // Kiểm tra id nếu cần
            return View();
        }
        [Route("/tenant-delete/{id?}")]
        [HttpGet]
        public IActionResult Delete(string id)
        {
            return Content(id.ToString());
        }


    }
}
