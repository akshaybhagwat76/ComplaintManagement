$(document).ready(function () {
    var table = $('#historyTable').DataTable({
    });
    $(table.table().container()).removeClass('form-inline');
    addClassFunction()
    function addClassFunction() {
        $('.pagination a').addClass('page-link');
    }
})