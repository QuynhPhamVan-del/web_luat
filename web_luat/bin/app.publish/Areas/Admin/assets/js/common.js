
function load() {
    $('#responsive').show();
    $('#responsive select').select2({
        width: '100%'
    });
}
function loadfrmEditSussess() {
    $('#responsive').show();
    $('#responsive select').select2({
        width: '100%'
    });
}
function doimatkhau(id) {
    $.ajax({
        url: "/Admin/QuanTri/DoiMK?Id=" + id, 
        type: "GET", 
        dataType: "html",
        success: function (response) {
            debugger;
            console.log(response);
            $('#responsive').html(response);
        }
    });
    $('#responsive').show();
}
function loadfrmEditTTSussess() {
    $('#responsive').show();
}
function closedialog(){
    $("#responsive").hide();
}
function onAddSuccess(res) {
    if (res.status == true) {
        $('p#notify').text(res.message);
        $('.message-box').css("background-color", "white");
        $('.message-model').show();
        setTimeout(() => {
            $('.message-model').hide();
            $("#responsive").hide();
            setTimeout(() => {
                location.reload();
            }, 1000);
        }, 1000);
       
    }
    else {
        $('p#notify').text(res.message);
        $('.message-box').css("background-color", "white");
        $('.message-model').show();
        setTimeout(() => {
            $('.message-model').hide();
        }, 1000);
    }
}
function don(res) {
    if (res.status) {
        if (res.message != undefined) {
            $('p#notify').text(res.message);
            $('.message-box').css("background-color", "white");
            $('.message-model').show();
            setTimeout(() => {
                $('.message-model').hide();
            }, 3000);
            $("#responsive").hide();
            location.reload();
        }
    } else {
        if (res.message != undefined) {
            $('p#notify').text(res.message);
            $('.message-box').css("background-color", "white");
            $('.message-model').show();
            setTimeout(() => {
                $('.message-model').hide(10000);
            }, 3000);
        }
    }
}
function onEditSuccess(res) {
    if (res.status == true) {
        $('p#notify').text(res.message);
        $('.message-box').css("background-color", "white");
        $('.message-model').show();
        setTimeout(() => {
            $('.message-model').hide();
            $("#responsive").hide();
            setTimeout(() => {
                location.reload();
            }, 1000);
          
        }, 1000);
      
    }
    else {
        $('p#notify').text(res.message);
        $('.message-box').css("background-color", "white");
        $('.message-model').show();
        setTimeout(() => {
            $('.message-model').hide();
        }, 1000);
    }

}
function btndelete(id,control) {

        $('.message-delete').show();    //Event cancel dialog
    $('.huy').click(function () {
        $('.message-delete').hide();
    })
    $('.xoa').on('click', function () {
        debugger;
        $.ajax({
            url: `/Admin/${control}/Delete?id=` + id,
        }).done(function (res) {
            debugger;
            $('.message-delete').hide();
            $('p#notify').text(res.message);
            $('.message-box').css("background-color", "#e5e5e5");
            $('.message-model').show();
            setTimeout(() => {
                $('.message-model').hide();
            }, 1000);
            setTimeout(() => {
         
                location.reload();
            }, 1000);
            
        }).fail(function (res) {
            $('.message-delete').hide();
            $('p#notify').text(res.message);
            $('.message-box').css("background-color", "#e5e5e5");
            $('.message-model').show();
            setTimeout(() => {
                $('.message-model').hide();
            }, 1500);
        })
    })
}
function btndeletetvc(id, control) {

    $('.message-delete').show();    //Event cancel dialog
    $('.huy').click(function () {
        $('.message-delete').hide();
    })
    $('.xoa').on('click', function () {
        debugger;
        $.ajax({
            url: `/Admin/${control}/DeleteTVC?id=` + id,
        }).done(function (res) {
            debugger;
            $('.message-delete').hide();
            $('p#notify').text(res.message);
            $('.message-box').css("background-color", "#e5e5e5");
            $('.message-model').show();
            setTimeout(() => {
                $('.message-model').hide();
            }, 1000);
            setTimeout(() => {

                location.reload();
            }, 1000);

        }).fail(function (res) {
            $('.message-delete').hide();
            $('p#notify').text(res.message);
            $('.message-box').css("background-color", "#e5e5e5");
            $('.message-model').show();
            setTimeout(() => {
                $('.message-model').hide();
            }, 1500);
        })
    })
}
function btnGiaoHang(id, control) {

    $('.message-giao').show();    //Event cancel dialog
    $('.huy').click(function () {
        $('.message-delete').hide();
    })
    $('.xoa').on('click', function () {
        debugger;
        $.ajax({
            url: `/Admin/${control}/GiaoHang?id=` + id,
        }).done(function (res) {
            debugger;
            $('.message-giao').hide();
            $('p#notify').text(res.message);
            $('.message-box').css("background-color", "#e5e5e5");
            $('.message-model').show();
            setTimeout(() => {
                $('.message-model').hide(10000);
            }, 3000);
            location.reload();
        }).fail(function (res) {
            $('.message-giao').hide();
            $('p#notify').text(res.message);
            $('.message-box').css("background-color", "#e5e5e5");
            $('.message-model').show();
            setTimeout(() => {
                $('.message-model').hide(10000);
            }, 3000);
        })
    })
}
function login(res) {
    if (res.status) {
        if (res.message != undefined) {
            $('p#notify').text(res.message);
            $('.message-box').css("background-color", "white");
            $('.message-model').show();
            $('.message-model').hide(10000);
            if (res.phanquyen == 1) {
                location.href = "/Admin/Homes/Index";
            }
            else {
                location.href = "/Home/Index";
            }
        }
    } else {
        if (res.message != undefined) {
            $('p#notify').text(res.message);
            $('.message-box').css("background-color", "white");
            $('.message-model').show();
            $('.message-model').hide(10000);
        }
    }
}
function doimk(res) {
    if (res.status) {
        if (res.message != undefined) {
            $('p#notify').text(res.message);
            $('.message-box').css("background-color", "white");
            $('.message-model').show();
            setTimeout(() => {
                $('.message-model').hide(10000);
            }, 3000);
                location.href = "/Home/Index";
        }
    } else {
        if (res.message != undefined) {
            $('p#notify').text(res.message);
            $('.message-box').css("background-color", "white");
            $('.message-model').show();
            setTimeout(() => {
                $('.message-model').hide(10000);
            }, 3000);
        }
    }
}
function register(res) {
    if (res.status) {
        if (res.message != undefined) {
            $('p#notify').text(res.message);
            $('.message-box').css("background-color", "white");
            $('.message-model').show();
            $('.message-model').hide(10000);
            location.href = "/Home/Index";

        }
    } else {
        if (res.message != undefined) {
            $('p#notify').text(res.message);
            $('.message-box').css("background-color", "white");
            $('.message-model').show();
            $('.message-model').hide(10000);
        }
    }
}
function thongtin(res) {
    if (res.status) {
        if (res.message != undefined) {
            $('p#notify').text(res.message);
            $('.message-box').css("background-color", "white");
            $('.message-model').show();
            $('.message-model').hide(10000);
            location.href = "/Home/Index";
        }
    } else {
        if (res.message != undefined) {
            $('p#notify').text(res.message);
            $('.message-box').css("background-color", "white");
            $('.message-model').show();
            $('.message-model').hide(10000);
        }
    }
}
function xoagh(id, idbnt, idmau) {
    $.ajax({
        url: '/Cart/Xoa?Id=' + id + '&&IdBNT=' + idbnt + '&&IdMau=' + idmau,
    }).done(function (res) {
        debugger;
        $('.message-delete').hide();
        $('p#notify').text(res.message);
        $('.message-box').css("background-color", "#e5e5e5");
        $('.message-model').show();
        $('.message-model').hide(10000);
        location.reload();
    }).fail(function (res) {
        $('.message-delete').hide();
        $('p#notify').text(res.message);
        $('.message-box').css("background-color", "#e5e5e5");
        $('.message-model').show();
        $('.message-model').hide(10000);
    })
}
function dathang(res) {
    if (res.status) {
        if (res.message != undefined) {
            $('p#notify').text(res.message);
            $('.message-box').css("background-color", "white");
            $('.message-model').show();
            $('.message-model').hide(10000);
                location.href = "/Home/Index";
        }
    } else {
        if (res.message != undefined) {
            $('p#notify').text(res.message);
            $('.message-box').css("background-color", "white");
            $('.message-model').show();
            $('.message-model').hide(10000);
        }
    }
}
// ================= CHECKBOX =================
$('#checkAll').on('change', function () {
    $('.chkItem').prop('checked', $(this).prop('checked'));
});

