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
            this.Init("M20");
            this.DB.FillAll();
            return Ok(this.DB);
        }
        [HttpPost("ReloadClassList")]
        public IActionResult ReloadClassList(JSTable gtb)
        {
            this.Init("M20");
            return Ok(this.DB.ReloadTable(gtb));
        }
        [HttpPost("ReloadClassDetail")]
        public IActionResult ReloadClassDetail(JSTable jsTable)
        {
            this.Init("M20");
            return Ok(this.DB.ReloadTable(jsTable));
        }
        [HttpGet("InitClassCalendar")]
        public IActionResult InitCalendarClass()
        {
            this.Init("M30");
            this.DB.FillAll();
            return Ok(this.DB);
        }
        [HttpPost("LoadCalendar")]
        public IActionResult LoadCalendar(Calendar calendar)
        {
            this.Init("M30");
            if (calendar.IsActive)
            {
                int countryId = calendar.Data["CountryId"].GetInt() ?? -1;
                int siteId = calendar.Data["SiteId"].GetInt() ?? -1;
                this.DB.Tables["ClassCalendar"].Filters["CountryFilter"].Value1 = countryId;
                this.DB.Tables["ClassCalendar"].Filters["SiteFilter"].Value1 = siteId;
                this.DB.Tables["ClassCalendar"].Filters["classDateFilter"].Value1 = calendar.Start.YMD();
                this.DB.Tables["ClassCalendar"].Filters["classDateFilter"].Value2 = calendar.End.YMD();
                this.DB.Tables["ClassCalendar"].FillData();
                if (this.DB.Tables["ClassCalendar"].Rows.Count > 0)
                {
                    foreach (Row row in this.DB.Tables["ClassCalendar"].Rows)
                    {
                        Event evt = new Event
                        {
                            Id = row.GetValue("ClassId").GetInt() ?? 0,
                            Date = row.GetValue("ClassDate").GetDateTime() ?? DateTime.MinValue,
                            From = row.GetValue("StartTime").GetTimeSpan() ?? TimeSpan.MinValue,
                            To = row.GetValue("EndTime").GetTimeSpan() ?? TimeSpan.MinValue,
                            Title = row.GetValue("DateTitle").GetString(),
                            Detail = "",
                            Status = row.GetValue("DateStatus").GetBool() ?? false,
                            State = row.GetValue("ClassStatus").GetInt() ?? 0,
                            Subject = row.GetValue("Subject").GetString(),
                            DateNo = row.GetValue("DateNo").GetInt() ?? 0,
                            Color = row.GetValue("Color").GetString()
                        };
                        calendar.Add(evt);
                    }
                }
            }
            return Ok(calendar);
        }
        [HttpPost("ReloadClassCalendar")]
        public IActionResult ReloadClassCalendar(JSTable jsTable)
        {
            this.Init("M30");
            return Ok(this.DB.ReloadTable(jsTable));
        }
        [HttpPost("ReloadClassList1")]
        public IActionResult ReloadClassList1(JSTable gtb)
        {
            this.Init("M30");
            return Ok(this.DB.ReloadTable(gtb));
        }
        [HttpPost("ReloadClassDetail1")]
        public IActionResult ReloadClassDetail1(JSTable jsTable)
        {
            this.Init("M30");
            return Ok(this.DB.ReloadTable(jsTable));
        }

        [HttpGet("InitClassPayment/{id}")]
        public IActionResult InitClassPayment(int Id)
        {
            this.Init("ClassPayment");
            this.DB.Tables["ClassList"].Filters["ClassId"].Value1 = Id;
            this.DB.Tables["ClassDetail"].RefKey = Id;
            this.DB.FillAll();
            return Ok(this.DB);
        }
        [HttpPost("SaveClassEnroll")]
        public IActionResult SaveClassEnroll(JSTable jsTable)
        {
            this.Init("ClassPayment");

            if(jsTable.Rows.Count>0)
            {
                try
                {
                    int ClassId = jsTable.Rows[0].GetValue("ClassId").GetInt() ?? 0;
                    int UserId = jsTable.RefKey;
                    Dictionary<string, string> row = this.DB.DSQL.QuerySingle("SELECT FirstName, LastName, Email FROM Pub_User WHERE Id=@UserId", new Dictionary<string, object> { { "UserId", UserId } });
                    string fname = row.GetValue("FirstName");
                    string lname = row.GetValue("LastName");
                    string email = row.GetValue("Email");

                    row = this.DB.DSQL.QuerySingle($"SELECT {this.DB.DSQL.LangSmartColumn("ClassTitle")} AS Title, {this.DB.DSQL.LangSmartColumn("Email")} as Email, {this.DB.DSQL.LangSmartColumn("SiteTitle")} as SiteTitle FROM VW_ActiveClass_List WHERE Id=@ClassId", new Dictionary<string, object> { { "ClassId", ClassId } });
                    string classname = row.GetValue("Title");
                    string sitename = row.GetValue("SiteTitle");
                    string siteemail = row.GetValue("Email");

                    MMEmail myemail = new MMEmail("mail.shaolinworld.org", "info@shaolinworld.org", "SL2020$");
                    myemail.Port = 26;
                    myemail.enableSSL = false;
                    myemail.addFrom("info@shaolinworld.org");
                    myemail.addTo(email);
                    myemail.addReply("info@shaolinworld.org");
                    if (string.IsNullOrWhiteSpace(siteemail) ==false) myemail.addBCC(siteemail, "SysAdmin");
                    myemail.Subject = Words("email.enroll.success.subject");// "New Student Enrolled";
                    myemail.Content = "<html><body>";
                    myemail.Content += string.Format(Words("email.enroll.success.content"), fname, lname, classname); // $"Dear {fname} {lname}, <br><br>Welcome to {classname}<br><br>We are looking forward to see you soon.<br><br>Shaolin";
                    myemail.Content += sitename;
                    myemail.Content += "</body></html>";
                    myemail.SendAsync();
                }
                catch { }
            }

            return Ok(this.DB.SaveTable(jsTable));
        }
        [HttpPost("SaveClassUserPayment")]
        public IActionResult SaveClassUserPayment(JSTable jsTable)
        {
            this.Init("ClassPayment");

            if (jsTable.Rows.Count > 0)
            {
                try
                {
                    int ClassId = jsTable.Rows[0].GetValue("ClassId").GetInt() ?? 0;
                    int UserId = jsTable.Rows[0].GetValue("UserId").GetInt() ?? 0;
                    string payer = jsTable.Rows[0].GetValue("Payer").GetString();
                    
                    string paiddate = (jsTable.Rows[0].GetValue("PaidDate").GetLong() ?? 0).IntDate().YMDHMS();
                    string paidinvoice = jsTable.Rows[0].GetValue("PaidInvoice").GetString();
                    string paidamount = jsTable.Rows[0].GetValue("PaidAmount").GetString();
                    string paidcurrency = jsTable.Rows[0].GetValue("Currency").GetString();


                    Dictionary<string, string> row = this.DB.DSQL.QuerySingle("SELECT FirstName, LastName, Email FROM Pub_User WHERE Id=@UserId", new Dictionary<string, object> { { "UserId", UserId } });
                    string fname = row.GetValue("FirstName");
                    string lname = row.GetValue("LastName");
                    string email = row.GetValue("Email");

                    row = this.DB.DSQL.QuerySingle($"SELECT {this.DB.DSQL.LangSmartColumn("ClassTitle")} AS Title, {this.DB.DSQL.LangSmartColumn("Email")} as Email, {this.DB.DSQL.LangSmartColumn("SiteTitle")} as SiteTitle FROM VW_ActiveClass_List WHERE Id=@ClassId", new Dictionary<string, object> { { "ClassId", ClassId } });
                    string classname = row.GetValue("Title");
                    string sitename = row.GetValue("SiteTitle");
                    string siteemail = row.GetValue("Email");

                    MMEmail myemail = new MMEmail("mail.shaolinworld.org", "info@shaolinworld.org", "SL2020$");
                    myemail.Port = 26;
                    myemail.enableSSL = false;
                    myemail.addFrom("info@shaolinworld.org");
                    myemail.addTo(email);
                    myemail.addReply("info@shaolinworld.org");
                    if (string.IsNullOrWhiteSpace(siteemail) == false) myemail.addBCC(siteemail, "SysAdmin");
                    myemail.Subject = Words("email.enroll.success.subject");
                    myemail.Content = "<html><body>";
                    myemail.Content += string.Format(Words("email.enroll.success.content"), fname, lname, classname); 
                    myemail.Content += string.Format(Words("email.enroll.payment.content"), payer, paiddate, paidinvoice, paidamount, paidcurrency); 
                    myemail.Content += sitename;
                    myemail.Content += "</body></html>";
                    myemail.SendAsync();
                }
                catch { }
            }

            return Ok(this.DB.SaveTable(jsTable));
        }
        [HttpPost("ReloadPaymentClassDetail1")]
        public IActionResult ReloadPaymentClassDetail1(JSTable jsTable)
        {
            this.Init("ClassPayment");
            return Ok(this.DB.ReloadTable(jsTable));
        }


        [HttpPost("SaveDonate")]
        public IActionResult SaveDonate(Dictionary<string, object> data)
        {
            this.Init("M50");
            data.Add("CreatedTime", DateTime.Now.UTCSeconds());
            string query = "INSERT INTO Donation(UserId, SiteId, FullName, Email, Notes, Payer, PaidDate, PaidAmount, Currency, PaidMethod, PaidInvoice, PaidStatus, IsSuccess, TrackNumber, TrackMessage, CreatedTime) ";
            query += "Values(@UserId, @SiteId, @FullName, @Email, @Notes, @Payer, @PaidDate, @PaidAmount, @Currency, @PaidMethod, @PaidInvoice, @PaidStatus, @IsSuccess, @TrackNumber, @TrackMessage, @CreatedTime)";
            this.DB.DSQL.ExecuteQuery(query, data);


            if( string.IsNullOrWhiteSpace(data.GetValue("Email"))==false && ValidateHelper.IsMatch("Email", data.GetValue("Email")) )
            {
                string paidDate = (data.GetValue("PaidDate").GetLong() ?? 0).IntDate().YMDHMS();

                MMEmail myemail = new MMEmail("mail.shaolinworld.org", "info@shaolinworld.org", "SL2020$");
                myemail.Port = 26;
                myemail.enableSSL = false;
                myemail.addFrom("info@shaolinworld.org");
                myemail.addTo(data.GetValue("Email"));
                myemail.addReply("info@shaolinworld.org");
                myemail.addBCC("info@shaolinworld.org");
                myemail.Subject = Words("donate.success");// "New Student Enrolled";
                myemail.Content = "<html><body>";
                myemail.Content += string.Format(Words("email.donate.success.content"), data.GetValue("FullName")); // $"Dear {fname} {lname}, <br><br>Welcome to {classname}<br><br>We are looking forward to see you soon.<br><br>Shaolin";
                myemail.Content += string.Format(Words("email.enroll.payment.content"), data.GetValue("Payer"), paidDate, data.GetValue("PaidInvoice"), data.GetValue("PaidAmount"), data.GetValue("Currency"));
                myemail.Content += "</body></html>";
                myemail.SendAsync();
            }



            if (this.DB.DSQL.Error.HasError)
            {
                return Conflict("Save Fail");
            }
            else
                return Ok("Save Ok");
        }

        protected override void InitDatabase(string menuId)
        {
            switch (menuId)
            {
                case "M20":
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

                        classList.AddMetas(id, className, classTitle, siteTitle, classNotes, startDate, endDate, email, phone, address, photo);
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
                        Meta dtitle = new Meta { Name = "Title", DbName = "Title", Title = Words("class.content"), Type = EInput.String, IsLang=true, MaxLength = 64 };
                        Meta dclassDate = new Meta { Name = "ClassDate", DbName = "ClassDate", Title = Words("class.date"), Order = "ASC", Type = EInput.Date };
                        Meta dstartTime = new Meta { Name = "StartTime", DbName = "StartTime", Title = Words("start.time"), Type = EInput.Time };
                        Meta dendTime = new Meta { Name = "EndTime", DbName = "EndTime", Title = Words("end.time"), Type = EInput.Time };

                        ClassDetail.AddMetas(did, dtitle, dclassDate, dstartTime, dendTime);
                        ClassDetail.Navi.IsActive = false;
                        ClassDetail.AddRelation(new Relation(ERef.O2M, "ClassId", -1));
                        ClassDetail.Navi.Order = "ASC";
                        ClassDetail.Navi.By = "ClassDate";
                        ClassDetail.GetUrl = "/api/ClassEvent/ReloadClassDetail";
                        ClassDetail.AddQueryKV("Deleted", false).AddQueryKV("Active", true);

                        CollectionTable c1 = new CollectionTable("CountryList", "GCountry", true, "Id", "Title", "Detail", "", "DESC", "Sort");
                        Collection CountryList = new Collection(ECollectionType.Table, c1);
                        CollectionTable c2 = new CollectionTable("SiteList", "GSite", true, "Id", "Title", "Detail", "Country", "DESC", "Sort");
                        Collection SiteList = new Collection(ECollectionType.Table, c2);


                        this.DB.AddTables(classList, ClassDetail).AddCollections(CountryList, SiteList);
                    }
                    break;
                case "M30":
                    {
                        Table ClassCalendar = new Table("ClassCalendar", "VW_Class_PubCalendar", Words("class.event"));
                        Meta dateId = new Meta { Name = "ClassId", DbName = "ClassId", Title = "ID", IsKey = true };
                        Meta classDate = new Meta { Name = "ClassDate", DbName = "ClassDate", Title = Words("col.classdate"), Type = EInput.Date };
                        Meta startTime = new Meta { Name = "StartTime", DbName = "StartTime", Title = Words("start.time"), Type = EInput.Time };
                        Meta endTime = new Meta { Name = "EndTime", DbName = "EndTime", Title = Words("end.time"), Type = EInput.Time };
                        Meta title = new Meta { Name = "DateTitle", DbName = "Date_Title", Title = Words("col.title"), IsLang = true, Type = EInput.String };
                        Meta status = new Meta { Name = "DateStatus", DbName = "DateStatus", Title = Words("status.active"), Type = EInput.Bool };
                        Meta ccstatus = new Meta { Name = "ClassStatus", DbName = "ClassStatus", Title = Words("status.active"), Type = EInput.Int };
                        ccstatus.AddListRef("StatusList");
                        Meta subject = new Meta { Name = "Subject", DbName = "ClassName", Title = Words("col.class.name"), Type = EInput.String };
                        Meta color = new Meta { Name = "Color", DbName = "Color", Title = Words("col.color"), Type = EInput.String };
                        Meta dateNo = new Meta { Name = "DateNo", DbName = "DateNo", Title = Words("col.date.number"), Type = EInput.Int };
                        ClassCalendar.AddMetas(dateId, classDate, startTime, endTime, title, status, ccstatus, subject, color, dateNo);

                        int def_country = 1;
                        int def_site = 1;
                        Filter f1 = new Filter() { Name = "CountryFilter", DbName = "Country", Title = Words("col.country"), Required=true, Type = EFilter.Int, Compare = ECompare.Equal, Value1=def_country };
                        f1.AddListRef("CountryList");
                        Filter f2 = new Filter() { Name = "SiteFilter", DbName = "SiteId", Title = Words("col.site"), Required=true, Type = EFilter.Int, Compare = ECompare.Equal, Value1=def_site };
                        f2.AddListRef("SiteList");
                        Filter f3 = new Filter() { Name = "classDateFilter", DbName = "ClassDate", Title = Words("class.date"), Type = EFilter.Date, Compare = ECompare.Range };
                        ClassCalendar.AddFilters(f1, f2, f3);
                        ClassCalendar.Navi.IsActive = false;
                        ClassCalendar.Navi.Order = "ASC,ASC,ASC";
                        ClassCalendar.Navi.By = "ClassDate,StartTime,EndTime";
                        ClassCalendar.GetUrl = "/api/ClassEvent/ReloadClassCalendar";


                        Table classList = new Table("ClassList", "VW_ActiveClass_List", Words("class.list"));
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
                        Meta isFree = new Meta { Name = "IsFree", DbName = "IsFree", Title = Words("col.isfree"), Type = EInput.Bool };
                        Meta feeAmount = new Meta { Name = "FeeAmount", DbName = "FeeAmount", Title = Words("col.feeamount"), Type = EInput.Float };
                        Meta currency = new Meta { Name = "Currency", DbName = "Currency", Title = Words("col.currency"), Type = EInput.String };

                        classList.AddMetas(id, className, classTitle, siteTitle, classNotes, startDate, endDate, email, phone, address, photo)
                                    .AddMetas(isFree, feeAmount, currency);
                        classList.Navi.IsActive = false;
                        classList.AddRelation(new Relation(ERef.O2O, "Id", -1));
                        classList.GetUrl = "/api/ClassEvent/ReloadClassList1";


                        Table ClassDetail = new Table("ClassDetail", "Class_Detail", Words("class.detail"));
                        Meta did = new Meta { Name = "Id", DbName = "Id", Title = "ID", IsKey = true };
                        Meta dtitle = new Meta { Name = "Title", DbName = "Title", Title = Words("class.content"), Type = EInput.String, IsLang = true, MaxLength = 64 };
                        Meta dclassDate = new Meta { Name = "ClassDate", DbName = "ClassDate", Title = Words("class.date"), Required = true, Order = "ASC", Type = EInput.Date };
                        Meta dstartTime = new Meta { Name = "StartTime", DbName = "StartTime", Title = Words("start.time"), Required = true, Type = EInput.Time };
                        Meta dendTime = new Meta { Name = "EndTime", DbName = "EndTime", Title = Words("end.time"), Required = true, Type = EInput.Time };

                        ClassDetail.AddMetas(did, dtitle, dclassDate, dstartTime, dendTime);
                        ClassDetail.Navi.IsActive = false;
                        ClassDetail.AddRelation(new Relation(ERef.O2M, "ClassId", -1));
                        ClassDetail.Navi.Order = "ASC";
                        ClassDetail.Navi.By = "ClassDate";
                        ClassDetail.Navi.PageSize = 10;
                        ClassDetail.GetUrl = "/api/ClassEvent/ReloadClassDetail1";
                        ClassDetail.AddQueryKV("Deleted", false).AddQueryKV("Active", true);

                        CollectionTable c1 = new CollectionTable("CountryList", "GCountry", true, "Id", "Title", "Detail", "", "DESC", "Sort");
                        Collection CountryList = new Collection(ECollectionType.Table, c1);
                        CollectionTable c2 = new CollectionTable("SiteList", "GSite", true, "Id", "Title", "Detail", "Country", "DESC", "Sort");
                        Collection SiteList = new Collection(ECollectionType.Table, c2);


                        this.DB.AddCollections(CountryList, SiteList).AddTables(ClassCalendar, classList, ClassDetail);
                    }
                    break;
                case "ClassPayment":
                    {
                        Table classList = new Table("ClassList", "USP_Class_UserPayment", Words("class.list"), "", ESource.StoreProcedure);
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
                        Meta isfree = new Meta { Name = "IsFree", DbName = "IsFree", Title = Words("col.isfree"), Type = EInput.Bool };
                        Meta amount = new Meta { Name = "FeeAmount", DbName = "FeeAmount", Title = Words("col.feeamount"), Type = EInput.Float };
                        Meta oweamount = new Meta { Name = "OweAmount", DbName = "OweAmount", Title = Words("col.owe.amount"), Type = EInput.Float };
                        Meta currency = new Meta { Name = "Currency", DbName = "Currency", Title = Words("col.currency"), Type = EInput.String };
                        Meta discountText = new Meta { Name = "DiscountText", DbName = "DiscountText", Title = Words("col.discount.text"), IsLang = true, Type = EInput.String };
                        Meta discountAmt = new Meta { Name = "DiscountAmount", DbName = "DiscountAmount", Title = Words("col.discount.amount"), Type = EInput.Float };
                        Meta discount = new Meta { Name = "Discount", DbName = "Discount", Title = Words("col.discount"), Type = EInput.Float };

                        Filter f1 = new Filter() { Name = "ClassId", SqlParam = "ClassId", Title = Words("col.class.id"), Type = EFilter.Int, Compare = ECompare.Equal };
                        Filter f2 = new Filter() { Name = "UserId", SqlParam = "UserId", Title = Words("col.userid"), Type = EFilter.Int, Compare = ECompare.Equal, Value1 = this.DB.User.Id};

                        classList.AddMetas(id, className, classTitle, siteTitle, classNotes, startDate, endDate, email, phone, address, photo, discountText, discountAmt, discount)
                                 .AddMetas(isfree, amount, oweamount, currency);
                        classList.AddFilters(f1, f2);

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
                        ClassDetail.AddQueryKV("Deleted", false).AddQueryKV("Active", true);
                        ClassDetail.GetUrl = "/api/ClassEvent/ReloadPaymentClassDetail1";


                        Table classEnroll = new Table("ClassEnroll", "Class_Enroll", Words("class.enroll"));
                        // Meta Data
                        Meta eid = new Meta { Name = "Id", DbName = "Id", Title = Words("col.id"), IsKey = true, Type = EInput.Read };
                        Meta eclassId = new Meta { Name = "ClassId", DbName = "ClassId", Title = Words("col.classid") };
                        Meta ememberId = new Meta { Name = "UserId", DbName = "UserId", Title = Words("col.userid") };

                        Meta egroup = new Meta { Name = "Grp", DbName = "Grp", Title = Words("col.grp"), Type = EInput.String, MaxLength = 16, Order = "ASC" };
                        Meta eisPaid = new Meta { Name = "IsPaid", DbName = "IsPaid", Title = Words("col.ispaid"), Description = Words("col.ispaid.yesno"), Type = EInput.Bool, Order = "DESC" };
                        Meta epaidDate = new Meta { Name = "PaidDate", DbName = "PaidDate", Title = Words("col.paiddate"), Type = EInput.Read, Sync = true, Order = "ASC" };
                        Meta epaidAmount = new Meta { Name = "PaidAmount", DbName = "PaidAmount", Title = Words("col.paidinfo"), Type = EInput.Float, MaxLength = 18 };
                        Meta epaidInvoice = new Meta { Name = "PaidInvoice", DbName = "PaidInvoice", Title = Words("col.paidinvoice"), Type = EInput.String, MaxLength = 32, Order = "ASC" };
                        classEnroll.AddMetas(eid, eclassId, ememberId, egroup, eisPaid, epaidDate, epaidAmount, epaidInvoice);
                        // Navi 
                        classEnroll.Navi.IsActive = false;
                        classEnroll.AddRelation(new Relation(ERef.O2O, "UserId", this.DB.User.Id));
                        classEnroll.SaveUrl = "/api/ClassEvent/SaveClassEnroll";

                        classEnroll.AddInsertKV("Deleted", false)
                                   .AddInsertKV("Active", true)
                                   .AddInsertKV("CreatedTime", DateTime.Now.UTCSeconds());



                        Table Payment = new Table("Payment", "Class_UserPayment", Words("class.user.payment"));
                        // Meta Data
                        Meta pid = new Meta { Name = "Id", DbName = "Id", Title = Words("col.id"), IsKey = true, Type = EInput.Read };
                        Meta pclassId = new Meta { Name = "ClassId", DbName = "ClassId", Title = Words("col.classid"), Required=true, Type=EInput.Int };
                        Meta puserId = new Meta { Name = "UserId", DbName = "UserId", Title = Words("col.userid"), Required = true, Type = EInput.Int };

                        Meta ppayer = new Meta { Name = "Payer", DbName = "Payer", Title = Words("col.payer"), Type = EInput.String, MaxLength = 128 };
                        Meta ppaydate = new Meta { Name = "PaidDate", DbName = "PaidDate", Title = Words("col.paid.date"), Type = EInput.Long };
                        Meta ppaidAmount = new Meta { Name = "PaidAmount", DbName = "PaidAmount", Title = Words("col.paid.amount"), Type = EInput.Float, MaxLength = 18 };
                        Meta ppaidInvoice = new Meta { Name = "PaidInvoice", DbName = "PaidInvoice", Title = Words("col.paid,invoice"), Type = EInput.String, MaxLength = 64 };
                        Meta paidcurrency = new Meta { Name = "Currency", DbName = "Currency", Title = Words("col.paid,currency"), Type = EInput.String, MaxLength = 64 };
                        Meta ppaidMethod = new Meta { Name = "PaidMethod", DbName = "PaidMethod", Title = Words("col.paid.method"), Type = EInput.String, MaxLength = 64 };
                        Meta ppaidStatus = new Meta { Name = "PaidStatus", DbName = "PaidStatus", Title = Words("col.paid.status"), Type = EInput.String, MaxLength = 64 };
                        Meta pisSuccess = new Meta { Name = "IsSuccess", DbName = "IsSuccess", Title = Words("col.paid.is.success"), Type = EInput.Bool };
                        Meta ptrackNumber = new Meta { Name = "TrackNumber", DbName = "TrackNumber", Title = Words("col.track.number"), Type = EInput.String, MaxLength = 256 };
                        Meta ptrackMessage = new Meta { Name = "TrackMessage", DbName = "TrackMessage", Title = Words("col.track.message"), Type = EInput.String };
                        Payment.AddMetas(pid, pclassId, puserId, ppayer, ppaydate, ppaidAmount, paidcurrency, ppaidInvoice, ppaidMethod, ppaidStatus, pisSuccess, ptrackNumber, ptrackMessage);
                        // Navi 
                        Payment.Navi.IsActive = false;
                        Payment.AddRelation(new Relation(ERef.O2O, "UserId", -1));
                        Payment.SaveUrl = "/api/ClassEvent/SaveClassUserPayment";

                        Payment.AddInsertKV("CreatedTime", DateTime.Now.UTCSeconds());


                        this.DB.AddTables(classList, ClassDetail, classEnroll, Payment);

                    }
                    break;
            }
        }
    }
}