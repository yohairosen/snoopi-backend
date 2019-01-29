using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using dg.Utilities.Apns;
using dg.Utilities;
using dg.Utilities.GoogleCloudMessaging;
using System.Web;
using System.IO;
using Snoopi.core.BL;
using dg.Utilities.Imaging;

namespace Snoopi.core
{
    public class MediaUtility
    {


        public static string GetOriginalFilePath(string subFolder, string fileName,Int64 DinamicId = 0)
        {
            string basePath = Settings.GetSetting(Settings.Keys.API_APPUSERS_UPLOAD_FOLDER);
            if (basePath.StartsWith("~")) basePath = HttpContext.Current.Server.MapPath(basePath.Substring(1));
            if (DinamicId != 0)
            {

                if (!string.IsNullOrEmpty(subFolder))
                {
                    subFolder = Path.Combine(subFolder , DinamicId.ToString());
                }
                else
                {
                    subFolder = DinamicId.ToString();
                }
            }

            return Path.Combine(basePath, subFolder, fileName);
        }

        public static string GetAdminImageFileUrl(string fileName, string DinamicId, int w, int h)
        {
            if (string.IsNullOrEmpty(fileName)) return "";
            return @"/web-api/image/?image=" + HttpUtility.UrlEncode(fileName) + "&drive_id=" + DinamicId + "&w=" + w + "&h=" + h;
        }

        public static string GetImageFilePath(string subFolder, string fileName, int w, int h,Int64 DinamicId = 0)
        {
            string basePath = Settings.GetSetting(Settings.Keys.API_APPUSERS_UPLOAD_FOLDER);
            if (basePath.StartsWith("~")) basePath = HttpContext.Current.Server.MapPath(basePath.Substring(1));
            if (DinamicId != 0)
            {

                if (!string.IsNullOrEmpty(subFolder))
                {
                    subFolder = Path.Combine(subFolder , DinamicId.ToString());
                }
                else
                {
                    subFolder = DinamicId.ToString();
                }
            }

            string uploadFolderImage = Path.Combine(basePath, subFolder);

            string thumbnailSubfolder = w.ToString() + 'x' + h.ToString();

            string thumbnailLocal = Path.Combine(uploadFolderImage, thumbnailSubfolder, fileName);

            if (System.IO.File.Exists(thumbnailLocal)) return thumbnailLocal;
            else
            {
                string imageLocal = Path.Combine(uploadFolderImage, fileName);
                if (System.IO.File.Exists(imageLocal))
                {
                    try
                    {
                        if (Folders.VerifyDirectoryExists(System.IO.Path.GetDirectoryName(thumbnailLocal)))
                        {
                            ImagingUtility.ProcessImage(
                                imageLocal, thumbnailLocal, null,
                                System.Drawing.Color.Empty,
                                w, h,
                                true, true, false, false,
                                false, 0.0d,
                                CropAnchor.None,
                                Corner.None,
                                0, System.Drawing.Color.Empty, 0);
                            return thumbnailLocal;
                        }
                    }
                    catch { }
                }
            }
            return string.Empty;
        }

        public static void DeleteImageFilePath(string subFolder, string fileName, int w, int h, Int64 DinamicId = 0,bool isSupplier=false)
        {
            try
            {
                string basePath = Settings.GetSetting(Settings.Keys.API_APPUSERS_UPLOAD_FOLDER);
                if (basePath.StartsWith("~")) basePath = HttpContext.Current.Server.MapPath(basePath.Substring(1));
                if (DinamicId != 0)
                {

                    if (!string.IsNullOrEmpty(subFolder))
                    {
                        subFolder = Path.Combine(subFolder ,DinamicId.ToString());
                    }
                    else
                    {
                        subFolder = DinamicId.ToString();
                    }
                }

                string uploadFolderImage = Path.Combine(basePath, subFolder);

                string thumbnailSubfolder = w.ToString() + 'x' + h.ToString();

                string thumbnailLocal = Path.Combine(uploadFolderImage, thumbnailSubfolder, fileName);

                if (System.IO.File.Exists(thumbnailLocal)) File.Delete(thumbnailLocal);
                else
                {
                    string imageLocal = Path.Combine(uploadFolderImage, fileName);
                    if (System.IO.File.Exists(imageLocal)) File.Delete(imageLocal);
                }
                // delete file from both of the projects
                if(isSupplier)
                {
                    string pathThumbnailLocal = thumbnailLocal.Replace("Suppliers.web console", "Snoopi.web console").Replace("supplier.snoopi-app.com", "app.snoopi-app.com");
                    if (System.IO.File.Exists(pathThumbnailLocal)) File.Delete(pathThumbnailLocal);
                    else
                    {
                        string imageLocal = Path.Combine(uploadFolderImage, fileName).Replace("Suppliers.web console", "Snoopi.web console").Replace("supplier.snoopi-app.com", "app.snoopi-app.com");
                        if (System.IO.File.Exists(imageLocal)) File.Delete(imageLocal);
                    }
                }


            }
            catch (Exception) { }
            
        }

