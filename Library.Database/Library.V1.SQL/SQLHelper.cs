using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Library.V1.Common;

namespace Library.V1.SQL
{
    public class SqlHelper : IDisposable
    {
        #region Constructor
        public SqlHelper()
        {
            this.SqlAccount = new SQLAccount();
            this.SqlConnect = new SqlConnection();
            this.SqlCommand = new SqlCommand();
            this.SqlAdpt = new SqlDataAdapter();
            this.Error = new Error();
            this.Switch = new Dictionary<string, object>();

        }
        public SqlHelper(SQLAccount sqlAcct) : this()
        {
            this.SqlAccount = sqlAcct;
        }
        #endregion

        #region Properties
        public string Lang
        {
            get
            {
                return this.SqlAccount.Lang;
            }
        }
        public bool IsDebug
        {
            get
            {
                return this.SqlAccount.IsDebug;
            }
        }
        public string Debug { get; set; }
        public Error Error { get; set; }
        #endregion

        #region Private Fields
        private SQLAccount SqlAccount { get; set; }
        private SqlConnection SqlConnect { get; set; }
        private SqlCommand SqlCommand { get; set; }
        private SqlDataAdapter SqlAdpt { get; set; }
        #endregion

        #region Public Dictionary Switch
        private Dictionary<string, object> Switch { get; set; }
        public object SwitchValue(string key)
        {
            return this.Switch.ContainsKey(key) ? this.Switch[key] : null;
        }
        public void AddSwitch(string key, object value)
        {
            if (this.Switch.ContainsKey(key))
                this.Switch[key] = value;
            else
                this.Switch.Add(key, value);
        }
        #endregion

        #region Common Methods
        public void Open()
        {
            try
            {
                this.Error.Reset();
                if (string.IsNullOrWhiteSpace(this.SqlAccount.ConnectString))
                {
                    this.Error.Append(ErrorCode.SQLConnection, "SQL connection string is empty, please add it to configuration file.");
                }
                if (!this.Error.HasError)
                {
                    if (this.SqlConnect.State == ConnectionState.Closed || this.SqlConnect.State == ConnectionState.Broken)
                    {
                        this.SqlConnect.ConnectionString = this.SqlAccount.ConnectString;
                        this.SqlConnect.Open();
                    }

                    if (this.SqlConnect.State != ConnectionState.Closed || this.SqlConnect.State != ConnectionState.Broken)
                    {
                        this.SqlCommand.Connection = this.SqlConnect;
                        this.SqlCommand.CommandTimeout = 3600;
                        this.SqlAdpt.SelectCommand = this.SqlCommand;
                    }
                    else
                    {
                        this.Error.Append(ErrorCode.SQLConnection, "SQL connection open failed.");
                    }
                }
            }
            catch (Exception err)
            {
                this.Error.Append(ErrorCode.SQLConnection, $"SQL Connection Open Failed: {err.Message}\nMessage: {err.StackTrace}");
            }
        }
        public void Close()
        {
            try
            {
                if (this.SqlConnect.State != ConnectionState.Closed || this.SqlConnect.State != ConnectionState.Broken)
                    this.SqlConnect.Close();
            }
            catch (Exception err)
            {
                this.Error.Append(ErrorCode.SQLConnection, $"SQL Connection Close Failed: {err.Message}\nMessage: {err.StackTrace}");
            }
        }
        public void Dispose()
        {
            this.SqlCommand.Dispose();
            this.SqlAdpt.Dispose();
            this.SqlConnect.Dispose();
        }
        public string LangColumn(string colName)
        {
            if (string.IsNullOrWhiteSpace(colName))
                return "";
            else
                return $"{colName}_{this.Lang}";
        }
        public string LangSmartColumn(string colName)
        {
            if (string.IsNullOrWhiteSpace(colName))
            {
                return "";
            }
            else
            {
                string retName = string.Empty;
                switch (this.Lang)
                {
                    case "cn":
                        {
                            string dbName = $"{colName}_cn";
                            string dbName1 = $"{colName}_en";
                            retName = $"ISNULL(IIF({dbName} = '', null, {dbName}), {dbName1})";
                        }
                        break;
                    case "en":
                        {
                            string dbName = $"{colName}_en";
                            string dbName1 = $"{colName}_cn";
                            retName = $"ISNULL(IIF({dbName} = '', null, {dbName}), {dbName1})";
                        }
                        break;
                    default:
                        {
                            string dbName = $"{colName}_en";
                            string dbName1 = $"{colName}_cn";
                            retName = $"ISNULL(IIF({dbName} = '', null, {dbName}), {dbName1})";
                        }
                        break;
                }
                return retName;
            }
        }
        #endregion


