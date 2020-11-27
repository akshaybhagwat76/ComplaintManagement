function submitForm() {
   
    $("#lblError").removeClass("success").removeClass("adderror").text('');
    var retval = true;
    $("#myForm .required").each(function () {
        if (!$(this).val()) {
            var $label = $("<label class='adderror'>").text('This field is required.');
            if ($(this).parent().find("label").length == 1) {
                $(this).parent().append($label);

                $(this).addClass("adderror");
            }
            retval = false;
        }
        else {
            if ($(this).parent().find("label").length > 1) {
                $(this).parent().find("label:eq(1)").remove();
                $(this).removeClass("adderror");
            }
        }
    });

    var losFields = $("input[name='losList']").serializeArray();
    if (losFields.length === 0) {
        $("#lblErrorLOS").addClass("adderror").text('Please select atleast one LOS.');
    }
    else {
        $("#lblErrorLOS").removeClass("adderror").text('');
    }


    var sbufields = $("input[name='sbuList']").serializeArray();
    if (sbufields.length === 0) {
        $("#lblErrorSbu").addClass("adderror").text('Please select atleast one SBU.');
    }
    else {
        $("#lblErrorSbu").removeClass("adderror").text('');
    }

    var subsbufields = $("input[name='subsbuList']").serializeArray();
    if (subsbufields.length === 0) {
        $("#lblErrorSubSbu").addClass("adderror").text('Please select atleast one Sub SBU.');
    }
    else {
        $("#lblErrorSubSbu").removeClass("adderror").text('');
    }

    var Competencyfields = $("input[name='CompetencyList']").serializeArray();
    if (Competencyfields.length === 0) {
        $("#lblErrorCompetency").addClass("adderror").text('Please select atleast one Competency.');
    }
    else {
        $("#lblErrorCompetency").removeClass("adderror").text('');
    }

    if (retval && sbufields.length > 0 && subsbufields.length > 0 && Competencyfields.length > 0 && losFields.length>0) {
        var data = {
            Id: $("#Id").val().trim(),
            UserId: $("#UserId").val(),
            LOSId: $("#LOSIds").val().toString(),
            SBUId: $("#SBUIds").val().toString(),
            SubSBUId: $("#SubSBUIds").val().toString(),
            CompetencyId: $("#CompentencyIds").val().toString(),
            Status: $("#Status").val() == "true" ? true : false
        }

        data = JSON.stringify(data);
        StartProcess();
        $.ajax({
            type: "POST",
            url: "/Role/AddOrUpdateRole",
            data: { roleParams: data },
            success: function (response) {
                if (data.status == "Fail") {
                    StopProcess();
                    $("#lblError").addClass("adderror").text(data.error).show();
                }
                else {
                    window.location.href = '/Role/Index'
                }
            },
            error: function (error) {
                toastr.error(error)
            }
        });
    }
}

$(document).ready(function () {
    if ($("#Id").val() === "0") {
        $("#Status").val("true");
    }
    else {
        if ($("#SBUIds").val().includes(',')) {
            var sbuIds = $('#SBUIds').val().split(",")

            $.each(sbuIds, function (i, keyword) {
                var el = $("#sbu-" + keyword);
                if (el[0] != null) {
                    el.attr("checked", true)
                }
            });

            $("#SBUIds").val(sbuIds)
        }
        else {
            if ($("#SBUIds").val() != "") {
                var el = $("#sbu-" + $("#SBUIds").val());
                if (el[0] != null) {
                    el.attr("checked", true)
                }
            }
        }
        if ($("#LOSIds").val().includes(',')) {
            var LOSIds = $('#LOSIds').val().split(",")

            $.each(LOSIds, function (i, keyword) {
                var el = $("#los-" + keyword);
                if (el[0] != null) {
                    el.attr("checked", true)
                }
            });

            $("#LOSIds").val(sbuIds)
        }
        else {
            if ($("#LOSIds").val() != "") {
                var el = $("#los-" + $("#LOSIds").val());
                if (el[0] != null) {
                    el.attr("checked", true)
                }
            }
        }


        if ($("#SubSBUIds").val().includes(',')) {
            var subsbuIds = $('#SubSBUIds').val().split(",")

            $.each(subsbuIds, function (i, keyword) {
                var el = $("#subsbu-" + keyword);
                if (el[0] != null) {
                    el.attr("checked", true)
                }
            });

            $("#SubSBUIds").val(subsbuIds)
        }
        else {
            if ($("#SubSBUIds").val() != "") {
                var el = $("#subsbu-" + $("#SubSBUIds").val());
                if (el[0] != null) {
                    el.attr("checked", true)
                }
            }
        }

        if ($("#CompentencyIds").val().includes(',')) {
            var CompentencyIds = $('#CompentencyIds').val().split(",")

            $.each(CompentencyIds, function (i, keyword) {
                var el = $("#Competency-" + keyword);
                if (el[0] != null) {
                    el.attr("checked", true)
                }
            });

            $("#CompentencyIds").val(CompentencyIds)
        }
        else {
            if ($("#CompentencyIds").val() != "") {
                var el = $("#Competency-" + $("#CompentencyIds").val());
                if (el[0] != null) {
                    el.attr("checked", true)
                }
            }
        }

    }
  
    if ($("#pageState").val() != null && $("#pageState").val() != "") {
        let page_state = JSON.parse($("#pageState").val().toLowerCase());
        if (page_state) {
            $(".text-right").addClass("hide")
            $('.container-fluid').addClass("disabled-div");
        }
        else {
            $(".text-right").removeClass("hide")

            $('.container-fluid').removeClass("disabled-div");
        }
    }
});

$('.checks-subsbu').change(function () {
    var values = $('.checks-subsbu:checked').map(function () {
        return this.value;
    }).get().join(',');
    $('#SubSBUIds').val(values);
});


$('.checks-sbu').change(function () {
    var values = $('.checks-sbu:checked').map(function () {
        return this.value;
    }).get().join(',');
    $('#SBUIds').val(values);
});

$('.checks-Competency').change(function () {
    var values = $('.checks-Competency:checked').map(function () {
        return this.value;
    }).get().join(',');
    $('#CompentencyIds').val(values);
});

$('.checks-los').change(function () {
    var values = $('.checks-los:checked').map(function () {
        return this.value;
    }).get().join(',');
    $('#LOSIds').val(values);
});