var isValidRegion = false
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
    if (isValidRegion) {
        $("#Region").addClass("adderror");
        funToastr(false, "This Region is already exist."); return;
    }
    else {
        $("#Region").removeClass("adderror");
    }

    if (retval) {
        var data = {
            Id: $("#Id").val().trim(),
            Region: $("#Region").val().trim(),
            Status: $("#Status").val() == "true" ? true : false
        }
        StartProcess();
        $.ajax({
            type: "POST",
            url: "/Region/AddOrUpdateRegion",
            data: { RegionVM: data },
            success: function (data) {
                if (data.status == "Fail") {
                    StopProcess();
                    $("#lblError").addClass("adderror").text(data.error).show();
                }
                else {
                    window.location.href = '/Region/Index'
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
   
    var Region = $("#Region").val();
    var Id = $("#Id").val();
    var data = { Region: Region, Id: Id };
    if (Region !== "") {
        $.post("/Region/CheckIfExist", { data: data }, function (data) {
            if (data != null) {
                if (data.data) {
                    isValidRegion = true;
                    $("#Region").addClass("adderror");
                    funToastr(false, 'This Region is already exist.');
                }
                else {
                    isValidRegion = false;
                    $("#Region").removeClass("adderror");
                }
            }
        })

    }
}