//var ctx1 = document.getElementById("categoryChart").getContext('2d');
//var myChart1 = null;

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
    if (currentUserrole.toLowerCase() == "lead" || currentUserrole.toLowerCase() == "admin") {
        let today = new Date();
        $('#txt_dateFrom').datepicker({
            dateFormat: 'dd/mm/yy',
            maxDate: 0,
            minDate: new Date(`01/01/${today.getUTCFullYear()-1}`)
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
        backYear = '01/01/'+backYear ;
        $('#txt_dateFrom').val(backYear);
        //$('#txt_dateFrom').prop('min', backYear)
        //$('#txt_dateTo').prop('min', backYear)
        //$('#txt_dateFrom').prop("max", `${today.getUTCFullYear()}-${(today.getUTCMonth() + 1).toString().padStart(2, "0")}-${today.getUTCDate().toString().padStart(2, "0")}`);
        //$('#txt_dateTo').prop("max", `${today.getUTCFullYear()}-${(today.getUTCMonth() + 1).toString().padStart(2, "0")}-${today.getUTCDate().toString().padStart(2, "0")}`);

    }
    
})

$(document).on('click', '#btn_dataSearch', function () {
    if ($('#ddlChart').val() == 'default') {
        $('#divAgeingChart').css('display', 'block');
        DashboardPiGraphData();
    }
    else {
        $('#divAgeingChart').css('display', 'none');
        //StartProcess();
        DashboardBarGraphData();
    }
})


