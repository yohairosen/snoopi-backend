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
using System.Linq;

namespace Snoopi.api
{
    public class ComparePricesHandler : ApiHandlerBase
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

            Response.ContentType = @"application/json";

            JToken jt;
            JArray products = null;
            var productsOffers = new List<BidProductUI>();
            Dictionary<Int64, int> lstProduct = new Dictionary<Int64, int>();

            Int64 AppUserId;
            IsAuthorizedRequest(Request, Response, false, out AppUserId);

            Int64 TempAppUserId = 0;
            if (inputData.TryGetValue(@"temp_app_user_id", out jt)) TempAppUserId = jt.Value<Int64>();
            if (AppUserId == 0 && TempAppUserId == 0)
            {
                RespondError(Response, HttpStatusCode.Forbidden, @"authorization-error");
                return;
            }
            var user = AppUser.FetchByID(AppUserId);
            var tempUser = TempAppUser.FetchByID(TempAppUserId);
            if (user == null && tempUser == null)
            {
                RespondError(Response, HttpStatusCode.Forbidden, @"authorization-error");
                return;
            }

            long cityId = 0;

            if (user != null)
            {
                bool _locked = user.IsLocked;
                if (_locked)
                {
                    RespondError(Response, HttpStatusCode.BadRequest, @"appuser-locked");
                    return;
                }
                cityId = user.CityId;
            }
            else if (tempUser != null)
                cityId = tempUser.CityId;

            if (inputData.TryGetValue(@"products", out jt)) products = jt.Value<JArray>();
            foreach (JObject obj in products.Children<JObject>())
            {
                Int64 product_id = 0;
                int amount = 1;
                if (obj.TryGetValue(@"product_id", out jt)) product_id = jt.Value<Int64>();
                if (obj.TryGetValue(@"amount", out jt)) amount = jt.Value<int>();
                lstProduct.Add(product_id, amount);
            }

            var lstOfferUI = OfferController.GetAllOfferByProductIds(lstProduct, cityId);
            var now = DateTime.Now;
            bool isCallAvailable = BIdMessageController.IsSystemActive(now);
            Response.ContentType = @"application/json";
            using (StreamWriter streamWriter = new StreamWriter(Response.OutputStream))
            {
                using (JsonTextWriter jsonWriter = new JsonTextWriter(streamWriter))
                {
                    jsonWriter.WriteStartObject();
                   
                   
                    jsonWriter.WritePropertyName(@"is_call_available");
                    jsonWriter.WriteValue(isCallAvailable);
                    
                    jsonWriter.WritePropertyName(@"offers");
                    jsonWriter.WriteStartArray();
                    if (lstOfferUI != null)
                        foreach (OfferUI item in lstOfferUI)
                        {
                            jsonWriter.WriteStartObject();

                            jsonWriter.WritePropertyName(@"offer_id");
                            jsonWriter.WriteValue(item.OfferId);
                            jsonWriter.WritePropertyName(@"supplier_id");
                            jsonWriter.WriteValue(item.SupplierId);
                            jsonWriter.WritePropertyName(@"mastercard_code");
                            if (item.MastercardCode == "")
                            {
                                jsonWriter.WriteValue((-1).ToString());
                            }
                            else
                            {
                                jsonWriter.WriteValue(item.MastercardCode);
                            }
                            jsonWriter.WritePropertyName(@"avg_rate");
                            jsonWriter.WriteValue(item.AvgRate);
                            jsonWriter.WritePropertyName(@"comments_number");
                            jsonWriter.WriteValue(item.NumberOfComments);
                            jsonWriter.WritePropertyName(@"supplier_name");
                            jsonWriter.WriteValue(item.SupplierName);
                            jsonWriter.WritePropertyName(@"total_price");
                            jsonWriter.WriteValue(item.TotalPrice);
                            jsonWriter.WritePropertyName(@"gift");
                            jsonWriter.WriteValue("");

                            jsonWriter.WritePropertyName(@"products");
                            jsonWriter.WriteStartArray();
                            foreach (var product in item.Products) /* TODO: should be deleted*/
                            {
                                jsonWriter.WriteStartObject();

                                jsonWriter.WritePropertyName(@"product_id");
                                jsonWriter.WriteValue(product.ProductId);
                                jsonWriter.WritePropertyName(@"product_price");
                                jsonWriter.WriteValue(product.ProductPrice);
                                jsonWriter.WritePropertyName(@"product_amount");
                                jsonWriter.WriteValue(product.Amount);

                                jsonWriter.WriteEndObject();
                            }

                            jsonWriter.WriteEndArray();

                            jsonWriter.WriteEndObject();
                        }

                    jsonWriter.WriteEndArray();

                    jsonWriter.WriteEndObject();
                }
            }

        }


        //using (StreamWriter streamWriter = new StreamWriter(Response.OutputStream))
        //{
        //    using (JsonTextWriter jsonWriter = new JsonTextWriter(streamWriter))
        //    {
        //        if (BidId == 0)
        //        {
        //            RespondError(Response, HttpStatusCode.BadRequest, "only-one-bid");
        //        }
        //        else
        //        {
        //            jsonWriter.WriteStartObject();
        //            jsonWriter.WritePropertyName("bid_id");
        //            jsonWriter.WriteValue(BidId);
        //            jsonWriter.WriteEndObject();
        //        }
        //    }
        //}



        public override void Put(HttpRequest Request, HttpResponse Response, params string[] PathParams)
        {
            Post(Request, Response, PathParams);
        }

    }
}
