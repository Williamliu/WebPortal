$.widget("wliu.calendar", {
    options: {
        url:        "",
        year:       1999,
        month:      1,
        lang:       "en",
        short:      false,
        select:     false,
        start:      new Date("2000-01-01"),
        end:        new Date("2199-12-31"),
        data:       null,
        eventclick: null,
        dateclick:  null,  //Params:  evt.data, date, isActive
        change:     null   //Params:  evt.data, start, end, isActive
    },
    _today: new Date(),
    _weekdays: [],
    _calendar: null,
    _words: {
        en: {
            months:         ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"],
            months_short:   ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"],
            days:           ["Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"],
            days_short:     ["Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat"]
        },
        cn: {
            months:         ["一月", "二月", "三月", "四月", "五月", "六月", "七月", "八月", "九月", "十月", "十一月", "十二月"],
            months_short:   ["一月", "二月", "三月", "四月", "五月", "六月", "七月", "八月", "九月", "十月", "十一月", "十二月"],
            days:           ["星期日", "星期一", "星期二", "星期三", "星期四", "星期五", "星期六"],
            days_short:     ["日", "一", "二", "三", "四", "五", "六"]
        }
    },
    _create: function () {
        this.options.year   = this._today.getFullYear();
        this.options.month  = this._today.getMonth();
        this.options.short  = this.element.hasAttr("short") ? true : false;
        this.options.select = this.element.hasAttr("select") ? true : false;
    },
    _init: function () {
        this._calendar = new WLIU.Calendar();
        this._eventChange();
        this._getMonthDate();
        this._toHTML();
    },
    _setOption: function (key, val) {
        this._super(key, val);
        switch (key) {
            case "lang":
                this._toHTML();
                break;
            case "year":
            case "month":
            case "start":
            case "end":
            case "dateclick":
            case "change":
            case "data":
                this._getMonthDate();
                break;
            case "url":
                this._changeUrl();
                break;
        }
    },
    _changeUrl: function () {
        this._calendar.url = this.options.url;
    },
    destroy: function () {
        $.Widget.prototype.destroy.call(this);
    },

    // navigate calendar
    _prev: function () {
        this.options.month--;
        this._getMonthDate();
        this._toHTML();
    },

    _next: function () {
        this.options.month++;
        this._getMonthDate();
        this._toHTML();
    },

    _current: function () {
        this._today         = new Date();
        this.options.year   = this._today.getFullYear();
        this.options.month  = this._today.getMonth();
        this._getMonthDate();
        this._toHTML();
    },
    _change: function (evt) {
        if (this.options.change)
            if ($.isFunction(this.options.change)) {
                evt.data = this.options.data;
                let dd = this._eventChange();
                this._trigger("change", evt, [dd.start, dd.end, dd.isActive]);
            }
    },
    _eventBinding: function () {
        let self = this;
        $("a.previousMonth", this.element).off("click.calendar").on("click.calendar", function (evt) {
            self._prev();
            self._change(evt);
        });
        $("a.nextMonth", this.element).off("click.calendar").on("click.calendar", function (evt) {
            self._next();
            self._change(evt);
        });
        $("a.todayMonth", this.element).off("click.calendar").on("click.calendar", function (evt) {
            self._current();
            self._change(evt);
        });
        $("td.date", this.element).off("click.calendar").on("click.calendar", function (evt) {
            if ($(this).hasClass("date-na") === false) {
                if (self.options.dateclick)
                    if ($.isFunction(self.options.dateclick)) {
                        if (self.options.select) {
                            $("td.date", this.element).removeClass("selected");
                            $(this).addClass("selected");
                        }
                        let dt = parseInt($(this).attr("yy")) + "-" + (parseInt($(this).attr("mm")) + 1) + "-" + parseInt($(this).attr("dd"));
                        let isActive = $(this).hasClass("date-na") ? false : true;
                        evt.data = self.options.data;
                        self._trigger("dateclick", evt, [dt, isActive]);
                    }
            }
        });
    },
    _eventChange: function () {
        let self = this;
        let dateObj = {};
        dateObj.start = null;
        dateObj.end = null;
        dateObj.isActive = false;

        let isActive = true;
        var fday = this._getWDay(this.options.year, this.options.month, 1);
        var ldate = this._getLastDate(this.options.year, this.options.month);
        var row_num = Math.ceil((fday + ldate) / 7);
        let mstart = this._toDate(this.options.year, this.options.month, 1 - fday);
        let mend = this._toDate(this.options.year, this.options.month, row_num * 7 - fday);
        let start = mstart.ymdString();
        let end = mend.ymdString();

        if (this.options.start <= mstart)
            start = mstart.ymdString();
        else if (this.options.start > mstart && this.options.start <= mend)
            start = this.options.start.ymdString();
        else if (this.options.start > mend) {
            isActive = false;
            start = null;
        }

        if (isActive) {
            if (this.options.end >= mend)
                end = mend.ymdString();
            else if (this.options.end < mend && this.options.end >= mstart)
                end = this.options.end.ymdString();
            else if (this.options.end < mstart) {
                isActive = false;
                end = null;
                start = null;
            }
        }
        else {
            start = null;
            end = null;
        }
        dateObj.isActive = isActive;
        dateObj.start = start;
        dateObj.end = end;

        this._calendar.url = this.options.url;
        this._calendar.start = dateObj.start;
        this._calendar.end = dateObj.end;
        this._calendar.isActive = dateObj.isActive;
        this._calendar.data = this.options.data;
        this._calendar.error = new WLIU.Error();
        this._calendar.Get().then(d => {
            $("td.date>div.date-event>li.date-item", self.element).off("click.calendar");
            $("td.date>div.date-event", self.element).empty();
            for (let idx in d.events) {
                let evtObj = d.events[idx];
                if (evtObj) {
                    let timestr = evtObj.from ? evtObj.from.hhmm() : "";
                    timestr += (timestr ? "~" : "") + evtObj.to.hhmm();
                    let time = `<span style="color:blue;">${timestr}</span>`;

                    let ctooltip = [
                        'tooltip-toggle="hover" ',
                        `tooltip-body="${timestr} ${evtObj.title}" `,
                        'tooltip-target="wliuToolTip" ',
                        'tooltip-placement="down" '
                    ].join('');

                    let dateStatus = 0;
                    if (evtObj.status) {
                        dateStatus = evtObj.state;
                    }

                    let dateItem_html = `<li class="date-item status-${dateStatus}" eid="${evtObj.id}" ${ctooltip}>${time} ${evtObj.title}</li>`;
                    $(`td.date>div.date-event[yy=${evtObj.yy}][mm=${evtObj.mm}][dd=${evtObj.dd}]`, this.element).append(dateItem_html);
                }
            }
            $("td.date>div.date-event>li.date-item", self.element).on("click.calendar", function (evt) {
                if (self.options.eventclick)
                    if ($.isFunction(self.options.eventclick)) {
                        evt.data = self.options.data;
                        self._trigger("eventclick", evt, $(this).attr("eid"));
                    }

            });

        }).catch(e => {
            $("td.date>div.date-event").empty();
        });
        return dateObj;
    },
    // convert date 
    _toYMD: function (dt) {
        var tmpObj = {};
        tmpObj.year = dt.getFullYear();
        tmpObj.month = dt.getMonth();
        tmpObj.date = dt.getDate();
        return tmpObj;
    },

    _toDate: function (yyyy, mm, dd) {
        var tmp = new Date(yyyy, mm, dd);
        return tmp;
    },

    _getWDay: function (yyyy, mm, dd) {
        var tmpD = new Date(yyyy, mm, dd);
        return tmpD.getDay();
    },

    _getLastDate: function (yyyy, mm) {
        var last_date = new Date(yyyy, mm + 1, 0);
        return last_date.getDate();
    },

    // Create weekdays array
    _getMonthDate: function () {
        var startDate = this.options.start ? this.options.start.ymd() : new Date("2000-01-01");
        var endDate = this.options.end ? this.options.end.ymd(): new Date("2199-12-31");
        // recalculate  year and month
        var cur_dd0 = this._toDate(this.options.year, this.options.month, 1);
        var cur_dd1 = this._toYMD(cur_dd0);
        this.options.year = cur_dd1.year;
        this.options.month = cur_dd1.month;
        //end of recalculate year and month

        var fday = this._getWDay(this.options.year, this.options.month, 1);
        var ldate = this._getLastDate(this.options.year, this.options.month);
        var row_num = Math.ceil((fday + ldate) / 7);

        // Weeks in month;
        this._weekdays = new Array(row_num);
        for (let i = 0; i < row_num; i++) {
            this._weekdays[i] = new Array(7);
        }
        // Weeks in month;

        for (let i = 0; i < row_num; i++) {
            for (let j = 0; j < 7; j++) {
                var cell_day = (i * 7 + j) - fday + 1;  // important formular for locate the date to calendar cell;
                var cell_date = this._toDate(this.options.year, this.options.month, cell_day);
                var cell_ymd = this._toYMD(cell_date);

                var cell_obj = {};
                cell_obj.year   = cell_ymd.year;
                cell_obj.month  = cell_ymd.month;
                cell_obj.date   = cell_ymd.date;
                cell_obj.status = (cell_ymd.month === this.options.month && cell_ymd.date >= 1 && cell_ymd.date <= ldate) ? 1 : 0;
                cell_obj.na = (cell_date >= startDate && cell_date <= endDate) ? 0 : 1;
                this._weekdays[i][j] = cell_obj;
            }
        }
    },
    _toHTML: function () {
        var html = '';
        html += '<table class="lwhCalendar-table" style="width:100%;">';
        html += '<tr>';
        html += '<td class="subject" colspan="7" style="background-color:#1b93b6; border:1px solid #1b93b6;" align="center" valign="middle">';
        if (this.options.short) {
            html += '<div style="display:inline-block;font-size:20px;vertical-align:middle;color:#ffffff;margin-right:6px;">' + this.options.year + '</div>';
            html += '<a wliu btn24 nav-left class="previousMonth" title="Previous Month"></a>';
            html += '<div style="display:inline-block;font-size:20px;vertical-align:middle;color:#ffffff; min-width:110px;padding:0px 2px;">' + this._words[this.options.lang].months[this.options.month] + '</div>';
            html += '<a wliu btn24 nav-right class="nextMonth" title="Next Month"></a>';
            html += '<a wliu btn24 today class="todayMonth" style="margin-left:6px;" title="Today"></a>';
        }
        else
        {
            html += '<div style="display:inline-block;font-size:24px;vertical-align:middle;color:#ffffff;margin-right:20px;">' + this.options.year + '</div>';
            html += '<a wliu btn24 nav-left class="previousMonth" title="Previous Month"></a>';
            html += '<div style="display:inline-block;font-size:24px;vertical-align:middle;color:#ffffff; min-width:160px;padding:0px 2px;">' + this._words[this.options.lang].months[this.options.month] + '</div>';
            html += '<a wliu btn24 nav-right class="nextMonth" title="Next Month"></a>';
            html += '<a wliu btn24 today class="todayMonth" style="margin-left:20px;" title="Today"></a>';
        }
        html += '</td >';
        html += '</tr>';

        html += '<tr>';
        html += '<td colspan="7" style="background-color:#003c54; border:1px solid #003c54; height:5px;"></td>';
        html += '</tr>';

        html += '<tr >';
        html += '<td class="title" width="14.25%">' + (this.options.short ? this._words[this.options.lang].days_short[0] : this._words[this.options.lang].days[0]) + '</td>';
        html += '<td class="title" width="14.25%">' + (this.options.short ? this._words[this.options.lang].days_short[1] : this._words[this.options.lang].days[1]) + '</td>';
        html += '<td class="title" width="14.25%">' + (this.options.short ? this._words[this.options.lang].days_short[2] : this._words[this.options.lang].days[2]) + '</td>';
        html += '<td class="title" width="14.25%">' + (this.options.short ? this._words[this.options.lang].days_short[3] : this._words[this.options.lang].days[3]) + '</td>';
        html += '<td class="title" width="14.25%">' + (this.options.short ? this._words[this.options.lang].days_short[4] : this._words[this.options.lang].days[4]) + '</td>';
        html += '<td class="title" width="14.25%">' + (this.options.short ? this._words[this.options.lang].days_short[5] : this._words[this.options.lang].days[5]) + '</td>';
        html += '<td class="title" width="14.25%">' + (this.options.short ? this._words[this.options.lang].days_short[6] : this._words[this.options.lang].days[6]) + '</td>';
        html += '</tr>';

        for (var i = 0; i < this._weekdays.length; i++) {
            html += '<tr>';
            for (var j = 0; j < 7; j++) {
                let today_css   = this._weekdays[i][j].year === this._today.getFullYear() && this._weekdays[i][j].month === this._today.getMonth() && this._weekdays[i][j].date === this._today.getDate()?' today':'';
                let date_na     = this._weekdays[i][j].na?' date-na':'';
               
                
                html += '<td class="date' + today_css + date_na + '"  yy="' + this._weekdays[i][j].year + '" mm="' + this._weekdays[i][j].month + '" dd="' + this._weekdays[i][j].date + '" valign="top" width="14.25%">';
                html += '<div class="datedigi' + (this._weekdays[i][j].status ? "" : " datedigi-status") + (this._weekdays[i][j].na? " datedigi-na" : "") + '">';
                html += this._weekdays[i][j].date;
                html += this.options.short?'':' <sup style="font-size:10px;font-weight:500;text-decoration:none;">' + this._words[this.options.lang].months_short[this._weekdays[i][j].month] + '</sup>';
                html += '</div>';
                html += '<div class="date-event" yy="' + this._weekdays[i][j].year + '" mm="' + this._weekdays[i][j].month + '" dd="' + this._weekdays[i][j].date + '">';
                html += '</div>';
                html += '</div></td>';
            }
            html += '</tr>';
        }

        html += '<tr>';
        html += '<td colspan="7" style="background-color:#003c54; border:1px solid #003c54; height:5px;"></td>';
        html += '</tr>';

        html += '</table>';
        html += '</div>';
        this.element.empty().html(html);
        this._eventBinding();
    }
});

