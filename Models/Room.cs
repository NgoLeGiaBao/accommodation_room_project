using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Models
{
    [Table("Room")]
    public class Room
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Room name is required.")]
        [StringLength(100, ErrorMessage = "Room name cannot be longer than 100 characters.")]
        [Display(Name = "Room Name", Prompt = "Enter room name...")]
        public string RoomName { get; set; }

        [Required(ErrorMessage = "Area is required.")]
        [Range(1, double.MaxValue, ErrorMessage = "Area must be greater than zero.")]
        [Display(Name = "Area (sq. m)", Prompt = "Enter area in square meters...")]
        public double Area { get; set; }

        [Display(Name = "Maximum Number of People", Prompt = "Enter maximum number of people...")]
        [Range(1, int.MaxValue, ErrorMessage = "Maximum number of people must be at least 1.")]
        public int MaximumNumberOfPeople { get; set; }

        [Required(ErrorMessage = "Price is required.")]
        [DataType(DataType.Currency)]
        [Display(Name = "Price", Prompt = "Enter room price...")]
        public decimal Price { get; set; }

        [Display(Name = "Description", Prompt = "Enter room description...")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Rental Property ID is required.")]
        public int RentalPropertyId { get; set; }

        public bool IsActive { get; set; }

        [Display(Name = "Status", Prompt = "Enter room status...")]
        public string? Status { get; set; }

        [ForeignKey("RentalPropertyId")]
        public RentalProperty? RentalProperty { get; set; }

        public ICollection<RentalContract>? RentalContracts { get; set; }
        public virtual ICollection<OwnAsset>? OwnAssets { get; set; }
        public virtual ICollection<Invoice>? Invoices { get; set; }
    }
}