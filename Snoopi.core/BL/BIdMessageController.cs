using dg.Sql;
using dg.Sql.Connector;
using Snoopi.core.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Collections.Concurrent;
using dg.Utilities;

namespace Snoopi.core.BL
{
    public class BIdMessageController
    {
        static BIdMessageController()
        {
            _prioritizedSuppliers = new List<long>();
            var prioritizedSuppliers = AppConfig.GetString(@"prioritized_suppliers", @"43");
            var prioritizedSuppliersArray = prioritizedSuppliers.Split(',');
            foreach(string supplier in prioritizedSuppliersArray)
            {
                long supplierId;
                long.TryParse(supplier, out supplierId);
                if (supplierId > 0)
                    _prioritizedSuppliers.Add(supplierId);
            }
        }
        public const string START_STAGE = "startStage";
        public const string SUPPLIER_STAGE = "supplier";
        public const string PREMIUM_STAGE = "premium";
        public const string SPECIAL_DEAL_STAGE = "specialDeal";
        public const string ADMIN_STAGE = "admin";
        private static ConcurrentQueue<BidMessage> WaitingEmails = new ConcurrentQueue<BidMessage>();
        private static List<long> _prioritizedSuppliers;
        #region public methods

        public static Int64 AddNewMessage(Int64 bidId, Int64 supplierId, long originalSupplierId = 0, string state = START_STAGE)
        {
            using (ConnectorBase conn = ConnectorBase.NewInstance())
            {
                //BidMessage message = get_message_by_bid_and_supplier(bidId, supplierId);
                // if (message != null) return 0;
                BidMessage message = new BidMessage();
                message.BidId = bidId;
                message.SupplierId = supplierId;
                message.Stage = state;
                message.IsActive = true;
                message.ExpirationTime = DateTime.Now;
                message.SendingTime = DateTime.Now;
                message.OriginalSupplierId = originalSupplierId==0? supplierId : originalSupplierId;
                message.Save(conn);
                return message.MessageId;
            }
        }

        public static BidMessage GetMessageByBidAndSupplier(Int64 bidId, Int64 supplierId)
        {
            return get_message_by_bid_and_supplier(bidId, supplierId);
        }

        public static bool IsSystemActive(DateTime now)
        {
            bool isSystemActive= Settings.GetSettingBool(Settings.Keys.IS_SYSTEM_ACTIVE, false);
            bool isActive = isSystemActive && IsInWorkingHours(now);
            return isActive;
        }

        public static bool IsInWorkingHours(DateTime now)
        {
            var startTime = Settings.GetSettingInt64(Settings.Keys.START_WORKING_TIME, 9);
            var endTime = Settings.GetSettingInt64(Settings.Keys.END_WORKING_TIME, 18);

            if (now.Hour < startTime || now.Hour > endTime)
                return false;
            return true;
        }

        #endregion

        #region internal methods

        internal static List<BidMessage> GetAllRelevantMessages(DateTime now)
        {
            var qry = new Query(BidMessage.TableSchema);
            qry.Where(BidMessage.Columns.IsActive, true);
            qry.AddWhere(BidMessage.Columns.ExpirationTime, WhereComparision.LessThan, now);
            var messageList = new List<BidMessage>();
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                while (reader.Read())
                {
                    BidMessage item = new BidMessage();
                    item.Read(reader);
                    messageList.Add(item);
                }
            }
            return messageList;
        }

