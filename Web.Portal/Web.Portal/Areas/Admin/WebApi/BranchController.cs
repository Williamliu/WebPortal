using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Library.V1.Common;
using Library.V1.Entity;
using Library.V1.SQL;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Web.Portal.Areas.Admin.WebApi
{
    [Route("/Admin/api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class BranchController : AdminBaseController
    {
        public BranchController(AppSetting appConfig) : base(appConfig) { }
        [HttpGet("InitBranch")]
        public IActionResult InitBranch()
        {
            this.Init("M4010");
            this.DB.FillAll();
            if (this.DB.Error.HasError)
                return BadRequest(this.DB);
            else
                return Ok(this.DB);
        }
        [HttpPost("ReloadBranch")]
        public IActionResult ReloadBranch(JSTable jsTable)
        {
            this.Init("M4010");
            return Ok(this.DB.ReloadTable(jsTable));
        }

        [HttpPost("SaveBranch")]
        public IActionResult SaveBranch(JSTable jsTable)
        {
            this.Init("M4010");
            return Ok(this.DB.SaveTable(jsTable));
        }

        [HttpGet("InitSite")]
        public IActionResult InitSite()
        {
            this.Init("M4020");
            this.DB.FillAll();
            if (this.DB.Error.HasError)
                return BadRequest(this.DB);
            else
                return Ok(this.DB);
        }
        [HttpPost("ReloadSite")]
        public IActionResult ReloadSite(JSTable jsTable)
        {
            this.Init("M4020");
            return Ok(this.DB.ReloadTable(jsTable));
        }
        [HttpPost("SaveSite")]
        public IActionResult SaveSite(JSTable jsTable)
        {
            this.Init("M4020");
            return Ok(this.DB.SaveTable(jsTable));
        }

        protected override void InitDatabase(string menuId)
        {
            switch (menuId)
            {
                case "M4010":
                    {
                        Table table = new Table("Branch", "GBranch", Words("website.branch"));
                        Meta id = new Meta { Name = "Id", DbName = "Id", Title = "ID", IsKey = true };
                        Meta titleEN = new Meta { Name = "TitleEN", DbName = "Title_en", Title = Words("title.en"), Order = "ASC", Required = true, Type = EInput.String, MaxLength = 64 };
                        Meta titleCN = new Meta { Name = "TitleCN", DbName = "Title_cn", Title = Words("title.cn"), Order = "ASC", Type = EInput.String, MaxLength = 64 };
                        Meta detailEN = new Meta { Name = "DetailEN", DbName = "Detail_en", Title = Words("detail.en"), Order = "ASC", Type = EInput.String, MaxLength = 256 };
                        Meta detailCN = new Meta { Name = "DetailCN", DbName = "Detail_cn", Title = Words("detail.cn"), Order = "ASC", Type = EInput.String, MaxLength = 256 };
                        Meta foundDate = new Meta { Name = "FoundDate", DbName = "FoundDate", Title = Words("col.found.date"), Order = "ASC", Type = EInput.Date };
                        Meta active = new Meta { Name = "Active", DbName = "Active", Title = Words("status.active"), Type = EInput.Bool };
                        Meta sort = new Meta { Name = "Sort", DbName = "Sort", Title = Words("col.sort"), Type = EInput.Int, Order = "DESC" };
                        table.AddMetas(id, titleEN, titleCN, detailEN, detailCN, active, sort, foundDate);

                        Filter f1 = new Filter() { Name = "search_keyword", DbName = "Title_en,Title_cn,Detail_en,Detail_cn", Title = Words("col.keyword"), Type = EFilter.String, Compare = ECompare.Like };
                        Filter f2 = new Filter() { Name = "filterBranches", DbName = "Id", Title = Words("col.branch"), Type = EFilter.Hidden, Required = true, Compare = ECompare.In, Value1 = this.DB.User.Branches };
                        table.AddFilters(f1, f2);
                        table.Navi.IsActive = true;
                        table.Navi.Order = "DESC";
                        table.Navi.By = "Sort";
                        table.GetUrl = "/Admin/api/Branch/ReloadBranch";
                        table.SaveUrl = "/Admin/api/Branch/SaveBranch";
                        table.AddQueryKV("Deleted", false).AddDeleteKV("LastUpdated", DateTime.Now.UTCSeconds())
                            .AddUpdateKV("LastUpdated", DateTime.Now.UTCSeconds()).AddInsertKV("Deleted", false)
                            .AddInsertKV("CreatedTime", DateTime.Now.UTCSeconds());

                        this.DB.AddTable(table);
                    }
                    break;
                case "M4020":
                    {
                        Table table = new Table("Site", "GSite", Words("col.site"));
                        Meta id = new Meta { Name = "Id", DbName = "Id", Title = "ID", IsKey = true };
                        Meta titleEN = new Meta { Name = "TitleEN", DbName = "Title_en", Title = Words("title.en"), Order = "ASC", Required = true, Type = EInput.String, MaxLength = 64 };
                        Meta titleCN = new Meta { Name = "TitleCN", DbName = "Title_cn", Title = Words("title.cn"), Order = "ASC", Type = EInput.String, MaxLength = 64 };
                        Meta detailEN = new Meta { Name = "DetailEN", DbName = "Detail_en", Title = Words("detail.en"), Order = "ASC", Type = EInput.String, MaxLength = 256 };
                        Meta detailCN = new Meta { Name = "DetailCN", DbName = "Detail_cn", Title = Words("detail.cn"), Order = "ASC", Type = EInput.String, MaxLength = 256 };
                        Meta foundDate = new Meta { Name = "FoundDate", DbName = "FoundDate", Title = Words("col.found.date"), Order = "ASC", Type = EInput.Date };
                        Meta active = new Meta { Name = "Active", DbName = "Active", Title = Words("status.active"), Type = EInput.Bool };
                        Meta sort = new Meta { Name = "Sort", DbName = "Sort", Title = Words("col.sort"), Type = EInput.Int, Order = "DESC" };

                        Meta address = new Meta { Name = "Address", DbName = "Address", Title = Words("col.address"), Type = EInput.String, MaxLength = 256 };
                        Meta city = new Meta { Name = "City", DbName = "City", Title = Words("col.city"), Type = EInput.String, MaxLength = 64 };
                        Meta state = new Meta { Name = "State", DbName = "State", Title = Words("col.state"), Type = EInput.Int };
                        state.AddListRef("StateList");
                        Meta country = new Meta { Name = "Country", DbName = "Country", Title = Words("col.country"), Type = EInput.Int };
                        country.AddListRef("CountryList");
                        Meta postal = new Meta { Name = "Postal", DbName = "Postal", Title = Words("col.postal"), Type = EInput.String, MaxLength = 16 };

                        Meta emailEn = new Meta { Name = "EmailEN", DbName = "Email_en", Title = Words("col.email.en"), Type = EInput.Email, MaxLength = 256 };
                        Meta emailCn = new Meta { Name = "EmailCN", DbName = "Email_cn", Title = Words("col.email.cn"), Type = EInput.Email, MaxLength = 256 };
                        Meta phoneEn = new Meta { Name = "PhoneEN", DbName = "Phone_en", Title = Words("col.phone.en"), Type = EInput.String, MaxLength = 32 };
                        Meta phoneCn = new Meta { Name = "PhoneCN", DbName = "Phone_cn", Title = Words("col.phone.cn"), Type = EInput.String, MaxLength = 32 };

                        table.AddMetas(id, titleEN, titleCN, detailEN, detailCN, foundDate, active, sort)
                            .AddMetas(address, city, state, country, postal)
                            .AddMetas(emailEn, emailCn, phoneEn, phoneCn);

                        Filter f1 = new Filter() { Name = "BranchFilter", DbName = "BranchId", Title = Words("col.branch"), Type = EFilter.Int, Compare = ECompare.Equal };
                        f1.AddListRef("BranchList");
                        table.AddFilter(f1);
                        table.Navi.IsActive = true;
                        table.AddRelation(new Relation(ERef.O2M, "BranchId", 1));
                        table.Navi.Order = "DESC";
                        table.Navi.By = "Sort";
                        table.GetUrl = "/Admin/api/Branch/ReloadSite";
                        table.SaveUrl = "/Admin/api/Branch/SaveSite";
                        table.AddQueryKV("Deleted", false).AddDeleteKV("LastUpdated", DateTime.Now.UTCSeconds())
                            .AddUpdateKV("LastUpdated", DateTime.Now.UTCSeconds())
                            .AddInsertKV("Deleted", false).AddInsertKV("CreatedTime", DateTime.Now.UTCSeconds());

                        CollectionTable c1 = new CollectionTable("BranchList", "GBranch", true, "Id", "Title", "Detail", "", "DESC", "Sort");
                        Collection BranchList = new Collection(ECollectionType.Table, c1);
                        BranchList.AddFilter("Id", this.DB.User.ActiveBranches);

                        CollectionTable c2 = new CollectionTable("CountryList", "GCountry", true, "Id", "Title", "Detail", "", "DESC", "Sort");
                        Collection CountryList = new Collection(ECollectionType.Table, c2);
                        CollectionTable c3 = new CollectionTable("StateList", "GState", true, "Id", "Title","Detail", "", "DESC","Sort");
                        Collection StateList = new Collection(ECollectionType.Table, c3);

                        this.DB.AddTable(table).AddCollections(BranchList, StateList, CountryList);
                    }
                    break;
            }
        }

    }
}