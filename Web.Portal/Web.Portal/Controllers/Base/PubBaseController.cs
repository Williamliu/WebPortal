using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Web.Portal.Common;
using Library.V1.Common;
using Library.V1.Entity;
using Library.V1.SQL;

namespace Web.Portal.Controllers
{
    public abstract class PubBaseController : Controller
    {
        #region Protected Fields
        protected AppSetting AppConfig { get; set; }
        protected Database DB;
        #endregion

        public PubBaseController(AppSetting appConfig)
        {
            this.AppConfig = appConfig;
        }

        #region Methods
        protected abstract void InitDatabase(string menuId);
        protected void Init(string menuId)
        {
            //Step1: get language from queryString/Session/Cookie/Browser
            this.AppConfig.Lang = this.HttpContext.GetWebLang();

            //Step2: Init Global Database Context
            this.DB = new Database(this.AppConfig);

            //Step3: Init language dictionary
            LanguageHelper.InitWords(this.DB.DSQL);

            //Step4: Get AdminUser for DB User.
            this.DB.User = this.HttpContext.GetPubUser(this.DB.DSQL, menuId);

            InitPubMenus(menuId);

            WriteWebLog(menuId);
            //Step5: Init Database Schema defined in Controller
            this.InitDatabase(menuId);
        }
        private void InitPubMenus(string menuId)
        {
            string query0 = $"SELECT Id, ParentId, MenuId, Position, Indent, Url, MenuImage, Sort, {this.DB.DSQL.LangSmartColumn("Title")} AS Title, {this.DB.DSQL.LangSmartColumn("Detail")} AS Detail FROM Pub_Menu WHERE Position=1 AND Active=1 AND Deleted=0 AND ParentId=0 ORDER BY Sort DESC";
            GTable parent = this.DB.DSQL.ExecuteTable(query0, new Dictionary<string, object>());

            PubPubMenus pubPubMenus = new PubPubMenus();
            Menus pubMenu = new Menus(menuId, pubPubMenus);

            foreach (GRow srow in parent.Rows)
            {
                Menu pmenu = new Menu();
                pmenu.Id = srow.GetValue("Id").GetInt() ?? 0;
                pmenu.ParentId = srow.GetValue("ParentId");
                pmenu.MenuId = srow.GetValue("MenuId");
                pmenu.Url = srow.GetValue("Url");
                pmenu.Position = srow.GetValue("Position");
                pmenu.Indent = srow.GetValue("Indent");
                pmenu.MenuImage = srow.GetValue("MenuImage");
                pmenu.Sort = srow.GetValue("Sort");
                pmenu.Title = srow.GetValue("Title");
                pmenu.Detail = srow.GetValue("Detail");
                // Public Menu don't need view right
                //if (this.DB.User.ViewMenus.Contains(pmenu.MenuId) == false) continue;

                string query1 = $"SELECT Id, ParentId, MenuId, Url, Position, Indent, MenuImage, Sort, {this.DB.DSQL.LangSmartColumn("Title")} AS Title, {this.DB.DSQL.LangSmartColumn("Detail")} AS Detail FROM Pub_Menu WHERE Active=1 AND Deleted=0 AND ParentId={pmenu.Id} ORDER BY Sort DESC";
                GTable child = this.DB.DSQL.ExecuteTable(query1, new Dictionary<string, object>());
                foreach (GRow srow1 in child.Rows)
                {
                    Menu cmenu = new Menu();
                    cmenu.Id = srow1.GetValue("Id").GetInt() ?? 0;
                    cmenu.ParentId = srow1.GetValue("ParentId");
                    cmenu.MenuId = srow1.GetValue("MenuId");
                    cmenu.Url = srow1.GetValue("Url");
                    cmenu.Position = srow1.GetValue("Position");
                    cmenu.Indent = srow1.GetValue("Indent");
                    cmenu.MenuImage = srow1.GetValue("MenuImage");
                    cmenu.Sort = srow1.GetValue("Sort");
                    cmenu.Title = srow1.GetValue("Title");
                    cmenu.Detail = srow1.GetValue("Detail");
                    //Public Menu don't need view right
                    //if (this.DB.User.ViewMenus.Contains(cmenu.MenuId) == false) continue;
                    if (cmenu.MenuId.ToLower() == pubMenu.MenuId.ToLower()) pubMenu.MenuTitle = cmenu.Title;
                    pmenu.Nodes.Add(cmenu);
                }
                if (pmenu.MenuId.ToLower() == pubMenu.MenuId.ToLower()) pubMenu.MenuTitle = pmenu.Title;
                pubMenu.AddTop(pmenu);
            }

            query0 = $"SELECT Id, ParentId, MenuId, Url, Position, Indent, MenuImage, Sort, {this.DB.DSQL.LangSmartColumn("Title")} AS Title, {this.DB.DSQL.LangSmartColumn("Detail")} AS Detail FROM Pub_Menu WHERE Position=2 AND Active=1 AND Deleted=0 AND ParentId=0 ORDER BY Sort DESC";
            parent = this.DB.DSQL.ExecuteTable(query0, new Dictionary<string, object>());
            foreach (GRow srow in parent.Rows)
            {
                Menu pmenu = new Menu();
                pmenu.Id = srow.GetValue("Id").GetInt() ?? 0;
                pmenu.ParentId = srow.GetValue("ParentId");
                pmenu.MenuId = srow.GetValue("MenuId");
                pmenu.Url = srow.GetValue("Url");
                pmenu.Position = srow.GetValue("Position");
                pmenu.Indent = srow.GetValue("Indent");
                pmenu.MenuImage = srow.GetValue("MenuImage");
                pmenu.Sort = srow.GetValue("Sort");
                pmenu.Title = srow.GetValue("Title");
                pmenu.Detail = srow.GetValue("Detail");
                //Positon = 1 Menu,  used for My Account
                if (this.DB.User.ViewMenus.Contains(pmenu.MenuId) == false) continue;

                string query1 = $"SELECT Id, ParentId, MenuId, Url, Position, Indent, MenuImage, Sort, {this.DB.DSQL.LangSmartColumn("Title")} AS Title, {this.DB.DSQL.LangSmartColumn("Detail")} AS Detail FROM Pub_Menu WHERE Active=1 AND Deleted=0 AND ParentId={pmenu.Id} ORDER BY Sort DESC";
                GTable child = this.DB.DSQL.ExecuteTable(query1, new Dictionary<string, object>());
                foreach (GRow srow1 in child.Rows)
                {
                    Menu cmenu = new Menu();
                    cmenu.Id = srow1.GetValue("Id").GetInt() ?? 0;
                    cmenu.ParentId = srow1.GetValue("ParentId");
                    cmenu.MenuId = srow1.GetValue("MenuId");
                    cmenu.Url = srow1.GetValue("Url");
                    cmenu.Position = srow1.GetValue("Position");
                    cmenu.Indent = srow1.GetValue("Indent");
                    cmenu.MenuImage = srow1.GetValue("MenuImage");
                    cmenu.Sort = srow1.GetValue("Sort");
                    cmenu.Title = srow1.GetValue("Title");
                    cmenu.Detail = srow1.GetValue("Detail");
                    //Positon = 1 Menu,  used for My Account
                    if (this.DB.User.ViewMenus.Contains(cmenu.MenuId) == false) continue;
                    if (cmenu.MenuId.ToLower() == pubMenu.MenuId.ToLower()) pubMenu.MenuTitle = cmenu.Title;
                    pmenu.Nodes.Add(cmenu);
                }
                if (pmenu.MenuId.ToLower() == pubMenu.MenuId.ToLower()) pubMenu.MenuTitle = pmenu.Title;
                pubMenu.AddBottom(pmenu);
            }

            foreach (Menu pmenu in pubMenu.ProfileMenus)
            {
                if (pmenu.MenuId.ToLower() == pubMenu.MenuId.ToLower()) pubMenu.MenuTitle = LanguageHelper.Words(pmenu.Title);
            }
            foreach (Menu hmenu in pubMenu.HideMenus)
            {
                if (hmenu.MenuId.ToLower() == pubMenu.MenuId.ToLower()) pubMenu.MenuTitle = LanguageHelper.Words(hmenu.Title);
            }

            this.HttpContext.Items.Add("PubMenus", pubMenu);
        }

        protected void GetWebContent(string menuId)
        {
            WebContent webcon = new WebContent(this.DB.DSQL, menuId);
            webcon.FillData();
            this.HttpContext.Items.Add("WebContent", webcon);
        }
        private void WriteWebLog(string menuId)
        {
            SQLRow row = new SQLRow();
            row.Add("MenuId",       "MenuId",       menuId);
            row.Add("Url",          "Url",          $"{this.HttpContext.Request.Path.ToString()}{this.HttpContext.Request.QueryString.ToString()}");
            row.Add("UserAgent",    "UserAgent",    this.HttpContext.Request.Headers["User-Agent"].ToString());
            row.Add("PubUserId",    "PubUserId",    0);
            row.Add("IPAddress",    "IPAddress",    this.HttpContext.Connection.RemoteIpAddress.ToString());
            row.Add("Lang",         "Lang",         this.DB.DSQL.Lang);
            row.Add("UserLang",     "UserLang",     this.HttpContext.Request.Headers["Accept-Language"].ToString());
            this.DB.DSQL.InsertTable("Pub_WebAccessLog", row);
        }
        #endregion
    }
}