function getSelectedIds() {
    let ids = [];
    $('.chkItem:checked').each(function () {
        ids.push($(this).val());
    });
    return ids;
}

// ================= CORE AJAX =================
function callBulkAction(url, ids) {
    $.ajax({
        url: url,
        type: "POST",
        traditional: true,
        data: { ids: ids }
    }).done(function (res) {

        $('.message-delete').hide();

        $('p#notify').text(res.message);
        $('.message-box').css("background-color", "#e5e5e5");
        $('.message-model').show();

        setTimeout(() => {
            $('.message-model').hide();
        }, 1000);

        if (res.status) {
            setTimeout(() => location.reload(), 1000);
        }

    }).fail(function () {
        $('.message-delete').hide();

        $('p#notify').text("Có lỗi xảy ra");
        $('.message-model').show();

        setTimeout(() => {
            $('.message-model').hide();
        }, 1500);
    });
}


function btnRestoreSingle(id, control) {

    $('.message-giao').show();    //Event cancel dialog
    $('.huy').click(function () {
        $('.message-giao').hide();
    })
    $('.xoa').on('click', function () {
        debugger;
        $.ajax({
            url: `/Admin/${control}/btnRestoreSingle?id=` + id,
        }).done(function (res) {
            debugger;
            $('.message-giao').hide();
            $('p#notify').text(res.message);
            $('.message-box').css("background-color", "#e5e5e5");
            $('.message-model').show();
            setTimeout(() => {
                $('.message-model').hide();
            }, 1000);
            setTimeout(() => {

                location.reload();
            }, 1000);

        }).fail(function (res) {
            $('.message-giao').hide();
            $('p#notify').text(res.message);
            $('.message-box').css("background-color", "#e5e5e5");
            $('.message-model').show();
            setTimeout(() => {
                $('.message-model').hide();
            }, 1500);
        })
    })
}
