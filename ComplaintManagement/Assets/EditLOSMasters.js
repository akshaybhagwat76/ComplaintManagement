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
            LOSName: $("#LOSName").val().trim(),
            SBUId: $("#SBUId").val().toString(),
            SubSBUId: $("#SubSBUId").val().toString(),
            CompetencyId: $("#CompetencyId").val().toString(),
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
            $("#SBUId").val($("#sbuIds").val().split(','))  
        }
        if ($("#SubsbuIds").val().includes(',')) {
            $("#SubSBUId").val($("#SubsbuIds").val().split(','))
        }
        if ($("#CompentencyIds").val().includes(',')) {

            $("#CompetencyId").val($("#CompentencyIds").val().split(','))
        }
    }
    let page_state = JSON.parse($("#pageState").val().toLowerCase());
    if (page_state) {
        $(".text-right").addClass("hide")
        $('.container-fluid').addClass("disabled-div");
    }
    else {
        $(".text-right").removeClass("hide")

        $('.container-fluid').removeClass("disabled-div");
    }
});