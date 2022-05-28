$('input[type="radio"]').click(function () {
    var questionId = $(this).attr('question-id');
    $('#entire-container-' + questionId).removeClassStartingWith('question-border-').addClass('question-border-green');
})