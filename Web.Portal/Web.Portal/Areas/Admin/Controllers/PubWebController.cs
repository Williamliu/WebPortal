using Microsoft.AspNetCore.Mvc;
using Library.V1.SQL;
using Library.V1.Entity;
using Library.V1.Common;

namespace Web.Portal.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PubWebController : AdminBaseController
    {
        public PubWebController(AppSetting appConfig) : base(appConfig) { }
        public IActionResult WebMenu()
        {
            this.Init("M7010");
            return View();
        }
        public IActionResult UserRole()
        {
            this.Init("M7020");
            return View();
        }
        public IActionResult WebContent()
        {
            this.Init("M7050");
            return View();
        }
        public IActionResult AccessLog()
        {
            this.Init("M7080");
            return View();
        }
        public IActionResult AccessStats()
        {
            this.Init("M7090");
            return View();
        }
        protected override void InitDatabase(string menuId)
        {
        }
    }
}