using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using dg.Utilities;
using Snoopi.core.BL;
using System.IO;
using dg.Utilities.Imaging;
using dg.Utilities.Encryption;
using dg.Utilities.WebApiServices;
using Snoopi.core.DAL;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net;
using dg.Sql;
using dg.Sql.Connector;
using System.Globalization;
using GoogleMaps.LocationServices;
using Snoopi.core;

namespace Snoopi.api
{
    public class MyAnimalHandler : ApiHandlerBase
    {

        public override void Get(HttpRequest Request, HttpResponse Response, params string[] PathParams)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetMaxAge(TimeSpan.Zero);

            Int64 AppUserId;
            if (IsAuthorizedRequest(Request, Response, true, out AppUserId))
            {
                Response.ContentType = @"application/json";

                using (StreamWriter streamWriter = new StreamWriter(Response.OutputStream))
                {
                    using (JsonTextWriter jsonWriter = new JsonTextWriter(streamWriter))
                    {
                        AppUserAnimal appUserAnimal = AppUserAnimal.FetchByID(AppUserId);

                        jsonWriter.WriteStartObject();

                        jsonWriter.WritePropertyName(@"animal_type");
                        jsonWriter.WriteValue(appUserAnimal != null ? appUserAnimal.AnimalType : "");
                        jsonWriter.WritePropertyName(@"animal_age");
                        jsonWriter.WriteValue(appUserAnimal != null ? appUserAnimal.AnimagAge : "");
                        jsonWriter.WritePropertyName(@"animal_name");
                        jsonWriter.WriteValue(appUserAnimal != null ? appUserAnimal.AnimalName : "");
                        jsonWriter.WritePropertyName(@"animal_image");
                        jsonWriter.WriteValue(appUserAnimal != null ? appUserAnimal.AnimalImg : "");

                        jsonWriter.WriteEndObject();

                    }
                }
            }
        }

        private static string[] AcceptedImageExtensions = new string[] { @".jpg", @".jpeg", @".png" };
        public static bool IsAcceptedImageExtension(string fileName)
        {
            string ext = System.IO.Path.GetExtension(fileName).ToLower();
            return AcceptedImageExtensions.Contains(ext);
        }

        public override void Post(HttpRequest Request, HttpResponse Response, params string[] PathParams)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetMaxAge(TimeSpan.Zero);


            Int64 AppUserId;
            if (IsAuthorizedRequest(Request, Response, true, out AppUserId))
            {
                Response.ContentType = @"application/json";

                string animal_type = null, animal_name = null, animal_age = null;

                if (Request.Form["animal_type"] != null) animal_type = (Request.Form["animal_type"]).ToString();
                if (Request.Form["animal_name"] != null) animal_name = (Request.Form["animal_name"]).ToString();
                if (Request.Form["animal_age"] != null) animal_age = (Request.Form["animal_age"]).ToString();
                
                using (StreamWriter streamWriter = new StreamWriter(Response.OutputStream))
                {
                    using (JsonTextWriter jsonWriter = new JsonTextWriter(streamWriter))
                    {
                        string fn = null;
                        if (Request.Files.Count == 1)
                        {
                            if (!IsAcceptedImageExtension(Request.Files[0].FileName))
                            {
                                RespondUnauthorized(Response);
                            }
                            fn = MediaUtility.SaveFile(Request.Files[0], @"Animal", AppUserId);
                            if (string.IsNullOrEmpty(fn))
                            {
                                RespondInternalServerError(Response);
                            }
                        }
                        Int32 w = Request.Form["w"] != null ? Convert.ToInt32(Request.Form["w"]) : 0;
                        Int32 h = Request.Form["h"] != null ? Convert.ToInt32(Request.Form["h"]) : 0;

                        AppUserAnimal appUserAnimal = AppUserAnimal.FetchByID(AppUserId);
                        if (appUserAnimal != null && appUserAnimal.AnimalImg != "" && fn != null) MediaUtility.DeleteImageFilePath(@"Animal", appUserAnimal.AnimalImg, w, h, AppUserId);
                        if (appUserAnimal == null) appUserAnimal = new AppUserAnimal();
                        if (animal_type != null) appUserAnimal.AnimalType = animal_type;
                        if (animal_name != null) appUserAnimal.AnimalName = animal_name;
                        if (animal_age != null) appUserAnimal.AnimagAge = animal_age;
                        if (fn != null && fn !="") appUserAnimal.AnimalImg = fn;
                        appUserAnimal.AppUserId = AppUserId;
                        appUserAnimal.Save();

                        jsonWriter.WriteStartObject();
                        jsonWriter.WriteEndObject();
                    }
                }
            }
        }

        public override void Put(HttpRequest Request, HttpResponse Response, params string[] PathParams)
        {
            Post(Request, Response, PathParams);
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
                        Query.New<AppUserAnimal>().Where(AppUserAnimal.Columns.AppUserId, AppUserId).Delete().Execute();

                        jsonWriter.WriteStartObject();
                        jsonWriter.WriteEndObject();
                    }
                }
            }
        }
    }
}
