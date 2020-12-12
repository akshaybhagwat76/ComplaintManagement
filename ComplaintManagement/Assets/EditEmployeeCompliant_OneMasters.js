var attachementfiles = [];
var attachementfiles1 = [];

function removeFile(id) {
    $('#deleteModal').data('id', id).modal('show');
    $('#deleteModal').modal('show');
    document.getElementById("delete-btn").addEventListener("click", deleteFile);
}
$(document).ready(function () {
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
function deleteFile() {
    var fileName = $('#deleteModal').data('id');
    StartProcess();
    $.ajax({
        type: "POST",
        url: "/Compliant/RemoveFile",
        data: { fileName: fileName },
        success: function (response) {
            StopProcess();
            if (response.status !== "Fail") {
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

function submitForm(flag) {
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
        if (retval) {
            $('#Remark').removeClass("adderror");
        }
    }

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
            if (attachementfiles[i].file.IsDeleted == false) {
                formData.append(attachementfiles[i].file.name, attachementfiles[i].file);
            }
        }

        data = JSON.stringify(data);
        formData.append("EmpCompliantParams", data);
        formData.append("flag", flag);
        
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
        $("#form-attachement").append(attachement);
    }
}

removeAttachementFile = function (index) {
    let getFile = attachementfiles[index];
    if (getFile !== undefined) {
        document.getElementById("file_" + getFile.file.name).remove();
        attachementfiles[index].file.IsDeleted = true;
    }
}

// Import Attachement 
var inputAttachement = document.getElementById('attachementFile');
document.getElementById('attachementFile').addEventListener('change', addAttachementUploadedFile, false);

function addAttachementUploadedFile() {
    var files = this.files;
    var file;
    if (files && files.length) {
        file = files[0];
        const filename = file.name;
        let last_dot = filename.lastIndexOf('.')
        let ext = "." + filename.slice(last_dot + 1);
        var reader = new FileReader(); let isdeleted = "IsDeleted";
        file[isdeleted] = false;
        reader.onloadend = function () {
            attachementfiles.push({ file });
        }
        reader.readAsDataURL(file);
    }
}
function ComplaintSubmit(saltedId,Id) {
    if (Id == 0) {
        submitForm('B');
    }
    submitComplaint(saltedId);
}



submitComplaint = function (id) {
    StartProcess();
    $.ajax({
        type: "GET",
        url: "/Compliant/SubmitComplaint",
        data: { id: id },
        success: function (response) {
            StopProcess();
            if (response.status === "Fail") {
                $("#lblError").removeClass("success").removeClass("adderror").addClass("adderror").text(response.error).show();
            }
            else {
                $("#btnSave").addClass("disabled-div");
                $("#lblError").removeClass("success").removeClass("adderror").addClass("success").text(response.data).show();
            }
        },
        error: function (error) {
            funToastr(false, error.statusText);
        }
    });
}
addAttachement1 = function () {
    if (attachementfiles1 && attachementfiles1.length > 0) {
        var lastFile = attachementfiles1[attachementfiles1.length - 1];
        var index = attachementfiles1.findIndex(x => x.file.name === lastFile.file.name);
        var attachement1 = '<br /><div id="file_' + lastFile.file.name + '" class="col-md-12">' + lastFile.file.name + ' &nbsp;&nbsp;<span class="fa fa-times-circle fa-lg closeBtn" onclick="removeAttachementFile(' + index + ')" title="remove"></span></div>';
        $("#form-attachement1").append(attachement1);
    }
}
// Import Attachement 
var inputAttachements = document.getElementById('attachementFile1');
document.getElementById('attachementFile1').addEventListener('change', addAttachementUploadedFile1, false);


function addAttachementUploadedFile1() {
    var files = this.files;
    var file;
    if (files && files.length) {
        file = files[0];
        const filename = file.name;
        let last_dot = filename.lastIndexOf('.')
        let ext = "." + filename.slice(last_dot + 1);
        var reader = new FileReader(); let isdeleted = "IsDeleted";
        file[isdeleted] = false;
        reader.onloadend = function () {
            attachementfiles1.push({ file });
        }
        reader.readAsDataURL(file);
    }
}

function SubmitComplaintHr() {
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
        if (retval) {
            $('#Remark').removeClass("adderror");
        }
    }
    var str1 = $('#Remarked').val();
    if (/^[a-zA-Z0-9- ]*$/.test(str1) == false) {
        retval = false;
        $('#Remarked').addClass("adderror");
        funToastr(false, "Remarks cannot contain special characters.");
    }
    else {
        if (retval) {
            $('#Remarked').removeClass("adderror");
        }
    }

    if (retval) {
        var data = {

            DueDate: $("#DueDate").val(),
            CategoryId: $("#CategoryId").val(),
            SubCategoryId: $("#SubCategoryId").val(),
            Remark: $("#Remark").val(),
            CaseType: $("#CaseType option:selected").val(),
            UserId: $("#UserId").val(),
            ComplaintStatus: $("#ComplaintStatus").val()
        }
        var Id = $("#Id").val();
        var UserInvolved = $(".test").val();
        var formData = new FormData();

        for (var i = 0; i < attachementfiles1.length; i++) {
            if (attachementfiles1[i].file.IsDeleted == false) {
                formData.append(attachementfiles1[i].file.name, attachementfiles1[i].file);
            }
        }

        data = JSON.stringify(data);
        formData.append("EmpCompliantParams", data);
        formData.append("Id", Id);
        formData.append("UserInvolved", UserInvolved);
        formData.append("Status", 1);

        StartProcess();
        $.ajax({
            type: "POST",
            url: "/Compliant/AddOrEmployeeCompliantHR",
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

function PushComplaintHr() {
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
        if (retval) {
            $('#Remark').removeClass("adderror");
        }
    }
    var str1 = $('#Remarked').val();
    if (/^[a-zA-Z0-9- ]*$/.test(str1) == false) {
        retval = false;
        $('#Remarked').addClass("adderror");
        funToastr(false, "Remarks cannot contain special characters.");
    }
    else {
        if (retval) {
            $('#Remarked').removeClass("adderror");
        }
    }

    if (retval) {
        var data = {

            DueDate: $("#DueDate").val(),
            CategoryId: $("#CategoryId").val(),
            SubCategoryId: $("#SubCategoryId").val(),
            Remark: $("#Remarked").val(),
            UserId: $("#UserId").val(),
            CaseType: $("#CaseType option:selected").val(),
            ComplaintStatus: $("#ComplaintStatus").val(),
        }

        data = JSON.stringify(data);
        var Id = $("#Id").val();
        var UserInvolved = $(".test").val();
        var formData = new FormData();

        for (var i = 0; i < attachementfiles1.length; i++) {
            if (attachementfiles1[i].file.IsDeleted == false) {
                formData.append(attachementfiles1[i].file.name, attachementfiles1[i].file);

                formData.append("EmpCompliantParams", data);
                formData.append("Id", Id);
                formData.append("UserInvolved", UserInvolved);
                formData.append("Status", 2);


                StartProcess();
                $.ajax({
                    type: "POST",
                    url: "/Compliant/AddOrEmployeeCompliantHR",
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
    }
}




function SubmitGOBack() {
    let url = `/Compliant/ComplaintTwo_Index`
    location.href = url;
}