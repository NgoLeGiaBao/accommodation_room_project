document.addEventListener('DOMContentLoaded', function () {
    // Select all the nav links
    const navLinks = document.querySelectorAll('.nav-link');

    navLinks.forEach(link => {
        link.addEventListener('click', function (event) {
            // Remove 'active' class from all links
            navLinks.forEach(link => link.classList.remove('active'));
            // Add 'active' class to the clicked link
            this.classList.add('active');
        });
    });
});
