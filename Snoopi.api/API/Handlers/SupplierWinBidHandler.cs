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
    public class SupplierWinBidHandler : ApiHandlerBase
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

                         Int64 OrderId = Request.QueryString["order_id"] != null ? Int64.Parse(Request.QueryString["order_id"].ToString()) : 0;
                         OrderUI orderUI = OrderController.GetOrderById(OrderId, SupplierId);
                         jsonWriter.WriteStartObject();

                         jsonWriter.WritePropertyName(@"order_id");
                         jsonWriter.WriteValue(orderUI.OrderId);
                         jsonWriter.WritePropertyName(@"order_date");
                         jsonWriter.WriteValue(orderUI.OrderDate.ToShortDateString());
                         jsonWriter.WritePropertyName(@"order_time");
                         jsonWriter.WriteValue(orderUI.OrderDate.ToShortTimeString());
                         jsonWriter.WritePropertyName(@"first_name");
                         jsonWriter.WriteValue(orderUI.user != null ? orderUI.user.FirstName : "");
                         jsonWriter.WritePropertyName(@"last_name");
                         jsonWriter.WriteValue(orderUI.user != null ? orderUI.user.LastName : "");
                         jsonWriter.WritePropertyName(@"total_price");
                         jsonWriter.WriteValue(orderUI.Price);
                         jsonWriter.WritePropertyName(@"gift");
                         jsonWriter.WriteValue(orderUI.Gift);
                         jsonWriter.WritePropertyName(@"special_instructions");
                         jsonWriter.WriteValue(orderUI.SpecialInstructions);
                         jsonWriter.WritePropertyName(@"is_supplied");
                         jsonWriter.WriteValue(orderUI.SuppliedDate < DateTime.Now);
                         jsonWriter.WritePropertyName(@"city");
                         jsonWriter.WriteValue(orderUI.City);
                         jsonWriter.WritePropertyName(@"email");
                         jsonWriter.WriteValue(orderUI.user != null ? orderUI.user.Email : "");
                         jsonWriter.WritePropertyName(@"phone");
                         jsonWriter.WriteValue(orderUI.user != null ? orderUI.user.Phone : "");

                         string address = (orderUI.user != null ? String.Format("{0} {1} {2}, {3} {4}, {5} {6}",
                         Snoopi.web.Localization.PushStrings.GetText("Street", new CultureInfo("he-IL")),
                         orderUI.user.Street, orderUI.user.HouseNum,
                         Snoopi.web.Localization.PushStrings.GetText("ApartmentNumber", new CultureInfo("he-IL")),
                         orderUI.user.ApartmentNumber,
                         Snoopi.web.Localization.PushStrings.GetText("Floor", new CultureInfo("he-IL")),
                         orderUI.user.Floor) : "");

                         jsonWriter.WritePropertyName(@"address");
                         jsonWriter.WriteValue(address);
                         

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
                             jsonWriter.WriteValue(item.ProductAmount);
                             jsonWriter.WritePropertyName(@"order_amount");
                             jsonWriter.WriteValue(item.Amount);
                             jsonWriter.WritePropertyName(@"product_image");
                             jsonWriter.WriteValue(item.ProductImage);
                             jsonWriter.WritePropertyName(@"product_price");
                             jsonWriter.WriteValue(item.Amount * item.Price);
                            

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
                Int64 OrderId = 0;
                if (inputData.TryGetValue(@"order_id", out jt)) OrderId = jt.Value<Int64>();

                using (StreamWriter streamWriter = new StreamWriter(Response.OutputStream))
                {
                    using (JsonTextWriter jsonWriter = new JsonTextWriter(streamWriter))
                    {
                        Order order = Order.FetchByID(OrderId);
                        if (order == null) RespondError(Response, HttpStatusCode.Found, "order-not-exists");
                       
                        order.SuppliedDate = DateTime.UtcNow;
                        
                        order.Save();
                       // Notification.SendNotificationAppUserReceviedOrder(Snoopi.web.Localization.PushStrings.GetText("ReceivedOrder"), order.AppUserId, order.OrderId);

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
