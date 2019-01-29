using System;
using System.Collections.Generic;
using System.Text;
using dg.Sql.Connector;
using dg.Sql;
using dg.Utilities;
using dg.Utilities.Apns;
using dg.Utilities.GoogleCloudMessaging;
using System.Globalization;
using System.Net;
using Newtonsoft.Json.Linq;
using Snoopi.core.DAL;
using System.Linq;
using Snoopi.core.Properties;


namespace Snoopi.core.BL
{
    public static class SupplierNotification
    {

        public static void 
            SendNotificationCloseBidToSupplier(Int64 OrderId, Int64 SupplierId)
        {
            Dictionary<string, object> additionalInfo = new Dictionary<string, object>();
            additionalInfo[@"order_id"] = OrderId;
            additionalInfo[@"message"] = null;
            BadgeCountForSupplier(SupplierId, 1);
            SendNotificationMessage(SupplierId, "Win", @"close-bid", null, GetBadgeCountForSupplier(SupplierId), null, @"close-bid", additionalInfo);   
        }

        public static void SendNotificationNewServiceBidToSupplier(Int64 BidId, List<Int64> LstSupplier)
        {
            Dictionary<string, object> additionalInfo = new Dictionary<string, object>();
            additionalInfo[@"bid_id"] = BidId;
            additionalInfo[@"message"] = AppResources.newServiceBid;
            string timeToLive = Settings.GetSetting(Settings.Keys.EXPIRY_OFFER_TIME_HOURS) ?? @"3600"; 
            foreach (Int64 supplierId in LstSupplier)
            {
                BadgeCountForSupplier(supplierId, 1);
                SendNotificationMessage(supplierId, "Message", @"new-service-bid", null, GetBadgeCountForSupplier(supplierId), null, @"new-service-bid", additionalInfo, timeToLive);
            }
        }

        public static void SendNotificationNewBidToSupplier(Int64 BidId, List<Int64> LstSupplier)
        {
            Dictionary<string, object> additionalInfo = new Dictionary<string, object>();
            additionalInfo[@"bid_id"] = BidId;
            additionalInfo[@"message"] = AppResources.newBid;
            string timeToLive = Settings.GetSetting(Settings.Keys.EXPIRY_OFFER_TIME_HOURS) ?? @"3600"; 
            foreach (Int64 supplierId in LstSupplier)
            {
                BadgeCountForSupplier(supplierId, 1);
                SendNotificationMessage(supplierId, "Message", @"new-bid", null, GetBadgeCountForSupplier(supplierId), null, @"new-bid", additionalInfo, timeToLive);
            }
        }

        public static void SendNotificationNewBidToPremiumSupplier(Int64 BidId, Int64 supplierId)
        {
            Dictionary<string, object> additionalInfo = new Dictionary<string, object>();
            additionalInfo[@"bid_id"] = BidId;
            additionalInfo[@"message"] = AppResources.newBidPremium;
            string timeToLive = Settings.GetSetting(Settings.Keys.EXPIRY_OFFER_TIME_HOURS) ?? @"3600"; 

            BadgeCountForSupplier(supplierId, 1);
            SendNotificationMessage(supplierId, "Message", @"new-bid-premium", null, GetBadgeCountForSupplier(supplierId), null, @"new-bid-premium", additionalInfo,timeToLive);
        }

        public static void SendNotificationNewSpecialDealSupplier(Int64 BidId, Int64 supplierId)
        {
            Dictionary<string, object> additionalInfo = new Dictionary<string, object>();
            additionalInfo[@"bid_id"] = BidId;
            additionalInfo[@"message"] = AppResources.newBidPremium;
            string timeToLive = Settings.GetSetting(Settings.Keys.EXPIRY_OFFER_TIME_HOURS) ?? @"3600"; 

            BadgeCountForSupplier(supplierId, 1);
            SendNotificationMessage(supplierId, "Message", @"new-bid-special-deal", null, GetBadgeCountForSupplier(supplierId), null, @"new-bid-premium", additionalInfo, timeToLive);
        }

        public static void SendNotificationNewMessage(string Message)
        {
            Dictionary<string, object> additionalInfo = new Dictionary<string, object>();
            additionalInfo[@"message"] = Message;

            Query q = Query.New<AppSupplier>()
                .Where(AppSupplier.Columns.IsDeleted, false);

            foreach (AppSupplier item in AppSupplierCollection.FetchByQuery(q))
            {
                BadgeCountForSupplier(item.SupplierId, 1);
                SendNotificationMessage(item.SupplierId, Message, @"new-message", null, GetBadgeCountForSupplier(item.SupplierId), null, @"new-message", additionalInfo);
            }

        }

