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
using GoogleMaps.LocationServices;

namespace Snoopi.api
{
    public class TempAppUserHandler : ApiHandlerBase
    {
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

            string device_udid = inputData.Value<string>(@"device_udid") ?? "";
            Int64 city_id = inputData.Value<Int64>(@"city_id") != null ? inputData.Value<Int64>(@"city_id") : 0;

            Response.ContentType = @"application/json";

            using (StreamWriter streamWriter = new StreamWriter(Response.OutputStream))
            {
                using (JsonTextWriter jsonWriter = new JsonTextWriter(streamWriter))
                {

                    TempAppUser temp = new TempAppUser();
                    temp.CityId = city_id;
                    temp.DeviceUdid = device_udid;
                    City c = City.FetchByID(city_id);
                    if (c != null)
                    {
                        var locationService = new GoogleLocationService();
                        var point = locationService.GetLatLongFromAddress(c.CityName);
                        temp.Location = (point != null ? new dg.Sql.Geometry.Point(point.Latitude, point.Longitude) : new dg.Sql.Geometry.Point(0, 0));
                    }
                    temp.Save();

                    jsonWriter.WriteStartObject();

                    jsonWriter.WritePropertyName(@"temp_app_user_id");
                    jsonWriter.WriteValue(temp.TempAppUserId);

                    jsonWriter.WriteEndObject();

                }
            }
        }
   

        public override void Put(HttpRequest Request, HttpResponse Response, params string[] PathParams)
        {
            Post(Request, Response, PathParams);
        }
    }
}
