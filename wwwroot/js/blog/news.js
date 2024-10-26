$(document).ready(function () {
    loadNews();

    function loadNews() {
        $.ajax({
            url: '/get-all-news', // Ensure the path is correct
            method: 'GET',
            dataType: 'json',
            success: function (data) {
                console.log(data);

                // Destroy the DataTable if it exists
                if ($.fn.DataTable.isDataTable('#news-list')) {
                    $('#news-list').DataTable().destroy();
                }

                // Clear the existing table body
                $('#news-list tbody').empty();

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
                                <input type="checkbox" ${item.published ? "checked" : ""}  /> 
                            </td>
                            <td>
                                <a href="/edit-news/${item.id}" data-toggle="tooltip" data-placement="top" title="Edit">
                                    <i class="fa fa-pencil color-muted m-r-5"></i>
                                </a>
                                <a href="/delete-news/${item.id}" data-toggle="tooltip" data-placement="top" title="Delete">
                                    <i class="fas fa-trash color-muted m-r-5"></i>
                                </a>
                            </td>
                        </tr>`;
                    $('#news-list tbody').append(row); // Add the new row to the table
                });

                // Reinitialize the DataTable
                $('#news-list').DataTable({
                    responsive: true,
                    // You can add other options for DataTable here
                });
            },
            error: function (xhr, status, error) {
                console.error("Error loading news:", status, error);
            }
        });
    }

    // Function to format dates if needed
    function formatDate(dateString) {
        const date = new Date(dateString);
        return date.toLocaleDateString('vi-VN', {
            year: 'numeric',
            month: '2-digit',
            day: '2-digit'
        });
    }
});
