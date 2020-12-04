
$(document).ready(function () {
});

function deleteEmployeeCompliant(id) {
    Confirm('Are you sure?', 'You will not be able to recover this', 'Yes', 'Cancel', id); /*change*/

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
};

function performAction(id, isView) {
    let url = `/Compliant/Edit?id=${id}&isView=${isView}`
    location.href = url;
}

addAttachement = function () {

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
                $("#tblList").find('tbody').empty();
                $("#tblList").find('tbody').html(data.data.Url);
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
