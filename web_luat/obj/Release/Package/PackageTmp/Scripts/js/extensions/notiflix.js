$(function () {
    Notiflix.Loading.Init({
        svgSize: '80px',
        svgColor: '#007EFF',
    });

    Notiflix.Confirm.Init({
        titleColor: '#007EFF',
    })
})

$.fn.loading = () => {
    Notiflix.Loading.Dots();
}

$.fn.offLoading = () => {
    Notiflix.Loading.Remove();
}

$.fn.notiflixConfirm = (msg, callback) => {
    Notiflix.Confirm.Show(
        'Xác nhận',
        msg,
        'Đồng ý',
        'Hủy bỏ',
        () => { if(callback) callback(true) },
        () => { if(callback) callback(false) }
    );
}