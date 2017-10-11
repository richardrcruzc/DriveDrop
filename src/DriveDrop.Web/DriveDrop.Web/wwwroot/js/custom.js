$(document).ready(function(){
  $(window).scroll(function(){
  	var scroll = $(window).scrollTop();
	  if (scroll > 50) {
	    $("header").css("background" , "#223669");
	  }

	  else{
		  $("header").css("background" , "none");  	
	  }
  })
})

var owl = $('.owl-testi');
owl.owlCarousel({
    items:3,
    loop:true,
    margin:10,
    autoplay:false,
    autoplayTimeout:1000,
	responsive:{
        0:{
            items:1,
        },
        600:{
            items:2,
        },
        1000:{
            items:3,
            loop:false
        }
    },
    autoplayHoverPause:true
});