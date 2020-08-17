using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Library.V1.SQL;
using Library.V1.Common;
using Library.V1.Entity;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Web.Portal.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class UserClassController : ApiBaseController
    {
        public UserClassController(AppSetting appSetting) : base(appSetting) { }
        [HttpGet("InitUpcoming")]
        public IActionResult InitUpcoming()
        {
            this.Init("Y10");
            this.DB.FillAll();
            return Ok(this.DB);
        }
        [HttpPost("ReloadUpcoming")]
        public IActionResult ReloadUpcoming(JSTable gtb)
        {
            this.Init("Y10");
            return Ok(this.DB.ReloadTable(gtb));
        }
        [HttpPost("ReloadClassDetail")]
        public IActionResult ReloadClassDetail(JSTable jsTable)
        {
            this.Init("Y10");
            return Ok(this.DB.ReloadTable(jsTable));
        }
        [HttpGet("InitHistory")]
        public IActionResult InitHistory()
        {
            this.Init("Y20");
            this.DB.FillAll();
            return Ok(this.DB);
        }
        [HttpPost("ReloadHistory")]
        public IActionResult ReloadHistory(JSTable gtb)
        {
            this.Init("Y20");
            return Ok(this.DB.ReloadTable(gtb));
        }

        [HttpPost("ReloadClassDetail1")]
        public IActionResult ReloadClassDetail1(JSTable jsTable)
        {
            this.Init("Y20");
            return Ok(this.DB.ReloadTable(jsTable));
        }

        protected override void InitDatabase(string menuId)
        {
            switch (menuId)
            {
                case "Y10":
                    {
                        Table classList = new Table("ClassList", "VW_UserClass_Upcoming", Words("class.list"));
                        Meta id = new Meta { Name = "Id", DbName = "Id", Title = Words("col.id"), IsKey = true };
                        Meta className = new Meta { Name = "ClassName", DbName = "ClassName", Title = Words("col.class.name"), Type = EInput.String };
                        Meta classTitle = new Meta { Name = "ClassTitle", DbName = "ClassTitle", Title = Words("class.title"), IsLang = true, Type = EInput.String };
                        Meta classNotes = new Meta { Name = "ClassNotes", DbName = "ClassNotes", Title = Words("col.notes"), IsLang = true, Type = EInput.String };
                        Meta siteTitle = new Meta { Name = "SiteTitle", DbName = "SiteTitle", Title = Words("col.center"), IsLang = true, Type = EInput.String };
                        Meta startDate = new Meta { Name = "StartDate", DbName = "StartDate", Title = Words("start.date"), Description = Words("col.date"), Type = EInput.Date };
                        Meta endDate = new Meta { Name = "EndDate", DbName = "EndDate", Title = Words("end.date"), Type = EInput.Date };
                        Meta email = new Meta { Name = "Email", DbName = "Email", Title = Words("col.email"), IsLang = true, Type = EInput.String };
                        Meta phone = new Meta { Name = "Phone", DbName = "Phone", Title = Words("col.contact"), IsLang = true, Type = EInput.String };
                        Meta address = new Meta { Name = "Address", DbName = "Address", Title = Words("col.address"), IsLang = true, Type = EInput.String };
                        Meta photo = new Meta { Name = "Photo", DbName = "Id", Title = Words("col.photo"), Description = "ClassEvent|Medium", Type = EInput.ImageUrl };

                        classList.AddMetas(id, className, classTitle, siteTitle, classNotes, startDate, endDate, email, phone, address, photo);
                        classList.Navi.IsActive = false;
                        classList.Navi.Order = "ASC";
                        classList.Navi.By = "StartDate";
                        classList.GetUrl = "/api/UserClass/ReloadUpcoming";

                        Filter f1 = new Filter() { Name = "FilterUserId", DbName = "UserId", Title = Words("col.userid"), Type = EFilter.Int, Compare = ECompare.Equal, Value1=this.DB.User.Id };
                        classList.AddFilters(f1);


                        Table ClassDetail = new Table("ClassDetail", "Class_Detail", Words("class.detail"));
                        Meta did = new Meta { Name = "Id", DbName = "Id", Title = "ID", IsKey = true };
                        Meta dtitle = new Meta { Name = "Title", DbName = "Title", Title = Words("class.content"), Type = EInput.String, IsLang = true, MaxLength = 64 };
                        Meta dclassDate = new Meta { Name = "ClassDate", DbName = "ClassDate", Title = Words("class.date"), Order = "ASC", Type = EInput.Date };
                        Meta dstartTime = new Meta { Name = "StartTime", DbName = "StartTime", Title = Words("start.time"), Type = EInput.Time };
                        Meta dendTime = new Meta { Name = "EndTime", DbName = "EndTime", Title = Words("end.time"), Type = EInput.Time };

                        ClassDetail.AddMetas(did, dtitle, dclassDate, dstartTime, dendTime);
                        ClassDetail.Navi.IsActive = false;
                        ClassDetail.AddRelation(new Relation(ERef.O2M, "ClassId", -1));
                        ClassDetail.Navi.Order = "ASC";
                        ClassDetail.Navi.By = "ClassDate";
                        ClassDetail.GetUrl = "/api/UserClass/ReloadClassDetail";
                        ClassDetail.AddQueryKV("Deleted", false).AddQueryKV("Active", true);

                        CollectionTable c1 = new CollectionTable("CountryList", "GCountry", true, "Id", "Title", "Detail", "", "DESC", "Sort");
                        Collection CountryList = new Collection(ECollectionType.Table, c1);
                        CollectionTable c2 = new CollectionTable("SiteList", "GSite", true, "Id", "Title", "Detail", "Country", "DESC", "Sort");
                        Collection SiteList = new Collection(ECollectionType.Table, c2);


                        this.DB.AddTables(classList, ClassDetail).AddCollections(CountryList, SiteList);

                    }
                    break;
                case "Y20":
                    {
                        Table classList = new Table("ClassList", "VW_UserClass_History", Words("class.list"));
                        Meta id = new Meta { Name = "Id", DbName = "Id", Title = Words("col.id"), IsKey = true };
                        Meta className = new Meta { Name = "ClassName", DbName = "ClassName", Title = Words("col.class.name"), Type = EInput.String };
                        Meta classTitle = new Meta { Name = "ClassTitle", DbName = "ClassTitle", Title = Words("class.title"), IsLang = true, Type = EInput.String };
                        Meta classNotes = new Meta { Name = "ClassNotes", DbName = "ClassNotes", Title = Words("col.notes"), IsLang = true, Type = EInput.String };
                        Meta siteTitle = new Meta { Name = "SiteTitle", DbName = "SiteTitle", Title = Words("col.center"), IsLang = true, Type = EInput.String };
                        Meta startDate = new Meta { Name = "StartDate", DbName = "StartDate", Title = Words("start.date"), Description = Words("col.date"), Type = EInput.Date };
                        Meta endDate = new Meta { Name = "EndDate", DbName = "EndDate", Title = Words("end.date"), Type = EInput.Date };
                        Meta email = new Meta { Name = "Email", DbName = "Email", Title = Words("col.email"), IsLang = true, Type = EInput.String };
                        Meta phone = new Meta { Name = "Phone", DbName = "Phone", Title = Words("col.contact"), IsLang = true, Type = EInput.String };
                        Meta address = new Meta { Name = "Address", DbName = "Address", Title = Words("col.address"), IsLang = true, Type = EInput.String };
                        Meta photo = new Meta { Name = "Photo", DbName = "Id", Title = Words("col.photo"), Description = "ClassEvent|Medium", Type = EInput.ImageUrl };

                        classList.AddMetas(id, className, classTitle, siteTitle, classNotes, startDate, endDate, email, phone, address, photo);
                        classList.Navi.IsActive = true;
                        classList.Navi.Order = "DESC";
                        classList.Navi.By = "StartDate";
                        classList.Navi.PageSize = 12;
                        classList.GetUrl = "/api/UserClass/ReloadHistory";

                        Filter f1 = new Filter() { Name = "FilterUserId", DbName = "UserId", Title = Words("col.userid"), Type = EFilter.Int, Compare = ECompare.Equal, Value1 = this.DB.User.Id };
                        classList.AddFilters(f1);


                        Table ClassDetail = new Table("ClassDetail", "Class_Detail", Words("class.detail"));
                        Meta did = new Meta { Name = "Id", DbName = "Id", Title = "ID", IsKey = true };
                        Meta dtitle = new Meta { Name = "Title", DbName = "Title", Title = Words("class.content"), Type = EInput.String, IsLang = true, MaxLength = 64 };
                        Meta dclassDate = new Meta { Name = "ClassDate", DbName = "ClassDate", Title = Words("class.date"), Order = "ASC", Type = EInput.Date };
                        Meta dstartTime = new Meta { Name = "StartTime", DbName = "StartTime", Title = Words("start.time"), Type = EInput.Time };
                        Meta dendTime = new Meta { Name = "EndTime", DbName = "EndTime", Title = Words("end.time"), Type = EInput.Time };

                        ClassDetail.AddMetas(did, dtitle, dclassDate, dstartTime, dendTime);
                        ClassDetail.Navi.IsActive = false;
                        ClassDetail.AddRelation(new Relation(ERef.O2M, "ClassId", -1));
                        ClassDetail.Navi.Order = "ASC";
                        ClassDetail.Navi.By = "ClassDate";
                        ClassDetail.GetUrl = "/api/UserClass/ReloadClassDetail1";
                        ClassDetail.AddQueryKV("Deleted", false).AddQueryKV("Active", true);

                        CollectionTable c1 = new CollectionTable("CountryList", "GCountry", true, "Id", "Title", "Detail", "", "DESC", "Sort");
                        Collection CountryList = new Collection(ECollectionType.Table, c1);
                        CollectionTable c2 = new CollectionTable("SiteList", "GSite", true, "Id", "Title", "Detail", "Country", "DESC", "Sort");
                        Collection SiteList = new Collection(ECollectionType.Table, c2);


                        this.DB.AddTables(classList, ClassDetail).AddCollections(CountryList, SiteList);
                    }
                    break;
            }
        }
    }
}