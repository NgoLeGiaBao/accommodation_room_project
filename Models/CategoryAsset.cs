using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Models
{
    public class CategoryAsset
    {
        [Key]
        public int CategoryAssetID { get; set; } // Mã số nhận dạng danh mục tài sản

        [Required]
        [StringLength(50)]
        public string Name { get; set; } // Tên danh mục tài sản

        // Mối quan hệ một-nhiều
        public virtual ICollection<Asset> Assets { get; set; } // Danh sách tài sản trong danh mục
    }
}
