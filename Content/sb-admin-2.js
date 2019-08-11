(function ($) {
    "use strict"; // Start of use strict

    // Toggle the side navigation
    $("#sidebarToggle, #sidebarToggleTop").on('click', function (e) {
        $("body").toggleClass("sidebar-toggled");
        $(".sidebar").toggleClass("toggled");
        if ($(".sidebar").hasClass("toggled")) {
            $('.sidebar .collapse').collapse('hide');
        };
    });

    // Close any open menu accordions when window is resized below 768px
    $(window).resize(function () {
        if ($(window).width() < 768) {
            $('.sidebar .collapse').collapse('hide');
        };
    });

    // Prevent the content wrapper from scrolling when the fixed side navigation hovered over
    $('body.fixed-nav .sidebar').on('mousewheel DOMMouseScroll wheel', function (e) {
        if ($(window).width() > 768) {
            var e0 = e.originalEvent,
                delta = e0.wheelDelta || -e0.detail;
            this.scrollTop += (delta < 0 ? 1 : -1) * 30;
            e.preventDefault();
        }
    });

    // Scroll to top button appear
    $(document).on('scroll', function () {
        var scrollDistance = $(this).scrollTop();
        if (scrollDistance > 100) {
            $('.scroll-to-top').fadeIn();
        } else {
            $('.scroll-to-top').fadeOut();
        }
    });

    // Smooth scrolling using jQuery easing
    $(document).on('click', 'a.scroll-to-top', function (e) {
        var $anchor = $(this);
        $('html, body').stop().animate({
            scrollTop: ($($anchor.attr('href')).offset().top)
        }, 1000, 'easeInOutExpo');
        e.preventDefault();
    });

    $(document).on('submit', 'form', function (e) {
        $('.btn').not('button.btn').each(function () {
            $(this).fadeTo("fast", 0.60);
            $(this).removeAttr("href");
        });
        $('button.btn').text('AGUARDE...');
        $('button.btn').fadeTo("fast", 0.50);
    });

    $(document).on('click', 'a.btn', function (e) {
        $('.btn').not(this).each(function () {
            $(this).fadeTo("fast", 0.60);
            if ($(this).is('button')) {
                $(this).attr("disabled", true);
            } else {
                $(this).removeAttr("href");
            }
        });
        $(this).text('AGUARDE...');
        $(this).fadeTo("fast", 0.50);
    });

})(jQuery); // End of use strict
