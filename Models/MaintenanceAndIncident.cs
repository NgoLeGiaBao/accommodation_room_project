using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Models
{
    [Table("MaintenanceAndIncident")]
    public class MaintenanceAndIncident
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string? IncidentId { get; set; }

        public string Description { get; set; }

        [StringLength(100)]
        public string? ReportedBy { get; set; }

        public DateTime? ReportedDate { get; set; }

        [StringLength(50)]
        public string? Status { get; set; }

        public DateTime? ResolvedOrMaintenanceCompletedDate { get; set; }

        public string? Notes { get; set; }

        public decimal? Cost { get; set; }

        [ForeignKey("Asset")]
        [Display(Name = "Assest Name")]
        public string AssetID { get; set; }
        public virtual Asset? Asset { get; set; }
        public string? RentalPropertyId { get; set; }
    }
}
