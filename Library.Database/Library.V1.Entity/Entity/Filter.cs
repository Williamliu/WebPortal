using Library.V1.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

using Library.V1.SQL;
using System.Data.SqlClient;
using System.Linq;

namespace Library.V1.Entity
{
    public enum EFilter
    {
        Hidden,
        Scan,
        String,
        Int,
        Long,
        Float,
        Date,
        DateTime,
        Time,
        Checkbox
    }
    public enum ECompare
    {
        Like = 0,
        Equal = 1,
        NotEqual = 2,
        Gthan = 3,
        Lthan = 4,
        Range = 5,
        Include = 6,
        Checkbox = 7,
        In
    }
    public class Filter
    {
        public Filter()
        {
            this.Type = EFilter.String;
            this.Compare = ECompare.Like;
            this.Value1 = null;
            this.Value2 = null;
            this.Error = new Error();
        }

        #region Public Fields
        public string Name { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool Required { get; set; }
        public object Value1 { get; set; }
        public object Value2 { get; set; }
        public ListRef ListRef { get; set; }
        public Error Error { get; set; }
        [JsonIgnore]
        public bool HasError
        {
            get
            {
                return this.Error.HasError;
            }
        }
        #endregion

        #region Internal Use, not for client side json
        [JsonIgnore]
        public string DbName { get; set; }
        private EFilter _ftype;
        [JsonIgnore]
        public EFilter Type { 
            get
            {
                return _ftype;
            }  
            set 
            {
                _ftype = value;
                if (_ftype == EFilter.Checkbox) this.Value1 = new Dictionary<string, bool>();
            } 
        }
        [JsonIgnore]
        public ECompare Compare { get; set; }
        [JsonIgnore]
        public int MinLength { get; set; }
        [JsonIgnore]
        public int MaxLength { get; set; }
        [JsonIgnore]
        public int Min { get; set; }
        [JsonIgnore]
        public int Max { get; set; }
        [JsonIgnore]
        public SQLWhereSearch WhereSearch
        {
            get
            {
                SQLWhereSearch wsearch = new SQLWhereSearch();
                if (this.HasError)
                {
                    wsearch.Add("1=0");
                }
                else
                {
                    string nameStrs = string.IsNullOrWhiteSpace(this.DbName) ? this.Name : this.DbName;
                    string[] dbNames = nameStrs.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string tdbName in dbNames)
                    {
                        SQLWhereColumn wcolumn = new SQLWhereColumn();
                        string dbName = tdbName.Trim();
                        switch (this.Compare)
                        {
                            case ECompare.Like:
                                if (this.Required && ObjectHelper.IsEmpty(this.Value1))
                                {
                                    wcolumn.Add("1=0");
                                }
                                else if (ObjectHelper.IsEmpty(this.Value1) == false)
                                {
                                    wcolumn.Add($"{dbName} LIKE @filter_{this.Name}_{dbName}_value1", new SqlParameter($"@filter_{this.Name}_{dbName}_value1", this.GetValue1()));
                                }
                                break;
                            case ECompare.Equal:
                                if (this.Required && ObjectHelper.IsEmpty(this.Value1))
                                {
                                    wcolumn.Add("1=0");
                                }
                                else if (ObjectHelper.IsEmpty(this.Value1) == false)
                                {
                                    wcolumn.Add($"{dbName} = @filter_{this.Name}_{dbName}_value1", new SqlParameter($"@filter_{this.Name}_{dbName}_value1", this.GetValue1()));
                                }
                                break;
                            case ECompare.NotEqual:
                                if (this.Required && ObjectHelper.IsEmpty(this.Value1))
                                {
                                    wcolumn.Add("1=0");
                                }
                                else if (ObjectHelper.IsEmpty(this.Value1) == false)
                                {
                                    wcolumn.Add($"{dbName} <> @filter_{this.Name}_{dbName}_value1", new SqlParameter($"@filter_{this.Name}_{dbName}_value1", this.GetValue1()));
                                }
                                break;
                            case ECompare.Gthan:
                                if (this.Required && ObjectHelper.IsEmpty(this.Value1))
                                {
                                    wcolumn.Add("1=0");
                                }
                                else if (ObjectHelper.IsEmpty(this.Value1) == false)
                                {
                                    wcolumn.Add($"{dbName} >= @filter_{this.Name}_{dbName}_value1", new SqlParameter($"@filter_{this.Name}_{dbName}_value1", this.GetValue1()));
                                }
                                break;
                            case ECompare.Lthan:
                                if (this.Required && ObjectHelper.IsEmpty(this.Value1))
                                {
                                    wcolumn.Add("1=0");
                                }
                                else if (ObjectHelper.IsEmpty(this.Value1) == false)
                                {
                                    wcolumn.Add($"{dbName} <= @filter_{this.Name}_{dbName}_value1", new SqlParameter($"@filter_{this.Name}_{dbName}_value1", this.GetValue1()));
                                }
                                break;
                            case ECompare.Range:
                                if (this.Required && ObjectHelper.IsEmpty(this.Value1) && ObjectHelper.IsEmpty(this.Value2))
                                {
                                    wcolumn.Add("1=0");
                                }
                                else if (!ObjectHelper.IsEmpty(this.Value1) && !ObjectHelper.IsEmpty(this.Value2))
                                {
                                    wcolumn.Add(
                                            $"{dbName} BETWEEN @filter_{this.Name}_{dbName}_value1 AND @filter_{this.Name}_{dbName}_value2",
                                            new SqlParameter($"@filter_{this.Name}_{dbName}_value1", this.GetValue1()),
                                            new SqlParameter($"@filter_{this.Name}_{dbName}_value2", this.GetValue2())
                                            );
                                }
                                else if (!ObjectHelper.IsEmpty(this.Value1))
                                {
                                    wcolumn.Add(
                                            $"{dbName} >= @filter_{this.Name}_{dbName}_value1",
                                            new SqlParameter($"@filter_{this.Name}_{dbName}_value1", this.GetValue1())
                                            );
                                }
                                else if (!ObjectHelper.IsEmpty(this.Value2))
                                {
                                    wcolumn.Add(
                                            $"{dbName} <= @filter_{this.Name}_{dbName}_value2",
                                            new SqlParameter($"@filter_{this.Name}_{dbName}_value2", this.GetValue2())
                                            );
                                }
                                break;
                            case ECompare.Include:
                                // Pub_User (Id, MemeberId) -> Pub_User_Id (UserId,IdNumber)
                                // this.dbName = "Id",  RefList("MemeberId"|"", "Pub_User_Id", "UserId|IdNumber")
                                if (this.Required && ObjectHelper.IsEmpty(this.Value1))
                                {
                                    wcolumn.Add("1=0");
                                }
                                else if (ObjectHelper.IsEmpty(this.Value1) == false)
                                {
                                    // combine with normal column search, Collection is normal column search
                                    if (string.IsNullOrWhiteSpace(this.ListRef.Collection) == false)
                                    {
                                        string[] refCols = this.ListRef.ValueKey.Split('|');
                                        wcolumn.Add(
                                        $"({this.ListRef.Collection} = @filter_{this.Name}_{dbName}_value1 OR {dbName} IN (SELECT {refCols[0]} FROM {this.ListRef.valueTable} WHERE Deleted=0 AND Active=1 AND {refCols[1]} = @filter_{this.Name}_{dbName}_value1) )",
                                            new SqlParameter($"@filter_{this.Name}_{dbName}_value1", this.GetValue1())
                                        );
                                    }
                                    else
                                    {
                                        string[] refCols = this.ListRef.ValueKey.Split('|');
                                        wcolumn.Add(
                                                $"{dbName} IN (SELECT {refCols[0]} FROM {this.ListRef.valueTable} WHERE Deleted=0 AND Active=1 AND {refCols[1]} = @filter_{this.Name}_{dbName}_value1)",
                                                new SqlParameter($"@filter_{this.Name}_{dbName}_value1", this.GetValue1())
                                            );
                                    }
                                }
                                break;
                            case ECompare.Checkbox:  // Filter EInput.Checkbox must use ECompare.Checkbox :compare multiple select values
                                Dictionary<string, bool> ckValues = this.Value1.GetCheckbox();
                                if (this.Required && ckValues.Count == 0)
                                {
                                    wcolumn.Add("1=0");
                                }
                                else if (ckValues.Count > 0)
                                {
                                    // { {"1", true}, {"2", false}, {"3", true} }
                                    //  @filter_xxx_1, @filter_xxx_3 
                                    List<SqlParameter> sqlParams = new List<SqlParameter>();
                                    string filterString = string.Empty;
                                    foreach (string ckValue in ckValues.Keys)
                                    {
                                        if (ckValues[ckValue])
                                        {
                                            filterString = filterString.Concat($"@filter_{this.Name}_{dbName}_{ckValue}", ",");
                                            sqlParams.Add(new SqlParameter($"@filter_{this.Name}_{dbName}_{ckValue}", ckValue.GetInt()));
                                        }
                                    }
                                    if (string.IsNullOrWhiteSpace(filterString) == false)
                                    {
                                        wcolumn.Add($"{dbName} IN ({filterString})", sqlParams.ToArray());
                                    }
                                }
                                break;
                            case ECompare.In:  // compare single value
                                {
                                    // Admin_User.Branch in AdminUser.ActiveBranches[1,2,3]                                    
                                    List<int> values = this.Value1 as List<int>;
                                    if (this.Required && values.Count==0)
                                    {
                                        wcolumn.Add("1=0");
                                    }
                                    else if (values.Count > 0)
                                    {
                                        List<SqlParameter> sqlParams = new List<SqlParameter>();
                                        string inFStr = string.Empty;
                                        foreach (int inVal in values)
                                        {
                                            inFStr = inFStr.Concat($"@filter_{this.Name}_{dbName}_{inVal}", ", ");
                                            sqlParams.Add(new SqlParameter($"@filter_{this.Name}_{dbName}_{inVal}", inVal));
                                        }
                                        wcolumn.Add($"{dbName} IN ({inFStr})", sqlParams.ToArray());
                                    }
                                }
                                break;
                        }
                        wsearch.Add(wcolumn);
                    }
                }
                return wsearch;
            }
        }
        #endregion

