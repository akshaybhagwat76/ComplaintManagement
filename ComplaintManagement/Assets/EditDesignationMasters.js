var isValidDesignation = false;
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
    if (isValidDesignation) {
        $("#Designation").addClass("adderror");
        funToastr(false, "This Designation is already exist."); return;
    }

    if (retval && !isValidDesignation) {
        var data = {
            Id: $("#Id").val().trim(),
            Designation: $("#Designation").val().trim(),
            Status: $("#Status").val() == "true" ? true : false
        }
        StartProcess();
        $.ajax({
            type: "POST",
            url: "/Designation/AddOrUpdateDesignation",
            data: { DesignationVM: data },
            success: function (data) {
                if (data.status == "Fail") {
                    StopProcess();
                    $("#lblError").addClass("adderror").text(data.error).show();
                }
                else {
                    window.location.href = '/Designation/Index'
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
    var Designation = $("#Designation").val();
    var Id = $("#Id").val();
    var data = { Designation: Designation, Id: Id }
    if (Designation !== "") {
        $.post("/Designation/CheckIfExist", { data: data }, function (data) {
            if (data != null) {
                if (data.data) {
                    isValidDesignation = true;
                    $("#Designation").addClass("adderror");
                    funToastr(false, 'This Designation is already exist.');
                }
                else {
                    isValidDesignation = false;
                    $("#Designation").removeClass("adderror");
                }
            }
        })

    }
}