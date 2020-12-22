import { type } from "jquery";

function deleteLOS(id) {
    $('#deleteModal').data('id', id).modal('show');
    $('#deleteModal').modal('show');
}
    
function searchKeyPress(e) {
    
    // look for window.event in case event isn't passed in
    e = e || window.event;
    if (e.keyCode == 13) {
        search(e.target.value);
        return false;
    }
    return true;
}

function search(searchText) {
    if (searchText !== null && searchText !== "") {
        location.href = "/LOS/SearchLOS?search=" + searchText;
    }

}




$('#delete-btn').click(function () {
    var id = $('#deleteModal').data('id');
    StartProcess();
    $.ajax({
        type: "POST",
        url: "/LOS/Delete",
        data: { id: id },
        success: function (response) {
           
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
});
function performAction(id, isView) {
    let url = `/LOS/Edit?id=${id}&isView=${isView}`
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
        location.href = '/LOS/GetLOS?range=' + fromDate + ',' + toDate + '&currentPage=' + $("#hfCurrentPageIndex").val();
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
    if ($("#history").val() == undefined) {
    location.href = '/LOS/LoadLOS?currentPageIndex=' + $("#hfCurrentPageIndex").val() + '&range=' + range;
    }
    else {
        location.href = '/LOS/LoadHistoryLOS?currentPageIndex=' + $("#hfCurrentPageIndex").val() + '&id=' + $("#history").val();
    }
} function column_sort() {
    getCellValue = (tr, idx) => tr.children[idx].innerText || tr.children[idx].textContent;
    comparer = (idx, asc) => (a, b) => ((v1, v2) =>
        v1 !== '' && v2 !== '' && !isNaN(v1) && !isNaN(v2) ? v1 - v2 : v1.toString().localeCompare(v2)
    )(getCellValue(asc ? a : b, idx), getCellValue(asc ? b : a, idx));

    table = $(this).closest('table')[0];
    tbody = $(table).find('tbody')[0];

    elm = $(this)[0];
    children = elm.parentNode.children;
    Array.from(tbody.querySelectorAll('tr')).sort(comparer(
        Array.from(children).indexOf(elm), table.asc = !table.asc))
        .forEach(tr => tbody.appendChild(tr));
}

$("#myTable").find('thead td').on('click', column_sort);

// Import excel
var inputImage = document.getElementById('excelFile');
document.getElementById('excelFile').addEventListener('change',
    importExcel,
    false);
function importExcel() {
    var files = this.files;
    var file;
    if (files && files.length) {
        file = files[0];
        const filename = file.name;
        let last_dot = filename.lastIndexOf('.')
        let ext = "." + filename.slice(last_dot + 1);
        if (ext.toLowerCase() == '.xlsx' || ext.toLowerCase() == '.xlsm' || ext.toLowerCase() == '.xltx' || ext.toLowerCase() == '.xltm' || ext.toLowerCase() == '.xls') {
            var reader = new FileReader();
            reader.onloadend = function () {
                $(inputImage).attr('src', '');
                $(inputImage).attr("data-base64string", '');
                $(inputImage).attr("data-extension", '');

                $(inputImage).attr('src', reader.result);
                $(inputImage).attr("data-base64string", reader.result);
                $(inputImage).attr("data-extension", ext);
            }
            reader.readAsDataURL(file);
        } else {
            funToastr(false, 'Please choose an excel file');
        }

    }
};

$("#btnUpload").on("click", function () {
    var excelFile = $("#excelFile").get(0).files;
    var excelUploadedFile = "";
    if (excelFile.length > 0) {
        var excelFileExt = excelFile[0].name.split('.').pop();
        excelUploadedFile = $("#excelFile").attr('data-base64string');
        excelUploadedFile = excelUploadedFile + ',' + excelFileExt;
    }
    else {
        funToastr(false, "Please upload file."); return;
    }
    StartProcess();
    $.ajax({
        type: "POST",
        url: "/LOS/ImportLOS",
        data: { file: excelUploadedFile },
        success: function (data) {
            if (data.status == "Fail") {
                StopProcess();
                funToastr(false, data.error);
                $("#lblError").addClass("adderror").text(data.error).show();
            }
            else {
                StopProcess();
                funToastr(true, data.msg + " Records imported." + " Please reload page to refresh changes.")
                $("#lblError").removeClass("adderror").addClass("success").text(data.msg + " Records imported." + " Please reload page to refresh changes.").show();
            }
        }, error: function (response) {
            funToastr(false, response.responseText);
        },
        failure: function (response) {
            funToastr(false, response.responseText);
        }
    });
})



//17/12/2020
function filterLosReportGrid() {
    var fromDate = $("#fromDate").val(); var toDate = $("#toDate").val();
    var losid = $(".losval option:selected").val();
    if (fromDate == "" || toDate == "" || losid == "") {
        funToastr(false, "Please select from and to date and a LOSName"); return;
    }
    else {
       
        if ($("#hfCurrentPageIndex").val() == "") {
            $("#hfCurrentPageIndex").val("1");
        }
        location.href = '/LOS/GetLOSReport?range=' + fromDate + ',' + toDate + '&losid=' + losid + '&currentPage=' + $("#hfCurrentPageIndex").val();
    }
}
    //18/12/2020
    function exportLosReport() {
        var fromDate = $("#fromDate").val(); var toDate = $("#toDate").val();
        var losid = $(".losval option:selected").val();
        if (fromDate == "" || toDate == "" || losid == "") {
            funToastr(false, "Please select from and to date and a LOSName"); return;
        }
        else {
          
            if ($("#hfCurrentPageIndex").val() == "") {
                $("#hfCurrentPageIndex").val("1");
            }
            location.href = '/LOS/ExportLDataLOsReport?range=' + fromDate + ',' + toDate + '&losid=' + losid + '&currentPage=' + $("#hfCurrentPageIndex").val();
        }
    
}
//19/12/2020
function filterXOLReportGrid() {
    var fromDate = $("#fromDate").val(); var toDate = $("#toDate").val();
    var losid = $(".losval option:selected").val();
    var types = $("#types").val();
    var typevalues = $("#typevalues").val();
    if (fromDate == "" || toDate == "" || types == "" || typevalues=="") {
        funToastr(false, "Please select from and to date and a types and a values"); return;
    }
    else {

        if ($("#hfCurrentPageIndex").val() == "") {
            $("#hfCurrentPageIndex").val("1");
        }
        location.href = '/LOS/GetXOLReport?range=' + fromDate + ',' + toDate + '&types=' + types + '&typevalues=' + typevalues +  '&currentPage=' + $("#hfCurrentPageIndex").val();
    }
}


