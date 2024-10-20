document.addEventListener('DOMContentLoaded', function () {
    // Load rental properties
    $.ajax({
        url: '/get-list-rental-property',
        method: 'POST',
        dataType: 'json',
        success: function (data) {
            var tabList = $('#tab-list');
            tabList.empty();

            let activeTabId = localStorage.getItem('activeTab'); // Only declare this once
            if (data.rentalProperties && data.rentalProperties.length > 0) {
                $.each(data.rentalProperties, function (index, item) {
                    var activeClass = item.rentalPropertyId == activeTabId ? 'active' : (index === 0 && !activeTabId ? 'active' : '');
                    var tabItem = `<li class="nav-item">
                        <a class="nav-link ${activeClass}" href="javascript:void(0);" data-id="${item.rentalPropertyId}">${item.propertyName}</a>
                    </li>`;
                    tabList.append(tabItem);
                });

                // Load assets for the first or active tab
                loadAssets(activeTabId || data.rentalProperties[0].rentalPropertyId);

                // Add click event for tab links
                $('.nav-link').on('click', function (event) {
                    event.preventDefault();
                    $('.nav-link').removeClass('active');
                    $(this).addClass('active');
                    localStorage.setItem('activeTab', $(this).data('id'));
                    loadAssets($(this).data('id'));
                });
            }
        },
        error: function (xhr, status, error) {
            console.error("AJAX Error:", status, error);
        }
    });

    function loadAssets(homeId) {
        $.ajax({
            url: `/get-list-maintenance-incident/${homeId}`,
            method: 'GET',
            dataType: 'json',
            success: function (response) {
                console.log(response);
                if ($.fn.DataTable.isDataTable('#maintenance-list')) {
                    $('#maintenance-list').DataTable().destroy();
                }

                var btnAction = $('#btn-action');
                var statusInfo = $('#status-info');
                var maintenance = $('#maintenance-list tbody');

                btnAction.empty();
                statusInfo.empty();

                var createMaintenanceIncident = `/create-maintenance-incident/${homeId}`;
                var exportAssetUrl = `/create-asset-equipment/${homeId}`;
                btnAction.append(`
                    <div class="action-buttons mb-2">
                        <a href="${createMaintenanceIncident}" class="btn btn-primary me-2">
                            <i class="fas fa-plus"></i> Add Maintenance And Incident
                        </a>
                        <a href="${exportAssetUrl}" class="btn btn-info me-2">
                            <i class="fas fa-edit"></i> Export file
                        </a>
                    </div>`);

                statusInfo.append(`<span>All ${response.total}</span> | <span>Requested ${response.requested}</span> | <span>Processed ${response.processed}</span>`);

                maintenance.empty();

                if (response.maintenanceAndIncidents && response.maintenanceAndIncidents.length === 0) {
                    maintenance.append('<tr><td colspan="9" class="text-center">No maintenance and incidents found</td></tr>');
                    return;
                }

                $.each(response.maintenanceAndIncidents, function (index, incident) {
                    var incidentRow = `<tr>
                        <td>${index + 1}</td>
                        <td>${incident.assetName}</td>
                        <td>${incident.description}</td>
                        <td>${formatDate(incident.reportedDate)}</td>
                        <td>${incident.roomName}</td>
                        <td>
                            <a href="/edit-incident/${incident.incidentID}" data-toggle="tooltip" title="Edit">
                                <i class="fa fa-pencil color-muted m-r-5"></i>
                            </a>
                        </td>
                    </tr>`;
                    maintenance.append(incidentRow);
                });

                $('#maintenance-list').DataTable({
                    responsive: true
                });

                $('[data-toggle="tooltip"]').tooltip(); // Ensure Bootstrap JS is loaded
            },
            error: function (xhr, status, error) {
                console.error("Error loading assets:", status, error);
            }
        });
    }

    // Format date function
    function formatDate(dateString) {
        const date = new Date(dateString);
        return date.toLocaleDateString('vi-VN', {
            year: 'numeric',
            month: '2-digit',
            day: '2-digit'
        });
    }

    // Check for saved tab
    let activeTabId = localStorage.getItem('activeTab');
    if (activeTabId) {
        $(`.nav-link[data-id="${activeTabId}"]`).trigger('click');
    } else {
        $('.nav-link').first().trigger('click');
    }
});
