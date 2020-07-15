function CreateUUID() {
    var s = [];
    var hexDigits = "0123456789abcdef";
    for (var i = 0; i < 36; i++) {
        s[i] = hexDigits.substr(Math.floor(Math.random() * 0x10), 1);
    }
    s[14] = "4";  // bits 12-15 of the time_hi_and_version field to 0010
    s[19] = hexDigits.substr((s[19] & 0x3) | 0x8, 1);  // bits 6-7 of the clock_seq_hi_and_reserved to 01
    s[8] = s[13] = s[18] = s[23] = "-";

    var uuid = s.join("");
    return uuid;
}

function Clone(src) {
    if ($.isPlainObject(src)) {
        let target = {};
        for (let prop in src) {
            if (src.hasOwnProperty(prop)) {
                if ($.isPlainObject(src[prop])) {
                    target[prop] = Clone(src[prop]);
                }
                else
                {
                    target[prop] = src[prop];
                }
            }
        }
        return target;
    } else if ($.isArray(src)) {
        let target = [];
        for (let prop in src) {
            if (src.hasOwnProperty(prop)) {
                if ($.isPlainObject(src[prop])) {
                    target[prop] = Clone(src[prop]);
                }
                else {
                    target[prop] = src[prop];
                }
            }
        }
        return target;
    } else {
        return src;
    }
}

function CreateToken(length) {
    //edit the token allowed characters
    var a = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".split("");
    var b = [];
    for (var i = 0; i < length; i++) {
        var j = (Math.random() * (a.length - 1)).toFixed(0);
        b[i] = a[j];
    }
    return b.join("");
}

/* string fucntion */
String.prototype.nl2br = function () {
    var str = this.toString();
    str = str.replace(/\n|\r/gi, "<br>");
    str = str.replace(/ /gi, "&nbsp;");
    return str;
};
String.prototype.nl2br1 = function () {
    var str = this.toString();
    str = str.replace(/\n|\r/gi, "<br>");
    //str = str.replace(/ /gi, "&nbsp;");
    return str;
};
String.prototype.br2nl = function () {
    var str = this.toString();
    str = str.replace(/<br>|<br \/>/gi, "\n");
    str = str.replace(/&nbsp;/gi, " ");
    return str;
};
String.prototype.trim = function () {
    return this.replace(/^\s+|\s+$/g, '');
};
String.prototype.replaceAll = function (s1, s2) {
    return this.replace(new RegExp(s1, "gm"), s2);
};
String.prototype.holderArray = function () {
    let regex = /({[A-Za-z_]+,?[A-Za-z_,<>\-\s]+})/gi;
    let matches = this.match(regex);
    let ret_arr = [];
    if (matches) {
        for (let i = 0; i < matches.length; i++) {
            let match = ('' + matches[i]).replaceAll("{", "").replaceAll("}", "");
            if (ret_arr.indexOf(match) < 0)
                ret_arr.push(match);
        }
    }
    return ret_arr;
};
String.prototype.replaceHolder = function (valueArr) {
    let originStr = '' + this.toString();
    let hArr = originStr.holderArray();
    for (let i = 0; i < hArr.length; i++) {
        if (hArr[i].indexOf(",") >= 0) {
            let ha = hArr[i].split(",");
            if (valueArr[ha[0]]) 
                originStr = originStr.replaceAll('{' + hArr[i] + '}', ha[1]);
            else 
                originStr = originStr.replaceAll('{' + hArr[i] + '}', '');
        }
        else {
            if (valueArr[hArr[i]])
                originStr = originStr.replaceAll('{' + hArr[i] + '}', valueArr[hArr[i]]);
            else
                originStr = originStr.replaceAll('{' + hArr[i] + '}', '');
        }

    }
    return originStr;
};
String.prototype.ucword = function () {
    if (this !== "") {
        return ('' + this).replace(/^([a-z])|\s+([a-z])/g, function ($1) { return $1.toUpperCase(); });
    }
    else
        return "";
};
String.prototype.capital = function () {
    if (this !== "") {
        return this.charAt(0).toUpperCase() + this.slice(1);
    } else {
        return "";
    }
};
String.prototype.join = function (sp, str) {
    if (this !== "" && ("" + str) !== "")
        return this + sp + str;
    else
        return this + ("" + str);
};
String.prototype.labelYes = function () {
    let str = '' + this.toString();
    if (str !== "" && str.indexOf("|") >= 0)
        return str.split("|")[0];
    else
        return "";
};
String.prototype.labelNo = function () {
    let str = '' + this.toString();
    if (str !== "" && str.indexOf("|") >= 0)
        return str.split("|")[1];
    else
        return "";
};

