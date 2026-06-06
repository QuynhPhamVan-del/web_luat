$(function () {
    let path = location.pathname;
    let elm = $("a[href~='" + path + "']");
    elm.closest('li').addClass('active');
    elm.closest('.nav-item').addClass('open');
})