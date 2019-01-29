using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using dg.Utilities;
using System.IO;
using dg.Utilities.Imaging;
using dg.Utilities.Encryption;
using dg.Utilities.WebApiServices;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net;
using dg.Sql;
using dg.Sql.Connector;
using Snoopi.core.BL;
using Snoopi.core;
using Snoopi.core.DAL;

namespace Snoopi.web.API
{
    public class ImageUploadHandler : ApiHandlerBase
    {
        private static string[] AcceptedImageExtensions = new string[] { @".jpg", @".jpeg", @".png" };
        public static bool IsAcceptedImageExtension(string fileName)
        {
            string ext = System.IO.Path.GetExtension(fileName).ToLower();
            return AcceptedImageExtensions.Contains(ext);
        }
        public override void Post(HttpRequest Request, HttpResponse Response, params string[] PathParams)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);

            Response.ContentType = @"text/plain"; // IE<=9 can't parse JSON when content type is any different than text/plain

            string sub_folder = "Product";

            string fn = null;
            if (Request.Files.Count == 1)
            {
                if (!IsAcceptedImageExtension(Request.Files[0].FileName))
                {
                    RespondUnauthorized(Response);
                }
                fn = MediaUtility.SaveFile(Request.Files[0], sub_folder, 0);
                if (string.IsNullOrEmpty(fn))
                {
                    RespondInternalServerError(Response);
                }
            }
            Int64 ProductId = Request.Form["product_id"] != null ? Convert.ToInt64(Request.Form["product_id"]) : 0;
            Int32 w = Request.Form["w"] != null ? Convert.ToInt32(Request.Form["w"]) : 0;
            Int32 h = Request.Form["h"] != null ? Convert.ToInt32(Request.Form["h"]) : 0;

            if (sub_folder == @"Product")
            {
                Product p = Product.FetchByID(ProductId);
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

                        jsonWriter.WritePropertyName("preview");
                        jsonWriter.WriteValue(MediaUtility.GetImageFilePath(sub_folder, HttpUtility.UrlEncode(fn), 64, 64));

                        jsonWriter.WritePropertyName("url");
                        jsonWriter.WriteValue(MediaUtility.GetOriginalFilePath(sub_folder, HttpUtility.UrlEncode(fn)));

                        jsonWriter.WriteEndObject();
                    }
                }
            }
            else
            {
                RespondInternalServerError(Response);
            }

        }

        public override void Put(HttpRequest Request, HttpResponse Response, params string[] PathParams)
        {
            Post(Request, Response, PathParams);
        }

           
    }
}
