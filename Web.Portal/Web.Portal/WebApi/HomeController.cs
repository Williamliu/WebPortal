using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

using Library.V1.Common;
using Library.V1.Entity;
using Library.V1.SQL;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Web.Portal.Common;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace Web.Portal.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ApiBaseController
    {
        public HomeController(AppSetting appConfig) : base(appConfig) { }

        [HttpGet("Index")]
        public IActionResult Index()
        {
            Init("Home");
            return Ok();
        }
        [HttpGet("InitSignIn")]
        public IActionResult InitSignIn()
        {
            Init("SignIn");
            this.DB.FillAll();
            return Ok(this.DB);
        }
        [HttpPost("Register")]
        public IActionResult PostRegister(JSTable gtb)
        {
            this.Init("SignIn");
            return Ok(this.DB.SaveTable(gtb));
        }
        [HttpPost("Login")]
        public IActionResult PostLogin(JSTable gtb)
        {
            this.Init("SignIn");
            this.DB.ValidateTable(gtb);
            Table table = this.DB.Tables[gtb.Name];
            if (table.IndexFirst())
            {
                Row row = table.IndexFetch();
                if (row.HasError == false)
                {
                    string passValue = row.GetValue("Password").GetString();
                    string userValue = row.GetValue("LoginUser").GetString();

                    string query = "SELECT COUNT(Id) as CNT FROM Pub_User WHERE Active=1 AND Deleted=0 AND (UserName=@LoginUser OR Email=@LoginUser) AND Password=@Password";
                    IDictionary<string, object> ps = new Dictionary<string, object>();
                    ps.Add("LoginUser", userValue);
                    ps.Add("Password", passValue);

                    if (this.DB.DSQL.IsExisted(query, ps))
                    {
                        string jwtToken = CreateAuthToken(userValue, "PubWeb");
                        HttpContext.SaveSession("pubSite_jwtToken", jwtToken);

                        Guid guid = Guid.NewGuid();
                        HttpContext.SaveSession("pubSite_Session", guid.ToString());

                        query = "UPDATE Pub_User SET LoginCount = 0, LoginTotal = LoginTotal + 1, LoginTime = @LoginTime WHERE Active=1 AND Deleted=0 AND (UserName=@LoginUser OR Email=@LoginUser)";
                        ps.Add("LoginTime", DateTime.Now.UTCSeconds());
                        this.DB.DSQL.ExecuteQuery(query, ps);

                        query = "SELECT Id FROM Pub_User WHERE Active=1 AND Deleted=0 AND (UserName=@LoginUser OR Email=@LoginUser) AND Password=@Password";
                        List<Dictionary<string, string>> rows = this.DB.DSQL.Query(query, ps);
                        int pubUserId = rows.Select(p=>p["Id"].GetInt()??0).FirstOrDefault();

                        query = "UPDATE Pub_User_Session SET Deleted=1, LastUpdated=@LastTime WHERE Deleted=0 AND PubUserId=@UserId";
                        IDictionary<string, object> sess_ps = new Dictionary<string, object>();
                        sess_ps.Add("LastTime", DateTime.Now.UTCSeconds());
                        sess_ps.Add("UserId", pubUserId);
                        this.DB.DSQL.ExecuteQuery(query, sess_ps);
                        query = "INSERT Pub_User_Session(PubUserId, Session, Deleted, CreatedTime) Values(@UserId, @Session, 0, @LastTime)";
                        sess_ps.Add("Session", guid.ToString());
                        this.DB.DSQL.ExecuteQuery(query, sess_ps);
                    }
                    else
                    {
                        query = "SELECT COUNT(Id) as CNT FROM Pub_User WHERE Deleted=0 AND (UserName=@LoginUser OR Email=@LoginUser)";
                        if (this.DB.DSQL.IsExisted(query, ps))
                        {
                            query = "UPDATE Pub_User SET LoginCount = LoginCount + 1, Active = IIF(LoginCount>=4, 0, 1), LoginTime = @LoginTime WHERE Active=1 AND Deleted=0 AND (UserName=@LoginUser OR Email=@LoginUser)";
                            ps.Add("LoginTime", DateTime.Now.UTCSeconds());
                            this.DB.DSQL.ExecuteQuery(query, ps);
                            query = "SELECT Top 1 Active FROM Pub_User WHERE Deleted=0 AND (UserName=@LoginUser OR Email=@LoginUser)";
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

        [HttpPost("SignOut")]
        public IActionResult PostSignOut()
        {
            this.HttpContext.DeleteSession("pubSite_jwtToken");
            this.HttpContext.DeleteSession("pubSite_Session");
            return Ok("Sign Out");
        }

        private string CreateAuthToken(string email, string role)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("info@shaolinworld.org");
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
            switch (menuId)
            {
                case "SignIn":
                    {
                        Table UserLogin = new Table("UserLogin", "Pub_User", Words("pub.user"));
                        Meta loginEmail = new Meta { Name = "LoginUser", DbName = "Email", Title = Words("login.user"), Type = EInput.String, Required = true, MaxLength = 256 };
                        Meta loginPassword = new Meta { Name = "Password", DbName = "Password", Title = Words("col.password"), Type = EInput.Password, Required = true, MinLength = 6, MaxLength = 32 };
                        Meta memo = new Meta { Name = "Memo", DbName = "Phone", Title = Words("col.phone"), Type = EInput.String };
                        UserLogin.AddMetas(loginEmail, loginPassword, memo);
                        UserLogin.SaveUrl = "/api/Home/Login";
                        UserLogin.AddQueryKV("Id", -1).AddQueryKV("Deleted", false).AddQueryKV("Active", true)
                        .AddUpdateKV("LastUpdated", DateTime.Now.UTCSeconds())
                        .AddInsertKV("Deleted", false).AddInsertKV("Active", true).AddInsertKV("CreatedTime", DateTime.Now.UTCSeconds());


                        Table UserRegister = new Table("UserRegister", "Pub_User", Words("pub.user"));
                        Meta id = new Meta { Name = "Id", DbName = "Id", Title = Words("col.id"), IsKey = true };
                        Meta firstName = new Meta { Name = "FirstName", DbName = "FirstName", Title = Words("col.firstname"), Required = true, Type = EInput.String, MaxLength = 64, Value = "" };
                        Meta lastName = new Meta { Name = "LastName", DbName = "LastName", Title = Words("col.lastname"), Required = true, Type = EInput.String, MaxLength = 64 };
                        Meta userName = new Meta { Name = "UserName", DbName = "UserName", Title = Words("col.username"), Required = true, Unique = true, Type = EInput.String, MaxLength = 32 };
                        Meta email = new Meta { Name = "Email", DbName = "Email", Title = Words("col.email"), Type = EInput.Email, Required = true, Unique = true, MaxLength = 256 };
                        Meta phone = new Meta { Name = "Phone", DbName = "Phone", Title = Words("col.phone"), Type = EInput.String, MaxLength = 32 };
                        Meta cell = new Meta { Name = "Cell", DbName = "Cell", Title = Words("col.cell"), Type = EInput.String, MaxLength = 32 };
                        Meta gender = new Meta { Name = "Gender", DbName = "Gender", Title = Words("col.gender"), Type = EInput.Int };
                        gender.AddListRef("GenderList");

                        Meta photo = new Meta { Name = "Photo", DbName = "Id", Title = Words("col.photo"), Description = "PubUser|tiny|small", Type = EInput.ImageContent };

                        Meta password = new Meta { Name = "Password", DbName = "Password", Title = Words("col.password"), Description = Words("confirm.password"), Type = EInput.Passpair, Required = true, MinLength = 6, MaxLength = 32 };
                        Meta branch = new Meta { Name = "BranchId", DbName = "BranchId", Title = Words("col.branch"), Type = EInput.Int, Required = true };
                        branch.AddListRef("BranchList");
                        UserRegister.AddMetas(id, firstName, lastName, userName, email, phone, cell, gender, photo, password, branch);

                        UserRegister.AddQueryKV("Id", -1).AddQueryKV("Deleted", false).AddQueryKV("Active", true)
                        .AddUpdateKV("LastUpdated", DateTime.Now.UTCSeconds())
                        .AddInsertKV("Deleted", false).AddInsertKV("Active", true).AddInsertKV("CreatedTime", DateTime.Now.UTCSeconds());

                        UserRegister.SaveUrl = "/api/Home/Register";

                        CollectionTable c1 = new CollectionTable("BranchList", "GBranch", true, "Id", "Title", "Detail");
                        Collection BranchList = new Collection(ECollectionType.Table, c1);
                        Collection genderList = new Collection("GenderList");
                        this.DB.AddTables(UserLogin, UserRegister).AddCollections(BranchList, genderList);
                    }
                    break;
            }

        }
    }

}