        public static void  SendNotificationMaxAutoModeMessage(Int64 SupplierId)
        {
            Dictionary<string, object> additionalInfo = new Dictionary<string, object>();
            additionalInfo[@"message"] = null;
            BadgeCountForSupplier(SupplierId, 1);
            SendNotificationMessage(SupplierId, "MaxAuto", @"max-auto-mode", null, GetBadgeCountForSupplier(SupplierId), null, @"max-auto-mode", additionalInfo);
        }

        public static void SendNotificationNewMessageToAllDevices(string Message)
        {
            Dictionary<string, object> additionalInfo = new Dictionary<string, object>();
            additionalInfo[@"message"] = Message;

            UpdateUnreadNotificationCount(1);
            SendGcmNotification("Message", @"new-message", additionalInfo);
            SendApnsNotification(@"new-message", null, null, @"new-message", additionalInfo);
        }

        public static int GetBadgeCountForSupplier(Int64 SupplierId)
        {
            return (Query.New<AppSupplier>().Select(AppSupplier.Columns.UnreadNotificationCount)
                .Where(AppSupplier.Columns.SupplierId, SupplierId)
                .ExecuteScalar() as int?) ?? 0;
        }

        public static void BadgeCountForSupplier(Int64 SupplierId, int Increment)
        {
            Query.New<AppSupplier>()
                .Update(AppSupplier.Columns.UnreadNotificationCount, new dg.Sql.Phrases.Add(AppSupplier.Columns.UnreadNotificationCount, Increment))
                .Where(AppSupplier.Columns.SupplierId, SupplierId)
                .Execute();
        }

        private static void SendNotificationMessage(Int64 SupplierId, string CollapseKey, string MessageLocKey, object[] MessageLocArgs, Int32 Badge, string Sound, string ActionType, Dictionary<string, object> CustomItems, string timeToLive = null)
        {
            SendApnsNotification(SupplierId, CustomItems != null && CustomItems["message"] != null ? CustomItems["message"].ToString() : MessageLocKey, MessageLocArgs, Badge, Sound, ActionType, CustomItems);
            SendGcmNotification(SupplierId, CollapseKey, Badge, ActionType, CustomItems, timeToLive);
        }

        private static void SendApnsNotification(Int64 SupplierId, string MessageLocKey, object[] MessageLocArgs, Int32 Badge, string Sound, string ActionType)
        {
            SendApnsNotification(SupplierId, MessageLocKey, MessageLocArgs, Badge, Sound, ActionType, null);
        }

        private static void SendApnsNotification(Int64 SupplierId, string MessageLocKey, object[] MessageLocArgs, Int32 Badge, string Sound, string ActionType, Dictionary<string, object> CustomItems)
        {
            List<string> tokens = new List<string>();

            using (ConnectorBase conn = ConnectorBase.NewInstance())
            {
                Query qry = Query.New<AppSupplierAPNSToken>()
                    .Select(AppSupplierAPNSToken.Columns.Token)
                    .Where(AppSupplierAPNSToken.Columns.SupplierId, SupplierId);

                using (DataReaderBase reader = qry.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        tokens.Add(reader.GetString(0));
                    }
                }
            }

            NotificationAlert alert = new NotificationAlert();
            alert.LocalizedKey = MessageLocKey;
            if (MessageLocArgs != null)
            {
                alert.LocalizedArgs.AddRange(MessageLocArgs);
            }

            Dictionary<string, object[]> customItems = null;
            if (ActionType != null)
            {
                customItems = new Dictionary<string, object[]>();
                customItems[@"app-action"] = new object[] { ActionType };
            }
            if (CustomItems != null)
            {
                foreach (string key in CustomItems.Keys)
                {
                    if (CustomItems[key] == null) continue;
                    if (customItems == null)
                    {
                        customItems = new Dictionary<string, object[]>();
                    }
                    customItems[key] = new object[] { CustomItems[key] };
                }
            }

