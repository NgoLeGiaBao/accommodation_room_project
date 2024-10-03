using System;
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
        public string AssetName { get; set; }
        public int CategoryAssetID { get; set; }
        [ForeignKey("CategoryAssetID")]
        public virtual CategoryAsset CategoryAsset { get; set; }
        [Required]
        public DateTime PurchaseDate { get; set; }
        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Cost { get; set; }
        [StringLength(50)]
        public string Condition { get; set; }
        [StringLength(100)]
        public string Location { get; set; }
        public DateTime? NextMaintenanceDueDate { get; set; }
        [StringLength(255)]
        public string ImagePath { get; set; }
        public virtual ICollection<OwnAsset> OwnAssets { get; set; }

    }

}
