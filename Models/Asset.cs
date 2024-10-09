using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Models
{
    public class Asset
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AssetID { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Asset Name", Prompt = "Enter asset name")]
        public string AssetName { get; set; }

        [ForeignKey("CategoryAssetID")]
        [Display(Name = "Category", Prompt = "Select category")]
        public int? CategoryAssetID { get; set; }
        public virtual CategoryAsset? CategoryAsset { get; set; }

        [Required]
        [Display(Name = "Purchase Date", Prompt = "Select purchase date")]
        public DateTime PurchaseDate { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        [Display(Name = "Cost", Prompt = "Enter cost")]
        public decimal Cost { get; set; }

        [StringLength(50)]
        [Display(Name = "Condition", Prompt = "Enter asset condition")]
        public string Condition { get; set; }

        [StringLength(100)]
        [Display(Name = "Location", Prompt = "Enter location")]
        public string Location { get; set; }

        [Display(Name = "Next Maintenance Due Date", Prompt = "Select next maintenance date")]
        public DateTime? NextMaintenanceDueDate { get; set; }

        [StringLength(255)]
        [Display(Name = "Image Path", Prompt = "Upload an image")]
        public string? ImagePath { get; set; }

        public virtual ICollection<OwnAsset>? OwnAssets { get; set; }
    }
}
