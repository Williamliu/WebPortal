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
                        Meta pos_f = new Meta { Name = "Position", DbName = "Position", Title = Words("col.position"), Type = EInput.Int, Required=true, Value = 1 };
                        pos_f.AddListRef("MenuTypeList");
                        Meta sort_f = new Meta { Name = "Sort", DbName = "Sort", Title = Words("col.sort"), Type = EInput.Int };
                        Meta active_f = new Meta { Name = "Active", DbName = "Active", Title = Words("col.status"), Description = Words("status.active.inactive"), Type = EInput.Bool };

                        firstMenu.AddMetas(id_f, pid_f, menuId_f, title_en_f, title_cn_f, detail_en_f, detail_cn_f, url_f, pos_f, sort_f, active_f);
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
                        Meta pos_s = new Meta { Name = "Position", DbName = "Position", Title = Words("col.position"), Type = EInput.Int, Value = 1 };
                        pos_s.AddListRef("MenuTypeList");
                        Meta sort_s = new Meta { Name = "Sort", DbName = "Sort", Title = Words("col.sort"), Type = EInput.Int };
                        Meta active_s = new Meta { Name = "Active", DbName = "Active", Title = Words("col.status"), Description = Words("status.active.inactive"), Type = EInput.Bool };

                        secondMenu.AddMetas(id_s, pid_s, menuId_s, title_en_s, title_cn_s, detail_en_s, detail_cn_s, url_s, pos_s, sort_s, active_s);
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
            }
        }

    }
}