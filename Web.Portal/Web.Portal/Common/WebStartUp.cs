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

namespace Web.Portal.Common
{
    public static class HtmlHelperExt
    {
        public static HtmlString Words<t>(this Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<t> htmlHelper, string keyword)
        {
            return new HtmlString(LanguageHelper.Words(keyword).NL2BR());
        }
        public static HtmlString JWords<t>(this Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<t> htmlHelper)
        {
            //Dictionary<string, string> words = htmlHelper.ViewBag.Words as Dictionary<string, string>;
            Dictionary<string, string> words = LanguageHelper.GetDictionary();
            string ret = "<script>";
            //unstable to read session
            //ret += $"var GAdminSiteJwtToken = '{htmlHelper.ViewContext.HttpContext.Session.GetString("adminSite_jwtToken")}';";
            ret += "function Words(keyword) { ";
            ret += "var wordArr={}; ";
            if (words == null)
            {
                ret += "return keyword.replaceAll('[.]', ' ').ucword(); ";
            }
            else
            {
                foreach (string key in words.Keys)
                {
                    if (String.IsNullOrWhiteSpace(words[key]))
                        ret += $"wordArr['{key}']=`{key.Replace(".", " ")}`; ";
                    else
                        ret += $"wordArr['{key}']=`{words[key].Replace(@"`", @"'")}`; ";
                }
            }
            ret += "if(wordArr[keyword]) return wordArr[keyword]; else return keyword.replaceAll('[.]', ' ').ucword(); ";
            ret += "} ";
            ret += "function ChangeLang(lang) { ";
            ret += "LangForm.elements['lang'].value = lang; ";
            ret += "LangForm.submit(); ";
            ret += "} ";
            ret += "</script>";

            return new HtmlString(ret);
        }
        public static HtmlString LangForm<t>(this Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<t> htmlHelper)
        {
            StringBuilder ret = new StringBuilder();
            string url = htmlHelper.ViewContext.HttpContext.Request.Path;
            ret.Append($"<form name='LangForm' action='{url}' method='get'>");
            if (htmlHelper.ViewContext.HttpContext.Request.QueryString.HasValue)
            {
                foreach (string key in htmlHelper.ViewContext.HttpContext.Request.Query.Keys)
                {
                    if (key != "lang")
                        ret.Append($"<input type='hidden' name='{key}' value='{htmlHelper.ViewContext.HttpContext.Request.Query[key]}' />");
                }
            }
            ret.Append("<input type='hidden' name='lang' value='' />");
            ret.Append("</form>");
            return new HtmlString(ret.ToString());
        }
    }

    public static class WebStartUp
    {
        public static void SaveSession(this HttpContext httpContext, string sessName, string value)
        {
            if(string.IsNullOrWhiteSpace(value))
            {
                httpContext.Session.Remove(sessName);
                // we can not delete request cookies which from user request
                // we can delete response cookie,  but delete response cookies will effect for next request.
                httpContext.Response.Cookies.Delete(sessName);
            }
            else
            {
                byte[] langByte = System.Text.ASCIIEncoding.ASCII.GetBytes(value);
                httpContext.Session.Set(sessName, langByte);

                httpContext.Response.Cookies.Delete(sessName);
                CookieOptions cookieOpt = new CookieOptions();
                cookieOpt.Expires = System.DateTime.Now.AddDays(8);
                httpContext.Response.Cookies.Append(sessName, value, cookieOpt);
            }
        }
        public static string GetSession(this HttpContext httpContext, string sessName)
        {
            if (string.IsNullOrWhiteSpace(httpContext.Session.GetString(sessName)??"") == false)
            {
                return httpContext.Session.GetString(sessName) ?? "";
            }
            else if (string.IsNullOrWhiteSpace(httpContext.Request.Cookies["WebPortalLang"]??"") == false)
            {
                return httpContext.Request.Cookies["WebPortalLang"] ?? "";
            }
            return string.Empty;
        }

