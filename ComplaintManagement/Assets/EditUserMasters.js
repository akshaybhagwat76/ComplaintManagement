var isValidEmail = false; var isValidEmp = false;
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
    var email = $("#WorkEmail").val().trim();
    if (email === "") {
        $("#WorkEmail").addClass("adderror");
        retval = false;
    }
    if (email  && !isEmail(email)) {
        $("#WorkEmail").addClass("adderror");
        retval = false;
    }
    var mobile = $("#MobileNo").val();
    if ($("#MobileNo").val() != "" && mobile.length < 10) {
        $("#MobileNo").addClass("adderror");
        funToastr(false, "Please enter mobile number atleast 10 digits");
        retval = false;
    }


    if (retval && !isValidEmail && !isValidEmp) {
        var documentFile = $("#inputImage").get(0).files;
        var docfile = "";
        if (documentFile.length > 0) {
            var documentFileExt = documentFile[0].name.split('.').pop();
            docfile = $("#inputImage").attr('data-base64string');
            docfile = docfile + ',' + documentFileExt;
        }
        var data = {
            Id: $("#Id").val(),
            EmployeeName: $("#EmployeeName").val().trim(),
            EmployeeId: $("#EmployeeId").val().trim(),
            Gender: $("#Gender").val().trim(),
            Age: $("#Age").val().trim(),
            WorkEmail: $("#WorkEmail").val().trim(),
            TimeType: $("#TimeType").val().trim(),
            BusinessTitle: $("#BusinessTitle").val().trim(),
            Company: $("#Company").val().trim(),
            LOSId: $("#LOSId").val().trim(),
            SBUId: $("#SBUId").val().trim(),
            SubSBUId: $("#SubSBUId").val().trim(),
            CompentencyId: $("#CompentencyId").val().trim(),
            LocationId: $("#LocationId").val().trim(),
            RegionId: $("#RegionId").val().trim(),
            DateOfJoining: $("#DateOfJoining").val().trim(),
            MobileNo: $("#MobileNo").val().trim(),
            Manager: $("#Manager").val().trim(),
            Type: $("#Type").val().trim(),
            ImagePath: docfile,
            Status: $("#Status").val() == "true" ? true : false
        }
        StartProcess();
        $.ajax({
            type: "POST",
            url: "/UserMaster/AddOrUpdateUser",
            data: { UserVM: data },
            success: function (data) {
                if (data.status == "Fail") {
                    StopProcess();
                    $("#lblError").addClass("adderror").text(data.error).show();
                }
                else {
                    window.location.href = '/UserMaster/Index'
                }
            }
        });
    }
    if (isValidEmail) {
        $("#WorkEmail").addClass("adderror");
    }
    if (isValidEmp) {
        $("#EmployeeId").addClass("adderror");

    }
}

function RemoveImage(id) {
    $('#deleteModal').data('id', id).modal('show');
    $('#deleteModal').modal('show');
}


