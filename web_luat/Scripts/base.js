// loading
function loading() {
    document.getElementById("loading").classList.add("active");
}

function offLoading() {
    document.getElementById("loading").classList.remove("active");
}

// modal
function showModal() {
    $("#nv-modal").removeClass("xl")
    $("#nv-modal").removeClass("lg")
    $("#nv-modal").fadeIn();
}

function closeModal() {
    $("#nv-modal").fadeOut();
}
$(".nv-modal-close, .btn-close-modal").on("click", function () {
    $(this).closest('.nv-modal').hide();
})


$(function () {
    //set width select 2
    $("span.select2-container").css("width", "100%")
})