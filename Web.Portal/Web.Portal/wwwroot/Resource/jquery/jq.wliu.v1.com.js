$.extend({
    inList: function (arr, obj) {
        if ($.isPlainObject(arr) || $.isArray(arr)) {
            var tempArr = $.map(arr, function (el, idx) {
                var ret_flag = true;
                if ($.isPlainObject(obj))
                    for (var key in obj) {
                        if (el[key] !== obj[key]) ret_flag = false;
                    }
                else
                    if (el !== obj) ret_flag = false;
                if (ret_flag) return el;
            });
            return tempArr.length > 0;
        } else {
            return false;
        }
    },
    objInList: function (arr, obj) {
        var tArr = $.objsInList(arr, obj);
        return tArr.length > 0 ? tArr[0] : null;
    },
    objsInList: function (arr, obj) {
        if ($.isPlainObject(arr) || $.isArray(arr)) {
            // $.grep used for Array , not for JSON Object properties
            var tempArr = $.map(arr, function (el, idx) {
                var ret_flag = true;
                if ($.isPlainObject(obj))
                    for (var key in obj) {
                        if (el[key] !== obj[key]) ret_flag = false;
                    }
                else
                    if (el !== obj) ret_flag = false;
                if (ret_flag) return el;

            });
            return tempArr;
        } else {
            return [];
        }
    },
    indexInList: function (arr, obj) {
        var idxs = $.indexsInList(arr, obj);
        return idxs.length > 0 ? idxs[0] : -1;
    },
    indexsInList: function (arr, obj) {
        if ($.isPlainObject(arr) || $.isArray(arr)) {
            var tempArr = $.map(arr, function (el, idx) {
                var ret_flag = true;
                if ($.isPlainObject(obj))
                    for (var key in obj) {
                        if (el[key] !== obj[key]) ret_flag = false;
                    }
                else
                    if (el !== obj) ret_flag = false;

                if (ret_flag) return idx;
            });
            return tempArr;
        }
        else {
            return [];
        }
    },
    removeInList: function (arr, obj) {
        var index = $.indexInList(arr, obj);
        if ($.isPlainObject(arr)) delete arr[index];
        if ($.isArray(arr)) arr.splice(index, 1);
        return arr; // arr is reference type
    },
    removesInList: function (arr, obj) {
        var indexs = $.indexsInList(arr, obj);
        for (var i = indexs.length - 1; i >= 0; i--) {
            if ($.isPlainObject(arr)) delete arr[indexs[i]];
            if ($.isArray(arr)) arr.splice(indexs[i], 1);
        }
        return arr;  // arr is reference type
    },
    removeByIndex: function (arr, ridx) {
        if ($.isArray(arr)) {
            if (ridx >= 0 && ridx < arr.length) {
                arr.splice(ridx, 1);
                return arr;
            } else {
                return arr;
            }
        }
    },
    findInArray: function (arr, obj) {
        if ($.isArray(arr)) {
            for (var ridx = 0; ridx < arr.length; ridx++) {
                let find = true;
                var el = arr[ridx];
                if ($.isPlainObject(obj)) {
                    for (var key in obj) {
                        if (el[key] !== obj[key]) {
                            find = false;
                        }
                    }
                }
                else {
                    if (el !== obj) {
                        find = false;
                    }
                }
                if (find) return el;
            }
            return null;
        }
        return null;
    },
    findsInArray: function (arr, obj) {
        let narr = [];
        if ($.isArray(arr)) {
            for (var ridx = 0; ridx < arr.length; ridx++) {
                let find = true;
                var el = arr[ridx];
                if ($.isPlainObject(obj)) {
                    for (var key in obj) {
                        if (el[key] !== obj[key]) {
                            find = false;
                        }
                    }
                }
                else {
                    if (el !== obj) {
                        find = false;
                    }
                }
                if (find) narr.push(el);
            }
        }
        return narr;
    }
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