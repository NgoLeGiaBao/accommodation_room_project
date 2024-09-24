using Microsoft.AspNetCore.Mvc;

[Area("Account")]
public class LoginController : Controller
{
    [Route("/login")]
    public IActionResult Index()
    {
        return View();
    }
}