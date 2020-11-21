var isValidExcel = false;
$(document).ready(function () {
    $.noConflict();
    // $("#myTable").DataTable();
  
});
function deleteUser(id) {

    //$('#deleteModal').data('id', id).modal('show');
    //$('#deleteModal').modal('show');
    Confirm('Are you sure?', 'You will not be able to recover this', 'Yes', 'Cancel', id); /*change*/

}


function Confirm(title, msg, $true, $false, $link) { /*change*/
    var $content = "<div class='dialog-ovelay'>" +
        "<div class='dialog'><header>" +
        " <h3> " + title + " </h3> " +
        "<i class='fa fa-close'></i>" +
        "</header>" +
        "<div class='dialog-msg'>" +
        " <p> " + msg + " </p> " +
        "</div>" +
        "<footer>" +
        "<div class='controls' style='margin-left: 235px;'>" +
        " <button class='button button-danger doAction'>" + $true + "</button> " +
        " <button class='button button-default cancelAction'>" + $false + "</button> " +
        "</div>" +
        "</footer>" +
        "</div>" +
        "</div>";
    $('body').prepend($content);
    $('.doAction').click(function () {
        deleteAction($link);
        $(this).parents('.dialog-ovelay').fadeOut(500, function () {
            $(this).remove();
        });
    });
    $('.cancelAction, .fa-close').click(function () {
        $(this).parents('.dialog-ovelay').fadeOut(500, function () {
            $(this).remove();
        });
    });

}

function deleteAction(id) {
    StartProcess();
    $.ajax({
        type: "POST",
        url: "/UserMaster/Delete",
        data: { id: id },
        success: function (response) {
            StopProcess()
            if (response.status != "Fail") {
                location.reload();
            }
            else {
                $('#deleteModal').modal('hide');
                funToastr(false, response.error);
            }
        },
        error: function (error) {
            toastr.error(error)
        }
    });
};

function performAction(id, isView) {
    let url = `/UserMaster/Edit?id=${id}&isView=${isView}`
    location.href = url;
}
function filterGrid() {
    var fromDate = $("#fromDate").val(); var toDate = $("#toDate").val();
    if (fromDate == "" || toDate == "") {
        funToastr(false, "Please select from and to date."); return;
    }
    else {
        StartProcess();
        if ($("#hfCurrentPageIndex").val() == "") {
            $("#hfCurrentPageIndex").val("1");
        }
        location.href = '/UserMaster/GetUsers?range=' + fromDate + ',' + toDate + '&currentPage=' + $("#hfCurrentPageIndex").val();
    }
}
function PagerClick(index) {
    StartProcess();
    $("#hfCurrentPageIndex").val(index);
    var fromDate = $("#fromDate").val() != undefined ? $("#fromDate").val() : ""; var toDate = $("#toDate").val() == undefined ? "" : $("#toDate").val();
    var range = "";
    if (fromDate !== "" && toDate !== "") {
        range = fromDate + ',' + toDate;
    }
    location.href = '/UserMaster/LoadUserMasters?currentPageIndex=' + $("#hfCurrentPageIndex").val() + '&range=' + range;
}
var ExcelFile = document.getElementById('ExcelFile');

ExcelFile.onchange = function () {
    var files = this.files;
    var file = files[0];
    var filename = file.name
    var acceptedFiles = ["xlsx", "xls"];
    debugger
    let last_dot = filename.lastIndexOf('.')
    let ext = filename.slice(last_dot + 1);
    var isAcceptedExcelFormat = ($.inArray(ext, acceptedFiles)) !== -1;
    if (!isAcceptedExcelFormat) {
        isValidExcel = false;
        funToastr(false, 'Please select excel file');
    }
    else {
        isValidExcel = true;
        var reader = new FileReader();
        reader.onloadend = function () {
           
            $(ExcelFile).attr("data-base64string", reader.result);
            $(ExcelFile).attr("data-extension", ext);
            //PreviewBase64Image(reader.result, $this.id + "Preview");
        }
        reader.readAsDataURL(file);
       
    }
}
function ImportExcelFile() {
    if (!isValidExcel) {
        funToastr(false, 'Please select excel file first.');

    }
    else {
        var documentFile = $("#ExcelFile").get(0).files;
        var docfile = "";
        if (documentFile.length > 0) {
            var documentFileExt = documentFile[0].name.split('.').pop();
            docfile = $("#ExcelFile").attr('data-base64string');
            docfile = docfile + ',' + documentFileExt;
        }
       debugger
        StartProcess();
        $.ajax({
            type: "POST",
            url: "/UserMaster/ImportUsers",
            data: { file: docfile },
            success: function (data) {
                StopProcess();
                console.log(data);
                if (data.status == "Fail") {
                    
                    $("#lblError").addClass("adderror").text(data.error).show();
                }
                else {
                   
                }
            }
        });
    }
}
    

    



