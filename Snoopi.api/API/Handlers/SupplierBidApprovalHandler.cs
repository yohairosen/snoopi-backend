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
using System.Web;
using Snoopi.core.Properties;

namespace Snoopi.api
{
    public class SupplierBidApprovalHandler : ApiHandlerBase
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

          Int64 supplierId;
          if (IsAuthorizedRequestSupplier(Request, Response, true, out supplierId))
            {
                Response.ContentType = @"application/json";

                try
                {
                    JToken jt;
                    Int64 bidId = 0;
                    bool isApproved = false;
                    if (inputData.TryGetValue(@"bid_id", out jt)) bidId = jt.Value<Int64>();
                    if (inputData.TryGetValue(@"is_approved", out jt)) isApproved = jt.Value<bool>();
                    var supplier = AppSupplier.FetchByID(supplierId);
                    var bid = Bid.FetchByID(bidId);
                    if (bid.IsActive == false)
                        RespondError(Response, HttpStatusCode.NotAcceptable, @"inactive-bid");
                    long orderId = 0;
                    string response = "";
                    if (isApproved)
                    {
                        var order = Order.FetchByBidId(bidId);                  
                        var offerUi = SupplierController.GetBidOfferById(bidId, supplierId);
                        if (offerUi== null || offerUi.BidId<=0)
                            RespondError(Response, HttpStatusCode.NotAcceptable, @"inactive-bid");
                        decimal TotalPrice = offerUi.TotalPrice;
                        var discount = BidController.GetDiscountForUser(TotalPrice, bid.AppUserId.Value);
                        decimal PriceAfterDiscount = Convert.ToDecimal(discount["PriceAfterDiscount"]);
                        decimal PrecentDiscount = Convert.ToDecimal(discount["PrecentDiscount"]);
                        Int64? CampaignId = Convert.ToInt64(discount["CampaignId"]);
                        var paymentDetails = new PaymentDetails
                        {
                            Amount = (float)PriceAfterDiscount * 100,
                            CreditId = order.Transaction,
                            Exp = order.ExpiryDate,
                            AuthNumber = order.AuthNumber,
                            NumOfPayments = order.NumOfPayments,
                            SupplierToken = supplier.MastercardCode
                        };
                        try
                        {
                             response = CreditGuardManager.CreateMPITransaction(paymentDetails);
                        }
                        catch (Exception ex)
                        {
                            Helpers.LogProcessing("SupplierBidApprovalHandler - ex -", "\n exception: " + ex.ToString(), true);

                            endRequest(Response, order.AppUserId, bidId);
                        }
                        if (response != "000")
                        {
                            endRequest(Response, order.AppUserId, bidId);
                        }
                        order.IsSendRecived = false;
                        if (CampaignId != 0)
                            order.CampaignId = CampaignId;
                        order.TotalPrice = TotalPrice;
                        order.PriceAfterDiscount = PriceAfterDiscount;
                        order.PrecentDiscount = PrecentDiscount;
                        order.CreateDate = DateTime.UtcNow;
                        // order.SpecialInstructions = special_instructions;
                        order.BidId = bidId;
                        order.AppUserId = bid.AppUserId.Value;
                        order.UserPaySupplierStatus = UserPaymentStatus.Payed;
                        order.SupplierId = supplierId;
                        order.Save();
                        bid.IsActive = false;
                        bid.Save();
                        var message = BIdMessageController.GetMessageByBidAndSupplier(bidId, supplierId);
                        message.IsActive = false;
                        message.Save();
                        orderId = order.OrderId;
                        //Notification.SendNotificationAppUserSupplierApproved(Snoopi.web.Localization.PushStrings.GetText("SupplierApproved"), bid.AppUserId.Value, order.OrderId);
                    }

                    else
                    {
                        var message = BIdMessageController.GetMessageByBidAndSupplier(bidId, supplierId);
                        message.ExpirationTime = DateTime.Now.AddHours(-1);
                        message.Save();
                    }

                    using (StreamWriter streamWriter = new StreamWriter(Response.OutputStream))
                    {
                        using (JsonTextWriter jsonWriter = new JsonTextWriter(streamWriter))
                        {
                            jsonWriter.WriteStartObject();
                            jsonWriter.WritePropertyName(@"success");
                            jsonWriter.WriteValue(true);
                            jsonWriter.WritePropertyName(@"order_id");
                            jsonWriter.WriteValue(orderId);
                            jsonWriter.WriteEndObject();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Helpers.LogProcessing("SupplierBidApprovalHandler - ex -", "\n exception: " + ex.ToString(), true);
                    RespondError(Response, HttpStatusCode.NotAcceptable, @"inactive-bid");
                }
            }
        }
        private void endRequest(HttpResponse response, long userId, long bidId)
        {
            Notification.SendNotificationAppUserCreditRejected(userId, bidId);
            RespondError(response, HttpStatusCode.NotAcceptable, @"credit-rejected");
        }
    }
}
