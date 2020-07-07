using System;
using System.Collections.Generic;
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
    public class ClassEventController : AdminBaseController
    {
        public ClassEventController(AppSetting appConfig) : base(appConfig) { }

        [HttpGet("InitEditClass")]
        public IActionResult InitEditClass()
        {
            this.Init("M2010");
            this.DB.FillAll();
            return Ok(this.DB);
        }
        [HttpPost("ReloadEditClass")]
        public IActionResult ReloadEditClass(JSTable jsTable)
        {
            this.Init("M2010");
            return Ok(this.DB.ReloadTable(jsTable));
        }
        [HttpPost("SaveEditClass")]
        public IActionResult SaveEditClass(JSTable jsTable)
        {
            this.Init("M2010");
            return Ok(this.DB.SaveTable(jsTable));
        }

        [HttpPost("ReloadClassDetail")]
        public IActionResult ReloadClassDetail(JSTable jsTable)
        {
            this.Init("M2010");
            return Ok(this.DB.ReloadTable(jsTable));
        }
        [HttpPost("SaveClassDetail")]
        public IActionResult SaveClassDetail(JSTable jsTable)
        {
            this.Init("M2010");
            return Ok(this.DB.SaveTable(jsTable));
        }

        [HttpGet("InitCalendarClass")]
        public IActionResult InitCalendarClass()
        {
            this.Init("M2020");
            this.DB.FillAll();
            return Ok(this.DB);
        }
        [HttpPost("ReloadCalendarClass")]
        public IActionResult ReloadCalendarClass(JSTable jsTable)
        {
            this.Init("M2020");
            return Ok(this.DB.ReloadTable(jsTable));
        }
        [HttpPost("SaveCalendarClass")]
        public IActionResult SaveCalendarClass(JSTable jsTable)
        {
            this.Init("M2020");
            return Ok(this.DB.SaveTable(jsTable));
        }
        [HttpPost("LoadCalendar")]
        public IActionResult LoadCalendar(Calendar calendar)
        {
            this.Init("M2020");
            if (calendar.IsActive)
            {
                int branchId = calendar.Data["BranchId"].GetInt() ?? -1;
                int siteId = calendar.Data["SiteId"].GetInt() ?? -1;
                this.DB.Tables["ClassCalendar"].Filters["search_branch"].Value1 = branchId;
                this.DB.Tables["ClassCalendar"].Filters["search_site"].Value1 = siteId;
                this.DB.Tables["ClassCalendar"].Filters["search_date"].Value1 = calendar.Start.YMD();
                this.DB.Tables["ClassCalendar"].Filters["search_date"].Value2 = calendar.End.YMD();
                this.DB.Tables["ClassCalendar"].FillData();
                if (this.DB.Tables["ClassCalendar"].Rows.Count > 0)
                {
                    foreach (Row row in this.DB.Tables["ClassCalendar"].Rows)
                    {
                        calendar.Add(
                                        row.GetValue("DateId").GetInt() ?? 0, 
                                        row.GetValue("ClassDate").GetDateTime() ?? DateTime.MinValue, 
                                        row.GetValue("StartTime").GetTimeSpan() ?? TimeSpan.MinValue, 
                                        row.GetValue("EndTime").GetTimeSpan() ?? TimeSpan.MinValue, 
                                        row.GetValue("DateTitle").GetString(), 
                                        "", 
                                        row.GetValue("DateStatus").GetBool() ?? false, 
                                        row.GetValue("ClassStatus").GetInt() ?? 0
                                    );
                    }
                }
            }
            return Ok(calendar);
        }

        [HttpGet("InitEnroll")]
        public IActionResult InitEnroll()
        {
            this.Init("M2030");
            this.DB.FillAll();
            return Ok(this.DB);
        }

        [HttpPost("ReloadClassEnroll")]
        public IActionResult ReloadClassEnroll(JSTable jsTable)
        {
            this.Init("M2030");
            var UserLists = this.DB.DSQL.ExecuteTable("SELECT UserId FROM Class_Enroll WHERE Deleted=0 AND ClassId = @classId", new Dictionary<string, object> { { "classId", jsTable.RefKey } });
            List<int> Users = new List<int>();
            foreach (GRow e in UserLists.Rows)
            {
                if (e.GetValue("UserId").GetInt().HasValue) Users.Add(e.GetValue("UserId").GetInt().Value);
            }
            jsTable.Other.Clear();
            jsTable.Other.Add("InClass", Users.ToArray());
            return Ok(this.DB.ReloadTable(jsTable));
        }

        [HttpPost("ReloadPubUser")]
        public IActionResult ReloadPubUser(JSTable jsTable)
        {
            this.Init("M2030");
            return Ok(this.DB.ReloadTable(jsTable));
        }

        [HttpPost("ReloadPubUserId")]
        public IActionResult ReloadPubUserId(JSTable jsTable)
        {
            this.Init("M2030");
            return Ok(this.DB.ReloadTable(jsTable));
        }

        [HttpPost("ReloadAllUser")]
        public IActionResult ReloadAllUser(JSTable gtb)
        {
            this.Init("M2030");
            return Ok(this.DB.ReloadTable(gtb));
        }
        [HttpPost("ReloadStudent")]
        public IActionResult ReloadStudent(JSTable jsTable)
        {
            this.Init("M2030");
            return Ok(this.DB.ReloadTable(jsTable));
        }
        [HttpPost("ReloadStudentId")]
        public IActionResult ReloadStudentId(JSTable jsTable)
        {
            this.Init("M2030");
            return Ok(this.DB.ReloadTable(jsTable));
        }

        [HttpPost("SaveAllUser")]
        public IActionResult SaveAllUser(JSTable jsTable)
        {
            this.Init("M2030");
            return Ok(this.DB.SaveTable(jsTable));
        }
        [HttpPost("SaveClassEnroll")]
        public IActionResult ClassEnroll(JSTable jsTable)
        {
            this.Init("M2030");
            Table ntab = this.DB.SaveTable(jsTable);

            var UserLists = this.DB.DSQL.ExecuteTable("SELECT UserId FROM Class_Enroll WHERE Deleted=0 AND ClassId = @classId", new Dictionary<string, object> { { "classId", jsTable.RefKey } });
            List<int> Users = new List<int>();
            foreach (GRow e in UserLists.Rows)
            {
                if (e.GetValue("UserId").GetInt().HasValue) Users.Add(e.GetValue("UserId").GetInt().Value);
            }
            ntab.Other.Clear();
            ntab.Other.Add("InClass", Users.ToArray());

            return Ok(ntab);
        }
        [HttpPost("SaveStudent")]
        public IActionResult SaveStudent(JSTable jsTable)
        {
            this.Init("M2030");
            return Ok(this.DB.SaveTable(jsTable));
        }
        [HttpPost("SaveStudentId")]
        public IActionResult SaveStudentId(JSTable jsTable)
        {
            this.Init("M2030");
            return Ok(this.DB.SaveTable(jsTable));
        }

        protected override void InitDatabase(string menuId)
        {
            switch (menuId)
            {
                case "M2010":
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
                        Filter sf1 = new Filter() { Name = "fitler_site", DbName = "Id", Title = "col.site", Type = EFilter.Hidden, Required = true, Compare = ECompare.In, Value1 = this.DB.User.ActiveSites };
                        site.AddMetas(sid, rid, stitle);
                        site.AddFilter(sf1);
                        site.AddQueryKV("Active", true).AddQueryKV("Deleted", false);



                        Table course = new Table("CourseDefine", "Course", Words("course.category"));
                        Meta id = new Meta { Name = "Id", DbName = "Id", Title = "ID", IsKey = true };
                        Meta title = new Meta { Name = "Title", DbName = "Title", IsLang = true, Title = Words("title.en"), Order = "ASC", Type = EInput.Read, Sync = true, MaxLength = 64 };
                        Meta titleEN = new Meta { Name = "Title_en", DbName = "Title_en", Title = Words("title.en"), Order = "ASC", Required = true, Type = EInput.String, MaxLength = 64 };
                        Meta titleCN = new Meta { Name = "Title_cn", DbName = "Title_cn", Title = Words("title.cn"), Order = "ASC", Required = true, Type = EInput.String, MaxLength = 64 };
                        Meta detailEN = new Meta { Name = "Detail_en", DbName = "Detail_en", Title = Words("detail.en"), Type = EInput.String, MaxLength = 500 * 1024 };
                        Meta detailCN = new Meta { Name = "Detail_cn", DbName = "Detail_cn", Title = Words("detail.cn"), Type = EInput.String, MaxLength = 500 * 1024 };
                        Meta active = new Meta { Name = "Active", DbName = "Active", Title = Words("status.active"), Description = Words("status.active.inactive"), Type = EInput.Bool };
                        Meta isFree = new Meta { Name = "IsFree", DbName = "IsFree", Title = Words("col.isfree"), Description = Words("col.free.charge"), Type = EInput.Bool, Order = "ASC" };
                        Meta feeAmount = new Meta { Name = "FeeAmount", DbName = "FeeAmount", Title = Words("col.feeamount"), Type = EInput.Float, Order = "DESC" };
                        Meta currency = new Meta { Name = "Currency", DbName = "Currency", Title = Words("col.currency"), Type = EInput.Int };
                        currency.AddListRef("CurrencyList");

                        Meta branchId = new Meta { Name = "BranchId", DbName = "BranchId", Title = Words("col.branch"), Type = EInput.Int, Required = true, Order = "ASC" };
                        Meta siteId = new Meta { Name = "SiteId", DbName = "SiteId", Title = Words("col.site"), Type = EInput.Int, Required = true, Order = "ASC" };

                        course.AddMetas(id, title, titleEN, titleCN, detailEN, detailCN, active, isFree, feeAmount, currency, branchId, siteId);
                        course.AddQueryKV("Active", true).AddQueryKV("Deleted", false);

                        Filter f4 = new Filter() { Name = "fitler_branch", DbName = "BranchId", Title = "col.branch", Type = EFilter.Hidden, Required = true, Compare = ECompare.In, Value1 = this.DB.User.ActiveBranches };
                        Filter f5 = new Filter() { Name = "fitler_site", DbName = "SiteId", Title = "col.site", Type = EFilter.Hidden, Required = true, Compare = ECompare.In, Value1 = this.DB.User.ActiveSites };
                        course.AddFilters(f4, f5);
                        course.Navi.IsActive = false;
                        course.Navi.Order = "DESC,ASC";
                        course.Navi.By = "Sort,Title_en";


                        Table ClassEvt = new Table("ClassEvent", "Class", Words("class.event"));
                        Meta cid = new Meta { Name = "Id", DbName = "Id", Title = "ID", IsKey = true };
                        Meta ccourseId = new Meta { Name = "CourseId", DbName = "CourseId", Title = Words("col.course"), Type = EInput.Int, Required = true };
                        Meta ctitleEN = new Meta { Name = "Title_en", DbName = "Title_en", Title = Words("title.en"), Required = true, Order = "ASC", Type = EInput.String, MaxLength = 64 };
                        Meta ctitleCN = new Meta { Name = "Title_cn", DbName = "Title_cn", Title = Words("title.cn"), Required = true, Order = "ASC", Type = EInput.String, MaxLength = 64 };
                        Meta cdetailEN = new Meta { Name = "Detail_en", DbName = "Detail_en", Title = Words("detail.en"), Type = EInput.String, MaxLength = 500 * 1024 };
                        Meta cdetailCN = new Meta { Name = "Detail_cn", DbName = "Detail_cn", Title = Words("detail.cn"), Type = EInput.String, MaxLength = 500 * 1024 };
                        Meta cisFree = new Meta { Name = "IsFree", DbName = "IsFree", Title = Words("col.isfree"), Description = Words("col.free.charge"), Type = EInput.Bool, Order = "ASC" };
                        Meta cfeeAmount = new Meta { Name = "FeeAmount", DbName = "FeeAmount", Title = Words("col.feeamount"), Type = EInput.Float, Order = "DESC" };
                        Meta ccurrency = new Meta { Name = "Currency", DbName = "Currency", Title = Words("col.currency"), Type = EInput.Int };
                        ccurrency.AddListRef("CurrencyList");
                        Meta cstartDate = new Meta { Name = "StartDate", DbName = "StartDate", Title = Words("start.date"), Required = true, Order = "DESC", Type = EInput.Date };
                        Meta cendDate = new Meta { Name = "EndDate", DbName = "EndDate", Title = Words("end.date"), Order = "DESC", Type = EInput.Date };
                        Meta cstatus = new Meta { Name = "Status", DbName = "Status", Title = Words("col.status"), Order = "DESC", Type = EInput.Int, Value = 1 };
                        cstatus.AddListRef("StatusList");
                        Filter f6 = new Filter() { Name = "fitler_courseId", DbName = "CourseId", Title = "col.course", Type = EFilter.Int, Required = true, Compare = ECompare.Equal, Value1 = -1 };
                        ClassEvt.Navi.IsActive = true;
                        ClassEvt.Navi.Order = "DESC";
                        ClassEvt.Navi.By = "StartDate";


                        ClassEvt.AddMetas(cid, ccourseId, ctitleCN, ctitleEN, cdetailCN, cdetailEN, cisFree, cfeeAmount, ccurrency, cstartDate, cendDate, cstatus);
                        ClassEvt.AddFilter(f6);
                        ClassEvt.AddQueryKV("Active", true).AddQueryKV("Deleted", false)
                                .AddUpdateKV("Deleted", false).AddUpdateKV("LastUpdated", DateTime.Now.UTCSeconds())
                                .AddInsertKV("Active",true).AddInsertKV("Deleted", false).AddInsertKV("CreatedTime", DateTime.Now.UTCSeconds());

                        ClassEvt.GetUrl = "/Admin/api/ClassEvent/ReloadEditClass";
                        ClassEvt.SaveUrl = "/Admin/api/ClassEvent/SaveEditClass";

                        Table ClassDetail = new Table("ClassDetail", "Class_Detail", Words("class.detail"));
                        Meta did = new Meta { Name = "Id", DbName = "Id", Title = "ID", IsKey = true };
                        Meta dtitleEN = new Meta { Name = "Title_en", DbName = "Title_en", Title = Words("title.en"), Required = true, Type = EInput.String, MaxLength = 64 };
                        Meta dtitleCN = new Meta { Name = "Title_cn", DbName = "Title_cn", Title = Words("title.cn"), Required = true, Type = EInput.String, MaxLength = 64 };
                        Meta ddetailEN = new Meta { Name = "Detail_en", DbName = "Detail_en", Title = Words("detail.en"), Type = EInput.String, MaxLength = 1024 };
                        Meta ddetailCN = new Meta { Name = "Detail_cn", DbName = "Detail_cn", Title = Words("detail.cn"), Type = EInput.String, MaxLength = 1024 };
                        Meta dclassDate = new Meta { Name = "ClassDate", DbName = "ClassDate", Title = Words("class.date"), Required = true, Order = "ASC", Type = EInput.Date };
                        Meta dstartTime = new Meta { Name = "StartTime", DbName = "StartTime", Title = Words("start.time"), Required = true, Type = EInput.Time };
                        Meta dendTime = new Meta { Name = "EndTime", DbName = "EndTime", Title = Words("end.time"), Required = true, Type = EInput.Time };
                        Meta dactive = new Meta { Name = "Active", DbName = "Active", Title = Words("status.active"), Type = EInput.Bool, Value = true };

                        ClassDetail.AddMetas(did, dtitleEN, dtitleCN, ddetailCN, ddetailEN, dclassDate, dstartTime, dendTime, dactive);
                        ClassDetail.Navi.IsActive = true;
                        ClassDetail.AddRelation(new Relation(ERef.O2M, "ClassId", 0));
                        ClassDetail.Navi.Order = "ASC";
                        ClassDetail.Navi.By = "ClassDate";
                        ClassDetail.GetUrl = "/Admin/api/ClassEvent/ReloadClassDetail";
                        ClassDetail.SaveUrl = "/Admin/api/ClassEvent/SaveClassDetail";
                        ClassDetail.AddQueryKV("Deleted", false).AddDeleteKV("LastUpdated", DateTime.Now.UTCSeconds())
                            .AddUpdateKV("LastUpdated", DateTime.Now.UTCSeconds())
                            .AddInsertKV("Deleted", false).AddInsertKV("CreatedTime", DateTime.Now.UTCSeconds());

                        CollectionTable c1 = new CollectionTable("CurrencyList", "Currency", true, "Id", "Title", "Detail", "", "DESC", "Sort");
                        Collection CurrencyList = new Collection(ECollectionType.Category, c1);
                        CollectionTable c2 = new CollectionTable("StatusList", "StatusList");
                        Collection StatusList = new Collection(ECollectionType.Common, c2);

                        this.DB.AddCollections(CurrencyList, StatusList).AddTables(branch, site, course, ClassEvt, ClassDetail);
                    }
                    break;
                case "M2020":
                    {
                        Table ClassCalendar = new Table("ClassCalendar", "VW_Class_Calendar", Words("class.event"));
                        Meta dateId = new Meta { Name = "DateId", DbName = "DateId", Title = "ID", IsKey = true };
                        Meta classDate = new Meta { Name = "ClassDate", DbName = "ClassDate", Title = Words("col.classdate"), Type = EInput.Date };
                        Meta startTime = new Meta { Name = "StartTime", DbName = "StartTime", Title = Words("start.time"), Type = EInput.Time };
                        Meta endTime = new Meta { Name = "EndTime", DbName = "EndTime", Title = Words("end.time"), Type = EInput.Time };
                        Meta title = new Meta { Name = "DateTitle", DbName = "Date_Title", Title = Words("col.title"), IsLang = true, Type = EInput.String };
                        Meta status = new Meta { Name = "DateStatus", DbName = "DateStatus", Title = Words("status.active"), Type = EInput.Bool };
                        Meta ccstatus = new Meta { Name = "ClassStatus", DbName = "ClassStatus", Title = Words("status.active"), Type = EInput.Int };
                        ccstatus.AddListRef("StatusList");
                        ClassCalendar.AddMetas(dateId, classDate, startTime, endTime, title, status, ccstatus);

                        CollectionTable c1 = new CollectionTable("BranchList", "GBranch", true, "Id", "Title", "Detail", "", "DESC", "Sort");
                        Collection BranchList = new Collection(ECollectionType.Table, c1);
                        BranchList.AddFilter("Id", this.DB.User.ActiveBranches);

                        CollectionTable c2 = new CollectionTable("SiteList", "GSite", true, "Id", "Title", "Detail", "BranchId", "DESC", "Sort");
                        Collection SiteList = new Collection(ECollectionType.Table, c2);
                        SiteList.AddFilter("Id", this.DB.User.ActiveSites);

                        int def_site = this.DB.User.ActiveSites.Count > 0 ? this.DB.User.Sites[0] : -1;
                        Filter f1 = new Filter() { Name = "search_branch", DbName = "BranchId", Title = Words("col.branch"), Required = true, Type = EFilter.Int, Compare = ECompare.Equal, Value1 = this.DB.User.Branch };
                        f1.AddListRef("BranchList");
                        Filter f2 = new Filter() { Name = "search_site", DbName = "SiteId", Title = Words("col.site"), Required = true, Type = EFilter.Int, Compare = ECompare.Equal, Value1 = def_site };
                        f2.AddListRef("SiteList");
                        Filter f3 = new Filter() { Name = "search_date", DbName = "ClassDate", Title = Words("class.date"),Type = EFilter.Date, Compare = ECompare.Range };
                        Filter f4 = new Filter() { Name = "fitler_branch", DbName = "BranchId", Title = "col.branch", Type = EFilter.Hidden, Required = true, Compare = ECompare.In, Value1 = this.DB.User.ActiveBranches };
                        Filter f5 = new Filter() { Name = "fitler_site", DbName = "SiteId", Title = "col.site", Type = EFilter.Hidden, Required = true, Compare = ECompare.In, Value1 = this.DB.User.ActiveSites };

                        ClassCalendar.AddFilters(f1, f2, f3, f4, f5);
                        ClassCalendar.Navi.IsActive = false;
                        ClassCalendar.Navi.Order = "ASC,ASC,ASC";
                        ClassCalendar.Navi.By = "ClassDate,StartTime,EndTime";
                        ClassCalendar.GetUrl = "/Admin/api/ClassEvent/ReloadCalendarClass";

                        Table dateEvent = new Table("DateEvent", "VW_Class_Calendar", Words("class.event"));
                        Meta ddateId = new Meta { Name = "DateId", DbName = "DateId", Title = "ID", IsKey = true };
                        Meta dclassDate = new Meta { Name = "ClassDate", DbName = "ClassDate", Title = Words("class.date"), Type = EInput.Date };
                        Meta dstartTime = new Meta { Name = "StartTime", DbName = "StartTime", Title = Words("start.time"), Type = EInput.Time };
                        Meta dendTime = new Meta { Name = "EndTime", DbName = "EndTime", Title = Words("end.time"), Type = EInput.Time };
                        Meta ctitle = new Meta { Name = "ClassTitle", DbName = "Class_Title", Title = Words("class.name"), IsLang = true, Type = EInput.String };
                        Meta cdetail = new Meta { Name = "ClassDetail", DbName = "Class_Detail", Title = Words("col.detail"), IsLang = true, Type = EInput.String };
                        Meta dtitle = new Meta { Name = "DateTitle", DbName = "Date_Title", Title = Words("date.event"), IsLang = true, Type = EInput.String };
                        Meta ddetail = new Meta { Name = "DateDetail", DbName = "Date_Detail", Title = Words("col.detail"), IsLang = true, Type = EInput.String };
                        Meta dStartDate = new Meta { Name = "StartDate", DbName = "StartDate", Title = Words("start.date"), Type = EInput.Date };
                        Meta dEndDate = new Meta { Name = "EndDate", DbName = "EndDate", Title = Words("end.date"), Type = EInput.Date };
                        Meta cstatus = new Meta { Name = "ClassStatus", DbName = "ClassStatus", Title = Words("col.status"), Type = EInput.Int };
                        Meta dstatus = new Meta { Name = "DateStatus", DbName = "DateStatus", Title = Words("col.status"), Description = Words("status.active.inactive"), Type = EInput.Bool };
                        cstatus.AddListRef("StatusList");
                        dateEvent.AddMetas(ddateId, dclassDate, dstartTime, dendTime, ctitle, dtitle, ddetail, cdetail, dStartDate, dEndDate, cstatus, dstatus);

                        dateEvent.Navi.IsActive = false;
                        dateEvent.AddRelation(new Relation(ERef.O2O, "DateId", 0));
                        dateEvent.GetUrl = "/Admin/api/ClassEvent/ReloadCalendarClass";
                        dateEvent.SaveUrl = "/Admin/api/ClassEvent/SaveCalendarClass";

                        this.DB.AddCollections(BranchList, SiteList, CommonCollection.GetStatus()).AddTables(ClassCalendar, dateEvent);
                    }
                    break;
                case "M2030":
                    {
                        Table pubUser = new Table("PubUser", "Pub_User", Words("public.user"));
                        // Meta Data
                        Meta pid = new Meta { Name = "Id", DbName = "Id", Title = Words("col.id"), Type = EInput.Int, IsKey = true };
                        Meta pfirstName = new Meta { Name = "FirstName", DbName = "FirstName", Title = Words("col.firstname"), Type = EInput.String, MaxLength = 64, Required = true, Order = "ASC" };
                        Meta plastName = new Meta { Name = "LastName", DbName = "LastName", Title = Words("col.lastname"), Type = EInput.String, MaxLength = 64, Required = true, Order = "ASC" };
                        Meta paliasname = new Meta { Name = "AliasName", DbName = "AliasName", Title = Words("col.aliasname"), Type = EInput.String, MaxLength = 128, Order = "ASC" };
                        Meta pfirstNameLegal = new Meta { Name = "FirstNameLegal", DbName = "FirstNameLegal", Title = Words("col.fullname.legal"), Type = EInput.Read, MaxLength = 64, Order = "ASC" };
                        Meta plastNameLegl = new Meta { Name = "LastNameLegal", DbName = "LastNameLegal", Title = Words("col.lastname.legal"), Type = EInput.Read, Order = "ASC" };
                        Meta pdharmaName = new Meta { Name = "DharmaName", DbName = "DharmaName", Title = Words("col.dharmaname"), Type = EInput.Read, Order = "ASC" };
                        Meta pdisplayName = new Meta { Name = "DisplayName", DbName = "DisplayName", Title = Words("col.nameinfo"), Type = EInput.Read, Order = "ASC" };
                        Meta pcertName = new Meta { Name = "CertificateName", DbName = "CertificateName", Title = Words("col.certificatename"), Type = EInput.Read, Order = "ASC" };
                        Meta pgender = new Meta { Name = "Gender", DbName = "Gender", Title = Words("col.gender"), Type = EInput.Int, Required = true, Order = "ASC" };
                        pgender.AddListRef("GenderList");

                        Meta pemail = new Meta { Name = "Email", DbName = "Email", Title = Words("col.email"), Type = EInput.Email, Unique=true, MaxLength = 256, Order = "ASC" };
                        Meta pphone = new Meta { Name = "Phone", DbName = "Phone", Title = Words("col.phone"), Type = EInput.String, MaxLength = 32, Order = "ASC" };
                        Meta pcell = new Meta { Name = "Cell", DbName = "Cell", Title = Words("col.cell"), Type = EInput.Read, Order = "ASC" };
                        Meta pcity = new Meta { Name = "City", DbName = "City", Title = Words("col.city"), Type = EInput.Read, Order = "ASC" };
                        Meta pphoto = new Meta { Name = "Photo", DbName = "Id", Title = Words("col.photo"), Description = "PubUser|tiny|small", Type = EInput.ImageContent };

                        Filter f111 = new Filter() { Name = "search_name", DbName = "FirstName,LastName,FirstNameLegal,LastNameLegal,DharmaName,DisplayName,CertificateName,AliasName", Title = Words("col.fullname"), Type = EFilter.String, Compare = ECompare.Like };
                        Filter f112 = new Filter() { Name = "search_email", DbName = "Email", Title = Words("col.email"), Type = EFilter.String, Compare = ECompare.Like };
                        Filter f113 = new Filter() { Name = "search_phone", DbName = "Phone,Cell", Title = Words("col.phone"), Type = EFilter.String, Compare = ECompare.Like };
                        Filter f114 = new Filter() { Name = "search_idno", DbName = "Id", Title = Words("col.idno"), Type = EFilter.String, Compare = ECompare.Include };
                        f114.AddListRef("MemberId", "Pub_User_Id", "UserId|IdNumber");  // don't need UserId = value, int <> 3939-3993-xxx idnumber
                        Filter f115 = new Filter() { Name = "fitler_branch", DbName = "BranchId", Title = "col.branch", Type = EFilter.Hidden, Required = true, Compare = ECompare.In, Value1 = this.DB.User.ActiveBranches };

                        pubUser.Navi.IsActive = true;
                        pubUser.Navi.InitFill = false;
                        pubUser.Navi.PageSize = 3;
                        pubUser.Navi.Order = "DESC";
                        pubUser.Navi.By = "CreatedTime";

                        // Table Public User
                        pubUser.AddMetas(pid, pfirstName, plastName, pfirstNameLegal, plastNameLegl, pdharmaName, pdisplayName, pcertName, paliasname, pgender)
                                   .AddMetas(pemail, pphone, pcell, pcity, pphoto);

                        pubUser.AddFilters(f111, f112, f113, f114, f115);

                        pubUser.GetUrl = "/Admin/api/ClassEvent/ReloadAllUser";
                        pubUser.SaveUrl = "/Admin/api/ClassEvent/SaveAllUser";
                        pubUser.AddQueryKV("Deleted", false).AddQueryKV("Active", true)
                                .AddUpdateKV("LastUpdated", DateTime.Now.UTCSeconds())
                                .AddInsertKV("Deleted", false).AddInsertKV("Active", true).AddInsertKV("BranchId", this.DB.User.Branch).AddInsertKV("CreatedTime", DateTime.Now.UTCSeconds()); ;

                        Table classEnroll = new Table("ClassEnroll", "VW_Class_Enroll_Member", Words("class.enroll"));
                        // Meta Data
                        Meta eid = new Meta { Name = "Id", DbName = "Id", Title = Words("col.id"), IsKey = true, Type = EInput.Read };
                        Meta eclassId = new Meta { Name = "ClassId", DbName = "ClassId", Title = Words("col.classid") };
                        Meta ememberId = new Meta { Name = "UserId", DbName = "UserId", Title = Words("col.userid") };
                        Meta efirstName = new Meta { Name = "FirstName", DbName = "FirstName", Title = Words("col.fullname"), Type = EInput.Read, Order = "ASC" };
                        Meta elastName = new Meta { Name = "LastName", DbName = "LastName", Title = Words("col.lastname"), Type = EInput.Read, Order = "ASC" };
                        Meta efirstNameLegal = new Meta { Name = "FirstNameLegal", DbName = "FirstNameLegal", Title = Words("col.fullname.legal"), Type = EInput.Read, Order = "ASC" };
                        Meta elastNameLegl = new Meta { Name = "LastNameLegal", DbName = "LastNameLegal", Title = Words("col.lastname.legal"), Type = EInput.Read, Order = "ASC" };
                        Meta edharmaName = new Meta { Name = "DharmaName", DbName = "DharmaName", Title = Words("col.dharmaname"), Type = EInput.Read, Order = "ASC" };
                        Meta edisplayName = new Meta { Name = "DisplayName", DbName = "DisplayName", Title = Words("col.nameinfo"), Sync = true, Type = EInput.Read, Order = "ASC" };
                        Meta ecertName = new Meta { Name = "CertificateName", DbName = "CertificateName", Title = Words("col.certificatename"), Sync = true, Type = EInput.Read, Order = "ASC" };
                        Meta ealiasname = new Meta { Name = "AliasName", DbName = "AliasName", Title = Words("col.aliasname"), Type = EInput.Read, Order = "ASC" };
                        Meta egender = new Meta { Name = "Gender", DbName = "Gender", Title = Words("col.gender"), Type = EInput.Read, Order = "ASC" };
                        egender.AddListRef("GenderList");

                        Meta eemail = new Meta { Name = "Email", DbName = "Email", Title = Words("col.email"), Type = EInput.Read, Order = "ASC" };
                        Meta ephone = new Meta { Name = "Phone", DbName = "Phone", Title = Words("col.phone"), Type = EInput.Read, MaxLength = 32, Order = "ASC" };
                        Meta ecell = new Meta { Name = "Cell", DbName = "Cell", Title = Words("col.cell"), Type = EInput.Read, Order = "ASC" };
                        Meta ecity = new Meta { Name = "City", DbName = "City", Title = Words("col.city"), Type = EInput.Read, Order = "ASC" };
                        Meta ecreatedTime = new Meta { Name = "CreatedTime", DbName = "CreatedTime", Title = Words("col.enrolldate"), Type = EInput.Read, Sync = true, Order = "DESC" };

                        Meta ephoto = new Meta { Name = "Photo", DbName = "UserId", Title = Words("col.photo"), Description = "PubUser|tiny|small", Type = EInput.ImageContent };
                        Meta egroup = new Meta { Name = "Grp", DbName = "Grp", Title = Words("col.grp"), Type = EInput.String, MaxLength = 16, Order = "ASC" };
                        Meta eisPaid = new Meta { Name = "IsPaid", DbName = "IsPaid", Title = Words("col.ispaid"), Description = Words("col.ispaid.yesno"), Type = EInput.Bool, Order = "DESC" };
                        Meta epaidDate = new Meta { Name = "PaidDate", DbName = "PaidDate", Title = Words("col.paiddate"), Type = EInput.Read, Sync = true, Order = "ASC" };
                        Meta epaidAmount = new Meta { Name = "PaidAmount", DbName = "PaidAmount", Title = Words("col.paidinfo"), Type = EInput.Float, MaxLength = 18 };
                        Meta epaidInvoice = new Meta { Name = "PaidInvoice", DbName = "PaidInvoice", Title = Words("col.paidinvoice"), Type = EInput.String, MaxLength = 32, Order = "ASC" };
                        Meta ememberType = new Meta { Name = "MemberType", DbName = "MemberType", Title = Words("col.membertype"), Type = EInput.Int };
                        ememberType.AddListRef("MemberTypeList");


                        // Filter
                        Filter f10 = new Filter() { Name = "search_class", DbName = "ClassId", Title = Words("class.class"), Type = EFilter.Int, Compare = ECompare.Equal };
                        f10.AddListRef("ClassList");

                        Filter f11 = new Filter() { Name = "search_name", DbName = "FirstName,LastName,FirstNameLegal,LastNameLegal,DharmaName,DisplayName,CertificateName,AliasName", Title = Words("col.fullname"), Type = EFilter.String, Compare = ECompare.Like };
                        Filter f12 = new Filter() { Name = "search_email", DbName = "Email", Title = Words("col.email"), Type = EFilter.String, Compare = ECompare.Like };
                        Filter f13 = new Filter() { Name = "search_phone", DbName = "Phone,Cell", Title = Words("col.phone"), Type = EFilter.String, Compare = ECompare.Like };
                        Filter f14 = new Filter() { Name = "search_idno", DbName = "UserId", Title = Words("col.idno"), Type = EFilter.String, Compare = ECompare.Include };
                        f14.AddListRef("MemberId", "Pub_User_Id", "UserId|IdNumber");  // don't need UserId = value, int <> 3939-3993-xxx idnumber
                        //f14.AddListRef("UserId", "Pub_User_Id", "UserId|IdNumber");
                        Filter f15 = new Filter() { Name = "search_grp", DbName = "Grp", Title = Words("col.grp"), Type = EFilter.String, Compare = ECompare.Like };
                        Filter f16 = new Filter() { Name = "search_type", DbName = "MemberType", Title = Words("col.membertype"), Type = EFilter.Int, Compare = ECompare.Equal };
                        f16.AddListRef("MemberTypeList");
                        Filter f17 = new Filter() { Name = "fitler_branch", DbName = "BranchId", Title = "col.branch", Type = EFilter.Hidden, Required = true, Compare = ECompare.In, Value1 = this.DB.User.ActiveBranches };

                        // Collection
                        CollectionTable c1 = new CollectionTable("ClassList", "VW_Class_Enroll", true, "ClassId", "Title", "Detail", "", "DESC", "StartDate");
                        Collection ClassList = new Collection(ECollectionType.Table, c1);
                        ClassList.AddFilter("SiteId", this.DB.User.ActiveSites);

                        CollectionTable c2 = new CollectionTable("MemberTypeList", "MemberType", true, "Id", "Title", "Detail", "", "DESC", "Sort");
                        Collection MemberTypeList = new Collection(ECollectionType.Category, c2);

                        // Navi 
                        classEnroll.Navi.IsActive = true;
                        classEnroll.AddRelation(new Relation(ERef.O2M, "ClassId", 0));
                        classEnroll.Navi.Order = "DESC";
                        classEnroll.Navi.By = "CreatedTime";
                        classEnroll.GetUrl = "/Admin/api/ClassEvent/ReloadClassEnroll";
                        classEnroll.SaveUrl = "/Admin/api/ClassEvent/SaveClassEnroll";

                        classEnroll.AddQueryKV("Deleted", false)
                                    .AddDeleteKV("LastUpdated", DateTime.Now.UTCSeconds())
                                    .AddUpdateKV("LastUpdated", DateTime.Now.UTCSeconds())
                                    .AddInsertKV("Deleted", false)
                                    .AddInsertKV("Active", true)
                                    .AddInsertKV("CreatedTime", DateTime.Now.UTCSeconds());


                        // Table Class Enroll
                        classEnroll.AddMetas(eid, eclassId, ememberId, efirstName, elastName, efirstNameLegal, elastNameLegl, ecreatedTime)
                                   .AddMetas(edharmaName, edisplayName, ecertName, ealiasname, egender, eemail, ephone, ecell, ecity, ephoto)
                                   .AddMetas(egroup, eisPaid, epaidDate, epaidAmount, epaidInvoice, ememberType);

                        classEnroll.AddFilters(f10, f11, f12, f13, f14, f15, f16, f17);



                        //Member Detail
                        Table Member = new Table("Member", "Pub_User", Words("pub.user"));
                        /*******/
                        Meta id = new Meta { Name = "Id", DbName = "Id", Title = Words("col.id"), IsKey = true };
                        Meta memberId = new Meta { Name = "MemberId", DbName = "MemberId", Title = Words("col.memberid"), Order = "ASC" };
                        Meta firstName = new Meta { Name = "FirstName", DbName = "FirstName", Title = Words("col.fullname"), Required = true, Type = EInput.String, MaxLength = 64, Order = "ASC" };
                        Meta lastName = new Meta { Name = "LastName", DbName = "LastName", Title = Words("col.lastname"), Required = true, Type = EInput.String, MaxLength = 64, Order = "ASC" };
                        Meta firstNameLegal = new Meta { Name = "FirstNameLegal", DbName = "FirstNameLegal", Title = Words("col.firstname.legal"), Type = EInput.String, MaxLength = 64, Order = "ASC" };
                        Meta lastNameLegl = new Meta { Name = "LastNameLegal", DbName = "LastNameLegal", Title = Words("col.lastname.legal"), Type = EInput.String, MaxLength = 64, Order = "ASC" };
                        Meta dharmaName = new Meta { Name = "DharmaName", DbName = "DharmaName", Title = Words("col.dharmaname"), Type = EInput.String, MaxLength = 64, Order = "ASC" };
                        Meta displayName = new Meta { Name = "DisplayName", DbName = "DisplayName", Title = Words("col.displayname"), Sync = true, Type = EInput.String, MaxLength = 128, Order = "ASC" };
                        Meta certName = new Meta { Name = "CertificateName", DbName = "CertificateName", Title = Words("col.certificatename"), Sync = true, Type = EInput.String, MaxLength = 128, Order = "ASC" };
                        Meta aliasname = new Meta { Name = "AliasName", DbName = "AliasName", Title = Words("col.aliasname"), Type = EInput.String, MaxLength = 128, Order = "ASC" };
                        Meta occupation = new Meta { Name = "Occupation", DbName = "Occupation", Title = Words("col.occupation"), Type = EInput.String, MaxLength = 256 };
                        Meta memo = new Meta { Name = "Memo", DbName = "Memo", Title = Words("col.memo"), Type = EInput.String, MaxLength = 256 };
                        Meta userName = new Meta { Name = "UserName", DbName = "UserName", Title = Words("col.username"), Type = EInput.String, Unique = true, MaxLength = 32 };
                        Meta password = new Meta { Name = "Password", DbName = "Password", Title = Words("col.password"), Description = Words("confirm.password"), Type = EInput.Passpair, MinLength = 6, MaxLength = 12 };
                        Meta idNumber = new Meta { Name = "IDNumber", DbName = "IDNumber", Title = Words("col.idnumber"), Type = EInput.String, MaxLength = 32 };
                        Meta medicalConcern = new Meta { Name = "MedicalConcern", DbName = "MedicalConcern", Title = Words("medical.concern"), Type = EInput.String, MaxLength = 1024 };
                        Meta hearUsOther = new Meta { Name = "HearUs_Other", DbName = "HearUs_Other", Title = Words("other.specify"), Type = EInput.String, MaxLength = 32 };
                        Meta multiLangOther = new Meta { Name = "MultiLang_Other", DbName = "MultiLang_Other", Title = Words("other.specify"), Type = EInput.String, MaxLength = 32 };
                        Meta symbolOther = new Meta { Name = "Symbol_Other", DbName = "Symbol_Other", Title = Words("other.specify"), Type = EInput.String, MaxLength = 32 };
                        Meta photo = new Meta { Name = "Photo", DbName = "Id", Title = Words("col.photo"), Description = "PubUser|tiny|small", Type = EInput.ImageContent };
                        Meta active = new Meta { Name = "Active", DbName = "Active", Title = Words("col.status"), Description = Words("status.active.inactive"), Type = EInput.Bool };
                        Meta loginTime = new Meta { Name = "LoginTime", DbName = "LoginTime", Title = Words("col.lastlogin"), Type = EInput.Int };
                        Meta loginTotal = new Meta { Name = "LoginTotal", DbName = "LoginTotal", Title = Words("col.logintotal"), Type = EInput.Int };
                        Meta createdTime = new Meta { Name = "CreatedTime", DbName = "CreatedTime", Title = Words("col.createdtime"), Type = EInput.Read, Order = "DESC" };

                        Meta birthYY = new Meta { Name = "BirthYY", DbName = "BirthYY", Title = Words("col.birthdate"), Type = EInput.Int, MinLength = 4, MaxLength = 4 };
                        Meta birthMM = new Meta { Name = "BirthMM", DbName = "BirthMM", Title = Words("col.birthdate"), Type = EInput.Int, MaxLength = 2 };
                        birthMM.AddListRef("MonthList");
                        Meta birthDD = new Meta { Name = "BirthDD", DbName = "BirthDD", Title = Words("col.birth"), Type = EInput.Int, MaxLength = 2 };
                        birthDD.AddListRef("DayList");

                        Meta memberYY = new Meta { Name = "MemberYY", DbName = "MemberYY", Title = Words("col.memberdate"), Type = EInput.Int, MinLength = 4, MaxLength = 4, Value = System.DateTime.Now.Year };
                        Meta memberMM = new Meta { Name = "MemberMM", DbName = "MemberMM", Title = Words("col.memberdate"), Type = EInput.Int, MaxLength = 2, Value = System.DateTime.Now.Month };
                        memberMM.AddListRef("MonthList");
                        Meta memberDD = new Meta { Name = "MemberDD", DbName = "MemberDD", Title = Words("col.memberdate"), Type = EInput.Int, MaxLength = 2, Value = System.DateTime.Now.Day };
                        memberDD.AddListRef("DayList");

                        Meta dharmaYY = new Meta { Name = "DharmaYY", DbName = "DharmaYY", Title = Words("col.dharmadate"), Type = EInput.Int, MinLength = 4, MaxLength = 4 };
                        Meta dharmaMM = new Meta { Name = "DharmaMM", DbName = "DharmaMM", Title = Words("col.dharmadate"), Type = EInput.Int, MaxLength = 2 };
                        dharmaMM.AddListRef("MonthList");
                        Meta dharmaDD = new Meta { Name = "DharmaDD", DbName = "DharmaDD", Title = Words("col.dharmadate"), Type = EInput.Int, MaxLength = 2 };
                        dharmaDD.AddListRef("DayList");

                        Meta emerRelation = new Meta { Name = "EmergencyRelation", DbName = "EmergencyRelation", Title = Words("col.emergency.relation"), Type = EInput.String, MaxLength = 32 };
                        Meta emerPerson = new Meta { Name = "EmergencyPerson", DbName = "EmergencyPerson", Title = Words("col.emergency.person"), Type = EInput.String, MaxLength = 128 };
                        Meta emerPhone = new Meta { Name = "EmergencyPhone", DbName = "EmergencyPhone", Title = Words("col.emergency.phone"), Type = EInput.String, MaxLength = 32 };
                        Meta emerCell = new Meta { Name = "EmergencyCell", DbName = "EmergencyCell", Title = Words("col.emergency.cell"), Type = EInput.String, MaxLength = 32 };

                        Meta branch = new Meta { Name = "BranchId", DbName = "BranchId", Title = Words("col.branch"), Type = EInput.Int, Required = true, Value = this.DB.User.Branch };
                        branch.AddListRef("BranchList");
                        Meta gender = new Meta { Name = "Gender", DbName = "Gender", Title = Words("col.gender"), Type = EInput.Int };
                        gender.AddListRef("GenderList");
                        Meta education = new Meta { Name = "Education", DbName = "Education", Title = Words("col.education"), Type = EInput.Int };
                        education.AddListRef("EducationList");
                        Meta nationality = new Meta { Name = "Nationality", DbName = "Nationality", Title = Words("col.nationality"), Type = EInput.Int };
                        nationality.AddListRef("CountryList");
                        Meta religion = new Meta { Name = "Religion", DbName = "Religion", Title = Words("col.religion"), Type = EInput.Int };
                        religion.AddListRef("ReligionList");

                        Meta motherLang = new Meta { Name = "MotherLang", DbName = "MotherLang", Title = Words("col.motherlang"), Type = EInput.Int };
                        motherLang.AddListRef("LanguageList");
                        Meta multiLang = new Meta { Name = "MultiLang", DbName = "UserId", Title = Words("col.multilang"), Type = EInput.Checkbox, Value = new { } };
                        multiLang.AddListRef("LanguageList", "Pub_User_Language", "LanguageId");

                        Meta hearUs = new Meta { Name = "HearUs", DbName = "UserId", Title = Words("col.hearus"), Type = EInput.Checkbox, Value = new { } };
                        hearUs.AddListRef("HearUsList", "Pub_User_HearUs", "HearUsId");
                        Meta symbol = new Meta { Name = "Symbol", DbName = "UserId", Title = Words("col.symbol"), Type = EInput.Checkbox, Value = new { } };
                        symbol.AddListRef("SymbolList", "Pub_User_Symbol", "SymbolId");


                        Meta email = new Meta { Name = "Email", DbName = "Email", Title = Words("col.email"), Type = EInput.Email, Required = true, Unique = true, MaxLength = 256 };
                        Meta phone = new Meta { Name = "Phone", DbName = "Phone", Title = Words("col.phone"), Type = EInput.String, MaxLength = 32 };
                        Meta cell = new Meta { Name = "Cell", DbName = "Cell", Title = Words("col.cell"), Type = EInput.String, MaxLength = 32 };
                        Meta address = new Meta { Name = "Address", DbName = "Address", Title = Words("col.address"), Type = EInput.String, MaxLength = 256 };
                        Meta city = new Meta { Name = "City", DbName = "City", Title = Words("col.city"), Type = EInput.String, MaxLength = 64 };
                        Meta state = new Meta { Name = "State", DbName = "State", Title = Words("col.province"), Type = EInput.Int };
                        state.AddListRef("StateList");
                        Meta country = new Meta { Name = "Country", DbName = "Country", Title = Words("col.country"), Type = EInput.Int, Value = 1 };
                        country.AddListRef("CountryList");
                        Meta postal = new Meta { Name = "Postal", DbName = "Postal", Title = Words("col.postal"), Type = EInput.String, MaxLength = 16 };


                        Member.AddMetas(id, memberId, firstName, lastName, firstNameLegal, lastNameLegl, dharmaName, displayName, certName, aliasname, occupation, memo)
                        .AddMetas(userName, password, gender, education, nationality, religion, motherLang, multiLang)
                        .AddMetas(medicalConcern, hearUsOther, symbolOther, multiLangOther, idNumber, email, phone, cell, branch, address, city, state, country, postal)
                        .AddMetas(birthYY, birthMM, birthDD, memberYY, memberMM, memberDD, dharmaYY, dharmaMM, dharmaDD)
                        .AddMetas(emerRelation, emerPerson, emerPhone, emerCell, hearUs, symbol, photo, active, loginTime, loginTotal, createdTime);

                        Member.Navi.IsActive = false;
                        Member.Navi.Order = "";
                        Member.Navi.By = "";
                        Member.AddRelation(new Relation(ERef.O2O, "Id", 0));
                        Member.GetUrl = "/Admin/api/ClassEvent/ReloadStudent"; //"/Admin/api/ClassEvent/ReloadPubUser";
                        Member.SaveUrl = "/Admin/api/ClassEvent/SaveStudent";
                        Member.AddQueryKV("Deleted", false).AddDeleteKV("LastUpdated", DateTime.Now.UTCSeconds()).AddUpdateKV("LastUpdated", DateTime.Now.UTCSeconds())
                            .AddInsertKV("Deleted", false).AddInsertKV("Active", true).AddInsertKV("BranchId", this.DB.User.Branch).AddInsertKV("CreatedTime", DateTime.Now.UTCSeconds());


                        Table PubUserId = new Table("PubUserId", "Pub_User_Id");
                        Meta uid = new Meta { Name = "Id", DbName = "Id", Title = Words("col.id"), IsKey = true };
                        Meta idType = new Meta { Name = "IdType", DbName = "IdType", Title = Words("col.idtype"), Required = true, Type = EInput.Int };
                        idType.AddListRef("IdTypeList");
                        Meta idno = new Meta { Name = "IdNumber", DbName = "IdNumber", Title = Words("col.idno"), Required = true, Type = EInput.String, Unique = true, MaxLength = 128 };
                        PubUserId.AddMetas(uid, idType, idno);
                        PubUserId.Navi.IsActive = false;
                        PubUserId.AddRelation(new Relation(ERef.O2M, "UserId", 0));
                        PubUserId.GetUrl = "/Admin/api/ClassEvent/ReloadStudentId";
                        PubUserId.SaveUrl = "/Admin/api/ClassEvent/SaveStudentId";
                        PubUserId.AddQueryKV("Deleted", false).AddQueryKV("Active", true).AddDeleteKV("LastUpdated", DateTime.Now.UTCSeconds())
                            .AddUpdateKV("LastUpdated", DateTime.Now.UTCSeconds())
                            .AddInsertKV("Deleted", false).AddInsertKV("Active", true).AddInsertKV("CreatedTime", DateTime.Now.UTCSeconds());

                        CollectionTable c11 = new CollectionTable("EducationList", "Education", true, "Id", "Title", "Detail", "", "DESC", "Sort");
                        Collection EducationList = new Collection(ECollectionType.Category, c11);
                        CollectionTable c12 = new CollectionTable("LanguageList", "Language", true, "Id", "Title", "Detail", "", "DESC", "Sort");
                        Collection LanguageList = new Collection(ECollectionType.Category, c12);
                        CollectionTable c13 = new CollectionTable("ReligionList", "Religion", true, "Id", "Title", "Detail", "", "DESC", "Sort");
                        Collection ReligionList = new Collection(ECollectionType.Category, c13);
                        CollectionTable c14 = new CollectionTable("HearUsList", "HearUs", true, "Id", "Title", "Detail", "", "DESC", "Sort");
                        Collection HearUsList = new Collection(ECollectionType.Category, c14);
                        CollectionTable c15 = new CollectionTable("SymbolList", "Symbol", true, "Id", "Title", "Detail", "", "DESC", "Sort");
                        Collection SymbolList = new Collection(ECollectionType.Category, c15);
                        CollectionTable c16 = new CollectionTable("IdTypeList", "IdType", true, "Id", "Title", "Detail", "", "DESC", "Sort");
                        Collection IdTypeList = new Collection(ECollectionType.Category, c16);
                        CollectionTable c17 = new CollectionTable("BranchList", "GBranch", true, "Id", "Title", "Detail", "", "DESC", "Sort");
                        Collection BranchList = new Collection(ECollectionType.Table, c17);
                        CollectionTable c18 = new CollectionTable("StateList", "GState", true, "Id", "Title", "Detail", "CountryId", "DESC", "Sort");
                        Collection StateList = new Collection(ECollectionType.Table, c18);
                        CollectionTable c19 = new CollectionTable("CountryList", "GCountry", true, "Id", "Title", "Detail", "", "DESC", "Sort");
                        Collection CountryList = new Collection(ECollectionType.Table, c19);
                        CollectionTable c20 = new CollectionTable("GenderList", "GenderList");
                        Collection GenderList = new Collection(ECollectionType.Common, c20);
                        CollectionTable c21 = new CollectionTable("MonthList", "MonthList");
                        Collection MonthList = new Collection(ECollectionType.Common, c21);
                        CollectionTable c22 = new CollectionTable("DayList", "DayList");
                        Collection DayList = new Collection(ECollectionType.Common, c22);


                        // Add Objects 
                        this.DB.AddTables(pubUser, classEnroll, Member, PubUserId)
                                .AddCollections(ClassList, MemberTypeList)
                                .AddCollections(EducationList, LanguageList, ReligionList, HearUsList, SymbolList, IdTypeList, BranchList, StateList, CountryList)
                                .AddCollections(GenderList, MonthList, DayList);
                    }
                    break;
            }
        }
    }
}