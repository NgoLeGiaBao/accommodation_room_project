using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Models
{
    public class Notification
    {
        [Key]
        public int NotificationId { get; set; }
        public int CategoryNoID { get; set; }
        public string StatusNotification { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string CreatorUserId { get; set; }

        public AppUser Creator { get; set; }
        public ICollection<OwnNotification> OwnNotifications { get; set; }
        public CategoryNotification CategoryNotification { get; set; }

    }
}