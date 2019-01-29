using dg.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Snoopi.core;
using Snoopi.core.BL;
using Snoopi.core.DAL;
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
    class ProcessingResultsHandler : ApiHandlerBase
    {

        public override void Get(HttpRequest Request, HttpResponse Response, params string[] PathParams)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetMaxAge(TimeSpan.Zero);
            Response.ContentType = @"application/json";

            try
            {
                string transactionId = Request["txId"];
                var results = CreditGuardManager.QueryMPITransaction(transactionId);
                string redirectUrl = AppConfig.GetString(@"cgSuccessRedirectUrl", @"");

                if (results.ResultCode != "000")
                {
                    Response.ClearContent();
                    Response.StatusCode = 200;
                    Response.ContentType = @"text/plain";
                    Response.Output.Write("שגיאה נכשלה ! " + results.ErrorMessage + " מספר שגיאה: " + results.ResultCode + " אנא פנה לשירות לקוחות ");
                    Response.End();
                }

                var preOrder = PreOrder.FetchByID(results.UniqueId);
                if (preOrder == null)
                {
                    Helpers.LogProcessing("ProcessingResultsHandler - preorder not found -", "\n exception: " + results.UniqueId, false);
                    RespondError(Response, HttpStatusCode.OK, "preorder");
                }

                results.SpecialInstructions = Request["userData3"];
                redirectUrl = String.Format(redirectUrl, true);
                long AppUserId = Convert.ToInt64(Request["userData4"]);
                results.NumOfPayments = Convert.ToInt32(Request["userData2"]);
                var bid = Bid.FetchByID(preOrder.BidId);
                bid.IsActive = true;
                bid.Save();
                var products = ProductController.GetProductsByBid(bid.BidId);
                var order = OrderController.GenerateNewOrder(results, AppUserId, preOrder.BidId, preOrder.Gifts, preOrder.SupplierId, preOrder.TotalPrice, Source.WebSite);

                var sb = new StringBuilder();
                var sw = new StringWriter(sb);
                using (var jsonWriter = new JsonTextWriter(sw))
                {
                    jsonWriter.WriteStartObject();
                    jsonWriter.WritePropertyName(@"isSuccess");
                    jsonWriter.WriteValue(results != null);
                    jsonWriter.WritePropertyName(@"total_price");
                    jsonWriter.WriteValue(preOrder.TotalPrice);
                    jsonWriter.WritePropertyName(@"bid_id");
                    jsonWriter.WriteValue(bid.BidId);
                    jsonWriter.WritePropertyName(@"products");
                    jsonWriter.WriteStartArray();
                    foreach (var product in products)
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
                    jsonWriter.WriteEndObject();
                }
                Response.Redirect(redirectUrl + "&results=" + sb.ToString(), false);
            }

            catch (Exception ex)
            {
                Helpers.LogProcessing("ProcessingResultsHandler - ex -", "\n exception: " + ex.ToString(), true);
            }
        }

        void ReturnError(HttpResponse Response, string url, string message)
        {
            url = String.Format(url, false);
            url = url + "&message=" + message;
            Response.Redirect(url, false);
        }
    }
}
