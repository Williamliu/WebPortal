using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Library.V1.Common;
using Microsoft.AspNetCore.Mvc;

namespace Web.Portal.Controllers
{
    public class ProfileController : PrivateBaseController
    {
        public ProfileController(AppSetting appConfig) : base(appConfig) { }
        public IActionResult MyAccount()
        {
            this.Init("P01");
            return View();
        }

        public IActionResult ResetPassword()
        {
            this.Init("P02");
            return View();
        }
        public IActionResult MyMessage()
        {
            this.Init("P03");
            return View();
        }
        protected override void InitDatabase(string menuId)
        {
        }
    }
}