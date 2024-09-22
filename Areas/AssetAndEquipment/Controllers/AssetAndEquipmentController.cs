using Microsoft.AspNetCore.Mvc;

namespace Areas.AssetAndEquipment
{
    [Area("AssetAndEquipment")]
    public class AssetAndEquipmentController : Controller
    {
        [Route("/asset-equipment")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
