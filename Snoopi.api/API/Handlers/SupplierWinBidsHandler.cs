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
    public class SupplierWinBidsHandler : ApiHandlerBase
    {

        public override void Get(HttpRequest Request, HttpResponse Response, params string[] PathParams)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetMaxAge(TimeSpan.Zero);

            try
            {
                
             Int64 SupplierId;
             if (IsAuthorizedRequestSupplier(Request, Response, true, out SupplierId))
             {

                 Response.ContentType = @"application/json";
                 using (StreamWriter streamWriter = new StreamWriter(Response.OutputStream))
                 {
                     using (JsonTextWriter jsonWriter = new JsonTextWriter(streamWriter))
                     {

                         List<OrderUI> lstOrderUI = SupplierController.GetAllWinBids(SupplierId);
                         jsonWriter.WriteStartObject();

                        jsonWriter.WritePropertyName(@"bids");
                        jsonWriter.WriteStartArray();

                        foreach (OrderUI item in lstOrderUI)
                        {
                            jsonWriter.WriteStartObject();

                            jsonWriter.WritePropertyName(@"order_id");
                            jsonWriter.WriteValue(item.OrderId);
                            jsonWriter.WritePropertyName(@"order_date");
                            jsonWriter.WriteValue(item.OrderDate.ToShortDateString());
                            jsonWriter.WritePropertyName(@"order_time");
                            jsonWriter.WriteValue(item.OrderDate.ToShortTimeString());
                            jsonWriter.WritePropertyName(@"city");
                            jsonWriter.WriteValue(item.City);
                            jsonWriter.WritePropertyName(@"full_name");
                            jsonWriter.WriteValue(item.CustomerName);
                            jsonWriter.WritePropertyName(@"is_supplied");
                            jsonWriter.WriteValue(item.IsSupplied);

                            jsonWriter.WriteEndObject();
                        }

                        jsonWriter.WriteEndArray();

                         jsonWriter.WriteEndObject();
                     }
                 }
             }
            }
            catch (Exception) { }



        }

    }
}
