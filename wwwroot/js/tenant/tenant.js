document.addEventListener('DOMContentLoaded', function () {
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

            // Load users for the active or first rental property
            loadUsers(activeTabId || data.rentalProperties[0].rentalPropertyId);

            // Add click event for tab links
            $('.nav-link').on('click', function (event) {
                event.preventDefault(); // Prevent default action
                $('.nav-link').removeClass('active'); // Remove active class from all links
                $(this).addClass('active'); // Add active class to clicked link

                // Save current tab ID in local storage
                localStorage.setItem('activeTab', $(this).data('id'));

                // Load users for the selected rental property
                loadUsers($(this).data('id'));
            });
        },
        error: function (xhr, status, error) {
            console.error("AJAX Error:", status, error); // Handle error
        }
    });

    function loadUsers(homeId) {
        $.ajax({
            url: `/get-list-user/${homeId}`, // Change URL to get user data
            method: 'POST',
            dataType: 'json',
            success: function (response) {
                // Get the table body by ID
                var userList = $('#user-list tbody'); // Assuming you have a table with ID 'user-list'
                userList.empty(); // Clear existing user data

                // Check if there are users to display
                if (response.users.length === 0) {
                    userList.append('<tr><td colspan="9" class="text-center">No users found</td></tr>');
                    return;
                }

                // Loop through data and add users to the table
                $.each(response.users, function (index, user) {
                    var userRow = `<tr>
                        <td>${user.id}</td>
                        <td>${user.name}</td>
                        <td>${user.gender}</td>
                        <td>${user.dob}</td>
                        <td>${user.idCardNumber}</td>
                        <td>${user.phone}</td>
                        <td>${user.email}</td>
                        <td>${user.address}</td>
                        <td>
                            <span>
                                <a href="/edit-user/${user.id}" data-toggle="tooltip" data-placement="top" title="Edit">
                                    <i class="fa fa-pencil color-muted m-r-5"></i>
                                </a>
                                <a href="/view-user/${user.id}" data-toggle="tooltip" data-placement="top" title="View">
                                    <i class="fa fa-eye color-info"></i>
                                </a>
                            </span>
                        </td>
                    </tr>`;
                    userList.append(userRow); // Append the new row to the table body
                });



            },
            error: function (xhr, status, error) {
                console.error("Error loading users:", status, error); // Handle error
            }
        });
    }

    // When the page loads, check for saved tab
    let activeTabId = localStorage.getItem('activeTab');
    if (activeTabId) {
        // Find and activate the corresponding tab
        $(`.nav-link[data-id="${activeTabId}"]`).trigger('click'); // Simulate click on the saved tab
    } else {
        // If no saved tab, select the first tab
        $('.nav-link').first().trigger('click');
    }
});
