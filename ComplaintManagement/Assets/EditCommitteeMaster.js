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
    else {

    }
});