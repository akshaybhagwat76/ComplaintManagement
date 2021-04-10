//var ctx1 = document.getElementById("categoryChart").getContext('2d');
//var myChart1 = null;

$(document).ready(function () {
    $('.multiselect').fSelect();
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

    if (currentUserrole.toLowerCase() == "lead" || currentUserrole.toLowerCase() == "admin") {
        let today = new Date();
        $('#txt_dateFrom').datepicker({
            dateFormat: 'dd/mm/yy',
            maxDate: 0,
            minDate: new Date(`01/01/${today.getUTCFullYear() - 1}`)
        });
        $('#txt_dateTo').datepicker({
            dateFormat: 'dd/mm/yy',
            maxDate: 0,
            beforeShow: function (dateText, inst) {
                $(this).datepicker('option', 'minDate', $('#txt_dateFrom').val());
            }
        });
        $('#txt_dateTo').val(`${today.getUTCDate().toString().padStart(2, "0")}/${(today.getUTCMonth() + 1).toString().padStart(2, "0")}/${today.getUTCFullYear()}`);
        $('#txt_dateFrom').val(`01/01/${today.getUTCFullYear()}`);
        if ($('#ddlChart').val() == 'default') {
            DashboardPiGraphData();
        }
        else {
            DashboardBarGraphData();
        }
        $(".divHideShow").css("display", "block");

    }
    else {
        $(".divHideShow").css("display", "none");
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



function CaseType() {
    StartProcess();
    var typevalues ="InProgess";
    var url = "/Home/GetCaseint?typevalues=" + typevalues +"";
    $("#CasesContentTyped").load(url, function () {
        var table = $('#caseTables').DataTable({
        });
        $(table.table().container()).removeClass('form-inline');
        addClassFunction();
        StopProcess();
        $("#CaseTypes").modal("show")
       
    });
}


function CaseTyped() {
    StartProcess();
    var typevalues = "Closed";
    var url = "/Home/GetCaseint?typevalues=" + typevalues + "";
    $("#CasesContentTyped").load(url, function () {
        var table = $('#caseTables').DataTable({
        });
        $(table.table().container()).removeClass('form-inline');
        addClassFunction();
        StopProcess();
        $("#CaseTypes").modal("show")

    });
}


function addClassFunction() {
    $('.pagination a').addClass('page-link');
}

function getRandomColorHex() {
    var hex = "0123456789ABCDEF",
        color = "#";
    for (var i = 1; i <= 6; i++) {
        color += hex[Math.floor(Math.random() * 16)];
    }
    return color;
}

function formatDate(date) {
    var d = new Date(date),
        month = '' + (d.getMonth() + 1),
        day = '' + d.getDate(),
        year = d.getFullYear();

    if (month.length < 2)
        month = '0' + month;
    if (day.length < 2)
        day = '0' + day;

    return [year, month, day].join('-');
}

var myChart = null;
var myChart1 = null;
var myChart2 = null;
var myChart3 = null;
var myChart4 = null;
var myChart5 = null;
var myChart6 = null;
var myChart7 = null;
var myChart8 = null;
var myChart9 = null;
var myChart10 = null;
var myChart11 = null;
var myChart12 = null;
var myChart13 = null;

$(document).on('change', '#ddlChart', function () {
    var today = new Date();
    if ($(this).val() == 'default') {

        $('#txt_dateFrom').val(`01/01/${today.getUTCFullYear()}`);

    }
    else {

        let backYear = today.getFullYear() - 1;
        backYear = '01/01/' + backYear;
        $('#txt_dateFrom').val(backYear);
        //$('#txt_dateFrom').prop('min', backYear)
        //$('#txt_dateTo').prop('min', backYear)
        //$('#txt_dateFrom').prop("max", `${today.getUTCFullYear()}-${(today.getUTCMonth() + 1).toString().padStart(2, "0")}-${today.getUTCDate().toString().padStart(2, "0")}`);
        //$('#txt_dateTo').prop("max", `${today.getUTCFullYear()}-${(today.getUTCMonth() + 1).toString().padStart(2, "0")}-${today.getUTCDate().toString().padStart(2, "0")}`);

    }

})

//







$(document).on('click', '#btn_dataSearch', function () {
    if ($('#ddlChart').val() == 'default' && $('#ddlCharted').val() == 'numbers') {
        //$('#divAgeingChart').css('display', 'block');
        DashboardPiGraphData();
    }
    else if ($('#ddlChart').val() == 'comparison' && $('#ddlCharted').val() == 'numbers') {
        //$('#divAgeingChart').css('display', 'none');
        //StartProcess();
        DashboardBarGraphData();
    }
    else if ($('#ddlChart').val() == 'default' && $('#ddlCharted').val() == 'percentage') {
        DashboardpiePercent();
    }
    else if ($('#ddlChart').val() == 'comparison' && $('#ddlCharted').val() == 'percentage') {
        dashboardgraphpercent();
    }
})

function DashboardPiChartTableBind(dateFrom, dateTo, chart, label) {
    //alert(dateFrom);
    //alert(dateTo);
    //alert(chart);
    //alert(label);
    StartProcess();
    label = label.split(" ").join("");
    var url = `/Home/DashboardPiChartTableBind?dateFrom=${dateFrom}&dateTo=${dateTo}&chart=${chart}&label=${label}`;
    $("#chartWiseTableShow").load(url, function () {
        var table = $('#dashboardChart').DataTable({
        });
        $(table.table().container()).removeClass('form-inline');
        addClassFunction();
        StopProcess();
        $("#chartWiseDataBind").modal("show")
    });
}

function DashboardBarChartTableBind(dateFrom, dateTo, chart, label, year) {
    StartProcess();
    label = label.split(" ").join("");
    var url = `/Home/DashboardBarChartTableBind?dateFrom=${dateFrom}&dateTo=${dateTo}&chart=${chart}&label=${label}&year=${year}`;
    $("#chartWiseTableShow").load(url, function () {
        var table = $('#dashboardChart').DataTable({
        });
        $(table.table().container()).removeClass('form-inline');
        addClassFunction();
        StopProcess();
        $("#chartWiseDataBind").modal("show")
    });
}
var Base64Image = [];
function ChartBase64Image(base64, heading) {
    //var url_base64jp = document.getElementById("myChart").toDataURL("image/jpg");
    var url_base64 = base64.replace('data:image/png;base64,', '');
    Base64Image.push({ UrlBase64: url_base64, Heading: heading });

}

$(document).on('click', '#btn_chartToImage', function () {
    //alert();
    console.log(Base64Image);
    $.post('/home/PrintBase64ToPPT', { jsonInput: JSON.stringify(Base64Image) }, function (result) {

    });
})

$(document).on('click', '#btn_sendMail', function () {
    $('#ModalPopUp').modal('show');
})

$(document).on('click', '#sendemail', function () {
    StartProcess();
    var JsonData = [];
    JsonData.push({ Comment: $('#Comment').val(), DateFrom: $('#txt_dateFrom').val(), DateTo: $('#txt_dateTo').val(), ChartType: $('#ddlChart').val() });
    var UserInvolved = $('.multiselect').val();
    console.log(UserInvolved);
    $.post('/home/SendMailBase64ToPPT', { jsonInput: JSON.stringify(Base64Image), IsMailSend: true, JsonData: JSON.stringify(JsonData), InvolveUserId: UserInvolved }, function (result) {
        StopProcess();
        if (result.status != "Fail") {
            $('#ModalPopUp').modal('hide');
            //$('.multiselect').val('');
            //$('#Comment').val('');
            alert("Mail has been sent");
        } else {
            $('#ModalPopUp').modal('show');
            console.log(result);
            funToastr(false, "mail has been not sent");
        }
    });
})



function DashboardPiGraphData() {
    Base64Image = [];
    StartProcess();
    let dateFrom = ($('#txt_dateFrom').val());
    let dateTo = ($('#txt_dateTo').val());
    $.get('/home/DashboardPiChart', { dateFrom: dateFrom, dateTo: dateTo }, function (result) {
        StopProcess();
        if (result != null && result != "undefined" && result != "") {
            //Case Type Pi chart
            if (result.caseTypeofComplaint != null) {
                var Labels = []; var Values = []; var Colors = [];
                $.each(result.caseTypeofComplaint, function (i) {
                    Labels.push(result.caseTypeofComplaint[i].Label);
                    Values.push(result.caseTypeofComplaint[i].Value);
                    Colors.push(getRandomColorHex());
                });

                var ctx = document.getElementById("caseTypeChart").getContext('2d');
                if (myChart != null) {
                    myChart.destroy();
                }
                myChart = new Chart(ctx, {
                    //position: 'right',
                    type: 'pie',
                    data: result,
                    data: {
                        labels: Labels,
                        datasets: [{
                            backgroundColor: Colors,
                            data: Values,
                            datalabels: {
                                anchor: 'end'
                            }
                        }]
                    },
                    options: {
                        legend: {
                            position: 'bottom',
                        },
                        tooltips: {
                            enabled: true,
                        },
                        animation: {
                            animateScale: true,
                            animateRotate: true
                        },
                        plugins: {
                            datalabels: {
                                backgroundColor: function (context) {
                                    return context.dataset.backgroundColor;
                                },
                                borderColor: 'white',
                                borderRadius: 100,
                                borderWidth: 2,
                                color: 'white',
                                font: {
                                    weight: 'bold',
                                    size: 18
                                },
                                padding: 2,
                                align: 'start',

                                //formatter: Math.round
                            }
                        },
                        onClick: function (event) {
                            var activePoints = myChart.getElementsAtEvent(event);
                            var chartData = activePoints[0]['_chart'].config.data;
                            var idx = activePoints[0]['_index'];
                            var label = chartData.labels[idx];
                            DashboardPiChartTableBind(dateFrom, dateTo, "CaseType", label);
                        },
                        animation: {
                            onComplete: function () {
                                var url_base64 = document.getElementById("caseTypeChart").toDataURL("image/png");
                                ChartBase64Image(url_base64, "Case Type");
                                this.options.animation.onComplete = null;
                            }
                        },

                    }
                });
            }
            else {
                 $('#divCaseTypeChart').css('display', 'none');
            }
            //Category Wise Pi Chart
            if (result.categoryPiCharts != null) {
                var Labels = [];
                var Values = [];
                var Colors = [];
                $.each(result.categoryPiCharts, function (i) {
                    Labels.push(result.categoryPiCharts[i].Label);
                    Values.push(result.categoryPiCharts[i].Value);
                    Colors.push(getRandomColorHex());
                });
                var ctx1 = document.getElementById("categoryChart").getContext('2d');
                if (myChart1 != null) {
                    myChart1.destroy();
                }
                myChart1 = new Chart(ctx1, {
                    //position: 'right',
                    type: 'pie',
                    data: {
                        labels: Labels,
                        datasets: [{
                            backgroundColor: Colors,
                            data: Values,
                            datalabels: {
                                anchor: 'end'
                            }
                        }]
                    },
                    // Configuration options go here
                    options: {
                        //responsive: true,
                        legend: {
                            position: 'bottom',

                        },
                        tooltips: {
                            enabled: true
                        },
                        animation: {
                            animateScale: true,
                            animateRotate: true
                        },
                        plugins: {
                            datalabels: {
                                backgroundColor: function (context) {
                                    return context.dataset.backgroundColor;
                                },
                                borderColor: 'white',
                                borderRadius: 100,
                                borderWidth: 2,
                                color: 'white',
                                font: {
                                    weight: 'bold',
                                    size: 18
                                },
                                padding: 2,
                                align: 'start',

                                //formatter: Math.round
                            }
                        },
                        onClick: function (event) {
                            var activePoints = myChart1.getElementsAtEvent(event);
                            var chartData = activePoints[0]['_chart'].config.data;
                            var idx = activePoints[0]['_index'];
                            var label = chartData.labels[idx];
                            DashboardPiChartTableBind(dateFrom, dateTo, "Category", label);
                        },
                        animation: {
                            onComplete: function () {
                                var url_base64 = document.getElementById("categoryChart").toDataURL("image/png");
                                ChartBase64Image(url_base64, "Category Wise");
                                this.options.animation.onComplete = null;
                            }
                        }
                    }

                });
            }
            else {
                $('#divCategoryChart').css('display', 'none');
            }
            //Sub-Category Wise Pi Chart
            if (result.subCategoryPiCharts != null) {
                var Labels = [];
                var Values = [];
                var Colors = [];
                $.each(result.subCategoryPiCharts, function (i) {
                    Labels.push(result.subCategoryPiCharts[i].Label);
                    Values.push(result.subCategoryPiCharts[i].Value);
                    Colors.push(getRandomColorHex());
                });
                var ctx2 = document.getElementById("subCategoryChart").getContext('2d');
                if (myChart2 != null) {
                    myChart2.destroy();
                }
                myChart2 = new Chart(ctx2, {
                    type: 'pie',
                    data: {
                        labels: Labels,

                        datasets: [{
                            backgroundColor: Colors,
                            data: Values,
                            datalabels: {
                                anchor: 'end'
                            }
                        }]
                    },
                    // Configuration options go here
                    options: {
                        responsive: true,
                        legend: {
                            position: 'bottom',

                        },
                        tooltips: {
                            enabled: true
                        },
                        animation: {
                            animateScale: true,
                            animateRotate: true
                        },
                        plugins: {
                            datalabels: {
                                backgroundColor: function (context) {
                                    return context.dataset.backgroundColor;
                                },
                                borderColor: 'white',
                                borderRadius: 100,
                                borderWidth: 2,
                                color: 'white',
                                font: {
                                    weight: 'bold',
                                    size: 18
                                },
                                padding: 2,
                                align: 'start',

                                //formatter: Math.round
                            }
                        },
                        onClick: function (event) {
                            var activePoints = myChart2.getElementsAtEvent(event);
                            var chartData = activePoints[0]['_chart'].config.data;
                            var idx = activePoints[0]['_index'];
                            var label = chartData.labels[idx];
                            DashboardPiChartTableBind(dateFrom, dateTo, "SubCategory", label);
                        },
                        animation: {
                            onComplete: function () {
                                var url_base64 = document.getElementById("subCategoryChart").toDataURL("image/png");
                                ChartBase64Image(url_base64, "Sub-Category wise");
                                this.options.animation.onComplete = null;
                            }
                        }
                    }
                });
            }
            else {
                $('#divSubCategoryChart').css('display', 'none');
            }
            //Region Wise Pi Chart
            if (result.regionPiCharts != null) {
                var Labels = [];
                var Values = [];
                var Colors = [];
                $.each(result.regionPiCharts, function (i) {
                    Labels.push(result.regionPiCharts[i].Label);
                    Values.push(result.regionPiCharts[i].Value);
                    Colors.push(getRandomColorHex());
                });
                var ctx3 = document.getElementById("regionChart").getContext('2d');
                if (myChart3 != null) {
                    myChart3.destroy();
                }
                myChart3 = new Chart(ctx3, {
                    type: 'pie',
                    data: {
                        labels: Labels,

                        datasets: [{
                            backgroundColor: Colors,
                            data: Values,
                            datalabels: {
                                anchor: 'end'
                            }
                        }]
                    },
                    // Configuration options go here
                    options: {
                        responsive: true,
                        legend: {
                            position: 'bottom',

                        },
                        tooltips: {
                            enabled: true
                        },
                        animation: {
                            animateScale: true,
                            animateRotate: true
                        },
                        plugins: {
                            datalabels: {
                                backgroundColor: function (context) {
                                    return context.dataset.backgroundColor;
                                },
                                borderColor: 'white',
                                borderRadius: 100,
                                borderWidth: 2,
                                color: 'white',
                                font: {
                                    weight: 'bold',
                                    size: 18
                                },
                                padding: 2,
                                align: 'start',

                                //formatter: Math.round
                            }
                        }
                        ,
                        onClick: function (event) {
                            var activePoints = myChart3.getElementsAtEvent(event);
                            var chartData = activePoints[0]['_chart'].config.data;
                            var idx = activePoints[0]['_index'];
                            var label = chartData.labels[idx];
                            DashboardPiChartTableBind(dateFrom, dateTo, "Region", label);
                        },
                        animation: {
                            onComplete: function () {
                                var url_base64 = document.getElementById("regionChart").toDataURL("image/png");
                                ChartBase64Image(url_base64, " Region wise");
                                this.options.animation.onComplete = null;
                            }
                        }
                    }
                });
            }
            else {
                $('#regionChart').css('display', 'none');
            }
            //Office Wise Pi Chart
            if (result.officePiCharts != null) {
                var Labels = [];
                var Values = [];
                var Colors = [];
                $.each(result.officePiCharts, function (i) {
                    Labels.push(result.officePiCharts[i].Label);
                    Values.push(result.officePiCharts[i].Value);
                    Colors.push(getRandomColorHex());
                });
                var ctx4 = document.getElementById("officeChart").getContext('2d');
                if (myChart4 != null) {
                    myChart4.destroy();
                }
                myChart4 = new Chart(ctx4, {
                    type: 'pie',
                    data: {
                        labels: Labels,

                        datasets: [{
                            backgroundColor: Colors,
                            data: Values,
                            datalabels: {
                                anchor: 'end'
                            }
                        }]
                    },
                    // Configuration options go here
                    options: {
                        responsive: true,
                        legend: {
                            position: 'bottom',

                        },
                        tooltips: {
                            enabled: true
                        },
                        animation: {
                            animateScale: true,
                            animateRotate: true
                        },
                        plugins: {
                            datalabels: {
                                backgroundColor: function (context) {
                                    return context.dataset.backgroundColor;
                                },
                                borderColor: 'white',
                                borderRadius: 100,
                                borderWidth: 2,
                                color: 'white',
                                font: {
                                    weight: 'bold',
                                    size: 18
                                },
                                padding: 2,
                                align: 'start',

                                //formatter: Math.round
                            }
                        }
                        ,
                        onClick: function (event) {
                            var activePoints = myChart4.getElementsAtEvent(event);
                            var chartData = activePoints[0]['_chart'].config.data;
                            var idx = activePoints[0]['_index'];
                            var label = chartData.labels[idx];
                            DashboardPiChartTableBind(dateFrom, dateTo, "Office", label);
                        },
                        animation: {
                            onComplete: function () {
                                var url_base64 = document.getElementById("officeChart").toDataURL("image/png");
                                ChartBase64Image(url_base64, "Office wise");
                                this.options.animation.onComplete = null;
                            }
                        }
                    }
                });
            }
            else {
                $('#officeChart').css('display', 'none');
            }
            //LOS Wise Pi Chart
            if (result.losPiCharts != null) {
                var Labels = [];
                var Values = [];
                var Colors = [];
                $.each(result.losPiCharts, function (i) {
                    Labels.push(result.losPiCharts[i].Label);
                    Values.push(result.losPiCharts[i].Value);
                    Colors.push(getRandomColorHex());
                });
                var ctx5 = document.getElementById("losChart").getContext('2d');
                if (myChart5 != null) {
                    myChart5.destroy();
                }
                myChart5 = new Chart(ctx5, {
                    type: 'pie',
                    data: {
                        labels: Labels,

                        datasets: [{
                            backgroundColor: Colors,
                            data: Values,
                            datalabels: {
                                anchor: 'end'
                            }
                        }]
                    },
                    // Configuration options go here
                    options: {
                        responsive: true,
                        legend: {
                            position: 'bottom',

                        },
                        tooltips: {
                            enabled: true
                        },
                        animation: {
                            animateScale: true,
                            animateRotate: true
                        },
                        plugins: {
                            datalabels: {
                                backgroundColor: function (context) {
                                    return context.dataset.backgroundColor;
                                },
                                borderColor: 'white',
                                borderRadius: 100,
                                borderWidth: 2,
                                color: 'white',
                                font: {
                                    weight: 'bold',
                                    size: 18
                                },
                                padding: 2,
                                align: 'start',

                                //formatter: Math.round
                            }
                        }
                        ,
                        onClick: function (event) {
                            var activePoints = myChart5.getElementsAtEvent(event);
                            var chartData = activePoints[0]['_chart'].config.data;
                            var idx = activePoints[0]['_index'];
                            var label = chartData.labels[idx];
                            DashboardPiChartTableBind(dateFrom, dateTo, "LOS", label);
                        },
                        animation: {
                            onComplete: function () {
                                var url_base64 = document.getElementById("losChart").toDataURL("image/png");
                                ChartBase64Image(url_base64, "LOS wise");
                                this.options.animation.onComplete = null;
                            }
                        }
                    }
                });
            }
            else {
                $('#divLosChart').css('display', 'none');
            }
            //SBU Wise Pi Chart
            if (result.sbuPiCharts != null) {
                var Labels = [];
                var Values = [];
                var Colors = [];
                $.each(result.sbuPiCharts, function (i) {
                    Labels.push(result.sbuPiCharts[i].Label);
                    Values.push(result.sbuPiCharts[i].Value);
                    Colors.push(getRandomColorHex());
                });
                var ctx6 = document.getElementById("sbuChart").getContext('2d');
                if (myChart6 != null) {
                    myChart6.destroy();
                }
                myChart6 = new Chart(ctx6, {
                    type: 'pie',
                    data: {
                        labels: Labels,

                        datasets: [{
                            backgroundColor: Colors,
                            data: Values,
                            datalabels: {
                                anchor: 'end'
                            }
                        }]
                    },
                    // Configuration options go here
                    options: {
                        responsive: true,
                        legend: {
                            position: 'bottom',

                        },
                        tooltips: {
                            enabled: true
                        },
                        animation: {
                            animateScale: true,
                            animateRotate: true
                        },
                        plugins: {
                            datalabels: {
                                backgroundColor: function (context) {
                                    return context.dataset.backgroundColor;
                                },
                                borderColor: 'white',
                                borderRadius: 100,
                                borderWidth: 2,
                                color: 'white',
                                font: {
                                    weight: 'bold',
                                    size: 18
                                },
                                padding: 2,
                                align: 'start',

                                //formatter: Math.round
                            }
                        }
                        ,
                        onClick: function (event) {
                            var activePoints = myChart6.getElementsAtEvent(event);
                            var chartData = activePoints[0]['_chart'].config.data;
                            var idx = activePoints[0]['_index'];
                            var label = chartData.labels[idx];
                            DashboardPiChartTableBind(dateFrom, dateTo, "SBU", label);
                        },
                        animation: {
                            onComplete: function () {
                                var url_base64 = document.getElementById("sbuChart").toDataURL("image/png");
                                ChartBase64Image(url_base64, "SBU wise");
                                this.options.animation.onComplete = null;
                            }
                        }
                    }
                });
            }
            else {
                $('#divSbuChart').css('display', 'none');
            }
            //Sub-SBU Wise Pi Chart
            if (result.subSBUPiCharts != null) {
                var Labels = [];
                var Values = [];
                var Colors = [];
                $.each(result.subSBUPiCharts, function (i) {
                    Labels.push(result.subSBUPiCharts[i].Label);
                    Values.push(result.subSBUPiCharts[i].Value);
                    Colors.push(getRandomColorHex());
                });
                var ctx7 = document.getElementById("subSBUChart").getContext('2d');
                if (myChart7 != null) {
                    myChart7.destroy();
                }
                myChart7 = new Chart(ctx7, {
                    type: 'pie',
                    data: {
                        labels: Labels,

                        datasets: [{
                            backgroundColor: Colors,
                            data: Values,
                            datalabels: {
                                anchor: 'end'
                            }
                        }]
                    },
                    // Configuration options go here
                    options: {
                        responsive: true,
                        legend: {
                            position: 'bottom',

                        },
                        tooltips: {
                            enabled: true
                        },
                        animation: {
                            animateScale: true,
                            animateRotate: true
                        },
                        plugins: {
                            datalabels: {
                                backgroundColor: function (context) {
                                    return context.dataset.backgroundColor;
                                },
                                borderColor: 'white',
                                borderRadius: 100,
                                borderWidth: 2,
                                color: 'white',
                                font: {
                                    weight: 'bold',
                                    size: 18
                                },
                                padding: 2,
                                align: 'start',

                                //formatter: Math.round
                            }
                        }
                        ,
                        onClick: function (event) {
                            var activePoints = myChart7.getElementsAtEvent(event);
                            var chartData = activePoints[0]['_chart'].config.data;
                            var idx = activePoints[0]['_index'];
                            var label = chartData.labels[idx];
                            DashboardPiChartTableBind(dateFrom, dateTo, "SubSBU", label);
                        },
                        animation: {
                            onComplete: function () {
                                var url_base64 = document.getElementById("subSBUChart").toDataURL("image/png");
                                ChartBase64Image(url_base64, "Sub-SBU wise");
                                this.options.animation.onComplete = null;
                            }
                        }
                    }
                });
            }
            else {
                $('#divSubSBUChart').css('display', 'none');
            }
            //Gender of Complainant Wise Pi Chart
            if (result.genderofComplainant != null) {
                var Labels = [];
                var Values = [];
                var Colors = [];
                $.each(result.genderofComplainant, function (i) {
                    Labels.push(result.genderofComplainant[i].Label);
                    Values.push(result.genderofComplainant[i].Value);
                    Colors.push(getRandomColorHex());
                });
                var ctx8 = document.getElementById("genderOfComplainantChart").getContext('2d');
                if (myChart8 != null) {
                    myChart8.destroy();
                }
                myChart8 = new Chart(ctx8, {
                    type: 'pie',
                    data: {
                        labels: Labels,

                        datasets: [{
                            backgroundColor: Colors,
                            data: Values,
                            datalabels: {
                                anchor: 'end'
                            }
                        }]
                    },
                    // Configuration options go here
                    options: {
                        responsive: true,
                        legend: {
                            position: 'bottom',

                        },
                        tooltips: {
                            enabled: true
                        },
                        animation: {
                            animateScale: true,
                            animateRotate: true
                        },
                        plugins: {
                            datalabels: {
                                backgroundColor: function (context) {
                                    return context.dataset.backgroundColor;
                                },
                                borderColor: 'white',
                                borderRadius: 100,
                                borderWidth: 2,
                                color: 'white',
                                font: {
                                    weight: 'bold',
                                    size: 18
                                },
                                padding: 2,
                                align: 'start',

                                //formatter: Math.round
                            }
                        }
                        ,
                        onClick: function (event) {
                            var activePoints = myChart8.getElementsAtEvent(event);
                            var chartData = activePoints[0]['_chart'].config.data;
                            var idx = activePoints[0]['_index'];
                            var label = chartData.labels[idx];
                            DashboardPiChartTableBind(dateFrom, dateTo, "GenderofComplainant", label);
                        },
                        animation: {
                            onComplete: function () {
                                var url_base64 = document.getElementById("genderOfComplainantChart").toDataURL("image/png");
                                ChartBase64Image(url_base64, "Gender of Complainant");
                                this.options.animation.onComplete = null;
                            }
                        }
                    }
                });
            }
            else {
                $('#divGenderOfComplainantChart').css('display', 'none');
            }
            //Gender of Respondent Wise Pi Chart
            if (result.genderofRespondent != null) {
                var Labels = [];
                var Values = [];
                var Colors = [];
                $.each(result.genderofComplainant, function (i) {
                    Labels.push(result.genderofComplainant[i].Label);
                    Values.push(result.genderofComplainant[i].Value);
                    Colors.push(getRandomColorHex());
                });
                var ctx9 = document.getElementById("genderOfRespondentChart").getContext('2d');
                if (myChart9 != null) {
                    myChart9.destroy();
                }
                myChart9 = new Chart(ctx9, {
                    type: 'pie',
                    data: {
                        labels: Labels,

                        datasets: [{
                            backgroundColor: Colors,
                            data: Values,
                            datalabels: {
                                anchor: 'end'
                            }
                        }]
                    },
                    // Configuration options go here
                    options: {
                        responsive: true,
                        legend: {
                            position: 'bottom',

                        },
                        tooltips: {
                            enabled: true
                        },
                        animation: {
                            animateScale: true,
                            animateRotate: true
                        },
                        plugins: {
                            datalabels: {
                                backgroundColor: function (context) {
                                    return context.dataset.backgroundColor;
                                },
                                borderColor: 'white',
                                borderRadius: 100,
                                borderWidth: 2,
                                color: 'white',
                                font: {
                                    weight: 'bold',
                                    size: 18
                                },
                                padding: 2,
                                align: 'start',

                                //formatter: Math.round
                            }
                        }
                        ,
                        onClick: function (event) {
                            var activePoints = myChart9.getElementsAtEvent(event);
                            var chartData = activePoints[0]['_chart'].config.data;
                            var idx = activePoints[0]['_index'];
                            var label = chartData.labels[idx];
                            DashboardPiChartTableBind(dateFrom, dateTo, "GenderofRespondent", label);
                        },
                        animation: {
                            onComplete: function () {
                                var url_base64 = document.getElementById("genderOfRespondentChart").toDataURL("image/png");
                                ChartBase64Image(url_base64, "Gender of Respondent");
                                this.options.animation.onComplete = null;
                            }
                        }
                    }
                });
            }
            else {
                $('#divGenderOfRespondentChart').css('display', 'none');
            }
            //Designation of Complainant Wise Pi Chart
            if (result.designationofComplainant != null) {
                var Labels = [];
                var Values = [];
                var Colors = [];
                $.each(result.designationofComplainant, function (i) {
                    Labels.push(result.designationofComplainant[i].Label);
                    Values.push(result.designationofComplainant[i].Value);
                    Colors.push(getRandomColorHex());
                });
                var ctx10 = document.getElementById("designationOfComplainantChart").getContext('2d');
                if (myChart10 != null) {
                    myChart10.destroy();
                }
                myChart10 = new Chart(ctx10, {
                    type: 'pie',
                    data: {
                        labels: Labels,

                        datasets: [{
                            backgroundColor: Colors,
                            data: Values,
                            datalabels: {
                                anchor: 'end'
                            }
                        }]
                    },
                    // Configuration options go here
                    options: {
                        responsive: true,
                        legend: {
                            position: 'bottom',

                        },
                        tooltips: {
                            enabled: true
                        },
                        animation: {
                            animateScale: true,
                            animateRotate: true
                        },
                        plugins: {
                            datalabels: {
                                backgroundColor: function (context) {
                                    return context.dataset.backgroundColor;
                                },
                                borderColor: 'white',
                                borderRadius: 100,
                                borderWidth: 2,
                                color: 'white',
                                font: {
                                    weight: 'bold',
                                    size: 18
                                },
                                padding: 2,
                                align: 'start',

                                //formatter: Math.round
                            }
                        }
                        ,
                        onClick: function (event) {
                            var activePoints = myChart10.getElementsAtEvent(event);
                            var chartData = activePoints[0]['_chart'].config.data;
                            var idx = activePoints[0]['_index'];
                            var label = chartData.labels[idx];
                            DashboardPiChartTableBind(dateFrom, dateTo, "DesignationofComplainant", label);
                        },
                        animation: {
                            onComplete: function () {
                                var url_base64 = document.getElementById("designationOfComplainantChart").toDataURL("image/png");
                                ChartBase64Image(url_base64, "Designation of Complainant");
                                this.options.animation.onComplete = null;
                            }
                        }
                    }
                });
            }
            else {
                $('#divDesignationOfComplainantChart').css('display', 'none');
            }
            //Designation of Respondent Wise Pi Chart
            if (result.designationofRespondent != null) {
                var Labels = [];
                var Values = [];
                var Colors = [];
                $.each(result.designationofRespondent, function (i) {
                    Labels.push(result.designationofRespondent[i].Label);
                    Values.push(result.designationofRespondent[i].Value);
                    Colors.push(getRandomColorHex());
                });
                var ctx11 = document.getElementById("designationOfRespondentChart").getContext('2d');
                if (myChart11 != null) {
                    myChart11.destroy();
                }
                myChart11 = new Chart(ctx11, {
                    type: 'pie',
                    data: {
                        labels: Labels,

                        datasets: [{
                            backgroundColor: Colors,
                            data: Values,
                            datalabels: {
                                anchor: 'end'
                            }
                        }]
                    },
                    // Configuration options go here
                    options: {
                        responsive: true,
                        legend: {
                            position: 'bottom',

                        },
                        tooltips: {
                            enabled: true
                        },
                        animation: {
                            animateScale: true,
                            animateRotate: true
                        },
                        plugins: {
                            datalabels: {
                                backgroundColor: function (context) {
                                    return context.dataset.backgroundColor;
                                },
                                borderColor: 'white',
                                borderRadius: 100,
                                borderWidth: 2,
                                color: 'white',
                                font: {
                                    weight: 'bold',
                                    size: 18
                                },
                                padding: 2,
                                align: 'start',

                                //formatter: Math.round
                            }
                        }
                        ,
                        onClick: function (event) {
                            var activePoints = myChart11.getElementsAtEvent(event);
                            var chartData = activePoints[0]['_chart'].config.data;
                            var idx = activePoints[0]['_index'];
                            var label = chartData.labels[idx];
                            DashboardPiChartTableBind(dateFrom, dateTo, "DesignationofRespondent", label);
                        },
                        animation: {
                            onComplete: function () {
                                var url_base64 = document.getElementById("designationOfRespondentChart").toDataURL("image/png");
                                ChartBase64Image(url_base64, "Designation of Respondent");
                                this.options.animation.onComplete = null;
                            }
                        }
                    }
                });
            }
            else {
                $('#divDesignationOfRespondentChart').css('display', 'none');
            }
            //Mode of Complaint Wise Pi Chart
            if (result.modeofComplaint != null) {
                var Labels = [];
                var Values = [];
                var Colors = [];
                $.each(result.modeofComplaint, function (i) {
                    Labels.push(result.modeofComplaint[i].Label);
                    Values.push(result.modeofComplaint[i].Value);
                    Colors.push(getRandomColorHex());
                });
                var ctx12 = document.getElementById("modeofComplaintChart").getContext('2d');
                if (myChart12 != null) {
                    myChart12.destroy();
                }
                myChart12 = new Chart(ctx12, {
                    type: 'pie',
                    data: {
                        labels: Labels,
                        // id: Labels,
                        datasets: [{
                            backgroundColor: Colors,
                            data: Values,
                            datalabels: {
                                anchor: 'end'
                            }
                        }]
                    },
                    // Configuration options go here
                    options: {
                        responsive: true,
                        legend: {
                            position: 'bottom',

                        },
                        tooltips: {
                            enabled: true
                        },
                        animation: {
                            animateScale: true,
                            animateRotate: true
                        },
                        plugins: {
                            datalabels: {
                                backgroundColor: function (context) {
                                    return context.dataset.backgroundColor;
                                },
                                borderColor: 'white',
                                borderRadius: 100,
                                borderWidth: 2,
                                color: 'white',
                                font: {
                                    weight: 'bold',
                                    size: 18
                                },
                                padding: 2,
                                align: 'start',

                                //formatter: Math.round
                            }
                        }
                        ,
                        onClick: function (event) {
                            var activePoints = myChart12.getElementsAtEvent(event);
                            var chartData = activePoints[0]['_chart'].config.data;
                            var idx = activePoints[0]['_index'];
                            var label = chartData.labels[idx];
                            //var id = chartData.id[idx];
                            //alert(id);
                            DashboardPiChartTableBind(dateFrom, dateTo, "ModeofComplaint", label);
                        },
                        animation: {
                            onComplete: function () {
                                var url_base64 = document.getElementById("modeofComplaintChart").toDataURL("image/png");
                                ChartBase64Image(url_base64, "Mode of Complaint");
                                this.options.animation.onComplete = null;
                            }
                        }
                    }
                });
            }
            else {
                $('#divModeofComplaintChart').css('display', 'none');
            }
            //Ageing/Case Closure Wise Pi Chart
            if (result.categoryPiCharts != null) {
                var Labels = [];
                var Values = [];
                var Colors = [];
                $.each(result.ageingPiBarCharts, function (i) {
                    Labels.push(result.ageingPiBarCharts[i].Label);
                    Values.push(result.ageingPiBarCharts[i].Value1);
                    Colors.push(getRandomColorHex());
                });
                var ctx13 = document.getElementById("ageingChart").getContext('2d');
                if (myChart13 != null) {
                    myChart13.destroy();
                }
                myChart13 = new Chart(ctx13, {
                    type: 'pie',

                    data: {
                        labels: Labels,
                        datasets: [{
                            backgroundColor: Colors,
                            data: Values,
                            datalabels: {
                                anchor: 'end'
                            }
                        }]
                    },
                    // Configuration options go here
                    options: {
                        //responsive: true,
                        //cutoutPercentage: 50,
                        legend: {
                            position: 'bottom',

                        },
                        tooltips: {
                            enabled: true
                        },
                        animation: {
                            animateScale: true,
                            animateRotate: true
                        },
                        plugins: {
                            datalabels: {
                                backgroundColor: function (context) {
                                    return context.dataset.backgroundColor;
                                },
                                borderColor: 'white',
                                borderRadius: 100,
                                borderWidth: 2,
                                color: 'white',
                                font: {
                                    weight: 'bold',
                                    size:18
                                },
                                padding: 2,
                                align:'start',

                                //formatter: Math.round
                            }
                        }
                        ,
                        onClick: function (event) {
                            var activePoints = myChart13.getElementsAtEvent(event);
                            var chartData = activePoints[0]['_chart'].config.data;
                            var idx = activePoints[0]['_index'];
                            var label = chartData.labels[idx];
                            DashboardPiChartTableBind(dateFrom, dateTo, "Ageing", label);
                        },
                        animation: {
                            onComplete: function () {
                                var url_base64 = document.getElementById("ageingChart").toDataURL("image/png");
                                ChartBase64Image(url_base64, "Ageing/Case Closure");
                                this.options.animation.onComplete = null;
                            }
                        }
                    }

                });
            }
            else {
                $('#divAgeingChart').css('display', 'none');
            }
        }


    });
}

function DashboardBarGraphData() {
    Base64Image = [];
    StartProcess();
    let dateFrom = ($('#txt_dateFrom').val());
    let dateTo = ($('#txt_dateTo').val());

    var yearCurrent = new Date();
    yearCurrent = yearCurrent.getFullYear();
    let yearBack = new Date();
    yearBack = (yearBack.getFullYear() - 1);


    $.get('/home/DashboardBarChart', { dateFrom: dateFrom, dateTo: dateTo }, function (result) {
        StopProcess();
        if (result != null && result != "undefined" && result != "") {
            //Case Type Bar chart
            if (result.caseTypeofComplaint1 != null) {
                var newArr;
                var Labels = []; var Values1 = []; var Values2 = []; var Years = []; var Colors = [];
                $.each(result.caseTypeofComplaint1, function (i) {

                    Labels.push(result.caseTypeofComplaint1[i].Label);
                    Values1.push(result.caseTypeofComplaint1[i].Value1);
                    Values2.push(result.caseTypeofComplaint1[i].Value2);
                    Years.push(result.caseTypeofComplaint1[i].Year);
                    Colors.push(getRandomColorHex());
                });
                var ctx = document.getElementById("caseTypeChart").getContext('2d');
                if (myChart != null) {
                    myChart.destroy();
                }
                myChart = new Chart(ctx, {
                    type: 'bar',
                    data: {
                        labels: Years,
                        datasets: [{
                            label: "Actionable",
                            backgroundColor: "rgb(54, 162, 235)",
                            data: Values1,
                            datalabels: {
                                anchor: 'center'
                            }
                        },
                        {
                            label: "NonActionable",
                            backgroundColor: "rgb(255, 133, 102)",
                            data: Values2,
                            datalabels: {
                                anchor: 'center'
                            }
                        }]
                    },
                    options: {
                        scales: {
                            yAxes: [{
                                ticks: {
                                    beginAtZero: true
                                }
                            }]
                        },
                        legend: {
                            position: 'bottom',
                        },
                        tooltips: {
                            enabled: true
                        },
                        plugins: {
                            datalabels: {
                                color: '#fff',
                            }
                        },
                        onClick:
                            function (event, array) {
                                var active = window.myChart.getElementAtEvent(event);
                                var elementIndex = active[0]._datasetIndex;
                                var chartData = array[elementIndex]['_chart'].data;
                                var idx = array[elementIndex]['_index'];
                                var year = chartData.labels[idx];
                                var label = chartData.datasets[elementIndex].label;
                                DashboardBarChartTableBind(dateFrom, dateTo, "CaseType", label, year);
                            },

                        animation: {
                            onComplete: function () {
                                var url_base64 = document.getElementById("caseTypeChart").toDataURL("image/png");
                                ChartBase64Image(url_base64, "Case Type");
                                this.options.animation.onComplete = null;
                            }
                        },
                    }
                });
            }
            else {
                $('#divCaseTypeChart').css('display', 'none');
            }
            //Category Wise Bar Chart
            if (result.categoryPiBarCharts != null) {
                var Years = []; var arrData = [];
                $.each(result.categoryPiBarCharts, function (i) {
                    if (Years.indexOf(result.categoryPiBarCharts[i].Year) === -1) {
                        Years.push(result.categoryPiBarCharts[i].Year);
                    }
                    arrData.push({
                        label: result.categoryPiBarCharts[i].Label, backgroundColor: getRandomColorHex(),
                        data: [result.categoryPiBarCharts[i].Value2, result.categoryPiBarCharts[i].Value1], datalabels: { anchor: 'center' }
                    })
                });
                var ctx1 = document.getElementById("categoryChart").getContext('2d');
                if (myChart1 != null) {
                    myChart1.destroy();
                }
                myChart1 = new Chart(ctx1, {
                    type: 'bar',
                    data: {
                        labels: [yearBack, yearCurrent],
                        datasets: arrData
                    },
                    // Configuration options go here
                    options: {
                        scales: {
                            yAxes: [{
                                ticks: {
                                    beginAtZero: true
                                }
                            }]
                        },
                        legend: {
                            "position": "bottom"

                        },
                        tooltips: {
                            enabled: true
                        },
                        plugins: {
                            datalabels: {
                                color: '#fff',
                                font: {
                                    weight: 'bold'
                                },
                            }
                        },
                        onClick:
                            function (event, array) {
                                var active = window.myChart1.getElementAtEvent(event);
                                var elementIndex = active[0]._datasetIndex;
                                var chartData = array[elementIndex]['_chart'].data;
                                var idx = array[elementIndex]['_index'];
                                var year = chartData.labels[idx];
                                var label = chartData.datasets[elementIndex].label;
                                DashboardBarChartTableBind(dateFrom, dateTo, "Category", label, year);
                            },
                        animation: {
                            onComplete: function () {
                                var url_base64 = document.getElementById("categoryChart").toDataURL("image/png");
                                ChartBase64Image(url_base64, "Category wise");
                                this.options.animation.onComplete = null;
                            }
                        },
                    }

                });
            }
            else {
                $('#divCategoryChart').css('display', 'none');
            }
            //Sub-Category Wise Bar Chart
            if (result.subCategoryPiBarCharts != null) {
                var Years = []; var arrData = [];
                $.each(result.subCategoryPiBarCharts, function (i) {
                    if (Years.indexOf(result.subCategoryPiBarCharts[i].Year) === -1) {
                        Years.push(result.subCategoryPiBarCharts[i].Year);
                    }
                    arrData.push({
                        label: result.subCategoryPiBarCharts[i].Label, backgroundColor: getRandomColorHex(), data: [result.subCategoryPiBarCharts[i].Value2, result.subCategoryPiBarCharts[i].Value1], datalabels: { anchor: 'center' }
                    })
                });
                var ctx2 = document.getElementById("subCategoryChart").getContext('2d');
                if (myChart2 != null) {
                    myChart2.destroy();
                }
                myChart2 = new Chart(ctx2, {
                    type: 'bar',
                    data: {
                        labels: [yearBack, yearCurrent],
                        datasets: arrData
                    },
                    // Configuration options go here
                    options: {
                        scales: {
                            yAxes: [{
                                ticks: {
                                    beginAtZero: true
                                }
                            }]
                        },
                        legend: {
                            position: 'bottom',

                        },
                        tooltips: {
                            enabled: true
                        },
                        plugins: {
                            datalabels: {
                                color: '#fff',

                            }
                        },
                        onClick: function (event, array) {
                            var active = window.myChart2.getElementAtEvent(event);
                            var elementIndex = active[0]._datasetIndex;
                            var chartData = array[elementIndex]['_chart'].data;
                            var idx = array[elementIndex]['_index'];
                            var year = chartData.labels[idx];
                            var label = chartData.datasets[elementIndex].label;
                            DashboardBarChartTableBind(dateFrom, dateTo, "SubCategory", label, year);
                        },
                        animation: {
                            onComplete: function () {
                                var url_base64 = document.getElementById("subCategoryChart").toDataURL("image/png");
                                ChartBase64Image(url_base64, "Sub-Category wise");
                                this.options.animation.onComplete = null;
                            }
                        },
                    }
                });
            }
            else {
                $('#divSubCategoryChart').css('display', 'none');
            }
            //Region Wise Bar Chart
            if (result.regionPiBarCharts != null) {
                var Years = []; var arrData = [];
                $.each(result.regionPiBarCharts, function (i) {
                    if (Years.indexOf(result.regionPiBarCharts[i].Year) === -1) {
                        Years.push(result.regionPiBarCharts[i].Year);
                    }
                    arrData.push({
                        label: result.regionPiBarCharts[i].Label, backgroundColor: getRandomColorHex(), data: [result.regionPiBarCharts[i].Value2, result.regionPiBarCharts[i].Value1], datalabels: { anchor: 'center' }
                    })
                });
                var ctx3 = document.getElementById("regionChart").getContext('2d');
                if (myChart3 != null) {
                    myChart3.destroy();
                }
                myChart3 = new Chart(ctx3, {
                    type: 'bar',
                    data: {
                        labels: [yearBack, yearCurrent],
                        datasets: arrData
                    },
                    // Configuration options go here
                    options: {
                        scales: {
                            yAxes: [{
                                ticks: {
                                    beginAtZero: true
                                }
                            }]
                        },
                        legend: {
                            position: 'bottom',

                        },
                        tooltips: {
                            enabled: true
                        },
                        plugins: {
                            datalabels: {
                                color: '#fff',

                            }
                        },
                        onClick: function (event, array) {
                            var active = window.myChart3.getElementAtEvent(event);
                            var elementIndex = active[0]._datasetIndex;
                            var chartData = array[elementIndex]['_chart'].data;
                            var idx = array[elementIndex]['_index'];
                            var year = chartData.labels[idx];
                            var label = chartData.datasets[elementIndex].label;
                            DashboardBarChartTableBind(dateFrom, dateTo, "Region", label, year);
                        },
                        animation: {
                            onComplete: function () {
                                var url_base64 = document.getElementById("regionChart").toDataURL("image/png");
                                ChartBase64Image(url_base64, "Region wise");
                                this.options.animation.onComplete = null;
                            }
                        },
                    }
                });
            }
            else {
                $('#regionChart').css('display', 'none');
            }
            //Office Wise Bar Chart
            if (result.officePiBarCharts != null) {
                var Years = []; var arrData = [];
                $.each(result.officePiBarCharts, function (i) {
                    if (Years.indexOf(result.officePiBarCharts[i].Year) === -1) {
                        Years.push(result.officePiBarCharts[i].Year);
                    }
                    arrData.push({
                        label: result.officePiBarCharts[i].Label, backgroundColor: getRandomColorHex(), data: [result.officePiBarCharts[i].Value2, result.officePiBarCharts[i].Value1], datalabels: { anchor: 'center' }
                    })
                });
                var ctx4 = document.getElementById("officeChart").getContext('2d');
                if (myChart4 != null) {
                    myChart4.destroy();
                }
                myChart4 = new Chart(ctx4, {
                    type: 'bar',
                    data: {
                        labels: [yearBack, yearCurrent],
                        datasets: arrData
                    },
                    // Configuration options go here
                    options: {
                        scales: {
                            yAxes: [{
                                ticks: {
                                    beginAtZero: true
                                }
                            }]
                        },
                        legend: {
                            position: 'bottom',

                        },
                        tooltips: {
                            enabled: true
                        },

                        plugins: {
                            datalabels: {
                                color: '#fff',
                            }
                        },
                        onClick: function (event, array) {
                            var active = window.myChart4.getElementAtEvent(event);
                            var elementIndex = active[0]._datasetIndex;
                            var chartData = array[elementIndex]['_chart'].data;
                            var idx = array[elementIndex]['_index'];
                            var year = chartData.labels[idx];
                            var label = chartData.datasets[elementIndex].label;
                            DashboardBarChartTableBind(dateFrom, dateTo, "Office", label, year);
                        },
                        animation: {
                            onComplete: function () {
                                var url_base64 = document.getElementById("officeChart").toDataURL("image/png");
                                ChartBase64Image(url_base64, "Office wise");
                                this.options.animation.onComplete = null;
                            }
                        },
                    }
                });
            }
            else {
                $('#officeChart').css('display', 'none');
            }
            //LOS Wise Pi Chart
            if (result.losPiBarCharts != null) {
                var Years = []; var arrData = [];
                $.each(result.losPiBarCharts, function (i) {
                    if (Years.indexOf(result.losPiBarCharts[i].Year) === -1) {
                        Years.push(result.losPiBarCharts[i].Year);
                    }
                    arrData.push({
                        label: result.losPiBarCharts[i].Label, backgroundColor: getRandomColorHex(), data: [result.losPiBarCharts[i].Value2, result.losPiBarCharts[i].Value1], datalabels: { anchor: 'center' }
                    })
                });
                var ctx5 = document.getElementById("losChart").getContext('2d');
                if (myChart5 != null) {
                    myChart5.destroy();
                }
                myChart5 = new Chart(ctx5, {
                    type: 'bar',
                    data: {
                        labels: [yearBack, yearCurrent],
                        datasets: arrData
                    },
                    // Configuration options go here
                    options: {
                        scales: {
                            yAxes: [{
                                ticks: {
                                    beginAtZero: true
                                }
                            }]
                        },
                        legend: {
                            position: 'bottom',

                        },
                        tooltips: {
                            enabled: true
                        },
                        plugins: {
                            datalabels: {
                                color: '#fff',

                            }
                        },
                        onClick:
                            function (event, array) {
                                var active = window.myChart5.getElementAtEvent(event);
                                var elementIndex = active[0]._datasetIndex;
                                var chartData = array[elementIndex]['_chart'].data;
                                var idx = array[elementIndex]['_index'];
                                var year = chartData.labels[idx];
                                var label = chartData.datasets[elementIndex].label;
                                DashboardBarChartTableBind(dateFrom, dateTo, "LOS", label, year);
                            },
                        animation: {
                            onComplete: function () {
                                var url_base64 = document.getElementById("losChart").toDataURL("image/png");
                                ChartBase64Image(url_base64, "LOS wise");
                                this.options.animation.onComplete = null;
                            }
                        },
                    }
                });
            }
            else {
                $('#divLosChart').css('display', 'none');
            }
            //SBU Wise Pi Chart
            if (result.sbuPiBarCharts != null) {
                var Years = []; var arrData = [];
                $.each(result.sbuPiBarCharts, function (i) {
                    if (Years.indexOf(result.sbuPiBarCharts[i].Year) === -1) {
                        Years.push(result.sbuPiBarCharts[i].Year);
                    }
                    arrData.push({
                        label: result.sbuPiBarCharts[i].Label, backgroundColor: getRandomColorHex(), data: [result.sbuPiBarCharts[i].Value2, result.sbuPiBarCharts[i].Value1], datalabels: { anchor: 'center' }
                    })
                });
                var ctx6 = document.getElementById("sbuChart").getContext('2d');
                if (myChart6 != null) {
                    myChart6.destroy();
                }
                myChart6 = new Chart(ctx6, {
                    type: 'bar',
                    data: {
                        labels: [yearBack, yearCurrent],
                        datasets: arrData
                    },
                    // Configuration options go here
                    options: {
                        scales: {
                            yAxes: [{
                                ticks: {
                                    beginAtZero: true
                                }
                            }]
                        },
                        legend: {
                            position: 'bottom',

                        },
                        tooltips: {
                            enabled: true
                        },
                        plugins: {
                            datalabels: {
                                color: '#fff',
                            }
                        },
                        onClick:
                            function (event, array) {
                                var active = window.myChart6.getElementAtEvent(event);
                                var elementIndex = active[0]._datasetIndex;
                                var chartData = array[elementIndex]['_chart'].data;
                                var idx = array[elementIndex]['_index'];
                                var year = chartData.labels[idx];
                                var label = chartData.datasets[elementIndex].label;
                                DashboardBarChartTableBind(dateFrom, dateTo, "SBU", label, year);
                            },
                        animation: {
                            onComplete: function () {
                                var url_base64 = document.getElementById("sbuChart").toDataURL("image/png");
                                ChartBase64Image(url_base64, "SBU wise");
                                this.options.animation.onComplete = null;
                            }
                        },
                    }
                });
            }
            else {
                $('#divSbuChart').css('display', 'none');
            }
            //Sub-SBU Wise Bar Chart
            if (result.subSBUPiBarCharts != null) {
                var Years = []; var arrData = [];
                $.each(result.subSBUPiBarCharts, function (i) {
                    if (Years.indexOf(result.subSBUPiBarCharts[i].Year) === -1) {
                        Years.push(result.subSBUPiBarCharts[i].Year);
                    }
                    arrData.push({
                        label: result.subSBUPiBarCharts[i].Label, backgroundColor: getRandomColorHex(), data: [result.subSBUPiBarCharts[i].Value2, result.subSBUPiBarCharts[i].Value1], datalabels: { anchor: 'center' }
                    })
                });
                var ctx7 = document.getElementById("subSBUChart").getContext('2d');
                if (myChart7 != null) {
                    myChart7.destroy();
                }
                myChart7 = new Chart(ctx7, {
                    type: 'bar',
                    data: {
                        labels: [yearBack, yearCurrent],
                        datasets: arrData
                    },
                    // Configuration options go here
                    options: {
                        scales: {
                            yAxes: [{
                                ticks: {
                                    beginAtZero: true
                                }
                            }]
                        },
                        legend: {
                            position: 'bottom',

                        },
                        tooltips: {
                            enabled: true
                        },
                        plugins: {
                            datalabels: {
                                color: '#fff',
                            }
                        },
                        onClick:
                            function (event, array) {
                                var active = window.myChart7.getElementAtEvent(event);
                                var elementIndex = active[0]._datasetIndex;
                                var chartData = array[elementIndex]['_chart'].data;
                                var idx = array[elementIndex]['_index'];
                                var year = chartData.labels[idx];
                                var label = chartData.datasets[elementIndex].label;
                                DashboardBarChartTableBind(dateFrom, dateTo, "SubSBU", label, year);
                            },
                        animation: {
                            onComplete: function () {
                                var url_base64 = document.getElementById("subSBUChart").toDataURL("image/png");
                                ChartBase64Image(url_base64, "Sub-SBU wise");
                                this.options.animation.onComplete = null;
                            }
                        },
                    }
                });
            }
            else {
                $('#divSubSBUChart').css('display', 'none');
            }
            //Gender of Complainant Wise Bar Chart
            if (result.genderofComplainant != null) {
                var Years = []; var arrData = [];
                $.each(result.genderofComplainantPiBarCharts, function (i) {
                    if (Years.indexOf(result.genderofComplainantPiBarCharts[i].Year) === -1) {
                        Years.push(result.genderofComplainantPiBarCharts[i].Year);
                    }
                    arrData.push({
                        label: result.genderofComplainantPiBarCharts[i].Label, backgroundColor: getRandomColorHex(), data: [result.genderofComplainantPiBarCharts[i].Value2, result.genderofComplainantPiBarCharts[i].Value1], datalabels: { anchor: 'center' }
                    })
                });
                var ctx8 = document.getElementById("genderOfComplainantChart").getContext('2d');
                if (myChart8 != null) {
                    myChart8.destroy();
                }
                myChart8 = new Chart(ctx8, {
                    type: 'bar',
                    data: {
                        labels: [yearBack, yearCurrent],
                        datasets: arrData
                    },
                    // Configuration options go here
                    options: {
                        scales: {
                            yAxes: [{
                                ticks: {
                                    beginAtZero: true
                                }
                            }]
                        },
                        legend: {
                            position: 'bottom',

                        },
                        tooltips: {
                            enabled: true
                        },
                        plugins: {
                            datalabels: {
                                color: '#fff',
                            }
                        },
                        onClick:
                            function (event, array) {
                                var active = window.myChart8.getElementAtEvent(event);
                                var elementIndex = active[0]._datasetIndex;
                                var chartData = array[elementIndex]['_chart'].data;
                                var idx = array[elementIndex]['_index'];
                                var year = chartData.labels[idx];
                                var label = chartData.datasets[elementIndex].label;
                                DashboardBarChartTableBind(dateFrom, dateTo, "GenderofComplainant", label, year);
                            },
                        animation: {
                            onComplete: function () {
                                var url_base64 = document.getElementById("genderOfComplainantChart").toDataURL("image/png");
                                ChartBase64Image(url_base64, "Gender of Complainant");
                                this.options.animation.onComplete = null;
                            }
                        },
                    }
                });
            }
            else {
                $('#divGenderOfComplainantChart').css('display', 'none');
            }
            //Gender of Respondent Wise Bar Chart
            if (result.genderofRespondentPiBarCharts != null) {
                var Years = []; var arrData = [];
                $.each(result.genderofRespondentPiBarCharts, function (i) {
                    if (Years.indexOf(result.genderofRespondentPiBarCharts[i].Year) === -1) {
                        Years.push(result.genderofRespondentPiBarCharts[i].Year);
                    }
                    arrData.push({
                        label: result.genderofRespondentPiBarCharts[i].Label, backgroundColor: getRandomColorHex(), data: [result.genderofRespondentPiBarCharts[i].Value2, result.genderofRespondentPiBarCharts[i].Value1], datalabels: { anchor: 'center' }
                    })
                });
                var ctx9 = document.getElementById("genderOfRespondentChart").getContext('2d');
                if (myChart9 != null) {
                    myChart9.destroy();
                }
                myChart9 = new Chart(ctx9, {
                    type: 'bar',
                    data: {
                        labels: [yearBack, yearCurrent],
                        datasets: arrData
                    },
                    // Configuration options go here
                    options: {
                        scales: {
                            yAxes: [{
                                ticks: {
                                    beginAtZero: true
                                }
                            }]
                        },
                        legend: {
                            position: 'bottom',

                        },
                        tooltips: {
                            enabled: true
                        },
                        plugins: {
                            datalabels: {
                                color: '#fff',
                                font: {
                                    weight: 'bold'
                                },
                            }
                        },
                        onClick: function (event, array) {
                            var active = window.myChart9.getElementAtEvent(event);
                            var elementIndex = active[0]._datasetIndex;
                            var chartData = array[elementIndex]['_chart'].data;
                            var idx = array[elementIndex]['_index'];
                            var year = chartData.labels[idx];
                            var label = chartData.datasets[elementIndex].label;
                            DashboardBarChartTableBind(dateFrom, dateTo, "GenderofRespondent", label, year);
                        },
                        animation: {
                            onComplete: function () {
                                var url_base64 = document.getElementById("genderOfRespondentChart").toDataURL("image/png");
                                ChartBase64Image(url_base64, "Gender of Respondent");
                                this.options.animation.onComplete = null;
                            }
                        },
                    }
                });
            }
            else {
                $('#divGenderOfRespondentChart').css('display', 'none');
            }
            //Designation of Complainant Wise Bar Chart
            if (result.designationofComplainantPiBarCharts != null) {
                var Years = []; var arrData = [];
                $.each(result.designationofComplainantPiBarCharts, function (i) {
                    if (Years.indexOf(result.designationofComplainantPiBarCharts[i].Year) === -1) {
                        Years.push(result.designationofComplainantPiBarCharts[i].Year);
                    }
                    arrData.push({
                        label: result.designationofComplainantPiBarCharts[i].Label, backgroundColor: getRandomColorHex(), data: [result.designationofComplainantPiBarCharts[i].Value2, result.designationofComplainantPiBarCharts[i].Value1], datalabels: { anchor: 'center' }
                    })
                });
                var ctx10 = document.getElementById("designationOfComplainantChart").getContext('2d');
                if (myChart10 != null) {
                    myChart10.destroy();
                }
                myChart10 = new Chart(ctx10, {
                    type: 'bar',
                    data: {
                        labels: [yearBack, yearCurrent],
                        datasets: arrData
                    },
                    // Configuration options go here
                    options: {
                        scales: {
                            yAxes: [{
                                ticks: {
                                    beginAtZero: true
                                }
                            }]
                        },
                        legend: {
                            position: 'bottom',

                        },
                        tooltips: {
                            enabled: true
                        },
                        plugins: {
                            datalabels: {
                                color: '#fff'
                            }
                        },
                        onClick: function (event, array) {
                            var active = window.myChart10.getElementAtEvent(event);
                            var elementIndex = active[0]._datasetIndex;
                            var chartData = array[elementIndex]['_chart'].data;
                            var idx = array[elementIndex]['_index'];
                            var year = chartData.labels[idx];
                            var label = chartData.datasets[elementIndex].label;
                            DashboardBarChartTableBind(dateFrom, dateTo, "DesignationofComplainant", label, year);
                        },
                        animation: {
                            onComplete: function () {
                                var url_base64 = document.getElementById("designationOfComplainantChart").toDataURL("image/png");
                                ChartBase64Image(url_base64, "Designation of Complainant");
                                this.options.animation.onComplete = null;
                            }
                        },
                    }
                });
            }
            else {
                $('#divDesignationOfComplainantChart').css('display', 'none');
            }
            //Designation of Respondent Wise Bar Chart
            if (result.designationofRespondentPiBarCharts != null) {
                var Years = []; var arrData = [];
                $.each(result.designationofRespondentPiBarCharts, function (i) {
                    if (Years.indexOf(result.designationofRespondentPiBarCharts[i].Year) === -1) {
                        Years.push(result.designationofRespondentPiBarCharts[i].Year);
                    }
                    arrData.push({
                        label: result.designationofRespondentPiBarCharts[i].Label, backgroundColor: getRandomColorHex(), data: [result.designationofRespondentPiBarCharts[i].Value2, result.designationofRespondentPiBarCharts[i].Value1], datalabels: { anchor: 'center' }
                    })
                });
                var ctx11 = document.getElementById("designationOfRespondentChart").getContext('2d');
                if (myChart11 != null) {
                    myChart11.destroy();
                }
                myChart11 = new Chart(ctx11, {
                    type: 'bar',
                    data: {
                        labels: [yearBack, yearCurrent],
                        datasets: arrData
                    },
                    // Configuration options go here
                    options: {
                        scales: {
                            yAxes: [{
                                ticks: {
                                    beginAtZero: true
                                }
                            }]
                        },
                        legend: {
                            position: 'bottom',

                        },
                        tooltips: {
                            enabled: true
                        },
                        plugins: {
                            datalabels: {
                                color: '#fff',
                            }
                        }
                        ,

                        onClick: function (event, array) {
                            var active = window.myChart11.getElementAtEvent(event);
                            var elementIndex = active[0]._datasetIndex;
                            var chartData = array[elementIndex]['_chart'].data;
                            var idx = array[elementIndex]['_index'];
                            var year = chartData.labels[idx];
                            var label = chartData.datasets[elementIndex].label;
                            DashboardBarChartTableBind(dateFrom, dateTo, "DesignationofRespondent", label, year);
                        },
                        animation: {
                            onComplete: function () {
                                var url_base64 = document.getElementById("designationOfRespondentChart").toDataURL("image/png");
                                ChartBase64Image(url_base64, "Designation of Respondent");
                                this.options.animation.onComplete = null;
                            }
                        },
                    }
                });
            }
            else {
                $('#divDesignationOfRespondentChart').css('display', 'none');
            }
            //Designation of Respondent Wise Bar Chart
            if (result.modeofComplaintPiBarCharts != null) {
                var Years = []; var arrData = [];
                $.each(result.modeofComplaintPiBarCharts, function (i) {
                    if (Years.indexOf(result.modeofComplaintPiBarCharts[i].Year) === -1) {
                        Years.push(result.modeofComplaintPiBarCharts[i].Year);
                    }
                    arrData.push({
                        label: result.modeofComplaintPiBarCharts[i].Label, backgroundColor: getRandomColorHex(), data: [result.modeofComplaintPiBarCharts[i].Value2, result.modeofComplaintPiBarCharts[i].Value1], datalabels: { anchor: 'center' }
                    })
                });
                var ctx12 = document.getElementById("modeofComplaintChart").getContext('2d');
                if (myChart12 != null) {
                    myChart12.destroy();
                }
                myChart12 = new Chart(ctx12, {
                    type: 'bar',
                    data: {
                        labels: [yearBack, yearCurrent],
                        datasets: arrData
                    },
                    // Configuration options go here
                    options: {
                        scales: {
                            yAxes: [{
                                ticks: {
                                    beginAtZero: true
                                }
                            }]
                        },
                        legend: {
                            position: 'bottom',

                        },
                        tooltips: {
                            enabled: true
                        },
                        plugins: {
                            datalabels: {
                                color: '#fff'
                            }
                        },
                        onClick: function (event, array) {
                            var active = window.myChart12.getElementAtEvent(event);
                            var elementIndex = active[0]._datasetIndex;
                            var chartData = array[elementIndex]['_chart'].data;
                            var idx = array[elementIndex]['_index'];
                            var year = chartData.labels[idx];
                            var label = chartData.datasets[elementIndex].label;
                            DashboardBarChartTableBind(dateFrom, dateTo, "ModeofComplaint", label, year);
                        },
                        animation: {
                            onComplete: function () {
                                var url_base64 = document.getElementById("modeofComplaintChart").toDataURL("image/png");
                                ChartBase64Image(url_base64, "Mode of Complaint");
                                this.options.animation.onComplete = null;
                            }
                        },
                    }
                });
            }
            else {
                $('#divModeofComplaintChart').css('display', 'none');
            }
            //Ageing/Case Closure
            if (result.ageingPiBarCharts != null) {
                var Years = []; var arrData = [];
                $.each(result.ageingPiBarCharts, function (i) {
                    if (Years.indexOf(result.ageingPiBarCharts[i].Year) === -1) {
                        Years.push(result.ageingPiBarCharts[i].Year);
                    }
                    arrData.push({
                        label: result.ageingPiBarCharts[i].Label, backgroundColor: getRandomColorHex(), data: [result.ageingPiBarCharts[i].Value2, result.ageingPiBarCharts[i].Value1], datalabels: { anchor: 'center' }
                    })
                });
                var ctx13 = document.getElementById("ageingChart").getContext('2d');
                if (myChart13 != null) {
                    myChart13.destroy();
                }
                myChart13 = new Chart(ctx13, {
                    type: 'bar',
                    data: {
                        labels: [yearBack, yearCurrent],
                        datasets: arrData
                    },
                    // Configuration options go here
                    options: {
                        scales: {
                            yAxes: [{
                                ticks: {
                                    beginAtZero: true
                                }
                            }]
                        },
                        legend: {
                            position: 'bottom',

                        },
                        tooltips: {
                            enabled: true
                        },
                        plugins: {
                            datalabels: {
                                color: '#fff'
                            }
                        },
                        onClick: function (event, array) {
                            var active = window.myChart13.getElementAtEvent(event);
                            var elementIndex = active[0]._datasetIndex;
                            var chartData = array[elementIndex]['_chart'].data;
                            var idx = array[elementIndex]['_index'];
                            var year = chartData.labels[idx];
                            var label = chartData.datasets[elementIndex].label;
                            DashboardBarChartTableBind(dateFrom, dateTo, "Ageing", label, year);
                        },
                        animation: {
                            onComplete: function () {
                                var url_base64 = document.getElementById("ageingChart").toDataURL("image/png");
                                ChartBase64Image(url_base64, "Ageing/Case Closure");
                                this.options.animation.onComplete = null;
                            }
                        },
                    }
                });
            }
            else {
                $('#divAgeingChart').css('display', 'none');
            }
        }


    });
}

function DashboardpiePercent() {
    Base64Image = [];
    StartProcess();
    let dateFrom = ($('#txt_dateFrom').val());
    let dateTo = ($('#txt_dateTo').val());
    $.get('/home/DashboardPiChart', { dateFrom: dateFrom, dateTo: dateTo }, function (result) {
        StopProcess();
        if (result != null && result != "undefined" && result != "") {
            //Case Type Pi chart
            if (result.caseTypeofComplaint != null) {
                var sum = 0;
                var Labels = []; var Values = []; var Colors = []; var vale = [];
                $.each(result.caseTypeofComplaint, function (i) {
                    Labels.push(result.caseTypeofComplaint[i].Label);
                    vale.push(parseInt(result.caseTypeofComplaint[i].Value));
                    sum += result.caseTypeofComplaint[i].Value;

                    Colors.push(getRandomColorHex());
                });

                $.each(vale, function (i) {
                    Values.push(parseInt(vale[i] * 100 / sum));
                });

                var ctx = document.getElementById("caseTypeChart").getContext('2d');
                if (myChart != null) {
                    myChart.destroy();
                }
                myChart = new Chart(ctx, {
                    //position: 'right',
                    type: 'pie',
                    data: result,
                    data: {
                        labels: Labels,
                        type: "pie",
                        showInLegend: true,
                        toolTipContent: "<b>{name}</b>:-(#percent%)",
                        indexLabel: "{name} - (#percent%)",
                        legendText: "{name} - (#percent%)",
                        indexLabelPlacement: "inside",

                        datasets: [{
                            backgroundColor: Colors,
                            data: Values,
                            //datalabels: {
                            //    backgroundColor: function (context) {
                            //        return context.dataset.backgroundColor;
                            //    },
                            //    borderColor: 'white',
                            //    borderRadius: 100,
                            //    borderWidth: 2,
                            //    color: 'white',
                            //    font: {
                            //        weight: 'bold',
                            //        size: 18
                            //    },
                            //    padding: 2,
                            //    align: 'start',

                            //    //formatter: Math.round
                            //}

                            datalabels: {
                                anchor: 'end',
                                formatter: (value, ctx) => {
                                    let sum = 0;
                                    let dataArr = ctx.chart.data.datasets[0].data;
                                    dataArr.map(data => {
                                        sum += data;
                                    });
                                    let percentage = Math.floor(value * 100 / sum) + "%";
                                    return percentage;
                                },
                                font: {
                                    weight: 'bold',
                                    size: 18
                                },
                                padding: 2,
                                align: 'start',

                            }
                        }]
                    },
                    options: {
                        legend: {
                            position: 'bottom',
                        },
                        tooltips: {
                            enabled: true,
                            callbacks: {
                                label: function (tooltipItem, data) {
                                    return data['labels'][tooltipItem['index']] + ': ' + data['datasets'][0]['data'][tooltipItem['index']] + '%';
                                }
                            }
                        },
                        animation: {
                            animateScale: true,
                            animateRotate: true
                        },
                        plugins: {
                            datalabels: {
                                backgroundColor: function (context) {
                                    return context.dataset.backgroundColor;
                                },
                                borderColor: 'white',
                                borderRadius: 25,
                                borderWidth: 2,
                                color: 'white',
                                font: {
                                    weight: 'bold'
                                },
                            }
                        },
                        onClick: function (event) {
                            var activePoints = myChart.getElementsAtEvent(event);
                            var chartData = activePoints[0]['_chart'].config.data;
                            var idx = activePoints[0]['_index'];
                            var label = chartData.labels[idx];
                            DashboardPiChartTableBind(dateFrom, dateTo, "CaseType", label);
                        },
                        animation: {
                            onComplete: function () {
                                var url_base64 = document.getElementById("caseTypeChart").toDataURL("image/png");
                                ChartBase64Image(url_base64, "Case Type");
                                this.options.animation.onComplete = null;
                            }
                        },

                    }
                });
            }
            else {
                 $('#divCaseTypeChart').css('display', 'none');
            }
            //Category Wise Pi Chart
            if (result.categoryPiCharts != null) {
                var Labels = [];
                var Values = [];
                var Colors = [];
                var vale1 = [];
                var sum1 = 0;
                $.each(result.categoryPiCharts, function (i) {
                    Labels.push(result.categoryPiCharts[i].Label);
                    //Values.push(result.categoryPiCharts[i].Value);
                    vale1.push(parseInt(result.categoryPiCharts[i].Value));
                    sum1 += parseInt(result.categoryPiCharts[i].Value);

                    Colors.push(getRandomColorHex());
                });

                $.each(vale1, function (i) {
                    Values.push(parseInt(vale1[i] * 100 / sum1));
                });


                var ctx1 = document.getElementById("categoryChart").getContext('2d');
                if (myChart1 != null) {
                    myChart1.destroy();
                }
                myChart1 = new Chart(ctx1, {
                    //position: 'right',
                    type: 'pie',
                    data: {
                        labels: Labels,
                        type: "pie",
                        showInLegend: true,
                        toolTipContent: "<b>{name}</b>: ${y} (#percent%)",
                        indexLabel: "{name}",
                        legendText: "{name} (#percent%)",
                        indexLabelPlacement: "inside",
                        datasets: [{
                            backgroundColor: Colors,
                            data: Values,
                            datalabels: {
                                anchor: 'end',
                                formatter: (value, ctx) => {
                                    let sum = 0;
                                    let dataArr = ctx.chart.data.datasets[0].data;
                                    dataArr.map(data => {
                                        sum += data;
                                    });
                                    let percentage = Math.floor(value * 100 / sum) + "%";
                                    return percentage;
                                },
                                font: {
                                    weight: 'bold',
                                    size: 18
                                },
                                padding: 2,
                                align: 'start',
                            }
                        }]
                    },
                    // Configuration options go here
                    options: {
                        //responsive: true,
                        legend: {
                            position: 'bottom',

                        },
                        tooltips: {
                            enabled: true,
                            callbacks: {
                                label: function (tooltipItem, data) {
                                    return data['labels'][tooltipItem['index']] + ': ' + data['datasets'][0]['data'][tooltipItem['index']] + '%';
                                }
                            }
                        },
                        animation: {
                            animateScale: true,
                            animateRotate: true
                        },
                        plugins: {
                            datalabels: {
                                backgroundColor: function (context) {
                                    return context.dataset.backgroundColor;
                                },
                                borderColor: 'white',
                                borderRadius: 50,
                                borderWidth: 2,
                                color: 'white',
                                font: {
                                    weight: 'bold'
                                },

                                //formatter: Math.round
                            }
                        },
                        onClick: function (event) {
                            var activePoints = myChart1.getElementsAtEvent(event);
                            var chartData = activePoints[0]['_chart'].config.data;
                            var idx = activePoints[0]['_index'];
                            var label = chartData.labels[idx];
                            DashboardPiChartTableBind(dateFrom, dateTo, "Category", label);
                        },
                        animation: {
                            onComplete: function () {
                                var url_base64 = document.getElementById("categoryChart").toDataURL("image/png");
                                ChartBase64Image(url_base64, "Category Wise");
                                this.options.animation.onComplete = null;
                            }
                        }
                    }

                });
            }
            else {
                $('#divCategoryChart').css('display', 'none');
            }
            //Sub-Category Wise Pi Chart
            if (result.subCategoryPiCharts != null) {
                var Labels = [];
                var Values = [];
                var Colors = [];
                var vale1 = [];
                var sum1 = 0;
                $.each(result.subCategoryPiCharts, function (i) {
                    Labels.push(result.subCategoryPiCharts[i].Label);
                    //Values.push(result.subCategoryPiCharts[i].Value);
                    vale1.push(parseInt(result.subCategoryPiCharts[i].Value));
                    sum1 += parseInt(result.subCategoryPiCharts[i].Value);

                    Colors.push(getRandomColorHex());
                });

                $.each(vale1, function (i) {
                    Values.push(parseInt(vale1[i] * 100 / sum1));
                });

                var ctx2 = document.getElementById("subCategoryChart").getContext('2d');
                if (myChart2 != null) {
                    myChart2.destroy();
                }
                myChart2 = new Chart(ctx2, {
                    type: 'pie',
                    data: {
                        labels: Labels,

                        datasets: [{
                            backgroundColor: Colors,
                            data: Values,
                            datalabels: {
                                anchor: 'end',
                                formatter: (value, ctx) => {
                                    let sum = 0;
                                    let dataArr = ctx.chart.data.datasets[0].data;
                                    dataArr.map(data => {
                                        sum += data;
                                    });
                                    let percentage = Math.floor(value * 100 / sum) + "%";
                                    return percentage;
                                },
                                font: {
                                    weight: 'bold',
                                    size: 18
                                },
                                padding: 2,
                                align: 'start',
                            }
                        }]
                    },
                    // Configuration options go here
                    options: {
                        responsive: true,
                        legend: {
                            position: 'bottom',

                        },
                        tooltips: {
                            enabled: true,
                            callbacks: {
                                label: function (tooltipItem, data) {
                                    return data['labels'][tooltipItem['index']] + ': ' + data['datasets'][0]['data'][tooltipItem['index']] + '%';
                                }
                            }
                        },
                        animation: {
                            animateScale: true,
                            animateRotate: true
                        },
                        plugins: {
                            datalabels: {
                                backgroundColor: function (context) {
                                    return context.dataset.backgroundColor;
                                },
                                borderColor: 'white',
                                borderRadius: 25,
                                borderWidth: 2,
                                color: 'white',
                                font: {
                                    weight: 'bold'
                                },
                            }
                        },
                        onClick: function (event) {
                            var activePoints = myChart2.getElementsAtEvent(event);
                            var chartData = activePoints[0]['_chart'].config.data;
                            var idx = activePoints[0]['_index'];
                            var label = chartData.labels[idx];
                            DashboardPiChartTableBind(dateFrom, dateTo, "SubCategory", label);
                        },
                        animation: {
                            onComplete: function () {
                                var url_base64 = document.getElementById("subCategoryChart").toDataURL("image/png");
                                ChartBase64Image(url_base64, "Sub-Category wise");
                                this.options.animation.onComplete = null;
                            }
                        }
                    }
                });
            }
            else {
                $('#divSubCategoryChart').css('display', 'none');
            }
            //Region Wise Pi Chart
            if (result.regionPiCharts != null) {
                var Labels = [];
                var Values = [];
                var Colors = [];
                var sum1 = 0;
                var vale1 = [];
                $.each(result.regionPiCharts, function (i) {
                    Labels.push(result.regionPiCharts[i].Label);
                    vale1.push(parseInt(result.subCategoryPiCharts[i].Value));
                    sum1 += parseInt(result.subCategoryPiCharts[i].Value);

                    Colors.push(getRandomColorHex());
                });

                $.each(vale1, function (i) {
                    Values.push(parseInt(vale1[i] * 100 / sum1));
                });
                var ctx3 = document.getElementById("regionChart").getContext('2d');
                if (myChart3 != null) {
                    myChart3.destroy();
                }
                myChart3 = new Chart(ctx3, {
                    type: 'pie',
                    data: {
                        labels: Labels,

                        datasets: [{
                            backgroundColor: Colors,
                            data: Values,
                            datalabels: {
                                anchor: 'end',
                                formatter: (value, ctx) => {
                                    let sum = 0;
                                    let dataArr = ctx.chart.data.datasets[0].data;
                                    dataArr.map(data => {
                                        sum += data;
                                    });
                                    let percentage = Math.floor(value * 100 / sum) + "%";
                                    return percentage;
                                },
                                font: {
                                    weight: 'bold',
                                    size: 18
                                },
                                padding: 2,
                                align: 'start',
                            }
                        }]
                    },
                    // Configuration options go here
                    options: {
                        responsive: true,
                        legend: {
                            position: 'bottom',

                        },
                        tooltips: {
                            enabled: true,
                            callbacks: {
                                label: function (tooltipItem, data) {
                                    return data['labels'][tooltipItem['index']] + ': ' + data['datasets'][0]['data'][tooltipItem['index']] + '%';
                                }
                            }
                        },
                        animation: {
                            animateScale: true,
                            animateRotate: true
                        },
                        plugins: {
                            datalabels: {
                                backgroundColor: function (context) {
                                    return context.dataset.backgroundColor;
                                },
                                borderColor: 'white',
                                borderRadius: 25,
                                borderWidth: 2,
                                color: 'white',
                                font: {
                                    weight: 'bold'
                                },
                            }
                        }
                        ,
                        onClick: function (event) {
                            var activePoints = myChart3.getElementsAtEvent(event);
                            var chartData = activePoints[0]['_chart'].config.data;
                            var idx = activePoints[0]['_index'];
                            var label = chartData.labels[idx];
                            DashboardPiChartTableBind(dateFrom, dateTo, "Region", label);
                        },
                        animation: {
                            onComplete: function () {
                                var url_base64 = document.getElementById("regionChart").toDataURL("image/png");
                                ChartBase64Image(url_base64, " Region wise");
                                this.options.animation.onComplete = null;
                            }
                        }
                    }
                });
            }
            else {
                $('#regionChart').css('display', 'none');
            }
            //Office Wise Pi Chart
            if (result.officePiCharts != null) {
                var Labels = [];
                var Values = [];
                var Colors = [];
                var sum1 = 0;
                var vale1 = [];
                $.each(result.officePiCharts, function (i) {
                    Labels.push(result.officePiCharts[i].Label);
                    vale1.push(parseInt(result.officePiCharts[i].Value));
                    sum1 += parseInt(result.officePiCharts[i].Value);

                    Colors.push(getRandomColorHex());
                });

                $.each(vale1, function (i) {
                    Values.push(parseInt(vale1[i] * 100 / sum1));
                });
                var ctx4 = document.getElementById("officeChart").getContext('2d');
                if (myChart4 != null) {
                    myChart4.destroy();
                }
                myChart4 = new Chart(ctx4, {
                    type: 'pie',
                    data: {
                        labels: Labels,

                        datasets: [{
                            backgroundColor: Colors,
                            data: Values,
                            datalabels: {
                                anchor: 'end',
                                formatter: (value, ctx) => {
                                    let sum = 0;
                                    let dataArr = ctx.chart.data.datasets[0].data;
                                    dataArr.map(data => {
                                        sum += data;
                                    });
                                    let percentage = Math.floor(value * 100 / sum) + "%";
                                    return percentage;
                                },
                                font: {
                                    weight: 'bold',
                                    size: 18
                                },
                                padding: 2,
                                align: 'start',
                            }
                        }]
                    },
                    // Configuration options go here
                    options: {
                        responsive: true,
                        legend: {
                            position: 'bottom',

                        },
                        tooltips: {
                            enabled: true,
                            callbacks: {
                                label: function (tooltipItem, data) {
                                    return data['labels'][tooltipItem['index']] + ': ' + data['datasets'][0]['data'][tooltipItem['index']] + '%';
                                }
                            }
                        },
                        animation: {
                            animateScale: true,
                            animateRotate: true
                        },
                        plugins: {
                            datalabels: {
                                backgroundColor: function (context) {
                                    return context.dataset.backgroundColor;
                                },
                                borderColor: 'white',
                                borderRadius: 25,
                                borderWidth: 2,
                                color: 'white',
                                font: {
                                    weight: 'bold'
                                },
                            }
                        }
                        ,
                        onClick: function (event) {
                            var activePoints = myChart4.getElementsAtEvent(event);
                            var chartData = activePoints[0]['_chart'].config.data;
                            var idx = activePoints[0]['_index'];
                            var label = chartData.labels[idx];
                            DashboardPiChartTableBind(dateFrom, dateTo, "Office", label);
                        },
                        animation: {
                            onComplete: function () {
                                var url_base64 = document.getElementById("officeChart").toDataURL("image/png");
                                ChartBase64Image(url_base64, "Office wise");
                                this.options.animation.onComplete = null;
                            }
                        }
                    }
                });
            }
            else {
                $('#officeChart').css('display', 'none');
            }
            //LOS Wise Pi Chart
            if (result.losPiCharts != null) {
                var Labels = [];
                var Values = [];
                var Colors = [];
                var vale1 = [];
                var sum1 = "";
                $.each(result.losPiCharts, function (i) {
                    Labels.push(result.losPiCharts[i].Label);
                    vale1.push(parseInt(result.losPiCharts[i].Value));
                    sum1 += parseInt(result.losPiCharts[i].Value);

                    Colors.push(getRandomColorHex());
                });

                $.each(vale1, function (i) {
                    Values.push(parseInt(vale1[i] * 100 / sum1));
                });
                var ctx5 = document.getElementById("losChart").getContext('2d');
                if (myChart5 != null) {
                    myChart5.destroy();
                }
                myChart5 = new Chart(ctx5, {
                    type: 'pie',
                    data: {
                        labels: Labels,

                        datasets: [{
                            backgroundColor: Colors,
                            data: Values,
                            datalabels: {
                                anchor: 'end',
                                formatter: (value, ctx) => {
                                    let sum = 0;
                                    let dataArr = ctx.chart.data.datasets[0].data;
                                    dataArr.map(data => {
                                        sum += data;
                                    });
                                    let percentage = Math.floor(value * 100 / sum) + "%";
                                    return percentage;
                                },
                                font: {
                                    weight: 'bold',
                                    size: 18
                                },
                                padding: 2,
                                align: 'start',
                            }
                        }]
                    },
                    // Configuration options go here
                    options: {
                        responsive: true,
                        legend: {
                            position: 'bottom',

                        },
                        tooltips: {
                            enabled: true,
                            callbacks: {
                                label: function (tooltipItem, data) {
                                    return data['labels'][tooltipItem['index']] + ': ' + data['datasets'][0]['data'][tooltipItem['index']] + '%';
                                }
                            }
                        },
                        animation: {
                            animateScale: true,
                            animateRotate: true
                        },
                        plugins: {
                            datalabels: {
                                backgroundColor: function (context) {
                                    return context.dataset.backgroundColor;
                                },
                                borderColor: 'white',
                                borderRadius: 25,
                                borderWidth: 2,
                                color: 'white',
                                font: {
                                    weight: 'bold'
                                },
                            }
                        }
                        ,
                        onClick: function (event) {
                            var activePoints = myChart5.getElementsAtEvent(event);
                            var chartData = activePoints[0]['_chart'].config.data;
                            var idx = activePoints[0]['_index'];
                            var label = chartData.labels[idx];
                            DashboardPiChartTableBind(dateFrom, dateTo, "LOS", label);
                        },
                        animation: {
                            onComplete: function () {
                                var url_base64 = document.getElementById("losChart").toDataURL("image/png");
                                ChartBase64Image(url_base64, "LOS wise");
                                this.options.animation.onComplete = null;
                            }
                        }
                    }
                });
            }
            else {
                $('#divLosChart').css('display', 'none');
            }
            //SBU Wise Pi Chart
            if (result.sbuPiCharts != null) {
                var Labels = [];
                var Values = [];
                var Colors = [];
                var vale1 = [];
                var sum1 = 0;
                $.each(result.sbuPiCharts, function (i) {
                    Labels.push(result.sbuPiCharts[i].Label);
                    vale1.push(parseInt(result.sbuPiCharts[i].Value));
                    sum1 += parseInt(result.sbuPiCharts[i].Value);

                    Colors.push(getRandomColorHex());
                });

                $.each(vale1, function (i) {
                    Values.push(parseInt(vale1[i] * 100 / sum1));
                });
                var ctx6 = document.getElementById("sbuChart").getContext('2d');
                if (myChart6 != null) {
                    myChart6.destroy();
                }
                myChart6 = new Chart(ctx6, {
                    type: 'pie',
                    data: {
                        labels: Labels,

                        datasets: [{
                            backgroundColor: Colors,
                            data: Values,
                            datalabels: {
                                anchor: 'end',
                                formatter: (value, ctx) => {
                                    let sum = 0;
                                    let dataArr = ctx.chart.data.datasets[0].data;
                                    dataArr.map(data => {
                                        sum += data;
                                    });
                                    let percentage = Math.floor(value * 100 / sum) + "%";
                                    return percentage;
                                },
                                font: {
                                    weight: 'bold',
                                    size: 18
                                },
                                padding: 2,
                                align: 'start',
                            }
                        }]
                    },
                    // Configuration options go here
                    options: {
                        responsive: true,
                        legend: {
                            position: 'bottom',

                        },
                        tooltips: {
                            enabled: true,
                            callbacks: {
                                label: function (tooltipItem, data) {
                                    return data['labels'][tooltipItem['index']] + ': ' + data['datasets'][0]['data'][tooltipItem['index']] + '%';
                                }
                            }
                        },
                        animation: {
                            animateScale: true,
                            animateRotate: true
                        },
                        plugins: {
                            datalabels: {
                                backgroundColor: function (context) {
                                    return context.dataset.backgroundColor;
                                },
                                borderColor: 'white',
                                borderRadius: 25,
                                borderWidth: 2,
                                color: 'white',
                                font: {
                                    weight: 'bold'
                                },
                            }
                        }
                        ,
                        onClick: function (event) {
                            var activePoints = myChart6.getElementsAtEvent(event);
                            var chartData = activePoints[0]['_chart'].config.data;
                            var idx = activePoints[0]['_index'];
                            var label = chartData.labels[idx];
                            DashboardPiChartTableBind(dateFrom, dateTo, "SBU", label);
                        },
                        animation: {
                            onComplete: function () {
                                var url_base64 = document.getElementById("sbuChart").toDataURL("image/png");
                                ChartBase64Image(url_base64, "SBU wise");
                                this.options.animation.onComplete = null;
                            }
                        }
                    }
                });
            }
            else {
                $('#divSbuChart').css('display', 'none');
            }
            //Sub-SBU Wise Pi Chart
            if (result.subSBUPiCharts != null) {
                var Labels = [];
                var Values = [];
                var Colors = [];
                var vale1 = [];
                var sum1 = 0;
                $.each(result.subSBUPiCharts, function (i) {
                    Labels.push(result.subSBUPiCharts[i].Label);
                    vale1.push(parseInt(result.subSBUPiCharts[i].Value));
                    sum1 += parseInt(result.subSBUPiCharts[i].Value);

                    Colors.push(getRandomColorHex());
                });

                $.each(vale1, function (i) {
                    Values.push(parseInt(vale1[i] * 100 / sum1));
                });
                var ctx7 = document.getElementById("subSBUChart").getContext('2d');
                if (myChart7 != null) {
                    myChart7.destroy();
                }
                myChart7 = new Chart(ctx7, {
                    type: 'pie',
                    data: {
                        labels: Labels,

                        datasets: [{
                            backgroundColor: Colors,
                            data: Values,
                            datalabels: {
                                anchor: 'end',
                                formatter: (value, ctx) => {
                                    let sum = 0;
                                    let dataArr = ctx.chart.data.datasets[0].data;
                                    dataArr.map(data => {
                                        sum += data;
                                    });
                                    let percentage = Math.floor(value * 100 / sum) + "%";
                                    return percentage;
                                },
                                font: {
                                    weight: 'bold',
                                    size: 18
                                },
                                padding: 2,
                                align: 'start',
                            }
                        }]
                    },
                    // Configuration options go here
                    options: {
                        responsive: true,
                        legend: {
                            position: 'bottom',

                        },
                        tooltips: {
                            enabled: true,
                            callbacks: {
                                label: function (tooltipItem, data) {
                                    return data['labels'][tooltipItem['index']] + ': ' + data['datasets'][0]['data'][tooltipItem['index']] + '%';
                                }
                            }
                        },
                        animation: {
                            animateScale: true,
                            animateRotate: true
                        },
                        plugins: {
                            datalabels: {
                                backgroundColor: function (context) {
                                    return context.dataset.backgroundColor;
                                },
                                borderColor: 'white',
                                borderRadius: 25,
                                borderWidth: 2,
                                color: 'white',
                                font: {
                                    weight: 'bold'
                                },
                            }
                        }
                        ,
                        onClick: function (event) {
                            var activePoints = myChart7.getElementsAtEvent(event);
                            var chartData = activePoints[0]['_chart'].config.data;
                            var idx = activePoints[0]['_index'];
                            var label = chartData.labels[idx];
                            DashboardPiChartTableBind(dateFrom, dateTo, "SubSBU", label);
                        },
                        animation: {
                            onComplete: function () {
                                var url_base64 = document.getElementById("subSBUChart").toDataURL("image/png");
                                ChartBase64Image(url_base64, "Sub-SBU wise");
                                this.options.animation.onComplete = null;
                            }
                        }
                    }
                });
            }
            else {
                $('#divSubSBUChart').css('display', 'none');
            }
            //Gender of Complainant Wise Pi Chart
            if (result.genderofComplainant != null) {
                var Labels = [];
                var Values = [];
                var Colors = [];
                var vale1 = [];
                var sum1 = 0;
                $.each(result.genderofComplainant, function (i) {
                    Labels.push(result.genderofComplainant[i].Label);
                    vale1.push(result.genderofComplainant[i].Value);
                    sum1 += parseInt(result.genderofComplainant[i].Value);

                    Colors.push(getRandomColorHex());
                });

                $.each(vale1, function (i) {
                    Values.push(parseInt(vale1[i] * 100 / sum1));
                });
                var ctx8 = document.getElementById("genderOfComplainantChart").getContext('2d');
                if (myChart8 != null) {
                    myChart8.destroy();
                }
                myChart8 = new Chart(ctx8, {
                    type: 'pie',
                    data: {
                        labels: Labels,

                        datasets: [{
                            backgroundColor: Colors,
                            data: Values,
                            datalabels: {
                                anchor: 'end',
                                formatter: (value, ctx) => {
                                    let sum = 0;
                                    let dataArr = ctx.chart.data.datasets[0].data;
                                    dataArr.map(data => {
                                        sum += data;
                                    });
                                    let percentage = Math.floor(value * 100 / sum) + "%";
                                    return percentage;
                                },
                                font: {
                                    weight: 'bold',
                                    size: 18
                                },
                                padding: 2,
                                align: 'start',
                            }
                        }]
                    },
                    // Configuration options go here
                    options: {
                        responsive: true,
                        legend: {
                            position: 'bottom',

                        },
                        tooltips: {
                            enabled: true,
                            callbacks: {
                                label: function (tooltipItem, data) {
                                    return data['labels'][tooltipItem['index']] + ': ' + data['datasets'][0]['data'][tooltipItem['index']] + '%';
                                }
                            }
                        },
                        animation: {
                            animateScale: true,
                            animateRotate: true
                        },
                        plugins: {
                            datalabels: {
                                backgroundColor: function (context) {
                                    return context.dataset.backgroundColor;
                                },
                                borderColor: 'white',
                                borderRadius: 25,
                                borderWidth: 2,
                                color: 'white',
                                font: {
                                    weight: 'bold'
                                },
                            }
                        }
                        ,
                        onClick: function (event) {
                            var activePoints = myChart8.getElementsAtEvent(event);
                            var chartData = activePoints[0]['_chart'].config.data;
                            var idx = activePoints[0]['_index'];
                            var label = chartData.labels[idx];
                            DashboardPiChartTableBind(dateFrom, dateTo, "GenderofComplainant", label);
                        },
                        animation: {
                            onComplete: function () {
                                var url_base64 = document.getElementById("genderOfComplainantChart").toDataURL("image/png");
                                ChartBase64Image(url_base64, "Gender of Complainant");
                                this.options.animation.onComplete = null;
                            }
                        }
                    }
                });
            }
            else {
                $('#divGenderOfComplainantChart').css('display', 'none');
            }
            //Gender of Respondent Wise Pi Chart
            if (result.genderofRespondent != null) {
                var Labels = [];
                var Values = [];
                var Colors = [];
                var vale1 = [];
                var sum1 = 0;
                $.each(result.genderofComplainant, function (i) {
                    Labels.push(result.genderofComplainant[i].Label);
                    vale1.push(parseInt(result.genderofComplainant[i].Value));
                    sum1 += parseInt(result.genderofComplainant[i].Value);

                    Colors.push(getRandomColorHex());
                });

                $.each(vale1, function (i) {
                    Values.push(parseInt(vale1[i] * 100 / sum1));
                });
                var ctx9 = document.getElementById("genderOfRespondentChart").getContext('2d');
                if (myChart9 != null) {
                    myChart9.destroy();
                }
                myChart9 = new Chart(ctx9, {
                    type: 'pie',
                    data: {
                        labels: Labels,

                        datasets: [{
                            backgroundColor: Colors,
                            data: Values,
                            datalabels: {
                                anchor: 'end',
                                formatter: (value, ctx) => {
                                    let sum = 0;
                                    let dataArr = ctx.chart.data.datasets[0].data;
                                    dataArr.map(data => {
                                        sum += data;
                                    });
                                    let percentage = Math.floor(value * 100 / sum) + "%";
                                    return percentage;
                                },
                                font: {
                                    weight: 'bold',
                                    size: 18
                                },
                                padding: 2,
                                align: 'start',
                            }
                        }]
                    },
                    // Configuration options go here
                    options: {
                        responsive: true,
                        legend: {
                            position: 'bottom',

                        },
                        tooltips: {
                            enabled: true,
                            callbacks: {
                                label: function (tooltipItem, data) {
                                    return data['labels'][tooltipItem['index']] + ': ' + data['datasets'][0]['data'][tooltipItem['index']] + '%';
                                }
                            }
                        },
                        animation: {
                            animateScale: true,
                            animateRotate: true
                        },
                        plugins: {
                            datalabels: {
                                backgroundColor: function (context) {
                                    return context.dataset.backgroundColor;
                                },
                                borderColor: 'white',
                                borderRadius: 25,
                                borderWidth: 2,
                                color: 'white',
                                font: {
                                    weight: 'bold'
                                },
                            }
                        }
                        ,
                        onClick: function (event) {
                            var activePoints = myChart9.getElementsAtEvent(event);
                            var chartData = activePoints[0]['_chart'].config.data;
                            var idx = activePoints[0]['_index'];
                            var label = chartData.labels[idx];
                            DashboardPiChartTableBind(dateFrom, dateTo, "GenderofRespondent", label);
                        },
                        animation: {
                            onComplete: function () {
                                var url_base64 = document.getElementById("genderOfRespondentChart").toDataURL("image/png");
                                ChartBase64Image(url_base64, "Gender of Respondent");
                                this.options.animation.onComplete = null;
                            }
                        }
                    }
                });
            }
            else {
                $('#divGenderOfRespondentChart').css('display', 'none');
            }
            //Designation of Complainant Wise Pi Chart
            if (result.designationofComplainant != null) {
                var Labels = [];
                var Values = [];
                var Colors = [];
                var sum1 = 0;
                var vale1 = [];
                $.each(result.designationofComplainant, function (i) {
                    Labels.push(result.designationofComplainant[i].Label);
                    vale1.push(parseInt(result.designationofComplainant[i].Value));
                    sum1 += 0 + parseInt(result.designationofComplainant[i].Value);

                    Colors.push(getRandomColorHex());
                });

                $.each(vale1, function (i) {
                    Values.push(parseInt(vale1[i] * 100 / sum1));
                });
                var ctx10 = document.getElementById("designationOfComplainantChart").getContext('2d');
                if (myChart10 != null) {
                    myChart10.destroy();
                }
                myChart10 = new Chart(ctx10, {
                    type: 'pie',
                    data: {
                        labels: Labels,

                        datasets: [{
                            backgroundColor: Colors,
                            data: Values,
                            datalabels: {
                                anchor: 'end',
                                formatter: (value, ctx) => {
                                    let sum = 0;
                                    let dataArr = ctx.chart.data.datasets[0].data;
                                    dataArr.map(data => {
                                        sum += data;
                                    });
                                    let percentage = Math.floor(value * 100 / sum) + "%";
                                    return percentage;
                                },
                                font: {
                                    weight: 'bold',
                                    size: 18
                                },
                                padding: 2,
                                align: 'start',
                            }
                        }]
                    },
                    // Configuration options go here
                    options: {
                        responsive: true,
                        legend: {
                            position: 'bottom',

                        },
                        tooltips: {
                            enabled: true,
                            callbacks: {
                                label: function (tooltipItem, data) {
                                    return data['labels'][tooltipItem['index']] + ': ' + data['datasets'][0]['data'][tooltipItem['index']] + '%';
                                }
                            }
                        },
                        animation: {
                            animateScale: true,
                            animateRotate: true
                        },
                        plugins: {
                            datalabels: {
                                backgroundColor: function (context) {
                                    return context.dataset.backgroundColor;
                                },
                                borderColor: 'white',
                                borderRadius: 25,
                                borderWidth: 2,
                                color: 'white',
                                font: {
                                    weight: 'bold'
                                },
                            }
                        }
                        ,
                        onClick: function (event) {
                            var activePoints = myChart10.getElementsAtEvent(event);
                            var chartData = activePoints[0]['_chart'].config.data;
                            var idx = activePoints[0]['_index'];
                            var label = chartData.labels[idx];
                            DashboardPiChartTableBind(dateFrom, dateTo, "DesignationofComplainant", label);
                        },
                        animation: {
                            onComplete: function () {
                                var url_base64 = document.getElementById("designationOfComplainantChart").toDataURL("image/png");
                                ChartBase64Image(url_base64, "Designation of Complainant");
                                this.options.animation.onComplete = null;
                            }
                        }
                    }
                });
            }
            else {
                $('#divDesignationOfComplainantChart').css('display', 'none');
            }
            //Designation of Respondent Wise Pi Chart
            if (result.designationofRespondent != null) {
                var Labels = [];
                var Values = [];
                var Colors = [];
                var vale1 = [];
                var sum1 = 0;
                $.each(result.designationofRespondent, function (i) {
                    Labels.push(result.designationofRespondent[i].Label);
                    vale1.push(parseInt(result.designationofRespondent[i].Value));
                    sum1 += 0 + parseInt(result.designationofRespondent[i].Value);

                    Colors.push(getRandomColorHex());
                });

                $.each(vale1, function (i) {
                    Values.push(parseInt(vale1[i] * 100 / sum1));
                });
                var ctx11 = document.getElementById("designationOfRespondentChart").getContext('2d');
                if (myChart11 != null) {
                    myChart11.destroy();
                }
                myChart11 = new Chart(ctx11, {
                    type: 'pie',
                    data: {
                        labels: Labels,

                        datasets: [{
                            backgroundColor: Colors,
                            data: Values,
                            datalabels: {
                                anchor: 'end',
                                formatter: (value, ctx) => {
                                    let sum = 0;
                                    let dataArr = ctx.chart.data.datasets[0].data;
                                    dataArr.map(data => {
                                        sum += data;
                                    });
                                    let percentage = Math.floor(value * 100 / sum) + "%";
                                    return percentage;
                                },
                                font: {
                                    weight: 'bold',
                                    size: 18
                                },
                                padding: 2,
                                align: 'start',
                            }
                        }]
                    },
                    // Configuration options go here
                    options: {
                        responsive: true,
                        legend: {
                            position: 'bottom',

                        },
                        tooltips: {
                            enabled: true,
                            callbacks: {
                                label: function (tooltipItem, data) {
                                    return data['labels'][tooltipItem['index']] + ': ' + data['datasets'][0]['data'][tooltipItem['index']] + '%';
                                }
                            }
                        },
                        animation: {
                            animateScale: true,
                            animateRotate: true
                        },
                        plugins: {
                            datalabels: {
                                backgroundColor: function (context) {
                                    return context.dataset.backgroundColor;
                                },
                                borderColor: 'white',
                                borderRadius: 25,
                                borderWidth: 2,
                                color: 'white',
                                font: {
                                    weight: 'bold'
                                },
                            }
                        }
                        ,
                        onClick: function (event) {
                            var activePoints = myChart11.getElementsAtEvent(event);
                            var chartData = activePoints[0]['_chart'].config.data;
                            var idx = activePoints[0]['_index'];
                            var label = chartData.labels[idx];
                            DashboardPiChartTableBind(dateFrom, dateTo, "DesignationofRespondent", label);
                        },
                        animation: {
                            onComplete: function () {
                                var url_base64 = document.getElementById("designationOfRespondentChart").toDataURL("image/png");
                                ChartBase64Image(url_base64, "Designation of Respondent");
                                this.options.animation.onComplete = null;
                            }
                        }
                    }
                });
            }
            else {
                $('#divDesignationOfRespondentChart').css('display', 'none');
            }
            //Mode of Complaint Wise Pi Chart
            if (result.modeofComplaint != null) {
                var Labels = [];
                var Values = [];
                var Colors = [];
                var vale1 = [];
                var sum1 = 0;
                $.each(result.modeofComplaint, function (i) {
                    Labels.push(result.modeofComplaint[i].Label);
                    vale1.push(result.modeofComplaint[i].Value);
                    sum1 += 0 + parseInt(result.modeofComplaint[i].Value);
                    Colors.push(getRandomColorHex());
                });

                $.each(vale1, function (i) {
                    Values.push(parseInt(vale1[i] * 100 / sum1));
                });
                var ctx12 = document.getElementById("modeofComplaintChart").getContext('2d');
                if (myChart12 != null) {
                    myChart12.destroy();
                }
                myChart12 = new Chart(ctx12, {
                    type: 'pie',
                    data: {
                        labels: Labels,
                        // id: Labels,
                        datasets: [{
                            backgroundColor: Colors,
                            data: Values,
                            datalabels: {
                                anchor: 'end',
                                formatter: (value, ctx) => {
                                    let sum = 0;
                                    let dataArr = ctx.chart.data.datasets[0].data;
                                    dataArr.map(data => {
                                        sum += data;
                                    });
                                    let percentage = Math.floor(value * 100 / sum) + "%";
                                    return percentage;
                                },
                                font: {
                                    weight: 'bold',
                                    size: 18
                                },
                                padding: 2,
                                align: 'start',
                            }
                        }]
                    },
                    // Configuration options go here
                    options: {
                        responsive: true,
                        legend: {
                            position: 'bottom',

                        },
                        tooltips: {
                            enabled: true,
                            callbacks: {
                                label: function (tooltipItem, data) {
                                    return data['labels'][tooltipItem['index']] + ': ' + data['datasets'][0]['data'][tooltipItem['index']] + '%';
                                }
                            }
                        },
                        animation: {
                            animateScale: true,
                            animateRotate: true
                        },
                        plugins: {
                            datalabels: {
                                backgroundColor: function (context) {
                                    return context.dataset.backgroundColor;
                                },
                                borderColor: 'white',
                                borderRadius: 25,
                                borderWidth: 2,
                                color: 'white',
                                font: {
                                    weight: 'bold'
                                },
                            }
                        }
                        ,
                        onClick: function (event) {
                            var activePoints = myChart12.getElementsAtEvent(event);
                            var chartData = activePoints[0]['_chart'].config.data;
                            var idx = activePoints[0]['_index'];
                            var label = chartData.labels[idx];
                            //var id = chartData.id[idx];
                            //alert(id);
                            DashboardPiChartTableBind(dateFrom, dateTo, "ModeofComplaint", label);
                        },
                        animation: {
                            onComplete: function () {
                                var url_base64 = document.getElementById("modeofComplaintChart").toDataURL("image/png");
                                ChartBase64Image(url_base64, "Mode of Complaint");
                                this.options.animation.onComplete = null;
                            }
                        }
                    }
                });
            }
            else {
                $('#divModeofComplaintChart').css('display', 'none');
            }
            //Ageing/Case Closure Wise Pi Chart
            if (result.categoryPiCharts != null) {
                var Labels = [];
                var Values = [];
                var Colors = [];
                var sum1 = 0;
                var vale1 = [];
                $.each(result.ageingPiBarCharts, function (i) {
                    Labels.push(result.ageingPiBarCharts[i].Label);
                    vale1.push(result.ageingPiBarCharts[i].Value1);
                    sum1 += 0 + parseInt(result.ageingPiBarCharts[i].Value1);
                    Colors.push(getRandomColorHex());
                });

                $.each(vale1, function (i) {
                    Values.push(parseInt(vale1[i] * 100 / sum1));
                });
                var ctx13 = document.getElementById("ageingChart").getContext('2d');
                if (myChart13 != null) {
                    myChart13.destroy();
                }
                myChart13 = new Chart(ctx13, {
                    type: 'pie',

                    data: {
                        labels: Labels,
                        datasets: [{
                            backgroundColor: Colors,
                            data: Values,
                            datalabels: {
                                anchor: 'end',
                                formatter: (value, ctx) => {
                                    let sum = 0;
                                    let dataArr = ctx.chart.data.datasets[0].data;
                                    dataArr.map(data => {
                                        sum += data;
                                    });
                                    let percentage = Math.floor(value * 100 / sum) + "%";
                                    return percentage;
                                },
                                font: {
                                    weight: 'bold',
                                    size: 18
                                },
                                padding: 2,
                                align: 'start',
                            }
                        }]
                    },
                    // Configuration options go here
                    options: {
                        //responsive: true,
                        legend: {
                            position: 'bottom',

                        },
                        tooltips: {
                            enabled: true,
                            callbacks: {
                                label: function (tooltipItem, data) {
                                    return data['labels'][tooltipItem['index']] + ': ' + data['datasets'][0]['data'][tooltipItem['index']] + '%';
                                }
                            }
                        },
                        animation: {
                            animateScale: true,
                            animateRotate: true
                        },
                        plugins: {
                            datalabels: {
                                backgroundColor: function (context) {
                                    return context.dataset.backgroundColor;
                                },
                                borderColor: 'white',
                                borderRadius: 50,
                                borderWidth: 2,
                                color: 'white',
                                font: {
                                    weight: 'bold'
                                },

                                //formatter: Math.round
                            }
                        }
                        ,
                        onClick: function (event) {
                            var activePoints = myChart13.getElementsAtEvent(event);
                            var chartData = activePoints[0]['_chart'].config.data;
                            var idx = activePoints[0]['_index'];
                            var label = chartData.labels[idx];
                            DashboardPiChartTableBind(dateFrom, dateTo, "Ageing", label);
                        },
                        animation: {
                            onComplete: function () {
                                var url_base64 = document.getElementById("ageingChart").toDataURL("image/png");
                                ChartBase64Image(url_base64, "Ageing/Case Closure");
                                this.options.animation.onComplete = null;
                            }
                        }
                    }

                });
            }
            else {
                $('#divAgeingChart').css('display', 'none');
            }
        }



    });

}

function dashboardgraphpercent() {
    Base64Image = [];
    StartProcess();
    let dateFrom = ($('#txt_dateFrom').val());
    let dateTo = ($('#txt_dateTo').val());

    var yearCurrent = new Date();
    yearCurrent = yearCurrent.getFullYear();
    let yearBack = new Date();
    yearBack = (yearBack.getFullYear() - 1);


    $.get('/home/DashboardBarChart', { dateFrom: dateFrom, dateTo: dateTo }, function (result) {
        StopProcess();
        if (result != null && result != "undefined" && result != "") {
            //Case Type Bar chart
            if (result.caseTypeofComplaint1 != null) {
                var newArr;
                var Labels = []; var Values1 = []; var Values2 = []; var Years = []; var Colors = [];
                var sum = 0; var sum1 = 0; var vale = []; var vale1 = [];
                $.each(result.caseTypeofComplaint1, function (i) {

                    Labels.push(result.caseTypeofComplaint1[i].Label);
                    //Values1.push(result.caseTypeofComplaint1[i].Value1);
                    //Values2.push(result.caseTypeofComplaint1[i].Value2);
                    Years.push(result.caseTypeofComplaint1[i].Year);
                    Colors.push(getRandomColorHex());
                    vale.push(parseInt(result.caseTypeofComplaint1[i].Value1));
                    vale1.push(parseInt(result.caseTypeofComplaint1[i].Value2));
                    sum += result.caseTypeofComplaint1[i].Value1;
                    sum1 += result.caseTypeofComplaint1[i].Value2;
                    Colors.push(getRandomColorHex());
                });

                $.each(vale, function (i) {
                    Values1.push((vale[i] * 100 / sum));
                });
                $.each(vale1, function (i) {
                    Values2.push((vale1[i] * 100 / sum1));
                });


                var ctx = document.getElementById("caseTypeChart").getContext('2d');
                if (myChart != null) {
                    myChart.destroy();
                }
                myChart = new Chart(ctx, {
                    type: 'bar',
                    data: {
                        labels: Years,
                        datasets: [{
                            label: "Actionable",
                            backgroundColor: "rgb(54, 162, 235)",
                            data: Values1,
                            datalabels: {
                                anchor: 'center',
                                formatter: (value, ctx) => {
                                    let sum = 0;
                                    let dataArr = ctx.chart.data.datasets[0].data;
                                    dataArr.map(data => {
                                        sum += data;
                                    });
                                    let percentage = Math.floor(value * 100 / sum) + "%";
                                    return percentage;
                                },
                            }

                        },
                        {
                            label: "NonActionable",
                            backgroundColor: "rgb(255, 133, 102)",
                            data: Values2,
                            datalabels: {
                                anchor: 'center'
                            }
                        }]
                    },
                    options: {
                        scales: {
                            yAxes: [{
                                ticks: {
                                    beginAtZero: true,

                                }
                            }]

                        },
                        legend: {
                            position: 'bottom',
                        },
                        tooltips: {
                            enabled: true
                        },
                        plugins: {
                            datalabels: {
                                color: '#fff',
                            }
                        },
                        onClick:
                            function (event, array) {
                                var active = window.myChart.getElementAtEvent(event);
                                var elementIndex = active[0]._datasetIndex;
                                var chartData = array[elementIndex]['_chart'].data;
                                var idx = array[elementIndex]['_index'];
                                var year = chartData.labels[idx];
                                var label = chartData.datasets[elementIndex].label;
                                DashboardBarChartTableBind(dateFrom, dateTo, "CaseType", label, year);
                            },

                        animation: {
                            onComplete: function () {
                                var url_base64 = document.getElementById("caseTypeChart").toDataURL("image/png");
                                ChartBase64Image(url_base64, "Case Type");
                                this.options.animation.onComplete = null;
                            }
                        },
                    }
                });
            }
            else {
                $('#divCaseTypeChart').css('display', 'none');
            }
            //Category Wise Bar Chart
            if (result.categoryPiBarCharts != null) {
                var Years = []; var arrData = [];
                var sum = 0; var sum1 = 0; var vale = []; var vale1 = [];
                var Values1 = []; var Values2 = [];
                $.each(result.categoryPiBarCharts, function (i) {
                    if (Years.indexOf(result.categoryPiBarCharts[i].Year) === -1) {
                        Years.push(result.categoryPiBarCharts[i].Year);

                    }
                    vale.push(Number(result.categoryPiBarCharts[i].Value1));
                    vale1.push(Number(result.categoryPiBarCharts[i].Value2));
                    sum += Number(result.categoryPiBarCharts[i].Value1);
                    sum1 += Number(result.categoryPiBarCharts[i].Value2);

                });

                $.each(vale, function (i) {
                    Values1.push(parseInt(vale[i] * 100 / sum));

                });
                $.each(vale1, function (i) {
                    var secondValue = parseInt(vale1[i] * 100 / sum1) || 0;
                    Values2.push(secondValue);

                });


                $.each(result.categoryPiBarCharts, function (i) {
                    arrData.push({
                        label: result.categoryPiBarCharts[i].Label,
                        backgroundColor: getRandomColorHex(),
                        data: [Values2[i], Values1[i]],
                        datalabels: {
                            anchor: 'center',

                        }

                    })
                });



                var ctx1 = document.getElementById("categoryChart").getContext('2d');
                if (myChart1 != null) {
                    myChart1.destroy();
                }
                myChart1 = new Chart(ctx1, {
                    type: 'bar',
                    data: {
                        labels: [yearBack, yearCurrent],
                        datasets: arrData,

                    },
                    //Configuration options go here
                    options: {
                        scales: {
                            yAxes: [{
                                ticks: {
                                    beginAtZero: true
                                }
                            }]
                        },
                        legend: {
                            "position": "bottom"

                        },
                        tooltips: {
                            enabled: true
                        },
                        plugins: {
                            datalabels: {
                                color: '#fff',
                                formatter: (value, ctx) => {

                                    let index = ctx.datasetIndex === 0 ? 1 : 0;
                                    let total = ctx.chart.data.datasets[index].data[ctx.dataIndex] + value;
                                    let percentage = Math.floor(value / total * 100) + "%";
                                    return percentage;

                                    ////debugger
                                    ////let sum = 0;
                                    ////let index = ctx.dataIndex;
                                    ////let dataArr = ctx.chart.data.datasets[index].data;
                                    ////dataArr.map(data => {
                                    ////    sum += data;
                                    //});
                                    //let percentage = Math.floor(sum) + "%";
                                    //return percentage;
                                },
                                font: {
                                    weight: 'bold'
                                },
                            }
                        },
                        onClick:
                            function (event, array) {
                                var active = window.myChart1.getElementAtEvent(event);
                                var elementIndex = active[0]._datasetIndex;
                                var chartData = array[elementIndex]['_chart'].data;
                                var idx = array[elementIndex]['_index'];
                                var year = chartData.labels[idx];
                                var label = chartData.datasets[elementIndex].label;
                                DashboardBarChartTableBind(dateFrom, dateTo, "Category", label, year);
                            },
                        animation: {
                            onComplete: function () {
                                var url_base64 = document.getElementById("categoryChart").toDataURL("image/png");
                                ChartBase64Image(url_base64, "Category wise");
                                this.options.animation.onComplete = null;
                            }
                        },
                    }

                });
            }

            else {
                $('#divCategoryChart').css('display', 'none');
            }
            // Sub-Category Wise Bar Chart
            if (result.subCategoryPiBarCharts != null) {
                var Years = []; var arrData = [];
                var sum = 0; var sum1 = 0; var vale = []; var vale1 = [];
                var Values1 = []; var Values2 = [];
                $.each(result.subCategoryPiBarCharts, function (i) {
                    if (Years.indexOf(result.subCategoryPiBarCharts[i].Year) === -1) {
                        Years.push(result.subCategoryPiBarCharts[i].Year);
                    }

                    vale.push(parseInt(result.subCategoryPiBarCharts[i].Value1));
                    vale1.push(parseInt(result.subCategoryPiBarCharts[i].Value2));
                    sum += parseInt(result.subCategoryPiBarCharts[i].Value1);
                    sum1 += parseInt(result.subCategoryPiBarCharts[i].Value2);

                    //arrData.push({
                    //    label: result.subCategoryPiBarCharts[i].Label, backgroundColor: getRandomColorHex(),
                    //    data: [result.subCategoryPiBarCharts[i].Value2, result.subCategoryPiBarCharts[i].Value1],
                    //    datalabels: { anchor: 'center' }
                    //})
                });

                $.each(vale, function (i) {
                    Values1.push(parseInt(vale[i] * 100 / sum));

                });
                $.each(vale1, function (i) {
                    var secondValue = parseInt(vale1[i] * 100 / sum1) || 0;
                    Values2.push(secondValue);


                });
                $.each(result.subCategoryPiBarCharts, function (i) {
                    arrData.push({
                        label: result.subCategoryPiBarCharts[i].Label, backgroundColor: getRandomColorHex(),
                        data: [Values2[i], Values1[i]],
                        datalabels: { anchor: 'center' }

                    })
                });


                var ctx2 = document.getElementById("subCategoryChart").getContext('2d');
                if (myChart2 != null) {
                    myChart2.destroy();
                }
                myChart2 = new Chart(ctx2, {
                    type: 'bar',
                    data: {
                        labels: [yearBack, yearCurrent],
                        datasets: arrData
                    },
                    // Configuration options go here
                    options: {
                        scales: {
                            yAxes: [{
                                ticks: {
                                    beginAtZero: true
                                }
                            }]
                        },
                        legend: {
                            position: 'bottom',

                        },
                        tooltips: {
                            enabled: true
                        },
                        plugins: {
                            datalabels: {
                                color: '#fff',
                                formatter: (value, ctx) => {
                                    debugger
                                    let index = ctx.datasetIndex === 0 ? 1 : 0;
                                    let total = ctx.chart.data.datasets[index].data[ctx.dataIndex] + value;
                                    let percentage = Math.floor(value / total * 100) + "%";
                                    return percentage;


                                },

                            }
                        },
                        onClick: function (event, array) {
                            var active = window.myChart2.getElementAtEvent(event);
                            var elementIndex = active[0]._datasetIndex;
                            var chartData = array[elementIndex]['_chart'].data;
                            var idx = array[elementIndex]['_index'];
                            var year = chartData.labels[idx];
                            var label = chartData.datasets[elementIndex].label;
                            DashboardBarChartTableBind(dateFrom, dateTo, "SubCategory", label, year);
                        },
                        animation: {
                            onComplete: function () {
                                var url_base64 = document.getElementById("subCategoryChart").toDataURL("image/png");
                                ChartBase64Image(url_base64, "Sub-Category wise");
                                this.options.animation.onComplete = null;
                            }
                        },
                    }
                });
            }
            else {
                $('#divSubCategoryChart').css('display', 'none');
            }
            //Region Wise Bar Chart
            if (result.regionPiBarCharts != null) {
                var Years = []; var arrData = [];
                var sum = 0; var sum1 = 0; var vale = []; var vale1 = [];
                var Values1 = []; var Values2 = [];
                $.each(result.regionPiBarCharts, function (i) {
                    if (Years.indexOf(result.regionPiBarCharts[i].Year) === -1) {
                        Years.push(result.regionPiBarCharts[i].Year);
                    }
                    vale.push(Number(result.regionPiBarCharts[i].Value1));
                    vale1.push(Number(result.regionPiBarCharts[i].Value2));
                    sum += Number(result.regionPiBarCharts[i].Value1);
                    sum1 += Number(result.regionPiBarCharts[i].Value2);
                    //arrData.push({
                    //    label: result.regionPiBarCharts[i].Label, backgroundColor: getRandomColorHex(), data: [result.regionPiBarCharts[i].Value2, result.regionPiBarCharts[i].Value1], datalabels: { anchor: 'center' }
                    //})
                });

                $.each(vale, function (i) {
                    Values1.push(Number(vale[i] * 100 / sum));

                });
                $.each(vale1, function (i) {
                    var secondValue = Number(vale1[i] * 100 / sum1) || 0;
                    Values2.push(secondValue);

                });
                $.each(result.regionPiBarCharts, function (i) {
                    arrData.push({
                        label: result.regionPiBarCharts[i].Label, backgroundColor: getRandomColorHex(),
                        data: [Values2[i], Values1[i]],
                        datalabels: { anchor: 'center' }

                    })
                });




                var ctx3 = document.getElementById("regionChart").getContext('2d');
                if (myChart3 != null) {
                    myChart3.destroy();
                }
                myChart3 = new Chart(ctx3, {
                    type: 'bar',
                    data: {
                        labels: [yearBack, yearCurrent],
                        datasets: arrData
                    },
                    // Configuration options go here
                    options: {
                        scales: {
                            yAxes: [{
                                ticks: {
                                    beginAtZero: true
                                }
                            }]
                        },
                        legend: {
                            position: 'bottom',

                        },
                        tooltips: {
                            enabled: true
                        },
                        plugins: {
                            datalabels: {
                                color: '#fff',
                                formatter: (value, ctx) => {
                                    debugger
                                    let index = ctx.datasetIndex === 0 ? 1 : 0;
                                    let total = ctx.chart.data.datasets[index].data[ctx.dataIndex] + value;
                                    let percentage = Math.floor(value / total * 100) + "%";
                                    return percentage;

                                },
                            }
                        },
                        onClick: function (event, array) {
                            var active = window.myChart3.getElementAtEvent(event);
                            var elementIndex = active[0]._datasetIndex;
                            var chartData = array[elementIndex]['_chart'].data;
                            var idx = array[elementIndex]['_index'];
                            var year = chartData.labels[idx];
                            var label = chartData.datasets[elementIndex].label;
                            DashboardBarChartTableBind(dateFrom, dateTo, "Region", label, year);
                        },
                        animation: {
                            onComplete: function () {
                                var url_base64 = document.getElementById("regionChart").toDataURL("image/png");
                                ChartBase64Image(url_base64, "Region wise");
                                this.options.animation.onComplete = null;
                            }
                        },
                    }
                });
            }
            else {
                $('#regionChart').css('display', 'none');
            }
            //    Office Wise Bar Chart
            if (result.officePiBarCharts != null) {
                var Years = []; var arrData = [];
                var sum = 0; var sum1 = 0; var vale = []; var vale1 = [];
                var Values1 = []; var Values2 = [];
                $.each(result.officePiBarCharts, function (i) {
                    if (Years.indexOf(result.officePiBarCharts[i].Year) === -1) {
                        Years.push(result.officePiBarCharts[i].Year);
                    }
                    vale.push(parseInt(result.officePiBarCharts[i].Value1));
                    vale1.push(parseInt(result.officePiBarCharts[i].Value2));
                    sum += parseInt(result.officePiBarCharts[i].Value1);
                    sum1 += parseInt(result.officePiBarCharts[i].Value2);
                    //arrData.push({
                    //    label: result.officePiBarCharts[i].Label, backgroundColor: getRandomColorHex(), data: [result.officePiBarCharts[i].Value2, result.officePiBarCharts[i].Value1], datalabels: { anchor: 'center' }
                    //})
                });

                $.each(vale, function (i) {
                    Values1.push(parseInt(vale[i] * 100 / sum));

                });
                $.each(vale1, function (i) {
                    var secondValue = Number(vale1[i] * 100 / sum1) || 0;
                    Values2.push(secondValue);


                });
                $.each(result.officePiBarCharts, function (i) {
                    arrData.push({
                        label: result.officePiBarCharts[i].Label, backgroundColor: getRandomColorHex(),
                        data: [Values2[i], Values1[i]],
                        datalabels: { anchor: 'center' }

                    })
                });



                var ctx4 = document.getElementById("officeChart").getContext('2d');
                if (myChart4 != null) {
                    myChart4.destroy();
                }
                myChart4 = new Chart(ctx4, {
                    type: 'bar',
                    data: {
                        labels: [yearBack, yearCurrent],
                        datasets: arrData
                    },
                    //      Configuration options go here
                    options: {
                        scales: {
                            yAxes: [{
                                ticks: {
                                    beginAtZero: true
                                }
                            }]
                        },
                        legend: {
                            position: 'bottom',

                        },
                        tooltips: {
                            enabled: true
                        },

                        plugins: {
                            datalabels: {
                                color: '#fff',
                                formatter: (value, ctx) => {

                                    let index = ctx.datasetIndex === 0 ? 1 : 0;
                                    let total = ctx.chart.data.datasets[index].data[ctx.dataIndex] + value;
                                    let percentage = Math.floor(value / total * 100) + "%";
                                    return percentage;



                                },
                            }
                        },
                        onClick: function (event, array) {
                            var active = window.myChart4.getElementAtEvent(event);
                            var elementIndex = active[0]._datasetIndex;
                            var chartData = array[elementIndex]['_chart'].data;
                            var idx = array[elementIndex]['_index'];
                            var year = chartData.labels[idx];
                            var label = chartData.datasets[elementIndex].label;
                            DashboardBarChartTableBind(dateFrom, dateTo, "Office", label, year);
                        },
                        animation: {
                            onComplete: function () {
                                var url_base64 = document.getElementById("officeChart").toDataURL("image/png");
                                ChartBase64Image(url_base64, "Office wise");
                                this.options.animation.onComplete = null;
                            }
                        },
                    }
                });
            }
            else {
                $('#officeChart').css('display', 'none');
            }
            // LOS Wise Pi Chart
            if (result.losPiBarCharts != null) {
                var Years = []; var arrData = [];
                var sum = 0; var sum1 = 0; var vale = []; var vale1 = [];
                var Values1 = []; var Values2 = [];
                $.each(result.losPiBarCharts, function (i) {
                    if (Years.indexOf(result.losPiBarCharts[i].Year) === -1) {
                        Years.push(result.losPiBarCharts[i].Year);
                    }
                    vale.push(parseInt(result.losPiBarCharts[i].Value1));
                    vale1.push(parseInt(result.losPiBarCharts[i].Value2));
                    sum += parseInt(result.losPiBarCharts[i].Value1);
                    sum1 += parseInt(result.losPiBarCharts[i].Value2);
                    //arrData.push({
                    //    label: result.losPiBarCharts[i].Label, backgroundColor: getRandomColorHex(), data: [result.losPiBarCharts[i].Value2, result.losPiBarCharts[i].Value1], datalabels: { anchor: 'center' }
                    //})
                });


                $.each(vale, function (i) {
                    Values1.push(parseInt(vale[i] * 100 / sum));

                });
                $.each(vale1, function (i) {
                    var secondValue = Number(vale1[i] * 100 / sum1) || 0;
                    Values2.push(secondValue);


                });
                $.each(result.losPiBarCharts, function (i) {
                    arrData.push({
                        label: result.losPiBarCharts[i].Label, backgroundColor: getRandomColorHex(),
                        data: [Values2[i], Values1[i]],
                        datalabels: { anchor: 'center' }

                    })
                });


                var ctx5 = document.getElementById("losChart").getContext('2d');
                if (myChart5 != null) {
                    myChart5.destroy();
                }
                myChart5 = new Chart(ctx5, {
                    type: 'bar',
                    data: {
                        labels: [yearBack, yearCurrent],
                        datasets: arrData
                    },
                    //  Configuration options go here
                    options: {
                        scales: {
                            yAxes: [{
                                ticks: {
                                    beginAtZero: true
                                }
                            }]
                        },
                        legend: {
                            position: 'bottom',

                        },
                        tooltips: {
                            enabled: true
                        },
                        plugins: {
                            datalabels: {
                                color: '#fff',
                                formatter: (value, ctx) => {

                                    let index = ctx.datasetIndex === 0 ? 1 : 0;
                                    let total = ctx.chart.data.datasets[index].data[ctx.dataIndex] + value;
                                    let percentage = Math.floor(value / total * 100) + "%";
                                    return percentage;


                                },

                            }
                        },
                        onClick:
                            function (event, array) {
                                var active = window.myChart5.getElementAtEvent(event);
                                var elementIndex = active[0]._datasetIndex;
                                var chartData = array[elementIndex]['_chart'].data;
                                var idx = array[elementIndex]['_index'];
                                var year = chartData.labels[idx];
                                var label = chartData.datasets[elementIndex].label;
                                DashboardBarChartTableBind(dateFrom, dateTo, "LOS", label, year);
                            },
                        animation: {
                            onComplete: function () {
                                var url_base64 = document.getElementById("losChart").toDataURL("image/png");
                                ChartBase64Image(url_base64, "LOS wise");
                                this.options.animation.onComplete = null;
                            }
                        },
                    }
                });
            }
            else {
                $('#divLosChart').css('display', 'none');
            }
            // SBU Wise Pi Chart
            if (result.sbuPiBarCharts != null) {
                var Years = []; var arrData = [];
                var sum = 0; var sum1 = 0; var vale = []; var vale1 = [];
                var Values1 = []; var Values2 = [];
                $.each(result.sbuPiBarCharts, function (i) {
                    if (Years.indexOf(result.sbuPiBarCharts[i].Year) === -1) {
                        Years.push(result.sbuPiBarCharts[i].Year);
                    }
                    vale.push(parseInt(result.sbuPiBarCharts[i].Value1));
                    vale1.push(parseInt(result.sbuPiBarCharts[i].Value2));
                    sum += parseInt(result.sbuPiBarCharts[i].Value1);
                    sum1 += parseInt(result.sbuPiBarCharts[i].Value2);
                    //arrData.push({
                    //    label: result.sbuPiBarCharts[i].Label, backgroundColor: getRandomColorHex(), data: [result.sbuPiBarCharts[i].Value2, result.sbuPiBarCharts[i].Value1], datalabels: { anchor: 'center' }
                    //})
                });

                $.each(vale, function (i) {
                    Values1.push(parseInt(vale[i] * 100 / sum));

                });
                $.each(vale1, function (i) {
                    var secondValue = Number(vale1[i] * 100 / sum1) || 0;
                    Values2.push(secondValue);


                });
                $.each(result.sbuPiBarCharts, function (i) {
                    arrData.push({
                        label: result.sbuPiBarCharts[i].Label, backgroundColor: getRandomColorHex(),
                        data: [Values2[i], Values1[i]],
                        datalabels: { anchor: 'center' }

                    })
                });
                var ctx6 = document.getElementById("sbuChart").getContext('2d');
                if (myChart6 != null) {
                    myChart6.destroy();
                }
                myChart6 = new Chart(ctx6, {
                    type: 'bar',
                    data: {
                        labels: [yearBack, yearCurrent],
                        datasets: arrData
                    },
                    //   Configuration options go here
                    options: {
                        scales: {
                            yAxes: [{
                                ticks: {
                                    beginAtZero: true
                                }
                            }]
                        },
                        legend: {
                            position: 'bottom',

                        },
                        tooltips: {
                            enabled: true
                        },
                        plugins: {
                            datalabels: {
                                color: '#fff',
                                formatter: (value, ctx) => {

                                    let index = ctx.datasetIndex === 0 ? 1 : 0;
                                    let total = ctx.chart.data.datasets[index].data[ctx.dataIndex] + value;
                                    let percentage = Math.floor(value / total * 100) + "%";
                                    return percentage;


                                },
                            }
                        },
                        onClick:
                            function (event, array) {
                                var active = window.myChart6.getElementAtEvent(event);
                                var elementIndex = active[0]._datasetIndex;
                                var chartData = array[elementIndex]['_chart'].data;
                                var idx = array[elementIndex]['_index'];
                                var year = chartData.labels[idx];
                                var label = chartData.datasets[elementIndex].label;
                                DashboardBarChartTableBind(dateFrom, dateTo, "SBU", label, year);
                            },
                        animation: {
                            onComplete: function () {
                                var url_base64 = document.getElementById("sbuChart").toDataURL("image/png");
                                ChartBase64Image(url_base64, "SBU wise");
                                this.options.animation.onComplete = null;
                            }
                        },
                    }
                });
            }
            else {
                $('#divSbuChart').css('display', 'none');
            }
            //  Sub-SBU Wise Bar Chart
            if (result.subSBUPiBarCharts != null) {
                var Years = []; var arrData = [];
                var sum = 0; var sum1 = 0; var vale = []; var vale1 = [];
                var Values1 = []; var Values2 = [];
                $.each(result.subSBUPiBarCharts, function (i) {
                    if (Years.indexOf(result.subSBUPiBarCharts[i].Year) === -1) {
                        Years.push(result.subSBUPiBarCharts[i].Year);
                    }
                    vale.push(parseInt(result.subSBUPiBarCharts[i].Value1));
                    vale1.push(parseInt(result.subSBUPiBarCharts[i].Value2));
                    sum += parseInt(result.subSBUPiBarCharts[i].Value1);
                    sum1 += parseInt(result.subSBUPiBarCharts[i].Value2);
                    //arrData.push({
                    //    label: result.subSBUPiBarCharts[i].Label, backgroundColor: getRandomColorHex(), data: [result.subSBUPiBarCharts[i].Value2, result.subSBUPiBarCharts[i].Value1], datalabels: { anchor: 'center' }
                    //})
                });

                $.each(vale, function (i) {
                    Values1.push(parseInt(vale[i] * 100 / sum));

                });
                $.each(vale1, function (i) {
                    var secondValue = Number(vale1[i] * 100 / sum1) || 0;
                    Values2.push(secondValue);

                });
                $.each(result.subSBUPiBarCharts, function (i) {
                    arrData.push({
                        label: result.subSBUPiBarCharts[i].Label, backgroundColor: getRandomColorHex(),
                        data: [Values2[i], Values1[i]],
                        datalabels: { anchor: 'center' }

                    })
                });



                var ctx7 = document.getElementById("subSBUChart").getContext('2d');
                if (myChart7 != null) {
                    myChart7.destroy();
                }
                myChart7 = new Chart(ctx7, {
                    type: 'bar',
                    data: {
                        labels: [yearBack, yearCurrent],
                        datasets: arrData
                    },
                    //       Configuration options go here
                    options: {
                        scales: {
                            yAxes: [{
                                ticks: {
                                    beginAtZero: true
                                }
                            }]
                        },
                        legend: {
                            position: 'bottom',

                        },
                        tooltips: {
                            enabled: true
                        },
                        plugins: {
                            datalabels: {
                                color: '#fff',
                                formatter: (value, ctx) => {

                                    let index = ctx.datasetIndex === 0 ? 1 : 0;
                                    let total = ctx.chart.data.datasets[index].data[ctx.dataIndex] + value;
                                    let percentage = Math.floor(value / total * 100) + "%";
                                    return percentage;


                                },
                            }
                        },
                        onClick:
                            function (event, array) {
                                var active = window.myChart7.getElementAtEvent(event);
                                var elementIndex = active[0]._datasetIndex;
                                var chartData = array[elementIndex]['_chart'].data;
                                var idx = array[elementIndex]['_index'];
                                var year = chartData.labels[idx];
                                var label = chartData.datasets[elementIndex].label;
                                DashboardBarChartTableBind(dateFrom, dateTo, "SubSBU", label, year);
                            },
                        animation: {
                            onComplete: function () {
                                var url_base64 = document.getElementById("subSBUChart").toDataURL("image/png");
                                ChartBase64Image(url_base64, "Sub-SBU wise");
                                this.options.animation.onComplete = null;
                            }
                        },
                    }
                });
            }
            else {
                $('#divSubSBUChart').css('display', 'none');
            }
            //    Gender of Complainant Wise Bar Chart
            if (result.genderofComplainant != null) {
                var Years = []; var arrData = [];
                var sum = 0; var sum1 = 0; var vale = []; var vale1 = [];
                var Values1 = []; var Values2 = [];
                $.each(result.genderofComplainantPiBarCharts, function (i) {
                    if (Years.indexOf(result.genderofComplainantPiBarCharts[i].Year) === -1) {
                        Years.push(result.genderofComplainantPiBarCharts[i].Year);
                    }
                    vale.push(parseInt(result.genderofComplainantPiBarCharts[i].Value1));
                    vale1.push(parseInt(result.genderofComplainantPiBarCharts[i].Value2));
                    sum += parseInt(result.genderofComplainantPiBarCharts[i].Value1);
                    sum1 += parseInt(result.genderofComplainantPiBarCharts[i].Value2);

                    //arrData.push({
                    //    label: result.genderofComplainantPiBarCharts[i].Label, backgroundColor: getRandomColorHex(), data: [result.genderofComplainantPiBarCharts[i].Value2, result.genderofComplainantPiBarCharts[i].Value1], datalabels: { anchor: 'center' }
                    //})
                });

                $.each(vale, function (i) {
                    Values1.push(parseInt(vale[i] * 100 / sum));

                });
                $.each(vale1, function (i) {
                    Values2.push(parseInt(vale1[i] * 100 / sum1));

                });
                $.each(result.genderofComplainantPiBarCharts, function (i) {
                    arrData.push({
                        label: result.genderofComplainantPiBarCharts[i].Label, backgroundColor: getRandomColorHex(),
                        data: [Values2[i], Values1[i]],
                        datalabels: { anchor: 'center' }

                    })
                });


                var ctx8 = document.getElementById("genderOfComplainantChart").getContext('2d');
                if (myChart8 != null) {
                    myChart8.destroy();
                }
                myChart8 = new Chart(ctx8, {
                    type: 'bar',
                    data: {
                        labels: [yearBack, yearCurrent],
                        datasets: arrData
                    },
                    //   Configuration options go here
                    options: {
                        scales: {
                            yAxes: [{
                                ticks: {
                                    beginAtZero: true
                                }
                            }]
                        },
                        legend: {
                            position: 'bottom',

                        },
                        tooltips: {
                            enabled: true
                        },
                        plugins: {
                            datalabels: {
                                color: '#fff',
                                formatter: (value, ctx) => {

                                    let index = ctx.datasetIndex === 0 ? 1 : 0;
                                    let total = ctx.chart.data.datasets[index].data[ctx.dataIndex] + value;
                                    let percentage = Math.floor(value / total * 100) + "%";
                                    return percentage;


                                },
                            }
                        },
                        onClick:
                            function (event, array) {
                                var active = window.myChart8.getElementAtEvent(event);
                                var elementIndex = active[0]._datasetIndex;
                                var chartData = array[elementIndex]['_chart'].data;
                                var idx = array[elementIndex]['_index'];
                                var year = chartData.labels[idx];
                                var label = chartData.datasets[elementIndex].label;
                                DashboardBarChartTableBind(dateFrom, dateTo, "GenderofComplainant", label, year);
                            },
                        animation: {
                            onComplete: function () {
                                var url_base64 = document.getElementById("genderOfComplainantChart").toDataURL("image/png");
                                ChartBase64Image(url_base64, "Gender of Complainant");
                                this.options.animation.onComplete = null;
                            }
                        },
                    }
                });
            }
            else {
                $('#divGenderOfComplainantChart').css('display', 'none');
            }
            //  Gender of Respondent Wise Bar Chart
            if (result.genderofRespondentPiBarCharts != null) {
                var Years = []; var arrData = [];
                var sum = 0; var sum1 = 0; var vale = []; var vale1 = [];
                var Values1 = []; var Values2 = [];
                $.each(result.genderofRespondentPiBarCharts, function (i) {
                    if (Years.indexOf(result.genderofRespondentPiBarCharts[i].Year) === -1) {
                        Years.push(result.genderofRespondentPiBarCharts[i].Year);
                    }
                    vale.push(parseInt(result.genderofRespondentPiBarCharts[i].Value1));
                    vale1.push(parseInt(result.genderofRespondentPiBarCharts[i].Value2));
                    sum += parseInt(result.genderofRespondentPiBarCharts[i].Value1);
                    sum1 += parseInt(result.genderofRespondentPiBarCharts[i].Value2);

                    //arrData.push({
                    //    label: result.genderofRespondentPiBarCharts[i].Label, backgroundColor: getRandomColorHex(), data: [result.genderofRespondentPiBarCharts[i].Value2, result.genderofRespondentPiBarCharts[i].Value1], datalabels: { anchor: 'center' }
                    //})
                });

                $.each(vale, function (i) {
                    Values1.push(parseInt(vale[i] * 100 / sum));

                });
                $.each(vale1, function (i) {
                    Values2.push(parseInt(vale1[i] * 100 / sum1));

                });
                $.each(result.genderofRespondentPiBarCharts, function (i) {
                    arrData.push({
                        label: result.genderofRespondentPiBarCharts[i].Label, backgroundColor: getRandomColorHex(),
                        data: [Values2[i], Values1[i]],
                        datalabels: { anchor: 'center' }

                    })
                });

                var ctx9 = document.getElementById("genderOfRespondentChart").getContext('2d');
                if (myChart9 != null) {
                    myChart9.destroy();
                }
                myChart9 = new Chart(ctx9, {
                    type: 'bar',
                    data: {
                        labels: [yearBack, yearCurrent],
                        datasets: arrData
                    },
                    //   Configuration options go here
                    options: {
                        scales: {
                            yAxes: [{
                                ticks: {
                                    beginAtZero: true
                                }
                            }]
                        },
                        legend: {
                            position: 'bottom',

                        },
                        tooltips: {
                            enabled: true
                        },
                        plugins: {
                            datalabels: {
                                color: '#fff',
                                formatter: (value, ctx) => {

                                    let index = ctx.datasetIndex === 0 ? 1 : 0;
                                    let total = ctx.chart.data.datasets[index].data[ctx.dataIndex] + value;
                                    let percentage = Math.floor(value / total * 100) + "%";
                                    return percentage;


                                },
                                font: {
                                    weight: 'bold'
                                },
                            }
                        },
                        onClick: function (event, array) {
                            var active = window.myChart9.getElementAtEvent(event);
                            var elementIndex = active[0]._datasetIndex;
                            var chartData = array[elementIndex]['_chart'].data;
                            var idx = array[elementIndex]['_index'];
                            var year = chartData.labels[idx];
                            var label = chartData.datasets[elementIndex].label;
                            DashboardBarChartTableBind(dateFrom, dateTo, "GenderofRespondent", label, year);
                        },
                        animation: {
                            onComplete: function () {
                                var url_base64 = document.getElementById("genderOfRespondentChart").toDataURL("image/png");
                                ChartBase64Image(url_base64, "Gender of Respondent");
                                this.options.animation.onComplete = null;
                            }
                        },
                    }
                });
            }
            else {
                $('#divGenderOfRespondentChart').css('display', 'none');
            }
            //    Designation of Complainant Wise Bar Chart
            if (result.designationofComplainantPiBarCharts != null) {
                var Years = []; var arrData = [];
                var sum = 0; var sum1 = 0; var vale = []; var vale1 = [];
                var Values1 = []; var Values2 = [];
                $.each(result.designationofComplainantPiBarCharts, function (i) {
                    if (Years.indexOf(result.designationofComplainantPiBarCharts[i].Year) === -1) {
                        Years.push(result.designationofComplainantPiBarCharts[i].Year);
                    }

                    vale.push(parseInt(result.designationofComplainantPiBarCharts[i].Value1));
                    vale1.push(parseInt(result.designationofComplainantPiBarCharts[i].Value2));
                    sum += parseInt(result.designationofComplainantPiBarCharts[i].Value1);
                    sum1 += parseInt(result.designationofComplainantPiBarCharts[i].Value2);

                    //arrData.push({
                    //    label: result.designationofComplainantPiBarCharts[i].Label, backgroundColor: getRandomColorHex(), data: [result.designationofComplainantPiBarCharts[i].Value2, result.designationofComplainantPiBarCharts[i].Value1], datalabels: { anchor: 'center' }
                    //})
                });

                $.each(vale, function (i) {
                    Values1.push(parseInt(vale[i] * 100 / sum));

                });
                $.each(vale1, function (i) {
                    Values2.push(parseInt(vale1[i] * 100 / sum1));

                });

                $.each(result.designationofComplainantPiBarCharts, function (i) {
                    arrData.push({
                        label: result.designationofComplainantPiBarCharts[i].Label, backgroundColor: getRandomColorHex(),
                        data: [Values2[i], Values1[i]],
                        datalabels: { anchor: 'center' }

                    })
                });

                var ctx10 = document.getElementById("designationOfComplainantChart").getContext('2d');
                if (myChart10 != null) {
                    myChart10.destroy();
                }
                myChart10 = new Chart(ctx10, {
                    type: 'bar',
                    data: {
                        labels: [yearBack, yearCurrent],
                        datasets: arrData
                    },
                    //   Configuration options go here
                    options: {
                        scales: {
                            yAxes: [{
                                ticks: {
                                    beginAtZero: true
                                }
                            }]
                        },
                        legend: {
                            position: 'bottom',

                        },
                        tooltips: {
                            enabled: true
                        },
                        plugins: {
                            datalabels: {
                                color: '#fff',
                                formatter: (value, ctx) => {

                                    let index = ctx.datasetIndex === 0 ? 1 : 0;
                                    let total = ctx.chart.data.datasets[index].data[ctx.dataIndex] + value;
                                    let percentage = Math.floor(value / total * 100) + "%";
                                    return percentage;


                                },
                            }
                        },
                        onClick: function (event, array) {
                            var active = window.myChart10.getElementAtEvent(event);
                            var elementIndex = active[0]._datasetIndex;
                            var chartData = array[elementIndex]['_chart'].data;
                            var idx = array[elementIndex]['_index'];
                            var year = chartData.labels[idx];
                            var label = chartData.datasets[elementIndex].label;
                            DashboardBarChartTableBind(dateFrom, dateTo, "DesignationofComplainant", label, year);
                        },
                        animation: {
                            onComplete: function () {
                                var url_base64 = document.getElementById("designationOfComplainantChart").toDataURL("image/png");
                                ChartBase64Image(url_base64, "Designation of Complainant");
                                this.options.animation.onComplete = null;
                            }
                        },
                    }
                });
            }
            else {
                $('#divDesignationOfComplainantChart').css('display', 'none');
            }
            // Designation of Respondent Wise Bar Chart
            if (result.designationofRespondentPiBarCharts != null) {
                var Years = []; var arrData = [];
                var sum = 0; var sum1 = 0; var vale = []; var vale1 = [];
                var Values1 = []; var Values2 = [];
                $.each(result.designationofRespondentPiBarCharts, function (i) {
                    if (Years.indexOf(result.designationofRespondentPiBarCharts[i].Year) === -1) {
                        Years.push(result.designationofRespondentPiBarCharts[i].Year);
                    }
                    vale.push(parseInt(result.designationofRespondentPiBarCharts[i].Value1));
                    vale1.push(parseInt(result.designationofRespondentPiBarCharts[i].Value2));
                    sum += parseInt(result.designationofRespondentPiBarCharts[i].Value1);
                    sum1 += parseInt(result.designationofRespondentPiBarCharts[i].Value2);
                    //arrData.push({
                    //    label: result.designationofRespondentPiBarCharts[i].Label, backgroundColor: getRandomColorHex(), data: [result.designationofRespondentPiBarCharts[i].Value2, result.designationofRespondentPiBarCharts[i].Value1], datalabels: { anchor: 'center' }
                    //})
                });

                $.each(vale, function (i) {
                    Values1.push(parseInt(vale[i] * 100 / sum));

                });
                $.each(vale1, function (i) {
                    Values2.push(parseInt(vale1[i] * 100 / sum1));

                });

                $.each(result.designationofRespondentPiBarCharts, function (i) {
                    arrData.push({
                        label: result.designationofRespondentPiBarCharts[i].Label, backgroundColor: getRandomColorHex(),
                        data: [Values2[i], Values1[i]],
                        datalabels: { anchor: 'center' }

                    })
                });

                var ctx11 = document.getElementById("designationOfRespondentChart").getContext('2d');
                if (myChart11 != null) {
                    myChart11.destroy();
                }
                myChart11 = new Chart(ctx11, {
                    type: 'bar',
                    data: {
                        labels: [yearBack, yearCurrent],
                        datasets: arrData
                    },
                    //   Configuration options go here
                    options: {
                        scales: {
                            yAxes: [{
                                ticks: {
                                    beginAtZero: true
                                }
                            }]
                        },
                        legend: {
                            position: 'bottom',

                        },
                        tooltips: {
                            enabled: true
                        },
                        plugins: {
                            datalabels: {
                                color: '#fff',
                                formatter: (value, ctx) => {

                                    let index = ctx.datasetIndex === 0 ? 1 : 0;
                                    let total = ctx.chart.data.datasets[index].data[ctx.dataIndex] + value;
                                    let percentage = Math.floor(value / total * 100) + "%";
                                    return percentage;


                                },
                            }
                        }
                        ,

                        onClick: function (event, array) {
                            var active = window.myChart11.getElementAtEvent(event);
                            var elementIndex = active[0]._datasetIndex;
                            var chartData = array[elementIndex]['_chart'].data;
                            var idx = array[elementIndex]['_index'];
                            var year = chartData.labels[idx];
                            var label = chartData.datasets[elementIndex].label;
                            DashboardBarChartTableBind(dateFrom, dateTo, "DesignationofRespondent", label, year);
                        },
                        animation: {
                            onComplete: function () {
                                var url_base64 = document.getElementById("designationOfRespondentChart").toDataURL("image/png");
                                ChartBase64Image(url_base64, "Designation of Respondent");
                                this.options.animation.onComplete = null;
                            }
                        },
                    }
                });
            }
            else {
                $('#divDesignationOfRespondentChart').css('display', 'none');
            }
            //   Designation of Respondent Wise Bar Chart
            if (result.modeofComplaintPiBarCharts != null) {
                var Years = []; var arrData = [];
                var sum = 0; var sum1 = 0; var vale = []; var vale1 = [];
                var Values1 = []; var Values2 = [];
                $.each(result.modeofComplaintPiBarCharts, function (i) {
                    if (Years.indexOf(result.modeofComplaintPiBarCharts[i].Year) === -1) {
                        Years.push(result.modeofComplaintPiBarCharts[i].Year);
                    }
                    vale.push(parseInt(result.modeofComplaintPiBarCharts[i].Value1));
                    vale1.push(parseInt(result.modeofComplaintPiBarCharts[i].Value2));
                    sum += parseInt(result.modeofComplaintPiBarCharts[i].Value1);
                    sum1 += parseInt(result.modeofComplaintPiBarCharts[i].Value2);
                    //arrData.push({
                    //    label: result.modeofComplaintPiBarCharts[i].Label, backgroundColor: getRandomColorHex(), data: [result.modeofComplaintPiBarCharts[i].Value2, result.modeofComplaintPiBarCharts[i].Value1], datalabels: { anchor: 'center' }
                    //})
                });
                $.each(vale, function (i) {
                    Values1.push(parseInt(vale[i] * 100 / sum));

                });
                $.each(vale1, function (i) {
                    Values2.push(parseInt(vale1[i] * 100 / sum1));

                });
                $.each(result.modeofComplaintPiBarCharts, function (i) {
                    arrData.push({
                        label: result.modeofComplaintPiBarCharts[i].Label, backgroundColor: getRandomColorHex(),
                        data: [Values2[i], Values1[i]],
                        datalabels: { anchor: 'center' }

                    })
                });



                var ctx12 = document.getElementById("modeofComplaintChart").getContext('2d');
                if (myChart12 != null) {
                    myChart12.destroy();
                }
                myChart12 = new Chart(ctx12, {
                    type: 'bar',
                    data: {
                        labels: [yearBack, yearCurrent],
                        datasets: arrData
                    },
                    //  Configuration options go here
                    options: {
                        scales: {
                            yAxes: [{
                                ticks: {
                                    beginAtZero: true
                                }
                            }]
                        },
                        legend: {
                            position: 'bottom',

                        },
                        tooltips: {
                            enabled: true
                        },
                        plugins: {
                            datalabels: {
                                color: '#fff',
                                formatter: (value, ctx) => {

                                    let index = ctx.datasetIndex === 0 ? 1 : 0;
                                    let total = ctx.chart.data.datasets[index].data[ctx.dataIndex] + value;
                                    let percentage = Math.floor(value / total * 100) + "%";
                                    return percentage;


                                },
                            }
                        },
                        onClick: function (event, array) {
                            var active = window.myChart12.getElementAtEvent(event);
                            var elementIndex = active[0]._datasetIndex;
                            var chartData = array[elementIndex]['_chart'].data;
                            var idx = array[elementIndex]['_index'];
                            var year = chartData.labels[idx];
                            var label = chartData.datasets[elementIndex].label;
                            DashboardBarChartTableBind(dateFrom, dateTo, "ModeofComplaint", label, year);
                        },
                        animation: {
                            onComplete: function () {
                                var url_base64 = document.getElementById("modeofComplaintChart").toDataURL("image/png");
                                ChartBase64Image(url_base64, "Mode of Complaint");
                                this.options.animation.onComplete = null;
                            }
                        },
                    }
                });
            }
            else {
                $('#divModeofComplaintChart').css('display', 'none');
            }
            //Ageing/Case Closure
            if (result.ageingPiBarCharts != null) {
                var Years = []; var arrData = [];
                var sum = 0; var sum1 = 0; var vale = []; var vale1 = [];
                var Values1 = []; var Values2 = [];
                $.each(result.ageingPiBarCharts, function (i) {
                    if (Years.indexOf(result.ageingPiBarCharts[i].Year) === -1) {
                        Years.push(result.ageingPiBarCharts[i].Year);
                    }
                    vale.push(parseInt(result.ageingPiBarCharts[i].Value1));
                    vale1.push(parseInt(result.ageingPiBarCharts[i].Value2));
                    sum += parseInt(result.ageingPiBarCharts[i].Value1);
                    sum1 += parseInt(result.ageingPiBarCharts[i].Value2);
                    //arrData.push({
                    //    label: result.ageingPiBarCharts[i].Label, backgroundColor: getRandomColorHex(), data: [result.ageingPiBarCharts[i].Value2, result.ageingPiBarCharts[i].Value1], datalabels: { anchor: 'center' }
                    //})
                });

                $.each(vale, function (i) {
                    Values1.push(parseInt(vale[i] * 100 / sum));

                });
                $.each(vale1, function (i) {
                    Values2.push(parseInt(vale1[i] * 100 / sum1));

                });

                $.each(result.ageingPiBarCharts, function (i) {
                    arrData.push({
                        label: result.ageingPiBarCharts[i].Label, backgroundColor: getRandomColorHex(),
                        data: [Values2[i], Values1[i]],
                        datalabels: { anchor: 'center' }

                    })
                });


                var ctx13 = document.getElementById("ageingChart").getContext('2d');
                if (myChart13 != null) {
                    myChart13.destroy();
                }
                myChart13 = new Chart(ctx13, {
                    type: 'bar',
                    data: {
                        labels: [yearBack, yearCurrent],
                        datasets: arrData
                    },
                    //Configuration options go here
                    options: {
                        scales: {
                            yAxes: [{
                                ticks: {
                                    beginAtZero: true
                                }
                            }]
                        },
                        legend: {
                            position: 'bottom',

                        },
                        tooltips: {
                            enabled: true
                        },
                        plugins: {
                            datalabels: {
                                color: '#fff',
                                formatter: (value, ctx) => {

                                    let index = ctx.datasetIndex === 0 ? 1 : 0;
                                    let total = ctx.chart.data.datasets[index].data[ctx.dataIndex] + value;
                                    let percentage = Math.floor(value / total * 100) + "%";
                                    return percentage;


                                },
                            }
                        },
                        onClick: function (event, array) {
                            var active = window.myChart13.getElementAtEvent(event);
                            var elementIndex = active[0]._datasetIndex;
                            var chartData = array[elementIndex]['_chart'].data;
                            var idx = array[elementIndex]['_index'];
                            var year = chartData.labels[idx];
                            var label = chartData.datasets[elementIndex].label;
                            DashboardBarChartTableBind(dateFrom, dateTo, "Ageing", label, year);
                        },
                        animation: {
                            onComplete: function () {
                                var url_base64 = document.getElementById("ageingChart").toDataURL("image/png");
                                ChartBase64Image(url_base64, "Ageing/Case Closure");
                                this.options.animation.onComplete = null;
                            }
                        },
                    }
                });
            }
            else {
                $('#divAgeingChart').css('display', 'none');
            }



        }
    });
}



