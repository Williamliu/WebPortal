using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Library.V1.Common;
using Library.V1.SQL;


namespace Library.V1.Entity
{
    public class Gallery
    {
        #region Gallery Common
        public Gallery(AppSetting appSetting)
        {

            this.GalleryId = 0;
            this.GalleryName = "";
            this.Method = "none";
            this.GetUrl = "";
            this.SaveUrl = "";
            this.View = "small";  // default view size "small"
            this.Edit = "large";  // default edit size "large"

            this.Error = new Error();
            this.Navi = new Navi();
            this.RowGuid = Guid.NewGuid();
            this.Size = new Dictionary<string, Size>();
            this.Images = new List<Image>();

            this.appSetting = appSetting;
            this.DSQL = new SqlHelper(this.appSetting.DataAccount);
            this.FSQL = new SqlHelper(this.appSetting.FileAccount);
            this.Key = 0;
            this.MaxCount = 0;
        }
        public int Key { get; set; }
        public Guid RowGuid { get; set; }
        public string Method { get; set; }
        public string View { get; set; }
        public string Edit { get; set; }
        public IList<Image> Images { get; set; }
        public IDictionary<string, Size> Size { get; set; }
        public Navi Navi { get; set; }
        public Error Error { get; set; }
        public string GetUrl { get; set; }
        public string SaveUrl { get; set; }

        [JsonIgnore]
        public int GalleryId { get; set; }
        [JsonIgnore]
        public string GalleryName { get; set; }
        [JsonIgnore]
        public AppSetting appSetting { get; set; }
        [JsonIgnore]
        public SqlHelper DSQL { get; set; }
        [JsonIgnore]
        public SqlHelper FSQL { get; set; }

        [JsonIgnore]
        public bool IsSaveDB { get; set; }
        [JsonIgnore]
        public string FilePath { get; set; }
        public int MaxCount { get; set; }
        #endregion

