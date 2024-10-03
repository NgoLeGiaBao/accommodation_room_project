using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace App.Models
{
    [Table("RentalProperty")]
    public class RentalProperty
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        public string PropertyName { get; set; }
        [Required]
        [StringLength(100)]
        public string OwnerName { get; set; }
        [Required]
        [StringLength(512)]
        public string Address { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Number of rooms must be greater than 0.")]
        public int NumberOfRooms { get; set; }
        [Required]
        [Range(1, double.MaxValue, ErrorMessage = "Total area must be greater than 0.")]
        public double TotalArea { get; set; }
        [Required]
        [StringLength(50)]
        public string PropertyType { get; set; }
        public string Facilities { get; set; }
        [Required]
        [DataType(DataType.Currency)]
        public decimal ElectricityPrice { get; set; }
        [Required]
        [DataType(DataType.Currency)]
        public decimal WaterPrice { get; set; }
        [Required]
        [Phone]
        public string OwnerPhoneNumber { get; set; }
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }
        public string PropertyImage { get; set; }
        public bool IsActive { get; set; }
        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; }
        public ICollection<Room> Rooms { get; set; } = new List<Room>();
    }
}