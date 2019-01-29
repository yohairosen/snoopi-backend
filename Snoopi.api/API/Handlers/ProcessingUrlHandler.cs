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
    public class ProcessingUrlHandler : ApiHandlerBase
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
                    string specialInstruction = null, masterCardNumber = null;
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

                    bool isNumberOfPaymentsValid = numberOfPayments == 3 && totalPrice > 239 ||
                                                    numberOfPayments == 2 && totalPrice >= 150 ||
                                                    (totalPrice / 100 / numberOfPayments > 1 && numberOfPayments <= 12);
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

                    Response.ContentType = @"application/json";
                    using (StreamWriter streamWriter = new StreamWriter(Response.OutputStream))
                    {
                        using (JsonTextWriter jsonWriter = new JsonTextWriter(streamWriter))
                        {
                            string gifts;
                            Random rand = new Random();
                            long uniqueID = DateTime.Now.Ticks + rand.Next(0, 1000);
                            string tansactionId;
                            string url = CreditGuardManager.GetCgUrl(AppUserId, totalPrice, uniqueID, numberOfPayments, masterCardNumber, specialInstruction, out tansactionId);

                            var bidId = BidController.CreateBidProduct(AppUserId, supplierId, lstProduct, false, out gifts);
                            var preOrder = new Snoopi.core.DAL.PreOrder
                            {
                                BidId = bidId,
                                TotalPrice = totalPrice,
                                UniqueId = uniqueID,
                                TransactionId = tansactionId,
                                SupplierId = supplierId,
                                Created = DateTime.Now,
                                Gifts = gifts
                            };
                            preOrder.Save();

                            jsonWriter.WriteStartObject();
                            jsonWriter.WritePropertyName(@"url");
                            jsonWriter.WriteValue(url);
                            jsonWriter.WriteEndObject();
                        }
                    }
                }
            }
            catch (Exception e)
            {

                Helpers.LogProcessing("ProcessingUrlHandler - ex -", "\n exception: " + e.ToString(), true);
            }
        }
    }
}
