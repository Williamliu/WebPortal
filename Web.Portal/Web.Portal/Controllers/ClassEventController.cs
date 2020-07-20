using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Library.V1.Common;
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

        protected override void InitDatabase(string menuId)
        {
        }
    }
}