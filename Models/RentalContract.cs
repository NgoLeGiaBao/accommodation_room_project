using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Models
{
    [Table("RentalContract")]
    public class RentalContract
    {
        [Key]
        public int ContractID { get; set; } // Auto-generated contract ID

        [ForeignKey("Room")]
        public int RoomID { get; set; } // Foreign key for Room

        [ForeignKey("AppUser")]
        public string UserID { get; set; } // Foreign key for User

        public DateTime StartedDate { get; set; } // Start date of the contract
        public DateTime EndupDate { get; set; } // End date of the contract

        public decimal PElectricityPerKw { get; set; } // Price of electricity per kWh
        public decimal PWaterPerK { get; set; } // Price of water per unit
        public decimal PServicePerK { get; set; } // Price of additional services
        public decimal PRentalRoomPerM { get; set; } // Monthly rental price

        public string Rules { get; set; } // Rental rules and terms

        // Navigation properties
        public Room Room { get; set; } // Navigation property for the associated Room
        public AppUser AppUser { get; set; } // Navigation property for the associated AppUser
    }
}
