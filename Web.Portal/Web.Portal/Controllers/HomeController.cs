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
            return View();
        }

        protected override void InitDatabase(string menuId)
        {
        }
    }
}
