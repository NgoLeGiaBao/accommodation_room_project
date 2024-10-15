using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Models
{
    [Table("RentalContract")]
    public class RentalContract
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public int ContractID { get; set; }

        [ForeignKey("Room")]
        [Display(Name = "Room ID", Prompt = "Enter the Room ID...")]
        public int RoomID { get; set; }

        [ForeignKey("AppUser")]
        [Display(Name = "User ID", Prompt = "Enter the User ID...")]
        public string? UserID { get; set; }

        [Display(Name = "Start Date", Prompt = "Enter the contract start date...")]
        public DateTime StartedDate { get; set; }

        [Display(Name = "End Date", Prompt = "Enter the contract end date...")]
        public DateTime EndupDate { get; set; }

        [Display(Name = "Electricity Price per kW", Prompt = "Enter the price per kW...")]
        public decimal PElectricityPerKw { get; set; }

        [Display(Name = "Water Price per cubic meter", Prompt = "Enter the price per cubic meter...")]
        public decimal PWaterPerK { get; set; }

        [Display(Name = "Service Price per cubic meter", Prompt = "Enter the service price per cubic meter...")]
        public decimal PServicePerK { get; set; }

        [Display(Name = "Room Rental Price per month", Prompt = "Enter the rental price per month...")]
        public decimal PRentalRoomPerM { get; set; }

        [Display(Name = "Rules", Prompt = "Enter the rules for the rental contract...")]
        public string? Rules { get; set; }

        [Display(Name = "Personal Signature for Contract", Prompt = "Enter the personal signature...")]
        public string? PersonalSignContract { get; set; }

        public Room? Room { get; set; }
        public AppUser? AppUser { get; set; }
    }
}
