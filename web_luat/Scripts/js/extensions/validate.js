$.fn.renderRequired = (rules) => {
    if (rules) {
        for (var property in rules) {
            if (rules[property].required !== undefined && rules[property].required) {
                $("label[for='" + property + "'] span").html("*");
            }
            else {
                $("label[for='" + property + "'] span").html("");
            }
        }
    }
}

$.fn.validateForm = (elm, obj) => {
    let config = {
        errorClass: "invalid-feedback animated fadeInUp",
        errorElement: "span",
        errorPlacement: function (e, a) {
            jQuery(a).closest("div").append(e);
        },
        highlight: function (e) {
            jQuery(e).closest("div").removeClass("is-invalid").addClass("is-invalid");
        },
        success: function (e) {
            jQuery(e).closest("div").removeClass("is-invalid"), jQuery(e).remove();
        },
    }
    if (obj) config = $.extend({}, config, obj);
    $(elm).validate(config);
}

//#region rules
$.validator.methods.number = function (value, element) {
    return this.optional(element) || /(^[0-9]+$)|^([0-9]+([.,][0-9]+)?)$/.test(value);
}

$.validator.methods.greaterThan0 = function (value, element) {
    return value > 0;
}
//#endregion rules