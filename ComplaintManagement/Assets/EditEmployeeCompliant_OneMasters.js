var attachementfiles = [];

function removeFile(id) {
    $('#deleteModal').data('id', id).modal('show');
    $('#deleteModal').modal('show');
    document.getElementById("delete-btn").addEventListener("click", deleteFile);
}

function deleteFile() {
    var fileName = $('#deleteModal').data('id');
    StartProcess();
    $.ajax({
        type: "POST",
        url: "/Compliant/RemoveFIle",
        data: { fileName: fileName },
        success: function (response) {
            StopProcess();
            if (response.status != "Fail") {
                funToastr(true, response.msg);
                document.getElementById("file_" + response.data).remove();
                $('#deleteModal').modal('hide');
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

    var str = $('#Remark').val();
    if (/^[a-zA-Z0-9- ]*$/.test(str) == false) {
        retval = false;
        $('#Remark').addClass("adderror");
        funToastr(false, "Remarks cannot contain special characters.");
    }
    else {
        $('#Remark').removeClass("adderror");
    }
    debugger
    if (retval) {
        var data = {
            Id: $("#Id").val(),
            DueDate: $("#DueDate").val(),
            CategoryId: $("#CategoryId").val(),
            SubCategoryId: $("#SubCategoryId").val(),
            Remark: $("#Remark").val(),
            UserId: $("#UserId").val(),
            ComplaintStatus: $("#ComplaintStatus").val()
        }
        var formData = new FormData();

        for (var i = 0; i < attachementfiles.length; i++) {
            formData.append(attachementfiles[i].file.name, attachementfiles[i].file);
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
                    $("#lblError").removeClass("success").removeClass("adderror").addClass("adderror").text(response.error).show();
                }
                else {
                    window.location.href = '/Employee/Index';
                }
            },
            error: function (error) {
                funToastr(false, error.statusText);
            }
        });
    }
}

addAttachement = function () {
    if (attachementfiles && attachementfiles.length > 0) {
        var lastFile = attachementfiles[attachementfiles.length - 1];
        var index = attachementfiles.findIndex(x => x.file.name === lastFile.file.name);
        var attachement = '<br /><div id="file_' + lastFile.file.name + '" class="col-md-12">' + lastFile.file.name + ' &nbsp;&nbsp;<span class="fa fa-times-circle fa-lg closeBtn" onclick="removeAttachementFile(' + index + ')" title="remove"></span></div>';
        $("#form-empData").append(attachement);
    }
}
removeAttachementFile = function (index) {
    let getFile = attachementfiles[index];
    document.getElementById("file_" + getFile.file.name).remove();
    attachementfiles.splice(index, 1);
}

// Import Attachement 
var inputAttachement = document.getElementById('attachementFile');
document.getElementById('attachementFile').addEventListener('change',
    addAttachementUploadedFile,
    false);

function addAttachementUploadedFile() {
    var files = this.files;
    var file;
    if (files && files.length) {
        file = files[0];
        const filename = file.name;
        let last_dot = filename.lastIndexOf('.')
        let ext = "." + filename.slice(last_dot + 1);
        var reader = new FileReader();
        reader.onloadend = function () {
            attachementfiles.push({ file });
        }
        reader.readAsDataURL(file);
    }
}