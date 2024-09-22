using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace App.Models
{
    [Table("Menu")]
    public class Menu
    {
        [Key]
        public int Id { get; set; }

        [Column(TypeName = "nvarchar")]
        [StringLength(50)]
        [Required(ErrorMessage = "Required {0}")]
        [Display(Name = "Menu Name")]
        public string MenuName { get; set; }

        [Required(ErrorMessage = "Required  {0}")]
        [StringLength(100)]
        [Display(Name = "Menu Link")]
        public string MenuLink { get; set; }


        [Required(ErrorMessage = "Required  {0}")]
        [StringLength(100)]
        [Display(Name = "Menu Meta")]
        public string MenuMeta { get; set; }

        [DisplayName("Menu Date Begin")]
        public DateTime MenuDateBegin { get; set; }

        [DisplayName("Menu Hide")]
        public bool MenuHide {  get; set; }
    }
}