        public static string GetImagePath(string subFolder, string fileName, Int64 DinamicId =0, int w = 0, int h = 0)
        {
            string basePath = Settings.GetSetting(Settings.Keys.API_APPUSERS_UPLOAD_FOLDER);
            if (basePath.StartsWith("~")) basePath = HttpContext.Current.Server.MapPath(basePath.Substring(1));
            if (DinamicId != 0)
            {
                if (!string.IsNullOrEmpty(subFolder))
                {
                    subFolder = Path.Combine(subFolder,DinamicId.ToString());
                }
                else
                {
                    subFolder = DinamicId.ToString();
                }
            }

            string uploadFolderImage = Path.Combine(basePath, subFolder);
            string thumbnailLocal;
            if (w != 0 && h != 0)
            {
                string thumbnailSubfolder = w.ToString() + 'x' + h.ToString();
                thumbnailLocal = Path.Combine(uploadFolderImage, thumbnailSubfolder, fileName);

                if (System.IO.File.Exists(thumbnailLocal)) return thumbnailLocal.Replace(HttpContext.Current.Request.ServerVariables["APPL_PHYSICAL_PATH"], @"\");
                else
                {
                    string imageLocal = Path.Combine(uploadFolderImage, fileName);
                    if (System.IO.File.Exists(imageLocal))
                    {
                        try
                        {   
                            if (Folders.VerifyDirectoryExists(System.IO.Path.GetDirectoryName(thumbnailLocal)))
                            {
                                ImagingUtility.ProcessImage(
                                    imageLocal, thumbnailLocal, null,
                                    System.Drawing.Color.Empty,
                                    w, h,
                                    true, true, false, false,
                                    false, 0.0d,
                                    CropAnchor.None,
                                    Corner.None,
                                    0, System.Drawing.Color.Empty, 0);                                 
                            }
                        }
                        catch { }
                    }
                }
            }
            else
            {
                thumbnailLocal = Path.Combine(uploadFolderImage, fileName);
            }

            return thumbnailLocal.Replace(HttpContext.Current.Request.ServerVariables["APPL_PHYSICAL_PATH"], @"\");

        }

        public static string GetAdminImageFileUrl(Int64 DinamicId, string subFolder, string fileName, int w, int h)
        {
            if (string.IsNullOrEmpty(fileName)) return "";
            return @"/web-api/image/?DinamicId=" + DinamicId + "&image=" + HttpUtility.UrlEncode(fileName) + "&w=" + w + "&h=" + h;
        }

        public static string SaveFile(HttpPostedFile file, string subFolder , Int64 DinamicId = 0,bool isSupplier=false)
        {
            string baseFile = System.IO.Path.GetExtension(file.FileName);
            if (baseFile.Length == 0)
            {
                if (file.ContentType == @"image/jpeg") baseFile = @".jpg";
                else if (file.ContentType == @"image/png") baseFile = @".png";
                else if (file.ContentType == @"video/mp4") baseFile = @".mp4";
                else if (file.ContentType == @"video/mpeg") baseFile = @".mpg";
            }
            if (DinamicId != 0)
            {
                if (!string.IsNullOrEmpty(subFolder))
                {
                    subFolder = Path.Combine(subFolder, DinamicId.ToString());
                }
                else
                {
                    subFolder = DinamicId.ToString();
                }
            }

            string path = Files.AquireUploadFileName(baseFile, Settings.GetSetting(Settings.Keys.API_APPUSERS_UPLOAD_FOLDER), subFolder, true, true, true);
            if (path == null)return "";
            else
            {

                if (string.IsNullOrEmpty(path)) return "";
                else
                {
                    string dirPath = System.IO.Path.GetDirectoryName(path);
                    if (dirPath != null)
                    {
                        if (!System.IO.Directory.Exists(dirPath))
                        {
                            Folders.CreateDirectory(dirPath, false);
                        }
                    }
                    file.SaveAs(path);
                    if (isSupplier)
                        SaveFile(file, path.Replace("Suppliers.web console", "Snoopi.web console").Replace("supplier.snoopi-app.com","app.snoopi-app.com" ));
                    return System.IO.Path.GetFileName(path);
                }
            }
        }

        public static string SaveFile(HttpPostedFile file, string path)
        {

                    string dirPath = System.IO.Path.GetDirectoryName(path);
                    if (dirPath != null)
                    {
                        if (!System.IO.Directory.Exists(dirPath))
                        {
                            Folders.CreateDirectory(dirPath, false);
                        }
                    }
                    file.SaveAs(path);
                    return System.IO.Path.GetFileName(path);
            
        }

        public static string SaveFile(Stream stream, string fileName, string subFolder, Int64 DinamicId = 0)
        {
            if (DinamicId != 0)
            {

                if (!string.IsNullOrEmpty(subFolder))
                {
                    subFolder = Path.Combine(subFolder, DinamicId.ToString());
                }
                else
                {
                    subFolder = DinamicId.ToString();
                }
            }

            string path = Files.AquireUploadFileName(System.IO.Path.GetExtension(fileName), Settings.GetSetting(Settings.Keys.API_APPUSERS_UPLOAD_FOLDER), subFolder, true, true, true);
            if (string.IsNullOrEmpty(path)) return "";
            else
            {
                string dirPath = System.IO.Path.GetDirectoryName(path);
                if (dirPath != null)
                {
                    if (!System.IO.Directory.Exists(dirPath))
                    {
                        Folders.CreateDirectory(dirPath, false);
                    }
                }

                int read = 0;
                byte[] buffer = new byte[8 * 1024];
                using (FileStream file = File.Create(path))
                {
                    while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        file.Write(buffer, 0, read);
                    }
                }

                return System.IO.Path.GetFileName(path);
            }
        }
    }
}
