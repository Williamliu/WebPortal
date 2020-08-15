using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Library.V1.Common;
using Library.V1.Entity;
using Library.V1.SQL;
using Microsoft.AspNetCore.Html;
using System.Text;
using System.Data.SqlClient;

namespace Web.Portal.Common
{
    public static class HtmlHelperWebComponentExt
    {
        #region HTML Component
        public static HtmlString WLSelect<t>(this Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<t> htmlHelper, string name, GTable table, string style = "", int selectValue = 0)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"<select wliu id={name} name={name} {style}>");
            sb.Append($"<option value='0'{(selectValue == 0 ? " selected" : "")}></option>");
            foreach (GRow row in table.Rows)
            {
                sb.Append($"<option value='{row.GetValue("Value").GetInt() ?? 0}'{(selectValue == (row.GetValue("Value").GetInt() ?? 0) ? " selected" : "")}>{row.GetValue("Title")}</option>");
            }
            sb.Append($"</select>");
            return new HtmlString(sb.ToString());
        }
        #endregion
    }
}