        #region SQL Execute
        public void ExecuteQuery(string query, params SqlParameter[] paramters)
        {
            try
            {
                this.Open();
                if (this.Error.HasError == false)
                {
                    this.SqlCommand.CommandText = query;
                    this.SqlCommand.CommandType = CommandType.Text;
                    this.SqlCommand.Parameters.Clear();
                    this.SqlCommand.Parameters.AddRange(paramters);
                    this.SqlCommand.ExecuteNonQuery();
                }
                if (this.IsDebug) this.Debug = $"|Query:[{query}]\n\n";
            }
            catch (Exception err)
            {
                if (this.IsDebug) this.Debug = $"|Query:[{query}]\n\n";
                this.Error.Append(ErrorCode.SQLQuery, $"SQL ExecuteQuery Error: {err.Message}\nMessage: {err.StackTrace}");
            }
        }
        public void ExecuteQuery(string query, IDictionary<string, object> parameters)
        {
            try
            {
                this.Open();
                if (this.Error.HasError == false)
                {
                    this.SqlCommand.CommandText = query;
                    this.SqlCommand.CommandType = CommandType.Text;
                    this.SqlCommand.Parameters.Clear();
                    foreach (string key in parameters.Keys)
                    {
                        if (parameters[key] == null)
                            this.SqlCommand.Parameters.Add(new SqlParameter($"@{key}", DBNull.Value));
                        else
                            this.SqlCommand.Parameters.Add(new SqlParameter($"@{key}", parameters[key]));
                    }
                    this.SqlCommand.ExecuteNonQuery();
                }
                if (this.IsDebug) this.Debug = $@"|Query:[{query}]\n\n";
            }
            catch (Exception err)
            {
                if (this.IsDebug) this.Debug = $@"|Query:[{query}]\n\n";
                this.Error.Append(ErrorCode.SQLQuery, $@"SQL ExecuteQuery Error: {err.Message}\n\nMessage: {err.StackTrace}");
            }
        }
        public int ExecuteScalar(string query, params SqlParameter[] paramters)
        {
            int result = -1;
            try
            {
                this.Open();
                if (this.Error.HasError == false)
                {
                    this.SqlCommand.CommandText = query;
                    this.SqlCommand.CommandType = CommandType.Text;
                    this.SqlCommand.Parameters.Clear();
                    this.SqlCommand.Parameters.AddRange(paramters);
                    object res = this.SqlCommand.ExecuteScalar();
                    bool ff = false;
                    if (res.GetType() == typeof(System.Boolean))
                    {
                        bool.TryParse(res.ToString(), out ff);
                        if (ff) result = 1;
                    }
                    else
                        int.TryParse(res.ToString(), out result);
                }
                if (this.IsDebug) this.Debug = $@"|Query:[{query}]\n\n";
            }
            catch (Exception err)
            {
                if (this.IsDebug) this.Debug = $@"|Query:[{query}]\n\n";
                this.Error.Append(ErrorCode.SQLQuery, $@"SQL ExecuteScalar Error: {err.Message}\n\nMessage: {err.StackTrace}");
            }
            return result;
        }
        public int ExecuteScalar(string query, IDictionary<string, object> parameters)
        {
            List<SqlParameter> ps = new List<SqlParameter>();
            foreach (string key in parameters.Keys)
            {
                if (parameters[key] == null)
                    ps.Add(new SqlParameter($"@{key}", DBNull.Value));
                else
                    ps.Add(new SqlParameter($"@{key}", parameters[key]));
            }
            return this.ExecuteScalar(query, ps.ToArray());
        }
        public bool IsExisted(string query, IDictionary<string, object> parameters)
        {
            int cnt = this.ExecuteScalar(query, parameters);
            return cnt > 0 ? true : false;
        }
        public bool IsExisted(string query, params SqlParameter[] parameters)
        {
            int cnt = this.ExecuteScalar(query, parameters);
            return cnt > 0 ? true : false;
        }
        public DataTable ExecuteDataTable(string query, params SqlParameter[] parameters)
        {
            DataTable dt = new DataTable();
            try
            {
                this.Open();
                if (this.Error.HasError == false)
                {

                    this.SqlCommand.CommandText = query;
                    this.SqlCommand.Parameters.Clear();
                    this.SqlCommand.Parameters.AddRange(parameters);
                    this.SqlCommand.CommandType = CommandType.Text;
                    this.SqlAdpt.Fill(dt);
                }
                if (this.IsDebug) this.Debug = $@"|Query:[{query}]\n\n";
            }
            catch (Exception err)
            {
                if (this.IsDebug) this.Debug = $@"|Query:[{query}]\n\n";
                this.Error.Append(ErrorCode.SQLQuery, $@"SQL ExecuteTable Error: {err.Message}\n\nMessage: {err.StackTrace}");
            }
            return dt;
        }
        public DataTable ExecuteDataTable(string query, IDictionary<string, object> parameters)
        {
            DataTable dt = new DataTable();
            try
            {
                this.Open();
                if (this.Error.HasError == false)
                {

                    this.SqlCommand.CommandText = query;
                    this.SqlCommand.Parameters.Clear();
                    List<SqlParameter> ps = new List<SqlParameter>();
                    foreach (string key in parameters.Keys)
                    {
                        if (parameters[key] == null)
                            ps.Add(new SqlParameter($"@{key}", DBNull.Value));
                        else
                            ps.Add(new SqlParameter($"@{key}", parameters[key]));
                    }
                    this.SqlCommand.Parameters.AddRange(ps.ToArray());
                    this.SqlCommand.CommandType = CommandType.Text;
                    this.SqlAdpt.Fill(dt);
                }
                if (this.IsDebug) this.Debug = $@"|Query:[{query}]\n\n";
            }
            catch (Exception err)
            {
                if (this.IsDebug) this.Debug = $@"|Query:[{query}]\n\n";
                this.Error.Append(ErrorCode.SQLQuery, $@"SQL ExecuteTable Error: {err.Message}\n\nMessage: {err.StackTrace}");
            }
            return dt;
        }

