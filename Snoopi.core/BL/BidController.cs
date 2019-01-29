using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Snoopi.core.DAL;
using dg.Sql;
using dg.Sql.Connector;
using System.IO;
using System.Net.Mail;
using System.Diagnostics;

namespace Snoopi.core.BL
{
    [Serializable]
    public class BidProductUI
    {
        public Int64 ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
        public int Amount { get; set; }
        public decimal Price { get; set; }
        public string ProductAmount { get; set; }
        public string ProductGift { get; set; }
        public string ProductCode { get; set; }
        public int RecomendedPrice { get; set; }
    }

    public enum CustomerType
    {
        Temp = 1,
        AppUser = 2
    }
    [Serializable]


    public enum OrderDeliveryStatus
    {
        None = 0,
        ApprovedBySupplier = 1,
        SuppliedToTheCustomer = 2,
        CustomerApprovedSupplied = 3
    };
    [Serializable]

    public class BidUI
    {

        public Int64 BidId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public Int64 CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string Phone { get; set; }
        public string City { get; set; }
        public int OfferNum { get; set; }
        public Int64 SupplierId { get; set; }
        public string SupplierName { get; set; }
        public decimal Price { get; set; }
        public CustomerType CustomerType { get; set; }
        public List<BidProductUI> LstProduct { get; set; }
        public string Products { get; set; }
        public bool IsActive { get; set; }
        public string Gift { get; set; }
        public DateTime? OrderDate { get; set; }
        public DateTime? SuppliedDate { get; set; }
        public DateTime? ReceivedDate { get; set; }
        public OrderDeliveryStatus OrderStatus { get; set; }
        public Source Source { get; set; }
    }

    public class CustomerUI
    {
        public string CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerLink { get; set; }
        public string Phone { get; set; }
        public string City { get; set; }
        public CustomerType CustomerType { get; set; }
        public Int64 BidCount { get; set; }
        public Int64 BidAbandoned { get; set; }
        public Int64 BidPurchase { get; set; }
        public Int64 BidActive { get; set; }
    }

    public enum BidType
    {
        BidCount = 0,
        BidAbandoned = 1,
        BidPurchase = 2,
        BidActive = 3
    }

    public class BidController
    {

        public static Int64 GetAllAbandonedBids()
        {
            Query q = new Query(Bid.TableSchema);
            q.Join(JoinType.LeftJoin, Order.TableSchema, "o",
                new JoinColumnPair(Bid.TableSchema.SchemaName, Bid.Columns.BidId, Order.Columns.BidId));
            q.AddWhere("o", Order.Columns.OrderId, WhereComparision.EqualsTo, null);
            return q.Distinct().GetCount(Bid.TableSchema.SchemaName, Bid.Columns.BidId);
        }

        public static Int64 GetAllActiveBids()
        {
            Query q = new Query(Bid.TableSchema);
            q.AddWhere(Bid.TableSchema.SchemaName, Bid.Columns.EndDate, WhereComparision.GreaterThanOrEqual, DateTime.UtcNow);
            return q.Distinct().GetCount(Bid.TableSchema.SchemaName, Bid.Columns.BidId);

        }

