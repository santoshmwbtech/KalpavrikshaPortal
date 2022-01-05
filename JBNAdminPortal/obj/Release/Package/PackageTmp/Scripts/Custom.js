// number validation
function isNumber(evt) {
    evt = (evt) ? evt : window.event;
    var charCode = (evt.which) ? evt.which : evt.keyCode;
    if (charCode > 31 && (charCode < 48 || charCode > 57)) {
        return false;
    }
    return true;
}
//character validation
function isCharacter(evt) {
    evt = (evt) ? evt : window.event;
    var charCode = (evt.which) ? evt.which : evt.keyCode;
    if (charCode >= 48 && charCode <= 57) {
        return false;
    }
    return true;
}
//decimal validation
function isdecimalKey(evt) {
    var charCode = (evt.which) ? evt.which : event.keyCode;
    if (charCode == 46 && evt.srcElement.value.split('.').length > 1) {
        return false;
    }
    if (charCode != 46 && charCode > 31 && (charCode < 48 || charCode > 57))
        return false;
    return true;
}

//date validation dd/MM/yyyy
function validate_date(element) {
    //var invdate = $(element).val();
    //if (invdate != "") {
    //    var id = $(element).attr('id');
    //    if (!invdate.match(/^(0[1-9]|[12][0-9]|3[01])[\- \/.](?:(0[1-9]|1[012])[\- \/.](19|20)[0-9]{2})$/)) {
    //        CreateNotification("error", "Invalid Date Format", "Date format should be DD/MM/YYYY")
    //        //swal('Date format should be DD/MM/YYYY');
    //        document.getElementById(id).value = "";
    //        document.getElementById(id).focus();
    //        return false;
    //    }
    //}
}

$(document).ready(function () {
    $('.dated .dates').change(function () {
        var validDate = $(this).val().match(/^(0[1-9]|[12][0-9]|3[01])[\- \/.](?:(0[1-9]|1[012])[\- \/.](19|20)[0-9]{2})$/);
        if (!validDate) {
            CreateNotification("error", "Invalid Date Format", "Date format should be DD/MM/YYYY")
            $(this).val('').focus();
        } else {
            $(this).css('background', 'transparent');
        }
    });
});

//validate time
$(document).ready(function () {
    $('.timepicker').change(function () {
        var validTime = $(this).val().match(/^(0?[1-9]|1[012])(:[0-5]\d) [APap][mM]$/);
        if (!validTime) {
            CreateNotification("error", "Invalid Date Format", "Time format should be HH:MM AM/PM");
            $(this).val('').focus();
        } else {
            $(this).css('background', 'transparent');
        }
    });
});


//duplicate validation for two values

function duplicate_validation2(element, id, controller) {

    var name = $(element).val();
    var id = document.getElementById(id).value;
    if (name != "") {
        $.ajax({
            type: 'POST',
            url: '@Url.Action("duplicate_validate","' + controller + '")',
            dataType: "json",
            contentType: "application/json",
            data: JSON.stringify({ name: name, id: id }),
            success: function (data) {
                if (data == true) {
                    alert("Record already exists");
                    document.getElementById(id).value = "";
                    $(element).val('');
                }
            }
        });
    }
}

//gst validation
function gst_pattern() {
    gst_no = document.getElementById("gstin").value;
    var regex = /^[0-9]{2}[A-Z]{5}[0-9]{4}[A-Z]{1}[1-9A-Z]{1}Z[0-9A-Z]{1}$/;
    if (!regex.test(gst_no)) {
        document.getElementById("gstin").value = '';
        alert("Invalid Gst No");
    }
}

//state and gst validation

//auto complete

function autofill() {
    $('.clsmodelno').each(function () {
        $(this).autocomplete({
            source: function (request, response) {
                $.ajax({
                    url: '@Url.Action("AutoComplete", "LR")',
                    data: "{ 'prefix': '" + request.term + "'}",
                    dataType: "json",
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    success: function (data) {
                        //$("#display").html(html).show();

                        response($.map(data, function (item) {
                            return item;
                        }))
                    },
                    error: function (response) {
                        alert(response.responseText);
                    },
                    failure: function (response) {
                        alert(response.responseText);
                    }
                });
            },

            minLength: 1
        });
    });
}

//validate message success
function ValidateDelete() {

    return confirm("Are you sure?");

}


//delete message success
function deletesuccess(data) {
    alert("Record deleted successfully");
    $.validator.unobtrusive.parse(document);
    $("#tbldynamic").DataTable({
        dom: 'Bfrtip',
        buttons: [
            'copy', 'csv', 'excel', 'pdf', 'print'
        ],

    });
}

function CreateNotification(theme, title, message) {
    window.createNotification({
        closeOnClick: true,
        displayCloseButton: true,
        positionClass: "nfc-top-right",
        showDuration: "6000",
        theme: theme
    })({
        title: title,
        message: message
    });
}
$(function () {
    setTimeout(function () {
        $('.loader_bg').fadeToggle();
    }, 1500);
});

function loadDataTable() {
    $('.dataGrid').dataTable({
        dom: 'Bfrtip',
        buttons: [

        ]
    });
}
function loadDataTableWithButtons() {
    $('.dataGrid').dataTable({
        dom: 'Bfrtip',
        //buttons: [
        //    'csv', 'excel', 'pdf'
        //]
        buttons: [{
            extend: 'excel',
            exportOptions: {
                columns: "thead th:not(.noExport)"
            }
        }, {
            extend: 'csv',
            exportOptions: {
                columns: "thead th:not(.noExport)"
            }
        }, {
            extend: 'pdfHtml5',
            orientation: 'landscape',
            pageSize: 'LEGAL',
            exportOptions: {
                columns: "thead th:not(.noExport)"
            }
        }
        ]
    });
}