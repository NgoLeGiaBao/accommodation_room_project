using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace App.Models
{
    public class AppUser : IdentityUser
    {
        [Column(TypeName = "nvarchar")]
        [StringLength(12, MinimumLength = 12)]
        [RegularExpression(@"^\d{12}$", ErrorMessage = "IdentityCard must contain exactly 12 digits.")]
        public string? IdentityCard { get; set; }

        [Column(TypeName = "nvarchar")]
        [StringLength(100)]
        public string? FullName { get; set; }

        public bool Sex { get; set; }

        [DataType(DataType.Date)]
        public DateTime? Birthday { get; set; }

        [Column(TypeName = "nvarchar")]
        [StringLength(512)]
        public string? Address { get; set; }
    }
}
