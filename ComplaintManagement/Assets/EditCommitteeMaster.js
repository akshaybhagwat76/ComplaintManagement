var isValidCommittee = false;
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
            $(this).parent().find("label").remove();
            $(this).removeClass("adderror");
        }
    });
    if (isValidCommittee) {
        $("#CommitteeName").addClass("adderror");
        funToastr(false, "This Committee is already exist."); return;
    }
    var userfields = $("input[name='UserList']").serializeArray();
    if (userfields.length === 0) {
        $("#lblErrorUser").addClass("adderror").text('Please select atleast one User.');
    }
    else {
        $("#lblErrorUser").removeClass("adderror").text('');
    }

    if (retval && !isValidCommittee && userfields.length > 0) {
        var data = {
            id: $("#Id").val().trim(),
            UserId: $("#UserIds").val().toString(),
            CommitteeName: $("#CommitteeName").val().trim(),
            Status: $("#Status").val() == "true" ? true : false
        }
        StartProcess();
        $.ajax({
            type: "POST",
            url: "/Committee/AddOrUpdateCommittee",
            data: { CommitteeVM: data },
            success: function (data) {
                if (data.status == "Fail") {
                    StopProcess();
                    $("#lblError").addClass("adderror").text(data.error).show();
                }
                else {
                    window.location.href = '/Committee/Index'
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
        if ($("#UserIds").val().includes(',')) {
            var UserIds = $('#UserIds').val().split(",")

            $.each(UserIds, function (i, keyword) {
                var el = $("#User-" + keyword);
                if (el[0] != null) {
                    el.attr("checked", true)
                }
            });

            $("#UserId").val(UserIds)
        }
        else {
            if ($("#UserIds").val() != "") {
                var el = $("#User-" + $("#UserIds").val());
                if (el[0] != null) {
                    el.attr("checked", true)
                }
            }
        }
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
$('.checks-User').change(function () {
    var values = $('.checks-User:checked').map(function () {
        return this.value;
    }).get().join(',');
    $('#UserIds').val(values);
});
function checkDuplicate() {
    var CommitteeName = $("#CommitteeName").val();
    var Id = $("#Id").val();
    var data = { CommitteeName: CommitteeName, Id: Id }
    if (CommitteeName !== "") {
        $.post("/Committee/CheckIfExist", { data: data }, function (data) {
            
            if (data != null) {
                if (data.data) {
                    isValidCommittee = true;
                    $("#CommitteeName").addClass("adderror");
                    funToastr(false, 'This Committee is already exist.');
                }
                else {
                    isValidCommittee = false;
                    $("#CommitteeName").removeClass("adderror");
                }
            }
        })

    }
}