$(document).ready(function () {

    $("#lblError").removeClass("success").removeClass("error").text('');
    $('form').each(function () {
        $(this).find('input').keypress(function (e) {
            // Enter pressed?
            if (e.which == 10 || e.which == 13) {
                $("#btn-submit").click();
            }
        });
    });
    $("#btn-submit").on("click", function () {
        debugger
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
        var email = $("#Email").val().trim();
        var password = $("#Password").val().trim();
        var returnUrl = $('#myForm').data("url").trim();
        if (retval) {
            var data = {
                Email: email,
                Password: password,
                IsDeactive: $("#customCheck").is(":checked")
            }
            StartProcess();
            $.ajax({
                type: "POST",
                url: "/Account/Login",
                data: { loginVM: data },
                success: function (data) {
                    if (data.status == "Fail") {
                        StopProcess();
                        $("#lblError").addClass("adderror").text(data.error).show();
                    }
                    else {
                        if (returnUrl != null && returnUrl != "")
                            window.location.href = $('#myForm').data("url");
                        else
                            window.location.href = '/Category/Index';
                    }
                }
            });
        }
    })
});