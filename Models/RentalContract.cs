using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Models
{
    [Table("RentalContract")]
    public class RentalContract
    {
        [Key]
        public int ContractID { get; set; }

        [ForeignKey("Room")]
        public int RoomID { get; set; }

        [ForeignKey("AppUser")]
        public string UserID { get; set; }

        public DateTime StartedDate { get; set; }
        public DateTime EndupDate { get; set; }
        public decimal PElectricityPerKw { get; set; }
        public decimal PWaterPerK { get; set; }
        public decimal PServicePerK { get; set; }
        public decimal PRentalRoomPerM { get; set; }
        public string Rules { get; set; }
        public Room Room { get; set; }
        public AppUser AppUser { get; set; }
    }
}
