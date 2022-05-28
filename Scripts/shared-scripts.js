////$(document).ajaxStart(function () {
////    $('#loader').show();
////})
////$(document).ajaxStop(function () {
////    $('#loader').hide();
////})
//prevent spam click
$('.save').click(function() {
    $(this).attr('disabled');
})
//prevent load in to bottom glitch
$(window).load(function () {
    $(window).scrollTop();
})
function showWarningAlert(alertSelector, alertText) {
    $(alertSelector).html('<i class="fas fa-times pr-2"></i>' + alertText);
    showHideAlert(alertSelector);
}
function showSuccessAlert(alertSelector, alertText) {
    $(alertSelector).html('<i class="fas fa-check pr-2"></i>' + alertText);
    showHideAlert(alertSelector);
}
function showHideAlert(alertSelector) {
    $(alertSelector).slideToggle().delay('5000').slideToggle();
}
function replaceAmpersands(link) {
    link = link.replace(/\&amp;/g, '&');
    return link
}
function validateEmail($email) {
    var emailReg = /^([\w-\.]+@([\w-]+\.)+[\w-]{2,4})?$/;
    return emailReg.test($email);
}