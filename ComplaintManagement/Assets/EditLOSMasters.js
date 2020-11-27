var isValidLOS = false;
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

    if (isValidLOS) {
        $("#LOSName").addClass("adderror");
        funToastr(false, "This LOS is already exist."); return;
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

    if (retval && !isValidLOS && sbufields.length > 0 && subsbufields.length > 0 && Competencyfields.length > 0) {
        var data = {
            Id: $("#Id").val().trim(),
            LOSName: $("#LOSName").val().trim(),
            SBUId: $("#sbuIds").val().toString(),
            SubSBUId: $("#SubsbuIds").val().toString(),
            CompetencyId: $("#CompentencyIds").val().toString(),
            Status: $("#Status").val() == "true" ? true : false
        }
        StartProcess();
        $.ajax({
            type: "POST",
            url: "/LOS/AddOrUpdateLOS",
            data: { LOSVM: data },
            success: function (data) {
                if (data.status == "Fail") {
                    StopProcess();
                    $("#lblError").addClass("adderror").text(data.error).show();
                }
                else {
                    window.location.href = '/LOS/Index'
                }
            }
        });
    }
}

$(document).ready(function () {
    if ($("#Id").val() === "0") {
        $("#Status").val("true");
    }
    else {
        if ($("#sbuIds").val().includes(',')) {
            var sbuIds = $('#sbuIds').val().split(",")

            $.each(sbuIds, function (i, keyword) {
                var el = $("#sbu-" + keyword);
                if (el[0] != null) {
                    el.attr("checked", true)
                }
            });

            $("#SBUId").val(sbuIds)
        }
        else {
            if ($("#sbuIds").val() != "") {
                var el = $("#sbu-" + $("#sbuIds").val());
                if (el[0] != null) {
                    el.attr("checked", true)
                }
            }
        }

        if ($("#SubsbuIds").val().includes(',')) {
            var subsbuIds = $('#SubsbuIds').val().split(",")

            $.each(subsbuIds, function (i, keyword) {
                var el = $("#subsbu-" + keyword);
                if (el[0] != null) {
                    el.attr("checked", true)
                }
            });

            $("#SubsbuIds").val(subsbuIds)
        }
        else {
            if ($("#SubsbuIds").val() != "") {
                var el = $("#subsbu-" + $("#SubsbuIds").val());
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
    $('#SubsbuIds').val(values);
});


$('.checks-sbu').change(function () {
    var values = $('.checks-sbu:checked').map(function () {
        return this.value;
    }).get().join(',');
    $('#sbuIds').val(values);
});

$('.checks-Competency').change(function () {
    var values = $('.checks-Competency:checked').map(function () {
        return this.value;
    }).get().join(',');
    $('#CompentencyIds').val(values);
});


function checkDuplicate() {
    var LOSName = $("#LOSName").val();
    var Id = $("#Id").val();
    var data = { LOSName: LOSName, Id: Id }
    if (LOSName !== "") {
        $.post("/LOS/CheckIfExist", { data: data }, function (data) {
            if (data != null) {
                if (data.data) {
                    isValidLOS = true;
                    $("#LOSName").addClass("adderror");
                    funToastr(false, 'This LOS is already exist.');
                }
                else {
                    isValidLOS = false;
                    $("#LOSName").removeClass("adderror");
                }
            }
        })

    }
}