$('#delete-btn').click(function () {
    var fileName = $('#deleteModal').data('id');
        StartProcess();
        $.ajax({
            type: "POST",
            url: "/UserMaster/RemoveProfile",
            data: { fileName: fileName },
            success: function (response) {
                StopProcess()
                if (response.status != "Fail") {
                    funToastr(true, response.msg);
                    $("#profile_pic").attr('src', '/Images/profile.png');
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
})


$(document).ready(function () {
    if ($("#Id").val() === "0") {
        $("#Status").val("true");
    }
    else {
        $("#DateOfJoining").val($("#Doj").val().replaceAll("/", "-"))
    }
    if ($("#pageState").val() != null && $("#pageState").val() != "") {
        let page_state = JSON.parse($("#pageState").val().toLowerCase());
        if (page_state) {
            $(".text-right").addClass("hide")
            $('.container-fluid').addClass("disabled-div");
        }

    }
    else {
        $(".text-right").removeClass("hide")

        $('.container-fluid').removeClass("disabled-div");
    }
});

// Import image
var inputImage = document.getElementById('inputImage');

inputImage.onchange = function () {
    var files = this.files;
    var file;
    if (files && files.length) {
        file = files[0];
        const filename = file.name;

        let last_dot = filename.lastIndexOf('.')
        let ext = "." + filename.slice(last_dot + 1);
        if (bytesToSize(file.size) >= 500) {
            funToastr(false, 'The maximum file size for an image is 500 KB. Please reduce your file size and try again');
        }
        else {
            if (/^image\/\w+/.test(file.type)) {
                var reader = new FileReader();
                reader.onloadend = function () {
                    $('#profile_pic').attr('src', reader.result);
                    $(inputImage).attr("data-base64string", reader.result);
                    $(inputImage).attr("data-extension", ext);
                    //PreviewBase64Image(reader.result, $this.id + "Preview");
                }
                reader.readAsDataURL(file);

                image_pic = "profile_pic";
            } else {
                funToastr(false, 'Please choose an image file');
            }
        }
    }
};

function checkDuplicate() {
    var workemail = $("#WorkEmail").val();
    var EmployeeId = $("#EmployeeId").val();
    var Id = $("#Id").val();
    if (workemail != "" || EmployeeId != "") {
        var data = {
            Id: Id,
            EmployeeId: EmployeeId,
            WorkEmail: workemail
        }
        $.ajax({
            type: "POST",
            url: "/UserMaster/IsExist/",
            data: { UserVM: data },
            success: function (data) {
                if (data != null) {
                    if (data.msg != null && data.msg != "") {
                        if (data.msg.includes("Employee Id")) {
                            $("#EmployeeId").addClass("adderror");
                            isValidEmp = true;
                        }
                        else {
                            isValidEmp = false;
                            $("#EmployeeId").removeClass("adderror");
                        }
                        if (data.msg.includes("Work")) {
                            $("#WorkEmail").addClass("adderror");
                            isValidEmail = true;
                        }
                        else {
                            $("#WorkEmail").removeClass("adderror");
                            isValidEmail = false;
                        }
                        funToastr(false, data.msg);
                    }
                    else {
                        $("#WorkEmail").removeClass("adderror");
                        isValidEmail = false;
                        isValidEmp = false;
                        $("#EmployeeId").removeClass("adderror");
                    }
                }
            }

        })
    }
}

function GetLOSSettings() {
    var LOSId = $("#LOSId").val();

    if (LOSId != "") {
        var data = { LOSId: LOSId };
        $.ajax({
            type: "POST",
            url: "/UserMaster/GetLOSSettings/",
            data: { UserVM: data },
            success: function (response) {
                if (response != null) {
                    var data = response.data;
                    var dataOfCompentency = JSON.parse($("#hdnCompentencyList").val());
                    var dataOfsubList = JSON.parse($("#hdnSBUList").val());
                    var dataOfSubSBUList = JSON.parse($("#hdnSubSBUList").val());
                    if (data.CompetencyId != "" || data.CompetencyId != null && dataOfCompentency.length > 0) {
                        var lstCompentency = data.CompetencyId.split(",");
                        var losBaseCompantency = [];
                        $.each(lstCompentency, function (key, ctcId) {
                            if (ctcId > 0) {
                                var ctcObj = dataOfCompentency.find(x => x.Value === ctcId);
                                if (ctcObj !== null || ctcObj !== undefined) {
                                    losBaseCompantency.push(ctcObj);
                                }
                            }
                        })
                        $("#CompentencyId").empty();
                        losBaseCompantency = [{ Text: "", Value: "" }, ...losBaseCompantency];
                        $.each(losBaseCompantency, function (key, item) {
                            if (item != undefined || item != null) {
                                $("#CompentencyId").append(`<option value='${item.Value}'>${item.Text}</option>`)
                            }
                        })
                    }

                    if (data.SBUId != "" || data.SBUId != null && dataOfsubList.length > 0) {
                        var lstSBU = data.SBUId.split(",");
                        var losBaseSBU = [];
                        $.each(lstSBU, function (key, sbuId) {
                            if (sbuId > 0) {
                                var losSBUObj = dataOfsubList.find(x => x.Value === sbuId);
                                if (losSBUObj !== null || losSBUObj !== undefined) {
                                    losBaseSBU.push(losSBUObj);
                                }
                            }
                        })
                        $("#SBUId").empty();

                        losBaseSBU = [{ Text: "", Value: "" }, ...losBaseSBU];
                        $.each(losBaseSBU, function (key, item) {
                            if (item != undefined || item != null) {
                                $("#SBUId").append(`<option value='${item.Value}'>${item.Text}</option>`)
                            }
                        })
                    }


                    if (data.SubSBUId != "" || data.SubSBUId != null && dataOfSubSBUList.length > 0) {
                        var lstSubSBU = data.SubSBUId.split(",");
                        var losBaseSubSBU = [];
                        $.each(lstSubSBU, function (key, subSbuId) {
                            if (subSbuId > 0) {
                                var losSubSBUObj = dataOfSubSBUList.find(x => x.Value === subSbuId);
                                if (losSubSBUObj !== null || losSubSBUObj !== undefined) {
                                    losBaseSubSBU.push(losSubSBUObj);
                                }
                            }
                        })
                        $("#SubSBUId").empty();

                        losBaseSubSBU = [{ Text: "", Value: "" }, ...losBaseSubSBU];
                        $.each(losBaseSubSBU, function (key, item) {
                            if (item != undefined || item != null) {
                                $("#SubSBUId").append(`<option value='${item.Value}'>${item.Text}</option>`)
                            }
                        })
                    }
                }
            }
        })
    }

}

$("#Manager").autocomplete({
    source: function (request, response) {
      
        $.ajax({
            type: 'POST',
            url: '/UserMaster/AutoCompeleteManager',
            data: { managervalue: request.term  },
            success: function (resp) {
                if (resp.status === 'Success') {
                    var Managerdata = [];
                    if (resp.data != null && resp.data.length > 0) {
                        $.each(resp.data, function (key, item) {
                            if (item != null && item.Name != "" && item.EmpId != "") {
                                Managerdata.push({ val: item.Id, label: item.EmployeeName + " (" + item.EmployeeId + ")" });
                            }
                        })
                        response($.map(Managerdata, function (item) {
                            return item;
                        }))
                    }
                }
               
            },
            error: function (response) {
                alert(response.responseText);
            },
            failure: function (response) {
                alert(response.responseText);
            }
        });
    },
    select: function (e, i) {
        $("#ManagerId").val(i.item.label);
    },
    minLength: 1
});
