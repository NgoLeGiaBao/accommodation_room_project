using App.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace App.COntrollers
{
    public class NotificationController : Controller
    {
        private AppDbContext _appDbContext;
        private UserManager<AppUser> _userManager;

        public NotificationController(AppDbContext appDbContext, UserManager<AppUser> userManager)
        {
            _appDbContext = appDbContext;
            _userManager = userManager;
        }

        [Route("/get-list-notification")]
        [HttpPost]
        public async Task<IActionResult> GetListNotification()
        {
            var user = await _userManager.GetUserAsync(User);
            var notifications = _appDbContext.OwnNotifications.Where(n => n.UserId == user.Id).ToList();
            if (notifications == null) return Json("Null here");
            return Json(new { notifications });
        }

    }
}