// sticky blur khi scroll
window.addEventListener("scroll", function () {
    const menu = document.querySelector(".wrp");

    if (window.scrollY > 40) {
        menu.classList.add("scrolled");
    } else {
        menu.classList.remove("scrolled");
    }
});

// load menu ajax
function loadMenu() {
    fetch('/Home/Menu')
        .then(res => res.text())
        .then(html => {
            document.getElementById("menuMain").innerHTML = html;
        });
}

// gọi khi load page
document.addEventListener("DOMContentLoaded", function () {
    loadMenu();
});