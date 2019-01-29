//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Web;
//using dg.Utilities;
//using Snoopi.core.BL;
//using System.IO;
//using dg.Utilities.Imaging;
//using dg.Utilities.WebApiServices;
//using System.Threading;

//namespace Snoopi.api
//{
//    public class TempImageDownloadHandler : IRestHandlerTarget
//    {
//        public void HandleImageRequest(HttpRequest Request, HttpResponse Response, bool IsHead)
//        {
//            int cx, cy;
//            int.TryParse(Request.QueryString[@"cx"], out cx);
//            int.TryParse(Request.QueryString[@"cy"], out cy);

//            string fn = Request.QueryString[@"fn"];
//            if (fn == null || fn.Length == 0 || fn.Contains(@"/") || fn.Contains(@"\"))
//            {
//                Http.Respond404(true);
//            }
//            int userId;
//            if (!int.TryParse(Request.QueryString[@"id"], out userId))
//            {
//                Http.Respond404(true);
//            }
//            string tempPathBase = Settings.GetSetting(Settings.Keys.API_TEMP_UPLOAD_FOLDER) + userId;
//            if (!tempPathBase.Contains(@":/") && !tempPathBase.Contains(@":\"))
//            {
//                tempPathBase = HttpContext.Current.Server.MapPath(tempPathBase);
//            }
//            string tempThumbPathBase = Path.Combine(tempPathBase, @"thumbs", cx + @"_" + cy);
//            string outputFilePath = Path.Combine(tempThumbPathBase, fn);
//            if ((cx > 0 || cy > 0) && !File.Exists(outputFilePath))
//            {
//                if (!Folders.VerifyDirectoryExists(tempThumbPathBase))
//                {
//                    Http.Respond404(true);
//                }
//                ImagingUtility.ProcessImage(Path.Combine(tempPathBase, @"images", fn), outputFilePath, null, System.Drawing.Color.Empty, cx, cy, true, true, false, false, false, 0.0, 0, 0, 0, System.Drawing.Color.Empty, 0f);

//                // -- For timing reasons, the ETag may not be generated and throw HttpException. .NET bug...
//                // So fix it when creating a new image file.
//                Thread.Sleep(1); 
//            }
//            else
//            {
//                if (cx == 0 && cy == 0)
//                {
//                    outputFilePath = Path.Combine(tempPathBase, @"images", fn);
//                }
//            }

//            string contentType, ext = Path.GetExtension(outputFilePath).ToLower();
//            switch (ext)
//            {
//                default:
//                case @".jpg":
//                case @".jpeg":
//                    contentType = @"image/jpeg";
//                    break;
//                case @".png":
//                    contentType = @"image/png";
//                    break;
//                case @".gif":
//                    contentType = @"image/gif";
//                    break;
//            }

//            Response.ContentType = contentType;
//            Response.AddFileDependency(outputFilePath);
//            Response.Cache.SetLastModifiedFromFileDependencies();
//            Response.Cache.SetETagFromFileDependencies();
//            if (IsHead)
//            {
//                Response.AddHeader(@"Content-Length", new FileInfo(outputFilePath).Length.ToString());
//            }
//            else
//            {
//                Response.TransmitFile(outputFilePath);
//            }
//        }

//        public void Get(HttpRequest Request, HttpResponse Response, params string[] PathParams)
//        {
//            HandleImageRequest(Request, Response, false);
//        }

//        public void Post(HttpRequest Request, HttpResponse Response, params string[] PathParams)
//        {
//            throw new NotImplementedException();
//        }

//        public void Put(HttpRequest Request, HttpResponse Response, params string[] PathParams)
//        {
//            throw new NotImplementedException();
//        }

//        public void Delete(HttpRequest Request, HttpResponse Response, params string[] PathParams)
//        {
//            throw new NotImplementedException();
//        }

//        public void Head(HttpRequest Request, HttpResponse Response, params string[] PathParams)
//        {
//            HandleImageRequest(Request, Response, false);
//        }
//    }
//}
