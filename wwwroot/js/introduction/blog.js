$(document).ready(function () {
    const pageSize = 8; // Number of items per page

    // Load the first page on startup
    loadNews(1);

    /**
     * Load the news articles from the API
     * @param {number} pageNumber - The current page to load
     */
    function loadNews(pageNumber) {
        $.ajax({
            url: '/get-news-pagination', // API endpoint
            method: 'GET',
            data: { pageNumber, pageSize }, // Pagination parameters
            dataType: 'json',
            success: function (data) {
                if (data && data.items && data.items.length > 0) {
                    $('#news-list').empty();

                    // Render the news articles
                    $.each(data.items, function (index, item) {
                        const newsItem = `
                            <div class="col-lg-4 col-md-6">
                                <div class="single-blog-item">
                                    <div class="sb-pic">
                                        <img src="${item.imageUrl || './client_part/img/blog/default-image.jpg'}" alt="Blog Image" class="img-fluid">
                                    </div>
                                    <div class="sb-text">
                                        <ul>
                                            <li><i class="fa fa-user"></i> ${item.authorName || 'Unknown Author'}</li>
                                            <li><i class="fa fa-clock-o"></i> ${(item.dateUpdated)}</li>
                                        </ul>
                                        <h4><a href="/news-detail/${item.id}">${item.generalTitle}</a></h4>
                                    </div>
                                </div>
                            </div>`;
                        $('#news-list').append(newsItem);
                    });

                    // Update the pagination controls
                    updatePagination(data.totalCount, pageNumber);
                } else {
                    $('#news-list').html('<p class="text-center">No news available.</p>');
                }
            },
            error: function (xhr, status, error) {
                console.error("Error loading news:", xhr.responseText || status || error);
            }
        });
    }

    /**
     * Update the pagination controls
     * @param {number} totalCount - Total number of articles
     * @param {number} currentPage - The current page
     */
    function updatePagination(totalCount, currentPage) {
        const totalPages = Math.ceil(totalCount / pageSize);
        const paginationList = $('#pagination-list');
        paginationList.empty();

        // Previous button
        const previousClass = currentPage === 1 ? 'disabled' : '';
        paginationList.append(`
            <li class="page-item ${previousClass}">
                <a class="page-link" href="#" data-page="${currentPage - 1}">Previous</a>
            </li>
        `);

        // Page number buttons
        for (let i = 1; i <= totalPages; i++) {
            const activeClass = currentPage === i ? 'active' : '';
            paginationList.append(`
                <li class="page-item ${activeClass}">
                    <a class="page-link" href="#" data-page="${i}">${i}</a>
                </li>
            `);
        }

        // Next button
        const nextClass = currentPage === totalPages ? 'disabled' : '';
        paginationList.append(`
            <li class="page-item ${nextClass}">
                <a class="page-link" href="#" data-page="${currentPage + 1}">Next</a>
            </li>
        `);

        // Add click event listeners for pagination buttons
        $('.page-link').off('click').on('click', function (event) {
            event.preventDefault();
            const page = $(this).data('page');
            if (page > 0 && page <= totalPages) {
                loadNews(page); // Load the selected page

                // Update the active state
                $('.page-item').removeClass('active'); // Remove active from all
                $(this).parent().addClass('active'); // Add active to the clicked button
            }
        });
    }

    /**
     * Format a date string to "dd/MM/yyyy"
     * @param {string} dateString - The date string from the API
     * @returns {string} - Formatted date
     */
    function formatDate(dateString) {
        const date = new Date(dateString);
        return date.toLocaleDateString('vi-VN', {
            year: 'numeric',
            month: '2-digit',
            day: '2-digit'
        });
    }
});
