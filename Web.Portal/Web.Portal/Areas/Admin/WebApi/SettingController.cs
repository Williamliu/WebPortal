using System;
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
    public class SettingController : AdminBaseController
    {
        public SettingController(AppSetting appConfig) : base(appConfig) { }
        [HttpGet("InitCountry")]
        public IActionResult InitCountry()
        {
            this.Init("M8010");
            this.DB.FillAll();
            if (this.DB.Error.HasError)
                return BadRequest(this.DB);
            else
                return Ok(this.DB);
        }
        [HttpPost("ReloadCountry")]
        public IActionResult ReloadCountry(JSTable jsTable)
        {
            this.Init("M8010");
            return Ok(this.DB.ReloadTable(jsTable));
        }
        [HttpPost("SaveCountry")]
        public IActionResult SaveCountry(JSTable jsTable)
        {
            this.Init("M8010");
            return Ok(this.DB.SaveTable(jsTable));
        }

        [HttpGet("InitProvince")]
        public IActionResult InitProvince()
        {
            this.Init("M8020");
            this.DB.FillAll();
            if (this.DB.Error.HasError)
                return BadRequest(this.DB);
            else
                return Ok(this.DB);
        }
        [HttpPost("ReloadProvince")]
        public IActionResult ReloadProvince(JSTable jsTable)
        {
            this.Init("M8020");
            return Ok(this.DB.ReloadTable(jsTable));
        }
        [HttpPost("SaveProvince")]
        public IActionResult SaveProvince(JSTable jsTable)
        {
            this.Init("M8020");
            return Ok(this.DB.SaveTable(jsTable));
        }

        [HttpGet("InitCategory")]
        public IActionResult InitCategory()
        {
            this.Init("M8030");
            this.DB.FillAll();
            if (this.DB.Error.HasError)
                return BadRequest(this.DB);
            else
                return Ok(this.DB);
        }
        [HttpPost("ReloadCategory")]
        public IActionResult ReloadCategory(JSTable jsTable)
        {
            this.Init("M8030");
            return Ok(this.DB.ReloadTable(jsTable));
        }
        [HttpPost("SaveCategory")]
        public IActionResult SaveCategory(JSTable jsTable)
        {
            this.Init("M8030");
            return Ok(this.DB.SaveTable(jsTable));
        }


        [HttpGet("InitCategoryItem")]
        public IActionResult InitCategoryItem()
        {
            this.Init("M8040");
            this.DB.FillAll();
            if (this.DB.Error.HasError)
                return BadRequest(this.DB);
            else
                return Ok(this.DB);
        }
        [HttpPost("ReloadCategoryItem")]
        public IActionResult ReloadCategoryItem(JSTable jsTable)
        {
            this.Init("M8040");
            return Ok(this.DB.ReloadTable(jsTable));
        }
        [HttpPost("SaveCategoryItem")]
        public IActionResult SaveCategoryItem(JSTable jsTable)
        {
            this.Init("M8040");
            return Ok(this.DB.SaveTable(jsTable));
        }
        [HttpGet("InitTranslation")]
        public IActionResult InitTranslation()
        {
            this.Init("M8050");
            this.DB.FillAll();
            if (this.DB.Error.HasError)
                return BadRequest(this.DB);
            else
                return Ok(this.DB);
        }
        [HttpPost("ReloadTranslation")]
        public IActionResult ReloadTranslation(JSTable jsTable)
        {
            this.Init("M8050");
            return Ok(this.DB.ReloadTable(jsTable));
        }

        [HttpPost("SaveTranslation")]
        public IActionResult SaveTranslation(JSTable jsTable)
        {
            this.Init("M8050");
            return Ok(this.DB.SaveTable(jsTable));
        }

        [HttpGet("InitGallery")]
        public IActionResult InitGallery()
        {
            this.Init("M8060");
            this.DB.FillAll();
            if (this.DB.Error.HasError)
                return BadRequest(this.DB);
            else
                return Ok(this.DB);
        }
        [HttpPost("ReloadGallery")]
        public IActionResult ReloadGallery(JSTable jsTable)
        {
            this.Init("M8060");
            return Ok(this.DB.ReloadTable(jsTable));
        }
        [HttpPost("SaveGallery")]
        public IActionResult SaveGallery(JSTable jsTable)
        {
            this.Init("M8060");
            return Ok(this.DB.SaveTable(jsTable));
        }

        [HttpGet("InitSettings")]
        public IActionResult InitSettings()
        {
            this.Init("M8080");
            this.DB.FillAll();
            if (this.DB.Error.HasError)
                return BadRequest(this.DB);
            else
                return Ok(this.DB);
        }
        [HttpPost("ReloadSettings")]
        public IActionResult ReloadSettings(JSTable jsTable)
        {
            this.Init("M8080");
            return Ok(this.DB.ReloadTable(jsTable));
        }

        [HttpPost("SaveSettings")]
        public IActionResult SaveSettings(JSTable jsTable)
        {
            this.Init("M8080");
            return Ok(this.DB.SaveTable(jsTable));
        }

        protected override void InitDatabase(string menuId)
        {
            switch (menuId)
            {
                case "M8010":
                    {
                        Table table = new Table("Country", "GCountry", Words("col.country"));
                        Meta id = new Meta { Name = "Id", DbName = "Id", Title = "ID", IsKey = true };
                        Meta titleEN = new Meta { Name = "TitleEN", DbName = "Title_en", Title = Words("title.en"), Order = "ASC", Required = true, Type = EInput.String, MaxLength = 64 };
                        Meta titleCN = new Meta { Name = "TitleCN", DbName = "Title_cn", Title = Words("title.cn"), Order = "ASC", Type = EInput.String, MaxLength = 64 };
                        Meta detailEN = new Meta { Name = "DetailEN", DbName = "Detail_en", Title = Words("detail.en"), Order = "ASC", Type = EInput.String, MaxLength = 256 };
                        Meta detailCN = new Meta { Name = "DetailCN", DbName = "Detail_cn", Title = Words("detail.cn"), Order = "ASC", Type = EInput.String, MaxLength = 256 };
                        Meta active = new Meta { Name = "Active", DbName = "Active", Title = Words("status.active"), Type = EInput.Bool };
                        Meta sort = new Meta { Name = "Sort", DbName = "Sort", Title = Words("col.sort"), Type = EInput.Int, Order = "DESC" };
                        table.AddMetas(id, titleEN, titleCN, detailEN, detailCN, active, sort);

                        Filter f1 = new Filter() { Name = "search_keyword", DbName = "Title_en,Title_cn,Detail_en,Detail_cn", Title = Words("col.keyword"), Type = EFilter.String, Compare = ECompare.Like };
                        table.AddFilter(f1);
                        table.Navi.IsActive = true;
                        table.Navi.Order = "DESC";
                        table.Navi.By = "Sort";
                        table.GetUrl = "/Admin/api/Setting/ReloadCountry";
                        table.SaveUrl = "/Admin/api/Setting/SaveCountry";
                        table.AddQueryKV("Deleted", false).AddDeleteKV("LastUpdated", DateTime.Now.UTCSeconds())
                            .AddUpdateKV("LastUpdated", DateTime.Now.UTCSeconds())
                            .AddInsertKV("Deleted", false).AddInsertKV("CreatedTime", DateTime.Now.UTCSeconds());

                        this.DB.AddTable(table);
                    }
                    break;
                case "M8020":
                    {
                        Table table = new Table("Province", "GState", Words("col.state"));
                        Meta id = new Meta { Name = "Id", DbName = "Id", Title = "ID", IsKey = true };
                        Meta titleEN = new Meta { Name = "TitleEN", DbName = "Title_en", Title = Words("title.en"), Order = "ASC", Required = true, Type = EInput.String, MaxLength = 64 };
                        Meta titleCN = new Meta { Name = "TitleCN", DbName = "Title_cn", Title = Words("title.cn"), Order = "ASC", Type = EInput.String, MaxLength = 64 };
                        Meta detailEN = new Meta { Name = "DetailEN", DbName = "Detail_en", Title = Words("detail.en"), Order = "ASC", Type = EInput.String, MaxLength = 256 };
                        Meta detailCN = new Meta { Name = "DetailCN", DbName = "Detail_cn", Title = Words("detail.cn"), Order = "ASC", Type = EInput.String, MaxLength = 256 };
                        Meta active = new Meta { Name = "Active", DbName = "Active", Title = Words("status.active"), Type = EInput.Bool };
                        Meta sort = new Meta { Name = "Sort", DbName = "Sort", Title = Words("col.sort"), Type = EInput.Int, Order = "DESC" };
                        table.AddMetas(id, titleEN, titleCN, detailEN, detailCN, active, sort);

                        Filter f1 = new Filter() { Name = "CountryFilter", DbName = "CountryId", Title = Words("col.country"), Type = EFilter.Int, Compare = ECompare.Equal };
                        f1.AddListRef("CountryList");
                        table.AddFilter(f1);
                        table.Navi.IsActive = true;
                        table.AddRelation(new Relation(ERef.O2M, "CountryId", 1));
                        table.Navi.Order = "DESC";
                        table.Navi.By = "Sort";
                        table.GetUrl = "/Admin/api/Setting/ReloadProvince";
                        table.SaveUrl = "/Admin/api/Setting/SaveProvince";
                        table.AddQueryKV("Deleted", false).AddDeleteKV("LastUpdated", DateTime.Now.UTCSeconds())
                            .AddUpdateKV("LastUpdated", DateTime.Now.UTCSeconds())
                            .AddInsertKV("Deleted", false).AddInsertKV("CreatedTime", DateTime.Now.UTCSeconds());

                        CollectionTable c1 = new CollectionTable("CountryList", "GCountry", true, "Id", "Title", "Detail", "", "DESC", "Sort");
                        Collection CountryList = new Collection(ECollectionType.Table, c1);

                        this.DB.AddTable(table).AddCollection(CountryList);
                    }
                    break;
                case "M8030":
                    {
                        Table table = new Table("Category", "GCategory", Words("col.category"));
                        Meta id = new Meta { Name = "Id", DbName = "Id", Title = "ID", IsKey = true };
                        Meta tableName = new Meta { Name = "TableName", DbName = "TableName", Title = Words("col.tablename"), Order = "ASC", Required = true, Type = EInput.String, MaxLength = 64 };
                        Meta titleEN = new Meta { Name = "TitleEN", DbName = "Title_en", Title = Words("title.en"), Order = "ASC", Required = true, Type = EInput.String, MaxLength = 64 };
                        Meta titleCN = new Meta { Name = "TitleCN", DbName = "Title_cn", Title = Words("title.cn"), Order = "ASC", Type = EInput.String, MaxLength = 64 };
                        Meta detailEN = new Meta { Name = "DetailEN", DbName = "Detail_en", Title = Words("detail.en"), Order = "ASC", Type = EInput.String, MaxLength = 256 };
                        Meta detailCN = new Meta { Name = "DetailCN", DbName = "Detail_cn", Title = Words("detail.cn"), Order = "ASC", Type = EInput.String, MaxLength = 256 };
                        Meta active = new Meta { Name = "Active", DbName = "Active", Title = Words("status.active"), Type = EInput.Bool };
                        Meta sort = new Meta { Name = "Sort", DbName = "Sort", Title = Words("col.sort"), Type = EInput.Int, Order = "DESC" };
                        table.AddMetas(id, tableName, titleEN, titleCN, detailEN, detailCN, active, sort);

                        Filter f1 = new Filter() { Name = "search_keyword", DbName = "TableName,Title_en,Title_cn,Detail_en,Detail_cn", Title = Words("col.keyword"), Type = EFilter.String, Compare = ECompare.Like };
                        table.AddFilter(f1);
                        table.Navi.IsActive = true;
                        table.Navi.Order = "DESC";
                        table.Navi.By = "Sort";
                        table.GetUrl = "/Admin/api/Setting/ReloadCategory";
                        table.SaveUrl = "/Admin/api/Setting/SaveCategory";
                        table.AddQueryKV("Deleted", false).AddDeleteKV("LastUpdated", DateTime.Now.UTCSeconds())
                            .AddUpdateKV("LastUpdated", DateTime.Now.UTCSeconds()).AddInsertKV("Deleted", false).AddInsertKV("CreatedTime", DateTime.Now.UTCSeconds());

                        this.DB.AddTable(table);
                    }
                    break;
                case "M8040":
                    {
                        Table table = new Table("CategoryItem", "GCategoryItem", Words("col.category.item"));
                        Meta id = new Meta { Name = "Id", DbName = "Id", Title = "ID", IsKey = true };
                        Meta titleEN = new Meta { Name = "TitleEN", DbName = "Title_en", Title = Words("title.en"), Order = "ASC", Required = true, Type = EInput.String, MaxLength = 64 };
                        Meta titleCN = new Meta { Name = "TitleCN", DbName = "Title_cn", Title = Words("title.cn"), Order = "ASC", Type = EInput.String, MaxLength = 64 };
                        Meta detailEN = new Meta { Name = "DetailEN", DbName = "Detail_en", Title = Words("detail.en"), Order = "ASC", Type = EInput.String, MaxLength = 256 };
                        Meta detailCN = new Meta { Name = "DetailCN", DbName = "Detail_cn", Title = Words("detail.cn"), Order = "ASC", Type = EInput.String, MaxLength = 256 };
                        Meta active = new Meta { Name = "Active", DbName = "Active", Title = Words("status.active"), Type = EInput.Bool };
                        Meta sort = new Meta { Name = "Sort", DbName = "Sort", Title = Words("col.sort"), Type = EInput.Int, Order = "DESC" };
                        table.AddMetas(id, titleEN, titleCN, detailEN, detailCN, active, sort);

                        Filter f1 = new Filter() { Name = "CategoryFilter", DbName = "CategoryId", Title = Words("filter.category"), Type = EFilter.Int, Compare = ECompare.Equal };
                        f1.AddListRef("CategoryList");
                        table.AddFilter(f1);
                        table.Navi.IsActive = true;
                        table.AddRelation(new Relation(ERef.O2M, "CategoryId", 1));
                        table.Navi.Order = "DESC";
                        table.Navi.By = "Sort";
                        table.GetUrl = "/Admin/api/Setting/ReloadCategoryItem";
                        table.SaveUrl = "/Admin/api/Setting/SaveCategoryItem";
                        table.AddQueryKV("Deleted", false).AddDeleteKV("LastUpdated", DateTime.Now.UTCSeconds()).AddUpdateKV("LastUpdated", DateTime.Now.UTCSeconds()).AddInsertKV("Deleted", false).AddInsertKV("CreatedTime", DateTime.Now.UTCSeconds());

                        CollectionTable c1 = new CollectionTable("CategoryList", "GCategory", true, "Id", "Title", "Detail", "", "DESC", "Sort");
                        Collection CategoryList = new Collection(ECollectionType.Table, c1);

                        this.DB.AddTable(table).AddCollection(CategoryList);
                    }
                    break;
                case "M8050":
                    {
                        Table table = new Table("Translation", "GTranslation", Words("website.translation"));
                        Meta id = new Meta { Name = "Id", DbName = "Id", Title = "ID", IsKey = true };
                        Meta filter = new Meta { Name = "Filter", DbName = "Filter", Title = Words("col.filter"), Order = "ASC", Type = EInput.String, MaxLength = 32 };
                        Meta keyword = new Meta { Name = "Keyword", DbName = "Keyword", Title = Words("col.keyword"), Order = "ASC", Required = true, Type = EInput.String, MaxLength = 64, Unique = true };
                        Meta word_en = new Meta { Name = "Word_en", DbName = "Word_en", Title = Words("col.word.en"), Order = "ASC", Type = EInput.String };
                        Meta word_cn = new Meta { Name = "Word_cn", DbName = "Word_cn", Title = Words("col.word.cn"), Order = "DESC", Type = EInput.String };
                        Meta active = new Meta { Name = "Active", DbName = "Active", Title = Words("status.active"), Order = "", Type = EInput.Bool, Value = true };
                        table.AddMetas(id, filter, keyword, word_en, word_cn, active);

                        Filter f1 = new Filter() { Name = "search_keyword", DbName = "Keyword,Word_en,Word_cn", Title = Words("col.keyword"), Type = EFilter.String, Compare = ECompare.Like };
                        table.AddFilter(f1);
                        table.Navi.IsActive = true;
                        table.GetUrl = "/Admin/api/Setting/ReloadTranslation";
                        table.SaveUrl = "/Admin/api/Setting/SaveTranslation";
                        table.AddQueryKV("Deleted", false).AddDeleteKV("LastUpdated", DateTime.Now.UTCSeconds())
                            .AddUpdateKV("LastUpdated", DateTime.Now.UTCSeconds())
                            .AddInsertKV("Deleted", false).AddInsertKV("CreatedTime", DateTime.Now.UTCSeconds());
                        this.DB.AddTable(table);
                    }
                    break;
                case "M8060":
                    {
                        Table table = new Table("Gallery", "GGallery", Words("col.gallery"));
                        Meta id = new Meta { Name = "Id", DbName = "Id", Title = "ID", IsKey = true };
                        Meta galleryName = new Meta { Name = "GalleryName", DbName = "GalleryName", Title = Words("col.galleryname"), Order = "ASC", Required = true, Unique = true, Type = EInput.String, MaxLength = 64 };
                        Meta filePath = new Meta { Name = "FilePath", DbName = "FilePath", Title = Words("col.filepath"), Order = "ASC", Type = EInput.String, MaxLength = 256 };
                        Meta isSaveDB = new Meta { Name = "IsSaveDB", DbName = "IsSaveDB", Title = Words("col.issavedb"), Order = "ASC", Type = EInput.Bool, Value = true };
                        Meta maxCount = new Meta { Name = "MaxCount", DbName = "MaxCount", Title = Words("col.maxcount"), Type = EInput.Int, Value = 8 };
                        Meta large_w = new Meta { Name = "large_w", DbName = "large_w", Title = Words("col.large_w"), Required = true, Type = EInput.Int, Value = 1200, Min = 800, Max = 1200 };
                        Meta large_h = new Meta { Name = "large_h", DbName = "large_h", Title = Words("col.large_h"), Required = true, Type = EInput.Int, Value = 1200, Min = 800, Max = 1200 };
                        Meta medium_w = new Meta { Name = "medium_w", DbName = "medium_w", Title = Words("col.medium_w"), Required = true, Type = EInput.Int, Value = 800, Min = 400, Max = 800 };
                        Meta medium_h = new Meta { Name = "medium_h", DbName = "medium_h", Title = Words("col.medium_h"), Required = true, Type = EInput.Int, Value = 800, Min = 400, Max = 800 };
                        Meta small_w = new Meta { Name = "small_w", DbName = "small_w", Title = Words("col.small_w"), Required = true, Type = EInput.Int, Value = 400, Min = 200, Max = 400 };
                        Meta small_h = new Meta { Name = "small_h", DbName = "small_h", Title = Words("col.small_h"), Required = true, Type = EInput.Int, Value = 400, Min = 200, Max = 400 };
                        Meta tiny_w = new Meta { Name = "tiny_w", DbName = "tiny_w", Title = Words("col.tiny_w"), Required = true, Type = EInput.Int, Value = 160, Min = 100, Max = 200 };
                        Meta tiny_h = new Meta { Name = "tiny_h", DbName = "tiny_h", Title = Words("col.tiny_h"), Required = true, Type = EInput.Int, Value = 160, Min = 100, Max = 200 };
                        Meta active = new Meta { Name = "Active", DbName = "Active", Title = Words("status.active"), Type = EInput.Bool };
                        table.AddMetas(id, galleryName, filePath, isSaveDB, maxCount, large_w, large_h, medium_w, medium_h, small_w, small_h, tiny_w, tiny_h, active);

                        Filter f1 = new Filter() { Name = "search_keyword", DbName = "GalleryName", Title = Words("col.keyword"), Type = EFilter.String, Compare = ECompare.Like };
                        table.AddFilter(f1);

                        table.Navi.IsActive = true;
                        table.GetUrl = "/Admin/api/Setting/ReloadGallery";
                        table.SaveUrl = "/Admin/api/Setting/SaveGallery";
                        table.AddQueryKV("Deleted", false).AddDeleteKV("LastUpdated", DateTime.Now.UTCSeconds())
                            .AddUpdateKV("LastUpdated", DateTime.Now.UTCSeconds())
                            .AddInsertKV("Deleted", false).AddInsertKV("CreatedTime", DateTime.Now.UTCSeconds());
                        this.DB.AddTable(table);
                    }
                    break;
                case "M8080":
                    {
                        Table table = new Table("GSetting", "GSetting", Words("website.settings"));
                        Meta id = new Meta { Name = "Id", DbName = "Id", Title = "ID", IsKey = true };
                        Meta itemName = new Meta { Name = "ItemName", DbName = "ItemName", Title = Words("col.itemname"), Order = "ASC", Required = true, Type = EInput.String, MaxLength = 32, Unique = true };
                        Meta itemTitle = new Meta { Name = "ItemTitle", DbName = "ItemTitle", Title = Words("col.itemtitle"), Order = "ASC", Type = EInput.String, MaxLength = 64 };
                        Meta lastUpdate = new Meta { Name = "LastUpdated", DbName = "LastUpdated", Title = Words("col.lastupdated"), Order = "DESC", Type = EInput.Int };
                        Meta active = new Meta { Name = "Active", DbName = "Active", Title = Words("status.active"), Order = "", Type = EInput.Bool, Value = true };
                        table.AddMetas(id, itemName, itemTitle, lastUpdate, active);

                        table.Navi.IsActive = true;
                        table.GetUrl = "/Admin/api/Setting/ReloadSettings";
                        table.SaveUrl = "/Admin/api/Setting/SaveSettings";
                        table.AddUpdateKV("LastUpdated", DateTime.Now.UTCSeconds());
                        this.DB.AddTable(table);
                    }
                    break;
            }
        }
    }
}
