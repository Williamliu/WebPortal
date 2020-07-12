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

            InitPubMenus(this.DB.DSQL, menuId);

            //Step5: Init Database Schema defined in Controller
            this.InitDatabase(menuId);
        }
        private void InitPubMenus(SqlHelper dsql, string menuId)
        {
            SqlHelper DSQL = dsql;
            string query0 = $"SELECT Id, ParentId, MenuId, Position, Url, MenuImage, Sort, {DSQL.LangSmartColumn("Title")} AS Title, {DSQL.LangSmartColumn("Detail")} AS Detail FROM Pub_Menu WHERE Position=1 AND Active=1 AND Deleted=0 AND ParentId=0 ORDER BY Sort DESC";
            GTable parent = DSQL.ExecuteTable(query0, new Dictionary<string, object>());

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
                pmenu.MenuImage = srow.GetValue("MenuImage");
                pmenu.Sort = srow.GetValue("Sort");
                pmenu.Title = srow.GetValue("Title");
                pmenu.Detail = srow.GetValue("Detail");
                // Public Menu don't need view right
                //if (this.DB.User.ViewMenus.Contains(pmenu.MenuId) == false) continue;

                string query1 = $"SELECT Id, ParentId, MenuId, Url, Position, MenuImage, Sort, {DSQL.LangSmartColumn("Title")} AS Title, {DSQL.LangSmartColumn("Detail")} AS Detail FROM Pub_Menu WHERE Active=1 AND Deleted=0 AND ParentId={pmenu.Id} ORDER BY Sort DESC";
                GTable child = DSQL.ExecuteTable(query1, new Dictionary<string, object>());
                foreach (GRow srow1 in child.Rows)
                {
                    Menu cmenu = new Menu();
                    cmenu.Id = srow1.GetValue("Id").GetInt() ?? 0;
                    cmenu.ParentId = srow1.GetValue("ParentId");
                    cmenu.MenuId = srow1.GetValue("MenuId");
                    cmenu.Url = srow1.GetValue("Url");
                    cmenu.Position = srow1.GetValue("Position");
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

            query0 = $"SELECT Id, ParentId, MenuId, Url, Position, MenuImage, Sort, {DSQL.LangSmartColumn("Title")} AS Title, {DSQL.LangSmartColumn("Detail")} AS Detail FROM Pub_Menu WHERE Position=2 AND Active=1 AND Deleted=0 AND ParentId=0 ORDER BY Sort DESC";
            parent = DSQL.ExecuteTable(query0, new Dictionary<string, object>());
            foreach (GRow srow in parent.Rows)
            {
                Menu pmenu = new Menu();
                pmenu.Id = srow.GetValue("Id").GetInt() ?? 0;
                pmenu.ParentId = srow.GetValue("ParentId");
                pmenu.MenuId = srow.GetValue("MenuId");
                pmenu.Url = srow.GetValue("Url");
                pmenu.Position = srow.GetValue("Position");
                pmenu.MenuImage = srow.GetValue("MenuImage");
                pmenu.Sort = srow.GetValue("Sort");
                pmenu.Title = srow.GetValue("Title");
                pmenu.Detail = srow.GetValue("Detail");
                //Positon = 1 Menu,  used for My Account
                if (this.DB.User.ViewMenus.Contains(pmenu.MenuId) == false) continue;

                string query1 = $"SELECT Id, ParentId, MenuId, Url, Position, MenuImage, Sort, {DSQL.LangSmartColumn("Title")} AS Title, {DSQL.LangSmartColumn("Detail")} AS Detail FROM Pub_Menu WHERE Active=1 AND Deleted=0 AND ParentId={pmenu.Id} ORDER BY Sort DESC";
                GTable child = DSQL.ExecuteTable(query1, new Dictionary<string, object>());
                foreach (GRow srow1 in child.Rows)
                {
                    Menu cmenu = new Menu();
                    cmenu.Id = srow1.GetValue("Id").GetInt() ?? 0;
                    cmenu.ParentId = srow1.GetValue("ParentId");
                    cmenu.MenuId = srow1.GetValue("MenuId");
                    cmenu.Url = srow1.GetValue("Url");
                    cmenu.Position = srow1.GetValue("Position");
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
        #endregion
    }
}
