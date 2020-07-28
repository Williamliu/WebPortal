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
        public static IHtmlContent WebContent<t>(this Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<t> htmlHelper, string menuId)
        {
            WebContent webContent = htmlHelper.ViewContext.HttpContext.Items["WebContent"] as WebContent;
            if(webContent!=null)
                return htmlHelper.Raw(webContent.HTML(menuId));
            else 
                return htmlHelper.Raw(string.Empty);
        }

    }

    public static class WebStartUp
    {
        public static void SaveSession(this HttpContext httpContext, string sessName, string value)
        {
            if(string.IsNullOrWhiteSpace(value))
            {
                byte[] valueByte = System.Text.ASCIIEncoding.ASCII.GetBytes(string.Empty);
                httpContext.Session.Set(sessName, valueByte);
                httpContext.Session.Remove(sessName);
                // we can not delete request cookies which from user request
                // we can delete response cookie,  but delete response cookies will effect for next request.
                httpContext.Response.Cookies.Delete(sessName);
            }
            else
            {
                byte[] valueByte = System.Text.ASCIIEncoding.ASCII.GetBytes(value);
                httpContext.Session.Set(sessName, valueByte);

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
            else if (string.IsNullOrWhiteSpace(httpContext.Request.Cookies[sessName] ??"") == false)
            {
                return httpContext.Request.Cookies[sessName] ?? "";
            }
            return string.Empty;
        }
        public static void DeleteSession(this HttpContext httpContext, string sessName)
        {
            httpContext.Response.Cookies.Delete(sessName);
            httpContext.Session.SetString(sessName, "");
            httpContext.Session.Remove(sessName);
        }

        public static string GetWebLang(this HttpContext httpContext)
        {
            string queryLang = "";
            if (httpContext.Request.QueryString.HasValue)
            {
                queryLang = httpContext.Request.Query.ContainsKey("lang")?httpContext.Request.Query["lang"][0].GetString().ToLower():"";
                // save post lang to cookie
                if (!string.IsNullOrWhiteSpace(queryLang)) httpContext.SaveSession("WebPortalLang", queryLang);
                
            }

            //Priority 2: Get From Session & Cookie
            if (string.IsNullOrWhiteSpace(queryLang))
            {
                queryLang = httpContext.GetSession("WebPortalLang");
            }

            // Priority 3: using Browser Settings
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
        public static WebUser GetAdminUser(this HttpContext httpContext, SqlHelper DSQL, string menuId)
        {
            WebUser adminUser = new WebUser();
            string guid = string.Empty;
            guid = httpContext.Request.Headers["SiteSession"].GetString();
            if (string.IsNullOrWhiteSpace(guid)) guid = httpContext.GetSession("adminSite_Session");

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

                #region PrivateMenu and PublicMenu
                query = "SELECT * FROM VW_Admin_User_Menu_View WHERE Id=@UserId";
                rows = DSQL.Query(query, ps);
                adminUser.PrivateMenuIDs.AddRange(rows.Select(p => p["Menu"].GetString()).ToList<string>());

                AdminSharedMenus AdminPubMenu = new AdminSharedMenus();
                adminUser.PublicMenuIDs.AddRange(AdminPubMenu.GetSharedMenus());
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
                    if (adminUser.PrivateMenuIDs.Contains(menuId))
                    {
                        // if Private Menu, base on User Role
                        adminUser.Rights.Clear();
                        query = "SELECT * FROM VW_Admin_User_Menu_Right WHERE UserId=@UserId AND MenuId=@MenuId";
                        rows = DSQL.Query(query, ps);
                        rows.ForEach(p => adminUser.Rights.Add(p["Action"].GetString(), true));
                    }
                    if (adminUser.PublicMenuIDs.Contains(menuId))
                    {
                        // if Public menu, full right
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
                AdminSharedMenus AdminPubMenu = new AdminSharedMenus();
                adminUser.PublicMenuIDs.AddRange(AdminPubMenu.GetSharedMenus());
                if (adminUser.PublicMenuIDs.Contains(menuId))
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
        public static WebUser GetPubUser(this HttpContext httpContext, SqlHelper DSQL, string menuId)
        {
            WebUser publicUser = new WebUser();
            string guid = string.Empty;
            guid = httpContext.Request.Headers["SiteSession"].GetString();
            if (string.IsNullOrWhiteSpace(guid)) guid = httpContext.GetSession("pubSite_Session");

            if (string.IsNullOrWhiteSpace(guid) == false)
            {
                #region Get User by Session
                string query = "select top 1 * from VW_Pub_Session_User WHERE Session=@Session";
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
                    publicUser.IsAdmin = false;
                }
                #endregion

                #region ViewMenu and PubMenu
                query = "SELECT MenuId FROM VW_Pub_User_Menu_Private WHERE UserId=@UserId";
                ps.Add("UserId", publicUser.Id);
                rows = DSQL.Query(query, ps);
                publicUser.PrivateMenuIDs.AddRange(rows.Select(p => p["MenuId"].GetString()).ToList<string>());

                AdminSharedMenus AdminPubMenu = new AdminSharedMenus();
                publicUser.PublicMenuIDs.AddRange(AdminPubMenu.GetSharedMenus());

                query = "SELECT MenuId FROM VW_Pub_User_Menu_Public";
                rows = DSQL.Query(query, ps);
                publicUser.PublicMenuIDs.AddRange(rows.Select(p => p["MenuId"].GetString()).ToList<string>());
                #endregion

                #region Pub User Full Rights
                publicUser.Rights.Clear();
                query = "SELECT Action FROM Admin_Right WHERE deleted=0";
                ps.Clear();
                rows = DSQL.Query(query, ps);
                rows.ForEach(p => publicUser.Rights.Add(p["Action"].GetString(), true));
                #endregion

                #region PubUser Image
                query = $"SELECT Id FROM GGallery WHERE Deleted=0 AND Active=1 AND GalleryName='PubUser'";
                int galleryId = DSQL.ExecuteScalar(query, new SqlParameter[0]);

                query = "SELECT TOP 1 Id, Guid FROM GImage WHERE Deleted=0 AND Active=1 AND GalleryId=@GalleryId AND RefKey=@RefKey ORDER BY Main DESC, Sort DESC, CreatedTime DESC";
                ps.Clear();
                ps.Add("GalleryId", galleryId);
                ps.Add("RefKey", publicUser.Id);
                IList<Dictionary<string, string>> imageRows = DSQL.Query(query, ps);
                if(imageRows.Count>0) publicUser.ImageUrl = $"/api/Image/GetImage/{imageRows[0]["Guid"].GetString()}";
                #endregion
            }
            else
            {
                #region Pub User Full Rights
                publicUser.Rights.Clear();
                string query = "SELECT Action FROM Admin_Right WHERE deleted=0";
                Dictionary<string, object> ps = new Dictionary<string, object>();
                ps.Clear();
                List<Dictionary<string, string>> rows = DSQL.Query(query, ps);
                rows.ForEach(p => publicUser.Rights.Add(p["Action"].GetString(), true));
                #endregion
            }
            httpContext.Items.Add("PubUser", publicUser);
            return publicUser;
        }

    }
}
