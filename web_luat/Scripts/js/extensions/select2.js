//select2 init
$.fn.initSelect2 = (elm, obj) => {
    let config = {
        placeholder: {
            id: '', // the value of the option
            text: obj.placeholder != null ? obj.placeholder : "--Lựa chọn--"
        },
        width: "100%",
        dropdownAutoWidth: false,
        allowClear: true
    };
    if (obj) config = $.extend({}, config, obj);
    elm = elm != null && elm != '' ? elm : ".select2";
    $(elm).select2(config);
}

//load single select
$.fn.singleSelect = (elm, url, params, callback) => {
    $.fn.postData(url, params, res => {
        if (res.type == "success") {
            $(elm).select2({
                data: res.data,
                placeholder: {
                    id: '', // the value of the option
                    text: params.placeholder != null ? params.placeholder : "--Lựa chọn--",
                },
                width: "100%",
                allowClear: true
            })
            //callback
            if (callback)
                callback(res);
        }
    })
}

//load single select paging
$.fn.singleSelectPaging = (elm, url, obj, callback) => {
    $(elm).select2({
        placeholder: {
            id: '', // the value of the option
            text: obj.placeholder != null ? obj.placeholder : "--Lựa chọn--",
        },
        allowClear: true,
        width: "100%",
        ajax: {
            url: url,
            type: 'POST',
            data: function (params) {
                let data = {
                    page: params.page || 1,
                    length: params.length || 20,
                    Keywords: params.term || "",
                    start: (params.page ? (params.page - 1) : 0) * (params.length ? params.length : 20)
                };
                data = $.extend({}, data, obj);
                return data;
            },
            processResults: function (data, params) {
                params.page = params.page || 1;
                params.length = params.length || 20;
                params.term = params.term || "";
                if(callback) callback(data);
                return {
                    results: data.data,
                    pagination: {
                        more: (params.page * params.length) < data.recordsTotal
                    }
                };
            },
            cache: true
        }
    })
}
//hủy select2
$.fn.destroySelect2 = (elm) => {
    if ($(elm).hasClass("select2-hidden-accessible")) {
        $(elm).select2('destroy');
        $(elm).html('');
    }
}