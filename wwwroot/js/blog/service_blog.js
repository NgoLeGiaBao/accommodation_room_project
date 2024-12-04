$(document).ready(function () {
    // Danh sách trạng thái và tên hiển thị bằng tiếng Anh
    const statusList = [
        { id: 'all', name: 'All' },
        { id: 'active', name: 'Active' },
        { id: 'regular', name: 'Regular Rentals' },
        { id: 'vip', name: 'VIP Rentals' },
        { id: 'hot', name: 'Hot Rentals' },
        { id: 'inactive', name: 'Inactive' },
        { id: 'blocked', name: 'Blocked' },
        { id: 'closed', name: 'Closed' }
    ];

    // Hàm render các tab
    function renderTabs(statusList) {
        const tabList = $('#tab-list'); // Lấy danh sách tab
        tabList.empty(); // Xóa các phần tử cũ nếu có

        // Lặp qua từng trạng thái và thêm vào danh sách tab
        statusList.forEach((item, index) => {
            const activeClass = index === 0 ? 'active' : ''; // Tab đầu tiên là active
            const tabItem = `
                <li class="nav-item">
                    <a class="nav-link ${activeClass}" href="javascript:void(0);" data-id="${item.id}">
                        ${item.name}
                    </a>
                </li>`;
            tabList.append(tabItem); // Thêm tab vào danh sách
        });

        // Gắn sự kiện click cho các tab
        tabList.find('.nav-link').on('click', function () {
            // Xóa class active khỏi tất cả các tab
            tabList.find('.nav-link').removeClass('active');
            // Thêm class active cho tab hiện tại
            $(this).addClass('active');
            // Có thể thêm logic xử lý khác tại đây, ví dụ:
            const selectedId = $(this).data('id'); // Lấy ID tab được nhấn
            console.log(`Selected Tab ID: ${selectedId}`);
        });
    }

    // Hàm gọi API và render dữ liệu
    function fetchAndRenderServices() {
        $.ajax({
            url: "/get-all-services-news",
            method: "GET",
            dataType: "json",
            success: function (data) {
                const servicesContainer = $("#servies-news-list");
                servicesContainer.empty(); // Xóa nội dung cũ

                // Duyệt qua từng căn
                data.forEach(service => {
                    console.log(service)
                    const serviceHtml = `
                            <div class="row mb-4">
                                <div class="col-lg-3">
                                    <img style = "width: 250px; height: 200px" class="img-fluid" src="https://imbfaeyswkneestptfkr.supabase.co/storage/v1/object/public/home_img/${service.images[0]}" alt="${service.title}">
                                </div>
                                <div class="col-lg-9">
                                    <div class="row">
                                        <h2>${service.title}</h2>
                                        <p style="color:#71727a"><i class="fa fa-map-marker" aria-hidden="true"></i>
                                            ${service.address}</p>
                                        <p style="color:#71727a"><i class="fas fa-info-circle"></i>
                                            Nhà Trọ, phòng trọ</p>
                                        <div class="form-check form-check-inline">
                                            <label style="color: #71727a;" class="form-check-label padiding-left-label">
                                                <input type="checkbox" class="form-check-input" value="">
                                                Đã Đóng
                                            </label>
                                        </div>
                                        <div class="d-flex align-items-center mt-3">
                                            <a href="#" class="btn btn-danger me-2">Nâng cấp trọ</a>
                                            <div class="btn-group me-2">
                                                <button type="button" class="btn btn-primary dropdown-toggle"
                                                    data-bs-toggle="dropdown" aria-expanded="false">
                                                    <i class="fa fa-sort-numeric-desc" aria-hidden="true"></i>
                                                </button>
                                                <ul class="dropdown-menu">
                                                    <li><a class="dropdown-item" href="#"><i class="fa fa-refresh" style="padding-right: 8px" aria-hidden="true"></i>Làm mới ngay</a></li>
                                                    <li><a class="dropdown-item" href="#"><i class="fa fa-credit-card" aria-hidden="true" style="padding-right: 8px"></i>Mua gói làm mới</a></li>
                                                    <li><a class="dropdown-item" href="#"><i class="fa fa-magic" aria-hidden="true" style="padding-right: 8px"></i>Làm mới tự động</a></li>
                                                </ul>
                                            </div>
                                            <!-- Link "Thêm phòng" -->
                                            <a href="/add-room/${service.servicesBlogId}" class="btn btn-primary me-2" data-toggle="tooltip" data-placement="top" title="Thêm phòng">
                                                <i class="fa fa-plus-circle" aria-hidden="true"></i>
                                            </a>            
                                            <!-- Link "Chỉnh sửa trọ" -->
                                            <a href="/edit-property/${service.servicesBlogId}" class="btn btn-primary" data-toggle="tooltip" data-placement="top" title="Chỉnh sửa trọ">
                                                <i class="fa fa-pencil-square-o" aria-hidden="true"></i>
                                            </a>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-lg-12 card-child mt-3" id="list-rental-property">
                                    ${renderRooms(service.rooms)}
                                </div>
                            </div>`;
                    servicesContainer.append(serviceHtml);
                });
            },
            error: function (err) {
                console.error("Lỗi khi lấy dữ liệu:", err);
            }
        });
    }

    // Hàm render danh sách phòng
    function renderRooms(rooms) {
        let roomsHtml = "";
        rooms.forEach(room => {
            roomsHtml += `
                    <div class="row card-child-second pt-3 pb-3">
                        <div class="col-md-2 room-image">
                            <img style = "height: 150px !important; width: 150px !important" class="img-fluid" src="https://imbfaeyswkneestptfkr.supabase.co/storage/v1/object/public/room_in_ser_img/${room.imageUrls[0]}" alt="${room.name}">
                        </div>
                        <div class="col-md-4 bg-white d-flex align-items-center justify-content-between">
                            <div class="d-flex flex-column align-items-start">
                                <span class="title">${room.name}</span>
                                <span class="price">${formatCurrency(room.rentalPrice)}/tháng</span>
                            </div>
                            <div>
                                <i class="fas fa-chevron-right ms-2"></i>
                            </div>
                        </div>
                    </div>`;
        });
        return roomsHtml;
    }

    // Gọi hàm để load dữ liệu khi trang sẵn sàng
    fetchAndRenderServices();
    // Gọi hàm render
    renderTabs(statusList);

    function formatCurrency(amount) {
        return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(amount);
    }
});
