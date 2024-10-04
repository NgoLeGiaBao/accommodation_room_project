namespace App.Areas.Account.Models
{
    public class InputModel
    {
        public string Username { get; set; }
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}