using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Library.V1.Common;
using Library.V1.SQL;

namespace Library.V1.Entity
{
    public class Image
    {
        public Image()
        {
            this.Id = 0;
            this.RefKey = 0;
            this.GalleryId = 0;
            this.Guid = Guid.NewGuid();
            this.State = EState.Normal;
            this.Title_en = "";
            this.Title_cn = "";
            this.Detail_en = "";
            this.Detail_cn = "";
            this.Mime_Type = "";
            this.Full_Name = "";
            this.Short_Name = "";
            this.Ext_Name = "";
            this.Error = new Error();
            this.Data = new Dictionary<string, ImageContent>();
            // w, h not for resize,  it is actual width and height of the image
            //this.Data.Add("large", new ImageContent { w = 1200, h = 1200, size = 0, content = "" });
            //this.Data.Add("medium", new ImageContent { w = 800, h = 800, size = 0, content = "" });
            //this.Data.Add("small", new ImageContent { w = 400, h = 400, size = 0, content = "" });
            //this.Data.Add("tiny", new ImageContent { w = 160, h = 160, size = 0, content = "" });
        }

        public int Id { get; set; }
        public int RefKey { get; set; }
        [JsonIgnore]
        public int GalleryId { get; set; }
        public Guid Guid { get; set; }
        public EState State { get; set; }
        public string Title_en { get; set; }
        public string Title_cn { get; set; }
        public string Detail_en { get; set; }
        public string Detail_cn { get; set; }
        public string Mime_Type { get; set; }
        public string Full_Name { get; set; }
        public string Short_Name { get; set; }
        public string Ext_Name { get; set; }
        public bool Active { get; set; }
        public bool Main { get; set; }
        public int Sort { get; set; }
        public Error Error { get; set; }
        public IDictionary<string, ImageContent> Data { get; set; }


