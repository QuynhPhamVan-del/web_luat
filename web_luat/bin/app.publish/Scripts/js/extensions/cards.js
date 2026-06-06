$.fn.collapseAction = () => {
    $('a[data-action="collapse"]').off().on("click", function (e) {
        e.preventDefault();
        $(this).closest(".card").children(".card-content").collapse("toggle");
        $(this).closest(".card").find('[data-action="collapse"] i').toggleClass("ft-plus ft-minus");
    })
}

$.fn.closeAction = () => {
    $('a[data-action="close"]').off().on("click", function (e) {
        e.preventDefault();
        $(this).closest(".card").html("");
    })
}

$.fn.expandAction = () => {
    $('a[data-action="expand"]').off().on("click", function (e) {
        e.preventDefault();
        $(this).closest(".card").find('[data-action="expand"] i').toggleClass("ft-maximize ft-minimize");
        $(this).closest(".card").toggleClass("card-fullscreen");
    })
}

$.fn.reloadAction = () => {
    $('a[data-action="reload"]').on("click", function () {
        $(this).closest(".card").block({ message: '<div class="ft-refresh-cw icon-spin font-medium-2"></div>', timeout: 2e3, overlayCSS: { backgroundColor: "#FFF", cursor: "wait" }, css: { border: 0, padding: 0, backgroundColor: "none" } })
    })
}

$.fn.initAllAction = () => {
    $.fn.collapseAction();
    $.fn.closeAction();
    $.fn.expandAction();
    $.fn.reloadAction();
}