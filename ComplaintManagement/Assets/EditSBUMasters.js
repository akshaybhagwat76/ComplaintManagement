﻿var isValidSBU = false;
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
    if (isValidSBU) {
        $("#SBU").addClass("adderror");
        funToastr(false, "This SBU is already exist."); return;
    }

    if (retval && !isValidSBU) {
        var data = {
            Id: $("#Id").val().trim(),
            SBU: $("#SBU").val().trim(),
            InvolvedUsersId: $(".test").val().toString(),
            Status: $("#Status").val() == "true" ? true : false
        }
        StartProcess();
        $.ajax({
            type: "POST",
            url: "/SBU/AddOrUpdateSBU",
            data: { SBUVM: data },
            success: function (data) {
                if (data.status == "Fail") {
                    StopProcess();
                    $("#lblError").addClass("adderror").text(data.error).show();
                }
                else {
                    window.location.href = '/SBU/Index'
                }
            }
        });
    }
}

$(document).ready(function () {
    if ($("#Id").val() === "0") {
        $("#Status").val("true");
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
function checkDuplicate() {
    //var SBU = $("#SBU").val();
    //var Id = $("#Id").val();
    //var data = { SBU: SBU, Id: Id }
    //if (SBU !== "") {
    //    $.post("/SBU/CheckIfExist", { data: data }, function (data) {
    //        if (data != null) {
    //            if (data.data) {
    //                isValidSBU = true;
    //                $("#SBU").addClass("adderror");
    //                funToastr(false, 'This SBU is already exist.');
    //            }
    //            else {
    //                isValidSBU = false;
    //                $("#SBU").removeClass("adderror");
    //            }
    //        }
    //    })

    //}
}