using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using App.Models;
using App.Areas.Account.Models;
[Area("Account")]
public class LoginController : Controller
{
    private readonly SignInManager<AppUser> _signInManager;
    private readonly UserManager<AppUser> _userManager;

    public LoginController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }

    [BindProperty]
    public InputModel Input { get; set; }

    [Route("/")]
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    [Route("/")]
    [HttpPost]
    public async Task<IActionResult> Index(string returnUrl = null)
    {
        returnUrl = returnUrl ?? Url.Content("~/");

        if (ModelState.IsValid)
        {
            var result = await _signInManager.PasswordSignInAsync(Input.Username, Input.Password, Input.RememberMe, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return LocalRedirect(returnUrl);
                }
                return RedirectToAction("Index", "Dashboard", new { area = "Dashboard" });
            }
        }
        ViewBag.LoginFailed = true;
        return View(this);
    }
}