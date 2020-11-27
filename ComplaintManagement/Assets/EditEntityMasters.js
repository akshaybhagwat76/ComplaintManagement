var isValidEntity = false
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
            if ($(this).parent().find("label").length > 1) {
                $(this).parent().find("label:eq(1)").remove();
                $(this).removeClass("adderror");
            }
        }
    });
    if (isValidEntity) {
        $("#EntityName").addClass("adderror");
        funToastr(false, "This Entity is already exist."); return;
    }
 
    if (retval && !isValidEntity) {
        var data = {
            Id: $("#Id").val().trim(),
            EntityName: $("#EntityName").val().trim(),
            Status: $("#Status").val() == "true" ? true : false
        }
        StartProcess();
        $.ajax({
            type: "POST",
            url: "/Entity/AddOrUpdateEntity",
            data: { EntityVM: data },
            success: function (data) {
                if (data.status == "Fail") {
                    StopProcess();
                    $("#lblError").addClass("adderror").text(data.error).show();
                }
                else {
                    window.location.href = '/Entity/Index'
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
    var EntityName = $("#EntityName").val();
    var Id = $("#Id").val();
    var data = { EntityName: EntityName, Id: Id }
    if (EntityName !== "") {
        $.post("/Entity/CheckIfExist", { data: data }, function (data) {
            if (data != null) {
                if (data.data) {
                    isValidEntity = true;
                    $("#EntityName").addClass("adderror");
                    funToastr(false, 'This Entity is already exist.');
                }
                else {
                    isValidEntity = false;
                    $("#EntityName").removeClass("adderror");
                }
            }
        })

    }
}