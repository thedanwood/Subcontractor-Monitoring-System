//JS for signature pad
var originalWidth = $(window).width();
// Adjust canvas coordinate space taking into account pixel ratio,
// to make it look crisp on mobile devices.
// This also causes canvas to be cleared.
function resizeCanvas(canvas, newSection) {
    // When zoomed out to less than 100%, for some very strange reason,
    // some browsers report devicePixelRatio as less than 1
    // and only part of the canvas is cleared then.
    //var ratio = Math.max(window.devicePixelRatio || 1, 1);
    var ratio = 1;
    //var width = canvas.offsetWidth == 0 ? $(canvas).closest(".wrapper").width() : canvas.offsetWidth;
    var width = $(canvas).closest(".wrapper").width();
    var height = canvas.offsetHeight == 0 ? 400 : canvas.offsetHeight;
    //check is required to fix a bug on devices where the resize was being called when keyboard was in use
    if ((width != originalWidth && canvas.offsetWidth != 0) || newSection) {
        canvas.width = width * ratio;
        canvas.height = height * ratio;
        canvas.getContext("2d").scale(ratio, ratio);
        originalWidth = width;
    }
}

$(document).ready(function () {
    $(".signature-pad").each(function () {
        var canvas = $(this);
        var signaturePad = new SignaturePad(canvas.get(0), {
            backgroundColor: 'rgb(255, 255, 255, 0)' // necessary for saving image as JPEG; can be removed is only saving as PNG or SVG
        });
        resizeCanvas(canvas.get(0), false);
    });
});

$(window).resize(function () {
    $(".signature-pad").each(function () {
        var canvas = $(this);
        resizeCanvas(canvas.get(0), false);
    });
});

$(window).on("orientationchange", function () {
    $(".signature-pad").each(function () {
        var canvas = $(this);
        resizeCanvas(canvas.get(0), false);
    });
});


