using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Library.V1.Common;
using Microsoft.AspNetCore.Mvc;
using Web.Portal.Common;

namespace Web.Portal.Controllers
{
    public class ProfileController : PrivateBaseController
    {
        public ProfileController(AppSetting appConfig) : base(appConfig) { }
        public IActionResult MyAccount()
        {
            this.Init("P10");
            return View();
        }
        public IActionResult ResetPassword()
        {
            this.Init("P20");
            return View();
        }

        public IActionResult MyMessage()
        {
            this.Init("P30");
            return View();
        }
        public IActionResult SignOut()
        {
            Init("P90");
            this.HttpContext.DeleteSession("pubSite_jwtToken");
            this.HttpContext.DeleteSession("pubSite_Session");
            return Redirect("/Home/Index");
        }

        protected override void InitDatabase(string menuId)
        {
        }
    }
}