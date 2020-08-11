using Microsoft.AspNetCore.Mvc;
using Library.V1.Common;

namespace Web.Portal.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MemberController : AdminBaseController
    {
        public MemberController(AppSetting appConfig) : base(appConfig) { }
        public IActionResult Register()
        {
            this.Init("M3010");
            return View();
        }
        public IActionResult UserList()
        {
            this.Init("M3020");
            return View();
        }
        public IActionResult UserDetail()
        {
            this.Init("M3030");
            return View();
        }
        public IActionResult Donate()
        {
            this.Init("M3090");
            return View();
        }
        protected override void InitDatabase(string menuId)
        {
        }
    }
}