        public GTable ExecuteTable(string query, params SqlParameter[] parameters)
        {
            DataTable dt = new DataTable();
            GTable stable = new GTable();
            try
            {
                this.Open();
                if (this.Error.HasError == false)
                {
                    this.SqlCommand.CommandText = query;
                    this.SqlCommand.Parameters.Clear();
                    this.SqlCommand.Parameters.AddRange(parameters);
                    this.SqlCommand.CommandType = CommandType.Text;
                    this.SqlAdpt.Fill(dt);
                    foreach (DataRow dr in dt.Rows)
                    {
                        GRow srow = new GRow();
                        foreach (DataColumn dc in dt.Columns)
                        {
                            srow.AddCol(dc.ColumnName, dr[dc].ToString());
                        }
                        stable.AddRow(srow);
                    }

                }
                if (this.IsDebug) this.Debug = $@"|Query:[{query}]\n\n";
            }
            catch (Exception err)
            {
                if (this.IsDebug) this.Debug = $@"|Query:[{query}]\n\n";
                this.Error.Append(ErrorCode.SQLQuery, $@"SQL ExecuteTable Error: {err.Message}\n\nMessage: {err.StackTrace}");
            }
            return stable;
        }
        public GTable ExecuteTable(string query, IDictionary<string, object> parameters)
        {
            DataTable dt = new DataTable();
            GTable stable = new GTable();
            try
            {
                this.Open();
                if (this.Error.HasError == false)
                {
                    this.SqlCommand.CommandText = query;
                    this.SqlCommand.Parameters.Clear();
                    foreach (string key in parameters.Keys)
                    {
                        if (parameters[key] == null)
                            this.SqlCommand.Parameters.Add(new SqlParameter($"@{key}", DBNull.Value));
                        else
                            this.SqlCommand.Parameters.Add(new SqlParameter($"@{key}", parameters[key]));
                    }
                    this.SqlCommand.CommandType = CommandType.Text;
                    this.SqlAdpt.Fill(dt);
                    foreach (DataRow dr in dt.Rows)
                    {
                        GRow srow = new GRow();
                        foreach (DataColumn dc in dt.Columns)
                        {
                            srow.AddCol(dc.ColumnName, dr[dc].ToString());
                        }
                        stable.AddRow(srow);
                    }

                }
                if (this.IsDebug) this.Debug = $@"|Query:[{query}]\n\n";
            }
            catch (Exception err)
            {
                if (this.IsDebug) this.Debug = $@"|Query:[{query}]\n\n";
                this.Error.Append(ErrorCode.SQLQuery, $@"SQL ExecuteTable Error: {err.Message}\n\nMessage: {err.StackTrace}");
            }
            return stable;
        }
        public List<Dictionary<string, string>> Query(string query, IDictionary<string, object> parameters)
        {
            List<Dictionary<string, string>> rows = new List<Dictionary<string, string>>();
            try
            {
                GTable stable = this.ExecuteTable(query, parameters);
                foreach (GRow srow in stable.Rows)
                {
                    Dictionary<string, string> nrow = new Dictionary<string, string>();
                    foreach (string colName in srow.Cols.Keys)
                    {
                        nrow.Add(colName, srow.Cols[colName].Value);
                    }
                    rows.Add(nrow);
                }
                if (this.IsDebug) this.Debug = $@"|Query:[{query}]\n\n";
            }
            catch (Exception err)
            {
                if (this.IsDebug) this.Debug = $@"|Query:[{query}]\n\n";
                this.Error.Append(ErrorCode.SQLQuery, $@"SQL ExecuteTable Error: {err.Message}\n\nMessage: {err.StackTrace}");
            }
            return rows;
        }
        public bool IsExist(string tableName, SQLWhere whereFilter)
        {
            int result = -1;
            string query = $"SELECT COUNT(1) AS CNT FROM {tableName} WHERE {whereFilter.WhereGetFilter}";
            try
            {
                result = this.ExecuteScalar(query, whereFilter.GetParameters());
            }
            catch (Exception err)
            {
                if (this.IsDebug) this.Debug = $@"|Query:[{query}]\n\n";
                this.Error.Append(ErrorCode.SQLQuery, $@"SQL UpdateTable Error: {err.Message}\n\nMessage: {err.StackTrace}");
            }
            return result>0?true:false;
        }