        #region Methods
        public void AddListRef(string collection, string valueTable = "", string valueKey = "")
        {
            this.ListRef = new ListRef(collection, valueTable, valueKey);
        }

        public object GetValue1()
        {
            object val = this.Value1;
            switch (this.Type)
            {
                case EFilter.Hidden:
                    break;
                case EFilter.String:
                    val = this.Value1.GetString();
                    break;
                case EFilter.Int:
                    val = this.Value1.GetInt();
                    break;
                case EFilter.Long:
                    val = this.Value1.GetLong();
                    break;
                case EFilter.Float:
                    val = this.Value1.GetFloat();
                    break;
                case EFilter.DateTime:
                    val = this.Value1.GetDateTime();
                    break;
                case EFilter.Date:
                    val = this.Value1.GetDateTime();
                    break;
                case EFilter.Checkbox:
                    val = this.Value1.GetCheckbox();
                    break;
                case EFilter.Scan:
                    val = this.Value1.GetString();
                    break;
            }
            return val;

        }
        public object GetValue2()
        {
            object val = null;
            switch (this.Type)
            {
                case EFilter.Hidden:
                    val = this.Value2;
                    break;
                case EFilter.String:
                    val = this.Value2.GetString();
                    break;
                case EFilter.Int:
                    val = this.Value2.GetInt();
                    break;
                case EFilter.Long:
                    val = this.Value2.GetLong();
                    break;
                case EFilter.Float:
                    val = this.Value2.GetFloat();
                    break;
                case EFilter.DateTime:
                    val = this.Value2.GetDateTime();
                    break;
                case EFilter.Date:
                    val = this.Value2.GetDateTime();
                    break;
                case EFilter.Checkbox:
                    val = this.Value2.GetCheckbox();
                    break;
                case EFilter.Scan:
                    val = this.Value2.GetString();
                    break;
            }
            return val;

        }
        #endregion

