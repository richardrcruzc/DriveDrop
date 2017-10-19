// Write your Javascript code.


var stickySidebar = new StickySidebar('#sidebar', {
    topSpacing: 150,
    containerSelector: '.container',
    innerWrapperSelector: '.sidebar__inner'
});

$(document).ready(function () {
   // $(document).on("scroll", onScroll);

    //smoothscroll
    $('.gig-information').on('click', function (e) {
        
        //e.preventDefault();
       // $(document).off("scroll");

        $('a').each(function () {
            $(this).removeClass('active');
        })

        $(this).addClass('active');
        
        //var target = this.hash,
        //    menu = target;
        //$target = $(target);
        //$('html, body').stop().animate({
        //    'scrollTop': $target.offset().top + 2
        //}, 500, 'swing', function () {
        //    window.location.hash = target;
        //    $(document).on("scroll", onScroll);
        //});
    });
});

//function onScroll(event) {
//    var scrollPos = $(document).scrollTop();
//    $('#side-navbar a').each(function () {
//        var currLink = $(this);
//        var refElement = $(currLink.attr("href"));
//        if (refElement.position().top <= scrollPos && refElement.position().top + refElement.height() > scrollPos) {
//            $('#menu-center ul li a').removeClass("active");
//            currLink.addClass("active");
//        }
//        else {
//            currLink.removeClass("active");
//        }
//    });
//}


$(function () {
    var priorityClock = document.getElementsByClassName('priorityClock');
    if (priorityClock.length) {
        $(".priorityClock").each(function () {

            var temp = $(this);
            //alert($(this).data("pickupdate"));

            // Set the date we're counting down to
            // var countDownDate = new Date($.now()).getTime();
            var countDownDate = new Date(temp.data("pickupdate")).getTime();
            // Update the count down every 1 second
           var x = setInterval(function () {

                // Get todays date and time
                var now = new Date().getTime();

                // Find the distance between now an the count down date
                var distance = countDownDate - now;

                // Time calculations for days, hours, minutes and seconds
                var days = Math.floor(distance / (1000 * 60 * 60 * 24));
                var hours = Math.floor((distance % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60));
                var minutes = Math.floor((distance % (1000 * 60 * 60)) / (1000 * 60));
                var seconds = Math.floor((distance % (1000 * 60)) / 1000);

                // Display the result in the element with id="demo"
//                document.getElementById("demo").innerHTML = days + "d " + hours + "h "
//                    + minutes + "m " + seconds + "s ";


                temp.html(days + "d " + hours + "h "+ minutes + "m " + seconds + "s ");


                // If the count down is finished, write some text 
                if (distance < 0) {
                    clearInterval(x);
  //                  document.getElementById("demo").innerHTML = "EXPIRED";
                    temp.html("EXPIRED");
                }

            
            }, 1000);

        });
    }

});



//$('.acept-package').keypress(function (e) {  
     
//    var str = $(this).val();
     
//    $("#dynamic").load('/drive/ShippingSecurityCode', { id:@(Model.DriverId) },
//        function () {
//        });
//});
//$('.acept-package').keypress(function (e) { return e.which != 13; });



