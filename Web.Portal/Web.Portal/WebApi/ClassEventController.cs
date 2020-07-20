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
    public class ClassEventController : ApiBaseController
    {
        public ClassEventController(AppSetting appSetting) : base(appSetting) { }
        [HttpGet("InitClassList")]
        public IActionResult InitPubUser()
        {
            this.Init("M15");
            this.DB.FillAll();
            return Ok(this.DB);
        }
        [HttpPost("ReloadClassList")]
        public IActionResult ReloadClassList(JSTable gtb)
        {
            this.Init("M15");
            return Ok(this.DB.ReloadTable(gtb));
        }
        [HttpPost("ReloadClassDetail")]
        public IActionResult ReloadClassDetail(JSTable jsTable)
        {
            this.Init("M15");
            return Ok(this.DB.ReloadTable(jsTable));
        }

        protected override void InitDatabase(string menuId)
        {
            switch (menuId)
            {
                case "M15":
                    {
                        Table classList = new Table("ClassList", "VW_ActiveClass_List", Words("class.list"));
                        Meta id = new Meta { Name = "Id", DbName = "Id", Title = Words("col.id"), IsKey = true };
                        Meta className = new Meta { Name = "ClassName", DbName = "ClassName", Title = Words("col.class.name"), Type = EInput.String };
                        Meta classTitle = new Meta { Name = "ClassTitle", DbName = "ClassTitle", Title = Words("class.title"), IsLang=true, Type = EInput.String };
                        Meta classNotes = new Meta { Name = "ClassNotes", DbName = "ClassNotes", Title = Words("col.notes"), IsLang = true, Type = EInput.String };
                        Meta siteTitle = new Meta { Name = "SiteTitle", DbName = "SiteTitle", Title = Words("col.center"), IsLang = true, Type = EInput.String };
                        Meta startDate = new Meta { Name = "StartDate", DbName = "StartDate", Title = Words("start.date"), Description= Words("col.date"), Type = EInput.Date };
                        Meta endDate = new Meta { Name = "EndDate", DbName = "EndDate", Title = Words("end.date"), Type = EInput.Date };
                        Meta email = new Meta { Name = "Email", DbName = "Email", Title = Words("col.email"), IsLang = true, Type = EInput.String };
                        Meta phone = new Meta { Name = "Phone", DbName = "Phone", Title = Words("col.contact"), IsLang = true, Type = EInput.String };
                        Meta address = new Meta { Name = "Address", DbName = "Address", Title = Words("col.address"), IsLang = true, Type = EInput.String };
                        Meta photo = new Meta { Name = "Photo", DbName = "Id", Title = Words("col.photo"), Description = "ClassEvent|Medium", Type = EInput.ImageUrl };

                        classList.AddMetas(id, classTitle, siteTitle, startDate, endDate, email, phone, address, photo);
                        classList.Navi.IsActive = true;
                        classList.Navi.Order = "ASC";
                        classList.Navi.By = "StartDate";
                        classList.Navi.PageSize = 12;
                        classList.GetUrl = "/api/ClassEvent/ReloadClassList";

                        Filter f1 = new Filter() { Name = "CountryFilter", DbName = "Country", Title = Words("col.country"), Type = EFilter.Int, Compare = ECompare.Equal };
                        f1.AddListRef("CountryList");
                        Filter f2 = new Filter() { Name = "SiteFilter", DbName = "SiteId", Title = Words("col.site"), Type = EFilter.Int, Compare = ECompare.Equal };
                        f2.AddListRef("SiteList");
                        classList.AddFilters(f1, f2);


                        Table ClassDetail = new Table("ClassDetail", "Class_Detail", Words("class.detail"));
                        Meta did = new Meta { Name = "Id", DbName = "Id", Title = "ID", IsKey = true };
                        Meta dtitle = new Meta { Name = "Title", DbName = "Title", Title = Words("class.name"), Type = EInput.String, IsLang=true, MaxLength = 64 };
                        Meta dclassDate = new Meta { Name = "ClassDate", DbName = "ClassDate", Title = Words("class.date"), Required = true, Order = "ASC", Type = EInput.Date };
                        Meta dstartTime = new Meta { Name = "StartTime", DbName = "StartTime", Title = Words("start.time"), Required = true, Type = EInput.Time };
                        Meta dendTime = new Meta { Name = "EndTime", DbName = "EndTime", Title = Words("end.time"), Required = true, Type = EInput.Time };

                        ClassDetail.AddMetas(did, dtitle, dclassDate, dstartTime, dendTime);
                        ClassDetail.Navi.IsActive = true;
                        ClassDetail.AddRelation(new Relation(ERef.O2M, "ClassId", -1));
                        ClassDetail.Navi.Order = "ASC";
                        ClassDetail.Navi.By = "ClassDate";
                        ClassDetail.Navi.PageSize = 8;
                        ClassDetail.GetUrl = "/api/ClassEvent/ReloadClassDetail";
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