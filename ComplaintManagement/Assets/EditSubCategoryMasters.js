var isValidSubCategory = false; 

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

    if (isValidSubCategory) {
        $("#SubCategoryName").addClass("adderror");
        funToastr(false, "This sub category is already exist."); return;
    }

    if (retval && !isValidSubCategory) {
        var data = {
            Id: $("#Id").val().trim(),
            SubCategoryName: $("#SubCategoryName").val().trim(),
            Status: $("#Status").val() == "true" ? true : false
        }
        StartProcess();
        $.ajax({
            type: "POST",
            url: "/SubCategory/AddOrUpdateSubCategory",
            data: { SubcategoryVM: data },
            success: function (data) {
                if (data.status == "Fail") {
                    StopProcess();
                    $("#lblError").addClass("adderror").text(data.error).show();
                }
                else {
                    window.location.href = '/SubCategory/Index'
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
    var SubCategoryName = $("#SubCategoryName").val();
    var Id = $("#Id").val();
    var data = { SubCategoryName: SubCategoryName, Id: Id }
    if (SubCategoryName !== "") {
        $.post("/SubCategory/CheckIfExist", { data: data }, function (data) {
            debugger
            if (data != null) {
                if (data.data) {
                    isValidSubCategory = true;
                    $("#SubCategoryName").addClass("adderror");
                    funToastr(false, 'This sub category is already exist.');
                }
                else {
                    isValidSubCategory = false;
                    $("#SubCategoryName").removeClass("adderror");
                }
            }
        })

    }
}