using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using Library.V1.Common;
using Library.V1.SQL;
using System.Data.SqlClient;
using System.Text.Json;

namespace Library.V1.Entity
{
    public enum EInput
    {
        Hidden,
        Object,
        String,
        Email,
        Int,
        Long,
        Float,
        Bool,
        Date,
        DateTime,
        Time,
        Password,
        Passpair,

        ImageUrl,
        ImageContent,
        FileUrl,
        FileContent,

        Read,           // only for meta get data but save data
        Custom,         // only for meta custom  get and save code logic

        Checkbox        // for both

    }
    public class Meta
    {
        public Meta()
        {
            this.Name = string.Empty;
            this.DbName = string.Empty;
            this.Type = EInput.String;
            this.Title = string.Empty;
            this.Description = string.Empty;
            this.Order = string.Empty;
            this.Sync = false;
            this.Required = false;
            this.Unique = false;
            this.Value = null;
        }

        #region Public Fields
        public string Name { get; set; }
        public string Title { get; set; }
        // EInput.ImageContent: use for size "small", "medium" , "tiny", or "large"
        // EInput.ImageContent: small|medium,  if mutiple rows using small.  if single row, using medium
        // EInput.Bool:  use for true|false description  like  "status.active.inactive"  
        public string Description { get; set; }
        public bool IsKey { get; set; }
        public string Order { get; set; }
        public bool Sync { get; set; }
        public bool Required { get; set; }
        // default value for Insert 
        public object Value { get; set; }

        public ListRef ListRef { get; set; }
        #endregion

        #region Internal Use, not for client side json
        [JsonIgnore]
        public EInput Type { get; set; }
        [JsonIgnore]
        public string DbName { get; set; }
        [JsonIgnore]
        public bool IsLang { get; set; }
        [JsonIgnore]
        public int MinLength { get; set; }
        [JsonIgnore]
        public int MaxLength { get; set; }
        [JsonIgnore]
        public int Min { get; set; }
        [JsonIgnore]
        public int Max { get; set; }
        [JsonIgnore]
        public bool Unique { get; set; }
        [JsonIgnore]
        public bool AllowGet
        {
            get
            {
                bool IsAllow = true;
                if (this.Type == EInput.Hidden) IsAllow = false;
                if (this.Type == EInput.ImageUrl) IsAllow = false;
                if (this.Type == EInput.ImageContent) IsAllow = false;
                if (this.Type == EInput.FileUrl) IsAllow = false;
                if (this.Type == EInput.FileContent) IsAllow = false;
                if (this.Type == EInput.Custom) IsAllow = false;
                if (this.Type == EInput.Checkbox) IsAllow = false;
                return IsAllow;
            }
        }
        [JsonIgnore]
        public bool AllowSave
        {
            get
            {
                bool IsAllow = true;
                if (this.Type == EInput.Hidden) IsAllow = false;
                if (this.Type == EInput.Read) IsAllow = false;
                if (this.Type == EInput.ImageUrl) IsAllow = false;
                if (this.Type == EInput.ImageContent) IsAllow = false;
                if (this.Type == EInput.FileUrl) IsAllow = false;
                if (this.Type == EInput.FileContent) IsAllow = false;
                if (this.Type == EInput.Custom) IsAllow = false;
                if (this.Type == EInput.Checkbox) IsAllow = false;
                return IsAllow;
            }
        }
        #endregion