        public static Int64 GetAllPurchaseBids()
        {
            Query q = new Query(Bid.TableSchema);
            q.Join(JoinType.InnerJoin, Order.TableSchema, "o",
                new JoinColumnPair(Bid.TableSchema.SchemaName, Bid.Columns.BidId, Order.Columns.BidId));
            return q.Distinct().GetCount(Bid.TableSchema.SchemaName, Bid.Columns.BidId);
        }
        public static CustomerUI GetCustomerData(string CustomerId, CustomerType customerType)
        {
            CustomerUI c = new CustomerUI();
            Int64 _CustomerId = Convert.ToInt64(CustomerId);
            c.CustomerId = CustomerId;
            c.CustomerType = customerType;
            c.CustomerLink = "MyBids.aspx?Type=" + ((int)customerType).ToString() + "&Id=" + CustomerId;
            if (customerType == CustomerType.AppUser)
            {
                AppUser a = AppUser.FetchByID(_CustomerId);
                c.CustomerName = a.FirstName + " " + a.LastName;
                City city = City.FetchByID(a.CityId);
                c.City = city != null ? city.CityName : "";
                c.Phone = a.Phone;
                Query q = new Query(Bid.TableSchema);
                q.Where(Bid.TableSchema.SchemaName, Bid.Columns.AppUserId, WhereComparision.EqualsTo, CustomerId);
                c.BidCount = q.Distinct().GetCount(Bid.TableSchema.SchemaName, Bid.Columns.BidId);

                q = new Query(Bid.TableSchema);
                q.Join(JoinType.InnerJoin, Order.TableSchema, "o",
                    new JoinColumnPair(Bid.TableSchema.SchemaName, Bid.Columns.BidId, Order.Columns.BidId));
                q.Where(Bid.TableSchema.SchemaName, Bid.Columns.AppUserId, WhereComparision.EqualsTo, CustomerId);
                c.BidPurchase = q.Distinct().GetCount(Bid.TableSchema.SchemaName, Bid.Columns.BidId);

                q = new Query(Bid.TableSchema);
                q.Where(Bid.TableSchema.SchemaName, Bid.Columns.AppUserId, WhereComparision.EqualsTo, CustomerId);
                q.AddWhere(Bid.TableSchema.SchemaName, Bid.Columns.EndDate, WhereComparision.GreaterThanOrEqual, DateTime.UtcNow);
                c.BidActive = q.Distinct().GetCount(Bid.TableSchema.SchemaName, Bid.Columns.BidId);


                q = new Query(Bid.TableSchema);
                q.Join(JoinType.LeftJoin, Order.TableSchema, "o",
                    new JoinColumnPair(Bid.TableSchema.SchemaName, Bid.Columns.BidId, Order.Columns.BidId));
                q.Where(Bid.TableSchema.SchemaName, Bid.Columns.AppUserId, WhereComparision.EqualsTo, CustomerId);
                q.AddWhere("o", Order.Columns.OrderId, WhereComparision.EqualsTo, null);
                q.AddWhere(Bid.TableSchema.SchemaName, Bid.Columns.EndDate, WhereComparision.LessThan, DateTime.UtcNow);
                c.BidAbandoned = q.Distinct().GetCount(Bid.TableSchema.SchemaName, Bid.Columns.BidId);


            }
            else if (customerType == CustomerType.Temp)
            {
                c.CustomerId = "000" + CustomerId;

                TempAppUser temp = TempAppUser.FetchByID(_CustomerId);
                c.CustomerName = "Temp";
                City city = City.FetchByID(temp.CityId);
                c.City = city != null ? city.CityName : "";
                c.Phone = "";
                Query q = new Query(Bid.TableSchema);
                q.Where(Bid.TableSchema.SchemaName, Bid.Columns.TempAppUserId, WhereComparision.EqualsTo, CustomerId);
                c.BidCount = q.Distinct().GetCount(Bid.TableSchema.SchemaName, Bid.Columns.BidId);

                c.BidPurchase = 0;

                q = new Query(Bid.TableSchema);
                q.Where(Bid.TableSchema.SchemaName, Bid.Columns.TempAppUserId, WhereComparision.EqualsTo, CustomerId);
                q.AddWhere(Bid.TableSchema.SchemaName, Bid.Columns.EndDate, WhereComparision.GreaterThanOrEqual, DateTime.UtcNow);
                c.BidActive = q.Distinct().GetCount(Bid.TableSchema.SchemaName, Bid.Columns.BidId);


                q = new Query(Bid.TableSchema);
                q.Join(JoinType.LeftJoin, Order.TableSchema, "o",
                    new JoinColumnPair(Bid.TableSchema.SchemaName, Bid.Columns.BidId, Order.Columns.BidId));
                q.Where(Bid.TableSchema.SchemaName, Bid.Columns.TempAppUserId, WhereComparision.EqualsTo, CustomerId);
                q.AddWhere("o", Order.Columns.OrderId, WhereComparision.EqualsTo, null);
                c.BidAbandoned = q.Distinct().GetCount(Bid.TableSchema.SchemaName, Bid.Columns.BidId);
            }
            return c;


        }
        public static Int64 GetCountBidByCustomerIdAndBidType(string CustomerId, CustomerType customerType, BidType bidType)
        {
            List<BidUI> lstBids = new List<BidUI>();
            Query q = new Query(Bid.TableSchema);
            q.Join(JoinType.LeftJoin, AppUser.TableSchema, AppUser.TableSchema.SchemaName,
                new JoinColumnPair(Bid.TableSchema.SchemaName, Bid.Columns.AppUserId, AppUser.Columns.AppUserId));
            q.Join(JoinType.LeftJoin, TempAppUser.TableSchema, TempAppUser.TableSchema.SchemaName,
                new JoinColumnPair(Bid.TableSchema.SchemaName, Bid.Columns.TempAppUserId, TempAppUser.Columns.TempAppUserId));

            q.Join(JoinType.LeftJoin, Order.TableSchema, Order.TableSchema.SchemaName + "1",
                new JoinColumnPair(Bid.TableSchema.SchemaName, Bid.Columns.BidId, Order.Columns.BidId));

            Int64 Id = Convert.ToInt64(CustomerId);

            if (customerType == CustomerType.Temp) q.AddWhere(TempAppUser.TableSchema.SchemaName, TempAppUser.Columns.TempAppUserId, WhereComparision.EqualsTo, Id);
            else if (customerType == CustomerType.AppUser) q.AddWhere(AppUser.TableSchema.SchemaName, AppUser.Columns.AppUserId, WhereComparision.EqualsTo, Id);

            if (bidType == BidType.BidAbandoned) q.AddWhere(Order.TableSchema.SchemaName + "1", Order.Columns.OrderId, WhereComparision.EqualsTo, null);
            if (bidType == BidType.BidPurchase) q.AddWhere(Order.TableSchema.SchemaName + "1", Order.Columns.OrderId, WhereComparision.NotEqualsTo, null);
            if (bidType == BidType.BidActive) q.AddWhere(Bid.TableSchema.SchemaName, Bid.Columns.EndDate, WhereComparision.GreaterThanOrEqual, DateTime.UtcNow);
            return q.Distinct().GetCount(Bid.TableSchema.SchemaName, Bid.Columns.BidId);
        }
        public static Int64 GetCountAllBidByCustomerTypeAndBidType(CustomerType customerType, BidType bidType, bool bidLeave = false)
        {
            //List<BidUI> lstBids = new List<BidUI>();
            //Query q = new Query(Bid.TableSchema);
            //q.Join(JoinType.LeftJoin, AppUser.TableSchema, AppUser.TableSchema.SchemaName,
            //    new JoinColumnPair(Bid.TableSchema.SchemaName, Bid.Columns.AppUserId, AppUser.Columns.AppUserId));
            //q.Join(JoinType.LeftJoin, TempAppUser.TableSchema, TempAppUser.TableSchema.SchemaName,
            //    new JoinColumnPair(Bid.TableSchema.SchemaName, Bid.Columns.TempAppUserId, TempAppUser.Columns.TempAppUserId));
            //q.Join(JoinType.LeftJoin, Offer.TableSchema, Offer.TableSchema.SchemaName,
            //    new JoinColumnPair(Bid.TableSchema.SchemaName, Bid.Columns.BidId, Offer.Columns.BidId));
            //q.Join(JoinType.LeftJoin, Order.TableSchema, Order.TableSchema.SchemaName + "1",
            //    new JoinColumnPair(Bid.TableSchema.SchemaName, Bid.Columns.BidId, Order.Columns.BidId));
            //q.Join(JoinType.LeftJoin, Offer.TableSchema, "o",
            //    new JoinColumnPair(Order.TableSchema.SchemaName + "1", Order.Columns.OfferId, Offer.Columns.OfferId));
            ////  Int64 Id = Convert.ToInt64(CustomerId);
            //if (bidType == BidType.BidAbandoned) 
            //{
            //    q.AddWhere(Order.TableSchema.SchemaName + "1", Order.Columns.OrderId, WhereComparision.EqualsTo, null);

            //}
            //if (bidType == BidType.BidPurchase) q.AddWhere(Order.TableSchema.SchemaName + "1", Order.Columns.OrderId, WhereComparision.NotEqualsTo, null);
            //if (bidType == BidType.BidActive) q.AddWhere(Bid.TableSchema.SchemaName, Bid.Columns.EndDate, WhereComparision.GreaterThanOrEqual, DateTime.UtcNow);
            List<BidUI> lstBids = new List<BidUI>();
            Query q = new Query(Bid.TableSchema);
            q.Join(JoinType.LeftJoin, AppUser.TableSchema, AppUser.TableSchema.SchemaName,
                new JoinColumnPair(Bid.TableSchema.SchemaName, Bid.Columns.AppUserId, AppUser.Columns.AppUserId));
            q.Join(JoinType.LeftJoin, TempAppUser.TableSchema, TempAppUser.TableSchema.SchemaName,
                new JoinColumnPair(Bid.TableSchema.SchemaName, Bid.Columns.TempAppUserId, TempAppUser.Columns.TempAppUserId));

            q.Join(JoinType.LeftJoin, Order.TableSchema, Order.TableSchema.SchemaName + "1",
                new JoinColumnPair(Bid.TableSchema.SchemaName, Bid.Columns.BidId, Order.Columns.BidId));

            q.GroupBy(Bid.TableSchema.SchemaName, Bid.Columns.BidId);
            q.Select(Bid.TableSchema.SchemaName, Bid.Columns.BidId, Bid.Columns.BidId, true);
            q.AddSelect(Bid.TableSchema.SchemaName, Bid.Columns.StartDate, Bid.Columns.StartDate);
            q.AddSelect(Bid.TableSchema.SchemaName, Bid.Columns.EndDate, Bid.Columns.EndDate);
            q.AddSelect(AppUser.TableSchema.SchemaName, AppUser.Columns.AppUserId, AppUser.Columns.AppUserId);
            q.AddSelect(AppUser.TableSchema.SchemaName, AppUser.Columns.FirstName, AppUser.Columns.FirstName);
            q.AddSelect(AppUser.TableSchema.SchemaName, AppUser.Columns.LastName, AppUser.Columns.LastName);
            q.AddSelect(TempAppUser.TableSchema.SchemaName, TempAppUser.Columns.TempAppUserId, TempAppUser.Columns.TempAppUserId);
            q.AddSelectLiteral(" count(Bid.BidId) as 'bidsCount' ");
            q.AddSelect("o", Order.Columns.TotalPrice, Order.Columns.TotalPrice);
            q.AddSelect("o", Order.Columns.Gifts, Order.Columns.Gifts);
            q.AddSelect(Order.TableSchema.SchemaName + "1", Order.Columns.CreateDate, Order.Columns.CreateDate);

            if (bidType == BidType.BidAbandoned)
            {
                q.AddWhere(Order.TableSchema.SchemaName + "1", Order.Columns.OrderId, WhereComparision.EqualsTo, null);


                if (bidLeave)
                {
                    q.Having("count(Order.BidId)=0");
                }
                else
                {
                    q.Having(" count(Order.BidId)>0");
                }


            }

            if (bidType == BidType.BidPurchase) q.AddWhere(Order.TableSchema.SchemaName + "1", Order.Columns.OrderId, WhereComparision.NotEqualsTo, null);

            //b => b.OfferNum == 0 && b.OrderDate == (DateTime?)null && b.EndDate < DateTime.Now

            if (bidType == BidType.BidActive) q.AddWhere(Bid.TableSchema.SchemaName, Bid.Columns.EndDate, WhereComparision.GreaterThanOrEqual, DateTime.UtcNow);
            q.OrderBy(Bid.TableSchema.SchemaName, Bid.Columns.EndDate, SortDirection.DESC);
            //return q.Distinct().GetCount("OffersCount");
            Int32 sum = 0;
            using (DataReaderBase reader = q.ExecuteReader())
            {
                while (reader.Read())
                {
                    sum++;
                }
            }
            return sum;
        }
        public static List<BidUI> GetAllBidByCustomerTypeAndBidType(CustomerType customerType, BidType bidType, bool bidLeave = false, int PageSize = 0, int CurrentPageIndex = 0)
        {
            List<BidUI> lstBids = new List<BidUI>();
            Query q = new Query(Bid.TableSchema);
            q.Join(JoinType.LeftJoin, AppUser.TableSchema, AppUser.TableSchema.SchemaName,
                new JoinColumnPair(Bid.TableSchema.SchemaName, Bid.Columns.AppUserId, AppUser.Columns.AppUserId));
            q.Join(JoinType.LeftJoin, TempAppUser.TableSchema, TempAppUser.TableSchema.SchemaName,
                new JoinColumnPair(Bid.TableSchema.SchemaName, Bid.Columns.TempAppUserId, TempAppUser.Columns.TempAppUserId));

            q.Join(JoinType.LeftJoin, Order.TableSchema, Order.TableSchema.SchemaName + "1",
                new JoinColumnPair(Bid.TableSchema.SchemaName, Bid.Columns.BidId, Order.Columns.BidId));

            q.GroupBy(Bid.TableSchema.SchemaName, Bid.Columns.BidId);
            q.Select(Bid.TableSchema.SchemaName, Bid.Columns.BidId, Bid.Columns.BidId, true);
            q.AddSelect(Bid.TableSchema.SchemaName, Bid.Columns.StartDate, Bid.Columns.StartDate);
            q.AddSelect(Bid.TableSchema.SchemaName, Bid.Columns.EndDate, Bid.Columns.EndDate);
            q.AddSelect(AppUser.TableSchema.SchemaName, AppUser.Columns.AppUserId, AppUser.Columns.AppUserId);
            q.AddSelect(AppUser.TableSchema.SchemaName, AppUser.Columns.FirstName, AppUser.Columns.FirstName);
            q.AddSelect(AppUser.TableSchema.SchemaName, AppUser.Columns.LastName, AppUser.Columns.LastName);
            q.AddSelect(TempAppUser.TableSchema.SchemaName, TempAppUser.Columns.TempAppUserId, TempAppUser.Columns.TempAppUserId);
            q.AddSelectLiteral(" count(Bid.BidId) as 'bidsCount' ");
            q.AddSelect("o", Order.Columns.TotalPrice, Order.Columns.TotalPrice);
            q.AddSelect("o", Order.Columns.Gifts, Order.Columns.Gifts);
            q.AddSelect(Order.TableSchema.SchemaName + "1", Order.Columns.CreateDate, Order.Columns.CreateDate);

            if (bidType == BidType.BidAbandoned)
            {
                q.AddWhere(Order.TableSchema.SchemaName + "1", Order.Columns.OrderId, WhereComparision.EqualsTo, null);
                if (bidLeave)
                {
                    q.Having("count(Order.BidId)=0");
                }
                else
                {
                    q.Having(" count(Order.BidId)>0");
                }



            }

            if (bidType == BidType.BidPurchase) q.AddWhere(Order.TableSchema.SchemaName + "1", Order.Columns.OrderId, WhereComparision.NotEqualsTo, null);

            //b => b.OfferNum == 0 && b.OrderDate == (DateTime?)null && b.EndDate < DateTime.Now

            if (bidType == BidType.BidActive) q.AddWhere(Bid.TableSchema.SchemaName, Bid.Columns.EndDate, WhereComparision.GreaterThanOrEqual, DateTime.UtcNow);
            q.OrderBy(Bid.TableSchema.SchemaName, Bid.Columns.EndDate, SortDirection.DESC);
            if (PageSize > 0)
            {
                q.LimitRows(PageSize).OffsetRows(PageSize * CurrentPageIndex);
            }
            using (DataReaderBase reader = q.ExecuteReader())
            {
                while (reader.Read())
                {

                    BidUI b = new BidUI();
                    b.BidId = reader[Bid.Columns.BidId] is DBNull ? 0 : Convert.ToInt64(reader[Bid.Columns.BidId]);
                    b.StartDate = reader[Bid.Columns.StartDate] is DBNull ? (DateTime?)null : Convert.ToDateTime(reader[Bid.Columns.StartDate]).ToLocalTime();
                    b.EndDate = reader[Bid.Columns.EndDate] is DBNull ? (DateTime?)null : Convert.ToDateTime(reader[Bid.Columns.EndDate]).ToLocalTime();
                    //if (reader[AppUser.Columns.AppUserId] is DBNull)
                    //{
                    //    b.CustomerId = Convert.ToInt64(reader[TempAppUser.Columns.TempAppUserId]);
                    //    b.CustomerName = "Temp";
                    //    b.CustomerType = CustomerType.Temp;
                    //}
                    //if (reader[TempAppUser.Columns.TempAppUserId] is DBNull)
                    //{
                    //    b.CustomerId = Convert.ToInt64(reader[AppUser.Columns.AppUserId]);
                    //    b.CustomerName = (reader[AppUser.Columns.FirstName] is DBNull ? "" : reader[AppUser.Columns.FirstName].ToString() + " ") +
                    //        (reader[AppUser.Columns.LastName] is DBNull ? "" : reader[AppUser.Columns.LastName].ToString());
                    //    b.CustomerType = CustomerType.AppUser;
                    //}

                    b.OfferNum = reader["bidsCount"] is DBNull ? 0 : Convert.ToInt32(reader["bidsCount"]);

                    b.LstProduct = GetProductsByBid(b.BidId);
                    b.Products = GetStringProduct(b.LstProduct);
                    b.Gift = reader["Gifts"] is DBNull ? "" : reader["Gifts"].ToString();
                    b.OrderDate = reader[Order.Columns.CreateDate] is DBNull ? (DateTime?)null : Convert.ToDateTime(reader[Order.Columns.CreateDate]).ToLocalTime();
                    lstBids.Add(b);

                }
            }
            return lstBids;

        }
        public static List<BidUI> GetAllBidByCustomerIdAndBidType(string CustomerId, CustomerType customerType, BidType bidType, int PageSize = 0, int CurrentPageIndex = 0)
        {
            List<BidUI> lstBids = new List<BidUI>();
            Query q = new Query(Bid.TableSchema);
            q.Join(JoinType.LeftJoin, AppUser.TableSchema, AppUser.TableSchema.SchemaName,
                new JoinColumnPair(Bid.TableSchema.SchemaName, Bid.Columns.AppUserId, AppUser.Columns.AppUserId));
            q.Join(JoinType.LeftJoin, TempAppUser.TableSchema, TempAppUser.TableSchema.SchemaName,
                new JoinColumnPair(Bid.TableSchema.SchemaName, Bid.Columns.TempAppUserId, TempAppUser.Columns.TempAppUserId));

            q.Join(JoinType.LeftJoin, Order.TableSchema, Order.TableSchema.SchemaName + "1",
                new JoinColumnPair(Bid.TableSchema.SchemaName, Bid.Columns.BidId, Order.Columns.BidId));

            q.GroupBy(Bid.TableSchema.SchemaName, Bid.Columns.BidId);
            q.Select(Bid.TableSchema.SchemaName, Bid.Columns.BidId, Bid.Columns.BidId, true);
            q.AddSelect(Bid.TableSchema.SchemaName, Bid.Columns.StartDate, Bid.Columns.StartDate);
            q.AddSelect(Bid.TableSchema.SchemaName, Bid.Columns.EndDate, Bid.Columns.EndDate);
            q.AddSelect(AppUser.TableSchema.SchemaName, AppUser.Columns.AppUserId, AppUser.Columns.AppUserId);
            q.AddSelect(AppUser.TableSchema.SchemaName, AppUser.Columns.FirstName, AppUser.Columns.FirstName);
            q.AddSelect(AppUser.TableSchema.SchemaName, AppUser.Columns.LastName, AppUser.Columns.LastName);
            q.AddSelect(TempAppUser.TableSchema.SchemaName, TempAppUser.Columns.TempAppUserId, TempAppUser.Columns.TempAppUserId);
            q.AddSelectLiteral(" count(Bid.BidId) as 'bidsCount' ");
            //q.AddSelect("o", Order.Columns.TotalPrice, Order.Columns.TotalPrice);
            //q.AddSelect("o", Order.Columns.Gifts, Order.Columns.Gifts);
            q.AddSelect(Order.TableSchema.SchemaName + "1", Order.Columns.CreateDate, Order.Columns.CreateDate);

            Int64 Id = Convert.ToInt64(CustomerId);
            if (customerType == CustomerType.Temp) q.AddWhere(TempAppUser.TableSchema.SchemaName, TempAppUser.Columns.TempAppUserId, WhereComparision.EqualsTo, Id);
            else if (customerType == CustomerType.AppUser) q.AddWhere(AppUser.TableSchema.SchemaName, AppUser.Columns.AppUserId, WhereComparision.EqualsTo, Id);


            if (bidType == BidType.BidAbandoned) q.AddWhere(Order.TableSchema.SchemaName + "1", Order.Columns.OrderId, WhereComparision.EqualsTo, null);
            if (bidType == BidType.BidPurchase) q.AddWhere(Order.TableSchema.SchemaName + "1", Order.Columns.OrderId, WhereComparision.NotEqualsTo, null);
            if (bidType == BidType.BidActive) q.AddWhere(Bid.TableSchema.SchemaName, Bid.Columns.EndDate, WhereComparision.GreaterThanOrEqual, DateTime.UtcNow);
            q.OrderBy(Bid.TableSchema.SchemaName, Bid.Columns.EndDate, SortDirection.DESC);
            if (PageSize > 0)
            {
                q.LimitRows(PageSize).OffsetRows(PageSize * CurrentPageIndex);
            }
            using (DataReaderBase reader = q.ExecuteReader())
            {
                while (reader.Read())
                {
                    BidUI b = new BidUI();
                    b.BidId = reader[Bid.Columns.BidId] is DBNull ? 0 : Convert.ToInt64(reader[Bid.Columns.BidId]);
                    b.StartDate = reader[Bid.Columns.StartDate] is DBNull ? (DateTime?)null : Convert.ToDateTime(reader[Bid.Columns.StartDate]).ToLocalTime();
                    b.EndDate = reader[Bid.Columns.EndDate] is DBNull ? (DateTime?)null : Convert.ToDateTime(reader[Bid.Columns.EndDate]).ToLocalTime();
                    if (reader[AppUser.Columns.AppUserId] is DBNull)
                    {
                        b.CustomerId = Convert.ToInt64(reader[TempAppUser.Columns.TempAppUserId]);
                        b.CustomerName = "Temp";
                        b.CustomerType = CustomerType.Temp;
                    }
                    if (reader[TempAppUser.Columns.TempAppUserId] is DBNull)
                    {
                        b.CustomerId = Convert.ToInt64(reader[AppUser.Columns.AppUserId]);
                        b.CustomerName = (reader[AppUser.Columns.FirstName] is DBNull ? "" : reader[AppUser.Columns.FirstName].ToString() + " ") +
                            (reader[AppUser.Columns.LastName] is DBNull ? "" : reader[AppUser.Columns.LastName].ToString());
                        b.CustomerType = CustomerType.AppUser;
                    }

                    b.OfferNum = reader["bidsCount"] is DBNull ? 0 : Convert.ToInt32(reader["bidsCount"]);
                    b.LstProduct = GetProductsByBid(b.BidId);
                    b.Products = GetStringProduct(b.LstProduct);
                    // b.Gift = reader["Gift"] is DBNull ? "" : reader["Gift"].ToString();
                    b.OrderDate = reader[Order.Columns.CreateDate] is DBNull ? (DateTime?)null : Convert.ToDateTime(reader[Order.Columns.CreateDate]).ToLocalTime();
                    lstBids.Add(b);
                }
            }
            return lstBids;

        }
        public static Int64 GetAllBidsCount(DateTime? Start, DateTime? end, string phone, string customerId, Int64 BidId, string cityName)
        {

            Query q = new Query(Bid.TableSchema);
            q.Join(JoinType.LeftJoin, AppUser.TableSchema, AppUser.TableSchema.SchemaName,
                new JoinColumnPair(Bid.TableSchema.SchemaName, Bid.Columns.AppUserId, AppUser.Columns.AppUserId));
            q.Join(JoinType.LeftJoin, TempAppUser.TableSchema, TempAppUser.TableSchema.SchemaName,
                new JoinColumnPair(Bid.TableSchema.SchemaName, Bid.Columns.TempAppUserId, TempAppUser.Columns.TempAppUserId));
            q.Join(JoinType.LeftJoin, City.TableSchema, City.TableSchema.SchemaName,
                new JoinColumnPair(TempAppUser.TableSchema, TempAppUser.Columns.CityId, City.Columns.CityId).JoinOR(AppUser.TableSchema.SchemaName, AppUser.Columns.CityId, City.Columns.CityId));

            q.Join(JoinType.LeftJoin, Order.TableSchema, Order.TableSchema.SchemaName + "1",
                new JoinColumnPair(Bid.TableSchema.SchemaName, Bid.Columns.BidId, Order.Columns.BidId));

            q.Join(JoinType.LeftJoin, AppSupplier.TableSchema, AppSupplier.TableSchema.SchemaName,
                new JoinColumnPair("o", Order.Columns.SupplierId, AppSupplier.Columns.SupplierId));
            if (Start != null) q.Where(Bid.TableSchema.SchemaName, Bid.Columns.StartDate, WhereComparision.GreaterThanOrEqual, Start);
            if (end != null) q.AddWhere(Bid.TableSchema.SchemaName, Bid.Columns.EndDate, WhereComparision.LessThanOrEqual, end);
            if (phone != "") q.AddWhere(AppUser.TableSchema.SchemaName, AppUser.Columns.Phone, WhereComparision.Like, "%" + phone + "%");
            if (customerId != "" && customerId.StartsWith("000"))
            {
                Int64 Id = Convert.ToInt64(customerId);
                q.AddWhere(TempAppUser.TableSchema.SchemaName, TempAppUser.Columns.TempAppUserId, WhereComparision.EqualsTo, Id);
            }
            else if (customerId != "")
            {
                Int64 Id = Convert.ToInt64(customerId);
                q.AddWhere(AppUser.TableSchema.SchemaName, AppUser.Columns.AppUserId, WhereComparision.EqualsTo, Id);
            }
            if (BidId != 0) q.AddWhere(Bid.TableSchema.SchemaName, Bid.Columns.BidId, WhereComparision.EqualsTo, BidId);
            if (cityName != "") q.AddWhere(City.TableSchema.SchemaName, City.Columns.CityName, WhereComparision.EqualsTo, cityName);

            return q.Distinct().GetCount(Bid.TableSchema.SchemaName, Bid.Columns.BidId);
        }
        public static List<BidUI> GetAllBids(string filterSearch, DateTime from, DateTime to, string phone, string customerId, Int64 BidId, string cityName, int PageSize = 0, int CurrentPageIndex = 0, bool notDeleted = false)
        {
            List<BidUI> lstBids = new List<BidUI>();
            Query q = new Query(Bid.TableSchema);
            q.Join(JoinType.LeftJoin, AppUser.TableSchema, AppUser.TableSchema.SchemaName,
                new JoinColumnPair(Bid.TableSchema.SchemaName, Bid.Columns.AppUserId, AppUser.Columns.AppUserId));
            q.Join(JoinType.LeftJoin, TempAppUser.TableSchema, TempAppUser.TableSchema.SchemaName,
                new JoinColumnPair(Bid.TableSchema.SchemaName, Bid.Columns.TempAppUserId, TempAppUser.Columns.TempAppUserId));
            q.Join(JoinType.LeftJoin, City.TableSchema, City.TableSchema.SchemaName,
                new JoinColumnPair(TempAppUser.TableSchema, TempAppUser.Columns.CityId, City.Columns.CityId).JoinOR(AppUser.TableSchema.SchemaName, AppUser.Columns.CityId, City.Columns.CityId));
            q.Join(JoinType.LeftJoin, Order.TableSchema, Order.TableSchema.SchemaName,
                new JoinColumnPair(Bid.TableSchema.SchemaName, Bid.Columns.BidId, Order.Columns.BidId));
            q.Join(JoinType.LeftJoin, AppSupplier.TableSchema, AppSupplier.TableSchema.SchemaName,
                new JoinColumnPair(Order.TableSchema, Order.Columns.SupplierId, AppSupplier.Columns.SupplierId));
            q.GroupBy(Bid.TableSchema.SchemaName, Bid.Columns.BidId);
            q.Select(Bid.TableSchema.SchemaName, Bid.Columns.BidId, Bid.Columns.BidId, true);
            q.AddSelect(Bid.TableSchema.SchemaName, Bid.Columns.StartDate, Bid.Columns.StartDate);
            q.AddSelect(Bid.TableSchema.SchemaName, Bid.Columns.EndDate, Bid.Columns.EndDate);
            q.AddSelect(Bid.TableSchema.SchemaName, Bid.Columns.IsActive, Bid.Columns.IsActive);
            q.AddSelect(AppUser.TableSchema.SchemaName, AppUser.Columns.AppUserId, AppUser.Columns.AppUserId);
            q.AddSelect(AppUser.TableSchema.SchemaName, AppUser.Columns.FirstName, AppUser.Columns.FirstName);
            q.AddSelect(AppUser.TableSchema.SchemaName, AppUser.Columns.LastName, AppUser.Columns.LastName);
            q.AddSelect(AppUser.TableSchema.SchemaName, AppUser.Columns.CityId, AppUser.Columns.CityId);
            q.AddSelect(AppUser.TableSchema.SchemaName, AppUser.Columns.Phone, AppUser.Columns.Phone);
            q.AddSelect(TempAppUser.TableSchema.SchemaName, TempAppUser.Columns.TempAppUserId, TempAppUser.Columns.TempAppUserId);
            q.AddSelect(TempAppUser.TableSchema.SchemaName, TempAppUser.Columns.CityId, TempAppUser.Columns.CityId);
            q.AddSelectLiteral(" count(Bid.BidId) as bidsCount");
            q.AddSelect(AppSupplier.TableSchema.SchemaName, AppSupplier.Columns.SupplierId, AppSupplier.Columns.SupplierId);
            q.AddSelect(AppSupplier.TableSchema.SchemaName, AppSupplier.Columns.BusinessName, AppSupplier.Columns.BusinessName);
            q.AddSelect(Order.TableSchema.SchemaName, Order.Columns.TotalPrice, Order.Columns.TotalPrice);
            q.AddSelect(Order.TableSchema.SchemaName, Order.Columns.Gifts, Order.Columns.Gifts);
            q.AddSelect(City.TableSchema.SchemaName, City.Columns.CityName, City.Columns.CityName);
            q.AddSelect(Order.TableSchema.SchemaName, Order.Columns.CreateDate, "CD");
            q.AddSelect(Order.TableSchema.SchemaName, Order.Columns.SuppliedDate, Order.Columns.SuppliedDate);
            q.AddSelect(Order.TableSchema.SchemaName, Order.Columns.ReceivedDate, Order.Columns.ReceivedDate);
            q.AddSelect(Order.TableSchema.SchemaName, Order.Columns.Source, Order.Columns.Source);
            q.OrderBy(Bid.TableSchema.SchemaName, Bid.Columns.StartDate, SortDirection.DESC);
            if (from != DateTime.MinValue)
            {
                q.AddWhere(Bid.Columns.EndDate, WhereComparision.GreaterThanOrEqual, from);
            }
            if (to != DateTime.MinValue)
            {
                q.AddWhere(Bid.Columns.EndDate, WhereComparision.LessThanOrEqual, to.AddHours(24));
            }
            if (phone != "") q.AddWhere(AppUser.TableSchema.SchemaName, AppUser.Columns.Phone, WhereComparision.Like, "%" + phone + "%");
            if (customerId != "" && customerId.StartsWith("000"))
            {
                Int64 Id = Convert.ToInt64(customerId);
                q.AddWhere(TempAppUser.TableSchema.SchemaName, TempAppUser.Columns.TempAppUserId, WhereComparision.EqualsTo, Id);
            }
            if (notDeleted)
                q.AddWhere(Bid.TableSchema.SchemaName, Bid.Columns.Deleted, WhereComparision.EqualsTo, null);
            else if (customerId != "")
            {
                Int64 Id = Convert.ToInt64(customerId);
                q.AddWhere(AppUser.TableSchema.SchemaName, AppUser.Columns.AppUserId, WhereComparision.EqualsTo, Id);
            }
            if (BidId != -1) q.AddWhere(Bid.TableSchema.SchemaName, Bid.Columns.BidId, WhereComparision.EqualsTo, BidId);
            if (cityName != "") q.AddWhere(City.TableSchema.SchemaName, City.Columns.CityName, WhereComparision.EqualsTo, cityName);
            if (filterSearch == "ActiveBids")
            {
                // (b => b.EndDate >= DateTime.Now
                q.AddWhere(Bid.TableSchema.SchemaName, Bid.Columns.EndDate, WhereComparision.GreaterThanOrEqual, DateTime.Now);

            }
            else if (filterSearch == "BidsWithOffers")
            {
                q.AddWhere(Order.TableSchema.SchemaName + "1", Order.Columns.CreateDate, WhereComparision.EqualsTo, (DateTime?)null);
                q.AddWhere(Bid.TableSchema.SchemaName, Bid.Columns.EndDate, WhereComparision.LessThan, DateTime.Now);
                q.Having("count(Bid.BidId) > 0");

            }
            else if (filterSearch == "BidsWithOutOffers")
            {
                // (b => b.OfferNum == 0 && b.OrderDate == (DateTime?)null && b.EndDate < DateTime.Now
                q.AddWhere(Order.TableSchema.SchemaName, Order.Columns.CreateDate, WhereComparision.EqualsTo, (DateTime?)null);
                q.AddWhere(Bid.TableSchema.SchemaName, Bid.Columns.EndDate, WhereComparision.LessThan, DateTime.Now);
                q.Having("count(Bid.BidId) = 0");
            }
            else if (filterSearch == "PurchaseBids")
            {
                //b => b.OrderDate != (DateTime?)null
                q.AddWhere(Order.TableSchema.SchemaName, Order.Columns.CreateDate, WhereComparision.NotEqualsTo, (DateTime?)null);
            }

            if (PageSize > 0)
            {
                q.LimitRows(PageSize).OffsetRows(PageSize * CurrentPageIndex);
            }
            using (DataReaderBase reader = q.ExecuteReader())
            {
                while (reader.Read())
                {
                    BidUI b = new BidUI();
                    b.BidId = reader[Bid.Columns.BidId] is DBNull ? 0 : Convert.ToInt64(reader[Bid.Columns.BidId]);
                    b.StartDate = reader[Bid.Columns.StartDate] is DBNull ? (DateTime?)null : Convert.ToDateTime(reader[Bid.Columns.StartDate]).ToLocalTime();
                    b.EndDate = reader[Bid.Columns.EndDate] is DBNull ? (DateTime?)null : Convert.ToDateTime(reader[Bid.Columns.EndDate]).ToLocalTime();
                    b.OrderDate = reader["CD"] is DBNull ? (DateTime?)null : Convert.ToDateTime(reader["CD"]).ToLocalTime();
                    if (!(reader[TempAppUser.Columns.TempAppUserId] is DBNull))
                    {
                        b.CustomerId = Convert.ToInt64(reader[TempAppUser.Columns.TempAppUserId]);
                        b.CustomerName = "Temp";
                        b.CustomerType = CustomerType.Temp;
                        b.City = reader[City.Columns.CityName] is DBNull ? "" : reader[City.Columns.CityName].ToString();
                        b.Phone = "";
                    }
                    if (!(reader[AppUser.Columns.AppUserId] is DBNull))
                    {
                        b.CustomerId = Convert.ToInt64(reader[AppUser.Columns.AppUserId]);
                        b.CustomerName = (reader[AppUser.Columns.FirstName] is DBNull ? "" : reader[AppUser.Columns.FirstName].ToString() + " ") +
                            (reader[AppUser.Columns.LastName] is DBNull ? "" : reader[AppUser.Columns.LastName].ToString());
                        b.CustomerType = CustomerType.AppUser;
                        b.City = reader[City.Columns.CityName] is DBNull ? "" : reader[City.Columns.CityName].ToString();
                        b.Phone = reader[AppUser.Columns.Phone] is DBNull ? "" : reader[AppUser.Columns.Phone].ToString();
                    }

                    b.OfferNum = reader["bidsCount"] is DBNull ? 0 : Convert.ToInt32(reader["bidsCount"]);
                    b.SupplierId = reader[AppSupplier.Columns.SupplierId] is DBNull ? 0 : Convert.ToInt64(reader[AppSupplier.Columns.SupplierId]);
                    b.SupplierName = (reader[AppSupplier.Columns.BusinessName] is DBNull ? "" : (reader[AppSupplier.Columns.BusinessName]).ToString());
                    b.Price = reader[Order.Columns.TotalPrice] is DBNull ? 0 : Convert.ToDecimal(reader[Order.Columns.TotalPrice]);
                    b.IsActive = reader[Bid.Columns.IsActive] is DBNull ? false : Convert.ToBoolean(reader[Bid.Columns.IsActive]);
                    b.LstProduct = GetProductsByBid(b.BidId);
                    b.Products = GetStringProduct(b.LstProduct);
                    b.Gift = reader[Order.Columns.Gifts] is DBNull ? "" : reader[Order.Columns.Gifts].ToString();
                    b.SuppliedDate = reader[Order.Columns.SuppliedDate] is DBNull ? (DateTime?)null : Convert.ToDateTime(reader[Order.Columns.SuppliedDate]).ToLocalTime();
                    b.ReceivedDate = reader[Order.Columns.ReceivedDate] is DBNull ? (DateTime?)null : Convert.ToDateTime(reader[Order.Columns.ReceivedDate]).ToLocalTime();
                    b.OrderStatus = GetOrderStatus(b.SupplierId, b.SuppliedDate, b.ReceivedDate);
                    b.Source = (Source)Enum.Parse(typeof(Source), reader[Order.Columns.Source].ToString()==""?"0": reader[Order.Columns.Source].ToString());
                    lstBids.Add(b);
                }
            }
            return lstBids;

        }

