var ctx = document.getElementById("categoryChart").getContext('2d');
var myChart = new Chart(ctx, {
    position: 'right',
    type: 'pie',
    data: {
        labels: ["", "", "", "", ""],
        datasets: [{
            backgroundColor: [
                "#be5168",
                "#993767",
                "#3c8e9d",
                "#e9d78e",
                "#e2975d",
                "#e16552"
            ],
            data: [50, 55, 60, 33, 45, 60]
        }]
    },
    // Configuration options go here
    options: {
        legend: {
            position: 'right'
        },
        tooltips: {
            enabled: false
        }
    },
    //plugins: {
    //   datalabels: {
    //        formatter: (value, ctx) => {
    //           let datasets = ctx.chart.data.datasets;
    //           if (datasets.indexOf(ctx.dataset) === datasets.length - 1) {
    //               let sum = datasets[0].data.reduce((a, b) => a + b, 0);
    //               let percentage = Math.round((value / sum) * 100) + '%';
    //               return percentage;
    //           } else {
    //               return percentage;
    //           }
    //        }
    //    }
    //}
});

var ctx1 = document.getElementById("subcategoryChart").getContext('2d');
var myChart1 = new Chart(ctx1, {
    position: 'right',
    type: 'pie',
    data: {
        labels: ["Reyas", "Chu", "Williams"],
        datasets: [{
            backgroundColor: [
                "#7a61ba",
                "#d8b655",
                "#4198d7"
            ],
            data: [50, 55, 60]
        }]
    },
    // Configuration options go here
    options: {
        legend: {
            position: 'right',
            labels: {
                usePointStyle: true
            }
        },
        tooltips: {
            enabled: false
        }
    },
});

var ctx2 = document.getElementById("losChart").getContext('2d');
var myChart2 = new Chart(ctx2, {
    position: 'right',
    type: 'pie',
    data: {
        labels: ["Green", "Blue", "Purple", "Purple"],
        datasets: [{
            backgroundColor: [
                "#50514f",
                "#eec683",
                "#81ddc6",
                "#f45e58"
            ],
            data: [50, 55, 60, 55]
        }]
    },
    // Configuration options go here
    options: {
        legend: {
            display: false
        },
        tooltips: {
            enabled: false
        }
    },
});

var ctx3 = document.getElementById("overallChart").getContext('2d');
var myChart3 = new Chart(ctx3, {
    position: 'right',
    type: 'pie',
    data: {
        datasets: [{
            backgroundColor: [
                "#5a9bd5",
                "#4472c5",
                "#ed7d32",
                "#a5a5a5",
                "#ffc100"
            ],
            data: [50, 55, 60, 55, 50]
        }]
    },
    // Configuration options go here
    options: {
        legend: {
            display: false
        },
        tooltips: {
            enabled: false
        }
    },
});

$(document).ready(function () {
    var currentUserrole = $("#Role").val();
    if (currentUserrole !== "" || currentUserrole !== undefined) {
        if (currentUserrole.length > 2) {
            if (currentUserrole.toLowerCase() === "normal".toLowerCase()) {
                if ($("#AwaitingComplaintsCount").text() === "0") {
                    $("#awaitingComplaint").addClass("disabled-div");
                }
                if ($("#OverDueComplaintsCount").text() === "0") {
                    $("#OverDueComplaintPopup").addClass("disabled-div");
                }
                if ($("#dueComplaintsCount").text() === "0") {

                    $("#dueComplaintPopup").addClass("disabled-div");
                }
            }
        }
    }
});

getAwaitingComplaint = function () {
    StartProcess();
    var url = "/Compliant/GetAwaitingComplaints";
    $("#awaitingComplaintsContent").load(url, function () {
        $("#awaitingComplaints").modal("show");

        var table = $('#awaitingTable').DataTable({
        });
        $(table.table().container()).removeClass('form-inline');
        addClassFunction();
        StopProcess();

        $('#awaitingTable tbody').on('click', 'tr', function () {
            var data = table.row(this).data();
            if (data.length > 0) {
                if (data[0].includes("id=")) {
                    if (data[0].split("").length > 14) {
                        var text = data[0];
                        text = text.replace("<strong", "<strong class='id'");

                        $("body").append(text)
                        let isView = true; let id = $(".id").attr("id");
                        let url = `/Compliant/Edit?id=${id}&isView=${isView}`
                        location.href = url;
                    }
                }
            }
        });
    })
}

function getDueComplaints() {
    StartProcess();
    var url = "/Compliant/GetDueComplaints";
    $("#dueComplaintsContent").load(url, function () {
        var table = $('#dueTable').DataTable({
        });
        $(table.table().container()).removeClass('form-inline');
        addClassFunction();
        StopProcess();
        $("#dueComplaints").modal("show")
        $('#dueTable tbody').on('click', 'tr', function () {
            var data = table.row(this).data();
            if (data.length > 0) {
                if (data[0].includes("id=")) {
                    if (data[0].split("").length > 14) {
                        var text = data[0];
                        text = text.replace("<strong", "<strong class='id'");

                        $("body").append(text)
                        let isView = false; let id = $(".id").attr("id");
                        let isRedirect = "2";
                        var currentUserrole = $("#Role").val();
                        if (currentUserrole == "Committee") {
                            let url = `/Compliant/Compliant_three?Id=${id}&isView=${isView}`
                            location.href = url;
                        } else {
                            let url = `/Compliant/Edit?id=${id}&isView=${isView}&isRedirect=${isRedirect}`
                            location.href = url;
                        }
                       
                    }
                }
            }
        });
    });
}

function getOverDueComplaints() {
    StartProcess();
    var url = "/Compliant/GetOverDueComplaints";
    $("#overdueComplaintsContent").load(url, function () {
        var table = $('#overdueTable').DataTable({
        });
        $(table.table().container()).removeClass('form-inline');
        addClassFunction();
        StopProcess();
        $("#overdueComplaints").modal("show")
        $('#overdueTable tbody').on('click', 'tr', function () {
            var data = table.row(this).data();
            if (data.length > 0) {
                if (data[0].includes("id=")) {
                    if (data[0].split("").length > 14) {
                        var text = data[0];
                        text = text.replace("<strong", "<strong class='id'");

                        $("body").append(text)
                        let isView = false; let id = $(".id").attr("id");
                        let isRedirect = "2";
                        var currentUserrole = $("#Role").val();
                        if (currentUserrole == "Committee") {
                            let url = `/Compliant/Compliant_three?Id=${id}&isView=${isView}`
                            location.href = url;
                        } else {
                            let url = `/Compliant/Edit?id=${id}&isView=${isView}&isRedirect=${isRedirect}`
                            location.href = url;
                        }
                    }
                }
            }
        });
    });
}


function addClassFunction() {
    $('.pagination a').addClass('page-link');
}