        #region Methods
        public void AddListRef(string collection, string valueTable = "", string valueKey = "")
        {
            this.ListRef = new ListRef(collection, valueTable, valueKey);
        }
        public Column QueryCK(SqlHelper SQL, int key)
        {
            string queryCK = string.Format(this.ListRef.QueryString, this.DbName);
            GTable ckTable = SQL.ExecuteTable(queryCK, new SqlParameter("@checkbox_refkey", key));
            Dictionary<string, bool> ckValues = new Dictionary<string, bool>();
            foreach (var srow in ckTable.Rows) ckValues.Add(srow.GetValue("CKValue"), true);
            return new Column(this.Name, ckValues);
        }
        public void DeleteCK(SqlHelper SQL, int key)
        {
            string deleteCK = string.Format(this.ListRef.DeleteString, this.DbName);
            SQL.ExecuteQuery(deleteCK, new SqlParameter("@checkbox_refkey", key));
        }
        public void InsertCK(SqlHelper SQL, int key, object kvs)
        {
            string insertCK = string.Format(this.ListRef.InsertString, this.DbName);
            Dictionary<string, bool> ckValues = kvs as Dictionary<string, bool>;
            if (ckValues != null)
            {
                foreach (string ckKey in ckValues.Keys)
                {
                    if (ckValues[ckKey])
                    {
                        int ckValue = ckKey.GetInt() ?? 0;
                        if (ckValue > 0) SQL.ExecuteQuery(insertCK, new SqlParameter("@checkbox_refkey", key), new SqlParameter("@check_valuekey", ckValue));
                    }

                }
            }

        }
        #endregion

