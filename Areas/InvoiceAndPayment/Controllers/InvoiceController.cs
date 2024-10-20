using System.Composition.Hosting;
using App.Models;
using Humanizer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace App.Areas.Payment
{
    [Area("InvoiceAndPayment")]
    public class InvoiceController : Controller
    {
        private readonly AppDbContext _appDbContext;
        private readonly UserManager<AppUser> _userManager;

        public InvoiceController(AppDbContext appDbContext, UserManager<AppUser> userManager)
        {
            _appDbContext = appDbContext;
            _userManager = userManager;
        }

        [Route("/invoice")]
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

        // Get list invoice
        [Route("/get-list-invoice/{homeId}")]
        public IActionResult GetListInvoice(string homeId)
        {
            if (homeId == null)
            {
                return NotFound();
            }

            // Check if the house exists
            var homeExist = _appDbContext.RentalProperties.FirstOrDefault(r => r.Id == homeId);
            if (homeExist == null)
            {
                return NotFound();
            }

            // Get the list of current contracts for the house
            var currentContracts = _appDbContext.RentalContracts
                .Include(c => c.Room) // Include the Room entity
                .Where(c => c.Room.RentalPropertyId == homeId && c.EndupDate > DateTime.UtcNow) // Valid contracts
                .ToList();

            // Set the current date
            var currentDate = DateTime.UtcNow;

            foreach (var contract in currentContracts)
            {
                // Create the first invoice based on the contract's start date
                if (currentDate.Date >= contract.StartedDate.Date)
                {
                    // Check if an invoice for the contract's start date already exists
                    var existingFirstInvoice = _appDbContext.Invoices
                        .FirstOrDefault(i => i.RoomId == contract.RoomID &&
                                             i.InvoiceDate.Year == contract.StartedDate.Year &&
                                             i.InvoiceDate.Month == contract.StartedDate.Month);
                    if (existingFirstInvoice == null)
                    {
                        var firstInvoice = new Invoice
                        {
                            RoomId = contract.RoomID,
                            InvoiceDate = DateTime.SpecifyKind(contract.StartedDate, DateTimeKind.Utc),
                            PaymentDate = null,
                            AdditionalServiceFee = 0,
                            WaterUsageForBefore = contract.Room.WaterPriceInit,
                            ElectricityForBefore = contract.Room.ElectricityPriceInit,
                            WaterUsage = 0,
                            ElectricityUsage = 0,
                            StatusInvocie = "Pending",
                            QRCodeImage = Guid.NewGuid().ToString(),
                            ServiceDetails = "Service Details",
                            RentalContractId = contract.ContractID
                        };

                        _appDbContext.Invoices.Add(firstInvoice);
                        _appDbContext.SaveChanges();
                    }
                }

                // Set the initial invoice date to the first invoice's date
                var invoiceDate = contract.StartedDate;

                // Loop through each month until the contract's end date
                while (invoiceDate < contract.EndupDate)
                {
                    // Calculate the end date of the current invoice period
                    var endDate = invoiceDate.AddMonths(1); // The end of the current period is the start of the next period

                    // Calculate the due date for creating the next invoice (5 days before the end of the current invoice period)
                    var dueDate = endDate.AddDays(-5);

                    // Check if today's date is equal to or past the due date
                    if (currentDate.Date >= dueDate.Date)
                    {
                        // Check if an invoice for this month already exists
                        var existingInvoice = _appDbContext.Invoices
                            .FirstOrDefault(i => i.RoomId == contract.RoomID &&
                                                 i.InvoiceDate.Year == endDate.Year &&
                                                 i.InvoiceDate.Month == endDate.Month);

                        // Create a new invoice if it does not exist
                        if (existingInvoice == null)
                        {
                            var newInvoice = new Invoice
                            {
                                RoomId = contract.RoomID,
                                InvoiceDate = DateTime.SpecifyKind(endDate, DateTimeKind.Utc),
                                PaymentDate = null,
                                WaterUsageForBefore = contract.Room.WaterPriceInit,
                                ElectricityForBefore = contract.Room.ElectricityPriceInit,
                                AdditionalServiceFee = 0,
                                WaterUsage = 0,
                                StatusInvocie = "Pending",
                                QRCodeImage = Guid.NewGuid().ToString(),
                                ServiceDetails = "Service Details",
                                RentalContractId = contract.ContractID
                            };

                            _appDbContext.Invoices.Add(newInvoice);
                            _appDbContext.SaveChanges();
                        }
                    }

                    // Move to the next month
                    invoiceDate = endDate;
                }
            }

            // Get the list of invoices for the house
            // Get the list of invoices for the house
            var invoices = _appDbContext.Invoices
                .Where(i => i.Room.RentalPropertyId == homeId)
                .Join(_appDbContext.Rooms,
                      invoice => invoice.RoomId,
                      room => room.Id,
                      (invoice, room) => new
                      {
                          invoice.Id,
                          invoice.RoomId,
                          invoice.InvoiceDate,
                          invoice.PaymentDate,
                          invoice.AdditionalServiceFee,
                          invoice.WaterUsage,
                          invoice.TotalMoney,
                          invoice.StatusInvocie,
                          RoomName = room.RoomName
                      })
                .ToList();

            // Count invoices by status
            var totalInvoices = invoices.Count;
            var pendingCount = invoices.Count(i => i.StatusInvocie == "Pending");
            var unpaidCount = invoices.Count(i => i.StatusInvocie == "Unpaid");
            var paidCount = invoices.Count(i => i.StatusInvocie == "Paid");
            var overdueCount = invoices.Count(i => i.StatusInvocie == "Overdue");

            // Create a result object to return
            var result = new
            {
                TotalInvoices = totalInvoices,
                Pending = pendingCount,
                Unpaid = unpaidCount,
                Paid = paidCount,
                Overdue = overdueCount,
                Invoices = invoices
            };

            return Json(result);
        }

        // Edit invoice
        [Route("/edit-invoice/{id}")]
        [HttpGet]
        public async Task<IActionResult> EditInvoice(string id)
        {
            // Retrieve the invoice along with its associated room
            var invoice = await _appDbContext.Invoices
                .Include(i => i.Room)
                .ThenInclude(r => r.RentalContracts)
                .FirstOrDefaultAsync(i => i.Id == id);

            // Check if the invoice exists
            if (invoice == null)
            {
                return NotFound();
            }


            // Use DateTime.UtcNow for current date and time in UTC
            var currentDateTime = DateTime.UtcNow;

            // Get the active contract for the room
            var inforContract = _appDbContext.RentalContracts.FirstOrDefault(c => c.ContractID == invoice.RentalContractId);
            if (inforContract == null)
                return NotFound();
            // Optionally, you can pass the active contract to the view
            ViewBag.InforContract = inforContract;

            // Return the invoice view with the invoice data
            return View(invoice);
        }

        [Route("/edit-invoice/{id}")]
        [HttpPost]
        public async Task<IActionResult> EditInvoice([FromForm] string otherServices, int electricityUsage, int waterUsage, int totalAmount, string id)
        {
            // Get the current invocie
            var invoice = await _appDbContext.Invoices
                .Include(i => i.Room)
                .ThenInclude(r => r.RentalContracts)
                .FirstOrDefaultAsync(i => i.Id == id);

            // Check invoice
            if (invoice == null)
                return NotFound();

            // Update room with ElectricityUsage and WaterUsage
            var room = invoice.Room;
            room.ElectricityPriceInit = electricityUsage > invoice.ElectricityForBefore ? electricityUsage : room.ElectricityPriceInit;
            room.WaterPriceInit = waterUsage > invoice.WaterUsageForBefore ? waterUsage : room.WaterPriceInit;

            // Update invoice
            invoice.ServiceDetails = otherServices;
            invoice.ElectricityUsage = electricityUsage;
            invoice.WaterUsage = waterUsage;
            invoice.TotalMoney = totalAmount;
            invoice.StatusInvocie = "Unpaid";
            invoice.PaymentDate = null;
            invoice.QRCodeImage = Guid.NewGuid().ToString();

            _appDbContext.Invoices.Update(invoice);
            _appDbContext.Rooms.Update(room);
            await _appDbContext.SaveChangesAsync();

            TempData["SuccessMessage"] = "Invocie updated successfully.";
            return RedirectToAction("EditInvoice");
        }
    }

}