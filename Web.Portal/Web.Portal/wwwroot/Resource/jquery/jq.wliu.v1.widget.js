$.widget("wliu.tab9", {
    options: {
        tabsn:  0,
        height: 0,
        border: false,
        color:  ""
    },
    _create: function () {
        var self = this;
        $("li", this.element).not(":has(span)").each(function (idx, el) {
            var text = $(el).text();
            $(el).empty();
            $(el).append("<span>" + text + "</span>");
        });
        $("li", this.element).not(":has(s)").append("<s>");

        $("li", this.element).each(function (idx, el) {
            $(el).attr("tabsn", idx);
        });
        $("+div[wliu][tab9][body]>div", this.element).each(function (idx, el) {
            $(el).attr("tabsn", idx);
        });

        this.options.border = this.element.hasAttr("border") ? true : this.options.border;
        this.options.color = this.element.hasAttr("color") ? this.element.attr("color") : this.options.color;

        $("li", this.element).off("click.tab9").on("click.tab9", function (evt) {
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
        $("li", this.element).attr("selected", null);
        $("+div[wliu][tab9][body]>div", this.element).attr("selected", null);

        if ($(`li[tabsn="${this.options.tabsn}"]`, this.element).length <= 0) {
            this.options.tabsn = 0;
        }

        $(`li[tabsn="${this.options.tabsn}"]`, this.element).attr("selected", "");
        $(`+div[wliu][tab9][body]>div[tabsn="${this.options.tabsn}"]`, this.element).attr("selected", "");
    },
    _changeHeight: function () {
        if (this.options.height > 0) 
            $("+div[wliu][tab9][body]", this.element).height(this.options.height);
        else
            $("+div[wliu][tab9][body]", this.element).height("auto");
    },
    _changeColor: function () {
        if (("" + this.options.color).trim() === "")
            this.element.attr("color", null);
        else
            this.element.attr("color", this.options.color);
    },
    _changeBorder: function () {
        if (this.options.border)
            this.element.attr("border", "");
        else
            this.element.attr("border", null);
    },
    destroy: function () {
        $("li", this.element).off("click.tab9");
        $.Widget.prototype.destroy.call(this);
    }
});

$.widget("wliu.block", {
    options: {
        color: ""
    },
    _create: function () {
        var self = this;
        self.options.color = self.element.hasAttr("color") ? self.element.attr("color") : self.options.color;
        if (self.element.hasAttr("collapse")) $(">div[body]", self.element).slideUp(1200);

        $("[head][toggle]", self.element).off("click.block").on("click.block", function (evt) {
            if (self.element.hasAttr("collapse")) {
                self.element.removeAttr("collapse");
                $(">div[body]", self.element).slideDown(1200);
            } else {
                self.element.addAttr("collapse");
                $(">div[body]", self.element).slideUp(1200);
            }
        });
        self._changeColor();
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
        if (("" + this.options.color).trim() === "")
            this.element.attr("color", null);
        else
            this.element.attr("color", this.options.color);
    },
    destroy: function () {
        $("[toggle]+[head]", this.element).off("click.block");
        $.Widget.prototype.destroy.call(this);
    }
});

$.widget("wliu.loading", {
    options: {
        text: "Please Waiting...",
        maskable: true
    },
    _create: function () {
        this.element.appendTo("body");
        this.options.maskable = this.element.hasAttr("maskable") ? true : this.options.maskable;
        if ($(">[spin]", this.element).length <= 0) this.element.append(`<div spin></div>`);
        if ($(">[head]", this.element).length <= 0)
            this.element.append(`<div head></div>`);
        else
            this.options.text = $(">[head]", this.element).text();
        
        this._changeText();
        this._changeMask();
    },
    _init: function () {
    },
    _setOption: function (key, val) {
        this._super(key, val);
        switch (key) {
            case "text":
                this._changeText();
                break;
            case "maskable":
                this._changeMask();
                break;
        }
    },
    _changeText: function () {
        $(">[head]", this.element).text(this.options.text);
    },
    _changeMask: function () {
        if (this.options.maskable) {
            if ($("iframe[wliu][masker-loading]").length <= 0) $("body").append("<iframe wliu masker-loading></iframe>");
            if ($("div[wliu][masker-loading]").length <= 0) $("body").append("<div wliu masker-loading></div>");
        }
    },
    show: function () {
        this.element.show();
        if (this.options.maskable) {
            $("div[wliu][masker-loading]").show();
            $("iframe[wliu][masker-loading]").show();
        }
    },
    hide: function () {
        this.element.hide();
        if (this.options.maskable) {
            $("div[wliu][masker-loading]").hide();
            $("iframe[wliu][masker-loading]").hide();
        }
    },
    destroy: function () {
        $.Widget.prototype.destroy.call(this);
    }
});

$.widget("wliu.hint", {
    options: {
        text: "",
        type: "", // success, warning, error
        duration: 2000,
        pos: "cb"  // default "cb",  list: "cb", "ct", ""(cm),  "lb", "lt", "rb", "rt"
    },
    _create: function () {
        this.element.appendTo("body");
        this.options.pos = this.element.hasAttr("pos") ? this.element.attr("pos") : this.options.pos;
        this._changeText();
        this._changeType();
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
            case "type":
                this._changeType();
                break;
            case "pos":
                this._changePos();
                break;
        }
    },
    _changeText: function () {
        this.element.text(this.options.text);
    },
    _changeType: function () {
        this.element.removeAttr("sccess warning error").addAttr(this.options.type);
    },
    _changePos: function () {
        this.element.attr("pos", this.options.pos);
    },
    show: function (type, message) {
        if (("" + type).trim() !== "") {
            this.options.type = type;
            this._changeType();
        }

        if (("" + message).trim() !== "") {
            this.options.text = message;
            this._changeText();
        }
        if((""+this.options.text).trim()!=="")
            this.element.stop(true, true).fadeIn(10).delay(this.options.duration).fadeOut(1000);

    },
    destroy: function () {
        $.Widget.prototype.destroy.call(this);
    }
});

$.widget("wliu.float", {
    options: {
        text:   "",
        toggle: "",   // button  float-toggle  float-data
        pos: "cb",  // default "cb",  list: "cb", "ct", ""(cm),  "lb", "lt", "rb", "rt"
        duration: 0,
        data: null
    },
    _create: function () {
        this.element.appendTo("body");
        this.options.text = ("" + $(">[head]", this.element).html()).trim() === "" ? this.options.text : $(">[head]", this.element).html();
        this.options.toggle = this.element.hasAttr("float-toggle") ? this.element.attr("float-toggle") : this.options.toggle;
        this.options.pos = this.element.hasAttr("pos") ? this.element.attr("pos") : this.options.pos;
        this.options.duration = parseInt(this.element.hasAttr("duration") ? this.element.attr("duration") : this.options.duration);
        if ($(">a[wliu][btn16][close]", this.element).length <= 0) this.element.prepend("<a wliu btn16 close></a>");
    },
    _init: function () {
        let self = this;
        this._changeText();
        this._changePos();
        this._changeToggle();
        $(">a[wliu][btn16][close]", this.element).off("click.float").on("click.float", function (event) {
            self.hide();
        });
    },
    _setOption: function (key, val) {
        this._super(key, val);
        switch (key) {
            case "text":
                this._changeText();
                break;
            case "pos":
                this._changePos();
                break;
            case "toggle":
                this._changeToggle();
                break;
            case "duration":
                this._changeDuration();
                break;
        }
    },
    _changeText: function () {
        if (this.options.text === "" || !this.options.text) {
            $(">div[head]", this.element).remove();
            $(">a[wliu][btn16][close]", this.element).css({ "top": "2px", "right":"2px"});
            $(this.element).css({"padding-top": "2px"});
        }
        else
        {
            if ($(">div[head]", this.element).length <= 0) {
                this.element.prepend(`<div head>${this.options.text}</div>`);
                $(">a[wliu][btn16][close]", this.element).css({ "top": "6px", "right": "6px" });
                $(this.element).css({"padding-top": "28px"});
            } else {
                $(">div[head]", this.element).html(this.options.text);
            }
        }
    },
    _changePos: function () {
        this.element.attr("pos", this.options.pos);
    },
    _changeToggle: function () {
        var self = this;
        if (("" + self.options.toggle).trim() !== "") {
            $(`[float-toggle="${self.options.toggle}"]`).not("[wliu][float]").off("click.float").on("click.float", function (event) {
                if ($(this).hasAttr("float-data")) self.options.data = $(this).attr("float-data");
                if (self.element.is(":visible"))
                    self.hide();
                else
                    self.show();
            });
        }
    },
    _changeDuration: function () {
    },
    show: function () {
        let self = this;
        if (parseInt(this.options.duration) > 0) {
            this._trigger("open", null, this.options.data);
            this.element.stop(true, true).show().delay(this.options.duration).fadeOut(800, function () {
                self._trigger("close", null, self.options.data);
            });
        } else {
            this.element.show();
            this._trigger("open", null, this.options.data);
        }
    },
    hide: function () {
        this.element.hide();
        this._trigger("close", null, this.options.data);
    },
    destroy: function () {
        $.Widget.prototype.destroy.call(this);
    }
});


$.widget("wliu.tooltip", {
    options: {
        toggle: "",
        text: "",
        trigger:    "hover",  // hover,  click
        duration:   800,
        pos:        ""   // lc, rc,  ct, cb
    },
    _create: function () {
        this.element.appendTo("body");
        if ($(">s[arrow]", this.element).length <= 0) this.element.append("<s arrow></s>");
        this.options.pos = this.element.hasAttr("pos") ? this.element.attr("pos") : this.options.pos;
        this.options.toggle = this.element.hasAttr("tooltip-toggle") ? this.element.attr("tooltip-toggle") : this.options.toggle;
        this.options.trigger = this.element.hasAttr("trigger") ? this.element.attr("trigger") : this.options.trigger;

        this._changePos();
        this._changeToggle();
    },
    _init: function () {
    },
    _setOption: function (key, val) {
        switch (key) {
            case "pos":
                this._super(key, val);
                this._changePos();
                break;
            case "toggle":
                this._super(key, val);
                this._changeToggle();
                break;
            case "trigger":
                this._super(key, val);
                this._changeToggle();
                break;
        }
    },
    _changePos: function () {
        this.element.attr("pos", this.options.pos);
    },
    _changeToggle: function () {
        var self = this;
        if (("" + self.options.toggle).trim() !== "") {
            $(`[tooltip-toggle="${self.options.toggle}"]`).not("[wliu][tooltip]").each(function (idx, el) {
                let el_trigger = $(el).hasAttr("tooltip-trigger") ? $(el).attr("tooltip-trigger") : self.options.trigger;
                switch (("" + el_trigger).toLowerCase()) {
                    case "click":
                        $(el).off("click.tooltip").on("click.tooltip", function (event) {
                            let nleft = -900;
                            let ntop = -900;
                            let el_content = "" + $(el).attr("tooltip-body");
                            if (el_content !== "") {
                                // body
                                if ($(">div[body]", self.element).length <= 0)
                                    self.element.append(`<div body>${el_content}</div>`);
                                else
                                    $(">div[body]", self.element).empty().html(el_content);

                                // head
                                if ($(el).hasAttr("tooltip-head") && $(el).attr("tooltip-head")!=="") {
                                    if ($(">div[head]", self.element).length <= 0)
                                        self.element.prepend(`<div head>${$(el).attr("tooltip-head")}</div>`);
                                    else
                                        $(">div[head]", self.element).empty().html($(el).attr("tooltip-head"));

                                    $(">s[arrow]", self.element).addAttr("head");
                                } else {
                                    $(">div[head]", self.element).remove();
                                    $(">s[arrow][head]", self.element).removeAttr("head");
                                }

                                let el_pos = $(el).hasAttr("tooltip-pos") ? $(el).attr("tooltip-pos") : self.options.pos;
                                $(self.element).attr("pos", "cb");
                                switch (("" + el_pos).toLowerCase()) {
                                    case "lc":
                                        $(self.element).attr("pos", "lc");
                                        nleft = $(el).offset().left + $(el).outerWidth() + 12;
                                        ntop = $(el).offset().top + $(el).outerHeight() / 2 - self.element.outerHeight() / 2;
                                        break;
                                    case "rc":
                                        $(self.element).attr("pos", "rc");
                                        nleft = $(el).offset().left - $(self.element).outerWidth() - 12;
                                        ntop = $(el).offset().top + $(el).outerHeight() / 2 - self.element.outerHeight() / 2;
                                        break;
                                    case "ct":
                                        $(self.element).attr("pos", "ct");
                                        nleft = $(el).offset().left + ($(el).outerWidth() - self.element.outerWidth()) / 2;
                                        ntop = $(el).offset().top + $(el).outerHeight() + 12;
                                        break;
                                    case "cb":
                                        $(self.element).attr("pos", "cb");
                                        nleft = $(el).offset().left + ($(el).outerWidth() - self.element.outerWidth()) / 2;
                                        ntop = $(el).offset().top - self.element.outerHeight() - 12;
                                        break;
                                    default:
                                        $(self.element).attr("pos", "cb");
                                        nleft = $(el).offset().left + ($(el).outerWidth() - self.element.outerWidth()) / 2;
                                        ntop = $(el).offset().top - self.element.outerHeight() - 12;
                                        break;
                                }

                                // relocation if out of window
                                if ($(el).offset().left - self.element.outerWidth() <= 12) {
                                    self.element.attr("pos", "cb");
                                    nleft = $(el).offset().left + ($(el).outerWidth() - self.element.outerWidth()) / 2;
                                    ntop = $(el).offset().top - self.element.outerHeight() - 12;
                                }
                                if ($(el).offset().left + $(el).outerWidth() + self.element.outerWidth() > $(window).scrollLeft() + window.innerWidth) {
                                    self.element.attr("pos", "cb");
                                    nleft = $(el).offset().left + ($(el).outerWidth() - self.element.outerWidth()) / 2;
                                    ntop = $(el).offset().top - self.element.outerHeight() - 12;
                                }
                                if ($(el).offset().top + $(el).outerHeight() + self.element.outerHeight() > $(window).scrollTop() + window.innerWidth) {
                                    self.element.attr("pos", "cb");
                                    nleft = $(el).offset().left + ($(el).outerWidth() - self.element.outerWidth()) / 2;
                                    ntop = $(el).offset().top - self.element.outerHeight() - 12;
                                }

                            }
                            else {
                                $(">[body]", self.element).empty();
                                $(">[head]", self.element).remove();
                                $(">s[arrow][head]", self.element).removeAttr("head");

                                self.element.removeAttr("active");
                                $(">s[arrow]", self.element).removeAttr("active");
                            }

                            if ($(el).hasAttr("tooltip-active")) {
                                $(el).removeAttr("tooltip-active");
                                self.element.removeAttr("active");
                                $(">s[arrow]", self.element).removeAttr("active");
                                self.element.offset({ left: -900, top: -900 });
                            } else {
                                $(`[tooltip-toggle="${self.options.toggle}"]`).removeAttr("tooltip-active");
                                $(el).addAttr("tooltip-active");
                                self.element.addAttr("active");
                                $(">s[arrow]", self.element).addAttr("active");
                                self.element.offset({ left: nleft, top: ntop });
                            }
                        });
                        break;
                    case "hover":
                        $(el).off("mouseover.tooltip").on("mouseover.tooltip", function (event) {
                            let nleft = -900;
                            let ntop = -900;
                            let el_content = "" + $(el).attr("tooltip-body");
                            if (el_content !== "") {
                                // body
                                if ($(">div[body]", self.element).length <= 0)
                                    self.element.append(`<div body>${el_content}</div>`);
                                else
                                    $(">div[body]", self.element).empty().html(el_content);

                                // head
                                if ($(el).hasAttr("tooltip-head") && $(el).attr("tooltip-head") !== "") {
                                    if ($(">div[head]", self.element).length <= 0)
                                        self.element.prepend(`<div head>${$(el).attr("tooltip-head")}</div>`);
                                    else
                                        $(">div[head]", self.element).empty().html($(el).attr("tooltip-head"));

                                    $(">s[arrow]", self.element).addAttr("head");
                                } else {
                                    $(">div[head]", self.element).remove();
                                    $(">s[arrow][head]", self.element).removeAttr("head");
                                }

                                let el_pos = $(el).hasAttr("tooltip-pos") ? $(el).attr("tooltip-pos") : self.options.pos;
                                $(self.element).attr("pos", "cb");
                                switch (("" + el_pos).toLowerCase()) {
                                    case "lc":
                                        $(self.element).attr("pos", "lc");
                                        nleft = $(el).offset().left + $(el).outerWidth() + 12;
                                        ntop = $(el).offset().top + $(el).outerHeight() / 2 - self.element.outerHeight() / 2;
                                        break;
                                    case "rc":
                                        $(self.element).attr("pos", "rc");
                                        nleft = $(el).offset().left - $(self.element).outerWidth() - 12;
                                        ntop = $(el).offset().top + $(el).outerHeight() / 2 - self.element.outerHeight() / 2;
                                        break;
                                    case "ct":
                                        $(self.element).attr("pos", "ct");
                                        nleft = $(el).offset().left + ($(el).outerWidth() - self.element.outerWidth()) / 2;
                                        ntop = $(el).offset().top + $(el).outerHeight() + 12;
                                        break;
                                    case "cb":
                                        $(self.element).attr("pos", "cb");
                                        nleft = $(el).offset().left + ($(el).outerWidth() - self.element.outerWidth()) / 2;
                                        ntop = $(el).offset().top - self.element.outerHeight() - 12;
                                        break;
                                    default:
                                        $(self.element).attr("pos", "cb");
                                        nleft = $(el).offset().left + ($(el).outerWidth() - self.element.outerWidth()) / 2;
                                        ntop = $(el).offset().top - self.element.outerHeight() - 12;
                                        break;
                                }

                                // relocation if out of window
                                if ($(el).offset().left - self.element.outerWidth() <= 12) {
                                    self.element.attr("pos", "cb");
                                    nleft = $(el).offset().left + ($(el).outerWidth() - self.element.outerWidth()) / 2;
                                    ntop = $(el).offset().top - self.element.outerHeight() - 12;
                                }
                                if ($(el).offset().left + $(el).outerWidth() + self.element.outerWidth() > $(window).scrollLeft() + window.innerWidth) {
                                    self.element.attr("pos", "cb");
                                    nleft = $(el).offset().left + ($(el).outerWidth() - self.element.outerWidth()) / 2;
                                    ntop = $(el).offset().top - self.element.outerHeight() - 12;
                                }
                                if ($(el).offset().top + $(el).outerHeight() + self.element.outerHeight() > $(window).scrollTop() + window.innerWidth) {
                                    self.element.attr("pos", "cb");
                                    nleft = $(el).offset().left + ($(el).outerWidth() - self.element.outerWidth()) / 2;
                                    ntop = $(el).offset().top - self.element.outerHeight() - 12;
                                }
                            }
                            else {
                                $(">[body]", self.element).empty();
                                $(">[head]", self.element).remove();
                                $(">s[arrow][head]", self.element).removeAttr("head");

                                self.element.removeAttr("active");
                                $(">s[arrow]", self.element).removeAttr("active");
                            }

                            $(`[tooltip-toggle="${self.options.toggle}"]`).removeAttr("tooltip-active");
                            $(el).addAttr("tooltip-active");
                            self.element.addAttr("active");
                            $(">s[arrow]", self.element).addAttr("active");
                            self.element.offset({ left: nleft, top: ntop });
                        });

                        $(el).off("mouseout.tooltip").on("mouseout.tooltip", function (event) {
                            $(el).removeAttr("tooltip-active");
                            self.element.removeAttr("active");
                            $(">s[arrow]", self.element).removeAttr("active");
                            self.element.offset({ left: -900, top: -900 });
                        });
                        $(el).off("mouseleft.tooltip").on("mouseleft.tooltip", function (event) {
                            $(el).removeAttr("tooltip-active");
                            self.element.removeAttr("active");
                            $(">s[arrow]", self.element).removeAttr("active");
                            self.element.offset({ left: -900, top: -900 });
                        });
                        break;
                    default:
                        break;
                }
               
            });
        }
    },
    _changeText: function () {
        $(">[body]", this.element).html(this.options.text);
    },
    show: function (message) {
       if (("" + message).trim() !== "") {
            this.options.text = message;
            this._changeText();
            this.element.stop(true, true).fadeIn(10).delay(this.options.duration).fadeOut(1000);
        }
    },
    destroy: function () {
        $.Widget.prototype.destroy.call(this);
    }
});

$.widget("wliu.diag", {
    options: {
        text:           "Information",
        content:        "",            // body can not be init because body content is important, only change when setOption
        maskable:       false,
        maskclick:      true,
        movable:        false,
        resizable:      false,
        toggle:         "",   // button  diag-toggle  diag-data
        data:           null,
        zIndex:         8002
    },
    _create: function () {
        this.element.appendTo("body");
        var self = this;
        this.options.toggle = this.element.hasAttr("diag-toggle") ? this.element.attr("diag-toggle") : this.options.toggle;
        this.options.maskable = this.element.hasAttr("maskable") ? true : this.options.maskable;
        this.options.movable = this.element.hasAttr("movable") ? true : this.options.movable;
        this.options.resizable = this.element.hasAttr("resizable") ? true : this.options.resizable;
        this.options.zIndex = this.element.css("z-index") > 0 ? parseInt(this.element.css("z-index")) : this.options.zIndex;
        if ($(">a[wliu][btn24][close]", this.element).length <= 0) this.element.prepend("<a wliu btn24 close></a>");
        //this.options.text = ("" + $(">[head]", this.element).html()).trim() === "" ? this.options.text : $(">[head]", this.element).html();
        //this._changeText();
        this._changeMask();
        this._changeMove();
        this._changeResize();
        this._changeZIndex();
    },
    _init: function () {
        let self = this;
        /*** Init Center of Screen ***/
        var nTop = ($(window).innerHeight() - this.element.outerHeight()) / 2;
        var nLeft = ($(window).innerWidth() - this.element.outerWidth()) / 2;
        if (nTop <= 10) nTop = 20;
        if (nLeft <= 10) nLeft = 20;
        this.element.css("transform", "none").css({ "top": nTop, "left": nLeft });


        this._changeToggle();
        $(">a[wliu][btn24][close]", this.element).off("click.diag").on("click.diag", function (event) {
            self.hide();
        });

        $(this.element).off("click.diagOrder").on("click.diagOrder", function (event) {
            // re-order diag layer.
            if (self.options.zIndex < 8800) {
                self.element.css("z-index", self.options.zIndex + 100);
                $("div[wliu][diag]").not(self.element).each(function (idx, el) {
                    if ($(el).css("z-index") <= self.options.zIndex) {
                        $(el).css("z-index", self.options.zIndex); // if lower than initialize,  reset to init value
                    } else {
                        let el_index = $(el).css("z-index") - 1;
                        $(el).css("z-index", el_index);
                    }
                });
            }
        });
    },
    _setOption: function (key, val) {
        //important :  remove the old toggle
        if (key === "toggle" && this.options.toggle !== "")
            $(`[diag-toggle="${this.options.toggle}"]`).off("click.diag");

        this._super(key, val);
        switch (key) {
            case "text":
                this._changeText();
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
            case "content":
                this._changeContent();
                break;
            case "zIndex":
                this._changeZIndex();
                break;

        }
    },
    _changeText: function () {
        if ($(">[head]", this.element).length <= 0)
            this.element.prepend(`<div head>${this.options.text}</div>`);
        else
            $(">[head]", this.element).html(this.options.text);
    },
    _changeContent: function () {
        $(">div[body]", this.element).html(this.options.content);
    },
    _changeMask: function () {
        var self = this;
        if (this.options.maskable) {
            if ($("iframe[wliu][masker-diag]").length <= 0) $("body").append("<iframe wliu masker-diag></iframe>");
            if ($("div[wliu][masker-diag]").length <= 0) $("body").append("<div wliu masker-diag></div>");
            // don't make off here, it will delete other click event of other diag
            $("div[wliu][masker-diag]").on("click.diag", function (event) {
                if (self.options.maskclick) {
                    self.hide();
                }
            });
            this.element.addAttr("maskable");
        } else {
            this.element.removeAttr("maskable");
        }
    },
    _changeMove: function () {
        this.element.css("transform", "none");
        var self = this;
        if (this.options.movable) {
            this.element.addAttr("movable");

            this.element.draggable({
                handle: ">[head]",
                stop: function (event, ui) {
                    self._inBound();
                }
            });
        } else {
            this.element.removeAttr("movable");
            if (this.element.draggable('instance')) this.element.draggable('destroy');
        }
    },
    _changeResize: function () {
        var self = this;
        if (this.options.resizable) {
            this.element.addAttr("resizable");
            this.element.resizable({
                stop: function(event, ui) {
                    self._inBound();
                }
            });
        } else {
            this.element.removeAttr("resizable");
            if (this.element.resizable('instance')) this.element.resizable("destroy");
        }
    },
    _changeZIndex: function () {
        this.element.css("z-index", this.options.zIndex);
    },
    initShow: function () {
        var self = this;
        // re-order diag layer.
        this.element.css("z-index", this.options.zIndex + 100);
        $("div[wliu][diag]").not(this.element).each(function (idx, el) {
            if ($(el).css("z-index") <= self.options.zIndex) {
                $(el).css("z-index", self.options.zIndex);
            } else {
                let el_index = $(el).css("z-index") - 1;
                $(el).css("z-index", el_index);
            }
        });

        self.element.show();
        if (self.options.maskable) {
            $("div[wliu][masker-diag]").show();
            $("iframe[wliu][masker-diag]").show();
        }
        self._initBound();
        this._trigger("open", null, this.options.data);
    },
    show: function () {
        var self = this;
        // re-order diag layer.
        this.element.css("z-index", this.options.zIndex + 100);
        $("div[wliu][diag]").not(this.element).each(function (idx, el) {
            if ($(el).css("z-index") <= self.options.zIndex) {
                $(el).css("z-index", self.options.zIndex);
            } else {
                let el_index = $(el).css("z-index") - 1;
                $(el).css("z-index", el_index);
            }
        });

        self.element.show();
        if (self.options.maskable) {
            $("div[wliu][masker-diag]").show();
            $("iframe[wliu][masker-diag]").show();
        }
        self._inBound();
        this._trigger("open", null, this.options.data);
    },
    hide: function () {
        this.element.hide();
        if (this.options.maskable) {
            if ($("div[wliu][diag][maskable]:visible").length > 0) {
                $("div[wliu][masker-diag]").show();
                $("iframe[wliu][masker-diag]").show();
            } else {
                $("div[wliu][masker-diag]").hide();
                $("iframe[wliu][masker-diag]").hide();
            }
        }
        this._trigger("close",null, this.options.data);
    },
    _changeToggle: function () {
        var self = this;
        if (("" + self.options.toggle).trim() !== "") {
            $(`[diag-toggle="${self.options.toggle}"]`).not("[wliu][diag]").off("click.diag").on("click.diag", function (event) {
                if ($(this).hasAttr("diag-data")) self.options.data = $(this).attr("diag-data");
                if (self.element.is(":visible"))
                    self.hide();
                else
                    self.show();
            });
        } 
    },
    _initBound: function () {
        this.element.css("transform", "none");
        // center first
        let nTop = $(window).scrollTop() + ($(window).innerHeight() - this.element.outerHeight()) / 2 - 20;
        let nLeft = $(window).scrollLeft() + ($(window).innerWidth() - this.element.outerWidth()) / 2;
        this.element.css({ "left": nLeft, "top": nTop });
    },
    _inBound: function () {
        this.element.css("transform", "none");
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
        $(">a[wliu][btn24][close]", this.element).off("click.diag");
        $(`[diag-toggle="${this.options.toggle}"]`).off("click.diag");
        $("div[wliu][masker-diag]").off("click.diag");
        if (this.element.draggable('instance')) this.element.draggable('destroy');
        if (this.element.resizable('instance')) this.element.resizable("destroy");
        $.Widget.prototype.destroy.call(this);
    }
});

$.widget("wliu.flip", {
    options: {
        height: 200,
        toggle: ""
    },
    _create: function () {
        this.options.toggle = this.element.hasAttr("flip-toggle") ? this.element.attr("flip-toggle") : this.options.toggle;
        this.options.height = this.element.height();
        this._changeHeight();
        this._changeToggle();
    },
    _init: function () {
    },
    _setOption: function (key, val) {
        //important :  remove the old toggle
        if (key === "toggle" && this.options.toggle !== "")
            $(`[flip-toggle="${this.options.toggle}"]`).off("click.flip");
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
        if (this.options.height>=200)
            this.element.height(this.options.height);
    },
    _changeToggle: function () {
        var self = this;
        if (("" + self.options.toggle).trim() !== "") {
            $(`[flip-toggle="${self.options.toggle}"]`).not("[wliu][flip]").off("click.flip").on("click.flip", function (event) {
                if (self.element.hasAttr("reverse"))
                    self.element.removeAttr("reverse");
                else 
                    self.element.addAttr("reverse");

                return false;
            });
        } 
    },
    toggle: function () {
        if (this.element.hasAttr("reverse"))
            this.element.removeAttr("reverse");
        else
            this.element.addAttr("reverse");
    },
    destroy: function () {
        $(`[flip-toggle="${this.options.toggle}"]`).off("click.flip");
        $.Widget.prototype.destroy.call(this);
    }
});

$.widget("wliu.tree", {
    options: {
        name: "mytree",
        single: false,
        state: "open",
        icon: "",
        select: false
    },
    _create: function () {
    },
    _init: function () {
        var self = this;
        $("li[nodes]", this.element).each(function (idx, el) {
            if ($("s[hide]", el).length<=0) $(el).prepend("<s hide></s>");
            $(el).attr("eidx", idx++);
            let state = localStorage.getItem(self.options.name + "_" + $(el).attr("eidx"));
            state = state !== "open" && state !== "close" ? self.options.state : state;
            $(el).removeAttr("open").removeAttr("close").addAttr(state);
        });

        self._changeIcon();
        self._changeSelect();
        self._changeSingle();
    },
    _setOption: function (key, val) {
        this._super(key, val);
        switch (key) {
            case "name":
                this._changeName();
                break;
            case "single":
                this._changeSingle();
                break;
            case "state":
                this._changeState();
                break;
            case "icon":
                this._changeIcon();
                break;
            case "select":
                this._changeSelect();
                break;
        }
    },
    _changeName: function () { },
    _changeSingle: function () {
        let self = this;
        $("li>s", this.element).off("click.tree").on("click.tree", function (evt) {
            if ($(this).prop("tagName").toLowerCase() === "li") {
                if ($(this).hasAttr("nodes open")) {
                    $(this).removeAttr("open").addAttr("close");
                    localStorage.setItem(self.options.name + "_" + $(this).attr("eidx"), "close");
                } else if ($(this).hasAttr("nodes close")) {
                    if (self.options.single) $(this).siblings("li[nodes]").removeAttr("open").addAttr("close");
                    $(this).removeAttr("close").addAttr("open");
                    localStorage.setItem(self.options.name + "_" + $(this).attr("eidx"), "open");
                }
            }
            if ($(this).prop("tagName").toLowerCase() === "s") {
                if ($(this).parent("li").hasAttr("nodes open")) {
                    $(this).parent("li").removeAttr("open").addAttr("close");
                    localStorage.setItem(self.options.name + "_" + $(this).parent("li").attr("eidx"), "close");
                } else if ($(this).parent("li").hasAttr("nodes close")) {
                    if (self.options.single) $(this).parent("li").siblings("li[nodes]").removeAttr("open").addAttr("close");
                    $(this).parent("li").removeAttr("close").addAttr("open");
                    localStorage.setItem(self.options.name + "_" + $(this).parent("li").attr("eidx"), "open");
                }
            }
            //evt.preventDefault();
            //evt.stopPropagation();
            //return false;
        });
    },
    _changeState: function () {
        let self = this;
        $("li[nodes]", this.element).each(function (idx, el) {
            $(el).attr("eidx", idx++);
            $(el).removeAttr("open").removeAttr("close").addAttr(self.options.state);
        });
    },
    _changeIcon: function () {
        let self = this;
        if (self.options.icon !== "") {
            $("li[nodes], li[node]", this.element).each(function (idx, el) {
                if ($("s", el).not("s[hide]").length <= 0) {
                    $(el).prepend(`<s ${self.options.icon}></s>`);
                }
                else {
                    $("s", el).not("s[hide]").addAttr(self.options.icon);
                }
            });
        }
    },
    _changeSelect: function () {
        let self = this;
        if (self.options.select) {
            $("li[node]", this.element).off("click.select").on("click.select", function (evt) {
                $("li[node]", this.element).removeAttr("selected");
                $(this).addAttr("selected");
                evt.preventDefault();
                evt.stopPropagation();
                return false;
            });
        } else {
            $("li[node]", this.element).removeAttr("selected");
            $("li[node]", this.element).off("click.select");
        }
    },
    destroy: function () {
        $("li, li>s, li>div[hide]", this.element).off("click.tree");
        $("li, li>s, li>div[hide]", this.element).off("click.select");
        $.Widget.prototype.destroy.call(this);
    }
});

$(function () {
    $("ul[wliu][tab9]").tab9();
    $("div[wliu][block]").block();
    $("div[wliu][loading]").loading();
    $("div[wliu][hint]").hint();
    $("div[wliu][float]").float();
    $("div[wliu][tooltip]").tooltip();
    $("div[wliu][diag]").diag();
    $("div[wliu][flip]").flip();
    $("ul[wliu][tree][root]").tree();
});

/*** jquery plugin ***/
/*** ToolTip ***/
$.fn.extend({
    WliuToolTip: function (opts) {
        //placement: left right up down;
        var def_settings = {
            //placement:      ""  // left right up down, 
        };
        $.extend(def_settings, opts);

        /*** begin return ***/
        return this.each(function (idx, el) {
            $(el).appendTo("body");
            if (!$(el).hasAttr("wliu-tooltip")) $(el).addAttr("wliu-tooltip");
            if ($(el).has("s[arrow][left]").length <= 0) $(el).prepend('<s arrow left></s>');
            if ($(el).has("s[arrow][right]").length <= 0) $(el).prepend('<s arrow right></s>');
            if ($(el).has("s[arrow][up]").length <= 0) $(el).prepend('<s arrow up></s>');
            if ($(el).has("s[arrow][head-up]").length <= 0) $(el).prepend('<s arrow head-up></s>');
            if ($(el).has("s[arrow][down]").length <= 0) $(el).prepend('<s arrow down></s>');

            if ($(el).has("div[wliu-tooltip-body]").length <= 0) $(el).append('<div wliu-tooltip-body></div>');
        });
        /*** end return ***/
    }
});


$(function () {
    /*** popup ***/
    $("div[wliu-tooltip]").WliuToolTip({});

    $(document).off("click", "*[tooltip-toggle='click']").on("click", "*[tooltip-toggle='click']", function (evt) {
        //var target_el = $(this).attr("tooltip-target");
        var target_el = "div[wliu-tooltip][tooltip-target='" + $(this).attr("tooltip-target") + "']";
        var target_content = $(this).attr("tooltip-body");

        if ($(target_el).is(":hidden")) {
            /********************************************************************************** */
            if (target_content) {
                var target_pl = $(this).attr("tooltip-placement");
                var target_tt = $(this).attr("tooltip-head");

                if (target_tt) {
                    if ($(target_el).has("div[wliu-tooltip-head]").length <= 0)
                        $(target_el).prepend('<div wliu-tooltip-head>' + target_tt + '</div>');
                    else
                        $("div[wliu-tooltip-head]", $(target_el)).html(target_tt);
                } else {
                    $("div[wliu-tooltip-head]", $(target_el)).remove();
                }

                $("div[wliu-tooltip-body]", $(target_el)).html(target_content);
                $(target_el).addAttr("active");
                var nleft = -900;
                var ntop = -900;

                switch (("" + target_pl).toLowerCase()) {
                    case "left":
                        $("s[arrow]", $(target_el)).removeAttr("active");
                        $("s[arrow][left]", $(target_el)).addAttr("active");

                        nleft = $(this).offset().left + $(this).outerWidth() + 12;
                        ntop = $(this).offset().top + $(this).outerHeight() / 2 - $(target_el).outerHeight() / 2;
                        break;
                    case "right":
                        $("s[arrow]", $(target_el)).removeAttr("active");
                        $("s[arrow][right]", $(target_el)).addAttr("active");
                        nleft = $(this).offset().left - $(target_el).outerWidth() - 12;
                        ntop = $(this).offset().top + $(this).outerHeight() / 2 - $(target_el).outerHeight() / 2;
                        break;
                    case "up":
                        $("s[arrow]", $(target_el)).removeAttr("active");
                        if ($(target_el).has("div[wliu-tooltip-head]").length <= 0)
                            $("s[arrow][up]", $(target_el)).addAttr("active");
                        else
                            $("s[arrow][head-up]", $(target_el)).addAttr("active");

                        nleft = $(this).offset().left + ($(this).outerWidth() - $(target_el).outerWidth()) / 2;
                        ntop = $(this).offset().top + $(this).outerHeight() + 12;
                        break;
                    case "down":
                        $("s[arrow]", $(target_el)).removeAttr("active");
                        $("s[arrow][down]", $(target_el)).addAttr("active");
                        nleft = $(this).offset().left + ($(this).outerWidth() - $(target_el).outerWidth()) / 2;
                        ntop = $(this).offset().top - $(target_el).outerHeight() - 12;
                        break;
                    default:
                        $("s[arrow]", $(target_el)).removeAttr("active");
                        var nplace = "left";
                        nleft = $(this).offset().left + $(this).outerWidth() + 12;
                        ntop = $(this).offset().top + $(this).outerHeight() / 2 - $(target_el).outerHeight() / 2;
                        var cleft = $(this).offset().left + $(this).outerWidth() + $(target_el).outerWidth() + 12;

                        if (ntop > 0) {
                            // place to left first, then right
                            if (cleft <= $(window).scrollLeft() + window.innerWidth) {
                                nplace = "left";
                                $("s[arrow][left]", $(target_el)).addAttr("active");
                                nleft = $(this).offset().left + $(this).outerWidth() + 12;
                                ntop = $(this).offset().top + $(this).outerHeight() / 2 - $(target_el).outerHeight() / 2;
                            } else {
                                cleft = $(this).offset().left - $(target_el).outerWidth() - 12;
                                if (cleft >= 0) {
                                    nplace = "right";
                                    $("s[arrow][right]", $(target_el)).addAttr("active");
                                    nleft = $(this).offset().left - $(target_el).outerWidth() - 12;
                                    ntop = $(this).offset().top + $(this).outerHeight() / 2 - $(target_el).outerHeight() / 2;
                                } else {
                                    nplace = "up";
                                    if ($(target_el).has("div[wliu-tooltip-head]").length <= 0)
                                        $("s[arrow][up]", $(target_el)).addAttr("active");
                                    else
                                        $("s[arrow][head-up]", $(target_el)).addAttr("active");

                                    nleft = $(this).offset().left + ($(this).outerWidth() - $(target_el).outerWidth()) / 2;
                                    ntop = $(this).offset().top + $(this).outerHeight() + 12;
                                }
                            }

                        } else {
                            nplace = "up";
                            if ($(target_el).has("div[wliu-tooltip-head]").length <= 0)
                                $("s[arrow][up]", $(target_el)).addAttr("active");
                            else
                                $("s[arrow][head-up]", $(target_el)).addAttr("active");

                            nleft = $(this).offset().left + ($(this).outerWidth() - $(target_el).outerWidth()) / 2;
                            ntop = $(this).offset().top + $(this).outerHeight() + 12;
                        }
                        break;
                }

                $(target_el).offset({ left: nleft, top: ntop });
            } else {
                $("div[wliu-tooltip-head]", $(target_el)).remove();
                $("div[wliu-tooltip-body]", $(target_el)).empty();
            }
            /********************************************************************************** */
        } else {
            $(target_el).removeAttr("active");
            $("div[wliu-tooltip-head]", $(target_el)).remove();
            $("div[wliu-tooltip-body]", $(target_el)).empty();
        }
    });


    $(document).off("focus", "*[tooltip-toggle='focus']").on("focus", "*[tooltip-toggle='focus']", function (evt) {
        //var target_el = $(this).attr("tooltip-target");
        var target_el = "div[wliu-tooltip][tooltip-target='" + $(this).attr("tooltip-target") + "']";
        var target_content = $(this).attr("tooltip-body");

        if ($(target_el).is(":hidden")) {
            /********************************************************************************** */
            if (target_content) {
                var target_pl = $(this).attr("tooltip-placement");
                var target_tt = $(this).attr("tooltip-head");

                if (target_tt) {
                    if ($(target_el).has("div[wliu-tooltip-head]").length <= 0)
                        $(target_el).prepend('<div wliu-tooltip-head>' + target_tt + '</div>');
                    else
                        $("div[wliu-tooltip-head]", $(target_el)).html(target_tt);
                } else {
                    $("div[wliu-tooltip-head]", $(target_el)).remove();
                }

                $("div[wliu-tooltip-body]", $(target_el)).html(target_content);
                $(target_el).addAttr("active");
                var nleft = -900;
                var ntop = -900;

                switch (("" + target_pl).toLowerCase()) {
                    case "left":
                        $("s[arrow]", $(target_el)).removeAttr("active");
                        $("s[arrow][left]", $(target_el)).addAttr("active");

                        nleft = $(this).offset().left + $(this).outerWidth() + 12;
                        ntop = $(this).offset().top + $(this).outerHeight() / 2 - $(target_el).outerHeight() / 2;
                        break;
                    case "right":
                        $("s[arrow]", $(target_el)).removeAttr("active");
                        $("s[arrow][right]", $(target_el)).addAttr("active");
                        nleft = $(this).offset().left - $(target_el).outerWidth() - 12;
                        ntop = $(this).offset().top + $(this).outerHeight() / 2 - $(target_el).outerHeight() / 2;
                        break;
                    case "up":
                        $("s[arrow]", $(target_el)).removeAttr("active");
                        if ($(target_el).has("div[wliu-tooltip-head]").length <= 0)
                            $("s[arrow][up]", $(target_el)).addAttr("active");
                        else
                            $("s[arrow][head-up]", $(target_el)).addAttr("active");

                        nleft = $(this).offset().left + ($(this).outerWidth() - $(target_el).outerWidth()) / 2;
                        ntop = $(this).offset().top + $(this).outerHeight() + 12;
                        break;
                    case "down":
                        $("s[arrow]", $(target_el)).removeAttr("active");
                        $("s[arrow][down]", $(target_el)).addAttr("active");
                        nleft = $(this).offset().left + ($(this).outerWidth() - $(target_el).outerWidth()) / 2;
                        ntop = $(this).offset().top - $(target_el).outerHeight() - 12;
                        break;
                    default:
                        $("s[arrow]", $(target_el)).removeAttr("active");
                        var nplace = "left";
                        nleft = $(this).offset().left + $(this).outerWidth() + 12;
                        ntop = $(this).offset().top + $(this).outerHeight() / 2 - $(target_el).outerHeight() / 2;
                        var cleft = $(this).offset().left + $(this).outerWidth() + $(target_el).outerWidth() + 12;

                        if (ntop > 0) {
                            // place to left first, then right
                            if (cleft <= $(window).scrollLeft() + window.innerWidth) {
                                nplace = "left";
                                $("s[arrow][left]", $(target_el)).addAttr("active");
                                nleft = $(this).offset().left + $(this).outerWidth() + 12;
                                ntop = $(this).offset().top + $(this).outerHeight() / 2 - $(target_el).outerHeight() / 2;
                            } else {
                                cleft = $(this).offset().left - $(target_el).outerWidth() - 12;
                                if (cleft >= 0) {
                                    nplace = "right";
                                    $("s[arrow][right]", $(target_el)).addAttr("active");
                                    nleft = $(this).offset().left - $(target_el).outerWidth() - 12;
                                    ntop = $(this).offset().top + $(this).outerHeight() / 2 - $(target_el).outerHeight() / 2;
                                } else {
                                    nplace = "up";
                                    if ($(target_el).has("div[wliu-tooltip-head]").length <= 0)
                                        $("s[arrow][up]", $(target_el)).addAttr("active");
                                    else
                                        $("s[arrow][head-up]", $(target_el)).addAttr("active");

                                    nleft = $(this).offset().left + ($(this).outerWidth() - $(target_el).outerWidth()) / 2;
                                    ntop = $(this).offset().top + $(this).outerHeight() + 12;
                                }
                            }

                        } else {
                            nplace = "up";
                            if ($(target_el).has("div[wliu-tooltip-head]").length <= 0)
                                $("s[arrow][up]", $(target_el)).addAttr("active");
                            else
                                $("s[arrow][head-up]", $(target_el)).addAttr("active");

                            nleft = $(this).offset().left + ($(this).outerWidth() - $(target_el).outerWidth()) / 2;
                            ntop = $(this).offset().top + $(this).outerHeight() + 12;
                        }
                        break;
                }

                $(target_el).offset({ left: nleft, top: ntop });
            } else {
                $("div[wliu-tooltip-head]", $(target_el)).remove();
                $("div[wliu-tooltip-body]", $(target_el)).empty();
            }
            /********************************************************************************** */
        } else {
            $(target_el).removeAttr("active");
            $("div[wliu-tooltip-head]", $(target_el)).remove();
            $("div[wliu-tooltip-body]", $(target_el)).empty();
        }
    });


    $(document).off("mouseover", "*[tooltip-toggle='hover']").on("mouseover", "*[tooltip-toggle='hover']", function (evt) {
        //var target_el       = $(this).attr("tooltip-target");
        var target_el = "div[wliu-tooltip][tooltip-target='" + $(this).attr("tooltip-target") + "']";
        var target_content = $(this).attr("tooltip-body");

        /*** content not empty ***/
        if (target_content) {
            var target_pl = $(this).attr("tooltip-placement");
            var target_tt = $(this).attr("tooltip-head");

            if (target_tt) {
                if ($(target_el).has("div[wliu-tooltip-head]").length <= 0)
                    $(target_el).prepend('<div wliu-tooltip-head>' + target_tt + '</div>');
                else
                    $("div[wliu-tooltip-head]", $(target_el)).html(target_tt);
            } else {
                $("div[wliu-tooltip-head]", $(target_el)).remove();
            }

            $("div[wliu-tooltip-body]", $(target_el)).html(target_content);
            $(target_el).addAttr("active");
            var nleft = -900;
            var ntop = -900;

            switch (("" + target_pl).toLowerCase()) {
                case "left":
                    $("s[arrow]", $(target_el)).removeAttr("active");
                    $("s[arrow][left]", $(target_el)).addAttr("active");

                    nleft = $(this).offset().left + $(this).outerWidth() + 12;
                    ntop = $(this).offset().top + $(this).outerHeight() / 2 - $(target_el).outerHeight() / 2;
                    break;
                case "right":
                    $("s[arrow]", $(target_el)).removeAttr("active");
                    $("s[arrow][right]", $(target_el)).addAttr("active");
                    nleft = $(this).offset().left - $(target_el).outerWidth() - 12;
                    ntop = $(this).offset().top + $(this).outerHeight() / 2 - $(target_el).outerHeight() / 2;
                    break;
                case "up":
                    $("s[arrow]", $(target_el)).removeAttr("active");
                    if ($(target_el).has("div[wliu-tooltip-head]").length <= 0)
                        $("s[arrow][up]", $(target_el)).addAttr("active");
                    else
                        $("s[arrow][head-up]", $(target_el)).addAttr("active");

                    nleft = $(this).offset().left + ($(this).outerWidth() - $(target_el).outerWidth()) / 2;
                    ntop = $(this).offset().top + $(this).outerHeight() + 12;
                    break;
                case "down":
                    $("s[arrow]", $(target_el)).removeAttr("active");
                    $("s[arrow][down]", $(target_el)).addAttr("active");
                    nleft = $(this).offset().left + ($(this).outerWidth() - $(target_el).outerWidth()) / 2;
                    ntop = $(this).offset().top - $(target_el).outerHeight() - 12;
                    break;
                default:
                    $("s[arrow]", $(target_el)).removeAttr("active");
                    var nplace = "left";
                    nleft = $(this).offset().left + $(this).outerWidth() + 12;
                    ntop = $(this).offset().top + $(this).outerHeight() / 2 - $(target_el).outerHeight() / 2;
                    var cleft = $(this).offset().left + $(this).outerWidth() + $(target_el).outerWidth() + 12;

                    if (ntop > 0) {
                        // place to left first, then right
                        if (cleft <= $(window).scrollLeft() + window.innerWidth) {
                            nplace = "left";
                            $("s[arrow][left]", $(target_el)).addAttr("active");
                            nleft = $(this).offset().left + $(this).outerWidth() + 12;
                            ntop = $(this).offset().top + $(this).outerHeight() / 2 - $(target_el).outerHeight() / 2;
                        } else {
                            cleft = $(this).offset().left - $(target_el).outerWidth() - 12;
                            if (cleft >= 0) {
                                nplace = "right";
                                $("s[arrow][right]", $(target_el)).addAttr("active");
                                nleft = $(this).offset().left - $(target_el).outerWidth() - 12;
                                ntop = $(this).offset().top + $(this).outerHeight() / 2 - $(target_el).outerHeight() / 2;
                            } else {
                                nplace = "up";
                                if ($(target_el).has("div[wliu-tooltip-head]").length <= 0)
                                    $("s[arrow][up]", $(target_el)).addAttr("active");
                                else
                                    $("s[arrow][head-up]", $(target_el)).addAttr("active");

                                nleft = $(this).offset().left + ($(this).outerWidth() - $(target_el).outerWidth()) / 2;
                                ntop = $(this).offset().top + $(this).outerHeight() + 12;
                            }
                        }

                    } else {
                        nplace = "up";
                        if ($(target_el).has("div[wliu-tooltip-head]").length <= 0)
                            $("s[arrow][up]", $(target_el)).addAttr("active");
                        else
                            $("s[arrow][head-up]", $(target_el)).addAttr("active");

                        nleft = $(this).offset().left + ($(this).outerWidth() - $(target_el).outerWidth()) / 2;
                        ntop = $(this).offset().top + $(this).outerHeight() + 12;
                    }
                    break;
            }

            $(target_el).offset({ left: nleft, top: ntop });
        } else {
            $("div[wliu-tooltip-head]", $(target_el)).remove();
            $("div[wliu-tooltip-body]", $(target_el)).empty();
        }
        /*** --content not empty ***/
    });
    $(document).off("mouseout", "*[tooltip-toggle='hover']").on("mouseout", "*[tooltip-toggle='hover']", function (evt) {
        //var target_el  = $(this).attr("tooltip-target");
        var target_el = "div[wliu-tooltip][tooltip-target='" + $(this).attr("tooltip-target") + "']";
        $(target_el).removeAttr("active");
        $("div[wliu-tooltip-head]", $(target_el)).remove();
        $("div[wliu-tooltip-body]", $(target_el)).empty();
    });
    $(document).off("mouseleft", "*[tooltip-toggle='hover']").on("mouseleft", "*[tooltip-toggle='hover']", function (evt) {
        //var target_el  = $(this).attr("tooltip-target");
        var target_el = "div[wliu-tooltip][tooltip-target='" + $(this).attr("tooltip-target") + "']";
        $(target_el).removeAttr("active");
        $("div[wliu-tooltip-head]", $(target_el)).remove();
        $("div[wliu-tooltip-body]", $(target_el)).empty();
    });
});