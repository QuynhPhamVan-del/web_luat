function ToastMessage(title, message, type) {
    toast({
        title: title,
        message: message,
        type: type,
        duration: 5000
    });
}

function Message(title,message,icon) {
    Swal.fire({
        title: "",
        text: message,
        icon: icon,
        showConfirmButton: false,
        timer: 1500
    })
}

