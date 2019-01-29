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
    public class OrderReceivedHandler : ApiHandlerBase
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

            Int64 AppUserId;
            if (IsAuthorizedRequest(Request, Response, true, out AppUserId))
            {
                Response.ContentType = @"application/json";

                try
                {
                    JToken jt;
                    Int64 order_id = 0;
                    if (inputData.TryGetValue(@"order_id", out jt)) order_id = jt.Value<Int64>();


                    using (StreamWriter streamWriter = new StreamWriter(Response.OutputStream))
                    {
                        using (JsonTextWriter jsonWriter = new JsonTextWriter(streamWriter))
                        {
                            Order order = Order.FetchByID(order_id);
                            if (order == null) RespondError(Response, HttpStatusCode.BadRequest, "order-not-found");
                            order.ReceivedDate = DateTime.UtcNow;             
                            order.IsSendRecived = true;
                            order.Save();

                            jsonWriter.WriteStartObject();
                            jsonWriter.WriteEndObject();

                        }
                    }
                }
                catch (Exception) { }
            }


        }

    }
}
