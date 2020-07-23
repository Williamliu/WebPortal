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
            this.Init("M15");
            return View();
        }
        public IActionResult ClassCalendar()
        {
            this.Init("M16");
            return View();
        }
        public IActionResult ClassPayment(int? Id)
        {
            this.Init("ClassPayment");
            if (Id.HasValue == false || (Id ?? 0) == 0)
            {
                return Redirect("/ClassEvent/ClassList");
            }
            else
            {
                if (this.DB.User.Id > 0)
                {
                    ViewBag.ClassId = Id ?? 0;
                    List<Dictionary<string, string>> rows = this.DB.DSQL.Query("SELECT Currency, FeeAmount FROM VW_ActiveClass_List WHERE Id=@Id", new Dictionary<string, object> { { "Id", Id } });
                    ViewBag.Currency = "";
                    if (rows.Count > 0)
                    {
                        ViewBag.Currency = rows[0]["Currency"].GetString();
                        ViewBag.Amount = rows[0]["FeeAmount"].GetFloat()??0;
                    }
                    return View();
                }
                else
                    return Redirect($"/Home/SignIn?url=/ClassEvent/ClassPayment/{Id}");

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
                    ViewBag.ClassId = Id ?? 0;
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
                        this.DB.AddTable(classAgree);
                    }
                    break;
            }
        }
    }
}