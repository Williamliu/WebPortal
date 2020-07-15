using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Library.V1.Common;
using Library.V1.Entity;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Web.Portal.Areas.Admin.WebApi
{
    [Route("/Admin/api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PubWebController : AdminBaseController
    {
        public PubWebController(AppSetting appSetting) : base(appSetting) { }
        [HttpGet("InitWebMenu")]
        public IActionResult InitWebMenu()
        {
            this.Init("M7010");
            this.DB.FillAll();
            if (this.DB.Error.HasError)
                return BadRequest(this.DB);
            else
                return Ok(this.DB);
        }
        [HttpPost("SaveWebMenu")]
        public IActionResult SaveAdminMenu(JSTable jsTable)
        {
            this.Init("M7010");
            return Ok(this.DB.SaveTable(jsTable));
        }
        [HttpGet("InitUserRole")]
        public IActionResult InitAdminRole()
        {
            this.Init("M7020");
            this.DB.FillAll();
            return Ok(this.DB);
        }

        [HttpPost("SaveUserRole")]
        public IActionResult SaveUserRole(JSTable jsTable)
        {
            this.Init("M7020");
            return Ok(this.DB.SaveTable(jsTable));
        }
        [HttpPost("ReloadUserRole")]
        public IActionResult ReloadUserRole(JSTable jsTable)
        {
            this.Init("M7020");
            return Ok(this.DB.ReloadTable(jsTable));
        }


        [HttpGet("InitWebContent")]
        public IActionResult InitWebContent()
        {
            this.Init("M7050");
            this.DB.FillAll();
            return Ok(this.DB);
        }
        [HttpPost("ReloadWebContent")]
        public IActionResult ReloadWebContent(JSTable jsTable)
        {
            this.Init("M7050");
            return Ok(this.DB.ReloadTable(jsTable));
        }
        [HttpPost("SaveWebContent")]
        public IActionResult SaveCourseDetail(JSTable jsTable)
        {
            this.Init("M7050");
            return Ok(this.DB.SaveTable(jsTable));
        }
        [HttpGet("InitAccessLog")]
        public IActionResult InitAccessLog()
        {
            this.Init("M7080");
            this.DB.FillAll();
            if (this.DB.Error.HasError)
                return BadRequest(this.DB);
            else
                return Ok(this.DB);
        }
        [HttpPost("ReloadAccessLog")]
        public IActionResult ReloadAccessLog(JSTable jsTable)
        {
            this.Init("M7080");
            return Ok(this.DB.ReloadTable(jsTable));
        }
        protected override void InitDatabase(string menuId)
        {
            switch (menuId)
            {
                case "M7010":
                    {
                        Table firstMenu = new Table("FirstMenu", "Pub_Menu", Words("first.menu"));
                        Meta id_f = new Meta { Name = "Id", DbName = "Id", Title = Words("col.id"), IsKey = true };
                        Meta pid_f = new Meta { Name = "ParentId", DbName = "ParentId", Title = Words("col.parentid"), Required = true, Type = EInput.Int };
                        Meta menuId_f = new Meta { Name = "MenuId", DbName = "MenuId", Title = Words("col.menuid"), Required = true, Type = EInput.String, MaxLength = 16 };
                        Meta title_en_f = new Meta { Name = "Title_en", DbName = "Title_en", Title = Words("title.en"), Required = true, Type = EInput.String, MaxLength = 64 };
                        Meta title_cn_f = new Meta { Name = "Title_cn", DbName = "Title_cn", Title = Words("title.cn"), Required = true, Type = EInput.String, MaxLength = 64 };
                        Meta detail_en_f = new Meta { Name = "Detail_en", DbName = "Detail_en", Title = Words("detail.en"), Type = EInput.String, MaxLength = 256 };
                        Meta detail_cn_f = new Meta { Name = "Detail_cn", DbName = "Detail_cn", Title = Words("detail.cn"), Type = EInput.String, MaxLength = 256 };
                        Meta url_f = new Meta { Name = "Url", DbName = "Url", Title = Words("col.url"), Type = EInput.String, MaxLength = 1024 };
                        Meta indent_f = new Meta { Name = "Indent", DbName = "Indent", Title = Words("col.indent"), Order = "ASC", Type = EInput.String, MaxLength = 8 };
                        Meta pos_f = new Meta { Name = "Position", DbName = "Position", Title = Words("col.position"), Type = EInput.Int, Required=true, Value = 1 };
                        pos_f.AddListRef("MenuTypeList");
                        Meta sort_f = new Meta { Name = "Sort", DbName = "Sort", Title = Words("col.sort"), Type = EInput.Int };
                        Meta active_f = new Meta { Name = "Active", DbName = "Active", Title = Words("col.status"), Description = Words("status.active.inactive"), Type = EInput.Bool };

                        firstMenu.AddMetas(id_f, pid_f, menuId_f, title_en_f, title_cn_f, detail_en_f, detail_cn_f, url_f, indent_f, pos_f, sort_f, active_f);
                        Filter f1_f = new Filter() { Name = "ParentId", DbName = "ParentId", Title = Words("col.parentid"), Type = EFilter.Hidden, Compare = ECompare.Equal, Value1 = 0 };
                        firstMenu.AddFilters(f1_f);

                        firstMenu.Navi.IsActive = false;
                        firstMenu.Navi.By = "Position,Sort";
                        firstMenu.Navi.Order = "ASC,DESC";
                        firstMenu.AddQueryKV("Deleted", false).AddDeleteKV("LastUpdated", DateTime.Now.UTCSeconds())
                            .AddUpdateKV("LastUpdated", DateTime.Now.UTCSeconds())
                            .AddInsertKV("Deleted", false).AddInsertKV("CreatedTime", DateTime.Now.UTCSeconds());
                        firstMenu.SaveUrl = "/Admin/api/PubWeb/SaveWebMenu";

                        Table secondMenu = new Table("SecondMenu", "Pub_Menu", Words("second.menu"));
                        Meta id_s = new Meta { Name = "Id", DbName = "Id", Title = Words("col.id"), IsKey = true };
                        Meta pid_s = new Meta { Name = "ParentId", DbName = "ParentId", Title = Words("col.parentid"), Required = true, Type = EInput.Int };
                        Meta menuId_s = new Meta { Name = "MenuId", DbName = "MenuId", Title = Words("col.menuid"), Required = true, Type = EInput.String, MaxLength = 16 };
                        Meta title_en_s = new Meta { Name = "Title_en", DbName = "Title_en", Title = Words("title.en"), Required = true, Type = EInput.String, MaxLength = 64 };
                        Meta title_cn_s = new Meta { Name = "Title_cn", DbName = "Title_cn", Title = Words("title.cn"), Required = true, Type = EInput.String, MaxLength = 64 };
                        Meta detail_en_s = new Meta { Name = "Detail_en", DbName = "Detail_en", Title = Words("detail.en"), Type = EInput.String, MaxLength = 256 };
                        Meta detail_cn_s = new Meta { Name = "Detail_cn", DbName = "Detail_cn", Title = Words("detail.cn"), Type = EInput.String, MaxLength = 256 };
                        Meta url_s = new Meta { Name = "Url", DbName = "Url", Title = Words("col.url"), Type = EInput.String, MaxLength = 1024 };
                        Meta indent_s = new Meta { Name = "Indent", DbName = "Indent", Title = Words("col.indent"), Order = "ASC", Type = EInput.String, MaxLength = 8 };
                        Meta pos_s = new Meta { Name = "Position", DbName = "Position", Title = Words("col.position"), Type = EInput.Int, Value = 1 };
                        pos_s.AddListRef("MenuTypeList");
                        Meta sort_s = new Meta { Name = "Sort", DbName = "Sort", Title = Words("col.sort"), Type = EInput.Int };
                        Meta active_s = new Meta { Name = "Active", DbName = "Active", Title = Words("col.status"), Description = Words("status.active.inactive"), Type = EInput.Bool };

                        secondMenu.AddMetas(id_s, pid_s, menuId_s, title_en_s, title_cn_s, detail_en_s, detail_cn_s, url_s, indent_s, pos_s, sort_s, active_s);
                        Filter f1_s = new Filter() { Name = "ParentId", DbName = "ParentId", Title = Words("col.parentid"), Type = EFilter.Hidden, Compare = ECompare.NotEqual, Value1 = 0 };
                        secondMenu.AddFilters(f1_s);

                        secondMenu.Navi.IsActive = false;
                        secondMenu.Navi.By = "Position,Sort";
                        secondMenu.Navi.Order = "ASC,DESC";
                        secondMenu.AddQueryKV("Deleted", false).AddDeleteKV("LastUpdated", DateTime.Now.UTCSeconds())
                            .AddUpdateKV("LastUpdated", DateTime.Now.UTCSeconds())
                            .AddInsertKV("Deleted", false).AddInsertKV("Position", 1).AddInsertKV("CreatedTime", DateTime.Now.UTCSeconds());
                        
                        secondMenu.SaveUrl = "/Admin/api/PubWeb/SaveWebMenu";

                        Collection MenuTypeList = new Collection("MenuTypeList");
                        this.DB.AddTables(firstMenu, secondMenu).AddCollections(MenuTypeList);
                    }
                    break;
                case "M7020":
                    {
                        Table firstMenu = new Table("FirstMenu", "VW_Pub_User_FirstMenu", Words("first.menu"));
                        Meta id_f = new Meta { Name = "Id", DbName = "Id", Title = Words("col.id"), IsKey = true };
                        Meta pid_f = new Meta { Name = "ParentId", DbName = "ParentId", Title = Words("col.parentid"), Type = EInput.Int };
                        Meta title_f = new Meta { Name = "Title", DbName = "Title", Title = Words("col.title"), IsLang = true, Type = EInput.String, MaxLength = 64 };
                        Meta detail_f = new Meta { Name = "Detail", DbName = "Detail", Title = Words("col.detail"), IsLang = true, Type = EInput.String, MaxLength = 256 };
                        firstMenu.AddMetas(id_f, pid_f, title_f, detail_f);

                        firstMenu.Navi.IsActive = false;
                        firstMenu.Navi.By = "Sort";
                        firstMenu.Navi.Order = "DESC";

                        Table secondMenu = new Table("SecondMenu", "VW_Pub_User_SecondMenu", Words("second.menu"));
                        Meta id_s = new Meta { Name = "Id", DbName = "Id", Title = Words("col.id"), IsKey = true };
                        Meta pid_s = new Meta { Name = "ParentId", DbName = "ParentId", Title = Words("col.parentid"), Required = true, Type = EInput.Int };
                        Meta title_s = new Meta { Name = "Title", DbName = "Title", Title = Words("title.en"), IsLang = true, Type = EInput.String, MaxLength = 64 };
                        Meta detail_s = new Meta { Name = "Detail", DbName = "Detail", Title = Words("detail.en"), IsLang = true, Type = EInput.String, MaxLength = 256 };

                        secondMenu.AddMetas(id_s, pid_s, title_s, detail_s);

                        secondMenu.Navi.IsActive = false;
                        secondMenu.Navi.By = "Sort";
                        secondMenu.Navi.Order = "DESC";


                        Table UserRole = new Table("UserRole", "Pub_Role", Words("user.role"));
                        Meta id = new Meta { Name = "Id", DbName = "Id", Title = "ID", IsKey = true };
                        Meta titleEN = new Meta { Name = "TitleEN", DbName = "Title_en", Title = Words("title.en"), Order = "ASC", Required = true, Type = EInput.String, MaxLength = 64 };
                        Meta titleCN = new Meta { Name = "TitleCN", DbName = "Title_cn", Title = Words("title.cn"), Order = "ASC", Required = true, Type = EInput.String, MaxLength = 64 };
                        Meta detailEN = new Meta { Name = "DetailEN", DbName = "Detail_en", Title = Words("detail.en"), Order = "ASC", Type = EInput.String, MaxLength = 256 };
                        Meta detailCN = new Meta { Name = "DetailCN", DbName = "Detail_cn", Title = Words("detail.cn"), Order = "ASC", Type = EInput.String, MaxLength = 256 };
                        Meta active = new Meta { Name = "Active", DbName = "Active", Title = Words("status.active"), Description = Words("status.active.inactive"), Type = EInput.Bool };
                        Meta sort = new Meta { Name = "Sort", DbName = "Sort", Title = Words("col.sort"), Type = EInput.Int, Order = "DESC" };
                        Meta roleMenu = new Meta { Name = "RoleMenu", DbName = "RoleId", Title = Words("col.role"), Description = Words("status.allow.deny"), Type = EInput.Checkbox, Value = new { } };
                        roleMenu.AddListRef("", "Pub_Role_Menu","MenuId");

                        UserRole.AddMetas(id, titleEN, titleCN, detailEN, detailCN, active, sort, roleMenu);
                        Filter f1 = new Filter() { Name = "search_name", DbName = "Title_en,Title_cn", Title = Words("search.name"), Type = EFilter.String, Compare = ECompare.Like };
                        UserRole.AddFilters(f1);
                        UserRole.Navi.IsActive = true;
                        UserRole.Navi.By = "Title_en";

                        UserRole.AddQueryKV("Deleted", false)
                            .AddUpdateKV("LastUpdated", DateTime.Now.UTCSeconds())
                            .AddInsertKV("CreatedTime", DateTime.Now.UTCSeconds())
                            .AddInsertKV("Deleted", false);

                        UserRole.SaveUrl = "/Admin/api/PubWeb/SaveUserRole";
                        UserRole.GetUrl = "/Admin/api/PubWeb/ReloadUserRole";

                        this.DB.AddTables(firstMenu, secondMenu, UserRole);
                    }
                    break;
                case "M7050":
                    {
                        Table table = new Table("WebContent", "Pub_WebContent", Words("Web.Content"));
                        Meta id = new Meta { Name = "Id", DbName = "Id", Title = "ID", IsKey = true };
                        Meta menu = new Meta { Name = "PubMenuId", DbName = "PubMenuId", Title = Words("pub.menu"), Type = EInput.Int, Required = true,  MaxLength=16, Order = "ASC" };
                        menu.AddListRef("PubMenuList");
                        Meta place = new Meta { Name = "Place", DbName = "Place", Title = Words("col.position"), Order = "ASC", Required = true, Type = EInput.String, MaxLength = 16 };
                        Meta titleEN = new Meta { Name = "Title_en", DbName = "Title_en", Title = Words("title.en"), Order = "ASC", Required = true, Type = EInput.String, MaxLength = 64 };
                        Meta titleCN = new Meta { Name = "Title_cn", DbName = "Title_cn", Title = Words("title.cn"), Order = "ASC", Required = true, Type = EInput.String, MaxLength = 64 };
                        Meta detailEN = new Meta { Name = "Detail_en", DbName = "Detail_en", Title = Words("detail.en"), Order = "ASC", Type = EInput.String, MaxLength = 500 * 1024 };
                        Meta detailCN = new Meta { Name = "Detail_cn", DbName = "Detail_cn", Title = Words("detail.cn"), Order = "ASC", Type = EInput.String, MaxLength = 500 * 1024 };
                        Meta active = new Meta { Name = "Active", DbName = "Active", Title = Words("status.active"), Description = Words("status.active.inactive"), Type = EInput.Bool };
                        Meta sort = new Meta { Name = "Sort", DbName = "Sort", Title = Words("col.sort"), Type = EInput.Int, Order = "DESC" };
                
                        table.AddMetas(id, menu, place, titleEN, titleCN, detailEN, detailCN, active, sort);

                        CollectionTable c1 = new CollectionTable("PubMenuList", "VW_Pub_Menu_List", true, "Id", "Title", "Detail", "", "DESC", "Sort");
                        Collection PubMenuList = new Collection(ECollectionType.Table, c1);
                        PubMenuList.AddFilter("MenuId", ECompare.NotEqual, "space");

                        Filter f1 = new Filter() { Name = "search_keyword", DbName = "Title_en,Title_cn", Title = Words("col.keyword"), Type = EFilter.String, Compare = ECompare.Like };
                        Filter f2 = new Filter() { Name = "search_menu", DbName = "PubMenuId", Title = Words("menutype.public"), Type = EFilter.String, Compare = ECompare.Equal };
                        f2.AddListRef("PubMenuList");
                        table.AddFilters(f1, f2);
                        table.Navi.IsActive = true;
                        table.Navi.Order = "DESC";
                        table.Navi.By = "Sort";
                        table.GetUrl = "/Admin/api/PubWeb/ReloadWebContent";
                        table.SaveUrl = "/Admin/api/PubWeb/SaveWebContent";
                        table.AddQueryKV("Deleted", false).AddDeleteKV("LastUpdated", DateTime.Now.UTCSeconds())
                              .AddUpdateKV("LastUpdated", DateTime.Now.UTCSeconds())
                              .AddInsertKV("Deleted", false).AddInsertKV("CreatedTime", DateTime.Now.UTCSeconds());

                        this.DB.AddCollection(PubMenuList).AddTable(table);
                    }
                    break;
                case "M7080":
                    {
                        Table table = new Table("AccessLog", "VW_RPT_AccessLog", Words("access.log"));
                        Meta id = new Meta { Name = "Id", DbName = "Id", Title = "ID", IsKey = true };
                        Meta menuName = new Meta { Name = "MenuName", DbName = "MenuName", Title = Words("menutype.public"), IsLang=true, Order = "ASC", Type = EInput.String, MaxLength = 64 };
                        Meta firstName = new Meta { Name = "FirstName", DbName = "FirstName", Title = Words("col.fullname"), Type = EInput.String, MaxLength = 64, Order = "ASC" };
                        Meta lastName = new Meta { Name = "LastName", DbName = "LastName", Title = Words("col.lastname"), Type = EInput.String, MaxLength = 64, Order = "ASC" };
                        Meta email = new Meta { Name = "Email", DbName = "Email", Title = Words("col.email"), Type = EInput.Email, MaxLength = 256 };
                        Meta url = new Meta { Name = "Url", DbName = "Url", Title = Words("col.url"), Order = "ASC", Type = EInput.String, MaxLength = 256 };
                        Meta userAgent = new Meta { Name = "UserAgent", DbName = "UserAgent", Title = Words("col.user.agent"), Order = "ASC", Type = EInput.String, MaxLength = 256 };
                        Meta ipAddr = new Meta { Name = "IPAddress", DbName = "IPAddress", Title = Words("col.ip.address"), Type = EInput.String, MaxLength=64 };
                        Meta lang = new Meta { Name = "Lang", DbName = "Lang", Title = Words("language"), Type = EInput.String, MaxLength = 64 };
                        Meta userLang = new Meta { Name = "UserLang", DbName = "UserLang", Title = Words("language"), Type = EInput.String, MaxLength = 64 };
                        Meta createdTime = new Meta { Name = "CreatedTime", DbName = "CreatedTime", Title = Words("col.createdtime"), Type = EInput.Read, Order = "DESC" };
                        table.AddMetas(id, menuName, firstName, lastName, email, url, userAgent, ipAddr, lang, userLang, createdTime);

                        Filter f1 = new Filter() { Name = "search_person", DbName = "FirstName,LastName,Email", Title = Words("search.person"), Type = EFilter.String, Compare = ECompare.Like };
                        Filter f2 = new Filter() { Name = "search_menu", DbName = "PubMenuId", Title = Words("menutype.public"), Type = EFilter.String, Compare = ECompare.Equal };
                        f2.AddListRef("PubMenuList");
                        Filter f3 = new Filter() { Name = "search_url", DbName = "Url", Title = Words("col.url"), Type = EFilter.String, Compare = ECompare.Like };
                        Filter f4 = new Filter() { Name = "search_agent", DbName = "userAgent", Title = Words("col.user.agent"), Type = EFilter.String, Compare = ECompare.Like };
                        Filter f5 = new Filter() { Name = "search_lang", DbName = "Lang", Title = Words("language"), Type = EFilter.String, Compare = ECompare.Like };
                        Filter f6 = new Filter() { Name = "search_ipaddr", DbName = "IPAddress", Title = Words("col.ip.address"), Type = EFilter.String, Compare = ECompare.Like };
                        Filter f7 = new Filter() { Name = "search_date",   DbName = "CreatedTime", Title = Words("start.date"), Type = EFilter.Date, Compare = ECompare.Range, Value1=DateTime.Now.AddDays(-30).YMD(), Value2=DateTime.Now.YMD()};

                        table.AddFilters(f1, f2, f3, f4, f5, f6, f7);
                        table.Navi.IsActive = true;
                        table.Navi.Order = "DESC";
                        table.Navi.By = "CreatedTime";
                        table.GetUrl = "/Admin/api/PubWeb/ReloadAccessLog";

                        CollectionTable c1 = new CollectionTable("PubMenuList", "VW_Pub_Menu_List", true, "Id", "Title", "Detail", "", "DESC", "Sort");
                        Collection PubMenuList = new Collection(ECollectionType.Table, c1);
                        PubMenuList.AddFilter("MenuId", ECompare.NotEqual, "space");

                        this.DB.AddTable(table).AddCollection(PubMenuList);
                    }
                    break;
            }
        }

    }
}