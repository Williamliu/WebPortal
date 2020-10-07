$.widget("wliu.loading", {
    options: {
        text: "Please Waiting..."
    },
    _create: function () {
        this.element.html(this.options.text);
        this.element.appendTo("body");
        if($("div[wliu-loading-mask]").length>0) {
            $("div[wliu-loading-mask]").appendTo("body");
        } else {
            $("body").append("<div wliu-loading-mask></div>");
        }
        if($("iframe[wliu-loading-mask]").length>0) {
            $("iframe[wliu-loading-mask]").appendTo("body");
        } else {
            $("body").append("<iframe wliu-loading-mask></iframe>");
        }
    },
    _init: function () {  },
    _setOption: function (key, val) {
        this._super(key, val);
        switch (key) 
        {
            case "text":
                this._changeText();
                break;
        }
    },
    _changeText: function () {
        this.element.html(this.options.text);
    },
    show: function () {
        this.element.show();
        $("div[wliu-loading-mask]").show();
        $("iframe[wliu-loading-mask]").show();
    },
    hide: function () {
        this.element.hide();
        $("div[wliu-loading-mask]").hide();
        $("iframe[wliu-loading-mask]").hide();
    },
    destroy: function () {
        $("div[wliu-loading-mask]").remove();
        $("iframe[wliu-loading-mask]").remove();
        $.Widget.prototype.destroy.call(this);
    }
});

$.widget("wliu.hint", {
    options: {
        text:   "",
        color:  "",     //danger, warning, success, info, primary, secondary
        second: 2500,
        pos:    ""  // default "cb",  list: "lt", "ct",  "rt", "lb", "cb", "rb"
    },
    _create: function () {
        this.element.appendTo("body");
        this.options.text   = this.element.html() ? this.element.html() : this.options.text;
        this.options.color   = this.element.hasAttr("wl-color") ? this.element.attr("wl-color") : this.options.color;
        this.options.second = this.element.hasAttr("wl-second") ? this.element.attr("wl-second") : this.options.second;
        this.options.pos    = this.element.hasAttr("wl-pos") ? this.element.attr("wl-pos") : this.options.pos;

        this._changeText();
        this._changecolor();
        this._changePos();
    },
    _init: function () {
    },
    _setOption: function (key, val) {
        this._super(key, val);
        switch (key) {
            case "text":
                this._changeText();
                break;
            case "color":
                this._changecolor();
                break;
            case "pos":
                this._changePos();
                break;
        }
    },
    _changeText: function () {
        this.element.html(this.options.text);
    },
    _changecolor: function () {
        this.element.attr("wl-color", this.options.color);
    },
    _changePos: function () {
        this.element.attr("wl-pos", this.options.pos);
    },
    show: function (message) {
        if (message !== undefined) {
            this.options.text = message;
            this._changeText();
        }
        if((""+this.options.text).trim()!=="")
            this.element.stop(true, true).fadeIn(10).delay(this.options.second).fadeOut(1200);

    },
    destroy: function () {
        $.Widget.prototype.destroy.call(this);
    }
});

$.widget("wliu.tooltip", {
    options: {
        color:      "",         //danger, warning, success, info, primary, secondary
        trigger:    "hover",    // hover or click
        toggle:     ""          //              
    },
    _create: function () {
        var self = this;
        self.element.appendTo("body");
        self.options.color = self.element.hasAttr("wl-color") ? self.element.attr("wl-color") : self.options.color;
        self._changeColor();
        
        this.options.toggle = self.element.attr("wl-toggle")?self.element.attr("wl-toggle"):this.options.toggle; 
        this.options.trigger = self.element.attr("wl-trigger")?self.element.attr("wl-trigger"):this.options.trigger; 

        if(self.options.toggle) {

            self.element.off("click.tooltip").on("click.tooltip", function(evt){
                self.element.empty().hide();
                self.element.offset({ left: -900, top: -900 });
            });

            if(self.options.trigger=="click") {
                $("body").off("click.tooltip", `[tooltip-target="${self.options.toggle}"]`).on("click.tooltip", `[tooltip-target="${self.options.toggle}"]`, function(evt){
                    if( self.element.is(":visible") ) {

                        let nleft = -900;
                        let ntop = -900;
                        self.element.empty().html($(this).attr("tooltip-text")).show();
                        nleft = $(this).offset().left + 6;
                        ntop  = $(this).offset().top - self.element.outerHeight()-12;

                        if( Math.abs(self.element.offset().left - nleft) <= 20 && Math.abs(self.element.offset().top - ntop) <= 20 ) {
                            self.element.empty().hide();
                            self.element.offset({ left: -900, top: -900 });
                        } else {
                            if( $(this).attr("tooltip-text") ) {

                                if($(this).attr("tooltip-color")) 
                                    self.element.attr("wl-color", $(this).attr("tooltip-color"));
                                else 
                                    self.element.attr("wl-color", self.options.color);
        
                                self.element.offset({ left: nleft, top: ntop });
                                self.element.hide().fadeIn(600);
                            } else {
                                self.element.empty().hide();
                                self.element.offset({ left: -900, top: -900 });
                            } 
                        }

                    } else {
                        if( $(this).attr("tooltip-text") ) {

                            if($(this).attr("tooltip-color")) 
                                self.element.attr("wl-color", $(this).attr("tooltip-color"));
                            else 
                                self.element.attr("wl-color", self.options.color);
    
                            let nleft = -900;
                            let ntop = -900;
                            self.element.empty().html($(this).attr("tooltip-text")).show();
                            nleft = $(this).offset().left + 6;
                            ntop  = $(this).offset().top - self.element.outerHeight()-12;
                            
                            self.element.offset({ left: nleft, top: ntop });
                            self.element.hide().fadeIn(600);
                        } 
                    }
                });
            } else {
                $("body").off("mouseover.tooltip", `[tooltip-target="${self.options.toggle}"]`).on("mouseover.tooltip", `[tooltip-target="${self.options.toggle}"]`, function(evt){
                    if( $(this).attr("tooltip-text") ) {

                        if($(this).attr("tooltip-color")) 
                            self.element.attr("wl-color", $(this).attr("tooltip-color"));
                        else 
                            self.element.attr("wl-color", self.options.color);

                        let nleft = -900;
                        let ntop = -900;
                        self.element.empty().html($(this).attr("tooltip-text")).show();
                        nleft = $(this).offset().left + 6;
                        ntop  = $(this).offset().top - self.element.outerHeight()-12;
                        self.element.offset({ left: nleft, top: ntop });
                        self.element.hide().fadeIn(600);
                    } 
                });
                $("body").off("mouseout.tooltip", `[tooltip-target="${self.options.toggle}"]`).on("mouseout.tooltip", `[tooltip-target="${self.options.toggle}"]`, function(evt){
                    self.element.empty().hide();
                    self.element.offset({ left: -900, top: -900 });
                });
            }

        }
    },
    _init: function () {
    },
    _setOption: function (key, val) {
        this._super(key, val);
        switch (key) {
            case "color":
                this._changeColor();
                break;
        }
    },
    _changeColor: function () {
        this.element.attr("wl-color", this.options.color);
    },
    destroy: function () {
        $("body").off("mouseover.tooltip", `[tooltip-target="${this.options.toggle}"]`);
        $("body").off("mouseout.tooltip", `[tooltip-target="${this.options.toggle}"]`);
        $("body").off("click.tooltip", `[tooltip-target="${this.options.toggle}"]`);
        this.element.off("click.tooltip");
        $.Widget.prototype.destroy.call(this);
    }
});