        public int GetRowCount(string tableName, SQLWhere whereFilter)
        {
            int result = -1;
            string query = $"SELECT COUNT(1) AS CNT FROM {tableName} WHERE {whereFilter.WhereGetFilter}";
            try
            {
                result = this.ExecuteScalar(query, whereFilter.GetParameters());
            }
            catch (Exception err)
            {
                if (this.IsDebug) this.Debug = $@"|Query:[{query}]\n\n";
                this.Error.Append(ErrorCode.SQLQuery, $@"SQL UpdateTable Error: {err.Message}\n\nMessage: {err.StackTrace}");
            }
            return result;
        }

        public GTable GetTable(string tableName, SQLRow row, SQLWhere whereFilter)
        {
            DataTable dt = new DataTable();
            GTable stable = new GTable();
            string query = $"SELECT {row.ColumnGet} FROM {tableName} WHERE {whereFilter.WhereGetFilter}";
            try
            {
                this.Open();
                if (this.Error.HasError == false)
                {
                    this.SqlCommand.CommandText = query;
                    this.SqlCommand.CommandType = CommandType.Text;
                    this.SqlCommand.Parameters.Clear();
                    this.SqlCommand.Parameters.AddRange(whereFilter.GetParameters());
                    this.SqlCommand.Parameters.AddRange(row.GetParameters());
                    this.SqlAdpt.Fill(dt);
                    foreach (DataRow dr in dt.Rows)
                    {
                        GRow srow = new GRow();
                        foreach (DataColumn dc in dt.Columns)
                        {
                            srow.AddCol(dc.ColumnName, dr[dc].ToString());
                        }
                        stable.AddRow(srow);
                    }
                }
                if (this.IsDebug) this.Debug = $@"|Query:[{query}]\n\n";
            }
            catch (Exception err)
            {
                if (this.IsDebug) this.Debug = $@"|Query:[{query}]\n\n";
                this.Error.Append(ErrorCode.SQLQuery, $@"SQL UpdateTable Error: {err.Message}\n\nMessage: {err.StackTrace}");
            }
            return stable;
        }
        public GTable GetTable(string tableName, SQLRow row, SQLWhere whereFilter, string orderBy="", string paging="", int n=0)
        {
            DataTable dt = new DataTable();
            GTable stable = new GTable();
            string query = $"SELECT {row.ColumnGet} FROM {tableName} WHERE {whereFilter.WhereGetFilter}";
            if (string.IsNullOrWhiteSpace(orderBy) == false && string.IsNullOrWhiteSpace(paging) == false)
                query = $"SELECT { (n>0?$"TOP {n}":"") } {row.ColumnGet} FROM {tableName} WHERE {whereFilter.WhereGetFilter} {orderBy} {paging}";
            else if (string.IsNullOrWhiteSpace(orderBy)==false)
                query = $"SELECT { (n>0?$"TOP {n}":"") } {row.ColumnGet} FROM {tableName} WHERE {whereFilter.WhereGetFilter} {orderBy}";

            try
            {
                this.Open();
                if (this.Error.HasError == false)
                {
                    this.SqlCommand.CommandText = query;
                    this.SqlCommand.CommandType = CommandType.Text;
                    this.SqlCommand.Parameters.Clear();
                    this.SqlCommand.Parameters.AddRange(whereFilter.GetParameters());
                    this.SqlCommand.Parameters.AddRange(row.GetParameters());
                    this.SqlAdpt.Fill(dt);
                    foreach (DataRow dr in dt.Rows)
                    {
                        GRow srow = new GRow();
                        foreach (DataColumn dc in dt.Columns)
                        {
                            srow.AddCol(dc.ColumnName, dr[dc].ToString());
                        }
                        stable.AddRow(srow);
                    }
                }
                if (this.IsDebug) this.Debug = $@"|Query:[{query}]\n\n";
            }
            catch (Exception err)
            {
                if (this.IsDebug) this.Debug = $@"|Query:[{query}]\n\n";
                this.Error.Append(ErrorCode.SQLQuery, $@"SQL UpdateTable Error: {err.Message}\n\nMessage: {err.StackTrace}");
            }
            return stable;
        }
        public int UpdateTable(string tableName, SQLRow row, SQLWhere whereFilter)
        {
            int result = -1;
            string query = $"UPDATE {tableName} SET {row.ColumnUpdate} WHERE {whereFilter.WhereSaveFilter}";
            try
            {
                this.Open();
                if (this.Error.HasError == false)
                {
                    this.SqlCommand.CommandText = query;
                    this.SqlCommand.CommandType = CommandType.Text;
                    this.SqlCommand.Parameters.Clear();
                    this.SqlCommand.Parameters.AddRange(whereFilter.GetParameters());
                    this.SqlCommand.Parameters.AddRange(row.GetParameters());
                    SqlParameter retval = new SqlParameter("@ReturnValue", SqlDbType.Int);
                    retval.Direction = ParameterDirection.ReturnValue;
                    this.SqlCommand.Parameters.Add(retval);
                    this.SqlCommand.ExecuteNonQuery();
                    result = retval.Value.GetInt() ?? 0;
                }
                if (this.IsDebug) this.Debug = $@"|Query:[{query}]\n\n";
            }
            catch (Exception err)
            {
                if (this.IsDebug) this.Debug = $@"|Query:[{query}]\n\n";
                this.Error.Append(ErrorCode.SQLQuery, $@"SQL UpdateTable Error: {err.Message}\n\nMessage: {err.StackTrace}");
            }
            return result;
        }
        public int UpdateTable(string tableName, IDictionary<string, object> colKVs, SQLWhere whereFilter)
        {
            int result = -1;
            try
            {
                SQLRow row = new SQLRow();
                row.Add(colKVs);
                result = this.UpdateTable(tableName, row, whereFilter);
            }
            catch (Exception err)
            {
                this.Error.Append(ErrorCode.SQLQuery, $@"SQL UpdateTable Error: {err.Message}\n\nMessage: {err.StackTrace}");
            }
            return result;
        }
        public int UpdateTable(string tableName, IDictionary<string, object> colKVs, IDictionary<string, object> whereKVs)
        {
            int result = -1;
            try
            {
                SQLWhere whereFilter = new SQLWhere();
                whereFilter.Add(whereKVs);
                result = this.UpdateTable(tableName, colKVs, whereFilter);
            }
            catch (Exception err)
            {
                this.Error.Append(ErrorCode.SQLQuery, $@"SQL UpdateTable Error: {err.Message}\n\nMessage: {err.StackTrace}");
            }
            return result;
        }
        public int InsertTable(string tableName, SQLRow row)
        {
            int result = -1;
            string query = $"INSERT {tableName} ({row.ColumnInsert}) VALUES({row.ColumnValues});SELECT SCOPE_IDENTITY();";
            try
            {
                this.Open();
                if (this.Error.HasError == false)
                {
                    this.SqlCommand.CommandText = query;
                    this.SqlCommand.CommandType = CommandType.Text;
                    this.SqlCommand.Parameters.Clear();
                    this.SqlCommand.Parameters.AddRange(row.GetParameters());
                    object res = this.SqlCommand.ExecuteScalar();
                    int.TryParse(res.ToString(), out result);
                }
                if (this.IsDebug) this.Debug = $@"|Query:[{query}]\n\n";
            }
            catch (Exception err)
            {
                if (this.IsDebug) this.Debug = $@"|Query:[{query}]\n\n";
                this.Error.Append(ErrorCode.SQLQuery, $@"SQL InsertTable Error: {err.Message}\n\nMessage: {err.StackTrace}");
            }
            return result;
        }
        public int InsertTable(string tableName, IDictionary<string, object> colValues)
        {
            int result = -1;
            try
            {
                SQLRow row = new SQLRow();
                row.Add(colValues);
                result = this.InsertTable(tableName, row);
            }
            catch (Exception err)
            {
                this.Error.Append(ErrorCode.SQLQuery, $@"SQL InsertTable Error: {err.Message}\n\nMessage: {err.StackTrace}");
            }
            return result;
        }
        public int DetachTable(string tableName, SQLWhere whereFilter)
        {
            int result = -1;
            string query = $"UPDATE {tableName} SET Deleted = 1 WHERE {whereFilter.WhereSaveFilter}";
            try
            {
                this.Open();
                if (this.Error.HasError == false)
                {
                    this.SqlCommand.CommandText = query;
                    this.SqlCommand.CommandType = CommandType.Text;
                    this.SqlCommand.Parameters.Clear();
                    this.SqlCommand.Parameters.AddRange(whereFilter.GetParameters());

                    SqlParameter retval = new SqlParameter("@ReturnValue", SqlDbType.Int);
                    retval.Direction = ParameterDirection.ReturnValue;
                    this.SqlCommand.Parameters.Add(retval);
                    this.SqlCommand.ExecuteNonQuery();
                    result = retval.Value.GetInt() ?? 0;
                }
                if (this.IsDebug) this.Debug = $@"|Query:[{query}]\n\n";
            }
            catch (Exception err)
            {
                if (this.IsDebug) this.Debug = $@"|Query:[{query}]\n\n";
                this.Error.Append(ErrorCode.SQLQuery, $@"SQL DetachTable Error: {err.Message}\n\nMessage: {err.StackTrace}");
            }
            return result;
        }
        public int DetachTable(string tableName, IDictionary<string, object> whereKVs)
        {
            int result = -1;
            try
            {
                SQLWhere whereFilter = new SQLWhere();
                whereFilter.Add(whereKVs);
                result = this.DetachTable(tableName, whereFilter);
            }
            catch (Exception err)
            {
                this.Error.Append(ErrorCode.SQLQuery, $@"SQL DetachTable Error: {err.Message}\n\nMessage: {err.StackTrace}");
            }
            return result;
        }
        public int DeleteTable(string tableName, SQLWhere whereFilter)
        {
            int result = -1;
            string query = $"DELETE FROM {tableName} WHERE {whereFilter.WhereSaveFilter}";
            try
            {
                this.Open();
                if (this.Error.HasError == false)
                {
                    this.SqlCommand.CommandText = query;
                    this.SqlCommand.CommandType = CommandType.Text;
                    this.SqlCommand.Parameters.Clear();
                    this.SqlCommand.Parameters.AddRange(whereFilter.GetParameters());

                    SqlParameter retval = new SqlParameter("@ReturnValue", SqlDbType.Int);
                    retval.Direction = ParameterDirection.ReturnValue;
                    this.SqlCommand.Parameters.Add(retval);
                    this.SqlCommand.ExecuteNonQuery();
                    result = retval.Value.GetInt() ?? 0;
                }
                if (this.IsDebug) this.Debug = $@"|Query:[{query}]\n\n";
            }
            catch (Exception err)
            {
                if (this.IsDebug) this.Debug = $@"|Query:[{query}]\n\n";
                this.Error.Append(ErrorCode.SQLQuery, $@"SQL DetachTable Error: {err.Message}\n\nMessage: {err.StackTrace}");
            }
            return result;
        }
        public int DeleteTable(string tableName, IDictionary<string, object> whereKVs)
        {
            int result = 0;
            try
            {
                SQLWhere whereFilter = new SQLWhere();
                whereFilter.Add(whereKVs);
                result = this.DeleteTable(tableName, whereFilter);
            }
            catch (Exception err)
            {
                this.Error.Append(ErrorCode.SQLQuery, $@"SQL DetachTable Error: {err.Message}\n\nMessage: {err.StackTrace}");
            }
            return result;
        }
        #endregion
    }