//if ($("#DriverNotificationWS").length) {
    var socket;
   // var sendButton = document.getElementById("sendButton");
    var sendMessage = document.getElementById("sendMessage");
   // var commsLog = document.getElementById("DriverNotificationWS");
    var scheme = document.location.protocol == "https:" ? "wss" : "ws";
    var port = document.location.port ? (":" + document.location.port) : "";
    var connectionUrl  = scheme + "://" + document.location.hostname + port + "/ws";

    socket = new WebSocket(connectionUrl); 
     
    socket.onopen = function (event) {
       
       // updateState();
        //if ($("#DriverNotificationWS").length) {
        //    commsLog.innerHTML += 'Connection opened <br />';
        //}
    };
    socket.onclose = function (event) {
       
        //updateState();
        //if ($("#DriverNotificationWS").length) {
        //    commsLog.innerHTML += 'Connection closed. Code: ' + htmlEscape(event.code) + '. Reason: ' + htmlEscape(event.reason) + " <br />";
        //}
    };
    //socket.onerror = updateState;
    socket.onmessage = function (event) {
        
        if ($("#You-Got-Packages").length) { 
            $('#You-Got-Packages').removeClass('hidden');
        //    commsLog.innerHTML += htmlEscape(event.data) + " <br />";
        }
        
    };

 
    //sendButton.onclick = function () {
       
    //    if (!socket || socket.readyState != WebSocket.OPEN) {
    //        alert("socket not connected");
    //    }
    //   // var data = sendMessage.value;
    //   // socket.send('sfdfsfsfsfsfsdf'); 
    //}

    function htmlEscape(str) {
         return str
            .replace(/&/g, '&amp;')
            .replace(/"/g, '&quot;')
            .replace(/'/g, '&#39;')
            .replace(/</g, '&lt;')
            .replace(/>/g, '&gt;');
    }

    function htmlSendNotice(htmlStr) {

        //if (!socket || socket.readyState != WebSocket.OPEN) {
        //    alert("socket not connected");
        //}

        socket.send(htmlStr);
    }
//}
 
//function test(){
//var connectionForm = document.getElementById("connectionForm");
//var connectionUrl = document.getElementById("connectionUrl");
//var connectButton = document.getElementById("connectButton");
//var stateLabel = document.getElementById("stateLabel");
//var sendMessage = document.getElementById("sendMessage");
//var sendButton = document.getElementById("sendButton");
//var sendForm = document.getElementById("sendForm");
//var commsLog = document.getElementById("commsLog");
//var socket;
//var scheme = document.location.protocol == "https:" ? "wss" : "ws";
//var port = document.location.port ? (":" + document.location.port) : "";
//connectionUrl.value = scheme + "://" + document.location.hostname + port + "/ws";
//function updateState() {
//    function disable() {
//        sendMessage.disabled = true;
//        sendButton.disabled = true;
//        closeButton.disabled = true;
//    }
//    function enable() {
//        sendMessage.disabled = false;
//        sendButton.disabled = false;
//        closeButton.disabled = false;
//    }
//    connectionUrl.disabled = true;
//    connectButton.disabled = true;
//    if (!socket) {
//        disable();
//    } else {
//        switch (socket.readyState) {
//            case WebSocket.CLOSED:
//                stateLabel.innerHTML = "Closed";
//                disable();
//                connectionUrl.disabled = false;
//                connectButton.disabled = false;
//                break;
//            case WebSocket.CLOSING:
//                stateLabel.innerHTML = "Closing...";
//                disable();
//                break;
//            case WebSocket.CONNECTING:
//                stateLabel.innerHTML = "Connecting...";
//                disable();
//                break;
//            case WebSocket.OPEN:
//                stateLabel.innerHTML = "Open";
//                enable();
//                break;
//            default:
//                stateLabel.innerHTML = "Unknown WebSocket State: " + htmlEscape(socket.readyState);
//                disable();
//                break;
//        }
//    }
//}
//closeButton.onclick = function () {
//    if (!socket || socket.readyState != WebSocket.OPEN) {
//        alert("socket not connected");
//    }
//    socket.close(1000, "Closing from client");
//}
//sendButton.onclick = function () {
//    if (!socket || socket.readyState != WebSocket.OPEN) {
//        alert("socket not connected");
//    }
//    var data = sendMessage.value;
//    socket.send(data);
//    commsLog.innerHTML += '<tr>' +
//        '<td class="commslog-client">Client</td>' +
//        '<td class="commslog-server">Server</td>' +
//        '<td class="commslog-data">' + htmlEscape(data) + '</td>'
//    '</tr>';
//}
//connectButton.onclick = function () {
//    stateLabel.innerHTML = "Connecting";
//    socket = new WebSocket(connectionUrl.value);
//    socket.onopen = function (event) {
//        updateState();
//        commsLog.innerHTML += '<tr>' +
//            '<td colspan="3" class="commslog-data">Connection opened</td>' +
//            '</tr>';
//    };
//    socket.onclose = function (event) {
//        updateState();
//        commsLog.innerHTML += '<tr>' +
//            '<td colspan="3" class="commslog-data">Connection closed. Code: ' + htmlEscape(event.code) + '. Reason: ' + htmlEscape(event.reason) + '</td>' +
//            '</tr>';
//    };
//    socket.onerror = updateState;
//    socket.onmessage = function (event) {
//        commsLog.innerHTML += '<tr>' +
//            '<td class="commslog-server">Server</td>' +
//            '<td class="commslog-client">Client</td>' +
//            '<td class="commslog-data">' + htmlEscape(event.data) + '</td>'
//        '</tr>';
//    };
//};
//function htmlEscape(str) {
//    return str
//        .replace(/&/g, '&amp;')
//        .replace(/"/g, '&quot;')
//        .replace(/'/g, '&#39;')
//        .replace(/</g, '&lt;')
//        .replace(/>/g, '&gt;');
//}

//}

 
//$(function () {
//    var socket;
//    var protocol = location.protocol === "https:" ? "wss:" : "ws:";
//    var uri = protocol + "//" + window.location.host + "/ws";
//    var output;
//    var text = "test echo111";

//function write(s) {
//    var p = document.createElement("p");
//    p.innerHTML = s;
//    output.appendChild(p);
//}

//function doConnect() {
//    socket = new WebSocket(uri);
//    socket.onopen = function (e) { write("opened " + uri); doSend(); };
//    socket.onclose = function (e) { write("closed"); };
//    socket.onmessage = function (e) { write("Received: " + e.data); socket.close(); };
//    socket.onerror = function (e) { write("Error: " + e.data); };
//}

//function doSend() {
//    write("Sending: " + text);
//    socket.send(text);
//}

//function onInit() {
//    output = document.getElementById("DriverNotificationWS");
//    doConnect();
//}

//if ($("#DriverNotificationWS").length) {
//    window.onload = onInit;
    
//}
//});


//$(function () {
   // var userName = '@Model';

    //var protocol = location.protocol === "https:" ? "wss:" : "ws:";
    //var wsUri = protocol + "//" + window.location.host + "/ws";
    //var socket = new WebSocket(wsUri);

    //socket.onopen = e => {
    //    console.log("socket opened", e);
    //};

    //socket.onclose = function (e) {
    //    console.log("socket closed", e);
    //};

    //socket.onmessage = function (e) {
    //    console.log(e);
    //    $('#DriverNotificationWS').append(e.data + '<br />');
    //};

    //socket.onerror = function (e) {
    //    console.error(e.data);
    //};


    //$('#MessageField').keypress(function (e) {
    //    if (e.which != 13) {
    //        return;
    //    }

    //    e.preventDefault();

    //    var message = " userName +: " + $('#MessageField').val();
    //    socket.send(message);
    //    $('#MessageField').val('');
    //});
//});

$(document).ready(function () {
    $('.footer-block .title').click(function () {
        var e = window, a = 'inner';
        if (!('innerWidth' in window)) {
            a = 'client';
            e = document.documentElement || document.body;
        }
        var result = { width: e[a + 'Width'], height: e[a + 'Height'] };
        if (result.width < 769) {
            $(this).siblings('.list').slideToggle('slow');
        }
    });
});
$(document).ready(function () {
    $('.block .title').click(function () {
        var e = window, a = 'inner';
        if (!('innerWidth' in window)) {
            a = 'client';
            e = document.documentElement || document.body;
        }
        var result = { width: e[a + 'Width'], height: e[a + 'Height'] };
        if (result.width < 1001) {
            $(this).siblings('.listbox').slideToggle('slow');
        }
    });
});


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
         img.width = "700";
         img.height = "500";

    });
    $('.imgSum').on('mouseleave', function () {
        var img = this;
        img.width = "150";
        img.height = "150";
    });


    function openNav() {
        document.getElementById("mySidenav").style.width = "100%";
    }

    function closeNav() {
        document.getElementById("mySidenav").style.width = "0";
    }


    //$(document).ready(function () {
    //    $(window).scroll(function () {
    //        var scroll = $(window).scrollTop();
    //        if (scroll > 50) {
    //            $("header").css("background", "#223669");
    //        }

    //        else {
    //            $("header").css("background", "none");
    //        }
    //    })
    //})

    //var owl = $('.owl-testi');
    //owl.owlCarousel({
    //    items: 3,
    //    loop: true,
    //    margin: 10,
    //    autoplay: true,
    //    autoplayTimeout: 1000,
    //    autoplayHoverPause: true
    //});
    //$('.iframeClass').on('click', function () {
    //    alert(1);
    //    $(this).height(800);
    //    //var img = this;
    //    //img.width = "500%";
    //    //img.height = "500%";

    //});
    //$('.iframeClass').on('mouseleave', function () {
    //    var img = this;
    //    img.width = "100%";
    //    img.height = "100%";
    //});

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

