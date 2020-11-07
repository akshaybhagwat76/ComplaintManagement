function isEmail(email) {
    var regex = /^([a-zA-Z0-9_.+-])+\@(([a-zA-Z0-9-])+\.)+([a-zA-Z0-9]{2,4})+$/;
    return regex.test(email);
}
function StartProcess() {
    $(".preloader").fadeIn("slow");
}
function StopProcess() {
    $(".preloader").fadeOut("slow");
}
function bytesToSize(bytes) {
    if (bytes == 0) return '0 KB';
    var iSize = (bytes / 1024);
    return parseInt(Math.round(iSize * 100) / 100);
};
function funToastr(status, msg) {
    toastr.options = {
        "closeButton": true,
        "debug": false,
        "progressBar": true,
        "positionClass": "toast-top-right UserToaster",
        "onclick": null,
        "showDuration": "2000",
        "hideDuration": "1000",
        "timeOut": "7000",
        "extendedTimeOut": "1000",
        "showEasing": "swing",
        "hideEasing": "linear",
        "showMethod": "fadeIn",
        "hideMethod": "fadeOut"
    }
    if (status) {
        toastr.success('', msg)
    }
    else {
        toastr.error('', msg)
    }
}

function prepareDatatable(pageLength, selector, options, datatable_options, ExtraParams, initCallback, fnCallback) {
    debugger;
    var defOptions = {
        //"sDom": 'Rfrtlip',
        destroy: true,
        responsive: true,
        "autoWidth": false,
        "pageLength": pageLength,
        //dom: '<"pull-left"f><"pull-right"l>tip',
        dom: '<"pull-left"f>tlip',
        buttons: [
        ],
        "oLanguage": {
            "sSearch": ""
        },
        serverSide: true,
        searchDelay: 2000,
        ajax: {
            url: options.filterUrl,
            type: 'POST',
            data: function (d) {
                StartProcess();
                var addlData = {};
                d = $.extend({}, d, addlData);
                $.extend(d, ExtraParams);
                return d;
            }
        },
        initComplete: function () {
            var selector_wrapper = selector + "_filter";

            var data = '<button class="btn btn-primary" type="button" id="DetailSearch">Search</button>';
            $(selector_wrapper).find('input').replaceWith('<input type="text" class="form-control input-sm" id="txtDetailSearch" />');
            $("#txtDetailSearch").after(data);

            var selector_wrapper = selector + "_wrapper";
            $(selector_wrapper).on("click", "#btnJumpTo", function () {
                $(selector_wrapper + ' #txtJumpTo').removeClass("error");
                var start = /^[0-9]+$/.test($(selector_wrapper + ' #txtJumpTo').val());
                var info = tabApi.page.info();
                if (start && info.pages <= ($(selector_wrapper + ' #txtJumpTo').val() - 1)) {
                    tabApi.page(0);
                    tabApi.reloadGridData(null, null);
                }
                else if (start && ($(selector_wrapper + ' #txtJumpTo').val() - 1) >= 0) {
                    tabApi.page(parseInt($(selector_wrapper + ' #txtJumpTo').val() - 1));
                    tabApi.reloadGridData(null, null);
                }
                else {
                    $(selector_wrapper + ' #txtJumpTo').addClass("error");
                }
            });

            var info = tabApi.page.info();
            if (page + 1 <= info.pages) {
                tabApi.page(parseInt(page));
                tabApi.reloadGridData(null, null);
            }

            $(selector_wrapper).on("click", "#DetailSearch", function () {
                var ExtraParams = getFilters();
                var search = {
                    regex: false,
                    value: $(selector_wrapper + " #txtDetailSearch").val()
                };
                ExtraParams.search = search;
                tabApi.page(0);
                tabApi.reloadGridData(null, ExtraParams);
            });

            if (initCallback && typeof (initCallback) === "function") {
                initCallback(selector);
            }

            $(selector).show();
        },
        "fnDrawCallback": function (oSettings) {
            var paginate = selector + "_paginate";
            var addPaging = '<li><span class="jumpTo">Jump to</span></li>\
                <li><input type="number" class="form-control" min="1" id="txtJumpTo"></li>\
                <li><button id="btnJumpTo" class="btn btn-primary" type="button">GO</button></li>';
            $(paginate + " .pagination").append(addPaging);

            if (fnCallback && typeof (fnCallback) === "function") {
                fnCallback(selector);
            }
            $('[data-toggle="tooltip"]').tooltip();
            StopProcess();
        },
    };

    var _options = $.extend(defOptions, datatable_options);

    var $tab,
       tabApi;
    $tab = $(selector);
    tabApi = $tab.DataTable(_options);
    tabApi.reloadGridData = function (callback, data) {
        if (data)
            $.extend(ExtraParams, data)
        tabApi.ajax.reload(callback, false);
    }

    if (typeof generateFilter !== 'undefined' && generateFilter) {
        generateFilter(selector);
    }
    return tabApi;
}

function setDataTableDetail(SessionName, redirectUrl, info) {
    $.ajax({
        type: "POST",
        url: "/Home/setDataTableDetail",
        data: { SessionName: SessionName, PageLength: info.length, Page: info.page },
        success: function (response) {
            if (redirectUrl != '')
                window.location.href = redirectUrl;
        }
    });
    return true;
}


function PasswordChecker(password) {
    if (password != "") {
        if (password.length < 6)
            return "Password must contain minimum 6 characters with at least one capital Letter and one special Character.";

        re = /[A-Z]/;
        if (!re.test(password))
            return "Password must contain minimum 6 characters with at least one capital Letter and one special Character.";

        re = /[!@#$%\^&*(){}[\]<>?/|\-]/;
        if (!re.test(password))
            return "Password must contain minimum 6 characters with at least one capital Letter and one special Character.";
    }
    else {
        return "Password is required field.";
    }
    return "success";
}

function checkPassword(field, rules, i, options) {
    var password = $("#password").val().trim();
    var ReenterPassword = $("#ReenterPassword").val().trim();
    if (password != "" && ReenterPassword != "") {
        if (password == ReenterPassword) {
            var passwordMsg = PasswordChecker(password);
            if (password != "" && password != null) {
                if (passwordMsg != "success") {
                    return "<table><tr><td style='padding-right: 10px;vertical-align: top;font-size: 17px;color:#FAAB20'><i class='fa fa-warning'></i></td><td>" + passwordMsg + "</td></tr></table>";
                }
            }
        }
    }
}

function matchPassword(field, rules, i, options) {
    var password = $("#password").val().trim();
    var ReenterPassword = $("#ReenterPassword").val().trim();
    if (password != "" && ReenterPassword != "") {
        if (password != ReenterPassword) {
            return "<table><tr><td style='padding-right: 10px;vertical-align: top;font-size: 17px;color:#FAAB20'><i class='fa fa-warning'></i></td><td>Sorry this entry does not match your original Password, please try again.</td></tr></table>";
        }
    }
}

function encodeImagetoBase64($this) {
    var file = $this.files[0];
    var reader = new FileReader();
    reader.onloadend = function () {
        $($this).attr("data-base64string", reader.result);
        //PreviewBase64Image(reader.result, $this.id + "Preview");
    }
    reader.readAsDataURL(file);
}