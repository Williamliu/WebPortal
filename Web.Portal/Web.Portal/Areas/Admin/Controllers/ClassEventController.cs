using Microsoft.AspNetCore.Mvc;
using Library.V1.SQL;
using Library.V1.Common;
using Library.V1.Entity;

namespace Web.Portal.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ClassEventController : AdminBaseController
    {
        public ClassEventController(AppSetting appConfig) : base(appConfig) { }

        public IActionResult EditClass()
        {
            this.Init("M2010");
            return View();
        }
        public IActionResult CalendarClass()
        {
            this.Init("M2020");
            return View();
        }
        public IActionResult ClassStudent()
        {
            this.Init("M2030");
            return View();
        }
        protected override void InitDatabase(string menuId)
        {

        }
    }
}
