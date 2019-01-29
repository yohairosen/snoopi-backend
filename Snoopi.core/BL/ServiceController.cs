using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Snoopi.core.DAL;
using dg.Sql;
using dg.Sql.Connector;
using System.IO;
using Snoopi.core.DAL.Entities;

namespace Snoopi.core.BL
{
    public class SupplierServiceUI : AppSupplier
    {
        public double AvgRate { get; set; }
        public string CityName { get; set; }
        public CommentCollection LstComment { get; set; }
        public List<City> citiesSupplied { get; set; }
        public List<City> citiesHomeService { get; set; }
        public List<Service> LstServices { get; set; }
        public Int64 ClickNum { get; set; }
        public Int64 ClickToCallNum { get; set; }
        public int NumberOfComments { get; set; }
        public Decimal Distance { get; set; }


    }
    public class ServiceController
    {

        public static ServiceCollection GetAllService()
        {
            return ServiceCollection.FetchAll();
        }

        public static BidService GetActiveBidServiceByAppOrTempUserId(Int64 AppUserId, Int64 TempAppUserId)
        {
            Query qry = new Query(BidService.TableSchema);
           
            if (AppUserId != 0)
            {
                qry.Where(BidService.Columns.AppUserId, AppUserId);
            }
            if (TempAppUserId != 0)
            {
                qry.Where(BidService.Columns.TempAppUserId, TempAppUserId);
            }
            qry.AddWhere(BidService.Columns.EndDate, WhereComparision.GreaterThanOrEqual, DateTime.UtcNow);
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                if (reader.Read())
                {
                    BidService item = new BidService();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }

        public static void SendPushToSuppliers(Int64 BidId, Int64 ServiceId, Int64 CityId, Geometry.Point Location)
        {
           List< Int64> HomeServiceId = Service.FetchAllHomeServices();

            Query qry = new Query(AppSupplier.TableSchema);
            qry.AddWhere(AppSupplier.Columns.Status, false);
            qry.Join(JoinType.InnerJoin, AppSupplier.TableSchema, AppSupplier.Columns.SupplierId, AppSupplier.TableSchema.SchemaName,
                SupplierService.TableSchema, SupplierService.Columns.SupplierId, SupplierService.TableSchema.SchemaName);
            qry.Where(SupplierService.TableSchema.SchemaName, SupplierService.Columns.ServiceId, WhereComparision.EqualsTo, ServiceId);
            qry.Select(AppSupplier.TableSchema.SchemaName, AppSupplier.Columns.SupplierId, AppSupplier.Columns.SupplierId, true);
            qry.Distinct();

            if (HomeServiceId.Contains(ServiceId))
            {
                Query CityInnerQuery = new Query(SupplierHomeServiceCity.TableSchema);
                CityInnerQuery.Where(SupplierHomeServiceCity.Columns.CityId, WhereComparision.EqualsTo, CityId);
                CityInnerQuery.Select(SupplierHomeServiceCity.Columns.SupplierId).Distinct();

                qry.AddWhere(AppSupplier.TableSchema.SchemaName, AppSupplier.Columns.SupplierId, WhereComparision.In, CityInnerQuery);
            }
            //get all appusers that are in the radius of Location
            else 
            {
                if (Location != null)
                {
                    Query q = new Query(AppSupplier.TableSchema);
                    q.SelectAllTableColumns();
                    q.AddSelectLiteral(
                         "( 6371 * acos ( cos ( radians(" + Location.X + ") ) * cos( radians( X(" + AppSupplier.Columns.AddressLocation + ") ) ) " +
                        "* cos( radians( Y(" + AppSupplier.Columns.AddressLocation + ") ) - radians(" + Location.Y + ") ) " +
                    "+ sin ( radians(" + Location.X + ") ) * sin( radians( X(" + AppSupplier.Columns.AddressLocation + ") ) ) )) AS distance");
                    List<Int64> appSupplierIds = new List<Int64>();
                    using (DataReaderBase reader = q.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            decimal distance = reader["distance"] != null ? Convert.ToDecimal(reader["distance"]) : 0;
                            if (distance > Settings.GetSettingInt32(Settings.Keys.SUPPLIER_RADIUS, 10)) continue;
                            appSupplierIds.Add(Convert.ToInt64(reader[AppSupplier.Columns.SupplierId]));

                        }
                    }
                    qry.AddWhere(AppSupplier.TableSchema.SchemaName, AppSupplier.Columns.SupplierId, WhereComparision.In, appSupplierIds);
                }
            }

            try
            {
                SupplierNotification.SendNotificationNewServiceBidToSupplier(BidId, qry.ExecuteScalarList<Int64>());
            }
            catch (Exception) { }


        }

