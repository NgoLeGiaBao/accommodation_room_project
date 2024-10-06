document.addEventListener('DOMContentLoaded', function () {
    // Thực hiện cuộc gọi AJAX để tải danh sách tab
    $.ajax({
        url: '/get-list-rental-property', // Thay đổi thành URL endpoint của bạn
        method: 'POST',
        dataType: 'json',
        success: function (data) {
            var tabList = $('#tab-list');
            tabList.empty(); // Xóa các mục hiện tại
            console.log(data);

            // Lặp qua dữ liệu và thêm các mục vào danh sách
            $.each(data.rentalProperties, function (index, item) {
                console.log(item);
                var activeClass = index === 0 ? 'active' : ''; // Đặt class active cho mục đầu tiên
                var tabItem = `<li class="nav-item">
                    <a class="nav-link ${activeClass}" href="javascript:void(0);" data-id="${item.id}">${item.propertyName}</a>
                </li>`;
                tabList.append(tabItem);
            });

            // Tải danh sách phòng trọ cho tab đầu tiên
            loadRooms(data.rentalProperties[0].id);

            // Thêm sự kiện click cho các liên kết tab
            const navLinks = document.querySelectorAll('.nav-link');
            console.log(navLinks);
            navLinks.forEach(link => {
                link.addEventListener('click', function (event) {
                    event.preventDefault(); // Ngăn chặn hành động mặc định cho tab
                    navLinks.forEach(link => link.classList.remove('active'));
                    this.classList.add('active');
                    loadRooms(this.getAttribute('data-id'));
                });
            });
        },
        error: function (xhr, status, error) {
            console.error("AJAX Error:", status, error); // Xử lý lỗi
        }
    });

    // Hàm tải danh sách phòng trọ
    function loadRooms(homeId) {
        $.ajax({
            url: `/get-list-room/${homeId}`,
            method: 'POST', // Đảm bảo rằng controller cũng xử lý POST
            dataType: 'json',
            success: function (rooms) {
                var roomList = $('#room-list');
                roomList.empty(); // Xóa danh sách phòng hiện tại

                // Lặp qua dữ liệu và thêm vào danh sách phòng
                $.each(rooms, function (index, room) {
                    // Tạo URL cho EditRoom và ViewRoom
                    var editUrl = `/edit-room?homeId=${homeId}&roomId=${room.id}`;
                    var viewUrl = `/view-room?homeId=${homeId}&roomId=${room.id}`;

                    var roomItem = `<div class="col-lg-3 col-md-6 mb-4">
                        <div class="card text-center">
                            <div class="card-body">
                                <h5 class="card-title">${room.roomName}</h5>
                                <p class="card-text">
                                    Room Price: ${room.price}
                                    <br>
                                    Room Status: <span class="badge badge-primary px-2">${room.status}</span>
                                </p>
                                <div class="d-flex justify-content-center">
                                    <a href="${editUrl}" class="btn btn-primary">Edit</a>
                                    <a href="${viewUrl}" class="btn btn-secondary margin-room-update">View</a>
                                </div>
                            </div>
                        </div>
                    </div>`;
                    roomList.append(roomItem);
                    roomList.append(roomItem);
                    roomList.append(roomItem);
                    roomList.append(roomItem);
                    roomList.append(roomItem);
                });
            },
            error: function (xhr, status, error) {
                console.error("Error loading rooms:", status, error); // Xử lý lỗi
            }
        });
    }
});
