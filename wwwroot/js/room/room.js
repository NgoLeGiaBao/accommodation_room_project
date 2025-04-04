﻿document.addEventListener('DOMContentLoaded', function () {

    $('#room-status').on('change', function () {
        let selectedStatus = $(this).val(); // Lấy trạng thái đã chọn
        let activeTabId = localStorage.getItem('activeTab'); // Lấy tab hiện tại

        // Gọi hàm loadRooms với trạng thái đã chọn
        loadRooms(activeTabId, 1, selectedStatus);
    });


    $('#btnSearch').on('click', function () {
        // Process here
    });


    // Load rental properties
    $.ajax({
        url: '/get-list-rental-property',
        method: 'POST',
        dataType: 'json',

        success: function (data) {
            var tabList = $('#tab-list');
            tabList.empty(); // Clear current items

            // Check if there is an active tab stored in local storage
            let activeTabId = localStorage.getItem('activeTab');

            // Loop through data and add items to the list
            $.each(data.rentalProperties, function (index, item) {
                var activeClass = item.rentalPropertyId == activeTabId ? 'active' : (index === 0 && !activeTabId ? 'active' : ''); // Activate first tab if no active tab is stored
                var tabItem = `<li class="nav-item">
                    <a class="nav-link ${activeClass}" href="javascript:void(0);" data-id="${item.rentalPropertyId}">${item.propertyName}</a>
                </li>`;
                tabList.append(tabItem);
            });

            // Load rooms for the active or first rental property
            loadRooms(activeTabId || data.rentalProperties[0].rentalPropertyId);

            // Add click event for tab links
            $('.nav-link').on('click', function (event) {
                event.preventDefault(); // Prevent default action
                $('.nav-link').removeClass('active'); // Remove active class from all links
                $(this).addClass('active'); // Add active class to clicked link

                // Save current tab ID in local storage
                localStorage.setItem('activeTab', $(this).data('id'));

                // Load rooms for the selected rental property
                loadRooms($(this).data('id'));

                // Reload to all rooms
                $('#room-status').val('All');
            });
        },
        error: function (xhr, status, error) {
            console.error("AJAX Error:", status, error); // Handle error
        }
    });

    function loadRooms(homeId, pageNumber = 1, selectedStatus = null) {
        $.ajax({
            url: `/get-list-room/${homeId}`,
            method: 'POST',
            data: { pageNumber: pageNumber, pageSize: 8, selectedStatus: selectedStatus }, // Send pagination info
            dataType: 'json',
            success: function (response) {
                // Get elements by Id
                var roomList = $('#room-list');
                roomList.empty();

                // Get btnAction
                var btnAction = $('#btn-action');
                btnAction.empty();

                // Load btn action
                var createRoomUrl = `/create-room/${homeId}`;
                var editRoomUrl = `/edit-home/${homeId}`;
                btnAction.append(`
                    <div class="status-info mb-2">
                        <span>Available ${response.availableCount} </span> | <span>About to expire ${response.aboutToExpireCount}</span> | <span>Rented ${response.rentedCount}</span>
                    </div>
                    <div class="action-buttons mb-2">
                        <a href = ${createRoomUrl}
                            class="btn btn-primary me-2 ">
                            <i class="fas fa-plus"></i> Add room
                        </a>
                        <a href = ${editRoomUrl}
                            class="btn btn-info me-2 ">
                            <i class="fas fa-edit"></i> Update home
                        </a>
                     </div>
                    `);

                // Load rooms
                $.each(response.rooms, function (index, room) {
                    var editUrl = `/edit-room?rentalPropertyId=${homeId}&roomId=${room.id}`;
                    var viewUrl = `/view-room?rentalPropertyId=${homeId}&roomId=${room.id}`;

                    var roomItem = `<div class="col-lg-3 col-md-6 mb-4">
                        <div class="card text-center">
                            <div class="card-body">
                                <h5 class="card-title">${room.roomName}</h5>
                                <p class="card-text">
                                    Price: ${room.price}$
                                    <br>
                                    
                                    Status: <span class="badge badge-primary px-2">${room.status}</span>
                                </p>
                                <div class="d-flex justify-content-center">
                                    <a href="${editUrl}" class="btn btn-primary">Update</a>
                                    <a href="${viewUrl}" class="btn btn-secondary margin-room-update">View</a>
                                </div>
                            </div>
                        </div>
                    </div>`;
                    roomList.append(roomItem);
                });

                // Update pagination
                updatePagination(response.totalCount, homeId, pageNumber);
            },
            error: function (xhr, status, error) {
                console.error("Error loading rooms:", status, error); // Handle error
            }
        });
    }

    function updatePagination(totalCount, homeId, currentPage) {
        var pageSize = 8;
        var totalPages = Math.ceil(totalCount / pageSize);
        var paginationList = $('#pagination-list');
        paginationList.empty();

        // Add Previous button
        var previousClass = (currentPage === 1 || totalPages <= 1) ? 'disabled' : '';
        paginationList.append(
            `<li class="page-item ${previousClass}">
                <a class="page-link" href="#" data-page="${currentPage - 1}" data-home-id="${homeId}" tabindex="-1">Previous</a>
            </li>`
        );

        // Create page buttons
        for (var i = 1; i <= totalPages; i++) {
            var activeClass = currentPage === i ? 'active' : '';
            paginationList.append(
                `<li class="page-item ${activeClass}">
                    <a class="page-link" href="#" data-page="${i}" data-home-id="${homeId}">${i}</a>
                </li>`
            );
        }

        // Add Next button
        var nextClass = (currentPage === totalPages || totalPages <= 1) ? 'disabled' : '';
        paginationList.append(
            `<li class="page-item ${nextClass}">
                <a class="page-link" href="#" data-page="${currentPage + 1}" data-home-id="${homeId}">Next</a>
            </li>`
        );

        // Attach events to pagination links
        $('.page-link').on('click', function (event) {
            event.preventDefault(); // Prevent default action
            var page = $(this).data('page');
            var homeId = $(this).data('home-id');

            // Only load if not disabled
            if (page > 0 && page <= totalPages) {
                loadRooms(homeId, page); // Load rooms for selected page
            }
        });
    }

    // When the page loads, check for saved tab
    let activeTabId = localStorage.getItem('activeTab');
    if (activeTabId) {
        // Find and activate the corresponding tab
        $(`.nav-link[data-id="${activeTabId}"]`).trigger('click');
    } else {
        // If no saved tab, select the first tab
        $('.nav-link').first().trigger('click');
    }
});
