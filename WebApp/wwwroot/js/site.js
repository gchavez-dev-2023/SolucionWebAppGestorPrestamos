// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

//Funcion para activar el loader cuando esta ejecutando funcion ajax
$(function () {
    $("#loaderbody").addClass('hide');

    $(document).bind('ajaxStart', function () {
        $("#loaderbody").removeClass('hide');
    }).bind('ajaxStop', function () {
        $("#loaderbody").addClass('hide');
    });
});

//Funcion para realizar el llamado a la vista que se va a mostrar en el POPUP
showInPopup = (url, title) => {
    $.ajax({
        type: 'GET',
        url: url,
        success: function (res) {
            $('#form-modal .modal-body').html(res);
            $('#form-modal .modal-title').html(title);
            $('#form-modal').modal('show');
            // to make popup draggable
            //$('.modal-dialog').draggable({
            //    handle: ".modal-header"
            //});
        }
    })
}

//Funcion para realizar CREATEorUPDATE desde vista POPUP y volver al index
jQueryAjaxPost = form => {
    try {
        $.ajax({
            type: 'POST',
            url: form.action,
            data: new FormData(form),
            contentType: false,
            processData: false,
            success: function (res) {
                if (res.esValido) {
                    $('#view-all').html(res.html);
                    $('#form-modal .modal-body').html('');
                    $('#form-modal .modal-title').html('');
                    $('#form-modal').modal('hide');
                    $.notify('Actualizado Correctamente', { globalPosition: 'top center', className: 'success' });
                } else {
                    $('#form-modal .modal-body').html(res.html);
                }
            },
            error: function (err) {
                console.log(err);
            }
        });
    } catch (ex) {
        console.log(ex);
    }
    //to prevent default form submit event
    return false;
}

//Funcion para realizar DELET desde el index sin mostrar el registro
jQueryAjaxDelete = form => {
    if (confirm('¿Esta seguro que desea eliminar?')) {
        try {
            $.ajax({
                type: 'POST',
                url: form.action,
                data: new FormData(form),
                contentType: false,
                processData: false,
                success: function (res) {
                    $('#view-all').html(res.html);
                    $.notify('Borrado Correctamente', { globalPosition: 'top center', className: 'success' });
                },
                error: function (err) {
                    console.log(err);
                }
            });
        } catch (ex) {
            console.log(ex);
        }
    }

    //prevent default form submit event
    return false;
}