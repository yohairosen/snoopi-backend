using dg.Sql;
using dg.Sql.Connector;
using Snoopi.core.DAL;
using Snoopi.core.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Snoopi.core.BL
{
    public static class NotificationGroups
    {
        #region public

        public static List<NotificationUser> GetUsersOfFilter(NotificationFilter filteringParameters)
        {
            var users = new List<NotificationUser>();
            switch (filteringParameters.Group)
            {
                case NotificationGroupsEnum.All:
                    users = GetAll(filteringParameters);
                    break;
                case NotificationGroupsEnum.Purchase:
                    users = GetPurchased(filteringParameters);
                    break;
                case NotificationGroupsEnum.AddItemToCartWithoutPurchase:
                    break;
                case NotificationGroupsEnum.Registered:
                    users = GetRegistered(filteringParameters);
                    break;
                case NotificationGroupsEnum.NeverPurchased:
                    users = GetNeverPurchased(filteringParameters);
                    break;
                case NotificationGroupsEnum.Members:
                    break;
                case NotificationGroupsEnum.ApprovedPromotionalContent:
                    users = GetPromotionalContent(filteringParameters, true);
                    break;
                case NotificationGroupsEnum.NotApprovedPromotionalContent:
                    users = GetPromotionalContent(filteringParameters, false);
                    break;
                case NotificationGroupsEnum.CreditStageWithoutPurchase:
                    users = GetCreditStageWithoutPurchase(filteringParameters);
                    break;
                case NotificationGroupsEnum.Test:
                    users = GetTest();
                    break;
                case NotificationGroupsEnum.None:
                   // users = GetTest();
                    break;
                default:
                    break;
            }
            return users;
        }

        public static NotificationFilter GetUserRelevantFilter(long userId)
        {
            var allFilters = NotificationsFiltersCache.GetAllNotifications();
            var sortedFilters = allFilters.OrderByDescending(x => x.Priority);
            foreach(var filter in sortedFilters)
            {
                if (filter.Group == NotificationGroupsEnum.All)
                    return filter;
                var users = GetUsersOfFilter(filter);
                var isFound = users.Any(x => x.AppUserId == userId);
                if (isFound)
                    return filter;
            }
            return null;
        }

        public static NotificationFilter GetTempUserFilter()
        {
            var allUsersFilter = NotificationFilterCollection.FetchAll().Where(x => x.Deleted == null && !x.IsAuto && x.Group == NotificationGroupsEnum.All);
            var sortedFilters = allUsersFilter.OrderByDescending(x => x.Priority);
            var filter = allUsersFilter.FirstOrDefault();
            return filter;
        }

        public static List<NotificationFilter> GetAutoFilters()
        {
            var allFilters = NotificationFilterCollection.FetchAll().Where(x => x.Deleted == null && x.IsAuto);
            return allFilters.ToList();
        }

        public static List<NotificationUser> GetUsersOfAutoFilter(NotificationFilter filteringParameters)
        {
            var users = new List<NotificationUser>();
            switch (filteringParameters.Group)
            {
                case NotificationGroupsEnum.AverageOfLastThree:
                    users = GetAverageOfLastThree(filteringParameters);
                    break;
                case NotificationGroupsEnum.DaysSinceLastPurchase:
                    users = GetDaysSincePurchase(filteringParameters);
                    break;
                default:
                    break;
            }
            return users;
        }
        #endregion

        #region private

        private static List<NotificationUser> GetPurchased(NotificationFilter filteringParameters, long userId = 0)
        {
            Query qFcm = new Query(Order.TableSchema);
            qFcm.Join(JoinType.InnerJoin, Order.TableSchema, Order.Columns.AppUserId, Order.TableSchema.SchemaName,
                   AppUser.TableSchema, AppUser.Columns.AppUserId, AppUser.TableSchema.SchemaName);
            qFcm.Join(JoinType.LeftJoin, AppUser.TableSchema, AppUser.Columns.AppUserId, AppUser.TableSchema.SchemaName,
                  AppUserAPNSToken.TableSchema, AppUserAPNSToken.Columns.AppUserId, AppUserAPNSToken.TableSchema.SchemaName);
            qFcm.Join(JoinType.LeftJoin, AppUser.TableSchema, AppUser.Columns.AppUserId, AppUser.TableSchema.SchemaName,
                  AppUserGcmToken.TableSchema, AppUserGcmToken.Columns.AppUserId, AppUserGcmToken.TableSchema.SchemaName);

            qFcm.AddSelect(AppUser.TableSchema.SchemaName, AppUser.Columns.FirstName, AppUser.Columns.FirstName)
               .AddSelect(AppUser.TableSchema.SchemaName, AppUser.Columns.LastName, AppUser.Columns.LastName)
               .AddSelect(AppUser.TableSchema.SchemaName, AppUser.Columns.AppUserId, AppUser.Columns.AppUserId)
               .AddSelect(AppUser.TableSchema.SchemaName, AppUser.Columns.Phone, AppUser.Columns.Phone)
               .AddSelect(AppUserAPNSToken.TableSchema.SchemaName, AppUserAPNSToken.Columns.Token, AppUserAPNSToken.Columns.Token);
            if (filteringParameters.AreaId > 0)
            {
                qFcm.Join(JoinType.InnerJoin, AppUser.TableSchema, AppUser.Columns.CityId, AppUser.TableSchema.SchemaName,
                    City.TableSchema, City.Columns.CityId, City.TableSchema.SchemaName);
                qFcm.AddWhere(City.TableSchema.SchemaName, City.Columns.AreaId, WhereComparision.EqualsTo, filteringParameters.AreaId);
            }

            if (filteringParameters.FromDate != null)
                qFcm.AddWhere(Order.TableSchema.SchemaName, Order.Columns.CreateDate, WhereComparision.GreaterThanOrEqual, filteringParameters.FromDate);
            if (filteringParameters.ToDate != null)
                qFcm.AddWhere(Order.TableSchema.SchemaName, Order.Columns.CreateDate, WhereComparision.LessThanOrEqual, filteringParameters.ToDate.Value.AddDays(1));
            if (filteringParameters.MinFrequency > 0 || filteringParameters.MaxFrequency < int.MaxValue)
            {
                qFcm.Join(JoinType.InnerJoin, "SELECT AppUserId as uid, count(OrderId) numOfOrders FROM  tbl_order GROUP BY AppUserId", "OrdersCount",
                    new JoinColumnPair(Order.TableSchema.SchemaName, Order.Columns.AppUserId, "uid"));
                qFcm.AddWhere("OrdersCount", "numOfOrders", WhereComparision.GreaterThanOrEqual, filteringParameters.MinFrequency);
                qFcm.AddWhere("OrdersCount", "numOfOrders", WhereComparision.LessThanOrEqual, filteringParameters.MaxFrequency);
            }

            if (filteringParameters.AnimalTypeId > 0)
            {
                qFcm.Join(JoinType.InnerJoin, Order.TableSchema, Order.Columns.BidId, Order.TableSchema.SchemaName,
                     BidProduct.TableSchema, BidProduct.Columns.BidId, BidProduct.TableSchema.SchemaName);
                qFcm.Join(JoinType.InnerJoin, BidProduct.TableSchema, BidProduct.Columns.ProductId, BidProduct.TableSchema.SchemaName,
                     Product.TableSchema, Product.Columns.ProductId, Product.TableSchema.SchemaName);
                qFcm.Join(JoinType.InnerJoin, Product.TableSchema, Product.Columns.ProductId, Product.TableSchema.SchemaName,
                     ProductAnimal.TableSchema, ProductAnimal.Columns.ProductId, ProductAnimal.TableSchema.SchemaName);
                qFcm.AddWhere(ProductAnimal.TableSchema.SchemaName, ProductAnimal.Columns.AnimalId, WhereComparision.EqualsTo, filteringParameters.AnimalTypeId);
            }
            qFcm.GroupBy(AppUser.Columns.FirstName).GroupBy(AppUser.Columns.LastName).GroupBy(AppUser.Columns.AppUserId).GroupBy(AppUser.Columns.Phone);
            qFcm.AddWhere(AppUser.TableSchema.SchemaName, AppUser.Columns.IsDeleted, WhereComparision.EqualsTo, false);
            qFcm.AddWhere(AppUserGcmToken.TableSchema.SchemaName, AppUserGcmToken.Columns.Token, WhereComparision.EqualsTo, null);

            var LstFcmUser = new List<NotificationUser>();
            using (DataReaderBase reader = qFcm.ExecuteReader())
            {
                while (reader.Read())
                {
                    var user = new NotificationUser
                    {
                        AppUserId = int.Parse(reader[AppUser.Columns.AppUserId].ToString()),
                        FirstName = reader[AppUser.Columns.FirstName].ToString(),
                        LastName = reader[AppUser.Columns.LastName].ToString(),
                        Phone = reader[AppUser.Columns.Phone].ToString(),
                        ApnToken = reader[AppUserAPNSToken.Columns.Token] == null ? null: reader[AppUserAPNSToken.Columns.Token].ToString()
                    };
                    var countOfUser = LstFcmUser.Where(x => x.AppUserId == user.AppUserId);
                    if (countOfUser.Count() <= 1)
                        LstFcmUser.Add(user);
                }
            }

            Query qApn = new Query(Order.TableSchema);
            qApn.Join(JoinType.InnerJoin, Order.TableSchema, Order.Columns.AppUserId, Order.TableSchema.SchemaName,
                   AppUser.TableSchema, AppUser.Columns.AppUserId, AppUser.TableSchema.SchemaName);
           qApn.Join(JoinType.InnerJoin, AppUser.TableSchema, AppUser.Columns.AppUserId, AppUser.TableSchema.SchemaName,
                 AppUserGcmToken.TableSchema, AppUserGcmToken.Columns.AppUserId, AppUserGcmToken.TableSchema.SchemaName);

            qApn.AddSelect(AppUser.TableSchema.SchemaName, AppUser.Columns.FirstName, AppUser.Columns.FirstName)
               .AddSelect(AppUser.TableSchema.SchemaName, AppUser.Columns.LastName, AppUser.Columns.LastName)
               .AddSelect(AppUser.TableSchema.SchemaName, AppUser.Columns.AppUserId, AppUser.Columns.AppUserId)
               .AddSelect(AppUser.TableSchema.SchemaName, AppUser.Columns.Phone, AppUser.Columns.Phone)
               .AddSelect(AppUserGcmToken.TableSchema.SchemaName, AppUserGcmToken.Columns.Token, AppUserGcmToken.Columns.Token);
            if (filteringParameters.AreaId > 0)
            {
                qApn.Join(JoinType.InnerJoin, AppUser.TableSchema, AppUser.Columns.CityId, AppUser.TableSchema.SchemaName,
                    City.TableSchema, City.Columns.CityId, City.TableSchema.SchemaName);
                qApn.AddWhere(City.TableSchema.SchemaName, City.Columns.AreaId, WhereComparision.EqualsTo, filteringParameters.AreaId);
            }

            if (filteringParameters.FromDate != null)
                qApn.AddWhere(Order.TableSchema.SchemaName, Order.Columns.CreateDate, WhereComparision.GreaterThanOrEqual, filteringParameters.FromDate);
            if (filteringParameters.ToDate != null)
                qApn.AddWhere(Order.TableSchema.SchemaName, Order.Columns.CreateDate, WhereComparision.LessThanOrEqual, filteringParameters.ToDate.Value.AddDays(1));
            if (filteringParameters.MinFrequency > 0 || filteringParameters.MaxFrequency < int.MaxValue)
            {
                qApn.Join(JoinType.InnerJoin, "SELECT AppUserId as uid, count(OrderId) numOfOrders FROM  tbl_order GROUP BY AppUserId", "OrdersCount",
                    new JoinColumnPair(Order.TableSchema.SchemaName, Order.Columns.AppUserId, "uid"));
                qApn.AddWhere("OrdersCount", "numOfOrders", WhereComparision.GreaterThanOrEqual, filteringParameters.MinFrequency);
                qApn.AddWhere("OrdersCount", "numOfOrders", WhereComparision.LessThanOrEqual, filteringParameters.MaxFrequency);
            }

            if (filteringParameters.AnimalTypeId > 0)
            {
                qApn.Join(JoinType.InnerJoin, Order.TableSchema, Order.Columns.BidId, Order.TableSchema.SchemaName,
                     BidProduct.TableSchema, BidProduct.Columns.BidId, BidProduct.TableSchema.SchemaName);
                qApn.Join(JoinType.InnerJoin, BidProduct.TableSchema, BidProduct.Columns.ProductId, BidProduct.TableSchema.SchemaName,
                     Product.TableSchema, Product.Columns.ProductId, Product.TableSchema.SchemaName);
                qApn.Join(JoinType.InnerJoin, Product.TableSchema, Product.Columns.ProductId, Product.TableSchema.SchemaName,
                     ProductAnimal.TableSchema, ProductAnimal.Columns.ProductId, ProductAnimal.TableSchema.SchemaName);
                qApn.AddWhere(ProductAnimal.TableSchema.SchemaName, ProductAnimal.Columns.AnimalId, WhereComparision.EqualsTo, filteringParameters.AnimalTypeId);
            }
            qApn.GroupBy(AppUser.Columns.FirstName).GroupBy(AppUser.Columns.LastName).GroupBy(AppUser.Columns.AppUserId).GroupBy(AppUser.Columns.Phone);
            qApn.AddWhere(AppUser.TableSchema.SchemaName, AppUser.Columns.IsDeleted, WhereComparision.EqualsTo, false);
            var LstApnUser = new List<NotificationUser>();

            using (DataReaderBase reader = qApn.ExecuteReader())
            {
                while (reader.Read())
                {
                    var user = new NotificationUser
                    {
                        AppUserId = int.Parse(reader[AppUser.Columns.AppUserId].ToString()),
                        FirstName = reader[AppUser.Columns.FirstName].ToString(),
                        LastName = reader[AppUser.Columns.LastName].ToString(),
                        Phone = reader[AppUser.Columns.Phone].ToString(),
                        FcmToken = reader[AppUserGcmToken.Columns.Token] == null ? null : reader[AppUserGcmToken.Columns.Token].ToString()
                    };
                    var countOfUser = LstApnUser.Where(x => x.AppUserId == user.AppUserId);
                    if (countOfUser.Count() <= 1)
                        LstApnUser.Add(user);
                }
            }
            var LstUser = new List<NotificationUser>();
            LstUser.AddRange(LstFcmUser);
            LstUser.AddRange(LstApnUser);
            return LstUser;
        }

        private static List<NotificationUser> GetAll(NotificationFilter filteringParameters)
        {
            Query qFcm = new Query(AppUser.TableSchema);
            qFcm.Join(JoinType.LeftJoin, AppUser.TableSchema, AppUser.Columns.AppUserId, AppUser.TableSchema.SchemaName,
                 AppUserGcmToken.TableSchema, AppUserGcmToken.Columns.AppUserId, AppUserGcmToken.TableSchema.SchemaName);
            qFcm.Join(JoinType.LeftJoin, AppUser.TableSchema, AppUser.Columns.AppUserId, AppUser.TableSchema.SchemaName,
                   AppUserAPNSToken.TableSchema, AppUserAPNSToken.Columns.AppUserId, AppUserAPNSToken.TableSchema.SchemaName);
            qFcm.AddSelect(AppUser.TableSchema.SchemaName, AppUser.Columns.FirstName, AppUser.Columns.FirstName)
               .AddSelect(AppUser.TableSchema.SchemaName, AppUser.Columns.LastName, AppUser.Columns.LastName)
               .AddSelect(AppUser.TableSchema.SchemaName, AppUser.Columns.AppUserId, AppUser.Columns.AppUserId)
               .AddSelect(AppUser.TableSchema.SchemaName, AppUser.Columns.Phone, AppUser.Columns.Phone)
               .AddSelect(AppUserGcmToken.TableSchema.SchemaName, AppUserGcmToken.Columns.Token, AppUserGcmToken.Columns.Token);
            if (filteringParameters.AreaId > 0)
            {
                qFcm.Join(JoinType.InnerJoin, AppUser.TableSchema, AppUser.Columns.CityId, AppUser.TableSchema.SchemaName,
                    City.TableSchema, City.Columns.CityId, City.TableSchema.SchemaName);
                qFcm.AddWhere(City.TableSchema.SchemaName, City.Columns.AreaId, WhereComparision.EqualsTo, filteringParameters.AreaId);
            }
            qFcm.AddWhere(AppUser.TableSchema.SchemaName, AppUser.Columns.IsDeleted, WhereComparision.EqualsTo, false);
            qFcm.AddWhere(AppUserAPNSToken.TableSchema.SchemaName, AppUserAPNSToken.Columns.Token, WhereComparision.EqualsTo, null);

            var LstFcmUser = new List<NotificationUser>();
            using (DataReaderBase reader = qFcm.ExecuteReader())
            {
                while (reader.Read())
                {
                    var user = new NotificationUser
                    {
                        AppUserId = int.Parse(reader[AppUser.Columns.AppUserId].ToString()),
                        FirstName = reader[AppUser.Columns.FirstName].ToString(),
                        LastName = reader[AppUser.Columns.LastName].ToString(),
                        Phone = reader[AppUser.Columns.Phone].ToString(),
                        FcmToken = reader[AppUserGcmToken.Columns.Token] == null ? null : reader[AppUserGcmToken.Columns.Token].ToString(),
                        IsTempUser = false
                    };
                    var countOfUser = LstFcmUser.Where(x => x.AppUserId == user.AppUserId);
                    if (countOfUser.Count() <= 1)
                        LstFcmUser.Add(user);
                }
            }

            Query qApn = new Query(AppUser.TableSchema);
            qApn.Join(JoinType.InnerJoin, AppUser.TableSchema, AppUser.Columns.AppUserId, AppUser.TableSchema.SchemaName,
                   AppUserAPNSToken.TableSchema, AppUserAPNSToken.Columns.AppUserId, AppUserAPNSToken.TableSchema.SchemaName);

            qApn.AddSelect(AppUser.TableSchema.SchemaName, AppUser.Columns.FirstName, AppUser.Columns.FirstName)
               .AddSelect(AppUser.TableSchema.SchemaName, AppUser.Columns.LastName, AppUser.Columns.LastName)
               .AddSelect(AppUser.TableSchema.SchemaName, AppUser.Columns.AppUserId, AppUser.Columns.AppUserId)
               .AddSelect(AppUser.TableSchema.SchemaName, AppUser.Columns.Phone, AppUser.Columns.Phone)
               .AddSelect(AppUserAPNSToken.TableSchema.SchemaName, AppUserAPNSToken.Columns.Token, AppUserAPNSToken.Columns.Token);
            if (filteringParameters.AreaId > 0)
            {
                qApn.Join(JoinType.InnerJoin, AppUser.TableSchema, AppUser.Columns.CityId, AppUser.TableSchema.SchemaName,
                    City.TableSchema, City.Columns.CityId, City.TableSchema.SchemaName);
                qApn.AddWhere(City.TableSchema.SchemaName, City.Columns.AreaId, WhereComparision.EqualsTo, filteringParameters.AreaId);
            }
            qApn.AddWhere(AppUser.TableSchema.SchemaName, AppUser.Columns.IsDeleted, WhereComparision.EqualsTo, false);
            var LstApnUser = new List<NotificationUser>();
            using (DataReaderBase reader = qApn.ExecuteReader())
            {
                while (reader.Read())
                {
                    var user = new NotificationUser
                    {
                        AppUserId = int.Parse(reader[AppUser.Columns.AppUserId].ToString()),
                        FirstName = reader[AppUser.Columns.FirstName].ToString(),
                        LastName = reader[AppUser.Columns.LastName].ToString(),
                        Phone = reader[AppUser.Columns.Phone].ToString(),
                        ApnToken = reader[AppUserAPNSToken.Columns.Token] == null ? null : reader[AppUserAPNSToken.Columns.Token].ToString(),
                        IsTempUser = false
                    };
                    var countOfUser = LstApnUser.Where(x => x.AppUserId == user.AppUserId);
                    if (countOfUser.Count() <= 1)
                        LstApnUser.Add(user);
                }
            }

            Query qTemp = new Query(TempAppUser.TableSchema);
            qTemp.AddSelect(TempAppUser.TableSchema.SchemaName, TempAppUser.Columns.TempAppUserId, TempAppUser.Columns.TempAppUserId)
               .AddSelect(TempAppUser.TableSchema.SchemaName, TempAppUser.Columns.DeviceUdid, "Token");
            if (filteringParameters.AreaId > 0)
            {
                qTemp.Join(JoinType.InnerJoin, TempAppUser.TableSchema, TempAppUser.Columns.CityId, TempAppUser.TableSchema.SchemaName,
                    City.TableSchema, City.Columns.CityId, City.TableSchema.SchemaName);
                qTemp.AddWhere(City.TableSchema.SchemaName, City.Columns.AreaId, WhereComparision.EqualsTo, filteringParameters.AreaId);
            }
            var LstTempUser = new List<NotificationUser>();
            using (DataReaderBase reader = qTemp.ExecuteReader())
            {
                while (reader.Read())
                {
                    var token = reader["Token"] == null ? null : reader["Token"].ToString();
                    if (token != null && token.Length == 64)
                    {
                        var user = new NotificationUser
                        {
                            AppUserId = int.Parse(reader[TempAppUser.Columns.TempAppUserId].ToString()),
                            //FcmToken = ,
                            ApnToken = token,
                            IsTempUser = true
                        };
                        LstTempUser.Add(user);
                    }
                }
            }
            var LstUser = new List<NotificationUser>();
            LstUser.AddRange(LstFcmUser);
            LstUser.AddRange(LstApnUser);
            LstUser.AddRange(LstTempUser);
            return LstUser;
        }

        private static List<NotificationUser> GetRegistered(NotificationFilter filteringParameters, long userId = 0)
        {
            Query qApn = new Query(AppUser.TableSchema);
            qApn.Join(JoinType.LeftJoin, AppUser.TableSchema, AppUser.Columns.AppUserId, AppUser.TableSchema.SchemaName,
                   AppUserAPNSToken.TableSchema, AppUserAPNSToken.Columns.AppUserId, AppUserAPNSToken.TableSchema.SchemaName);
            qApn.Join(JoinType.LeftJoin, AppUser.TableSchema, AppUser.Columns.AppUserId, AppUser.TableSchema.SchemaName,
                 AppUserGcmToken.TableSchema, AppUserGcmToken.Columns.AppUserId, AppUserGcmToken.TableSchema.SchemaName);
            qApn.AddSelect(AppUser.TableSchema.SchemaName, AppUser.Columns.FirstName, AppUser.Columns.FirstName)
               .AddSelect(AppUser.TableSchema.SchemaName, AppUser.Columns.LastName, AppUser.Columns.LastName)
               .AddSelect(AppUser.TableSchema.SchemaName, AppUser.Columns.AppUserId, AppUser.Columns.AppUserId)
               .AddSelect(AppUser.TableSchema.SchemaName, AppUser.Columns.Phone, AppUser.Columns.Phone)
               .AddSelect(AppUserAPNSToken.TableSchema.SchemaName, AppUserAPNSToken.Columns.Token, AppUserAPNSToken.Columns.Token);
            if (filteringParameters.AreaId > 0)
            {
                qApn.Join(JoinType.InnerJoin, AppUser.TableSchema, AppUser.Columns.CityId, AppUser.TableSchema.SchemaName,
                    City.TableSchema, City.Columns.CityId, City.TableSchema.SchemaName);
                qApn.AddWhere(City.TableSchema.SchemaName, City.Columns.AreaId, WhereComparision.EqualsTo, filteringParameters.AreaId);
            }

            if (filteringParameters.FromDate != null)
                qApn.AddWhere(AppUser.TableSchema.SchemaName, AppUser.Columns.CreateDate, WhereComparision.GreaterThanOrEqual, filteringParameters.FromDate);
            if (filteringParameters.ToDate != null)
                qApn.AddWhere(AppUser.TableSchema.SchemaName, AppUser.Columns.CreateDate, WhereComparision.LessThanOrEqual, filteringParameters.ToDate.Value.AddDays(1));
            qApn.AddWhere(AppUser.TableSchema.SchemaName, AppUser.Columns.IsDeleted, WhereComparision.EqualsTo, false);
            qApn.AddWhere(AppUserGcmToken.TableSchema.SchemaName, AppUserGcmToken.Columns.Token, WhereComparision.EqualsTo, null);

            var LstFcmUser = new List<NotificationUser>();
            using (DataReaderBase reader = qApn.ExecuteReader())
            {
                while (reader.Read())
                {
                    var user = new NotificationUser
                    {
                        AppUserId = int.Parse(reader[AppUser.Columns.AppUserId].ToString()),
                        FirstName = reader[AppUser.Columns.FirstName].ToString(),
                        LastName = reader[AppUser.Columns.LastName].ToString(),
                        Phone = reader[AppUser.Columns.Phone].ToString(),
                        ApnToken = reader[AppUserAPNSToken.Columns.Token] == null ? null : reader[AppUserAPNSToken.Columns.Token].ToString()
                    };
                    var countOfUser = LstFcmUser.Where(x => x.AppUserId == user.AppUserId);
                    if (countOfUser.Count() <= 1)
                        LstFcmUser.Add(user);
                }
            }

            Query qFcm = new Query(AppUser.TableSchema);
            qFcm.Join(JoinType.InnerJoin, AppUser.TableSchema, AppUser.Columns.AppUserId, AppUser.TableSchema.SchemaName,
                 AppUserGcmToken.TableSchema, AppUserGcmToken.Columns.AppUserId, AppUserGcmToken.TableSchema.SchemaName);

            qFcm.AddSelect(AppUser.TableSchema.SchemaName, AppUser.Columns.FirstName, AppUser.Columns.FirstName)
               .AddSelect(AppUser.TableSchema.SchemaName, AppUser.Columns.LastName, AppUser.Columns.LastName)
               .AddSelect(AppUser.TableSchema.SchemaName, AppUser.Columns.AppUserId, AppUser.Columns.AppUserId)
               .AddSelect(AppUser.TableSchema.SchemaName, AppUser.Columns.Phone, AppUser.Columns.Phone)
               .AddSelect(AppUserGcmToken.TableSchema.SchemaName, AppUserGcmToken.Columns.Token, AppUserGcmToken.Columns.Token);

            if (filteringParameters.AreaId > 0)
            {
                qFcm.Join(JoinType.InnerJoin, AppUser.TableSchema, AppUser.Columns.CityId, AppUser.TableSchema.SchemaName,
                    City.TableSchema, City.Columns.CityId, City.TableSchema.SchemaName);
                qFcm.AddWhere(City.TableSchema.SchemaName, City.Columns.AreaId, WhereComparision.EqualsTo, filteringParameters.AreaId);
            }

            if (filteringParameters.FromDate != null)
                qFcm.AddWhere(AppUser.TableSchema.SchemaName, AppUser.Columns.CreateDate, WhereComparision.GreaterThanOrEqual, filteringParameters.FromDate);
            if (filteringParameters.ToDate != null)
                qFcm.AddWhere(AppUser.TableSchema.SchemaName, AppUser.Columns.CreateDate, WhereComparision.LessThanOrEqual, filteringParameters.ToDate.Value.AddDays(1));
            qFcm.AddWhere(AppUser.TableSchema.SchemaName, AppUser.Columns.IsDeleted, WhereComparision.EqualsTo, false);

            var LstApnUser = new List<NotificationUser>();
            using (DataReaderBase reader = qFcm.ExecuteReader())
            {
                while (reader.Read())
                {
                    var user = new NotificationUser
                    {
                        AppUserId = int.Parse(reader[AppUser.Columns.AppUserId].ToString()),
                        FirstName = reader[AppUser.Columns.FirstName].ToString(),
                        LastName = reader[AppUser.Columns.LastName].ToString(),
                        Phone = reader[AppUser.Columns.Phone].ToString(),
                        FcmToken = reader[AppUserGcmToken.Columns.Token] == null ? null : reader[AppUserGcmToken.Columns.Token].ToString()
                    };
                    var countOfUser = LstApnUser.Where(x => x.AppUserId == user.AppUserId);
                    if (countOfUser.Count() <= 1)
                        LstApnUser.Add(user);
                }
            }
            var LstUser = new List<NotificationUser>();
            LstUser.AddRange(LstFcmUser);
            LstUser.AddRange(LstApnUser);
            return LstUser;
        }

        private static List<NotificationUser> GetNeverPurchased(NotificationFilter filteringParameters, long userId = 0)
        {
            Query qApn = new Query(Order.TableSchema);
            qApn.Join(JoinType.RightJoin, Order.TableSchema, Order.Columns.AppUserId, Order.TableSchema.SchemaName,
                   AppUser.TableSchema, AppUser.Columns.AppUserId, AppUser.TableSchema.SchemaName);
            qApn.Join(JoinType.LeftJoin, AppUser.TableSchema, AppUser.Columns.AppUserId, AppUser.TableSchema.SchemaName,
                    AppUserAPNSToken.TableSchema, AppUserAPNSToken.Columns.AppUserId, AppUserAPNSToken.TableSchema.SchemaName);
            qApn.Join(JoinType.LeftJoin, AppUser.TableSchema, AppUser.Columns.AppUserId, AppUser.TableSchema.SchemaName,
                 AppUserGcmToken.TableSchema, AppUserGcmToken.Columns.AppUserId, AppUserGcmToken.TableSchema.SchemaName);

            qApn.AddSelect(AppUser.TableSchema.SchemaName, AppUser.Columns.FirstName, AppUser.Columns.FirstName)
               .AddSelect(AppUser.TableSchema.SchemaName, AppUser.Columns.LastName, AppUser.Columns.LastName)
               .AddSelect(AppUser.TableSchema.SchemaName, AppUser.Columns.AppUserId, AppUser.Columns.AppUserId)
               .AddSelect(AppUser.TableSchema.SchemaName, AppUser.Columns.Phone, AppUser.Columns.Phone)
               .AddSelect(AppUserAPNSToken.TableSchema.SchemaName, AppUserAPNSToken.Columns.Token, AppUserAPNSToken.Columns.Token);

            qApn.AddWhere(Order.Columns.OrderId, null);
            if (filteringParameters.AreaId > 0)
            {
                qApn.Join(JoinType.InnerJoin, AppUser.TableSchema, AppUser.Columns.CityId, AppUser.TableSchema.SchemaName,
                    City.TableSchema, City.Columns.CityId, City.TableSchema.SchemaName);
                qApn.AddWhere(City.TableSchema.SchemaName, City.Columns.AreaId, WhereComparision.EqualsTo, filteringParameters.AreaId);
            }
            qApn.GroupBy(AppUser.Columns.FirstName).GroupBy(AppUser.Columns.LastName).GroupBy(AppUser.Columns.AppUserId).GroupBy(AppUser.Columns.Phone);
            qApn.AddWhere(AppUser.TableSchema.SchemaName, AppUser.Columns.IsDeleted, WhereComparision.EqualsTo, false);
            qApn.AddWhere(AppUserGcmToken.TableSchema.SchemaName, AppUserGcmToken.Columns.Token, WhereComparision.EqualsTo, null);

            var LstFcmUser = new List<NotificationUser>();
            using (DataReaderBase reader = qApn.ExecuteReader())
            {
                while (reader.Read())
                {
                    var user = new NotificationUser
                    {
                        AppUserId = int.Parse(reader[AppUser.Columns.AppUserId].ToString()),
                        FirstName = reader[AppUser.Columns.FirstName].ToString(),
                        LastName = reader[AppUser.Columns.LastName].ToString(),
                        Phone = reader[AppUser.Columns.Phone].ToString(),
                        ApnToken = reader[AppUserAPNSToken.Columns.Token] == null ? null : reader[AppUserAPNSToken.Columns.Token].ToString()
                    };
                    var countOfUser = LstFcmUser.Where(x => x.AppUserId == user.AppUserId);
                    if (countOfUser.Count() <= 1)
                        LstFcmUser.Add(user);
                }
            }
            Query qFcm = new Query(Order.TableSchema);
            qFcm.Join(JoinType.RightJoin, Order.TableSchema, Order.Columns.AppUserId, Order.TableSchema.SchemaName,
                   AppUser.TableSchema, AppUser.Columns.AppUserId, AppUser.TableSchema.SchemaName);
           qFcm.Join(JoinType.InnerJoin, AppUser.TableSchema, AppUser.Columns.AppUserId, AppUser.TableSchema.SchemaName,
                 AppUserGcmToken.TableSchema, AppUserGcmToken.Columns.AppUserId, AppUserGcmToken.TableSchema.SchemaName);

            qFcm.AddSelect(AppUser.TableSchema.SchemaName, AppUser.Columns.FirstName, AppUser.Columns.FirstName)
               .AddSelect(AppUser.TableSchema.SchemaName, AppUser.Columns.LastName, AppUser.Columns.LastName)
               .AddSelect(AppUser.TableSchema.SchemaName, AppUser.Columns.AppUserId, AppUser.Columns.AppUserId)
               .AddSelect(AppUser.TableSchema.SchemaName, AppUser.Columns.Phone, AppUser.Columns.Phone)
               .AddSelect(AppUserGcmToken.TableSchema.SchemaName, AppUserGcmToken.Columns.Token, AppUserGcmToken.Columns.Token);

            qFcm.AddWhere(Order.Columns.OrderId, null);
            if (filteringParameters.AreaId > 0)
            {
                qFcm.Join(JoinType.InnerJoin, AppUser.TableSchema, AppUser.Columns.CityId, AppUser.TableSchema.SchemaName,
                    City.TableSchema, City.Columns.CityId, City.TableSchema.SchemaName);
                qFcm.AddWhere(City.TableSchema.SchemaName, City.Columns.AreaId, WhereComparision.EqualsTo, filteringParameters.AreaId);
            }
            qFcm.GroupBy(AppUser.Columns.FirstName).GroupBy(AppUser.Columns.LastName).GroupBy(AppUser.Columns.AppUserId).GroupBy(AppUser.Columns.Phone);
            qFcm.AddWhere(AppUser.TableSchema.SchemaName, AppUser.Columns.IsDeleted, WhereComparision.EqualsTo, false);

            var LstApnUser = new List<NotificationUser>();
            using (DataReaderBase reader = qFcm.ExecuteReader())
            {
                while (reader.Read())
                {
                    var user = new NotificationUser
                    {
                        AppUserId = int.Parse(reader[AppUser.Columns.AppUserId].ToString()),
                        FirstName = reader[AppUser.Columns.FirstName].ToString(),
                        LastName = reader[AppUser.Columns.LastName].ToString(),
                        Phone = reader[AppUser.Columns.Phone].ToString(),
                        FcmToken = reader[AppUserGcmToken.Columns.Token] == null ? null : reader[AppUserGcmToken.Columns.Token].ToString(),
                    };
                    var countOfUser = LstApnUser.Where(x => x.AppUserId == user.AppUserId);
                    if (countOfUser.Count() <= 1)
                        LstApnUser.Add(user);
                }
            }
            var LstUser = new List<NotificationUser>();
            LstUser.AddRange(LstFcmUser);
            LstUser.AddRange(LstApnUser);
            return LstUser;
        }

        private static List<NotificationUser> GetPromotionalContent(NotificationFilter filteringParameters, bool isApproved, long userId = 0)
        {
            Query qApn = new Query(AppUser.TableSchema);
            qApn.Join(JoinType.LeftJoin, AppUser.TableSchema, AppUser.Columns.AppUserId, AppUser.TableSchema.SchemaName,
                  AppUserAPNSToken.TableSchema, AppUserAPNSToken.Columns.AppUserId, AppUserAPNSToken.TableSchema.SchemaName);
            qApn.Join(JoinType.LeftJoin, AppUser.TableSchema, AppUser.Columns.AppUserId, AppUser.TableSchema.SchemaName,
               AppUserGcmToken.TableSchema, AppUserGcmToken.Columns.AppUserId, AppUserGcmToken.TableSchema.SchemaName);
            qApn.AddSelect(AppUser.TableSchema.SchemaName, AppUser.Columns.FirstName, AppUser.Columns.FirstName)
               .AddSelect(AppUser.TableSchema.SchemaName, AppUser.Columns.LastName, AppUser.Columns.LastName)
               .AddSelect(AppUser.TableSchema.SchemaName, AppUser.Columns.AppUserId, AppUser.Columns.AppUserId)
               .AddSelect(AppUser.TableSchema.SchemaName, AppUser.Columns.Phone, AppUser.Columns.Phone)
               .AddSelect(AppUserAPNSToken.TableSchema.SchemaName, AppUserAPNSToken.Columns.Token, AppUserAPNSToken.Columns.Token);
            qApn.AddWhere(AppUser.Columns.IsAdv, isApproved);
            if (filteringParameters.AreaId > 0)
            {
                qApn.Join(JoinType.InnerJoin, AppUser.TableSchema, AppUser.Columns.CityId, AppUser.TableSchema.SchemaName,
                    City.TableSchema, City.Columns.CityId, City.TableSchema.SchemaName);
                qApn.AddWhere(City.TableSchema.SchemaName, City.Columns.AreaId, WhereComparision.EqualsTo, filteringParameters.AreaId);
            }

            if (filteringParameters.FromDate != null)
                qApn.AddWhere(AppUser.TableSchema.SchemaName, AppUser.Columns.CreateDate, WhereComparision.GreaterThanOrEqual, filteringParameters.FromDate);
            if (filteringParameters.ToDate != null)
                qApn.AddWhere(AppUser.TableSchema.SchemaName, AppUser.Columns.CreateDate, WhereComparision.LessThanOrEqual, filteringParameters.ToDate.Value.AddDays(1));
            qApn.AddWhere(AppUser.TableSchema.SchemaName, AppUser.Columns.IsDeleted, WhereComparision.EqualsTo, false);
            qApn.AddWhere(AppUserGcmToken.TableSchema.SchemaName, AppUserGcmToken.Columns.Token, WhereComparision.EqualsTo, null);

            var LstFcmUser = new List<NotificationUser>();
            using (DataReaderBase reader = qApn.ExecuteReader())
            {
                while (reader.Read())
                {
                    var user = new NotificationUser
                    {
                        AppUserId = int.Parse(reader[AppUser.Columns.AppUserId].ToString()),
                        FirstName = reader[AppUser.Columns.FirstName].ToString(),
                        LastName = reader[AppUser.Columns.LastName].ToString(),
                        Phone = reader[AppUser.Columns.Phone].ToString(),
                        ApnToken = reader[AppUserAPNSToken.Columns.Token] == null ? null : reader[AppUserAPNSToken.Columns.Token].ToString()
                    };
                    var countOfUser = LstFcmUser.Where(x => x.AppUserId == user.AppUserId);
                    if (countOfUser.Count() <= 1)
                        LstFcmUser.Add(user);
                }
            }
            Query qFcm = new Query(AppUser.TableSchema);
            qFcm.Join(JoinType.InnerJoin, AppUser.TableSchema, AppUser.Columns.AppUserId, AppUser.TableSchema.SchemaName,
                 AppUserGcmToken.TableSchema, AppUserGcmToken.Columns.AppUserId, AppUserGcmToken.TableSchema.SchemaName);

            qFcm.AddSelect(AppUser.TableSchema.SchemaName, AppUser.Columns.FirstName, AppUser.Columns.FirstName)
               .AddSelect(AppUser.TableSchema.SchemaName, AppUser.Columns.LastName, AppUser.Columns.LastName)
               .AddSelect(AppUser.TableSchema.SchemaName, AppUser.Columns.AppUserId, AppUser.Columns.AppUserId)
               .AddSelect(AppUser.TableSchema.SchemaName, AppUser.Columns.Phone, AppUser.Columns.Phone)
               .AddSelect(AppUserGcmToken.TableSchema.SchemaName, AppUserGcmToken.Columns.Token, AppUserGcmToken.Columns.Token);
            qFcm.AddWhere(AppUser.Columns.IsAdv, isApproved);
            if (filteringParameters.AreaId > 0)
            {
                qFcm.Join(JoinType.InnerJoin, AppUser.TableSchema, AppUser.Columns.CityId, AppUser.TableSchema.SchemaName,
                    City.TableSchema, City.Columns.CityId, City.TableSchema.SchemaName);
                qFcm.AddWhere(City.TableSchema.SchemaName, City.Columns.AreaId, WhereComparision.EqualsTo, filteringParameters.AreaId);
            }

            if (filteringParameters.FromDate != null)
                qFcm.AddWhere(AppUser.TableSchema.SchemaName, AppUser.Columns.CreateDate, WhereComparision.GreaterThanOrEqual, filteringParameters.FromDate);
            if (filteringParameters.ToDate != null)
                qFcm.AddWhere(AppUser.TableSchema.SchemaName, AppUser.Columns.CreateDate, WhereComparision.LessThanOrEqual, filteringParameters.ToDate.Value.AddDays(1));
            qFcm.AddWhere(AppUser.TableSchema.SchemaName, AppUser.Columns.IsDeleted, WhereComparision.EqualsTo, false);

            var LstApnUser = new List<NotificationUser>();
            using (DataReaderBase reader = qFcm.ExecuteReader())
            {
                while (reader.Read())
                {
                    var user = new NotificationUser
                    {
                        AppUserId = int.Parse(reader[AppUser.Columns.AppUserId].ToString()),
                        FirstName = reader[AppUser.Columns.FirstName].ToString(),
                        LastName = reader[AppUser.Columns.LastName].ToString(),
                        Phone = reader[AppUser.Columns.Phone].ToString(),
                        FcmToken = reader[AppUserGcmToken.Columns.Token] == null ? null : reader[AppUserGcmToken.Columns.Token].ToString(),
                    };
                    var countOfUser = LstApnUser.Where(x => x.AppUserId == user.AppUserId);
                    if (countOfUser.Count() <= 1)
                        LstApnUser.Add(user);
                }
            }
            var LstUser = new List<NotificationUser>();
            LstUser.AddRange(LstFcmUser);
            LstUser.AddRange(LstApnUser);
            return LstUser;
        }

        private static List<NotificationUser> GetCreditStageWithoutPurchase(NotificationFilter filteringParameters, long userId = 0)
        {
            Query qApn = new Query(PreOrder.TableSchema);
            qApn.Join(JoinType.LeftJoin, PreOrder.TableSchema, PreOrder.Columns.BidId, PreOrder.TableSchema.SchemaName,
                   Order.TableSchema, Order.Columns.BidId, Order.TableSchema.SchemaName);
            qApn.Join(JoinType.InnerJoin, PreOrder.TableSchema, PreOrder.Columns.BidId, PreOrder.TableSchema.SchemaName,
                 Bid.TableSchema, Bid.Columns.BidId, Bid.TableSchema.SchemaName);
            qApn.Join(JoinType.InnerJoin, Bid.TableSchema, Bid.Columns.AppUserId, Bid.TableSchema.SchemaName,
                   AppUser.TableSchema, AppUser.Columns.AppUserId, AppUser.TableSchema.SchemaName);
            qApn.Join(JoinType.LeftJoin, AppUser.TableSchema, AppUser.Columns.AppUserId, AppUser.TableSchema.SchemaName,
                    AppUserAPNSToken.TableSchema, AppUserAPNSToken.Columns.AppUserId, AppUserAPNSToken.TableSchema.SchemaName);
            qApn.Join(JoinType.LeftJoin, AppUser.TableSchema, AppUser.Columns.AppUserId, AppUser.TableSchema.SchemaName,
               AppUserGcmToken.TableSchema, AppUserGcmToken.Columns.AppUserId, AppUserGcmToken.TableSchema.SchemaName);

            qApn.AddSelect(AppUser.TableSchema.SchemaName, AppUser.Columns.FirstName, AppUser.Columns.FirstName)
               .AddSelect(AppUser.TableSchema.SchemaName, AppUser.Columns.LastName, AppUser.Columns.LastName)
               .AddSelect(AppUser.TableSchema.SchemaName, AppUser.Columns.AppUserId, AppUser.Columns.AppUserId)
               .AddSelect(AppUser.TableSchema.SchemaName, AppUser.Columns.Phone, AppUser.Columns.Phone)
               .AddSelect(AppUserAPNSToken.TableSchema.SchemaName, AppUserAPNSToken.Columns.Token, AppUserAPNSToken.Columns.Token);
            qApn.AddWhere(Order.Columns.OrderId, null);

            if (filteringParameters.AreaId > 0)
            {
                qApn.Join(JoinType.InnerJoin, AppUser.TableSchema, AppUser.Columns.CityId, AppUser.TableSchema.SchemaName,
                    City.TableSchema, City.Columns.CityId, City.TableSchema.SchemaName);
                qApn.AddWhere(City.TableSchema.SchemaName, City.Columns.AreaId, WhereComparision.EqualsTo, filteringParameters.AreaId);
            }

            if (filteringParameters.FromDate != null)
                qApn.AddWhere(PreOrder.TableSchema.SchemaName, PreOrder.Columns.Created, WhereComparision.GreaterThanOrEqual, filteringParameters.FromDate);
            if (filteringParameters.ToDate != null)
                qApn.AddWhere(PreOrder.TableSchema.SchemaName, PreOrder.Columns.Created, WhereComparision.LessThanOrEqual, filteringParameters.ToDate.Value.AddDays(1));
            if (filteringParameters.MinFrequency > 0 || filteringParameters.MaxFrequency < int.MaxValue)
            {
                qApn.Join(JoinType.InnerJoin, "SELECT AppUserId as uid, count(*) numOfOrders FROM  preorder pr INNER JOIN bid b ON pr.BidId = b.BidId GROUP BY AppUserId", "OrdersCount",
                    new JoinColumnPair(AppUser.TableSchema.SchemaName, AppUser.Columns.AppUserId, "uid"));
                qApn.AddWhere("OrdersCount", "numOfOrders", WhereComparision.GreaterThanOrEqual, filteringParameters.MinFrequency);
                qApn.AddWhere("OrdersCount", "numOfOrders", WhereComparision.LessThanOrEqual, filteringParameters.MaxFrequency);
            }

            if (filteringParameters.AnimalTypeId > 0)
            {
                qApn.Join(JoinType.InnerJoin, PreOrder.TableSchema, PreOrder.Columns.BidId, PreOrder.TableSchema.SchemaName,
                     BidProduct.TableSchema, BidProduct.Columns.BidId, BidProduct.TableSchema.SchemaName);
                qApn.Join(JoinType.InnerJoin, BidProduct.TableSchema, BidProduct.Columns.ProductId, BidProduct.TableSchema.SchemaName,
                     Product.TableSchema, Product.Columns.ProductId, Product.TableSchema.SchemaName);
                qApn.Join(JoinType.InnerJoin, Product.TableSchema, Product.Columns.ProductId, Product.TableSchema.SchemaName,
                     ProductAnimal.TableSchema, ProductAnimal.Columns.ProductId, ProductAnimal.TableSchema.SchemaName);
                qApn.AddWhere(ProductAnimal.TableSchema.SchemaName, ProductAnimal.Columns.AnimalId, WhereComparision.EqualsTo, filteringParameters.AnimalTypeId);
            }
            qApn.GroupBy(AppUser.Columns.FirstName).GroupBy(AppUser.Columns.LastName).GroupBy(AppUser.Columns.AppUserId).GroupBy(AppUser.Columns.Phone);
            qApn.AddWhere(AppUser.TableSchema.SchemaName, AppUser.Columns.IsDeleted, WhereComparision.EqualsTo, false);
            qApn.AddWhere(AppUserGcmToken.TableSchema.SchemaName, AppUserGcmToken.Columns.Token, WhereComparision.EqualsTo, null);

            var LstFcmUser = new List<NotificationUser>();
            using (DataReaderBase reader = qApn.ExecuteReader())
            {
                while (reader.Read())
                {
                    var user = new NotificationUser
                    {
                        AppUserId = int.Parse(reader[AppUser.Columns.AppUserId].ToString()),
                        FirstName = reader[AppUser.Columns.FirstName].ToString(),
                        LastName = reader[AppUser.Columns.LastName].ToString(),
                        Phone = reader[AppUser.Columns.Phone].ToString(),
                        ApnToken = reader[AppUserAPNSToken.Columns.Token] == null ? null : reader[AppUserAPNSToken.Columns.Token].ToString()
                    };
                    var countOfUser = LstFcmUser.Where(x => x.AppUserId == user.AppUserId);
                    if (countOfUser.Count() <= 1)
                        LstFcmUser.Add(user);
                }
            }
            Query qFcm = new Query(PreOrder.TableSchema);
            qFcm.Join(JoinType.LeftJoin, PreOrder.TableSchema, PreOrder.Columns.BidId, PreOrder.TableSchema.SchemaName,
                   Order.TableSchema, Order.Columns.BidId, Order.TableSchema.SchemaName);
            qFcm.Join(JoinType.InnerJoin, PreOrder.TableSchema, PreOrder.Columns.BidId, PreOrder.TableSchema.SchemaName,
                 Bid.TableSchema, Bid.Columns.BidId, Bid.TableSchema.SchemaName);
            qFcm.Join(JoinType.InnerJoin, Bid.TableSchema, Bid.Columns.AppUserId, Bid.TableSchema.SchemaName,
                   AppUser.TableSchema, AppUser.Columns.AppUserId, AppUser.TableSchema.SchemaName);
            qFcm.Join(JoinType.InnerJoin, AppUser.TableSchema, AppUser.Columns.AppUserId, AppUser.TableSchema.SchemaName,
                 AppUserGcmToken.TableSchema, AppUserGcmToken.Columns.AppUserId, AppUserGcmToken.TableSchema.SchemaName);

            qFcm.AddSelect(AppUser.TableSchema.SchemaName, AppUser.Columns.FirstName, AppUser.Columns.FirstName)
               .AddSelect(AppUser.TableSchema.SchemaName, AppUser.Columns.LastName, AppUser.Columns.LastName)
               .AddSelect(AppUser.TableSchema.SchemaName, AppUser.Columns.AppUserId, AppUser.Columns.AppUserId)
               .AddSelect(AppUser.TableSchema.SchemaName, AppUser.Columns.Phone, AppUser.Columns.Phone)
               .AddSelect(AppUserGcmToken.TableSchema.SchemaName, AppUserGcmToken.Columns.Token, AppUserGcmToken.Columns.Token);
            qFcm.AddWhere(Order.Columns.OrderId, null);

            if (filteringParameters.AreaId > 0)
            {
                qFcm.Join(JoinType.InnerJoin, AppUser.TableSchema, AppUser.Columns.CityId, AppUser.TableSchema.SchemaName,
                    City.TableSchema, City.Columns.CityId, City.TableSchema.SchemaName);
                qFcm.AddWhere(City.TableSchema.SchemaName, City.Columns.AreaId, WhereComparision.EqualsTo, filteringParameters.AreaId);
            }

            if (filteringParameters.FromDate != null)
                qFcm.AddWhere(PreOrder.TableSchema.SchemaName, PreOrder.Columns.Created, WhereComparision.GreaterThanOrEqual, filteringParameters.FromDate);
            if (filteringParameters.ToDate != null)
                qFcm.AddWhere(PreOrder.TableSchema.SchemaName, PreOrder.Columns.Created, WhereComparision.LessThanOrEqual, filteringParameters.ToDate.Value.AddDays(1));
            if (filteringParameters.MinFrequency > 0 || filteringParameters.MaxFrequency < int.MaxValue)
            {
                qFcm.Join(JoinType.InnerJoin, "SELECT AppUserId as uid, count(*) numOfOrders FROM  preorder pr INNER JOIN bid b ON pr.BidId = b.BidId GROUP BY AppUserId", "OrdersCount",
                    new JoinColumnPair(AppUser.TableSchema.SchemaName, AppUser.Columns.AppUserId, "uid"));
                qFcm.AddWhere("OrdersCount", "numOfOrders", WhereComparision.GreaterThanOrEqual, filteringParameters.MinFrequency);
                qFcm.AddWhere("OrdersCount", "numOfOrders", WhereComparision.LessThanOrEqual, filteringParameters.MaxFrequency);
            }

            if (filteringParameters.AnimalTypeId > 0)
            {
                qFcm.Join(JoinType.InnerJoin, PreOrder.TableSchema, PreOrder.Columns.BidId, PreOrder.TableSchema.SchemaName,
                     BidProduct.TableSchema, BidProduct.Columns.BidId, BidProduct.TableSchema.SchemaName);
                qFcm.Join(JoinType.InnerJoin, BidProduct.TableSchema, BidProduct.Columns.ProductId, BidProduct.TableSchema.SchemaName,
                     Product.TableSchema, Product.Columns.ProductId, Product.TableSchema.SchemaName);
                qFcm.Join(JoinType.InnerJoin, Product.TableSchema, Product.Columns.ProductId, Product.TableSchema.SchemaName,
                     ProductAnimal.TableSchema, ProductAnimal.Columns.ProductId, ProductAnimal.TableSchema.SchemaName);
                qFcm.AddWhere(ProductAnimal.TableSchema.SchemaName, ProductAnimal.Columns.AnimalId, WhereComparision.EqualsTo, filteringParameters.AnimalTypeId);
            }
            qFcm.GroupBy(AppUser.Columns.FirstName).GroupBy(AppUser.Columns.LastName).GroupBy(AppUser.Columns.AppUserId).GroupBy(AppUser.Columns.Phone);
            qFcm.AddWhere(AppUser.TableSchema.SchemaName, AppUser.Columns.IsDeleted, WhereComparision.EqualsTo, false);

            var LstApnUser = new List<NotificationUser>();
            using (DataReaderBase reader = qFcm.ExecuteReader())
            {
                while (reader.Read())
                {
                    var user = new NotificationUser
                    {
                        AppUserId = int.Parse(reader[AppUser.Columns.AppUserId].ToString()),
                        FirstName = reader[AppUser.Columns.FirstName].ToString(),
                        LastName = reader[AppUser.Columns.LastName].ToString(),
                        Phone = reader[AppUser.Columns.Phone].ToString(),
                        FcmToken = reader[AppUserGcmToken.Columns.Token] == null ? null : reader[AppUserGcmToken.Columns.Token].ToString(),
                    };
                    var countOfUser = LstApnUser.Where(x => x.AppUserId == user.AppUserId);
                    if (countOfUser.Count() <= 1)
                        LstApnUser.Add(user);
                }
            }
            var LstUser = new List<NotificationUser>();
            LstUser.AddRange(LstFcmUser);
            LstUser.AddRange(LstApnUser);
            return LstUser;
        }

        private static List<NotificationUser> GetTest()
        {
            Query qApn = new Query(AppUser.TableSchema);
            qApn.Join(JoinType.LeftJoin, AppUser.TableSchema, AppUser.Columns.AppUserId, AppUser.TableSchema.SchemaName,
                AppUserAPNSToken.TableSchema, AppUserAPNSToken.Columns.AppUserId, AppUserAPNSToken.TableSchema.SchemaName);
            qApn.Join(JoinType.LeftJoin, AppUser.TableSchema, AppUser.Columns.AppUserId, AppUser.TableSchema.SchemaName,
              AppUserGcmToken.TableSchema, AppUserGcmToken.Columns.AppUserId, AppUserGcmToken.TableSchema.SchemaName);

            qApn.AddSelect(AppUser.TableSchema.SchemaName, AppUser.Columns.FirstName, AppUser.Columns.FirstName)
               .AddSelect(AppUser.TableSchema.SchemaName, AppUser.Columns.LastName, AppUser.Columns.LastName)
               .AddSelect(AppUser.TableSchema.SchemaName, AppUser.Columns.AppUserId, AppUser.Columns.AppUserId)
               .AddSelect(AppUser.TableSchema.SchemaName, AppUser.Columns.Phone, AppUser.Columns.Phone)
               .AddSelect(AppUserAPNSToken.TableSchema.SchemaName, AppUserAPNSToken.Columns.Token, AppUserAPNSToken.Columns.Token);
            var testUsers = new[] { 18283,18465,18578 };// moshe  13444
            var testTempUsers = new[] { 50199, 50195 };

            qApn.AddWhere(AppUser.TableSchema.SchemaName, AppUser.Columns.AppUserId, WhereComparision.In, testUsers);
            qApn.AddWhere(AppUserGcmToken.TableSchema.SchemaName, AppUserGcmToken.Columns.Token, WhereComparision.EqualsTo, null);

            var LstFcmUser = new List<NotificationUser>();
            using (DataReaderBase reader = qApn.ExecuteReader())
            {
                while (reader.Read())
                {
                    var user = new NotificationUser
                    {
                        AppUserId = int.Parse(reader[AppUser.Columns.AppUserId].ToString()),
                        FirstName = reader[AppUser.Columns.FirstName].ToString(),
                        LastName = reader[AppUser.Columns.LastName].ToString(),
                        Phone = reader[AppUser.Columns.Phone].ToString(),
                        ApnToken = reader[AppUserAPNSToken.Columns.Token] == null ? null : reader[AppUserAPNSToken.Columns.Token].ToString()
                    };
                    var countOfUser = LstFcmUser.Where(x => x.AppUserId == user.AppUserId);
                    if (countOfUser.Count() <= 1)
                        LstFcmUser.Add(user);
                }
            }

            Query qFcm = new Query(AppUser.TableSchema);
           qFcm.Join(JoinType.InnerJoin, AppUser.TableSchema, AppUser.Columns.AppUserId, AppUser.TableSchema.SchemaName,
                 AppUserGcmToken.TableSchema, AppUserGcmToken.Columns.AppUserId, AppUserGcmToken.TableSchema.SchemaName);

            qFcm.AddSelect(AppUser.TableSchema.SchemaName, AppUser.Columns.FirstName, AppUser.Columns.FirstName)
               .AddSelect(AppUser.TableSchema.SchemaName, AppUser.Columns.LastName, AppUser.Columns.LastName)
               .AddSelect(AppUser.TableSchema.SchemaName, AppUser.Columns.AppUserId, AppUser.Columns.AppUserId)
               .AddSelect(AppUser.TableSchema.SchemaName, AppUser.Columns.Phone, AppUser.Columns.Phone)
               .AddSelect(AppUserGcmToken.TableSchema.SchemaName, AppUserGcmToken.Columns.Token, AppUserGcmToken.Columns.Token);

            qFcm.AddWhere(AppUser.TableSchema.SchemaName, AppUser.Columns.AppUserId, WhereComparision.In, testUsers);

            var LstApnUser = new List<NotificationUser>();
            using (DataReaderBase reader = qFcm.ExecuteReader())
            {
                while (reader.Read())
                {
                    var user = new NotificationUser
                    {
                        AppUserId = int.Parse(reader[AppUser.Columns.AppUserId].ToString()),
                        FirstName = reader[AppUser.Columns.FirstName].ToString(),
                        LastName = reader[AppUser.Columns.LastName].ToString(),
                        Phone = reader[AppUser.Columns.Phone].ToString(),
                        FcmToken = reader[AppUserGcmToken.Columns.Token] == null ? null : reader[AppUserGcmToken.Columns.Token].ToString(),
                    };
                    var countOfUser = LstApnUser.Where(x => x.AppUserId == user.AppUserId);
                    if (countOfUser.Count() <= 1)
                        LstApnUser.Add(user);
                }
            }

            Query qTemp = new Query(TempAppUser.TableSchema);
            qTemp.AddSelect(TempAppUser.TableSchema.SchemaName, TempAppUser.Columns.TempAppUserId, TempAppUser.Columns.TempAppUserId)
               .AddSelect(TempAppUser.TableSchema.SchemaName, TempAppUser.Columns.DeviceUdid, "Token");
            qTemp.AddWhere(TempAppUser.TableSchema.SchemaName, TempAppUser.Columns.TempAppUserId, WhereComparision.In, testTempUsers);
            var LstTempUser = new List<NotificationUser>();
            using (DataReaderBase reader = qTemp.ExecuteReader())
            {
                while (reader.Read())
                {
                    var token = reader["Token"] == null ? null : reader["Token"].ToString();
                    if (token != null && token.Length == 64)
                    {
                        var user = new NotificationUser
                        {
                            AppUserId = int.Parse(reader[TempAppUser.Columns.TempAppUserId].ToString()),
                            //FcmToken = ,
                            ApnToken = token,
                            IsTempUser = true
                        };
                        LstTempUser.Add(user);
                    }
                }
            }
            var LstUser = new List<NotificationUser>();
            LstUser.AddRange(LstFcmUser);
            LstUser.AddRange(LstApnUser);
            LstUser.AddRange(LstTempUser);
            return LstUser;
        }

        private static List<NotificationUser> GetDaysSincePurchase(NotificationFilter filteringParameters, long userId = 0)
        {
            Query qApn = new Query(Order.TableSchema);
            qApn.Join(JoinType.InnerJoin, Order.TableSchema, Order.Columns.AppUserId, Order.TableSchema.SchemaName,
                   AppUser.TableSchema, AppUser.Columns.AppUserId, AppUser.TableSchema.SchemaName);
            qApn.Join(JoinType.LeftJoin, AppUser.TableSchema, AppUser.Columns.AppUserId, AppUser.TableSchema.SchemaName,
                  AppUserAPNSToken.TableSchema, AppUserAPNSToken.Columns.AppUserId, AppUserAPNSToken.TableSchema.SchemaName);
            qApn.Join(JoinType.LeftJoin, AppUser.TableSchema, AppUser.Columns.AppUserId, AppUser.TableSchema.SchemaName,
                 AppUserGcmToken.TableSchema, AppUserGcmToken.Columns.AppUserId, AppUserGcmToken.TableSchema.SchemaName);

            qApn.AddSelect(AppUser.TableSchema.SchemaName, AppUser.Columns.FirstName, AppUser.Columns.FirstName)
               .AddSelect(AppUser.TableSchema.SchemaName, AppUser.Columns.LastName, AppUser.Columns.LastName)
               .AddSelect(AppUser.TableSchema.SchemaName, AppUser.Columns.AppUserId, AppUser.Columns.AppUserId)
               .AddSelect(AppUser.TableSchema.SchemaName, AppUser.Columns.Phone, AppUser.Columns.Phone)
               .AddSelect(AppUserAPNSToken.TableSchema.SchemaName, AppUserAPNSToken.Columns.Token, AppUserAPNSToken.Columns.Token);
            if (filteringParameters.AreaId > 0)
            {
                qApn.Join(JoinType.InnerJoin, AppUser.TableSchema, AppUser.Columns.CityId, AppUser.TableSchema.SchemaName,
                    City.TableSchema, City.Columns.CityId, City.TableSchema.SchemaName);
                qApn.AddWhere(City.TableSchema.SchemaName, City.Columns.AreaId, WhereComparision.EqualsTo, filteringParameters.AreaId);
            }

            if (filteringParameters.AnimalTypeId > 0)
            {
                qApn.Join(JoinType.InnerJoin, Order.TableSchema, Order.Columns.BidId, Order.TableSchema.SchemaName,
                     BidProduct.TableSchema, BidProduct.Columns.BidId, BidProduct.TableSchema.SchemaName);
                qApn.Join(JoinType.InnerJoin, BidProduct.TableSchema, BidProduct.Columns.ProductId, BidProduct.TableSchema.SchemaName,
                     Product.TableSchema, Product.Columns.ProductId, Product.TableSchema.SchemaName);
                qApn.Join(JoinType.InnerJoin, Product.TableSchema, Product.Columns.ProductId, Product.TableSchema.SchemaName,
                     ProductAnimal.TableSchema, ProductAnimal.Columns.ProductId, ProductAnimal.TableSchema.SchemaName);
                qApn.AddWhere(ProductAnimal.TableSchema.SchemaName, ProductAnimal.Columns.AnimalId, WhereComparision.EqualsTo, filteringParameters.AnimalTypeId);
            }
            qApn.GroupBy(AppUser.Columns.FirstName).GroupBy(AppUser.Columns.LastName).GroupBy(AppUser.Columns.AppUserId).GroupBy(AppUser.Columns.Phone);
            qApn.AddWhere(AppUser.TableSchema.SchemaName, AppUser.Columns.IsDeleted, WhereComparision.EqualsTo, false);
            qApn.AddWhere(AppUserGcmToken.TableSchema.SchemaName, AppUserGcmToken.Columns.Token, WhereComparision.EqualsTo, null);
            qApn.AddWhere(Order.TableSchema.SchemaName, Order.Columns.CreateDate, WhereComparision.GreaterThan, DateTime.Today.AddDays(-filteringParameters.RunEvery));
            qApn.AddWhere(Order.TableSchema.SchemaName, Order.Columns.CreateDate, WhereComparision.LessThan, DateTime.Today.AddDays(-filteringParameters.RunEvery + 1));

            var LstFcmUser = new List<NotificationUser>();
            using (DataReaderBase reader = qApn.ExecuteReader())
            {
                while (reader.Read())
                {
                    var user = new NotificationUser
                    {
                        AppUserId = int.Parse(reader[AppUser.Columns.AppUserId].ToString()),
                        FirstName = reader[AppUser.Columns.FirstName].ToString(),
                        LastName = reader[AppUser.Columns.LastName].ToString(),
                        Phone = reader[AppUser.Columns.Phone].ToString(),
                        ApnToken = reader[AppUserAPNSToken.Columns.Token] == null ? null : reader[AppUserAPNSToken.Columns.Token].ToString()
                    };
                    var countOfUser = LstFcmUser.Where(x => x.AppUserId == user.AppUserId);
                    if (countOfUser.Count() <= 1)
                        LstFcmUser.Add(user);
                }
            }

            Query qFcm = new Query(Order.TableSchema);
            qFcm.Join(JoinType.InnerJoin, Order.TableSchema, Order.Columns.AppUserId, Order.TableSchema.SchemaName,
                   AppUser.TableSchema, AppUser.Columns.AppUserId, AppUser.TableSchema.SchemaName);
            qFcm.Join(JoinType.InnerJoin, AppUser.TableSchema, AppUser.Columns.AppUserId, AppUser.TableSchema.SchemaName,
                  AppUserGcmToken.TableSchema, AppUserGcmToken.Columns.AppUserId, AppUserGcmToken.TableSchema.SchemaName);

            qFcm.AddSelect(AppUser.TableSchema.SchemaName, AppUser.Columns.FirstName, AppUser.Columns.FirstName)
               .AddSelect(AppUser.TableSchema.SchemaName, AppUser.Columns.LastName, AppUser.Columns.LastName)
               .AddSelect(AppUser.TableSchema.SchemaName, AppUser.Columns.AppUserId, AppUser.Columns.AppUserId)
               .AddSelect(AppUser.TableSchema.SchemaName, AppUser.Columns.Phone, AppUser.Columns.Phone)
               .AddSelect(AppUserGcmToken.TableSchema.SchemaName, AppUserGcmToken.Columns.Token, AppUserGcmToken.Columns.Token);
            if (filteringParameters.AreaId > 0)
            {
                qFcm.Join(JoinType.InnerJoin, AppUser.TableSchema, AppUser.Columns.CityId, AppUser.TableSchema.SchemaName,
                    City.TableSchema, City.Columns.CityId, City.TableSchema.SchemaName);
                qFcm.AddWhere(City.TableSchema.SchemaName, City.Columns.AreaId, WhereComparision.EqualsTo, filteringParameters.AreaId);
            }

            if (filteringParameters.AnimalTypeId > 0)
            {
                qFcm.Join(JoinType.InnerJoin, Order.TableSchema, Order.Columns.BidId, Order.TableSchema.SchemaName,
                     BidProduct.TableSchema, BidProduct.Columns.BidId, BidProduct.TableSchema.SchemaName);
                qFcm.Join(JoinType.InnerJoin, BidProduct.TableSchema, BidProduct.Columns.ProductId, BidProduct.TableSchema.SchemaName,
                     Product.TableSchema, Product.Columns.ProductId, Product.TableSchema.SchemaName);
                qFcm.Join(JoinType.InnerJoin, Product.TableSchema, Product.Columns.ProductId, Product.TableSchema.SchemaName,
                     ProductAnimal.TableSchema, ProductAnimal.Columns.ProductId, ProductAnimal.TableSchema.SchemaName);
                qFcm.AddWhere(ProductAnimal.TableSchema.SchemaName, ProductAnimal.Columns.AnimalId, WhereComparision.EqualsTo, filteringParameters.AnimalTypeId);
            }
            qFcm.GroupBy(AppUser.Columns.FirstName).GroupBy(AppUser.Columns.LastName).GroupBy(AppUser.Columns.AppUserId).GroupBy(AppUser.Columns.Phone);
            qFcm.AddWhere(AppUser.TableSchema.SchemaName, AppUser.Columns.IsDeleted, WhereComparision.EqualsTo, false);
            qFcm.AddWhere(Order.TableSchema.SchemaName, Order.Columns.CreateDate, WhereComparision.GreaterThan, DateTime.Today.AddDays(-filteringParameters.RunEvery));
            qFcm.AddWhere(Order.TableSchema.SchemaName, Order.Columns.CreateDate, WhereComparision.LessThan, DateTime.Today.AddDays(-filteringParameters.RunEvery + 1));
            var LstApnUser = new List<NotificationUser>();

            using (DataReaderBase reader = qFcm.ExecuteReader())
            {
                while (reader.Read())
                {
                    var user = new NotificationUser
                    {
                        AppUserId = int.Parse(reader[AppUser.Columns.AppUserId].ToString()),
                        FirstName = reader[AppUser.Columns.FirstName].ToString(),
                        LastName = reader[AppUser.Columns.LastName].ToString(),
                        Phone = reader[AppUser.Columns.Phone].ToString(),
                        FcmToken = reader[AppUserGcmToken.Columns.Token] == null ? null : reader[AppUserGcmToken.Columns.Token].ToString()
                    };
                    var countOfUser = LstApnUser.Where(x => x.AppUserId == user.AppUserId);
                    if (countOfUser.Count() <= 1)
                        LstApnUser.Add(user);
                }
            }
            var LstUser = new List<NotificationUser>();
            LstUser.AddRange(LstFcmUser);
            LstUser.AddRange(LstApnUser);
            return LstUser;
        }

        private static List<NotificationUser> GetAverageOfLastThree(NotificationFilter filteringParameters)
        {
            var ids = GetUserIdsOfAverage();

            Query qFcm = new Query(AppUser.TableSchema);
            qFcm.Join(JoinType.LeftJoin, AppUser.TableSchema, AppUser.Columns.AppUserId, AppUser.TableSchema.SchemaName,
                 AppUserGcmToken.TableSchema, AppUserGcmToken.Columns.AppUserId, AppUserGcmToken.TableSchema.SchemaName);
            qFcm.Join(JoinType.LeftJoin, AppUser.TableSchema, AppUser.Columns.AppUserId, AppUser.TableSchema.SchemaName,
                   AppUserAPNSToken.TableSchema, AppUserAPNSToken.Columns.AppUserId, AppUserAPNSToken.TableSchema.SchemaName);
            qFcm.AddSelect(AppUser.TableSchema.SchemaName, AppUser.Columns.FirstName, AppUser.Columns.FirstName)
               .AddSelect(AppUser.TableSchema.SchemaName, AppUser.Columns.LastName, AppUser.Columns.LastName)
               .AddSelect(AppUser.TableSchema.SchemaName, AppUser.Columns.AppUserId, AppUser.Columns.AppUserId)
               .AddSelect(AppUser.TableSchema.SchemaName, AppUser.Columns.Phone, AppUser.Columns.Phone)
               .AddSelect(AppUserGcmToken.TableSchema.SchemaName, AppUserGcmToken.Columns.Token, AppUserGcmToken.Columns.Token);
            if (filteringParameters.AreaId > 0)
            {
                qFcm.Join(JoinType.InnerJoin, AppUser.TableSchema, AppUser.Columns.CityId, AppUser.TableSchema.SchemaName,
                    City.TableSchema, City.Columns.CityId, City.TableSchema.SchemaName);
                qFcm.AddWhere(City.TableSchema.SchemaName, City.Columns.AreaId, WhereComparision.EqualsTo, filteringParameters.AreaId);
            }
            qFcm.AddWhere(AppUser.TableSchema.SchemaName, AppUser.Columns.AppUserId, WhereComparision.In, ids);
            qFcm.AddWhere(AppUser.TableSchema.SchemaName, AppUser.Columns.IsDeleted, WhereComparision.EqualsTo, false);
            qFcm.AddWhere(AppUserAPNSToken.TableSchema.SchemaName, AppUserAPNSToken.Columns.Token, WhereComparision.EqualsTo, null);

            var LstFcmUser = new List<NotificationUser>();
            using (DataReaderBase reader = qFcm.ExecuteReader())
            {
                while (reader.Read())
                {
                    var user = new NotificationUser
                    {
                        AppUserId = int.Parse(reader[AppUser.Columns.AppUserId].ToString()),
                        FirstName = reader[AppUser.Columns.FirstName].ToString(),
                        LastName = reader[AppUser.Columns.LastName].ToString(),
                        Phone = reader[AppUser.Columns.Phone].ToString(),
                        FcmToken = reader[AppUserGcmToken.Columns.Token] == null ? null : reader[AppUserGcmToken.Columns.Token].ToString(),
                        IsTempUser = false
                    };
                    var countOfUser = LstFcmUser.Where(x => x.AppUserId == user.AppUserId);
                    if (countOfUser.Count() <= 1)
                        LstFcmUser.Add(user);
                }
            }

            Query qApn = new Query(AppUser.TableSchema);
            qApn.Join(JoinType.InnerJoin, AppUser.TableSchema, AppUser.Columns.AppUserId, AppUser.TableSchema.SchemaName,
                   AppUserAPNSToken.TableSchema, AppUserAPNSToken.Columns.AppUserId, AppUserAPNSToken.TableSchema.SchemaName);

            qApn.AddSelect(AppUser.TableSchema.SchemaName, AppUser.Columns.FirstName, AppUser.Columns.FirstName)
               .AddSelect(AppUser.TableSchema.SchemaName, AppUser.Columns.LastName, AppUser.Columns.LastName)
               .AddSelect(AppUser.TableSchema.SchemaName, AppUser.Columns.AppUserId, AppUser.Columns.AppUserId)
               .AddSelect(AppUser.TableSchema.SchemaName, AppUser.Columns.Phone, AppUser.Columns.Phone)
               .AddSelect(AppUserAPNSToken.TableSchema.SchemaName, AppUserAPNSToken.Columns.Token, AppUserAPNSToken.Columns.Token);
            if (filteringParameters.AreaId > 0)
            {
                qApn.Join(JoinType.InnerJoin, AppUser.TableSchema, AppUser.Columns.CityId, AppUser.TableSchema.SchemaName,
                    City.TableSchema, City.Columns.CityId, City.TableSchema.SchemaName);
                qApn.AddWhere(City.TableSchema.SchemaName, City.Columns.AreaId, WhereComparision.EqualsTo, filteringParameters.AreaId);
            }
            qApn.AddWhere(AppUser.TableSchema.SchemaName, AppUser.Columns.IsDeleted, WhereComparision.EqualsTo, false);
            qApn.AddWhere(AppUser.TableSchema.SchemaName, AppUser.Columns.AppUserId, WhereComparision.In, ids);

            var LstApnUser = new List<NotificationUser>();
            using (DataReaderBase reader = qApn.ExecuteReader())
            {
                while (reader.Read())
                {
                    var user = new NotificationUser
                    {
                        AppUserId = int.Parse(reader[AppUser.Columns.AppUserId].ToString()),
                        FirstName = reader[AppUser.Columns.FirstName].ToString(),
                        LastName = reader[AppUser.Columns.LastName].ToString(),
                        Phone = reader[AppUser.Columns.Phone].ToString(),
                        ApnToken = reader[AppUserAPNSToken.Columns.Token] == null ? null : reader[AppUserAPNSToken.Columns.Token].ToString(),
                        IsTempUser = false
                    };
                    var countOfUser = LstApnUser.Where(x => x.AppUserId == user.AppUserId);
                    if (countOfUser.Count() <= 1)
                        LstApnUser.Add(user);
                }
            }

            var LstUser = new List<NotificationUser>();
            LstUser.AddRange(LstFcmUser);
            LstUser.AddRange(LstApnUser);
            return LstUser;
        }

        private static List<int> GetUserIdsOfAverage()
        {
            Query query = new Query(Order.TableSchema);
            query.AddSelectLiteral(@" * from (SELECT AppUserId, CreateDate 
                                    FROM(select orders.AppUserId, orders.OrderId, orders.CreateDate, count(*) as row_number
                                            from(SELECT AppUserId, OrderId, CreateDate from tbl_order order by CreateDate desc) as orders
                                    JOIN tbl_order b ON orders.AppUserId = b.AppUserId AND orders.OrderId <= b.OrderId
                                    GROUP BY orders.AppUserId, orders.OrderId) as tbl_partition
                                    WHERE tbl_partition.row_number < 4) as a #");

            var LstUser = new Dictionary<int, List<DateTime>>();
            using (DataReaderBase reader = query.ExecuteReader())
            {
                while (reader.Read())
                {
                    int userId = int.Parse(reader["AppUserId"].ToString());
                    DateTime date = DateTime.Parse(reader["CreateDate"].ToString());
                    if (!LstUser.ContainsKey(userId))
                        LstUser[userId] = new List<DateTime>();
                    LstUser[userId].Add(date);
                }
            }
            var listToReturn = new List<int>();
            var listOfRelevantUsers = LstUser.Where(x => x.Value.Count > 1);
            foreach(var item in listOfRelevantUsers)
            {
                int days = 0;
                var sortedList = item.Value.OrderByDescending(x => x);
                DateTime lastPuchase = sortedList.First();
                DateTime lastDate = sortedList.First();
                foreach (var date in sortedList)
                    if (lastDate.Date != date.Date)
                    {
                        days += Math.Abs((lastDate.Date - date.Date).Days);
                        lastDate = date;
                    }
                var avg = days / (item.Value.Count -1);
                if (lastPuchase.AddDays(avg).Date == DateTime.Today)
                    listToReturn.Add(item.Key);
            }
            return listToReturn;
        }
     
        #endregion
    }

    public class NotificationUser : AppUser
    {
        public string ApnToken { get; set; }
        public string FcmToken { get; set; }
        public bool IsTempUser { get; set; }
    }
}