        public static List<SupplierServiceUI> GetServiceSuppliersByDistance(Int64 ServiceId, Int64 CityId, Geometry.Point Location, List<SupplierServiceUI> excludedSuppliers)
        {
            List<Int64> HomeServiceId = Service.FetchAllHomeServices();

            List<SupplierServiceUI> suppliersService = new List<SupplierServiceUI>();
            var excludedSupplierIds = excludedSuppliers.Select(x => x.SupplierId);
            const string DISTANCE = "distance";

            Query qry = new Query(AppSupplier.TableSchema);
            qry.Select(AppSupplier.TableSchema.SchemaName, AppSupplier.Columns.SupplierId, AppSupplier.Columns.SupplierId, true);
            qry.AddSelect(AppSupplier.TableSchema.SchemaName, AppSupplier.Columns.BusinessName, AppSupplier.Columns.BusinessName);
            qry.AddSelect(AppSupplier.TableSchema.SchemaName, AppSupplier.Columns.Street, AppSupplier.Columns.Street);
            qry.AddSelect(AppSupplier.TableSchema.SchemaName, AppSupplier.Columns.Description, AppSupplier.Columns.Description);
            qry.AddSelect(AppSupplier.TableSchema.SchemaName, AppSupplier.Columns.ProfileImage, AppSupplier.Columns.ProfileImage);
            qry.AddSelect(AppSupplier.TableSchema.SchemaName, AppSupplier.Columns.HouseNum, AppSupplier.Columns.HouseNum);
            qry.AddSelect(AppSupplier.TableSchema.SchemaName, AppSupplier.Columns.Phone, AppSupplier.Columns.Phone);
            qry.AddSelect(City.TableSchema.SchemaName, City.Columns.CityName, City.Columns.CityName);
            qry.AddSelect(SupplierService.TableSchema.SchemaName, SupplierService.Columns.ServiceId, SupplierService.Columns.ServiceId);
            qry.AddSelectLiteral("(SELECT avg(" + Comment.Columns.Rate + ") from " + Comment.TableSchema.SchemaName + " where "
                         + Comment.TableSchema.SchemaName + "." + Comment.Columns.SupplierId + "=" + AppSupplier.TableSchema.SchemaName + "." + AppSupplier.Columns.SupplierId +
                           " AND " + Comment.TableSchema.SchemaName + "." + Comment.Columns.Status + "=" + (int)CommentStatus.Approved + ")", "AvgRate");
            qry.AddSelectLiteral("(SELECT Count(" + Comment.Columns.Rate + ") from " + Comment.TableSchema.SchemaName + " where " +
                Comment.TableSchema.SchemaName + "." + Comment.Columns.SupplierId + "=" + AppSupplier.TableSchema.SchemaName + "." + AppSupplier.Columns.SupplierId +
                 " AND " + Comment.TableSchema.SchemaName + "." + Comment.Columns.Status + "=" + (int)CommentStatus.Approved + ")", "numberOfComments");

            qry.AddSelectLiteral(
                         "( 6371 * acos ( cos ( radians(" + Location.X + ") ) * cos( radians( X(" + AppSupplier.TableSchema.SchemaName + "." + AppSupplier.Columns.AddressLocation + ") ) ) " +
                        "* cos( radians( Y(" + AppSupplier.TableSchema.SchemaName + "." + AppSupplier.Columns.AddressLocation + ") ) - radians(" + Location.Y + ") ) " +
                    "+ sin ( radians(" + Location.X + ") ) * sin( radians( X(" + AppSupplier.TableSchema.SchemaName + "." + AppSupplier.Columns.AddressLocation + ") ) ) )) AS " + DISTANCE);

            qry.Join(JoinType.LeftJoin, AppSupplier.TableSchema, AppSupplier.Columns.CityId, AppSupplier.TableSchema.SchemaName, City.TableSchema, City.Columns.CityId, City.TableSchema.SchemaName);

            qry.Join(JoinType.LeftJoin, AppSupplier.TableSchema, AppSupplier.Columns.SupplierId, AppSupplier.TableSchema.SchemaName, Comment.TableSchema, Comment.Columns.SupplierId, Comment.TableSchema.SchemaName);
            qry.Join(JoinType.LeftJoin, AppSupplier.TableSchema, AppSupplier.Columns.SupplierId, AppSupplier.TableSchema.SchemaName, SupplierService.TableSchema, SupplierService.Columns.SupplierId, SupplierService.TableSchema.SchemaName);
            
            qry.Having("(" + DISTANCE + " <= " + Settings.GetSettingInt32(Settings.Keys.SUPPLIER_RADIUS, 10) +
                (HomeServiceId.Contains(ServiceId) ?
                " OR " + AppSupplier.TableSchema.SchemaName + "." + AppSupplier.Columns.SupplierId + " IN ( SELECT " + SupplierHomeServiceCity.Columns.SupplierId +
                  " FROM  " + SupplierHomeServiceCity.TableSchema.SchemaName + " WHERE " + SupplierHomeServiceCity.Columns.CityId + " = " + CityId + ")"
                  : "") + ") AND " + SupplierService.TableSchema.SchemaName + "." + SupplierService.Columns.ServiceId + " = " + ServiceId);
            if (excludedSupplierIds.Count() > 0)
                qry.AddWhere(AppSupplier.TableSchema.SchemaName, AppSupplier.Columns.SupplierId, WhereComparision.NotIn, excludedSupplierIds.ToArray());
            qry.AddWhere(AppSupplier.TableSchema.SchemaName, AppSupplier.Columns.IsDeleted, WhereComparision.EqualsTo, false);
            qry.AddWhere(AppSupplier.TableSchema.SchemaName, AppSupplier.Columns.IsLocked, WhereComparision.EqualsTo, false);
            qry.AddWhere(AppSupplier.TableSchema.SchemaName, AppSupplier.Columns.IsService, WhereComparision.EqualsTo, true);
            qry.Randomize();
            qry.Distinct();
            
            try
            {
                using (DataReaderBase reader = qry.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        SupplierServiceUI supplierService = new SupplierServiceUI
                        {
                            SupplierId = Convert.ToInt64(reader["SupplierId"]),
                            BusinessName = Convert.ToString(reader["BusinessName"]),
                            Phone = Convert.ToString(reader["Phone"]),
                            HouseNum = Convert.ToString(reader["HouseNum"]),
                            CityName = Convert.ToString(reader["CityName"]),
                            Street = Convert.ToString(reader["Street"]),
                            AvgRate = string.IsNullOrEmpty(reader["AvgRate"].ToString()) ? 0 : Convert.ToDouble(reader["AvgRate"]),
                            Distance = Convert.ToDecimal(reader[DISTANCE]),
                            Description = Convert.ToString(reader[AppSupplier.Columns.Description]),
                            NumberOfComments = Convert.ToInt32(reader["numberOfComments"]),
                            ProfileImage = Convert.ToString(reader[AppSupplier.Columns.ProfileImage])
                        };
                        suppliersService.Add(supplierService);
                    }
                }

            }
            catch (Exception ex) { }

