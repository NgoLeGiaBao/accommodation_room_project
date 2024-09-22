using Microsoft.AspNetCore.Mvc;

namespace App.Areas.Contract
{
    [Area("Contract")]
    public class ContractController : Controller
    {
        [Route("/contract")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
