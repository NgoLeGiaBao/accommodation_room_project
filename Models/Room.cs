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
        public int RentalPropertyId { get; set; }
        public bool IsActive { get; set; }
        public string Status { get; set; }
        [ForeignKey("RentalPropertyId")]
        public RentalProperty RentalProperty { get; set; }
        public ICollection<RentalContract> RentalContracts { get; set; }
        public virtual ICollection<OwnAsset> OwnAssets { get; set; }
        public virtual ICollection<Invoice> Invoices { get; set; }
    }
}