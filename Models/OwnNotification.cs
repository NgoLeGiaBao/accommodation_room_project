using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
namespace App.Models
{
    public class OwnNotification
    {
        [Key, Column(Order = 0)]
        public int NotificationId { get; set; } // Khóa ngoại đến bảng Notification

        [Key, Column(Order = 1)]
        public string UserId { get; set; } // Khóa ngoại đến bảng User

        [ForeignKey("NotificationId")]
        public Notification Notification { get; set; } // Mối quan hệ với Notification

        [ForeignKey("UserId")]
        public AppUser AppUsers { get; set; } // Mối quan hệ với User
    }

}
