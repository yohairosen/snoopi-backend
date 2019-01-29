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
    public class OrderHistoryHandler : ApiHandlerBase
    {

        public override void Get(HttpRequest Request, HttpResponse Response, params string[] PathParams)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetMaxAge(TimeSpan.Zero);

            try
            {
                
             Int64 AppUserId;
             if (IsAuthorizedRequest(Request, Response, true, out AppUserId))
             {

                 Response.ContentType = @"application/json";
                 using (StreamWriter streamWriter = new StreamWriter(Response.OutputStream))
                 {
                     using (JsonTextWriter jsonWriter = new JsonTextWriter(streamWriter))
                     {

                         List<OrderUI> lstOrderUI = OrderController.GetOrderHistory(AppUserId);
                         jsonWriter.WriteStartObject();

                         jsonWriter.WritePropertyName(@"orders");
                         jsonWriter.WriteStartArray();

                         foreach (OrderUI item in lstOrderUI)
                         {
                             jsonWriter.WriteStartObject();

                             jsonWriter.WritePropertyName(@"order_id");
                             jsonWriter.WriteValue(item.OrderId);
                             jsonWriter.WritePropertyName(@"total_price");
                             jsonWriter.WriteValue(item.TotalPrice);
                             jsonWriter.WritePropertyName(@"supplier_id");
                             jsonWriter.WriteValue(item.SupplierId);
                             jsonWriter.WritePropertyName(@"order_date");
                             jsonWriter.WriteValue(item.OrderDate.ToShortDateString());
                             jsonWriter.WritePropertyName(@"supplier_name");
                             jsonWriter.WriteValue(item.SupplierName);
                             jsonWriter.WritePropertyName(@"bid_id");
                             jsonWriter.WriteValue(item.BidId);

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
