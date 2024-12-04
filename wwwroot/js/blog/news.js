$(document).ready(function () {
    loadNews('all'); // Load all news initially

    // Listen for status change from the dropdown
    $('#news-status').on('change', function () {
        var selectedStatus = $(this).val();
        loadNews(selectedStatus);
    });

    // Function to load the news
    function loadNews(status) {
        $.ajax({
            url: '/get-all-news', // Make sure the URL matches your route
            method: 'POST',
            dataType: 'json',
            data: { status: status }, // Send the selected status to the server
            success: function (data) {
                console.log(data);

                // Destroy the DataTable if it exists
                if ($.fn.DataTable.isDataTable('#news-list')) {
                    $('#news-list').DataTable().destroy();
                }

                // Clear the existing table body
                $('#news-list tbody').empty();

                if (data.message === "No news found") {
                    // If no news is found, display a message
                    $('#news-list tbody').append('<tr><td colspan="6" class="text-center">No news found</td></tr>');
                } else {
                    // Iterate over the data and append rows to the table
                    $.each(data, function (index, item) {
                        var row = `
                            <tr>
                                <td>
                                    <input type="checkbox" id="check_${item.id}" />
                                </td>
                                <td>${index + 1}</td>
                                <td>${item.generalTitle}</td>
                                <td>
                                    <img src="${item.imageUrl || '/path/to/default/image.jpg'}" alt="Image" style="max-width: 50px; max-height: 50px;" />
                                </td>
                                <td>
                                    <input type="checkbox" ${item.published ? "checked" : ""} class="checkbox-published" data-id="${item.id}" />
                                </td>
                                <td>
                                    <a href="/edit-news/${item.id}" data-toggle="tooltip" data-placement="top" title="Edit">
                                        <i class="fa fa-pencil color-muted m-r-5"></i>
                                    </a>
                                    <a href="#" class="btn-delete" data-id="${item.id}" data-toggle="tooltip" data-placement="top" title="Delete">
                                        <i class="fas fa-trash color-muted m-r-5"></i>
                                    </a>
                                </td>
                            </tr>`;
                        $('#news-list tbody').append(row); // Add the new row to the table
                    });
                }

                // Reinitialize the DataTable
                $('#news-list').DataTable({
                    responsive: true,
                });

                // Bind the onchange event for the published checkbox
                $('.checkbox-published').on('change', handleCheckboxChange);
                // Bind the click event for the delete button
                $('.btn-delete').on('click', handleDeleteClick);
            },
            error: function (xhr, status, error) {
                console.error("Error loading news:", status, error);
                $('#news-list tbody').append('<tr><td colspan="6" class="text-center">Error loading news. Please try again.</td></tr>');
            }
        });
    }

    // Handler for checkbox change (publish/unpublish)
    function handleCheckboxChange(event) {
        const isChecked = event.target.checked;
        const id = $(this).data('id');
        // Send the updated status to the backend
        $.ajax({
            url: '/update-news-status',
            method: 'POST',
            contentType: 'application/json',
            data: JSON.stringify({
                id: id,
                published: isChecked
            }),
        });
    }

    // Handler for delete button click
    function handleDeleteClick(event) {
        event.preventDefault();
        const itemId = $(this).data('id'); // Get the ID of the item to be deleted

        // Create the modal dynamically
        var modalHtml = `
            <div class="modal fade" id="deleteModal" tabindex="-1" role="dialog" aria-labelledby="deleteModalLabel" aria-hidden="true">
                <div class="modal-dialog modal-dialog-centered" role="document">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title" id="deleteModalLabel">Confirm Deletion</h5>
                            <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                                <span aria-hidden="true">&times;</span>
                            </button>
                        </div>
                        <div class="modal-body">
                            Are you sure you want to delete this news item?
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-secondary btn-cancel" data-dismiss="modal">Cancel</button>
                            <button type="button" class="btn btn-primary" id="confirmDelete">Delete</button>
                        </div>
                    </div>
                </div>
            </div>
        `;

        // Append the modal to the body and show it
        $('body').append(modalHtml);
        $('#deleteModal').modal('show');

        // Bind the confirm delete action
        $('#confirmDelete').on('click', function () {
            // Perform AJAX delete request
            $.ajax({
                url: '/delete-news/' + itemId,
                method: 'POST',
                success: function (response) {
                    console.log('Item deleted successfully');
                    $('#deleteModal').modal('hide'); // Close the modal
                    $('#deleteModal').remove(); // Remove modal from DOM
                    loadNews($('#news-status').val()); // Reload the news list
                }
            });
        });

        // Ensure modal is removed from DOM after closing
        $('#deleteModal').on('hidden.bs.modal', function () {
            $(this).remove(); // Remove modal from DOM
        });

        // Handle cancel button and close icon
        $('.btn-cancel, .close').on('click', function () {
            $('#deleteModal').modal('hide'); // Close modal
        });
    }
});
