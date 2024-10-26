using Microsoft.AspNetCore.Mvc;

namespace App.Areas.Introduction
{
    [Area("Introduction")]
    public class HomePage : Controller
    {
        [Route("/home")]
        public IActionResult Home()
        {
            return View();
        }
    }
}