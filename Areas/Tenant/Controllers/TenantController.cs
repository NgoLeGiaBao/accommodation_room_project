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

            // If we get to this point, something went wrong, return to the view with the current appUser
            var errors = ModelState.Values.SelectMany(v => v.Errors)
                      .Select(e => e.ErrorMessage)
                      .ToList();
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


        [Route("/get-list-user/{homeId:int}")]
        [HttpPost]
        public async Task<IActionResult> GetListUser(int homeId)
        {
            // Get the currently logged-in user
            var currentUser = await _signInManager.UserManager.GetUserAsync(User);

            if (currentUser == null)
            /// <response code="404">No users associated with the property were found.</response>
            {
                return Unauthorized(new { message = "User is not authenticated." });
            }

            // Retrieve users associated with the specified rental property,
            // excluding the current user
            var users = await _appDbContext.UserRentalProperties
                .Where(rpu => rpu.RentalPropertyId == homeId && rpu.AppUserId != currentUser.Id) // Exclude current user
                .Include(rpu => rpu.AppUser) // Include user details
                .Select(rpu => rpu.AppUser)
                .ToListAsync();

            if (users == null || users.Count == 0)
            {
                return NotFound(new { message = "No users found for this property." });
            }

            return Ok(new { users });
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
