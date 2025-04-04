using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using App.Models;
using App.Areas.Account.Models;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using Humanizer;
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

    [Route("/login")]
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    [Route("/login")]
    [HttpPost]
    public async Task<IActionResult> Index(string returnUrl = null)
    {
        returnUrl = returnUrl ?? Url.Content("~/");
        if (ModelState.IsValid)
        {
            var result = await _signInManager.PasswordSignInAsync(Input.Username, Input.Password, Input.RememberMe, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                if (!string.IsNullOrEmpty(returnUrl) && returnUrl != "/" && Url.IsLocalUrl(returnUrl))
                {
                    return LocalRedirect(returnUrl);
                }
                return RedirectToAction("Index", "Dashboard", new { area = "Dashboard" });
            }
        }
        ViewBag.LoginFailed = true;
        return View(this);
    }

    public IActionResult Logout()
    {
        _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Login");
    }
}