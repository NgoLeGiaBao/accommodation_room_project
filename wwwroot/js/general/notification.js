document.addEventListener('DOMContentLoaded', function () {
    $.ajax({
        url: '/get-list-notification',  // URL để lấy thông báo
        method: 'POST',
        dataType: 'json',
        success: function (data) {
            // Kiểm tra nếu có thông báo trong dữ liệu
            if (data.length > 0) {
                // Cập nhật số lượng thông báo
                $('#notification-count').text(data.length);
                $('#notification-title').text(data.length + ' New Notifications');

                // Xây dựng HTML cho danh sách thông báo
                var notificationsHtml = '';
                data.forEach(function (notification) {
                    notificationsHtml += `
                        <li>
                            <a href="javascript:void(0)">
                                <span class="mr-3 avatar-icon ${notification.iconClass}">
                                    <i class="${notification.icon}"></i>
                                </span>
                                <div class="notification-content">
                                    <h6 class="notification-heading">${notification.title}</h6>
                                    <span class="notification-text">${notification.time}</span>
                                </div>
                            </a>
                        </li>
                    `;
                });

                // Chèn các thông báo vào #notification-layout
                $('#notification-layout').html(notificationsHtml);
            } else {
                // Nếu không có thông báo, tạo 1 <li> thông báo "No new notifications"
                var noNotificationsHtml = `
                    <li>
                        <a href="javascript:void(0)">
                            <span class="mr-3 avatar-icon bg-grey-lighten-2"><i class="icon-info"></i></span>
                            <div class="notification-content">
                                <h6 class="notification-heading">No new notifications</h6>
                                <span class="notification-text">You have no new notifications at this time.</span>
                            </div>
                        </a>
                    </li>
                `;
                $('#notification-layout').html(noNotificationsHtml); // Chèn thông báo không có mới
                $('#notification-title').text('No new notifications');
                $('#notification-count').text(0);
            }
        },
        error: function () {
            // Nếu có lỗi trong quá trình gọi AJAX
            $('#notification-title').text('Error fetching notifications');
            $('#notification-layout').html('<li>Failed to load notifications</li>');
            $('#notification-count').text(0);
        }
    });
});