        public Image SaveDBImage(AppSetting sqlEnvir)
        {
            if (this.RefKey > 0 && this.GalleryId > 0)
            {
                SqlHelper DSQL = new SqlHelper(sqlEnvir.DataAccount);
                SqlHelper FSQL = new SqlHelper(sqlEnvir.FileAccount);
                switch (this.State)
                {
                    case EState.Normal:
                        break;
                    case EState.Changed:
                        {
                            if (this.Main)
                            {
                                string qq = "UPDATE GImage SET Main=0 WHERE GalleryId=@GalleryId AND RefKey=@RefKey AND Id<>@Id";
                                Dictionary<string, object> qqps = new Dictionary<string, object>();
                                qqps.Add("GalleryId", this.GalleryId);
                                qqps.Add("RefKey", this.RefKey);
                                qqps.Add("Id", this.Id);
                                FSQL.ExecuteQuery(qq, qqps);
                            }
                            Dictionary<string, object> colValues = new Dictionary<string, object>();
                            colValues.Add("Title_en", this.Title_en);
                            colValues.Add("Title_cn", this.Title_cn);
                            colValues.Add("Detail_en", this.Detail_en);
                            colValues.Add("Detail_cn", this.Detail_cn);
                            colValues.Add("Full_Name", this.Full_Name);
                            colValues.Add("Short_Name", this.Short_Name);
                            colValues.Add("Ext_Name", this.Ext_Name);
                            colValues.Add("Mime_Type", this.Mime_Type);
                            colValues.Add("Main", this.Main);
                            colValues.Add("Sort", this.Sort);
                            colValues.Add("Large", this.Data["large"].content);
                            colValues.Add("Medium", this.Data["medium"].content);
                            colValues.Add("Small", this.Data["small"].content);
                            colValues.Add("Tiny", this.Data["tiny"].content);
                            colValues.Add("LastUpdated", DateTime.Now.UTCSeconds());

                            Dictionary<string, object> whereKVs = new Dictionary<string, object>();
                            whereKVs.Add("Deleted", false);
                            whereKVs.Add("Id", this.Id);
                            whereKVs.Add("RefKey", this.RefKey);
                            whereKVs.Add("GalleryId", this.GalleryId);
                            int result = FSQL.UpdateTable("GImage", colValues, whereKVs);
                            this.State = EState.Normal;
                        }
                        break;
                    case EState.Added:
                        {
                            Dictionary<string, object> ps = new Dictionary<string, object>();
                            string query = "SELECT MaxCount FROM GGallery WHERE Id=@GalleryId";
                            ps.Add("GalleryId", this.GalleryId);
                            int maxCount = DSQL.ExecuteScalar(query, ps);


                            query = "SELECT COUNT(1) AS CNT FROM GImage WHERE deleted=0 AND GalleryId=@GalleryId AND RefKey=@RefKey";
                            ps.Clear();
                            ps.Add("GalleryId", this.GalleryId);
                            ps.Add("RefKey", this.RefKey);
                            int rowCount = FSQL.ExecuteScalar(query, ps);
                            if (maxCount > 0 && rowCount >= maxCount)
                            {
                                this.Error.Append(ErrorCode.ImageKey, LanguageHelper.Words("image.number.exceed"));
                            }
                            else
                            {
                                ps.Add("Guid", this.Guid);
                                ps.Add("Title_en", this.Title_en);
                                ps.Add("Title_cn", this.Title_cn);
                                ps.Add("Detail_en", this.Detail_en);
                                ps.Add("Detail_cn", this.Detail_cn);
                                ps.Add("Full_Name", this.Full_Name);
                                ps.Add("Short_Name", this.Short_Name);
                                ps.Add("Ext_Name", this.Ext_Name);
                                ps.Add("Mime_Type", this.Mime_Type);
                                ps.Add("Main", this.Main);
                                ps.Add("Sort", this.Sort);
                                ps.Add("Large", this.Data["large"].content);
                                ps.Add("Medium", this.Data["medium"].content);
                                ps.Add("Small", this.Data["small"].content);
                                ps.Add("Tiny", this.Data["tiny"].content);
                                ps.Add("Deleted", false);
                                ps.Add("Active", true);
                                ps.Add("CreatedTime", DateTime.Now.UTCSeconds());
                                this.Id = FSQL.InsertTable("GImage", ps);
                                this.State = EState.Normal;
                            }
                        }
                        break;
                    case EState.Deleted:
                        {
                            if (string.IsNullOrWhiteSpace(this.Data["large"].content))
                            {
                                Dictionary<string, object> whereKVs = new Dictionary<string, object>();
                                whereKVs.Add("Deleted", false);
                                whereKVs.Add("Id", this.Id);
                                whereKVs.Add("RefKey", this.RefKey);
                                whereKVs.Add("GalleryId", this.GalleryId);
                                int result = FSQL.DeleteTable("GImage", whereKVs);
                                this.State = EState.Deleted;
                            }
                            else
                            {
                                Dictionary<string, object> colValues = new Dictionary<string, object>();
                                colValues.Add("Title_en", this.Title_en);
                                colValues.Add("Title_cn", this.Title_cn);
                                colValues.Add("Detail_en", this.Detail_en);
                                colValues.Add("Detail_cn", this.Detail_cn);
                                colValues.Add("Full_Name", this.Full_Name);
                                colValues.Add("Short_Name", this.Short_Name);
                                colValues.Add("Ext_Name", this.Ext_Name);
                                colValues.Add("Mime_Type", this.Mime_Type);
                                colValues.Add("Main", this.Main);
                                colValues.Add("Sort", this.Sort);
                                colValues.Add("Large", this.Data["large"].content);
                                colValues.Add("Medium", this.Data["medium"].content);
                                colValues.Add("Small", this.Data["small"].content);
                                colValues.Add("Tiny", this.Data["tiny"].content);
                                colValues.Add("LastUpdated", DateTime.Now.UTCSeconds());

                                Dictionary<string, object> whereKVs = new Dictionary<string, object>();
                                whereKVs.Add("Deleted", false);
                                whereKVs.Add("Id", this.Id);
                                whereKVs.Add("RefKey", this.RefKey);
                                whereKVs.Add("GalleryId", this.GalleryId);
                                int result = FSQL.UpdateTable("GImage", colValues, whereKVs);
                                this.State = EState.Normal;
                            }
                        }
                        break;

                }
            }
            else
            {
                this.Error.Append(ErrorCode.ImageKey, LanguageHelper.Words("image.key.missing"));
            }
            return this;
        }

        public Image SaveText(AppSetting sqlEnvir)
        {
            SqlHelper FSQL = new SqlHelper(sqlEnvir.FileAccount);
            Dictionary<string, object> colValues = new Dictionary<string, object>();
            colValues.Add("Title_en", this.Title_en);
            colValues.Add("Title_cn", this.Title_cn);
            colValues.Add("Detail_en", this.Detail_en);
            colValues.Add("Detail_cn", this.Detail_cn);
            colValues.Add("Main", this.Main);
            colValues.Add("Sort", this.Sort);
            colValues.Add("LastUpdated", DateTime.Now.UTCSeconds());

            Dictionary<string, object> whereKVs = new Dictionary<string, object>();
            whereKVs.Add("Deleted", false);
            whereKVs.Add("Id", this.Id);
            whereKVs.Add("RefKey", this.RefKey);
            whereKVs.Add("GalleryId", this.GalleryId);
            int result = FSQL.UpdateTable("GImage", colValues, whereKVs);
            return this;
        }
    }
    public class ImageContent
    {
        public string content { get; set; }
    }
    public class ImageObject
    {
        public ImageObject()
        {
            this.MimeType = "image/jpeg";
        }
        public string MimeType { get; set; }
        public string Base64 { get; set; }
        public byte[] Bytes
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Base64))
                {
                    return new byte[0];
                }
                else
                {
                    var base64Data = Regex.Match(Base64, @"data:image/(?<type>.+?),(?<data>.+)").Groups["data"].Value;
                    return Convert.FromBase64String(base64Data);
                }
            }
        }
    }
}
