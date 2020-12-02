function submitForm() {
   debugger
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
   
    var str = $('#Remark').val();
    if (/^[a-zA-Z0-9- ]*$/.test(str) == false) {
        retval = false;
        $('#Remark').addClass("adderror");
        funToastr(false, "Remarks cannot contain special characters.");
    }
    else {
        $('#Remark').removeClass("adderror");
    }

    
   
   

    
    if (retval ) {
        var data = {
            Id: $("#Id").val(),
            CategoryId: $("#CategoryId").val(),
            SubCategoryId: $("#SubCategoryId").val(),
            Remark: $("#Remark").val(),
            Attachments: $("#Attachment").val(),
           UserId:$("#UserId").val()
        }
        debugger
        data = JSON.stringify(data);
        StartProcess();
        $.ajax({
            type: "POST",
            url: "/Compliant/AddOrUpdateEmployeeCompliant",
            data: { EmpCompliantParams: data },
            success: function (response) {
                if (response.status == "Fail") {
                    StopProcess();
                    $("#lblError").addClass("adderror").text(data.error).show();
                }
                else {
                    window.location.href = '/Employee/Index'
                }
            },
            error: function (error) {
                toastr.error(error)
            }
        });
    }
}