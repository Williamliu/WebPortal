using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Library.V1.Common;
using Library.V1.SQL;
using Library.V1.Entity;
using Web.Portal.Common;

namespace Web.Portal.WebApi.Controllers
{
    public abstract class BaseController : ControllerBase
    {
        protected AppSetting AppConfig { get; set; }
        protected Database DB;
        public BaseController(AppSetting appConfig)
        {
            this.AppConfig = appConfig;
        }
        #region Database
        protected abstract void InitDatabase(string menuId);
        protected void Init(string menuId)
        {
            //Step1: get language from queryString/Session/Cookie/Browser
            this.AppConfig.Lang = this.HttpContext.GetWebLang();

            //Step2: Init Global Database Context
            this.DB = new Database(this.AppConfig);

            //Step3: Init language dictionary
            LanguageHelper.InitWords(this.DB.DSQL);


            //Step4: Get AdminUser for DB User.
            //this.DB.User = this.HttpContext.GetPubUser(this.DB.DSQL, menuId);


            //Step5: Init Database Schema defined in Controller
            this.InitDatabase(menuId);
        }
        #endregion Database
    }
}