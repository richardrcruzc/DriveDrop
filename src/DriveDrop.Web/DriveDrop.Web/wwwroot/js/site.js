// Write your Javascript code.

$(document).ready(function () {
    $(":input").inputmask(); 
});

function mouseOver() {
    var img1 = document.getElementById("img1");
    img1.src = "images/p2.jpg";
    img1.width = "";
    img1.height = "";
}
function mouseOut() {
    var img1 = document.getElementById("img1");
    img1.src = "images/p1.jpg";
    img1.width = "90";
    img1.height = "110";
}

$(document).ready(function () {

    $('.imgSum').on('click', function () {
        var img = this;
         img.width = "500";
         img.height = "500";

    });
    $('.imgSum').on('mouseleave', function () {
        var img = this;
        img.width = "150";
        img.height = "150";
    });

    $('.iframeClass').on('click', function () {
        var img = this;
        img.width = "500%";
        img.height = "500%";

    });
    $('.iframeClass').on('mouseleave', function () {
        var img = this;
        img.width = "100%";
        img.height = "100%";
    });

    //var $tooltip = $('#fullsize');

    //$('img').on('mouseenter', function () {
    //    var img = this,
    //        $img = $(img),
    //        offset = $img.offset();

    //    $tooltip
    //        .css({
    //            'top': offset.top,
    //            'left': offset.left
    //        })
    //        .append($img.clone())
    //        .removeClass('hidden');
    //});

    //$tooltip.on('mouseleave', function () {
    //    $tooltip.empty().addClass('hidden');
    //});

});