$.widget("wliu.block", {
    options: {
        color: "",   //danger, warning, success, info, primary, secondary
        shadow: false,
        border: false
    },
    _create: function () {
        var self = this;
        self.options.color = self.element.hasAttr("wl-color") ? self.element.attr("wl-color") : self.options.color;
        self._changeColor();
        
        self.options.shadow = self.element.hasAttr("wl-shadow");
        self._changeShadow();

        self.options.border = self.element.hasAttr("wl-border");
        self._changeBorder();

        if (self.element.hasAttr("wl-toggle")) this.options.toggle = self.element.attr("wl-toggle")?self.element.attr("wl-toggle"):""; 

        if(self.options.toggle!=undefined) {
            $(self.element).attr("wl-toggle", this.options.toggle);
            if(self.options.toggle=="close") $(">div[wliu-block-body]", self.element).hide();

            $(">div[wliu-block-head]", self.element).off("click.block").on("click.block", function (evt) {
                if(self.options.toggle=="close") 
                    self.options.toggle="open";
                else 
                    self.options.toggle="close";
                
                self._changeToggle();
            });
        }
    },
    _init: function () {
    },
    _setOption: function (key, val) {
        this._super(key, val);
        switch (key) {
            case "toggle":
                this._changeToggle();
                break;
            case "color":
                this._changeColor();
                break;
            case "shadow":
                this._changeShadow();
                break;
            case "border":
                this._changeBorder();
                break;
        }
    },
    _changeToggle: function () {
        if(this.options.toggle) {
            $(this.element).attr("wl-toggle", this.options.toggle);

            if (this.options.toggle=="close")
                $(">div[wliu-block-body]", this.element).stop().slideUp(1200);
            else
                $(">div[wliu-block-body]", this.element).stop().slideDown(1200);
        }
    },
    _changeColor: function () {
        this.element.attr("wl-color", this.options.color);
    },
    _changeShadow: function () {
        if(this.options.shadow)
            this.element.addAttr("wl-shadow");
        else 
            this.element.removeAttr("wl-shadow");  
    },
    _changeBorder: function () {
        if(this.options.border)
            this.element.addAttr("wl-border");
        else 
            this.element.removeAttr("wl-border");  
    },
    destroy: function () {
        $(">div[wliu-block-head]", this.element).off("click.block")
        $.Widget.prototype.destroy.call(this);
    }
});

$.widget("wliu.tab", {
    options: {
        tabsn:  0,
        height: 0,
        border: false,
        color:  ""
    },
    _create: function () {
        var self = this;
        $("li", this.element).each(function (idx, el) {
            $(el).attr("tabsn", idx);
        });
        $("+div[wliu-tab]>div", this.element).not("[wliu-tab-foot]").each(function (idx, el) {
            $(el).attr("tabsn", idx);
        });


        this.options.tabsn = $("li[wl-active]", this.element).hasAttr("tabsn")?$("li[wl-active]",this.element).attr("tabsn"):this.options.tabsn;
        this.options.border = this.element.hasAttr("wl-border") ? true : this.options.border;
        this.options.color = this.element.hasAttr("wl-color") ? this.element.attr("color") : this.options.color;
        this.options.height = this.element.hasAttr("wl-height") ? this.element.attr("wl-height") : this.options.height;

        $("li", this.element).off("click.tab").on("click.tab", function (evt) {
            self._setOption("tabsn", $(this).attr("tabsn"));
        });

        this._changeHeight();
        this._changeColor();
        this._changeBorder();
        this._changeTab();
    },
    _init: function () {
    },
    _setOption: function (key, val) {
        this._super(key, val);
        switch (key) {
            case "height":
                this._changeHeight();
                break;
            case "tabsn":
                this._changeTab();
                break;
            case "color":
                this._changeColor();
                break;
            case "border":
                this._changeBorder();
                break;
        }
    },
    _changeTab: function () {
        $("li", this.element).attr("wl-active", null);
        $("+div[wliu-tab]>div", this.element).not("[wliu-tab-foot]").attr("wl-active", null);

        if ($(`li[tabsn="${this.options.tabsn}"]`, this.element).length <= 0) {
            this.options.tabsn = 0;
        }

        $(`li[tabsn="${this.options.tabsn}"]`, this.element).addAttr("wl-active");
        $(`+div[wliu-tab]>div[tabsn="${this.options.tabsn}"]`, this.element).addAttr("wl-active");
    },
    _changeHeight: function () {
        if (this.options.height > 0) 
            $("+div[wliu-tab]>div", this.element).not("[wliu-tab-foot]").height(this.options.height);
        else
            $("+div[wliu-tab]>div", this.element).height("auto");
    },
    _changeColor: function () {
        if (("" + this.options.color).trim() === "")
            this.element.attr("wl-color", null);
        else
            this.element.attr("wl-color", this.options.color);
    },
    _changeBorder: function () {
        if (this.options.border)
            this.element.addAttr("wl-border");
        else
            this.element.attr("wl-border", null);
    },
    destroy: function () {
        $("li", this.element).off("click.tab");
        $.Widget.prototype.destroy.call(this);
    }
});