            foreach (string token in tokens)
            {
                NotificationService service = APNSServiceSupplier.SharedInstance;
                service.SendMessage(token,
                    alert,
                    Badge,
                    (Sound == null || Sound.Length == 0) ? @"default" : Sound,
                    customItems,
                    delegate()
                    {
                        Query qry = Query.New<AppSupplierAPNSToken>().Delete()
                            .Where(AppSupplierAPNSToken.Columns.Token, token);
                    });
            }
        }
        private static void SendApnsNotification(string MessageLocKey, object[] MessageLocArgs, string Sound, string ActionType, Dictionary<string, object> CustomItems)
        {
            List<AppSupplierTokenUI> tokens = AppSupplierTokenUI.GetAllAPNSAppSupplierTokenUI();

            NotificationAlert alert = new NotificationAlert();
            alert.LocalizedKey = MessageLocKey;
            if (MessageLocArgs != null)
            {
                alert.LocalizedArgs.AddRange(MessageLocArgs);
            }

            Dictionary<string, object[]> customItems = null;
            if (ActionType != null)
            {
                customItems = new Dictionary<string, object[]>();
                customItems[@"app-action"] = new object[] { ActionType };
            }
            if (CustomItems != null)
            {
                foreach (string key in CustomItems.Keys)
                {
                    if (customItems == null)
                    {
                        customItems = new Dictionary<string, object[]>();
                    }
                    customItems[key] = new object[] { CustomItems[key] };
                }
            }

            foreach (AppSupplierTokenUI token in tokens)
            {
                NotificationService service = APNSServiceSupplier.SharedInstance;
                service.SendMessage(token.Token,
                    alert,
                    token.UnreadNotificationCount,
                    (Sound == null || Sound.Length == 0) ? @"default" : Sound,
                    customItems,
                    delegate()
                    {
                        Query qry = Query.New<AppSupplierAPNSToken>().Delete()
                            .Where(AppSupplierAPNSToken.Columns.Token, token);
                    });
            }
        }
        private static void SendApnsNotification(Int64 SupplierId, string Message, Int32 Badge, string Sound, string ActionType)
        {
            List<string> tokens = new List<string>();

            using (ConnectorBase conn = ConnectorBase.NewInstance())
            {
                Query qry = Query.New<AppSupplierAPNSToken>()
                    .Select(AppSupplierAPNSToken.Columns.Token)
                    .Where(AppSupplierAPNSToken.Columns.SupplierId, SupplierId);

                using (DataReaderBase reader = qry.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        tokens.Add(reader.GetString(0));
                    }
                }
            }

