$(document).ready(function () {
    // Load danh sách Tỉnh (Lấy tất cả Tỉnh ngay khi trang tải)
    $.getJSON('https://esgoo.net/api-tinhthanh/1/0.htm', function (data_tinh) {
        if (data_tinh.error === 0) {
            $.each(data_tinh.data, function (key_tinh, val_tinh) {
                $("#tinh").append('<option value="' + val_tinh.id + '">' + val_tinh.full_name + '</option>');
            });
        }
    });

    // Khi người dùng chọn Tỉnh
    $("#tinh").change(function () {
        var idtinh = $(this).val();

        // Xóa dữ liệu cũ của Quận/Huyện và Phường/Xã
        $("#quan").html('<option value="0">Quận Huyện</option>');
        $("#phuong").html('<option value="0">Phường Xã</option>');

        if (idtinh !== "0") {
            // Load danh sách Quận/Huyện theo Tỉnh
            $.getJSON('https://esgoo.net/api-tinhthanh/2/' + idtinh + '.htm', function (data_quan) {
                if (data_quan.error === 0) {
                    $.each(data_quan.data, function (key_quan, val_quan) {
                        $("#quan").append('<option value="' + val_quan.id + '">' + val_quan.full_name + '</option>');
                    });
                }
            });
        }
    });

    // Khi người dùng chọn Quận/Huyện
    $("#quan").change(function () {
        var idquan = $(this).val();

        // Xóa dữ liệu cũ của Phường/Xã
        $("#phuong").html('<option value="0">Phường Xã</option>');
        if (idquan !== "0") {
            // Load danh sách Phường/Xã theo Quận/Huyện
            $.getJSON('https://esgoo.net/api-tinhthanh/3/' + idquan + '.htm', function (data_phuong) {
                if (data_phuong.error === 0) {
                    $.each(data_phuong.data, function (key_phuong, val_phuong) {
                        $("#phuong").append('<option value="' + val_phuong.id + '">' + val_phuong.full_name + '</option>');
                    });
                }
            }).fail(function () {
                console.log("API call failed for phường data.");
            });
        }
    });

    // Room Size Slider
    $("#roomsize-range-P").slider({
        range: true,
        min: 10,
        max: 200,
        values: [10, 200],
        slide: function (event, ui) {
            $("#roomsizeRangeP").val(ui.values[0] + " - " + ui.values[1] + " m²");
        }
    });
    $("#roomsizeRangeP").val($("#roomsize-range-P").slider("values", 0) + " - " + $("#roomsize-range-P").slider("values", 1) + " m²");

    // Price Slider
    $("#price-range-P").slider({
        range: true,
        min: 500000,
        max: 10000000,
        values: [500000, 10000000],
        slide: function (event, ui) {
            var formattedMinPrice = new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(ui.values[0]);
            var formattedMaxPrice = new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(ui.values[1]);
            $("#priceRangeP").val(formattedMinPrice + " - " + formattedMaxPrice);
        }
    });
    $("#priceRangeP").val(
        new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format($("#price-range-P").slider("values", 0)) +
        " - " +
        new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format($("#price-range-P").slider("values", 1))
    );

    // Handle form submission (with AJAX)
    $("#searchForm").submit(function (e) {
        e.preventDefault();  // Prevent the default form submission

        // Get selected values
        var province = $("#tinh").val();
        var district = $("#quan").val();
        var town = $("#phuong").val();
        var minSize = $("#roomsize-range-P").slider("values", 0);
        var maxSize = $("#roomsize-range-P").slider("values", 1);
        var minPrice = $("#price-range-P").slider("values", 0);
        var maxPrice = $("#price-range-P").slider("values", 1);

        // Construct the URL with query parameters
        var url = '/get-filtered-services-blogs?';
        url += 'Province=' + province + '&District=' + district + '&Town=' + town +
            '&MinSize=' + minSize + '&MaxSize=' + maxSize + '&MinPrice=' + minPrice + '&MaxPrice=' + maxPrice;

        // Perform AJAX request to fetch room listings
        $.ajax({
            url: url,
            type: 'POST',
            success: function (response) {
                // Assuming response is an array of properties (rooms or houses)
                var properties = response

                // Clear existing content
                $('#propertyList').empty();
                // Check if there are any properties to display
                if (properties.length === 0) {
                    $('#propertyList').html('<p>No properties found.</p>');
                    return;
                }

                // Loop through the properties and render them
                $.each(properties, function (index, property) {
                    var propertyHtml = `
                        <div class="single-property-item">
                            <div class="row">
                                <div class="col-md-4">
                                    <div class="property-picw">
                                        <img class="phong-tro-img" src="https://imbfaeyswkneestptfkr.supabase.co/storage/v1/object/public/home_img/${property.images[0]}" alt="Property Image">
                                    </div>
                                </div>
                                <div class="col-md-8">
                                    <div class="property-text">
                                        <a href = "/detail-service_blog/${property.slug}">
                                            <h5 class="r-title">${property.title}</h5>
                                        <a/>
                                        <div class="room-price">
                                            <span>Giá từ:</span>
                                            <h5>${property.rentalPrice.toLocaleString('vi-VN', { style: 'currency', currency: 'VND' })}</h5>
                                        </div>
                                        <div class="properties-location phong-tro"><p class="phong-tro-child">Nhà trọ phòng trọ</p><p class="phong-tro-child ml-2">${property.area}m<sup>2</sup><p></div>
                                            <div class="properties-location"><i class="icon_pin"></i> ${property.address}</div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            `;
                    // Append the HTML to the property list container
                    $('#propertyList').append(propertyHtml);
                });
            }
        });
    });

    // You may also want to fetch initial data when the page loads, for the very first time.
    function loadInitialRoomListings() {
        var url = '/get-filtered-services-blogs'
        $.ajax({
            url: url,
            type: 'POST',
            success: function (response) {

                // Assuming response is an array of properties (rooms or houses)
                var properties = response // Adjust this if the structure is different

                // Clear existing content
                $('#propertyList').empty();

                // Check if there are any properties to display
                if (properties.length === 0) {
                    $('#propertyList').html('<p>No properties found.</p>');
                    return;
                }

                // Loop through the properties and render them
                $.each(properties, function (index, property) {
                    var propertyHtml = `
                        <div class="single-property-item">
                            <div class="row">
                                <div class="col-md-4">
                                    <div class="property-picw">
                                        <img class="phong-tro-img" src="https://imbfaeyswkneestptfkr.supabase.co/storage/v1/object/public/home_img/${property.images[0]}" alt="Property Image">
                                    </div>
                                </div>
                                <div class="col-md-8">
                                    <div class="property-text">
                                        <a href = "/detail-service-blog/${property.slug}">
                                            <h5 class="r-title">${property.title}</h5>
                                        <a/>
                                        <div class="room-price">
                                            <span>Giá từ:</span>
                                            <h5>${property.rentalPrice.toLocaleString('vi-VN', { style: 'currency', currency: 'VND' })}</h5>
                                        </div>
                                        <div class="properties-location phong-tro"><p class="phong-tro-child">Nhà trọ phòng trọ</p><p class="phong-tro-child ml-2">${property.area}m<sup>2</sup><p></div>
                                            <div class="properties-location"><i class="icon_pin"></i> ${property.address}</div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            `;
                    // Append the HTML to the property list container
                    $('#propertyList').append(propertyHtml);
                });
            }
        });
    }
    loadInitialRoomListings();
});