            return suppliersService.OrderBy(x => x.Distance).ToList();
        }


      
        public static Int64 CreateBidService(Int64 AppUserId, Int64 TempAppUserId, Int64 ServiceId, string CustomerComment)
        {
            BidService NewBid = new BidService();
            BidService HelpBid = GetActiveBidServiceByAppOrTempUserId(AppUserId, TempAppUserId);
            Int64 CityId = 0;
            Geometry.Point Location = null;
            if (HelpBid != null) return 0;
            if (AppUserId != 0)
            {
                AppUser a = AppUser.FetchByID(AppUserId);
                NewBid.AppUserId = AppUserId;;
                CityId = a.CityId;
                Location = a.AddressLocation;
            }
            else if (TempAppUserId != 0)
            {
                TempAppUser t = TempAppUser.FetchByID(TempAppUserId);
                NewBid.TempAppUserId = TempAppUserId;
                CityId = t.CityId;
                Location = t.Location;
            }
            NewBid.ServiceId = ServiceId;
            NewBid.StartDate = DateTime.UtcNow;
            NewBid.ServiceComment = CustomerComment;
            NewBid.EndDate = DateTime.UtcNow.AddHours(Convert.ToDouble(Settings.GetSettingDecimal(Settings.Keys.END_BID_TIME_MIN, 15)));
            NewBid.Save();

            //send push to suppliers
            SendPushToSuppliers(NewBid.BidId, ServiceId, CityId, Location);
            return NewBid.BidId;
        }

