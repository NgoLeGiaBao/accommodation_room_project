using System.ComponentModel.DataAnnotations;

namespace App.Areas.Account.Models
{
    public class InputModel
    {
        public string Username { get; set; }
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }
}