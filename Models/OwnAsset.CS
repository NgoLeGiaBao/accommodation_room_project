using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace App.Models
{
    public class OwnAsset
    {
        [Key]
        public int OwnAssetID { get; set; } // Khóa chính của bảng OwnAsset

        [DataType(DataType.DateTime)]
        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

        [Required]
        public int AssetID { get; set; } // Khóa ngoại đến tài sản

        [Required]
        public int RoomID { get; set; } // Khóa ngoại đến phòng

        [ForeignKey("AssetID")]
        public virtual Asset Asset { get; set; } // Tài sản liên kết

        [ForeignKey("RoomID")]
        public virtual Room Room { get; set; } // Phòng liên kết

    }
}

