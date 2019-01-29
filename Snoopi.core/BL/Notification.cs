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
    public static class Notification
    {

        public static void SendNotificationNewMessage(string Message)
        {
            Dictionary<string, object> additionalInfo = new Dictionary<string, object>();
            additionalInfo[@"message"] = Message;

            Query q = Query.New<AppUser>();
               // .Where(AppUser.Columns.is, false);

            foreach (AppUser item in AppUserCollection.FetchByQuery(q))
            {
                BadgeCountForAppUser(item.AppUserId, 1);
                SendNotificationMessage(item.AppUserId, "Alert", @"hello", null, GetBadgeCountForAppUser(item.AppUserId), null, @"new-message", additionalInfo);
            }
        }

        public static void SendNotificationAppUserRateSupplier(string message, Int64 AppUserId, Int64 SupplierId, string supplierBussinessName, Int64 BidID)
        {
            Dictionary<string, object> additionalInfo = new Dictionary<string, object>();
            additionalInfo[@"message"] = message;
            additionalInfo[@"supplier_id"] = SupplierId;
            additionalInfo[@"supplier_name"] = supplierBussinessName;
            additionalInfo[@"bid_id"] = BidID;


            BadgeCountForAppUser(AppUserId, 1);
            SendNotificationMessage(AppUserId, "Message", @"rate-supplier", null, GetBadgeCountForAppUser(AppUserId), null, @"rate-supplier", additionalInfo);


        }

        public static void SendNotificationAppUserApprovedProduct(Int64 AppUserId, Int64 ProductId, string message)
        {
            Dictionary<string, object> additionalInfo = new Dictionary<string, object>();
            additionalInfo[@"product_id"] = ProductId;
            additionalInfo[@"message"] = message;

            BadgeCountForAppUser(AppUserId, 1);
            SendNotificationMessage(AppUserId, "Message", @"approved-product", null, GetBadgeCountForAppUser(AppUserId), null, @"approved-product", additionalInfo);
        }

        public static void SendNotificationDenyProduct(Int64 AppUserId, Int64 ProductId, string message)
        {
            Dictionary<string, object> additionalInfo = new Dictionary<string, object>();
            additionalInfo[@"product_id"] = ProductId;
            additionalInfo[@"message"] = message;

            BadgeCountForAppUser(AppUserId, 1);
            SendNotificationMessage(AppUserId, "Message", @"deny-product", null, GetBadgeCountForAppUser(AppUserId), null, @"deny-product", additionalInfo);
        }

        public static void SendNotificationAppUserOffers(string message, Int64 AppUserId, Int64 BidId, bool IsService = false)
        {
            Dictionary<string, object> additionalInfo = new Dictionary<string, object>();
            additionalInfo[@"message"] = message;
            additionalInfo[@"bid_id"] = BidId;
            BadgeCountForAppUser(AppUserId, 1);
            SendNotificationMessage(AppUserId, "Message", (IsService ? @"offers-service" : @"offers"), null, GetBadgeCountForAppUser(AppUserId), null, (IsService ? @"offers-service" : @"offers"), additionalInfo);
            
        }

        public static void SendNotificationAppUserCreditRejected(string message, Int64 AppUserId, Int64 BidId, bool IsService = false)
        {
            Dictionary<string, object> additionalInfo = new Dictionary<string, object>();
            additionalInfo[@"message"] = message;
            additionalInfo[@"bid_id"] = BidId;
            BadgeCountForAppUser(AppUserId, 1);
            SendNotificationMessage(AppUserId, "Message", @"credit-rejected" , null, GetBadgeCountForAppUser(AppUserId), null, (IsService ? @"offers-service" : @"offers"), additionalInfo);

        }

        public static void SendNotificationAppUserCreditRejected(Int64 AppUserId, Int64 BidId, bool IsService = false)
        {
            string message = AppResources.creditRejected;
            SendNotificationAppUserCreditRejected(message, AppUserId, BidId, IsService);
        }

        public static void SendNotificationAppUserAdminRejected(Int64 AppUserId, Int64 BidId)
        {
            string message = AppResources.adminRejected;
            Dictionary<string, object> additionalInfo = new Dictionary<string, object>();
            additionalInfo[@"message"] = message;
            additionalInfo[@"bid_id"] = BidId;
            BadgeCountForAppUser(AppUserId, 1);
            SendNotificationMessage(AppUserId, "Message", @"admin-rejected", null, GetBadgeCountForAppUser(AppUserId), null, @"SendNotificationNewMessageToAllDevicesadmin-rejected", additionalInfo);

        }

        public static void SendNotificationTempUserOffers(string message, Int64 TempAppUserId, Int64 BidId, bool IsService = false)
        {
            Dictionary<string, object> additionalInfo = new Dictionary<string, object>();
            additionalInfo[@"message"] = message;
            additionalInfo[@"bid_id"] = BidId;


            //BadgeCountForAppUser(TempAppUserId, 1); 
            SendApnsNotificationTemp(TempAppUserId, (IsService ? @"offers-service" : @"offers"), null, 0, null, (IsService ? @"offers-service" : @"offers"), additionalInfo);
            SendGcmNotificationTemp(TempAppUserId, "Message", 0, (IsService ? @"offers-service" : @"offers"), additionalInfo);
        }

        public static void SendNotificationAppUserReceviedOrder(string message, Int64 AppUserId, Int64 OrderId)
        {
            Dictionary<string, object> additionalInfo = new Dictionary<string, object>();
            additionalInfo[@"message"] = message;
            additionalInfo[@"order_id"] = OrderId;

            BadgeCountForAppUser(AppUserId, 1);
            SendNotificationMessage(AppUserId, "Message", @"recevied-order", null, GetBadgeCountForAppUser(AppUserId), null, @"recevied-order", additionalInfo);

        }

        public static void SendGenerealNotificationToApnUser(string message, Int64 AppUserId, bool isTemp, string token)
        {
            Dictionary<string, object> additionalInfo = new Dictionary<string, object>();
            additionalInfo[@"message"] = message;          
            SendApnsNotificationToUser(message, null, null, @"new-message", additionalInfo, token, null);
        }

        public static void SendNotificationAppUserSupplierApproved(string message, Int64 AppUserId, Int64 OrderId)
        {
            Dictionary<string, object> additionalInfo = new Dictionary<string, object>();
            additionalInfo[@"message"] = message;
            additionalInfo[@"order_id"] = OrderId;

            BadgeCountForAppUser(AppUserId, 1);
            SendNotificationMessage(AppUserId, "Message", @"supplier-approved", null, GetBadgeCountForAppUser(AppUserId), null, @"supplier-approved", additionalInfo);

        }

        public static void SendNotificationNewMessageToAllDevices(string Message)
        {
            Dictionary<string, object> additionalInfo = new Dictionary<string, object>();
            additionalInfo[@"message"] = Message;
          
            UpdateUnreadNotificationCount(1);
            SendGcmNotification("Message", @"new-message", additionalInfo);
            SendApnsNotification(Message, null, null, @"new-message", additionalInfo);
        }

        public static void BadgeCountForAppUser(Int64 AppUserId, int Increment)
        {
            Query.New<AppUser>()
                .Update(AppUser.Columns.UnreadNotificationCount, new dg.Sql.Phrases.Add(AppUser.Columns.UnreadNotificationCount, Increment))
                .Where(AppUser.Columns.AppUserId, AppUserId)
                .Execute();
        }

        public static int GetBadgeCountForAppUser(Int64 AppUserId)
        {
            return (Query.New<AppUser>().Select(AppUser.Columns.UnreadNotificationCount).Where(AppUser.Columns.AppUserId, AppUserId).ExecuteScalar() as int?) ?? 0;
        }

        private static void SendNotificationMessage(Int64 AppUserId, string CollapseKey, string MessageLocKey, object[] MessageLocArgs, Int32 Badge, string Sound, string ActionType, Dictionary<string, object> CustomItems)
        {
            string message = CustomItems != null && CustomItems["message"] != null ? CustomItems["message"].ToString() : MessageLocKey;
            SendApnsNotification(AppUserId, message, MessageLocArgs, Badge, Sound, ActionType, CustomItems);
            SendGcmNotification(AppUserId, CollapseKey, Badge, ActionType, CustomItems);
        }

        private static void SendApnsNotification(Int64 AppUserId, string MessageLocKey, object[] MessageLocArgs, Int32 Badge, string Sound, string ActionType)
        {
            SendApnsNotification(AppUserId, MessageLocKey, MessageLocArgs, Badge, Sound, ActionType, null);
        }
        private static void SendApnsNotificationTemp(Int64 TempId, string MessageLocKey, object[] MessageLocArgs, Int32 Badge, string Sound, string ActionType, Dictionary<string, object> CustomItems, string tok = null)
        {
            List<string> tokens = new List<string>();
            if (tok == null)
            {
                using (ConnectorBase conn = ConnectorBase.NewInstance())
                {
                    Query qry = Query.New<TempAppUser>()
                        .Select(TempAppUser.Columns.DeviceUdid)
                        .Where(TempAppUser.Columns.TempAppUserId, TempId);

                    using (DataReaderBase reader = qry.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tokens.Add(reader.GetString(0));
                        }
                    }
                }
            }
            else tokens.Add(tok);

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

            foreach (string token in tokens)
            {
                NotificationService service = APNSService.SharedInstance;
                service.SendMessage(token,
                    alert,
                    Badge,
                    (Sound == null || Sound.Length == 0) ? @"default" : Sound,
                    customItems,
                    delegate()
                    {
                        //Query qry = Query.New<TempAppUser>().Delete()
                        //    .Where(TempAppUser.Columns.DeviceUdid, token);
                    });
            }
        }
        private static void SendApnsNotification(Int64 AppUserId, string MessageLocKey, object[] MessageLocArgs, Int32 Badge, string Sound, string ActionType, Dictionary<string, object> CustomItems, string tok = null)
        {
            List<string> tokens = new List<string>();
            if (tok == null)
            {
                using (ConnectorBase conn = ConnectorBase.NewInstance())
                {
                    Query qry = Query.New<AppUserAPNSToken>()
                        .Select(AppUserAPNSToken.Columns.Token)
                        .Where(AppUserAPNSToken.Columns.AppUserId, AppUserId);

                    using (DataReaderBase reader = qry.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tokens.Add(reader.GetString(0));
                        }
                    }
                }
            }
            else tokens.Add(tok);

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
                NotificationService service = APNSService.SharedInstance;
                service.SendMessage(token,
                    alert,
                    Badge,
                    (Sound == null || Sound.Length == 0) ? @"default" : Sound,
                    customItems,
                    delegate()
                    {
                        Query qry = Query.New<AppUserAPNSToken>().Delete()
                            .Where(AppUserAPNSToken.Columns.Token, token);
                    });
            }
        }

        private static void SendApnsNotificationToUser(string MessageLocKey, object[] MessageLocArgs, string Sound, string ActionType, Dictionary<string, object> CustomItems, string token, int? unreadBadge)
        {
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
    
                NotificationService service = APNSService.SharedInstance;
            service.SendMessage(token,
                alert,
                unreadBadge,
                (Sound == null || Sound.Length == 0) ? @"default" : Sound,
                customItems,
                delegate ()
                {
                    Query qry = Query.New<AppUserAPNSToken>().Delete()
                        .Where(AppUserAPNSToken.Columns.Token, token);
                });
        }

    private static void SendApnsNotification(string MessageLocKey, object[] MessageLocArgs, string Sound, string ActionType, Dictionary<string, object> CustomItems)
        {
            List<AppUserTokenUI> tokens = AppUserTokenUI.GetAllAPNSAppUserTokenUI();

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

            foreach (AppUserTokenUI token in tokens)
            {
                NotificationService service = APNSService.SharedInstance;
                service.SendMessage(token.Token,
                    alert,
                    token.UnreadNotificationCount,
                    (Sound == null || Sound.Length == 0) ? @"default" : Sound,
                    customItems,
                    delegate()
                    {
                        Query qry = Query.New<AppUserAPNSToken>().Delete()
                            .Where(AppUserAPNSToken.Columns.Token, token);
                    });
            }
        }
        private static void SendApnsNotification(Int64 AppUserId, string Message, Int32 Badge, string Sound, string ActionType)
        {
            List<string> tokens = new List<string>();

            using (ConnectorBase conn = ConnectorBase.NewInstance())
            {
                Query qry = Query.New<AppUserAPNSToken>()
                    .Select(AppUserAPNSToken.Columns.Token)
                    .Where(AppUserAPNSToken.Columns.AppUserId, AppUserId);

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
                NotificationService service = APNSService.SharedInstance;
                service.SendMessage(token,
                    Message,
                    Badge,
                    (Sound == null || Sound.Length == 0) ? @"default" : Sound,
                    delegate()
                    {
                        Query.New<AppUserAPNSToken>().Delete()
                            .Where(AppUserAPNSToken.Columns.Token, token).Execute();
                    });
            }
        }
        private static void SendApnsNotification(string Message, Int32 Badge, string Sound, string ActionType)
        {
            List<string> tokens = new List<string>();

            using (ConnectorBase conn = ConnectorBase.NewInstance())
            {
                Query qry = Query.New<AppUserAPNSToken>()
                    .Select(AppUserAPNSToken.Columns.Token);

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
                NotificationService service = APNSService.SharedInstance;
                service.SendMessage(token,
                    Message,
                    Badge,
                    (Sound == null || Sound.Length == 0) ? @"default" : Sound,
                    delegate()
                    {
                        Query.New<AppUserAPNSToken>().Delete()
                            .Where(AppUserAPNSToken.Columns.Token, token).Execute();
                    });
            }
        }

        private static void SendGcmNotificationTemp(Int64 TempId, string CollapseKey, Int32 Badge, string ActionType, Dictionary<string, object> CustomItems)
        {
            List<string> tokens = new List<string>();

            using (ConnectorBase conn = ConnectorBase.NewInstance())
            {
                Query qry = Query.New<TempAppUser>()
                    .Select(TempAppUser.Columns.DeviceUdid)
                    .Where(TempAppUser.Columns.TempAppUserId, TempId);

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
                payload.RestrictedPackageName = GcmService.PackageName;
                payload.AddCustom("badge", Badge);
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

                HttpNotificationService service = GcmService.SharedInstance;
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
                                            Query.New<TempAppUser>()
                                                .Update(TempAppUser.Columns.DeviceUdid, jToken.Value<string>())
                                                .Where(TempAppUser.Columns.DeviceUdid, payload.RegistrationIds[resultIndex])
                                                .Execute();
                                        }
                                        else
                                        {
                                            if (result.TryGetValue("error", out jToken))
                                            {
                                                if (jToken.Value<string>() == "NotRegistered")
                                                {
                                                    Query.New<TempAppUser>()
                                                        .Delete()
                                                        .Where(TempAppUser.Columns.DeviceUdid, payload.RegistrationIds[resultIndex])
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

        private static void SendGcmNotification(Int64 AppUserId, string CollapseKey, Int32 Badge, string ActionType, Dictionary<string, object> CustomItems)
        {
            List<string> tokens = new List<string>();

            using (ConnectorBase conn = ConnectorBase.NewInstance())
            {
                Query qry = Query.New<AppUserGcmToken>()
                    .Select(AppUserGcmToken.Columns.Token)
                    .Where(AppUserGcmToken.Columns.AppUserId, AppUserId);

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
                payload.RestrictedPackageName = GcmService.PackageName;
                payload.AddCustom("badge", Badge);
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

                HttpNotificationService service = GcmService.SharedInstance;
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
                                            Query.New<AppUserGcmToken>()
                                                .Update(AppUserGcmToken.Columns.Token, jToken.Value<string>())
                                                .Where(AppUserGcmToken.Columns.Token, payload.RegistrationIds[resultIndex])
                                                .Execute();
                                        }
                                        else
                                        {
                                            if (result.TryGetValue("error", out jToken))
                                            {
                                                if (jToken.Value<string>() == "NotRegistered")
                                                {
                                                    Query.New<AppUserGcmToken>()
                                                        .Delete()
                                                        .Where(AppUserGcmToken.Columns.Token, payload.RegistrationIds[resultIndex])
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
        private static void SendGcmNotification(string CollapseKey, string ActionType, Dictionary<string, object> CustomItems)
        {
            List<AppUserTokenUI> tokens = AppUserTokenUI.GetAllGcmAppUserTokenUI();

            foreach (AppUserTokenUI t in tokens)
            {
                dg.Utilities.GoogleCloudMessaging.HttpNotificationPayload payload = new dg.Utilities.GoogleCloudMessaging.HttpNotificationPayload(t.Token);
                payload.CollapseKey = CollapseKey;
                payload.RestrictedPackageName = GcmService.PackageName;
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

                HttpNotificationService service = GcmService.SharedInstance;
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
                                            Query.New<AppUserGcmToken>()
                                                .Update(AppUserGcmToken.Columns.Token, jToken.Value<string>())
                                                .Where(AppUserGcmToken.Columns.Token, payload.RegistrationIds[resultIndex])
                                                .Execute();
                                        }
                                        else
                                        {
                                            if (result.TryGetValue("error", out jToken))
                                            {
                                                if (jToken.Value<string>() == "NotRegistered")
                                                {
                                                    Query.New<AppUserGcmToken>()
                                                        .Delete()
                                                        .Where(AppUserGcmToken.Columns.Token, payload.RegistrationIds[resultIndex])
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
            List<Int64> appUsersIds = new List<Int64>();

            using (ConnectorBase conn = ConnectorBase.NewInstance())
            {
                Query q1 = Query.New<AppUserGcmToken>()
                    .Select(AppUserGcmToken.Columns.AppUserId);

                using (DataReaderBase reader = q1.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        appUsersIds.Add(Convert.ToInt64(reader.GetString(0)));
                    }
                }
            }
            using (ConnectorBase conn = ConnectorBase.NewInstance())
            {
                Query q2 = Query.New<AppUserAPNSToken>()
                    .Select(AppUserAPNSToken.Columns.AppUserId);

                using (DataReaderBase reader = q2.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        appUsersIds.Add(Convert.ToInt64(reader.GetString(0)));
                    }
                }
            }
            appUsersIds = appUsersIds.Distinct().ToList();
            if (appUsersIds.Count > 0)
            {
                Query.New<AppUser>()
                    .Update(AppUser.Columns.UnreadNotificationCount, new dg.Sql.Phrases.Add(AppUser.Columns.UnreadNotificationCount, Increment))
                    .Where(AppUser.TableSchema.SchemaName, AppUser.Columns.AppUserId, WhereComparision.In, appUsersIds)
                    .Execute();
            }
        }

        public static void UpdateUnreadNotificationCountOfUsers(List<long> userTokens)
        {
            if (userTokens.Count() > 0)
            {
                Query.New<AppUser>()
                    .Update(AppUser.Columns.UnreadNotificationCount, new dg.Sql.Phrases.Add(AppUser.Columns.UnreadNotificationCount, 1))
                    .Where(AppUser.TableSchema.SchemaName, AppUser.Columns.AppUserId, WhereComparision.In, userTokens)
                    .Execute();
            }
        }

        public static void UpdateUnreadNotificationCountOfUser(long userId)
        {   
                Query.New<AppUser>()
                    .Update(AppUser.Columns.UnreadNotificationCount, new dg.Sql.Phrases.Add(AppUser.Columns.UnreadNotificationCount, 1))
                    .Where(AppUser.TableSchema.SchemaName, AppUser.Columns.AppUserId, WhereComparision.EqualsTo, userId)
                    .Execute();   
        }
    }

    public class AppUserTokenUI
    {
        public Int64 AppUserId { get; set; }
        public string Token { get; set; }
        public int UnreadNotificationCount { get; set; }

        public static List<AppUserTokenUI> GetAllGcmAppUserTokenUI()
        {
            List<AppUserTokenUI> appUsersTokensUI = new List<AppUserTokenUI>();
            using (ConnectorBase conn = ConnectorBase.NewInstance())
            {
                Query qry = Query.New<AppUserGcmToken>()
                    .Select(AppUserGcmToken.Columns.Token)
                    .AddSelect(AppUserGcmToken.Columns.AppUserId)
                    .AddSelect(AppUser.TableSchema.SchemaName, AppUser.Columns.UnreadNotificationCount, AppUser.Columns.UnreadNotificationCount)
                    .Join(JoinType.InnerJoin, AppUser.TableSchema, AppUser.TableSchema.SchemaName, new JoinColumnPair(AppUserGcmToken.TableSchema, AppUserGcmToken.Columns.AppUserId, AppUser.Columns.AppUserId));

                using (DataReaderBase reader = qry.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        AppUserTokenUI appUserTokensUI = new AppUserTokenUI();
                        appUserTokensUI.AppUserId = Convert.ToInt64(reader[AppUserGcmToken.Columns.AppUserId]);
                        appUserTokensUI.Token = reader[AppUserGcmToken.Columns.Token].ToString();
                        appUserTokensUI.UnreadNotificationCount = Convert.ToInt32(reader[AppUser.Columns.UnreadNotificationCount]);
                        appUsersTokensUI.Add(appUserTokensUI);
                    }
                }
            }
            return appUsersTokensUI;
        }
        public static List<AppUserTokenUI> GetAllAPNSAppUserTokenUI()
        {
            List<AppUserTokenUI> appUsersTokensUI = new List<AppUserTokenUI>();
            using (ConnectorBase conn = ConnectorBase.NewInstance())
            {
                Query qry = Query.New<AppUserAPNSToken>()
                    .Select(AppUserAPNSToken.Columns.Token)
                    .AddSelect(AppUserAPNSToken.Columns.AppUserId)
                    .AddSelect(AppUser.TableSchema.SchemaName, AppUser.Columns.UnreadNotificationCount, AppUser.Columns.UnreadNotificationCount)
                    .Join(JoinType.InnerJoin, AppUser.TableSchema, AppUser.TableSchema.SchemaName, new JoinColumnPair(AppUserAPNSToken.TableSchema, AppUserAPNSToken.Columns.AppUserId, AppUser.Columns.AppUserId));

                using (DataReaderBase reader = qry.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        AppUserTokenUI appUserTokensUI = new AppUserTokenUI();
                        appUserTokensUI.AppUserId = Convert.ToInt64(reader[AppUserAPNSToken.Columns.AppUserId]);
                        appUserTokensUI.Token = reader[AppUserAPNSToken.Columns.Token].ToString();
                        appUserTokensUI.UnreadNotificationCount = Convert.ToInt32(reader[AppUser.Columns.UnreadNotificationCount]);
                        appUsersTokensUI.Add(appUserTokensUI);
                    }
                }
            }
            return appUsersTokensUI;
        }
    }
}
