using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace App.Models
{
    public class OwnAsset
    {
        [Key]
        public int OwnAssetID { get; set; }
        [DataType(DataType.DateTime)]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }
        [Required]
        public int AssetID { get; set; }
        [Required]
        public int RoomID { get; set; }
        [ForeignKey("AssetID")]
        public virtual Asset Asset { get; set; }
        [ForeignKey("RoomID")]
        public virtual Room Room { get; set; }

    }
}

