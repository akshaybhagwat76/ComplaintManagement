var isValidPolicy = false;
    var retvalDetails = true;
var PolicyDetailsMaster = [];

function submitForm() {
    debugger;
    $("#lblError").removeClass("success").removeClass("adderror").text('');
    var retval = true;
    $("#myForm .required").each(function () {

        debugger;
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

    if (isValidPolicy) {
        $("#PolicyName").addClass("adderror");
        funToastr(false, "This policy is already exist."); return;
    }

    if (retval && !isValidPolicy) {

        debugger;
        var data = {
            id: ($("#PolicyId").val() != undefined || $("#PolicyId").val() != '') ? $("#PolicyId").val() : 0,
            PolicyName: $("#PolicyName").val().trim(),
            Status: $("#Status").val() == "true" ? true : false,
            CompanyId: $("#CompanyId").val(),
            PolicyNumber: $("#PolicyNumber").val(),
            TimeCode: $("#TimeCode").val(),
            Lastcertificatenumber: $("#Lastcertificatenumber").val(),
            Validsince: $("#Validsince").val(),
            Validuntil: $("#Validuntil").val(),
            OperationId: $("#OperationId").val(),
            Able: $("#Able").val(),
            Dry: $("#Dry").val(),
            InternationalCostDry: $("#InternationalCostDry").val(),
            InternationalCostReefer: $("#InternationalCostReefer").val(),
            PolicyDetailsMaster: PolicyDetailsMaster,
            Reefer: $("#Reefer").val(),
            Observations: $("#Observations").val()

        }
        StartProcess();
        $.ajax({
            type: "POST",
            url: "/Policy/AddOrUpdatepolicy",
            data: { policyVM: data },
            success: function (data) {
                if (data.status == "Fail") {
                    StopProcess();
                    $("#lblError").addClass("adderror").text(data.error).show();
                }
                else {
                    window.location.href = '/Policy/Index'
                }
            }
        });
    }
}







$(document).ready(function () {
    debugger;
    if ($("#PolicyId").val() === "0") {
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
    debugger;
    var PolicyName = $("#PolicyName").val();
    var Id = $("#Id").val();
    var data = { PolicyName: PolicyName, Id: Id }
    if (PolicyName !== "") {
        $.post("/Policy/CheckIfExist", { data: data }, function (data) {
            debugger
            if (data != null) {
                if (data.data) {
                    isValidPolicy = true;
                    $("#PolicyName").addClass("adderror");
                    funToastr(false, 'This policy is already exist.');
                }
                else {
                    isValidPolicy = false;
                    $("#PolicyName").removeClass("adderror");
                }
            }
        })

    }
}
$(function () {
    $("#deletebtn").click(function () {
     

        if ($("tbody > tr").length == 0) {
            $('#deletebtn').css('display', 'none');
        }
        Swal.fire({
            title: 'Are you sure?',
            text: "You won't be able to revert this!",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Yes, delete it!'
        }).then((result) => {
            if (result.isConfirmed) { 
                $("#del").find("tr:not(:first)").remove();
                Swal.fire(
                    'Deleted!',
                    'Your file has been deleted.',
                    'success'
                )
            }
        })      
    });
});





function submit() {
    $("#lblError").removeClass("success").removeClass("adderror").text('');
    $("#form .required").each(function () {
        if (!$(this).val()) {
            var $label = $("<label class='adderror'>").text('This field is required.');
            if ($(this).parent().find("label").length == 1) {
                $(this).parent().append($label);

                $(this).addClass("adderror");
            }
            retvalDetails = false;
        }
        else {
            if ($(this).parent().find("label").length > 1) {
                $(this).parent().find("label:eq(1)").remove();
                $(this).removeClass("adderror");
            }
        }
    });

    
}
$('#deletebtn').css('display', 'none');

 
$('#dataupload').on('click', function () {

   

    if ($('#Requirement').val() == '---SelectName---') {
        var $label = $("<label class='adderror'>").text('This field is required.');
        $('#Requirement').parent().append($label);
        $('#Requirement').addClass("adderror");
    }  
     else {
        if ($('#Requirement').parent().find("label").length >= 1) {
            $('#Requirement').parent().find("label:eq(1)").remove();
            $('#Requirement').removeClass("adderror");
        }
    }

    if ($('#SecurityRequirementsCommodity').val() == '---SelectName---') {
        var $label = $("<label class='adderror'>").text('This field is required.');
        $('#SecurityRequirementsCommodity').parent().append($label);
        $('#SecurityRequirementsCommodity').addClass("adderror");
    }  
    else {
        if ($('#SecurityRequirementsCommodity').parent().find("label").length >= 1) {
            $('#SecurityRequirementsCommodity').parent().find("label:eq(1)").remove();
            $('#SecurityRequirementsCommodity').removeClass("adderror");
        }
    }


       
   
    var Security = $('#SecurityRequirementsCommodity').val();
    var Requirement = $('#Requirement').val();
    var Minimum = $('#Minimum').val();
    var Maximum = $('#Maximum').val();
    var obj = {

        "SecurityRequirementsCommodity": Security,
        "Requirement": Requirement,
        "Minimum": Minimum,
        "Maximum": Minimum,
    }
    if (retvalDetails) {
        PolicyDetailsMaster.push(obj);
    
    if (Security != '---SelectName---' && Requirement != '---SelectName---' && Minimum != '' && Maximum != '') {
        $('#deletebtn').show(); 
        var data = '<tr class="trpolicy" data-id=' + PolicyDetailsMaster.length+'><td>' + Security + '</td> <td>' + Requirement + '</td> <td> ' + Minimum + '</td> <td>' + Maximum + '</td><td><a class="deletepolicy" href="#"> Delete </a></td></tr>';
        debugger
        $('tbody').append(data);

        $('#Minimum').val('');
        $('#Maximum').val('');
        $("#Requirement").val($("#Requirement option:first").val());
        $("#SecurityRequirementsCommodity").val($("#SecurityRequirementsCommodity option:first").val());

    } 
    }
    $('.deletepolicy').on('click', function () {
        var rowIndex = $(this).find("tr.trpolicy").data("id");
        debugger
       
        console.log($(this).parent().parent().remove())
        //$(this).find("tr").remove();
        delete PolicyDetailsMaster[parseInt($(this).closest('tr').attr('data-id'))];
    })

   
    /*$('.deletepolicy').remove();*/
    var htmlString = $(myForm).html();
});
