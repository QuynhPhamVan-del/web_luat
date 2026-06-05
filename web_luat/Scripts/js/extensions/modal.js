$.fn.showModal = (config) => {
    let defaultConfig = {
        title: "",
        bodyContent: "",
        dialogClass: "",
        buttonSave: !0
    }
    defaultConfig = $.extend({}, defaultConfig, config);
    $("#modal .modal-body").html(defaultConfig.bodyContent);
    $("#modal .modal-title").html(defaultConfig.title);
    $("#modal .modal-dialog").attr("class", "modal-dialog").addClass(defaultConfig.dialogClass);
    defaultConfig.buttonSave ? $("#modal #btnSave").show() : $("#modal #btnSave").hide();
    $("#modal").modal('show');
}

$.fn.showSubModal = (config) => {
    let defaultConfig = {
        title: "",
        bodyContent: "",
        dialogClass: "",
        buttonSave: !0
    }
    defaultConfig = $.extend({}, defaultConfig, config);
    $("#sub-modal .modal-body").html(defaultConfig.bodyContent);
    $("#sub-modal .modal-title").html(defaultConfig.title);
    $("#sub-modal .modal-dialog").attr("class", "modal-dialog").addClass(defaultConfig.dialogClass);
    defaultConfig.buttonSave ? $("#sub-modal #btnSubSave").show() : $("#sub-modal #btnSubSave").hide();
    $("#modal").modal('hide');
    setTimeout(() => {
        $("#sub-modal").modal('show');
    }, 400);
}

$.fn.closeSubModal = () => {
    $('#sub-modal .modal-body').html("");
    $("#sub-modal").modal('hide');
    setTimeout(() => {
        $("#modal").modal('show');
    }, 400);
}

$.fn.closeModal = (elmModal) => {
    $(elmModal + ' .modal-body').html("");
    $(elmModal).modal('hide');
}

$.fn.reloadModal = (e) => {
    let target = $(e).closest(".modal");
    $(target).modal('hide');
    setTimeout(() => {
        $(target).modal('show');
    }, 500);
}