    // For example :  "FirstName" Like "%@search_firstname%",  "Date" Between @search_date_start and @search_date_end
    public class SQLWhereColumn
    {
        public SQLWhereColumn() {
            this.WhereCol = string.Empty;
            this.Parameters = new List<SqlParameter>();
        }
        public SQLWhereColumn(string colExpr) : this()
        {
            this.Add(colExpr);
        }
        public SQLWhereColumn(string dbName, object value) : this()
        {
            this.Add(dbName, value);
        }
        public SQLWhereColumn(string colExpr, params SqlParameter[] sqlParams) : this()
        {
            this.Add(colExpr, sqlParams);
        }

        public string WhereCol { get; set; }
        private IList<SqlParameter> Parameters { get; set; }

        public IList<SqlParameter> GetParameters()
        {
            return this.Parameters;
        }
        // 1 = 1

        public SQLWhereColumn Add(string colExpr)
        {
            this.WhereCol = colExpr;
            return this;
        }
        public SQLWhereColumn Add(string dbName, object value)
        {
            this.WhereCol = $"{dbName}=@{dbName}_where_value";
            if (value == null)
                this.Parameters.Add(new SqlParameter($"@{dbName}_where_value", DBNull.Value));
            else
                this.Parameters.Add(new SqlParameter($"@{dbName}_where_value", value));
            return this;
        }
        // FirstName Like '%@firstname%'
        // BDate BETWEEN @start_date AND @end_date
        public SQLWhereColumn Add(string colExpr, params SqlParameter[] sqlParams)
        {
            this.WhereCol = colExpr;
            foreach(SqlParameter sqlParam in sqlParams) this.Parameters.Add(sqlParam);
            return this;
        }

    }
    // For example :  dbname="FirstName,LastName",  dbname="bdate,cdate"
    //"FirstName" Like "%@search_firstname%"  or "LastName" Like "%@search_firstname%", 
    //"BDate" Between @search_bdate_start and @search_bdate_end or "CDate" Between @search_cdate_start and @search_cdate_end
    public class SQLWhereSearch
    {
        public SQLWhereSearch() {
            this.WhereColumn = new List<SQLWhereColumn>();
        }
        public SQLWhereSearch(string colExpr) : this()
        {
            this.Add(colExpr);
        }
        public SQLWhereSearch(string dbName, object value):this()
        {
            this.Add(dbName, value);
        }
        public SQLWhereSearch(string colExpr, params SqlParameter[] sqlParams):this()
        {
            this.Add(colExpr, sqlParams);
        }