$.widget("wliu.tree", {
    options: {
        name:       "mytree",
        single:     false,
        toggle:     "open",
        icon:       "folder",
        select:     false
    },
    _create: function () {
        this.options.name = this.element.attr("name")?this.element.attr("name"):this.options.name;
        this.options.single = this.element.hasAttr("wl-single")?true:this.options.single;
        this.options.icon = this.element.attr("wl-icon")?this.element.attr("wl-icon"):this.options.icon;
        this.options.select = this.element.hasAttr("wl-select")?true:this.options.select;

        var self = this;
        $("li[wl-nodes]", this.element).each(function (idx, el) {
            let el_sn = idx++;
            if ($("s[hide]", el).length<=0) $(el).prepend("<s hide></s>");
            $(el).attr("eidx", el_sn);
            let toggle = localStorage.getItem( self.options.name  + "_" + $(el).attr("eidx"));
            toggle = toggle !== "open" && toggle !== "close" ? self.options.toggle : toggle;
            $(el).attr("wl-toggle", toggle);
        });
        self._changeIcon();
        self._changeSelect();
        self._changeSingle();
    },
    _init: function () { },
    _setOption: function (key, val) {
        this._super(key, val);
        switch (key) {
            case "single":
                this._changeSingle();
                break;
            case "toggle":
                this._changeToggle();
                break;
            case "icon":
                this._changeIcon();
                break;
            case "select":
                this._changeSelect();
                break;
        }
    },
    _changeSingle: function () {
        let self = this;
        this.element.off("click.tree", "li>s").on("click.tree", "li>s", function(evt){
            if($(this).prop("tagName").toLowerCase() === "li") {
                if($(this).hasAttr("wl-nodes")) {
                    if($(this).attr("wl-toggle")==="open") {
                        $(this).attr("wl-toggle", "close");
                        localStorage.setItem(self.options.name + "_" + $(this).attr("eidx"), "close");
                    } else if($(this).attr("wl-toggle")==="close") {
                        if(self.options.single) {
                            $(this).siblings("li[wl-nodes]").attr("wl-toggle", "close");
                            $(this).siblings("li[wl-nodes]").each(function(idx2, el2){
                                localStorage.setItem(self.options.name + "_" + $(el2).attr("eidx"), "close");
                            });
                        }
                        $(this).attr("wl-toggle", "open");
                        localStorage.setItem(self.options.name + "_" + $(this).attr("eidx"), "open");
                    }
                }
            }
            
            if($(this).prop("tagName").toLowerCase()==="s")
            {
                if($(this).parent("li").hasAttr("wl-nodes")) {
                    if($(this).parent("li").attr("wl-toggle")=="open") {
                        $(this).parent("li").attr("wl-toggle", "close");
                        localStorage.setItem(self.options.name + "_" + $(this).parent("li").attr("eidx"), "close");
                    } else if($(this).parent("li").attr("wl-toggle")=="close") {
                        if (self.options.single) {
                            $(this).parent("li").siblings("li[wl-nodes]").attr("wl-toggle","close");
                            $(this).parent("li").siblings("li[wl-nodes]").each(function(idx3, el3){
                                localStorage.setItem(self.options.name + "_" + $(el3).attr("eidx"), "close");
                            });
                        } 
                        $(this).parent("li").attr("wl-toggle", "open");
                        localStorage.setItem(self.options.name + "_" + $(this).parent("li").attr("eidx"), "open");
                    }
                } 
            }
        });
    },
    _changeToggle: function () {
        let self = this;
        $("li[wl-nodes]", self.element).each(function (idx, el) {
            $(el).attr("eidx", idx++);
            $(el).attr("wl-toggle", self.options.toggle);
        });
    },
    _changeIcon: function () {
        let self = this;
        if (self.options.icon !== "") {
            $("li[wl-nodes], li[wl-node]", self.element).each(function (idx, el) {
                if ($(">s", el).not("s[hide]").length <= 0) {
                    $(el).prepend(`<s ${self.options.icon}='${$(el).attr(self.options.icon)}'></s>`);
                }
                else {
                    $(">s", el).not("s[hide]").remove();
                    $(el).prepend(`<s ${self.options.icon}='${$(el).attr(self.options.icon)}'></s>`);
                }
            });
        } else {
            $("li[wl-nodes], li[wl-node]", self.element).each(function (idx, el) {
                if ($(">s", el).not("s[hide]").length > 0) {
                    $(`>s`, el).not("s[hide]").remove();
                }
            });
        }
    },
    _changeSelect: function () {
        let self = this;
        if (self.options.select) {
            self.element.off("click.active", "li[wl-node]").on("click.active", "li[wl-node]", function(evt){
                $("li[wl-node]", self.element).removeAttr("wl-active");
                $(this).addAttr("wl-active");
                evt.preventDefault();        
                evt.stopPropagation();
                return false;
            });
        } else {
            $("li[wl-node]", self.element).removeAttr("wl-active");
            self.element.off("click.active", "li[wl-node]");
        }
    },
    destroy: function () {
        this.element.off("click.tree", "li>s");
        this.element.off("click.active", "li[wl-node]");
        $.Widget.prototype.destroy.call(this);
    }
});

