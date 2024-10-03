using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace App.Models
{
    public class CategoryNotification
    {
        [Key]
        public int CategoryNoID { get; set; }
        [Required]
        [StringLength(255)]
        public string Name { get; set; }
        public ICollection<Notification> Notifications { get; set; }
    }
}

