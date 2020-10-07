using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

using Library.V1.Common;
using Library.V1.SQL;
using Newtonsoft.Json;

namespace Library.V1.Entity
{
    public enum ECollectionType
    {
        Category,
        Table,
        Common
    }

    public class CollectionTable
    {
        #region Constructors
        public CollectionTable()
        {
            this.Name = string.Empty;
            this.TableName = string.Empty;
            this.IsLang = false;
            this.ValueColumn = string.Empty;
            this.TitleColumn = string.Empty;
            this.DetailColumn = string.Empty;
            this.RefColumn = string.Empty;
            this.Order = string.Empty;
            this.By = string.Empty;
        }
        public CollectionTable(string name) : this()
        {
            this.Name = name;
        }
        public CollectionTable(string name, string tabName) : this(name)
        {
            this.TableName = tabName;
        }
        public CollectionTable(string name, string tabName, string valueColumn, string titleColumn, string detailColumn = "", string refColumn = "", string orderSort = "", string orderBy = "") : this(name, tabName)
        {
            this.ValueColumn = valueColumn;
            this.TitleColumn = titleColumn;
            this.DetailColumn = detailColumn;
            this.RefColumn = refColumn;
            this.Order = orderSort;
            this.By = orderBy;
        }
        public CollectionTable(string name, string tabName, bool isLang, string valueColumn, string titleColumn, string detailColumn="", string refColumn="", string orderSort="", string orderBy="")
            :this(name, tabName, valueColumn, titleColumn, detailColumn, refColumn, orderSort, orderBy)
        {
            this.IsLang = isLang;
        }
        #endregion

        #region Publid Fields
        public string Name { get; set; }
        public string TableName { get; set; }
        public bool IsLang { get; set; }
        public string ValueColumn { get; set; }
        public string TitleColumn { get; set; }
        public string DetailColumn { get; set; }
        public string RefColumn { get; set; }
        public string Order { get; set; }
        public string By { get; set; }
        #endregion
    }
    public class Collection
    {
        public Collection(string name)
        {
            this.Name = name;
            this.Type = ECollectionType.Common;
            this.CollectTable = new CollectionTable(string.Empty);
            this.Items = new List<CollectItem>();
            this.Error = new Error();
            this.DSQL = new SqlHelper();
            this.SqlWhere = new SQLWhere();
        }
        public Collection(ECollectionType type, CollectionTable collectTable):this(collectTable.Name)
        {
            this.Type = type;
            this.CollectTable = collectTable;
        }

        #region Public Fields
        public string Name { get; set; }
        public List<CollectItem> Items { get; set; }
        public Error Error { get; set; }
        public string Debug { get; set; }
        #endregion
     
        #region Private Fields
        private ECollectionType Type { get; set; }
        private CollectionTable CollectTable { get; set; }
        private SqlHelper DSQL { get; set; }
        private SQLWhere SqlWhere { get; set; }
        #endregion