function DashboardPiGraphData() {
    StartProcess();
    let dateFrom = ($('#txt_dateFrom').val());
    let dateTo = ($('#txt_dateTo').val());
    $.get('/home/DashboardPiChart', { dateFrom: dateFrom, dateTo: dateTo }, function (result) {
        console.log(result);
        StopProcess();
        //console.log(result.categoryPiCharts[1].Label);
        if (result != null && result != "undefined" && result != "") {
            //Case Type Pi chart
            if (result.caseTypeofComplaint != null) {//&& result.casePiChart.Actionable != '0' || result.casePiChart.NonActionable !='0') {

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
                                borderRadius: 25,
                                borderWidth: 2,
                                color: 'white',
                                font: {
                                    weight: 'bold'
                                },
                            }
                        }
                    }
                });
            }
            else {
               // $('#divCaseTypeChart').css('display', 'none');
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
                                borderRadius: 50,
                                borderWidth: 2,
                                color: 'white',
                                font: {
                                    weight: 'bold'
                                },

                                //formatter: Math.round
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
                    position: 'right',
                    type: 'doughnut',
                    data: {
                        labels: Labels,

                        datasets: [{
                            backgroundColor: Colors,
                            data: Values,
                            datalabels: {
                                anchor: 'start'
                            }
                        }]
                    },
                    // Configuration options go here
                    options: {
                        responsive: true,
                        legend: {
                            position: 'right',

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
                                borderRadius: 25,
                                borderWidth: 2,
                                color: 'white',
                                font: {
                                    weight: 'bold'
                                },
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
                    position: 'right',
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
                            position: 'right',

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
                                borderRadius: 25,
                                borderWidth: 2,
                                color: 'white',
                                font: {
                                    weight: 'bold'
                                },
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
                    position: 'right',
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
                            position: 'right',

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
                                borderRadius: 25,
                                borderWidth: 2,
                                color: 'white',
                                font: {
                                    weight: 'bold'
                                },
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
                    position: 'right',
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
                            position: 'right',

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
                                borderRadius: 25,
                                borderWidth: 2,
                                color: 'white',
                                font: {
                                    weight: 'bold'
                                },
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
                    position: 'right',
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
                            position: 'right',

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
                                borderRadius: 25,
                                borderWidth: 2,
                                color: 'white',
                                font: {
                                    weight: 'bold'
                                },
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
                    position: 'right',
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
                            position: 'right',

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
                                borderRadius: 25,
                                borderWidth: 2,
                                color: 'white',
                                font: {
                                    weight: 'bold'
                                },
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
                    position: 'right',
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
                            position: 'right',

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
                                borderRadius: 25,
                                borderWidth: 2,
                                color: 'white',
                                font: {
                                    weight: 'bold'
                                },
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
                    position: 'right',
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
                            position: 'right',

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
                                borderRadius: 25,
                                borderWidth: 2,
                                color: 'white',
                                font: {
                                    weight: 'bold'
                                },
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
                    position: 'right',
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
                            position: 'right',

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
                                borderRadius: 25,
                                borderWidth: 2,
                                color: 'white',
                                font: {
                                    weight: 'bold'
                                },
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
                    position: 'right',
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
                            position: 'right',

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
                                borderRadius: 25,
                                borderWidth: 2,
                                color: 'white',
                                font: {
                                    weight: 'bold'
                                },
                            }
                        }
                    }
                });
            }
            else {
                $('#divDesignationOfRespondentChart').css('display', 'none');
            }
            //Designation of Respondent Wise Pi Chart
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
                    position: 'right',
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
                            position: 'right',

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
                                borderRadius: 25,
                                borderWidth: 2,
                                color: 'white',
                                font: {
                                    weight: 'bold'
                                },
                            }
                        }
                    }
                });
            }
            else {
                $('#divModeofComplaintChart').css('display', 'none');
            }
            //Category Wise Pi Chart
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
                    position: 'right',
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
                            position: 'right',

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
                                borderRadius: 50,
                                borderWidth: 2,
                                color: 'white',
                                font: {
                                    weight: 'bold'
                                },

                                //formatter: Math.round
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
    //StartProcess();
    let dateFrom = ($('#txt_dateFrom').val());
    let dateTo = ($('#txt_dateTo').val());
    
    var yearCurrent = new Date();
    yearCurrent = yearCurrent.getFullYear();
    let yearBack = new Date();
    yearBack = (yearBack.getFullYear() - 1);


    $.get('/home/DashboardBarChart', { dateFrom: dateFrom, dateTo: dateTo }, function (result) {
        console.log(result);
        //console.log(result.categoryPiCharts[1].Label);
        if (result != null && result != "undefined" && result != "") {
            //Case Type Bar chart
            if (result.caseTypeofComplaint1 != null) {
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
                    position: 'right',
                    type: 'bar',
                    //data: result,
                    data: {
                        labels: Years, //["Actionable", "Non-Actionable"],
                        datasets: [{
                            label: "Actionable",
                            backgroundColor: "rgb(54, 162, 235)",
                            data: Values1,
                            datalabels: {
                                anchor: 'end'
                            }
                        },
                        {
                            label: "Non-Actionable",
                            backgroundColor: "rgb(255, 133, 102)",
                            data: Values2,
                            datalabels: {
                                anchor: 'end'
                            }
                        }]
                    },
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
                                align: 'end',
                                anchor: 'end',
                                color: function (context) {
                                    return context.dataset.backgroundColor;
                                },
                                font: {
                                    weight: 'bold'
                                },
                                //formatter: function (value, context) {
                                //    return context.chart.data.labels[context.dataIndex];
                                //}
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
                        label: result.categoryPiBarCharts[i].Label, backgroundColor: getRandomColorHex(), data: [result.categoryPiBarCharts[i].Value2, result.categoryPiBarCharts[i].Value1], datalabels: { anchor: 'end' }
                    })
                });
                var ctx1 = document.getElementById("categoryChart").getContext('2d');
                if (myChart1 != null) {
                    myChart1.destroy();
                }
                myChart1 = new Chart(ctx1, {
                    position: 'right',
                    type: 'bar',
                    data: {
                        labels: [yearBack, yearCurrent],
                        datasets: arrData
                    },
                    // Configuration options go here
                    options: {
                        legend: {
                            "position": "bottom"

                        },
                        tooltips: {
                            enabled: true
                        },
                        plugins: {
                            datalabels: {
                                align: 'end',
                                anchor: 'end',
                                color: function (context) {
                                    return context.dataset.backgroundColor;
                                },
                                font: {
                                    weight: 'bold'
                                },
                            }
                        }
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
                        label: result.subCategoryPiBarCharts[i].Label, backgroundColor: getRandomColorHex(), data: [result.subCategoryPiBarCharts[i].Value2, result.subCategoryPiBarCharts[i].Value1], datalabels: { anchor: 'end' }
                    })
                });
                var ctx2 = document.getElementById("subCategoryChart").getContext('2d');
                if (myChart2 != null) {
                    myChart2.destroy();
                }
                myChart2 = new Chart(ctx2, {
                    position: 'right',
                    type: 'bar',
                    data: {
                        labels: [yearBack, yearCurrent],
                        datasets: arrData
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
                        plugins: {
                            datalabels: {
                                align: 'end',
                                anchor: 'end',
                                color: function (context) {
                                    return context.dataset.backgroundColor;
                                },
                                font: {
                                    weight: 'bold'
                                },
                            }
                        }
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
                        label: result.regionPiBarCharts[i].Label, backgroundColor: getRandomColorHex(), data: [result.regionPiBarCharts[i].Value2, result.regionPiBarCharts[i].Value1], datalabels: { anchor: 'end' }
                    })
                });
                var ctx3 = document.getElementById("regionChart").getContext('2d');
                if (myChart3 != null) {
                    myChart3.destroy();
                }
                myChart3 = new Chart(ctx3, {
                    position: 'right',
                    type: 'bar',
                    data: {
                        labels: [yearBack,yearCurrent],
                        datasets: arrData
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
                        plugins: {
                            datalabels: {
                                align: 'end',
                                anchor: 'end',
                                color: function (context) {
                                    return context.dataset.backgroundColor;
                                },
                                font: {
                                    weight: 'bold'
                                },
                            }
                        }
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
                        label: result.officePiBarCharts[i].Label, backgroundColor: getRandomColorHex(), data: [result.officePiBarCharts[i].Value2, result.officePiBarCharts[i].Value1], datalabels: { anchor: 'end' }
                    })
                });
                var ctx4 = document.getElementById("officeChart").getContext('2d');
                if (myChart4 != null) {
                    myChart4.destroy();
                }
                myChart4 = new Chart(ctx4, {
                    position: 'right',
                    type: 'bar',
                    data: {
                        labels: [yearBack, yearCurrent],
                        datasets: arrData
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
                        
                        plugins: {
                            datalabels: {
                                align: 'end',
                                anchor: 'end',
                                color: function (context) {
                                    return context.dataset.backgroundColor;
                                },
                                font: {
                                    weight: 'bold'
                                },
                            }
                        }
                    }
                });
            }
            else {
                $('#officeChart').css('display', 'none');
            }
            //LOS Wise Pi Chart
            console.log(result.losPiBarCharts);
            if (result.losPiBarCharts != null) {
                var Years = []; var arrData = [];
                $.each(result.losPiBarCharts, function (i) {
                    if (Years.indexOf(result.losPiBarCharts[i].Year) === -1) {
                        Years.push(result.losPiBarCharts[i].Year);
                    }
                    arrData.push({
                        label: result.losPiBarCharts[i].Label, backgroundColor: getRandomColorHex(), data: [result.losPiBarCharts[i].Value2, result.losPiBarCharts[i].Value1], datalabels: { anchor: 'end' }
                    })
                });
                var ctx5 = document.getElementById("losChart").getContext('2d');
                if (myChart5 != null) {
                    myChart5.destroy();
                }
                myChart5 = new Chart(ctx5, {
                    position: 'right',
                    type: 'bar',
                    data: {
                        labels: [yearBack, yearCurrent],
                        datasets: arrData
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
                        plugins: {
                            datalabels: {
                                align: 'end',
                                anchor: 'end',
                                color: function (context) {
                                    return context.dataset.backgroundColor;
                                },
                                font: {
                                    weight: 'bold'
                                },
                            }
                        }
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
                        label: result.sbuPiBarCharts[i].Label, backgroundColor: getRandomColorHex(), data: [result.sbuPiBarCharts[i].Value2, result.sbuPiBarCharts[i].Value1], datalabels: { anchor: 'end' }
                    })
                });
                var ctx6 = document.getElementById("sbuChart").getContext('2d');
                if (myChart6 != null) {
                    myChart6.destroy();
                }
                myChart6 = new Chart(ctx6, {
                    position: 'right',
                    type: 'bar',
                    data: {
                        labels: [yearBack, yearCurrent],
                        datasets: arrData
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
                        plugins: {
                            datalabels: {
                                align: 'end',
                                anchor: 'end',
                                color: function (context) {
                                    return context.dataset.backgroundColor;
                                },
                                font: {
                                    weight: 'bold'
                                },
                            }
                        }
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
                        label: result.subSBUPiBarCharts[i].Label, backgroundColor: getRandomColorHex(), data: [result.subSBUPiBarCharts[i].Value2, result.subSBUPiBarCharts[i].Value1], datalabels: { anchor: 'end' }
                    })
                });
                var ctx7 = document.getElementById("subSBUChart").getContext('2d');
                if (myChart7 != null) {
                    myChart7.destroy();
                }
                myChart7 = new Chart(ctx7, {
                    position: 'right',
                    type: 'bar',
                    data: {
                        labels: [yearBack, yearCurrent],
                        datasets: arrData
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
                        plugins: {
                            datalabels: {
                                align: 'end',
                                anchor: 'end',
                                color: function (context) {
                                    return context.dataset.backgroundColor;
                                },
                                font: {
                                    weight: 'bold'
                                },
                            }
                        }
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
                        label: result.genderofComplainantPiBarCharts[i].Label, backgroundColor: getRandomColorHex(), data: [result.genderofComplainantPiBarCharts[i].Value2, result.genderofComplainantPiBarCharts[i].Value1], datalabels: { anchor: 'end' }
                    })
                });
                var ctx8 = document.getElementById("genderOfComplainantChart").getContext('2d');
                if (myChart8 != null) {
                    myChart8.destroy();
                }
                myChart8 = new Chart(ctx8, {
                    position: 'right',
                    type: 'bar',
                    data: {
                        labels: [yearBack, yearCurrent],
                        datasets: arrData
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
                        plugins: {
                            datalabels: {
                                align: 'end',
                                anchor: 'end',
                                color: function (context) {
                                    return context.dataset.backgroundColor;
                                },
                                font: {
                                    weight: 'bold'
                                },
                            }
                        }
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
                        label: result.genderofRespondentPiBarCharts[i].Label, backgroundColor: getRandomColorHex(), data: [result.genderofRespondentPiBarCharts[i].Value2, result.genderofRespondentPiBarCharts[i].Value1], datalabels: { anchor: 'end' }
                    })
                });
                var ctx9 = document.getElementById("genderOfRespondentChart").getContext('2d');
                if (myChart9 != null) {
                    myChart9.destroy();
                }
                myChart9 = new Chart(ctx9, {
                    position: 'right',
                    type: 'bar',
                    data: {
                        labels: [yearBack, yearCurrent],
                        datasets: arrData
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
                        plugins: {
                            datalabels: {
                                align: 'end',
                                anchor: 'end',
                                color: function (context) {
                                    return context.dataset.backgroundColor;
                                },
                                font: {
                                    weight: 'bold'
                                },
                            }
                        }
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
                        label: result.designationofComplainantPiBarCharts[i].Label, backgroundColor: getRandomColorHex(), data: [result.designationofComplainantPiBarCharts[i].Value2, result.designationofComplainantPiBarCharts[i].Value1], datalabels: { anchor: 'end' }
                    })
                });
                var ctx10 = document.getElementById("designationOfComplainantChart").getContext('2d');
                if (myChart10 != null) {
                    myChart10.destroy();
                }
                myChart10 = new Chart(ctx10, {
                    position: 'right',
                    type: 'bar',
                    data: {
                        labels: [yearBack, yearCurrent],
                        datasets: arrData
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
                        plugins: {
                            datalabels: {
                                align: 'end',
                                anchor: 'end',
                                color: function (context) {
                                    return context.dataset.backgroundColor;
                                },
                                font: {
                                    weight: 'bold'
                                },
                            }
                        }
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
                        label: result.designationofRespondentPiBarCharts[i].Label, backgroundColor: getRandomColorHex(), data: [result.designationofRespondentPiBarCharts[i].Value2, result.designationofRespondentPiBarCharts[i].Value1], datalabels: { anchor: 'end' }
                    })
                });
                var ctx11 = document.getElementById("designationOfRespondentChart").getContext('2d');
                if (myChart11 != null) {
                    myChart11.destroy();
                }
                myChart11 = new Chart(ctx11, {
                    position: 'right',
                    type: 'bar',
                    data: {
                        labels: [yearBack, yearCurrent],
                        datasets: arrData
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
                        plugins: {
                            datalabels: {
                                align: 'end',
                                anchor: 'end',
                                color: function (context) {
                                    return context.dataset.backgroundColor;
                                },
                                font: {
                                    weight: 'bold'
                                },
                            }
                        }
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
                        label: result.modeofComplaintPiBarCharts[i].Label, backgroundColor: getRandomColorHex(), data: [result.modeofComplaintPiBarCharts[i].Value2, result.modeofComplaintPiBarCharts[i].Value1], datalabels: { anchor: 'end' }
                    })
                });
                var ctx12 = document.getElementById("modeofComplaintChart").getContext('2d');
                if (myChart12 != null) {
                    myChart12.destroy();
                }
                myChart12 = new Chart(ctx12, {
                    position: 'right',
                    type: 'bar',
                    data: {
                        labels: [yearBack, yearCurrent],
                        datasets: arrData
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
                        plugins: {
                            datalabels: {
                                align: 'end',
                                anchor: 'end',
                                color: function (context) {
                                    return context.dataset.backgroundColor;
                                },
                                font: {
                                    weight: 'bold'
                                },
                            }
                        }
                    }
                });
            }
            else {
                $('#divModeofComplaintChart').css('display', 'none');
            }
        }


    });
}