        #region data operation methods
        public void Validate(Row rowObj)  // Meta: Validate 
        {
            if (rowObj.Columns.ContainsKey(this.Name) == false) return;
            if ( ValidateHelper.IsMatch(this.Type.ToString(), rowObj.GetValue(this.Name), rowObj.GetValue1(this.Name)) == false)
            {
                rowObj.AppendError(
                                    rowObj.Columns[this.Name], 
                                    ErrorCode.ValueValidate, 
                                    string.Format(LanguageHelper.Words("validate.type"), this.Title, LanguageHelper.Words(this.Type.ToString().ToLower()))
                                    );
            }

            switch (this.Type)
            {
                case EInput.Hidden:
                case EInput.Object:
                    if (this.Required && ObjectHelper.IsEmpty(rowObj.GetValue(this.Name)))
                        rowObj.AppendError(
                                            rowObj.Columns[this.Name],
                                            ErrorCode.ValueValidate, 
                                            string.Format(LanguageHelper.Words("validate.required"), this.Title)
                                            );
                    break;
                case EInput.String:
                case EInput.Email:
                    if (this.Required && ObjectHelper.IsEmpty(rowObj.GetValue(this.Name)))
                        rowObj.AppendError(
                                            rowObj.Columns[this.Name],
                                            ErrorCode.ValueValidate, 
                                            string.Format(LanguageHelper.Words("validate.required"), this.Title)
                                            );

                    if (this.MinLength > 0 && rowObj.GetValue(this.Name).GetString().Length > 0 && rowObj.GetValue(this.Name).GetString().Length < this.MinLength)
                        rowObj.AppendError(
                                            rowObj.Columns[this.Name],
                                            ErrorCode.ValueValidate, 
                                            string.Format(LanguageHelper.Words("validate.minlength"), this.Title, rowObj.GetValue(this.Name).GetString().Length, this.MinLength)
                                            );

                    if (this.MaxLength > 0 && rowObj.GetValue(this.Name).GetString().Length > 0 && rowObj.GetValue(this.Name).GetString().Length > this.MaxLength)
                        rowObj.AppendError(
                                            rowObj.Columns[this.Name],
                                            ErrorCode.ValueValidate, 
                                            string.Format(LanguageHelper.Words("validate.maxlength"), this.Title, rowObj.GetValue(this.Name).GetString().Length, this.MaxLength)
                                            );
                    break;
                case EInput.Int:
                    if (this.Required && ObjectHelper.IsEmpty(rowObj.GetValue(this.Name)))
                        rowObj.AppendError(
                                            rowObj.Columns[this.Name], 
                                            ErrorCode.ValueValidate, 
                                            string.Format(LanguageHelper.Words("validate.required"), this.Title)
                                            );

                    if (this.Min > 0 && (rowObj.GetValue(this.Name).GetInt()??0) < this.Min)
                        rowObj.AppendError(
                                            rowObj.Columns[this.Name], 
                                            ErrorCode.ValueValidate, 
                                            string.Format(LanguageHelper.Words("validate.min"), this.Title, rowObj.GetValue(this.Name).GetInt() ?? 0, this.Min)
                                            );
                    if (this.Max > 0 && (rowObj.GetValue(this.Name).GetInt() ?? 0) > this.Max)
                        rowObj.AppendError(
                                            rowObj.Columns[this.Name], 
                                            ErrorCode.ValueValidate, 
                                            string.Format(LanguageHelper.Words("validate.max"), this.Title, rowObj.GetValue(this.Name).GetInt() ?? 0, this.Max)
                                            );

                    if (rowObj.IsError(this.Name) == false)
                    {
                        rowObj.SetValue(this.Name, rowObj.GetValue(this.Name).GetInt());
                    }
                    break;
                case EInput.Long:
                    if (this.Required && ObjectHelper.IsEmpty(rowObj.GetValue(this.Name)))
                        rowObj.AppendError(
                                            rowObj.Columns[this.Name],
                                            ErrorCode.ValueValidate,
                                            string.Format(LanguageHelper.Words("validate.required"), this.Title)
                                            );

                    if (this.Min > 0 && (rowObj.GetValue(this.Name).GetLong() ?? 0) < this.Min)
                        rowObj.AppendError(
                                            rowObj.Columns[this.Name],
                                            ErrorCode.ValueValidate,
                                            string.Format(LanguageHelper.Words("validate.min"), this.Title, rowObj.GetValue(this.Name).GetLong() ?? 0, this.Min)
                                            );
                    if (this.Max > 0 && (rowObj.GetValue(this.Name).GetLong() ?? 0) > this.Max)
                        rowObj.AppendError(
                                            rowObj.Columns[this.Name],
                                            ErrorCode.ValueValidate,
                                            string.Format(LanguageHelper.Words("validate.max"), this.Title, rowObj.GetValue(this.Name).GetLong() ?? 0, this.Max)
                                            );

                    if (rowObj.IsError(this.Name) == false)
                    {
                        rowObj.SetValue(this.Name, rowObj.GetValue(this.Name).GetLong());
                    }
                    break;
                case EInput.Float:
                    if (this.Required && ObjectHelper.IsEmpty(rowObj.GetValue(this.Name)))
                        rowObj.AppendError(
                                            rowObj.Columns[this.Name],
                                            ErrorCode.ValueValidate,
                                            string.Format(LanguageHelper.Words("validate.required"), this.Title)
                                            );

                    if (this.Min > 0 && (rowObj.GetValue(this.Name).GetFloat() ?? 0) < this.Min)
                        rowObj.AppendError(
                                            rowObj.Columns[this.Name],
                                            ErrorCode.ValueValidate,
                                            string.Format(LanguageHelper.Words("validate.min"), this.Title, rowObj.GetValue(this.Name).GetFloat() ?? 0, this.Min)
                                            );
                    if (this.Max > 0 && (rowObj.GetValue(this.Name).GetFloat() ?? 0) > this.Max)
                        rowObj.AppendError(
                                            rowObj.Columns[this.Name],
                                            ErrorCode.ValueValidate,
                                            string.Format(LanguageHelper.Words("validate.max"), this.Title, rowObj.GetValue(this.Name).GetFloat() ?? 0, this.Max)
                                            );

                    if (rowObj.IsError(this.Name) == false)
                    {
                        rowObj.SetValue(this.Name, rowObj.GetValue(this.Name).GetFloat());
                    }
                    break;

                case EInput.Bool:
                    if (rowObj.IsError(this.Name) == false)
                    {
                        rowObj.SetValue(this.Name, rowObj.GetValue(this.Name).GetBool() ?? false);
                    }
                    break;

                case EInput.Date:
                    if (this.Required && ObjectHelper.IsEmpty(rowObj.GetValue(this.Name)))
                        rowObj.AppendError(
                                            rowObj.Columns[this.Name], 
                                            ErrorCode.ValueValidate, 
                                            string.Format(LanguageHelper.Words("validate.required"), this.Title)
                                            );

                    if (rowObj.IsError(this.Name) == false)
                    {
                        rowObj.SetValue(this.Name, rowObj.GetValue(this.Name).GetDate());
                    }
                    break;

                case EInput.DateTime:
                    if (this.Required && ObjectHelper.IsEmpty(rowObj.GetValue(this.Name)) && ObjectHelper.IsEmpty(rowObj.GetValue1(this.Name)))
                        rowObj.AppendError(
                                            rowObj.Columns[this.Name], 
                                            ErrorCode.ValueValidate, 
                                            string.Format(LanguageHelper.Words("validate.required"), this.Title)
                                            );

                    if (rowObj.IsError(this.Name) == false)
                    {
                        if (ObjectHelper.IsEmpty(rowObj.GetValue(this.Name)) == false)
                            rowObj.SetValue(this.Name,
                                rowObj.GetValue(this.Name).GetDateTime().Value.AddTicks(rowObj.GetValue1(this.Name).GetTimeSpan()?.Ticks ?? 0 )
                            );
                    }
                    break;
                case EInput.Time:
                    if (this.Required && ObjectHelper.IsEmpty(rowObj.GetValue(this.Name)))
                            rowObj.AppendError(
                                             rowObj.Columns[this.Name], 
                                             ErrorCode.ValueValidate, 
                                             string.Format(LanguageHelper.Words("validate.required"), this.Title)
                                             );

                    if (rowObj.IsError(this.Name) == false)
                    {
                        rowObj.SetValue(this.Name, rowObj.GetValue(this.Name).GetTime());
                    }
                    break;

                case EInput.Password:
                    if (this.Required && ObjectHelper.IsEmpty(rowObj.GetValue(this.Name)))
                        rowObj.AppendError(
                                            rowObj.Columns[this.Name],
                                            ErrorCode.ValueValidate,
                                            string.Format(LanguageHelper.Words("validate.required"), this.Title)
                                            );

                    if (this.MinLength > 0 && rowObj.GetValue(this.Name).GetString().Length > 0 && rowObj.GetValue(this.Name).GetString().Length < this.MinLength)
                        rowObj.AppendError(
                                            rowObj.Columns[this.Name],
                                            ErrorCode.ValueValidate,
                                            string.Format(LanguageHelper.Words("validate.minlength"), this.Title, rowObj.GetValue(this.Name).GetString().Length, this.MinLength)
                                            );

                    if (this.MaxLength > 0 && rowObj.GetValue(this.Name).GetString().Length > 0 && rowObj.GetValue(this.Name).GetString().Length > this.MaxLength)
                        rowObj.AppendError(
                                            rowObj.Columns[this.Name],
                                            ErrorCode.ValueValidate,
                                            string.Format(LanguageHelper.Words("validate.maxlength"), this.Title, rowObj.GetValue(this.Name).GetString().Length, this.MaxLength)
                                            );

                    if (rowObj.IsError(this.Name) == false)
                    {
                        rowObj.SetValue(this.Name, rowObj.GetValue(this.Name).GetString().MD5Hash());
                    }
                    break;
                case EInput.Passpair:
                    if (this.Required && ObjectHelper.IsEmpty(rowObj.GetValue(this.Name)) && ObjectHelper.IsEmpty(rowObj.GetValue1(this.Name)))
                        rowObj.AppendError(
                                            rowObj.Columns[this.Name],
                                            ErrorCode.ValueValidate,
                                            string.Format(LanguageHelper.Words("validate.required"), this.Title)
                                            );

                    if (this.MinLength > 0 && rowObj.GetValue(this.Name).GetString().Length > 0 && (rowObj.GetValue(this.Name).GetString().Length < this.MinLength || rowObj.GetValue1(this.Name).GetString().Length < this.MinLength))
                        rowObj.AppendError(
                                            rowObj.Columns[this.Name],
                                            ErrorCode.ValueValidate,
                                            string.Format(LanguageHelper.Words("validate.minlength"), this.Title, Math.Min(rowObj.GetValue(this.Name).GetString().Length, rowObj.GetValue1(this.Name).GetString().Length), this.MinLength));

                    if (this.MaxLength > 0 && rowObj.GetValue(this.Name).GetString().Length > 0 && (rowObj.GetValue(this.Name).GetString().Length > this.MaxLength || rowObj.GetValue1(this.Name).GetString().Length > this.MaxLength))
                        rowObj.AppendError(
                                            rowObj.Columns[this.Name],
                                            ErrorCode.ValueValidate,
                                            string.Format(LanguageHelper.Words("validate.maxlength"), this.Title, Math.Max(rowObj.GetValue(this.Name).GetString().Length, rowObj.GetValue1(this.Name).GetString().Length), this.MaxLength)
                                            );

                    if (rowObj.GetValue(this.Name).GetString() != rowObj.GetValue1(this.Name).GetString())
                        rowObj.AppendError(
                                            rowObj.Columns[this.Name],
                                            ErrorCode.ValueValidate,
                                            string.Format(LanguageHelper.Words("validate.notmatch"), this.Title, this.Title)
                                            );

                    if (rowObj.IsError(this.Name) == false)
                    {
                        rowObj.SetValue(this.Name, rowObj.GetValue(this.Name).GetString().MD5Hash());
                    }
                    break;

                case EInput.Checkbox:
                    if(this.Required && rowObj.GetValue(this.Name).GetCheckbox().Count==0)
                        rowObj.AppendError(
                                            rowObj.Columns[this.Name],
                                            ErrorCode.ValueValidate,
                                            string.Format(LanguageHelper.Words("validate.required"), this.Title)
                                            );
                    if (rowObj.IsError(this.Name) == false)
                    {
                        rowObj.SetValue(this.Name, rowObj.GetValue(this.Name).GetCheckbox());
                    }
                    break;
                case EInput.ImageUrl:
                case EInput.ImageContent:
                case EInput.FileUrl:
                case EInput.FileContent:
                case EInput.Read:
                case EInput.Custom:
                    break;
            }
        }
        #endregion
    }