            foreach (string token in tokens)
            {
                NotificationService service = APNSServiceSupplier.SharedInstance;
                service.SendMessage(token,
                    Message,
                    Badge,
                    (Sound == null || Sound.Length == 0) ? @"default" : Sound,
                    delegate()
                    {
                        Query.New<AppSupplierAPNSToken>().Delete()
                            .Where(AppSupplierAPNSToken.Columns.Token, token).Execute();
                    });
            }
        }

        private static void SendApnsNotification(string Message, Int32 Badge, string Sound, string ActionType)
        {
            List<string> tokens = new List<string>();

            using (ConnectorBase conn = ConnectorBase.NewInstance())
            {
                Query qry = Query.New<AppSupplierAPNSToken>()
                    .Select(AppSupplierAPNSToken.Columns.Token);

                using (DataReaderBase reader = qry.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        tokens.Add(reader.GetString(0));
                    }
                }
            }

            foreach (string token in tokens)
            {
                NotificationService service = APNSServiceSupplier.SharedInstance;
                service.SendMessage(token,
                    Message,
                    Badge,
                    (Sound == null || Sound.Length == 0) ? @"default" : Sound,
                    delegate()
                    {
                        Query.New<AppSupplierAPNSToken>().Delete()
                            .Where(AppSupplierAPNSToken.Columns.Token, token).Execute();
                    });
            }
        }

        private static void SendGcmNotification(Int64 SupplierId, string CollapseKey, Int32 Badge, string ActionType, Dictionary<string, object> CustomItems , string timeToLive = null)
        {
            List<string> tokens = new List<string>();

            using (ConnectorBase conn = ConnectorBase.NewInstance())
            {
                Query qry = Query.New<AppSupplierGcmToken>()
                    .Select(AppSupplierGcmToken.Columns.Token)
                    .Where(AppSupplierGcmToken.Columns.SupplierId, SupplierId);

                using (DataReaderBase reader = qry.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        tokens.Add(reader.GetString(0));
                    }
                }
            }

            if (tokens.Count > 0)
            {
                dg.Utilities.GoogleCloudMessaging.HttpNotificationPayload payload = new dg.Utilities.GoogleCloudMessaging.HttpNotificationPayload(tokens.ToArray());
                payload.CollapseKey = CollapseKey;
                payload.RestrictedPackageName = GcmServiceSupplier.PackageName;
                payload.AddCustom("badge", Badge);
                if (timeToLive!=null)
                    payload.TimeToLive = Convert.ToInt32(timeToLive) * 60;
                if (ActionType != null)
                {
                    payload.AddCustom("app-action", ActionType);
                }
                if (CustomItems != null)
                {
                    foreach (string key in CustomItems.Keys)
                    {
                        if (CustomItems[key] == null) continue;
                        payload.AddCustom(key, CustomItems[key]);
                    }
                }



                HttpNotificationService service = GcmServiceSupplier.SharedInstance;
                service.SendMessage(payload, delegate (NotificationDeliveryResult x)
                {
                    if (x.HttpStatusCode == HttpStatusCode.OK)
                    {
                        try
                        {
                            JObject response = JObject.Parse(x.Response);
                            if (response != null)
                            {
                                if (response.Value<int>("failure") > 0 || response.Value<int>("canonical_ids") > 0)
                                {
                                    JArray results = response["results"] as JArray;
                                    int resultIndex = 0;
                                    foreach (JObject result in results)
                                    {
                                        JToken jToken;
                                        if (result.TryGetValue("registration_id", out jToken))
                                        {
                                            Query.New<AppSupplierGcmToken>()
                                                .Update(AppSupplierGcmToken.Columns.Token, jToken.Value<string>())
                                                .Where(AppSupplierGcmToken.Columns.Token, payload.RegistrationIds[resultIndex])
                                                .Execute();
                                        }
                                        else
                                        {
                                            if (result.TryGetValue("error", out jToken))
                                            {
                                                if (jToken.Value<string>() == "NotRegistered")
                                                {
                                                    Query.New<AppSupplierGcmToken>()
                                                        .Delete()
                                                        .Where(AppSupplierGcmToken.Columns.Token, payload.RegistrationIds[resultIndex])
                                                        .Execute();
                                                }
                                            }
                                        }
                                        resultIndex++;
                                    }
                                }
                            }
                        }
                        catch(Exception) { }
                    }
                });
            }
        }
        private static void SendGcmNotification(string CollapseKey, string ActionType, Dictionary<string, object> CustomItems)
        {
            List<AppSupplierTokenUI> tokens = AppSupplierTokenUI.GetAllGcmAppSupplierTokenUI();

            foreach (AppSupplierTokenUI t in tokens)
            {
                dg.Utilities.GoogleCloudMessaging.HttpNotificationPayload payload = new dg.Utilities.GoogleCloudMessaging.HttpNotificationPayload(t.Token);
                payload.CollapseKey = CollapseKey;
                payload.RestrictedPackageName = GcmServiceSupplier.PackageName;
                payload.AddCustom("badge", t.UnreadNotificationCount);
                if (ActionType != null)
                {
                    payload.AddCustom("app-action", ActionType);
                }
                if (CustomItems != null)
                {
                    foreach (string key in CustomItems.Keys)
                    {
                        if (CustomItems[key] == null) continue;
                        payload.AddCustom(key, CustomItems[key]);
                    }
                }

                HttpNotificationService service = GcmServiceSupplier.SharedInstance;
                service.SendMessage(payload, x =>
                {
                    if (x.HttpStatusCode == HttpStatusCode.OK)
                    {
                        try
                        {
                            JObject response = JObject.Parse(x.Response);
                            if (response != null)
                            {
                                if (response.Value<int>("failure") > 0 || response.Value<int>("canonical_ids") > 0)
                                {
                                    JArray results = response["results"] as JArray;
                                    int resultIndex = 0;
                                    foreach (JObject result in results)
                                    {
                                        JToken jToken;
                                        if (result.TryGetValue("registration_id", out jToken))
                                        {
                                            Query.New<AppSupplierGcmToken>()
                                                .Update(AppSupplierGcmToken.Columns.Token, jToken.Value<string>())
                                                .Where(AppSupplierGcmToken.Columns.Token, tokens[resultIndex])
                                                .Execute();
                                        }
                                        else
                                        {
                                            if (result.TryGetValue("error", out jToken))
                                            {
                                                if (jToken.Value<string>() == "NotRegistered")
                                                {
                                                    Query.New<AppSupplierGcmToken>()
                                                        .Delete()
                                                        .Where(AppSupplierGcmToken.Columns.Token, tokens[resultIndex])
                                                        .Execute();
                                                }
                                            }
                                        }
                                        resultIndex++;
                                    }
                                }
                            }
                        }
                        catch { }
                    }
                });
            }
        }
        public static void UpdateUnreadNotificationCount(int Increment)
        {
            List<Int64> AppSuppliersIds = new List<Int64>();

            using (ConnectorBase conn = ConnectorBase.NewInstance())
            {
                Query q1 = Query.New<AppSupplierGcmToken>()
                    .Select(AppSupplierGcmToken.Columns.SupplierId);

                using (DataReaderBase reader = q1.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        AppSuppliersIds.Add(Convert.ToInt64(reader.GetString(0)));
                    }
                }
            }
            using (ConnectorBase conn = ConnectorBase.NewInstance())
            {
                Query q2 = Query.New<AppSupplierAPNSToken>()
                    .Select(AppSupplierAPNSToken.Columns.SupplierId);

                using (DataReaderBase reader = q2.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        AppSuppliersIds.Add(Convert.ToInt64(reader.GetString(0)));
                    }
                }
            }
            AppSuppliersIds = AppSuppliersIds.Distinct().ToList();
            Query.New<AppSupplier>()
                .Update(AppSupplier.Columns.UnreadNotificationCount, new dg.Sql.Phrases.Add(AppSupplier.Columns.UnreadNotificationCount, Increment))
                .Where(AppSupplier.TableSchema.SchemaName, AppSupplier.Columns.SupplierId, WhereComparision.In, AppSuppliersIds)
                .Execute();
        }
    }

    public class AppSupplierTokenUI
    {
        public Int64 SupplierId { get; set; }
        public string Token { get; set; }
        public int UnreadNotificationCount { get; set; }


        public static List<AppSupplierTokenUI> GetAllGcmAppSupplierTokenUI()
        {
            List<AppSupplierTokenUI> AppSuppliersTokensUI = new List<AppSupplierTokenUI>();
            using (ConnectorBase conn = ConnectorBase.NewInstance())
            {
                Query qry = Query.New<AppSupplierGcmToken>()
                    .Select(AppSupplierGcmToken.Columns.Token)
                    .AddSelect(AppSupplierGcmToken.Columns.SupplierId)
                    .AddSelect(AppSupplier.TableSchema.SchemaName, AppSupplier.Columns.UnreadNotificationCount, AppSupplier.Columns.UnreadNotificationCount)
                    .Join(JoinType.InnerJoin, AppSupplier.TableSchema, AppSupplier.TableSchema.SchemaName, new JoinColumnPair(AppSupplierGcmToken.TableSchema, AppSupplierGcmToken.Columns.SupplierId, AppSupplier.Columns.SupplierId));

                using (DataReaderBase reader = qry.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        AppSupplierTokenUI AppSupplierTokensUI = new AppSupplierTokenUI();
                        AppSupplierTokensUI.SupplierId = Convert.ToInt64(reader[AppSupplierGcmToken.Columns.SupplierId]);
                        AppSupplierTokensUI.Token = reader[AppSupplierGcmToken.Columns.Token].ToString();
                        AppSupplierTokensUI.UnreadNotificationCount = Convert.ToInt32(reader[AppSupplier.Columns.UnreadNotificationCount]);
                        AppSuppliersTokensUI.Add(AppSupplierTokensUI);
                    }
                }
            }
            return AppSuppliersTokensUI;
        }

        public static List<AppSupplierTokenUI> GetAllAPNSAppSupplierTokenUI()
        {
            List<AppSupplierTokenUI> AppSuppliersTokensUI = new List<AppSupplierTokenUI>();
            using (ConnectorBase conn = ConnectorBase.NewInstance())
            {
                Query qry = Query.New<AppSupplierAPNSToken>()
                    .Select(AppSupplierAPNSToken.Columns.Token)
                    .AddSelect(AppSupplierAPNSToken.Columns.SupplierId)
                    .AddSelect(AppSupplier.TableSchema.SchemaName, AppSupplier.Columns.UnreadNotificationCount, AppSupplier.Columns.UnreadNotificationCount)
                    .Join(JoinType.InnerJoin, AppSupplier.TableSchema, AppSupplier.TableSchema.SchemaName, new JoinColumnPair(AppSupplierAPNSToken.TableSchema, AppSupplierAPNSToken.Columns.SupplierId, AppSupplier.Columns.SupplierId));

                using (DataReaderBase reader = qry.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        AppSupplierTokenUI AppSupplierTokensUI = new AppSupplierTokenUI();
                        AppSupplierTokensUI.SupplierId = Convert.ToInt64(reader[AppSupplierAPNSToken.Columns.SupplierId]);
                        AppSupplierTokensUI.Token = reader[AppSupplierAPNSToken.Columns.Token].ToString();
                        AppSupplierTokensUI.UnreadNotificationCount = Convert.ToInt32(reader[AppSupplier.Columns.UnreadNotificationCount]);
                        AppSuppliersTokensUI.Add(AppSupplierTokensUI);
                    }
                }
            }
            return AppSuppliersTokensUI;
        }
    }
}
