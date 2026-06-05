$.fn.swalConfirm = (title, text, callback) => {
    Swal.fire({
        title: title,
        text: text,
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Đồng ý',
        cancelButtonText: 'Đóng bỏ'
    }).then((result) => {
        callback(result);
    });
}

$.fn.swalInput = (title, text, callback) => {
    swal({
        title: title,
        text: text,
        type: 'input',
        showCancelButton: true,
        confirmButtonColor: '#33c105',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Đồng ý',
        cancelButtonText: 'Hủy bỏ'
    }).then(callback).catch(swal.noop)
}

$.fn.swalTextArea = (title, text, callback) => {
    swal({
        title: title,
        text: text,
        input: 'textarea',
        showCancelButton: true,
        confirmButtonColor: '#33c105',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Đồng ý',
        cancelButtonText: 'Đóng bỏ'
    }).then(callback).catch(swal.noop)
}

$.fn.swalMessage = (title, text, type, callback) => {
    Swal.fire(
        title,
        text,
        type
    );
}

$.fn.swalSuccess = (title, text, callback) => {
    Swal.fire(
        title,
        text,
        'success'
    );
}

$.fn.swalWarning = (title, text, callback) => {
    Swal.fire(
        title,
        text,
        'warning'
    );
}

$.fn.swalError = (title, text, callback) => {
    Swal.fire(
        title,
        text,
        'error'
    );
}