        public static List<BidMessage> GetAllActiveMessagesByBidId(long bidId)
        {
            var qry = new Query(BidMessage.TableSchema);
            qry.Where(BidMessage.Columns.IsActive, true);
            qry.AddWhere(BidMessage.Columns.BidId, WhereComparision.EqualsTo, bidId);
            var messageList = new List<BidMessage>();
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                while (reader.Read())
                {
                    BidMessage item = new BidMessage();
                    item.Read(reader);
                    messageList.Add(item);
                }
            }
            return messageList;
        }

        public static List<BidMessage> GetAllMessagesByBidId(long bidId)
        {
            var qry = new Query(BidMessage.TableSchema);
            qry.AddWhere(BidMessage.Columns.BidId, WhereComparision.EqualsTo, bidId);
            var messageList = new List<BidMessage>();
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                while (reader.Read())
                {
                    BidMessage item = new BidMessage();
                    item.Read(reader);
                    messageList.Add(item);
                }
            }
            return messageList;
        }

        internal static void SendMessages(List<BidMessage> messages, DateTime now)
        {
            foreach (var msg in messages)
            {
                switch (msg.Stage)
                {
                    case START_STAGE:
                        WaitingEmails.Enqueue(msg);
                        SupplierNotification.SendNotificationNewBidToSupplier(msg.BidId, new List<Int64> { msg.SupplierId });
                        msg.ExpirationTime = now.AddMinutes(Settings.GetSettingInt64(Settings.Keys.MESSAGE_EXPIRATION_SUPPLIER, 60));
                        msg.Stage = SUPPLIER_STAGE;
                        msg.Save();
                        break;

                    case PREMIUM_STAGE:
                    case SUPPLIER_STAGE:
                        long supplierId = get_primium_supplier(msg);
                        if (supplierId == 0)
                            goto case SPECIAL_DEAL_STAGE;
                        SupplierNotification.SendNotificationNewBidToPremiumSupplier(msg.BidId, supplierId);
                        var newPremMessage = new BidMessage(msg);
                        newPremMessage.Stage = PREMIUM_STAGE;
                        newPremMessage.SupplierId = supplierId;
                        newPremMessage.ExpirationTime = now.AddMinutes(Settings.GetSettingInt64(Settings.Keys.MESSAGE_EXPIRATION_PREMIUM, 20));
                        newPremMessage.Save();
                        WaitingEmails.Enqueue(newPremMessage);
                        goto default;

                    case SPECIAL_DEAL_STAGE:
                        var supplierList = get_relevant_suppliers(msg);
                        if (supplierList.Count <= 0)
                        {
                            var bid = Bid.FetchByID(msg.BidId);
                            if (!bid.IsActive)
                                goto default;
                            goto case ADMIN_STAGE;
                        }
                        foreach (int sId in supplierList)
                        {
                            var newSpecMessage = new BidMessage(msg);
                            newSpecMessage.SupplierId = sId;
                            newSpecMessage.ExpirationTime = now.AddMinutes(Settings.GetSettingInt64(Settings.Keys.MESSAGE_EXPIRATION_SPECIAL_DEAL, 10));
                            newSpecMessage.Stage = SPECIAL_DEAL_STAGE;
                            newSpecMessage.IsActive = false;
                            newSpecMessage.Save();
                            SupplierNotification.SendNotificationNewBidToPremiumSupplier(msg.BidId, sId);
                            WaitingEmails.Enqueue(newSpecMessage);
                        }
                        var forAdminMessage = new BidMessage(msg);
                        forAdminMessage.ExpirationTime = now.AddMinutes(Settings.GetSettingInt64(Settings.Keys.MESSAGE_EXPIRATION_SPECIAL_DEAL, 10));
                        forAdminMessage.Stage = SPECIAL_DEAL_STAGE;
                        forAdminMessage.IsActive = true;
                        forAdminMessage.SupplierId = 1;
                        forAdminMessage.Save();
                        goto default;

                    case ADMIN_STAGE:
                        var expiredDid = Bid.FetchByID(msg.BidId);
                        expiredDid.IsActive = false;
                        expiredDid.Save();
                        msg.Stage = ADMIN_STAGE;
                        send_message_to_admin(msg); 
                        goto default;

                    default:
                        msg.IsActive = false;
                        msg.Save();
                        break;
                }

            }
        }

        public static void SendEmails()
        {
            try
            {
                BidMessage msg;
                while (WaitingEmails.TryDequeue(out msg))
                {
                    EmailMessagingService.SendNewBidToSupplier(msg);
                }       
            }
            catch (Exception ex)
            {
            }
        }

        #endregion

        #region private methods

        private static BidMessage get_message_by_bid_and_supplier(Int64 bidId, Int64 supplierId)
        {
            var qry = new Query(BidMessage.TableSchema);
            qry.Where(BidMessage.Columns.BidId, bidId);
            qry.AddWhere(BidMessage.Columns.SupplierId, supplierId);
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                if (reader.Read())
                {
                    BidMessage item = new BidMessage();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }

        private static void send_message_to_admin(BidMessage msg)
        {
            try
            {
                EmailMessagingService.SendEmailUntakenBidToAdmin(msg);
            }
            catch (Exception)
            {
            }
        }
       
        private static List<long> get_relevant_suppliers(BidMessage msg)
        {
            Int64 cityId = get_bid_city(msg.BidId);
            var query = get_relevant_suppliers_query(cityId, msg.BidId);
            List<long> supplierIds = get_relevant_suppliers(query);
            return supplierIds;
        }

        private static List<long> get_relevant_suppliers(Query query)
        {
            var supplierIds = new List<long>();
            try
            {
                using (DataReaderBase reader = query.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        supplierIds.Add(Convert.ToInt64(reader[SupplierProduct.Columns.SupplierId]));
                    }
                }
            }
            catch (Exception e)
            {

            }
            return supplierIds;
        }

        private static long get_bid_city(long bidId)
        {
            var userIdQuery = new Query(Bid.TableSchema);
            userIdQuery.Where(Bid.Columns.BidId, bidId);
            userIdQuery.Select(Bid.Columns.AppUserId);

            var cityIdQuery = new Query(AppUser.TableSchema);
            cityIdQuery.Where(AppUser.Columns.AppUserId, userIdQuery);
            cityIdQuery.Select(AppUser.Columns.CityId);
            long cityId = 0;
            try
            {
                cityId = (long)cityIdQuery.ExecuteScalar();
            }
            catch (Exception e)
            {

            }
            return cityId;
        }

        private static long get_primium_supplier(BidMessage msg)
        {
            Int64 cityId = get_bid_city(msg.BidId);
            var query = get_relevant_premium_suppliers_query(cityId, msg.BidId);
            List<long> supplierIds = get_relevant_suppliers(query);
            long prioritized = _prioritizedSuppliers.FirstOrDefault(x => supplierIds.Contains(x));
            if (prioritized > 0)
                return prioritized;
            return supplierIds.FirstOrDefault();
        }

        private static Query get_relevant_premium_suppliers_query(long cityId, long bidId)
        {
            var query = get_relevant_suppliers_query(cityId, bidId);
            query.AddWhere(AppSupplier.Columns.IsPremium, true);
            query.Randomize();
            return query;
        }

        private static Query get_relevant_suppliers_query(Int64 cityId, Int64 bidId)
        {
            var cityInnerQuery = new Query(SupplierCity.TableSchema);
            cityInnerQuery.Where(SupplierCity.Columns.CityId, WhereComparision.EqualsTo, cityId);
            cityInnerQuery.Select(SupplierCity.Columns.SupplierId).Distinct();
            var messagesInnerQuery = new Query(BidMessage.TableSchema);
            messagesInnerQuery.Where(BidMessage.Columns.BidId, bidId);
            messagesInnerQuery.Select(BidMessage.Columns.SupplierId);

            var qry = new Query(AppSupplier.TableSchema);
            qry.Where(AppSupplier.Columns.SupplierId, WhereComparision.In, cityInnerQuery);
            qry.AddWhere(AppSupplier.Columns.SupplierId, WhereComparision.NotIn, messagesInnerQuery);
            qry.AddWhere(AppSupplier.Columns.IsDeleted, WhereComparision.NotEqualsTo, true);
            qry.AddWhere( AppSupplier.Columns.IsLocked, WhereComparision.NotEqualsTo, true);
            qry.AddWhere(AppSupplier.Columns.Status, WhereComparision.NotEqualsTo, false);
            qry.AddWhere(AppSupplier.Columns.IsService, WhereComparision.NotEqualsTo, true);

            return qry;
        }

        #endregion
    }
}
