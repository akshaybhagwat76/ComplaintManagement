function deletePolicy(PolicyId) {
    debugger
    $('#deleteModal').data('PolicyId', PolicyId).modal('show');
    $('#deleteModal').modal('show');
}

function searchKeyPress(e) {
    // look for window.event in case event isn't passed in
    e = e || window.event;
    if (e.keyCode == 13) {
        SearchPolicy(e.target.value);
        return false;
    }
    return true;
}

function searchCategries(searchText) {
    if (searchText !== null && searchText !== "") {
        location.href = "/Policy/SearchPolicy?search=" + searchText;
    }

}
$('#delete-btn').click(function () {

    var id = $('#deleteModal').data('PolicyId');
    StartProcess();
    $.ajax({
        type: "POST",
        url: "/Policy/Delete",
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



function performAction(id, isView) {

    let url = `/Policy/Edit?PolicyId=${id}&isView=${isView}`



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
        location.href = '/Policy/GetPolicy?=' + fromDate + ',' + toDate + '&currentPage=' + $("#hfCurrentPageIndex").val();
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
        location.href = '/Policy/LoadPolicies?currentPageIndex=' + $("#hfCurrentPageIndex").val();
    }
    else {
        location.href = '/Policy/LoadPolicies?currentPageIndex=' + $("#hfCurrentPageIndex").val();
    }
}
