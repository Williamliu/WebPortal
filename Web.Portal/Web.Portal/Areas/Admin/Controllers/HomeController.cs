using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Library.V1.SQL;
using Library.V1.Common;
using Library.V1.Entity;

namespace Web.Portal.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : HomeBaseController
    {
        public HomeController(AppSetting appSetting) : base(appSetting) { }
        public IActionResult Index()
        {
            this.Init("Login");
            return View();
        }
        public IActionResult Backup()
        {
            this.Init("Login");
            return View();
        }
        public IActionResult Error()
        {
            this.Init("Login");
            IExceptionHandlerPathFeature exceptionHandlerPathFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            if (exceptionHandlerPathFeature?.Error is InvalidOperationException)
            {
                if (exceptionHandlerPathFeature.Path.StartsWith("/Admin/api"))
                {
                    Error err = new Error(ErrorCode.Authorization, LanguageHelper.Words("invalid.authorization"), "/Admin/Home/Index");
                    return Json(new { error = err });
                }
                else
                    return RedirectPermanent("/Admin/Home/Index");

            }
            return View(exceptionHandlerPathFeature);
        }
        protected override void InitDatabase(string menuId)
        {

        }
    }
}