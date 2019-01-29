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

namespace Snoopi.api
{
    public class SupplierNewBidHandler : ApiHandlerBase
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
                         Int64 BidId = Request.QueryString["bid_id"] != null ? Convert.ToInt64(Request.QueryString["bid_id"]) : 0;
                         bool IsService = Request.QueryString["is_service"] != null ? Convert.ToBoolean(Request.QueryString["is_service"]) : false;
                         MainBid mainBid = IsService ? SupplierController.GetNewServiceBidById(BidId, SupplierId) : SupplierController.GetNewBidById(BidId, SupplierId);
                         

                         if (mainBid != null)
                         {
                             jsonWriter.WriteStartObject();
                             jsonWriter.WritePropertyName(@"bid_id");
                             jsonWriter.WriteValue(mainBid.BidId);
                             jsonWriter.WritePropertyName(@"end_time");
                             jsonWriter.WriteValue(mainBid.EndBid);
                             jsonWriter.WritePropertyName(@"city");
                             jsonWriter.WriteValue(mainBid.City);

                             jsonWriter.WritePropertyName(@"products");
                             jsonWriter.WriteStartArray();
                             string strGift = "";
                             decimal totalPrice = 0;
                             if (mainBid.LstProduct != null && mainBid.LstProduct.Count > 0)
                             {
                                 foreach (BidProductUI item in mainBid.LstProduct)
                                 {
                                     jsonWriter.WriteStartObject();

                                     jsonWriter.WritePropertyName(@"product_id");
                                     jsonWriter.WriteValue(item.ProductId);
                                     jsonWriter.WritePropertyName(@"product_name");
                                     jsonWriter.WriteValue(item.ProductName);
                                     jsonWriter.WritePropertyName(@"product_amount");
                                     jsonWriter.WriteValue(item.ProductAmount);
                                     jsonWriter.WritePropertyName(@"product_image");
                                     jsonWriter.WriteValue(item.ProductImage);
                                     jsonWriter.WritePropertyName(@"order_amount");
                                     jsonWriter.WriteValue(item.Amount);
                                     jsonWriter.WritePropertyName(@"product_price");
                                     jsonWriter.WriteValue(item.Price * item.Amount);

                                     strGift += (item.ProductGift != null && item.ProductGift.Trim() != "" ? item.ProductGift + ", " : "");
                                     totalPrice += item.Price * item.Amount;
                                     jsonWriter.WriteEndObject();
                                 }
                             }

                             jsonWriter.WriteEndArray();

                             jsonWriter.WritePropertyName(@"total_price");
                             jsonWriter.WriteValue(totalPrice);

                             int index = strGift.LastIndexOf(",");
                             if (index > 0) strGift = strGift.Substring(0, index);

                             jsonWriter.WritePropertyName(@"gift");
                             jsonWriter.WriteValue(strGift);

                             jsonWriter.WritePropertyName(@"customer_comment");
                             jsonWriter.WriteValue(mainBid.CustomerComment);

                             jsonWriter.WritePropertyName(@"service_name");
                             jsonWriter.WriteValue(mainBid.ServiceName);


                             jsonWriter.WritePropertyName(@"service_id");
                             jsonWriter.WriteValue(mainBid.ServiceId);

                             jsonWriter.WriteEndObject();
                         }
                         else
                         {
                             RespondError(Response, HttpStatusCode.BadRequest, "bid-expiry-date");
                         }
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
                Int64 BidId = 0;
                decimal total_price = 0;
                string gift = null, supplier_remarks = null;
                bool is_service = false;
                if (inputData.TryGetValue(@"bid_id", out jt)) BidId = jt.Value<Int64>();
                if (inputData.TryGetValue(@"total_price", out jt)) total_price = jt.Value<decimal>();
                if (inputData.TryGetValue(@"gift", out jt)) gift = jt.Value<string>();
                if (inputData.TryGetValue(@"supplier_remarks", out jt)) supplier_remarks = Regex.Replace(jt.Value<string>(), @"\p{Cs}", ""); ;
                if (inputData.TryGetValue(@"is_service", out jt)) is_service = jt.Value<bool>();

                using (StreamWriter streamWriter = new StreamWriter(Response.OutputStream))
                {
                    using (JsonTextWriter jsonWriter = new JsonTextWriter(streamWriter))
                    {
                        if (is_service)
                        {
                            OfferService offer = OfferService.FetchByBidIdAndSupplierId(BidId, SupplierId);
                            if (offer != null) RespondError(Response, HttpStatusCode.Found, "offer-already-exists");
                            BidService b = BidService.FetchByID(BidId);
                            if (b != null && b.EndDate <= DateTime.UtcNow) RespondError(Response, HttpStatusCode.BadRequest, "bid-expiry-date");
                            offer = new OfferService();
                            offer.BidId = BidId;
                            offer.CreateDate = DateTime.UtcNow;
                            offer.SupplierRemarks = supplier_remarks;
                            offer.Price = total_price;
                            offer.SupplierId = SupplierId;
                            offer.Save();

                            jsonWriter.WriteStartObject();
                            jsonWriter.WritePropertyName(@"offer_id");
                            jsonWriter.WriteValue(offer.OfferId);
                            jsonWriter.WriteEndObject();
                        }
                        else if (total_price > 0)
                        {
                            Offer offer = Offer.FetchByBidIdAndSupplierId(BidId, SupplierId);
                            if (offer != null) RespondError(Response, HttpStatusCode.Found, "offer-already-exists");
                            Bid b = Bid.FetchByID(BidId);
                            if (b != null && b.EndDate <= DateTime.UtcNow) RespondError(Response, HttpStatusCode.BadRequest, "bid-expiry-date");
                            offer = new Offer();
                            offer.BidId = BidId;
                            offer.CreateDate = DateTime.UtcNow;
                            offer.Gift = gift;
                            offer.Price = total_price;
                            offer.SupplierId = SupplierId;
                            offer.Save();

                            jsonWriter.WriteStartObject();
                            jsonWriter.WritePropertyName(@"offer_id");
                            jsonWriter.WriteValue(offer.OfferId);
                            jsonWriter.WriteEndObject();
                        }
                        else
                        {
                            RespondError(Response, HttpStatusCode.BadRequest, @"price-zero");
                        }

                        

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
