using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Models
{
    [Table("Room")]
    public class Room
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string RoomName { get; set; }
        [Required]
        public double Area { get; set; }

        public int MaximumNumberOfPeople { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }

        // Foreign Key
        public int RentalPropertyId { get; set; }

        public bool IsActive { get; set; }
        public string Status { get; set; }

        [ForeignKey("RentalPropertyId")]
        public RentalProperty RentalProperty { get; set; } // Navigation property for associated RentalProperty

        // Navigation property for Rental Contracts
        public ICollection<RentalContract> RentalContracts { get; set; } // List of rental contracts associated with this room

        public virtual ICollection<OwnAsset> OwnAssets { get; set; } // Danh sách các tài sản thuộc phòng


    }
}