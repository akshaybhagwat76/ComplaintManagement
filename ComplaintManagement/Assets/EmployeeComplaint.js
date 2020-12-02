﻿$(document).ready(function () {
    $.noConflict();
   
    // $("#myTable").DataTable();
});

function deleteEmployeeCompliant(id) {
   
    //$('#deleteModal').data('id', id).modal('show');
    //$('#deleteModal').modal('show');
    Confirm('Are you sure?', 'You will not be able to recover this', 'Yes', 'Cancel', id); /*change*/

}

function searchKeyPress(e) {
    // look for window.event in case event isn't passed in
    debugger
    e = e || window.event;
    if (e.keyCode == 13) {
        searchEmployeeCompliant(e.target.value);
        return false;
    }
    return true;
}

function searchEmployeeCompliant(searchText) {
    debugger
    if (searchText !== null && searchText !== "") {
       
        location.href = "/EmployeeCompliant/searchEmployeeCompliant?search=" + searchText;
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

function filterGrid() {
    debugger
    var fromDate = $("#fromDate").val(); var toDate = $("#toDate").val();
    if (fromDate == "" || toDate == "") {
        funToastr(false, "Please select from and to date."); return;
    }
    else {
        StartProcess();
        if ($("#hfCurrentPageIndex").val() == "") {
            $("#hfCurrentPageIndex").val("1");
        }
        location.href = '/Employee/GetEmployeeCompliant?range=' + fromDate + ',' + toDate + '&currentPage=' + $("#hfCurrentPageIndex").val();
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
        location.href = '/Category/LoadCategories?currentPageIndex=' + $("#hfCurrentPageIndex").val() + '&range=' + range;
    }
    else {
        location.href = '/Category/LoadHistoryCategories?currentPageIndex=' + $("#hfCurrentPageIndex").val() + '&range=' + $("#history").val();
    }
}