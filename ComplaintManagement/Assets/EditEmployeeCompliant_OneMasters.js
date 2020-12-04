function removeFile(id) {

    Confirm('Are you sure?', 'You will not be able to recover this', 'Yes', 'Cancel', id); /*change*/
}


function Confirm(title, msg, $true, $false, $link) { /*change*/
    var $content = "<div class='dialog-ovelay'>" +
        "<div class='dialog'><header>" +
        " <h3> " + title + " </h3> " +
        "<i class='fa fa-close'></i>" +
        "</header>" +
        "<div class='dialog-msg'>" +
        " <p> " + msg + " </p> " +
        "</div>" +
        "<footer>" +
        "<div class='controls' style='margin-left: 235px;'>" +
        " <button class='button button-danger doAction'>" + $true + "</button> " +
        " <button class='button button-default cancelAction'>" + $false + "</button> " +
        "</div>" +
        "</footer>" +
        "</div>" +
        "</div>";
    $('body').prepend($content);
    $('.doAction').click(function () {
        deleteAction($link);
        $(this).parents('.dialog-ovelay').fadeOut(500, function () {
            $(this).remove();
        });
    });
    $('.cancelAction, .fa-close').click(function () {
        $(this).parents('.dialog-ovelay').fadeOut(500, function () {
            $(this).remove();
        });
    });

}
function deleteAction(fileName) {
    if (fileName != "" && fileName != undefined) {
        //StartProcess();
        $.ajax({
            type: "POST",
            url: "/Compliant/RemoveFIle",
            data: { fileName: fileName },
            success: function (response) {
                if (response.status != "Fail") {
                    funToastr(true, response.msg);
                    document.getElementById("file_" + response.data).remove()
                }
                else {
                    funToastr(false, response.error);
                }
            },
            error: function (error) {
                toastr.error(error)
            }
        });
    }
}
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

    
   
   

    
    if (retval) {
        debugger
        var data = {
            Id: $("#Id").val(),
            CategoryId: $("#CategoryId").val(),
            SubCategoryId: $("#SubCategoryId").val(),
            Remark: $("#Remark").val(),
           UserId:$("#UserId").val()
        }
        var file = $("#customFile").get(0).files;
        var formData = new FormData();

        var fileUpload = $("#customFile").get(0);
        var files = fileUpload.files;  

        for (var i = 0; i < files.length; i++) {
            formData.append(files[i].name, files[i]);
        }  

        data = JSON.stringify(data);
        formData.append("EmpCompliantParams", data);

        StartProcess();
        $.ajax({
            type: "POST",
            url: "/Compliant/AddOrUpdateEmployeeCompliant",
            data: formData,
            contentType: false,
            processData: false,
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