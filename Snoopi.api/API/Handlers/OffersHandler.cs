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
    public class OffersHandler : ApiHandlerBase
    {

        public override void Get(HttpRequest Request, HttpResponse Response, params string[] PathParams)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetMaxAge(TimeSpan.Zero);

            try
            {
                //Int64 bid_id = (Request.QueryString["bid_id"] != null ? Int64.Parse(Request.QueryString["bid_id"].ToString()) : 0 );            

                Response.ContentType = @"application/json";
                using (StreamWriter streamWriter = new StreamWriter(Response.OutputStream))
                {
                    using (JsonTextWriter jsonWriter = new JsonTextWriter(streamWriter))
                    {
                        Int64 bid_id = 0;
                        Int64 AppUserId;
                        Int64 TempAppUserId = 0;
                        if (IsAuthorizedRequest(Request, Response, false, out AppUserId))
                        {
                            //the last bid of the app user that ended
                            Bid b = Bid.FetchByAppUserId(AppUserId);
                            if (b != null)
                            {
                                //if the bid isn't closed (there arn't any orders for this bid)
                                Order o = Order.FetchByBidId(b.BidId);
                                //if (o == null)
                                if (o==null||o.UserPaySupplierStatus == UserPaymentStatus.NotPayed)
                                    bid_id = b.BidId;
                            }

                        }
                        else
                        {

                            TempAppUserId = Request["temp_app_user_id"] != null ? Convert.ToInt64(Request["temp_app_user_id"]) : 0;

                            if (TempAppUserId != 0)
                            {
                                //the last bid of the temp app user that ended
                                Bid b = Bid.FetchByTempAppUserId(TempAppUserId);
                                if (b != null)
                                {
                                    //if the bid isn't closed (there arn't any orders for this bid)
                                    Order o = Order.FetchByBidId(b.BidId);
                                    //if (o == null)
                                    if (o == null || o.UserPaySupplierStatus == UserPaymentStatus.NotPayed)
                                        bid_id = b.BidId;
                                }
                            }

                        }

                        Bid bid = Bid.FetchByID(bid_id);
                        if (bid_id == 0 || bid == null)
                        {
                            jsonWriter.WriteStartObject();
                            jsonWriter.WriteEndObject();
                        }
                        else
                        {
                            DateTime DateExpiryOffers = bid.EndDate.AddHours(Convert.ToDouble(Settings.GetSetting(Settings.Keys.EXPIRY_OFFER_TIME_HOURS)));

                            List<OfferUI> lstOfferUI = (bid_id != 0 && bid != null && DateTime.UtcNow < DateExpiryOffers ? OfferController.GetAllOfferByBidId(bid_id) : new List<OfferUI>());
                            jsonWriter.WriteStartObject();

                            jsonWriter.WritePropertyName(@"bid_id");
                            jsonWriter.WriteValue(bid_id);

                            jsonWriter.WritePropertyName(@"products");
                            jsonWriter.WriteStartArray();
                            List<BidProductUI> products = BidController.GetProductsByBid(bid_id);
                            foreach (BidProductUI item in products)
                            {
                                jsonWriter.WriteStartObject();

                                jsonWriter.WritePropertyName(@"product_id");
                                jsonWriter.WriteValue(item.ProductId);
                                jsonWriter.WritePropertyName(@"order_amount");
                                jsonWriter.WriteValue(item.Amount);
                                jsonWriter.WritePropertyName(@"product_name");
                                jsonWriter.WriteValue(item.ProductName);
                                jsonWriter.WritePropertyName(@"product_image");
                                jsonWriter.WriteValue(item.ProductImage);

                                jsonWriter.WriteEndObject();
                            }

                            jsonWriter.WriteEndArray();

                            jsonWriter.WritePropertyName(@"offers");
                            jsonWriter.WriteStartArray();

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
                                jsonWriter.WritePropertyName(@"supplier_name");
                                jsonWriter.WriteValue(item.SupplierName);
                                jsonWriter.WritePropertyName(@"total_price");
                                jsonWriter.WriteValue(item.TotalPrice);
                                jsonWriter.WritePropertyName(@"gift");
                                jsonWriter.WriteValue(item.Gift);

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
