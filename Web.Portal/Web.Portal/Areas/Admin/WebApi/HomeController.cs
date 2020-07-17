using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

using Web.Portal.Common;
using Library.V1.Entity;
using Library.V1.Common;
using Library.V1.SQL;
namespace Web.Portal.Areas.Admin.WebApi
{
    [Route("/Admin/api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class HomeController : HomeBaseController
    {
        public HomeController(AppSetting appSetting) : base(appSetting) { }
        // GET: api/<controller>
        [HttpGet("init")]
        public IActionResult GetLoginUser()
        {
            this.Init("Login");
            this.DB.FillCollection("BranchList");
            return Ok(this.DB);
        }
        
        [HttpPost("Login")]
        public IActionResult PostLogin(JSTable gtb)
        {
            this.Init("Login");
            this.DB.ValidateTable(gtb);
            Table table = this.DB.Tables[gtb.Name];
            if (table.IndexFirst())
            {
                Row row = table.IndexFetch();
                if (row.HasError == false)
                {
                    string passValue = row.GetValue("Password").GetString();
                    string userValue = row.GetValue("LoginUser").GetString();

                    string query = "SELECT COUNT(Id) as CNT FROM Admin_User WHERE Active=1 AND Deleted=0 AND (UserName=@LoginUser OR Email=@LoginUser) AND Password=@Password";
                    IDictionary<string, object> ps = new Dictionary<string, object>();
                    ps.Add("LoginUser", userValue);
                    ps.Add("Password", passValue);

                    if (this.DB.DSQL.IsExisted(query, ps))
                    {
                        string jwtToken = CreateAuthToken(userValue, "admin");
                        HttpContext.SaveSession("adminSite_jwtToken", jwtToken);

                        Guid guid = Guid.NewGuid();
                        HttpContext.SaveSession("adminSite_Session", guid.ToString());

                        query = "UPDATE Admin_User SET LoginCount = 0, LoginTotal = LoginTotal + 1, LoginTime = @LoginTime WHERE Active=1 AND Deleted=0 AND (UserName=@LoginUser OR Email=@LoginUser)";
                        ps.Add("LoginTime", DateTime.Now.UTCSeconds());
                        this.DB.DSQL.ExecuteQuery(query, ps);

                        query = "SELECT Id FROM Admin_User WHERE Active=1 AND Deleted=0 AND (UserName=@LoginUser OR Email=@LoginUser) AND Password=@Password";
                        List<Dictionary<string, string>> rows = this.DB.DSQL.Query(query, ps);
                        int adminId = 0;
                        if (rows.Count > 0) adminId = rows[0]["Id"].GetInt() ?? 0;

                        query = "UPDATE Admin_Session SET Deleted=1, LastUpdated=@LastTime WHERE Deleted=0 AND AdminId=@AdminId";
                        IDictionary<string, object> sess_ps = new Dictionary<string, object>();
                        sess_ps.Add("LastTime", DateTime.Now.UTCSeconds());
                        sess_ps.Add("AdminId", adminId);
                        this.DB.DSQL.ExecuteQuery(query, sess_ps);
                        query = "INSERT Admin_Session(AdminId, Session, Deleted, CreatedTime) Values(@AdminId, @Session, 0, @LastTime)";
                        sess_ps.Add("Session", guid.ToString());
                        this.DB.DSQL.ExecuteQuery(query, sess_ps);
                    }
                    else
                    {
                        query = "SELECT COUNT(Id) as CNT FROM Admin_User WHERE Deleted=0 AND (UserName=@LoginUser OR Email=@LoginUser)";
                        if (this.DB.DSQL.IsExisted(query, ps))
                        {
                            query = "UPDATE Admin_User SET LoginCount = LoginCount + 1, Active = IIF(LoginCount>=4, 0, 1), LoginTime = @LoginTime WHERE Active=1 AND Deleted=0 AND (UserName=@LoginUser OR Email=@LoginUser)";
                            ps.Add("LoginTime", DateTime.Now.UTCSeconds());
                            this.DB.DSQL.ExecuteQuery(query, ps);
                            query = "SELECT Top 1 Active FROM Admin_User WHERE Deleted=0 AND (UserName=@LoginUser OR Email=@LoginUser)";
                            int Active = this.DB.DSQL.ExecuteScalar(query, ps);
                            if (Active == 1)
                            {
                                row.Error.Append(ErrorCode.LoginFail, Words("login.failed"));
                            }
                            else
                            {
                                row.Error.Append(ErrorCode.LoginFail, Words("account.lock"));
                            }
                        }
                        else
                        {
                            row.Error.Append(ErrorCode.LoginFail, Words("login.failed"));
                        }
                    }
                }
            }
            return Ok(table);
        }
        
        [HttpPost("Register")]
        public IActionResult PostRegister(JSTable gtb)
        {
            this.Init("Login");
            return Ok(this.DB.SaveTable(gtb));
        }

        [HttpPost("Logout")]
        public IActionResult Logout()
        {
            this.HttpContext.DeleteSession("adminSite_jwtToken");
            this.HttpContext.DeleteSession("adminSite_Session");
            return Ok("Logout");
        }

        private string CreateAuthToken(string email, string role)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("infosecurity@shaolinworld.org");
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                            {
                                new Claim(ClaimTypes.Name, email)
                            }),
                Expires = System.DateTime.UtcNow.AddHours(16),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var stringToken = tokenHandler.WriteToken(token);
            return stringToken;
        }
        

        protected override void InitDatabase(string menuId)
        {
            switch(menuId)
            {
                case "Login":
                    {
                        Table UserLogin = new Table("UserLogin", "Admin_User", Words("admin.user"));
                        Meta loginEmail = new Meta { Name = "LoginUser", DbName = "Email", Title = Words("col.email"), Type = EInput.String, Required = true, MaxLength = 256 };
                        Meta loginPassword = new Meta { Name = "Password", DbName = "Password", Title = Words("col.password"), Type = EInput.Password, Required = true, MinLength = 6, MaxLength = 32 };
                        Meta memo = new Meta { Name = "Memo", DbName = "Phone", Title = Words("col.phone"), Type = EInput.String };
                        UserLogin.AddMetas(loginEmail, loginPassword, memo);
                        UserLogin.SaveUrl = "/Admin/api/Home/Login";
                        Table UserRegister = new Table("UserRegister", "Admin_User", Words("admin.user"));
                        Meta id = new Meta { Name = "Id", DbName = "Id", Title = Words("col.id"), IsKey = true };
                        Meta firstName = new Meta { Name = "FirstName", DbName = "FirstName", Title = Words("col.firstname"), Required = true, Type = EInput.String, MaxLength = 64, Value = "" };
                        Meta lastName = new Meta { Name = "LastName", DbName = "LastName", Title = Words("col.lastname"), Required = true, Type = EInput.String, MaxLength = 64 };
                        Meta userName = new Meta { Name = "UserName", DbName = "UserName", Title = Words("col.username"), Required = true, Unique = true, Type = EInput.String, MaxLength = 32 };
                        Meta email = new Meta { Name = "Email", DbName = "Email", Title = Words("col.email"), Type = EInput.Email, Required = true, Unique = true, MaxLength = 256 };
                        Meta phone = new Meta { Name = "Phone", DbName = "Phone", Title = Words("col.phone"), Type = EInput.String, MaxLength = 32 };
                        Meta password = new Meta { Name = "Password", DbName = "Password", Title = Words("col.password"), Description = Words("confirm.password"), Type = EInput.Passpair, Required = true, MinLength = 6, MaxLength = 32 };
                        Meta branch = new Meta { Name = "BranchId", DbName = "BranchId", Title = Words("col.branch"), Type = EInput.Int, Required = true };
                        branch.AddListRef("BranchList");
                        UserRegister.AddMetas(id, firstName, lastName, userName, email, phone, password, branch);
                        UserRegister.AddQueryKV("Id", -1).AddQueryKV("Deleted", false).AddQueryKV("Active", true)
                        .AddUpdateKV("LastUpdated", DateTime.Now.UTCSeconds())
                        .AddInsertKV("Deleted", false).AddInsertKV("Active", true).AddInsertKV("CreatedTime", DateTime.Now.UTCSeconds());

                        UserRegister.SaveUrl = "/Admin/api/Home/Register";

                        CollectionTable c1 = new CollectionTable("BranchList", "GBranch", true, "Id", "Title", "Detail");
                        Collection BranchList = new Collection(ECollectionType.Table, c1);

                        this.DB.AddTable(UserLogin).AddTable(UserRegister).AddCollection(BranchList);
                    }
                    break;
            }
 
        }

    }
}