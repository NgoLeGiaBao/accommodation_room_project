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
        var btnAction = $('#btn-action');
        btnAction.empty();

        var createContractUrl = `/create-contract/${homeId}`;
        var exportContractUrl = `/edit-home/${homeId}`;
        btnAction.append(`
                                <div class="action-buttons mb-2">
                                    <a href = ${createContractUrl}
                                        class="btn btn-primary me-2 ">
                                        <i class="fas fa-plus"></i> Create new contract
                                    </a>
                                    <a href = ${exportContractUrl}
                                        class="btn btn-info me-2 ">
                                        <i class="fas fa-edit"></i> Export file
                                    </a>
                                </div>`);
        $.ajax({
            url: `/get-list-contracts/${homeId}`, // Update the URL to fetch contracts
            method: 'GET',
            dataType: 'json',
            success: function (response) {
                if ($.fn.DataTable.isDataTable('#contract-list')) {
                    $('#contract-list').DataTable().destroy();
                }

                var contractList = $('#contract-list tbody');
                var btnAction = $('#btn-action');
                var statusInfo = $('#status-info');

                btnAction.empty();
                statusInfo.empty();
                contractList.empty();

                // Example status info (adjust based on contract data)
                statusInfo.append(`<span>All ${response.allContracts}</span> | <span>Soon ${response.pendingContracts}</span> | <span>Active ${response.activeContracts}</span> | <span>Completed ${response.completedContracts}</span>`);

                // Adjust button actions if needed
                var createContractUrl = `/create-contract/${homeId}`;
                var exportContractUrl = `/export-contracts/${homeId}`;
                btnAction.append(`
                                                        <div class="action-buttons mb-2">
                                                            <a href = ${createContractUrl}
                                                                class="btn btn-primary me-2 ">
                                                                <i class="fas fa-plus"></i> Create Contract
                                                            </a>
                                                            <a href = ${exportContractUrl}
                                                                class="btn btn-info me-2 ">
                                                                <i class="fas fa-file-export"></i> Export Contracts
                                                            </a>
                                                        </div>`);

                if (response.contracts && response.contracts.length === 0) {
                    contractList.append('<tr><td colspan="6" class="text-center">No contracts found</td></tr>');
                    return;
                }

                $.each(response.contracts, function (index, contract) {
                    console.log(contract);
                    var contractRow = `<tr>
                                                <td>${index + 1}</td>
                                                <td>${contract.tenantName}</td>
                                                <td>${contract.roomName}</td>
                                                <td>${formatDate(contract.startingDate)}</td>
                                                <td>${formatDate(contract.expirationDate)}</td>
                                                <td>
                                                    <span>
                                                        <a href="/edit-contract/${contract.contractID}" data-toggle="tooltip" data-placement="top" title="Edit">
                                                            <i class="fa fa-pencil color-muted m-r-5"></i>
                                                        </a>
                                                        <a href="/view-contract/${contract.contractID}" data-toggle="tooltip" data-placement="top" title="View">
                                                            <i class="fa fa-eye color-info"></i>
                                                        </a>
                                                    </span>
                                                </td>
                                            </tr>`;
                    contractList.append(contractRow);
                });

                $('#contract-list').DataTable({
                    responsive: true
                });

                $('[data-toggle="tooltip"]').tooltip();
            },
            error: function (xhr, status, error) {
                console.error("Error loading contracts:", status, error);
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
