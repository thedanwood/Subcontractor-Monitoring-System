function areAllChecksRespondedTo() {
    var allChecksConfirmed = true;
    $('.check-' + subsectionId).each(function () {
        if (!$(this).is(':checked')) {
            allChecksConfirmed = false;
        }
    })
    if (!allChecksConfirmed) {
        showWarningAlert('#alert-checklist-completion', 'Please confirm all the checklist questions');
    }
    return allChecksConfirmed;
}
function areCommentsValid() {
    $('#checklist-question-comment-form').validate();
    var checklistAlertSelector = '#checklist-alert-provide-comment';
    commentsValid = true;
    $(".add-information-textarea").each(function () {
        if (!$(this).valid()) {
            commentsValid = false;
        }
    })
    if (!commentsValid) {
        showWarningAlert(checklistAlertSelector, 'Please provide comments outlining any additional works that are required to the checklist questions that have been marked as rejected or conditional.');
    } else {
        $(checklistAlertSelector).fadeOut('500');
    }
    return commentsValid;
}
$('#save-' + subsectionId).click(function () {
    if (areAllChecksRespondedTo() && isSignatureProvided() && areCommentsValid()) {
        saveAllFilesUploaded();

        var data = getChecklistSaveData();
        
        $.ajax({
            url: saveUrl,
            type: 'post',
            data: data,
            beforeSend: function () {
                $('#loader').show();
            },
            success: function (result) {
                $(window).scrollTop(0);
                alert('reached!!!');
                window.location.reload();
            }
        });
    }
})
function getChecklistSaveData() {
    var data = [];
    var canvas = document.getElementById('signature-pad-' + subsectionId);
    var sigdata = canvas.toDataURL();
    sigdata = sigdata.substring(sigdata.indexOf(',') + 1);
    var printName = $('#print-name-' + subsectionId).val();
    data = {
        ChecklistQuestionResponses: getChecklistQuestionResponses(),
        PrintName: printName,
        Signature: sigdata,
        plotId: plotId,
        subsectionId: subsectionId,
    }
    return data;
}
function getChecklistQuestionResponses() {
    var checklistQuestionResponses = [];
    $('.response-radio-container').each(function () {
        var radioSelector = $(this).find('.response-radio:checked');
        var questionid = $(radioSelector).attr('question-id');
        var response = $(radioSelector).attr('response-enum-value');
        var note = $('#textarea-' + questionid).val();
        checklistQuestionResponses.push({
            Note: note,
            QuestionID: questionid,
            ApproverResponseEnum: response,
        })
    })
    return checklistQuestionResponses;
}
function saveAllFilesUploaded() {
    $('.upload-file-form').each(function () {
        $(this).submit(function (e) {
            e.preventDefault();
            $.ajax({
                url: this.action,
                type: this.method,
                data: new FormData(this),
                cache: false,
                contentType: false,
                processData: false,
            });
        });
        $(this).submit();
    });
}