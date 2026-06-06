//get request
$.fn.getData = (url, data, onSuccess, onError) => {
    $.ajax({
        url: url,
        type: 'GET',
        data: data,
        success: function (response, status, xhr) {
            if (onSuccess) onSuccess(response);
        },
        error: function (xhr, status, error) {
            if (onError) onError(error);
            offLoading();
        },
    });
};

//post request
$.fn.postData = (url, data, onSuccess, onError) => {
    $.ajax({
        url: url,
        type: 'POST',
        data: data,
        success: function (response, status, xhr) {
            if (onSuccess) onSuccess(response);
        },
        error: function (xhr, status, error) {
            if (onError) onError(error);
        },
    });
};

//post request with FormData
$.fn.postFormData = (url, formData, onSuccess, onError) => {
    $.ajax({
        url: url,
        type: 'POST',
        processData: false,
        contentType: false,
        data: formData,
        success: function (response, status, xhr) {
            if (onSuccess) onSuccess(response);
        },
        error: function (xhr, status, error) {
            if (onError) onError(error);
            offLoading();
        },
    });
};

//post request with JSON data
$.fn.postJsonData = (url, data, onSuccess, onError) => {
    $.ajax({
        url: url,
        type: 'POST',
        contentType: 'application/json',
        dataType: 'json',
        data: JSON.stringify(data),
        success: function (response, status, xhr) {
            if (onSuccess) onSuccess(response);
        },
        error: function (xhr, status, error) {
            if (onError) onError(error);
            offLoading();
        },
    });
};

$.fn.serializeObject = function (obj) {
    var o = {};
    // Find disabled, and remove the "disabled" attribute
    var disabled = obj.find('select:disabled').removeAttr('disabled');
    // serialize the form
    var a = obj.serializeArray();
    // re-disabled the set of inputs that you previously enabled
    disabled.attr('disabled', 'disabled');
    $.each(a, function () {
        if (o[this.name] !== undefined) {
            if (!o[this.name].push) {
                o[this.name] = [o[this.name]];
            }
            o[this.name].push(this.value || '');
        } else {
            o[this.name] = this.value || '';
        }
    });
    return o;
};

//start upload files | arr: danh sách file upload, type: tên thư mục lưu, callback 
$.fn.startUploadFiles = (arr, type, callback) => {
    let formData = new FormData();
    formData.append("type", type);
    if (arr.length > 0) {
        arr.map(item => {
            if (item.value.length > 0) {
                item.value.map(e => {
                    formData.append(item.key, e);
                })
            }
        })
    }
    $.fn.postFormData("/FileManage/UploadFiles", formData, res => {
        if (callback) callback(res);
    })
}

$.fn.generateID = () => {
    // Math.random should be unique because of its seeding algorithm.
    // Convert it to base 36 (numbers + letters), and grab the first 16 characters
    // after the decimal.
    return Math.random().toString(36).substr(2, 16);
}

//check null, undefined, string empty
$.fn.isEmpty = (val) => {
    let check = null;
    if (val) check = !1;
    else check = !0;
    return check;
}

//convert date
$.fn.convertDate = (dateString, type) => {
    let str = "";
    if (dateString != null && dateString != "") {
        switch (type) {
            //date format: mm/dd/yyyy => dd/mm/yyyy
            case 1:
                str = moment(dateString).format("DD/MM/YYYY");
                break;
            default:
                break;
        }
    }
    return str;
}

//format currency
$.fn.formatCurrency = (n, currency) => {
    let str = "";
    currency = currency != null ? currency : "";
    if (n != null) {
        str += currency + n.toString().replace(/\D/g, "").replace(/\B(?=(\d{3})+(?!\d))/g, ",");
    }
    return str;
}

//download file
$.fn.downloadFile = (filePath) => {
    if (filePath != null && filePath != "")
        window.open(`${ACT_FILEMANAGE_DOWNLOAD}${filePath}`);
    else {
        ToastMessage("Không tìm thấy file", "Thông báo", "error");
    }
}

//export file
$.fn.exportFile = (url, params) => {
     loading();
    $.ajax({
        type: "POST",
        url: url,
        data: params,
        xhrFields: {
            responseType: 'blob' // to avoid binary data being mangled on charset conversion
        },
        success: function (blob, status, xhr) {
            offLoading();
            let filename = '';
            var disposition = xhr.getResponseHeader('Content-Disposition');
            if (disposition && disposition.indexOf('attachment') !== -1) {
                var filenameRegex = /filename[^;=\n]*=((['"]).*?\2|[^;\n]*)/;
                var matches = filenameRegex.exec(disposition);
                if (matches != null && matches[1]) filename = matches[1].replace(/['"]/g, '');
            }
            var a = document.createElement('a');
            a.href = window.URL.createObjectURL(blob);
            a.download = decodeURIComponent(filename);
            a.dispatchEvent(new MouseEvent('click'));
        },
        error: function (res) {
            offLoading();
            ToastMessage("Không thể xuất file", "Thông báo", "error");
        }
    });
}

//preview file
$.fn.previewFile = (elm, url, params, callback) => {
     loading();
    $.ajax({
        type: "POST",
        url: url,
        data: params,
        xhrFields: {
            responseType: 'blob' // to avoid binary data being mangled on charset conversion
        },
        success: function (blob, status, xhr) {
            offLoading();
            let filename = '';
            var disposition = xhr.getResponseHeader('Content-Disposition');
            if (disposition && disposition.indexOf('attachment') !== -1) {
                var filenameRegex = /filename[^;=\n]*=((['"]).*?\2|[^;\n]*)/;
                var matches = filenameRegex.exec(disposition);
                if (matches != null && matches[1]) filename = matches[1].replace(/['"]/g, '');
            }
            if (window.navigator && window.navigator.msSaveOrOpenBlob) {
                window.navigator.msSaveOrOpenBlob(blob, filename);
            }
            if (typeof blob === 'object') {
                let url = window.URL.createObjectURL(blob);
                $(elm).attr("src", url);
                URL.revokeObjectURL(url);
            }
            else {
                $(elm).attr("src", "/file-not-found");
            }
        },
        error: function (res) {
            offLoading();
            ToastMessage("Không thể xuất file", "Thông báo", "error");
        }
    });
}

//fullscreen DOM
$.fn.openFullscreenDOM = (elm) => {
    let element = $(elm).get(0);
    if (element.requestFullscreen) {
        element.requestFullscreen();
    } else if (element.webkitRequestFullscreen) {
        /* Safari */
        element.webkitRequestFullscreen();
    } else if (element.msRequestFullscreen) {
        /* IE11 */
        element.msRequestFullscreen();
    }
}