$.widget("wliu.flip", {
    options: {
        height: 0,
        toggle: ""
    },
    _create: function () {
        this.options.toggle = this.element.hasAttr("wl-toggle") ? this.element.attr("wl-toggle") : this.options.toggle;
        this.options.height = this.element.hasAttr("wl-height") ? this.element.attr("wl-height") : this.options.height;
        this._changeHeight();
        this._changeToggle();
    },
    _init: function () {},
    _setOption: function (key, val) {
        //important :  remove the old toggle
        if (key === "toggle" && this.options.toggle !== "")
            $(`[flip-target="${this.options.toggle}"]`).off("click.flip");
        this._super(key, val);
        switch (key) {
            case "height":
                this._changeHeight();
                break;
            case "toggle":
                this._changeToggle();
                break;
        }
    },
    _changeHeight: function () {
        if (this.options.height>0)
            this.element.height(this.options.height);
    },
    _changeToggle: function () {
        var self = this;
        $("body").off("click.flip", `[flip-target='${self.options.toggle}']`).on("click.flip", `[flip-target='${self.options.toggle}']`, function(evt){
            if (self.element.hasAttr("wl-reverse"))
                self.element.removeAttr("wl-reverse");
            else 
                self.element.addAttr("wl-reverse");
            return false;
        });
    },
    toggle: function () {
        if (this.element.hasAttr("wl-reverse"))
            this.element.removeAttr("wl-reverse");
        else
            this.element.addAttr("wl-reverse");
    },
    destroy: function () {
        $(`[flip-toggle="${this.options.toggle}"]`).off("click.flip");
        $.Widget.prototype.destroy.call(this);
    }
});

