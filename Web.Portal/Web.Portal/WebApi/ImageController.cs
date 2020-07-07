using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using System.Text.RegularExpressions;
using System.IO;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Library.V1.Common;
using Library.V1.Entity;
using Library.V1.SQL;

namespace Web.Portal.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ImageController : ImageBaseController
    {
        protected Gallery Gallery { get; set; }
        public ImageController(AppSetting appSetting) : base(appSetting) { }
        [HttpGet("InitGallery/{galleryName}")]
        public IActionResult InitGallery(string galleryName, string view = "small", string edit = "large")
        {
            this.Init(galleryName);
            this.Gallery.GetUrl = $"/api/Image/GetGallery/{galleryName}";
            this.Gallery.SaveUrl = $"/api/Image/SaveImage/{galleryName}";
            this.Gallery.Navi.IsActive = false;
            this.Gallery.RowGuid = Guid.Empty;
            this.Gallery.View = view;
            this.Gallery.Edit = edit;
            return Ok(this.Gallery);
        }
        [HttpGet("GetGallery/{galleryName}/{refkey}")]
        public IActionResult GetGallery(string galleryName, int refkey, string view = "small", string edit = "large", string main = "1")
        {
            this.Init(galleryName);
            this.Gallery.View = view;
            this.Gallery.Edit = edit;
            if (main != "1")
                this.Gallery.GetGallery(refkey, view, edit, false);
            else
                this.Gallery.GetGallery(refkey, view, edit, true);

            return Ok(this.Gallery);
        }

        [HttpPost("SaveImage/{galleryName}")]
        public IActionResult SaveImage(string galleryName, Image image)
        {
            this.Init(galleryName);
            if (image.Data != null && image.Data.Count > 0)
            {
                this.Gallery.SaveImage(image);
            }
            else
            {
                this.Gallery.SaveText(image);
            }
            return Ok(image);
        }
        [HttpGet("GetImage/{guid}/{size?}")]
        [AllowAnonymous]
        public IActionResult GetImage(string guid, string size = "small")
        {
            this.Init("");
            ImageObject imgObj = this.Gallery.GetImage(guid, size);
            return File(imgObj.Bytes, imgObj.MimeType);
        }

        protected override void InitDatabase(string galleryName)
        {
            this.Gallery = new Gallery(this.AppConfig);
            this.Gallery.InitGallery(galleryName);
        }

    }
}