        public static OrderDeliveryStatus GetOrderStatus(long SupplierId, DateTime? SuppliedDate, DateTime? ReceivedDate)
        {
            if (ReceivedDate != null)
                return OrderDeliveryStatus.CustomerApprovedSupplied;
            if (SuppliedDate != null)
                return OrderDeliveryStatus.SuppliedToTheCustomer;
            if (SupplierId > 0)
                return OrderDeliveryStatus.ApprovedBySupplier;
            return OrderDeliveryStatus.None;
        }

        public static List<BidUI> GetUntakenBids(string filterSearch, DateTime from, DateTime to, string phone, string customerId, Int64 BidId, string cityName, int PageSize = 0, int CurrentPageIndex = 0)
        {
            List<BidUI> lstBids = new List<BidUI>();
            Query q = new Query(Bid.TableSchema);
            q.Join(JoinType.LeftJoin, AppUser.TableSchema, AppUser.TableSchema.SchemaName,
                new JoinColumnPair(Bid.TableSchema.SchemaName, Bid.Columns.AppUserId, AppUser.Columns.AppUserId));
            q.Join(JoinType.LeftJoin, City.TableSchema, City.TableSchema.SchemaName,
                new JoinColumnPair(AppUser.TableSchema.SchemaName, AppUser.Columns.CityId, City.Columns.CityId));
            q.Join(JoinType.LeftJoin, Order.TableSchema, Order.TableSchema.SchemaName,
                new JoinColumnPair(Bid.TableSchema.SchemaName, Bid.Columns.BidId, Order.Columns.BidId));
            q.Join(JoinType.InnerJoin, Order.TableSchema, Order.Columns.BidId, Order.TableSchema.SchemaName, BidMessage.TableSchema, BidMessage.Columns.BidId, BidMessage.TableSchema.SchemaName);
            q.GroupBy(Bid.TableSchema.SchemaName, Bid.Columns.BidId);
            q.Select(Bid.TableSchema.SchemaName, Bid.Columns.BidId, Bid.Columns.BidId, true);
            q.AddSelect(Bid.TableSchema.SchemaName, Bid.Columns.StartDate, Bid.Columns.StartDate);
            q.AddSelect(Bid.TableSchema.SchemaName, Bid.Columns.EndDate, Bid.Columns.EndDate);
            q.AddSelect(Bid.TableSchema.SchemaName, Bid.Columns.IsActive, Bid.Columns.IsActive);

            q.AddSelect(AppUser.TableSchema.SchemaName, AppUser.Columns.AppUserId, AppUser.Columns.AppUserId);
            q.AddSelect(AppUser.TableSchema.SchemaName, AppUser.Columns.FirstName, AppUser.Columns.FirstName);
            q.AddSelect(AppUser.TableSchema.SchemaName, AppUser.Columns.LastName, AppUser.Columns.LastName);
            q.AddSelect(AppUser.TableSchema.SchemaName, AppUser.Columns.CityId, AppUser.Columns.CityId);
            q.AddSelect(AppUser.TableSchema.SchemaName, AppUser.Columns.Phone, AppUser.Columns.Phone);

            q.AddSelect(Order.TableSchema.SchemaName, Order.Columns.TotalPrice, Order.Columns.TotalPrice);
            q.AddSelect(Order.TableSchema.SchemaName, Order.Columns.Gifts, Order.Columns.Gifts);
            q.AddSelect(City.TableSchema.SchemaName, City.Columns.CityName, City.Columns.CityName);
            q.AddSelect(Order.TableSchema.SchemaName, Order.Columns.CreateDate, "CD");
            q.AddWhere(BidMessage.Columns.Stage, BIdMessageController.ADMIN_STAGE);
            q.AddWhere(Order.TableSchema.SchemaName, Order.Columns.SupplierId, WhereComparision.EqualsTo, null);
            q.AddWhere(BidMessage.TableSchema.SchemaName, BidMessage.Columns.IsActive, WhereComparision.EqualsTo, 0);

            q.OrderBy(Bid.TableSchema.SchemaName, Bid.Columns.StartDate, SortDirection.DESC);
            if (from != DateTime.MinValue)
            {
                q.AddWhere(Bid.Columns.EndDate, WhereComparision.GreaterThanOrEqual, from);
            }
            if (to != DateTime.MinValue)
            {
                q.AddWhere(Bid.Columns.EndDate, WhereComparision.LessThanOrEqual, to.AddHours(24));
            }
            if (phone != "") q.AddWhere(AppUser.TableSchema.SchemaName, AppUser.Columns.Phone, WhereComparision.Like, "%" + phone + "%");

            if (customerId != "")
            {
                Int64 Id = Convert.ToInt64(customerId);
                q.AddWhere(AppUser.TableSchema.SchemaName, AppUser.Columns.AppUserId, WhereComparision.EqualsTo, Id);
            }
            if (BidId != -1) q.AddWhere(Bid.TableSchema.SchemaName, Bid.Columns.BidId, WhereComparision.EqualsTo, BidId);
            if (cityName != "") q.AddWhere(City.TableSchema.SchemaName, City.Columns.CityName, WhereComparision.EqualsTo, cityName);
            if (filterSearch == "ActiveBids")
            {
                // (b => b.EndDate >= DateTime.Now
                q.AddWhere(Bid.TableSchema.SchemaName, Bid.Columns.EndDate, WhereComparision.GreaterThanOrEqual, DateTime.Now);

            }
            else if (filterSearch == "BidsWithOffers")
            {
                q.AddWhere(Order.TableSchema.SchemaName + "1", Order.Columns.CreateDate, WhereComparision.EqualsTo, (DateTime?)null);
                q.AddWhere(Bid.TableSchema.SchemaName, Bid.Columns.EndDate, WhereComparision.LessThan, DateTime.Now);
                q.Having("count(Bid.BidId) > 0");

            }
            else if (filterSearch == "BidsWithOutOffers")
            {
                // (b => b.OfferNum == 0 && b.OrderDate == (DateTime?)null && b.EndDate < DateTime.Now
                q.AddWhere(Order.TableSchema.SchemaName, Order.Columns.CreateDate, WhereComparision.EqualsTo, (DateTime?)null);
                q.AddWhere(Bid.TableSchema.SchemaName, Bid.Columns.EndDate, WhereComparision.LessThan, DateTime.Now);
                q.Having("count(Bid.BidId) = 0");
            }
            else if (filterSearch == "PurchaseBids")
            {
                //b => b.OrderDate != (DateTime?)null
                q.AddWhere(Order.TableSchema.SchemaName, Order.Columns.CreateDate, WhereComparision.NotEqualsTo, (DateTime?)null);
            }

            if (PageSize > 0)
            {
                q.LimitRows(PageSize).OffsetRows(PageSize * CurrentPageIndex);
            }
            using (DataReaderBase reader = q.ExecuteReader())
            {
                while (reader.Read())
                {
                    BidUI b = new BidUI();
                    b.BidId = reader[Bid.Columns.BidId] is DBNull ? 0 : Convert.ToInt64(reader[Bid.Columns.BidId]);
                    b.StartDate = reader[Bid.Columns.StartDate] is DBNull ? (DateTime?)null : Convert.ToDateTime(reader[Bid.Columns.StartDate]).ToLocalTime();
                    b.EndDate = reader[Bid.Columns.EndDate] is DBNull ? (DateTime?)null : Convert.ToDateTime(reader[Bid.Columns.EndDate]).ToLocalTime();
                    b.OrderDate = reader["CD"] is DBNull ? (DateTime?)null : Convert.ToDateTime(reader["CD"]).ToLocalTime();

                    if (!(reader[AppUser.Columns.AppUserId] is DBNull))
                    {
                        b.CustomerId = Convert.ToInt64(reader[AppUser.Columns.AppUserId]);
                        b.CustomerName = (reader[AppUser.Columns.FirstName] is DBNull ? "" : reader[AppUser.Columns.FirstName].ToString() + " ") +
                            (reader[AppUser.Columns.LastName] is DBNull ? "" : reader[AppUser.Columns.LastName].ToString());
                        b.CustomerType = CustomerType.AppUser;
                        b.City = reader[City.Columns.CityName] is DBNull ? "" : reader[City.Columns.CityName].ToString();
                        b.Phone = reader[AppUser.Columns.Phone] is DBNull ? "" : reader[AppUser.Columns.Phone].ToString();
                    }

                    b.Price = reader[Order.Columns.TotalPrice] is DBNull ? 0 : Convert.ToDecimal(reader[Order.Columns.TotalPrice]);
                    b.IsActive = reader[Bid.Columns.IsActive] is DBNull ? false : Convert.ToBoolean(reader[Bid.Columns.IsActive]);
                    b.LstProduct = GetProductsByBid(b.BidId);
                    b.Products = GetStringProduct(b.LstProduct);
                    b.Gift = reader[Order.Columns.Gifts] is DBNull ? "" : reader[Order.Columns.Gifts].ToString();
                    lstBids.Add(b);
                }
            }
            return lstBids;

        }

