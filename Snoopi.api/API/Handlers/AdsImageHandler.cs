using Snoopi.core;
using Snoopi.core.DAL.Entities;
using System;
using System.IO;
using System.Web;

namespace Snoopi.api
{
    public class AdsImageHandler : ApiHandlerBase
    {
        public override void Get(HttpRequest Request, HttpResponse Response, params string[] PathParams)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetMaxAge(TimeSpan.Zero);

            try
            {
               int bannerId = (Request.QueryString["prom_img_id"] != null ? Int32.Parse(Request.QueryString["prom_img_id"].ToString()) : 0);
               var banner =  Advertisement.FetchByID(bannerId);
                HandleImageRequest(Response, banner.FilePath);
            }
            catch (Exception) { }
        }


        public void HandleImageRequest(HttpResponse Response,string image)
        {
            string path = MediaUtility.GetOriginalFilePath("Banners", image);
            if (path.Length <= 0 || !File.Exists(path))
            {
                Response.StatusCode = 404;
                return;
            }

            string mimeType = @"image";
            if (image.EndsWith(@".jpg", StringComparison.OrdinalIgnoreCase)) mimeType = @"image/jpg";
            else if (image.EndsWith(@".jpeg", StringComparison.OrdinalIgnoreCase)) mimeType = @"image/jpeg";
            else if (image.EndsWith(@".png", StringComparison.OrdinalIgnoreCase)) mimeType = @"image/png";
            else if (image.EndsWith(@".gif", StringComparison.OrdinalIgnoreCase)) mimeType = @"image/gif";
            else if (image.EndsWith(@".mp4", StringComparison.OrdinalIgnoreCase)) mimeType = @"video/mp4";
            else if (image.EndsWith(@".mpg", StringComparison.OrdinalIgnoreCase)) mimeType = @"video/mpeg";

            Response.ContentType = mimeType;
            Response.AddFileDependency(path);
            Response.TransmitFile(path);
        }
    }
}
