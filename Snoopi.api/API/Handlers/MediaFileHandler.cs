using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using dg.Utilities;
using System.IO;
using dg.Utilities.Imaging;
using dg.Utilities.Encryption;
using dg.Utilities.WebApiServices;
using Snoopi.core.BL;
using Snoopi.core.DAL;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net;
using dg.Sql;
using dg.Sql.Connector;
using System.Globalization;
using Snoopi.core;
using Snoopi.core.DAL.Entities;

namespace Snoopi.api
{
    public class MediaFileHandler : ApiHandlerBase
    {
        private static string[] AcceptedImageExtensions = new string[] { @".jpg", @".jpeg", @".png" };
        public static bool IsAcceptedImageExtension(string fileName)
        {
            string ext = System.IO.Path.GetExtension(fileName).ToLower();
            return AcceptedImageExtensions.Contains(ext);
        }

        public void HandleImageRequest(HttpRequest Request, HttpResponse Response, bool IsHead, Int64 AppUserId, string SubFolder)
        {
            Int32 w, h;
            Int32.TryParse(Request.QueryString[@"w"] ?? "", out w);
            Int32.TryParse(Request.QueryString[@"h"] ?? "", out h);

            string image = Request.QueryString[@"image"];

            if (string.IsNullOrEmpty(image))
            {
                Response.StatusCode = 404;
                return;
            }

            string path = null;
            if (w <= 0 && h <= 0)
            {
                path = MediaUtility.GetOriginalFilePath(SubFolder, image, (SubFolder == @"Animal" ? AppUserId : 0));
            }
            else
            {
                path = MediaUtility.GetImageFilePath(SubFolder, image, w, h, (SubFolder == @"Animal" ? AppUserId : 0));
            }

            if (path.Length > 0 && File.Exists(path))
            {
                Response.Cache.SetMaxAge(TimeSpan.FromDays(30));
                Response.Cache.SetCacheability(HttpCacheability.Public);
                Response.ClearContent();

                string mimeType = @"image";
                if (image.EndsWith(@".jpg", StringComparison.OrdinalIgnoreCase)) mimeType = @"image/jpg";
                else if (image.EndsWith(@".jpeg", StringComparison.OrdinalIgnoreCase)) mimeType = @"image/jpeg";
                else if (image.EndsWith(@".png", StringComparison.OrdinalIgnoreCase)) mimeType = @"image/png";
                else if (image.EndsWith(@".gif", StringComparison.OrdinalIgnoreCase)) mimeType = @"image/gif";
                else if (image.EndsWith(@".mp4", StringComparison.OrdinalIgnoreCase)) mimeType = @"video/mp4";
                else if (image.EndsWith(@".mpg", StringComparison.OrdinalIgnoreCase)) mimeType = @"video/mpeg";

                Response.ContentType = mimeType;
                Response.AddFileDependency(path);
                Response.Cache.SetLastModifiedFromFileDependencies();
                Response.Cache.SetETagFromFileDependencies();

                if (IsHead)
                {
                    Response.AddHeader(@"Content-Length", new FileInfo(path).Length.ToString());
                }
                else
                {
                    Response.TransmitFile(path);
                }

                return;
            }
            else
            {
                Response.StatusCode = 404;
                return;
            }
        }

        public override void Head(HttpRequest Request, HttpResponse Response, params string[] PathParams)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetMaxAge(TimeSpan.Zero);

            Int64 AppUserId;
            if (IsAuthorizedRequest(Request, Response, true, out AppUserId))
            {
                string SubFolder = Request.QueryString["sub_folder"] != null ? Request.QueryString["sub_folder"].ToString() : "";
                HandleImageRequest(Request, Response, true, AppUserId, SubFolder);
            }
        }

        public override void Get(HttpRequest Request, HttpResponse Response, params string[] PathParams)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetMaxAge(TimeSpan.Zero);

            Int64 AppUserId = 0;
            string SubFolder = Request.QueryString["sub_folder"] != null ? Request.QueryString["sub_folder"].ToString() : "";
            Int64 TempAppUserId = (Request.QueryString["temp_app_user_id"] != null ? Convert.ToInt64(Request.QueryString["temp_app_user_id"]) : 0);

            bool isAuthorized = IsAuthorizedRequest(Request, Response, false, out AppUserId) || TempAppUserId > 0;
            if (Request.QueryString[@"image"].Contains("2018_03_19_12_05_04") && isAuthorized)
            {
                NotificationFilter filter = null;
                if (AppUserId > 0)
                    filter = NotificationGroups.GetUserRelevantFilter(AppUserId);
                else if (TempAppUserId > 0)
                    filter = NotificationGroups.GetTempUserFilter();

                if (filter != null)
                {
                    string image = filter.AdImageUrl;
                    string path = MediaUtility.GetOriginalFilePath(SubFolder, filter.AdImageUrl, 0);                 
                    if (path.Length > 0 && File.Exists(path))
                    {
                        Response.Cache.SetMaxAge(TimeSpan.FromDays(30));
                        Response.Cache.SetCacheability(HttpCacheability.Public);
                        Response.ClearContent();

                        string mimeType = @"image";
                        if (image.EndsWith(@".jpg", StringComparison.OrdinalIgnoreCase)) mimeType = @"image/jpg";
                        else if (image.EndsWith(@".jpeg", StringComparison.OrdinalIgnoreCase)) mimeType = @"image/jpeg";
                        else if (image.EndsWith(@".png", StringComparison.OrdinalIgnoreCase)) mimeType = @"image/png";
                        else if (image.EndsWith(@".gif", StringComparison.OrdinalIgnoreCase)) mimeType = @"image/gif";

                        Response.ContentType = mimeType;
                        Response.AddFileDependency(path);
                        Response.Cache.SetLastModifiedFromFileDependencies();
                        Response.Cache.SetETagFromFileDependencies();
                        Response.TransmitFile(path);
                        return;
                    }
                }
            }

