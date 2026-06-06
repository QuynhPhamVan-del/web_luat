var DataTables = {
    //thiết lập ngôn ngữ
    language: {
        decimal: "",
        emptyTable: "Không có dữ liệu hiển thị",
        info: "Hiển thị _START_ đến _END_ trong _TOTAL_ bản ghi",
        infoEmpty: "Hiển thị 0 đến 0 trong 0 bản ghi",
        infoFiltered: "(Lọc từ _MAX_ tổng số bản ghi)",
        infoPostFix: "",
        thousands: ",",
        lengthMenu: "Hiển thị _MENU_ bản ghi",
        loadingRecords: "Đang tải dữ liệu...",
        processing: "Đang tải...",
        search: "Tìm kiếm:",
        zeroRecords: "Không tìm thấy dữ liệu phù hợp",
        paginate: {
            first: "Trang đầu",
            last: "Trang cuối",
            next: `<i class="fa fa-angle-double-right" aria-hidden="true"></i>`,
            previous: `<i class="fa fa-angle-double-left" aria-hidden="true"></i>`
        },
        aria: {
            sortAscending: ": sắp xếp theo thứ tự tăng",
            sortDescending: ": sắp xếp theo thứ tự giảm"
        },
        select: {
            rows: {
                _: "%d bản ghi đã chọn",
                0: ""
            }
        }
    }
}

$.fn.initDataTable = (elm, exSetting) => {
    let defaultSetting = {
        language: DataTables.language,
        paging: true,
        lengthChange: false,
        searching: false,
        ordering: true,
        info: true,
        autoWidth: false,
        responsive: false,
        select: true,
        pageLength: 10
    };
    let settings = $.extend({}, defaultSetting, exSetting);
    return $(elm).DataTable(settings);
}

$.fn.dataTable.Api.prototype.destroyTable = (elm) => {
    if ($.fn.dataTable.isDataTable(elm)) {
        $(elm).destroy();
        $(elm + ' tbody').empty();
    }
}

//get data selected items
$.fn.dataTable.Api.prototype.selectedItems = function () {
    let selectedRows = this.rows(".selected");
    let selectedItems = selectedRows.data();
    return selectedItems;
}

//get data selected ids
$.fn.dataTable.Api.prototype.selectedIds = function (fieldname) {
    let selectedItems = [];
    let dataItems = this.rows(".selected").data();
    if (fieldname == null) fieldname = "Id";
    if (fieldname) {
        for (var i = 0; i < dataItems.length; i++) {
            selectedItems.push(dataItems[i][fieldname]);
        }
    }
    else {
        for (var i = 0; i < dataItems.length; i++) {
            selectedItems.push(dataItems[i]);
        }
    };
    return selectedItems;
}

//get data selected ids
$.fn.dataTable.Api.prototype.ordinalNumbers = function () {
    this.on('order.dt search.dt', function () {
        t.column(0, { search: 'applied', order: 'applied' }).nodes().each(function (cell, i) {
            cell.innerHTML = i + 1;
        });
    }).draw();
}