using Library.V1.Common;
using Library.V1.SQL;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;


namespace Library.V1.Entity
{
    public enum EState
    {
        Normal = 0,
        Changed = 1,
        Added = 2,
        Deleted = 3
    }
    public enum ESource
    {
        Table = 0,
        StoreProcedure=1
    }
    public class Table
    {
        #region Constructors
        public Table()
        {
            this.Name = string.Empty;
            this.DbName = string.Empty;
            this.Title = string.Empty;
            this.Description = string.Empty;
            this.State = 0;
            this.Method = string.Empty;
            this.Error = new Error();
            this.Navi = new Navi();
            this.Metas = new Dictionary<string, Meta>();
            this.Filters = new Dictionary<string, Filter>();
            this.Relation = new Relation();
            this.RowGuid = Guid.Empty;
            this.QueryKVs = new Dictionary<string, object>();
            this.UpdateKVs = new Dictionary<string, object>();
            this.InsertKVs = new Dictionary<string, object>();
            this.DeleteKVs = new Dictionary<string, object>();
            this.GetUrl = string.Empty;
            this.SaveUrl = string.Empty;
            this.ValidateUrl = string.Empty;
            this.Rows = new List<Row>();
            this.Other = new Dictionary<string, object>();
            this.DSQL = new SqlHelper();
            this.FSQL = new SqlHelper();
            this.User = new WebUser();
            this.Source = ESource.Table;
            this.Index = -1;
        }
        public Table(string name, string dbName, string title = "", string desc = "", ESource source=ESource.Table) : this()
        {
            this.Name = name;
            this.DbName = dbName;
            this.Title = title;
            this.Description = desc;
            this.Source = source;

            List<string> restrictFields = new List<string>();
            foreach (string metaName in this.Metas.Keys)
            {
                if (this.User.Fields.Contains(metaName))
                    restrictFields.Add(metaName);
            }
            foreach (string rField in restrictFields)
            {
                if (this.Metas.ContainsKey(rField))
                    this.Metas.Remove(rField);
            }
        }
        #endregion

