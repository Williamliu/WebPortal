using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Library.V1.Common;
using Library.V1.Entity;
using Microsoft.AspNetCore.Mvc;
using Web.Portal.Common;

namespace Web.Portal.Controllers
{
    public class ClassEventController: PublicBaseController
    {
        public ClassEventController(AppSetting appConfig) : base(appConfig) { }
        public IActionResult ClassList()
        {
            this.Init("M20");
            return View();
        }
        public IActionResult ClassCalendar()
        {
            this.Init("M30");
            return View();
        }
        public IActionResult ClassPayment(int? Id)
        {
            this.Init("ClassPayment");
            if (Id.HasValue == false || (Id ?? 0) == 0)
            {
                // Missing Class Id , go back to class list
                return Redirect("/ClassEvent/ClassList");
            }
            else
            {
                if (this.DB.User.Id > 0)
                {
                    this.DB.User.AddItem("ClassId", Id ?? 0);

                    this.DB.Tables["ClassList"].Filters["ClassId"].Value1 = Id ?? 0;
                    this.DB.FillAll();
                    return View(this.DB.Tables["ClassList"]);
                }
                else
                    // User not login, go to login page
                    return Redirect($"/Home/SignIn?url=/ClassEvent/ClassPayment/{Id??0}");

            }
        }
        
        public IActionResult ClassAgree(int? Id)
        {
            this.Init("ClassAgree");
            if (Id.HasValue == false || (Id??0) == 0)
            {
                return Redirect("/ClassEvent/ClassList");
            }
            else
            {
                this.DB.Tables["ClassAgree"].RefKey = Id.Value;
                this.DB.FillAll();
                if (this.DB.Tables["ClassAgree"].Rows.Count > 0)
                {
                    this.DB.User.AddItem("ClassId", Id ?? 0);
                    return View(this.DB.Tables["ClassAgree"]);
                }
                else
                    if (this.DB.User.Id > 0)
                        return Redirect($"/ClassEvent/ClassPayment/{Id}");
                    else
                        return Redirect($"/Home/SignIn?url=/ClassEvent/ClassPayment/{Id}");
            }
        }

        protected override void InitDatabase(string menuId)
        {
            switch(menuId)
            {
                case "ClassAgree":
                    {
                        Table classAgree = new Table("ClassAgree", "VW_Class_Agreement", Words("class.agree"));
                        Meta id = new Meta { Name = "Id", DbName = "Id", Title = Words("col.id"), IsKey = true };
                        Meta agreeTitle = new Meta { Name = "AgreeTitle", DbName = "AgreeTitle", Title = Words("agree.title"), IsLang=true, Type = EInput.String };
                        Meta agreement = new Meta { Name = "Agreement", DbName = "Agreement", Title = Words("col.agreement"),Type = EInput.String };
                        classAgree.AddMetas(id, agreeTitle, agreement);
                        classAgree.AddRelation(new Relation(ERef.O2O, "Id", -1));


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

                        this.DB.AddTables(classAgree, classEnroll);
                    }
                    break;
                case "ClassPayment":
                    {
                        Table classList = new Table("ClassList", "USP_Class_UserPayment", Words("class.list"), "", ESource.StoreProcedure);
                        Meta id = new Meta { Name = "Id", DbName = "Id", Title = Words("col.id"), IsKey = true };
                        Meta isfree = new Meta { Name = "IsFree", DbName = "IsFree", Title = Words("col.isfree"), Type = EInput.Bool };
                        Meta amount = new Meta { Name = "FeeAmount", DbName = "FeeAmount", Title = Words("col.feeamount"), Type = EInput.Float };
                        Meta paidamount = new Meta { Name = "PaidAmount", DbName = "PaidAmount", Title = Words("col.paid.amount"), Type = EInput.Float };
                        Meta oweamount = new Meta { Name = "OweAmount", DbName = "OweAmount", Title = Words("col.owe.amount"), Type = EInput.Float };
                        Meta currency = new Meta { Name = "Currency", DbName = "Currency", Title = Words("col.currency"), Type = EInput.String };
                        Meta userId = new Meta { Name = "UserId", DbName = "UserId", Title = Words("col.userid"), Type = EInput.Int };
                        Meta isEnroll = new Meta { Name = "IsEnroll", DbName = "IsEnroll", Title = Words("col.isenroll"), Type = EInput.Bool };

                        Filter f1 = new Filter() { Name = "ClassId", SqlParam = "ClassId", Title = Words("col.class.id"), Type = EFilter.Int, Compare = ECompare.Equal };
                        Filter f2 = new Filter() { Name = "UserId", SqlParam = "UserId", Title = Words("col.userid"), Type = EFilter.Int, Compare = ECompare.Equal, Value1 = this.DB.User.Id };

                        classList.AddMetas(id, isfree, amount, paidamount, oweamount, currency, userId, isEnroll);
                        classList.AddFilters(f1, f2);

                        this.DB.AddTable(classList);
                    }
                    break;
            }
        }
    }
}