        private IList<SQLWhereColumn> WhereColumn { get; set; }
        public string WhereSearch {
            get
            {
                string searchStr = string.Empty;
                foreach (SQLWhereColumn sc in this.WhereColumn)
                {
                  if(string.IsNullOrWhiteSpace(sc.WhereCol)==false) searchStr = searchStr.Concat(sc.WhereCol, " OR ");
                }
                if(this.WhereColumn.Count > 1 && string.IsNullOrWhiteSpace(searchStr) == false) searchStr = $"({searchStr})";
                return searchStr;
            }
        }
        private IList<SqlParameter> Parameters {
            get
            {
                IList<SqlParameter> ps = new List<SqlParameter>();
                foreach (SQLWhereColumn sc in this.WhereColumn)
                    foreach (SqlParameter e in sc.GetParameters()) ps.Add(e);
                return ps;
            }
        }
        public IList<SqlParameter> GetParameters()
        {
            return this.Parameters;
        }

        public SQLWhereSearch Add(string colExpr)
        {
            this.WhereColumn.Add(new SQLWhereColumn(colExpr));
            return this;
        }
        public SQLWhereSearch Add(string dbName, object value)
        {
            this.WhereColumn.Add(new SQLWhereColumn(dbName, value));
            return this;
        }
        public SQLWhereSearch Add(string colExpr, params SqlParameter[] sqlParams)
        {
            this.WhereColumn.Add(new SQLWhereColumn(colExpr, sqlParams));
            return this;
        }

