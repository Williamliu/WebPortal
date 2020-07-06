$.fn.extend({
    WliuToolTip: function (opts) {
        //placement: left right up down;
        var def_settings = {
            //placement:      ""  // left right up down, 
        };
        $.extend(def_settings, opts);

        /*** begin return ***/
        return this.each(function (idx, el) {
            if( !$(el).hasAttr("wliu-tooltip") ) $(el).addAttr("wliu-tooltip");
            if( $(el).has("s[arrow][left]").length<=0 ) $(el).prepend('<s arrow left></s>');
            if( $(el).has("s[arrow][right]").length<=0 ) $(el).prepend('<s arrow right></s>');
            if( $(el).has("s[arrow][up]").length<=0 ) $(el).prepend('<s arrow up></s>');
            if( $(el).has("s[arrow][head-up]").length<=0 ) $(el).prepend('<s arrow head-up></s>');
            if( $(el).has("s[arrow][down]").length<=0 ) $(el).prepend('<s arrow down></s>');
            
            if( $(el).has("div[wliu-tooltip-body]").length<=0 ) $(el).append('<div wliu-tooltip-body></div>');
        });
        /*** end return ***/
    }
});    


$(function(){
    /*** popup ***/
    $("div[wliu-tooltip]").WliuToolTip({});

    $(document).off("click", "*[tooltip-toggle='click']").on("click", "*[tooltip-toggle='click']", function(evt){
        //var target_el = $(this).attr("tooltip-target");
        var target_el = "div[tooltip-target='" + $(this).attr("tooltip-target") + "']";
        var target_content  = $(this).attr("tooltip-body");

        if( $( target_el ).is(":hidden")  ) {
            /********************************************************************************** */
            if(target_content) {
                    var target_pl       = $(this).attr("tooltip-placement");
                    var target_tt       = $(this).attr("tooltip-head");

                    if( target_tt ) {
                        if( $(target_el).has("div[wliu-tooltip-head]").length<=0 )
                            $(target_el).prepend('<div wliu-tooltip-head>' + target_tt + '</div>');
                        else 
                            $( "div[wliu-tooltip-head]", $(target_el) ).html(target_tt);
                    } else {
                        $( "div[wliu-tooltip-head]", $(target_el) ).remove();
                    }

                    $( "div[wliu-tooltip-body]", $(target_el) ).html(target_content);
                    $(target_el).addAttr("active");
                    var nleft   = -900;
                    var ntop    = -900;

                    switch( ("" + target_pl).toLowerCase() ) {
                        case "left":
                            $("s[arrow]", $(target_el)).removeAttr("active");
                            $("s[arrow][left]", $(target_el)).addAttr("active");

                            nleft = $(this).offset().left + $(this).outerWidth() + 12;
                            ntop = $(this).offset().top + $(this).outerHeight()/2 - $(target_el).outerHeight()/2;
                            break;
                        case "right":
                            $("s[arrow]", $(target_el)).removeAttr("active");
                            $("s[arrow][right]", $(target_el)).addAttr("active");
                            nleft = $(this).offset().left - $(target_el).outerWidth() - 12;
                            ntop = $(this).offset().top + $(this).outerHeight()/2 - $(target_el).outerHeight()/2;
                            break;
                        case "up":
                            $("s[arrow]", $(target_el)).removeAttr("active");
                            if( $(target_el).has("div[wliu-tooltip-head]").length<=0 )
                                $("s[arrow][up]", $(target_el)).addAttr("active");
                            else 
                                $("s[arrow][head-up]", $(target_el)).addAttr("active");
        
                            nleft = $(this).offset().left + ( $(this).outerWidth() - $(target_el).outerWidth() ) / 2;
                            ntop = $(this).offset().top + $(this).outerHeight()  + 12;
                            break;
                        case "down":
                            $("s[arrow]", $(target_el)).removeAttr("active");
                            $("s[arrow][down]", $(target_el)).addAttr("active");
                            nleft = $(this).offset().left + ( $(this).outerWidth() - $(target_el).outerWidth() ) / 2;
                            ntop = $(this).offset().top - $(target_el).outerHeight() - 12;
                            break;
                        default:
                            $("s[arrow]", $(target_el)).removeAttr("active");
                            var nplace = "left";
                            nleft = $(this).offset().left + $(this).outerWidth() + 12;
                            ntop = $(this).offset().top + $(this).outerHeight()/2 - $(target_el).outerHeight()/2;
                            var cleft = $(this).offset().left + $(this).outerWidth() + $(target_el).outerWidth() + 12;

                            if( ntop > 0 ) {
                                    // place to left first, then right
                                    if( cleft <= $(window).scrollLeft() + window.innerWidth ) {
                                            nplace = "left";
                                            $("s[arrow][left]", $(target_el)).addAttr("active");
                                            nleft = $(this).offset().left + $(this).outerWidth() + 12;
                                            ntop = $(this).offset().top + $(this).outerHeight()/2 - $(target_el).outerHeight()/2;
                                    } else {
                                            cleft = $(this).offset().left - $(target_el).outerWidth() - 12;
                                            if( cleft >= 0 ) {
                                                    nplace = "right";
                                                    $("s[arrow][right]", $(target_el)).addAttr("active");
                                                    nleft = $(this).offset().left - $(target_el).outerWidth() - 12;
                                                    ntop = $(this).offset().top + $(this).outerHeight()/2 - $(target_el).outerHeight()/2;
                                            } else { 
                                                    nplace = "up";
                                                    if( $(target_el).has("div[wliu-tooltip-head]").length<=0 )
                                                        $("s[arrow][up]", $(target_el)).addAttr("active");
                                                    else 
                                                        $("s[arrow][head-up]", $(target_el)).addAttr("active");
                                
                                                    nleft = $(this).offset().left + ( $(this).outerWidth() - $(target_el).outerWidth() ) / 2;
                                                    ntop = $(this).offset().top + $(this).outerHeight()  + 12;
                                            }
                                    }                      

                            } else {
                                    nplace = "up";
                                    if( $(target_el).has("div[wliu-tooltip-head]").length<=0 )
                                        $("s[arrow][up]", $(target_el)).addAttr("active");
                                    else 
                                        $("s[arrow][head-up]", $(target_el)).addAttr("active");
                
                                    nleft = $(this).offset().left + ( $(this).outerWidth() - $(target_el).outerWidth() ) / 2;
                                    ntop = $(this).offset().top + $(this).outerHeight()  + 12;
                            }
                            break;
                    }

                    $(target_el).offset({left:nleft, top:ntop});
            } else {
                $( "div[wliu-tooltip-head]", $(target_el) ).remove();
                $( "div[wliu-tooltip-body]", $(target_el) ).empty();
            }
            /********************************************************************************** */
        } else {
            $(target_el).removeAttr("active");
            $( "div[wliu-tooltip-head]", $(target_el) ).remove();
            $( "div[wliu-tooltip-body]", $(target_el) ).empty();
        }
    });


    $(document).off("focus", "*[tooltip-toggle='focus']").on("focus", "*[tooltip-toggle='focus']", function(evt){
        //var target_el = $(this).attr("tooltip-target");
        var target_el = "div[tooltip-target='" + $(this).attr("tooltip-target") + "']";
        var target_content  = $(this).attr("tooltip-body");

        if( $( target_el ).is(":hidden")  ) {
            /********************************************************************************** */
            if(target_content) {
                    var target_pl       = $(this).attr("tooltip-placement");
                    var target_tt       = $(this).attr("tooltip-head");

                    if( target_tt ) {
                        if( $(target_el).has("div[wliu-tooltip-head]").length<=0 )
                            $(target_el).prepend('<div wliu-tooltip-head>' + target_tt + '</div>');
                        else 
                            $( "div[wliu-tooltip-head]", $(target_el) ).html(target_tt);
                    } else {
                        $( "div[wliu-tooltip-head]", $(target_el) ).remove();
                    }

                    $( "div[wliu-tooltip-body]", $(target_el) ).html(target_content);
                    $(target_el).addAttr("active");
                    var nleft   = -900;
                    var ntop    = -900;

                    switch( ("" + target_pl).toLowerCase() ) {
                        case "left":
                            $("s[arrow]", $(target_el)).removeAttr("active");
                            $("s[arrow][left]", $(target_el)).addAttr("active");

                            nleft = $(this).offset().left + $(this).outerWidth() + 12;
                            ntop = $(this).offset().top + $(this).outerHeight()/2 - $(target_el).outerHeight()/2;
                            break;
                        case "right":
                            $("s[arrow]", $(target_el)).removeAttr("active");
                            $("s[arrow][right]", $(target_el)).addAttr("active");
                            nleft = $(this).offset().left - $(target_el).outerWidth() - 12;
                            ntop = $(this).offset().top + $(this).outerHeight()/2 - $(target_el).outerHeight()/2;
                            break;
                        case "up":
                            $("s[arrow]", $(target_el)).removeAttr("active");
                            if( $(target_el).has("div[wliu-tooltip-head]").length<=0 )
                                $("s[arrow][up]", $(target_el)).addAttr("active");
                            else 
                                $("s[arrow][head-up]", $(target_el)).addAttr("active");
        
                            nleft = $(this).offset().left + ( $(this).outerWidth() - $(target_el).outerWidth() ) / 2;
                            ntop = $(this).offset().top + $(this).outerHeight()  + 12;
                            break;
                        case "down":
                            $("s[arrow]", $(target_el)).removeAttr("active");
                            $("s[arrow][down]", $(target_el)).addAttr("active");
                            nleft = $(this).offset().left + ( $(this).outerWidth() - $(target_el).outerWidth() ) / 2;
                            ntop = $(this).offset().top - $(target_el).outerHeight() - 12;
                            break;
                        default:
                            $("s[arrow]", $(target_el)).removeAttr("active");
                            var nplace = "left";
                            nleft = $(this).offset().left + $(this).outerWidth() + 12;
                            ntop = $(this).offset().top + $(this).outerHeight()/2 - $(target_el).outerHeight()/2;
                            var cleft = $(this).offset().left + $(this).outerWidth() + $(target_el).outerWidth() + 12;

                            if( ntop > 0 ) {
                                    // place to left first, then right
                                    if( cleft <= $(window).scrollLeft() + window.innerWidth ) {
                                            nplace = "left";
                                            $("s[arrow][left]", $(target_el)).addAttr("active");
                                            nleft = $(this).offset().left + $(this).outerWidth() + 12;
                                            ntop = $(this).offset().top + $(this).outerHeight()/2 - $(target_el).outerHeight()/2;
                                    } else {
                                            cleft = $(this).offset().left - $(target_el).outerWidth() - 12;
                                            if( cleft >= 0 ) {
                                                    nplace = "right";
                                                    $("s[arrow][right]", $(target_el)).addAttr("active");
                                                    nleft = $(this).offset().left - $(target_el).outerWidth() - 12;
                                                    ntop = $(this).offset().top + $(this).outerHeight()/2 - $(target_el).outerHeight()/2;
                                            } else { 
                                                    nplace = "up";
                                                    if( $(target_el).has("div[wliu-tooltip-head]").length<=0 )
                                                        $("s[arrow][up]", $(target_el)).addAttr("active");
                                                    else 
                                                        $("s[arrow][head-up]", $(target_el)).addAttr("active");
                                
                                                    nleft = $(this).offset().left + ( $(this).outerWidth() - $(target_el).outerWidth() ) / 2;
                                                    ntop = $(this).offset().top + $(this).outerHeight()  + 12;
                                            }
                                    }                      

                            } else {
                                    nplace = "up";
                                    if( $(target_el).has("div[wliu-tooltip-head]").length<=0 )
                                        $("s[arrow][up]", $(target_el)).addAttr("active");
                                    else 
                                        $("s[arrow][head-up]", $(target_el)).addAttr("active");
                
                                    nleft = $(this).offset().left + ( $(this).outerWidth() - $(target_el).outerWidth() ) / 2;
                                    ntop = $(this).offset().top + $(this).outerHeight()  + 12;
                            }
                            break;
                    }

                    $(target_el).offset({left:nleft, top:ntop});
            } else {
                $( "div[wliu-tooltip-head]", $(target_el) ).remove();
                $( "div[wliu-tooltip-body]", $(target_el) ).empty();
            }
            /********************************************************************************** */
        } else {
            $(target_el).removeAttr("active");
            $( "div[wliu-tooltip-head]", $(target_el) ).remove();
            $( "div[wliu-tooltip-body]", $(target_el) ).empty();
        }
    });


    $(document).off("mouseover", "*[tooltip-toggle='hover']").on("mouseover", "*[tooltip-toggle='hover']", function(evt){
        //var target_el       = $(this).attr("tooltip-target");
        var target_el = "div[tooltip-target='" + $(this).attr("tooltip-target") + "']";
        var target_content  = $(this).attr("tooltip-body");

        /*** content not empty ***/
        if(target_content) {
                var target_pl       = $(this).attr("tooltip-placement");
                var target_tt       = $(this).attr("tooltip-head");
                
                if( target_tt ) {
                    if( $(target_el).has("div[wliu-tooltip-head]").length<=0 )
                        $(target_el).prepend('<div wliu-tooltip-head>' + target_tt + '</div>');
                    else 
                        $( "div[wliu-tooltip-head]", $(target_el) ).html(target_tt);
                } else {
                    $( "div[wliu-tooltip-head]", $(target_el) ).remove();
                }
              
                $( "div[wliu-tooltip-body]", $(target_el) ).html(target_content);
                $(target_el).addAttr("active");
                var nleft   = -900;
                var ntop    = -900;

                switch( ("" + target_pl).toLowerCase() ) {
                    case "left":
                        $("s[arrow]", $(target_el)).removeAttr("active");
                        $("s[arrow][left]", $(target_el)).addAttr("active");

                        nleft = $(this).offset().left + $(this).outerWidth() + 12;
                        ntop = $(this).offset().top + $(this).outerHeight()/2 - $(target_el).outerHeight()/2;
                        break;
                    case "right":
                        $("s[arrow]", $(target_el)).removeAttr("active");
                        $("s[arrow][right]", $(target_el)).addAttr("active");
                        nleft = $(this).offset().left - $(target_el).outerWidth() - 12;
                        ntop = $(this).offset().top + $(this).outerHeight()/2 - $(target_el).outerHeight()/2;
                        break;
                    case "up":
                        $("s[arrow]", $(target_el)).removeAttr("active");
                        if( $(target_el).has("div[wliu-tooltip-head]").length<=0 )
                            $("s[arrow][up]", $(target_el)).addAttr("active");
                        else 
                            $("s[arrow][head-up]", $(target_el)).addAttr("active");
    
                        nleft = $(this).offset().left + ( $(this).outerWidth() - $(target_el).outerWidth() ) / 2;
                        ntop = $(this).offset().top + $(this).outerHeight()  + 12;
                        break;
                    case "down":
                        $("s[arrow]", $(target_el)).removeAttr("active");
                        $("s[arrow][down]", $(target_el)).addAttr("active");
                        nleft = $(this).offset().left + ( $(this).outerWidth() - $(target_el).outerWidth() ) / 2;
                        ntop = $(this).offset().top - $(target_el).outerHeight() - 12;
                        break;
                    default:
                        $("s[arrow]", $(target_el)).removeAttr("active");
                        var nplace = "left";
                        nleft = $(this).offset().left + $(this).outerWidth() + 12;
                        ntop = $(this).offset().top + $(this).outerHeight()/2 - $(target_el).outerHeight()/2;
                        var cleft = $(this).offset().left + $(this).outerWidth() + $(target_el).outerWidth() + 12;

                        if( ntop > 0 ) {
                                // place to left first, then right
                                if( cleft <=  $(window).scrollLeft() + window.innerWidth ) {
                                        nplace = "left";
                                        $("s[arrow][left]", $(target_el)).addAttr("active");
                                        nleft = $(this).offset().left + $(this).outerWidth() + 12;
                                        ntop = $(this).offset().top + $(this).outerHeight()/2 - $(target_el).outerHeight()/2;
                                } else {
                                        cleft = $(this).offset().left - $(target_el).outerWidth() - 12;
                                        if( cleft >= 0 ) {
                                                nplace = "right";
                                                $("s[arrow][right]", $(target_el)).addAttr("active");
                                                nleft = $(this).offset().left - $(target_el).outerWidth() - 12;
                                                ntop = $(this).offset().top + $(this).outerHeight()/2 - $(target_el).outerHeight()/2;
                                        } else { 
                                                nplace = "up";
                                                if( $(target_el).has("div[wliu-tooltip-head]").length<=0 )
                                                    $("s[arrow][up]", $(target_el)).addAttr("active");
                                                else 
                                                    $("s[arrow][head-up]", $(target_el)).addAttr("active");
                            
                                                nleft = $(this).offset().left + ( $(this).outerWidth() - $(target_el).outerWidth() ) / 2;
                                                ntop = $(this).offset().top + $(this).outerHeight()  + 12;
                                        }
                                }                      

                        } else {
                                nplace = "up";
                                if( $(target_el).has("div[wliu-tooltip-head]").length<=0 )
                                    $("s[arrow][up]", $(target_el)).addAttr("active");
                                else 
                                    $("s[arrow][head-up]", $(target_el)).addAttr("active");
            
                                nleft = $(this).offset().left + ( $(this).outerWidth() - $(target_el).outerWidth() ) / 2;
                                ntop = $(this).offset().top + $(this).outerHeight()  + 12;
                        }
                        break;
                }

                $(target_el).offset({left:nleft, top:ntop});
        } else {
            $( "div[wliu-tooltip-head]", $(target_el) ).remove();
            $( "div[wliu-tooltip-body]", $(target_el) ).empty();
        }
        /*** --content not empty ***/
    });
    $(document).off("mouseout", "*[tooltip-toggle='hover']").on("mouseout", "*[tooltip-toggle='hover']", function(evt){
        //var target_el  = $(this).attr("tooltip-target");
        var target_el = "div[tooltip-target='" + $(this).attr("tooltip-target") + "']";
        $(target_el).removeAttr("active");
        $( "div[wliu-tooltip-head]", $(target_el) ).remove();
        $( "div[wliu-tooltip-body]", $(target_el) ).empty();
    });
    $(document).off("mouseleft", "*[tooltip-toggle='hover']").on("mouseleft", "*[tooltip-toggle='hover']", function(evt){
        //var target_el  = $(this).attr("tooltip-target");
        var target_el = "div[tooltip-target='" + $(this).attr("tooltip-target") + "']";
        $(target_el).removeAttr("active");
        $( "div[wliu-tooltip-head]", $(target_el) ).remove();
        $( "div[wliu-tooltip-body]", $(target_el) ).empty();
    });
    
});