String.prototype.toBlob = function (mime) {
    var bstr = "" + this.toString();
    var n = bstr.length;
    var u8arr = new Uint8Array(n);

    while (n--) {
        u8arr[n] = bstr.charCodeAt(n);
    }
    return new Blob([u8arr], { type: mime });
};

String.prototype.toArray = function (sp, case_sense) {
    case_sense = case_sense ? case_sense : "origin";
    sp = sp ? sp : ",";
    var bstr = "" + this.toString();
    var barr = bstr.split(sp);
    var narr = [];
    for (var key in barr) {
        switch (case_sense) {
            case "upper":
                narr.push(barr[key].trim().toUpperCase());
                break;
            case "lower":
                narr.push(barr[key].trim().toLowerCase());
                break;
            default:
                narr.push(barr[key].trim());
                break;
        }
    }
    return narr;
};

/* File Name */
String.prototype.fileName = function () {
    return this.toString().replace(/.*(\/|\\)/, "");
};
String.prototype.shortName = function () {
    var name = this.toString().replace(/.*(\/|\\)/, "");
    return name.substr(0, name.lastIndexOf("."));
};
String.prototype.extName = function () {
    return (this.toString().indexOf('.') !== -1) ? this.toString().replace(/.*[.]/, '').toLowerCase() : '';
};
String.prototype.subName = function (n) {
    n = n || 10;
    var name = this.toString().fileName();
    if (name.length > n) {
        name = name.slice(0, n - 6) + '...' + name.slice(-6);
    }
    return name;
};
String.prototype.toDate = function () {
    if (isNaN(this) || parseInt(this) <= 0) {
        return new Date(0);
    } else {
        return new Date(parseInt(this) * 1000);
    }
};

String.prototype.intDate = function () {
    if (isNaN(this) || parseInt(this) <= 0) {
        return "";
    } else {
        var ndate = new Date(parseInt(this) * 1000);
        return ndate.format("Y-m-d H:i");
    }
};
String.prototype.hhmm = function () {
    let nstr = "" + this.toString();
    let ret = nstr.substr(0, nstr.lastIndexOf(":"));
    if (ret === "00:00") ret = "";
    return ret;
};
Number.prototype.toSize = function () {
    if (isNaN(this) || parseFloat(this) <= 0) {
        return "";
    } else {
        /* main function here */
        var bytes = parseInt(this);
        var i = -1;
        do {
            bytes = bytes / 1024;
            i++;
        } while (bytes > 999);
        /* end of main function here */

        return Math.max(bytes, 1).toFixed(1) + ['KB', 'MB', 'GB', 'TB', 'PB', 'EB'][i];
    }
};

Number.prototype.toDate = function () {
    if (isNaN(this) || parseInt(this) <= 0) {
        return new Date(0);
    } else {
        return new Date(parseInt(this) * 1000);
    }
};
Number.prototype.intDate = function () {
    if (isNaN(this) || parseInt(this) <= 0) {
        return "";
    } else {
        var ndate = new Date(parseInt(this) * 1000);
        return ndate.format("Y-m-d H:i");
    }
};

Date.prototype.diff = function (d2) {
    var t2 = this.getTime();
    var t1 = d2.getTime();
    return parseInt((t2 - t1) / (24 * 3600 * 1000));
};

Date.prototype.timezone = function () {
    return this.getTimezoneOffset() >= 0 ? "-" + (this.getTimezoneOffset() / 60) + ":00" : "+" + (Math.abs(this.getTimezoneOffset()) / 60) + ":00";
};

Date.prototype.ticks = function () {
    return Date.UTC(this.getFullYear(), this.getMonth(), this.getDate(), this.getHours(), this.getMinutes(), this.getSeconds()) / 1000
};

Date.prototype.format = function (format) {
    var returnStr = '';
    var replace = Date.replaceChars;
    for (var i = 0; i < format.length; i++) {
        var curChar = format.charAt(i); if (i - 1 >= 0 && format.charAt(i - 1) == "\\") {
            returnStr += curChar;
        }
        else if (replace[curChar]) {
            returnStr += replace[curChar].call(this);
        } else if (curChar != "\\") {
            returnStr += curChar;
        }
    }
    return returnStr;
};
Date.prototype.ymd = function () {
    return new Date(this.getFullYear(), this.getMonth(), this.getDate());
};
Date.prototype.ymdString = function () {
    return this.getFullYear() + "-" +  (parseInt(this.getMonth()) + 1) + "-" + this.getDate();
};

