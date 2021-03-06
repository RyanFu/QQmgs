﻿(function ($) {
    $(function () {

        $('.button-collapse').sideNav();
        $('.parallax').parallax();

    }); // end of document ready
})(jQuery); // end of jQuery name space

$(document).ready(function () {
    $('.slider').slider({ full_width: true });
});

// Materialize.toast(message, displayLength, className, completeCallback);
//Materialize.toast("Welcome to QQmgs.com", 5000); // 5000 is the duration of the toast

// toc moving
$(document).ready(function () {
    $('.scrollspy').scrollSpy();
});

// load full image in photo view
function loadFullImage(e) {
    e.src = e.src.replace('Thumbnails', '');

    // remove all tooltips
    $('.tooltipped').tooltip('remove');

    // readd all tooltips
    $(document).ready(function () {
        $('.tooltipped').tooltip({ delay: 50 });
    });
}
