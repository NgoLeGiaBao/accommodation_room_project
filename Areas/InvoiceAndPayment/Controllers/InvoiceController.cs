using Microsoft.AspNetCore.Mvc;

namespace App.Areas.Payment
{
    [Area("InvoiceAndPayment")]
    public class InvoiceController : Controller
    {
        [Route("/invoice")]
        public IActionResult Index()
        {
            return View();
        }
    }
}