Date.replaceChars = {
    shortMonths: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'],
    longMonths: ['January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December'],
    shortDays: ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'],
    longDays: ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'],

    // Day
    d: function () { return (this.getDate() < 10 ? '0' : '') + this.getDate(); },
    D: function () { return Date.replaceChars.shortDays[this.getDay()]; },
    j: function () { return this.getDate(); },
    l: function () { return Date.replaceChars.longDays[this.getDay()]; },
    N: function () { return this.getDay() + 1; },
    S: function () { return (this.getDate() % 10 == 1 && this.getDate() != 11 ? 'st' : (this.getDate() % 10 == 2 && this.getDate() != 12 ? 'nd' : (this.getDate() % 10 == 3 && this.getDate() != 13 ? 'rd' : 'th'))); },
    w: function () { return this.getDay(); },
    z: function () { var d = new Date(this.getFullYear(), 0, 1); return Math.ceil((this - d) / 86400000); }, // Fixed now
    // Week
    W: function () { var d = new Date(this.getFullYear(), 0, 1); return Math.ceil((((this - d) / 86400000) + d.getDay() + 1) / 7); }, // Fixed now
    // Month
    F: function () { return Date.replaceChars.longMonths[this.getMonth()]; },
    m: function () { return (this.getMonth() < 9 ? '0' : '') + (this.getMonth() + 1); },
    M: function () { return Date.replaceChars.shortMonths[this.getMonth()]; },
    n: function () { return this.getMonth() + 1; },
    t: function () { var d = new Date(); return new Date(d.getFullYear(), d.getMonth(), 0).getDate() }, // Fixed now, gets #days of date
    // Year
    L: function () { var year = this.getFullYear(); return (year % 400 == 0 || (year % 100 != 0 && year % 4 == 0)); },   // Fixed now
    o: function () { var d = new Date(this.valueOf()); d.setDate(d.getDate() - ((this.getDay() + 6) % 7) + 3); return d.getFullYear(); }, //Fixed now
    Y: function () { return this.getFullYear(); },
    y: function () { return ('' + this.getFullYear()).substr(2); },
    // Time
    a: function () { return this.getHours() < 12 ? 'am' : 'pm'; },
    A: function () { return this.getHours() < 12 ? 'AM' : 'PM'; },
    B: function () { return Math.floor((((this.getUTCHours() + 1) % 24) + this.getUTCMinutes() / 60 + this.getUTCSeconds() / 3600) * 1000 / 24); }, // Fixed now
    g: function () { return this.getHours() % 12 || 12; },
    G: function () { return this.getHours(); },
    h: function () { return ((this.getHours() % 12 || 12) < 10 ? '0' : '') + (this.getHours() % 12 || 12); },
    H: function () { return (this.getHours() < 10 ? '0' : '') + this.getHours(); },
    i: function () { return (this.getMinutes() < 10 ? '0' : '') + this.getMinutes(); },
    s: function () { return (this.getSeconds() < 10 ? '0' : '') + this.getSeconds(); },
    u: function () { var m = this.getMilliseconds(); return (m < 10 ? '00' : (m < 100 ? '0' : '')) + m; },
    // Timezone
    e: function () { return "Not Yet Supported"; },
    I: function () {
        var DST = null;
        for (var i = 0; i < 12; ++i) {
            var d = new Date(this.getFullYear(), i, 1);
            var offset = d.getTimezoneOffset();

            if (DST === null) DST = offset;
            else if (offset < DST) { DST = offset; break; } else if (offset > DST) break;
        }
        return (this.getTimezoneOffset() == DST) | 0;
    },
    O: function () { return (-this.getTimezoneOffset() < 0 ? '-' : '+') + (Math.abs(this.getTimezoneOffset() / 60) < 10 ? '0' : '') + (Math.abs(this.getTimezoneOffset() / 60)) + '00'; },
    P: function () { return (-this.getTimezoneOffset() < 0 ? '-' : '+') + (Math.abs(this.getTimezoneOffset() / 60) < 10 ? '0' : '') + (Math.abs(this.getTimezoneOffset() / 60)) + ':00'; }, // Fixed now
    T: function () { var m = this.getMonth(); this.setMonth(0); var result = this.toTimeString().replace(/^.+ \(?([^\)]+)\)?$/, '$1'); this.setMonth(m); return result; },
    Z: function () { return -this.getTimezoneOffset() * 60; },
    // Full Date/Time
    c: function () { return this.format("Y-m-d\\TH:i:sP"); }, // Fixed now
    r: function () { return this.toString(); },
    U: function () { return this.getTime() / 1000; }
};
/*** End of prototype common function ***/