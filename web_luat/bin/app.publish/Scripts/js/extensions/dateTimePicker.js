var dpLocale = {
    "format": "DD/MM/YYYY",
    "separator": " - ",
    "applyLabel": "Áp dụng",
    "cancelLabel": "Đóng",
    "fromLabel": "Từ",
    "toLabel": "Đến",
    "customRangeLabel": "Tùy chỉnh",
    "weekLabel": "Tuần",
    "daysOfWeek": [
        "CN",
        "T2",
        "T3",
        "T4",
        "T5",
        "T6",
        "T7"
    ],
    "monthNames": [
        "Tháng 1",
        "Tháng 2",
        "Tháng 3",
        "Tháng 4",
        "Tháng 5",
        "Tháng 6",
        "Tháng 7",
        "Tháng 8",
        "Tháng 9",
        "Tháng 10",
        "Tháng 11",
        "Tháng 12"
    ],
    "firstDay": 1
}

//Chọn ngày tháng năm
$.fn.singleDatePicker = (elm, setting, callback) => {
    let config = {
        minDate: "01-01-1900",
        icons: {
            time: 'far fa-clock',
            date: 'far fa-calendar-alt',
            today: 'fas fa-crosshairs'
        },
        locale: moment.locale('vi'),
        showTodayButton: true,
        keepOpen: true,
        useCurrent: false,
        format: 'DD-MM-YYYY',
    }
    if (setting) config = $.extend({}, config, setting);
    $(elm).datetimepicker(config, callback);
}

//Chọn năm
$.fn.yearOnly = (elm, setting, callback) => {
    let config = {
        minDate: "01/01/1900",
        format: 'YYYY',
        useCurrent: false,
        showClear: true,
        icons: {
            clear: 'far fa-calendar-times'
        },
    }
    if(setting) config = $.extend({}, config, setting);
    $(elm).datetimepicker(config);
}

//Chọn thời gian (định dạng 24h)
$.fn.timeOnly = (elm, setting, callback) => {
    let config = {
        format: 'HH:mm'
    }
    if(setting) config = $.extend({}, config, setting);
    $(elm).datetimepicker(config);
}