        public static void AddClickEvent(Int64 userId, Int64 supplierId,string eventType)
        {
            string eType = "";
            switch (eventType)
            {
                case SupplierEvent.PHONE_CALL:
                    eType = SupplierEvent.PHONE_CALL;
                    break;
                case SupplierEvent.CLICK:
                    eType = SupplierEvent.CLICK;
                    break;
                default:
                    break;
            }
            if (!string.IsNullOrEmpty(eType))
            {
                SupplierEvent se = new SupplierEvent()
                {
                    EventTime = DateTime.Now,
                    EventType = eType,
                    IsNewRecord = true,
                    SupplierId = supplierId,
                    UserId = userId

                };
                se.Insert();

            }
            
       
        }

        public static SupplierServiceUI GetServiceSuppliersAndNumEvents(Int64 SupplierId,DateTime from = new DateTime(), DateTime to = new DateTime())
        {
            return GetServiceSuppliersAndNumEvents("", "", "", "", from, to, true, 0, 0, SupplierId).FirstOrDefault();
        }


        public static List<SupplierServiceUI> GetServiceSuppliersAndNumEvents( string SearchName = "", string SearchPhone = "", string SearchId = "", string SearchCity = "", DateTime from = new DateTime(), DateTime to = new DateTime(), bool IsSearch = false, int PageSize = 0, int CurrentPageIndex = 0,Int64 SupplierId=0)
        {
            Query qry = new Query(AppSupplier.TableSchema);
            qry.SelectAllTableColumns();
            qry.AddSelectLiteral("COUNT(IF( " + SupplierEvent.TableSchema.SchemaName + "." + SupplierEvent.Columns.EventType + "= '" + SupplierEvent.CLICK + "',1, NULL)) 'ClickNum'");
            qry.AddSelectLiteral("COUNT(IF( " + SupplierEvent.TableSchema.SchemaName + "." + SupplierEvent.Columns.EventType + "= '" + SupplierEvent.PHONE_CALL + "',1, NULL)) 'ClickToCallNum'");
            qry.AddSelect(City.TableSchema.SchemaName, City.Columns.CityName, "CityName");
            qry.Join(JoinType.LeftJoin, SupplierEvent.TableSchema, AppSupplier.Columns.SupplierId, AppSupplier.TableSchema.SchemaName, SupplierEvent.TableSchema, SupplierEvent.Columns.SupplierId, SupplierEvent.TableSchema.SchemaName);

            qry.Join(JoinType.InnerJoin, AppSupplier.TableSchema, AppSupplier.Columns.CityId, AppSupplier.TableSchema.SchemaName, City.TableSchema, City.Columns.CityId, City.TableSchema.SchemaName);
            
            qry.Where(AppSupplier.Columns.IsService, true);
            qry.GroupBy(AppSupplier.Columns.SupplierId);
            if (IsSearch == true)
            {
                WhereList wl = new WhereList();
                if (SearchPhone != "")
                {
                    wl.OR(AppSupplier.Columns.ContactPhone, WhereComparision.Like, "%" + SearchPhone + "%")
                       .OR(AppSupplier.Columns.Phone, WhereComparision.Like, "%" + SearchPhone + "%");
                    qry.AND(wl);
                }

                WhereList wl1 = new WhereList();
                if (SearchName != "")
                {
                    wl1.OR(AppSupplier.Columns.ContactName, WhereComparision.Like, "%" + SearchName + "%")
                       .OR(AppSupplier.Columns.BusinessName, WhereComparision.Like, "%" + SearchName + "%");
                    qry.AND(wl1);
                }
                if (SearchId != "")
                {
                    qry.AddWhere(AppSupplier.TableSchema.SchemaName, AppSupplier.Columns.SupplierId, WhereComparision.Like, "%" + SearchId + "%");
                }
                if (SearchCity != "")
                {
                    qry.AddWhere(City.TableSchema.SchemaName, City.Columns.CityName, WhereComparision.Like, "%" + SearchCity);
                }
                if (SupplierId != 0)
                {
                    qry.AddWhere(AppSupplier.TableSchema.SchemaName, AppSupplier.Columns.SupplierId, WhereComparision.EqualsTo,SupplierId);
                }

                if (from != DateTime.MinValue)
                {
                    qry.AddWhere(SupplierEvent.Columns.EventTime, WhereComparision.GreaterThanOrEqual, from.Date);
                }
                if (to != DateTime.MinValue)
                {
                    qry.AddWhere(SupplierEvent.Columns.EventTime, WhereComparision.LessThanOrEqual, to.AddHours(24).Date);
                }
            }


            if (PageSize > 0)
            {
                qry.LimitRows(PageSize).OffsetRows(PageSize * CurrentPageIndex);
            }
  //          qry.OrderBy(AppSupplier.TableSchema.SchemaName, AppSupplier.Columns.CreateDate, SortDirection.DESC);
            List<SupplierServiceUI> list = new List<SupplierServiceUI>();
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                while (reader.Read())
                {
                    //Query q = new Query(Order.TableSchema).Join(JoinType.LeftJoin, Bid.TableSchema, Bid.TableSchema.SchemaName,
                    //new JoinColumnPair(Order.TableSchema.SchemaName, Order.Columns.BidId, Bid.Columns.BidId));
                    //q.SelectAllTableColumns();
                    //q.Where(Order.Columns.SupplierId, Convert.ToInt64(reader["SupplierId"]))
                    //.AddWhere(Bid.Columns.EndDate, WhereComparision.GreaterThanOrEqual, DateTime.UtcNow);
                    //if (from != DateTime.MinValue) q.AddWhere(Bid.Columns.EndDate, WhereComparision.GreaterThanOrEqual, from.Date);
                    //if (to != DateTime.MinValue) q.AddWhere(Bid.Columns.EndDate, WhereComparision.LessThanOrEqual, to.AddHours(24).Date);
                    //Int64 ActiveOrder = q.GetCount(Order.Columns.OrderId);

                    list.Add(new SupplierServiceUI
                    {
                        SupplierId = Convert.ToInt64(reader["SupplierId"]),
                        BusinessName = Convert.ToString(reader["BusinessName"]),
                        ContactPhone = Convert.ToString(reader["ContactPhone"]),
                        Phone = Convert.ToString(reader["Phone"]),
                        ContactName = Convert.ToString(reader["ContactName"]),
                        CityId = Convert.ToInt64(reader["CityId"]),
                        CityName = Convert.ToString(reader["CityName"]),
                        ClickNum=Convert.ToInt64(reader["ClickNum"]),
                        ClickToCallNum=Convert.ToInt64(reader["ClickToCallNum"])
                    });
                }
            }
            return list;
        }

    }

}

