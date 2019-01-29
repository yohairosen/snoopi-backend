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
    public class ServicesAllHandler : ApiHandlerBase
    {

        public override void Get(HttpRequest Request, HttpResponse Response, params string[] PathParams)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetMaxAge(TimeSpan.Zero);

            try
            {
                Response.ContentType = @"application/json";
                using (StreamWriter streamWriter = new StreamWriter(Response.OutputStream))
                {
                    using (JsonTextWriter jsonWriter = new JsonTextWriter(streamWriter))
                    {

                        
                        jsonWriter.WriteStartObject();

                        jsonWriter.WritePropertyName(@"services");
                        jsonWriter.WriteStartArray();

                        foreach (Service item in ServiceController.GetAllService())
                        {
                            jsonWriter.WriteStartObject();

                            jsonWriter.WritePropertyName(@"service_id");
                            jsonWriter.WriteValue(item.ServiceId);
                            jsonWriter.WritePropertyName(@"service_name");
                            jsonWriter.WriteValue(item.ServiceName);

                            jsonWriter.WriteEndObject();
                        }

                        jsonWriter.WriteEndArray();

                        jsonWriter.WriteEndObject();
                    }
                }
            }
            catch (Exception) { }



        }

    }
}
