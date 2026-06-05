var toastrConfig = {
    timeOut: 3000,
    closeButton: !0,
    debug: !1,
    newestOnTop: !0,
    progressBar: !0,
    positionClass: "toast-top-right",
    preventDuplicates: !1,
    onclick: null,
    showDuration: "300",
    hideDuration: "1000",
    extendedTimeOut: "1000",
    showEasing: "swing",
    hideEasing: "linear",
    showMethod: "fadeIn",
    hideMethod: "fadeOut",
    tapToDismiss: !1
}

$.fn.toastrMessage = (message, title, type) => {
    switch (type) {
        case "success":
            toastr.success(message, title, toastrConfig);
            break;
        case "warning":
            toastr.warning(message, title, toastrConfig);
            break;
        case "error":
            toastr.error(message, title, toastrConfig);
            break;
    }
}