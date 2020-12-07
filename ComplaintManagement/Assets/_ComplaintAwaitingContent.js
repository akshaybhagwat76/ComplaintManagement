$(document).ready(function () {
    var table = $('#awaitingTable').DataTable({
    });
    $(table.table().container()).removeClass('form-inline');
    addClassFunction()
    function addClassFunction() {
        $('.pagination a').addClass('page-link');
    }
})