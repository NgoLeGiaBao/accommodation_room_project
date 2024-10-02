using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Models
{
    public class Asset
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AssetID { get; set; } // Mã số nhận dạng tài sản

        [Required]
        [StringLength(100)]
        public string AssetName { get; set; } // Tên tài sản

        public int CategoryAssetID { get; set; } // Khóa ngoại đến danh mục tài sản

        [ForeignKey("CategoryAssetID")]
        public virtual CategoryAsset CategoryAsset { get; set; } // Danh mục tài sản

        [Required]
        public DateTime PurchaseDate { get; set; } // Ngày mua

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Cost { get; set; } // Giá thành (đơn vị là đồng Việt Nam)

        [StringLength(50)]
        public string Condition { get; set; } // Tình trạng

        [StringLength(100)]
        public string Location { get; set; } // Vị trí

        public DateTime? NextMaintenanceDueDate { get; set; } // Ngày bảo trì tiếp theo

        [StringLength(255)]
        public string ImagePath { get; set; } // Đường dẫn đến hình ảnh
        public virtual ICollection<OwnAsset> OwnAssets { get; set; } // Danh sách các phòng sở hữu tài sản

    }

}