        public void InitGallery(string galleryName)
        {
            this.GalleryName = galleryName;
            string query = "SELECT TOP 1 * FROM GGallery WHERE deleted=0 AND Active=1 AND GalleryName=@GalleryName";
            Dictionary<string, object> ps = new Dictionary<string, object>();
            ps.Add("GalleryName", this.GalleryName);
            List<Dictionary<string, string>> rows = this.DSQL.Query(query, ps);
            if (rows.Count <= 0)
            {
                this.Error.Append(ErrorCode.Gallery, LanguageHelper.Words("gallery.notfound"));
            }
            else
            {
                this.GalleryId = rows[0]["Id"].GetInt() ?? 0;
                this.IsSaveDB = rows[0]["IsSaveDB"].GetBool() ?? true;
                this.FilePath = rows[0]["FilePath"].GetString();
                this.MaxCount = rows[0]["MaxCount"].GetInt() ?? 0;
                this.Size.Add("origin", new Size { w = rows[0]["large_w"].GetInt() ?? 0, h = rows[0]["large_h"].GetInt() ?? 0 });
                this.Size.Add("large", new Size { w = rows[0]["large_w"].GetInt() ?? 0, h = rows[0]["large_h"].GetInt() ?? 0 });
                this.Size.Add("medium", new Size { w = rows[0]["medium_w"].GetInt() ?? 0, h = rows[0]["medium_h"].GetInt() ?? 0 });
                this.Size.Add("small", new Size { w = rows[0]["small_w"].GetInt() ?? 0, h = rows[0]["small_h"].GetInt() ?? 0 });
                this.Size.Add("tiny", new Size { w = rows[0]["tiny_w"].GetInt() ?? 0, h = rows[0]["tiny_h"].GetInt() ?? 0 });
            }
        }
        public void GetGallery(int refKey, string view = "small", string edit = "large", bool main = true)
        {

            this.Images = new List<Image>();
            this.MaxCount = this.MaxCount > 0 ? this.MaxCount : 8;
            string query = string.Empty;
            if (main)
                query = "SELECT TOP 1 * FROM GImage WHERE Deleted=0 AND GalleryId=@GalleryId AND RefKey=@RefKey ORDER BY Main DESC, Sort DESC, CreatedTime DESC";
            else
                query = $"SELECT TOP {this.MaxCount} * FROM GImage WHERE Deleted=0 AND GalleryId=@GalleryId AND RefKey=@RefKey ORDER BY Main DESC, Sort DESC, CreatedTime DESC";

            Dictionary<string, object> ps = new Dictionary<string, object>();
            ps.Add("GalleryId", this.GalleryId);
            ps.Add("RefKey", refKey);
            List<Dictionary<string, string>> rows = this.FSQL.Query(query, ps);
            if (rows.Count > 0)
            {
                foreach (var row in rows)
                {
                    Image img = new Image();
                    img.GalleryId = this.GalleryId;
                    img.RefKey = refKey;
                    img.Id = row["Id"].GetInt() ?? 0;
                    img.State = EState.Normal;
                    Guid nguid = Guid.NewGuid();
                    Guid.TryParse(row["Guid"].GetString(), out nguid);
                    img.Guid = nguid;
                    img.Title_en = row["Title_en"].GetString();
                    img.Title_cn = row["Title_cn"].GetString();
                    img.Detail_en = row["Detail_en"].GetString();
                    img.Detail_cn = row["Detail_cn"].GetString();
                    img.Full_Name = row["Full_Name"].GetString();
                    img.Short_Name = row["Short_Name"].GetString();
                    img.Ext_Name = row["Ext_Name"].GetString();
                    img.Mime_Type = row["Mime_Type"].GetString();
                    img.Active = row["Active"].GetBool() ?? false;
                    img.Main = row["Main"].GetBool() ?? false;
                    img.Sort = row["Sort"].GetInt() ?? 0;
                    img.Data.Add("origin", new ImageContent { content = "" });
                    img.Data.Add("tiny", new ImageContent { content = row["Tiny"].GetString() });
                    img.Data.Add("small", new ImageContent { content = row["Small"].GetString() });
                    img.Data.Add("medium", new ImageContent { content = row["Medium"].GetString() });
                    img.Data.Add("large", new ImageContent { content = row["Large"].GetString() });
                    this.Images.Add(img);
                }
                this.RowGuid = this.Images[0].Guid;
            }
            else // rows is 0
            {
                if (main)
                {
                    Image img = new Image();
                    img.GalleryId = this.GalleryId;
                    img.RefKey = refKey;
                    img.Id = 0;
                    img.State = EState.Added;
                    Guid nguid = Guid.NewGuid();
                    img.Guid = nguid;
                    img.Title_en = "";
                    img.Title_cn = "";
                    img.Detail_en = "";
                    img.Detail_cn = "";
                    img.Full_Name = "";
                    img.Short_Name = "";
                    img.Ext_Name = "";
                    img.Mime_Type = "image/jpeg";
                    img.Active = true;
                    img.Main = true;
                    img.Sort = 99;

                    img.Data.Add("origin", new ImageContent { content = "" });
                    img.Data.Add("tiny", new ImageContent { content = "" });
                    img.Data.Add("small", new ImageContent { content = "" });
                    img.Data.Add("medium", new ImageContent { content = "" });
                    img.Data.Add("large", new ImageContent { content = "" });
                    this.Images.Add(img);
                    this.RowGuid = this.Images[0].Guid;
                }
            }

        }
        public ImageObject GetImage(string guid, string size = "small")
        {

            ImageObject imgobj = new ImageObject();
            imgobj.MimeType = "image/jpeg";
            size = size.Capital();

            string query = "SELECT Id as ImageId, Mime_Type, GalleryId FROM GImage WHERE Deleted=0 AND Active=1 AND Guid=@guid";
            Dictionary<string, object> ps = new Dictionary<string, object>();
            ps.Add("guid", guid);
            IList<Dictionary<string, string>> rows = this.FSQL.Query(query, ps);
            if (rows.Count > 0)
            {
                int imageId = rows[0]["ImageId"].GetInt() ?? 0;
                string mimeType = rows[0]["Mime_Type"].GetString();
                this.GalleryId = rows[0]["GalleryId"].GetInt() ?? 0;

                query = "SELECT GalleryName, IsSaveDB, MaxCount, FilePath FROM GGallery WHERE Deleted=0 AND Active=1 AND Id=@galleryid";
                ps.Clear();
                ps.Add("galleryid", this.GalleryId);
                IList<Dictionary<string, string>> grows = this.DSQL.Query(query, ps);
                if (grows.Count > 0)
                {
                    this.GalleryName = grows[0]["GalleryName"].GetString();
                    this.IsSaveDB = grows[0]["IsSaveDB"].GetBool() ?? true;
                    this.FilePath = grows[0]["FilePath"].GetString();
                    this.MaxCount = grows[0]["MaxCount"].GetInt() ?? 0;
                }
                imgobj.MimeType = string.IsNullOrWhiteSpace(mimeType) ? imgobj.MimeType : mimeType;
                if (this.IsSaveDB)
                {
                    query = $"SELECT {size} FROM GImage WHERE Id=@Id";
                    ps.Clear();
                    ps.Add("Id", imageId);
                    IList<Dictionary<string, string>> irows = this.FSQL.Query(query, ps);
                    if (irows.Count > 0)
                    {
                        imgobj.Base64 = irows[0][size].GetString();
                    }
                }
                else
                {

                }
            }
            return imgobj;
        }

        public Image SaveImage(Image image)
        {
            if (this.IsSaveDB)
            {
                image.GalleryId = this.GalleryId;
                image.SaveDBImage(this.appSetting);
            }
            else
            {

            }
            image.Data = new Dictionary<string, ImageContent>();
            return image;
        }
        public Image SaveText(Image image)
        {
            image.GalleryId = this.GalleryId;
            image.SaveText(this.appSetting);
            return image;
        }
    }
    public class Size
    {
        public Size()
        {
            this.w = 0;
            this.h = 0;
        }
        public int w { get; set; }
        public int h { get; set; }
    }
}
