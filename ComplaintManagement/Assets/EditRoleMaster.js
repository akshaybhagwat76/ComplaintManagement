function submitForm() {
    $("#lblError").removeClass("success").removeClass("adderror").text('');
    var retval = true;
    $("#myForm .required").each(function () {
        if (!$(this).val()) {
            var $label = $("<label class='adderror'>").text('This field is required:');
            if ($(this).parent().find("label").length == 1) {
                $(this).parent().append($label);

                $(this).addClass("adderror");
            }
            retval = false;
        }
        else {
            $(this).parent().find("label").remove();
            $(this).removeClass("adderror");
        }
    });

    if (retval) {
        var data = {
            Id: $("#Id").val().trim(),
            UserId: $("#UserId").val(),
            LOSId: $("#LOSId").val().toString(),
            SBUId: $("#SBUId").val().toString(),
            SubSBUId: $("#SubSBUId").val().toString(),
            CompetencyId: $("#CompetencyId").val().toString(),
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
            $("#SBUId").val($("#SBUIds").val().split(','))
        }
        if ($("#SubSBUIds").val().includes(',')) {
            $("#SubSBUId").val($("#SubSBUIds").val().split(','))
        }
        if ($("#CompentencyIds").val().includes(',')) {

            $("#CompetencyId").val($("#CompentencyIds").val().split(','))
        }
        if ($("#LOSIds").val().includes(',')) {
            $("#LOSId").val($("#LOSIds").val().split(','))
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