        public static string GetWebLang(this HttpContext httpContext)
        {
            string queryLang = "";
            if (httpContext.Request.QueryString.HasValue)
            {
                queryLang = httpContext.Request.Query.ContainsKey("lang")?httpContext.Request.Query["lang"][0].GetString().ToLower():"";
                // save post lang to cookie
                if (!string.IsNullOrWhiteSpace(queryLang))
                {
                    byte[] langByte = System.Text.ASCIIEncoding.ASCII.GetBytes(queryLang);
                    httpContext.Session.Set("WebPortalLang", langByte);
                    httpContext.Response.Cookies.Delete("WebPortalLang");
                    CookieOptions cookieOpt = new CookieOptions();
                    cookieOpt.Expires = System.DateTime.Now.AddDays(365);
                    httpContext.Response.Cookies.Append("WebPortalLang", queryLang, cookieOpt);
                }
            }

            //Priority 2:  using session
            if (string.IsNullOrWhiteSpace(queryLang))
            {
                queryLang = httpContext.Session.GetString("WebPortalLang") ?? "";
            }

            // Priority 3: using Cookies
            if (string.IsNullOrWhiteSpace(queryLang))
            {
                queryLang = httpContext.Request.Cookies["WebPortalLang"] ?? "";
            }

            // Priority 4: using Browser Settings
            if (string.IsNullOrWhiteSpace(queryLang))
            {
                string browserLang = httpContext.Request.Headers["Accept-Language"];
                if (!string.IsNullOrWhiteSpace(browserLang))
                {
                    List<string> acceptLangs = new List<string> { "zh", "cn", "en" };
                    foreach (string acceptLang in acceptLangs)
                    {
                        if (browserLang.Contains(acceptLang))
                        {
                            queryLang = acceptLang;
                        }
                    }
                }
            }
            httpContext.Items.Add("Lang", queryLang);
            return queryLang;
        }
        public static AdminUser GetAdminUser(this HttpContext httpContext, SqlHelper DSQL, string menuId)
        {
            AdminUser adminUser = new AdminUser();
            string guid = string.Empty;
            // Request Header -> Session -> Cookie
            if (string.IsNullOrWhiteSpace(httpContext.Request.Headers["AdminSession"].GetString()) == false) 
                guid = httpContext.Request.Headers["AdminSession"].GetString();
            else if (string.IsNullOrWhiteSpace(httpContext.Session.GetString("adminSite_Session")) == false) 
                guid = httpContext.Session.GetString("adminSite_Session");
            else if(string.IsNullOrWhiteSpace(httpContext.Request.Cookies["adminSite_Session"].GetString()) == false) 
                guid = httpContext.Request.Cookies["adminSite_Session"].GetString();

            if (string.IsNullOrWhiteSpace(guid) == false)
            {
                #region Get User by Session
                string query = "select top 1 * from VW_Admin_Session_User WHERE Session=@Session";
                Dictionary<string, object> ps = new Dictionary<string, object>();
                ps.Add("Session", guid);
                List<Dictionary<string, string>> rows = DSQL.Query(query, ps);
                if (rows.Count > 0)
                {
                    adminUser.Id = rows[0]["Id"].GetInt() ?? 0;
                    adminUser.FirstName = rows[0]["FirstName"].GetString();
                    adminUser.LastName = rows[0]["LastName"].GetString();
                    adminUser.UserName = rows[0]["UserName"].GetString();
                    adminUser.Email = rows[0]["Email"].GetString();
                    adminUser.Phone = rows[0]["Phone"].GetString();
                    adminUser.Branch = rows[0]["BranchId"].GetInt() ?? 0;
                    adminUser.IsAdmin = rows[0]["IsAdmin"].GetBool() ?? false;
                }
                #endregion

                #region  Branches and Sites
                query = "SELECT * FROM VW_Admin_User_Branch WHERE Id=@UserId";
                ps.Clear();
                ps.Add("UserId", adminUser.Id);
                rows = DSQL.Query(query, ps);
                adminUser.ActiveBranches = rows.Select(p => p["BranchId"].GetInt() ?? 0).ToList<int>();
                if (adminUser.ActiveBranches.Contains(adminUser.Branch) == false)
                    adminUser.ActiveBranches.Add(adminUser.Branch);

                query = "SELECT * FROM VW_Admin_User_Branch_Full WHERE Id=@UserId";
                rows = DSQL.Query(query, ps);
                adminUser.Branches = rows.Select(p => p["BranchId"].GetInt() ?? 0).ToList<int>();

                query = "SELECT * FROM VW_Admin_User_Site WHERE Id=@UserId";
                rows = DSQL.Query(query, ps);
                adminUser.ActiveSites = rows.Select(p => p["SiteId"].GetInt() ?? 0).ToList<int>();

                query = "SELECT * FROM VW_Admin_User_Site_Full WHERE Id=@UserId";
                rows = DSQL.Query(query, ps);
                adminUser.Sites = rows.Select(p => p["SiteId"].GetInt() ?? 0).ToList<int>();
                #endregion

                #region ViewMenu and PubMenu
                query = "SELECT * FROM VW_Admin_User_Menu_View WHERE Id=@UserId";
                rows = DSQL.Query(query, ps);
                adminUser.ViewMenus.AddRange(rows.Select(p => p["Menu"].GetString()).ToList<string>());

                AdminPubMenus AdminPubMenu = new AdminPubMenus();
                adminUser.PubMenus.AddRange(AdminPubMenu.GetPublicMenus());
                #endregion

                #region Restrict Fields
                query = "SELECT * FROM VW_Admin_User_Menu_Field WHERE UserId=@UserId AND MenuId=@MenuId";
                ps.Add("MenuId", menuId);
                rows = DSQL.Query(query, ps);
                foreach (Dictionary<string, string> row in rows)
                {
                    string restrictField = row["Fields"].GetString() ?? "";
                    string[] rfields = restrictField.Split(",");
                    foreach (string tfield in rfields)
                    {
                        if (string.IsNullOrWhiteSpace(tfield.Trim()) == false && adminUser.Fields.Contains(tfield.Trim()) == false)
                            adminUser.Fields.Add(tfield.Trim());
                    }
                }
                #endregion

                #region User Rights: if admin, full right;  ViewMenu[MenuId] base on MenuId, PubMenu[MenuId] base on full right 
                if (adminUser.IsAdmin)
                {
                    // if Admin, full right
                    adminUser.Rights.Clear();
                    query = "SELECT Action FROM Admin_Right WHERE deleted=0";
                    ps.Clear();
                    rows = DSQL.Query(query, ps);
                    rows.ForEach(p => adminUser.Rights.Add(p["Action"].GetString(), true));
                }
                else
                {
                    if (adminUser.ViewMenus.Contains(menuId))
                    {
                        // if ViewMenu, base on User Role
                        adminUser.Rights.Clear();
                        query = "SELECT * FROM VW_Admin_User_Menu_Right WHERE UserId=@UserId AND MenuId=@MenuId";
                        rows = DSQL.Query(query, ps);
                        rows.ForEach(p => adminUser.Rights.Add(p["Action"].GetString(), true));
                    }
                    if (adminUser.PubMenus.Contains(menuId))
                    {
                        // if pub menu, full right
                        adminUser.Rights.Clear();
                        query = "SELECT Action FROM Admin_Right WHERE deleted=0";
                        ps.Clear();
                        rows = DSQL.Query(query, ps);
                        rows.ForEach(p => adminUser.Rights.Add(p["Action"].GetString(), true));
                    }
                }
                #endregion
            }
            else
            {
                AdminPubMenus AdminPubMenu = new AdminPubMenus();
                adminUser.PubMenus.AddRange(AdminPubMenu.GetPublicMenus());
                if (adminUser.PubMenus.Contains(menuId))
                {
                    //if pub menu,  full right
                    adminUser.Rights.Add("view", true);
                    adminUser.Rights.Add("detail", true);
                    adminUser.Rights.Add("save", true);
                    adminUser.Rights.Add("add", true);
                    adminUser.Rights.Add("delete", true);
                    adminUser.Rights.Add("print", true);
                    adminUser.Rights.Add("output", true);
                    adminUser.Rights.Add("email", true);
                }
            }

            return adminUser;
        }
        public static AdminUser GetPubUser(this HttpContext httpContext, SqlHelper DSQL, string menuId)
        {
            AdminUser publicUser = new AdminUser();
            string guid = httpContext.Request.Cookies["WebPortal_Session"] ?? httpContext.Session.GetString("WebPortal_Session") ?? httpContext.Request.Headers["PublicSession"].GetString() ?? "";
            if (string.IsNullOrWhiteSpace(guid) == false)
            {
                #region Get User by Session
                string query = "select top 1 * from VW_Admin_Session_User WHERE Session=@Session";
                Dictionary<string, object> ps = new Dictionary<string, object>();
                ps.Add("Session", guid);
                List<Dictionary<string, string>> rows = DSQL.Query(query, ps);
                if (rows.Count > 0)
                {
                    publicUser.Id = rows[0]["Id"].GetInt() ?? 0;
                    publicUser.FirstName = rows[0]["FirstName"].GetString();
                    publicUser.LastName = rows[0]["LastName"].GetString();
                    publicUser.UserName = rows[0]["UserName"].GetString();
                    publicUser.Email = rows[0]["Email"].GetString();
                    publicUser.Phone = rows[0]["Phone"].GetString();
                    publicUser.Branch = rows[0]["BranchId"].GetInt() ?? 0;
                    publicUser.IsAdmin = rows[0]["IsAdmin"].GetBool() ?? false;
                }
                #endregion

                #region  Branches and Sites
                query = "SELECT * FROM VW_Admin_User_Branch WHERE Id=@UserId";
                ps.Clear();
                ps.Add("UserId", publicUser.Id);
                rows = DSQL.Query(query, ps);
                publicUser.ActiveBranches = rows.Select(p => p["BranchId"].GetInt() ?? 0).ToList<int>();
                if (publicUser.ActiveBranches.Contains(publicUser.Branch) == false)
                    publicUser.ActiveBranches.Add(publicUser.Branch);

                query = "SELECT * FROM VW_Admin_User_Branch_Full WHERE Id=@UserId";
                rows = DSQL.Query(query, ps);
                publicUser.Branches = rows.Select(p => p["BranchId"].GetInt() ?? 0).ToList<int>();

                query = "SELECT * FROM VW_Admin_User_Site WHERE Id=@UserId";
                rows = DSQL.Query(query, ps);
                publicUser.ActiveSites = rows.Select(p => p["SiteId"].GetInt() ?? 0).ToList<int>();

                query = "SELECT * FROM VW_Admin_User_Site_Full WHERE Id=@UserId";
                rows = DSQL.Query(query, ps);
                publicUser.Sites = rows.Select(p => p["SiteId"].GetInt() ?? 0).ToList<int>();
                #endregion

                #region ViewMenu and PubMenu
                query = "SELECT * FROM VW_Admin_User_Menu_View WHERE Id=@UserId";
                rows = DSQL.Query(query, ps);
                publicUser.ViewMenus.AddRange(rows.Select(p => p["Menu"].GetString()).ToList<string>());

                AdminPubMenus AdminPubMenu = new AdminPubMenus();
                publicUser.PubMenus.AddRange(AdminPubMenu.GetPublicMenus());
                #endregion

                #region Restrict Fields
                query = "SELECT * FROM VW_Admin_User_Menu_Field WHERE UserId=@UserId AND MenuId=@MenuId";
                ps.Add("MenuId", menuId);
                rows = DSQL.Query(query, ps);
                foreach (Dictionary<string, string> row in rows)
                {
                    string restrictField = row["Fields"].GetString() ?? "";
                    string[] rfields = restrictField.Split(",");
                    foreach (string tfield in rfields)
                    {
                        if (string.IsNullOrWhiteSpace(tfield.Trim()) == false && publicUser.Fields.Contains(tfield.Trim()) == false)
                            publicUser.Fields.Add(tfield.Trim());
                    }
                }
                #endregion

                #region User Rights
                if (publicUser.IsAdmin)
                {
                    publicUser.Rights.Clear();
                    query = "SELECT Action FROM Admin_Right WHERE deleted=0";
                    ps.Clear();
                    rows = DSQL.Query(query, ps);
                    rows.ForEach(p => publicUser.Rights.Add(p["Action"].GetString(), true));
                }
                else
                {
                    if (publicUser.ViewMenus.Contains(menuId))
                    {
                        publicUser.Rights.Clear();
                        query = "SELECT * FROM VW_Admin_User_Menu_Right WHERE UserId=@UserId AND MenuId=@MenuId";
                        rows = DSQL.Query(query, ps);
                        rows.ForEach(p => publicUser.Rights.Add(p["Action"].GetString(), true));
                    }
                    if (publicUser.PubMenus.Contains(menuId))
                    {
                        publicUser.Rights.Clear();
                        query = "SELECT Action FROM Admin_Right WHERE deleted=0";
                        ps.Clear();
                        rows = DSQL.Query(query, ps);
                        rows.ForEach(p => publicUser.Rights.Add(p["Action"].GetString(), true));
                    }
                }
                #endregion
            }
            else
            {
                AdminPubMenus AdminPubMenu = new AdminPubMenus();
                publicUser.PubMenus.AddRange(AdminPubMenu.GetPublicMenus());
                if (publicUser.PubMenus.Contains(menuId))
                {
                    publicUser.Rights.Add("view", true);
                    publicUser.Rights.Add("detail", true);
                    publicUser.Rights.Add("save", true);
                    publicUser.Rights.Add("add", true);
                    publicUser.Rights.Add("delete", true);
                    publicUser.Rights.Add("print", true);
                    publicUser.Rights.Add("output", true);
                    publicUser.Rights.Add("email", true);
                }
            }

            return publicUser;
        }

    }
}
