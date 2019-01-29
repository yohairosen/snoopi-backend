
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
    public class SupplierAdvertismentHandler : ApiHandlerBase
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
            Int64 SupplierId;
            if (IsAuthorizedRequestSupplier(Request, Response, true, out SupplierId))
            {
                JToken jt;
                bool? IsAdv = null;
                if (inputData.TryGetValue(@"is-adv", out jt)) IsAdv = jt.Value<bool>();

                using (StreamWriter streamWriter = new StreamWriter(Response.OutputStream))
                {
                    using (JsonTextWriter jsonWriter = new JsonTextWriter(streamWriter))
                    {

                        Query q = new Query(AppSupplier.TableSchema).Where(AppSupplier.Columns.SupplierId, SupplierId);
                        if (IsAdv != null) q.Update(AppSupplier.Columns.IsAdv, IsAdv);
                        q.Execute();

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

    }
}