        public SQLWhereSearch Add(params SQLWhereColumn[] sqlWhereCols)
        {
            foreach (SQLWhereColumn item in sqlWhereCols) this.WhereColumn.Add(item);
            return this;
        }
    }
    public class SQLWhere
    {
        public SQLWhere() {
            this.WhereSearch = new List<SQLWhereSearch>();
        }
        public SQLWhere(string colExpr):this()
        {
            this.Add(colExpr);
        }
        public string WhereGetFilter {
            get
            {
                string clauseStr = string.Empty;
                foreach (SQLWhereSearch sc in this.WhereSearch) clauseStr = clauseStr.Concat(sc.WhereSearch, " AND ");
                if (string.IsNullOrWhiteSpace(clauseStr)) clauseStr = "1=1";
                return clauseStr;
            }
        }
        public string WhereSaveFilter
        {
            get
            {
                string clauseStr = string.Empty;
                foreach (SQLWhereSearch sc in this.WhereSearch) clauseStr = clauseStr.Concat(sc.WhereSearch, " AND ");
                if (string.IsNullOrWhiteSpace(clauseStr)) clauseStr = "1=0";
                return clauseStr;
            }
        }
        private List<SqlParameter> Parameters {
            get
            {
                List<SqlParameter> ps = new List<SqlParameter>();
                foreach (SQLWhereSearch sc in this.WhereSearch)
                    foreach (SqlParameter e in sc.GetParameters()) ps.Add(e);
                return ps;
            }
        }
        public SqlParameter[] GetParameters()
        {
            return this.Parameters.ToArray();
        }
        private IList<SQLWhereSearch> WhereSearch { get; set; }

        public SQLWhere Add(string colExpr)
        {
            this.WhereSearch.Add(new SQLWhereSearch(colExpr));
            return this;
        }
        public SQLWhere Add(string dbName, object value)
        {
            this.WhereSearch.Add(new SQLWhereSearch(dbName, value));
            return this;
        }
        public SQLWhere Add(string colExpr, params SqlParameter[] sqlParams)
        {
            this.WhereSearch.Add(new SQLWhereSearch(colExpr, sqlParams));
            return this;
        }