        public static List<BidUI> GetUntakenBidsTempForCampaign(string filterSearch, DateTime from, DateTime to, string phone, string customerId, Int64 BidId, string cityName, int PageSize = 0, int CurrentPageIndex = 0)
        {
            List<BidUI> lstBids = new List<BidUI>();
            Query q = new Query(Bid.TableSchema);
            q.Join(JoinType.LeftJoin, AppUser.TableSchema, AppUser.TableSchema.SchemaName,
                new JoinColumnPair(Bid.TableSchema.SchemaName, Bid.Columns.AppUserId, AppUser.Columns.AppUserId));
            q.Join(JoinType.LeftJoin, City.TableSchema, City.TableSchema.SchemaName,
                new JoinColumnPair(AppUser.TableSchema.SchemaName, AppUser.Columns.CityId, City.Columns.CityId));
            q.Join(JoinType.LeftJoin, Order.TableSchema, Order.TableSchema.SchemaName,
                new JoinColumnPair(Bid.TableSchema.SchemaName, Bid.Columns.BidId, Order.Columns.BidId));
            q.Join(JoinType.InnerJoin, Order.TableSchema, Order.Columns.BidId, Order.TableSchema.SchemaName, BidMessage.TableSchema, BidMessage.Columns.BidId, BidMessage.TableSchema.SchemaName);
            q.GroupBy(Bid.TableSchema.SchemaName, Bid.Columns.BidId);
            q.Select(Bid.TableSchema.SchemaName, Bid.Columns.BidId, Bid.Columns.BidId, true);
            q.AddSelect(Bid.TableSchema.SchemaName, Bid.Columns.StartDate, Bid.Columns.StartDate);
            q.AddSelect(Bid.TableSchema.SchemaName, Bid.Columns.EndDate, Bid.Columns.EndDate);
            q.AddSelect(Bid.TableSchema.SchemaName, Bid.Columns.IsActive, Bid.Columns.IsActive);

            q.AddSelect(AppUser.TableSchema.SchemaName, AppUser.Columns.AppUserId, AppUser.Columns.AppUserId);
            q.AddSelect(AppUser.TableSchema.SchemaName, AppUser.Columns.FirstName, AppUser.Columns.FirstName);
            q.AddSelect(AppUser.TableSchema.SchemaName, AppUser.Columns.LastName, AppUser.Columns.LastName);
            q.AddSelect(AppUser.TableSchema.SchemaName, AppUser.Columns.CityId, AppUser.Columns.CityId);
            q.AddSelect(AppUser.TableSchema.SchemaName, AppUser.Columns.Phone, AppUser.Columns.Phone);

            q.AddSelect(Order.TableSchema.SchemaName, Order.Columns.TotalPrice, Order.Columns.TotalPrice);
            q.AddSelect(Order.TableSchema.SchemaName, Order.Columns.Gifts, Order.Columns.Gifts);
            q.AddSelect(City.TableSchema.SchemaName, City.Columns.CityName, City.Columns.CityName);
            q.AddSelect(Order.TableSchema.SchemaName, Order.Columns.CreateDate, "CD");
            q.AddWhere(BidMessage.Columns.Stage, BIdMessageController.ADMIN_STAGE);
           
            q.OrderBy(Bid.TableSchema.SchemaName, Bid.Columns.StartDate, SortDirection.DESC);
            if (from != DateTime.MinValue)
            {
                q.AddWhere(Bid.Columns.EndDate, WhereComparision.GreaterThanOrEqual, from);
            }
            if (to != DateTime.MinValue)
            {
                q.AddWhere(Bid.Columns.EndDate, WhereComparision.LessThanOrEqual, to.AddHours(24));
            }
            if (phone != "") q.AddWhere(AppUser.TableSchema.SchemaName, AppUser.Columns.Phone, WhereComparision.Like, "%" + phone + "%");

            if (customerId != "")
            {
                Int64 Id = Convert.ToInt64(customerId);
                q.AddWhere(AppUser.TableSchema.SchemaName, AppUser.Columns.AppUserId, WhereComparision.EqualsTo, Id);
            }
            if (BidId != -1) q.AddWhere(Bid.TableSchema.SchemaName, Bid.Columns.BidId, WhereComparision.EqualsTo, BidId);
            if (cityName != "") q.AddWhere(City.TableSchema.SchemaName, City.Columns.CityName, WhereComparision.EqualsTo, cityName);
            if (filterSearch == "ActiveBids")
            {
                // (b => b.EndDate >= DateTime.Now
                q.AddWhere(Bid.TableSchema.SchemaName, Bid.Columns.EndDate, WhereComparision.GreaterThanOrEqual, DateTime.Now);

            }
            else if (filterSearch == "BidsWithOffers")
            {
                q.AddWhere(Order.TableSchema.SchemaName + "1", Order.Columns.CreateDate, WhereComparision.EqualsTo, (DateTime?)null);
                q.AddWhere(Bid.TableSchema.SchemaName, Bid.Columns.EndDate, WhereComparision.LessThan, DateTime.Now);
                q.Having("count(Bid.BidId) > 0");

            }
            else if (filterSearch == "BidsWithOutOffers")
            {
                // (b => b.OfferNum == 0 && b.OrderDate == (DateTime?)null && b.EndDate < DateTime.Now
                q.AddWhere(Order.TableSchema.SchemaName, Order.Columns.CreateDate, WhereComparision.EqualsTo, (DateTime?)null);
                q.AddWhere(Bid.TableSchema.SchemaName, Bid.Columns.EndDate, WhereComparision.LessThan, DateTime.Now);
                q.Having("count(Bid.BidId) = 0");
            }
            else if (filterSearch == "PurchaseBids")
            {
                //b => b.OrderDate != (DateTime?)null
                q.AddWhere(Order.TableSchema.SchemaName, Order.Columns.CreateDate, WhereComparision.NotEqualsTo, (DateTime?)null);
            }

            if (PageSize > 0)
            {
                q.LimitRows(PageSize).OffsetRows(PageSize * CurrentPageIndex);
            }
            using (DataReaderBase reader = q.ExecuteReader())
            {
                while (reader.Read())
                {
                    BidUI b = new BidUI();
                    b.BidId = reader[Bid.Columns.BidId] is DBNull ? 0 : Convert.ToInt64(reader[Bid.Columns.BidId]);
                    b.StartDate = reader[Bid.Columns.StartDate] is DBNull ? (DateTime?)null : Convert.ToDateTime(reader[Bid.Columns.StartDate]).ToLocalTime();
                    b.EndDate = reader[Bid.Columns.EndDate] is DBNull ? (DateTime?)null : Convert.ToDateTime(reader[Bid.Columns.EndDate]).ToLocalTime();
                    b.OrderDate = reader["CD"] is DBNull ? (DateTime?)null : Convert.ToDateTime(reader["CD"]).ToLocalTime();

                    if (!(reader[AppUser.Columns.AppUserId] is DBNull))
                    {
                        b.CustomerId = Convert.ToInt64(reader[AppUser.Columns.AppUserId]);
                        b.CustomerName = (reader[AppUser.Columns.FirstName] is DBNull ? "" : reader[AppUser.Columns.FirstName].ToString() + " ") +
                            (reader[AppUser.Columns.LastName] is DBNull ? "" : reader[AppUser.Columns.LastName].ToString());
                        b.CustomerType = CustomerType.AppUser;
                        b.City = reader[City.Columns.CityName] is DBNull ? "" : reader[City.Columns.CityName].ToString();
                        b.Phone = reader[AppUser.Columns.Phone] is DBNull ? "" : reader[AppUser.Columns.Phone].ToString();
                    }

                    b.Price = reader[Order.Columns.TotalPrice] is DBNull ? 0 : Convert.ToDecimal(reader[Order.Columns.TotalPrice]);
                    b.IsActive = reader[Bid.Columns.IsActive] is DBNull ? false : Convert.ToBoolean(reader[Bid.Columns.IsActive]);
                    b.LstProduct = GetProductsByBid(b.BidId);
                    b.Products = GetStringProduct(b.LstProduct);
                    b.Gift = reader[Order.Columns.Gifts] is DBNull ? "" : reader[Order.Columns.Gifts].ToString();
                    lstBids.Add(b);
                }
            }
            return lstBids;

        }

