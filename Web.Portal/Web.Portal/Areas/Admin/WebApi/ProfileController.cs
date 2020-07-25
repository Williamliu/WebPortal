using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Library.V1.SQL;
using Library.V1.Common;
using Library.V1.Entity;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Web.Portal.Areas.Admin.WebApi
{
    [Route("/Admin/api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ProfileController : AdminBaseController
    {
        public ProfileController(AppSetting appSetting) : base(appSetting) { }
        [HttpGet("InitUser")]
        public IActionResult GetMyAccount()
        {
            this.Init("P01");
            this.DB.FillAll();
            return Ok(this.DB);
        }

        [HttpPost("UpdateUser")]
        public IActionResult SaveMyAccount(JSTable jsTable)
        {
            this.Init("P01");
            return Ok(this.DB.SaveTable(jsTable));
        }
        [HttpPost("ValidateUser")]
        public IActionResult ValidateUser(JSTable jsTable)
        {
            this.Init("P01");
            return Ok(this.DB.ValidateTable(jsTable));
        }
        [HttpGet("InitPass")]
        public IActionResult GetPass()
        {
            this.Init("P02");
            return Ok(this.DB);
        }
        [HttpPost("ChangePass")]
        public IActionResult PostPass(JSTable jsTable)
        {
            this.Init("P02");
            this.DB.ValidateTable(jsTable);
            Table table = this.DB.Tables[jsTable.Name];
            if (table.IndexFirst())
            {
                Row row = table.IndexFetch();
                if (row.Error.HasError == false)
                {
                    string oldValue = row.GetValue("OldPass").GetString();
                    string newValue = row.GetValue("NewPass").GetString();

                    string query = "SELECT COUNT(Id) as CNT FROM Admin_User WHERE Active=1 AND Deleted=0 AND Id=@AdminId AND Password=@Password";
                    IDictionary<string, object> ps = new Dictionary<string, object>();
                    ps.Add("AdminId", this.DB.User.Id);
                    ps.Add("Password", oldValue);
                    if (this.DB.DSQL.IsExist(query, ps))
                    {
                        query = "UPDATE Admin_User SET Password=@NewPass WHERE Active=1 AND Deleted=0 AND Id=@AdminId AND Password=@Password";
                        ps.Add("NewPass", newValue);
                        this.DB.DSQL.ExecuteQuery(query, ps);
                    }
                    else
                    {
                        row.Error.Append(ErrorCode.LoginFail, Words("password.change.failed"));
                    }
                }
            }
            return Ok(table);
        }

        protected override void InitDatabase(string menuId)
        {
            switch (menuId)
            {
                case "P01":
                    Table AdUser = new Table("AdminUser", "Admin_User", Words("admin.user"));
                    Meta id = new Meta { Name = "Id", DbName = "Id", Title = Words("col.id"), IsKey = true };
                    Meta firstName = new Meta { Name = "FirstName", DbName = "FirstName", Title = Words("col.firstname"), Required = true, Type = EInput.String, MaxLength = 64 };
                    Meta lastName = new Meta { Name = "LastName", DbName = "LastName", Title = Words("col.lastname"), Required = true, Type = EInput.String, MaxLength = 64 };
                    Meta userName = new Meta { Name = "UserName", DbName = "UserName", Title = Words("col.username"), Required = true, Unique = true, Type = EInput.String, MaxLength = 32 };
                    Meta email = new Meta { Name = "Email", DbName = "Email", Title = Words("col.email"), Type = EInput.Email, Required = true, Unique = true, MaxLength = 256 };
                    Meta phone = new Meta { Name = "Phone", DbName = "Phone", Title = Words("col.phone"), Type = EInput.String, MaxLength = 32 };
                    Meta branch = new Meta { Name = "BranchId", DbName = "BranchId", Title = Words("col.branch"), Type = EInput.Int, Required = true };
                    branch.AddListRef("BranchList");
                    Meta address = new Meta { Name = "Address", DbName = "Address", Title = Words("col.address"), Type = EInput.String, MaxLength = 256 };
                    Meta city = new Meta { Name = "City", DbName = "City", Title = Words("col.city"), Type = EInput.String, MaxLength = 64 };
                    Meta state = new Meta { Name = "State", DbName = "State", Title = Words("col.state"), Type = EInput.Int };
                    state.AddListRef("StateList");
                    Meta country = new Meta { Name = "Country", DbName = "Country", Title = Words("col.country"), Type = EInput.Int };
                    country.AddListRef("CountryList");
                    Meta postal = new Meta { Name = "Postal", DbName = "Postal", Title = Words("col.postal"), Type = EInput.String, MaxLength = 16 };

                    Meta loginTime = new Meta { Name = "LastLogin", DbName = "LoginTime", Title = Words("col.lastlogin"), Type = EInput.Int };
                    Meta loginTotal = new Meta { Name = "LoginTotal", DbName = "LoginTotal", Title = Words("col.logintotal"), Type = EInput.Int };
                    Meta createdTime = new Meta { Name = "CreatedTime", DbName = "CreatedTime", Title = Words("col.createdtime"), Type = EInput.Int };

                    AdUser.AddMetas(id, firstName, lastName, userName, email, phone, branch, address, city, state, country, postal, loginTime, loginTotal, createdTime);
                    AdUser.AddQueryKV("Id", this.DB.User.Id).AddQueryKV("Deleted", false).AddQueryKV("Active", true).AddUpdateKV("LastUpdated", DateTime.Now.UTCSeconds());
                    AdUser.SaveUrl = "/Admin/api/Profile/UpdateUser";
                    AdUser.ValidateUrl = "/Admin/api/Profile/ValidateUser";

                    CollectionTable c1 = new CollectionTable("BranchList", "GBranch", true, "Id", "Title", "Detail", "", "DESC", "Sort");
                    Collection BranchList = new Collection(ECollectionType.Table, c1);
                    CollectionTable c2 = new CollectionTable("StateList", "GState", true, "Id", "Title", "Detail", "CountryId", "DESC", "Sort");
                    Collection StateList = new Collection(ECollectionType.Table, c2);
                    CollectionTable c3 = new CollectionTable("CountryList", "GCountry", true, "Id", "Title", "Detail", "", "DESC", "Sort");

                    Collection CountryList = new Collection(ECollectionType.Table, c3);
                    this.DB.AddTable(AdUser).AddCollections(BranchList, StateList, CountryList);
                    break;
                case "P02":
                    Table UserPass = new Table("UserPassword", "Admin_User", Words("admin.user"));
                    Meta oldPass = new Meta { Name = "OldPass", DbName = "Password", Title = Words("col.oldpass"), Required = true, Type = EInput.Password, MinLength = 6, MaxLength = 12 };
                    Meta newPass = new Meta { Name = "NewPass", DbName = "NewPass", Title = Words("col.newpass"), Description = Words("confirm.password"), Required = true, Type = EInput.Passpair, MinLength = 6, MaxLength = 12 };
                    UserPass.AddMetas(oldPass, newPass);
                    UserPass.SaveUrl = "/Admin/api/Profile/ChangePass";
                    this.DB.AddTable(UserPass);
                    break;
                case "P03":
                    break;
            }
        }
    }
}