

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
    public class UserCityHandler : ApiHandlerBase
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

            Response.ContentType = @"application/json";
            Int64 AppUserId;
            if (IsAuthorizedRequest(Request, Response, true, out AppUserId))
            {
                JToken jt;
                Int64 cityId = 0;

                if (inputData.TryGetValue(@"city_id", out jt)) cityId = jt.Value<Int64>();
                Query.New<AppUser>()
                           .Update(AppUser.Columns.CityId, cityId)
                           .Where(AppUser.Columns.AppUserId, AppUserId)
                           .Execute();

                using (StreamWriter streamWriter = new StreamWriter(Response.OutputStream))
                {
                    using (JsonTextWriter jsonWriter = new JsonTextWriter(streamWriter))
                    {

                        jsonWriter.WriteStartObject();
                        jsonWriter.WriteEndObject();
                    }
                }
            }
            
        }

    }
}
