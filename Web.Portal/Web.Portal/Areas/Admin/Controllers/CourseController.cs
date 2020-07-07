using Microsoft.AspNetCore.Mvc;
using Library.V1.SQL;
using Library.V1.Common;
using Library.V1.Entity;

namespace Web.Portal.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CourseController : AdminBaseController
    {
        public CourseController(AppSetting appConfig) : base(appConfig) { }
        public IActionResult Agreement()
        {
            this.Init("M1010");
            return View();
        }
        public IActionResult CourseDefine()
        {
            this.Init("M1020");
            return View();
        }
        public IActionResult CourseAll()
        {
            this.Init("M1030");
            return View();
        }
        protected override void InitDatabase(string menuId)
        {

        }
    }
}
