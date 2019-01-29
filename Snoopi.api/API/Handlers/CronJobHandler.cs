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
using Snoopi.core;
using Snoopi.web.Localization;
using System.Threading.Tasks;

namespace Snoopi.api
{
    public class CronJobHandler : ApiHandlerBase
    {
        private void HandleAll(HttpRequest Request, HttpResponse Response, params string[] PathParams)
        {
            if (!Request.IsLocal)
            {
                Http.Respond404(true);
            }

            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetMaxAge(TimeSpan.Zero);

            if (PathParams[0] == @"rematch")
            {
                using (StreamWriter streamWriter = new StreamWriter(Response.OutputStream))
                {
                    using (JsonTextWriter jsonWriter = new JsonTextWriter(streamWriter))
                    {
                        jsonWriter.WriteStartObject();
                        try
                        {

                            jsonWriter.WritePropertyName(@"success");
                            jsonWriter.WriteValue(true);
                        }
                        catch (System.Exception ex)
                        {
                            jsonWriter.WritePropertyName(@"error");
                            jsonWriter.WriteValue(@"unknown");
                            jsonWriter.WritePropertyName(@"description");
                            jsonWriter.WriteValue(ex.ToString());
                        }
                        jsonWriter.WriteEndObject();
                    }
                }
            }
            else if (PathParams[0] == @"clean_tokens")
            {
                using (StreamWriter streamWriter = new StreamWriter(Response.OutputStream))
                {
                    using (JsonTextWriter jsonWriter = new JsonTextWriter(streamWriter))
                    {
                        jsonWriter.WriteStartObject();
                        try
                        {
                            AuthTokens.DeleteAllExpired();


                            jsonWriter.WritePropertyName(@"success");
                            jsonWriter.WriteValue(true);
                        }
                        catch (System.Exception ex)
                        {
                            jsonWriter.WritePropertyName(@"error");
                            jsonWriter.WriteValue(@"unknown");
                            jsonWriter.WritePropertyName(@"description");
                            jsonWriter.WriteValue(ex.ToString());
                        }
                        jsonWriter.WriteEndObject();
                    }
                }
            }
            else if (PathParams[0] == @"offer")
            {
                //using (StreamWriter streamWriter = new StreamWriter(Response.OutputStream))
                //{
                //    using (JsonTextWriter jsonWriter = new JsonTextWriter(streamWriter))
                //    {
                //        jsonWriter.WriteStartObject();
                //        try
                //        {
                //            Query qry = new Query(Bid.TableSchema);
                //            qry.Where(Bid.Columns.IsSendOffer, WhereComparision.EqualsTo, false);
                //            qry.AddWhere(Bid.Columns.EndDate, WhereComparision.LessThan, DateTime.UtcNow);

                //            BidCollection bidCollection = BidCollection.FetchByQuery(qry);


                //            Query.New<Bid>().Where(Bid.Columns.IsSendOffer, WhereComparision.EqualsTo, false)
                //                .AddWhere(Bid.Columns.EndDate, WhereComparision.LessThan, DateTime.UtcNow)
                //                .Update(Bid.Columns.IsSendOffer, true)
                //                .Execute();

                //            foreach (Bid item in bidCollection)
                //            {
                //                Query q = new Query(Offer.TableSchema);
                //                q.Where(Offer.Columns.BidId, WhereComparision.EqualsTo, item.BidId);

                //                OfferCollection offerCollection = OfferCollection.FetchByQuery(q);
                //                if (offerCollection != null && offerCollection.Count > 0)
                //                {
                //                    if (item.AppUserId != null && item.AppUserId != 0)
                //                    {
                //                        Notification.SendNotificationAppUserOffers(string.Format(Snoopi.web.Localization.PushStrings.GetText("PushOfferText"), offerCollection.Count), (Int64)item.AppUserId, item.BidId);
                //                    }
                //                    else if (item.TempAppUserId != null && item.TempAppUserId != 0)
                //                    {
                //                        Notification.SendNotificationTempUserOffers(string.Format(Snoopi.web.Localization.PushStrings.GetText("PushOfferText"), offerCollection.Count), (Int64)item.TempAppUserId, item.BidId);
                //                    }
                //                }
                //                else
                //                {
                //                    if (item.AppUserId != null && item.AppUserId != 0)
                //                    {
                //                        Notification.SendNotificationAppUserOffers(Snoopi.web.Localization.PushStrings.GetText("NoPushOfferText"), (Int64)item.AppUserId, item.BidId);
                //                        AppUserUI user = AppUserUI.GetAppUserUI((Int64)item.AppUserId);
                //                        List<BidProductUI> products = BidController.GetProductsByBid(item.BidId);
                //                        Bid b = Bid.FetchByID(item.BidId);
                //                        string subject = GlobalStrings.GetText("MailToAdmin", new CultureInfo("he-IL"));
                //                        string body = GlobalStrings.GetText("SubjectMailToAdminOffers",new CultureInfo("he-IL"));
                //                        EmailMessagingService.SendMailNoOffersToAdmin(user, b.StartDate, products, subject, body);
                //                    }
                //                    else if (item.TempAppUserId != null && item.TempAppUserId != 0)
                //                    {
                //                        Notification.SendNotificationTempUserOffers(Snoopi.web.Localization.PushStrings.GetText("NoPushOfferText"), (Int64)item.TempAppUserId, item.BidId);
                //                    }

                //                }
                //                item.IsSendOffer = true;
                //                item.Save();

                //            }
                //            jsonWriter.WritePropertyName(@"success");
                //            jsonWriter.WriteValue(true);
                //        }
                //        catch (System.Exception ex)
                //        {
                //            //RespondError(Response, HttpStatusCode.BadRequest, ex.ToString());
                //            jsonWriter.WritePropertyName(@"error");
                //            jsonWriter.WriteValue(@"unknown");
                //            jsonWriter.WritePropertyName(@"description");
                //            jsonWriter.WriteValue(ex.ToString());
                //        }
                //        jsonWriter.WriteEndObject();
                //    }
                //}
            }
            else if (PathParams[0] == @"service_offer")
            {
                using (StreamWriter streamWriter = new StreamWriter(Response.OutputStream))
                {
                    using (JsonTextWriter jsonWriter = new JsonTextWriter(streamWriter))
                    {
                        jsonWriter.WriteStartObject();
                        try
                        {
                            Query qry = new Query(BidService.TableSchema);
                            qry.Where(BidService.Columns.IsSendOffer, WhereComparision.EqualsTo, false);
                            qry.AddWhere(BidService.Columns.EndDate, WhereComparision.LessThan, DateTime.UtcNow);

                            BidServiceCollection bidCollection = BidServiceCollection.FetchByQuery(qry);
                            jsonWriter.WritePropertyName(@"qry");
                            jsonWriter.WriteValue(qry.ToString());

                            Query.New<BidService>().Where(BidService.Columns.IsSendOffer, WhereComparision.EqualsTo, false)
                            .AddWhere(BidService.Columns.EndDate, WhereComparision.LessThan, DateTime.UtcNow)
                            .Update(BidService.Columns.IsSendOffer, true)
                            .Execute();

                            foreach (BidService item in bidCollection)
                            {
                                Query q = new Query(OfferService.TableSchema);
                                q.Where(OfferService.Columns.BidId, WhereComparision.EqualsTo, item.BidId);

                                OfferServiceCollection offerCollection = OfferServiceCollection.FetchByQuery(q);
                                if (offerCollection != null && offerCollection.Count > 0)
                                {
                                    if (item.AppUserId != null && item.AppUserId != 0)
                                    {
                                        Notification.SendNotificationAppUserOffers(string.Format(Snoopi.web.Localization.PushStrings.GetText("PushOfferText"), offerCollection.Count), (Int64)item.AppUserId, item.BidId, true);
                                    }
                                    else if (item.TempAppUserId != null && item.TempAppUserId != 0)
                                    {
                                        Notification.SendNotificationTempUserOffers(string.Format(Snoopi.web.Localization.PushStrings.GetText("PushOfferText"), offerCollection.Count), (Int64)item.TempAppUserId, item.BidId, true);
                                    }
                                }
                                else
                                {
                                    if (item.AppUserId != null && item.AppUserId != 0)
                                    {
                                        Notification.SendNotificationAppUserOffers(Snoopi.web.Localization.PushStrings.GetText("NoPushOfferText"), (Int64)item.AppUserId, item.BidId, true);
                                        AppUserUI user = AppUserUI.GetAppUserUI((Int64)item.AppUserId);
                                        List<BidProductUI> products = BidController.GetProductsByBid(item.BidId);
                                        Bid b = Bid.FetchByID(item.BidId);
                                        string subject = GlobalStrings.GetText("MailToAdmin");
                                        string body = GlobalStrings.GetText("SubjectMailToAdminOffers");
                                        EmailMessagingService.SendMailNoOffersToAdmin(user, b.StartDate, products, subject, body);
                                    }
                                    else if (item.TempAppUserId != null && item.TempAppUserId != 0)
                                    {
                                        Notification.SendNotificationTempUserOffers(Snoopi.web.Localization.PushStrings.GetText("NoPushOfferText"), (Int64)item.TempAppUserId, item.BidId, true);
                                    }

                                }
                                item.IsSendOffer = true;
                                item.Save();

                            }
                            jsonWriter.WritePropertyName(@"success");
                            jsonWriter.WriteValue(true);
                        }
                        catch (System.Exception ex)
                        {
                            //RespondError(Response, HttpStatusCode.BadRequest, ex.ToString());
                            jsonWriter.WritePropertyName(@"error");
                            jsonWriter.WriteValue(@"unknown");
                            jsonWriter.WritePropertyName(@"description");
                            jsonWriter.WriteValue(ex.ToString());
                        }
                        jsonWriter.WriteEndObject();
                    }
                }
            }
            else if (PathParams[0] == @"order_received")
            {
                using (StreamWriter streamWriter = new StreamWriter(Response.OutputStream))
                {
                    using (JsonTextWriter jsonWriter = new JsonTextWriter(streamWriter))
                    {
                        jsonWriter.WriteStartObject();
                        try
                        {
                            Query qry = new Query(Order.TableSchema);
                            qry.Where(Order.Columns.IsSendRecived, WhereComparision.EqualsTo, false);
                            qry.AddWhere(Order.Columns.ReceivedDate, WhereComparision.EqualsTo, null);
                            qry.AddWhere(Order.Columns.UserPaySupplierStatus,WhereComparision.EqualsTo, UserPaymentStatus.Payed);
                            qry.AddWhere(Order.Columns.SuppliedDate, WhereComparision.LessThanOrEqual, DateTime.UtcNow.AddHours(-24));

                            OrderCollection orderCollection = OrderCollection.FetchByQuery(qry);

                            //Query.New<Order>().Where(Order.Columns.IsSendRecived, WhereComparision.EqualsTo, false)
                            //     .AddWhere(Order.Columns.ReceivedDate, WhereComparision.EqualsTo, null)
                            //     .AddWhere(Order.Columns.CreateDate, WhereComparision.LessThanOrEqual, DateTime.UtcNow.AddHours(-24))
                            //     .Update(Order.Columns.IsSendRecived, true)
                            //     .Execute();

                            foreach (Order item in orderCollection)
                            {
                                Notification.SendNotificationAppUserReceviedOrder(Snoopi.web.Localization.PushStrings.GetText("ReceivedOrder"), (Int64)item.AppUserId, item.OrderId);
                                item.IsSendRecived = true;
                                item.Save();
                            }

                            jsonWriter.WritePropertyName(@"success");
                            jsonWriter.WriteValue(true);
                        }
                        catch (System.Exception ex)
                        {
                            //RespondError(Response, HttpStatusCode.BadRequest, ex.ToString());
                            jsonWriter.WritePropertyName(@"error");
                            jsonWriter.WriteValue(@"unknown");
                            jsonWriter.WritePropertyName(@"description");
                            jsonWriter.WriteValue(ex.ToString());
                        }
                        jsonWriter.WriteEndObject();
                    }
                }
            }
            else if (PathParams[0] == @"auto_push")
            {
                using (StreamWriter streamWriter = new StreamWriter(Response.OutputStream))
                {
                    using (JsonTextWriter jsonWriter = new JsonTextWriter(streamWriter))
                    {
                        jsonWriter.WriteStartObject();
                        try
                        {
                            var filters = NotificationGroups.GetAutoFilters();
                            foreach (var item in filters)
                            {
                                if (item.LastRun == null || item.LastRun.Value.AddDays(1) < DateTime.Now)
                                {
                                    var users = NotificationGroups.GetUsersOfAutoFilter(item);
                                    try
                                    {
                                        Task.Run(() => Snoopi.core.FcmService.SendTemplateToMany(item.Name, item.MessageTemplate, users)).Wait();
                                    }
                                    catch (Exception ex)
                                    {
                                        using (System.IO.StreamWriter sw = System.IO.File.AppendText(AppDomain.CurrentDomain.BaseDirectory + @"\Output\push-log.txt"))
                                        {
                                            sw.WriteLine(@" ------------" + DateTime.Now + "--------------------" + '\n' + "Exception  " + ex.Message + " CallStack : " + ex.StackTrace);
                                        }
                                    }
                                    item.LastRun = DateTime.Now;
                                    item.Save();
                                }
                            }

                            jsonWriter.WritePropertyName(@"success");
                            jsonWriter.WriteValue(true);
                        }
                        catch (System.Exception ex)
                        {
                            //RespondError(Response, HttpStatusCode.BadRequest, ex.ToString());
                            jsonWriter.WritePropertyName(@"error");
                            jsonWriter.WriteValue(@"unknown");
                            jsonWriter.WritePropertyName(@"description");
                            jsonWriter.WriteValue(ex.ToString());
                        }
                        jsonWriter.WriteEndObject();
                    }
                }
            }
            else if (PathParams[0] == @"rate_supplier")
            {
                using (StreamWriter streamWriter = new StreamWriter(Response.OutputStream))
                {
                    using (JsonTextWriter jsonWriter = new JsonTextWriter(streamWriter))
                    {
                        jsonWriter.WriteStartObject();
                        try
                        {
                            Query qry = new Query(Order.TableSchema);
                            qry.Where(Order.Columns.IsSendRateSupplier, WhereComparision.EqualsTo, false);
                            qry.AddWhere(Order.Columns.SuppliedDate, WhereComparision.LessThanOrEqual, DateTime.UtcNow.AddHours(-Settings.GetSettingInt32(Settings.Keys.RATE_SUPPLIER_AFTER_ORDER_HOUR, 24)));

                            OrderCollection orderCollection = OrderCollection.FetchByQuery(qry);

                            Query.New<Order>().Where(Order.Columns.IsSendRateSupplier, WhereComparision.EqualsTo, false)
                                .AddWhere(Order.Columns.SuppliedDate, WhereComparision.LessThanOrEqual, DateTime.UtcNow.AddHours(-Settings.GetSettingInt32(Settings.Keys.RATE_SUPPLIER_AFTER_ORDER_HOUR, 24)))
                                .Update(Order.Columns.IsSendRateSupplier, true)
                                .Execute();

                            foreach (Order item in orderCollection)
                            {
                                var bid = Bid.FetchByID(item.BidId);
                                AppSupplier supplier = AppSupplier.FetchByID(item.SupplierId);
                                Notification.SendNotificationAppUserRateSupplier(Snoopi.web.Localization.PushStrings.GetText("RateSupplier"), item.AppUserId, item.SupplierId.Value, supplier.BusinessName, item.BidId);
                                item.IsSendRateSupplier = true;
                                item.Save();
                            }

                            jsonWriter.WritePropertyName(@"success");
                            jsonWriter.WriteValue(true);
                        }
                        catch (System.Exception ex)
                        {
                            //RespondError(Response, HttpStatusCode.BadRequest, ex.ToString());
                            jsonWriter.WritePropertyName(@"error");
                            jsonWriter.WriteValue(@"unknown");
                            jsonWriter.WritePropertyName(@"description");
                            jsonWriter.WriteValue(ex.ToString());
                        }
                        jsonWriter.WriteEndObject();
                    }
                }
            }
            else if (PathParams[0] == @"test_rate_supplier")
             {
                Notification.SendNotificationAppUserRateSupplier(Snoopi.web.Localization.PushStrings.GetText("RateSupplier"), 18283, 387,"PetBool", 2345);
            }
            else
            {
                using (StreamWriter streamWriter = new StreamWriter(Response.OutputStream))
                {
                    using (JsonTextWriter jsonWriter = new JsonTextWriter(streamWriter))
                    {
                        jsonWriter.WriteStartObject();
                        jsonWriter.WritePropertyName(@"error");
                        jsonWriter.WriteValue(@"unknown");
                        jsonWriter.WriteEndObject();
                    }
                }
            }
        }

        public override void Get(HttpRequest Request, HttpResponse Response, params string[] PathParams)
        {
            HandleAll(Request, Response, PathParams);
        }

        public override void Post(HttpRequest Request, HttpResponse Response, params string[] PathParams)
        {
            HandleAll(Request, Response, PathParams);
        }

        public override void Put(HttpRequest Request, HttpResponse Response, params string[] PathParams)
        {
            HandleAll(Request, Response, PathParams);
        }
    }
}
