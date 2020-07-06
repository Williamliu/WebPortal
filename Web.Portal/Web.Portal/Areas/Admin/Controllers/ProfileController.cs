using Microsoft.AspNetCore.Mvc;
using Library.V1.Common;

namespace Web.Portal.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProfileController : AdminBaseController
    {
        public ProfileController(AppSetting appConfig) : base(appConfig) { }
        public IActionResult MyAccount()
        {
            this.Init("P01");
            return View();
        }

        public IActionResult ResetPassword()
        {
            this.Init("P02");
            return View();
        }
        public IActionResult MyMessage()
        {
            this.Init("P03");
            return View();
        }
        protected override void InitDatabase(string menuId)
        {
        }
    }
}