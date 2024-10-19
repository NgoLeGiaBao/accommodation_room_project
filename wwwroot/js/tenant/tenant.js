document.addEventListener('DOMContentLoaded', function () {
    // Load rental properties
    $.ajax({
        url: '/get-list-rental-property',
        method: 'POST',
        dataType: 'json',
        success: function (data) {
            // Get element by id
            var tabList = $('#tab-list');
            tabList.empty();

            let activeTabId = localStorage.getItem('activeTab');
            if (data.rentalProperties && data.rentalProperties.length > 0) {
                $.each(data.rentalProperties, function (index, item) {
                    var activeClass = item.rentalPropertyId == activeTabId ? 'active' : (index === 0 && !activeTabId ? 'active' : '');
                    var tabItem = `<li class="nav-item">
                        <a class="nav-link ${activeClass}" href="javascript:void(0);" data-id="${item.rentalPropertyId}">${item.propertyName}</a>
                    </li>`;
                    tabList.append(tabItem);
                });

                loadUsers(activeTabId || data.rentalProperties[0].rentalPropertyId);

                // Add click event for tab links
                $('.nav-link').on('click', function (event) {
                    event.preventDefault();
                    $('.nav-link').removeClass('active');
                    $(this).addClass('active');
                    localStorage.setItem('activeTab', $(this).data('id'));
                    loadUsers($(this).data('id'));
                });
            }
        },
        error: function (xhr, status, error) {
            console.error("AJAX Error:", status, error); // Handle error
        }
    });

    function loadUsers(homeId) {
        $.ajax({
            url: `/get-list-user/${homeId}`,
            method: 'POST',
            dataType: 'json',
            success: function (response) {
                if ($.fn.DataTable.isDataTable('#user-list')) {
                    $('#user-list').DataTable().destroy();
                }

                console.log(response)
                var userList = $('#user-list tbody');
                var btnAction = $('#btn-action');
                var statusInfo = $('#status-info');

                btnAction.empty();
                statusInfo.empty();
                userList.empty();


                statusInfo.append(`<span>All tenants ${response.totalUsers}</span> | <span>Current tenants ${response.totalUsers}</span> | <span>Previous tenants ${response.totalUsers}</span>`);
                var createTenantUrl = `/create-tenant/${homeId}`;
                var exportTenantUrl = `/edit-home/${homeId}`;
                btnAction.append(`
                                <div class="action-buttons mb-2">
                                    <a href = ${createTenantUrl}
                                        class="btn btn-primary me-2 ">
                                        <i class="fas fa-plus"></i> Add tenant
                                    </a>
                                    <a href = ${exportTenantUrl}
                                        class="btn btn-info me-2 ">
                                        <i class="fas fa-edit"></i> Export file
                                    </a>
                                </div>`);
                if (response.users && response.users.length === 0) {
                    userList.append('<tr><td colspan="9" class="text-center">No tenants found</td></tr>');
                    return;
                }
                $.each(response.users, function (index, user) {
                    var userRow = `<tr>
                        <td>${index + 1}</td>
                        <td>${user.user.fullName}</td>
                        <td>${user.user.sex ? 'Male' : 'Female'}</td>
                        <td>${formatDate(user.user.birthday)}</td>
                        <td>${user.user.identityCard}</td>
                        <td>${user.user.address}</td>
                        <td>
                            <span>
                                <a href="/edit-user/${user.user.id}" data-toggle="tooltip" data-placement="top" title="Edit">
                                    <i class="fa fa-pencil color-muted m-r-5"></i>
                                </a>
                                <a href="/view-user/${user.user.id}" data-toggle="tooltip" data-placement="top" title="View">
                                    <i class="fa fa-eye color-info"></i>
                                </a>
                            </span>
                        </td>
                    </tr>`;
                    userList.append(userRow);
                });

                $('#user-list').DataTable({
                    responsive: true
                });

                $('[data-toggle="tooltip"]').tooltip();
            },
            error: function (xhr, status, error) {
                console.error("Error loading users:", status, error);
            }
        });
    }

    // Formate date
    function formatDate(dateString) {
        const date = new Date(dateString);
        return date.toLocaleDateString('vi-VN', {
            year: 'numeric',
            month: '2-digit',
            day: '2-digit'
        });
    }

    // When the page loads, check for saved tab
    let activeTabId = localStorage.getItem('activeTab');
    if (activeTabId) {
        $(`.nav-link[data-id="${activeTabId}"]`).trigger('click');
    } else {
        $('.nav-link').first().trigger('click');
    }
});