        #region Public Methods
        public void SetConfig(SqlHelper dsql)
        {
            this.DSQL = dsql;
        }
        public Collection AddItem(int value, string title, string detail = "", int refVal = 0)
        {
            this.Items.Add(new CollectItem { Value = value, Title = title, Detail = detail, RefValue = refVal });
            return this;
        }
        public Collection AddFilter(string dbColName, ECompare compare, object value)
        {

            if (string.IsNullOrWhiteSpace(dbColName) == false)
            {
                switch (compare)
                {
                    case ECompare.Like:
                        this.SqlWhere.Add($"{dbColName} LIKE '%{value.GetString()}%'");
                        break;
                    case ECompare.NotLike:
                        this.SqlWhere.Add($"{dbColName} NOT LIKE '%{value.GetString()}%'");
                        break;
                    case ECompare.Equal:
                        this.SqlWhere.Add($"{dbColName} = '{value.GetString()}'");
                        break;
                    case ECompare.NotEqual:
                        this.SqlWhere.Add($"{dbColName} <> '{value.GetString()}'");
                        break;
                    case ECompare.Gthan:
                        this.SqlWhere.Add($"{dbColName} >= '{value.GetString()}'");
                        break;
                    case ECompare.Lthan:
                        this.SqlWhere.Add($"{dbColName} <= '{value.GetString()}'");
                        break;
                    case ECompare.In:
                        List<int> values = value as List<int>;
                        if (values != null)
                        {
                            if (values.Count > 0)
                            {
                                List<SqlParameter> sqlParams = new List<SqlParameter>();
                                string filterString = string.Join(",", values.Select(p => $"@filter_{dbColName}_{p}").ToArray<string>());
                                values.ForEach(p => sqlParams.Add(new SqlParameter($"@filter_{dbColName}_{p}", p)));
                                this.SqlWhere.Add($"{dbColName} IN ({filterString})", sqlParams.ToArray());
                            }
                            else
                            {
                                this.SqlWhere.Add($"1=0");
                            }
                        }
                        else
                        {
                            this.SqlWhere.Add($"1=0");
                        }
                        break;
                }
            }
            return this;
        }
        public void FillData()
        {
            this.Items.Clear();
            switch(this.Type)
            {
                case ECollectionType.Category:
                    {
                        string cvalue = $"b.{this.CollectTable.ValueColumn}";
                        string rvalue = string.IsNullOrWhiteSpace(this.CollectTable.RefColumn) ? "0" : $"b.{this.CollectTable.RefColumn}";
                        string ctitle = this.CollectTable.IsLang ? this.DSQL.LangSmartColumn($"b.{this.CollectTable.TitleColumn}") : $"b.{this.CollectTable.TitleColumn}";
                        string cdetail = this.CollectTable.IsLang ? this.DSQL.LangSmartColumn($"b.{this.CollectTable.DetailColumn}") : $"b.{this.CollectTable.DetailColumn}";
                        cdetail = string.IsNullOrWhiteSpace(this.CollectTable.DetailColumn) ? "''" : cdetail;
                        if (string.IsNullOrWhiteSpace(this.CollectTable.By)==false)
                        {
                            string[] bys = this.CollectTable.By.Split(',');
                            string newBy = string.Empty;
                            foreach (string tby in bys) newBy = newBy.Concat($"b.{tby.Trim()}", ",");
                            this.CollectTable.By = newBy;
                        }
                        string orderBy = this.OrderByString();
                        string query = $"SELECT {cvalue} AS value, {rvalue} AS rvalue, {ctitle} AS title, {cdetail} AS detail FROM GCategory a INNER JOIN GCategoryItem b ON (a.Id=b.CategoryId AND a.TableName='{this.CollectTable.TableName}') WHERE b.Deleted=0 AND b.Active=1 AND {this.SqlWhere.WhereGetFilter} {orderBy}";

                        if (this.DSQL.IsDebug) this.Debug += $"Query:[{query}]\n\n";
                        GTable stable = this.DSQL.ExecuteTable(query, this.SqlWhere.GetParameters());

                        foreach (var row in stable.Rows)
                        {
                            this.AddItem(row.GetValue("value").GetInt()??0, row.GetValue("title"), row.GetValue("detail"), row.GetValue("rvalue").GetInt()??0);
                        }

                        if (this.DSQL.Error.HasError) this.Error = this.DSQL.Error;
                    }
                    break;
                case ECollectionType.Table:
                    {
                        string cvalue = this.CollectTable.ValueColumn;
                        string rvalue = string.IsNullOrWhiteSpace(this.CollectTable.RefColumn) ? "0" : this.CollectTable.RefColumn;
                        string ctitle = this.CollectTable.IsLang ? this.DSQL.LangSmartColumn(this.CollectTable.TitleColumn) : this.CollectTable.TitleColumn;
                        string cdetail = this.CollectTable.IsLang ? this.DSQL.LangSmartColumn(this.CollectTable.DetailColumn) : this.CollectTable.DetailColumn;
                        cdetail = string.IsNullOrWhiteSpace(cdetail) ? "''" : cdetail;

                        string orderBy = this.OrderByString();
                        string query = $"SELECT {cvalue} AS value, {rvalue} AS rvalue, {ctitle} AS title, {cdetail} AS detail FROM {this.CollectTable.TableName} WHERE Deleted=0 AND Active=1 AND {this.SqlWhere.WhereGetFilter} {orderBy}";

                        if (this.DSQL.IsDebug) this.Debug += $"Query:[{query}]\n\n";
                        GTable stable = this.DSQL.ExecuteTable(query, this.SqlWhere.GetParameters());
                        foreach (var row in stable.Rows)
                        {
                            this.AddItem(row.GetValue("value").GetInt()??0, row.GetValue("title"), row.GetValue("detail"), row.GetValue("rvalue").GetInt()??0);
                        }

                        if (this.DSQL.Error.HasError) this.Error = this.DSQL.Error;
                    }
                    break;
                case ECollectionType.Common:
                    this.Items = CommonCollection.Get(this.Name);
                    break;
            }
        }
        #endregion

        #region Private Methods
        private string OrderByString()
        {
            string orderBy = string.Empty;
            if (!string.IsNullOrWhiteSpace(this.CollectTable.By) && !string.IsNullOrWhiteSpace(this.CollectTable.Order))
            {
                if (this.CollectTable.By.Contains(","))
                {
                    string[] bys = this.CollectTable.By.Split(',');
                    string[] orders = this.CollectTable.Order.Split(',');
                    List<string> orderbys = new List<string>();
                    for (var idx = 0; idx < bys.Length; idx++)
                    {
                        orderbys.Add($"{bys[idx]} {orders[idx]}");

                    }
                    string orderbyString = string.Join(", ", orderbys);
                    orderBy = $"ORDER BY {orderbyString}";
                }
                else
                    orderBy = $"ORDER BY {this.CollectTable.By} {this.CollectTable.Order.ToUpper()}";
            }
            else if (!string.IsNullOrWhiteSpace(this.CollectTable.By))
            {
                string sort = "ASC";
                if (this.CollectTable.By.Contains(","))
                {
                    string[] bys = this.CollectTable.By.Split(',');
                    List<string> orderbys = new List<string>();
                    for (var idx = 0; idx < bys.Length; idx++)
                    {
                        orderbys.Add($"{bys[idx]} {sort}");
                    }
                    string orderbyString = string.Join(", ", orderbys);
                    orderBy = $"ORDER BY {orderbyString}";
                }
                else
                    orderBy = $"ORDER BY {this.CollectTable.By} {sort}";

            }

            return orderBy;
        }
        #endregion
    }
    public class CollectItem
    {
        #region Publid Fields
        public int Value { get; set; }
        public int RefValue { get; set; }
        public string Title { get; set; }
        public string Detail { get; set; }
        #endregion
    }

