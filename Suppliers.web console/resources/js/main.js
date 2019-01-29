
function funCity() {
    $("#PopupCity").css("display", "block");
    $(".popup-black").css("display", "block");
    //return false;
}

function funTerms(id) {
    if (is_approved == "false") {
        $("#popupTerms").css("display", "block");
        $(".popup-black").css("display", "block");
        return false;
    }
    return true;
}

function funHomeCity() {
    $("#PopupCityHome").css("display", "block");
    $(".popup-black").css("display", "block");
    //return false;
}


function funclose(obj)
{
    $(obj).parent().css("display", "none");
    $(".popup-black").css("display", "none");    
}
function MainChecked(obj)
{
    var status = $(obj).is(':checked');
    $('input[type="checkbox"]', $(obj).parent().next()).attr('checked', status);
    $(obj).parent().next().toggle();
}


//$(".top-checkbox label").click(function () { $("table", $(this).parent()).toggle(); })
function CloseOrder(obj)
{
    $(obj).parent("#Popup").hide();
    $(obj).parent("#Popup").prev().hide();
}

function printDiv() {
    var printContents = $(".items-list").parent().html();
    var originalContents = document.body.innerHTML;
    printContents = printContents.replace(/<div class="vi-class">/g,"V");
    document.body.innerHTML = printContents;
    window.print();

    document.body.innerHTML = originalContents;
}

function OrderDetails(obj) {
    var order_id = $(obj).attr("OrderId");
    $("#Popup").load("OrderDetails.aspx?OrderId=" + order_id);
    $("#Popup").show();
    $(".popup-black").show();
    return false;
};
function toggleMenu() {
    if ($(".ul-menu").css("display") == "none") {
        $(".ul-menu").css("display", "block");
    }
    else {
        $(".ul-menu").css("display", "none");
    }
}

function openMyCity(obj)
{
    var display = $(obj).next().css("display")
    $(".wrapper-columns span.city-checkbox").css("display", "none");
    if (display == "block") {
        $(obj).next().css("display", "none")
    }
    else {
        $(obj).next().css("display", "block")
    }
    if ($(obj).attr("src").indexOf("arrow_down") > -1) {
        $(obj).attr("src", "resources/images/arrow_up.png")
    }
    else {
        $(obj).attr("src", "resources/images/arrow_down.png")
    }
}

var t = 0;
var t1 = 0;
$(document).ready(function () {
    $(".top-checkbox input[type=checkbox]").change(function () {
        var status = $(this).is(':checked');
        $('input[type="checkbox"]', $(this).parent().next().next()).attr('checked', status);
   
    });
    $(".wrapper-columns img").mousedown(function () {
        var src = $(this).attr("src").split(".");
        if (src.length > 0) {
            $(this).attr("src", src[0] + "_pressed.png");
        }
    });
 
    $(".li-menu").each(function () { if ($(this).html().trim().length === 0) { $(this).hide(); } });
    $(".file").change(function () { $(".fakefile input[type=text]").val($(this).val()); });
    $(window).resize(function () { if ($(window).width() > 1085) { $(".ul-menu").css("display", "block"); } else { $(".ul-menu").css("display", "none"); } });
});
