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

    $(document).on('click', 'a.btn', function (e) {
        if ($(this).hasClass("list-delete") == false) {
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
        }
    });

    $(document).on('click', 'button.btn', function (e) {
        var invalids = $("input:invalid").length;
        invalids = invalids + $("select:invalid").length;
        if (invalids == 0) {
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
        }
    });

    $(document).on('click', '.list-delete', function (e) {
        var name = $(this).attr('data-item-name');
        $("#modal-item").text(name);
        var url = $(this).attr('data-item-url');
        $("#modal-url").attr('href', url);
    });

    $(document).ready( function() {
        $(".cpf").inputmask("mask", {
            "mask": "999.999.999-99"
        }, {
                reverse: true
            });

        $(".birthday").inputmask("mask", {
            "mask": "99/99/9999"
        }, {
                reverse: true
            });

        if ($(".birthday").val() == "01/01/0001") {
            $(".birthday").val("");
        }

        if ($(".price").val() == "0") {
            $(".price").val(null);
        }

        $(".price").inputmask('decimal', {
            'alias': 'numeric',
            'groupSeparator': '.',
            'autoGroup': true,
            'digits': 2,
            'radixPoint': ",",
            'digitsOptional': false,
            'allowMinus': false,
            'prefix': '',
            'rightAlign': false
        });
    });
})(jQuery); // End of use strict
