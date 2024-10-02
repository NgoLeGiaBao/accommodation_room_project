using Microsoft.AspNetCore.Mvc;

namespace App.Areas.Payment
{
    [Area("InvoiceAndPayment")]
    public class PaymentController : Controller
    {
        [Route("/payment")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
