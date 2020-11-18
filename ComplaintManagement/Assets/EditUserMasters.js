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
    var email = $("#WorkEmail").val().trim();
    if (email && !isEmail(email)) {
        $("#WorkEmail").addClass("adderror");
        retval = false;
    }

    if (retval) {
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
}

$(document).ready(function () {
    if ($("#Id").val() === "0") {
        $("#Status").val("true");
    }
    else {
        $("#DateOfJoining").val(new Date($("#Doj").val()).toISOString().split('T')[0]);
    }
    let page_state = JSON.parse($("#pageState").val().toLowerCase());
    if (page_state) {
        $(".text-right").addClass("hide")
        $('.container-fluid').addClass("disabled-div");
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
        if (bytesToSize(file.size) >= 500) {
            funToastr(false, 'The maximum file size for an image is 500 KB. Please reduce your file size and try again');
        }
        else {
            if (/^image\/\w+/.test(file.type)) {
                var reader = new FileReader();
                reader.onloadend = function () {
                    $('#profile_pic').attr('src', reader.result);
                    $(inputImage).attr("data-base64string", reader.result);
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

$("#btnUpload").on("click", function () {
    var documentFile = $("#inputImage").get(0).files;

        var documentFileExt = documentFile[0].name.split('.').pop();
    var docfile = $("#inputImage").attr('data-base64string');
        docfile = docfile + ',' + documentFileExt;
        var data = {    
            Id: orderitem,
            OrderId: orderid,
            UserId: userid,
            VideoFile: $("#videoFile").attr('data-base64string'),
            DocumentFile: docfile,

            ZipFile: $("#zipFile").attr('data-base64string'),
            OrderStatus: $('#orderstatus').val()
        }

        StartProcess();
        $.ajax({
            url: '/Orders/UploadVideo',
            type: "POST",
            data: { orderitemsVM: data },
            success: function (result) {
                StopProcess();
                $("#btnUpload").prop('disabled', true)
                $("#lblError").addClass("success").text(result.status).show();
            },
            error: function (err) {
                StopProcess();
                $("#lblError").addClass("error").text(err.status).show();
            }
        });
    

});