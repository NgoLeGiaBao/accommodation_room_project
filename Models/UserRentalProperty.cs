using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace App.Models
{
    public class UserRentalProperty
    {
        [Key]
        [Column(Order = 1)]
        public string AppUserId { get; set; }  // Foreign key to AppUser

        [Key]
        [Column(Order = 2)]
        public int RentalPropertyId { get; set; }  // Foreign key to RentalProperty

        [ForeignKey("AppUserId")]
        public AppUser AppUser { get; set; }  // Navigation property

        [ForeignKey("RentalPropertyId")]
        public RentalProperty RentalProperty { get; set; }  // Navigation property
    }
}
