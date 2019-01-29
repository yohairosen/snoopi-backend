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
    public class CloseOrderPaymentHandler : ApiHandlerBase
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
                    string card_tk = null, expire_date = null, last4_digits = null, id_number = null, special_instructions = null;
                    Int64 order_id = 0;
                    bool is_payment_succesed = true;
                    if (inputData.TryGetValue(@"is_payment_succesed", out jt)) is_payment_succesed = jt.Value<bool>();
                    if (is_payment_succesed)
                    {
                        if (inputData.TryGetValue(@"card_tk", out jt)) card_tk = jt.Value<string>();
                        if (inputData.TryGetValue(@"expire_date", out jt)) expire_date = jt.Value<string>();
                        if (inputData.TryGetValue(@"last4_digits", out jt)) last4_digits = jt.Value<string>();
                        if (inputData.TryGetValue(@"id_number", out jt)) id_number = jt.Value<string>();
                    }
                    if (inputData.TryGetValue(@"order_id", out jt)) order_id = jt.Value<Int64>();



                    using (StreamWriter streamWriter = new StreamWriter(Response.OutputStream))
                    {
                        using (JsonTextWriter jsonWriter = new JsonTextWriter(streamWriter))
                        {
                            if (Order.FetchByOrderId(order_id) == null) RespondError(Response, HttpStatusCode.BadRequest, @"order not exist");
                            Order order = Order.FetchByOrderId(order_id);
                            if (is_payment_succesed)
                            {
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

                                order.Transaction = card_tk;
                                order.Last4Digits = last4_digits;
                                order.ExpiryDate = expire_date;
                                order.AppUserId = AppUserId;
                                order.UserPaySupplierStatus = UserPaymentStatus.Payed;
                            }
                            else
                            {
                                order.UserPaySupplierStatus = UserPaymentStatus.NotPayed;
                            }
                            order.Save();

                            jsonWriter.WriteStartObject();
                            jsonWriter.WritePropertyName(@"order_id");
                            jsonWriter.WriteValue(order.OrderId);
                            jsonWriter.WriteEndObject();

                            long supplierId = 0; // need to update from offer
                            SupplierNotification.SendNotificationCloseBidToSupplier(order.OrderId, supplierId );

                            AppSupplier supplier = AppSupplier.FetchByID(supplierId);
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
