using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace App.Models
{
    public class CategoryNotification
    {
        [Key]
        public int CategoryNoID { get; set; } // Khóa chính

        [Required]
        [StringLength(255)]
        public string Name { get; set; } // Tên danh mục thông báo

        public ICollection<Notification> Notifications { get; set; } // Mối quan hệ một-nhiều
    }
}

