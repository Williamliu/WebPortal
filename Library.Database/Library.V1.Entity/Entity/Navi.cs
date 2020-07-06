using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

using Library.V1.SQL;
using System.Data.SqlClient;

namespace Library.V1.Entity
{
    public class Navi
    {
        public Navi()
        {
            this.IsActive = false;
            this.Order = "";
            this.By = "";
            this.InitFill = true;
            this.IsLoading = false;
        }
        public int PageNo { get; set; }
        public int PageSize { get; set; }
        public int PageTotal { get; set; }
        public int RowTotal { get; set; }
        public bool IsLoading { get; set; }
        public string Order { get; set; }
        public string By { get; set; }
        public bool IsActive { get; set; }
        [JsonIgnore]
        public bool InitFill { get; set; }
        #region Navi Methods
        public void Validate()
        {
            if (this.PageSize <= 0 || this.PageSize > 200) this.PageSize = 20;
        }
        public void Reset(int rowstotal)
        {
            this.Validate();
            if (rowstotal <= 0)
            {
                this.PageNo = 0;
                this.PageTotal = 0;
                this.RowTotal = 0;
            }
            else
            {
                this.RowTotal = rowstotal;
                this.PageTotal = (int)Math.Ceiling((double)this.RowTotal / (double)this.PageSize);

                if (this.PageNo > this.PageTotal) this.PageNo = this.PageTotal;
                if (this.PageNo <= 0) this.PageNo = 1;
            }
        }
        public string NaviPaging()
        {
            string paging = string.Empty;
            if (this.IsActive)
            {
                paging = $"OFFSET {(this.PageNo > 0 ? (this.PageNo - 1) * this.PageSize : 0)} ROW FETCH NEXT {this.PageSize} ROW ONLY";
            }
            return paging;
        }
        public string NaviOrderBy(IDictionary<string, Meta> metas)
        {
            string orderBy = string.Empty;
            if (string.IsNullOrWhiteSpace(this.By))
            {
                foreach (var key in metas.Keys)
                {
                    if (metas[key].IsKey)
                    {
                        this.By = key;
                        this.Order = "DESC";
                        break;
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(this.By) && !string.IsNullOrWhiteSpace(this.Order))
            {
                if (this.By.Contains(","))
                {
                    string[] bys = this.By.Split(',');
                    string[] orders = this.Order.Split(',');
                    List<string> orderbys = new List<string>();
                    for (var idx = 0; idx < bys.Length; idx++)
                    {
                        orderbys.Add($"{bys[idx]} {orders[idx]}");
                    }
                    string orderbyString = string.Join(", ", orderbys);
                    orderBy = $"ORDER BY {orderbyString}";
                }
                else
                    orderBy = $"ORDER BY {this.By} {this.Order.ToUpper()}";
            }
            else if (!string.IsNullOrWhiteSpace(this.By))
            {

                string sort = metas.ContainsKey(this.By) ? (metas[this.By].Order?.ToUpper() ?? "ASC") : "ASC";

                if (this.By.Contains(","))
                {
                    string[] bys = this.By.Split(',');
                    List<string> orderbys = new List<string>();
                    for (var idx = 0; idx < bys.Length; idx++)
                    {
                        orderbys.Add($"{bys[idx]} {sort}");
                    }
                    string orderbyString = string.Join(", ", orderbys);
                    orderBy = $"ORDER BY {orderbyString}";
                }
                else
                    orderBy = $"ORDER BY {this.By} {sort}";
            }
            return orderBy;
        }
        #endregion Navi Methods
    }
}
