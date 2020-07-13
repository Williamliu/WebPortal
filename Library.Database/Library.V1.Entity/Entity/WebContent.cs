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
    public class WebContent
    {
        public WebContent() {
            this.MenuId = string.Empty;
            this.DSQL = new SqlHelper();
            this.Content = new Dictionary<string, string>();
        }
        public WebContent(SqlHelper sql, string menuId) :this()
        {
            this.MenuId = menuId;
            this.DSQL = sql;
        } 
        private string MenuId { get; set; }
        private SqlHelper DSQL { get; set; }
        public IDictionary<string, string> Content { get; set; }

        public void FillData()
        {
            GTable gtable = this.DSQL.ExecuteTable($"SELECT Place, {this.DSQL.LangSmartColumn("Detail")} as Detail FROM Pub_WebContent WHERE Deleted=0 AND Active=1 AND MenuId=@MenuId", new Dictionary<string, object>{{"MenuId", this.MenuId}} );
            foreach(GRow row in  gtable.Rows)
            {
                this.Content.Add(row.GetValue("Place").ToLower(), row.GetValue("Detail"));
            }
        }
        public string HTML(string place)
        {
            if (this.Content.ContainsKey(place))
                return   this.Content[place];
            else
                return string.Empty;
        }
    }
}
