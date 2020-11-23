﻿var isValidCommittee = false;
function submitForm() {
    $("#lblError").removeClass("success").removeClass("adderror").text('');
    var retval = true;
    $("#myForm .required").each(function () {
        if (!$(this).val()) {
            $(this).addClass("adderror");
            retval = false;
        }
        else {
            $(this).removeClass("adderror");
        }
    });
    if (isValidCommittee) {
        $("#CommitteeName").addClass("adderror");
        funToastr(false, "This Committee is already exist."); return;
    }
    else {
        $("#CommitteeName").removeClass("adderror");
    }

    if (retval) {
        var data = {
            id: $("#Id").val().trim(),
            UserId  : $("#UserId").val().trim(),
            CommitteeName: $("#CommitteeName").val().trim(),
            Status: $("#Status").val() == "true" ? true : false
        }
        StartProcess();
        $.ajax({
            type: "POST",
            url: "/Committee/AddOrUpdateCommittee",
            data: { CommitteeVM: data },
            success: function (data) {
                if (data.status == "Fail") {
                    StopProcess();
                    $("#lblError").addClass("adderror").text(data.error).show();
                }
                else {
                    window.location.href = '/Committee/Index'
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
  
    var CommitteeName = $("#CommitteeName").val();
    var Id = $("#Id").val();
    var data = { CommitteeName: CommitteeName, Id: Id }
    if (CommitteeName !== "") {
        $.post("/Committee/CheckIfExist", { data: data }, function (data) {
            
            if (data != null) {
                if (data.data) {
                    isValidCommittee = true;
                    $("#CommitteeName").addClass("adderror");
                    funToastr(false, 'This Committee is already exist.');
                }
                else {
                    isValidCommittee = false;
                    $("#CommitteeName").removeClass("adderror");
                }
            }
        })

    }
}