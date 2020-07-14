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
            string query = $@"SELECT Place, {this.DSQL.LangSmartColumn("Pub_WebContent.Detail")} as Detail 
FROM Pub_WebContent 
INNER JOIN Pub_Menu 
ON Pub_Menu.Id = Pub_WebContent.PubMenuId 
WHERE 
	Pub_Menu.Deleted = 0 AND Pub_Menu.Active = 1 AND 	
	Pub_WebContent.Deleted = 0 AND Pub_WebContent.Active = 1 AND
	Pub_Menu.MenuId = @MenuId";
            GTable gtable = this.DSQL.ExecuteTable(query, new Dictionary<string, object>{{"MenuId", this.MenuId}} );
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
