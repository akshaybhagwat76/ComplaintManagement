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

    if (retval) {
        var data = {
            Id: $("#Id").val().trim(),
            UserId: $("#UserId").val(),
            LOSId: $("#LOSId").val(),
            SBUId: $("#SBUId").val().toString(),
            SubSBUId: $("#SubSBUId").val().toString(),
            CompetencyId: $("#CompetencyId").val().toString(),
            Status: $("#Status").val() == "true" ? true : false
        }
        debugger
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
            $("#SBUId").val($("#SBUIds").val().split(','))
        }
        if ($("#SubSBUIds").val().includes(',')) {
            $("#SubSBUId").val($("#SubSBUIds").val().split(','))
        }
        if ($("#CompentencyIds").val().includes(',')) {

            $("#CompetencyId").val($("#CompentencyIds").val().split(','))
        }
    }
});