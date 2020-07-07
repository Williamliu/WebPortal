var WLIU = WLIU || {};
WLIU.ViewData = function () {
    this.database = new WLIU.Database();
    this.scope = null;
};
WLIU.ViewData.prototype = {
    Init: function (url) {
        let defer = $.Deferred();

        var self = this;
        AJAX.Get(url)
            .then(data => {
                self.database = new WLIU.Database(data);
                self.database.DBAjaxErrorHandle();
                defer.resolve(self);
                self.Apply();
            })
            .catch(err => {
                self.database.error = new WLIU.Error(err.data.error);
                self.database.DBAjaxErrorHandle();
                defer.reject(self);
                self.Apply();
            });

        return defer.promise();
    },
    LinkScope: function (p_scope) {
        p_scope.ViewData = this;
        this.scope = p_scope;
    },
    Apply: function () {
        if (this.scope) {
            if (!this.scope.$root.$$phase) this.scope.$apply();
        }
    }
};

WLIU.Database = function (database) {
    if ($.isPlainObject(database)) {
        this.method = database.method || "get";
        this.getUrl = database.getUrl;
        this.saveUrl = database.saveUrl;
        this.user = database.user || {};
        this.user.rights = this.user.rights || {};
        this.error = new WLIU.Error(database.error);
        this.AddOTables(database.tables);
        this.AddOCollections(database.collections);
        this.scope = null;
        this.other = database.other || {};
    }
    else {
        this.method = "get";
        this.getUrl = "";
        this.saveUrl = "";
        this.user = {};
        this.user.rights = {};
        this.error = new WLIU.Error();
        this.tables = {};
        this.collections = {};
        this.method = "";
        this.scope = null;
        this.other = {};
    }
};
WLIU.Database.prototype = {
    AddTable: function (table) {
        this.tables[table.name] = table;
        return this;
    },
    AddOTable: function (table) {
        this.tables[table.name] = new WLIU.Table(table);
        return this;
    },
    AddTables: function (tables) {
        this.tables = {};
        if ($.isPlainObject(tables)) {
            for (let tableName in tables) {
                this.AddTable(tables[tableName]);
            }
        }
        return this;
    },
    AddOTables: function (tables) {
        this.tables = {};
        if ($.isPlainObject(tables)) {
            for (let tableName in tables) {
                this.AddOTable(tables[tableName]);
            }
        }
        return this;
    },
    AddCollection: function (collection) {
        this.collections[collection.name] = collection;
        return this;
    },
    AddOCollection: function (collection) {
        this.collections[collection.name] = new WLIU.Collection(collection);
        return this;
    },
    AddCollections: function (collections) {
        this.collections = {};
        if ($.isPlainObject(collections)) {
            for (let collectionName in collections) {
                this.AddCollection(collections[collectionName]);
            }
        }
        return this;
    },
    AddOCollections: function (collections) {
        this.collections = {};
        if ($.isPlainObject(collections)) {
            for (let collectionName in collections) {
                this.AddOCollection(collections[collectionName]);
            }
        }
        return this;
    },
    GetCollection: function (tbName, colName) {
        if (this.tables[tbName])
            if (this.tables[tbName].metas)
                if (this.tables[tbName].metas[colName])
                    if (this.collections[this.tables[tbName].metas[colName].listRef.collection])
                        return this.collections[this.tables[tbName].metas[colName].listRef.collection];
        return null;
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
};

WLIU.Collection = function (collection) {
    if ($.isPlainObject(collection)) {
        this.name = collection.name || "Unknown";
        this.error = new WLIU.Error(collection.error);
        this.AddOItems(collection.items);
    }
    else {
        this.name = "Unknown";
        this.error = new WLIU.Error();
        this.items = [];
    }
};
WLIU.Collection.prototype = {
    AddItem: function (item) {
        this.items.push(item);
        return this;
    },
    AddOItem: function (item) {
        this.items.push(new WLIU.Item(item));
        return this;
    },
    AddItemKV: function (value, title, detail) {
        let item = new WLIU.Item();
        item.value = value || "";
        item.title = title || "";
        item.detail = detail || "";
        this.AddItem(item);
        return this;
    },
    AddItems: function (items) {
        this.items = [];
        if ($.isArray(items)) {
            for (let itemIdx in items) {
                this.AddItem(items[itemIdx]);
            }
        }
        return this;
    },
    AddOItems: function (items) {
        this.items = [];
        if ($.isArray(items)) {
            for (let itemIdx in items) {
                this.AddOItem(items[itemIdx]);
            }
        }
        return this;
    },
    FindItem: function (value) {
        for (let idx in this.items) {
            if (("" + this.items[idx].value).toString() === ("" + value).toString()) return this.items[idx];
        }
        return null;
    },
    FilterItems: function (kvs) {
        let ret_items = [];
        for (let idx in this.items) {
            for (let val in kvs) {
                if (kvs[val]) {
                    if (("" + this.items[idx].value).toString() === ("" + val).toString()) ret_items.push(this.items[idx]);
                }
            }
        }
        return ret_items;
    }
};
WLIU.Item = function (item) {
    if ($.isPlainObject(item)) {
        this.value = item.value || "";
        this.refValue = item.refValue || "";
        this.title = item.title || "";
        this.detail = item.detail || "";
    }
    else {
        this.value = "";
        this.refValue = "";
        this.title = "";
        this.detail = "";
    }
};

WLIU.Table = function (table) {
    if ($.isPlainObject(table)) {
        this.name = table.name || "Unknown";
        this.refKey = table.refKey || 0;
        this.title = table.title || "";
        this.description = table.description || "";
        this.method = table.method || "none";
        this.state = table.state || 0;
        this.rowGuid = table.rowGuid || "";
        this.getUrl = table.getUrl || "";
        this.scanUrl = table.scanUrl || "";
        this.saveUrl = table.saveUrl || "";
        this.validateUrl = table.validateUrl || "";
        this.error = new WLIU.Error(table.error);
        this.navi = new WLIU.Navi(table.navi);
        this.AddOMetas(table.metas);
        this.AddOFilters(table.filters);
        this.AddORows(table.rows);
        this.CKEditor = [];
        this.other = table.other || {};
    }
    else {
        this.name = "Unknown";
        this.refKey = 0;
        this.title = "";
        this.description = "";
        this.method = "none";
        this.state = 0;
        this.rowGuid = "";
        this.getUrl = "";
        this.scanUrl = "";
        this.saveUrl = "";
        this.validateUrl = "";
        this.error = new WLIU.Error();
        this.navi = new WLIU.Navi();
        this.metas = {};
        this.filters = {};
        this.rows = [];
        this.CKEditor = [];
        this.other = {};
    }
};
WLIU.Table.prototype = {
    Clear: function () {
        this.refKey = 0;
        this.state = 0;
        this.method = "none";
        this.error = new WLIU.Error();

        let oldOrder = this.navi.order;
        let oldBy = this.navi.by;
        this.navi = new WLIU.Navi();
        this.navi.order = oldOrder;
        this.navi.by = oldBy;

        this.rows = [];
        this.other = {};
    },
    AddRow: function (row) {
        this.rows.push(row);
        return this;
    },
    AddORow: function (row) {
        this.rows.push(new WLIU.Row(row));
        return this;
    },
    AddRows: function (rows) {
        this.rows = [];
        if ($.isArray(rows)) {
            for (let rowIdx in rows) {
                this.AddRow(rows[rowIdx]);
            }
        }
        return this;
    },
    AddORows: function (rows) {
        this.rows = [];
        if ($.isArray(rows)) {
            for (let rowIdx in rows) {
                this.AddORow(rows[rowIdx]);
            }
        }
        return this;
    },

    AddMeta: function (meta) {
        this.metas[meta.name] = meta;
        return this;
    },
    AddOMeta: function (meta) {
        this.metas[meta.name] = new WLIU.Meta(meta);
        return this;
    },
    AddMetas: function (metas) {
        this.metas = {};
        for (let metaName in metas) {
            this.AddMeta(metas[metaName]);
        }
        return this;
    },
    AddOMetas: function (metas) {
        this.metas = {};
        if ($.isPlainObject(metas)) {
            for (let metaName in metas) {
                this.AddOMeta(metas[metaName]);
            }
        }
        return this;
    },

    AddFilter: function (filter) {
        this.filters[filter.name] = filter;
        return this;
    },
    AddOFilter: function (filter) {
        this.filters[filter.name] = new WLIU.Filter(filter);
        return this;
    },
    AddFilters: function (filters) {
        this.filters = {};
        for (let filterName in filters) {
            this.AddFilter(filters[filterName]);
        }
        return this;
    },
    AddOFilters: function (filters) {
        this.filters = {};
        if ($.isPlainObject(filters)) {
            for (let filterName in filters) {
                this.AddOFilter(filters[filterName]);
            }
        }
        return this;
    },
    //New Row
    NewRowB: function (kvs) {
        let nrow = new WLIU.Row();
        nrow.state = 2;
        for (let colName in this.metas) {
            if (kvs && $.isPlainObject(kvs)) {
                if (("" + kvs[colName]) !== "") {
                    let ncol = {
                        name: this.metas[colName].name,
                        value: Clone(kvs[colName])
                    };
                    nrow.AddOColumn(ncol);
                }
                else {
                    let ncol = {
                        name: this.metas[colName].name,
                        value: Clone(this.metas[colName].value)
                    };
                    nrow.AddOColumn(ncol);
                }
            }
            else {
                let ncol = {
                    name: this.metas[colName].name,
                    value: Clone(this.metas[colName].value)
                };
                nrow.AddOColumn(ncol);
            }
        }
        this.rows.unshift(nrow);
        this.rowGuid = nrow.guid;
        this.state = 1;
        return nrow;
    },
    NewRowA: function () {
        let nrow = new WLIU.Row();
        nrow.state = 2;
        for (let colName in this.metas) {
            let ncol = {
                name: this.metas[colName].name,
                value: Clone(this.metas[colName].value)
            };
            nrow.AddOColumn(ncol);
        }
        this.rows.push(nrow);
        this.rowGuid = nrow.guid;
        this.state = 1;
        return nrow;
    },
    CurrentGuid: function () {
        return this.rowGuid;
    },
    CurrentIndex: function () {
        return $.indexInList(this.rows, { guid: this.rowGuid });
    },
    CurrentKey: function () {
        if (this.rows[this.CurrentIndex()]) {
            return this.rows[this.CurrentIndex()].key;
        }
        return -1;
    },
    CurrentState: function () {
        if (this.rows[this.CurrentIndex()]) {
            return this.rows[this.CurrentIndex()].state;
        }
        return 0;
    },
    CurrentRow: function () {
        if (this.rows[this.CurrentIndex()]) {
            return this.rows[this.CurrentIndex()];
        }
        return null;
    },
    CurrentSet: function (colName, colValue) {
        if (this.CurrentColumn(colName)) {
            this.CurrentColumn(colName).value = colValue;
            this.CurrentColumn(colName).current = colValue;
        }
    },
    CurrentValue: function (colName) {
        if (this.CurrentColumn(colName)) {
            return this.CurrentColumn(colName).value;
        }
        return "";
    },
    CurrentColumn: function (colName) {
        if (this.CurrentRow())
            if (this.CurrentRow().columns)
                if (this.CurrentRow().columns[colName])
                    return this.CurrentRow().columns[colName];

        return null;
    },
    CurrentColumnHasError: function (colName) {
        let curRow = this.CurrentRow();
        if (curRow)
            return curRow.ColHasError(colName);
        else
            return false;
    },
    CurrentColumnErrorMsg: function (colName) {
        let curRow = this.CurrentRow();
        if (curRow)
            if (curRow.errors[colName])
                return curRow.errors[colName].Message().nl2br();
            else
                return "";
        else
            return "";
    },

    GuidRow: function (p_guid) {
        return $.findInArray(this.rows, { guid: p_guid });
    },
    GuidIndex: function (p_guid) {
        return $.indexInList(this.rows, { guid: p_guid });
    },
    GuidColumn: function (p_guid, colName) {
        if (this.GuidRow(p_guid))
            if (this.GuidRow(p_guid).columns)
                if (this.GuidRow(p_guid).columns[colName])
                    return this.GuidRow(p_guid).columns[colName];
        return null;
    },
    GuidColumnHasError: function (p_guid, colName) {
        let guidRow = this.GuidRow(p_guid);
        if (guidRow)
            return guidRow.ColHasError(colName);
        else
            return false;
    },
    GuidColumnErrorMsg: function (p_guid, colName) {
        let guidRow = this.GuidRow(p_guid);
        if (guidRow)
            if (guidRow.errors[colName])
                return guidRow.errors[colName].Message().nl2br();
            else
                return "";
        else
            return "";
    },
    GuidKey: function (p_guid) {
        if (this.GuidRow(p_guid))
            return this.GuidRow(p_guid).key;
        else
            return null;

    },
    GuidState: function (p_guid) {
        if (this.GuidRow(p_guid))
            return this.GuidRow(p_guid).state;
        else
            return null;

    },
    GuidValue: function (p_guid, colName) {
        if (this.GuidColumn(p_guid, colName))
            return this.GuidColumn(p_guid, colName).value;
        else
            return null;

    },
    IndexRow: function (rowIdx) {
        if (this.rows[rowIdx]) return this.rows[rowIdx];
        return null;
    },
    IndexColumn: function (rowIdx, colName) {
        if (this.IndexRow(rowIdx))
            if (this.IndexRow(rowIdx).columns)
                if (this.IndexRow(rowIdx).columns[colName])
                    return this.IndexRow(rowIdx).columns[colName];
        return null;
    },
    FilterRows: function (kvs) {
        let rows = [];
        if ($.isArray(this.rows)) {
            for (let idx = 0; idx < this.rows.length; idx++) {
                if (this.rows[idx] && this.rows[idx].columns) {
                    let isEqual = true;
                    if ($.isPlainObject(kvs)) {
                        for (let colName in kvs) {
                            if (this.rows[idx].columns[colName].value !== kvs[colName]) isEqual = false;
                        }
                    }
                    else {
                        if (this.rows[idx].key !== kvs) isEqual = false;
                    }

                    if (isEqual) rows.push(this.rows[idx]);
                }
            }
        }
        return rows;
    },
    /*** CKEditor ***/
    CKEditorClear: function () {
        for (let i = 0; i < this.CKEditor.length; i++) {
            CKEDITOR.instances[`${this.name}_${this.CKEditor[i]}_ckeditor`].setData("");
        }
    },
    CKEditorReset: function () {
        for (let i = 0; i < this.CKEditor.length; i++) {
            let content = "";
            if (this.CurrentColumn(this.CKEditor[i]))
                content = this.CurrentColumn(this.CKEditor[i]).value;
            if (content) {
                CKEDITOR.instances[`${this.name}_${this.CKEditor[i]}_ckeditor`].setData(content);
                //console.log(this.CKEditor[i] + " content has: " + content);
            } else {
                CKEDITOR.instances[`${this.name}_${this.CKEditor[i]}_ckeditor`].setData("");
                //console.log(this.CKEditor[i] + " content none: " + content);
            }
        }
    },
    CKEditorCancel: function () {
        for (let i = 0; i < this.CKEditor.length; i++) {
            let content = "";
            if (this.CurrentColumn(this.CKEditor[i]))
                content = this.CurrentColumn(this.CKEditor[i]).current;
            if (content)
                CKEDITOR.instances[`${this.name}_${this.CKEditor[i]}_ckeditor`].setData(content);
            else
                CKEDITOR.instances[`${this.name}_${this.CKEditor[i]}_ckeditor`].setData("");
        }
    },
    //Order 
    orderState: function (colName, sort) {
        let flag = false;
        if (this.navi.by === colName) {
            if (("" + this.navi.order).toLowerCase() === ("" + sort).toLowerCase())
                flag = true;
        }
        return flag;
    },
    orderChange: function (colName) {
        if (this.metas[colName]) {
            if (this.navi.by === colName) {
                switch (("" + this.navi.order).toUpperCase()) {
                    case "ASC":
                        this.navi.order = "DESC";
                        break;
                    case "DESC":
                        this.navi.order = "ASC";
                        break;
                    default:
                        this.navi.order = ("" + this.metas[colName].order).toUpperCase();
                        break;
                }
            } else {
                this.navi.order = ("" + this.metas[colName].order).toUpperCase();
            }
            this.navi.by = colName;
            this.navi.pageNo = 0;
            return this.firstPage();
        }
        let defer = $.Deferred();
        defer.resolve({});
        return defer.promise();
    },

    // Table State: change col, row, table state
    Change: function (guid, colName) {
        if (this.GuidRow(guid)) {
            this.GuidRow(guid).Change(colName);
            this.ChangeState();
        }
    },
    SetChange: function (guid, colName) {
        if (this.GuidRow(guid)) {
            this.GuidRow(guid).SetChange(colName);
            this.ChangeState();
        }
    },
    ChangeState: function () {
        this.state = 0;
        if ($.isArray(this.rows)) {
            for (let rowIndex in this.rows) {
                if (this.IndexRow(rowIndex).state > 0) this.state = 1;
            }
        }
    },
    // Filter Change 
    FilterChange: function (filterName) {
        if (this.filters[filterName]) {
            this.filters[filterName].Change();
        }
    },

    // Navigation
    firstPage: function (url) {
        let pageNo = 1;
        if (this.navi.pageTotal <= 0) pageNo = 0;
        //if (this.navi.pageNo !== 1 && this.navi.pageTotal > 0) {
        if (this.navi.pageNo !== 1) {
            pageNo = 1;

            return this.Reload(url, pageNo);
        }
        let defer = $.Deferred();
        defer.resolve({});
        return defer.promise();
    },
    firstState: function () {
        return !this.navi.pageNo || !this.navi.pageTotal || this.navi.pageNo <= 1 || this.navi.pageTotal <= 0;
    },
    previousPage: function (url) {
        let pageNo = 1;
        if (this.navi.pageNo <= 0) {
            pageNo = 1;
        }
        if (this.navi.pageTotal <= 0) pageNo = 0;
        if (this.navi.pageNo > 1) {
            pageNo = this.navi.pageNo - 1;

            return this.Reload(url, pageNo);
        }
        let defer = $.Deferred();
        defer.resolve({});
        return defer.promise();
    },
    previousState: function () {
        return !this.navi.pageNo || !this.navi.pageTotal || this.navi.pageNo <= 1 || this.navi.pageTotal <= 0;
    },
    nextPage: function (url) {
        let pageNo = 1;
        if (this.navi.pageTotal <= 0) pageNo = 0;
        if (this.navi.pageNo > this.navi.pageTotal) {
            pageNo = this.navi.pageTotal;
            this.reload(url, pageNo);
        }
        if (this.navi.pageNo < this.navi.pageTotal) {
            pageNo = this.navi.pageNo + 1;

            return this.Reload(url, pageNo);
        }
        let defer = $.Deferred();
        defer.resolve({});
        return defer.promise();
    },
    nextState: function () {
        return !this.navi.pageNo || !this.navi.pageTotal || this.navi.pageNo >= this.navi.pageTotal || this.navi.pageTotal <= 0;
    },
    lastPage: function (url) {
        let pageNo = 1;
        if (this.navi.pageTotal <= 0) pageNo = 0;
        if (this.navi.pageNo !== this.navi.pageTotal) {
            pageNo = this.navi.pageTotal;

            return this.Reload(url, pageNo);
        }
        let defer = $.Deferred();
        defer.resolve({});
        return defer.promise();
    },
    lastState: function () {
        return !this.navi.pageNo || !this.navi.pageTotal || this.navi.pageNo >= this.navi.pageTotal || this.navi.pageTotal <= 0;
    },
    Reload: function (url, pageNo) {
        this.getUrl = url || this.getUrl;
        //prepare post table
        this.method = "get";
        this.navi.isLoading = 1;
        let ntable = new WLIU.JSTable(this);
        ntable.navi.pageNo = pageNo || this.navi.pageNo;
        let defer = $.Deferred();
        var self = this;
        $("div[wliu][loading][method='get']").loading("show");
        self.TableAjax(self.getUrl, ntable).then(data => {
            self.state = 0;
            self.navi.isLoading = 0;
            self.SyncGetTable(data);

            self.TableAjaxErrorHandle();
            if (self.error.HasError() === false)
                defer.resolve(self);
            else
                defer.reject(self);

        }).catch(data => {
            self.state = 0;
            self.navi.isLoading = 0;
            self.error = new WLIU.Error(data.error);

            self.TableAjaxErrorHandle();
            defer.reject(self);
        });
        return defer.promise();
    },
    Scan: function (url) {
        return this.Reload(url, 1);
    },
    Validate: function (guid, colName) {
        var self = this;
        let defer = $.Deferred();
        //prepare post table
        if (this.GuidRow(guid) && this.GuidColumn(guid, colName)) {
            if (this.GuidRow(guid).state > 0 && this.GuidRow(guid).state < 3) {
                if (this.GuidColumn(guid, colName).state === 1) {
                    this.method = "validate";

                    let vcolumn = {};
                    vcolumn[colName] = {
                        name: this.GuidColumn(guid, colName).name,
                        value: this.GuidColumn(guid, colName).value,
                        guid: this.GuidColumn(guid, colName).guid,
                        state: this.GuidColumn(guid, colName).state
                    };
                    let vrow = {
                        key: this.GuidRow(guid).key,
                        guid: this.GuidRow(guid).guid,
                        state: this.GuidRow(guid).state,
                        columns: vcolumn
                    };
                    let trow = new WLIU.Row(vrow);
                    let ntable = new WLIU.JSTable(this);
                    ntable.AddRow(trow);
                    if (ntable.rows.length <= 0) return;
                    this.TableAjax(this.validateUrl, ntable).then(data => {
                        self.SyncValidate(data);
                        self.RowAjaxErrorHandle(guid);
                        self.TableAjaxErrorHandle();

                        if (self.HasError(guid) === false)
                            defer.resolve(self.GuidRow(guid));
                        else
                            defer.reject(self.GuidRow(guid));

                    }).catch(data => {
                        self.error = new WLIU.Error(data.error);
                        self.TableAjaxErrorHandle();
                        defer.reject(self.GuidRow(guid));
                    });
                }
            }
            else {
                let resp = {};
                resp.error = {};
                resp.error.code = 0;
                resp.error.messages = "";
                defer.resolve(resp);
            }
        } else {
            let resp = {};
            resp.error = {};
            resp.error.code = 404;
            resp.error.messages = [Words("validate.notfound")];
            defer.reject(resp);
        }
        return defer.promise();
    },
    // Table Operation: Add, Save, Delete, Cancel
    Save: function (guid, url) {
        $("div[wliu][loading][method='save']").loading("show");
        var self = this;
        let defer = $.Deferred();

        this.saveUrl = url || this.saveUrl;
        //prepare post table
        this.method = "save";
        let ntable = new WLIU.JSTable(this);
        ntable.AddRow(this.GuidRow(guid));
        if (ntable.rows.length <= 0) {
            //$("#wliuHint").hint("show", "warning", Words("save.nothing"));
            this.TableAjaxErrorHandle();
            this.RowAjaxErrorHandle(guid);
            if (self.HasError(guid) === false)
                defer.resolve(self);
            else
                defer.reject(self);

            $("div[wliu][loading][method='save']").loading("hide");
            return defer.promise();
        }
        this.navi.isLoading = 1;
        //create promise
        this.TableAjax(this.saveUrl, ntable).then(data => {
            self.state = 0;
            self.navi.isLoading = 0;
            self.SyncSaveTable(data);
            self.ChangeState();

            let affectCount = 0;
            if (data.rows) if (data.rows.length > 0)
                affectCount = data.rows.length;

            self.TableAjaxErrorHandle(affectCount);
            self.RowAjaxErrorHandle(guid, affectCount);

            if (self.HasError(guid) === false)
                defer.resolve(self);
            else
                defer.reject(self);

        }).catch(data => {
            self.state = 0;
            self.navi.isLoading = 0;
            self.ChangeState();
            self.error = new WLIU.Error(data.error);

            self.TableAjaxErrorHandle();

            defer.reject(self);
        });
        return defer.promise();
    },
    SaveAll: function (url) {
        var self = this;
        let defer = $.Deferred();

        this.saveUrl = url || this.saveUrl;
        //prepare post table
        this.method = "save";
        let ntable = new WLIU.JSTable(this);
        ntable.AddRows(this.rows);
        if (ntable.rows.length <= 0) {
            //$("#wliuHint").hint("show", "warning", Words("save.nothing"));
            self.TableAjaxErrorHandle();
            self.RowAjaxErrorHandle(self.CurrentGuid());
            if (self.HasError(self.CurrentGuid()) === false)
                defer.resolve(self);
            else
                defer.reject(self);

            defer.resolve(self);
            return defer.promise();
        }
        this.navi.isLoading = 1;

        //create promise
        $("div[wliu][loading][method='save']").loading("show");
        this.TableAjax(this.saveUrl, ntable).then(data => {
            self.state = 0;
            self.navi.isLoading = 0;
            self.SyncSaveTable(data);
            self.ChangeState();

            let affectCount = 0;
            if (data.rows) if (data.rows.length > 0)
                affectCount = data.rows.length;

            self.TableAjaxErrorHandle(affectCount);
            self.RowAjaxErrorHandle(self.CurrentGuid(), affectCount);

            if (self.HasError(self.CurrentGuid()) === false)
                defer.resolve(self);
            else
                defer.reject(self);

        }).catch(data => {
            self.state = 0;
            self.navi.isLoading = 0;

            self.ChangeState();
            self.error = new WLIU.Error(data.error);

            self.TableAjaxErrorHandle();

            defer.reject(self);
        });
        return defer.promise();
    },
    Cancel: function (guid) {
        if (this.GuidRow(guid)) {
            this.GuidRow(guid).Cancel();
            this.ChangeState();

            this.CKEditorReset();

            return this.GuidRow(guid);
        }
    },
    CancelAll: function () {
        if ($.isArray(this.rows)) {
            for (let rowIndex = this.rows.length - 1; rowIndex >= 0; rowIndex--) {
                switch (this.IndexRow(rowIndex).state) {
                    case 0:
                        break;
                    case 1:
                    case 3:
                        this.IndexRow(rowIndex).Cancel();
                        this.CKEditorReset();
                        break;
                    case 2:
                        $.removeByIndex(this.rows, rowIndex);
                        this.CKEditorReset();
                        break;
                }
            }
            this.ChangeState();
        }
    },
    Remove: function (guid) {
        if (this.rows) {
            if (this.CurrentGuid() === guid) {
                $.removeInList(this.rows, { guid: guid });
                this.rowGuid = "";
            } else {
                $.removeInList(this.rows, { guid: guid });
            }
            this.CKEditorReset();
            this.ChangeState();
            return this.GuidRow(guid);
        }
    },
    EmptyRemove: function (guid) {
        if (this.rows) {
            if (this.CurrentGuid() === guid) {
                $.removeInList(this.rows, { guid: guid });
                this.rowGuid = "";
            } else {
                $.removeInList(this.rows, { guid: guid });
            }
            //this.CKEditorReset();
            this.ChangeState();
            return this.GuidRow(guid);
        }
    },
    Empty: function () {
        if (this.CurrentRow()) {
            if (this.CurrentRow().error.HasError() === false) {
                this.EmptyCurrent({});
            }
        }
    },
    EmptyCurrent: function (kvs) {
        if (this.CurrentRow()) {
            if (this.CurrentRow().error.HasError() === false) {
                this.EmptyRemove(this.CurrentGuid());
                if (this.rows.length > 0)
                    this.rowGuid = this.rows[0].guid;
                else
                    this.NewRowB(kvs);

                this.CKEditorReset();
            }
        } else {
            this.NewRowB(kvs);
            this.CKEditorReset();
        }
    },
    Detach: function (guid) {
        if (this.GuidRow(guid)) {
            this.GuidRow(guid).Detach();
            this.ChangeState();
            return this.GuidRow(guid);
        }
    },
    HasError: function (guid) {
        let isError = false;
        if (this.error.HasError()) isError = true;
        if (this.GuidRow(guid))
            if (this.GuidRow(guid).error.HasError())
                isError = true;
        return isError;
    },

    // Table Data Ajax
    TableAjax: function (url, ntable) {
        if (url !== "") {
            return AJAX.Post(url, ntable);
        } else {
            let defer = $.Deferred();
            let resp = {};
            resp.error = {};
            resp.error.code = 404;
            resp.error.messages = [Words("table.save.failed")];
            defer.reject(resp);
            return defer.promise();
        }
    },
    TableAjaxErrorHandle: function (rowCount) {
        var self = this;
        if (self.method === "save")
            $("div[wliu][loading][method='save']").loading("hide");
        else
            $("div[wliu][loading][method='get']").loading("hide");

        if (self.error.HasError()) {
            $("#wliuHint").hint("show", "error", (self.method === "save" ? Words("save.error") : Words("get.error")));
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
            self.method = "none";
        }
        else {
            if (self.method === "save" && rowCount > 0) $("#wliuHint").hint("show", "success", Words("save.success"));
            self.method = "none";
        }
    },
    RowAjaxErrorHandle: function (guid, rowCount) {
        var self = this;
        if (self.GuidRow(guid)) {
            if (self.GuidRow(guid).error.HasError()) {
                $("#wliuHint").hint("show", "error", Words("save.error"));
                $("#wliuDiagFormError").diag({
                    content: [
                        '<center>' + Words("error.remind") + '</center>',
                        '<span style="color:orange;">' + Words("error.code") + '</span> : <span style="color:orange;">' + self.GuidRow(guid).error.code + '</span><br>',
                        self.GuidRow(guid).error.Message().nl2br()
                    ].join('')
                }).diag("show");
            }
            else {
                if (self.method === "save" && rowCount > 0) $("#wliuHint").hint("show", "success", Words("save.success"));
            }
        }
    },
    SyncGetTable: function (gtable) {
        if (gtable) {
            this.refKey = gtable.refKey;
            this.rowGuid = gtable.rowGuid;

            let getError = false;

            // Sync Error
            // Notes:  back-end code logic:  filter error will add to table error  to prevent query data.
            this.error = new WLIU.Error(gtable.error);
            if (this.error.HasError()) getError = true;

            if (gtable && gtable.other) {
                for (let fName in gtable.other) {
                    if (gtable.other[fName]) this.other[fName] = gtable.other[fName];
                }
            }

            if (gtable && gtable.filters) {
                for (let fName in gtable.filters) {
                    if (this.filters[fName]) {
                        this.filters[fName].error = new WLIU.Error(gtable.filters[fName].error);
                        if (this.filters[fName].error.HasError()) getError = true;
                    }
                }
            }

            if (getError === false) {
                // If get data no error,  sync navi and add rows
                this.navi = new WLIU.Navi(gtable.navi);
                this.AddORows(gtable.rows);
            }
            // if get data error,  only update error (filter and table) above,  no other update.
        }

    },
    SyncSaveTable: function (gtable) {
        //this.refKey = gtable.refKey;
        //this.rowGuid = gtable.rowGuid;
        if (gtable && gtable.other) {
            for (let fName in gtable.other) {
                if (gtable.other[fName]) this.other[fName] = gtable.other[fName];
            }
        }

        if (gtable && $.isArray(gtable.rows)) {
            if (gtable.error.code > 0)
                this.error = new WLIU.Error(gtable.error);
            else
                this.error = new WLIU.Error();

            for (let rowIdx = 0; rowIdx < gtable.rows.length; rowIdx++) {
                let ogrow = gtable.rows[rowIdx];
                let row = $.findInArray(this.rows, { guid: ogrow.guid });
                if (ogrow && row) {
                    row.ResetError();
                    row.error.AppendError(ogrow.error);
                    if ($.isPlainObject(ogrow.errors)) {
                        for (let colName in ogrow.errors) {
                            row.AddColumnError(colName, ogrow.errors[colName]);
                        }
                    }
                    row.UpdateError();

                    switch (row.state) {
                        case 0:
                            break;
                        case 1:
                            if (ogrow.columns) {
                                for (let colName in ogrow.columns) {
                                    let ogcol = ogrow.columns[colName];
                                    let col = row.columns[colName];
                                    if (ogcol && col) {
                                        if (row.ColHasError(colName)==false) {
                                            col.state = 0;
                                            //wrong logic for row error
                                            if (row.error.HasError() === false) col.current = Clone(col.value);
                                        }
                                    }

                                    if (this.metas[colName] && this.metas[colName].sync) {
                                        col.value = Clone(ogcol.value);
                                        col.current = Clone(col.value);
                                    }
                                }
                            }
                            break;
                        case 2:
                            row.key = ogrow.key;
                            if (ogrow.columns) {
                                for (let colName in ogrow.columns) {
                                    let ogcol = ogrow.columns[colName];
                                    let col = row.columns[colName];
                                    let meta = this.metas[colName];
                                    if (ogcol && col && meta) {
                                        if (row.ColHasError(colName) == false) {
                                            if (meta.isKey) {
                                                col.value = Clone(ogcol.value);
                                            }
                                            col.state = 0;
                                            if (row.error.HasError() === false) col.current = Clone(col.value);
                                        }
                                    }

                                    if (this.metas[colName] && this.metas[colName].sync) {
                                        col.value = ogcol.value;
                                        col.current = Clone(col.value);
                                    }
                                }
                            }

                            if (row.error.HasError() === false) this.navi.rowTotal++;
                            break;
                        case 3:
                            if (row.error.HasError() == false) {
                                $.removeInList(this.rows, { guid: ogrow.guid });
                                this.navi.rowTotal--;
                            }
                            break;
                    }
                    if (row.error.HasError() === false) row.state = 0;
                }
            }
        }
    },
    SyncValidate: function (gtable) {
        if (gtable && $.isArray(gtable.rows)) {
            if (gtable.error.code > 0)
                this.error = new WLIU.Error(gtable.error);
            else
                this.error = new WLIU.Error();

            for (let rowIdx = 0; rowIdx < gtable.rows.length; rowIdx++) {
                let ogrow = gtable.rows[rowIdx];
                let row = $.findInArray(this.rows, { guid: ogrow.guid });
                if (ogrow && row) {
                    row.ResetError();
                    row.error.AppendError(ogrow.error);
                    if ($.isPlainObject(ogrow.errors)) {
                        for (let colName in ogrow.errors) {
                            row.AddColumnError(colName, ogrow.errors[colName]);
                        }
                    }
                    row.UpdateError();
                }
            }
        }
    }
};

WLIU.Navi = function (navi) {
    if ($.isPlainObject(navi)) {
        this.pageNo = navi.pageNo || 0;
        this.pageSize = navi.pageSize || 20;
        this.pageTotal = navi.pageTotal || 0;
        this.rowTotal = navi.rowTotal || 0;
        this.isLoading = navi.isLoading || false;
        this.order = navi.order || "ASC";
        this.by = navi.by || "";
        this.isActive = navi.isActive || false;
    }
    else {
        this.pageNo = 0;
        this.pageSize = 20;
        this.pageTotal = 0;
        this.rowTotal = 0;
        this.isLoading = false;
        this.order = "ASC";
        this.by = "";
        this.isActive = false;
    }
};
WLIU.Meta = function (meta) {
    if ($.isPlainObject(meta)) {
        this.isKey = meta.isKey || false;
        this.order = meta.order || "";
        this.value = meta.value || "";
        this.name = meta.name || "";
        this.sync = meta.sync || false;
        this.title = meta.title || "";
        this.description = meta.description || "";
        this.required = meta.required || false;
        this.listRef = new WLIU.ListRef(meta.listRef);
    }
    else {
        this.isKey = false;
        this.order = "";
        this.value = "";
        this.name = "";
        this.sync = false;
        this.title = "";
        this.description = "";
        this.required = false;
        this.listRef = new WLIU.ListRef();
    }
};
WLIU.Filter = function (filter) {
    if ($.isPlainObject(filter)) {
        this.value1 = filter.value1 || "";
        this.value2 = filter.value2 || "";
        this.name = filter.name || "";
        this.title = filter.title || "";
        this.description = filter.description || "";
        this.required = filter.required || false;
        this.error = new WLIU.Error(filter.error);
        this.listRef = new WLIU.ListRef(filter.listRef);
    }
    else {
        this.value1 = "";
        this.value2 = "";
        this.name = "";
        this.title = "";
        this.description = "";
        this.required = false;
        this.error = new WLIU.Error();
        this.listRef = new WLIU.ListRef();
    }
};
WLIU.Filter.prototype = {
    Change: function () {
        this.error.Reset();
        return this;
    }
};

WLIU.ListRef = function (ref) {
    if ($.isPlainObject(ref)) {
        this.collection = ref.collection || "";
    }
    else {
        this.collection = "";
    }
};

WLIU.Row = function (row) {
    if ($.isPlainObject(row)) {
        this.key = row.key || 0;
        this.state = row.state || 0;
        this.guid = row.guid || CreateUUID();
        this.error = new WLIU.Error(row.error);
        this.errors = {};
        if ($.isPlainObject(row.errors)) {
            for (let colName in row.errors)
                this.errors[colName] = new WLIU.Error(row.errors[colName]); 
        }
        this.columns = {};
        this.AddOColumns(row.columns);
    }
    else {
        this.key = 0;
        this.state = 0;
        this.guid = CreateUUID();
        this.error = new WLIU.Error();
        this.errors = {};
        this.columns = {};
    }
};
WLIU.Row.prototype = {
    AddColumn: function (column) {
        this.columns[column.name] = column;
    },
    AddOColumn: function (column) {
        this.columns[column.name] = new WLIU.Column(column);
        return this;
    },
    AddColumnKV: function (name, value) {
        let column = new WLIU.Column();
        column.name = name;
        column.value = value;
        this.columns[name] = column;
    },
    AddColumns: function (columns) {
        this.columns = {};
        for (let colname in columns) {
            this.AddColumn(columns[colname]);
        }
    },
    AddOColumns: function (columns) {
        this.columns = {};
        if ($.isPlainObject(columns)) {
            for (let colname in columns) {
                this.AddOColumn(columns[colname]);
            }
        }
    },
    Change: function (colName) {
        if (this.columns[colName]) {
            this.columns[colName].Change();
            if (this.errors[colName]) this.errors[colName].Reset();
            this.ChangeState();
        }
    },
    SetChange: function (colName) {
        if (this.columns[colName]) {
            this.columns[colName].SetChange();
            if (this.errors[colName]) this.errors[colName].Reset();
            this.ChangeState();
        }
    },
    ChangeState: function () {    // Row Error combine from  Columns Error 
        if (this.state <= 1) {
            let normal = true;
            for (let colName in this.columns) {
                if (this.columns[colName].state === 1) normal = false;
            }
            if (normal) this.state = 0; else this.state = 1;
        }
        this.error.Reset();
        for (let colName in this.errors) {
            if (this.errors[colName].HasError()) {
                this.error.code = Math.max(this.errors[colName].code, this.error.code);
                this.error.messages.push(this.errors[colName].Message());
            }
        }
    },
    ResetError: function () {
        this.error.Reset();
        this.errors = {};
    },
    AddColumnError: function (colName, oError) {
        this.errors[colName] = new WLIU.Error(oError);
    },
    UpdateError: function () {
        for (let colName in this.errors) {
            if (this.errors[colName].HasError()) {
                this.error.code = Math.max(this.errors[colName].code, this.error.code);
                this.error.messages.push(this.errors[colName].Message());
            }
        }
    },
    ColHasError: function (colName) {
        if (this.errors[colName])
            return this.errors[colName].HasError();
        else 
            return false;
    }, 
    Cancel: function () {
        for (let colName in this.columns) {
            this.columns[colName].Cancel();
        }
        if (this.state !== 2) this.state = 0;
        this.ResetError();
    },
    Detach: function () {
        if (this.state <= 1) this.state = 3;
    }
};

WLIU.Column = function (column) {
    if ($.isPlainObject(column)) {
        this.name = column.name;

        this.guid = column.guid || CreateUUID();
        this.state = column.state || 0;

        this.value = Clone(column.value);
        this.value1 = Clone(column.value1);
        this.current = Clone(this.value);
        this.current1 = Clone(this.value1);
    } else {
        this.name = "";

        this.guid = CreateUUID();
        this.state = 0;

        this.value = "";
        this.value1 = "";
        this.current = "";
        this.current1 = "";
    }
};
WLIU.Column.prototype = {
    Change: function () {
        if ($.isPlainObject(this.value)) {

            if (this.current === 0) this.current = {};
            if (this.current === "") this.current = {};
            if (this.current === null) this.current = {};
            if (this.current === undefined) this.current = {};

            var objectSame = true;
            for (let vkey in this.value) {
                let curVal = this.value[vkey] ? this.value[vkey] : false;
                let curCur = this.current[vkey] ? this.current[vkey] : false;
                if (this.Equal(curVal, curCur) === false) objectSame = false;

                if ($.isPlainObject(this.value[vkey])) objectSame = false;
            }
            for (let vkey in this.current) {
                let curVal = this.value[vkey] ? this.value[vkey] : false;
                let curCur = this.current[vkey] ? this.current[vkey] : false;
                if (this.Equal(curVal, curCur) === false) objectSame = false;

                if ($.isPlainObject(this.current[vkey])) objectSame = false;
            }
            if (objectSame)
                this.state = 0;
            else
                this.state = 1;

        } else {
            if (this.Equal(this.value, this.current))
                this.state = 0;
            else
                this.state = 1;
        }
        return this;
    },
    Equal: function (val, cur) {
        if (val === 0) val = false;
        if (val === null) val = false;
        if (val === undefined) val = false;
        if (val === "") val = false;

        if (cur === 0) cur = false;
        if (cur === null) cur = false;
        if (cur === undefined) cur = false;
        if (cur === "") cur = false;

        if (val !== cur)
            return false;
        else
            return true;
    },
    Cancel: function () {
        this.state = 0;
        this.value = Clone(this.current);
        this.value1 = Clone(this.current1);
    },
    SetChange: function () {
        this.state = 1;
    }
};

WLIU.Error = function (error) {
    if ($.isPlainObject(error)) {
        this.code = error.code || 0;
        this.messages = error.messages || [];
        this.memo = error.memo || "";
    }
    else {
        this.code = 0;
        this.messages = [];
        this.memo = "";
    }
};
WLIU.Error.prototype = {
    Reset: function () {
        this.code = 0;
        this.messages = [];
        this.memo = "";
    },
    HasError: function () {
        if (this.code > 0)
            return true;
        else
            return false;
    },
    Count: function () {
        return this.messages.length;
    },
    Message: function () {
        return this.messages.join("\n");
    },
    Clear: function () {
        this.code = 0;
        this.messages = [];
        this.memo = "";
    },
    Append: function (code, message, memo) {
        this.code = Math.max(this.code, code);
        if (this.code > 0) {
            this.messages.push(message);
            if (code > this.code) this.memo = memo || this.memo;
        }
    },
    AppendError: function (oError) {
        if ($.isPlainObject(oError)) {
            oError.code = oError.code || 0;
            oError.message = $.isArray(oError.messages) ? oError.messages.join("\n") : "";
            oError.memo = oError.memo || "";
            this.Append(oError.code, oError.message, oError.memo);      
        }
    }
};

WLIU.JSTable = function (table) {
    if (table) {
        this.name = table.name || "unknown";
        this.method = table.method || "none";
        this.state = table.state || 0;
        this.refKey = table.refKey || 0;
        this.rowGuid = table.rowGuid || "";
        this.navi = {};
        this.rows = [];
        if (this.method === "get") {
            this.CloneNavi(table);
            this.filters = table.filters || {};
        }
        if (this.method === "save") {
            //this.metas = table.metas;
            //this.AddRows(table.rows);
        }
        if (this.method === "validate") {
            // no implement
        }
        this.other = table.other;
    }
    else {
        this.name = "unknown";
        this.state = 0;
        this.refKey = 0;
        this.rowGuid = "";
        this.method = "none";
        this.navi = {};
        this.filters = {};
        this.rows = [];
        this.other = {};
    }
};
WLIU.JSTable.prototype = {
    AddRow: function (row) {
        if (row.error.HasError() === false && row.state > 0)
            this.rows.push(new WLIU.JSRow(row));
        return this;
    },
    AddRows: function (rows) {
        this.rows = [];
        if ($.isArray(rows)) {
            for (let rowIdx in rows) {
                this.AddRow(rows[rowIdx]);
            }
        }
        return this;
    },
    CloneNavi: function (table) {
        if (table && table.navi) {
            this.navi.pageNo = table.navi.pageNo;
            this.navi.pageSize = table.navi.pageSize;
            this.navi.pageTotal = table.navi.pageTotal;
            this.navi.rowTotal = table.navi.rowTotal;
            this.navi.isLoading = table.navi.isLoading;
            this.navi.order = table.navi.order;
            this.navi.by = table.navi.by;
            this.navi.isActive = table.navi.isActive;
        }
    }
};
WLIU.JSRow = function (row) {
    if (row) {
        this.key = row.key || 0;
        this.guid = row.guid || CreateUUID();
        this.state = row.state || 0;
        this.AddColumns(row.columns);
    }
    else {
        this.key = row.key || 0;
        this.guid = row.guid || CreateUUID();
        this.state = row.state || 0;
        this.columns = {};
    }
};
WLIU.JSRow.prototype = {
    AddColumn: function (column) {
        switch (this.state) {
            case 0:
                break;
            case 1:
                if (column.state==1) this.columns[column.name] = new WLIU.JSColumn(column);
                break;
            case 2:
                this.columns[column.name] = new WLIU.JSColumn(column);
                break;
            case 3:
                break;
        }
    },
    AddColumns: function (columns) {
        this.columns = {};
        for (let colname in columns) {
            this.AddColumn(columns[colname]);
        }
    }
};
WLIU.JSColumn = function (column) {
    if (column) {
        this.name = column.name;
        this.value = Clone(column.value);
        this.value1 = Clone(column.value1);
        this.guid = column.guid || CreateUUID();
        this.state = column.state || 0;
    }
    else {
        this.name = "";
        this.value = "";
        this.value1 = "";
        this.guid = CreateUUID();
        this.state = 0;
    }
};

var AJAX = {
    Get: function (url, callback) {
        return this.Call(url, "get", callback);
    },
    Post: function (url, data, callback) {
        return this.Call(url, "post", data, callback);
    },
    Call: function (url, method, data, callback) {
        let defer = $.Deferred();
        $.ajax({
            dataType: 'json',
            data: JSON.stringify(data),
            /*
            xhr: function () {
                var xhr = new window.XMLHttpRequest();
                // Upload progress
                xhr.upload.addEventListener("progress", function (evt) {
                    if (evt.lengthComputable) {
                        var percentComplete = ((evt.loaded / evt.total).toFixed(2)) * 100 ;
                        if(callback) if($.isFunction(callback)) callback(evt.loaded, evt.total, percentComplete);
                    }
                }, false);

                // Download progress
                xhr.addEventListener("progress", function (evt) {
                    if (evt.lengthComputable) {
                        var percentComplete = ((evt.loaded / evt.total).toFixed(2)) * 100;
                        if(callback) if($.isFunction(callback)) callback(evt.loaded, evt.total, percentComplete);
                    }
                }, false);

                return xhr;
            },
            */
            beforeSend: function (xhr) {   //Include the bearer token in header
                xhr.setRequestHeader("Authorization", 'Bearer ' + GAdminSiteJwtToken);
                xhr.setRequestHeader("AdminSession", GAdminSiteSession);
            },
            /*
            beforeSend: function (xhr) {
                xhr.setRequestHeader("XSRF-TOKEN",
                    $('input:hidden[name="__RequestVerificationToken"]').val());
            },
            */
            //contentType: "application/x-www-form-urlencoded",
            contentType: "application/json; charset=utf-8",
            error: function (xhr, tStatus, errorTh) {
                let resp = {};

                resp.status = xhr.status;
                resp.statusText = xhr.statusText;
                resp.data = xhr.responseJSON;
                switch (resp.status) {
                    case 401:
                    case 404:
                        resp.error = { code: 1000, hasError: true, message: "URL Not Found!", messages: [] };
                        break;
                    case 200:
                    case 201:
                        resp.error = { code: 0, hasError: "", message: "", messages: [] };
                        break;
                    default:
                        resp.error = resp.data.error ? resp.data.error : {};
                        break;
                }

                defer.reject(resp);
            },
            success: function (respData, tStatus) {
                defer.resolve(respData);
            },
            type: method,
            url: url
        });
        return defer.promise();
    }
}
