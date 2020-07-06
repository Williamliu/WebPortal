using System;
using System.Collections.Generic;
using System.Text;
using Library.V1.Common;
using Newtonsoft.Json;

namespace Library.V1.Entity
{
    public class Row
    {
        public Row()
        {
            this.Key = 0;
            this.Guid = Guid.NewGuid();
            this.State = EState.Normal;
            this.Columns = new Dictionary<string, Column>();
            this.Error = new Error();
            this.Errors = new Dictionary<string, Error>();
        }
        public Row(Row row)
        {
            this.Key = row.Key;
            this.Guid = row.Guid;
            this.State = row.State;
            this.Columns = row.Columns;
            this.Error = row.Error;
            this.Errors = row.Errors;
        }

        #region fields
        public Guid Guid { get; set; }
        public int Key { get; set; }
        public EState State { get; set; }
        public Dictionary<string, Column> Columns { get; set; }
        public Error Error { get; set; }
        public Dictionary<string, Error> Errors { get; set; }
        [JsonIgnore]
        public bool HasError
        {
            get
            {
                bool hasErr = this.Error.HasError;
                foreach (string colName in this.Errors.Keys)
                {
                    hasErr = hasErr || this.Errors[colName].HasError;
                }
                return hasErr;
            }
        }

        #endregion

        #region Methods
        public void AddColumn(Column column)
        {
            this.Columns.Add(column.Name, column);
        }
        public void AddColumns(params Column[] columns)
        {
            foreach (Column column in columns) this.AddColumn(column);
        }
        public void ResetError()
        {
            this.Errors.Clear();
        }
        public void ResetError(Column col)
        {
            if (this.Errors.ContainsKey(col.Name)) this.Errors.Remove(col.Name);
        }
        public void AppendError(Column col, ErrorCode code, string message, string memo = "")
        {
            if (this.Errors.ContainsKey(col.Name))
                this.Errors[col.Name].Append(code, message, string.IsNullOrWhiteSpace(memo) ? col.Name : memo);
            else
                this.Errors.Add(col.Name, new Error(code, message, string.IsNullOrWhiteSpace(memo) ? col.Name : memo));
        }
        public void AppendError(Column col, Error error)
        {
            if (this.Errors.ContainsKey(col.Name))
                this.Errors[col.Name].Append(error);
            else
                this.Errors.Add(col.Name, new Error(error));
        }
        #endregion

        #region Get Methods
        public object GetValue(string colName)
        {
            return this.Columns.ContainsKey(colName) ? this.Columns[colName].Value : null;
        }
        public object GetValue1(string colName)
        {
            return this.Columns.ContainsKey(colName) ? this.Columns[colName].Value1 : null;
        }
        public void SetValue(string colName, object value)
        {
            if(this.Columns.ContainsKey(colName)) this.Columns[colName].Value = value;
        }
        public void SetValue1(string colName, object value)
        {
            if (this.Columns.ContainsKey(colName)) this.Columns[colName].Value1 = value;
        }
        public bool IsError(string colName)
        {
            return this.Errors.ContainsKey(colName) ? this.Errors[colName].HasError : false;
        }
        #endregion
    }
}
