function submitForm() {
    $("#lblError").removeClass("success").removeClass("adderror").text('');
    var retval = true;
    alert('called');
    $("#myForm .required").each(function () {
        if (!$(this).val()) {
            $(this).addClass("adderror");
            retval = false;
        }
        else {
            $(this).removeClass("adderror");
        }
    });
    var email = $("#WorkEmail").val().trim();
    if (email && !isEmail(email)) {
        $("#WorkEmail").addClass("adderror");
        retval = false;
    }

    if (retval) {
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
});