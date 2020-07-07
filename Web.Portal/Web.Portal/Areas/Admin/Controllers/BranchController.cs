using Microsoft.AspNetCore.Mvc;
using Library.V1.Common;
using Library.V1.Entity;

namespace Web.Portal.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BranchController : AdminBaseController
    {
        public BranchController(AppSetting appConfig) : base(appConfig) { }

        public IActionResult CompanyBranch()
        {
            this.Init("M4010");
            return View();
        }
        public IActionResult CompanySite()
        {
            this.Init("M4020");
            return View();
        }
        protected override void InitDatabase(string menuId)
        {
        }
    }
}
