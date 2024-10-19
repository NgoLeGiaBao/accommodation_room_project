using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Models
{
    public class CategoryAsset
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string CategoryAssetID { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        public virtual ICollection<Asset> Assets { get; set; }
    }
}
