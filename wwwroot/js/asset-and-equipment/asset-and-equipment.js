document.addEventListener('DOMContentLoaded', function () {
    // Load rental properties
    $.ajax({
        url: '/get-list-rental-property',
        method: 'POST',
        dataType: 'json',
        success: function (data) {
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
            console.error("AJAX Error:", status, error); // Handle error
        }
    });

    function loadAssets(homeId) {
        $.ajax({
            url: `/get-list-assets/${homeId}`,
            method: 'POST',
            dataType: 'json',
            success: function (response) {
                if ($.fn.DataTable.isDataTable('#asset-list')) {
                    $('#asset-list').DataTable().destroy();
                }

                var assetList = $('#asset-list tbody');
                assetList.empty();

                if (response.assets && response.assets.length === 0) {
                    assetList.append('<tr><td colspan="9" class="text-center">No assets found</td></tr>');
                    return;
                }

                $.each(response.assets, function (index, asset) {
                    var assetRow = `<tr>
                        <td>${index + 1}</td>
                        <td>${asset.assetName}</td>
                        <td>${asset.category}</td>
                        <td>${formatDate(asset.purchaseDate)}</td>
                        <td>${asset.cost}</td>
                        <td>${asset.condition}</td>
                        <td>${asset.roomName}</td>
                        <td>${formatDate(asset.nextMaintenanceDueDate)}</td>
                        <td>
                            <a href="/edit-asset/${asset.assetID}" data-toggle="tooltip" title="Edit">
                                <i class="fa fa-pencil color-muted m-r-5"></i>
                            </a>
                        </td>
                    </tr>`;
                    assetList.append(assetRow);
                });

                $('#asset-list').DataTable({
                    responsive: true
                });

                $('[data-toggle="tooltip"]').tooltip();
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
