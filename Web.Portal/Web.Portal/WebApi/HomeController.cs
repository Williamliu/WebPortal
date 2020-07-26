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
            Init("M10");
            return Ok();
        }
        [HttpGet("InitSignIn")]
        public IActionResult InitSignIn()
        {
            Init("SignIn");
            this.DB.FillAll();
            return Ok(this.DB);
        }
        [HttpPost("SaveRegister")]
        public IActionResult SaveRegister(JSTable gtb)
        {
            this.Init("SignIn");
            return Ok(this.DB.SaveTable(gtb));
        }
        [HttpPost("RequestResetPassword")]
        public IActionResult RequestResetPassword(JSTable gtb)
        {
            this.Init("SignIn");
            this.DB.ValidateTable(gtb);
            Table table = this.DB.Tables[gtb.Name];
            if (table.IndexFirst())
            {
                Row row = table.IndexFetch();
                if (row.HasError == false)
                {
                    string email = row.GetValue("Email").GetString();
                    string query = "SELECT Id, guid, FirstName, LastName FROM Pub_User WHERE Deleted=0 AND Active=1 AND Email=@Email";
                    if(this.DB.DSQL.IsExist(query, new Dictionary<string, object> { {"Email", email } }))
                    {
                        Dictionary<string, string> record = this.DB.DSQL.QuerySingle(query, new Dictionary<string, object> { { "Email", email } });
                        string passGuid = record.GetValue("guid");
                        string fname = record.GetValue("FirstName");
                        string lname = record.GetValue("LastName");

                        long expiry = DateTime.Now.AddHours(2).UTCSeconds();
                        this.DB.DSQL.ExecuteQuery("Update Pub_User Set Expiry=@Exp WHERE Email=@Email", new Dictionary<string, object> { {"Exp" , expiry},{"Email",email} });
                        string url = $"{this.HttpContext.Request.Scheme}://{this.HttpContext.Request.Host.Host}/Home/ResetPassword/{passGuid}";

                        MMEmail myemail = new MMEmail("mail.shaolinworld.org", "info@shaolinworld.org", "SL2020$");
                        myemail.Port = 26;
                        myemail.enableSSL = false;
                        myemail.addFrom("info@shaolinworld.org");
                        myemail.addTo(email);
                        myemail.addReply("info@shaolinworld.org");
                        myemail.Subject = Words("reset.password.request");// "New Student Enrolled";
                        myemail.Content = "<html><body>";
                        myemail.Content += string.Format(Words("reset.password.email.content"), fname, lname, url); // $"Dear {fname} {lname}, <br><br>Welcome to {classname}<br><br>We are looking forward to see you soon.<br><br>Shaolin";
                        myemail.Content += "</body></html>";
                        myemail.SendAsync();
                    }
                    else
                    {
                        table.Error.Append(ErrorCode.NotFound, Words("user.email.not.exist"));
                    }
                }
            }
            return Ok(table);
        }
        
        [HttpPost("SaveSignIn")]
        public IActionResult SaveSignIn(JSTable gtb)
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

                    if (this.DB.DSQL.IsExist(query, ps))
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
                        if (this.DB.DSQL.IsExist(query, ps))
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


        [HttpGet("InitResetPassword/{Id?}")]
        public IActionResult InitResetPassword(string Id)
        {
            Init("ResetPassword");
            bool found = false;
            Guid guid = Guid.Empty;
            if(Guid.TryParse(Id, out guid))
            {
                string query = "SELECT Id FROM Pub_User WHERE Deleted=0 AND Active=1 AND guid=@Guid AND Expiry>=@Now";
                if (this.DB.DSQL.IsExist(query, new Dictionary<string, object> { { "Guid", guid }, { "Now", DateTime.Now.UTCSeconds() } }))
                {
                    found = true;
                    Dictionary<string, string> record = this.DB.DSQL.QuerySingle(query, new Dictionary<string, object> { { "Guid", guid }, { "Now", DateTime.Now.UTCSeconds() } });
                    int UserId = record.GetValue("Id").GetInt() ?? 0;
                    this.DB.Tables["UserPassword"].RefKey = UserId;
                    this.DB.Tables["UserPassword"].Filters["filterGuid"].Value1 = guid.ToString();
                }
                else
                {
                    found = false;
                }
            }
            else
            {
                found = false;
            }


            if (found)
            {
                this.DB.FillAll();
                this.DB.Tables["UserPassword"].Other.Add("token", guid.ToString());
            }
            else this.DB.Tables["UserPassword"].Error.Append(ErrorCode.NotFound, Words("reset.password.link.invalid"));
            return Ok(this.DB);
        }
        [HttpPost("SaveResetPassword")]
        public IActionResult SaveResetPassword(JSTable gtb)
        {
            this.Init("ResetPassword");
            this.DB.Tables["UserPassword"].Filters["filterGuid"].Value1 = gtb.Other.GetValue("token");
            return Ok(this.DB.SaveTable(gtb));
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
            switch (menuId)
            {
                case "SignIn":
                    {
                        Table UserLogin = new Table("UserLogin", "Pub_User", Words("pub.user"));
                        Meta loginEmail = new Meta { Name = "LoginUser", DbName = "Email", Title = Words("login.user"), Type = EInput.String, Required = true, MaxLength = 256 };
                        Meta loginPassword = new Meta { Name = "Password", DbName = "Password", Title = Words("col.password"), Type = EInput.Password, Required = true, MinLength = 6, MaxLength = 32 };
                        Meta memo = new Meta { Name = "Memo", DbName = "Phone", Title = Words("col.phone"), Type = EInput.String };
                        UserLogin.AddMetas(loginEmail, loginPassword, memo);
                        UserLogin.SaveUrl = "/api/Home/SaveSignIn";
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

                        UserRegister.SaveUrl = "/api/Home/SaveRegister";


                        Table Password = new Table("UserPassword", "Pub_User", Words("pub.user"));
                        Meta passId = new Meta { Name = "Id", DbName = "Id", Title = Words("col.id"), Type= EInput.Int, IsKey = true };
                        Meta passEmail = new Meta { Name = "Email", DbName = "Email", Title = Words("col.email"), Type = EInput.Email, Required = true, MaxLength = 256 };
                        Password.AddMetas(passId, passEmail);
                        Password.Navi.IsActive = false;
                        Password.SaveUrl = "/api/Home/RequestResetPassword";
                        Password.AddQueryKV("Id", -1);

                        CollectionTable c1 = new CollectionTable("BranchList", "GBranch", true, "Id", "Title", "Detail");
                        Collection BranchList = new Collection(ECollectionType.Table, c1);
                        Collection genderList = new Collection("GenderList");
                        this.DB.AddTables(UserLogin, UserRegister, Password).AddCollections(BranchList, genderList);
                    }
                    break;
                case "ResetPassword":
                    {
                        Table UserPass = new Table("UserPassword", "Pub_User", Words("pub.user"));
                        Meta id = new Meta { Name = "Id", DbName = "Id", Title = Words("col.id"), Required=true, Type=EInput.Int, IsKey = true };
                        Meta newPass = new Meta { Name = "Password", DbName = "Password", Title = Words("col.newpass"), Description = Words("confirm.password"), Required = true, Type = EInput.Passpair, MinLength = 6, MaxLength = 12 };
                        UserPass.AddMetas(id, newPass);
                        UserPass.SaveUrl = "/api/Home/SaveResetPassword";
                        UserPass.AddRelation(new Relation(ERef.O2O, "Id", -1));
                        UserPass.AddQueryKV("Deleted", false).AddQueryKV("Active", true)
                            .AddUpdateKV("LastUpdated", DateTime.Now.UTCSeconds()).AddUpdateKV("Expiry", DateTime.Now.AddHours(-2).UTCSeconds());

                        Filter f1 = new Filter { Name="filterGuid", DbName="guid", Type=EFilter.Hidden, Compare=ECompare.Equal, Value1=Guid.Empty.ToString() };
                        UserPass.AddFilter(f1);

                        this.DB.AddTable(UserPass);
                    }
                    break;
            }

        }
    }

}