$.widget("wliu.popup", {    // wl-title, wl-text, wl-color, wl-pos, wl-height, wl-maskable, wl-movable, wl-resizable, wl-toggle, wl-icon="confirm|确定|取消"
    options: {
        title:          "",
        text:           "",
        color:          "",
        pos:            "", //Important: pos must be ahaad of movable:  {pos:"",  movable:true}, otherwise: movable not working  //default: center, lt, ct, rt, lb, cb, rb
        height:         0,
        maskable:       false,
        movable:        false,
        resizable:      false,
        toggle:         "",   // button  diag-toggle  diag-data
        data:           null,
        icon:           "",
        zIndex:         9002

    },
    _create: function () {
        this.element.appendTo("body");
        this.options.title = this.element.hasAttr("wl-title") ? this.element.attr("wl-title") : this.options.title;
        this.options.text = this.element.hasAttr("wl-text") ? this.element.attr("wl-text") : this.options.text;
        this.options.color = this.element.hasAttr("wl-color") ? this.element.attr("wl-color") : this.options.color;
        this.options.pos = this.element.hasAttr("wl-pos") ? this.element.attr("wl-pos") : this.options.pos;
        this.options.height = this.element.hasAttr("wl-height") ? this.element.attr("wl-height") : this.options.height;

        this.options.maskable = this.element.hasAttr("wl-maskable") ? true : this.options.maskable;
        this.options.movable = this.element.hasAttr("wl-movable") ? true : this.options.movable;
        this.options.resizable = this.element.hasAttr("wl-resizable") ? true : this.options.resizable;

        this.options.toggle = this.element.hasAttr("wl-toggle") ? this.element.attr("wl-toggle") : this.options.toggle;
        this.options.icon = this.element.hasAttr("wl-icon") ? this.element.attr("wl-icon") : this.options.icon;

        
        if( $(">div[wliu-popup-head]", this.element).length<=0 )
            this.element.prepend(`<div wliu-popup-head>${this.options.title}<a wliu-popup-close-icon wliu-popup-close></a></div>`);
        else 
            if($(">div[wliu-popup-head]>a[wliu-popup-close-icon]", this.element).length<=0)
                $(">div[wliu-popup-head]", this.element).append('<a wliu-popup-close-icon wliu-popup-close></a>');

        if( $(">div[wliu-popup-body]", this.element).length<=0 )
            this.element.prepend(`<div wliu-popup-body>${this.options.text}</div>`);
        else if(this.options.text!="")
            $(">div[wliu-popup-body]", this.element).html(this.options.text);
            
        if( $(">div[wliu-popup-foot]", this.element).length<=0 )
            this.element.prepend(`<div wliu-popup-foot><button wliu-button wliu-button-popup wliu-popup-close>CLOSE</button></div>`);
        
        var self = this;
        this.element.off("click.popupClose", "[wliu-popup-close]").on("click.popupClose", "[wliu-popup-close]", function(evt){
            self.hide();
        });
        this.element.off("click.popupActive").on("click.popupActive", function(evt){
            if(self.element.is(":visible")) {
                self.element.css("z-index", self.options.zIndex + 100);
            }
            $("div[wliu-popup]:visible").not(self.element).each(function (idx, el) {
                if ($(el).css("z-index") <= self.options.zIndex) {
                    $(el).css("z-index", self.options.zIndex);
                } else {
                    let el_index = $(el).css("z-index") - 1;
                    $(el).css("z-index", el_index);
                }
            });
        });

        this._changeColor();
        //this._changeTitle();  // Keep Original <div wliu-popup-head>XXXXX</div>
        //this._changeText();   // Keep Original 
        this._changeHeight();
        this._changeMask();
        this._changeMove();
        this._changeResize();
        this._changeToggle();
        this._changePos();
        this._changeIcon();
    },
    _init: function () { },
    _setOption: function (key, val) {
        //important :  remove the old toggle
        if (key === "toggle" && this.options.toggle !== "")
            $("body").off("click.popup", `[popup-target="${this.options.toggle}"]`);

        this._super(key, val);
        switch (key) {
            case "color":
                this._changeColor();
                break;
            case "title":
                this._changeTitle();
                break;
            case "text":
                this._changeText();
                break;
            case "height":
                this._changeHeight();
                break;
            case "maskable":
                this._changeMask();
                break;
            case "movable":
                this._changeMove();
                break;
            case "resizable":
                this._changeResize();
                break;
            case "toggle":
                this._changeToggle();
                break;
            case "pos":
                this._changePos();
                break;
            case "icon":
                this._changeIcon();
                break;
           }
    },
    _changeColor: function () {
        this.element.attr("wl-color", this.options.color);
    },
    _changeTitle: function () {
        $(">div[wliu-popup-head]", this.element).empty().html(`${this.options.title}<a wliu-popup-close-icon wliu-popup-close></a>`);
    },
    _changeText: function () {
        $(">div[wliu-popup-body]", this.element).empty().html(this.options.text);
    },
    _changeHeight: function () {
        if(this.options.height>0)
            $(this.element).height(this.options.height);
        else 
            $(this.element).css("height","auto");
    },
    _changeMask: function () {
        var self = this;

        if(this.options.maskable) {
            if($("div[wliu-popup-mask]").length>0) {
                $("div[wliu-popup-mask]").appendTo("body");
            } else {
                $("body").append("<div wliu-popup-mask></div>");
            }

            $("div[wliu-popup-mask]").off("click.popup").on("click.popup", function (evt) {
                $("div[wliu-popup][wl-maskable]:visible").each(function (idx, el) {
                        $(el).hide();
                        $(el).css("z-index", self.options.zIndex);

                        $("div[wliu-popup-mask]").hide();
                        $("iframe[wliu-popup-mask]").hide();
                });
            });


            if($("iframe[wliu-popup-mask]").length>0) {
                $("iframe[wliu-popup-mask]").appendTo("body");
            } else {
                $("body").append("<iframe wliu-popup-mask></iframe>");
            }

        }


        if(this.options.maskable) {
            self.element.addAttr("wl-maskable");
        } else {
            this.element.removeAttr("wl-maskable");
        }
    },
    _changeMove: function () {
        var self = this;
        if (this.options.movable) {
            if( this.element.hasAttr("wl-pos")==false) {
                this.element.addAttr("wl-movable");
                this.element.show();
                var nTop = ($(window).innerHeight() - this.element.outerHeight()) / 2;
                var nLeft = ($(window).innerWidth() - this.element.outerWidth()) / 2;
                if (nTop <= 10) nTop = 20;
                if (nLeft <= 10) nLeft = 20;
                this.element.hide();
                this.element.css("transform", "none").css({ "top": nTop, "left": nLeft });
                this.element.draggable({
                    handle: ">[wliu-popup-head]",
                    stop: function (event, ui) {
                        self._inBound();
                    }
                });
            }
        } else {
            this.element.removeAttr("wl-movable");
            if (this.element.draggable('instance')) this.element.draggable('destroy');
        }
    },
    _changeResize: function () {
        var self = this;
        if (this.options.resizable) {
            this.element.addAttr("wl-resizable");
            this.element.resizable({
                stop: function(event, ui) {
                    self._inBound();
                }
            });
        } else {
            this.element.removeAttr("wl-resizable");
            if (this.element.resizable('instance')) this.element.resizable("destroy");
        }
    },
    _changePos: function () {
        if((""+this.options.pos)!="")
            this.element.attr("wl-pos", this.options.pos);
        else 
            this.element.removeAttr("wl-pos");
    },
    _changeIcon: function () {
        let self = this;
        if((""+this.options.icon)!="") {
            this.element.attr("wl-icon", this.options.icon);
            if(this.options.icon.indexOf("confirm")>=0 ) {
                let tmp = this.options.icon.split("|");
                let yesBtn = tmp[1]?tmp[1]:"OK";
                let noBtn = tmp[2]?tmp[2]:"CANCEL";
                $(`>div[wliu-popup-foot]`, this.element).empty().append(`<button wliu-button wliu-button-popup wliu-popup-yes>${yesBtn}</button><button wliu-button wliu-button-popup wliu-popup-no>${noBtn}</button>`);
                $(">div[wliu-popup-foot]>button[wliu-button][wliu-button-popup][wliu-popup-yes]", this.element).off("click.popup").on("click.popup", function(evt){
                    self._trigger("yes", null, self.options.data);
                    self.hide();
                });
                $(">div[wliu-popup-foot]>button[wliu-button][wliu-button-popup][wliu-popup-no]", this.element).off("click.popup").on("click.popup", function(evt){
                    self._trigger("no", null, self.options.data);
                    self.hide();
                });
            } else {
                $("button[wliu-button][wliu-button-popup][wliu-popup-close]", this.element).text(this.options.icon);
            } 
        } else {
            this.element.removeAttr("wl-icon");
        }
    },
    show: function () {
        var self = this;
        // re-order diag layer.
        this.element.css("z-index", this.options.zIndex + 100);
        $("div[wliu-popup]").not(this.element).each(function (idx, el) {
            if ($(el).css("z-index") <= self.options.zIndex) {
                $(el).css("z-index", self.options.zIndex);
            } else {
                let el_index = $(el).css("z-index") - 1;
                $(el).css("z-index", el_index);
            }
        });

        self.element.show();
        if (self.options.maskable) {
            $("div[wliu-popup-mask]").show();
            $("iframe[wliu-popup-mask]").show();
        }
        if(self.options.movable)  self._inBound();
        this._trigger("open", null, this.options.data);
    },
    hide: function () {
        this.element.hide();
        this.element.css("z-index", this.options.zIndex);
        if (this.options.maskable) {
            if ($("div[wliu-popup][wl-maskable]:visible").length > 0) {
                $("div[wliu-popup-mask]").show();
                $("iframe[wliu-popup-mask]").show();
            } else {
                $("div[wliu-popup-mask]").hide();
                $("iframe[wliu-popup-mask]").hide();
            }
        }
        this._trigger("close",null, this.options.data);
    },
    _changeToggle: function () {
        var self = this;
        if (("" + self.options.toggle).trim() !== "") {
            $("body").off("click.popup", `[popup-target="${this.options.toggle}"]`).on("click.popup", `[popup-target="${this.options.toggle}"]`, function(evt){
                if( $(this).hasAttr("popup-color") ) {
                    self.options.color=$(this).attr("popup-color");
                    self._changeColor();
                }
                if( $(this).hasAttr("popup-title") ) {
                    self.options.title=$(this).attr("popup-title");
                    self._changeTitle();
                }
                if( $(this).hasAttr("popup-text") ) {
                    self.options.text=$(this).attr("popup-text");
                    self._changeText();
                }
                self.show();
            });
        } else {
            $("body").off("click.popup", `[popup-target="${this.options.toggle}"]`);
        } 
    },
    _inBound: function () {
        if ($(window).innerHeight() - (this.element.offset().top - $(window).scrollTop() + this.element.outerHeight()) <= 10) {
            let nTop = $(window).innerHeight() - this.element.outerHeight() - 20;
            if (nTop <= 10) nTop = 20;
            this.element.css({ "top": nTop });
        }
        if (this.element.offset().top - $(window).scrollTop() <= 10) {
            let nTop = 20;
            this.element.css({ "top": nTop });
        }

        if ($(window).innerWidth() - (this.element.offset().left - $(window).scrollLeft() + this.element.outerWidth()) <= 10) {
            let nLeft = $(window).innerWidth() - this.element.outerWidth() - 20;
            if (nLeft <= 10) nLeft = 20;
            this.element.css({ "left": nLeft });
        }

        if (this.element.offset().left - $(window).scrollLeft() <= 10) {
            let nLeft = 20;
            this.element.css({ "left": nLeft });
        }
    },
    destroy: function () {
        this.element.off("click.popupClose", "[wliu-popup-close]");
        this.element.off("click.popupActive");
        $("body").off("click.popup", `[popup-target="${this.options.toggle}"]`);        

        if($("div[wliu-popup][wl-maskable]").not(this.element).length<=0)
            $("div[wliu-popup-mask]").off("click.popup");

        $(">div[wliu-popup-foot]>button[wliu-button][wliu-button-popup][wliu-popup-yes]", this.element).off("click.popup");            
        $(">div[wliu-popup-foot]>button[wliu-button][wliu-button-popup][wliu-popup-no]", this.element).off("click.popup");            
        
        if (this.element.draggable('instance')) this.element.draggable('destroy');
        if (this.element.resizable('instance')) this.element.resizable("destroy");
        $.Widget.prototype.destroy.call(this);
    }
});