        #region Data Operation Methods
        public void Validate() //filter: pass validate and need to push to where clause
        {
            if (this.Compare == ECompare.In) return;
            if (this.Compare == ECompare.Checkbox) return;

            // Required Value
            if (this.Compare == ECompare.Range)
            {
                if (this.Required && ObjectHelper.IsEmpty(this.Value1.GetString()) && ObjectHelper.IsEmpty(this.Value2.GetString()))
                {
                    this.Error.Append(ErrorCode.FilterValidate, string.Format(LanguageHelper.Words("validate.required"), this.Title));
                }
            }
            else
            {
                if (this.Required && ObjectHelper.IsEmpty(this.Value1.GetString()))
                    this.Error.Append(ErrorCode.FilterValidate, string.Format(LanguageHelper.Words("validate.required"), this.Title));
            }

            // value is invalid or empty,  it will return null value
            if (this.Compare == ECompare.Range)
            {
                if (!ObjectHelper.IsEmpty(this.Value1.GetString()) && !ValidateHelper.IsMatch(this.Type.ToString(), this.Value1.GetString()) || !ObjectHelper.IsEmpty(this.Value2.GetString()) && !ValidateHelper.IsMatch(this.Type.ToString(), this.Value2.GetString()))
                {
                    this.Error.Append(ErrorCode.FilterValidate, string.Format(LanguageHelper.Words("validate.type"), this.Title, LanguageHelper.Words(this.Type.ToString().ToLower())));
                }
            }
            else
            {
                if (!ObjectHelper.IsEmpty(this.Value1.GetString()) && !ValidateHelper.IsMatch(this.Type.ToString(), this.Value1.GetString()))
                {
                    this.Error.Append(ErrorCode.FilterValidate, string.Format(LanguageHelper.Words("validate.type"), this.Title, LanguageHelper.Words(this.Type.ToString().ToLower())));
                }
            }

            switch (this.Type)
            {
                case EFilter.Hidden:
                    break;
                case EFilter.String:
                    if (this.Compare == ECompare.Range)
                    {
                        if (this.MinLength > 0 && this.Value1.GetString().Length < this.MinLength)
                            this.Error.Append(ErrorCode.FilterValidate, string.Format(LanguageHelper.Words("validate.minlength"), this.Title, this.Value1.GetString().Length, this.MinLength));
                        if (this.MinLength > 0 && this.Value2.GetString().Length < this.MinLength)
                            this.Error.Append(ErrorCode.FilterValidate, string.Format(LanguageHelper.Words("validate.minlength"), this.Title, this.Value2.GetString().Length, this.MinLength));

                        if (this.MaxLength > 0 && this.Value1.GetString().Length > this.MaxLength)
                            this.Error.Append(ErrorCode.FilterValidate, string.Format(LanguageHelper.Words("validate.maxlength"), this.Title, this.Value1.GetString().Length, this.MaxLength));
                        if (this.MaxLength > 0 && this.Value2.GetString().Length > this.MaxLength)
                            this.Error.Append(ErrorCode.FilterValidate, string.Format(LanguageHelper.Words("validate.maxlength"), this.Title, this.Value2.GetString().Length, this.MaxLength));

                    }
                    else
                    {
                        if (this.MinLength > 0 && this.Value1.GetString().Length < this.MinLength)
                            this.Error.Append(ErrorCode.FilterValidate, string.Format(LanguageHelper.Words("validate.minlength"), this.Title, this.Value1.GetString().Length, this.MinLength));

                        if (this.MaxLength > 0 && this.Value1.GetString().Length > this.MaxLength)
                            this.Error.Append(ErrorCode.FilterValidate, string.Format(LanguageHelper.Words("validate.maxlength"), this.Title, this.Value1.GetString().Length, this.MaxLength));
                    }
                    if (this.Type == EFilter.String)
                    {
                        this.Value1 = this.Value1.GetString();
                        this.Value2 = this.Value2.GetString();
                    }
                    break;
                case EFilter.Int:
                    if (this.Compare == ECompare.Range)
                    {
                        if (this.Min > 0 && (this.Value1.GetInt()??0) < this.Min)
                            this.Error.Append(ErrorCode.FilterValidate, string.Format(LanguageHelper.Words("validate.min"), this.Title, this.Value1.GetInt()??0, this.Min));
                        if (this.Min > 0 && (this.Value2.GetInt()??0) < this.Min)
                            this.Error.Append(ErrorCode.FilterValidate, string.Format(LanguageHelper.Words("validate.min"), this.Title, this.Value2.GetInt()??0, this.Min));

                        if (this.Max > 0 && (this.Value1.GetInt()??0) > this.Max)
                            this.Error.Append(ErrorCode.FilterValidate, string.Format(LanguageHelper.Words("validate.max"), this.Title, this.Value1.GetInt()??0, this.Max));
                        if (this.Max > 0 && (this.Value2.GetInt()??0) > this.Max)
                            this.Error.Append(ErrorCode.FilterValidate, string.Format(LanguageHelper.Words("validate.max"), this.Title, this.Value2.GetInt()??0, this.Max));

                        this.Value1 = (this.Value1.GetInt() ?? 0) == 0 ? null : this.Value1.GetInt();
                        this.Value2 = (this.Value2.GetInt() ?? 0) == 0 ? null : this.Value2.GetInt();
                    }
                    else
                    {
                        if (this.Min > 0 && (this.Value1.GetInt()??0) < this.Min)
                            this.Error.Append(ErrorCode.FilterValidate, string.Format(LanguageHelper.Words("validate.min"), this.Title, this.Value1.GetInt()??0, this.Min));
                        if (this.Max > 0 && (this.Value1.GetInt()??0) > this.Max)
                            this.Error.Append(ErrorCode.FilterValidate, string.Format(LanguageHelper.Words("validate.max"), this.Title, this.Value1.GetInt()??0, this.Max));

                        this.Value1 = (this.Value1.GetInt() ?? 0) == 0 ? null : this.Value1.GetInt();
                    }
                    break;
                case EFilter.Long:
                    if (this.Compare == ECompare.Range)
                    {
                        if (this.Min > 0 && (this.Value1.GetLong()??0) < this.Min)
                            this.Error.Append(ErrorCode.FilterValidate, string.Format(LanguageHelper.Words("validate.min"), this.Title, this.Value1.GetLong()??0, this.Min));
                        if (this.Min > 0 && (this.Value2.GetLong()??0) < this.Min)
                            this.Error.Append(ErrorCode.FilterValidate, string.Format(LanguageHelper.Words("validate.min"), this.Title, this.Value2.GetLong()??0, this.Min));

                        if (this.Max > 0 && (this.Value1.GetLong()??0) > this.Max)
                            this.Error.Append(ErrorCode.FilterValidate, string.Format(LanguageHelper.Words("validate.max"), this.Title, this.Value1.GetLong()??0, this.Max));
                        if (this.Max > 0 && (this.Value2.GetLong()??0) > this.Max)
                            this.Error.Append(ErrorCode.FilterValidate, string.Format(LanguageHelper.Words("validate.max"), this.Title, this.Value2.GetLong()??0, this.Max));

                        this.Value1 = (this.Value1.GetLong() ?? 0) == 0 ? null : this.Value1.GetLong();
                        this.Value2 = (this.Value2.GetLong() ?? 0) == 0 ? null : this.Value2.GetLong();
                    }
                    else
                    {
                        if (this.Min > 0 && (this.Value1.GetLong()??0) < this.Min)
                            this.Error.Append(ErrorCode.FilterValidate, string.Format(LanguageHelper.Words("validate.min"), this.Title, this.Value1.GetLong()??0, this.Min));
                        if (this.Max > 0 && (this.Value1.GetLong()??0) > this.Max)
                            this.Error.Append(ErrorCode.FilterValidate, string.Format(LanguageHelper.Words("validate.max"), this.Title, this.Value1.GetLong()??0, this.Max));

                        this.Value1 = (this.Value1.GetLong() ?? 0) == 0 ? null : this.Value1.GetLong();
                    }
                    break;
                case EFilter.Float:
                    if (this.Compare == ECompare.Range)
                    {
                        if (this.Min > 0 && (this.Value1.GetFloat()??0) < this.Min)
                            this.Error.Append(ErrorCode.FilterValidate, string.Format(LanguageHelper.Words("validate.min"), this.Title, this.Value1.GetFloat()??0, this.Min));
                        if (this.Min > 0 && (this.Value2.GetFloat()??0) < this.Min)
                            this.Error.Append(ErrorCode.FilterValidate, string.Format(LanguageHelper.Words("validate.min"), this.Title, this.Value2.GetFloat()??0, this.Min));

                        if (this.Max > 0 && (this.Value1.GetFloat()??0) > this.Max)
                            this.Error.Append(ErrorCode.FilterValidate, string.Format(LanguageHelper.Words("validate.max"), this.Title, this.Value1.GetFloat()??0, this.Max));
                        if (this.Max > 0 && (this.Value2.GetFloat()??0) > this.Max)
                            this.Error.Append(ErrorCode.FilterValidate, string.Format(LanguageHelper.Words("validate.max"), this.Title, this.Value2.GetFloat()??0, this.Max));

                        this.Value1 = (this.Value1.GetFloat() ?? 0) == 0 ? null : this.Value1.GetFloat();
                        this.Value2 = (this.Value2.GetFloat() ?? 0) == 0 ? null : this.Value2.GetFloat();
                    }
                    else
                    {
                        if (this.Min > 0 && (this.Value1.GetFloat()??0) < this.Min)
                            this.Error.Append(ErrorCode.FilterValidate, string.Format(LanguageHelper.Words("validate.min"), this.Title, this.Value1.GetFloat()??0, this.Min));
                        if (this.Max > 0 && (this.Value1.GetFloat()??0) > this.Max)
                            this.Error.Append(ErrorCode.FilterValidate, string.Format(LanguageHelper.Words("validate.max"), this.Title, this.Value1.GetFloat()??0, this.Max));

                        this.Value1 = (this.Value1.GetFloat() ?? 0) == 0 ? null : this.Value1.GetFloat();
                    }
                    break;
                case EFilter.Date:
                    if (this.Compare == ECompare.Range)
                    {
                        this.Value1 = this.Value1.GetDate();
                        this.Value2 = this.Value2.GetDateTime()?.AddTicks(TimeSpan.FromSeconds(24 * 3600 - 1).Ticks);
                    }
                    else
                    {
                        this.Value1 = this.Value1.GetDate();
                        this.Value2 = this.Value1.GetDateTime()?.AddTicks(TimeSpan.FromSeconds(24 * 3600 - 1).Ticks);
                    }
                    break;
                case EFilter.DateTime:
                    if (this.Compare == ECompare.Range)
                    {
                        this.Value1 = this.Value1.GetDateTime();
                        this.Value2 = this.Value2.GetDateTime()?.AddTicks(TimeSpan.FromSeconds(24 * 3600 - 1).Ticks);
                    }
                    else
                    {
                        this.Value1 = this.Value1.GetDateTime();
                        this.Value2 = this.Value1.GetDateTime()?.AddTicks(TimeSpan.FromSeconds(24 * 3600 - 1).Ticks);
                    }
                    break;
                case EFilter.Time:
                    if (this.Compare == ECompare.Range)
                    {
                        this.Value1 = this.Value1.GetTime();
                        this.Value2 = this.Value2.GetTime();
                    }
                    else
                    {
                        this.Value1 = this.Value1.GetTime();
                        this.Value2 = this.Value1.GetTimeSpan()?.Add(TimeSpan.FromSeconds(59));
                    }
                    break;
                case EFilter.Checkbox:
                    this.Value1 = this.Value1.GetCheckbox();
                    break;
                case EFilter.Scan:
                    break;
            }
        }
        #endregion
    }

}