    public class ListRef
    {
        public ListRef() {
            this.Collection = string.Empty;
            this.valueTable = string.Empty;
            this.ValueKey = string.Empty;
        }
        public ListRef(string collection) :this()
        {
            this.Collection = collection;
        }
        public ListRef(string collection, string valueTable, string valueKey) : this(collection)
        {
            this.valueTable = valueTable;
            this.ValueKey = valueKey;
        }

        // Filter:  ECompare.Include:  collection used for normal column name for normal search.
        public string Collection { get; set; }
        [JsonIgnore]
        public string valueTable { get; set; }
        [JsonIgnore]
        public string ValueKey { get; set; }
        [JsonIgnore]
        public string DeleteString
        {
            get
            {
                return $"DELETE FROM {this.valueTable} WHERE {{0}}=@checkbox_refkey";
            }
        }
        [JsonIgnore]
        public string InsertString
        {
            get
            {
                return $"INSERT {this.valueTable} ({{0}}, {this.ValueKey}) VALUES (@checkbox_refkey, @check_valuekey)";
            }
        }
        [JsonIgnore]
        public string QueryString
        {
            get
            {
                return $"SELECT {this.ValueKey} AS CKValue FROM {this.valueTable} WHERE {{0}}=@checkbox_refkey";
            }
        }
    }

}
