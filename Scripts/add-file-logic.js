function getTotalFilesAddedForAllQuestionsCount() {
    return $('.upload-file-form').length;
}
function getFilesAddedCountInQuestion(questionID) {
    return $('.form-' + questionID).find('.file').length;
}
function getFileIDs(selector) {
    //created because was mismatching questionIDs when using Model.
    var questionID = $(selector).attr('question-id');
    var uniqueID = $(selector).attr('unique-id');
    return {
        questionID: questionID,
        uniqueID: uniqueID,
    }
}
//delete file
function deleteFile(selector) {
    fileIDs = getFileIDs(selector);
    var addFileContainerSelector = $(selector).parent().parent();

    //remove contents of file input
    var addFileElementSelector = '#file-' + fileIDs.questionID + '-' + fileIDs.uniqueID;
    $(addFileElementSelector).val('');

    //if unique id/count of add files currently in page is 1 then dont remove as there is only 1 add file
    if (fileIDs.uniqueID != 1) {
        $(addFileContainerSelector).remove();
    }
}

//add file
function addFile(AcceptorOrApproverEnum, selector, questionAttemptCountForNewFileSave) {
    var questionID = $(selector).attr('question-id');
    var filesAddedInQuestionCount = getFilesAddedCountInQuestion(questionID);

    //check if empty
    var empty = false;
    if ($('#file-' + questionID + '-' + filesAddedInQuestionCount).get(0).files.length == 0) {
        empty = true;
    }

    if (!empty) {
        getAddFilePartialView(AcceptorOrApproverEnum, questionID, questionAttemptCountForNewFileSave, filesAddedInQuestionCount);
    } else {
        showWarningAlert('#file-upload-error-' + questionID, 'Please upload a file before adding another.');
    }
}
function appendNewAddFilePartialView(partialViewData, questionID) {
    $('#add-file-forms-container-' + questionID).append(partialViewData);
}
function getAddFilePartialView(AcceptorOrApproverEnum, questionID, questionAttemptCountForNewFileSave, questionCurrentFilesAddedCount) {
    if (questionCurrentFilesAddedCount == null) {
        questionCurrentFilesAddedCount = 0;
    }
    data = {
        QuestionID: questionID,
        plotId: plotId,
        subsectionId: subsectionId,
        QuestionCurrentFilesAddedCount: questionCurrentFilesAddedCount,
        QuestionAttemptCountForNewFileSave: questionAttemptCountForNewFileSave,
        AcceptorOrApproverEnum: AcceptorOrApproverEnum,
    }
    var url = relativePath + "Checklist/GetAddFileFormPartialView";
    console.log(relativePath);
    $.ajax({
        url: url,
        type: 'post',
        data: data,
        success: function (data) {
            appendNewAddFilePartialView(data, questionID);
        }
    });
}