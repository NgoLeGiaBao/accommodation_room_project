using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using App.Models.NewsModel;

namespace App.Models
{
    public class AppUser : IdentityUser
    {
        [StringLength(12, MinimumLength = 12, ErrorMessage = "Identity card must be exactly 12 digits.")]
        [RegularExpression(@"^\d{12}$", ErrorMessage = "Identity card must contain exactly 12 digits.")]
        [Display(Name = "Identity Card", Prompt = "Enter identity card...")]
        public string? IdentityCard { get; set; }

        [Column(TypeName = "text")]
        [StringLength(100, ErrorMessage = "Full name cannot be longer than 100 characters.")]
        [Display(Name = "Full Name", Prompt = "Enter full name...")]
        public string? FullName { get; set; }

        [Display(Name = "Gender", Prompt = "Enter gender...")]
        public bool Sex { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Birthday")]
        public DateTime? Birthday { get; set; }

        [Column(TypeName = "text")]
        [StringLength(512, ErrorMessage = "Address cannot be longer than 512 characters.")]
        [Display(Name = "Address", Prompt = "Enter address...")]
        public string? Address { get; set; }
        // Navigation property for the many-to-many relationship
        public virtual ICollection<UserRentalProperty> UserRentalProperties { get; set; } = new List<UserRentalProperty>();
        public virtual ICollection<RentalContract> RentalContracts { get; set; } = new List<RentalContract>();
        public virtual ICollection<Notification> CreatedNotifications { get; set; } = new List<Notification>();
        public virtual ICollection<OwnNotification> OwnNotifications { get; set; } = new List<OwnNotification>();
        public virtual ICollection<ContentNews> ContentNews { get; set; } = new List<ContentNews>();

    }
}
