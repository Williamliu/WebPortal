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
        [HttpGet("InitWebContent")]
        public IActionResult InitWebContent()
        {
            this.Init("M7020");
            this.DB.FillAll();
            return Ok(this.DB);
        }
        [HttpPost("ReloadWebContent")]
        public IActionResult ReloadWebContent(JSTable jsTable)
        {
            this.Init("M7020");
            return Ok(this.DB.ReloadTable(jsTable));
        }
        [HttpPost("SaveWebContent")]
        public IActionResult SaveCourseDetail(JSTable jsTable)
        {
            this.Init("M7020");
            return Ok(this.DB.SaveTable(jsTable));
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
                        Table table = new Table("WebContent", "Pub_WebContent", Words("Web.Content"));
                        Meta id = new Meta { Name = "Id", DbName = "Id", Title = "ID", IsKey = true };
                        Meta menu = new Meta { Name = "MenuId", DbName = "MenuId", Title = Words("pub.menu"), Type = EInput.String, Required = true,  MaxLength=16, Order = "ASC" };
                        menu.AddListRef("PubMenuList");
                        Meta place = new Meta { Name = "Place", DbName = "Place", Title = Words("col.position"), Order = "ASC", Required = true, Type = EInput.String, MaxLength = 16 };
                        Meta titleEN = new Meta { Name = "Title_en", DbName = "Title_en", Title = Words("title.en"), Order = "ASC", Required = true, Type = EInput.String, MaxLength = 64 };
                        Meta titleCN = new Meta { Name = "Title_cn", DbName = "Title_cn", Title = Words("title.cn"), Order = "ASC", Required = true, Type = EInput.String, MaxLength = 64 };
                        Meta detailEN = new Meta { Name = "Detail_en", DbName = "Detail_en", Title = Words("detail.en"), Order = "ASC", Type = EInput.String, MaxLength = 500 * 1024 };
                        Meta detailCN = new Meta { Name = "Detail_cn", DbName = "Detail_cn", Title = Words("detail.cn"), Order = "ASC", Type = EInput.String, MaxLength = 500 * 1024 };
                        Meta active = new Meta { Name = "Active", DbName = "Active", Title = Words("status.active"), Description = Words("status.active.inactive"), Type = EInput.Bool };
                        Meta sort = new Meta { Name = "Sort", DbName = "Sort", Title = Words("col.sort"), Type = EInput.Int, Order = "DESC" };
                
                        table.AddMetas(id, menu, place, titleEN, titleCN, detailEN, detailCN, active, sort);

                        CollectionTable c1 = new CollectionTable("PubMenuList", "VW_Pub_Menu_List", true, "MenuId", "Title", "Detail", "", "DESC", "Sort");
                        Collection PubMenuList = new Collection(ECollectionType.Table, c1);

                        Filter f1 = new Filter() { Name = "search_keyword", DbName = "Title_en,Title_cn", Title = Words("col.keyword"), Type = EFilter.String, Compare = ECompare.Like };
                        Filter f2 = new Filter() { Name = "search_menu", DbName = "MenuId", Title = Words("col.menu"), Type = EFilter.String, Compare = ECompare.Equal };
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
            }
        }

    }
}