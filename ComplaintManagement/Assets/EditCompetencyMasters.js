var isValidCompetency = false;
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
    if (isValidCompetency) {
        $("#CompetencyName").addClass("adderror");
        funToastr(false, "This category is already exist."); return;
    }
    
    if (retval && !isValidCompetency) {
        var data = {
            Id: $("#Id").val().trim(),
            CompetencyName: $("#CompetencyName").val().trim(),
            Status: $("#Status").val() == "true" ? true : false
        }
        StartProcess();
        $.ajax({
            type: "POST",
            url: "/Competency/AddOrUpdateCompetency",
            data: { CompetencyVM: data },
            success: function (data) {
                if (data.status == "Fail") {
                    StopProcess();
                    $("#lblError").addClass("adderror").text(data.error).show();
                }
                else {
                    window.location.href = '/Competency/Index'
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
    var CompetencyName = $("#CompetencyName").val();
    var Id = $("#Id").val();
    var data = { CompetencyName: CompetencyName, Id: Id }
    if (CompetencyName !== "") {
        $.post("/Competency/CheckIfExist", { data: data }, function (data) {
            if (data != null) {
                if (data.data) {
                    isValidCompetency = true;
                    $("#CompetencyName").addClass("adderror");
                    funToastr(false, 'This Competency is already exist.');
                }
                else {
                    isValidCompetency = false;
                    $("#CompetencyName").removeClass("adderror");
                }
            }
        })

    }
}