$.widget("wliu.diag", {     // wl-title, wl-color, wl-pos, wl-height, wl-maskable, wl-movable, wl-resizable, wl-toggle, wl-icon="关闭"
    options: {
        title:          "",
        color:          "",
        pos:            "", //Important: pos must be ahaad of movable:  {pos:"",  movable:true}, otherwise: movable not working  //default: center, lt, ct, rt, lb, cb, rb
        height:         0,
        maskable:       false,
        movable:        false,
        resizable:      false,
        toggle:         "",   // button  diag-toggle  diag-data
        icon:           "",
        data:           null,
        zIndex:         8002
    },
    _create: function () {
        this.element.appendTo("body");
        this.options.title = this.element.hasAttr("wl-title") ? this.element.attr("wl-title") : this.options.title;
        this.options.color = this.element.hasAttr("wl-color") ? this.element.attr("wl-color") : this.options.color;
        this.options.pos = this.element.hasAttr("wl-pos") ? this.element.attr("wl-pos") : this.options.pos;
        this.options.height = this.element.hasAttr("wl-height") ? this.element.attr("wl-height") : this.options.height;

        this.options.maskable = this.element.hasAttr("wl-maskable") ? true : this.options.maskable;
        this.options.movable = this.element.hasAttr("wl-movable") ? true : this.options.movable;
        this.options.resizable = this.element.hasAttr("wl-resizable") ? true : this.options.resizable;

        this.options.toggle = this.element.hasAttr("wl-toggle") ? this.element.attr("wl-toggle") : this.options.toggle;
        this.options.icon = this.element.hasAttr("wl-icon") ? this.element.attr("wl-icon") : this.options.icon;
        
        if( $(">div[wliu-diag-head]", this.element).length<=0 )
            this.element.prepend(`<div wliu-diag-head>${this.options.title}<a wliu-diag-close-icon wliu-diag-close></a></div>`);
        else 
            if($(">div[wliu-diag-head]>a[wliu-diag-close-icon]", this.element).length<=0)
                $(">div[wliu-diag-head]", this.element).append('<a wliu-diag-close-icon wliu-diag-close></a>');

        if( $(">div[wliu-diag-body]", this.element).length<=0 )
            this.element.prepend(`<div wliu-diag-body>${this.options.text}</div>`);
        else if(this.options.text!="")
            $(">div[wliu-diag-body]", this.element).html(this.options.text);
            
        if( $(">div[wliu-diag-foot]", this.element).length<=0 )
            this.element.prepend(`<div wliu-diag-foot><button wliu-button wliu-button-diag wliu-diag-close>CLOSE</button></div>`);
        
        var self = this;
        this.element.off("click.diagClose", "[wliu-diag-close]").on("click.diagClose", "[wliu-diag-close]", function(evt){
            self.hide();
        });
        this.element.off("click.diagActive").on("click.diagActive", function(evt){
            if(self.element.is(":visible")) {
                self.element.css("z-index", self.options.zIndex + 100);
            }
            $("div[wliu-diag]:visible").not(self.element).each(function (idx, el) {
                if ($(el).css("z-index") <= self.options.zIndex) {
                    $(el).css("z-index", self.options.zIndex);
                } else {
                    let el_index = $(el).css("z-index") - 1;
                    $(el).css("z-index", el_index);
                }
            });
        });

        this._changeColor();
        //this._changeTitle();  // Keep Original <div wliu-diag-head>XXXXX</div>
        this._changeHeight();
        this._changeMask();
        this._changeMove();
        this._changeResize();
        this._changeToggle();
        this._changePos();
        this._changeIcon();
    },
    _init: function () { },
    _setOption: function (key, val) {
        //important :  remove the old toggle
        if (key === "toggle" && this.options.toggle !== "")
            $("body").off("click.diag", `[diag-target="${this.options.toggle}"]`);

        this._super(key, val);
        switch (key) {
            case "color":
                this._changeColor();
                break;
            case "title":
                this._changeTitle();
                break;
            case "height":
                this._changeHeight();
                break;
            case "maskable":
                this._changeMask();
                break;
            case "movable":
                this._changeMove();
                break;
            case "resizable":
                this._changeResize();
                break;
            case "toggle":
                this._changeToggle();
                break;
            case "pos":
                this._changePos();
                break;
            case "icon":
                this._changeIcon();
                break;

        }
    },
    _changeColor: function () {
        this.element.attr("wl-color", this.options.color);
    },
    _changeTitle: function () {
        $(">div[wliu-diag-head]", this.element).empty().html(`${this.options.title}<a wliu-diag-close-icon wliu-diag-close></a>`);
    },
    _changeHeight: function () {
        if(this.options.height>0)
            $(this.element).height(this.options.height);
        else 
            $(this.element).css("height","auto");
    },
    _changeMask: function () {
        var self = this;

        if(this.options.maskable) {
            if($("div[wliu-diag-mask]").length>0) {
                $("div[wliu-diag-mask]").appendTo("body");
            } else {
                $("body").append("<div wliu-diag-mask></div>");
            }

            $("div[wliu-diag-mask]").off("click.diag").on("click.diag", function (evt) {
                $("div[wliu-diag][wl-maskable]:visible").each(function (idx, el) {
                        $(el).hide();
                        $(el).css("z-index", self.options.zIndex);

                        $("div[wliu-diag-mask]").hide();
                        $("iframe[wliu-diag-mask]").hide();
                });
            });


            if($("iframe[wliu-diag-mask]").length>0) {
                $("iframe[wliu-diag-mask]").appendTo("body");
            } else {
                $("body").append("<iframe wliu-diag-mask></iframe>");
            }

        }


        if(this.options.maskable) {
            self.element.addAttr("wl-maskable");
        } else {
            this.element.removeAttr("wl-maskable");
        }
    },
    _changeMove: function () {
        var self = this;
        if (this.options.movable) {
            if( this.element.hasAttr("wl-pos")==false) {
                this.element.addAttr("wl-movable");
                this.element.show();
                var nTop = ($(window).innerHeight() - this.element.outerHeight()) / 2;
                var nLeft = ($(window).innerWidth() - this.element.outerWidth()) / 2;
                if (nTop <= 10) nTop = 20;
                if (nLeft <= 10) nLeft = 20;
                this.element.hide();
                this.element.css("transform", "none").css({ "top": nTop, "left": nLeft });
                this.element.draggable({
                    handle: ">[wliu-diag-head]",
                    stop: function (event, ui) {
                        self._inBound();
                    }
                });
            }
        } else {
            this.element.removeAttr("wl-movable");
            if (this.element.draggable('instance')) this.element.draggable('destroy');
        }
    },
    _changeResize: function () {
        var self = this;
        if (this.options.resizable) {
            this.element.addAttr("wl-resizable");
            this.element.resizable({
                stop: function(event, ui) {
                    self._inBound();
                }
            });
        } else {
            this.element.removeAttr("wl-resizable");
            if (this.element.resizable('instance')) this.element.resizable("destroy");
        }
    },
    _changePos: function () {
        if((""+this.options.pos)!="")
            this.element.attr("wl-pos", this.options.pos);
        else 
            this.element.removeAttr("wl-pos");
    },
    _changeIcon: function () {
        let self = this;
        if((""+this.options.icon)!="") {
            this.element.attr("wl-icon", this.options.icon);
            $("button[wliu-button][wliu-button-diag][wliu-diag-close]", this.element).text(this.options.icon);
        } else {
            this.element.removeAttr("wl-icon");
        }
    },    
    show: function () {
        var self = this;
        // re-order diag layer.
        this.element.css("z-index", this.options.zIndex + 100);
        $("div[wliu-diag]").not(this.element).each(function (idx, el) {
            if ($(el).css("z-index") <= self.options.zIndex) {
                $(el).css("z-index", self.options.zIndex);
            } else {
                let el_index = $(el).css("z-index") - 1;
                $(el).css("z-index", el_index);
            }
        });

        self.element.show();
        if (self.options.maskable) {
            $("div[wliu-diag-mask]").show();
            $("iframe[wliu-diag-mask]").show();
        }
        if(self.options.movable)  self._inBound();
        this._trigger("open", null, this.options.data);
    },
    hide: function () {
        this.element.hide();
        this.element.css("z-index", this.options.zIndex);
        if (this.options.maskable) {
            if ($("div[wliu-diag][wl-maskable]:visible").length > 0) {
                $("div[wliu-diag-mask]").show();
                $("iframe[wliu-diag-mask]").show();
            } else {
                $("div[wliu-diag-mask]").hide();
                $("iframe[wliu-diag-mask]").hide();
            }
        }
        this._trigger("close",null, this.options.data);
    },
    _changeToggle: function () {
        var self = this;
        if (("" + self.options.toggle).trim() !== "") {
            $("body").off("click.diag", `[diag-target="${this.options.toggle}"]`).on("click.diag", `[diag-target="${this.options.toggle}"]`, function(evt){
                if( $(this).hasAttr("diag-color") ) {
                    self.options.color=$(this).attr("diag-color");
                    self._changeColor();
                }
                if( $(this).hasAttr("diag-title") ) {
                    self.options.title=$(this).attr("diag-title");
                    self._changeTitle();
                }
                self.show();
            });
        } else {
            $("body").off("click.diag", `[diag-target="${this.options.toggle}"]`);
        } 
    },
    _inBound: function () {
        if ($(window).innerHeight() - (this.element.offset().top - $(window).scrollTop() + this.element.outerHeight()) <= 10) {
            let nTop = $(window).innerHeight() - this.element.outerHeight() - 20;
            if (nTop <= 10) nTop = 20;
            this.element.css({ "top": nTop });
        }
        if (this.element.offset().top - $(window).scrollTop() <= 10) {
            let nTop = 20;
            this.element.css({ "top": nTop });
        }

        if ($(window).innerWidth() - (this.element.offset().left - $(window).scrollLeft() + this.element.outerWidth()) <= 10) {
            let nLeft = $(window).innerWidth() - this.element.outerWidth() - 20;
            if (nLeft <= 10) nLeft = 20;
            this.element.css({ "left": nLeft });
        }

        if (this.element.offset().left - $(window).scrollLeft() <= 10) {
            let nLeft = 20;
            this.element.css({ "left": nLeft });
        }
    },
    destroy: function () {
        this.element.off("click.diagClose", "[wliu-diag-close]");
        this.element.off("click.diagActive");
        $("body").off("click.diag", `[diag-target="${this.options.toggle}"]`);        

        if($("div[wliu-diag][wl-maskable]").not(this.element).length<=0)
            $("div[wliu-diag-mask]").off("click.diag");

        if (this.element.draggable('instance')) this.element.draggable('destroy');
        if (this.element.resizable('instance')) this.element.resizable("destroy");
        $.Widget.prototype.destroy.call(this);
    }
});

