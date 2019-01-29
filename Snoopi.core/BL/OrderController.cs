using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Snoopi.core.DAL;
using dg.Sql;
using dg.Sql.Connector;
using System.IO;

namespace Snoopi.core.BL
{

    public class OrderUI
    {
        public Int64 OrderId { get; set; }
        public Int64 SupplierId { get; set; }
        public string SupplierName { get; set; }
        public string CustomerName { get { return FirstName + " " + LastName; } }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string City { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? ReceviedDate { get; set; }
        public DateTime? SuppliedDate { get; set; }
        public Int64 BidId { get; set; }
        //public DateTime? BidStartDate { get; set; }
        public DateTime? BidEndDate { get; set; }
        public decimal DonationPrice { get; set; }
        public Int64 OfferId { get; set; }
        public decimal OfferPrice { get; set; }
        public bool IsSupplied { get; set; }
        public bool IsSendReceived { get; set; }
        public decimal Price { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal PriceAfterDiscount { get; set; }
        public int PrecentDiscount { get; set; }
        public int Precent { get; set; }
        public decimal PaymentForSupplier { get; set; }
        public string Gift { get; set; }
        public List<BidProductUI> LstProduct { get; set; }
        public AppUser user { get; set; }
        public string TransactionResponseCode { get; set; }
        public OrderStatus TransactionStatus { get; set; }
        public PaymentStatus PaySupplierStatus { get; set; }
        public bool IsPayed { get; set; }
        public string SpecialInstructions { get; set; }
        public string CampaignName { get; set; }
        public string Remarks { get; set; }
    }

    public class OrderController
    {
        public const string RESPONSE_CODE_OK = "000";
        public const string RESPONSE_CODE_ERROR = "004";
        public static List<OrderUI> GetOrderHistory(Int64 AppUserId)
        {
            Query q = new Query(Order.TableSchema);

            q.Join(JoinType.InnerJoin, Order.TableSchema, Order.Columns.SupplierId, Order.TableSchema.SchemaName,
                    AppSupplier.TableSchema, AppSupplier.Columns.SupplierId, AppSupplier.TableSchema.SchemaName);
            q.OrderBy(Order.TableSchema.SchemaName, Order.Columns.CreateDate, SortDirection.DESC);
            q.Select(Order.TableSchema.SchemaName, Order.Columns.OrderId, Order.Columns.OrderId, true);
            q.AddSelect(Order.TableSchema.SchemaName, Order.Columns.BidId, Order.Columns.BidId);
            q.AddSelect(AppSupplier.TableSchema.SchemaName, AppSupplier.Columns.BusinessName, AppSupplier.Columns.BusinessName);
            q.AddSelect(AppSupplier.TableSchema.SchemaName, AppSupplier.Columns.SupplierId, AppSupplier.Columns.SupplierId);
            q.AddSelect(Order.TableSchema.SchemaName, Order.Columns.CreateDate, Order.Columns.CreateDate);
            q.AddSelect(Order.TableSchema.SchemaName, Order.Columns.TotalPrice, Order.Columns.TotalPrice);
            q.Where(Order.Columns.AppUserId, AppUserId);
            q.AddWhere(Order.TableSchema.SchemaName, Order.Columns.UserPaySupplierStatus, WhereComparision.EqualsTo, UserPaymentStatus.Payed);
            //   Query q= new Query(Order.TableSchema);
            //   q.Join(JoinType.InnerJoin,)
            List<OrderUI> LstOrder = new List<OrderUI>();
            using (DataReaderBase reader = q.ExecuteReader())
            {
                while (reader.Read())
                {
                    OrderUI orderUI = new OrderUI();
                    orderUI.SupplierName = (reader[AppSupplier.Columns.BusinessName] != null ? reader[AppSupplier.Columns.BusinessName].ToString() : "");
                    orderUI.SupplierId = (reader[AppSupplier.Columns.SupplierId] != null ? Int64.Parse(reader[AppSupplier.Columns.SupplierId].ToString()) : 0);
                    orderUI.OrderId = (reader[Order.Columns.OrderId] != null ? Int64.Parse(reader[Order.Columns.OrderId].ToString()) : 0);
                    orderUI.OrderDate = (reader[Order.Columns.CreateDate] != null ? DateTime.Parse(reader[Order.Columns.CreateDate].ToString()).ToLocalTime() : DateTime.MinValue);
                    orderUI.BidId = (reader[Order.Columns.BidId] != null ? Int64.Parse(reader[Order.Columns.BidId].ToString()) : 0);
                    orderUI.TotalPrice = (reader[Order.Columns.TotalPrice] != null ? decimal.Parse(reader[Order.Columns.TotalPrice].ToString()) : 0);
                    LstOrder.Add(orderUI);
                }

            }
            return LstOrder;
        }

        public static List<OrderUI> GetOrderSupplierHistoryExcel(Int64 SupplierId, DateTime? StartDate = null, DateTime? EndDate = null)
        {
            Query q = new Query(Order.TableSchema);

            q.Join(JoinType.InnerJoin, Order.TableSchema, Order.Columns.SupplierId, Order.TableSchema.SchemaName,
                    AppSupplier.TableSchema, AppSupplier.Columns.SupplierId, AppSupplier.TableSchema.SchemaName);
            q.OrderBy(Order.TableSchema.SchemaName, Order.Columns.CreateDate, SortDirection.DESC);
            q.Select(Order.TableSchema.SchemaName, Order.Columns.OrderId, Order.Columns.OrderId, true);
            q.AddSelect(AppSupplier.TableSchema.SchemaName, AppSupplier.Columns.BusinessName, AppSupplier.Columns.BusinessName);
            q.AddSelect(AppSupplier.TableSchema.SchemaName, AppSupplier.Columns.SupplierId, AppSupplier.Columns.SupplierId);
            q.AddSelect(Order.TableSchema.SchemaName, Order.Columns.CreateDate, Order.Columns.CreateDate);
            q.AddSelect(Order.TableSchema.SchemaName, Order.Columns.BidId, Order.Columns.BidId);
            q.AddSelect(Order.TableSchema.SchemaName, Order.Columns.TotalPrice, Order.Columns.TotalPrice);
            q.AddSelect(Order.TableSchema.SchemaName, Order.Columns.TransactionResponseCode, Order.Columns.TransactionResponseCode);
            q.Where(AppSupplier.TableSchema.SchemaName, AppSupplier.Columns.SupplierId, WhereComparision.EqualsTo, SupplierId);
            if (StartDate != null && EndDate != null)
            {
                q.AddWhere(Order.TableSchema.SchemaName, Order.Columns.CreateDate, WhereComparision.GreaterThanOrEqual, StartDate);
                q.AddWhere(Order.TableSchema.SchemaName, Order.Columns.CreateDate, WhereComparision.LessThanOrEqual, EndDate.Value.AddDays(1));
            }
            List<OrderUI> LstOrder = new List<OrderUI>();
            using (DataReaderBase reader = q.ExecuteReader())
            {
                while (reader.Read())
                {
                    OrderUI orderUI = new OrderUI();
                    orderUI.SupplierName = (reader[AppSupplier.Columns.BusinessName] != null ? reader[AppSupplier.Columns.BusinessName].ToString() : "");
                    orderUI.SupplierId = (reader[AppSupplier.Columns.SupplierId] != null ? Int64.Parse(reader[AppSupplier.Columns.SupplierId].ToString()) : 0);
                    orderUI.OrderId = (reader[Order.Columns.OrderId] != null ? Int64.Parse(reader[Order.Columns.OrderId].ToString()) : 0);
                    orderUI.OrderDate = (reader[Order.Columns.CreateDate] != null ? DateTime.Parse(reader[Order.Columns.CreateDate].ToString()).ToLocalTime() : DateTime.MinValue);
                    orderUI.BidId = (reader[Order.Columns.BidId] != null ? Int64.Parse(reader[Order.Columns.BidId].ToString()) : 0);
                    orderUI.Price = (reader[Order.Columns.TotalPrice] != null ? Int64.Parse(reader[Order.Columns.TotalPrice].ToString()) : 0);
                    orderUI.LstProduct = BidController.GetProductsByBid(orderUI.BidId);
                    LstOrder.Add(orderUI);
                }

            }
            return LstOrder;
        }

        public static List<OrderUI> GetOrderSupplierHistory(Int64 SupplierId, DateTime? StartDate = null, DateTime? EndDate = null)
        {
            Query q = new Query(Order.TableSchema);

            q.Join(JoinType.InnerJoin, Order.TableSchema, Order.Columns.SupplierId, Order.TableSchema.SchemaName,
                    AppSupplier.TableSchema, AppSupplier.Columns.SupplierId, AppSupplier.TableSchema.SchemaName);
            q.OrderBy(Order.TableSchema.SchemaName, Order.Columns.CreateDate, SortDirection.DESC);
            q.Select(Order.TableSchema.SchemaName, Order.Columns.OrderId, Order.Columns.OrderId, true);
            q.AddSelect(AppSupplier.TableSchema.SchemaName, AppSupplier.Columns.BusinessName, AppSupplier.Columns.BusinessName);
            q.AddSelect(AppSupplier.TableSchema.SchemaName, AppSupplier.Columns.SupplierId, AppSupplier.Columns.SupplierId);
            q.AddSelect(Order.TableSchema.SchemaName, Order.Columns.CreateDate, Order.Columns.CreateDate);
            q.AddSelect(Order.TableSchema.SchemaName, Order.Columns.BidId, Order.Columns.BidId);
            q.AddSelect(Order.TableSchema.SchemaName, Order.Columns.TotalPrice, Order.Columns.TotalPrice);
            q.AddSelect(Order.TableSchema.SchemaName, Order.Columns.ReceivedDate, Order.Columns.ReceivedDate);
            //  q.AddSelect(Order.TableSchema.SchemaName, Order.Columns.TransactionResponseCode, Order.Columns.TransactionResponseCode);
            // q.AddSelect(Order.TableSchema.SchemaName, Order.Columns.UserPaySupplierStatus, Order.Columns.UserPaySupplierStatus);
            q.Where(Order.TableSchema.SchemaName, Order.Columns.SupplierId, WhereComparision.EqualsTo, SupplierId);
            q.AddWhere(Order.TableSchema.SchemaName, Order.Columns.UserPaySupplierStatus, WhereComparision.EqualsTo, UserPaymentStatus.Payed);
            if (StartDate != null && EndDate != null)
            {
                q.AddWhere(Order.TableSchema.SchemaName, Order.Columns.CreateDate, WhereComparision.GreaterThanOrEqual, StartDate);
                q.AddWhere(Order.TableSchema.SchemaName, Order.Columns.CreateDate, WhereComparision.LessThanOrEqual, EndDate.Value.AddDays(1));
            }
            List<OrderUI> LstOrder = new List<OrderUI>();
            using (DataReaderBase reader = q.ExecuteReader())
            {
                while (reader.Read())
                {
                    OrderUI orderUI = new OrderUI();
                    orderUI.SupplierName = (reader[AppSupplier.Columns.BusinessName] != null ? reader[AppSupplier.Columns.BusinessName].ToString() : "");
                    orderUI.SupplierId = (reader[AppSupplier.Columns.SupplierId] != null ? Int64.Parse(reader[AppSupplier.Columns.SupplierId].ToString()) : 0);
                    orderUI.OrderId = (reader[Order.Columns.OrderId] != null ? Int64.Parse(reader[Order.Columns.OrderId].ToString()) : 0);
                    orderUI.OrderDate = (reader[Order.Columns.CreateDate] != null ? DateTime.Parse(reader[Order.Columns.CreateDate].ToString()).ToLocalTime() : DateTime.MinValue);
                    orderUI.BidId = (reader[Order.Columns.BidId] != null ? Int64.Parse(reader[Order.Columns.BidId].ToString()) : 0);
                    orderUI.Price = (reader[Order.Columns.TotalPrice] != null ? Int64.Parse(reader[Order.Columns.TotalPrice].ToString()) : 0);
                    //  orderUI.IsPayed = (reader[Order.Columns.TransactionResponseCode] != null && reader[Order.Columns.TransactionResponseCode].ToString() == RESPONSE_CODE_OK ? true : false);
                    // orderUI.IsPayed = (reader[Order.Columns.UserPaySupplierStatus] != null && Int64.Parse(reader[Order.Columns.UserPaySupplierStatus].ToString()) == 1 ? true : false);
                    orderUI.IsPayed = (reader[Order.Columns.ReceivedDate] != null && reader[Order.Columns.ReceivedDate].ToString() != String.Empty ? true : false);
                    LstOrder.Add(orderUI);
                }

            }
            return LstOrder;
        }

        public static OrderUI GetOrderById(Int64 OrderId, Int64 SupplierId = 0)
        {
            Query q = new Query(Order.TableSchema);
            // q.Join(JoinType.InnerJoin, Order.TableSchema, Order.Columns.BidId, Order.TableSchema.SchemaName,
            //  Offer.TableSchema, Offer.Columns.BidId, Offer.TableSchema.SchemaName);


            q.Join(JoinType.InnerJoin, Order.TableSchema, Order.Columns.SupplierId, Order.TableSchema.SchemaName,
                    AppSupplier.TableSchema, AppSupplier.Columns.SupplierId, AppSupplier.TableSchema.SchemaName);
            q.Join(JoinType.InnerJoin, Order.TableSchema, Order.Columns.AppUserId, Order.TableSchema.SchemaName,
                    AppUser.TableSchema, AppUser.Columns.AppUserId, AppUser.TableSchema.SchemaName);
            q.Join(JoinType.InnerJoin, AppUser.TableSchema, AppUser.Columns.CityId, AppUser.TableSchema.SchemaName,
                    City.TableSchema, City.Columns.CityId, City.TableSchema.SchemaName);
            q.OrderBy(Order.TableSchema.SchemaName, Order.Columns.CreateDate, SortDirection.DESC);
            q.Select(Order.TableSchema.SchemaName, Order.Columns.OrderId, Order.Columns.OrderId, true);
            q.AddSelect(AppSupplier.TableSchema.SchemaName, AppSupplier.Columns.BusinessName, AppSupplier.Columns.BusinessName);
            q.AddSelect(AppSupplier.TableSchema.SchemaName, AppSupplier.Columns.SupplierId, AppSupplier.Columns.SupplierId);
            q.AddSelect(Order.TableSchema.SchemaName, Order.Columns.CreateDate, Order.Columns.CreateDate);
            q.AddSelect(Order.TableSchema.SchemaName, Order.Columns.TotalPrice, Order.Columns.TotalPrice);
            q.AddSelect(Order.TableSchema.SchemaName, Order.Columns.BidId, Order.Columns.BidId);
            q.AddSelect(Order.TableSchema.SchemaName, Order.Columns.ReceivedDate, Order.Columns.ReceivedDate);
            q.AddSelect(City.TableSchema.SchemaName, City.Columns.CityName, City.Columns.CityName);
            q.AddSelect(Order.TableSchema.SchemaName, Order.Columns.Gifts, Order.Columns.Gifts);
            q.AddSelect(Order.TableSchema.SchemaName, Order.Columns.SuppliedDate, Order.Columns.SuppliedDate);
            q.AddSelect(Order.TableSchema.SchemaName, Order.Columns.AppUserId, Order.Columns.AppUserId);
            q.AddSelect(Order.TableSchema.SchemaName, Order.Columns.SpecialInstructions, Order.Columns.SpecialInstructions);
            q.AddSelect(Order.TableSchema.SchemaName, Order.Columns.IsSendRecived, Order.Columns.IsSendRecived);
            q.Where(Order.TableSchema.SchemaName, Order.Columns.OrderId, WhereComparision.EqualsTo, OrderId);

            if (SupplierId != 0)
            {
                q.AddWhere(AppSupplier.TableSchema.SchemaName, AppSupplier.Columns.SupplierId, WhereComparision.EqualsTo, SupplierId);
            }
            OrderUI orderUI = null;
            using (DataReaderBase reader = q.ExecuteReader())
            {
                if (reader.Read())
                {
                    orderUI = new OrderUI();
                    orderUI.SupplierName = (reader[AppSupplier.Columns.BusinessName] != null ? reader[AppSupplier.Columns.BusinessName].ToString() : "");
                    orderUI.SupplierId = (reader[AppSupplier.Columns.SupplierId] != null ? Int64.Parse(reader[AppSupplier.Columns.SupplierId].ToString()) : 0);
                    orderUI.OrderId = (reader[Order.Columns.OrderId] != null ? Int64.Parse(reader[Order.Columns.OrderId].ToString()) : 0);
                    orderUI.OrderDate = (reader[Order.Columns.CreateDate] != null ? DateTime.Parse(reader[Order.Columns.CreateDate].ToString()).ToLocalTime() : DateTime.MinValue);
                    orderUI.Price = (reader[Order.Columns.TotalPrice] != null ? decimal.Parse(reader[Order.Columns.TotalPrice].ToString()) : 0);
                    orderUI.ReceviedDate = (reader[Order.Columns.ReceivedDate] != null && (reader[Order.Columns.ReceivedDate]).ToString() != "" ? (DateTime?)Convert.ToDateTime(reader[Order.Columns.ReceivedDate]).ToLocalTime() : null);
                    orderUI.LstProduct = BidController.GetProductsByBid(reader[Order.Columns.BidId] != null ? Int64.Parse(reader[Order.Columns.BidId].ToString()) : 0, orderUI.SupplierId);
                    orderUI.City = (reader[City.Columns.CityName] != null ? reader[City.Columns.CityName].ToString() : "");
                    orderUI.user = AppUser.FetchByID(Int64.Parse(reader[Order.Columns.AppUserId].ToString()));
                    orderUI.SuppliedDate = (reader[Order.Columns.SuppliedDate] != null && (reader[Order.Columns.SuppliedDate]).ToString() != "" ? (DateTime?)Convert.ToDateTime(reader[Order.Columns.SuppliedDate]).ToLocalTime() : null);
                    orderUI.Gift = (reader[Order.Columns.Gifts] != null ? reader[Order.Columns.Gifts].ToString() : "");
                    orderUI.SpecialInstructions = (reader[Order.Columns.SpecialInstructions] != null ? reader[Order.Columns.SpecialInstructions].ToString() : "");
                    orderUI.BidId = (reader[Order.Columns.BidId] != null ? Int64.Parse(reader[Order.Columns.BidId].ToString()) : 0);
                    orderUI.IsSendReceived = (reader[Order.Columns.IsSendRecived] != null) ? Convert.ToBoolean(reader[Order.Columns.IsSendRecived]) : false;
                }

            }
            return orderUI;
        }

        public static List<OrderUI> GetAllOrders(DateTime from = new DateTime(), DateTime to = new DateTime(), List<Int64> suppliersIds = null, List<bool> IsSendReceived = null, List<int> StatusIdList = null, List<int> StatusPayementIdList = null, string SearchBid = "", int PageSize = 0, int CurrentPageIndex = 0, Int64 AppUserId = 0)
        {
            Query q = new Query(Order.TableSchema);
            q.Join(JoinType.InnerJoin, Order.TableSchema, Order.Columns.BidId, Order.TableSchema.SchemaName,
                     Bid.TableSchema, Bid.Columns.BidId, Bid.TableSchema.SchemaName);
            q.Join(JoinType.LeftJoin, Order.TableSchema, Order.Columns.SupplierId, Order.TableSchema.SchemaName,
                    AppSupplier.TableSchema, AppSupplier.Columns.SupplierId, AppSupplier.TableSchema.SchemaName);
            q.Join(JoinType.LeftJoin, Order.TableSchema, Order.Columns.DonationId, Order.TableSchema.SchemaName,
                    Donation.TableSchema, Donation.Columns.DonationId, Donation.TableSchema.SchemaName);

            q.OrderBy(Order.TableSchema.SchemaName, Order.Columns.CreateDate, SortDirection.DESC);
            q.Select(Order.TableSchema.SchemaName, Order.Columns.OrderId, Order.Columns.OrderId, true);
            q.AddSelect(AppSupplier.TableSchema.SchemaName, AppSupplier.Columns.BusinessName, AppSupplier.Columns.BusinessName);
            q.AddSelect(AppSupplier.TableSchema.SchemaName, AppSupplier.Columns.SupplierId, AppSupplier.Columns.SupplierId);
            q.AddSelect(AppSupplier.TableSchema.SchemaName, AppSupplier.Columns.Precent, AppSupplier.Columns.Precent);
            q.AddSelect(Order.TableSchema.SchemaName, Order.Columns.CreateDate, Order.Columns.CreateDate);
            q.AddSelect(Bid.TableSchema.SchemaName, Bid.Columns.BidId, Bid.Columns.BidId);
            q.AddSelect(Bid.TableSchema.SchemaName, Bid.Columns.StartDate, Bid.Columns.StartDate);
            q.AddSelect(Bid.TableSchema.SchemaName, Bid.Columns.EndDate, Bid.Columns.EndDate);
            q.AddSelect(Donation.TableSchema.SchemaName, Donation.Columns.DonationPrice, Donation.Columns.DonationPrice);
            q.AddSelect(Order.TableSchema.SchemaName, Order.Columns.TotalPrice, Order.Columns.TotalPrice);
            q.AddSelect(Order.TableSchema.SchemaName, Order.Columns.PriceAfterDiscount, Order.Columns.PriceAfterDiscount);
            q.AddSelect(Order.TableSchema.SchemaName, Order.Columns.PrecentDiscount, Order.Columns.PrecentDiscount);
            q.AddSelect(Order.TableSchema.SchemaName, Order.Columns.IsSendRecived, Order.Columns.IsSendRecived);
            q.AddSelect(Order.TableSchema.SchemaName, Order.Columns.SuppliedDate, Order.Columns.SuppliedDate);
            //q.AddSelect(Order.TableSchema.SchemaName, Order.Columns.TransactionResponseCode, Order.Columns.TransactionResponseCode);
            //q.AddSelect(Order.TableSchema.SchemaName, Order.Columns.TransactionStatus, Order.Columns.TransactionStatus);
            q.AddSelect(Order.TableSchema.SchemaName, Order.Columns.PaySupplierStatus, Order.Columns.PaySupplierStatus);
            q.AddSelect(Order.TableSchema.SchemaName, Order.Columns.Remarks, Order.Columns.Remarks);

            if (AppUserId != 0)
            {
                q.AddWhere(Order.TableSchema.SchemaName, Order.Columns.AppUserId, WhereComparision.EqualsTo, AppUserId);
            }
            if (from != DateTime.MinValue)
            {
                q.AddWhere(Order.TableSchema.SchemaName, Order.Columns.CreateDate, WhereComparision.GreaterThanOrEqual, from.ToUniversalTime());
            }
            if (to != DateTime.MinValue)
            {
                q.AddWhere(Order.TableSchema.SchemaName, Order.Columns.CreateDate, WhereComparision.LessThanOrEqual, to.ToUniversalTime());
            }
            if (suppliersIds != null && suppliersIds.Count != 0)
            {
                q.AddWhere(AppSupplier.TableSchema.SchemaName, AppSupplier.Columns.SupplierId, WhereComparision.In, suppliersIds);
            }
            if (IsSendReceived != null && IsSendReceived.Count != 0)
            {
                q.AddWhere(Order.Columns.IsSendRecived, WhereComparision.In, IsSendReceived);
            }
            //if (StatusIdList != null && StatusIdList.Count != 0)
            //{
            //    q.AddWhere(Order.Columns.TransactionStatus, WhereComparision.In, StatusIdList);
            //}
            if (StatusPayementIdList != null && StatusPayementIdList.Count != 0)
            {
                q.AddWhere(Order.Columns.PaySupplierStatus, WhereComparision.In, StatusPayementIdList);
            }
            //if (SearchBid != "")
            //{
            //    q.AddWhere(Order.TableSchema.SchemaName, Order.Columns.BidId, WhereComparision.Like, SearchBid);
            //}
            if (PageSize > 0)
            {
                q.LimitRows(PageSize).OffsetRows(PageSize * CurrentPageIndex);
            }

            List<OrderUI> LstOrder = new List<OrderUI>();
            using (DataReaderBase reader = q.ExecuteReader())
            {
                while (reader.Read())
                {
                    OrderUI orderUI = new OrderUI();
                    orderUI.SupplierName = (reader[AppSupplier.Columns.BusinessName] != null ? reader[AppSupplier.Columns.BusinessName].ToString() : "");
                    orderUI.SupplierId = (String.IsNullOrWhiteSpace(reader[AppSupplier.Columns.SupplierId].ToString()) ? 0 : Convert.ToInt64(reader[AppSupplier.Columns.SupplierId].ToString()));
                    orderUI.OrderId = (reader[Order.Columns.OrderId] != null ? Int64.Parse(reader[Order.Columns.OrderId].ToString()) : 0);
                    orderUI.OrderDate = (reader[Order.Columns.CreateDate] != null ? DateTime.Parse(reader[Order.Columns.CreateDate].ToString()).ToLocalTime() : DateTime.MinValue);
                    orderUI.BidId = (reader[Bid.Columns.BidId] != null ? Int64.Parse(reader[Bid.Columns.BidId].ToString()) : 0);
                    orderUI.BidEndDate = (reader[Bid.Columns.EndDate] != null ? DateTime.Parse(reader[Bid.Columns.EndDate].ToString()).ToLocalTime() : DateTime.MinValue);
                    orderUI.DonationPrice = ((reader[Donation.Columns.DonationPrice] is DBNull) ? 0 : Convert.ToDecimal(reader[Donation.Columns.DonationPrice]));
                    orderUI.TotalPrice = (reader[Order.Columns.TotalPrice] != null ? decimal.Parse(reader[Order.Columns.TotalPrice].ToString()) : 0);
                    orderUI.PriceAfterDiscount = (reader[Order.Columns.PriceAfterDiscount] != null ? decimal.Parse(reader[Order.Columns.PriceAfterDiscount].ToString()) : 0);
                    orderUI.Precent = (String.IsNullOrWhiteSpace(reader[AppSupplier.Columns.Precent].ToString()) ? 0 : Int32.Parse(reader[AppSupplier.Columns.Precent].ToString()));
                    orderUI.PrecentDiscount = (reader[Order.Columns.PrecentDiscount] != null ? Int32.Parse(reader[Order.Columns.PrecentDiscount].ToString()) : 0);
                    orderUI.IsSendReceived = (reader[Order.Columns.IsSendRecived] != null) ? Convert.ToBoolean(reader[Order.Columns.IsSendRecived]) : false;
                    //orderUI.TransactionResponseCode = (reader[Order.Columns.TransactionResponseCode] != null ? reader[Order.Columns.TransactionResponseCode].ToString() : "");
                    //orderUI.TransactionStatus = (OrderStatus)Enum.Parse(typeof(OrderStatus), reader[Order.Columns.TransactionStatus].ToString());
                    orderUI.PaySupplierStatus = (PaymentStatus)Enum.Parse(typeof(PaymentStatus), reader[Order.Columns.PaySupplierStatus].ToString());
                    orderUI.SuppliedDate = (reader[Order.Columns.SuppliedDate] is DBNull) ? DateTime.MinValue : DateTime.Parse(reader[Order.Columns.SuppliedDate].ToString());
                    orderUI.Remarks = (reader[Order.Columns.Remarks] != null ? reader[Order.Columns.Remarks].ToString() : "");
                    if (orderUI.PaySupplierStatus == PaymentStatus.NotPayed)
                        orderUI.PaymentForSupplier = orderUI.TotalPrice * ((100m - orderUI.Precent) / 100);
                    else if (orderUI.PaySupplierStatus == PaymentStatus.Payed)
                        orderUI.PaymentForSupplier = 0;
                    else if (orderUI.PaySupplierStatus == PaymentStatus.Canceled)
                        orderUI.PaymentForSupplier = (orderUI.TotalPrice * ((100m - orderUI.Precent) / 100)) * (-1);
                    LstOrder.Add(orderUI);
                }

            }
            return LstOrder;
        }

        public static List<OrderUI> GetAllAppUserOrders(Int64 AppUserId = 0, DateTime from = new DateTime(), DateTime to = new DateTime(), string SearchBid = "", int PageSize = 0, int CurrentPageIndex = 0)
        {
            Query q = new Query(Order.TableSchema);

            q.Join(JoinType.InnerJoin, Order.TableSchema, Order.Columns.BidId, Order.TableSchema.SchemaName,
                     Bid.TableSchema, Bid.Columns.BidId, Bid.TableSchema.SchemaName);
            q.Join(JoinType.LeftJoin, Order.TableSchema, Order.Columns.DonationId, Order.TableSchema.SchemaName,
                    Donation.TableSchema, Donation.Columns.DonationId, Donation.TableSchema.SchemaName);
            q.Join(JoinType.InnerJoin, Order.TableSchema, Order.Columns.SupplierId, Order.TableSchema.SchemaName,
                    AppSupplier.TableSchema, AppSupplier.Columns.SupplierId, AppSupplier.TableSchema.SchemaName);
            q.Join(JoinType.LeftJoin, Order.TableSchema, Order.Columns.CampaignId, Order.TableSchema.SchemaName,
                    Campaign.TableSchema, Campaign.Columns.CampaignId, Campaign.TableSchema.SchemaName);

            q.OrderBy(Order.TableSchema.SchemaName, Order.Columns.CreateDate, SortDirection.DESC);
            q.Select(Order.TableSchema.SchemaName, Order.Columns.OrderId, Order.Columns.OrderId, true);
            q.AddSelect(AppSupplier.TableSchema.SchemaName, AppSupplier.Columns.BusinessName, AppSupplier.Columns.BusinessName);
            q.AddSelect(AppSupplier.TableSchema.SchemaName, AppSupplier.Columns.SupplierId, AppSupplier.Columns.SupplierId);
            q.AddSelect(AppSupplier.TableSchema.SchemaName, AppSupplier.Columns.Precent, AppSupplier.Columns.Precent);
            q.AddSelect(Order.TableSchema.SchemaName, Order.Columns.CreateDate, Order.Columns.CreateDate);
            q.AddSelect(Bid.TableSchema.SchemaName, Bid.Columns.BidId, Bid.Columns.BidId);
            q.AddSelect(Bid.TableSchema.SchemaName, Bid.Columns.StartDate, Bid.Columns.StartDate);
            q.AddSelect(Bid.TableSchema.SchemaName, Bid.Columns.EndDate, Bid.Columns.EndDate);
            q.AddSelect(Donation.TableSchema.SchemaName, Donation.Columns.DonationPrice, Donation.Columns.DonationPrice);
            q.AddSelect(Order.TableSchema.SchemaName, Order.Columns.TotalPrice, Order.Columns.TotalPrice);
            q.AddSelect(Order.TableSchema.SchemaName, Order.Columns.Gifts, Order.Columns.Gifts);
            q.AddSelect(Order.TableSchema.SchemaName, Order.Columns.TotalPrice, Order.Columns.TotalPrice);
            q.AddSelect(Order.TableSchema.SchemaName, Order.Columns.PriceAfterDiscount, Order.Columns.PriceAfterDiscount);
            q.AddSelect(Order.TableSchema.SchemaName, Order.Columns.PrecentDiscount, Order.Columns.PrecentDiscount);
            q.AddSelect(Order.TableSchema.SchemaName, Order.Columns.IsSendRecived, Order.Columns.IsSendRecived);
            //q.AddSelect(Order.TableSchema.SchemaName, Order.Columns.TransactionResponseCode, Order.Columns.TransactionResponseCode);
            //q.AddSelect(Order.TableSchema.SchemaName, Order.Columns.TransactionStatus, Order.Columns.TransactionStatus);
            q.AddSelect(Campaign.TableSchema.SchemaName, Campaign.Columns.CampaignName, Campaign.Columns.CampaignName);
            q.Where(Order.TableSchema.SchemaName, Order.Columns.AppUserId, WhereComparision.EqualsTo, AppUserId);
            q.AddWhere(Order.TableSchema.SchemaName, Order.Columns.UserPaySupplierStatus, WhereComparision.EqualsTo, UserPaymentStatus.Payed);
            if (from != DateTime.MinValue)
            {
                q.AddWhere(Order.TableSchema.SchemaName, Order.Columns.CreateDate, WhereComparision.GreaterThanOrEqual, from);
            }
            if (to != DateTime.MinValue)
            {
                q.AddWhere(Order.TableSchema.SchemaName, Order.Columns.CreateDate, WhereComparision.LessThanOrEqual, to);
            }
            if (SearchBid != "")
            {
                q.AddWhere(Order.TableSchema.SchemaName, Order.Columns.BidId, WhereComparision.Like, SearchBid);
            }
            if (PageSize > 0)
            {
                q.LimitRows(PageSize).OffsetRows(PageSize * CurrentPageIndex);
            }

            List<OrderUI> LstOrder = new List<OrderUI>();
            using (DataReaderBase reader = q.ExecuteReader())
            {
                while (reader.Read())
                {
                    OrderUI orderUI = new OrderUI();
                    orderUI.SupplierName = (reader[AppSupplier.Columns.BusinessName] != null ? reader[AppSupplier.Columns.BusinessName].ToString() : "");
                    orderUI.SupplierId = (reader[AppSupplier.Columns.SupplierId] != null ? Int64.Parse(reader[AppSupplier.Columns.SupplierId].ToString()) : 0);
                    orderUI.OrderId = (reader[Order.Columns.OrderId] != null ? Int64.Parse(reader[Order.Columns.OrderId].ToString()) : 0);
                    orderUI.OrderDate = (reader[Order.Columns.CreateDate] != null ? DateTime.Parse(reader[Order.Columns.CreateDate].ToString()).ToLocalTime() : DateTime.MinValue);
                    orderUI.BidId = (reader[Bid.Columns.BidId] != null ? Int64.Parse(reader[Bid.Columns.BidId].ToString()) : 0);
                    orderUI.BidEndDate = (reader[Bid.Columns.EndDate] != null ? DateTime.Parse(reader[Bid.Columns.EndDate].ToString()).ToLocalTime() : DateTime.MinValue);
                    orderUI.DonationPrice = ((reader[Donation.Columns.DonationPrice] is DBNull) ? 0 : Convert.ToDecimal(reader[Donation.Columns.DonationPrice]));
                    orderUI.TotalPrice = (reader[Order.Columns.TotalPrice] != null ? decimal.Parse(reader[Order.Columns.TotalPrice].ToString()) : 0);
                    orderUI.PriceAfterDiscount = (reader[Order.Columns.PriceAfterDiscount] != null ? decimal.Parse(reader[Order.Columns.PriceAfterDiscount].ToString()) : 0);
                    orderUI.Precent = (reader[AppSupplier.Columns.Precent] != null ? Int32.Parse(reader[AppSupplier.Columns.Precent].ToString()) : 0);
                    orderUI.PaymentForSupplier = orderUI.TotalPrice * ((100m - orderUI.Precent) / 100);
                    orderUI.PrecentDiscount = (reader[Order.Columns.PrecentDiscount] != null ? Int32.Parse(reader[Order.Columns.PrecentDiscount].ToString()) : 0);
                    orderUI.IsSendReceived = (reader[Order.Columns.IsSendRecived] != null) ? Convert.ToBoolean(reader[Order.Columns.IsSendRecived]) : false;
                    // orderUI.TransactionResponseCode = (reader[Order.Columns.TransactionResponseCode] != null ? reader[Order.Columns.TransactionResponseCode].ToString() : "");
                    // orderUI.TransactionStatus = (OrderStatus)Enum.Parse(typeof(OrderStatus), reader[Order.Columns.TransactionStatus].ToString());
                    orderUI.Gift = (reader[Order.Columns.Gifts] != null ? reader[Order.Columns.Gifts].ToString() : "");
                    orderUI.CampaignName = (reader[Campaign.Columns.CampaignName] != null ? reader[Campaign.Columns.CampaignName].ToString() : "");
                    orderUI.LstProduct = BidController.GetProductsByBid(orderUI.BidId);
                    LstOrder.Add(orderUI);
                }

            }
            return LstOrder;
        }

        public static void ChangeStatusToPayed(List<Int64> orderIds)
        {
            Query.New<Order>().Where(Order.Columns.OrderId, WhereComparision.In, orderIds)
                .Update(Order.Columns.PaySupplierStatus, OrderStatus.Payed)
                .Execute();
        }

        public static void UpdateOrderRemarks(Int64 OrderId, string Remark)
        {
            Query.New<Order>().Where(Order.Columns.OrderId, OrderId)
                .Update(Order.Columns.Remarks, Remark)
                .Execute();
        }

        public static Order GenerateNewOrder(ProcessingResults results, long userId, long bidId, string gifts, long supplierId, decimal totalPrice,Source source)
        {
            //  var messageId = BIdMessageController.AddNewMessage(bidId, supplierId, 0, BIdMessageController.ADMIN_STAGE);
              var messageId = BIdMessageController.AddNewMessage(bidId, supplierId);

            var order = new Order
            {
                AppUserId = userId,
                BidId = bidId,
                CreateDate = DateTime.UtcNow,
                Last4Digits = results.Last4Digits,
                UserPaySupplierStatus = UserPaymentStatus.NotPayed,
                TotalPrice = totalPrice,
                Transaction = results.CardToken,
                ExpiryDate = results.CardExpiration,
                AuthNumber = results.AuthNumber,
                Gifts = gifts,
                SpecialInstructions = results.SpecialInstructions ?? "",
                NumOfPayments = results.NumOfPayments,
                Source=(int)source,
            };
            order.Save();

            AppUserCard paymentToken = AppUserCard.FetchByAppUserId(userId);
            if (paymentToken == null)
            {
                paymentToken = new AppUserCard();
            }
            paymentToken.AppUserId = userId;
            paymentToken.CardToken = results.CardToken;
            paymentToken.ExpiryDate = results.CardExpiration;
            paymentToken.Last4Digit = results.Last4Digits;
            if (!String.IsNullOrEmpty(results.PersonalId)) paymentToken.IdNumber = results.PersonalId;
            paymentToken.Save();

            AppSupplier supplier = AppSupplier.FetchByID(supplierId);
            if (supplier != null)
            {
                supplier.MaxWinningsNum = (supplier.MaxWinningsNum > 0 ? supplier.MaxWinningsNum - 1 : 0);
                if (supplier.MaxWinningsNum == 0)
                {
                    // SupplierNotification.SendNotificationMaxAutoModeMessage(supplier.SupplierId);
                }
                supplier.Save();
            }
            SMSController.sendNewBidSMS(AppUser.FetchByID(userId).Phone);
            return order;
        }
    }

}

