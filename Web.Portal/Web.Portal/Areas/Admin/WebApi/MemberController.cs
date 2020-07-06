using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Library.V1.Common;
using Library.V1.Entity;
using Library.V1.SQL;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Web.Portal.Areas.Admin.WebApi
{
    [Route("/Admin/api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class MemberController : AdminBaseController
    {
        public MemberController(AppSetting appSetting) : base(appSetting) { }

        [HttpGet("InitRegister")]
        public IActionResult InitRegister()
        {
            this.Init("M3010");
            this.DB.FillAll();
            return Ok(this.DB);
        }
        [HttpPost("SaveRegister")]
        public IActionResult SaveRegister(JSTable gtb)
        {
            this.Init("M3010");
            return Ok(this.DB.SaveTable(gtb));
        }
        [HttpPost("ValidateRegister")]
        public IActionResult ValidateRegister(JSTable gtb)
        {
            this.Init("M3010");
            return Ok(this.DB.ValidateTable(gtb));
        }

        [HttpGet("InitUserList")]
        public IActionResult InitUserList()
        {
            this.Init("M3020");
            this.DB.FillAll();
            return Ok(this.DB);
        }
        [HttpPost("ScanUserList")]
        public IActionResult ScanUserList(JSTable gtb)
        {
            this.Init("M3020");
            return Ok(this.DB.ReloadTable(gtb));
        }
        [HttpPost("ReloadUserList")]
        public IActionResult ReloadUserList(JSTable gtb)
        {
            this.Init("M3020");
            return Ok(this.DB.ReloadTable(gtb));
        }
        [HttpPost("SaveUserList")]
        public IActionResult SaveUserList(JSTable gtb)
        {
            this.Init("M3020");
            return Ok(this.DB.SaveTable(gtb));
        }

        [HttpPost("ReloadPubUserId")]
        public IActionResult ReloadPubUserId(JSTable gtb)
        {
            this.Init("M3010");
            return Ok(this.DB.ReloadTable(gtb));
        }
        [HttpPost("SavePubUserId")]
        public IActionResult SavePubUserId(JSTable gtb)
        {
            this.Init("M3010");
            return Ok(this.DB.SaveTable(gtb));
        }


        protected override void InitDatabase(string menuId)
        {
            switch (menuId)
            {
                case "M3010":
                    {
                        Table PubUser = new Table("PubUser", "Pub_User", Words("pub.user"));
                        Meta id = new Meta { Name = "Id", DbName = "Id", Title = Words("col.id"), IsKey = true };
                        Meta firstName = new Meta { Name = "FirstName", DbName = "FirstName", Title = Words("col.firstname"), Required = true, Type = EInput.String, MaxLength = 64 };
                        Meta lastName = new Meta { Name = "LastName", DbName = "LastName", Title = Words("col.lastname"), Required = true, Type = EInput.String, MaxLength = 64 };
                        Meta firstNameLegal = new Meta { Name = "FirstNameLegal", DbName = "FirstNameLegal", Title = Words("col.firstname.legal"), Type = EInput.String, MaxLength = 64 };
                        Meta lastNameLegl = new Meta { Name = "LastNameLegal", DbName = "LastNameLegal", Title = Words("col.lastname.legal"), Type = EInput.String, MaxLength = 64 };
                        Meta dharmaName = new Meta { Name = "DharmaName", DbName = "DharmaName", Title = Words("col.dharmaname"), Type = EInput.String, MaxLength = 64 };
                        Meta displayName = new Meta { Name = "DisplayName", DbName = "DisplayName", Title = Words("col.displayname"), Sync = true, Type = EInput.String, MaxLength = 128 };
                        Meta certName = new Meta { Name = "CertificateName", DbName = "CertificateName", Title = Words("col.certificatename"), Sync = true, Type = EInput.String, MaxLength = 128 };
                        Meta aliasname = new Meta { Name = "AliasName", DbName = "AliasName", Title = Words("col.aliasname"), Type = EInput.String, MaxLength = 128 };
                        Meta occupation = new Meta { Name = "Occupation", DbName = "Occupation", Title = Words("col.occupation"), Type = EInput.String, MaxLength = 256 };
                        Meta memo = new Meta { Name = "Memo", DbName = "Memo", Title = Words("col.memo"), Type = EInput.String, MaxLength = 256 };
                        Meta memberId = new Meta { Name = "MemberId", DbName = "MemberId", Title = Words("col.memberid"), Sync = true, Type = EInput.String, MaxLength = 32 };
                        Meta idNumber = new Meta { Name = "IDNumber", DbName = "IDNumber", Title = Words("col.idnumber"), Type = EInput.String, MaxLength = 32 };
                        Meta medicalConcern = new Meta { Name = "MedicalConcern", DbName = "MedicalConcern", Title = Words("medical.concern"), Type = EInput.String, MaxLength = 1024 };
                        Meta hearUsOther = new Meta { Name = "HearUs_Other", DbName = "HearUs_Other", Title = Words("other.specify"), Type = EInput.String, MaxLength = 32 };
                        Meta multiLangOther = new Meta { Name = "MultiLang_Other", DbName = "MultiLang_Other", Title = Words("other.specify"), Type = EInput.String, MaxLength = 32 };
                        Meta symbolOther = new Meta { Name = "Symbol_Other", DbName = "Symbol_Other", Title = Words("other.specify"), Type = EInput.String, MaxLength = 32 };

                        Meta birthYY = new Meta { Name = "BirthYY", DbName = "BirthYY", Title = Words("col.birthdate"), Type = EInput.Int, MinLength = 4, MaxLength = 4 };
                        Meta birthMM = new Meta { Name = "BirthMM", DbName = "BirthMM", Title = Words("col.birthdate"), Type = EInput.Int, MaxLength = 2 };
                        birthMM.AddListRef("MonthList");
                        Meta birthDD = new Meta { Name = "BirthDD", DbName = "BirthDD", Title = Words("col.birth"), Type = EInput.Int, MaxLength = 2 };
                        birthDD.AddListRef("DayList");

                        Meta memberYY = new Meta { Name = "MemberYY", DbName = "MemberYY", Title = Words("col.memberdate"), Type = EInput.Int, MinLength = 4, MaxLength = 4, Value = System.DateTime.Now.Year };
                        Meta memberMM = new Meta { Name = "MemberMM", DbName = "MemberMM", Title = Words("col.memberdate"), Type = EInput.Int, MaxLength = 2, Value = System.DateTime.Now.Month };
                        memberMM.AddListRef("MonthList");
                        Meta memberDD = new Meta { Name = "MemberDD", DbName = "MemberDD", Title = Words("col.memberdate"), Type = EInput.Int, MaxLength = 2, Value = System.DateTime.Now.Day };
                        memberDD.AddListRef("DayList");

                        Meta dharmaYY = new Meta { Name = "DharmaYY", DbName = "DharmaYY", Title = Words("col.dharmadate"), Type = EInput.Int, MinLength = 4, MaxLength = 4 };
                        Meta dharmaMM = new Meta { Name = "DharmaMM", DbName = "DharmaMM", Title = Words("col.dharmadate"), Type = EInput.Int, MaxLength = 2 };
                        dharmaMM.AddListRef("MonthList");
                        Meta dharmaDD = new Meta { Name = "DharmaDD", DbName = "DharmaDD", Title = Words("col.dharmadate"), Type = EInput.Int, MaxLength = 2 };
                        dharmaDD.AddListRef("DayList");

                        Meta emerRelation = new Meta { Name = "EmergencyRelation", DbName = "EmergencyRelation", Title = Words("col.emergency.relation"), Type = EInput.String, MaxLength = 32 };
                        Meta emerPerson = new Meta { Name = "EmergencyPerson", DbName = "EmergencyPerson", Title = Words("col.emergency.person"), Type = EInput.String, MaxLength = 128 };
                        Meta emerPhone = new Meta { Name = "EmergencyPhone", DbName = "EmergencyPhone", Title = Words("col.emergency.phone"), Type = EInput.String, MaxLength = 32 };
                        Meta emerCell = new Meta { Name = "EmergencyCell", DbName = "EmergencyCell", Title = Words("col.emergency.cell"), Type = EInput.String, MaxLength = 32 };

                        Meta branch = new Meta { Name = "BranchId", DbName = "BranchId", Title = Words("col.branch"), Type = EInput.Int, Required = true, Value = this.DB.User.Branch };
                        branch.AddListRef("BranchList");
                        Meta gender = new Meta { Name = "Gender", DbName = "Gender", Title = Words("col.gender"), Type = EInput.Int };
                        gender.AddListRef("GenderList");
                        Meta education = new Meta { Name = "Education", DbName = "Education", Title = Words("col.education"), Type = EInput.Int };
                        education.AddListRef("EducationList");
                        Meta nationality = new Meta { Name = "Nationality", DbName = "Nationality", Title = Words("col.nationality"), Type = EInput.Int };
                        nationality.AddListRef("CountryList");
                        Meta religion = new Meta { Name = "Religion", DbName = "Religion", Title = Words("col.religion"), Type = EInput.Int };
                        religion.AddListRef("ReligionList");

                        Meta motherLang = new Meta { Name = "MotherLang", DbName = "MotherLang", Title = Words("col.motherlang"), Type = EInput.Int };
                        motherLang.AddListRef("LanguageList");
                        Meta multiLang = new Meta { Name = "MultiLang", DbName = "UserId", Title = Words("col.multilang"), Type = EInput.Checkbox, Value = new { } };
                        multiLang.AddListRef("LanguageList", "Pub_User_Language", "LanguageId");

                        Meta hearUs = new Meta { Name = "HearUs", DbName = "UserId", Title = Words("col.hearus"), Type = EInput.Checkbox, Value = new { } };
                        hearUs.AddListRef("HearUsList", "Pub_User_HearUs", "HearUsId");
                        Meta symbol = new Meta { Name = "Symbol", DbName = "UserId", Title = Words("col.symbol"), Type = EInput.Checkbox, Value = new { } };
                        symbol.AddListRef("SymbolList", "Pub_User_Symbol", "SymbolId");


                        Meta email = new Meta { Name = "Email", DbName = "Email", Title = Words("col.email"), Type = EInput.Email, Required = true, Unique = true, MaxLength = 256 };
                        Meta phone = new Meta { Name = "Phone", DbName = "Phone", Title = Words("col.phone"), Type = EInput.String, MaxLength = 32 };
                        Meta cell = new Meta { Name = "Cell", DbName = "Cell", Title = Words("col.cell"), Type = EInput.String, MaxLength = 32 };
                        Meta address = new Meta { Name = "Address", DbName = "Address", Title = Words("col.address"), Type = EInput.String, MaxLength = 256 };
                        Meta city = new Meta { Name = "City", DbName = "City", Title = Words("col.city"), Type = EInput.String, MaxLength = 64 };
                        Meta state = new Meta { Name = "State", DbName = "State", Title = Words("col.province"), Type = EInput.Int };
                        state.AddListRef("StateList");
                        Meta country = new Meta { Name = "Country", DbName = "Country", Title = Words("col.country"), Type = EInput.Int, Value = 1 };
                        country.AddListRef("CountryList");
                        Meta postal = new Meta { Name = "Postal", DbName = "Postal", Title = Words("col.postal"), Type = EInput.String, MaxLength = 16 };


                        PubUser.AddMetas(id, firstName, lastName, firstNameLegal, lastNameLegl, dharmaName, displayName, certName, aliasname, occupation, memo)
                        .AddMetas(gender, education, nationality, religion, motherLang, multiLang)
                        .AddMetas(medicalConcern, hearUsOther, symbolOther, multiLangOther, memberId, idNumber, email, phone, cell, branch, address, city, state, country, postal)
                        .AddMetas(birthYY, birthMM, birthDD, memberYY, memberMM, memberDD, dharmaYY, dharmaMM, dharmaDD)
                        .AddMetas(emerRelation, emerPerson, emerPhone, emerCell, hearUs, symbol);

                        PubUser.AddQueryKV("Id", -1).AddQueryKV("Deleted", false).AddQueryKV("Active", true)
                            .AddUpdateKV("LastUpdated", DateTime.Now.UTCSeconds())
                            .AddInsertKV("CreatedTime", DateTime.Now.UTCSeconds());

                        PubUser.SaveUrl = "/Admin/api/Member/SaveRegister";
                        PubUser.ValidateUrl = "/Admin/api/Member/ValidateRegister";


                        Table PubUserId = new Table("PubUserId", "Pub_User_Id");
                        Meta uid = new Meta { Name = "Id", DbName = "Id", Title = Words("col.id"), IsKey = true };
                        Meta idType = new Meta { Name = "IdType", DbName = "IdType", Title = Words("col.idtype"), Required = true, Type = EInput.Int };
                        idType.AddListRef("IdTypeList");
                        Meta idno = new Meta { Name = "IdNumber", DbName = "IdNumber", Title = Words("col.idno"), Required = true, Type = EInput.String, Unique = true, MaxLength = 128 };
                        PubUserId.AddRelation(new Relation(ERef.O2M, "UserId", 0));
                        PubUserId.AddMetas(uid, idType, idno);
                        PubUserId.Navi.IsActive = true;
                        PubUserId.GetUrl = "/Admin/api/Member/ReloadPubUserId";
                        PubUserId.SaveUrl = "/Admin/api/Member/SavePubUserId";
                        PubUserId.AddQueryKV("Deleted", false).AddDeleteKV("LastUpdated", DateTime.Now.UTCSeconds())
                                .AddUpdateKV("LastUpdated", DateTime.Now.UTCSeconds())
                                .AddInsertKV("Deleted", false).AddInsertKV("Active", true).AddInsertKV("CreatedTime", DateTime.Now.UTCSeconds());

                        CollectionTable c1 = new CollectionTable("EducationList", "Education",true, "Id", "Title", "Detail");
                        Collection EducationList = new Collection(ECollectionType.Category, c1);
                        CollectionTable c2 = new CollectionTable("LanguageList", "Language", true, "Id", "Title", "Detail");
                        Collection LanguageList = new Collection(ECollectionType.Category, c2);
                        CollectionTable c3 = new CollectionTable("ReligionList", "Religion", true, "Id", "Title", "Detail");
                        Collection ReligionList = new Collection(ECollectionType.Category, c3);
                        CollectionTable c4 = new CollectionTable("HearUsList", "HearUs", true, "Id", "Title", "Detail");
                        Collection HearUsList = new Collection(ECollectionType.Category, c4);
                        CollectionTable c5 = new CollectionTable("SymbolList", "Symbol", true, "Id", "Title", "Detail");
                        Collection SymbolList = new Collection(ECollectionType.Category, c5);
                        CollectionTable c6 = new CollectionTable("IdTypeList", "IdType", true, "Id", "Title", "Detail");
                        Collection IdTypeList = new Collection(ECollectionType.Category, c6);
                        CollectionTable c7 = new CollectionTable("BranchList", "GBranch", true, "Id", "Title", "Detail", "", "DESC", "Sort");
                        Collection BranchList = new Collection(ECollectionType.Table, c7);
                        CollectionTable c8 = new CollectionTable("StateList", "GState", true, "Id", "Title", "Detail","CountryId","DESC","Sort");
                        Collection StateList = new Collection(ECollectionType.Table, c8);
                        CollectionTable c9 = new CollectionTable("CountryList", "GCountry", true, "Id", "Title", "Detail", "", "DESC","Sort");
                        Collection CountryList = new Collection(ECollectionType.Table, c9);
                        Collection genderList = new Collection("GenderList");
                        Collection monthList = new Collection("MonthList");
                        Collection dayList = new Collection("DayList");
                        this.DB.AddTables(PubUser, PubUserId).AddCollections(EducationList, LanguageList, ReligionList, HearUsList, SymbolList, IdTypeList, BranchList, StateList, CountryList, genderList, monthList, dayList);
                    }
                    break;
                case "M3020":
                    {
                        Table Member = new Table("Member", "Pub_User", Words("pub.user"));
                        /*******/
                        Meta id = new Meta { Name = "Id", DbName = "Id", Title = Words("col.id"), IsKey = true, Order = "DESC" };
                        Meta memberId = new Meta { Name = "MemberId", DbName = "MemberId", Title = Words("col.memberid"), Order = "ASC" };
                        Meta firstName = new Meta { Name = "FirstName", DbName = "FirstName", Title = Words("col.fullname"), Required = true, Type = EInput.String, MaxLength = 64, Order = "ASC" };
                        Meta lastName = new Meta { Name = "LastName", DbName = "LastName", Title = Words("col.lastname"), Required = true, Type = EInput.String, MaxLength = 64, Order = "ASC" };
                        Meta firstNameLegal = new Meta { Name = "FirstNameLegal", DbName = "FirstNameLegal", Title = Words("col.firstname.legal"), Type = EInput.String, MaxLength = 64, Order = "ASC" };
                        Meta lastNameLegl = new Meta { Name = "LastNameLegal", DbName = "LastNameLegal", Title = Words("col.lastname.legal"), Type = EInput.String, MaxLength = 64, Order = "ASC" };
                        Meta dharmaName = new Meta { Name = "DharmaName", DbName = "DharmaName", Title = Words("col.dharmaname"), Type = EInput.String, MaxLength = 64, Order = "ASC" };
                        Meta displayName = new Meta { Name = "DisplayName", DbName = "DisplayName", Title = Words("col.displayname"), Sync = true, Type = EInput.String, MaxLength = 128, Order = "ASC" };
                        Meta certName = new Meta { Name = "CertificateName", DbName = "CertificateName", Title = Words("col.certificatename"), Sync = true, Type = EInput.String, MaxLength = 128, Order = "ASC" };
                        Meta aliasname = new Meta { Name = "AliasName", DbName = "AliasName", Title = Words("col.aliasname"), Type = EInput.String, MaxLength = 128, Order = "ASC" };
                        Meta occupation = new Meta { Name = "Occupation", DbName = "Occupation", Title = Words("col.occupation"), Type = EInput.String, MaxLength = 256 };
                        Meta memo = new Meta { Name = "Memo", DbName = "Memo", Title = Words("col.memo"), Type = EInput.String, MaxLength = 256 };
                        Meta userName = new Meta { Name = "UserName", DbName = "UserName", Title = Words("col.username"), Type = EInput.String, Unique = true, MaxLength = 32, Order = "ASC" };
                        Meta idNumber = new Meta { Name = "IDNumber", DbName = "IDNumber", Title = Words("col.idnumber"), Type = EInput.String, MaxLength = 32 };
                        Meta medicalConcern = new Meta { Name = "MedicalConcern", DbName = "MedicalConcern", Title = Words("medical.concern"), Type = EInput.String, MaxLength = 1024 };
                        Meta hearUsOther = new Meta { Name = "HearUs_Other", DbName = "HearUs_Other", Title = Words("other.specify"), Type = EInput.String, MaxLength = 32 };
                        Meta multiLangOther = new Meta { Name = "MultiLang_Other", DbName = "MultiLang_Other", Title = Words("other.specify"), Type = EInput.String, MaxLength = 32 };
                        Meta symbolOther = new Meta { Name = "Symbol_Other", DbName = "Symbol_Other", Title = Words("other.specify"), Type = EInput.String, MaxLength = 32 };
                        Meta photo = new Meta { Name = "Photo", DbName = "Id", Title = Words("col.photo"), Description = "PubUser|tiny|small", Type = EInput.ImageContent };
                        Meta active = new Meta { Name = "Active", DbName = "Active", Title = Words("col.status"), Description = Words("status.active.inactive"), Type = EInput.Bool, Order = "ASC" };
                        Meta loginTime = new Meta { Name = "LoginTime", DbName = "LoginTime", Title = Words("col.lastlogin"), Type = EInput.Int };
                        Meta loginTotal = new Meta { Name = "LoginTotal", DbName = "LoginTotal", Title = Words("col.logintotal"), Type = EInput.Int };
                        Meta createdTime = new Meta { Name = "CreatedTime", DbName = "CreatedTime", Title = Words("col.createdtime"), Type = EInput.Read, Order = "DESC" };

                        Meta birthYY = new Meta { Name = "BirthYY", DbName = "BirthYY", Title = Words("col.birthdate"), Type = EInput.Int, MinLength = 4, MaxLength = 4 };
                        Meta birthMM = new Meta { Name = "BirthMM", DbName = "BirthMM", Title = Words("col.birthdate"), Type = EInput.Int, MaxLength = 2 };
                        birthMM.AddListRef("MonthList");
                        Meta birthDD = new Meta { Name = "BirthDD", DbName = "BirthDD", Title = Words("col.birth"), Type = EInput.Int, MaxLength = 2 };
                        birthDD.AddListRef("DayList");

                        Meta memberYY = new Meta { Name = "MemberYY", DbName = "MemberYY", Title = Words("col.memberdate"), Type = EInput.Int, MinLength = 4, MaxLength = 4, Value = System.DateTime.Now.Year };
                        Meta memberMM = new Meta { Name = "MemberMM", DbName = "MemberMM", Title = Words("col.memberdate"), Type = EInput.Int, MaxLength = 2, Value = System.DateTime.Now.Month };
                        memberMM.AddListRef("MonthList");
                        Meta memberDD = new Meta { Name = "MemberDD", DbName = "MemberDD", Title = Words("col.memberdate"), Type = EInput.Int, MaxLength = 2, Value = System.DateTime.Now.Day };
                        memberDD.AddListRef("DayList");

                        Meta dharmaYY = new Meta { Name = "DharmaYY", DbName = "DharmaYY", Title = Words("col.dharmadate"), Type = EInput.Int, MinLength = 4, MaxLength = 4 };
                        Meta dharmaMM = new Meta { Name = "DharmaMM", DbName = "DharmaMM", Title = Words("col.dharmadate"), Type = EInput.Int, MaxLength = 2 };
                        dharmaMM.AddListRef("MonthList");
                        Meta dharmaDD = new Meta { Name = "DharmaDD", DbName = "DharmaDD", Title = Words("col.dharmadate"), Type = EInput.Int, MaxLength = 2 };
                        dharmaDD.AddListRef("DayList");

                        Meta emerRelation = new Meta { Name = "EmergencyRelation", DbName = "EmergencyRelation", Title = Words("col.emergency.relation"), Type = EInput.String, MaxLength = 32 };
                        Meta emerPerson = new Meta { Name = "EmergencyPerson", DbName = "EmergencyPerson", Title = Words("col.emergency.person"), Type = EInput.String, MaxLength = 128 };
                        Meta emerPhone = new Meta { Name = "EmergencyPhone", DbName = "EmergencyPhone", Title = Words("col.emergency.phone"), Type = EInput.String, MaxLength = 32 };
                        Meta emerCell = new Meta { Name = "EmergencyCell", DbName = "EmergencyCell", Title = Words("col.emergency.cell"), Type = EInput.String, MaxLength = 32 };

                        Meta branch = new Meta { Name = "BranchId", DbName = "BranchId", Title = Words("col.branch"), Type = EInput.Int, Required = true, Value = this.DB.User.Branch };
                        branch.AddListRef("BranchList");
                        Meta gender = new Meta { Name = "Gender", DbName = "Gender", Title = Words("col.gender"), Type = EInput.Int };
                        gender.AddListRef("GenderList");
                        Meta education = new Meta { Name = "Education", DbName = "Education", Title = Words("col.education"), Type = EInput.Int };
                        education.AddListRef("EducationList");
                        Meta nationality = new Meta { Name = "Nationality", DbName = "Nationality", Title = Words("col.nationality"), Type = EInput.Int };
                        nationality.AddListRef("CountryList");
                        Meta religion = new Meta { Name = "Religion", DbName = "Religion", Title = Words("col.religion"), Type = EInput.Int };
                        religion.AddListRef("ReligionList");

                        Meta motherLang = new Meta { Name = "MotherLang", DbName = "MotherLang", Title = Words("col.motherlang"), Type = EInput.Int };
                        motherLang.AddListRef("LanguageList");
                        Meta multiLang = new Meta { Name = "MultiLang", DbName = "UserId", Title = Words("col.multilang"), Type = EInput.Checkbox, Value = new { } };
                        multiLang.AddListRef("LanguageList", "Pub_User_Language", "LanguageId");

                        Meta hearUs = new Meta { Name = "HearUs", DbName = "UserId", Title = Words("col.hearus"), Type = EInput.Checkbox, Value = new { } };
                        hearUs.AddListRef("HearUsList", "Pub_User_HearUs", "HearUsId");
                        Meta symbol = new Meta { Name = "Symbol", DbName = "UserId", Title = Words("col.symbol"), Type = EInput.Checkbox, Value = new { } };
                        symbol.AddListRef("SymbolList", "Pub_User_Symbol", "SymbolId");


                        Meta email = new Meta { Name = "Email", DbName = "Email", Title = Words("col.email"), Type = EInput.Email, Required = true, Unique = true, MaxLength = 256 };
                        Meta phone = new Meta { Name = "Phone", DbName = "Phone", Title = Words("col.phone"), Type = EInput.String, MaxLength = 32 };
                        Meta cell = new Meta { Name = "Cell", DbName = "Cell", Title = Words("col.cell"), Type = EInput.String, MaxLength = 32 };
                        Meta address = new Meta { Name = "Address", DbName = "Address", Title = Words("col.address"), Type = EInput.String, MaxLength = 256 };
                        Meta city = new Meta { Name = "City", DbName = "City", Title = Words("col.city"), Type = EInput.String, MaxLength = 64 };
                        Meta state = new Meta { Name = "State", DbName = "State", Title = Words("col.province"), Type = EInput.Int };
                        state.AddListRef("StateList");
                        Meta country = new Meta { Name = "Country", DbName = "Country", Title = Words("col.country"), Type = EInput.Int, Value = 1 };
                        country.AddListRef("CountryList");
                        Meta postal = new Meta { Name = "Postal", DbName = "Postal", Title = Words("col.postal"), Type = EInput.String, MaxLength = 16 };


                        Member.AddMetas(id, memberId, firstName, lastName, firstNameLegal, lastNameLegl, dharmaName, displayName, certName, aliasname, occupation, memo)
                        .AddMetas(userName, gender, education, nationality, religion, motherLang, multiLang)
                        .AddMetas(medicalConcern, hearUsOther, symbolOther, multiLangOther, idNumber, email, phone, cell, branch, address, city, state, country, postal)
                        .AddMetas(birthYY, birthMM, birthDD, memberYY, memberMM, memberDD, dharmaYY, dharmaMM, dharmaDD)
                        .AddMetas(emerRelation, emerPerson, emerPhone, emerCell, hearUs, symbol, photo, active, loginTime, loginTotal, createdTime);

                        Filter f1 = new Filter() { Name = "search_name", DbName = "FirstName,LastName,FirstNameLegal,LastNameLegal,DharmaName,DisplayName,CertificateName,AliasName,UserName", Title = Words("col.fullname"), Type = EFilter.String, Compare = ECompare.Like };
                        Filter f2 = new Filter() { Name = "search_email", DbName = "Email", Title = Words("col.email"), Type = EFilter.String, Compare = ECompare.Like };
                        Filter f3 = new Filter() { Name = "search_phone", DbName = "Phone,Cell", Title = Words("col.phone"), Type = EFilter.String, Compare = ECompare.Like };
                        Filter f4 = new Filter() { Name = "search_idno", DbName = "Id", Title = Words("col.idno"), Type = EFilter.String, Compare = ECompare.Include };
                        f4.AddListRef("MemberId", "Pub_User_Id", "UserId|IdNumber");
                        Filter f5 = new Filter() { Name = "search_scan", DbName = "MemberId", Title = Words("qr.code"), Type = EFilter.Scan, Compare = ECompare.Equal };
                        //Filter f6 = new Filter() { Name = "fitler_branch", DbName = "BranchId", Title = "col.branch", Type = EFilter.Hidden, Required = true, Compare = ECompare.In, Value1 = this.DB.User.ActiveBranches };

                        Member.AddFilters(f1, f2, f3, f4, f5);
                        Member.Navi.IsActive = true;
                        Member.Navi.Order = "DESC";
                        Member.Navi.By = "CreatedTime";
                        Member.GetUrl = "/Admin/api/Member/ReloadUserList";
                        Member.SaveUrl = "/Admin/api/Member/SaveUserList";
                        Member.AddQueryKV("Deleted", false).AddDeleteKV("LastUpdated", DateTime.Now.UTCSeconds())
                            .AddUpdateKV("LastUpdated", DateTime.Now.UTCSeconds())
                            .AddInsertKV("Deleted", false).AddInsertKV("CreatedTime", DateTime.Now.UTCSeconds());


                        Table PubUserId = new Table("PubUserId", "Pub_User_Id");
                        Meta uid = new Meta { Name = "Id", DbName = "Id", Title = Words("col.id"), IsKey = true };
                        Meta idType = new Meta { Name = "IdType", DbName = "IdType", Title = Words("col.idtype"), Required = true, Type = EInput.Int };
                        idType.AddListRef("IdTypeList");
                        Meta idno = new Meta { Name = "IdNumber", DbName = "IdNumber", Title = Words("col.idno"), Required = true, Type = EInput.String, Unique = true, MaxLength = 128 };
                        PubUserId.AddMetas(uid, idType, idno);
                        PubUserId.AddRelation(new Relation(ERef.O2M, "UserId", 0));
                        PubUserId.Navi.IsActive = false;
                        PubUserId.GetUrl = "/Admin/api/Member/ReloadPubUserId";
                        PubUserId.SaveUrl = "/Admin/api/Member/SavePubUserId";
                        PubUserId.AddQueryKV("Deleted", false).AddDeleteKV("LastUpdated", DateTime.Now.UTCSeconds()).AddUpdateKV("LastUpdated", DateTime.Now.UTCSeconds()).AddInsertKV("Deleted", false).AddInsertKV("CreatedTime", DateTime.Now.UTCSeconds());


                        CollectionTable c1 = new CollectionTable("EducationList", "Education", true, "Id", "Title", "Detail");
                        Collection EducationList = new Collection(ECollectionType.Category, c1);
                        CollectionTable c2 = new CollectionTable("LanguageList", "Language", true, "Id", "Title", "Detail");
                        Collection LanguageList = new Collection(ECollectionType.Category, c2);
                        CollectionTable c3 = new CollectionTable("ReligionList", "Religion", true, "Id", "Title", "Detail");
                        Collection ReligionList = new Collection(ECollectionType.Category, c3);
                        CollectionTable c4 = new CollectionTable("HearUsList", "HearUs", true, "Id", "Title", "Detail");
                        Collection HearUsList = new Collection(ECollectionType.Category, c4);
                        CollectionTable c5 = new CollectionTable("SymbolList", "Symbol", true, "Id", "Title", "Detail");
                        Collection SymbolList = new Collection(ECollectionType.Category, c5);
                        CollectionTable c6 = new CollectionTable("IdTypeList", "IdType", true, "Id", "Title", "Detail");
                        Collection IdTypeList = new Collection(ECollectionType.Category, c6);
                        CollectionTable c7 = new CollectionTable("BranchList", "GBranch", true, "Id", "Title", "Detail", "", "DESC", "Sort");
                        Collection BranchList = new Collection(ECollectionType.Table, c7);
                        CollectionTable c8 = new CollectionTable("StateList", "GState", true, "Id", "Title", "Detail", "CountryId", "DESC", "Sort");
                        Collection StateList = new Collection(ECollectionType.Table, c8);
                        CollectionTable c9 = new CollectionTable("CountryList", "GCountry", true, "Id", "Title", "Detail", "", "DESC", "Sort");
                        Collection CountryList = new Collection(ECollectionType.Table, c9);
                        Collection genderList = new Collection("GenderList");
                        Collection monthList = new Collection("MonthList");
                        Collection dayList = new Collection("DayList");
                        this.DB.AddTables(Member, PubUserId).AddCollections(EducationList, LanguageList, ReligionList, HearUsList, SymbolList, IdTypeList, BranchList, StateList, CountryList, genderList, monthList, dayList);
                    }
                    break;
            }
        }

    }
}