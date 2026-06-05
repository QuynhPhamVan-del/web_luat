document.addEventListener("DOMContentLoaded", function () {
    const track = document.querySelector(".mv-slider-track");
    const slides = document.querySelectorAll(".mv-slide");

    let currentIndex = 0;
    const slideInterval = 4000; // Thời gian chuyển slide (4000ms = 4 giây)

    function autoPlay() {
        // Tính toán slide tiếp theo, nếu là slide cuối thì tự quay về slide đầu tiên
        currentIndex = (currentIndex + 1) % slides.length;

        // Thực hiện lệnh dịch chuyển băng chuyền sang trái
        track.style.transform = `translateX(-${currentIndex * 100}%)`;
    }

    // Kích hoạt chu kỳ tự động chạy lướt liên tục
    setInterval(autoPlay, slideInterval);
});

document.addEventListener("DOMContentLoaded", function () {
    const track = document.querySelector(".mv-team-track");
    const slides = document.querySelectorAll(".mv-team-slide");
    const prevBtn = document.querySelector(".team-prev-btn");
    const nextBtn = document.querySelector(".team-next-btn");

    if (!track || slides.length === 0) return;

    let currentIndex = 0;

    // Hàm lấy số lượng slide đang hiển thị trên màn hình hiện tại
    function getVisibleSlidesCount() {
        if (window.innerWidth >= 992) return 3; // PC hiển thị 3
        if (window.innerWidth >= 768) return 2; // Tablet hiển thị 2
        return 1; // Mobile hiển thị 1
    }

    function updateSlider() {
        const visibleSlides = getVisibleSlidesCount();
        const maxIndex = slides.length - visibleSlides;

        // Giới hạn không cho bấm quá số lượng ảnh có thể dịch chuyển
        if (currentIndex > maxIndex) currentIndex = maxIndex;
        if (currentIndex < 0) currentIndex = 0;

        // Lấy độ rộng của 1 slide đơn lẻ + khoảng cách gap (24px)
        const slideWidth = slides[0].getBoundingClientRect().width;
        const gap = 24;

        // Tính tổng số pixel cần dịch chuyển qua trái
        const amountToMove = currentIndex * (slideWidth + gap);
        track.style.transform = `translateX(-${amountToMove}px)`;

        // Trạng thái bật/tắt độ mờ của nút điều hướng khi chạm giới hạn đầu/cuối
        prevBtn.style.opacity = currentIndex === 0 ? "0.4" : "1";
        nextBtn.style.opacity = currentIndex === maxIndex ? "0.4" : "1";
    }

    nextBtn.addEventListener("click", function () {
        const maxIndex = slides.length - getVisibleSlidesCount();
        if (currentIndex < maxIndex) {
            currentIndex++;
        } else {
            currentIndex = 0; // Nếu hết bộ thì xoay vòng về đầu tiên
        }
        updateSlider();
    });

    prevBtn.addEventListener("click", function () {
        const maxIndex = slides.length - getVisibleSlidesCount();
        if (currentIndex > 0) {
            currentIndex--;
        } else {
            currentIndex = maxIndex; // Nếu ở đầu tiên thì lướt nhanh đến cuối cùng
        }
        updateSlider();
    });

    // Lắng nghe sự kiện đổi kích thước màn hình để tính toán lại layout lập tức
    window.addEventListener("resize", updateSlider);

    // Chạy khởi tạo lần đầu tiên
    updateSlider();
});

/**
 * XỬ LÝ MENU ĐA CẤP (DROPDOWN MULTI-LEVEL)
 * Đảm bảo menu hoạt động mượt mà trên cả máy tính (hover) và điện thoại (click)
 */
document.addEventListener("DOMContentLoaded", function () {
    
    // Kiểm tra nếu là thiết bị di động / máy tính bảng
    if (window.innerWidth < 992) {
        
        // Tìm toàn bộ các nút kích hoạt sub-menu cấp 2
        document.querySelectorAll('.dropdown-submenu > a').forEach(function (element) {
            element.addEventListener('click', function (e) {
                // Ngăn chặn chuyển trang ngay lập tức để mở rộng menu
                e.preventDefault();
                e.stopPropagation();
                
                let nextEl = this.nextElementSibling;
                if (nextEl && nextEl.classList.contains('dropdown-menu')) {
                    // Đóng các sub-menu khác cùng cấp đang mở (nếu có)
                    let openedMenu = this.closest('.dropdown-menu').querySelectorAll('.sub-menu-level-2');
                    openedMenu.forEach(function(menu) {
                        if(menu !== nextEl) menu.style.display = 'none';
                    });

                    // Ẩn/hiện sub-menu được click
                    if (nextEl.style.display === 'block') {
                        nextEl.style.display = 'none';
                    } else {
                        nextEl.style.display = 'block';
                    }
                }
            });
        });
    }
    
    // Tự động đóng sub-menu khi menu cha chính bị đóng lại
    document.querySelectorAll('.custom-dropdown').forEach(function (dropDown) {
        dropDown.addEventListener('hidden.bs.dropdown', function () {
            this.querySelectorAll('.sub-menu-level-2').forEach(function (subMenu) {
                subMenu.style.display = 'none';
            });
        });
    });
});