    public static class CommonCollection
    {
        #region Common List
        public static List<CollectItem> Get(string collectName)
        {
            List<CollectItem> items = new List<CollectItem>();
            switch (collectName)
            {
                case "GenderList":
                    items = GetGender().Items;
                    break;
                case "MonthList":
                    items = GetMonth().Items;
                    break;
                case "DayList":
                    items = GetDay().Items;
                    break;
                case "PositionList":
                    items = GetPosition().Items;
                    break;
                case "MenuTypeList":
                    items = GetMenuType().Items;
                    break;
                case "StatusList":
                    items = GetStatus().Items;
                    break;
                case "PaidMethodList":
                    items = GetPaidMethod().Items;
                    break;
            }
            return items;
        }
        public static Collection GetGender()
        {
            Collection list = new Collection("GenderList");
            list.AddItem(1, LanguageHelper.Words("gender.male"), LanguageHelper.Words("gender.male"));
            list.AddItem(2, LanguageHelper.Words("gender.female"), LanguageHelper.Words("gender.female"));
            return list;
        }
        public static Collection GetMonth()
        {
            Collection list = new Collection("MonthList");
            list.AddItem(1, LanguageHelper.Words("month.jan"), LanguageHelper.Words("month.jan"));
            list.AddItem(2, LanguageHelper.Words("month.feb"), LanguageHelper.Words("month.feb"));
            list.AddItem(3, LanguageHelper.Words("month.mar"), LanguageHelper.Words("month.mar"));
            list.AddItem(4, LanguageHelper.Words("month.apr"), LanguageHelper.Words("month.apr"));
            list.AddItem(5, LanguageHelper.Words("month.may"), LanguageHelper.Words("month.may"));
            list.AddItem(6, LanguageHelper.Words("month.jun"), LanguageHelper.Words("month.jun"));
            list.AddItem(7, LanguageHelper.Words("month.jul"), LanguageHelper.Words("month.jul"));
            list.AddItem(8, LanguageHelper.Words("month.aug"), LanguageHelper.Words("month.aug"));
            list.AddItem(9, LanguageHelper.Words("month.sep"), LanguageHelper.Words("month.sep"));
            list.AddItem(10, LanguageHelper.Words("month.oct"), LanguageHelper.Words("month.oct"));
            list.AddItem(11, LanguageHelper.Words("month.nov"), LanguageHelper.Words("month.nov"));
            list.AddItem(12, LanguageHelper.Words("month.dec"), LanguageHelper.Words("month.dec"));
            return list;
        }
        public static Collection GetDay()
        {
            Collection list = new Collection("DayList");
            for (int i = 1; i <= 31; i++)
            {
                list.AddItem(i, i.ToString(), i.ToString());
            }
            return list;
        }
        public static Collection GetPosition()
        {
            Collection list = new Collection("PositionList");
            list.AddItem(1, LanguageHelper.Words("position.top"), LanguageHelper.Words("position.top"));
            list.AddItem(2, LanguageHelper.Words("position.bottom"), LanguageHelper.Words("position.bottom"));
            return list;
        }
        public static Collection GetMenuType()
        {
            Collection list = new Collection("MenuTypeList");
            list.AddItem(1, LanguageHelper.Words("menutype.public"), LanguageHelper.Words("menutype.public"));
            list.AddItem(2, LanguageHelper.Words("menutype.private"), LanguageHelper.Words("menutype.private"));
            return list;
        }

        public static Collection GetStatus()
        {
            Collection list = new Collection("StatusList");
            list.AddItem(1, LanguageHelper.Words("status.hidden"), LanguageHelper.Words("status.hidden"));
            list.AddItem(2, LanguageHelper.Words("status.internal"), LanguageHelper.Words("status.internal"));
            list.AddItem(3, LanguageHelper.Words("status.publish"), LanguageHelper.Words("status.publish"));
            list.AddItem(4, LanguageHelper.Words("status.progress"), LanguageHelper.Words("status.progress"));
            list.AddItem(5, LanguageHelper.Words("status.close"), LanguageHelper.Words("status.close"));
            return list;
        }

        public static Collection GetPaidMethod()
        {
            Collection list = new Collection("PaidMethodList");
            list.AddItem(1, LanguageHelper.Words("class.class"), LanguageHelper.Words("class.class"));
            list.AddItem(2, LanguageHelper.Words("donation"), LanguageHelper.Words("donation"));
            return list;
        }

        #endregion
    }
}
