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
using Web.Portal.Common;

namespace Web.Portal.Controllers
{
    public class HomeController : PublicBaseController
    {
        public HomeController(AppSetting appConfig) : base(appConfig) {}

        public IActionResult Index()
        {
            Init("M10");
            this.GetWebContent("M10");
            return View();
        }
        public IActionResult SignIn()
        {
            Init("SignIn");
            return View();
        }
        protected override void InitDatabase(string menuId)
        {
        }
    }
}
