var isValidCategory = false;

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

    if (isValidCategory) {
        $("#CategoryName").addClass("adderror");
        funToastr(false, "This category is already exist."); return;
    }
    else {
        $("#CategoryName").removeClass("adderror");
    }

    if (retval) {
        var data = {
            id: $("#Id").val().trim(),
            CategoryName: $("#CategoryName").val().trim(),
            Status: $("#Status").val() == "true" ? true: false
        }
        StartProcess();
        $.ajax({
            type: "POST",
            url: "/Category/AddOrUpdateCategories",
            data: { categoryVM: data },
            success: function (data) {
                if (data.status == "Fail") {
                    StopProcess();
                    $("#lblError").addClass("adderror").text(data.error).show();
                }
                else {
                    window.location.href = '/Category/Index'
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
    var CategoryName = $("#CategoryName").val();
    var Id = $("#Id").val();
    var data = { CategoryName: CategoryName ,Id:Id}
    if (CategoryName !== "") {
        $.post("/Category/CheckIfExist", { data: data }, function (data) {
            if (data != null) {
                if (data.data) {
                    isValidCategory = true;
                    $("#CategoryName").addClass("adderror");
                    funToastr(false, 'This category is already exist.');
                }
                else {
                    isValidCategory = false;
                    $("#CategoryName").removeClass("adderror");
                }
            }
        })
      
    }
}