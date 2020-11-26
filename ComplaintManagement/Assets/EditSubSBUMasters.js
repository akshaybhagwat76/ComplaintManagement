var isValidSubSBU = false;

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
            $(this).parent().find("label").remove();
            $(this).removeClass("adderror");
        }
    });
    if (isValidSubSBU) {
        $("#SubSBU").addClass("adderror");
        funToastr(false, "This SubSBU is already exist."); return;
    }

    if (retval && !isValidSubSBU) {
        var data = {
            Id: $("#Id").val().trim(),
            SubSBU: $("#SubSBU").val().trim(),
            Status: $("#Status").val() == "true" ? true : false
        }
        StartProcess();
        $.ajax({
            type: "POST",
            url: "/SubSBU/AddOrUpdateSubSBU",
            data: { SubSBUVM: data },
            success: function (data) {
                if (data.status == "Fail") {
                    StopProcess();
                    $("#lblError").addClass("adderror").text(data.error).show();
                }
                else {
                    window.location.href = '/SubSBU/Index'
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
    var SubSBU = $("#SubSBU").val();
    var Id = $("#Id").val();
    var data = { SubSBU: SubSBU, Id: Id }
    if (SubSBU !== "") {
        $.post("/SubSBU/CheckIfExist", { data: data }, function (data) {
            if (data != null) {
                if (data.data) {
                   
           
                    isValidSubSBU = true;
                    $("#SubSBU").addClass("adderror");
                    funToastr(false, 'This SubSBU is already exist.');
                }
                else {
                    isValidSubSBU = false;
                    $("#SubSBU").removeClass("adderror");
                }
            }
        })

    }
}