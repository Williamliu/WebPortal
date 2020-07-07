using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Library.V1.Common;
using Library.V1.Entity;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Web.Portal.Areas.Admin.WebApi
{
    [Route("/Admin/api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class SystemController : AdminBaseController
    {
        public SystemController(AppSetting appSetting) : base(appSetting) { }
        [HttpGet("InitAdminUser")]
        public IActionResult InitAdminUser()
        {
            this.Init("M9010");
            this.DB.FillAll();
            if (this.DB.Error.HasError)
                return BadRequest(this.DB);
            else
                return Ok(this.DB);
        }
        [HttpPost("ReloadAdminUser")]
        public IActionResult ReloadAdminUser(JSTable jsTable)
        {
            this.Init("M9010");
            return Ok(this.DB.ReloadTable(jsTable));
        }

        [HttpPost("SaveAdminUser")]
        public IActionResult SaveAdminUser(JSTable jsTable)
        {
            this.Init("M9010");
            return Ok(this.DB.SaveTable(jsTable));
        }
        [HttpPost("ValidateAdminUser")]
        public IActionResult ValidateAdminUser(JSTable jsTable)
        {
            this.Init("M9010");
            return Ok(this.DB.ValidateTable(jsTable));
        }

        [HttpGet("InitAdminMenu")]
        public IActionResult InitAdminMenu()
        {
            this.Init("M9030");
            this.DB.FillAll();
            if (this.DB.Error.HasError)
                return BadRequest(this.DB);
            else
                return Ok(this.DB);
        }
        [HttpPost("SaveAdminMenu")]
        public IActionResult SaveAdminMenu(JSTable jsTable)
        {
            this.Init("M9030");
            return Ok(this.DB.SaveTable(jsTable));
        }


        [HttpGet("InitAdminRole")]
        public IActionResult InitAdminRole()
        {
            this.Init("M9040");
            this.DB.FillAll();

            if (this.DB.Error.HasError == false)
            {
                Table adminRole = this.DB.Tables["AdminRole"];
                if (adminRole.Rows.Count > 0)
                {
                    foreach (Row row in adminRole.Rows)
                    {
                        Dictionary<string, Dictionary<string, bool>> roleRight = new Dictionary<string, Dictionary<string, bool>>();
                        List<Dictionary<string, string>> menuRight = this.DB.DSQL.Query("SELECT * FROM Admin_Role_Right WHERE RoleId=@RoleId", new Dictionary<string, object> { { "RoleId", row.Key } });
                        foreach (Dictionary<string, string> mr in menuRight)
                        {
                            string mmId = mr["MenuId"].GetString();
                            string rrId = mr["RightId"].GetString();
                            if (roleRight.ContainsKey(mmId))
                            {
                                roleRight[mmId].Add(rrId, true);
                            }
                            else
                            {
                                roleRight.Add(mmId, new Dictionary<string, bool> { { rrId, true } });
                            }
                        }
                        row.AddColumn(new Column("RoleRight", roleRight));

                        Dictionary<string, string> roleField = new Dictionary<string, string>();
                        List<Dictionary<string, string>> frows = this.DB.DSQL.Query("SELECT * FROM Admin_Role_Field WHERE RoleId=@RoleId", new Dictionary<string, object> { { "RoleId", row.Key } });
                        foreach (Dictionary<string, string> frow in frows)
                        {
                            string mmId = frow["MenuId"].GetString();
                            string fields = frow["Fields"].GetString();
                            roleField.Add(mmId, fields);
                        }
                        row.AddColumn(new Column("RoleField", roleField));
                    }
                }
                return Ok(this.DB);
            }
            else
                return BadRequest(this.DB);
        }

        [HttpPost("SaveAdminRole")]
        public IActionResult SaveAdminRole(JSTable jsTable)
        {
            this.Init("M9040");
            Table adminRole = this.DB.SaveTable(jsTable);
            if (adminRole.Error.HasError == false)
            {
                if (adminRole.Rows.Count > 0)
                {
                    foreach (Row row in adminRole.Rows)
                    {
                        if (row.Error.HasError == false)
                        {
                            if (row.Columns.ContainsKey("RoleRight"))
                            {
                                Dictionary<string, object> wh = new Dictionary<string, object>();
                                wh.Add("RoleId", row.Key);
                                this.DB.DSQL.DeleteTable("Admin_Role_Right", wh);

                                if (string.IsNullOrWhiteSpace(row.Columns["RoleRight"].Value.ToString())) continue;
                                try
                                {
                                    JObject jo = JObject.Parse(row.Columns["RoleRight"].Value.ToString());
                                    var roleRight = jo.ToObject<Dictionary<string, Dictionary<string, bool>>>();

                                    foreach (string menuId in roleRight.Keys)
                                    {
                                        foreach (string rightId in roleRight[menuId].Keys)
                                        {
                                            if (roleRight[menuId][rightId])
                                            {
                                                Dictionary<string, object> kvs = new Dictionary<string, object>();
                                                kvs.Add("RoleId", row.Key);
                                                kvs.Add("MenuId", menuId);
                                                kvs.Add("RightId", rightId);
                                                this.DB.DSQL.InsertTable("Admin_Role_Right", kvs);
                                            }
                                        }
                                    }
                                    row.Columns["RoleRight"].Value = "";
                                }
                                catch
                                {
                                    continue;
                                }
                            }

                            if (row.Columns.ContainsKey("RoleField"))
                            {
                                Dictionary<string, object> wh = new Dictionary<string, object>();
                                wh.Add("RoleId", row.Key);
                                this.DB.DSQL.DeleteTable("Admin_Role_Field", wh);

                                if (string.IsNullOrWhiteSpace(row.Columns["RoleField"].Value.ToString())) continue;
                                try
                                {
                                    JObject jo = JObject.Parse(row.Columns["RoleField"].Value.ToString());
                                    var rfObj = jo.ToObject<Dictionary<string, string>>();
                                    foreach (string menuId in rfObj.Keys)
                                    {
                                        if (string.IsNullOrWhiteSpace(rfObj[menuId]) == false)
                                        {
                                            Dictionary<string, object> kvs = new Dictionary<string, object>();
                                            kvs.Add("RoleId", row.Key);
                                            kvs.Add("MenuId", menuId);
                                            kvs.Add("Fields", rfObj[menuId]);
                                            this.DB.DSQL.InsertTable("Admin_Role_Field", kvs);
                                        }
                                    }
                                    row.Columns["RoleField"].Value = "";
                                }
                                catch
                                {
                                    continue;
                                }
                            }

                        }
                    }
                }
            }
            return Ok(adminRole);
        }
        [HttpPost("ReloadAdminRole")]
        public IActionResult ReloadAdminRole(JSTable jsTable)
        {
            this.Init("M9040");
            Table adminRole = this.DB.ReloadTable(jsTable);
            if (adminRole.Rows.Count > 0)
            {
                foreach (Row row in adminRole.Rows)
                {
                    Dictionary<string, Dictionary<string, bool>> roleRight = new Dictionary<string, Dictionary<string, bool>>();
                    List<Dictionary<string, string>> menuRight = this.DB.DSQL.Query("SELECT * FROM Admin_Role_Right WHERE RoleId=@RoleId", new Dictionary<string, object> { { "RoleId", row.Key } });
                    foreach (Dictionary<string, string> mr in menuRight)
                    {
                        string mmId = mr["MenuId"].GetString();
                        string rrId = mr["RightId"].GetString();
                        if (roleRight.ContainsKey(mmId))
                        {
                            roleRight[mmId].Add(rrId, true);
                        }
                        else
                        {
                            roleRight.Add(mmId, new Dictionary<string, bool> { { rrId, true } });
                        }
                    }
                    row.AddColumn(new Column("RoleRight", roleRight));
                }
            }
            return Ok(adminRole);
        }

        protected override void InitDatabase(string menuId)
        {
            switch (menuId)
            {
                case "M9010":
                    {
                        Table table = new Table("AdminUser", "Admin_User", Words("admin.user"));
                        Meta id = new Meta { Name = "Id", DbName = "Id", Title = Words("col.id"), IsKey = true };
                        Meta firstName = new Meta { Name = "FirstName", DbName = "FirstName", Title = Words("col.firstname"), Order = "ASC", Required = true, Type = EInput.String, MaxLength = 64 };
                        Meta lastName = new Meta { Name = "LastName", DbName = "LastName", Title = Words("col.lastname"), Order = "ASC", Required = true, Type = EInput.String, MaxLength = 64 };
                        Meta userName = new Meta { Name = "UserName", DbName = "UserName", Title = Words("col.username"), Order = "ASC", Required = true, Unique = true, Type = EInput.String, MaxLength = 32 };
                        Meta email = new Meta { Name = "Email", DbName = "Email", Title = Words("col.email"), Type = EInput.Email, Order = "ASC", Required = true, Unique = true, MaxLength = 256 };
                        Meta phone = new Meta { Name = "Phone", DbName = "Phone", Title = Words("col.phone"), Type = EInput.String, MaxLength = 32 };
                        Meta isAdmin = new Meta { Name = "IsAdmin", DbName = "IsAdmin", Title = Words("col.isadmin"), Description = Words("status.yes.no"), Order = "DESC", Type = EInput.Bool };
                        Meta branch = new Meta { Name = "BranchId", DbName = "BranchId", Title = Words("col.branch"), Order = "ASC", Type = EInput.Int, Required = true, Value = this.DB.User.Branch };
                        branch.AddListRef("BranchList");
                        Meta address = new Meta { Name = "Address", DbName = "Address", Title = Words("col.address"), Type = EInput.String, MaxLength = 256 };
                        Meta city = new Meta { Name = "City", DbName = "City", Title = Words("col.city"), Type = EInput.String, MaxLength = 64 };
                        Meta state = new Meta { Name = "State", DbName = "State", Title = Words("col.state"), Type = EInput.Int };
                        state.AddListRef("StateList");
                        Meta country = new Meta { Name = "Country", DbName = "Country", Title = Words("col.country"), Type = EInput.Int };
                        country.AddListRef("CountryList");
                        Meta postal = new Meta { Name = "Postal", DbName = "Postal", Title = Words("col.postal"), Type = EInput.String, MaxLength = 16 };
                        Meta active = new Meta { Name = "Active", DbName = "Active", Title = Words("col.status"), Description = Words("status.active.inactive"), Order = "DESC", Type = EInput.Bool };
                        Meta password = new Meta { Name = "Password", DbName = "Password", Title = Words("col.password"), Description = Words("confirm.password"), Type = EInput.Passpair, MinLength = 6, MaxLength = 12 };
                        Meta loginTime = new Meta { Name = "LastLogin", DbName = "LoginTime", Title = Words("col.lastlogin"), Type = EInput.Int };
                        Meta loginTotal = new Meta { Name = "LoginTotal", DbName = "LoginTotal", Title = Words("col.logintotal"), Type = EInput.Int };
                        Meta createdTime = new Meta { Name = "CreatedTime", DbName = "CreatedTime", Title = Words("col.createdtime"), Type = EInput.Read, Sync = true };
                        Meta userRole = new Meta { Name = "UserRole", DbName = "UserId", Title = Words("admin.role"), Type = EInput.Checkbox };
                        userRole.AddListRef("AdminRoleList", "Admin_User_Role", "RoleId");

                        Meta userBranch = new Meta { Name = "UserBranch", DbName = "UserId", Title = Words("col.branch"), Description = Words("status.allow.deny"), Type = EInput.Checkbox };
                        userBranch.AddListRef("", "Admin_User_Branch", "BranchId");
                        Meta userSite = new Meta { Name = "UserSite", DbName = "UserId", Title = Words("col.site"), Description = Words("status.allow.deny"), Type = EInput.Checkbox };
                        userSite.AddListRef("", "Admin_User_Site", "SiteId");


                        table.AddMetas(id, firstName, lastName, userName, email, phone, isAdmin, branch, address, city, state, country, postal, active, password, loginTime, loginTotal, createdTime, userRole, userBranch, userSite);

                        Filter f1 = new Filter() { Name = "search_name", DbName = "FirstName,LastName,UserName", Title = Words("search.name"), Type = EFilter.String, Compare = ECompare.Like };
                        Filter f2 = new Filter() { Name = "search_phone", DbName = "Phone", Title = Words("search.phone"), Type = EFilter.String, Compare = ECompare.Like };
                        Filter f3 = new Filter() { Name = "search_email", DbName = "Email", Title = Words("search.email"), Type = EFilter.String, Compare = ECompare.Like };
                        Filter f4 = new Filter() { Name = "search_branch", DbName = "BranchId", Title = Words("search.branch"), Type = EFilter.Int, Compare = ECompare.Equal };
                        f4.AddListRef("BranchList");
                        Filter f5 = new Filter() { Name = "search_role", DbName = "Id", Title = Words("admin.role"), Type = EFilter.Checkbox, Compare = ECompare.Checkbox };
                        f5.AddListRef("AdminRoleList", "Admin_User_Role", "UserId|RoleId");
                        table.AddFilters(f1, f2, f3, f4, f5);

                        table.Navi.IsActive = true;

                        table.AddQueryKV("Deleted", false).AddDeleteKV("LastUpdated", DateTime.Now.UTCSeconds()).AddUpdateKV("LastUpdated", DateTime.Now.UTCSeconds()).AddInsertKV("Deleted", false).AddInsertKV("CreatedTime", DateTime.Now.UTCSeconds());
                        table.SaveUrl = "/Admin/api/System/SaveAdminUser";
                        table.GetUrl = "/Admin/api/System/ReloadAdminUser";
                        table.ValidateUrl = "/Admin/api/System/ValidateAdminUser";

                        CollectionTable c1 = new CollectionTable("AdminRoleList", "Admin_Role", true, "Id", "Title", "Detail", "", "DESC", "Sort");
                        Collection AdminRoleList = new Collection(ECollectionType.Table, c1);

                        CollectionTable c2 = new CollectionTable("BranchList", "GBranch", true, "Id", "Title", "Detail", "", "DESC", "Sort");
                        Collection BranchList = new Collection(ECollectionType.Table, c2);
                        BranchList.AddFilter("Id", this.DB.User.ActiveBranches);

                        CollectionTable c3 = new CollectionTable("StateList", "GState", true, "Id", "Title", "Detail", "CountryId", "DESC", "Sort");
                        Collection StateList = new Collection(ECollectionType.Table, c3);
                        CollectionTable c4 = new CollectionTable("CountryList", "GCountry", true, "Id", "Title", "Detail", "", "DESC", "Sort");
                        Collection CountryList = new Collection(ECollectionType.Table, c4);

                        Table Branch = new Table("Branch", "GBranch", "col.branch", "Branch");
                        Meta bid = new Meta { Name = "Id", DbName = "Id", Title = Words("col.id"), IsKey = true };
                        Meta btitle = new Meta { Name = "Title", DbName = "Title", IsLang = true, Title = "col.title", Type = EInput.String };
                        Branch.AddMetas(bid, btitle);
                        Branch.Navi.IsActive = false;
                        Branch.Navi.By = "Sort,Title";
                        Branch.Navi.Order = "DESC,ASC";
                        Branch.AddQueryKV("Deleted", false).AddQueryKV("Active", true);

                        Table Site = new Table("Site", "GSite", "col.site", "Site");
                        Meta sid = new Meta { Name = "Id", DbName = "Id", Title = Words("col.id"), IsKey = true };
                        Meta stitle = new Meta { Name = "Title", DbName = "Title", IsLang = true, Title = "col.title", Type = EInput.String };
                        Meta branchId = new Meta { Name = "BranchId", DbName = "BranchId", Title = "col.branchid", Type = EInput.Int };
                        Site.AddMetas(sid, stitle, branchId);
                        Site.Navi.IsActive = false;
                        Site.Navi.By = "Sort,Title";
                        Site.Navi.Order = "DESC,ASC";
                        Site.AddQueryKV("Deleted", false).AddQueryKV("Active", true);

                        this.DB.AddTables(table, Branch, Site).AddCollections(AdminRoleList, BranchList, StateList, CountryList);
                    }
                    break;
                case "M9030":
                    {
                        Table firstMenu = new Table("FirstMenu", "Admin_Menu", Words("first.menu"));
                        Meta id_f = new Meta { Name = "Id", DbName = "Id", Title = Words("col.id"), IsKey = true };
                        Meta pid_f = new Meta { Name = "ParentId", DbName = "ParentId", Title = Words("col.parentid"), Required = true, Type = EInput.Int };
                        Meta menuId_f = new Meta { Name = "MenuId", DbName = "MenuId", Title = Words("col.menuid"), Required = true, Type = EInput.String, MaxLength = 16 };
                        Meta title_en_f = new Meta { Name = "Title_en", DbName = "Title_en", Title = Words("title.en"), Required = true, Type = EInput.String, MaxLength = 64 };
                        Meta title_cn_f = new Meta { Name = "Title_cn", DbName = "Title_cn", Title = Words("title.cn"), Required = true, Type = EInput.String, MaxLength = 64 };
                        Meta detail_en_f = new Meta { Name = "Detail_en", DbName = "Detail_en", Title = Words("detail.en"), Type = EInput.String, MaxLength = 256 };
                        Meta detail_cn_f = new Meta { Name = "Detail_cn", DbName = "Detail_cn", Title = Words("detail.cn"), Type = EInput.String, MaxLength = 256 };
                        Meta url_f = new Meta { Name = "Url", DbName = "Url", Title = Words("col.url"), Type = EInput.String, MaxLength = 1024 };
                        Meta pos_f = new Meta { Name = "Position", DbName = "Position", Title = Words("col.position"), Type = EInput.Int };
                        pos_f.AddListRef("PositionList");
                        Meta sort_f = new Meta { Name = "Sort", DbName = "Sort", Title = Words("col.sort"), Type = EInput.Int };
                        Meta active_f = new Meta { Name = "Active", DbName = "Active", Title = Words("col.status"), Description = Words("status.active.inactive"), Type = EInput.Bool };

                        Meta right_f = new Meta { Name = "MenuRight", DbName = "MenuId", Title = Words("menu.action"), Type = EInput.Checkbox, Value = new { } };
                        right_f.AddListRef("AdminRightList", "Admin_Menu_Right", "RightId");

                        firstMenu.AddMetas(id_f, pid_f, menuId_f, title_en_f, title_cn_f, detail_en_f, detail_cn_f, url_f, pos_f, sort_f, active_f, right_f);
                        Filter f1_f = new Filter() { Name = "ParentId", DbName = "ParentId", Title = Words("col.parentid"), Type = EFilter.Hidden, Compare = ECompare.Equal, Value1 = 0 };
                        firstMenu.AddFilters(f1_f);

                        firstMenu.Navi.IsActive = false;
                        firstMenu.Navi.By = "Position,Sort";
                        firstMenu.Navi.Order = "ASC,DESC";
                        firstMenu.AddQueryKV("Deleted", false).AddDeleteKV("LastUpdated", DateTime.Now.UTCSeconds()).AddUpdateKV("LastUpdated", DateTime.Now.UTCSeconds()).AddInsertKV("Deleted", false).AddInsertKV("CreatedTime", DateTime.Now.UTCSeconds());
                        firstMenu.SaveUrl = "/Admin/api/System/SaveAdminMenu";

                        Table secondMenu = new Table("SecondMenu", "Admin_Menu", Words("second.menu"));
                        Meta id_s = new Meta { Name = "Id", DbName = "Id", Title = Words("col.id"), IsKey = true };
                        Meta pid_s = new Meta { Name = "ParentId", DbName = "ParentId", Title = Words("col.parentid"), Required = true, Type = EInput.Int };
                        Meta menuId_s = new Meta { Name = "MenuId", DbName = "MenuId", Title = Words("col.menuid"), Required = true, Type = EInput.String, MaxLength = 16 };
                        Meta title_en_s = new Meta { Name = "Title_en", DbName = "Title_en", Title = Words("title.en"), Required = true, Type = EInput.String, MaxLength = 64 };
                        Meta title_cn_s = new Meta { Name = "Title_cn", DbName = "Title_cn", Title = Words("title.cn"), Required = true, Type = EInput.String, MaxLength = 64 };
                        Meta detail_en_s = new Meta { Name = "Detail_en", DbName = "Detail_en", Title = Words("detail.en"), Type = EInput.String, MaxLength = 256 };
                        Meta detail_cn_s = new Meta { Name = "Detail_cn", DbName = "Detail_cn", Title = Words("detail.cn"), Type = EInput.String, MaxLength = 256 };
                        Meta url_s = new Meta { Name = "Url", DbName = "Url", Title = Words("col.url"), Type = EInput.String, MaxLength = 1024 };
                        Meta pos_s = new Meta { Name = "Position", DbName = "Position", Title = Words("col.position"), Type = EInput.Int };
                        pos_s.AddListRef("PositionList");
                        Meta sort_s = new Meta { Name = "Sort", DbName = "Sort", Title = Words("col.sort"), Type = EInput.Int };
                        Meta active_s = new Meta { Name = "Active", DbName = "Active", Title = Words("col.status"), Description = Words("status.active.inactive"), Type = EInput.Bool };
                        Meta right_s = new Meta { Name = "MenuRight", DbName = "MenuId", Title = Words("menu.action"), Type = EInput.Checkbox, Value = new { } };
                        right_s.AddListRef("AdminRightList", "Admin_Menu_Right", "RightId");

                        secondMenu.AddMetas(id_s, pid_s, menuId_s, title_en_s, title_cn_s, detail_en_s, detail_cn_s, url_s, pos_s, sort_s, active_s, right_s);
                        Filter f1_s = new Filter() { Name = "ParentId", DbName = "ParentId", Title = Words("col.parentid"), Type = EFilter.Hidden, Compare = ECompare.NotEqual, Value1 = 0 };
                        secondMenu.AddFilters(f1_s);

                        secondMenu.Navi.IsActive = false;
                        secondMenu.Navi.By = "Position,Sort";
                        secondMenu.Navi.Order = "ASC,DESC";
                        secondMenu.AddQueryKV("Deleted", false).AddDeleteKV("LastUpdated", DateTime.Now.UTCSeconds()).AddUpdateKV("LastUpdated", DateTime.Now.UTCSeconds()).AddInsertKV("Deleted", false).AddInsertKV("CreatedTime", DateTime.Now.UTCSeconds());
                        secondMenu.SaveUrl = "/Admin/api/System/SaveAdminMenu";

                        CollectionTable c1 = new CollectionTable("AdminRightList", "Admin_Right", true, "Id", "Title", "Detail", "", "DESC", "Sort");
                        Collection AdminRightList = new Collection(ECollectionType.Table, c1);
                        Collection PositionList = new Collection("PositionList");
                        this.DB.AddTables(firstMenu, secondMenu).AddCollections(AdminRightList, PositionList);
                    }
                    break;
                case "M9040":
                    {
                        Table firstMenu = new Table("FirstMenu", "Admin_Menu", Words("first.menu"));
                        Meta id_f = new Meta { Name = "Id", DbName = "Id", Title = Words("col.id"), IsKey = true };
                        Meta pid_f = new Meta { Name = "ParentId", DbName = "ParentId", Title = Words("col.parentid"), Type = EInput.Int };
                        Meta title_f = new Meta { Name = "Title", DbName = "Title", Title = Words("col.title"), IsLang = true, Type = EInput.String, MaxLength = 64 };
                        Meta detail_f = new Meta { Name = "Detail", DbName = "Detail", Title = Words("col.detail"), IsLang = true, Type = EInput.String, MaxLength = 256 };
                        Meta right_f = new Meta { Name = "MenuRight", DbName = "MenuId", Title = Words("menu.action"), Type = EInput.Checkbox, Value = new { } };
                        right_f.AddListRef("AdminRightList", "Admin_Menu_Right", "RightId");

                        firstMenu.AddMetas(id_f, pid_f, title_f, detail_f, right_f);
                        Filter f1_f = new Filter() { Name = "ParentId", DbName = "ParentId", Title = Words("col.parentid"), Type = EFilter.Hidden, Compare = ECompare.Equal, Value1 = 0 };
                        firstMenu.AddFilters(f1_f);

                        firstMenu.Navi.IsActive = false;
                        firstMenu.Navi.By = "Position,Sort";
                        firstMenu.Navi.Order = "ASC,DESC";
                        firstMenu.AddQueryKV("Deleted", false).AddQueryKV("Active", true);

                        Table secondMenu = new Table("SecondMenu", "Admin_Menu", Words("second.menu"));
                        Meta id_s = new Meta { Name = "Id", DbName = "Id", Title = Words("col.id"), IsKey = true };
                        Meta pid_s = new Meta { Name = "ParentId", DbName = "ParentId", Title = Words("col.parentid"), Required = true, Type = EInput.Int };
                        Meta title_s = new Meta { Name = "Title", DbName = "Title", Title = Words("title.en"), IsLang = true, Type = EInput.String, MaxLength = 64 };
                        Meta detail_s = new Meta { Name = "Detail", DbName = "Detail", Title = Words("detail.en"), IsLang = true, Type = EInput.String, MaxLength = 256 };
                        Meta right_s = new Meta { Name = "MenuRight", DbName = "MenuId", Title = Words("menu.action"), Type = EInput.Checkbox, Value = new { } };
                        right_s.AddListRef("AdminRightList", "Admin_Menu_Right", "RightId");

                        secondMenu.AddMetas(id_s, pid_s, title_s, detail_s, right_s);
                        Filter f1_s = new Filter() { Name = "ParentId", DbName = "ParentId", Title = Words("col.parentid"), Type = EFilter.Hidden, Compare = ECompare.NotEqual, Value1 = 0 };
                        secondMenu.AddFilters(f1_s);

                        secondMenu.Navi.IsActive = false;
                        secondMenu.Navi.By = "Position,Sort";
                        secondMenu.Navi.Order = "ASC,DESC";
                        secondMenu.AddQueryKV("Deleted", false).AddQueryKV("Active", true);


                        Table AdminRole = new Table("AdminRole", "Admin_Role", Words("admin.role"));
                        Meta id = new Meta { Name = "Id", DbName = "Id", Title = "ID", IsKey = true };
                        Meta titleEN = new Meta { Name = "TitleEN", DbName = "Title_en", Title = Words("title.en"), Order = "ASC", Required = true, Type = EInput.String, MaxLength = 64 };
                        Meta titleCN = new Meta { Name = "TitleCN", DbName = "Title_cn", Title = Words("title.cn"), Order = "ASC", Required = true, Type = EInput.String, MaxLength = 64 };
                        Meta detailEN = new Meta { Name = "DetailEN", DbName = "Detail_en", Title = Words("detail.en"), Order = "ASC", Type = EInput.String, MaxLength = 256 };
                        Meta detailCN = new Meta { Name = "DetailCN", DbName = "Detail_cn", Title = Words("detail.cn"), Order = "ASC", Type = EInput.String, MaxLength = 256 };
                        Meta active = new Meta { Name = "Active", DbName = "Active", Title = Words("status.active"), Description = Words("status.active.inactive"), Type = EInput.Bool };
                        Meta sort = new Meta { Name = "Sort", DbName = "Sort", Title = Words("col.sort"), Type = EInput.Int, Order = "DESC" };
                        Meta roleRight = new Meta { Name = "RoleRight", DbName = "RoleRight", Title = Words("menu.action"), Type = EInput.Custom, Value = new { } };

                        Meta roleField = new Meta { Name = "RoleField", DbName = "RoleField", Title = Words("role.field"), Type = EInput.Custom, Value = new { } };
                        AdminRole.AddMetas(id, titleEN, titleCN, detailEN, detailCN, active, sort, roleRight, roleField);
                        Filter f1 = new Filter() { Name = "search_name", DbName = "Title_en,Title_cn", Title = Words("search.name"), Type = EFilter.String, Compare = ECompare.Like };
                        AdminRole.AddFilters(f1);
                        AdminRole.Navi.IsActive = true;
                        AdminRole.Navi.By = "Title_en";

                        AdminRole.AddQueryKV("Deleted", false)
                            .AddUpdateKV("LastUpdated", DateTime.Now.UTCSeconds())
                            .AddInsertKV("CreatedTime", DateTime.Now.UTCSeconds())
                            .AddInsertKV("Deleted", false);

                        AdminRole.SaveUrl = "/Admin/api/System/SaveAdminRole";
                        AdminRole.GetUrl = "/Admin/api/System/ReloadAdminRole";

                        CollectionTable c1 = new CollectionTable("AdminRoleList", "Admin_Role",true, "Id", "Title", "Detail", "", "DESC", "Sort");
                        Collection AdminRoleList = new Collection(ECollectionType.Table, c1);
                        CollectionTable c2 = new CollectionTable("AdminRightList", "Admin_Right", true, "Id", "Title", "Detail", "", "DESC", "Sort");
                        Collection AdminRightList = new Collection(ECollectionType.Table, c2);
                        this.DB.AddTables(firstMenu, secondMenu, AdminRole).AddCollections(AdminRoleList, AdminRightList);
                    }
                    break;
            }
        }

    }
}