var WLIU = WLIU || {};
WLIU.Calendar = function (calendar) {
    if ($.isPlainObject(calendar)) {
        this.start      = calendar.start || null;
        this.end        = calendar.end || null;
        this.isActive   = calendar.isActive || false;
        this.data       = calendar.data || {};
        this.error      = new WLIU.Error(calendar.error);
        this.url        = calendar.url || ""; 
        this.AddOEvents(calendar.events);
    }
    else {
        this.start      = null;
        this.end        = null;
        this.isActive   = false;
        this.data       = {};
        this.events     = [];
        this.error      = new WLIU.Error();
        this.url        = "";
    }
};
WLIU.Calendar.prototype = {
    AddOEvent: function (event) {
        this.events.push(new WLIU.Event(event));
        return this;
    },
    AddOEvents: function (events) {
        this.events = [];
        if ($.isArray(events)) {
            for (let evtIdx in events) {
                this.AddOEvent(events[evtIdx]);
            }
        }
        return this;
    },
    Get: function () {
        this.events = [];
        if (this.url !== "" && this.isActive) {
            var self        = this;
            var ncal        = {};
            ncal.isActive   = this.isActive;
            ncal.start      = this.start;
            ncal.end        = this.end;
            ncal.data       = this.data;
            console.log(ncal);
            let defer = $.Deferred();
            AJAX.Post(self.url, ncal).then(data => {
                self.error = new WLIU.Error(data.error);
                self.isActive = data.isActive;
                if (self.isActive) {
                    self.start = new Date(data.start);
                    self.end = new Date(data.end);
                    self.AddOEvents(data.events);
                }
                self.DBAjaxErrorHandle();
                defer.resolve(self);
            }).catch(data => {
                self.DBAjaxErrorHandle();
                defer.reject(self);
            });
            return defer.promise();
        }
        let defer = $.Deferred();
        defer.resolve({});
        return defer.promise();
    },
    DBAjaxErrorHandle: function () {
        var self = this;
        if (self.error.HasError()) {
            $("#wliuDiagTableError").diag({
                close: function () {
                    if (self.error.code >= 9999) {
                        if (self.error.memo !== "") window.location.href = self.error.memo;
                    }
                },
                content: [
                    '<center>' + Words("error.remind") + '</center>',
                    '<span style="color:orange;">' + Words("error.code") + '</span> : <span style="color:orange;">' + self.error.code + '</span><br>',
                    self.error.Message().nl2br()
                ].join('')
            }).diag("show");
        }
    }
}
WLIU.Event = function (event) {
    if ($.isPlainObject(event)) {
        this.id = event.id || 0;
        this.date = event.date || null;
        this.yy = event.yy || "";
        this.mm = event.mm || "";
        this.dd = event.dd || "";
        this.from = event.from || "";
        this.to = event.to || "";
        this.status = event.status || 0;
        this.state = event.state || 0;
        this.title = event.title || "";
        this.detail = event.detail || "";
    }
    else {
        this.id = 0;
        this.date = null;
        this.yy = "";
        this.mm = "";
        this.dd = "";
        this.from = "";
        this.to = "";
        this.status = 0;
        this.state = 0;
        this.title = "";
        this.detail = "";
    }
};

$(function () {
    $("div[wliu][calendar]").calendar();
});