        #region Public Fields
        public string Name { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int State { get; set; }
        public string Method { get; set; }
        public int RefKey { 
            get
            {
               return this.Relation.RefKey;
            }
            set 
            {
                this.Relation.RefKey = value;
            }
        }
        public Guid RowGuid { get; set; }
        public string GetUrl { get; set; }
        public string SaveUrl { get; set; }
        public string ValidateUrl { get; set; }
        public string Debug { get; set; }
        #endregion

        #region Public Objects
        public Navi Navi { get; set; }
        public IDictionary<string, Meta> Metas { get; set; }
        public IDictionary<string, Filter> Filters { get; set; }
        public IList<Row> Rows { get; set; }
        public Dictionary<string, object> Other { get; set; }
        public Error Error { get; set; }
        #endregion

        #region Private
        private string DbName { get; set; }
        private SqlHelper DSQL { get; set; }
        private SqlHelper FSQL { get; set; }
        private WebUser User { get; set; }
        private ESource Source { get; set; }
        private int Index { get; set; }
        private Relation Relation { get; set; }
        private Dictionary<string, object> QueryKVs { get; set; }
        private Dictionary<string, object> UpdateKVs { get; set; }
        private Dictionary<string, object> InsertKVs { get; set; }
        private Dictionary<string, object> DeleteKVs { get; set; }
        private Meta KeyMeta
        {
            get
            {
                foreach (string key in this.Metas.Keys)
                    if (this.Metas[key].IsKey) return this.Metas[key];

                return null;
            }
        }
        private SQLRow SQLRowGet
        {
            get
            {
                SQLRow sqlRow = new SQLRow();
                foreach (string metaName in this.Metas.Keys)
                {
                    if (this.Metas[metaName].AllowGet)
                    {
                        var jsName = this.Metas[metaName].Name;
                        var dbName = string.IsNullOrWhiteSpace(this.Metas[metaName].DbName) ? jsName : this.Metas[metaName].DbName;
                        
                        if(this.Source==ESource.Table) 
                            dbName = this.Metas[metaName].IsLang ? this.DSQL.LangSmartColumn(dbName) : dbName;
                        if(this.Source==ESource.StoreProcedure) 
                            dbName = this.Metas[metaName].IsLang ? this.DSQL.LangColumn(dbName) : dbName;

                        sqlRow.Add(dbName, jsName);
                    }

                }
                return sqlRow;
            }
        }
        private SQLWhere WhereGet
        {
            get
            {
                //1. QueryKVs
                SQLWhere sw = new SQLWhere();
                foreach (string k in this.QueryKVs.Keys)
                {
                    sw.Add(k, this.QueryKVs[k]);
                }
                //2. All Filters
                foreach (string fname in this.Filters.Keys)
                {
                    this.Filters[fname].Validate();
                    this.Error.Append(this.Filters[fname].Error);
                    sw.Add(this.Filters[fname].WhereSearch);
                }
                //3. Relation: o2o, o2m
                sw.Add(this.Relation.WhereSearchGet);
                return sw;
            }
        }
        private List<SqlParameter> SqlParams { 
            get 
            {
                List<SqlParameter> ps = new List<SqlParameter>();
                foreach (string fname in this.Filters.Keys)
                {
                    this.Filters[fname].Validate();
                    this.Error.Append(this.Filters[fname].Error);
                    ps.AddRange(this.Filters[fname].SqlParams);
                }
                return ps;
            }
        }
        private SQLRow SQLRowUpdate
        {
            get
            {
                SQLRow sqlRow = new SQLRow();
                sqlRow.Add(this.UpdateKVs);
                return sqlRow;
            }
        }
        private SQLWhere WhereUpdate
        {
            get
            {
                //1. QueryKVs
                SQLWhere sw = new SQLWhere();
                foreach (string k in this.QueryKVs.Keys)
                {
                    sw.Add(k, this.QueryKVs[k]);
                }

                //2. Hidden Filters
                foreach (string fname in this.Filters.Keys)
                {
                    if (this.Filters[fname].Type == EFilter.Hidden)
                    {
                        this.Filters[fname].Validate();
                        this.Error.Append(this.Filters[fname].Error);
                        sw.Add(this.Filters[fname].WhereSearch);
                    }
                }
                //3. Relation: o2o, o2m
                sw.Add(this.Relation.WhereSearchGet);
                return sw;

            }
        }
        private SQLRow SQLRowInsert
        {
            get
            {
                SQLRow sqlRow = this.Relation.SQLRowInsert;
                sqlRow.Add(this.InsertKVs);
                return sqlRow;
            }
        }
        private SQLRow SQLRowDelete
        {
            get
            {
                SQLRow sqlRow = new SQLRow();
                sqlRow.Add(this.DeleteKVs);
                sqlRow.Add("Deleted", "Deleted", true);
                return sqlRow;
            }
        }

        #endregion

        #region KVs & Filters Methods
        public void SetConfig(SqlHelper dsql, SqlHelper fsql, WebUser user)
        {
            this.DSQL = dsql;
            this.FSQL = fsql;
            this.User = user;
        }
        public Table AddQueryKV(string dbColName, object value)
        {
            this.QueryKVs.Add(dbColName, value);
            return this;
        }
        public Table AddQueryKVs(Dictionary<string, object> kvs)
        {
            foreach (var key in kvs.Keys) this.AddQueryKV(key, kvs[key]);
            return this;
        }
        public Table AddUpdateKV(string dbColName, object value)
        {
            this.UpdateKVs.Add(dbColName, value);
            return this;
        }
        public Table AddUpdateKVs(Dictionary<string, object> kvs)
        {
            foreach (var key in kvs.Keys) this.AddUpdateKV(key, kvs[key]);
            return this;
        }
        public Table AddInsertKV(string dbColName, object value)
        {
            this.InsertKVs.Add(dbColName, value);
            return this;
        }
        public Table AddInsertKVs(Dictionary<string, object> kvs)
        {
            foreach (var key in kvs.Keys) this.AddInsertKV(key, kvs[key]);
            return this;
        }
        public Table AddDeleteKV(string dbColName, object value)
        {
            this.DeleteKVs.Add(dbColName, value);
            return this;
        }
        public Table AddDeleteKVs(Dictionary<string, object> kvs)
        {
            foreach (var key in kvs.Keys) this.AddDeleteKV(key, kvs[key]);
            return this;
        }

        public Table AddFilter(Filter filter)
        {
            this.Filters.Add(filter.Name, filter);
            return this;
        }
        public Table AddFilters(params Filter[] filters)
        {
            foreach (Filter filter in filters) this.AddFilter(filter);
            return this;
        }
        public Table AddMeta(Meta meta)
        {
            if (this.User.Fields.Contains(meta.Name) == false)
                this.Metas.Add(meta.Name, meta);
            return this;
        }
        public Table AddMetas(params Meta[] metas)
        {
            foreach (Meta meta in metas) this.AddMeta(meta);
            return this;
        }
        public Table AddRelation(Relation relation)
        {
            this.Relation = relation;
            return this;
        }
        #endregion

        #region Row Methods
        public Table AddRow(Row row)
        {
            this.Rows.Add(row);
            return this;
        }
        public Table AddRows(params Row[] rows)
        {
            foreach (Row row in Rows) this.AddRow(row);
            return this;
        }
        public Table AddRow(GRow grow, int rowCount)
        {
            Row nrow = new Row();
            foreach (var colName in this.Metas.Keys)
            {
                string dbColName = colName;
                if(this.Source==ESource.StoreProcedure) 
                    dbColName = this.Metas[colName].IsLang ? this.DSQL.LangColumn(this.Metas[colName].DbName) : this.Metas[colName].DbName;
 
                if (this.Metas[colName].IsKey) nrow.Key = grow.GetValue(dbColName).GetInt()??-1;
                Column ncolumn = new Column(colName);
                switch (this.Metas[colName].Type)
                {
                    case EInput.Hidden:
                        break;
                    case EInput.Object:
                    case EInput.String:
                    case EInput.Email:
                        ncolumn.Value = grow.GetValue(dbColName);
                        nrow.AddColumn(ncolumn);
                        break;
                    case EInput.Int:
                        ncolumn.Value = grow.GetValue(dbColName).GetInt();
                        nrow.AddColumn(ncolumn);
                        break;
                    case EInput.Long:
                        ncolumn.Value = grow.GetValue(dbColName).GetLong();
                        nrow.AddColumn(ncolumn);
                        break;
                    case EInput.Float:
                        ncolumn.Value = grow.GetValue(dbColName).GetFloat();
                        nrow.AddColumn(ncolumn);
                        break;
                    case EInput.Bool:
                        ncolumn.Value = grow.GetValue(dbColName).GetBool();
                        nrow.AddColumn(ncolumn);
                        break;
                    case EInput.Date:
                        ncolumn.Value = grow.GetValue(dbColName).GetDate();
                        nrow.AddColumn(ncolumn);
                        break;
                    case EInput.DateTime:
                        ncolumn.Value = grow.GetValue(dbColName).GetDate();
                        ncolumn.Value1 = grow.GetValue(dbColName).GetTime();
                        nrow.AddColumn(ncolumn);
                        break;
                    case EInput.Time:
                        ncolumn.Value = grow.GetValue(dbColName).GetTime(); 
                        nrow.AddColumn(ncolumn);
                        break;
                    case EInput.Password:  // never return password
                        ncolumn.Value = "";
                        nrow.AddColumn(ncolumn);
                        break;
                    case EInput.Passpair:
                        ncolumn.Value = "";  // never return password
                        ncolumn.Value1 = "";
                        nrow.AddColumn(ncolumn);
                        break;
                    case EInput.Read:           // only for get data but save data
                        ncolumn.Value = grow.GetValue(dbColName);
                        nrow.AddColumn(ncolumn);
                        break;
                    case EInput.ImageUrl:       // handle later
                    case EInput.ImageContent:   // handle later
                    case EInput.FileUrl:        // handle later
                    case EInput.FileContent:   // handle later
                    case EInput.Custom:         // handle later
                    case EInput.Checkbox:       // handle later
                        break;
                }
            }

            foreach (var colName in this.Metas.Keys)
            {
                switch (this.Metas[colName].Type)
                {
                    case EInput.Checkbox:
                        {
                            Column ckCol = this.Metas[colName].QueryCK(this.DSQL, nrow.Key);
                            nrow.AddColumn(ckCol);
                        }
                        break;
                    case EInput.ImageUrl:
                        {
                            string[] imageRef = this.Metas[colName].Description.Split('|');
                            string galleryName = imageRef[0]; // Description = "PubUser|Medium"
                            string imageSize = string.IsNullOrWhiteSpace(imageRef[1]) ? "Small" : imageRef[1];
                            string query = $"SELECT Id FROM GGallery WHERE Deleted=0 AND Active=1 AND GalleryName=@GalleryName";
                            int galleryId = this.DSQL.ExecuteScalar(query, new SqlParameter("@GalleryName", galleryName));

                            query = "SELECT TOP 1 Id, Guid FROM GImage WHERE Deleted=0 AND Active=1 AND GalleryId=@GalleryId AND RefKey=@RefKey ORDER BY Main DESC, Sort DESC, CreatedTime DESC";
                            Dictionary<string, object> ps = new Dictionary<string, object>();
                            ps.Add("GalleryId", galleryId);
                            ps.Add("RefKey", nrow.GetValue(this.Metas[colName].DbName) ?? 0);
                            IList<Dictionary<string, string>> imageRows = this.FSQL.Query(query, ps);
                            Column imgCol = new Column(this.Metas[colName].Name, "");
                            if (imageRows.Count > 0)
                                imgCol = new Column(this.Metas[colName].Name, $"/api/Image/GetImage/{imageRows[0]["Guid"].GetString()}/{imageSize}", imageRows[0]["Id"].GetInt() ?? 0);
                            nrow.AddColumn(imgCol);
                        }
                        break;
                    case EInput.ImageContent:
                        {
                            string query = $"SELECT Id FROM GGallery WHERE Deleted=0 AND Active=1 AND GalleryName=@GalleryName";
                            string[] imageRef = this.Metas[colName].Description.Split('|');
                            string galleryName = imageRef[0]; // Description = "PubUser|tiny|small"
                            int galleryId = this.DSQL.ExecuteScalar(query, new SqlParameter("@GalleryName", galleryName));
                            string imageSize = "Small";

                            if (imageRef.Length <= 2)
                            {
                                imageSize = imageRef[1].Capital(); // Only one size of image
                            }
                            else
                            {
                                if (rowCount > 1)  // multiple row use smaller size,  one row use larger size
                                    imageSize = imageRef[1].Capital();
                                else
                                    imageSize = imageRef[2].Capital();
                            }

                            query = $"SELECT TOP 1 Id, Guid, {imageSize} AS ImageContent FROM GImage WHERE Deleted=0 AND Active=1 AND GalleryId=@GalleryId AND RefKey=@RefKey ORDER BY Main DESC, Sort DESC, CreatedTime DESC";
                            Dictionary<string, object> ps = new Dictionary<string, object>();
                            ps.Add("GalleryId", galleryId);
                            ps.Add("RefKey", nrow.GetValue(this.Metas[colName].DbName) ?? 0);
                            IList<Dictionary<string, string>> imageRows = this.FSQL.Query(query, ps);
                            Column imgCol = new Column(this.Metas[colName].Name, "");
                            if (imageRows.Count > 0)
                                imgCol = new Column(this.Metas[colName].Name, imageRows[0]["ImageContent"].GetString(), $"/api/Image/GetImage/{imageRows[0]["Guid"].GetString()}");
                            nrow.AddColumn(imgCol);
                        }
                        break;
                }
            }
            this.AddRow(nrow);
            return this;

        }
        #endregion

        #region Table Row Operation
        public void IndexReset()
        {
            this.Index = -1;
        }
        public bool IndexFirst()
        {
            this.Index = -1;
            if (this.Rows.Count <= 0) return false;
            if (this.Index < this.Rows.Count) this.Index = 0;
            if (this.Index < this.Rows.Count)
                return true;
            else
                return false;

        }
        public bool IndexNext()
        {
            if (this.Rows.Count <= 0) return false;
            if (this.Index < this.Rows.Count) this.Index++;
            if (this.Index < this.Rows.Count)
                return true;
            else
                return false;
        }
        public Row IndexFetch()
        {
            return this.Rows[this.Index];
        }
        #endregion 

        #region Database Action Methods
        public void SyncJSTable(JSTable jsTable)
        {
            this.Name = jsTable.Name;
            this.Method = jsTable.Method;
            this.State = jsTable.State;

            this.RefKey = jsTable.RefKey;
            this.RowGuid = jsTable.RowGuid;
            this.Other = jsTable.Other;
            switch (this.Method)
            {
                case "get":
                    this.Navi.PageNo = jsTable.Navi.PageNo;
                    this.Navi.PageSize = jsTable.Navi.PageSize;
                    this.Navi.Order = jsTable.Navi.Order;
                    this.Navi.By = jsTable.Navi.By;
                    this.Navi.IsLoading = false;

                    foreach (var fname in jsTable.Filters.Keys)
                    {
                        if (this.Filters.ContainsKey(fname))
                        {
                            if (this.Filters[fname].Type != EFilter.Hidden)
                            {
                                this.Filters[fname].Value1 = jsTable.Filters[fname].Value1;
                                this.Filters[fname].Value2 = jsTable.Filters[fname].Value2;
                            }
                        }
                    }
                    break;
                case "save":
                    foreach(Row jrow in jsTable.Rows) this.Rows.Add(jrow);
                    break;
                case "validate":
                    foreach (Row jrow in jsTable.Rows) this.Rows.Add(jrow);
                    break;
            }
        }
        public Table ValidateTable(JSTable jsTable)
        {
            this.SyncJSTable(jsTable);
            this.ValidateRow();
            this.Metas.Clear();
            this.Navi = null;
            return this;
        }
        public void ValidateRow()
        {
            foreach(Row row in this.Rows)
            {
                // Validate Row Key Column
                switch (row.State)
                {
                    case EState.Normal:
                        break;
                    case EState.Added:
                        if(this.Relation.RefType== ERef.O2O || this.Relation.RefType==ERef.O2M)
                        {
                            string fkName = this.Relation.ForeignKey;
                            if (this.Metas.ContainsKey(this.Relation.ForeignKey)) fkName = this.Metas[this.Relation.ForeignKey].Title;
                            if(this.Relation.RefKey<=0) row.Error.Append(ErrorCode.ValueValidate, $"{LanguageHelper.Words("key.missing")} {fkName}");
                        }
                        break;
                    case EState.Changed:
                    case EState.Deleted:
                        // row key from row.key , not from key column
                        /*
                        if (row.Columns.ContainsKey(this.KeyMeta.Name) == false)
                            row.Error.Append(ErrorCode.ValueValidate, string.Format(LanguageHelper.Words("key.missing")));
                        if (row.Error.HasError == false && ValidateHelper.IsMatch(this.KeyMeta.Type.ToString(), row.GetValue(this.KeyMeta.Name)) == false)
                            row.Error.Append(
                                    ErrorCode.ValueValidate,
                                    string.Format(LanguageHelper.Words("validate.type"), this.Title, LanguageHelper.Words(this.KeyMeta.ToString().ToLower()))
                                    );
                        */
                        break;
                }

                // Validate Row 
                switch (row.State)
                {
                    case EState.Normal:
                        break;
                    case EState.Changed:
                        foreach (string colName in row.Columns.Keys)
                        {
                            if (this.Metas.ContainsKey(colName) == false) continue;
                            this.Metas[colName].Validate(row);
                            this.ValidateUnique(row, row.Columns[colName]);
                        }
                        break;
                    case EState.Added:
                        foreach (string colName in row.Columns.Keys)
                        {
                            if (this.Metas.ContainsKey(colName) == false) continue;
                            if (this.Metas[colName].IsKey) continue;
                            this.Metas[colName].Validate(row);
                            this.ValidateUnique(row, row.Columns[colName]);
                        }
                        break;
                    case EState.Deleted:
                        break;
                }
            }
        }
        private void ValidateUnique(Row row, Column colObj)
        {
            if (this.Metas[colObj.Name].Unique && ObjectHelper.IsEmpty(colObj.Value) == false)
            {
                string queryUnique = $"SELECT COUNT(1) AS CNT FROM {this.DbName} WHERE Deleted<>1 AND {this.KeyMeta.DbName}<>@KeyValue AND {this.Metas[colObj.Name].DbName}=@UniqueValue";
                int rowCount = this.DSQL.ExecuteScalar(
                    queryUnique,
                    new SqlParameter("@KeyValue", row.Key),
                    new SqlParameter("@UniqueValue", colObj.Value)
                );
                if (rowCount > 0)
                {
                    row.AppendError(colObj, ErrorCode.ValueValidate, string.Format(LanguageHelper.Words("validate.unique"), this.Metas[colObj.Name].Title));
                }
            }

        }

        public Table ReloadData(JSTable jsTable)
        {
            this.SyncJSTable(jsTable);
            this.Navi.InitFill = true;
            this.FillData();
            this.Metas.Clear();
            return this;

        }
        public Table FillData()
        {
            if (this.Navi.InitFill == false) return this;
            if (this.User.Rights.ContainsKey("view") == false) return this;
            if (this.User.Rights["view"] == false) return this;

            GTable gtable = new GTable();
            SQLRow sqlRow = this.SQLRowGet;
            if(sqlRow.ColumnCount>0)
            {
                switch(this.Source)
                {
                    case ESource.Table:
                        {
                            #region ESource.Table
                            SQLWhere sqlWhere = this.WhereGet;
                            if (this.DSQL.IsDebug) this.Debug = this.Debug.Concat($"|Cols:[{sqlRow.ColumnGet}]", @"\n\n");
                            if (this.DSQL.IsDebug) this.Debug = this.Debug.Concat($"|Where:[{sqlWhere.WhereGetFilter}]", @"\n\n");
                            if (this.DSQL.IsDebug) this.Debug = this.Debug.Concat($"|OrderBy:[{this.Navi.NaviOrderBy(this.Metas)}]", @"\n\n");

                            if (this.Error.HasError == false)
                            {
                                switch (this.Relation.RefType)
                                {
                                    case ERef.None:
                                    case ERef.O2M:
                                        if (this.Navi.IsActive)
                                        {
                                            int rowCount = this.DSQL.GetRowCount(this.DbName, sqlWhere);
                                            this.Navi.Reset(rowCount);
                                            gtable = this.DSQL.GetTable(this.DbName, sqlRow, sqlWhere, this.Navi.NaviOrderBy(this.Metas), this.Navi.NaviPaging());
                                            if (this.DSQL.IsDebug) this.Debug = this.Debug.Concat($"|Table:[{this.DSQL.Debug}]", @"\n\n");
                                        }
                                        else
                                        {
                                            int rowCount = this.DSQL.GetRowCount(this.DbName, sqlWhere);
                                            this.Navi.Reset(rowCount);
                                            gtable = this.DSQL.GetTable(this.DbName, sqlRow, sqlWhere, this.Navi.NaviOrderBy(this.Metas));
                                            if (this.DSQL.IsDebug) this.Debug = this.Debug.Concat($"|Table:[{this.DSQL.Debug}]", @"\n\n");
                                        }
                                        break;
                                    case ERef.O2O:
                                        {
                                            int rowCount = this.DSQL.GetRowCount(this.DbName, sqlWhere);
                                            this.Navi.Reset(rowCount);
                                            gtable = this.DSQL.GetTable(this.DbName, sqlRow, sqlWhere, this.Navi.NaviOrderBy(this.Metas), "", 1);
                                            if (this.DSQL.IsDebug) this.Debug = this.Debug.Concat($"|Table:[{this.DSQL.Debug}]", @"\n\n");
                                        }
                                        break;
                                }
                                this.Error.Append(this.DSQL.Error);
                                foreach (GRow grow in gtable.Rows) this.AddRow(grow, gtable.RowCount);
                                if (this.Rows.Count > 0) this.RowGuid = this.Rows[0].Guid;
                            }
                            #endregion
                        }
                        break;
                    case ESource.StoreProcedure:
                        {
                            if (this.DSQL.IsDebug) {
                                string debugStr = string.Empty;
                                this.SqlParams.ForEach(p => debugStr = debugStr.Concat($"{p.ParameterName}={p.Value}", ";"));
                                this.Debug = this.Debug.Concat($"|SqlParams:[{debugStr}]", @"\n\n");
                            }
                            gtable = this.DSQL.ExecuteSP(this.DbName, this.SqlParams.ToArray());
                            if (this.DSQL.IsDebug) this.Debug = this.Debug.Concat($"|StoreProcedure:[{this.DSQL.Debug}]", @"\n\n");
                            this.Error.Append(this.DSQL.Error);
                            foreach (GRow grow in gtable.Rows) this.AddRow(grow, gtable.RowCount);
                            if (this.Rows.Count > 0) this.RowGuid = this.Rows[0].Guid;
                        }
                        break;
                }
            }
            return this;
        }
        public Table SaveData(JSTable jsTable)
        {
            this.SyncJSTable(jsTable);
            this.ValidateRow();
            foreach (Row row in this.Rows)
            {
                if (this.Error.HasError == false && row.HasError == false)
                {
                    switch (row.State)
                    {
                        case EState.Normal:
                            break;
                        case EState.Changed:
                            {
                                if (this.User.Rights.ContainsKey("save") && this.User.Rights["save"])
                                {
                                    if (row.Columns.Count > 0)
                                    {
                                        //1. WhereUpdate
                                        SQLWhere sqlWhere = this.WhereUpdate;
                                        //2. Row Primary Key
                                        Filter keyFilter = new Filter() { Name = "RowKey", DbName = this.KeyMeta.DbName, Type = EFilter.Hidden, Required = true, Compare = ECompare.Equal, Value1 = row.Key };
                                        sqlWhere.Add(keyFilter.WhereSearch);

                                        // UpdateKVs
                                        SQLRow sqlRow = this.SQLRowUpdate;
                                        foreach (string colName in row.Columns.Keys)
                                        {
                                            if (this.Metas[colName].AllowSave && this.Metas[colName].IsKey == false)
                                            {
                                                string dbName = this.Metas[colName].IsLang ? this.DSQL.LangColumn(this.Metas[colName].DbName) : this.Metas[colName].DbName;
                                                sqlRow.Add(this.Metas[colName].DbName, this.Metas[colName].DbName, row.Columns[colName].Value);
                                            }
                                        }
                                        int result = this.DSQL.UpdateTable(this.DbName, sqlRow, sqlWhere);

                                        // need to add checkbox insert here
                                        #region checkbox handle
                                        foreach (string colName in row.Columns.Keys)
                                        {
                                            if (this.Metas[colName].IsKey) continue;
                                            if (this.Metas[colName].Type == EInput.Checkbox)
                                            {
                                                this.Metas[colName].DeleteCK(this.DSQL, row.Key);
                                                this.Metas[colName].InsertCK(this.DSQL, row.Key, row.Columns[colName].Value);
                                            }

                                            // don't need to bring the value back to client, except Custom
                                            if (this.Metas[colName].Type != EInput.Custom) row.Columns[colName].Value = "";
                                        }

                                        // handle sync back columns
                                        foreach (string colName in this.Metas.Keys)
                                        {
                                            if (this.Metas[colName].Sync)
                                            {
                                                string sync_dbName = this.Metas[colName].IsLang ? this.DSQL.LangColumn(this.Metas[colName].DbName) : this.Metas[colName].DbName;
                                                IList<Dictionary<string, string>> syncRows = this.DSQL.Query($"SELECT {sync_dbName} as {colName} FROM {this.DbName} WHERE {this.KeyMeta.DbName}={row.Key}", new Dictionary<string, object>());
                                                if (row.Columns.ContainsKey(colName))
                                                {
                                                    row.Columns[colName].Value = "";
                                                    if (syncRows.Count > 0) row.Columns[colName].Value = syncRows[0][colName];
                                                }
                                                else
                                                {
                                                    row.AddColumn(new Column(colName, syncRows[0][colName]));
                                                }

                                            }
                                        }
                                        #endregion

                                        if (this.DSQL.Error.HasError)
                                        {
                                            row.Error.Append(this.DSQL.Error);
                                            this.Error.Append(this.DSQL.Error);
                                        }
                                    }
                                }
                                else
                                {
                                    row.Error.Append(ErrorCode.UserRight, LanguageHelper.Words("rights.save.na"));
                                    this.Error.Append(row.Error);
                                }
                            }
                            break;
                        case EState.Added:
                            if (this.User.Rights.ContainsKey("add") && this.User.Rights["add"])
                            {
                                if (row.Columns.Count > 0)
                                {
                                    SQLRow sqlRow = this.SQLRowInsert;

                                    foreach (string colName in row.Columns.Keys)
                                    {
                                        if (this.Metas[colName].IsKey) continue;
                                        if (this.Metas[colName].AllowSave)
                                        {
                                            string dbName = this.Metas[colName].IsLang ? this.DSQL.LangColumn(this.Metas[colName].DbName) : this.Metas[colName].DbName;
                                            if(sqlRow.ContainCol(dbName)==false)
                                                sqlRow.Add(this.Metas[colName].DbName, this.Metas[colName].DbName, row.Columns[colName].Value);
                                        }
                                    }

                                    int result = this.DSQL.InsertTable(this.DbName, sqlRow);

                                    // need to add checkbox insert here
                                    #region checkbox handle
                                    foreach (string colName in row.Columns.Keys)
                                    {
                                        // handle checkbox
                                        if (this.Metas[colName].IsKey) continue;
                                        if (this.Metas[colName].Type == EInput.Checkbox)
                                        {
                                            this.Metas[colName].DeleteCK(this.DSQL, result);
                                            this.Metas[colName].InsertCK(this.DSQL, result, row.Columns[colName].Value);
                                        }

                                        // don't need to bring the value back to client
                                        if (this.Metas[colName].Type != EInput.Custom) row.Columns[colName].Value = "";

                                        // handle sync back column
                                        if (this.Metas[colName].Sync)
                                        {
                                            string sync_dbName = this.Metas[colName].IsLang ? this.DSQL.LangColumn(this.Metas[colName].DbName) : this.Metas[colName].DbName;
                                            IList<Dictionary<string, string>> syncRows = this.DSQL.Query($"SELECT {sync_dbName} as {colName} FROM {this.DbName} WHERE {this.KeyMeta.DbName}={result}", new Dictionary<string, object>());
                                            if (syncRows.Count > 0) row.Columns[colName].Value = syncRows[0][colName];
                                        }
                                    }
                                    #endregion

                                    if (this.DSQL.Error.HasError)
                                    {
                                        row.Error.Append(this.DSQL.Error);
                                        this.Error.Append(this.DSQL.Error);
                                    }
                                    else
                                    {
                                        row.Key = result;
                                        row.Columns[this.KeyMeta.Name].Value = row.Key;
                                    }
                                }
                            }
                            else
                            {
                                row.Error.Append(ErrorCode.UserRight, LanguageHelper.Words("rights.add.na"));
                                this.Error.Append(row.Error);
                            }
                            break;
                        case EState.Deleted:
                            {
                                if (this.User.Rights.ContainsKey("delete") && this.User.Rights["delete"])
                                {
                                    //1. WhereUpdate
                                    SQLWhere sqlWhere = this.WhereUpdate;
                                    //2. Row Primary Key
                                    Filter keyFilter = new Filter() { Name = "RowKey", DbName = this.KeyMeta.DbName, Type = EFilter.Hidden, Required = true, Compare = ECompare.Equal, Value1 = row.Key };
                                    sqlWhere.Add(keyFilter.WhereSearch);

                                    SQLRow sqlRow = this.SQLRowDelete;

                                    int result = this.DSQL.UpdateTable(this.DbName, sqlRow, sqlWhere);

                                    //int result = this.SQL.DetachTable(this.DbName, sqlWhere, sqlClause);
                                    if (this.DSQL.Error.HasError)
                                    {
                                        row.Error.Append(this.DSQL.Error);
                                        this.Error.Append(this.DSQL.Error);
                                    }
                                }
                                else
                                {
                                    row.Error.Append(ErrorCode.UserRight, LanguageHelper.Words("rights.delete.na"));
                                    this.Error.Append(row.Error);
                                }
                            }
                            break;
                    }
                }
            }

            this.Metas.Clear();
            this.Filters.Clear();
            this.Navi = null;
            return this;
        }
        #endregion
    }

    public class JSTable
    {
        #region Constructors
        public JSTable()
        {
            this.Name = "";
            this.Method = "";
            this.State = 0;
            this.RefKey = 0;
            this.RowGuid = Guid.Empty;
            this.Navi = new Navi();
            this.Filters = new Dictionary<string, Filter>();
            this.Rows = new List<Row>();
            this.Other = new Dictionary<string, object>();
        }
        public JSTable(string name) : this()
        {
            this.Name = name;
        }
        #endregion

        #region Public Fields
        public string Name { get; set; }
        public string Method { get; set; }
        public int State { get; set; }
        public int RefKey { get; set; }
        public Guid RowGuid { get; set; }
        public Navi Navi { get; set; }
        public IDictionary<string, Filter> Filters { get; set; }
        public IList<Row> Rows { get; set; }
        public Dictionary<string, object> Other { get; set; }
        #endregion
    }

}
