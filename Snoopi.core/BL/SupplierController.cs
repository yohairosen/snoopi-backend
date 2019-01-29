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
    public class SupplierUI : AppSupplier
    {
        public double AvgRate { get; set; }
        public string CityName { get; set; }
        public CommentCollection LstComment { get; set; }
        public List<City> citiesSupplied { get; set; }
        public List<City> citiesHomeService { get; set; }
        public List<Service> LstServices { get; set; }
        public int OrderNum { get; set; }
        public int BidNum { get; set; }
        public int ActiveNum { get; set; }
        public Int64 ActiveOrder { get; set; }
        public bool IsProduct { get; set; }
    }

    public class MainBid
    {
        public Int64 BidId { get; set; }
        public string EndBid { get; set; }
        public string City { get; set; }
        public bool IsService { get; set; }
        public string CustomerComment { get; set; }
        public string ServiceName { get; set; }
        public List<BidProductUI> LstProduct { get; set; }
        public Int64 ServiceId { get; set; }
        public DateTime DateOrder { get; set; }
    }

    public class MainOffer : MainBid
    {
        public Int64 OfferId { get; set; }
        public decimal TotalPrice { get; set; }
        public string Gift { get; set; }
        public string Status { get; set; }
        public int NumOfPayments { get; set; }

        public string SupplierRemarks { get; set; }

    }


    public class SupplierController
    {
        public static int DeleteAllSupplierCity(Int64 SupplierId)
        {
            return new Query(SupplierCity.TableSchema).Where(SupplierCity.Columns.SupplierId, SupplierId).Delete().ExecuteNonQuery();
        }

        public static int DeleteAllSupplierHomeCity(Int64 SupplierId)
        {
            return new Query(SupplierHomeServiceCity.TableSchema).Where(SupplierHomeServiceCity.Columns.SupplierId, SupplierId).Delete().ExecuteNonQuery();
        }


        public static MainBid GetNewBidById(Int64 BidId, Int64 SupplierId)
        {

            Query qry = new Query(Bid.TableSchema);
            qry.SelectAllTableColumns();
            qry.AddSelect(City.TableSchema.SchemaName, City.Columns.CityName, City.Columns.CityName);
            qry.Join(JoinType.LeftJoin, Bid.TableSchema, Bid.Columns.AppUserId, Bid.TableSchema.SchemaName, AppUser.TableSchema, AppUser.Columns.AppUserId, AppUser.TableSchema.SchemaName);
            qry.Join(JoinType.LeftJoin, Bid.TableSchema, Bid.Columns.TempAppUserId, Bid.TableSchema.SchemaName, TempAppUser.TableSchema, TempAppUser.Columns.TempAppUserId, TempAppUser.TableSchema.SchemaName);
            qry.Join(JoinType.InnerJoin, City.TableSchema, City.TableSchema.SchemaName,
                new JoinColumnPair(TempAppUser.TableSchema, TempAppUser.Columns.CityId, City.Columns.CityId).JoinOR(AppUser.TableSchema.SchemaName, AppUser.Columns.CityId, City.Columns.CityId));
            //qry.Where(Bid.TableSchema.SchemaName, Bid.Columns.EndDate, WhereComparision.GreaterThanOrEqual, DateTime.UtcNow);
            qry.Where(Bid.TableSchema.SchemaName, Bid.Columns.BidId, WhereComparision.EqualsTo, BidId);
            qry.AddWhere(Bid.Columns.EndDate, WhereComparision.GreaterThanOrEqual, DateTime.UtcNow);

            using (DataReaderBase reader = qry.ExecuteReader())
            {
                if (reader.Read())
                {
                    MainBid m = new MainBid();
                    TimeSpan diff = (Convert.ToDateTime(reader[Bid.Columns.EndDate]) - DateTime.UtcNow);
                    if (reader[Bid.Columns.EndDate] != null)
                    {
                        string hours = (Math.Truncate(diff.TotalHours) < 10) ? "0" + Math.Truncate(diff.TotalHours).ToString() : Math.Truncate(diff.TotalHours).ToString();
                        string minutes = (((int)((diff.TotalHours - Math.Truncate(diff.TotalHours)) * 60)) < 10) ? "0" + ((int)((diff.TotalHours - Math.Truncate(diff.TotalHours)) * 60)).ToString() : ((int)((diff.TotalHours - Math.Truncate(diff.TotalHours)) * 60)).ToString();
                        m.EndBid = hours + ":" + minutes;
                    }
                    else
                        m.EndBid = "00:00";
                    m.City = reader[City.Columns.CityName] != null ? reader[City.Columns.CityName].ToString() : "";
                    m.BidId = reader[Bid.Columns.BidId] != null ? Convert.ToInt64(reader[Bid.Columns.BidId]) : 0;
                    m.LstProduct = BidController.GetProductsByBid(m.BidId, SupplierId);

                    return m;
                }

            }
            return null;
        }

        public static MainBid GetNewServiceBidById(Int64 BidId, Int64 SupplierId)
        {
            Query qry = new Query(BidService.TableSchema);
            qry.SelectAllTableColumns();
            qry.AddSelect(City.TableSchema.SchemaName, City.Columns.CityName, City.Columns.CityName);
            qry.AddSelect(Service.TableSchema.SchemaName, Service.Columns.ServiceName, Service.Columns.ServiceName);
            qry.AddSelect(Service.TableSchema.SchemaName, Service.Columns.ServiceId, Service.Columns.ServiceId);
            qry.Join(JoinType.LeftJoin, BidService.TableSchema, BidService.Columns.AppUserId, BidService.TableSchema.SchemaName, AppUser.TableSchema, AppUser.Columns.AppUserId, AppUser.TableSchema.SchemaName);
            qry.Join(JoinType.LeftJoin, BidService.TableSchema, BidService.Columns.TempAppUserId, BidService.TableSchema.SchemaName, TempAppUser.TableSchema, TempAppUser.Columns.TempAppUserId, TempAppUser.TableSchema.SchemaName);
            qry.Join(JoinType.InnerJoin, BidService.TableSchema, BidService.Columns.ServiceId, BidService.TableSchema.SchemaName, Service.TableSchema, Service.Columns.ServiceId, Service.TableSchema.SchemaName);
            qry.Join(JoinType.LeftJoin, City.TableSchema, City.TableSchema.SchemaName,
                new JoinColumnPair(TempAppUser.TableSchema, TempAppUser.Columns.CityId, City.Columns.CityId).JoinOR(AppUser.TableSchema.SchemaName, AppUser.Columns.CityId, City.Columns.CityId));
            //qry.Where(Bid.TableSchema.SchemaName, Bid.Columns.EndDate, WhereComparision.GreaterThanOrEqual, DateTime.UtcNow);
            qry.Where(BidService.TableSchema.SchemaName, BidService.Columns.BidId, WhereComparision.EqualsTo, BidId);
            qry.AddWhere(Bid.Columns.EndDate, WhereComparision.GreaterThanOrEqual, DateTime.UtcNow);

            using (DataReaderBase reader = qry.ExecuteReader())
            {
                if (reader.Read())
                {
                    MainBid m = new MainBid();
                    TimeSpan diff = (Convert.ToDateTime(reader[Bid.Columns.EndDate]) - DateTime.UtcNow);
                    if (reader[Bid.Columns.EndDate] != null)
                    {
                        string hours = (Math.Truncate(diff.TotalHours) < 10) ? "0" + Math.Truncate(diff.TotalHours).ToString() : Math.Truncate(diff.TotalHours).ToString();
                        string minutes = (((int)((diff.TotalHours - Math.Truncate(diff.TotalHours)) * 60)) < 10) ? "0" + ((int)((diff.TotalHours - Math.Truncate(diff.TotalHours)) * 60)).ToString() : ((int)((diff.TotalHours - Math.Truncate(diff.TotalHours)) * 60)).ToString();
                        m.EndBid = hours + ":" + minutes;
                    }
                    else
                        m.EndBid = "00:00";
                    m.City = reader[City.Columns.CityName] != null ? reader[City.Columns.CityName].ToString() : "";
                    m.BidId = reader[BidService.Columns.BidId] != null ? Convert.ToInt64(reader[BidService.Columns.BidId]) : 0;
                    m.CustomerComment = reader[BidService.Columns.ServiceComment] != null ? reader[BidService.Columns.ServiceComment].ToString() : "";
                    m.ServiceName = reader[Service.Columns.ServiceName] != null ? reader[Service.Columns.ServiceName].ToString() : "";
                    m.ServiceId = reader[Service.Columns.ServiceId] is DBNull ? 0 : Convert.ToInt64(reader[Service.Columns.ServiceId]);
                    return m;
                }

            }

            return null;
        }

        public static MainOffer GetServiceOfferById(Int64 OfferId, Int64 SupplierId)
        {
            Query qry = new Query(OfferService.TableSchema);
            qry.Join(JoinType.InnerJoin, OfferService.TableSchema, OfferService.Columns.BidId, OfferService.TableSchema.SchemaName, BidService.TableSchema, BidService.Columns.BidId, BidService.TableSchema.SchemaName);
            qry.Join(JoinType.LeftJoin, BidService.TableSchema, BidService.Columns.AppUserId, BidService.TableSchema.SchemaName, AppUser.TableSchema, AppUser.Columns.AppUserId, AppUser.TableSchema.SchemaName);
            qry.Join(JoinType.LeftJoin, BidService.TableSchema, BidService.Columns.TempAppUserId, BidService.TableSchema.SchemaName, TempAppUser.TableSchema, TempAppUser.Columns.TempAppUserId, TempAppUser.TableSchema.SchemaName);
            qry.Join(JoinType.InnerJoin, BidService.TableSchema, BidService.Columns.ServiceId, BidService.TableSchema.SchemaName, Service.TableSchema, Service.Columns.ServiceId, Service.TableSchema.SchemaName);
            qry.Join(JoinType.LeftJoin, City.TableSchema, City.TableSchema.SchemaName,
                new JoinColumnPair(TempAppUser.TableSchema, TempAppUser.Columns.CityId, City.Columns.CityId).JoinOR(AppUser.TableSchema.SchemaName, AppUser.Columns.CityId, City.Columns.CityId));
            //qry.Where(Bid.TableSchema.SchemaName, Bid.Columns.EndDate, WhereComparision.GreaterThanOrEqual, DateTime.UtcNow);
            qry.Where(OfferService.TableSchema.SchemaName, OfferService.Columns.OfferId, WhereComparision.EqualsTo, OfferId);

            MainOffer m = new MainOffer();
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                if (reader.Read())
                {
                    //TimeSpan diff = (Convert.ToDateTime(reader[Bid.Columns.EndDate]) - DateTime.UtcNow);
                    //if (reader[Bid.Columns.EndDate] != null)
                    //{
                    //    string hours = (Math.Truncate(diff.TotalHours) < 10) ? "0" + Math.Truncate(diff.TotalHours).ToString() : Math.Truncate(diff.TotalHours).ToString();
                    //    string minutes = (((int)((diff.TotalHours - Math.Truncate(diff.TotalHours)) * 60)) < 10) ? "0" + ((int)((diff.TotalHours - Math.Truncate(diff.TotalHours)) * 60)).ToString() : ((int)((diff.TotalHours - Math.Truncate(diff.TotalHours)) * 60)).ToString();
                    //    m.EndBid = hours + ":" + minutes;
                    //}
                    //else
                    //    m.EndBid = "00:00";
                    m.EndBid = reader[Bid.Columns.EndDate].ToString();
                    m.City = reader[City.Columns.CityName] != null ? reader[City.Columns.CityName].ToString() : "";
                    m.BidId = reader[BidService.Columns.BidId] != null ? Convert.ToInt64(reader[BidService.Columns.BidId]) : 0;
                    m.OfferId = reader[OfferService.Columns.OfferId] != null ? Convert.ToInt64(reader[OfferService.Columns.OfferId]) : 0;
                    m.TotalPrice = reader[OfferService.Columns.Price] != null ? Convert.ToDecimal(reader[OfferService.Columns.Price]) : 0;
                    m.SupplierRemarks = reader[OfferService.Columns.SupplierRemarks] != null ? reader[OfferService.Columns.SupplierRemarks].ToString() : "";
                    m.CustomerComment = reader[BidService.Columns.ServiceComment] != null ? reader[BidService.Columns.ServiceComment].ToString() : "";
                    m.ServiceName = reader[Service.Columns.ServiceName] != null ? reader[Service.Columns.ServiceName].ToString() : "";
                    m.ServiceId = reader[Service.Columns.ServiceId] != null ? Convert.ToInt64(reader[Service.Columns.ServiceId]) : 0;
                }
            }

            return m;
        }

        //public static MainOffer GetOfferById(Int64 OfferId, Int64 SupplierId)
        //{
        //    Query qry = new Query(Offer.TableSchema);
        //    qry.Join(JoinType.InnerJoin, Offer.TableSchema, Offer.Columns.BidId, Offer.TableSchema.SchemaName, Bid.TableSchema, Bid.Columns.BidId, Bid.TableSchema.SchemaName);
        //    qry.Join(JoinType.LeftJoin, Bid.TableSchema, Bid.Columns.AppUserId, Bid.TableSchema.SchemaName, AppUser.TableSchema, AppUser.Columns.AppUserId, AppUser.TableSchema.SchemaName);
        //    qry.Join(JoinType.LeftJoin, Bid.TableSchema, Bid.Columns.TempAppUserId, Bid.TableSchema.SchemaName, TempAppUser.TableSchema, TempAppUser.Columns.TempAppUserId, TempAppUser.TableSchema.SchemaName);
        //    qry.Join(JoinType.LeftJoin, City.TableSchema, City.TableSchema.SchemaName,
        //        new JoinColumnPair(TempAppUser.TableSchema, TempAppUser.Columns.CityId, City.Columns.CityId).JoinOR(AppUser.TableSchema.SchemaName, AppUser.Columns.CityId, City.Columns.CityId));
        //    //qry.Where(Bid.TableSchema.SchemaName, Bid.Columns.EndDate, WhereComparision.GreaterThanOrEqual, DateTime.UtcNow);
        //    qry.Where(Offer.TableSchema.SchemaName, Offer.Columns.OfferId, WhereComparision.EqualsTo, OfferId);

        //    MainOffer m = new MainOffer();
        //    using (DataReaderBase reader = qry.ExecuteReader())
        //    {
        //        if (reader.Read())
        //        {
        //            TimeSpan diff = (Convert.ToDateTime(reader[Bid.Columns.EndDate]) - DateTime.UtcNow);
        //            if (reader[Bid.Columns.EndDate] != null)
        //            {
        //                string hours = (Math.Truncate(diff.TotalHours) < 10) ? "0" + Math.Truncate(diff.TotalHours).ToString() : Math.Truncate(diff.TotalHours).ToString();
        //                string minutes = (((int)((diff.TotalHours - Math.Truncate(diff.TotalHours)) * 60)) < 10) ? "0" + ((int)((diff.TotalHours - Math.Truncate(diff.TotalHours)) * 60)).ToString() : ((int)((diff.TotalHours - Math.Truncate(diff.TotalHours)) * 60)).ToString();
        //                m.EndBid = hours + ":" + minutes;
        //            }
        //            else
        //                m.EndBid = "00:00";
        //            m.City = reader[City.Columns.CityName] != null ? reader[City.Columns.CityName].ToString() : "";
        //            m.BidId = reader[Bid.Columns.BidId] != null ? Convert.ToInt64(reader[Bid.Columns.BidId]) : 0;
        //            m.LstProduct = BidController.GetProductsByBid(m.BidId, SupplierId);
        //            m.OfferId = reader[Offer.Columns.OfferId] != null ? Convert.ToInt64(reader[Offer.Columns.OfferId]) : 0;
        //            m.TotalPrice = reader[Offer.Columns.Price] != null ? Convert.ToDecimal(reader[Offer.Columns.Price]) : 0;
        //            m.Gift = reader[Offer.Columns.Gift] != null ? reader[Offer.Columns.Gift].ToString() : "";
        //        }
        //    }

        //    return m;
        //}

        public static MainOffer GetBidOfferById(Int64 bidId, Int64 SupplierId)
        {
            List<MainOffer> lstMainBid = new List<MainOffer>();
            Query qry = new Query(BidMessage.TableSchema);
            qry.Join(JoinType.LeftJoin, BidMessage.TableSchema, BidMessage.Columns.BidId, BidMessage.TableSchema.SchemaName, Bid.TableSchema, Bid.Columns.BidId, Bid.TableSchema.SchemaName);
            qry.Join(JoinType.LeftJoin, Bid.TableSchema, Bid.Columns.BidId, Bid.TableSchema.SchemaName, BidProduct.TableSchema, BidProduct.Columns.BidId, BidProduct.TableSchema.SchemaName);
            qry.Join(JoinType.LeftJoin, Bid.TableSchema, Bid.Columns.BidId, Bid.TableSchema.SchemaName, Order.TableSchema, Order.Columns.BidId, Order.TableSchema.SchemaName);
            qry.Join(JoinType.LeftJoin, Bid.TableSchema, Bid.Columns.CityId, Bid.TableSchema.SchemaName, City.TableSchema, City.Columns.CityId, City.TableSchema.SchemaName);
            qry.SelectAllTableColumns();
            qry.AddSelect(City.TableSchema.SchemaName, City.Columns.CityName, City.Columns.CityName);
            qry.AddSelect(Bid.TableSchema.SchemaName, Bid.Columns.EndDate, Bid.Columns.EndDate);
            qry.AddSelect(Order.TableSchema.SchemaName, Order.Columns.SpecialInstructions, Order.Columns.SpecialInstructions);
            qry.AddSelect(Order.TableSchema.SchemaName, Order.Columns.NumOfPayments, Order.Columns.NumOfPayments);
            qry.AddSelectLiteral("SUM(" + BidProduct.Columns.Amount + "*" + BidProduct.Columns.Price + ")", "TotalPrice");
            qry.AddWhere(BidMessage.Columns.ExpirationTime, WhereComparision.GreaterThanOrEqual, DateTime.Now);
            qry.AddWhere(BidMessage.TableSchema.SchemaName, BidMessage.Columns.SupplierId, WhereComparision.EqualsTo, SupplierId);
            qry.AddWhere(Bid.TableSchema.SchemaName, Bid.Columns.BidId, WhereComparision.EqualsTo, bidId);
            qry.AddWhere(Bid.TableSchema.SchemaName, Bid.Columns.IsActive, WhereComparision.EqualsTo, true);

            qry.GroupBy(Bid.TableSchema.SchemaName, Bid.Columns.BidId);
            qry.OrderBy(Bid.Columns.EndDate, SortDirection.ASC);

            MainOffer m = new MainOffer();
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                if (reader.Read())
                {
                    TimeSpan diff = (Convert.ToDateTime(reader[Bid.Columns.EndDate]) - DateTime.UtcNow);
                    if (reader[Bid.Columns.EndDate] != null)
                    {
                        string hours = (Math.Truncate(diff.TotalHours) < 10) ? "0" + Math.Truncate(diff.TotalHours).ToString() : Math.Truncate(diff.TotalHours).ToString();
                        string minutes = (((int)((diff.TotalHours - Math.Truncate(diff.TotalHours)) * 60)) < 10) ? "0" + ((int)((diff.TotalHours - Math.Truncate(diff.TotalHours)) * 60)).ToString() : ((int)((diff.TotalHours - Math.Truncate(diff.TotalHours)) * 60)).ToString();
                        m.EndBid = hours + ":" + minutes;
                    }
                    else
                        m.EndBid = "00:00";
                    m.City = reader[City.Columns.CityName] != null ? reader[City.Columns.CityName].ToString() : "";
                    m.BidId = reader[Bid.Columns.BidId] != null ? Convert.ToInt64(reader[Bid.Columns.BidId]) : 0;
                    string orid = reader[BidMessage.Columns.OriginalSupplierId].ToString();
                    long origianlSupplierId = Convert.ToInt64(orid);
                    m.LstProduct = BidController.GetProductsByBid(m.BidId, origianlSupplierId);
                    // m.OfferId = reader[Offer.Columns.OfferId] != null ? Convert.ToInt64(reader[Offer.Columns.OfferId]) : 0;
                    m.TotalPrice = m.LstProduct.Sum(p => p.Price * p.Amount);
                    m.Gift = string.Join(",", m.LstProduct.Select(p => p.ProductGift));
                    m.CustomerComment = reader[Order.Columns.SpecialInstructions] != null ? reader[Order.Columns.SpecialInstructions].ToString() : "";
                    m.NumOfPayments = reader[Order.Columns.NumOfPayments] != null ? (int)reader[Order.Columns.NumOfPayments] : 1;
                }
            }

            return m;
        }

        public static List<MainOffer> GetAllOfferBid(Int64 SupplierId)
        {
            List<MainOffer> lstMainBid = new List<MainOffer>();
            Query qry = new Query(BidMessage.TableSchema);
            qry.Join(JoinType.LeftJoin, BidMessage.TableSchema, BidMessage.Columns.BidId, BidMessage.TableSchema.SchemaName, Bid.TableSchema, Bid.Columns.BidId, Bid.TableSchema.SchemaName);
            qry.Join(JoinType.LeftJoin, Bid.TableSchema, Bid.Columns.BidId, Bid.TableSchema.SchemaName, BidProduct.TableSchema, BidProduct.Columns.BidId, BidProduct.TableSchema.SchemaName);
            qry.Join(JoinType.LeftJoin, Bid.TableSchema, Bid.Columns.CityId, Bid.TableSchema.SchemaName, City.TableSchema, City.Columns.CityId, City.TableSchema.SchemaName);
            qry.Join(JoinType.LeftJoin, Bid.TableSchema, Bid.Columns.BidId, Bid.TableSchema.SchemaName, Order.TableSchema, Order.Columns.BidId, Order.TableSchema.SchemaName);
            qry.SelectAllTableColumns();
            qry.AddSelect(City.TableSchema.SchemaName, City.Columns.CityName, City.Columns.CityName);
            qry.AddSelect(Order.TableSchema.SchemaName, Order.Columns.TotalPrice, Order.Columns.TotalPrice);
            qry.AddSelect(Order.TableSchema.SchemaName, Order.Columns.NumOfPayments, Order.Columns.NumOfPayments);
            qry.AddSelect(Order.TableSchema.SchemaName, Order.Columns.SpecialInstructions, Order.Columns.SpecialInstructions);
            qry.AddSelectLiteral("SUM(" + BidProduct.Columns.Amount + "*" + BidProduct.Columns.Price + ")", "TotalPrice");
            qry.AddWhere(BidMessage.Columns.ExpirationTime, WhereComparision.GreaterThanOrEqual, DateTime.Now);
            qry.AddWhere(Bid.TableSchema.SchemaName, Bid.Columns.IsActive, WhereComparision.EqualsTo, true);
            qry.AddWhere(BidMessage.TableSchema.SchemaName, BidMessage.Columns.SupplierId, WhereComparision.EqualsTo, SupplierId);
            var wl = new WhereList();
            wl.OR(BidMessage.Columns.Stage, WhereComparision.NotEqualsTo, BIdMessageController.START_STAGE);
            wl.OR(BidMessage.Columns.Stage, WhereComparision.NotEqualsTo, BIdMessageController.ADMIN_STAGE);
            qry.AND(wl);
            qry.GroupBy(Bid.TableSchema.SchemaName, Bid.Columns.BidId);
            qry.OrderBy(Bid.Columns.EndDate, SortDirection.ASC);

            using (DataReaderBase reader = qry.ExecuteReader())
            {
                while (reader.Read())
                {
                    //TimeSpan diff = (Convert.ToDateTime(reader[Bid.Columns.EndDate]) - DateTime.UtcNow);
                    //string EndBid = "00:00";
                    //if (reader[Bid.Columns.EndDate] != null)
                    //{
                    //    string hours = (Math.Truncate(diff.TotalHours) < 10) ? "0" + Math.Truncate(diff.TotalHours).ToString() : Math.Truncate(diff.TotalHours).ToString();
                    //    string minutes = (((int)((diff.TotalHours - Math.Truncate(diff.TotalHours)) * 60)) < 10) ? "0" + ((int)((diff.TotalHours - Math.Truncate(diff.TotalHours)) * 60)).ToString() : ((int)((diff.TotalHours - Math.Truncate(diff.TotalHours)) * 60)).ToString();
                    //    EndBid = hours + ":" + minutes;
                    //}
                    //else
                    //    EndBid = "00:00";
                    lstMainBid.Add(new MainOffer
                    {
                        BidId = reader[BidMessage.Columns.BidId] != null ? Convert.ToInt64(reader[Bid.Columns.BidId]) : 0,
                        City = reader[City.Columns.CityName] != null ? reader[City.Columns.CityName].ToString() : "",
                        EndBid = reader[BidMessage.Columns.ExpirationTime].ToString(),
                        DateOrder = reader[BidMessage.Columns.ExpirationTime] != null ? Convert.ToDateTime(reader[BidMessage.Columns.ExpirationTime]) : DateTime.MinValue,
                        IsService = false,
                        TotalPrice = Convert.ToDecimal(reader[Order.Columns.TotalPrice]),
                        Status = reader[BidMessage.Columns.Stage] != null ? convert_stage_to_status(reader[BidMessage.Columns.Stage].ToString()) : "",
                        NumOfPayments =!string.IsNullOrEmpty(reader[Order.Columns.NumOfPayments].ToString()) ? Convert.ToInt16(reader[Order.Columns.NumOfPayments].ToString()) :1,
                        CustomerComment = !string.IsNullOrEmpty(reader[Order.Columns.SpecialInstructions].ToString()) ? reader[Order.Columns.SpecialInstructions].ToString() :""
                    });

                }
            }
            //List<MainOffer> lstMainBid = new List<MainOffer>();

            //Query qry = new Query(Bid.TableSchema);
            //qry.Join(JoinType.LeftJoin, Bid.TableSchema, Bid.Columns.AppUserId, Bid.TableSchema.SchemaName, AppUser.TableSchema, AppUser.Columns.AppUserId, AppUser.TableSchema.SchemaName);
            //qry.Join(JoinType.LeftJoin, Bid.TableSchema, Bid.Columns.TempAppUserId, Bid.TableSchema.SchemaName, TempAppUser.TableSchema, TempAppUser.Columns.TempAppUserId, TempAppUser.TableSchema.SchemaName);
            //qry.Join(JoinType.LeftJoin, City.TableSchema, City.TableSchema.SchemaName,
            //    new JoinColumnPair(TempAppUser.TableSchema, TempAppUser.Columns.CityId, City.Columns.CityId).JoinOR(AppUser.TableSchema.SchemaName, AppUser.Columns.CityId, City.Columns.CityId));
            //qry.Join(JoinType.LeftJoin, Bid.TableSchema, Bid.Columns.BidId, Bid.TableSchema.SchemaName, Order.TableSchema, Order.Columns.BidId, Order.TableSchema.SchemaName + "1");
            //qry.Join(JoinType.InnerJoin, Bid.TableSchema, Bid.Columns.BidId, Bid.TableSchema.SchemaName, Offer.TableSchema, Offer.Columns.BidId, Offer.TableSchema.SchemaName);
            //qry.Where(Bid.TableSchema.SchemaName, Bid.Columns.EndDate, WhereComparision.GreaterThanOrEqual, DateTime.UtcNow);
            //qry.AddWhere(Offer.TableSchema.SchemaName, Offer.Columns.SupplierId, WhereComparision.EqualsTo, SupplierId);
            //qry.AddWhere(Order.TableSchema.SchemaName + "1", Order.Columns.OrderId, WhereComparision.EqualsTo, null);
            //qry.Select(Bid.TableSchema.SchemaName, Bid.Columns.BidId, Bid.Columns.BidId, true);
            //qry.AddSelect(City.TableSchema.SchemaName, City.Columns.CityName, City.Columns.CityName);
            //qry.AddSelect(Bid.TableSchema.SchemaName, Bid.Columns.EndDate, Bid.Columns.EndDate);
            //qry.AddSelect(Offer.TableSchema.SchemaName, Offer.Columns.OfferId, Offer.Columns.OfferId);

            //using (DataReaderBase reader = qry.ExecuteReader())
            //{
            //    while (reader.Read())
            //    {
            //        TimeSpan diff = (Convert.ToDateTime(reader[Bid.Columns.EndDate]) - DateTime.UtcNow);
            //        string EndBid = "00:00";
            //        if (reader[Bid.Columns.EndDate] != null)
            //        {
            //            string hours = (Math.Truncate(diff.TotalHours) < 10) ? "0" + Math.Truncate(diff.TotalHours).ToString() : Math.Truncate(diff.TotalHours).ToString();
            //            string minutes = (((int)((diff.TotalHours - Math.Truncate(diff.TotalHours)) * 60)) < 10) ? "0" + ((int)((diff.TotalHours - Math.Truncate(diff.TotalHours)) * 60)).ToString() : ((int)((diff.TotalHours - Math.Truncate(diff.TotalHours)) * 60)).ToString();
            //            EndBid = hours + ":" + minutes;
            //        }
            //        else
            //            EndBid = "00:00";
            //        lstMainBid.Add(new MainOffer
            //        {
            //            BidId = reader[Bid.Columns.BidId] != null ? Convert.ToInt64(reader[Bid.Columns.BidId]) : 0,
            //            City = reader[City.Columns.CityName] != null ? reader[City.Columns.CityName].ToString() : "",
            //            EndBid = EndBid,
            //            OfferId = reader[Offer.Columns.OfferId] != null ? Convert.ToInt64(reader[Offer.Columns.OfferId]) : 0,
            //            IsService = false
            //        });

            //    }
            //}
            qry = new Query(BidService.TableSchema);
            qry.Join(JoinType.InnerJoin, BidService.TableSchema, BidService.Columns.BidId, BidService.TableSchema.SchemaName, OfferService.TableSchema,
                OfferService.Columns.BidId, OfferService.TableSchema.SchemaName);
            qry.Join(JoinType.LeftJoin, BidService.TableSchema, BidService.Columns.AppUserId, BidService.TableSchema.SchemaName, AppUser.TableSchema, AppUser.Columns.AppUserId, AppUser.TableSchema.SchemaName);
            qry.Join(JoinType.LeftJoin, BidService.TableSchema, BidService.Columns.TempAppUserId, BidService.TableSchema.SchemaName, TempAppUser.TableSchema, TempAppUser.Columns.TempAppUserId, TempAppUser.TableSchema.SchemaName);
            qry.Join(JoinType.LeftJoin, City.TableSchema, City.TableSchema.SchemaName,
                new JoinColumnPair(TempAppUser.TableSchema, TempAppUser.Columns.CityId, City.Columns.CityId).JoinOR(AppUser.TableSchema.SchemaName, AppUser.Columns.CityId, City.Columns.CityId));
            //qry.Where(BidService.TableSchema.SchemaName, BidService.Columns.EndDate, WhereComparision.GreaterThanOrEqual, DateTime.UtcNow);
            qry.AddWhere(OfferService.TableSchema.SchemaName, OfferService.Columns.SupplierId, WhereComparision.EqualsTo, SupplierId);
            qry.Select(BidService.TableSchema.SchemaName, BidService.Columns.BidId, BidService.Columns.BidId, true);
            qry.AddSelect(City.TableSchema.SchemaName, City.Columns.CityName, City.Columns.CityName);
            qry.AddSelect(BidService.TableSchema.SchemaName, BidService.Columns.EndDate, BidService.Columns.EndDate);
            qry.AddSelect(OfferService.TableSchema.SchemaName, OfferService.Columns.OfferId, OfferService.Columns.OfferId);
            qry.OrderBy(BidService.Columns.EndDate, SortDirection.DESC);
            qry.LimitRows(50);
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                while (reader.Read())
                {
                    TimeSpan diff = (Convert.ToDateTime(reader[Bid.Columns.EndDate]) - DateTime.UtcNow);
                        
                    string EndBid = "00:00";
                    
                    if (reader[Bid.Columns.EndDate] != null)
                    {
                        if (diff.TotalHours < 0 || diff.TotalMinutes < 0)
                            EndBid = Convert.ToDateTime(reader[Bid.Columns.EndDate]).ToString();
                        else
                        {
                            string hours = (Math.Truncate(diff.TotalHours) < 10) ? "0" + Math.Truncate(diff.TotalHours).ToString() : Math.Truncate(diff.TotalHours).ToString();
                            string minutes = (((int)((diff.TotalHours - Math.Truncate(diff.TotalHours)) * 60)) < 10) ? "0" + ((int)((diff.TotalHours - Math.Truncate(diff.TotalHours)) * 60)).ToString() : ((int)((diff.TotalHours - Math.Truncate(diff.TotalHours)) * 60)).ToString();
                            EndBid = hours + ":" + minutes;
                        }
                    }
                    else
                        EndBid = "00:00";
                    lstMainBid.Add(new MainOffer
                    {
                        BidId = reader[BidService.Columns.BidId] != null ? Convert.ToInt64(reader[BidService.Columns.BidId]) : 0,
                        City = reader[City.Columns.CityName] != null ? reader[City.Columns.CityName].ToString() : "",
                        EndBid = EndBid,
                        OfferId = reader[OfferService.Columns.OfferId] != null ? Convert.ToInt64(reader[OfferService.Columns.OfferId]) : 0,
                        IsService = true
                    });

                }
            }
            return lstMainBid;
        }

        private static string convert_stage_to_status(string stage)
        {
            if (stage.ToLower() == "supplier")
                return "זכיה! ממתין לאישור";
            return "הזדמנות! ממתין לאישור";
        }

        public static List<MainBid> GetAllNewBid(Int64 SupplierId)
        {
            if (AppSupplier.FetchByID(SupplierId).Status == true)
            {
                List<MainBid> lstMainBid = new List<MainBid>();
            //    Query qry = new Query(BidMessage.TableSchema);
            //    qry.Join(JoinType.LeftJoin, BidMessage.TableSchema, BidMessage.Columns.BidId, BidMessage.TableSchema.SchemaName, Bid.TableSchema, Bid.Columns.BidId, Bid.TableSchema.SchemaName);
            //    qry.Join(JoinType.LeftJoin, Bid.TableSchema, Bid.Columns.AppUserId, Bid.TableSchema.SchemaName,
            //     AppUser.TableSchema, AppUser.Columns.AppUserId, AppUser.TableSchema.SchemaName);
            //    qry.Join(JoinType.LeftJoin, Bid.TableSchema, Bid.Columns.TempAppUserId, Bid.TableSchema.SchemaName, TempAppUser.TableSchema, TempAppUser.Columns.TempAppUserId, TempAppUser.TableSchema.SchemaName);
            //    qry.Join(JoinType.LeftJoin, City.TableSchema, City.TableSchema.SchemaName,
            //        new JoinColumnPair(TempAppUser.TableSchema, TempAppUser.Columns.CityId, City.Columns.CityId).JoinOR(AppUser.TableSchema.SchemaName, AppUser.Columns.CityId, City.Columns.CityId));
            //    qry.SelectAllTableColumns();
            //    qry.AddSelect(City.TableSchema.SchemaName, City.Columns.CityName, City.Columns.CityName);
            //    qry.AddWhere(BidMessage.Columns.IsActive, true);
            //    qry.AddWhere(BidMessage.Columns.SupplierId, SupplierId);
            //    var wl = new WhereList();
            //    wl.OR(BidMessage.Columns.Stage, WhereComparision.NotEqualsTo, BIdMessageController.START_STAGE);
            //    wl.OR(BidMessage.Columns.Stage, WhereComparision.NotEqualsTo, BIdMessageController.ADMIN_STAGE);
            //    qry.AND(wl);
            //    qry.OrderBy(Bid.Columns.EndDate, SortDirection.ASC);

            //    using (DataReaderBase reader = qry.ExecuteReader())
            //    {
            //        while (reader.Read())
            //        {
            //            //TimeSpan diff = (Convert.ToDateTime(reader[Bid.Columns.EndDate]) - DateTime.UtcNow);
            //            //string EndBid = "00:00";
            //            //if (reader[Bid.Columns.EndDate] != null)
            //            //{
            //            //    string hours = (Math.Truncate(diff.TotalHours) < 10) ? "0" + Math.Truncate(diff.TotalHours).ToString() : Math.Truncate(diff.TotalHours).ToString();
            //            //    string minutes = (((int)((diff.TotalHours - Math.Truncate(diff.TotalHours)) * 60)) < 10) ? "0" + ((int)((diff.TotalHours - Math.Truncate(diff.TotalHours)) * 60)).ToString() : ((int)((diff.TotalHours - Math.Truncate(diff.TotalHours)) * 60)).ToString();
            //            //    EndBid = hours + ":" + minutes;
            //            //}
            //            //else
            //            //    EndBid = "00:00";
            //            lstMainBid.Add(new MainBid
            //                {
            //                    BidId = reader[BidMessage.Columns.BidId] != null ? Convert.ToInt64(reader[Bid.Columns.BidId]) : 0,
            //                    City = reader[City.Columns.CityName] != null ? reader[City.Columns.CityName].ToString() : "",
            //                    EndBid = reader[BidMessage.Columns.ExpirationTime].ToString(),
            //                    DateOrder = reader[BidMessage.Columns.ExpirationTime] != null ? Convert.ToDateTime(reader[BidMessage.Columns.ExpirationTime]) : DateTime.MinValue,
            //                    IsService = false
            //                });

            //        }
            //    }
            //    //inner.Where(SupplierProduct.Columns.SupplierId, SupplierId);
            //    //inner.Select(SupplierProduct.Columns.ProductId);

            //    ////select all the bids that the supplier doesn't supply one of its products
            //    //Query notBidInner = new Query(BidProduct.TableSchema);
            //    //notBidInner.Where(BidProduct.Columns.ProductId, WhereComparision.NotIn, inner);
            //    //notBidInner.Select(BidProduct.Columns.BidId);

            //    //Query bidInner = new Query(Bid.TableSchema);
            //    //bidInner.Select(BidProduct.Columns.BidId);
            //    //bidInner.Where(Bid.Columns.BidId, WhereComparision.NotIn, notBidInner);

            //    //Query BidOffer = new Query(Offer.TableSchema);
            //    //BidOffer.Where(Offer.Columns.SupplierId, SupplierId);
            //    //BidOffer.Select(Offer.Columns.BidId).Distinct();


            //    // Query SupplierCityQ = new Query(SupplierCity.TableSchema).Where(SupplierCity.Columns.SupplierId, SupplierId).Select(SupplierCity.Columns.CityId);

            //    //Query qry = new Query(Bid.TableSchema);
            //    //qry.Join(JoinType.LeftJoin, Bid.TableSchema, Bid.Columns.AppUserId, Bid.TableSchema.SchemaName,
            //    //    AppUser.TableSchema, AppUser.Columns.AppUserId, AppUser.TableSchema.SchemaName);
            //    //qry.Join(JoinType.LeftJoin, Bid.TableSchema, Bid.Columns.TempAppUserId, Bid.TableSchema.SchemaName, TempAppUser.TableSchema, TempAppUser.Columns.TempAppUserId, TempAppUser.TableSchema.SchemaName);
            //    //qry.Join(JoinType.LeftJoin, City.TableSchema, City.TableSchema.SchemaName,
            //    //new JoinColumnPair(TempAppUser.TableSchema, TempAppUser.Columns.CityId, City.Columns.CityId).JoinOR(AppUser.TableSchema.SchemaName, AppUser.Columns.CityId, City.Columns.CityId));
            //    //qry.SelectAllTableColumns();
            //    //qry.AddSelect(City.TableSchema.SchemaName, City.Columns.CityName, City.Columns.CityName);
            //    ////qry.Where(Bid.Columns.EndDate, WhereComparision.GreaterThanOrEqual, DateTime.UtcNow);
            //    //qry.AddWhere(City.TableSchema.SchemaName, City.Columns.CityId, WhereComparision.In, SupplierCityQ);
            //    //qry.AddWhere(Bid.Columns.BidId, WhereComparision.NotIn, BidOffer);
            //    //qry.AddWhere(Bid.Columns.BidId, WhereComparision.In, bidInner);
            //    //qry.OrderBy(Bid.Columns.EndDate, SortDirection.ASC);

            //    //using (DataReaderBase reader = qry.ExecuteReader())
            //    //{
            //    //    while (reader.Read())
            //    //    {
            //    //        TimeSpan diff = (Convert.ToDateTime(reader[Bid.Columns.EndDate]) - DateTime.UtcNow);
            //    //        string EndBid = "00:00";
            //    //        if (reader[Bid.Columns.EndDate] != null)
            //    //        {
            //    //            string hours = (Math.Truncate(diff.TotalHours) < 10) ? "0" + Math.Truncate(diff.TotalHours).ToString() : Math.Truncate(diff.TotalHours).ToString();
            //    //            string minutes = (((int)((diff.TotalHours - Math.Truncate(diff.TotalHours)) * 60)) < 10) ? "0" + ((int)((diff.TotalHours - Math.Truncate(diff.TotalHours)) * 60)).ToString() : ((int)((diff.TotalHours - Math.Truncate(diff.TotalHours)) * 60)).ToString();
            //    //            EndBid = hours + ":" + minutes;
            //    //        }
            //    //        else
            //    //            EndBid = "00:00";
            //    //        lstMainBid.Add(new MainBid
            //    //            {
            //    //                BidId = reader[Bid.Columns.BidId] != null ? Convert.ToInt64(reader[Bid.Columns.BidId]) : 0,
            //    //                City = reader[City.Columns.CityName] != null ? reader[City.Columns.CityName].ToString() : "",
            //    //                EndBid = EndBid,
            //    //                DateOrder = reader[Bid.Columns.EndDate] != null ? Convert.ToDateTime(reader[Bid.Columns.EndDate]) : DateTime.MinValue,
            //    //                IsService = false
            //    //            });

            //    //    }
            //    //}



                List<Int64> serviceId = Service.FetchAllHomeServices();

                var inner = new Query(SupplierService.TableSchema);
                inner.Where(SupplierService.Columns.SupplierId, SupplierId).AddWhere(SupplierService.Columns.ServiceId, WhereComparision.NotIn, serviceId);
                inner.Select(SupplierService.Columns.ServiceId);

                var BidOffer = new Query(OfferService.TableSchema);
                BidOffer.Where(OfferService.Columns.SupplierId, SupplierId);
                BidOffer.Select(OfferService.Columns.BidId).Distinct();

                AppSupplier supplier = AppSupplier.FetchByID(SupplierId);

                Query qry1 = new Query(BidService.TableSchema);


                qry1.Join(JoinType.LeftJoin, BidService.TableSchema, BidService.Columns.AppUserId, BidService.TableSchema.SchemaName, AppUser.TableSchema, AppUser.Columns.AppUserId, AppUser.TableSchema.SchemaName);
                qry1.Join(JoinType.LeftJoin, BidService.TableSchema, BidService.Columns.TempAppUserId, BidService.TableSchema.SchemaName, TempAppUser.TableSchema, TempAppUser.Columns.TempAppUserId, TempAppUser.TableSchema.SchemaName);
                qry1.Join(JoinType.LeftJoin, City.TableSchema, City.TableSchema.SchemaName,
                new JoinColumnPair(TempAppUser.TableSchema, TempAppUser.Columns.CityId, City.Columns.CityId).JoinOR(AppUser.TableSchema.SchemaName, AppUser.Columns.CityId, City.Columns.CityId));
                qry1.SelectAllTableColumns();
                qry1.AddSelectLiteral("case when " + AppUser.Columns.AddressLocation + " is not null then " +
                    "( 6371 * acos ( cos ( radians(" + supplier.AddressLocation.X + ") ) * cos( radians( X(" + AppSupplier.Columns.AddressLocation + ") ) ) " +
                    "* cos( radians( Y(" + AppSupplier.Columns.AddressLocation + ") ) - radians(" + supplier.AddressLocation.Y + ") ) " +
                    "+ sin ( radians(" + supplier.AddressLocation.X + ") ) * sin( radians( X(" + AppSupplier.Columns.AddressLocation + ") ) ) )) else " +
                      "( 6371 * acos ( cos ( radians(" + supplier.AddressLocation.X + ") ) * cos( radians( X(" + TempAppUser.Columns.Location + ") ) ) " +
                    "* cos( radians( Y(" + TempAppUser.Columns.Location + ") ) - radians(" + supplier.AddressLocation.Y + ") ) " +
                    "+ sin ( radians(" + supplier.AddressLocation.X + ") ) * sin( radians( X(" + TempAppUser.Columns.Location + ") ) ) )) end " +
                "AS distance");
                qry1.AddSelect(City.TableSchema.SchemaName, City.Columns.CityName, City.Columns.CityName);
                qry1.Where(BidService.Columns.EndDate, WhereComparision.GreaterThanOrEqual, DateTime.UtcNow);
                qry1.AddWhere(BidService.Columns.ServiceId, WhereComparision.In, inner);
                qry1.AddWhere(BidService.Columns.BidId, WhereComparision.NotIn, BidOffer);
                qry1.OrderBy(BidService.Columns.EndDate, SortDirection.ASC);
                using (DataReaderBase reader = qry1.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        decimal distance = reader["distance"] != null ? Convert.ToDecimal(reader["distance"]) : 0;
                        if (distance > Settings.GetSettingInt32(Settings.Keys.SUPPLIER_RADIUS, 10)) continue;
                        TimeSpan diff = (Convert.ToDateTime(reader[Bid.Columns.EndDate]) - DateTime.UtcNow);
                        string EndBid = "00:00";
                        if (reader[Bid.Columns.EndDate] != null)
                        {
                            string hours = (Math.Truncate(diff.TotalHours) < 10) ? "0" + Math.Truncate(diff.TotalHours).ToString() : Math.Truncate(diff.TotalHours).ToString();
                            string minutes = (((int)((diff.TotalHours - Math.Truncate(diff.TotalHours)) * 60)) < 10) ? "0" + ((int)((diff.TotalHours - Math.Truncate(diff.TotalHours)) * 60)).ToString() : ((int)((diff.TotalHours - Math.Truncate(diff.TotalHours)) * 60)).ToString();
                            EndBid = hours + ":" + minutes;
                        }
                        else
                            EndBid = "00:00";
                        lstMainBid.Add(new MainBid
                                        {
                                            BidId = reader[BidService.Columns.BidId] != null ? Convert.ToInt64(reader[BidService.Columns.BidId]) : 0,
                                            City = reader[City.Columns.CityName] != null ? reader[City.Columns.CityName].ToString() : "",
                                            EndBid = EndBid,
                                            DateOrder = reader[BidService.Columns.EndDate] != null ? Convert.ToDateTime(reader[BidService.Columns.EndDate]) : DateTime.MinValue,
                                            IsService = true
                                        });
                    }
                }

                inner = new Query(SupplierService.TableSchema);
                inner.Where(SupplierService.Columns.SupplierId, SupplierId).AddWhere(SupplierService.Columns.ServiceId, WhereComparision.In, serviceId);
                inner.Select(SupplierService.Columns.ServiceId);

                Query SupplierCityQry = new Query(SupplierHomeServiceCity.TableSchema).Where(SupplierHomeServiceCity.Columns.SupplierId, supplier.SupplierId).Select(SupplierHomeServiceCity.Columns.CityId);

                if (!(inner.GetCount() > 0)) return lstMainBid;
                qry1 = new Query(BidService.TableSchema);
                qry1.Join(JoinType.LeftJoin, BidService.TableSchema, BidService.Columns.AppUserId, BidService.TableSchema.SchemaName, AppUser.TableSchema, AppUser.Columns.AppUserId, AppUser.TableSchema.SchemaName);
                qry1.Join(JoinType.LeftJoin, BidService.TableSchema, BidService.Columns.TempAppUserId, BidService.TableSchema.SchemaName, TempAppUser.TableSchema, TempAppUser.Columns.TempAppUserId, TempAppUser.TableSchema.SchemaName);
                qry1.Join(JoinType.LeftJoin, City.TableSchema, City.TableSchema.SchemaName,
                new JoinColumnPair(TempAppUser.TableSchema, TempAppUser.Columns.CityId, City.Columns.CityId).JoinOR(AppUser.TableSchema.SchemaName, AppUser.Columns.CityId, City.Columns.CityId));
                qry1.SelectAllTableColumns();
                qry1.AddSelect(City.TableSchema.SchemaName, City.Columns.CityName, City.Columns.CityName);
                qry1.Where(BidService.Columns.EndDate, WhereComparision.GreaterThanOrEqual, DateTime.UtcNow);
                qry1.AddWhere(BidService.Columns.ServiceId, WhereComparision.In, serviceId);
                qry1.AddWhere(BidService.Columns.BidId, WhereComparision.NotIn, BidOffer);
                qry1.AddWhere(City.TableSchema.SchemaName, City.Columns.CityId, WhereComparision.In, SupplierCityQry);
                qry1.OrderBy(BidService.Columns.EndDate, SortDirection.ASC);
                using (DataReaderBase reader = qry1.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        //decimal distance = reader["distance"] != null ? Convert.ToDecimal(reader["distance"]) : 0;
                        //if (distance > Settings.GetSettingInt32(Settings.Keys.SUPPLIER_RADIUS, 10)) continue;
                        TimeSpan diff = (Convert.ToDateTime(reader[Bid.Columns.EndDate]) - DateTime.UtcNow);
                        string EndBid = "00:00";
                        if (reader[Bid.Columns.EndDate] != null)
                        {
                            string hours = (Math.Truncate(diff.TotalHours) < 10) ? "0" + Math.Truncate(diff.TotalHours).ToString() : Math.Truncate(diff.TotalHours).ToString();
                            string minutes = (((int)((diff.TotalHours - Math.Truncate(diff.TotalHours)) * 60)) < 10) ? "0" + ((int)((diff.TotalHours - Math.Truncate(diff.TotalHours)) * 60)).ToString() : ((int)((diff.TotalHours - Math.Truncate(diff.TotalHours)) * 60)).ToString();
                            EndBid = hours + ":" + minutes;
                        }
                        else
                            EndBid = "00:00";
                        lstMainBid.Add(new MainBid
                        {
                            BidId = reader[BidService.Columns.BidId] != null ? Convert.ToInt64(reader[BidService.Columns.BidId]) : 0,
                            City = reader[City.Columns.CityName] != null ? reader[City.Columns.CityName].ToString() : "",
                            EndBid = EndBid,
                            DateOrder = reader[BidService.Columns.EndDate] != null ? Convert.ToDateTime(reader[BidService.Columns.EndDate]) : DateTime.MinValue,
                            IsService = true
                        });
                    }
                }


                return lstMainBid;
            }
            return null;
        }

        public static List<OrderUI> GetAllWinBids(Int64 SupplierId)
        {
            List<OrderUI> LstOrderUI = new List<OrderUI>();


            Query qry = new Query(Order.TableSchema);

            qry.Join(JoinType.LeftJoin, Order.TableSchema, Order.Columns.AppUserId, Order.TableSchema.SchemaName,
               AppUser.TableSchema, AppUser.Columns.AppUserId, AppUser.TableSchema.SchemaName);
            qry.Join(JoinType.InnerJoin, AppUser.TableSchema, AppUser.Columns.CityId, AppUser.TableSchema.SchemaName,
               City.TableSchema, City.Columns.CityId, City.TableSchema.SchemaName);
            qry.Where(Order.TableSchema.SchemaName, Order.Columns.SupplierId, WhereComparision.EqualsTo, SupplierId);
            qry.AddWhere(Order.TableSchema.SchemaName, Order.Columns.UserPaySupplierStatus, WhereComparision.EqualsTo, UserPaymentStatus.Payed);
            // qry.AddWhere(Order.TableSchema.SchemaName, Order.Columns.UserPaySupplierStatus, WhereComparision.EqualsTo, UserPaymentStatus.Payed);
            qry.AddWhere(Order.TableSchema.SchemaName, Order.Columns.SuppliedDate, WhereComparision.EqualsTo, null);
            qry.OrderBy(Order.TableSchema.SchemaName, Order.Columns.CreateDate, SortDirection.DESC);
            qry.Distinct();
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                while (reader.Read())
                {
                    OrderUI o = new OrderUI();
                    o.OrderDate = reader[Order.Columns.CreateDate] != null ? Convert.ToDateTime(reader[Order.Columns.CreateDate]).ToLocalTime() : DateTime.MinValue;
                    o.OrderId = reader[Order.Columns.OrderId] != null ? Convert.ToInt64(reader[Order.Columns.OrderId]) : 0;
                    o.FirstName = reader[AppUser.Columns.FirstName] != null ? reader[AppUser.Columns.FirstName].ToString() : "";
                    o.LastName = reader[AppUser.Columns.LastName] != null ? reader[AppUser.Columns.LastName].ToString() : "";
                    o.City = reader[City.Columns.CityName] != null ? reader[City.Columns.CityName].ToString() : "";
                    o.IsSupplied = reader[Order.Columns.SuppliedDate] != null && reader[Order.Columns.SuppliedDate].ToString() != "" ? true : false;
                    LstOrderUI.Add(o);

                }
            }

            return LstOrderUI;
        }

        public static List<OrderUI> GetAllWinBidsOffSupplier(Int64 SupplierId)
        {

            List<OrderUI> LstOrderUI = new List<OrderUI>();

            Query qry = new Query(Order.TableSchema);

            qry.Join(JoinType.LeftJoin, Order.TableSchema, Order.Columns.AppUserId, Order.TableSchema.SchemaName,
               AppUser.TableSchema, AppUser.Columns.AppUserId, AppUser.TableSchema.SchemaName);
            qry.Join(JoinType.InnerJoin, AppUser.TableSchema, AppUser.Columns.CityId, AppUser.TableSchema.SchemaName,
               City.TableSchema, City.Columns.CityId, City.TableSchema.SchemaName);
            qry.Where(Order.TableSchema.SchemaName, Order.Columns.SupplierId, WhereComparision.EqualsTo, SupplierId);
            qry.AddWhere(Order.TableSchema.SchemaName, Order.Columns.SuppliedDate, WhereComparision.EqualsTo, null);
            qry.OrderBy(Order.TableSchema.SchemaName, Order.Columns.CreateDate, SortDirection.DESC);
            qry.Distinct();
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                while (reader.Read())
                {
                    OrderUI o = new OrderUI();
                    o.OrderDate = reader[Order.Columns.CreateDate] != null ? Convert.ToDateTime(reader[Order.Columns.CreateDate]).ToLocalTime() : DateTime.MinValue;
                    o.OrderId = reader[Order.Columns.OrderId] != null ? Convert.ToInt64(reader[Order.Columns.OrderId]) : 0;
                    o.FirstName = reader[AppUser.Columns.FirstName] != null ? reader[AppUser.Columns.FirstName].ToString() : "";
                    o.LastName = reader[AppUser.Columns.LastName] != null ? reader[AppUser.Columns.LastName].ToString() : "";
                    o.City = reader[City.Columns.CityName] != null ? reader[City.Columns.CityName].ToString() : "";
                    o.IsSupplied = reader[Order.Columns.SuppliedDate] != null && reader[Order.Columns.SuppliedDate].ToString() != "" ? true : false;
                    LstOrderUI.Add(o);

                }
            }

            return LstOrderUI;
        }

        public static CommentCollection GetCommentBySupplierId(Int64 SupplierId)
        {
            Query q = new Query(Comment.TableSchema);
            q.Where(Comment.Columns.SupplierId, SupplierId);
            q.AddWhere(Comment.Columns.Status, CommentStatus.Approved);
            return CommentCollection.FetchByQuery(q);
        }

        public static List<City> GetCitiesBySupplier(Int64 supplierID)
        {
            Query qry = new Query(City.TableSchema);
            qry.Join(JoinType.InnerJoin, City.TableSchema,
            City.Columns.CityId, City.TableSchema.SchemaName,
            SupplierCity.TableSchema, SupplierCity.Columns.CityId,
            SupplierCity.TableSchema.SchemaName);
            qry.Where(SupplierCity.TableSchema.SchemaName,
            SupplierCity.Columns.SupplierId, WhereComparision.EqualsTo,
            supplierID);
            qry.OrderBy(City.Columns.CityName, SortDirection.ASC);
            List<City> list = new List<City>();
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                while (reader.Read())
                {
                    list.Add(new City
                    {
                        CityName = Convert.ToString(reader["CityName"]),
                        CityId = Convert.ToInt64(reader["CityId"]),
                        AreaId = Convert.ToInt64(reader["AreaId"])
                    });
                }
            }
            return list;
        }

        public static List<City> GetCitiesHomeServiceBySupplier(Int64 SupplierID)
        {
            Query qry = new Query(City.TableSchema);
            qry.Join(JoinType.InnerJoin, City.TableSchema,
            City.Columns.CityId, City.TableSchema.SchemaName,
            SupplierHomeServiceCity.TableSchema,
            SupplierHomeServiceCity.Columns.CityId,
            SupplierHomeServiceCity.TableSchema.SchemaName);
            qry.Where(SupplierHomeServiceCity.TableSchema.SchemaName,
                    SupplierHomeServiceCity.Columns.SupplierId, WhereComparision.EqualsTo, SupplierID);
            qry.OrderBy(City.Columns.CityName, SortDirection.ASC);
            List<City> list = new List<City>();
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                while (reader.Read())
                {
                    list.Add(new City
                    {
                        CityName = Convert.ToString(reader["CityName"]),
                        CityId = Convert.ToInt64(reader["CityId"]),
                        AreaId = Convert.ToInt64(reader["AreaId"])
                    });
                }
            }
            return list;
        }

        public static List<Service> GetServicesBySupplier(Int64 supplierID)
        {
            Query qry = new Query(Service.TableSchema);
            qry.Join(JoinType.InnerJoin, Service.TableSchema, Service.Columns.ServiceId, Service.TableSchema.SchemaName, SupplierService.TableSchema, SupplierService.Columns.ServiceId, SupplierService.TableSchema.SchemaName);
            qry.Where(SupplierService.TableSchema.SchemaName, SupplierService.Columns.SupplierId, WhereComparision.EqualsTo, supplierID);
            List<Service> list = new List<Service>();
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                while (reader.Read())
                {
                    list.Add(new Service
                    {
                        ServiceName = Convert.ToString(reader["ServiceName"]),
                        ServiceId = Convert.ToInt64(reader["ServiceId"]),
                        ServiceComment = Convert.ToString(reader["ServiceComment"]),
                        IsHomeService = Convert.ToBoolean(reader["IsHomeService"])
                    });
                }
            }
            return list;
        }

        public static SupplierUI GetSupplierForAppById(Int64 SupplierId)
        {
            AppSupplier supplier = AppSupplier.FetchByID(SupplierId);
            CommentCollection c = GetCommentBySupplierId(SupplierId);
            SupplierUI supplierUI = new SupplierUI();
            supplierUI.BusinessName = supplier.BusinessName;
            supplierUI.CityName = City.FetchByID(supplier.CityId).CityName;
            supplierUI.Street = supplier.Street;
            supplierUI.HouseNum = supplier.HouseNum;
            supplierUI.SupplierId = supplier.SupplierId;
            supplierUI.IsService = supplier.IsService;
            supplierUI.LstComment = c;
            supplierUI.AvgRate = (c != null && c.Count > 0 ? c.Average(r => r.Rate) : 0);
            supplierUI.ProfileImage = supplier.ProfileImage;
            supplierUI.Description = supplier.Description;
            supplierUI.Discount = supplier.Discount;
            supplierUI.Phone = supplier.Phone;

            return supplierUI;
        }

        public static List<SupplierUI> GetAllSuppliersUI(bool IsSearch = false, string SearchName = "", string SearchPhone = "", int PageSize = 0, int CurrentPageIndex = 0)
        {

            Query qry = new Query(AppSupplier.TableSchema);
            qry.SelectAllTableColumns();
            qry.Join(JoinType.LeftJoin, AppSupplier.TableSchema, AppSupplier.Columns.SupplierId, AppSupplier.TableSchema.SchemaName, Comment.TableSchema, Comment.Columns.SupplierId, Comment.TableSchema.SchemaName);
            qry.AddSelectLiteral(" case when avg(" + Comment.Columns.Rate + ") is null then 0 else avg(" + Comment.Columns.Rate + ") end as AvgRate");
            qry.GroupBy(AppSupplier.Columns.UniqueIdString);
            //qry.AddSelectLiteral("  case when count(case when Approve is true then " + Comment.Columns.CommentId + " end) is null then 0 else count(case when Approve is true then " + Comment.Columns.CommentId + " end) end as CommentCount");
            qry.Join(JoinType.LeftJoin, AppSupplier.TableSchema, AppSupplier.Columns.CityId, AppSupplier.TableSchema.SchemaName, City.TableSchema, City.Columns.CityId, City.TableSchema.SchemaName);
            qry.AddSelect(City.TableSchema.SchemaName, City.Columns.CityName, "CityName");
            qry.Distinct();
            qry.Where(AppSupplier.Columns.IsDeleted, false);
            if (IsSearch == true)
            {
                WhereList wl = new WhereList();
                wl.OR(AppSupplier.Columns.ContactPhone, WhereComparision.Like, SearchPhone)
                   .OR(AppSupplier.Columns.Phone, WhereComparision.Like, SearchPhone);
                qry.AND(wl);
                qry.AddWhere(AppSupplier.Columns.ContactName, WhereComparision.Like, SearchName);
            }
            if (PageSize > 0)
            {
                qry.LimitRows(PageSize).OffsetRows(PageSize * CurrentPageIndex);
            }

            List<SupplierUI> list = new List<SupplierUI>();
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                while (reader.Read())
                {
                    list.Add(new SupplierUI
                    {
                        SupplierId = Convert.ToInt64(reader["SupplierId"]),
                        BusinessName = Convert.ToString(reader["BusinessName"]),
                        Email = Convert.ToString(reader["Email"]),
                        ContactPhone = Convert.ToString(reader["ContactPhone"]),
                        Phone = Convert.ToString(reader["Phone"]),
                        ContactName = Convert.ToString(reader["ContactName"]),
                        CityId = Convert.ToInt64(reader["CityId"]),
                        CityName = Convert.ToString(reader["CityName"]),
                        Street = Convert.ToString(reader["Street"]),
                        HouseNum = Convert.ToString(reader["HouseNum"]),
                        Precent = Convert.ToInt32(reader["Precent"]),
                        SumPerMonth = Convert.ToInt32(reader["SumPerMonth"]),
                        CreateDate = Convert.ToDateTime(reader["CreateDate"]).ToLocalTime(),
                        MaxWinningsNum = Convert.ToInt32(reader["MaxWinningsNum"]),
                        AvgRate = (reader["AvgRate"] is DBNull) ? 0 : Convert.ToInt64(Math.Floor(Convert.ToDouble(reader["AvgRate"]))),
                        //CommentCount = (reader["CommentCount"] is DBNull) ? 0 : Convert.ToInt64(reader["CommentCount"]),
                        citiesSupplied = GetCitiesBySupplier(Convert.ToInt64(reader["SupplierId"])),
                        citiesHomeService = GetCitiesHomeServiceBySupplier(Convert.ToInt64(reader["SupplierId"])),
                        IsProduct = Convert.ToBoolean(reader[AppSupplier.Columns.IsProduct])
                    });
                }
            }
            return list;
        }

        public static List<SupplierUI> GetSuppliersAndNumBids(string filterSearch, string SearchName = "", string SearchPhone = "", string SearchId = "", string SearchCity = "", Int64 searchBidId = -1, DateTime from = new DateTime(), DateTime to = new DateTime(), bool IsSearch = false, int PageSize = 0, int CurrentPageIndex = 0)
        {

            Query qry = new Query(AppSupplier.TableSchema);


            qry.SelectAll();

            qry.Join(JoinType.LeftJoin, AppSupplier.TableSchema, AppSupplier.Columns.SupplierId, AppSupplier.TableSchema.SchemaName, Order.TableSchema, Order.Columns.SupplierId, Order.TableSchema.SchemaName);
            qry.SelectAllTableColumns();
            qry.AddSelectLiteral(" (select count(" + Order.TableSchema.SchemaName + "." + Order.Columns.SupplierId + ") from " + Bid.TableSchema.SchemaName +
                " LEFT JOIN  " + Order.TableSchema.SchemaName + " ON " + Order.TableSchema.SchemaName + "." + Order.Columns.BidId + " = " + Bid.TableSchema.SchemaName + "." + Bid.Columns.BidId +
                " where " + Order.TableSchema.SchemaName + "." + Order.Columns.SupplierId + "=" + AppSupplier.TableSchema.SchemaName + "." + AppSupplier.Columns.SupplierId +
                 (from != DateTime.MinValue ? " AND  " + Bid.Columns.EndDate + " >= '" + from.ToString("yyyy-MM-dd") + "' " : "") + (to != DateTime.MinValue ? " AND  " + Bid.Columns.EndDate + " <= '" + to.ToString("yyyy-MM-dd") + "' " : "") +
                ") as 'BidsCount' ");

            qry.AddSelectLiteral(" (select count(" + Order.TableSchema.SchemaName + "." + Order.Columns.SupplierId + ") from " + Order.TableSchema.SchemaName +
                " LEFT JOIN  " + Bid.TableSchema.SchemaName + " ON " + Order.TableSchema.SchemaName + "." + Order.Columns.BidId + " = " + Bid.TableSchema.SchemaName + "." + Bid.Columns.BidId +
                " where " + Order.TableSchema.SchemaName + "." + Order.Columns.SupplierId + "=" + AppSupplier.TableSchema.SchemaName + "." + AppSupplier.Columns.SupplierId +
                " AND " + Order.TableSchema.SchemaName + "." + Order.Columns.OrderId + " is not null " +
                (from != DateTime.MinValue ? " AND " + Bid.Columns.EndDate + " >= '" + from.ToString("yyyy-MM-dd") + "' " : "") + (to != DateTime.MinValue ? " AND  " + Bid.Columns.EndDate + " <= '" + to.ToString("yyyy-MM-dd") + "' " : "") +
                ") as 'OrdersCount' ");
            qry.Join(JoinType.LeftJoin, Bid.TableSchema, "b",
                    new JoinColumnPair(Order.TableSchema.SchemaName, Order.Columns.BidId, Bid.Columns.BidId));
            qry.AddSelectLiteral(" count(CASE WHEN b." + Bid.Columns.EndDate + " > utc_timestamp()  " + (from != DateTime.MinValue ? " AND " + Bid.Columns.EndDate + " >= '" + from.ToString("yyyy-MM-dd") + "' " : "") + (to != DateTime.MinValue ? " AND  " + Bid.Columns.EndDate + " <= '" + to.ToString("yyyy-MM-dd") + "' " : "") + " THEN 1 END) as 'ActiveCount' ");
            qry.Join(JoinType.InnerJoin, AppSupplier.TableSchema, AppSupplier.Columns.CityId, AppSupplier.TableSchema.SchemaName, City.TableSchema, City.Columns.CityId, City.TableSchema.SchemaName);
            qry.AddSelect(City.TableSchema.SchemaName, City.Columns.CityName, "CityName");
            //            qry.SelectAllTableColumns();
            qry.Distinct();
            qry.Where(AppSupplier.Columns.IsDeleted, false);
            qry.GroupBy(AppSupplier.Columns.UniqueIdString);
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
                if (searchBidId != -1)
                    qry.AddWhere(Order.TableSchema.SchemaName, Order.Columns.BidId, WhereComparision.EqualsTo, searchBidId);
                if (from != DateTime.MinValue)
                {
                    qry.AddWhere(Bid.Columns.EndDate, WhereComparision.GreaterThanOrEqual, from.Date);
                }
                if (to != DateTime.MinValue)
                {
                    qry.AddWhere(Bid.Columns.EndDate, WhereComparision.LessThanOrEqual, to.AddHours(24).Date);
                }
            }
            //lblSumOffers.Text = coll.Sum(b => b.OfferNum).ToString();
            //lblSumWin.Text = coll.Sum(b => b.OrderNum).ToString();
            //lblSumNoWin.Text = (coll.Sum(b => b.OfferNum) - coll.Sum(b => b.OrderNum)).ToString();
            //lblSumActiveBids.Text = coll.Sum(b => b.ActiveNum).ToString();

            if (filterSearch == "Win")
            {

                qry.Having(" (select count(" + Order.TableSchema.SchemaName + "." + Order.Columns.SupplierId + ") from " + Order.TableSchema.SchemaName +
                " LEFT JOIN  " + Bid.TableSchema.SchemaName + " ON " + Order.TableSchema.SchemaName + "." + Order.Columns.BidId + " = " + Bid.TableSchema.SchemaName + "." + Bid.Columns.BidId +
                " where " + Order.TableSchema.SchemaName + "." + Order.Columns.SupplierId + "=" + AppSupplier.TableSchema.SchemaName + "." + AppSupplier.Columns.SupplierId +
                " AND " + Order.TableSchema.SchemaName + "." + Order.Columns.OrderId + " is not null " +
                (from != DateTime.MinValue ? " AND " + Bid.Columns.EndDate + " >= '" + from.ToString("yyyy-MM-dd") + "' " : "") + (to != DateTime.MinValue ? " AND  " + Bid.Columns.EndDate + " <= '" + to.ToString("yyyy-MM-dd") + "' " : "") +
                ") >0 ");

            }
            else if (filterSearch == "NoWin")
            {

                qry.Having(" (select count(" + Order.TableSchema.SchemaName + "." + Order.Columns.SupplierId + ") from " + Order.TableSchema.SchemaName +
           " LEFT JOIN  " + Bid.TableSchema.SchemaName + " ON " + Order.TableSchema.SchemaName + "." + Order.Columns.BidId + " = " + Bid.TableSchema.SchemaName + "." + Bid.Columns.BidId +
           " where " + Order.TableSchema.SchemaName + "." + Order.Columns.SupplierId + "=" + AppSupplier.TableSchema.SchemaName + "." + AppSupplier.Columns.SupplierId +
           " AND " + Order.TableSchema.SchemaName + "." + Order.Columns.OrderId + " is not null " +
           (from != DateTime.MinValue ? " AND " + Bid.Columns.EndDate + " >= '" + from.ToString("yyyy-MM-dd") + "' " : "") + (to != DateTime.MinValue ? " AND  " + Bid.Columns.EndDate + " <= '" + to.ToString("yyyy-MM-dd") + "' " : "") +
           ") =0 ");
            }
            else if (filterSearch == "ActiveBids")
            {
                //b => b.OrderDate != (DateTime?)null
                qry.Having(" count(CASE WHEN b." + Bid.Columns.EndDate + " > utc_timestamp()  " + (from != DateTime.MinValue ? " AND " + Bid.Columns.EndDate + " >= '" + from.ToString("yyyy-MM-dd") + "' " : "") + (to != DateTime.MinValue ? " AND  " + Bid.Columns.EndDate + " <= '" + to.ToString("yyyy-MM-dd") + "' " : "") + " THEN 1 END) >0 ");
            }
            if (PageSize > 0)
            {
                qry.LimitRows(PageSize).OffsetRows(PageSize * CurrentPageIndex);
            }
            qry.OrderBy(AppSupplier.TableSchema.SchemaName, AppSupplier.Columns.CreateDate, SortDirection.DESC);
            List<SupplierUI> list = new List<SupplierUI>();
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                while (reader.Read())
                {
                    Query q = new Query(Order.TableSchema).Join(JoinType.LeftJoin, Bid.TableSchema, Bid.TableSchema.SchemaName,
                    new JoinColumnPair(Order.TableSchema.SchemaName, Order.Columns.BidId, Bid.Columns.BidId));
                    q.SelectAllTableColumns();
                    q.Where(Order.Columns.SupplierId, Convert.ToInt64(reader["SupplierId"]))
                    .AddWhere(Bid.Columns.EndDate, WhereComparision.GreaterThanOrEqual, DateTime.UtcNow);
                    if (from != DateTime.MinValue) q.AddWhere(Bid.Columns.EndDate, WhereComparision.GreaterThanOrEqual, from.Date);
                    if (to != DateTime.MinValue) q.AddWhere(Bid.Columns.EndDate, WhereComparision.LessThanOrEqual, to.AddHours(24).Date);
                    Int64 ActiveOrder = q.GetCount(Order.Columns.OrderId);

                    list.Add(new SupplierUI
                    {
                        SupplierId = Convert.ToInt64(reader["SupplierId"]),
                        BusinessName = Convert.ToString(reader["BusinessName"]),
                        ContactPhone = Convert.ToString(reader["ContactPhone"]),
                        Phone = Convert.ToString(reader["Phone"]),
                        ContactName = Convert.ToString(reader["ContactName"]),
                        CityId = Convert.ToInt64(reader["CityId"]),
                        CityName = Convert.ToString(reader["CityName"]),
                        OrderNum = reader["OrdersCount"] is DBNull ? 0 : Convert.ToInt32(reader["OrdersCount"]),
                        BidNum = reader["BidsCount"] is DBNull ? 0 : Convert.ToInt32(reader["BidsCount"]),
                        ActiveNum = reader["ActiveCount"] is DBNull ? 0 : Convert.ToInt32(reader["ActiveCount"]),
                        ActiveOrder = ActiveOrder,
                    });
                }
            }
            return list;
        }


        public static SupplierUI GetSupplierUIForSupplierProfile(Int64 SupplierId)
        {

            Query qry = new Query(AppSupplier.TableSchema);
            qry.Join(JoinType.LeftJoin, AppSupplier.TableSchema, AppSupplier.Columns.CityId, AppSupplier.TableSchema.SchemaName, City.TableSchema, City.Columns.CityId, City.TableSchema.SchemaName);
            qry.Where(AppSupplier.Columns.SupplierId, SupplierId);
            qry.AddWhere(AppSupplier.Columns.IsDeleted, false);

            using (DataReaderBase reader = qry.ExecuteReader())
            {
                while (reader.Read())
                {
                    SupplierUI supplier = new SupplierUI
                    {
                        StatusJoinBid = Convert.ToBoolean(reader["StatusJoinBid"]),
                        AllowChangeStatusJoinBid = Convert.ToBoolean(reader["AllowChangeStatusJoinBid"]),
                        MaxWinningsNum = Convert.ToInt32(reader["MaxWinningsNum"]),
                        citiesSupplied = GetCitiesBySupplier(Convert.ToInt64(reader["SupplierId"])),
                        citiesHomeService = GetCitiesHomeServiceBySupplier(Convert.ToInt64(reader["SupplierId"])),
                        LstComment = GetCommentBySupplierId(SupplierId),
                        LstServices = GetServicesBySupplier(SupplierId),
                        CityName = Convert.ToString(reader["CityName"]),
                        SupplierId = Convert.ToInt64(reader["SupplierId"]),
                        BusinessName = Convert.ToString(reader["BusinessName"]),
                        Email = Convert.ToString(reader["Email"]),
                        IsLocked = Convert.ToBoolean(reader["IsLocked"]),
                        IsService = Convert.ToBoolean(reader["IsService"]),
                        IsProduct = Convert.ToBoolean(reader["IsProduct"]),
                        IsPremium = Convert.ToBoolean(reader["IsPremium"]),
                        ContactPhone = Convert.ToString(reader["ContactPhone"]),
                        Phone = Convert.ToString(reader["Phone"]),
                        ContactName = Convert.ToString(reader["ContactName"]),
                        CityId = Convert.ToInt64(reader["CityId"]),
                        Street = Convert.ToString(reader["Street"]),
                        HouseNum = Convert.ToString(reader["HouseNum"]),
                        ProfileImage= Convert.ToString(reader["ProfileImage"]),
                        Description = Convert.ToString(reader["Description"]),
                        Discount = Convert.ToString(reader["Discount"]),
                        ApprovedTermsDate = reader[AppSupplier.Columns.ApprovedTermsDate] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader[AppSupplier.Columns.ApprovedTermsDate])

                    };
                    return supplier;
                 }
            }
            return null;
        }
        public static List<AppSupplier> GetAllSuppliers()
        {
            Query q = new Query(AppSupplier.TableSchema)
                        .Where(AppSupplier.Columns.IsDeleted, false);

            List<AppSupplier> list = new List<AppSupplier>();
            using (DataReaderBase reader = q.ExecuteReader())
            {
                while (reader.Read())
                {
                    list.Add(new AppSupplier
                    {
                        SupplierId = Convert.ToInt64(reader["SupplierId"]),
                        BusinessName = Convert.ToString(reader["BusinessName"]),
                        Email = Convert.ToString(reader["Email"]),
                        IsLocked = Convert.ToBoolean(reader["IsLocked"]),
                        IsService = Convert.ToBoolean(reader["IsService"]),
                        IsProduct = Convert.ToBoolean(reader["IsProduct"]),
                        IsPremium = Convert.ToBoolean(reader["IsPremium"]),
                        ContactPhone = Convert.ToString(reader["ContactPhone"]),
                        Phone = Convert.ToString(reader["Phone"]),
                        ContactName = Convert.ToString(reader["ContactName"]),
                        CityId = Convert.ToInt64(reader["CityId"]),

                        Street = Convert.ToString(reader["Street"]),
                        HouseNum = Convert.ToString(reader["HouseNum"]),
                        Precent = Convert.ToInt32(reader["Precent"]),
                        SumPerMonth = Convert.ToInt32(reader["SumPerMonth"]),
                        CreateDate = Convert.ToDateTime(reader["CreateDate"]).ToLocalTime(),


                        MaxWinningsNum = Convert.ToInt32(reader["MaxWinningsNum"]),
                    });
                }
            }
            return list;
        }

        public static SupplierUI GetSupplierUI(Int64 SupplierId)
        {

            Query qry = new Query(AppSupplier.TableSchema);
            qry.Join(JoinType.LeftJoin, AppSupplier.TableSchema, AppSupplier.Columns.CityId, AppSupplier.TableSchema.SchemaName, City.TableSchema, City.Columns.CityId, City.TableSchema.SchemaName);
            qry.Where(AppSupplier.Columns.SupplierId, SupplierId);
            qry.AddWhere(AppSupplier.Columns.IsDeleted, false);

            using (DataReaderBase reader = qry.ExecuteReader())
            {
                while (reader.Read())
                {
                    SupplierUI supplier = new SupplierUI
                    {
                        SupplierId = Convert.ToInt64(reader["SupplierId"]),
                        BusinessName = Convert.ToString(reader["BusinessName"]),
                        Email = Convert.ToString(reader["Email"]),
                        IsLocked = Convert.ToBoolean(reader["IsLocked"]),
                        IsService = Convert.ToBoolean(reader["IsService"]),
                        IsProduct = Convert.ToBoolean(reader["IsProduct"]),
                        IsPremium = Convert.ToBoolean(reader["IsPremium"]),
                        ContactPhone = Convert.ToString(reader["ContactPhone"]),
                        Phone = Convert.ToString(reader["Phone"]),
                        ContactName = Convert.ToString(reader["ContactName"]),
                        CityId = Convert.ToInt64(reader["CityId"]),
                        CityName = Convert.ToString(reader["CityName"]),
                        Street = Convert.ToString(reader["Street"]),
                        HouseNum = Convert.ToString(reader["HouseNum"]),
                        Precent = Convert.ToInt32(reader["Precent"]),
                        SumPerMonth = Convert.ToInt32(reader["SumPerMonth"]),
                        CreateDate = Convert.ToDateTime(reader["CreateDate"]).ToLocalTime(),
                        StatusJoinBid = Convert.ToBoolean(reader["StatusJoinBid"]),
                        AllowChangeStatusJoinBid = Convert.ToBoolean(reader["AllowChangeStatusJoinBid"]),
                        MaxWinningsNum = Convert.ToInt32(reader["MaxWinningsNum"]),
                        MastercardCode = Convert.ToString(reader["MastercardCode"]),
                        //citiesSupplied = GetCitiesBySupplier(Convert.ToInt64(reader["SupplierId"])),
                        //citiesHomeService = GetCitiesHomeServiceBySupplier(Convert.ToInt64(reader["SupplierId"])),
                        LstComment = GetCommentBySupplierId(SupplierId),
                        LstServices = GetServicesBySupplier(SupplierId),
                        ProfileImage = Convert.ToString(reader["ProfileImage"])
                    };
                    return supplier;
                }
            }
            return null;
        }

        public static bool IsSelectedService(Int64 SupplierId, Int64 ServiceId)
        {
            Query q = new Query(SupplierService.TableSchema)
                     .Select(SupplierService.Columns.ServiceId)
                     .Where(SupplierService.Columns.SupplierId, SupplierId)
                     .AddWhere(SupplierService.Columns.ServiceId, ServiceId);
            return (q.GetCount() > 0);
        }

        public static int DeleteAllSupplierServices(Int64 SupplierId)
        {
            Query qry = new Query(SupplierService.TableSchema)
                .Delete().Where(SupplierService.Columns.SupplierId, SupplierId);
            return qry.Execute();
        }
    }

}

