using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace App.Models
{
    public class OwnAsset
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string OwnAssetID { get; set; }
        [DataType(DataType.DateTime)]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }
        [Required]
        public string AssetID { get; set; }
        [Required]
        public string RoomID { get; set; }
        [ForeignKey("AssetID")]
        public virtual Asset Asset { get; set; }
        [ForeignKey("RoomID")]
        public virtual Room Room { get; set; }

    }
}