            if (SubFolder == "Product" || SubFolder == "ProductYad2" || SubFolder == "Banners" || SubFolder == "Supplier")
            {
                HandleImageRequest(Request, Response, false, 0, SubFolder);
            }
            else if (AppUserId > 0 || IsAuthorizedRequest(Request, Response, true, out AppUserId))
            {
                HandleImageRequest(Request, Response, false, AppUserId, SubFolder);
            }
        }

        public override void Delete(HttpRequest Request, HttpResponse Response, params string[] PathParams)
        {
            Int64 AppUserId;
            if (IsAuthorizedRequest(Request, Response, true, out AppUserId))
            {
                Response.ContentType = @"application/json";

                using (StreamWriter streamWriter = new StreamWriter(Response.OutputStream))
                {
                    using (JsonTextWriter jsonWriter = new JsonTextWriter(streamWriter))
                    {
                        Query.New<AppUserAnimal>().Where(AppUserAnimal.Columns.AppUserId, AppUserId).Update(AppUserAnimal.Columns.AnimalImg, @"").Execute();

                        jsonWriter.WriteStartObject();
                        jsonWriter.WriteEndObject();
                    }
                }
            }
        }

        public override void Post(HttpRequest Request, HttpResponse Response, params string[] PathParams)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Int64 AppUserId;
            if (IsAuthorizedRequest(Request, Response, true, out AppUserId))
            {
                Response.ContentType = @"text/plain"; // IE<=9 can't parse JSON when content type is any different than text/plain

                string sub_folder = Request.Form["sub_folder"] != null ? Request.Form["sub_folder"].ToString() : "";

                string fn = null;
                if (Request.Files.Count == 1)
                {
                    if (!IsAcceptedImageExtension(Request.Files[0].FileName))
                    {
                        RespondUnauthorized(Response);
                    }
                    fn = MediaUtility.SaveFile(Request.Files[0], sub_folder, (sub_folder == @"Animal" ? AppUserId : 0));
                    if (string.IsNullOrEmpty(fn))
                    {
                        RespondInternalServerError(Response);
                    }
                }
                Int64 ProductId = Request.Form["product_id"] != null ? Convert.ToInt64(Request.Form["product_id"]) : 0;
                Int32 w = Request.Form["w"] != null ? Convert.ToInt32(Request.Form["w"]) : 0;
                Int32 h = Request.Form["h"] != null ? Convert.ToInt32(Request.Form["h"]) : 0;

                if (sub_folder == @"Animal")
                {
                    AppUserAnimal animal = AppUserAnimal.FetchByID(AppUserId);
                    MediaUtility.DeleteImageFilePath(sub_folder, animal.AnimalImg, w, h, AppUserId);
                    animal.AnimalImg = fn;
                    animal.Save();
                }
                else if (sub_folder == @"Product")
                {
                    Product p = Product.FetchByID(ProductId);
                    MediaUtility.DeleteImageFilePath(sub_folder, p.ProductImage, w, h, 0);
                    p.ProductImage = fn;
                    p.Save();

                }
                else if (sub_folder == @"ProductYad2")
                {
                    ProductYad2 p = ProductYad2.FetchByID(ProductId);
                    MediaUtility.DeleteImageFilePath(sub_folder, p.ProductImage, w, h, 0);
                    p.ProductImage = fn;
                    p.Save();
                }

                if (!string.IsNullOrEmpty(fn))
                {
                    using (StreamWriter streamWriter = new StreamWriter(Response.OutputStream))
                    {
                        using (JsonTextWriter jsonWriter = new JsonTextWriter(streamWriter))
                        {
                            jsonWriter.WriteStartObject();

                            jsonWriter.WritePropertyName("file_name");
                            jsonWriter.WriteValue(fn);

                            //jsonWriter.WritePropertyName("preview");
                            //jsonWriter.WriteValue(MediaUtility.GetImageFilePath(AppUserId, @"Animal",HttpUtility.UrlEncode(fn),64,64));

                            //jsonWriter.WritePropertyName("url");
                            //jsonWriter.WriteValue(MediaUtility.GetOriginalFilePath(AppUserId, @"Animal",HttpUtility.UrlEncode(fn)));

                            jsonWriter.WriteEndObject();
                        }
                    }
                }
                else
                {
                    RespondInternalServerError(Response);
                }
            }
        }

        public override void Put(HttpRequest Request, HttpResponse Response, params string[] PathParams)
        {
            Post(Request, Response, PathParams);
        }
    }
}