        public SQLWhere Add(params SQLWhereSearch[] sqlWhereSearchs)
        {
            foreach (SQLWhereSearch item in sqlWhereSearchs) this.WhereSearch.Add(item);
            return this;
        }
        public SQLWhere Add(IDictionary<string, object> whereKVs)
        {
            foreach (string dbName in whereKVs.Keys) this.Add(dbName, whereKVs[dbName]);
            return this;
        }

    }
    public class SQLRow
    {
        public SQLRow()
        {
            this.Cols = new List<SQLCol>();
            this.Parameters = new List<SqlParameter>();
        }

        public int ColumnCount
        {
            get
            {
                return this.Cols.Count;
            }
        }

        public string ColumnGet
        {
            get
            {
                string getStr = string.Empty;
                foreach(SQLCol col in this.Cols)
                {
                    getStr = getStr.Concat($"{col.DBName} AS {col.ColName}", ", ");
                }
                return getStr;
            }
        }
        public string ColumnUpdate
        {
            get
            {
                string getStr = string.Empty;
                foreach (SQLCol col in this.Cols)
                {
                    getStr = getStr.Concat($"{col.DBName} = @{col.DBName}_update_value", ", ");
                    this.Parameters.Add(new SqlParameter($"@{col.DBName}_update_value", col.Value));
                }
                return getStr;
            }
        }
        public string ColumnInsert
        {
            get
            {
                string getStr = string.Empty;
                foreach (SQLCol col in this.Cols)
                {
                    getStr = getStr.Concat($"{col.DBName}", ", ");
                }
                return getStr;
            }
        }
        public string ColumnValues
        {
            get
            {
                string getStr = string.Empty;
                foreach (SQLCol col in this.Cols)
                {
                    getStr = getStr.Concat($"@{col.DBName}_insert_value", ", ");
                    this.Parameters.Add(new SqlParameter($"@{col.DBName}_insert_value", col.Value));
                }
                return getStr;
            }
        }

        private List<SQLCol> Cols { get; set; }
        private List<SqlParameter> Parameters { get; set; }
        public SqlParameter[] GetParameters()
        {
            return this.Parameters.ToArray();
        }
        public SQLRow Add(SQLCol col)
        {
            this.Cols.Add(col);
            return this;
        }
        public SQLRow Add(string dbName, string colName)
        {
            this.Add(new SQLCol(dbName, colName));
            return this;
        }
        public SQLRow Add(string dbName, object value)
        {
            this.Add(new SQLCol(dbName, value));
            return this;
        }
        public SQLRow Add(string dbName, string colName, object value)
        {
            this.Add(new SQLCol(dbName, colName, value));
            return this;
        }
        public SQLRow Add(IDictionary<string, object> colKVs)
        {
            foreach(string dbName in colKVs.Keys)
            {
                this.Add(dbName, colKVs[dbName]);
            }
            return this;
        }
        public bool ContainCol(string dbName)
        {
            return this.Cols.Exists(p => p.DBName == dbName);
        }
    }
    public class SQLCol {
        public SQLCol()
        {
            this.DBName = string.Empty;
            this.ColName = this.DBName;
            this.Value = DBNull.Value;
        }
        public SQLCol(string dbName, string colName) : this()
        {
            this.DBName = dbName;
            this.ColName = colName;
        }
        public SQLCol(string dbName, object value):this()
        {
            this.DBName = dbName;
            this.ColName = this.DBName;
            this.Value = value ?? DBNull.Value;
        }
        public SQLCol(string dbName, string colName, object value) : this()
        {
            this.DBName = dbName;
            this.ColName = colName;
            this.Value = value ?? DBNull.Value;
        }
        public string DBName { get; set; }
        public string ColName { get; set; }
        public object Value { get; set; }
    }

    public class GTable
    {
        public GTable()
        {
            this.Rows = new List<GRow>();
        }
        public IList<GRow> Rows { get; set; }
        public void AddRow(GRow row)
        {
            this.Rows.Add(row);
        }
        public int RowCount
        {
            get
            {
                return this.Rows.Count;
            }
        }
    }
    public class GRow
    {
        public GRow()
        {
            this.Cols = new Dictionary<string, GCol>();
        }
        public IDictionary<string, GCol> Cols { get; set; }
        public void AddCol(string name, string value)
        {
            this.Cols.Add(name, new GCol(name, value));
        }
        public void AddCol(GCol col)
        {
            this.Cols.Add(col.Name, col);
        }
        public string GetValue(string colName)
        {
            if (this.Cols.ContainsKey(colName))
                return this.Cols[colName].Value;
            else
                return ""; 
        }
    }
    public class GCol
    {
        public GCol(string name, string value)
        {
            this.Name = name;
            this.Value = value;
        }
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
