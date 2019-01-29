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
    public class OrderPaymentHandler : ApiHandlerBase
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
                    string response_code = null, card_tk = null, expire_date = null, last4_digits = null, id_number = null, special_instructions = null, response_error_message = null;
                    Int64 bid_id = 0, offer_id = 0, donation_id = 0;
                    Int64 campaign_id = 0;
                    //if (inputData.TryGetValue(@"response_code", out jt)) response_code = jt.Value<string>();
                    //if (inputData.TryGetValue(@"response_error_message", out jt)) response_error_message = jt.Value<string>();
                    if (inputData.TryGetValue(@"card_tk", out jt)) card_tk = jt.Value<string>();
                    if (inputData.TryGetValue(@"expire_date", out jt)) expire_date = jt.Value<string>();
                    if (inputData.TryGetValue(@"special_instructions", out jt)) special_instructions = Regex.Replace(jt.Value<string>(), @"\p{Cs}", ""); 
                    if (inputData.TryGetValue(@"last4_digits", out jt)) last4_digits = jt.Value<string>();
                    if (inputData.TryGetValue(@"id_number", out jt)) id_number = jt.Value<string>();
                    if (inputData.TryGetValue(@"bid_id", out jt)) bid_id = jt.Value<Int64>();
                    if (inputData.TryGetValue(@"offer_id", out jt)) offer_id = jt.Value<Int64>();
                    //if (inputData.TryGetValue(@"donation_id", out jt)) donation_id = jt.Value<Int64>();
                    if (inputData.TryGetValue(@"campaign_id", out jt) && jt != null) campaign_id = jt.Value<Int64?>() ?? 0;


                    using (StreamWriter streamWriter = new StreamWriter(Response.OutputStream))
                    {
                        using (JsonTextWriter jsonWriter = new JsonTextWriter(streamWriter))
                        {
                            if (Order.FetchByBidId(bid_id) != null) RespondError(Response, HttpStatusCode.BadRequest, @"already-order");
                            Order order = new Order();
                            //if (response_code == OrderController.RESPONSE_CODE_OK)
                            //{
                            AppUserCard paymentToken = AppUserCard.FetchByAppUserId(AppUserId);
                            if (paymentToken == null)
                            {
                                paymentToken = new AppUserCard();
                            }
                            paymentToken.AppUserId = AppUserId;
                            paymentToken.CardToken = card_tk;
                            paymentToken.ExpiryDate = expire_date;
                            paymentToken.Last4Digit = last4_digits;
                            if (!String.IsNullOrEmpty(id_number)) paymentToken.IdNumber = id_number;
                            paymentToken.Save();
                            // }


                            Dictionary<string, string> result = BidController.GetDiscount(offer_id, AppUserId);

                            decimal TotalPrice = result["TotalPrice"] != null ? Convert.ToDecimal(result["TotalPrice"].ToString()) : 0;
                            decimal PriceAfterDiscount = result["PriceAfterDiscount"] != null ? Convert.ToDecimal(result["PriceAfterDiscount"].ToString()) : 0;
                            decimal PrecentDiscount = result["PrecentDiscount"] != null ? Convert.ToDecimal(result["PrecentDiscount"].ToString()) : 0;
                            Int64? CampaignId = result["CampaignId"] != null ? (Int64?)Convert.ToInt64(result["CampaignId"].ToString()) : null;
                            if (CampaignId != 0)
                                order.CampaignId = CampaignId;
                            order.TotalPrice = TotalPrice;
                            order.PriceAfterDiscount = PriceAfterDiscount;
                            order.PrecentDiscount = PrecentDiscount;
                            order.BidId = bid_id;
                            order.SpecialInstructions = special_instructions;
                            //order.TransactionResponseCode = response_code;
                            //order.TransactionErrorMessage = response_error_message;
                            order.Transaction = card_tk;
                            //switch (response_code)
                            //{
                            //    case OrderController.RESPONSE_CODE_OK: order.TransactionStatus = OrderStatus.Payed;
                            //        break;
                            //    case OrderController.RESPONSE_CODE_ERROR: order.TransactionStatus = OrderStatus.NotPayed;
                            //        break;
                            //    default: order.TransactionStatus = OrderStatus.NotPayed;
                            //        break;
                            //}
                            // if (donation_id != 0) order.DonationId = donation_id;
                            order.Last4Digits = last4_digits;
                            order.ExpiryDate = expire_date;
                            order.AppUserId = AppUserId;
                            order.Save();

                            jsonWriter.WriteStartObject();
                            jsonWriter.WritePropertyName(@"order_id");
                            jsonWriter.WriteValue(order.OrderId);
                            jsonWriter.WriteEndObject();


                            if (campaign_id != null && campaign_id != 0)
                            {
                                AppUserCampaign appUserCampaign = new AppUserCampaign();
                                appUserCampaign.AppUserId = AppUserId;
                                appUserCampaign.CampaignId = campaign_id;
                                appUserCampaign.Save();
                            }

                            Offer offer = Offer.FetchByID(offer_id);
                           // SupplierNotification.SendNotificationCloseBidToSupplier(order.OrderId, offer.SupplierId);

                            AppSupplier supplier = AppSupplier.FetchByID(offer.SupplierId);
                            if (supplier != null && supplier.StatusJoinBid == true)
                            {
                                supplier.MaxWinningsNum = (supplier.MaxWinningsNum > 0 ? supplier.MaxWinningsNum - 1 : 0);
                                if (supplier.MaxWinningsNum == 0)
                                {
                                    SupplierNotification.SendNotificationMaxAutoModeMessage(supplier.SupplierId);
                                    supplier.StatusJoinBid = false;
                                }
                                supplier.Save();
                            }

                         
                        }
                    }
                }
                catch (Exception)
                {
                    RespondError(Response, HttpStatusCode.InternalServerError, @"db-error");
                }
            }


        }
    }
}