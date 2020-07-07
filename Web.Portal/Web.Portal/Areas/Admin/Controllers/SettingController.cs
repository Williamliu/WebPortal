using Microsoft.AspNetCore.Mvc;
using Library.V1.SQL;
using Library.V1.Entity;
using Library.V1.Common;

namespace Web.Portal.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SettingController : AdminBaseController
    {
        public SettingController(AppSetting appConfig) : base(appConfig) { }
        public IActionResult Country()
        {
            this.Init("M8010");
            return View();
        }
        public IActionResult Province()
        {
            this.Init("M8020");
            return View();
        }
        public IActionResult Category()
        {
            this.Init("M8030");
            return View();
        }
        public IActionResult CategoryItem()
        {
            this.Init("M8040");
            return View();
        }
        public IActionResult Translation()
        {
            this.Init("M8050");
            return View();
        }
        public IActionResult Gallery()
        {
            this.Init("M8060");
            return View();
        }
        public IActionResult Settings()
        {
            this.Init("M8080");
            return View();
        }
        protected override void InitDatabase(string menuId)
        {
        }
    }
}