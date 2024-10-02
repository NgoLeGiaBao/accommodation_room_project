using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Models
{
    public class Notification
    {
        [Key]
        public int NotificationId { get; set; } // Khóa chính
        public int CategoryNoID { get; set; } // Khóa ngoại đến Category
        public string StatusNotification { get; set; } // Trạng thái thông báo
        public DateTime UpdatedDate { get; set; } // Ngày cập nhật
        public string CreatorUserId { get; set; } // Khóa ngoại đến User, người tạo thông báo

        public AppUser Creator { get; set; } // Thông tin người tạo
        public ICollection<OwnNotification> OwnNotifications { get; set; } // Danh sách người nhận thông báo

        // Danh sách các người nhận thông báo này
        public CategoryNotification CategoryNotification { get; set; }

    }
}