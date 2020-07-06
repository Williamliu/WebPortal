using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Library.V1.Common;
using Library.V1.SQL;
using Library.V1.Entity;

namespace Web.Portal.Controllers
{
    public class HomeController : PubBaseController
    {
        public HomeController(AppSetting appConfig) : base(appConfig) {}

        public IActionResult Index()
        {
            Init("Index");
            this.DB.User.Rights.Add("view", true);
            this.DB.User.Rights.Add("save", true);

            Table table = new Table("Test", "Test");
            Meta c1 = new Meta { Name = "FirstName", DbName = "FirstName", IsLang = true, Required = true, MaxLength = 256 };
            Meta c2 = new Meta { Name = "LastName", DbName = "LastName", IsLang = true, Required = true, MaxLength = 256 };
            table.AddMetas(c1, c2);
            Filter f1 = new Filter { Name = "SearchName", DbName = "FirstName,LastName", Title = "Search Name", Type = EFilter.String, Compare = ECompare.Like };
            table.AddFilter(f1);
            ViewBag.AppConf = this.AppConfig;
            var uu = this.DB.User;
            return View(this.DB.User);
        }

        public IActionResult Privacy()
        {
            Init("Index");
            this.DB.User.Rights.Add("detail", true);
            this.DB.User.Rights.Add("print", true);
            var uu = this.DB.User;
            return Ok(uu);
        }
        [HttpPost]
        public IActionResult PostTable(Table table)
        {
            var tt = table;
            return Ok(table);
        }

        protected override void InitDatabase(string menuId)
        {
        }
    }
}
