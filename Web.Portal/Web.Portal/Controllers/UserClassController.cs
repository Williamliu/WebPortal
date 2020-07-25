using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Library.V1.Common;
using Microsoft.AspNetCore.Mvc;
using Web.Portal.Common;

namespace Web.Portal.Controllers
{
    public class UserClassController : PrivateBaseController
    {
        public UserClassController(AppSetting appConfig) : base(appConfig) { }
        public IActionResult Upcoming()
        {
            this.Init("Y10");
            return View();
        }

        public IActionResult History()
        {
            this.Init("Y20");
            return View();
        }
        protected override void InitDatabase(string menuId)
        {
        }
    }
}