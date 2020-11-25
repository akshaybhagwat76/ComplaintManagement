var isValidLocation = false;
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
    if (isValidLocation) {
        $("#LocationName").addClass("adderror");
        funToastr(false, "This Location is already exist."); return;
    }

    if (retval && !isValidLocation) {
        var data = {
            Id: $("#Id").val().trim(),
            LocationName: $("#LocationName").val().trim(),
            Status: $("#Status").val() == "true" ? true : false
        }
        StartProcess();
        $.ajax({
            type: "POST",
            url: "/Location/AddOrUpdateLocation",
            data: { LocationVM: data },
            success: function (data) {
                if (data.status == "Fail") {
                    StopProcess();
                    $("#lblError").addClass("adderror").text(data.error).show();
                }
                else {
                    window.location.href = '/Location/Index'
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
    var LocationName = $("#LocationName").val();
    var Id = $("#Id").val();
    var data = { LocationName: LocationName, Id: Id }
    if (LocationName !== "") {
        $.post("/Location/CheckIfExist", { data: data }, function (data) {
            if (data != null) {
                if (data.data) {
                    isValidLocation = true;
                    $("#LocationName").addClass("adderror");
                    funToastr(false, 'This category is already exist.');
                }
                else {
                    isValidLocation = false;
                    $("#LocationName").removeClass("adderror");
                }
            }
        })

    }
}