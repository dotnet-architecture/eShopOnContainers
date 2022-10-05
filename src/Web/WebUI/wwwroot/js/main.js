/***************************************************
==================== JS INDEX ======================
****************************************************
01. PreLoader Js
02. Mobile Menu Js
03. Sidebar Js
04. Cart Toggle Js
05. Search Js
06. Sticky Header Js
07. Data Background Js
08. Testimonial Slider Js
09. Slider Js (Home 3)
10. Brand Js
11. Tesimonial Js
12. Course Slider Js
13. Masonary Js
14. Wow Js
15. Data width Js
16. Cart Quantity Js
17. Show Login Toggle Js
18. Show Coupon Toggle Js
19. Create An Account Toggle Js
20. Shipping Box Toggle Js
21. Counter Js
22. Parallax Js
23. InHover Active Js

****************************************************/

(function ($) {
	"use strict";

	var windowOn = $(window);
	////////////////////////////////////////////////////
	//01. PreLoader Js
	windowOn.on('load',function() {
		$("#loading").fadeOut(500);
	});

	////////////////////////////////////////////////////
	// 02. Mobile Menu Js
	$('#mobile-menu').meanmenu({
		meanMenuContainer: '.mobile-menu',
		meanScreenWidth: "991",
		meanExpand: ['<i class="fal fa-plus"></i>'],
	});

	////////////////////////////////////////////////////
    // 03. Mobile Menu 2 Js
    $('#mobile-menu-2').meanmenu({
        meanMenuContainer: '.mobile-menu-2',
        meanScreenWidth: "30000",
        meanExpand: ['<i class="fal fa-plus"></i>'],
    });

	////////////////////////////////////////////////////
	// 03. Sidebar Js
	$(".offcanvas-toggle-btn").on("click", function () {
		$(".offcanvas__area").addClass("opened");
		$(".body-overlay").addClass("opened");
	});
	$(".offcanvas__close-btn").on("click", function () {
		$(".offcanvas__area").removeClass("opened");
		$(".body-overlay").removeClass("opened");
	});

	////////////////////////////////////////////////////
	// 04. Body overlay Js
	$(".body-overlay").on("click", function () {
		$(".offcanvas__area").removeClass("opened");
		$(".body-overlay").removeClass("opened");
	});

	////////////////////////////////////////////////////
	// 05. Search Js
	$(".search-toggle").on("click", function () {
		$(".search__area").addClass("opened");
	});
	$(".search-close-btn").on("click", function () {
		$(".search__area").removeClass("opened");
	});

	////////////////////////////////////////////////////
	// 06. Sticky Header Js
	windowOn.on('scroll', function () {
		var scroll = $(window).scrollTop();
		if (scroll < 100) {
			$("#header-sticky").removeClass("header-sticky");
		} else {
			$("#header-sticky").addClass("header-sticky");
		}
	});

	////////////////////////////////////////////////////
	// 07. Data CSS Js
	$("[data-background").each(function () {
		$(this).css("background-image", "url( " + $(this).attr("data-background") + "  )");
	});
	$("[data-width]").each(function () {
		$(this).css("width", $(this).attr("data-width"));
	});
	$("[data-bg-color]").each(function () {
        $(this).css("background-color", $(this).attr("data-bg-color"));
    });

	////////////////////////////////////////////////////
	// 07. Nice Select Js
	$('select').niceSelect();

	////////////////////////////////////////////////////
	// 08. slider__active Slider Js
	if (jQuery(".slider__active").length > 0) {
		let sliderActive1 = ".slider__active";
		let sliderInit1 = new Swiper(sliderActive1, {
			// Optional parameters
			slidesPerView: 1,
			slidesPerColumn: 1,
			paginationClickable: true,
			loop: true,
			effect: 'fade',

			autoplay: {
				delay: 5000,
			},

			// If we need pagination
			pagination: {
				el: ".main-slider-paginations",
				// dynamicBullets: true,
				clickable: true,
			},

			// Navigation arrows
			navigation: {
				nextEl: ".swiper-button-next",
				prevEl: ".swiper-button-prev",
			},

			a11y: false,
		});

		function animated_swiper(selector, init) {
			let animated = function animated() {
				$(selector + " [data-animation]").each(function () {
					let anim = $(this).data("animation");
					let delay = $(this).data("delay");
					let duration = $(this).data("duration");

					$(this)
						.removeClass("anim" + anim)
						.addClass(anim + " animated")
						.css({
							webkitAnimationDelay: delay,
							animationDelay: delay,
							webkitAnimationDuration: duration,
							animationDuration: duration,
						})
						.one(
							"webkitAnimationEnd mozAnimationEnd MSAnimationEnd oanimationend animationend",
							function () {
								$(this).removeClass(anim + " animated");
							}
						);
				});
			};
			animated();
			// Make animated when slide change
			init.on("slideChange", function () {
				$(sliderActive1 + " [data-animation]").removeClass("animated");
			});
			init.on("slideChange", animated);
		}

		animated_swiper(sliderActive1, sliderInit1);
	}

	var sliderr = new Swiper('.active-class', {
		slidesPerView: 1,
		spaceBetween: 30,
		loop: true,
		pagination: {
			el: ".testimonial-pagination-6",
			clickable: true,
			renderBullet: function (index, className) {
			  return '<span class="' + className + '">' + '<button>'+(index + 1)+'</button>' + "</span>";
			},
		},
		breakpoints: {
			'1200': {
				slidesPerView: 3,
			},
			'992': {
				slidesPerView: 2,
			},
			'768': {
				slidesPerView: 2,
			},
			'576': {
				slidesPerView: 1,
			},
			'0': {
				slidesPerView: 1,
			},
		},
	});

	///////////////////////////////////////////////////
	// 13. Masonary Js
	$(".package__slider").owlCarousel({
		//add owl carousel in activation class
		loop: true,
		margin: 30,
		items: 4,
		navText: ['<button class="nav-left"><i class="far fa-angle-left"></i></button>', '<button class="nav-right"><i class="far fa-angle-right"></i></button>'],
		nav: false,
		dots: true,
		responsive: {
			0: {
				items: 1
			},
			767: {
				items: 2
			},
			992: {
				items: 3
			},
			1200: {
				items: 4
			}
		}
	});

	////////////////////////////////////////////////////
	// 13. Masonary Js
	$('.grid').imagesLoaded(function () {
		// init Isotope
		var $grid = $('.grid').isotope({
			itemSelector: '.grid-item',
			percentPosition: true,
			masonry: {
				// use outer width of grid-sizer for columnWidth
				columnWidth: '.grid-item',
			}
		});


		// filter items on button click
		$('.masonary-menu').on('click', 'button', function () {
			var filterValue = $(this).attr('data-filter');
			$grid.isotope({ filter: filterValue });
		});

		//for menu active class
		$('.masonary-menu button').on('click', function (event) {
			$(this).siblings('.active').removeClass('active');
			$(this).addClass('active');
			event.preventDefault();
		});

	});

	/* magnificPopup img view */
	$('.popup-image').magnificPopup({
		type: 'image',
		gallery: {
			enabled: true
		}
	});

	/* magnificPopup video view */
	$(".popup-video").magnificPopup({
		type: "iframe",
	});

	////////////////////////////////////////////////////
	// 14. Wow Js
	new WOW().init();

	////////////////////////////////////////////////////
	// 21. Cart Plus Minus Js
	$(".cart-plus-minus").append('<div class="dec qtybutton">-</div><div class="inc qtybutton">+</div>');
	$(".qtybutton").on("click", function () {
		var $button = $(this);
		var oldValue = $button.parent().find("input").val();
		if ($button.text() == "+") {
			var newVal = parseFloat(oldValue) + 1;
		} else {
			// Don't allow decrementing below zero
			if (oldValue > 0) {
				var newVal = parseFloat(oldValue) - 1;
			} else {
				newVal = 0;
			}
		}
		$button.parent().find("input").val(newVal);
	});

	////////////////////////////////////////////////////
	// 17. Show Login Toggle Js
	$('#showlogin').on('click', function () {
		$('#checkout-login').slideToggle(900);
	});

	////////////////////////////////////////////////////
	// 18. Show Coupon Toggle Js
	$('#showcoupon').on('click', function () {
		$('#checkout_coupon').slideToggle(900);
	});

	////////////////////////////////////////////////////
	// 19. Create An Account Toggle Js
	$('#cbox').on('click', function () {
		$('#cbox_info').slideToggle(900);
	});

	////////////////////////////////////////////////////
	// 20. Shipping Box Toggle Js
	$('#ship-box').on('click', function () {
		$('#ship-box-info').slideToggle(1000);
	});

	////////////////////////////////////////////////////
	// 21. Counter Js
	$('.counter').counterUp({
		delay: 10,
		time: 1000
	});

	////////////////////////////////////////////////////
	// 22. Parallax Js
	if ($('.scene').length > 0) {
		$('.scene').parallax({
			scalarX: 10.0,
			scalarY: 15.0,
		});
	};

	////////////////////////////////////////////////////
	// 23. InHover Active Js
	$('.hover__active').on('mouseenter', function () {
		$(this).addClass('active').parent().siblings().find('.hover__active').removeClass('active');
	});

	////////////////////////////////////////////////////
	// 00. Toggle MEnu Js
	$('.cat-toggle-btn').on('click', function () {
		$('.cat__menu').slideToggle(500);
	});
	$('.cat-toggle-btn-2').on('click', function () {
		$('.side-menu').slideToggle(500);
	});

	 ////////////////////////////////////////////////////
    // 63. Data Countdown Js
    if (jQuery(".data-countdown").length > 0) {
		$('[data-countdown]').each(function() {
	
		  var $this = $(this),
			  finalDate = $(this).data('countdown');
	
		  $this.countdown(finalDate, function(event) {
	
			  $this.html(event.strftime('<span class="cdown days"><span class="time-count">%-D</span> <p>Days</p><span class="colon">:</span></span> <span class="cdown hour"><span class="time-count">%-H</span> <p>Hours</p><span class="colon">:</span></span> <span class="cdown minutes"><span class="time-count">%M</span> <p>Mins</p><span class="colon">:</span></span> <span class="cdown second"> <span><span class="time-count">%S</span> <p>Secs</p></span>'));
	
		  });
	
	  });
	  }

	////////////////////////////////////////////////////
	// 11. Product Slider Activation Js
	if (jQuery(".product-slider").length > 0) {
		let testimonialTwo = new Swiper('.product-slider', {
			slidesPerView: 1,
			spaceBetween: 0,
			// direction: 'vertical',
			loop: true,
			observer: true,
			observeParents: true,
			autoplay: {
					delay: 6000,
				},
			
			// If we need pagination
			pagination: {
				el: '.swiper-pagination',
				clickable: true,
			},
			// Navigation arrows
			navigation: {
				nextEl: '.bs-button-next',
				prevEl: '.bs-button-prev',
			},
			
			// And if we need scrollbar
			scrollbar: {
				el: '.swiper-scrollbar',
			},
			breakpoints: {
				550: {
					slidesPerView: 2,
				},
				768: {
					slidesPerView: 3,
				},
				1200: {
					slidesPerView: 4,
				},
				1400: {
					slidesPerView: 5,
					}
				}
			});
	}

	////////////////////////////////////////////////////
	// 11. Product Slider Activation Js
	if (jQuery(".product-slider-2").length > 0) {
		let testimonialTwo = new Swiper('.product-slider-2', {
			slidesPerView: 1,
			spaceBetween: 0,
			// direction: 'vertical',
			loop: true,
			observer: true,
			observeParents: true,
			autoplay: {
					delay: 6000,
				},
			
			// If we need pagination
			pagination: {
				el: '.swiper-pagination',
				clickable: true,
			},
			// Navigation arrows
			navigation: {
				nextEl: '.bs2-button-next',
				prevEl: '.bs2-button-prev',
			},
			
			// And if we need scrollbar
			scrollbar: {
				el: '.swiper-scrollbar',
			},
			breakpoints: {
				550: {
					slidesPerView: 2,
				},
				768: {
					slidesPerView: 3,
				},
				1200: {
					slidesPerView: 4,
				},
				1400: {
					slidesPerView: 5,
					}
				}
			});
	}

	////////////////////////////////////////////////////
	// 11. Product Slider Activation Js
	if (jQuery(".product-slider-3").length > 0) {
		let testimonialTwo = new Swiper('.product-slider-3', {
			slidesPerView: 1,
			spaceBetween: 0,
			// direction: 'vertical',
			loop: true,
			autoplay: {
					delay: 6000,
				},
			
			// If we need pagination
			pagination: {
				el: '.swiper-pagination',
				clickable: true,
			},
			// Navigation arrows
			navigation: {
				nextEl: '.bs2-button-next',
				prevEl: '.bs2-button-prev',
			},
			
			// And if we need scrollbar
			scrollbar: {
				el: '.swiper-scrollbar',
			},
			breakpoints: {
				550: {
					slidesPerView: 2,
				},
				768: {
					slidesPerView: 3,
				},
				1200: {
					slidesPerView: 4,
				},
				1400: {
					slidesPerView: 5,
					}
				}
			});
	}

	////////////////////////////////////////////////////
	// 11. Product Slider Activation Js
	if (jQuery(".brand-slider").length > 0) {
		let testimonialTwo = new Swiper('.brand-slider', {
			slidesPerView: 1,
			spaceBetween: 30,
			// direction: 'vertical',
			loop: true,
			autoplay: {
					delay: 6000,
				},
			
			// If we need pagination
			pagination: {
				el: '.swiper-pagination',
				clickable: true,
			},
			// Navigation arrows
			navigation: {
				nextEl: '.bs-button-next',
				prevEl: '.bs-button-prev',
			},
			
			// And if we need scrollbar
			scrollbar: {
				el: '.swiper-scrollbar',
			},
			breakpoints: {
				550: {
					slidesPerView: 3,
				},
				768: {
					slidesPerView: 4,
				},
				1200: {
					slidesPerView: 5,
				},
				1400: {
					slidesPerView: 6,
					}
				}
			});
	}

	////////////////////////////////////////////////////
	// 14. Range Slider Js
	if (jQuery("#slider-range").length > 0) {
		$("#slider-range").slider({
			range: true,
			min: 20,
			max: 280,
			values: [75, 300],
			slide: function (event, ui) {
				$("#amount").val("$" + ui.values[0] + " To $" + ui.values[1]);
			}
		});
		$("#amount").val("$" + $("#slider-range").slider("values", 0) +
			" To $" + $("#slider-range").slider("values", 1));
	}

	////////////////////////////////////////////////////
	//loading item
	if (jQuery("#loading").length > 0){
		let cart = $('#cart'),
		soda = $('#soda'),
		meat = $('#meat'),
		image = $('#image'),
		mustard = $('#mustard'),
		path = [{x:-250, y:0}, {x:-100, y:-90}, {x:0, y:0}],
		path2 = [{x:250, y:0}, {x:150, y:-80}, {x:60, y:0}],
		path3 = [{x:-170, y:0}, {x:-80, y:-70}, {x:70, y:0}];
	  
	   
		
		var setupSequence = function() {
		  let tl = new TimelineMax({repeat: -1, timeScale: 1.8});
		  
		  tl.set(mustard, {x:-250})
		  .set(meat, {x:250})
		  .set(soda, {x:-170})
		  .to(cart, 2.1, { 
			x:750, 
			ease: SlowMo.ease.config(0.5, 0.5, false),
		  })
		  .to(mustard, 1, {
			bezier: {curviness: 0.3, values:path},
			opacity: 1,
			scale:1,
			ease: Back.easeOut.config(1.4)
		  }, 0.5)
		  .to(mustard, .2, {
			scale: 0,
		  }, 0.8)
		  .to(meat, 1, {
			bezier: {curviness: 0.3, values:path2},
			opacity: 1,
			scale:1,
			ease: Back.easeOut.config(1.4)
		  }, 0.8)
		  .to(meat, .2, {
			scale: 0
		  }, 1.2)
		  .to(soda, .7, {
			bezier: {curviness: 0.3, values:path3},
			opacity: 1,
			scale:1,
			ease: Back.easeOut.config(1.4)
		  }, 1.2)
		  .to(soda, .1, {
			scale: 0,
		  },1.5);
		}
		
		setupSequence();
	}
	
	

})(jQuery);