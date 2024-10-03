using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
namespace App.Models
{
    public class OwnNotification
    {
        [Key, Column(Order = 0)]
        public int NotificationId { get; set; }
        [Key, Column(Order = 1)]
        public string UserId { get; set; }
        [ForeignKey("NotificationId")]
        public Notification Notification { get; set; }
        [ForeignKey("UserId")]
        public AppUser AppUsers { get; set; }
    }

}
