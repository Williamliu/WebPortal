/************************************************************************************/
/*  JQuery Plugin Calendar	                     		                        	*/
/*  Author:	William Liu                                                            	*/
/*  Date: 	2013-1-25      															*/
/*  Files: 	jquery.lwh.calendar.js ;  jquery.lwh.calendar.css						*/
/************************************************************************************/

LWH.CALENDAR  = function( opts ) {
	$.extend(this.settings, opts);
	var _self = this;
	this.admin_sess = this.settings.admin_sess;
	this.admin_menu = this.settings.admin_menu;
	this.admin_oper = this.settings.admin_oper;
	this.site		= this.settings.site;
	//internal  method	
	var _constructor = function() {
		$(".lwhCalendar-button-nav-prev", _self.settings.container).die("click.calendar").live("click.calendar", function(ev){
			_self.prev();
		});
		$(".lwhCalendar-button-nav-next", _self.settings.container).die("click.calendar").live("click.calendar", function(ev){
			_self.next();
		});
		$(".lwhCalendar-button-nav-today", _self.settings.container).die("click.calendar").live("click.calendar", function(ev){
			_self.current();
		});
		$(".lwhCalendar-year", _self.settings.container).die("change.calendar").live("change.calendar", function(ev){
			_self.current();
		});

		
		var today = new Date();
		_self.today = _self.toYMD(today);
	};
	_constructor();
}