$(function () {
    $("div[wliu-loading]").loading();
    $("div[wliu-hint]").hint();
    $("div[wliu-tooltip]").tooltip();
    $("div[wliu-block]").block();
    $("ul[wliu-tab]").tab();
    $("ul[wliu-tree][root]").tree();
    $("div[wliu-flip]").flip();
    $("div[wliu-popup]").popup();
    $("div[wliu-diag]").diag();
});

$.fn.extend({
    // $(xxx).hasAttr("attr1 attr2 attr3")  or  $(xxx).hasAttr("att1 att2, att3 att4)  "att1 att2" or "att3 att4"
    hasAttr: function (attrs) {
        attrs = "" + attrs;
        if ( (""+attrs).trim() === "") return false;
        var flag = true;
        this.each(function (idx, el) {
            var attrArr = [];
            attrArr.push(attrs); // default assume single attribute
            var seperate = "";

            if (attrs.indexOf(" ") >= 0) {
                attrArr = attrs.split(" ");  // multple attribute seperate by “ ”
                seperate = "";
            }
            if (attrs.indexOf(",") >= 0) {    
                attrArr = attrs.split(",");  // or seperate by ","
                seperate = ",";
            }

            var attrStr = "";
            for (var idx1 in attrArr) {
                if (attrArr[idx1].trim() !== "") {
                    var temp1 = attrArr[idx1].trim();
                    var attrName = "";
                    if (temp1.indexOf(" ") >= 0) {
                        temp2 = temp1.split(" ");
                        for (var idx2 in temp2) {
                            var temp3 = temp2[idx2].trim();
                            attrName += "[" + temp3 + "]";
                        }
                    } else {
                        attrName = "[" + attrArr[idx1].trim() + "]";
                    }
                    attrStr += (attrStr ? seperate : "") + attrName;
                }
            }
            if (attrStr === "") attrStr = "*";
            if ($(el).filter(attrStr).length <= 0) flag = false;
        });
        return flag;
    },
    // $(xxx).addAttr("attr1 attr2 attr3")
    addAttr: function (attrs) {
        attrs = "" + attrs;
        return this.each(function (idx, el) {
            if (("" + attrs).trim() === "") return false;
            var attrArr = {};
            if (attrs.indexOf(" ") >= 0) {
                attrArr1 = attrs.split(" ");  // multple attribute seperate by “ ”
                for (var idx1 in attrArr1) {
                    if (attrArr1[idx1].trim() !== "") {
                        attrArr[attrArr1[idx1].trim()] = "";
                    }
                }
            }
            else
            {
                attrArr[attrs] = "";
            }
            $(el).attr(attrArr); 
            return true;
        });
    }
    // removeAttr  already defined in JQuery   $(xxx).removeAttr("attr1  attr2 attr3")
});