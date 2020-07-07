using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

using Library.V1.Common;
using Library.V1.Entity;
using Library.V1.SQL;

namespace Web.Portal.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : BaseController
    {
        public HomeController(AppSetting appConfig) : base(appConfig) { }

        [HttpGet("Index")]
        public IActionResult Index()
        {
            Init("Home");
            return Ok();
        }

        protected override void InitDatabase(string menuId)
        {
        }
    }

}