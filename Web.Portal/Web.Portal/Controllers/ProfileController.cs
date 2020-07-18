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
            this.Init("P01");
            return View();
        }

        public IActionResult MyMessage()
        {
            this.Init("P02");
            return View();
        }
        public IActionResult SignOut()
        {
            Init("P03");
            this.HttpContext.DeleteSession("pubSite_jwtToken");
            this.HttpContext.DeleteSession("pubSite_Session");
            return RedirectToAction("Index", "/Home");
        }

        protected override void InitDatabase(string menuId)
        {
        }
    }
}