$(document).ready(function () {
});

function deleteEmployeeCompliant(id) {
    $('#deleteModal').data('id', id).modal('show');
    $('#deleteModal').modal('show');

}

function searchKeyPress(e) {
    e = e || window.event;
    if (e.keyCode == 13) {
        e.preventDefault();
        searchEmployeeCompliant(e.target.value);
        return false;
    }
    return true;
}


function searchEmployeeCompliant(searchText) {
    if (searchText !== null && searchText !== "") {
        location.href = "/Employee/searchEmployeeCompliant?search=" + searchText;
    }
}

function column_sort() {
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


widthdrawComplaint  = function (id) {
    $('#widthdrawModel').data('id', id).modal('show');
    $('#widthdrawModel').modal('show');
}


withdrawComplaintForSubmission = function () {
    var id = $('#widthdrawModel').data('id');
    var retval = true;
    var str = $('#Remark').val();
    if (str == "" || str.length == 0) {
        $('#Remark').addClass("adderror");
        funToastr(false, "Remarks cannot be blank."); return;
    }
    if (/^[a-zA-Z0-9- ]*$/.test(str) == false) {
        retval = false;
        $('#Remark').addClass("adderror");
        funToastr(false, "Remarks cannot contain special characters.");
    }
    else {
        $('#Remark').removeClass("adderror");
    }

    var data = {
        Id: id,
        Remark: $("#Remark").val()
    }
    if (retval) {
        StartProcess();

        $.ajax({
            type: "POST",
            url: "/Compliant/WithdrawComplaint",
            data: {
                withdrawData: JSON.stringify(data)
            },
            success: function (data) {
                if (data.status == "Fail") {
                    StopProcess();
                    $("#lblError").removeClass("success").removeClass("adderror").addClass("adderror").text(data.error).show();
                }
                else {
                    location.reload();
                }
            },
            error: function (error) {
                funToastr(false, error);
            },
        });
    }
}

$('#delete-btn').click(function () {
    var id = $('#deleteModal').data('id');
    StartProcess();
    $.ajax({
        type: "POST",
        url: "/Compliant/Delete",
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

$('#delete-btn').click(function () {
    var id = $('#deleteModal').data('id');
    StartProcess();
    $.ajax({
        type: "POST",
        url: "/Compliant/WithdrawComplaint",
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
    let url = `/Compliant/Edit?id=${id}&isView=${isView}`
    location.href = url;
}


function filterGrid() {
    var data = {
        fromDate: document.getElementById("fromDate").value,
        toDate: document.getElementById("toDate").value,
        CategoryId: document.getElementById("CategoryId").value,
        SubCategoryId: document.getElementById("SubCategoryId").value
    };
    StartProcess();
    if (fromDate == "" || toDate == "") {
        StopProcess();
        funToastr(false, "Please select from and to date."); return;
    }
    $.ajax({
        type: "POST",
        url: "/Employee/LoadEmployeeComplaints",
        data: { data: data },
        success: function (data) {
            if (data.status == "Fail") {
                StopProcess();
                funToastr(false, data.error);
            }
            else {
                StopProcess();
                $("#myTable").find('tbody').empty();
                $("#myTable").find('tbody').html(data.data.Url);
            }
        }, error: function (response) {
            funToastr(false, response.responseText);
        },
        failure: function (response) {
            funToastr(false, response.responseText);
        }
    })
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
        location.href = '/Employee/LoadEmployeeComplain?currentPageIndex=' + $("#hfCurrentPageIndex").val() + '&range=' + range;
    }
    else {
        location.href = '/Employee/LoadEmployeeComplain?currentPageIndex=' + $("#hfCurrentPageIndex").val() + '&range=' + $("#history").val();
    }
}

function getHistory(id) {
    if (id != "") {
        var url = "/Compliant/GetHistoryByComplaint?ComplaintId=" + id;
        $("#historyContent").load(url, function () {
            $("#historyModal").modal("show");
        })
    }
}

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
        url: "/Employee/ImportEmployeeCompliant",
        data: { file: excelUploadedFile },
        success: function (data) {
            if (data.status == "Fail") {
                StopProcess();
                funToastr(false, data.error);
                $("#lblError").removeClass("success").removeClass("adderror").addClass("adderror").text(data.error).show();
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
