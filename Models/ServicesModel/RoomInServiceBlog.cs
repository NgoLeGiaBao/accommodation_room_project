using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Models.ServicesModel
{
    public class RooomInServiceBlog
    {
        [Key]
        public string? RooomInServiceBlogId { get; set; }
        public string? Name { get; set; }
        public double Area { get; set; }
        public decimal RentalPrice { get; set; }
        public int NumberOfRooms { get; set; }
        public int MaximumPerson { get; set; }
        public string? Description { get; set; }
        public bool Bed { get; set; } = false;
        public bool AirConditioner { get; set; } = false;
        public bool TV { get; set; } = false;
        public bool Socket { get; set; } = false;
        public bool Wardrobe { get; set; } = false;
        public bool Fan { get; set; } = false;
        public bool Refrigerator { get; set; } = false;
        public bool WaterHeater { get; set; } = false;
        public bool Desk { get; set; } = false;
        public bool Chair { get; set; } = false;
        public bool Curtains { get; set; } = false;
        public bool Lighting { get; set; } = false;
        public bool Wifi { get; set; } = false;
        public bool Window { get; set; } = false;
        public bool Mirror { get; set; }
        public List<string> ImageUrls { get; set; } = new List<string>();

        // Foreign Key and Navigation Property
        [ForeignKey("ServicesBlog")]
        public string? ServicesBlogId { get; set; }
        public ServicesBlog? ServicesBlog { get; set; }
    }
}