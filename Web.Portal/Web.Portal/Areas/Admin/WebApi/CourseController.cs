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
    public class CourseController : AdminBaseController
    {
        public CourseController(AppSetting appConfig) : base(appConfig) { }

        [HttpGet("InitAgreement")]
        public IActionResult InitAgreement()
        {
            this.Init("M1010");
            this.DB.FillAll();
            return Ok(this.DB);
        }
        [HttpPost("SaveAgreement")]
        public IActionResult SaveAgreement(JSTable jsTable)
        {
            this.Init("M1010");
            return Ok(this.DB.SaveTable(jsTable));
        }
        [HttpPost("ReloadAgreement")]
        public IActionResult ReloadAgreement(JSTable jsTable)
        {
            this.Init("M1010");
            return Ok(this.DB.ReloadTable(jsTable));
        }

        [HttpGet("InitCourse")]
        public IActionResult InitCourse()
        {
            this.Init("M1020");
            this.DB.FillAll();
            return Ok(this.DB);
        }
        [HttpPost("SaveCourse")]
        public IActionResult SaveCourse(JSTable jsTable)
        {
            this.Init("M1020");
            return Ok(this.DB.SaveTable(jsTable));
        }
        [HttpPost("ReloadCourse")]
        public IActionResult ReloadCourse(JSTable jsTable)
        {
            this.Init("M1020");
            return Ok(this.DB.ReloadTable(jsTable));
        }
        [HttpGet("InitCourseAll")]
        public IActionResult InitCourseAll()
        {
            this.Init("M1030");
            this.DB.FillAll();
            return Ok(this.DB);
        }
        [HttpPost("SaveCourseDetail")]
        public IActionResult SaveCourseDetail(JSTable jsTable)
        {
            this.Init("M1030");
            return Ok(this.DB.SaveTable(jsTable));
        }
        [HttpPost("ReloadCourseDetail")]
        public IActionResult ReloadCourseDetail(JSTable jsTable)
        {
            this.Init("M1030");
            return Ok(this.DB.ReloadTable(jsTable));
        }

        protected override void InitDatabase(string menuId)
        {
            switch (menuId)
            {
                case "M1010":
                    {
                        Table table = new Table("CourseAgreement", "Course_Agreement", Words("course.agreement"));
                        Meta id = new Meta { Name = "Id", DbName = "Id", Title = "ID", IsKey = true };
                        Meta titleEN = new Meta { Name = "TitleEN", DbName = "Title_en", Title = Words("title.en"), Order = "ASC", Required = true, Type = EInput.String, MaxLength = 64 };
                        Meta titleCN = new Meta { Name = "TitleCN", DbName = "Title_cn", Title = Words("title.cn"), Order = "ASC", Type = EInput.String, MaxLength = 64 };
                        Meta detailEN = new Meta { Name = "DetailEN", DbName = "Detail_en", Title = Words("detail.en"), Order = "ASC", Type = EInput.String, MaxLength = 256 };
                        Meta detailCN = new Meta { Name = "DetailCN", DbName = "Detail_cn", Title = Words("detail.cn"), Order = "ASC", Type = EInput.String, MaxLength = 256 };
                        Meta active = new Meta { Name = "Active", DbName = "Active", Title = Words("status.active"), Type = EInput.Bool };
                        Meta sort = new Meta { Name = "Sort", DbName = "Sort", Title = Words("col.sort"), Type = EInput.Int, Order = "DESC" };
                        Meta lastUpdated = new Meta { Name = "LastUpdated", DbName = "LastUpdated", Title = Words("col.lastupdated"), Type = EInput.Int, Order = "DESC" };
                        Meta agreement = new Meta { Name = "Agreement", DbName = "Agreement", Title = Words("course.agreement"), Type = EInput.String, MaxLength = 500 * 1024 };
                        table.AddMetas(id, titleEN, titleCN, detailEN, detailCN, active, sort, lastUpdated, agreement);

                        Filter f1 = new Filter() { Name = "search_keyword", DbName = "Title_en,Title_cn,Detail_en,Detail_cn", Title = Words("col.keyword"), Type = EFilter.String, Compare = ECompare.Like };
                        table.AddFilter(f1);
                        table.Navi.IsActive = true;
                        table.Navi.Order = "DESC";
                        table.Navi.By = "Sort";
                        table.GetUrl = "/Admin/api/Course/ReloadAgreement";
                        table.SaveUrl = "/Admin/api/Course/SaveAgreement";
                        table.AddQueryKV("Deleted", false).AddDeleteKV("LastUpdated", DateTime.Now.UTCSeconds())
                            .AddUpdateKV("LastUpdated", DateTime.Now.UTCSeconds())
                            .AddInsertKV("Deleted", false).AddInsertKV("CreatedTime", DateTime.Now.UTCSeconds());

                        this.DB.AddTable(table);
                    }
                    break;
                case "M1020":
                    {
                        Table table = new Table("CourseDefine", "Course", Words("course.category"));
                        Meta id = new Meta { Name = "Id", DbName = "Id", Title = "ID", IsKey = true };
                        Meta titleEN = new Meta { Name = "Title_en", DbName = "Title_en", Title = Words("title.en"), Order = "ASC", Required = true, Type = EInput.String, MaxLength = 64 };
                        Meta titleCN = new Meta { Name = "Title_cn", DbName = "Title_cn", Title = Words("title.cn"), Order = "ASC", Required = true, Type = EInput.String, MaxLength = 64 };
                        Meta detailEN = new Meta { Name = "Detail_en", DbName = "Detail_en", Title = Words("detail.en"), Order = "ASC", Type = EInput.String, MaxLength = 500 * 1024 };
                        Meta detailCN = new Meta { Name = "Detail_cn", DbName = "Detail_cn", Title = Words("detail.cn"), Order = "ASC", Type = EInput.String, MaxLength = 500 * 1024 };
                        Meta active = new Meta { Name = "Active", DbName = "Active", Title = Words("status.active"), Description = Words("status.active.inactive"), Type = EInput.Bool };
                        Meta isFree = new Meta { Name = "IsFree", DbName = "IsFree", Title = Words("col.isfree"), Description = Words("col.free.charge"), Type = EInput.Bool, Order = "ASC" };
                        Meta feeAmount = new Meta { Name = "FeeAmount", DbName = "FeeAmount", Title = Words("col.feeamount"), Type = EInput.Float, Order = "DESC" };
                        Meta currency = new Meta { Name = "Currency", DbName = "Currency", Title = Words("col.currency"), Type = EInput.Int };
                        currency.AddListRef("CurrencyList");

                        Meta sort = new Meta { Name = "Sort", DbName = "Sort", Title = Words("col.sort"), Type = EInput.Int, Order = "DESC" };
                        Meta branchId = new Meta { Name = "BranchId", DbName = "BranchId", Title = Words("col.branch"), Type = EInput.Int, Required = true, Order = "ASC" };
                        branchId.AddListRef("BranchList");
                        Meta siteId = new Meta { Name = "SiteId", DbName = "SiteId", Title = Words("col.site"), Type = EInput.Int, Required = true, Order = "ASC" };
                        siteId.AddListRef("SiteList");
                        Meta agreeId = new Meta { Name = "AgreeId", DbName = "AgreeId", Title = Words("course.agreement"), Type = EInput.Int, Order = "ASC" };
                        agreeId.AddListRef("AgreeList");
                        Meta category = new Meta { Name = "Category", DbName = "Category", Title = Words("col.category"), Order = "ASC", Type = EInput.String, MaxLength = 16 };

                        table.AddMetas(id, titleEN, titleCN, detailEN, detailCN, active, sort, branchId, siteId, agreeId, category, isFree, feeAmount, currency);

                        CollectionTable c1 = new CollectionTable("BranchList", "GBranch", true,"Id", "Title","Detail", "", "DESC", "Sort");
                        Collection BranchList = new Collection(ECollectionType.Table, c1);
                        BranchList.AddFilter("Id", this.DB.User.ActiveBranches);

                        CollectionTable c2 = new CollectionTable("SiteList", "GSite", true, "Id", "Title", "Detail", "BranchId", "DESC", "Sort");
                        Collection SiteList = new Collection(ECollectionType.Table, c2);
                        SiteList.AddFilter("Id", this.DB.User.ActiveSites);

                        CollectionTable c3 = new CollectionTable("AgreeList", "Course_Agreement", true, "Id", "Title", "Detail", "", "DESC", "Sort");
                        Collection AgreeList = new Collection(ECollectionType.Table, c3);

                        CollectionTable c4 = new CollectionTable("CurrencyList", "Currency", true, "Id", "Title", "Detail", "", "DESC", "Sort");
                        Collection CurrencyList = new Collection(ECollectionType.Category, c4);

                        Filter f1 = new Filter() { Name = "search_keyword", DbName = "Title_en,Title_cn", Title = Words("col.keyword"), Type = EFilter.String, Compare = ECompare.Like };
                        Filter f2 = new Filter() { Name = "search_branch", DbName = "BranchId", Title = Words("col.branch"), Type = EFilter.Int, Compare = ECompare.Equal };
                        f2.AddListRef("BranchList");
                        Filter f3 = new Filter() { Name = "search_site", DbName = "SiteId", Title = Words("col.site"), Type = EFilter.Int, Compare = ECompare.Equal };
                        f3.AddListRef("SiteList");

                        Filter f4 = new Filter() { Name = "fitler_branch", DbName = "BranchId", Title = "col.branch", Type = EFilter.Hidden, Required = true, Compare = ECompare.In, Value1 = this.DB.User.ActiveBranches };
                        Filter f5 = new Filter() { Name = "fitler_site", DbName = "SiteId", Title = "col.site", Type = EFilter.Hidden, Required = true, Compare = ECompare.In, Value1 = this.DB.User.ActiveSites };
                        table.AddFilters(f1, f2, f3, f4, f5);
                        table.Navi.IsActive = true;
                        table.Navi.Order = "DESC";
                        table.Navi.By = "Sort";
                        table.GetUrl = "/Admin/api/Course/ReloadCourse";
                        table.SaveUrl = "/Admin/api/Course/SaveCourse";
                        table.AddQueryKV("Deleted", false).AddDeleteKV("LastUpdated", DateTime.Now.UTCSeconds())
                              .AddUpdateKV("LastUpdated", DateTime.Now.UTCSeconds())
                              .AddInsertKV("Deleted", false).AddInsertKV("CreatedTime", DateTime.Now.UTCSeconds());

                        this.DB.AddCollections(BranchList, SiteList, AgreeList, CurrencyList).AddTable(table);
                    }
                    break;
                case "M1030":
                    {
                        Table branch = new Table("Branch", "GBranch", Words("col.branch"));
                        Meta bid = new Meta { Name = "Id", DbName = "Id", Title = "ID", Type = EInput.Int, IsKey = true };
                        Meta btitle = new Meta { Name = "Title", DbName = "Title", Title = Words("title.en"), IsLang = true, Type = EInput.String, MaxLength = 64 };
                        branch.AddMetas(bid, btitle);
                        branch.Navi.IsActive = false;
                        branch.Navi.By = "Sort,Title";
                        branch.Navi.Order = "DESC,ASC";
                        Filter bf1 = new Filter() { Name = "fitler_branch", DbName = "Id", Title = "col.branch", Type = EFilter.Hidden, Required = true, Compare = ECompare.In, Value1 = this.DB.User.ActiveBranches };
                        branch.AddFilter(bf1);
                        branch.AddQueryKV("Active", true).AddQueryKV("Deleted", false);

                        Table site = new Table("Site", "GSite", Words("col.site"));
                        Meta sid = new Meta { Name = "Id", DbName = "Id", Title = "ID", Type = EInput.Int, IsKey = true };
                        Meta rid = new Meta { Name = "BranchId", DbName = "BranchId", Type = EInput.Int, Title = "col.branch" };
                        Meta stitle = new Meta { Name = "Title", DbName = "Title", Title = Words("title.en"), IsLang = true, Type = EInput.String, MaxLength = 64 };
                        Filter sf1 = new Filter() { Name = "fitler_site", DbName = "Id", Title = "col.site", Type = EFilter.Hidden, Required = true, Compare = ECompare.In, Value1 = this.DB.User.ActiveSites};
                        site.AddMetas(sid, rid, stitle);
                        site.AddFilter(sf1);
                        site.AddQueryKV("Active", true).AddQueryKV("Deleted", false);



                        Table table = new Table("CourseDefine", "Course", Words("course.category"));
                        Meta id = new Meta { Name = "Id", DbName = "Id", Title = "ID", IsKey = true };
                        Meta title = new Meta { Name = "Title", DbName = "Title", IsLang = true, Title = Words("title.en"), Order = "ASC", Type = EInput.Read, Sync = true, MaxLength = 64 };
                        Meta titleEN = new Meta { Name = "Title_en", DbName = "Title_en", Title = Words("title.en"), Order = "ASC", Required = true, Type = EInput.String, MaxLength = 64 };
                        Meta titleCN = new Meta { Name = "Title_cn", DbName = "Title_cn", Title = Words("title.cn"), Order = "ASC", Required = true, Type = EInput.String, MaxLength = 64 };
                        Meta detailEN = new Meta { Name = "Detail_en", DbName = "Detail_en", Title = Words("detail.en"), Order = "ASC", Type = EInput.String, MaxLength = 500 * 1024 };
                        Meta detailCN = new Meta { Name = "Detail_cn", DbName = "Detail_cn", Title = Words("detail.cn"), Order = "ASC", Type = EInput.String, MaxLength = 500 * 1024 };
                        Meta isFree = new Meta { Name = "IsFree", DbName = "IsFree", Title = Words("col.isfree"), Description = Words("col.free.charge"), Type = EInput.Bool, Order = "ASC" };
                        Meta feeAmount = new Meta { Name = "FeeAmount", DbName = "FeeAmount", Title = Words("col.feeamount"), Type = EInput.Float, Order = "DESC" };
                        Meta currency = new Meta { Name = "Currency", DbName = "Currency", Title = Words("col.currency"), Type = EInput.Int };
                        currency.AddListRef("CurrencyList");
                        Meta active = new Meta { Name = "Active", DbName = "Active", Title = Words("status.active"), Description = Words("status.active.inactive"), Type = EInput.Bool };
                        Meta sort = new Meta { Name = "Sort", DbName = "Sort", Title = Words("col.sort"), Type = EInput.Int, Order = "DESC" };
                        Meta branchId = new Meta { Name = "BranchId", DbName = "BranchId", Title = Words("col.branch"), Type = EInput.Int, Required = true, Order = "ASC" };
                        branchId.AddListRef("BranchList");
                        Meta siteId = new Meta { Name = "SiteId", DbName = "SiteId", Title = Words("col.site"), Type = EInput.Int, Required = true, Order = "ASC" };
                        siteId.AddListRef("SiteList");
                        Meta agreeId = new Meta { Name = "AgreeId", DbName = "AgreeId", Title = Words("course.agreement"), Type = EInput.Int, Order = "ASC" };
                        agreeId.AddListRef("AgreeList");
                        Meta category = new Meta { Name = "Category", DbName = "Category", Title = Words("col.category"), Order = "ASC", Type = EInput.String, MaxLength = 16 };

                        table.AddMetas(id, title, titleEN, titleCN, detailEN, detailCN, isFree, feeAmount, currency, active, sort, branchId, siteId, agreeId, category);

                        CollectionTable c1 = new CollectionTable("BranchList", "GBranch", true, "Id", "Title", "Detail", "", "DESC", "Sort");
                        Collection BranchList = new Collection(ECollectionType.Table, c1);
                        BranchList.AddFilter("Id", this.DB.User.ActiveBranches);

                        CollectionTable c2 = new CollectionTable("SiteList", "GSite", true, "Id", "Title", "Detail", "BranchId", "DESC", "Sort");
                        Collection SiteList = new Collection(ECollectionType.Table, c2);
                        SiteList.AddFilter("Id", this.DB.User.Sites);

                        CollectionTable c3 = new CollectionTable("AgreeList", "Course_Agreement", true, "Id", "Title", "Detail", "", "DESC", "Sort");
                        Collection AgreeList = new Collection(ECollectionType.Table, c3);
                        CollectionTable c4 = new CollectionTable("CurrencyList", "Currency", true, "Id", "Title", "Detail", "", "DESC", "Sort");
                        Collection CurrencyList = new Collection(ECollectionType.Category, c4);

                        Filter f4 = new Filter() { Name = "fitler_branch", DbName = "BranchId", Title = "col.branch", Type = EFilter.Hidden, Required = true, Compare = ECompare.In, Value1 = this.DB.User.ActiveBranches };
                        Filter f5 = new Filter() { Name = "fitler_site", DbName = "SiteId", Title = "col.site", Type = EFilter.Hidden, Required = true, Compare = ECompare.In, Value1 = this.DB.User.ActiveSites };
                        table.AddFilters(f4, f5);
                        table.Navi.IsActive = false;
                        table.Navi.Order = "DESC,ASC";
                        table.Navi.By = "Sort,Title_en";
                        table.GetUrl = "/Admin/api/Course/ReloadCourseDetail";
                        table.SaveUrl = "/Admin/api/Course/SaveCourseDetail";
                        table.AddQueryKV("Deleted", false).AddDeleteKV("LastUpdated", DateTime.Now.UTCSeconds())
                            .AddUpdateKV("LastUpdated", DateTime.Now.UTCSeconds())
                            .AddInsertKV("Deleted", false).AddInsertKV("CreatedTime", DateTime.Now.UTCSeconds());

                        this.DB.AddCollections(BranchList, SiteList, AgreeList, CurrencyList).AddTables(table, branch, site);
                    }
                    break;
            }
        }

    }
}