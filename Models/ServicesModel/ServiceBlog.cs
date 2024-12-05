using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Models.ServicesModel
{
    public class ServicesBlog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string? ServicesBlogId { get; set; }
        // [Required]
        public string Title { get; set; }
        public string? SelfManagedAccommodation { get; set; }
        public string? Area { get; set; }
        public decimal? Size { get; set; }
        public decimal RentalPrice { get; set; }
        public string? ContentDescription { get; set; }

        // Object
        public bool IsStudent { get; set; } = false;
        public bool IsWorker { get; set; } = false;
        public bool IsFamily { get; set; } = false;
        public bool IsCouple { get; set; } = false;

        // Utilities
        public bool HasLoft { get; set; } = false;
        public bool HasWiFi { get; set; } = false;
        public bool HasIndoorCleaning { get; set; } = false;
        public bool HasBathroom { get; set; } = false;
        public bool HasWaterHeater { get; set; } = false;
        public bool HasKitchenShelf { get; set; } = false;
        public bool HasWashingMachine { get; set; } = false;
        public bool HasTV { get; set; } = false;
        public bool HasAirConditioner { get; set; } = false;
        public bool HasRefrigerator { get; set; } = false;
        public bool HasBedAndMattress { get; set; } = false;
        public bool HasWardrobe { get; set; } = false;
        public bool HasBalcony { get; set; } = false;
        public bool HasElevator { get; set; } = false;
        public bool HasPrivateParkingLot { get; set; } = false;
        public bool HasSecurityCamera { get; set; } = false;
        public bool HasSwimmingPool { get; set; } = false;
        public bool HasGarden { get; set; } = false;

        // Surrounding Environment
        public bool NearMarket { get; set; } = false;
        public bool NearSupermarket { get; set; } = false;
        public bool NearHospital { get; set; } = false;
        public bool NearSchool { get; set; } = false;
        public bool NearPark { get; set; } = false;
        public bool NearBusStation { get; set; } = false;
        public bool NearGym { get; set; } = false;

        // Location
        public string? Province { get; set; }
        public string? District { get; set; }
        public string? Town { get; set; }
        public string? Address { get; set; }

        // Contact Information
        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? ZaloNumber { get; set; }

        public string? Slug { get; set; }
        // Images
        public List<string> Images { get; set; } = new List<string>();
        public string? StatusBlog { get; set; } = "Active";
        public string? CreatedBy { get; set; }

        // Navigation Property: One-to-Many with RooomInServiceBlog
        public List<RooomInServiceBlog> Rooms { get; set; } = new List<RooomInServiceBlog>();
    }
}