        public static string GetStringProduct(List<BidProductUI> lst)
        {
            string Products = "";
            foreach (BidProductUI item in lst)
            {
                //Products += item.Amount.ToString() + " " + item.ProductName + " " + item.ProductAmount + " <br>";
                Products += item.Amount.ToString() + " " + item.ProductName + " <br>";
            }
            return Products;
        }
        public static List<BidProductUI> GetLastShoppingCart(Int64 AppUserId, out decimal TotalPrice)
        {
            Query q = new Query(Order.TableSchema);

            q.Join(JoinType.InnerJoin, Order.TableSchema, Order.Columns.BidId, Order.TableSchema.SchemaName,
                 Bid.TableSchema, Bid.Columns.BidId, Bid.TableSchema.SchemaName);

            q.OrderBy(Order.TableSchema.SchemaName, Order.Columns.CreateDate, SortDirection.DESC);
            q.Where(Bid.TableSchema.SchemaName, Bid.Columns.AppUserId, WhereComparision.EqualsTo, AppUserId);
            q.Select(Order.TableSchema.SchemaName, Order.Columns.BidId, Order.Columns.BidId, true);
            //q.AddSelect(Product.TableSchema.SchemaName, Product.Columns.RecomendedPrice, Product.Columns.RecomendedPrice);
            q.AddSelect(Order.TableSchema.SchemaName, Order.Columns.TotalPrice, Order.Columns.TotalPrice);
            q.LimitRows(1);
            Int64 BidId = 0;
            TotalPrice = 0;
            using (DataReaderBase reader = q.ExecuteReader())
            {
                if (reader.Read())
                {
                    TotalPrice = (reader[Order.Columns.TotalPrice] != null ? (decimal)reader[Order.Columns.TotalPrice] : 0);
                    BidId = (reader[Order.Columns.BidId] != null ? (Int64)reader[Order.Columns.BidId] : 0);
                }

            }
            return GetProductsByBid(BidId);
        }

