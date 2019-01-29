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

    public class OfferServiceUI
    {
        public Int64 OfferId { get; set; }
        public string SupplierName { get; set; }
        public Int64 SupplierId { get; set; }
        public string Remarks { get; set; }
        public string Address { get {
            return City + " " + Street + " " + Number;
        } }

        public string City { get; set; }
        public string Street { get; set; }
        public string Number { get; set; }

        public string Phone { get; set; }

        public string Price { get; set; }

    }

    public class OfferServiceController
    {

        public static OfferServiceUI GetOfferByOfferId(Int64 OfferId)
        {
            Query q = new Query(OfferService.TableSchema);
            q.Join(JoinType.InnerJoin, OfferService.TableSchema, OfferService.Columns.SupplierId, OfferService.TableSchema.SchemaName,
                AppSupplier.TableSchema, AppSupplier.Columns.SupplierId, AppSupplier.TableSchema.SchemaName);
            q.Where(OfferService.Columns.OfferId, WhereComparision.EqualsTo, OfferId);
            q.SelectAll();
            OfferServiceUI offerServiceUI = new OfferServiceUI();
            using (DataReaderBase reader = q.ExecuteReader())
            {
                while (reader.Read())
                {
                    City c = City.FetchByID(reader[AppSupplier.Columns.CityId] != null ? (Int64)(reader[AppSupplier.Columns.CityId]) : 0);
                    offerServiceUI.SupplierId = (reader[OfferService.Columns.SupplierId] != null ? (Int64)reader[OfferService.Columns.SupplierId] : 0);
                    offerServiceUI.OfferId = (reader[OfferService.Columns.OfferId] != null ? (Int64)reader[OfferService.Columns.OfferId] : 0);
                    offerServiceUI.SupplierName = (reader[AppSupplier.Columns.BusinessName] != null ? (reader[AppSupplier.Columns.BusinessName]).ToString() : "");
                    offerServiceUI.Price = (reader[OfferService.Columns.Price] != null ? reader[OfferService.Columns.Price].ToString() : "");
                    offerServiceUI.City = (c != null ? c.CityName : "");
                    offerServiceUI.Street = (reader[AppSupplier.Columns.Street] != null ? (reader[AppSupplier.Columns.Street]).ToString() : "");
                    offerServiceUI.Number = (reader[AppSupplier.Columns.HouseNum] != null ? (reader[AppSupplier.Columns.HouseNum]).ToString() : "");
                    offerServiceUI.Phone = (reader[AppSupplier.Columns.Phone] != null ? (reader[AppSupplier.Columns.Phone]).ToString() : "");
                    offerServiceUI.Remarks = (reader[OfferService.Columns.SupplierRemarks] != null ? (reader[OfferService.Columns.SupplierRemarks]).ToString() : "");   
                }
            }
            return offerServiceUI;
        }
        public static List<OfferServiceUI> GetAllOfferByBidId(Int64 BidId, DateTime EndBid,Geometry.Point point)
        {
            Query q = new Query(OfferService.TableSchema);
          
            q.Join(JoinType.InnerJoin, OfferService.TableSchema, OfferService.Columns.SupplierId, OfferService.TableSchema.SchemaName,
                AppSupplier.TableSchema, AppSupplier.Columns.SupplierId, AppSupplier.TableSchema.SchemaName);
            q.SelectAllTableColumns();
            q.AddSelect(AppSupplier.TableSchema.SchemaName, AppSupplier.Columns.CityId, AppSupplier.Columns.CityId);
            q.AddSelect(AppSupplier.TableSchema.SchemaName, AppSupplier.Columns.BusinessName, AppSupplier.Columns.BusinessName);
            q.AddSelect(AppSupplier.TableSchema.SchemaName, AppSupplier.Columns.Street, AppSupplier.Columns.Street);
            q.AddSelect(AppSupplier.TableSchema.SchemaName, AppSupplier.Columns.HouseNum, AppSupplier.Columns.HouseNum);
            q.AddSelect(AppSupplier.TableSchema.SchemaName, AppSupplier.Columns.Phone, AppSupplier.Columns.Phone);
            q.Where(OfferService.Columns.BidId, WhereComparision.EqualsTo, BidId);
            q.AddWhere(OfferService.TableSchema.SchemaName , OfferService.Columns.CreateDate, WhereComparision.LessThanOrEqual, EndBid.AddHours(Settings.GetSettingInt32(Settings.Keys.EXPIRY_OFFER_TIME_HOURS, 24)));
            q.AddSelectLiteral("( 6371 * acos ( cos ( radians(" + point.X + ") ) * cos( radians( X(" + AppSupplier.Columns.AddressLocation + ") ) ) * cos( radians( Y(" + AppSupplier.Columns.AddressLocation + ") ) - radians(" + point.Y + ") ) + sin ( radians(" + point.X + ") ) * sin( radians( X(" + AppSupplier.Columns.AddressLocation + ") ) ) )) AS distance");
            q.GroupBy(AppSupplier.TableSchema.SchemaName, AppSupplier.Columns.SupplierId);
            q.OrderBy("distance", SortDirection.ASC);
            q.OrderBy(OfferService.Columns.Price, SortDirection.ASC);

            List<OfferServiceUI> LstOfferUI = new List<OfferServiceUI>();
            using (DataReaderBase reader= q.ExecuteReader())
            {
                while (reader.Read())
                {
                    City c = City.FetchByID(reader[AppSupplier.Columns.CityId] != null ? (Int64)(reader[AppSupplier.Columns.CityId]) : 0);
                    LstOfferUI.Add(new OfferServiceUI
                    {
                        SupplierId = (reader[OfferService.Columns.SupplierId] != null ? (Int64)reader[OfferService.Columns.SupplierId] : 0),
                        OfferId = (reader[OfferService.Columns.OfferId] != null ? (Int64)reader[OfferService.Columns.OfferId] : 0),
                        SupplierName = (reader[AppSupplier.Columns.BusinessName] != null ? (reader[AppSupplier.Columns.BusinessName]).ToString() : ""),
                        Price = (reader[OfferService.Columns.Price] != null ? reader[OfferService.Columns.Price].ToString() : ""),
                        City = (c != null ? c.CityName : ""),
                        Street = (reader[AppSupplier.Columns.Street] != null ? (reader[AppSupplier.Columns.Street]).ToString() : ""),
                        Number = (reader[AppSupplier.Columns.HouseNum] != null ? (reader[AppSupplier.Columns.HouseNum]).ToString() : "") ,
                        Phone = (reader[AppSupplier.Columns.Phone] != null ? (reader[AppSupplier.Columns.Phone]).ToString() : "")
                    });
                
                }
            }
            return LstOfferUI;
        
        }
     
    }

}

