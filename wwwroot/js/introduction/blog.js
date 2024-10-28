$(document).ready(function () {
    loadNews();

    function loadNews() {
        $.ajax({
            url: '/get-all-news-intro',
            method: 'GET',
            dataType: 'json',
            success: function (data) {
                console.log(data);

                // Clear the existing content
                $('#news-list').empty();
                $('#news-list').append(`
                    <div>
                    <div class="fh5co_heading fh5co_heading_border_bottom py-2 mb-4">News</div>
                </div>
                <div class="row pb-4">
                    <div class="col-md-5">
                        <div class="fh5co_hover_news_img">
                            <div class="fh5co_news_img"><img src="lib/lib_intro/images/nathan-mcbride-229637.jpg"
                                    alt="" /></div>
                            <div></div>
                        </div>
                    </div>
                    <div class="col-md-7 animate-box">
                        <a href="single.html" class="fh5co_magna py-2"> Magna aliqua ut enim ad minim veniam quis
                            nostrud quis xercitation ullamco. </a> <a href="#" class="fh5co_mini_time py-3"> Thomson
                            Smith -
                            April 18,2016 </a>
                        <div class="fh5co_consectetur"> Amet consectetur adipisicing elit, sed do eiusmod tempor
                            incididunt
                            ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation.
                        </div>
                    </div>
                </div>`);
                // Iterate over the data and append news cards
                $.each(data, function (index, item) {
                    var newsItem = `
                        <div class="row pb-4">
                            <div class="col-md-5">
                                <div class="fh5co_hover_news_img">
                                    <div class="fh5co_news_img">
                                        <img src="${item.imageUrl || 'lib/lib_intro/images/default-image.jpg'}" alt="" />
                                    </div>
                                </div>
                            </div>
                            <div class="col-md-7 animate-box">
                                <a href="/single/${item.id}" class="fh5co_magna py-2">${item.generalTitle}</a>
                                <br/>
                                <a href="#" class="fh5co_mini_time py-3">${item.authorName} - ${formatDate(item.dateUpdated)}</a>
                                <div class="fh5co_consectetur">${item.generalDescription}</div>
                            </div>
                        </div>`;
                    $('#news-list').append(newsItem); // Append each news item to the container
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
