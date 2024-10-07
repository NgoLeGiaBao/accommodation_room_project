using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Models
{
    [Table("RentalProperty")]
    public class RentalProperty
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Property name is required.")]
        [StringLength(100, ErrorMessage = "Property name cannot exceed 100 characters.")]
        [Display(Name = "Property Name", Prompt = "Enter property name...")]
        public string PropertyName { get; set; }

        [Required(ErrorMessage = "Owner name is required.")]
        [StringLength(100, ErrorMessage = "Owner name cannot exceed 100 characters.")]
        [Display(Name = "Owner Name", Prompt = "Enter owner name...")]
        public string OwnerName { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        [StringLength(512, ErrorMessage = "Address cannot exceed 512 characters.")]
        [Display(Name = "Address", Prompt = "Enter address...")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Number of rooms is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Number of rooms must be greater than 0.")]
        [Display(Name = "Number of Rooms", Prompt = "Enter number of rooms...")]
        public int NumberOfRooms { get; set; }

        [Required(ErrorMessage = "Total area is required.")]
        [Range(1, double.MaxValue, ErrorMessage = "Total area must be greater than 0.")]
        [Display(Name = "Total Area", Prompt = "Enter total area...")]
        public double TotalArea { get; set; }

        [Required(ErrorMessage = "Property type is required.")]
        [StringLength(50, ErrorMessage = "Property type cannot exceed 50 characters.")]
        [Display(Name = "Property Type", Prompt = "Enter property type...")]
        public string PropertyType { get; set; }

        [Display(Name = "Facilities", Prompt = "Enter facilities...")]
        public string Facilities { get; set; }

        [Required(ErrorMessage = "Electricity price is required.")]
        [DataType(DataType.Currency)]
        [Display(Name = "Electricity Price", Prompt = "Enter electricity price...")]
        public decimal ElectricityPrice { get; set; }

        [Required(ErrorMessage = "Water price is required.")]
        [DataType(DataType.Currency)]
        [Display(Name = "Water Price", Prompt = "Enter water price...")]
        public decimal WaterPrice { get; set; }

        [Required(ErrorMessage = "Owner's phone number is required.")]
        [Phone(ErrorMessage = "Invalid phone number.")]
        [Display(Name = "Owner's Phone Number", Prompt = "Enter owner's phone number...")]
        public string OwnerPhoneNumber { get; set; }

        [Required(ErrorMessage = "Start date is required.")]
        [DataType(DataType.Date)]
        [Display(Name = "Start Date", Prompt = "Enter start date...")]
        public DateTime StartDate { get; set; }

        [Display(Name = "Property Image", Prompt = "Select the path to the property image...")]
        public string? PropertyImage { get; set; }

        [Display(Name = "Active Status")]
        public bool IsActive { get; set; }

        [Display(Name = "Application User")]
        // Navigation property for the many-to-many relationship
        public virtual ICollection<UserRentalProperty> UserRentalProperties { get; set; } = new List<UserRentalProperty>();

        [Display(Name = "Room List")]
        public ICollection<Room> Rooms { get; set; } = new List<Room>();
    }
}
