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
    public class OrderHandler : ApiHandlerBase
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
                 Int64 OrderId = Request.QueryString["order_id"] != null ? Int64.Parse(Request.QueryString["order_id"].ToString()) : 0;
                 Response.ContentType = @"application/json";
                 using (StreamWriter streamWriter = new StreamWriter(Response.OutputStream))
                 {
                     using (JsonTextWriter jsonWriter = new JsonTextWriter(streamWriter))
                     {

                         OrderUI orderUI = OrderController.GetOrderById(OrderId);
                         jsonWriter.WriteStartObject();

                        jsonWriter.WritePropertyName(@"order_id");
                        jsonWriter.WriteValue(orderUI.OrderId);
                        jsonWriter.WritePropertyName(@"supplier_id");
                        jsonWriter.WriteValue(orderUI.SupplierId);
                        jsonWriter.WritePropertyName(@"order_date");
                        jsonWriter.WriteValue(orderUI.OrderDate.ToShortDateString());
                        jsonWriter.WritePropertyName(@"supplier_name");
                        jsonWriter.WriteValue(orderUI.SupplierName);
                        jsonWriter.WritePropertyName(@"total_price");
                        jsonWriter.WriteValue(orderUI.Price);
                        jsonWriter.WritePropertyName(@"SpecialInstructions");
                        jsonWriter.WriteValue(orderUI.SpecialInstructions);
                        jsonWriter.WritePropertyName(@"is_enable_received");
                        jsonWriter.WriteValue(orderUI.ReceviedDate == null ? (orderUI.OrderDate < DateTime.UtcNow.AddHours(-24) ? false : true) : true);
                        jsonWriter.WritePropertyName(@"isSendReceived");
                        jsonWriter.WriteValue(orderUI.ReceviedDate != null ? true : false); 
                        jsonWriter.WritePropertyName(@"isSupplied");
                        jsonWriter.WriteValue(orderUI.SuppliedDate == null ? false : true);
                        jsonWriter.WritePropertyName(@"products");
                        jsonWriter.WriteStartArray();

                        foreach (BidProductUI item in orderUI.LstProduct)
                        {
                            jsonWriter.WriteStartObject();


                            jsonWriter.WritePropertyName(@"product_id");
                            jsonWriter.WriteValue(item.ProductId);
                            jsonWriter.WritePropertyName(@"product_name");
                            jsonWriter.WriteValue(item.ProductName);
                            jsonWriter.WritePropertyName(@"product_amount");
                            jsonWriter.WriteValue(item.Amount);
                            jsonWriter.WritePropertyName(@"order_amount");
                            jsonWriter.WriteValue(item.Amount);
                            jsonWriter.WritePropertyName(@"product_image");
                            jsonWriter.WriteValue(item.ProductImage);

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
