$.fn.allInputMask = () => {
    $.fn.dateInputMask();
    $.fn.yearInputMask();
}

$.fn.dateInputMask = () => {
    $(".date-inputmask").inputmask('dd/mm/yyyy', { 'placeholder': '__/__/____' });
}

$.fn.yearInputMask = () => {
    $(".year-inputmask").inputmask('9999', { 'placeholder': '____' });
}