using System;
using System.Collections.Generic;
using System.Text;
using dg.Sql.Connector;
using dg.Sql;
using dg.Utilities;
using Snoopi.core.DAL;
using Snoopi.core.Caching.Manager;
using System.Globalization;
using dg.Utilities.Apns;

namespace Snoopi.core.BL
{
    public static class AppUserController
    {
        public static void SendNotificationAboutNewMessage(Int64 AppUserId, Int64 FromAppUserId, string Message)
        {
            Dictionary<string, object> additionalUserInfo = new Dictionary<string, object>();
            additionalUserInfo[@"to_user_id"] = AppUserId;
            additionalUserInfo[@"from_user_id"] = FromAppUserId;

            SendNotification(AppUserId, @"APNS_NEW_MESSAGE", new object[] { AppUserNameForNotification(FromAppUserId), Message.CutByWords(50, @"...") }, BadgeCountForAppUser(AppUserId, 1), null, @"new-message", additionalUserInfo);
        }
        
        public static void SendNotificationAboutNewMatch(Int64 AppUserId)
        {
            SendNotification(AppUserId, @"APNS_NEW_TREMP_MATCH", null, BadgeCountForAppUser(AppUserId, 1), null, @"new-tremp-match");
        }
        public static void SendNotificationAboutTrempJoinRequest(Int64 AppUserId, Int64 FromAppUserId, Int64 TrempId)
        {
            Dictionary<string, object> additionalUserInfo = new Dictionary<string, object>();
            additionalUserInfo[@"tremp_id"] = TrempId;

            SendNotification(AppUserId, @"APNS_NEW_TREMP_JOIN_REQUEST", new object[] { AppUserNameForNotification(FromAppUserId) }, BadgeCountForAppUser(AppUserId, 1), null, @"new-tremp-join-request", additionalUserInfo);
        }

        public static void SendNotificationAboutGroupAdminRequestApproved(Int64 AppUserId, string GroupName)
        {
            SendNotification(AppUserId, @"APNS_GROUP_ADMIN_APPROVED", new object[] { GroupName.CutByWords(50, @"...") }, BadgeCountForAppUser(AppUserId, 1), null, @"group-admin-request-approved");
        }
        public static void SendNotificationAboutGroupAdminRequestRejected(Int64 AppUserId, string GroupName)
        {
            SendNotification(AppUserId, @"APNS_GROUP_ADMIN_REJECTED", new object[] { GroupName.CutByWords(50, @"...") }, BadgeCountForAppUser(AppUserId, 1), null, @"group-admin-request-rejected");
        }
        public static void SendNotificationAboutGroupJoinRequestApproved(Int64 AppUserId, string GroupName)
        {
            SendNotification(AppUserId, @"APNS_GROUP_JOIN_APPROVED", new object[] {  GroupName.CutByWords(50, @"...") }, BadgeCountForAppUser(AppUserId, 1), null, @"group-join-request-approved");
        }
        public static void SendNotificationAboutGroupJoinRequestRejected(Int64 AppUserId, string GroupName)
        {
            SendNotification(AppUserId, @"APNS_GROUP_JOIN_REJECTED", new object[] { GroupName.CutByWords(50, @"...") }, BadgeCountForAppUser(AppUserId, 1), null, @"group-join-request-rejected");
        }
        public static void SendNotificationAboutTrempJoinRequestApproved(Int64 AppUserId, Int64 FromAppUserId)
        {
            SendNotification(AppUserId, @"APNS_TREMP_JOIN_APPROVED", new object[] { AppUserNameForNotification(FromAppUserId) }, BadgeCountForAppUser(AppUserId, 1), null, @"tremp-join-request-approved");
        }
        public static void SendNotificationAboutTrempJoinRequestRejected(Int64 AppUserId, Int64 FromAppUserId)
        {
            SendNotification(AppUserId, @"APNS_TREMP_JOIN_REJECTED", new object[]{ AppUserNameForNotification(FromAppUserId)}, BadgeCountForAppUser(AppUserId, 1), null, @"tremp-join-request-rejected");
        }	

        public static string AppUserLangCode(Int64 AppUserId)
        {
            return CacheHelper.CacheObject<string>(() =>
            {
                object ret = Query.New<AppUser>().Select(AppUser.Columns.LangCode).Where(AppUser.Columns.AppUserId, AppUserId).ExecuteScalar();
                if (ret != null) return (string)ret;
                return @"";
            }, @"AULangCode_" + AppUserId, 120);
        }
        public static string AppUserNameForNotification(Int64 AppUserId)
        {
            return CacheHelper.CacheObject<string>(() =>
            {
                Query qry = Query.New<AppUser>()
                    .Select(AppUser.Columns.FirstName)
                    .AddSelect(AppUser.Columns.Email)
                    .Where(AppUser.Columns.AppUserId, AppUserId);
                using (DataReaderBase reader = qry.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        string name = reader.GetStringOrEmpty(0);
                        if (name.Length > 0) return name;
                        return reader.GetStringOrEmpty(1);
                    }
                }
                return @"";
            }, @"AUName_" + AppUserId, 120);
        }
        public static int BadgeCountForAppUser(Int64 AppUserId, int Increment)
        {
            Query.New<AppUser>()
                .Update(AppUser.Columns.UnreadNotificationCount, new dg.Sql.Phrases.Add(AppUser.Columns.UnreadNotificationCount, Increment))
                .Where(AppUser.Columns.AppUserId, AppUserId)
                .Execute();
            return (Query.New<AppUser>().Select(AppUser.Columns.UnreadNotificationCount).ExecuteScalar() as int?) ?? 0;
        }

        public static void SendNotification(Int64 AppUserId, string MessageLocKey, object[] MessageLocArgs, Int32 Badge, string Sound, string ActionType)
        {
            SendNotification(AppUserId, MessageLocKey, MessageLocArgs, Badge, Sound, ActionType, null);
        }

        public static void SendNotification(Int64 AppUserId, string MessageLocKey, object[] MessageLocArgs, Int32 Badge, string Sound, string ActionType, Dictionary<string, object> CustomItems)
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
                APNSService.SharedInstance.SendMessage(token,
                    alert,
                    Badge,
                    (Sound == null || Sound.Length == 0) ? @"default" : Sound,
                    customItems,
                    delegate()
                    {
                        Query qry = Query.New<AppUserAPNSToken>().Delete()
                            .Where(AppUserAPNSToken.Columns.Token, token);
                        qry.Execute();
                    });
            }
        }
        public static void SendNotification(Int64 AppUserId, string Message, Int32 Badge, string Sound, string ActionType)
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
                APNSService.SharedInstance.SendMessage(token,
                    Message,
                    Badge,
                    (Sound == null || Sound.Length == 0) ? @"default" : Sound,
                    delegate()
                    {
                        Query qry = Query.New<AppUserAPNSToken>().Delete()
                            .Where(AppUserAPNSToken.Columns.Token, token);
                        qry.Execute();
                    });
            }
        }
    }
}