        public static List<BidProductUI> GetProductsByBid(Int64 BidId)
        {
            Query qry = new Query(BidProduct.TableSchema);
            qry.Join(JoinType.InnerJoin, BidProduct.TableSchema, BidProduct.Columns.ProductId, BidProduct.TableSchema.SchemaName,
                Product.TableSchema, Product.Columns.ProductId, Product.TableSchema.SchemaName);
            qry.SelectAllTableColumns();
            // qry.Join(JoinType.InnerJoin, BidProduct.TableSchema, BidProduct.Columns.BidId, BidProduct.TableSchema.SchemaName,
            //     Offer.TableSchema, Offer.Columns.BidId, Offer.TableSchema.SchemaName);
            // qry.Join(JoinType.InnerJoin, Offer.TableSchema, Offer.Columns.SupplierId, Offer.TableSchema.SchemaName,
            //    SupplierProduct.TableSchema, SupplierProduct.Columns.SupplierId, SupplierProduct.TableSchema.SchemaName);
            qry.AddSelect(Product.TableSchema.SchemaName, Product.Columns.RecomendedPrice, Product.Columns.RecomendedPrice);
            qry.AddSelect(Product.TableSchema.SchemaName, Product.Columns.ProductName, Product.Columns.ProductName);
            qry.AddSelect(Product.TableSchema.SchemaName, Product.Columns.Amount, Product.Columns.Amount);
            qry.AddSelect(Product.TableSchema.SchemaName, Product.Columns.ProductImage, Product.Columns.ProductImage);
            qry.AddSelect(Product.TableSchema.SchemaName, Product.Columns.ProductCode, Product.Columns.ProductCode);
            qry.Where(BidProduct.TableSchema.SchemaName, BidProduct.Columns.BidId, WhereComparision.EqualsTo, BidId);
            qry.GroupBy(BidProduct.TableSchema.SchemaName, BidProduct.Columns.ProductId);

            List<BidProductUI> lstProduct = new List<BidProductUI>();
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                while (reader.Read())
                {
                    lstProduct.Add(new BidProductUI
                    {
                        Amount = (reader[BidProduct.Columns.Amount] != null ? (int)reader[BidProduct.Columns.Amount] : 1),
                        Price = (reader[BidProduct.Columns.Price] != null ? (decimal)reader[BidProduct.Columns.Price] : 1),
                        ProductId = (reader[Product.Columns.ProductId] != null ? (Int64)reader[Product.Columns.ProductId] : 0),
                        ProductName = (reader[Product.Columns.ProductName] != null ? reader[Product.Columns.ProductName].ToString() : ""),
                        ProductAmount = (reader[Product.Columns.Amount] != null ? reader[Product.Columns.Amount].ToString() : ""),
                        ProductImage = (reader[Product.Columns.ProductImage] != null ? reader[Product.Columns.ProductImage].ToString() : ""),
                        ProductCode = (reader[Product.Columns.ProductCode] != null ? reader[Product.Columns.ProductCode].ToString() : ""),
                        RecomendedPrice = reader[Product.Columns.RecomendedPrice] != null ? Convert.ToInt32(reader[Product.Columns.RecomendedPrice]) : 0
                    });
                }
            }

            return lstProduct;


        }

        public static List<BidProductUI> GetProductsByBid(Int64 BidId, Int64 SupplierId)
        {
            Query qry = new Query(BidProduct.TableSchema);
            qry.Join(JoinType.InnerJoin, BidProduct.TableSchema, BidProduct.Columns.ProductId, BidProduct.TableSchema.SchemaName,
                Product.TableSchema, Product.Columns.ProductId, Product.TableSchema.SchemaName);
            qry.Join(JoinType.LeftJoin, Product.TableSchema, Product.Columns.ProductId, Product.TableSchema.SchemaName,
                SupplierProduct.TableSchema, SupplierProduct.Columns.ProductId, SupplierProduct.TableSchema.SchemaName);
            qry.SelectAllTableColumns();
            qry.AddSelect(Product.TableSchema.SchemaName, Product.Columns.ProductName, Product.Columns.ProductName);
            qry.AddSelect(Product.TableSchema.SchemaName, Product.Columns.ProductImage, Product.Columns.ProductImage);
            qry.AddSelect(Product.TableSchema.SchemaName, Product.Columns.ProductCode, Product.Columns.ProductCode);
            //qry.AddSelect(SupplierProduct.TableSchema.SchemaName, SupplierProduct.Columns.Price, SupplierProduct.Columns.Price);
            qry.AddSelect(BidProduct.TableSchema.SchemaName, BidProduct.Columns.Price, BidProduct.Columns.Price);
            qry.AddSelect(SupplierProduct.TableSchema.SchemaName, SupplierProduct.Columns.Gift, SupplierProduct.Columns.Gift);
            qry.Where(BidProduct.TableSchema.SchemaName, BidProduct.Columns.BidId, WhereComparision.EqualsTo, BidId);
            //qry.AddWhere(SupplierProduct.TableSchema.SchemaName, SupplierProduct.Columns.SupplierId, WhereComparision.EqualsTo, SupplierId);
            qry.GroupBy(BidProduct.TableSchema.SchemaName, BidProduct.Columns.ProductId);
            qry.Select(BidProduct.TableSchema.SchemaName, BidProduct.Columns.Amount, BidProduct.Columns.Amount, false);
            qry.AddSelect(Product.TableSchema.SchemaName, Product.Columns.Amount, "ProductAmount");
            List<BidProductUI> lstProduct = new List<BidProductUI>();
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                while (reader.Read())
                {
                    lstProduct.Add(new BidProductUI
                    {
                        Amount = (reader[BidProduct.Columns.Amount] != null ? (int)reader[BidProduct.Columns.Amount] : 1),
                        ProductId = (reader[Product.Columns.ProductId] != null ? (Int64)reader[Product.Columns.ProductId] : 0),
                        ProductName = (reader[Product.Columns.ProductName] != null ? reader[Product.Columns.ProductName].ToString() : ""),
                        Price = (reader[BidProduct.Columns.Price] != null ? (decimal)(reader[BidProduct.Columns.Price]) : 0),
                        ProductAmount = (reader["ProductAmount"] != null ? reader["ProductAmount"].ToString() : ""),
                        ProductGift = (reader[SupplierProduct.Columns.Gift] != null ? reader[SupplierProduct.Columns.Gift].ToString() : ""),
                        ProductImage = (reader[Product.Columns.ProductImage] != null ? reader[Product.Columns.ProductImage].ToString() : ""),
                        ProductCode = (reader[Product.Columns.ProductCode] != null ? reader[Product.Columns.ProductCode].ToString() : "")
                    });
                }
            }