LWH.CALENDAR.prototype = {
	today:			{},
	site:			1,
	curYY:			1970,
	curMM:			0,
	evtList:		[],
	settings: 		{ 
						admin_sess:  	"",
						admin_menu:  	"",
						admin_oper:  	"",
						
						container:		"",
						site:			1,
						monthChange:	null,
						dateClick:		null
					},
	month_desc: 	["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"],
	month_short: 	["Jan", "Feb", "Mar","Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"],
	day_desc: 		["Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"],
	day_short: 		["Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat"],
	
	weekdays:		[],
	
	getMonthDate: function() {
		// recalculate  year and month
		var cur_dd0	= this.toDate(this.curYY, this.curMM,1);
		var cur_dd1	= this.toYMD(cur_dd0);
		this.curYY 	= cur_dd1.year;
		this.curMM	= cur_dd1.month;
		//end of recalculate year and month
		
		var fday 	= this.getWDay(this.curYY,this.curMM,1);
		var ldate 	= this.getLastDate(this.curYY, this.curMM);
		var row_num = Math.ceil((fday + ldate)/7);
		//alert("Days:" + ldate + "  day:" + this.day_desc[fday] + "  rows:" + row_num);
		
		// rest weekdays  array  for each time;
		this.weekdays = new Array(row_num);
		for(var i=0; i<row_num; i++) {
			this.weekdays[i] = new Array(7);
		}
		// end of reset;
		
		for(var i=0; i<row_num; i++) {
			for(var j=0; j<7; j++) {
				var cell_day 	= (i * 7 + j) - fday + 1;  // important formular for locate the date to calendar cell;
				var cell_date 	= this.toDate(this.curYY, this.curMM, cell_day);
				var cell_ymd	= this.toYMD(cell_date);
				
				var cell_obj 		= {};
				cell_obj.year		= cell_ymd.year;
				cell_obj.month		= cell_ymd.month;
				cell_obj.date		= cell_ymd.date;
				cell_obj.status		= (cell_ymd.month==this.curMM && cell_ymd.date>=1 && cell_ymd.date<=ldate)?1:0;  
				this.weekdays[i][j] = cell_obj;
			}
		}
		
		this.toHTML();
	},
	// navigate calendar
	prev: function() {
		this.curMM--;
		this.getMonthDate();
		this.month_change();
		if(this.settings.monthChange && $.isFunction(this.settings.monthChange)) this.settings.monthChange(this.curYY, this.curMM);
	},

	next: function() {
		this.curMM++;
		this.getMonthDate();
		this.month_change();
		if(this.settings.monthChange && $.isFunction(this.settings.monthChange)) this.settings.monthChange(this.curYY, this.curMM);
	},
	
	current: function() {
		this.curYY = this.today.year;
		this.curMM	=this.today.month;
		this.getMonthDate();
		this.month_change();
		if(this.settings.monthChange && $.isFunction(this.settings.monthChange)) this.settings.monthChange(this.curYY, this.curMM);
	},
	
	fresh: function() {
		this.getMonthDate();
		this.month_change();
		if(this.settings.monthChange && $.isFunction(this.settings.monthChange)) this.settings.monthChange(this.curYY, this.curMM);
	},
	
	toYMD: function(dt) {
		var tmpObj = {};
		tmpObj.year		= dt.getFullYear();
		tmpObj.month 	= dt.getMonth();
		tmpObj.date		= dt.getDate();
		return tmpObj;		
	},
	
	toDate: function(yyyy, mm, dd) {
		var tmp = new Date(yyyy,mm,dd);
		return tmp;
	},
	
	getWDay: function(yyyy,mm,dd) {
		var tmpD = new Date(yyyy,mm,dd);
		return tmpD.getDay()
	},
	
	getLastDate: function(yyyy,mm) {
		var last_date = new Date(yyyy,mm + 1, 0);
		return last_date.getDate();
	},
	
	date_add_html: function(sdate, edate, wkd) {
		var diff = edate.diff(sdate);
		var dddd = sdate.getDate();
		var sddd = this.toYMD(sdate);
		for(var i=0; i<= diff; i++) {
			var html = '';
			var nddd = new Date(sddd.year, sddd.month, sddd.date + i);
			var tObj = this.toYMD(nddd);
			if(wkd && $.inArray(wkd)) {
				if( wkd.indexOf(this.getWDay(tObj.year,tObj.month,tObj.date)) < 0 ) continue;
			}
			if( $("li[yy='" + tObj.year  + "'][mm='" + tObj.month  + "'][dd='" + tObj.date  + "']", "#cal_event_list").length <= 0 ) {
				html += '<li class="date-area" yy="' + tObj.year + '" mm="' + tObj.month + '" dd="' + tObj.date + '" style="margin-left:20px; width:400px;">';
				html += '<table border="0">';
				html += '<tr>';
				html += '<td valign="top" style="padding:0px 5px 0px 5px; width:60px;">';
				html += 'Date: ';
				html += '</td style="width:300px;">';
				html += '<td align="left" valign="top"  style="padding:0px 5px 0px 5px; font-size:12px;  text-transform:none; font-weight:bold;">';
				html += tObj.date + ' ' + this.month_short[tObj.month] + ', ' + tObj.year + '  ' + this.day_desc[this.getWDay(tObj.year,tObj.month,tObj.date)];
				html += '<input type="button" class="date-btn-clear" yy="' + tObj.year + '" mm="' + tObj.month + '" dd="' + tObj.date + '" style="float:right;" value="Delete" />';
				html += '</td>';
				html += '</tr><tr>';
				html += '<td style="padding:0px 5px 0px 5px; width:60px;">';
				html += 'Time: ';
				html += '</td>';
				html += '<td valign="top" style="padding:0px 5px 0px 5px;">';
				html += 'From: ';
				html += this.hour_html(tObj.year, tObj.month, tObj.date, "cal-start-time");
				html += ' To: ';
				html += this.hour_html(tObj.year, tObj.month, tObj.date, "cal-end-time");
				
				html += '</td>';
				html += '</tr><tr>';
				html += '<td valign="top" style="padding:0px 5px 0px 5px; width:60px;">';
				html += 'Subject: ';
				html += '</td>';
				html += '<td valign="top" style="padding:0px 5px 0px 5px; width:300px;">';
				html += '<input class="date-title" yy="' + tObj.year + '" mm="' + tObj.month + '" dd="' + tObj.date + '" type="text" value="" />';
				html += '</td>';
				html += '</tr><tr>';
				html += '<td valign="top" style="padding:0px 5px 0px 5px; width:60px;">';
				html += 'Description: ';
				html += '</td>';
				html += '<td valign="top" style="padding:0px 5px 0px 5px;">';
				html += '<textarea class="date-desc" yy="' + tObj.year + '" mm="' + tObj.month + '" dd="' + tObj.date + '"></textarea>';
				html += '</td>';
				html += '</tr><tr>';
				html += '<td style="padding:0px 5px 0px 5px; width:60px;">';
				html += '</td>';
				html += '<td valign="top" style="padding:0px 5px 0px 5px;">';
				html += '<div class="cal-time-list" yy="' + tObj.year + '" mm="' + tObj.month + '" dd="' + tObj.date + '"></div>';
				html += '</td>';

				html += '</tr>';
				html += '</table>';
				html += '</li>';
				$("#cal_event_list").append(html);
				$("input.date-title[yy='" + tObj.year + "'][mm='" + tObj.month + "'][dd='" + tObj.date + "']").val($("#cal_event_subject").val());
				$("textarea.date-desc[yy='" + tObj.year + "'][mm='" + tObj.month + "'][dd='" + tObj.date + "']").val($("#cal_event_desc").val());
				
			}
		}
	},

	toHTML: function() {
		var html = '';
		html += '<table class="lwhCalendar-table">';
		html += '<tr>';
		html += '<td class="subject" colspan="7" valign="middle"><div style="position:relative;">';
		html += '<a class="lwhCalendar-button lwhCalendar-button-nav lwhCalendar-button-nav-prev" title="Previous Month"></a>';
		html += this.month_desc[this.curMM] + ', ' + this.curYY;
		html += '<a class="lwhCalendar-button lwhCalendar-button-nav lwhCalendar-button-nav-today" title="Current Month"></a>';
		html += '<a class="lwhCalendar-button lwhCalendar-button-nav lwhCalendar-button-nav-next" title="Next Month"></a>';
		html += '</div></td>';
		html += '</tr>';

		html += '<tr>';
		html += '<td colspan="7" style="background-color:#003c54; height:5px;"></td>';
		html += '</tr>';

		html += '<tr >';
		html += '<td class="title" width="200">Sunday</td>';
		html += '<td class="title" width="200">Monday</td>';
		html += '<td class="title" width="200">Tuesday</td>';
		html += '<td class="title" width="200">Wednesday</td>';
		html += '<td class="title" width="200">Thursday</td>';
		html += '<td class="title" width="200">Friday</td>';
		html += '<td class="title" width="200">Saturday</td>';
		html += '</tr>';

		for(var i=0; i<this.weekdays.length; i++) {
			html += '<tr>';
			for(var j=0;j<7;j++) {
				var today_css = '';
				if( this.weekdays[i][j].year == this.today.year && this.weekdays[i][j].month == this.today.month && this.weekdays[i][j].date == this.today.date ) {
					today_css = ' today';
				}
				html += '<td class="'+ today_css +'" valign="top"><div class="date">';
				html += '<div class="datedigi ' + (this.weekdays[i][j].status?"":"datedigi_na")  +  '">';
				//html += (this.weekdays[i][j].status?"":this.month_short[this.weekdays[i][j].month] + ' ') + this.weekdays[i][j].date;
                html += '<span style="font-size:10px;">' + this.month_short[this.weekdays[i][j].month] + '</span> ' + this.weekdays[i][j].date;
				html += '</div><br>';
				html += '<div class="date-event" yy="' + this.weekdays[i][j].year + '" mm="' + this.weekdays[i][j].month + '" dd="' +  this.weekdays[i][j].date + '">';
				html += '</div>';
				html += '</div></td>';
			}
			html += '</tr>';
		}

		html += '<tr>';
		html += '<td colspan="7" style="background-color:#003c54; height:5px;"></td>';
		html += '</tr>';

		html += '</table>';
		html += '</div>';
		
		$(this.settings.container).html(html);
	},
	toJSON: function() {
		var _self = this;
		var evtObj = {};
		evtObj.title 			= $("input#cal_event_subject").val();
		evtObj.description 		= $("textarea#cal_event_desc").val();
		evtObj.agreement 		= $("select#agreement").val();
		evtObj.start_date		= $("input#cal_start_date").val();
		evtObj.end_date			= $("input#cal_end_date").val();
		evtObj.dates			= [];
		$("li.date-area[yy][mm][dd]").each(function(idx1, el1) {
			var yy = $(this).attr("yy");
			var mm = $(this).attr("mm");
			var dd = $(this).attr("dd");
			
			var dateObj = {};
			dateObj.yy 			= yy;
			dateObj.mm 			= mm;
			dateObj.dd 			= dd;
			dateObj.event_date 	= yy + "-" + (parseInt(mm) + 1) + "-" + dd;
			dateObj.start_time 	= _self.hour_val(yy,mm,dd,"cal-start-time"); 
			dateObj.end_time 	= _self.hour_val(yy,mm,dd,"cal-end-time"); 
			dateObj.title 		= $("input.date-title[yy='" + yy + "'][mm='" + mm + "'][dd='" + dd + "']").val();
			dateObj.description = $("textarea.date-desc[yy='" + yy + "'][mm='" + mm + "'][dd='" + dd + "']").val();

			evtObj.dates[evtObj.dates.length] = dateObj;
		});
		return evtObj;
	},
	jsonToCalendar: function() {
		var _self = this;
		var evtObj = this.evtList;
		
		for(var key in evtObj) {
			var dateObj = evtObj[key];
				
				var timespan = dateObj.start_time?dateObj.start_time + (dateObj.end_time?'~'+dateObj.end_time:''):(dateObj.end_time?dateObj.end_time:'');
				var dateHTML = '<li class="calendar-item" yy="' + dateObj.yy + '" mm="' + dateObj.mm + '" dd="' + dateObj.dd + '" ' 
							   + 'did="' + dateObj.date_id + '" ' 
							   + 'rid0="' + key + '" ' 
							   + 'evtype="date" ' 
							   + 'logform="' + dateObj.logform + '" ' 
							   + 'title="'+ timespan + ' - ' + dateObj.title + ' - ' + words["event_place"] + " : " +  dateObj.place_desc +'">';
							   
				var active = dateObj.active?'':'<span style="color:red;">*</span>'; 

				var tcss = '<span style="color:blue; white-space:nowrap;">';
				if(dateObj.event_status==0) tcss = '<span style="color:#4950AF; white-space:nowrap;">'; 
				if(dateObj.event_status==9) tcss = '<span style="color:#4950AF; white-space:nowrap;">'; 
				
				if(dateObj.event_status==0) tcss = '<span style="color:#4950AF; white-space:nowrap;">'; 
				if(dateObj.event_status==9) tcss = '<span style="color:#4950AF; white-space:nowrap;">'; 

				//var timeHTML = tcss + timespan + ' <span style="font-style:normal;">' + dateObj.site_desc + '</span></span><br>';
				var timeHTML = tcss + timespan + ' <span style="font-style:normal;">&nbsp;</span></span>';
				
				var dcss = '<span>';
				if(dateObj.event_status==0) dcss = '<span style="color:#BBC101;">'; 
				if(dateObj.event_status==1) dcss = '<span style="color:black;">'; 
				if(dateObj.event_status==2) dcss = '<span style="color:#F92AC9;">'; 
				if(dateObj.event_status==9) dcss = '<span style="color:#888888;">'; 
				var place = ' @' + dateObj.place_desc; 
				var titleHTML = dcss + dateObj.title + place + '</span>';
				
				//var place = dateObj.place>0?'-' + dateObj.place_desc:''; 
				dateHTML += active + timeHTML + titleHTML;

				dateHTML += '</li>';
				$("div.date-event[yy='" + dateObj.yy + "'][mm='" + dateObj.mm + "'][dd='" + dateObj.dd + "']").append(dateHTML);
		}
		
		$("li.calendar-item[yy][mm][dd]").die("click.calendar").live("click.calendar", function(ev) {
			var date_sn = $(this).attr("rid0");
			_self.settings.dateClick(_self.evtList[date_sn]);
		});
	},
	month_change: function() {
			var _self = this;
			$.ajax({
				data: {
					admin_sess: this.admin_sess,
					admin_menu:	this.admin_menu,
					admin_oper:	this.admin_oper,
					
					site:		this.site,
					year:		this.curYY,
					month:		this.curMM
				},
				dataType: "json",  
				error: function(xhr, tStatus, errorTh ) {
					alert("Error (event_calendar_list.php): " + xhr.responseText + "\nStatus: " + tStatus);
				},
				success: function(req, tStatus) {
					if( req.errorCode > 0 ) { 
						errObj.set(req.errorCode, req.errorMessage, req.errorField);
						return false;
					} else {
						_self.evtList = req.data.evt;
						_self.jsonToCalendar();
					}
				},
				type: "post",
				url: "ajax/event_calendar_list.php"
			});
	},
	showWD: function() {
		var str = '';
		for(var i=0; i<this.weekdays.length; i++) {
			str += "Week: " + i + "\n";
			for(var j=0; j<7; j++) {
				if(this.weekdays[i][j]) 
					str += "(" + j + ")-[" + this.weekdays[i][j].year + "-" + (this.weekdays[i][j].month + 1) + "-" + this.weekdays[i][j].date + ":" + this.weekdays[i][j].status + "]; ";
				else 
					str += "(" + j + ")-[na]; "; 
			}
			str += "\n";
		}
		alert(str);
	},
	
	hour_html: function(yy, mm, dd, cid, val) {
			var html = '';
			var hm = []; 
			hm[0] = '';
			hm[1] = '';
			if(val) hm = val.split(":");
			html += '<select yy="' + yy + '" mm="' + mm + '" dd="' + dd + '" hm="hour" style="text-align:center;" class="cal-time ' + cid + '">';
			html += '<option value=""></option>';
			for(var i=5; i<=23; i++) {
				if( i == parseInt(hm[0]) ) 
					html += '<option value="' + i + '" selected>' + i + '</option>';
				else 
					html += '<option value="' + i + '">' + i + '</option>';
			}
			html += '</select>';
			html += '<b> : </b>';
			html += '<select yy="' + yy + '" mm="' + mm + '" dd="' + dd + '" hm="min" style="text-align:center;" class="cal-time ' + cid + '">';
			html += '<option value=""></option>';
			html += '<option value="00" ' + (hm[1]=="00"?'selected':'')+ '>00</option>';
			html += '<option value="15" ' + (hm[1]=="15"?'selected':'')+ '>15</option>';
			html += '<option value="30" ' + (hm[1]=="30"?'selected':'')+ '>30</option>';
			html += '<option value="45" ' + (hm[1]=="45"?'selected':'')+ '>45</option>';
			html += '</select>';
			return html;
	},
	
	hour_val: function(yy, mm, dd, cid) {
		var thh = $("select." + cid + "[yy='" + yy + "'][mm='" + mm + "'][dd='" + dd + "'][hm='hour']").val();
		var tmm = $("select." + cid + "[yy='" + yy + "'][mm='" + mm + "'][dd='" + dd + "'][hm='min']").val();
		var hour =  thh + ":" + (tmm!=""?tmm:"00"); 
		 			
		var regExp = /\d+:\d{2}/gi;
		return regExp.test(hour)?hour:''; 	
	}
}


