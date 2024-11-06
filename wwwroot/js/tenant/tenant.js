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

                console.log(response);
                var userList = $('#user-list tbody');
                var btnAction = $('#btn-action');
                var statusInfo = $('#status-info');

                btnAction.empty();
                statusInfo.empty();
                userList.empty();

                // Update the status info with the correct counts
                statusInfo.append(`
                    <span>All tenants: ${response.totalUsers}</span> | 
                    <span>Current tenants: ${response.totalCurrentTenants}</span> | 
                    <span>Previous tenants: ${response.totalPastTenants}</span> | 
                    <span>Never rented: ${response.totalNeverRented}</span>
                `);

                var createTenantUrl = `/create-tenant/${homeId}`;
                btnAction.append(`
                    <div class="action-buttons mb-2">
                        <a href="${createTenantUrl}" class="btn btn-primary me-2">
                            <i class="fas fa-plus"></i> Add tenant
                        </a>
                        <a href="javascript:void(0);" data-home-id="${homeId}" onclick="exportExcel(this)" class="btn btn-info me-2">
                            <i class="fas fa-edit"></i> Export file
                        </a>
                    </div>
                `);

                if (response.usersWithCategories && response.usersWithCategories.length === 0) {
                    userList.append('<tr><td colspan="7" class="text-center">No tenants are found</td></tr>');
                    return;
                }

                $.each(response.usersWithCategories, function (index, user) {
                    var userRow = `<tr>
                        <td>${index + 1}</td>
                        <td>${user.user.fullName}</td>
                        <td>${user.user.sex ? 'Female' : 'Male'}</td>
                        <td>${formatDate(user.user.birthday)}</td>
                        <td>${user.user.identityCard}</td>
                        <td>${user.user.address}</td>
                        <td>
                            <span>
                                <a href="/edit-tenant/${user.user.id}" data-toggle="tooltip" data-placement="top" title="Edit">
                                    <i class="fa fa-pencil color-muted m-r-5"></i>
                                </a>
                                <a href="javascript:void(0)" onclick="viewTenant('${user.user.id}')" data-toggle="tooltip" data-placement="top" title="View">
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

    // Format date
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

function exportExcel(element) {
    var homeId = element.getAttribute("data-home-id");
    var url = `/export-list-user/${homeId}`;
    // Navigate to URL to download the Excel file
    window.location.href = url;
}

function viewTenant(idUser) {
    // Make AJAX call to get tenant data
    $.ajax({
        url: '/get-tenant/' + idUser, // Ensure the correct route is being used
        type: 'post',
        success: function (data) {
            // If data is returned, populate the modal
            if (data) {
                var targetElement = document.getElementById('view-profile');
                targetElement.innerHTML = `
                    <div class="modal fade" id="viewProfileModalpopover" tabindex="-1" role="dialog" aria-labelledby="viewProfileModalpopoverLabel" aria-hidden="true">
                        <div class="modal-dialog modal-dialog-centered" role="document">
                            <div class="modal-content">
                                <div class="modal-header">
                                    <h5 class="modal-title" id="viewProfileModalpopoverLabel">View Profile</h5>
                                    <button onclick="closeModal()" type="button" class="close" data-dismiss="modal" aria-label="Close">
                                        <span aria-hidden="true">&times;</span>
                                    </button>
                                </div>
                                <div class="modal-body">
                                    <div class="card">
                                        <div class="card-body">
                                            <!-- Avatar and Name -->
                                            <div class="media align-items-center mb-4">
                                                <img class="mr-3" src="images/avatar/11.png" width="80" height="80" alt="Avatar">
                                                <div class="media-body">
                                                    <h3 class="mb-0">${data.fullName}</h3>
                                                    <p class="text-muted mb-0">${data.address}</p>
                                                </div>
                                            </div>

                                            <!-- About Me Section -->
                                            <h4>About Me</h4>
                                            <p class="text-muted">Hi, I'm ${data.fullName}, and I've been the industry standard dummy text ever since the 1500s.</p>

                                            <!-- Contact Information -->
                                            <ul class="card-profile__info">
                                                <li class="mb-1"><strong class="text-dark mr-4">Mobile</strong>
                                                    <span>${data.phoneNumber}</span>
                                                </li>
                                                <li><strong class="text-dark mr-4">Identity Card</strong>
                                                    <span>${data.identityCard}</span>
                                                </li>
                                            </ul>
                                        </div>
                                    </div>
                                </div>
                                <div class="modal-footer">
                                    <button type="button" class="btn btn-secondary" data-dismiss="modal" onclick="closeModal()">Close</button>
                                    <button type="button" class="btn btn-primary" onclick="directToUpdate('${data.id}')">Update</button>
                                </div>
                            </div>
                        </div>
                    </div>
                `;

                // Initialize the modal and show it
                $('#viewProfileModalpopover').modal('show');
            }
        },
        error: function () {
            alert("An error occurred while fetching tenant data.");
        }
    });
}

function closeModal() {
    $('#viewProfileModalpopover').modal('hide');
    window.location.href = "/"
}
function directToUpdate(idUser) {
    window.location.href = "/edit-tenant/" + idUser;
}
