$(function () {
    var tabContainer = $("#page_tab_nav");
    var tabHeight = tabContainer.innerHeight();
    var lowerContainer = $("#main-page-content .container");
    var originalTabWidth = tabContainer.width();

    if (originalTabWidth > lowerContainer.width()) {
        $("#page_tab_nav li").not(".current").css("display", "none");
    }
    else {
        $("#page_tab_nav li").not(".current").css("display", "inline-block");
    }

    $(window).resize(function () {
        if (originalTabWidth > lowerContainer.width()) {
            //$("#page_tab_nav li").not(".current").css("display", "none");
            $("#page_tab_nav li").not(".current").fadeOut();
        }
        else {
            //$("#page_tab_nav li").not(".current").css("display", "inline-block");
            $("#page_tab_nav li").not(".current").fadeIn();
        }

        if ($(window).width() <= 767)
            $("#float_menu").hide();

    });

    showSubMenu();

});

function showSubMenu() {
    $(".navbar-toggle").on("click", function () {
        $("#float_menu").toggle("slow");
    });
}