            return lstProduct;


        }

        public static Bid GetActiveBidByAppOrTempUserId(Int64 AppUserId, Int64 TempAppUserId)
        {
            Query qry = new Query(Bid.TableSchema);
            if (AppUserId != 0) qry.Where(Bid.Columns.AppUserId, AppUserId);
            if (TempAppUserId != 0) qry.Where(Bid.Columns.TempAppUserId, TempAppUserId);
            qry.AddWhere(Bid.Columns.EndDate, WhereComparision.GreaterThanOrEqual, DateTime.UtcNow);
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                if (reader.Read())
                {
                    Bid item = new Bid();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }

        public static Int64 CreateBidProduct(Int64 AppUserId, Int64 supplierId, Dictionary<Int64, int> LstProduct, bool isActive, out string gifts)
        {
            Bid NewBid = new Bid();
            gifts = "";
            NewBid.AppUserId = AppUserId;
            long CityId = AppUser.FetchByID(AppUserId).CityId;
            NewBid.CityId = CityId;
            NewBid.IsActive = isActive;
            NewBid.StartDate = DateTime.UtcNow;
            NewBid.EndDate = DateTime.UtcNow.AddHours(Convert.ToDouble(Settings.GetSettingDecimal(Settings.Keys.END_BID_TIME_MIN, 15)));
            
            using (ConnectorBase conn = ConnectorBase.NewInstance())
            {
                NewBid.Save(conn);
            }

            foreach (KeyValuePair<Int64, int> item in LstProduct)
            {
                var supplierProduct = SupplierProduct.FetchByID(supplierId, item.Key);
                BidProduct bidProduct = new BidProduct();
                bidProduct.BidId = NewBid.BidId;
                bidProduct.ProductId = item.Key;
                bidProduct.Amount = item.Value;
                bidProduct.Price = supplierProduct.Price;
                gifts += supplierProduct.Gift;
                bidProduct.Save();
            }

            return NewBid.BidId;
        }
        public static void SendAutoBidsToSuppliers(Int64 BidId, Dictionary<Int64, int> LstProduct, Int64 CityId)
        {

            Query innerQuery = new Query(SupplierProduct.TableSchema);
            innerQuery.Where(SupplierProduct.Columns.ProductId, WhereComparision.In, LstProduct.Select(r => r.Key).ToList());
            innerQuery.AddWhere(SupplierProduct.Columns.Price, WhereComparision.GreaterThan, 0);
            innerQuery.Select(SupplierProduct.Columns.SupplierId).GroupBy(SupplierProduct.Columns.SupplierId);
            innerQuery.AddSelectLiteral(" COUNT(" + SupplierProduct.Columns.SupplierId + ") as `suppliercount`");
            int count = LstProduct.Count;
            List<Int64> Suppliers = new List<Int64>();
            using (DataReaderBase reader = innerQuery.ExecuteReader())
            {
                while (reader.Read())
                {
                    if (Convert.ToInt32(reader["suppliercount"]) == count)
                    {
                        Suppliers.Add(Convert.ToInt64(reader[SupplierProduct.Columns.SupplierId]));
                    }
                }
            }

            Query CityInnerQuery = new Query(SupplierCity.TableSchema);
            CityInnerQuery.Where(SupplierCity.Columns.CityId, WhereComparision.EqualsTo, CityId);
            CityInnerQuery.Select(SupplierCity.Columns.SupplierId).Distinct();

            if (Suppliers.Count == 0 || CityInnerQuery.ExecuteScalarList<Int64>().Count() == 0)
                return;

            Query qry = new Query(AppSupplier.TableSchema);
            qry.SelectAllTableColumns();
            qry.Join(JoinType.InnerJoin, SupplierProduct.TableSchema, SupplierProduct.TableSchema.SchemaName,
                new JoinColumnPair(AppSupplier.TableSchema.SchemaName, AppSupplier.Columns.SupplierId, SupplierProduct.Columns.SupplierId));

            qry.AddSelect(SupplierProduct.TableSchema.SchemaName, SupplierProduct.Columns.ProductId, SupplierProduct.Columns.ProductId);
            qry.Where(AppSupplier.TableSchema.SchemaName, AppSupplier.Columns.SupplierId, WhereComparision.In, Suppliers);
            qry.AddWhere(AppSupplier.TableSchema.SchemaName, AppSupplier.Columns.SupplierId, WhereComparision.In, CityInnerQuery);
            qry.AddWhere(SupplierProduct.TableSchema.SchemaName, SupplierProduct.Columns.ProductId, WhereComparision.In, LstProduct.Select(r => r.Key).ToList());
            qry.AddWhere(AppSupplier.Columns.StatusJoinBid, WhereComparision.EqualsTo, true);
            qry.AddWhere(AppSupplier.Columns.Status, WhereComparision.EqualsTo, true);
            //qry.AddWhere(AppSupplier.Columns.MaxWinningsNum, WhereComparision.GreaterThan, 0);
            // qry.Select(AppSupplier.TableSchema.SchemaName, AppSupplier.Columns.SupplierId, AppSupplier.Columns.SupplierId, true);
            qry.AddSelect(AppSupplier.TableSchema.SchemaName, AppSupplier.Columns.SupplierId, AppSupplier.Columns.SupplierId);
            //SELECT GROUP_CONCAT(case Gift when '' then null else Gift end SEPARATOR ', ') FROM snoopi.supplierproduct;
            qry.AddSelectLiteral(" GROUP_CONCAT(case " + SupplierProduct.TableSchema.SchemaName + "." + SupplierProduct.Columns.Gift +
                " when '' then null else " + SupplierProduct.TableSchema.SchemaName + "." + SupplierProduct.Columns.Gift + " end SEPARATOR ', '" + ") as TotalGift");
            qry.GroupBy(AppSupplier.TableSchema.SchemaName, AppSupplier.Columns.SupplierId);

            using (DataReaderBase reader = qry.ExecuteReader())
            {
                while (reader.Read())
                {
                    Int64 SupplierId = reader[AppSupplier.Columns.SupplierId] != null ? Convert.ToInt64(reader[AppSupplier.Columns.SupplierId]) : 0;
                    AppSupplier supplier = AppSupplier.FetchByID(SupplierId);
                    if (supplier != null)
                    {
                        //if (supplier.MaxWinningsNum == 0)
                        //{
                        //    SupplierNotification.SendNotificationMaxAutoModeMessage(SupplierId);
                        //    supplier.StatusJoinBid = false;
                        //    supplier.Save();
                        //}
                        //else
                        //{

                        Offer offer = Offer.FetchByBidIdAndSupplierId(BidId, SupplierId);
                        if (offer == null)
                        {
                            Query q = new Query(BidProduct.TableSchema);
                            q.Join(JoinType.LeftJoin, SupplierProduct.TableSchema, SupplierProduct.TableSchema.SchemaName,
                                new JoinColumnPair(BidProduct.TableSchema.SchemaName, BidProduct.Columns.ProductId, SupplierProduct.Columns.ProductId));
                            q.Where(BidProduct.Columns.BidId, BidId);
                            q.AddWhere(SupplierProduct.Columns.SupplierId, SupplierId);
                            q.SelectLiteral(" sum(" + BidProduct.Columns.Amount + "*" + SupplierProduct.Columns.Price + ")");
                            object price = q.ExecuteScalar();
                            if (price != null && Convert.ToDecimal(price) > 0)
                            {
                                //supplier.MaxWinningsNum = (supplier.MaxWinningsNum > 0 ? supplier.MaxWinningsNum - 1 : 0);
                                //if (supplier.MaxWinningsNum == 0)
                                //{
                                //    SupplierNotification.SendNotificationMaxAutoModeMessage(SupplierId);
                                //    supplier.StatusJoinBid = false;
                                //}
                                //supplier.Save();
                                offer = new Offer();
                                offer.BidId = BidId;
                                offer.CreateDate = DateTime.UtcNow;
                                offer.Gift = reader["TotalGift"] != null ? (reader["TotalGift"]).ToString() : "";
                                offer.Price = price != null ? Convert.ToDecimal(price) : 0;
                                offer.SupplierId = SupplierId;
                                offer.Save();
                            }
                            // }
                        }
                    }
                }
            }

        }

        public static void SendPushToSuppliers(Int64 BidId, List<Int64> LstProducts, Int64 CityId)
        {

            Query innerQuery = new Query(SupplierProduct.TableSchema);
            innerQuery.Where(SupplierProduct.Columns.ProductId, WhereComparision.In, LstProducts);
            innerQuery.Select(SupplierProduct.Columns.SupplierId).GroupBy(SupplierProduct.Columns.SupplierId);
            innerQuery.AddSelectLiteral(" COUNT(" + SupplierProduct.Columns.SupplierId + ") as `suppliercount`");
            int count = LstProducts.Count;
            List<Int64> Suppliers = new List<Int64>();
            using (DataReaderBase reader = innerQuery.ExecuteReader())
            {
                while (reader.Read())
                {
                    if (Convert.ToInt32(reader["suppliercount"]) == count)
                    {
                        Suppliers.Add(Convert.ToInt64(reader[SupplierProduct.Columns.SupplierId]));
                    }
                }
            }

            Query CityInnerQuery = new Query(SupplierCity.TableSchema);
            CityInnerQuery.Where(SupplierCity.Columns.CityId, WhereComparision.EqualsTo, CityId);
            CityInnerQuery.Select(SupplierCity.Columns.SupplierId).Distinct();

            if (Suppliers.Count > 0 && CityInnerQuery.ExecuteScalarList<Int64>().Count() > 0)
            {

                Query qry = new Query(AppSupplier.TableSchema);
                qry.Where(AppSupplier.Columns.SupplierId, WhereComparision.In, Suppliers);
                qry.AddWhere(AppSupplier.Columns.SupplierId, WhereComparision.In, CityInnerQuery);
                //qry.AddWhere(AppSupplier.Columns.StatusJoinBid, WhereComparision.EqualsTo, false);
                //qry.AddWhere(AppSupplier.Columns.Status, WhereComparision.EqualsTo, true);
                try
                {
                    SupplierNotification.SendNotificationNewBidToSupplier(BidId, qry.ExecuteScalarList<Int64>());
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                }
            }


        }

        public static Dictionary<string, string> GetDiscount(Int64 OfferId, Int64 AppUserId)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            Query q = new Query(Campaign.TableSchema);
            q.Where(Campaign.Columns.EndDate, WhereComparision.GreaterThanOrEqual, DateTime.UtcNow);
            q.AddWhere(Campaign.Columns.StartDate, WhereComparision.LessThanOrEqual, DateTime.UtcNow);

            CampaignCollection cc = CampaignCollection.FetchByQuery(q);

            Offer offer = Offer.FetchByID(OfferId);

            result["TotalPrice"] = offer.Price.ToString();
            result["PriceAfterDiscount"] = offer.Price.ToString();
            result["GiftContent"] = "";
            result["PrecentDiscount"] = "0";
            result["CampaignId"] = "0";
            bool Entitled = false;

            if (cc != null && cc.Count != 0 && cc.First() != null)
            {
                Campaign c = cc.FirstOrDefault();
                Int64 ImplemationCount = AppUserCampaign.FetchByCampaignIdAppUserId(AppUserId, c.CampaignId);

                Query qry = new Query(Order.TableSchema);

                qry.Where(Order.TableSchema.SchemaName, Order.Columns.AppUserId, WhereComparision.EqualsTo, AppUserId);
                //qry.AddWhere(Order.TableSchema.SchemaName, Order.Columns.TransactionResponseCode, WhereComparision.EqualsTo, OrderController.RESPONSE_CODE_OK);

                if (c.DestinationCount != 0)
                {
                    Int64 count = qry.GetCount(Order.TableSchema.SchemaName, Order.Columns.OrderId);
                    if ((count - (ImplemationCount * c.DestinationCount)) >= c.DestinationCount && c.ImplemationCount > ImplemationCount)
                    {
                        Entitled = true;
                    }
                }
                else if (c.DestinationSum != 0)
                {
                    object sum = qry.GetSum(Offer.TableSchema.SchemaName, Offer.Columns.Price);
                    decimal dsum = sum != null ? Convert.ToDecimal(sum) : 0;
                    if ((dsum - (ImplemationCount * c.DestinationSum)) >= c.DestinationSum && c.ImplemationCount > ImplemationCount)
                    {
                        Entitled = true;
                    }
                }

                if (c.IsDiscount == true && Entitled)
                {
                    result["PriceAfterDiscount"] = (offer.Price * (1 - ((decimal)c.PrecentDiscount / 100))).ToString();
                    result["PrecentDiscount"] = (c.PrecentDiscount).ToString();
                }
                else if (c.IsGift == true && Entitled)
                {
                    result["GiftContent"] = c.CampaignName;
                    EmailMessagingService.SendGiftToAdmin(AppUserId, c.CampaignId, c.CampaignName);
                }

                if (Entitled)
                {
                    result["CampaignId"] = c.CampaignId.ToString();
                }


            }
            return result;
        }

        public static Dictionary<string, string> GetDiscountForUser(decimal price, Int64 AppUserId)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            Query q = new Query(Campaign.TableSchema);
            q.Where(Campaign.Columns.EndDate, WhereComparision.GreaterThanOrEqual, DateTime.UtcNow);
            q.AddWhere(Campaign.Columns.StartDate, WhereComparision.LessThanOrEqual, DateTime.UtcNow);

            CampaignCollection cc = CampaignCollection.FetchByQuery(q);

            result["TotalPrice"] = price.ToString();
            result["PriceAfterDiscount"] = price.ToString();
            result["GiftContent"] = "";
            result["PrecentDiscount"] = "0";
            result["CampaignId"] = "0";
            bool Entitled = false;

            if (cc != null && cc.Count != 0 && cc.First() != null)
            {
                Campaign c = cc.FirstOrDefault();
                Int64 ImplemationCount = AppUserCampaign.FetchByCampaignIdAppUserId(AppUserId, c.CampaignId);

                Query qry = new Query(Order.TableSchema);

                qry.Where(Order.TableSchema.SchemaName, Order.Columns.AppUserId, WhereComparision.EqualsTo, AppUserId);
                //qry.AddWhere(Order.TableSchema.SchemaName, Order.Columns.TransactionResponseCode, WhereComparision.EqualsTo, OrderController.RESPONSE_CODE_OK);

                if (c.DestinationCount != 0)
                {
                    Int64 count = qry.GetCount(Order.TableSchema.SchemaName, Order.Columns.OrderId);
                    if ((count - (ImplemationCount * c.DestinationCount)) >= c.DestinationCount && c.ImplemationCount > ImplemationCount)
                    {
                        Entitled = true;
                    }
                }
                else if (c.DestinationSum != 0)
                {
                    object sum = price;
                    decimal dsum = sum != null ? Convert.ToDecimal(sum) : 0;
                    if ((dsum - (ImplemationCount * c.DestinationSum)) >= c.DestinationSum && c.ImplemationCount > ImplemationCount)
                    {
                        Entitled = true;
                    }
                }

                if (c.IsDiscount == true && Entitled)
                {
                    result["PriceAfterDiscount"] = (price * (1 - ((decimal)c.PrecentDiscount / 100))).ToString();
                    result["PrecentDiscount"] = (c.PrecentDiscount).ToString();
                }
                else if (c.IsGift == true && Entitled)
                {
                    result["GiftContent"] = c.CampaignName;
                    EmailMessagingService.SendGiftToAdmin(AppUserId, c.CampaignId, c.CampaignName);
                }

                if (Entitled)
                {
                    result["CampaignId"] = c.CampaignId.ToString();
                    AppUserCampaign appUserCampaign = new AppUserCampaign();
                    appUserCampaign.AppUserId = AppUserId;
                    appUserCampaign.CampaignId = c.CampaignId;
                    appUserCampaign.Save();

                }


            }
            return result;
        }


        private static Query get_bids_query(long supplierId)
        {
            var query = new Query(BidMessage.TableSchema);
            query.Where(BidMessage.Columns.SupplierId, supplierId);
            return query;
        }

        private static Query get_orders_query(long supplierId)
        {
            var query = new Query(Order.TableSchema);
            query.Join(JoinType.InnerJoin, Order.TableSchema, Order.Columns.BidId, Order.TableSchema.SchemaName, BidMessage.TableSchema, BidMessage.Columns.BidId, BidMessage.TableSchema.SchemaName);
            return query;
        }

        public static List<BidUI> GetSupplierBids(Int64 SupplierId, string Action, DateTime? FromDate, DateTime? ToDate, Int64 BidId, int PageSize = 0, int CurrentPageIndex = 0)
        {
            List<BidUI> lstBids = new List<BidUI>();

            Query q = null;

            switch (Action)
            {
                case "Offers":
                    q = get_bids_query(SupplierId);
                    break;
                case "Win":
                    q = get_orders_query(SupplierId);
                    q.AddWhere(Order.TableSchema.SchemaName, Order.Columns.SupplierId, WhereComparision.EqualsTo, SupplierId);
                    break;
                case "NoWin":
                    q = get_orders_query(SupplierId);
                    q.AddWhere(Order.TableSchema.SchemaName, Order.Columns.SupplierId, WhereComparision.NotEqualsTo, SupplierId);
                    q.AddWhere(BidMessage.Columns.IsActive, false);
                    break;
                case "Active":
                    q = get_bids_query(SupplierId);
                    q.AddWhere(BidMessage.Columns.IsActive, false);
                    break;
                default:
                    break;

            }
            if (q == null)
                return new List<BidUI>();

            //if (SupplierId != -1)
            //{
            //    q.Where(Order.TableSchema.SchemaName, Order.Columns.SupplierId, WhereComparision.EqualsTo, SupplierId);
            //}
            //q.AddSelect(Bid.TableSchema.SchemaName, Bid.Columns.BidId, Bid.Columns.BidId);


            q.SelectAllTableColumns();
            q.AddSelect(BidMessage.TableSchema.SchemaName, BidMessage.Columns.SendingTime, BidMessage.Columns.SendingTime);
            q.AddSelect(BidMessage.TableSchema.SchemaName, BidMessage.Columns.ExpirationTime, BidMessage.Columns.ExpirationTime);

            // q.AddSelect("O", Order.Columns.CreateDate, "CD");
            //q.AddWhere("O", Order.Columns.OrderId, WhereComparision.EqualsTo, null);

            if (FromDate != null) q.AddWhere(BidMessage.TableSchema.SchemaName, BidMessage.Columns.ExpirationTime, WhereComparision.GreaterThanOrEqual, ((DateTime)FromDate).Date);
            if (ToDate != null) q.AddWhere(BidMessage.TableSchema.SchemaName, BidMessage.Columns.ExpirationTime, WhereComparision.LessThanOrEqual, ((DateTime)ToDate).AddHours(24).Date);
            if (BidId != 0) q.AddWhere(BidMessage.TableSchema.SchemaName, BidMessage.Columns.BidId, WhereComparision.EqualsTo, BidId);
            if (PageSize > 0)
            {
                q.LimitRows(PageSize).OffsetRows(PageSize * CurrentPageIndex);
            }
            q.GroupBy(BidMessage.TableSchema.SchemaName, BidMessage.Columns.BidId);
            q.OrderBy(BidMessage.TableSchema.SchemaName, BidMessage.Columns.ExpirationTime, SortDirection.DESC);
            using (DataReaderBase reader = q.ExecuteReader())
            {
                while (reader.Read())
                {
                    BidUI bid = new BidUI();

                    bid.BidId = Convert.ToInt64(reader["BidId"]);
                    bid.StartDate = reader[BidMessage.Columns.SendingTime] is DBNull ? (DateTime?)null : Convert.ToDateTime(reader[BidMessage.Columns.SendingTime]).ToLocalTime();
                    bid.EndDate = reader[BidMessage.Columns.ExpirationTime] is DBNull ? (DateTime?)null : Convert.ToDateTime(reader[BidMessage.Columns.ExpirationTime]).ToLocalTime();
                    bid.LstProduct = GetProductsByBid(bid.BidId);
                    bid.Price = bid.LstProduct.Sum(p => p.Amount * p.Price);

                    if (Action == "Win")
                        bid.OrderDate = reader[Order.Columns.CreateDate] is DBNull ? (DateTime?)null : Convert.ToDateTime(reader[Order.Columns.CreateDate]).ToLocalTime();
                    lstBids.Add(bid);
                }
            }

            return lstBids;

        }


        public static void UpdateTempAppUserBidsToUserBid(Int64 TempUppUserId, Int64 AppUserId)
        {
            Query.New<Bid>().Where(Bid.Columns.TempAppUserId, TempUppUserId)
                .Update(Bid.Columns.AppUserId, AppUserId)
                .Update(Bid.Columns.TempAppUserId, null)
                .Execute();

            Query.New<Bid>().Where(BidService.Columns.TempAppUserId, TempUppUserId)
                .Update(BidService.Columns.AppUserId, AppUserId)
                .Update(BidService.Columns.TempAppUserId, null)
                .Execute();

        }

    }

}

