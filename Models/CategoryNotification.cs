using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Models
{
    public class CategoryNotification
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string CategoryNoID { get; set; }
        [Required]
        [StringLength(255)]
        public string Name { get; set; }
        public ICollection<Notification> Notifications { get; set; }
    }
}

