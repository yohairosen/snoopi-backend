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
using System.Text.RegularExpressions;
using Snoopi.core;

namespace Snoopi.api
{
    public class ProductOrderHandler : ApiHandlerBase
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
                    string card_tk = null, expire_date = null,authNumber = null, last4_digits = null, id_number = null, special_instructions = null;
                    //Int64 order_id = 0;
                    JArray products = null;
                    Int64 supplierId = 0;
                    int numOfPayments = 1;
                    decimal totalPrice = 0;
                    var lstProduct = new Dictionary<Int64, int>();

                    if (inputData.TryGetValue(@"card_tk", out jt)) card_tk = jt.Value<string>();
                    if (inputData.TryGetValue(@"expire_date", out jt)) expire_date = jt.Value<string>();
                    if (inputData.TryGetValue(@"last4_digits", out jt)) last4_digits = jt.Value<string>();
                    if (inputData.TryGetValue(@"id_number", out jt)) id_number = jt.Value<string>();
                    if (inputData.TryGetValue(@"products", out jt)) products = jt.Value<JArray>();
                    if (inputData.TryGetValue(@"supplier_id", out jt)) supplierId = jt.Value<Int64>();
                    if (inputData.TryGetValue(@"total_price", out jt) && jt != null) totalPrice = jt.Value<decimal>();
                    if (inputData.TryGetValue(@"auth_num", out jt) && jt != null) authNumber = jt.Value<string>();
                    if (inputData.TryGetValue(@"special_instructions", out jt) && jt != null) special_instructions = jt.Value<string>();
                    if (inputData.TryGetValue(@"num_of_payments", out jt) && jt != null) numOfPayments = jt.Value<int>();
                    foreach (JObject obj in products.Children<JObject>())
                    {
                        Int64 product_id = 0;
                        int amount = 1;
                        if (obj.TryGetValue(@"product_id", out jt)) product_id = jt.Value<Int64>();
                        if (obj.TryGetValue(@"amount", out jt)) amount = jt.Value<int>();
                        lstProduct.Add(product_id, amount);
                    }

                    bool isPriceValid = false;
                    if (supplierId > 0 && totalPrice > 0)
                        isPriceValid = OfferController.IsOfferStillValid(lstProduct, supplierId, totalPrice);
                    if (!isPriceValid)
                        RespondError(Response, HttpStatusCode.ExpectationFailed, @"price-not-valid");
                   
                    var results = new ProcessingResults {
                        AuthNumber = authNumber,
                        CardExpiration = expire_date,
                        CardToken = card_tk,
                        NumOfPayments = numOfPayments,
                        SpecialInstructions = special_instructions,
                        Last4Digits = last4_digits                      
                    };
                    using (StreamWriter streamWriter = new StreamWriter(Response.OutputStream))
                    {
                        using (JsonTextWriter jsonWriter = new JsonTextWriter(streamWriter))
                        {
                            string gifts;
                            var bidId = BidController.CreateBidProduct(AppUserId, supplierId, lstProduct, true, out gifts);
                            var order = OrderController.GenerateNewOrder(results, AppUserId,  bidId, gifts, supplierId, totalPrice,Source.Application);
                            var offerProducts = ProductController.GetProductsByBid(order.BidId);
                            jsonWriter.WriteStartObject();
                            jsonWriter.WritePropertyName(@"products");
                            jsonWriter.WriteStartArray();
                            foreach (var product in offerProducts)
                            {
                                jsonWriter.WriteStartObject();
                                jsonWriter.WritePropertyName(@"product_id");
                                jsonWriter.WriteValue(product.ProductId);
                                jsonWriter.WritePropertyName(@"product_name");
                                jsonWriter.WriteValue(product.ProductName);
                                jsonWriter.WritePropertyName(@"product_category");
                                jsonWriter.WriteValue(product.CategoryName);
                                jsonWriter.WritePropertyName(@"product_sub_category");
                                jsonWriter.WriteValue(product.SubCategoryName);
                                jsonWriter.WritePropertyName(@"product_animal_name");
                                jsonWriter.WriteValue(product.AnimalName);
                                jsonWriter.WriteEndObject();
                            }
                            jsonWriter.WriteEndArray();
                            jsonWriter.WritePropertyName(@"total_price");
                            jsonWriter.WriteValue(order.TotalPrice);
                            jsonWriter.WritePropertyName(@"order_id");
                            jsonWriter.WriteValue(order.OrderId);
                            jsonWriter.WritePropertyName(@"bid_id");
                            jsonWriter.WriteValue(order.BidId);
                            jsonWriter.WriteEndObject();

                        }
                    }
                }

                catch (Exception ex)
                {
                    Helpers.LogProcessing("ProductOrderHandler - ex -", "\n exception: " + ex.ToString(), true);
                    RespondError(Response, HttpStatusCode.InternalServerError, @"db-error");
                }
            }


        }
    }
}

