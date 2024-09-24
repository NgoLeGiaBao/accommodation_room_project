using Microsoft.AspNetCore.Mvc;
[Area("Account")]
public class ProfileController : Controller
{
    [Route("/profile")]
    public IActionResult Index()
    {
        return View();
    }
}