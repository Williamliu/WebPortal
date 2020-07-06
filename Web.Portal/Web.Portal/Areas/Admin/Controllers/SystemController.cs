using Microsoft.AspNetCore.Mvc;
using Library.V1.SQL;
using Library.V1.Entity;
using Library.V1.Common;

namespace Web.Portal.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SystemController : AdminBaseController
    {
        public SystemController(AppSetting appConfig) : base(appConfig) { }
        public IActionResult AdminManager()
        {
            this.Init("M9010");
            return View();
        }
        public IActionResult AdminMenu()
        {
            this.Init("M9030");
            return View();
        }
        public IActionResult AdminRole()
        {
            this.Init("M9040");
            return View();
        }
        protected override void InitDatabase(string menuId)
        {
        }
    }
}