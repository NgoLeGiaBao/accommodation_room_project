using App.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;

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
                            RentalPropertyId = rentalPropertyId
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
                        return RedirectToAction("CreateTenant");
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
                        return RedirectToAction("CreateTenant"); // Redirect after successful creation
                    }
                }
            }
            TempData["FailureMessage"] = "User created failurely, please try again.";
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

            // Fetch users associated with the specified rental property, excluding the current user
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

            // Calculate totals
            var totalCurrentTenants = users.Count(u => u.CurrentContractsCount > 0);
            var totalPastTenants = users.Count(u => u.PastContractsCount > 0 && u.CurrentContractsCount == 0);
            var totalNeverRented = users.Count(u => u.CurrentContractsCount == 0 && u.PastContractsCount == 0);

            return Ok(new
            {
                totalUsers = users.Count, // Total users excluding the current user
                totalCurrentTenants,
                totalPastTenants,
                totalNeverRented,
                UsersWithCategories = users.Select(u => new
                {
                    User = u.User,
                    IsCurrentTenant = u.CurrentContractsCount > 0,
                    IsPastTenant = u.PastContractsCount > 0 && u.CurrentContractsCount == 0,
                    IsNeverRented = u.CurrentContractsCount == 0 && u.PastContractsCount == 0
                }).ToList()
            });
        }


        [Route("/edit-tenant/{id}")]
        [HttpGet]
        public IActionResult EditTenant(string id)
        {
            // Assuming you have a DbContext named ApplicationDbContext
            var tenant = _appDbContext.AppUsers.FirstOrDefault(u => u.Id == id);

            if (tenant == null)
                return NotFound();

            // Return the view with the tenant's data
            return View(tenant);
        }

        [Route("/edit-tenant/{id}")]
        [HttpPost]
        public async Task<IActionResult> EditTenant(AppUser appUser, string id)
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

                    // Save changes to the database
                    await _userManager.UpdateAsync(existingUser);

                    TempData["SuccessMessage"] = "Update user successfully.";
                    return RedirectToAction("EditTenant", new { id = id });

                }
            }
            TempData["FailureMessage"] = "Update user failurely, please try again.";
            return Content("Update user failurely, please try again.");
            return View(appUser);

        }

        [Route("/tenant-delete/{id?}")]
        [HttpGet]
        public IActionResult Delete(string id)
        {
            return Content(id.ToString());
        }

        // Export list user in home
        [Route("/export-list-user/{homeId}")]
        public async Task<IActionResult> ExportListUser(string homeId)
        {
            var home = await _appDbContext.RentalProperties.FindAsync(homeId);
            if (home == null)
                return NotFound();

            // Check if the current user is authenticated
            var currentUser = await _signInManager.UserManager.GetUserAsync(User);
            if (currentUser == null)
                return Unauthorized(new { message = "User is not authenticated." });

            // Fetch users associated with the specified RentalPropertyId (homeId), excluding the current user
            var users = await _appDbContext.UserRentalProperties
                .Where(rpu => rpu.RentalPropertyId == homeId && rpu.AppUserId != currentUser.Id)
                .Select(rpu => rpu.AppUser)
                .ToListAsync();

            // Create Excel file with the list of users
            var package = new ExcelPackage(); // Do not use 'using' here

            var worksheet = package.Workbook.Worksheets.Add("Tenants");

            if (users == null || users.Count == 0)
            {
                worksheet.Cells["A1"].Value = "No tenants are found.";
                worksheet.Cells["A1:F1"].Merge = true; // Merge cells for the title
                worksheet.Cells["A1"].Style.Font.Bold = true;
                worksheet.Cells["A1"].Style.Font.Size = 16;
                worksheet.Cells["A1"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            }
            else
            {
                // Add title
                worksheet.Cells["A1"].Value = "Tenant List";
                worksheet.Cells["A1:F1"].Merge = true; // Merge cells for the title
                worksheet.Cells["A1"].Style.Font.Bold = true;
                worksheet.Cells["A1"].Style.Font.Size = 16;
                worksheet.Cells["A1"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                // Add header row
                worksheet.Cells["A2"].Value = "No";
                worksheet.Cells["B2"].Value = "Full Name";
                worksheet.Cells["C2"].Value = "Gender";
                worksheet.Cells["D2"].Value = "Date of Birth";
                worksheet.Cells["E2"].Value = "ID Card";
                worksheet.Cells["F2"].Value = "Address";

                worksheet.Cells["A2:F2"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                worksheet.Cells["A2:F2"].Style.Font.Bold = true; // Make header bold
                worksheet.Cells["A2:F2"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid; // Fill color for header
                worksheet.Cells["A2:F2"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray); // Set background color

                // Set column widths for better visibility
                worksheet.Column(1).Width = 5;
                worksheet.Column(2).Width = 30;
                worksheet.Column(3).Width = 10;
                worksheet.Column(4).Width = 15;
                worksheet.Column(5).Width = 20;
                worksheet.Column(6).Width = 70;

                int row = 3; // Start adding user data from the third row
                foreach (var user in users)
                {
                    worksheet.Cells[row, 1].Value = (row - 2).ToString();
                    worksheet.Cells[row, 2].Value = user.FullName;
                    worksheet.Cells[row, 3].Value = user.Sex ? "Male" : "Female";
                    worksheet.Cells[row, 4].Value = user.Birthday.HasValue
                        ? user.Birthday.Value.ToString("dd/MM/yyyy")
                        : string.Empty;
                    worksheet.Cells[row, 5].Value = user.IdentityCard;
                    worksheet.Cells[row, 6].Value = user.Address;

                    for (int col = 1; col <= 6; col++)
                    {
                        worksheet.Cells[row, col].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    }

                    row++;
                }
            }
            // Prepare Excel file for download
            var fileName = $"TenantList_{home.PropertyName}.xlsx";
            var memoryStream = new MemoryStream();
            package.SaveAs(memoryStream);
            memoryStream.Position = 0;

            // Return the file while keeping the MemoryStream alive
            return File(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

    }
}
