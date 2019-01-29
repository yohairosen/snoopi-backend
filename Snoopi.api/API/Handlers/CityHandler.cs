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

namespace Snoopi.api
{
    public class CityHandler : ApiHandlerBase
    {

        public override void Get(HttpRequest Request, HttpResponse Response, params string[] PathParams)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetMaxAge(TimeSpan.Zero);

            Response.ContentType = @"application/json";

            using (StreamWriter streamWriter = new StreamWriter(Response.OutputStream))
            {
                using (JsonTextWriter jsonWriter = new JsonTextWriter(streamWriter))
                {
                    CityCollection c = CityCollection.FetchAll();

                    jsonWriter.WriteStartObject();

                    jsonWriter.WritePropertyName(@"cities-title");
                    jsonWriter.WriteValue(Settings.GetSetting(Settings.Keys.TITLE_CITIES) ?? @"");

                    jsonWriter.WritePropertyName(@"cities");
                    jsonWriter.WriteStartArray();


                    foreach (City item in c)
                    {
                        jsonWriter.WriteStartObject();

                        jsonWriter.WritePropertyName(@"city_id");
                        jsonWriter.WriteValue(item.CityId);
                        jsonWriter.WritePropertyName(@"city_name");
                        jsonWriter.WriteValue(item.CityName);

                        jsonWriter.WriteEndObject();


                    }

                    jsonWriter.WriteEndArray();

                    jsonWriter.WriteEndObject();

                }
            }
        }

        public override void Post(HttpRequest Request, HttpResponse Response, params string[] PathParams)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetMaxAge(TimeSpan.Zero);

            JObject inputData = null;
            try
            {
                using (StreamReader reader = new StreamReader(Request.InputStream))
                {
                    using (JsonTextReader jsonReader = new JsonTextReader(reader))
                    {
                        inputData = JObject.Load(jsonReader);
                    }
                }
            }
            catch
            {
                RespondBadRequest(Response);
            }

            Int64 AppUserId;
            if (IsAuthorizedRequest(Request, Response, true, out AppUserId))
            {
                Response.ContentType = @"application/json";

                JToken jt;
                string animal_type = null, animal_name = null, animal_age = null;

                if (inputData.TryGetValue(@"animal_type", out jt)) animal_type = jt.Value<string>();
                if (inputData.TryGetValue(@"animal_name", out jt)) animal_name = jt.Value<string>();
                if (inputData.TryGetValue(@"animal_age", out jt)) animal_age = jt.Value<string>();

                using (StreamWriter streamWriter = new StreamWriter(Response.OutputStream))
                {
                    using (JsonTextWriter jsonWriter = new JsonTextWriter(streamWriter))
                    {
                        AppUserAnimal appUserAnimal = AppUserAnimal.FetchByID(AppUserId);
                        if (appUserAnimal == null) appUserAnimal = new AppUserAnimal();
                        if (animal_type != null) appUserAnimal.AnimalType = animal_type;
                        if (animal_name != null) appUserAnimal.AnimalName = animal_name;
                        if (animal_age != null) appUserAnimal.AnimagAge = animal_age;
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
