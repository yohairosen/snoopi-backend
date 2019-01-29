using dg.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Snoopi.core;
using Snoopi.core.BL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Snoopi.api
{
    public class SavedCardProcessingHandler : ApiHandlerBase
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

            try
            {
                Int64 AppUserId;
                if (IsAuthorizedRequest(Request, Response, true, out AppUserId))
                {
                    JToken jt;
                    string specialInstruction = null, masterCardNumber = null, cardToken = null, cardExp = null;
                    JArray products = null;
                    Int64 supplierId = 0;
                    int numberOfPayments = 1;
                    decimal totalPrice = 0;
                    var lstProduct = new Dictionary<Int64, int>();

                    if (inputData.TryGetValue(@"products", out jt)) products = jt.Value<JArray>();
                    if (inputData.TryGetValue(@"supplier_id", out jt)) supplierId = jt.Value<Int64>();
                    if (inputData.TryGetValue(@"total_price", out jt) && jt != null) totalPrice = jt.Value<decimal>();
                    if (inputData.TryGetValue(@"special_instructions", out jt) && jt != null) specialInstruction = jt.Value<string>();
                    if (inputData.TryGetValue(@"mastercardCode", out jt) && jt != null) masterCardNumber = jt.Value<string>();
                    if (inputData.TryGetValue(@"num_of_payments", out jt) && jt != null) numberOfPayments = jt.Value<int>();
                    if (inputData.TryGetValue(@"card_token", out jt) && jt != null) cardToken = jt.Value<string>();
                    if (inputData.TryGetValue(@"card_exp", out jt) && jt != null) cardExp = jt.Value<string>();

                    bool isNumberOfPaymentsValid = numberOfPayments == 3 && totalPrice > 239 ||
                                                   numberOfPayments == 2 && totalPrice >= 150 ||
                                                   totalPrice / 100 / numberOfPayments > 1;
                    if (!isNumberOfPaymentsValid)
                        RespondError(Response, HttpStatusCode.OK, @"num-of-payments-not-valid");

                    foreach (JObject obj in products.Children<JObject>())
                    {
                        Int64 product_id = 0;
                        int amount = 1;
                        if (obj.TryGetValue(@"product_id", out jt)) product_id = jt.Value<Int64>();
                        if (obj.TryGetValue(@"amount", out jt)) amount = jt.Value<int>();
                        lstProduct.Add(product_id, amount);
                    }
                    string token = Request.Headers["Authorization"].Substring(6);
                    bool isPriceValid = false;

                    if (supplierId > 0 && totalPrice > 0)
                        isPriceValid = OfferController.IsOfferStillValid(lstProduct, supplierId, totalPrice);
                    if (!isPriceValid)
                        RespondError(Response, HttpStatusCode.ExpectationFailed, @"price-not-valid");

                    string gifts;
                    Random rand = new Random();
                    long uniqueID = DateTime.Now.Ticks + rand.Next(0, 1000);
                    string tansactionId;
                    var results = CreditGuardManager.ProcessSavedCard(AppUserId, totalPrice, numberOfPayments, masterCardNumber, specialInstruction, cardToken, cardExp, out tansactionId);
                    if (results.ResultCode != "000")
                        RespondError(Response, HttpStatusCode.ExpectationFailed, @"failed");


                    results.SpecialInstructions = specialInstruction;
                    results.NumOfPayments = numberOfPayments;
                    var bidId = BidController.CreateBidProduct(AppUserId, supplierId, lstProduct, true, out gifts);
                    var order = OrderController.GenerateNewOrder(results, AppUserId, bidId, gifts, supplierId, totalPrice, core.DAL.Source.WebSite);
                    var productsParams = ProductController.GetProductsWithIds(lstProduct.Select(x => x.Key));
                    using (StreamWriter streamWriter = new StreamWriter(Response.OutputStream))
                    {
                        using (JsonTextWriter jsonWriter = new JsonTextWriter(streamWriter))
                        {
                            jsonWriter.WriteStartObject();
                            jsonWriter.WritePropertyName(@"isSuccess");
                            jsonWriter.WriteValue(results != null);
                            jsonWriter.WritePropertyName(@"total_price");
                            jsonWriter.WriteValue(totalPrice);
                            jsonWriter.WritePropertyName(@"bid_id");
                            jsonWriter.WriteValue(bidId);


                            jsonWriter.WritePropertyName(@"products");
                            jsonWriter.WriteStartArray();
                            foreach (var product in productsParams) 
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
                                jsonWriter.WritePropertyName(@"product_quentity");
                                jsonWriter.WriteValue(lstProduct[product.ProductId]);
                                jsonWriter.WriteEndObject();
                            }

                            jsonWriter.WriteEndArray();

                            jsonWriter.WriteEndObject();
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Helpers.LogProcessing("SavedCardProcessingHandler - ex -", "\n exception: " + ex.ToString(), true);
            }
        }

    }
}
