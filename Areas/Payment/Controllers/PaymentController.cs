using Microsoft.AspNetCore.Mvc;

namespace App.Areas.Payment
{
    [Area("Payment")]
    public class PaymentController : Controller
    {
        [Route("/payment")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
