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

    public class OfferUI
    {
        public Int64 OfferId { get; set; }
        public string SupplierName { get; set; }
        public Int64 SupplierId { get; set; }
        public double AvgRate { get; set; }
        public int NumberOfComments { get; set; }
        public string Gift { get; set; }
        public decimal TotalPrice { get; set; }
        public bool IsOrder { get; set; }
        public string MastercardCode { get; set; }
        public List<ProductUI> Products { get; set; }

    }
    public class OfferController
    {
        public static List<OfferUI> GetAllOfferByBidIdWithIsOrder(Int64 BidId)
        {
            Query q = new Query(Offer.TableSchema);
            q.SelectAllTableColumns();
            q.AddSelect(AppSupplier.TableSchema.SchemaName, AppSupplier.Columns.BusinessName, AppSupplier.Columns.BusinessName);
            q.AddSelect(Offer.TableSchema.SchemaName, Offer.Columns.Price, Offer.Columns.Price);
            q.OrderBy(Offer.Columns.Price, SortDirection.ASC);
           
            q.Join(JoinType.InnerJoin, Offer.TableSchema, Offer.Columns.SupplierId, Offer.TableSchema.SchemaName,
                AppSupplier.TableSchema, AppSupplier.Columns.SupplierId, AppSupplier.TableSchema.SchemaName);
            q.Where(Offer.TableSchema.SchemaName, Offer.Columns.BidId, WhereComparision.EqualsTo, BidId);
            //q.AddWhere("o", Order.Columns.PaySupplierStatus, WhereComparision.EqualsTo, UserPaymentStatus.NotPayed);
            q.SelectAll();
            q.GroupBy(AppSupplier.TableSchema.SchemaName, AppSupplier.Columns.SupplierId);

            List<OfferUI> LstOfferUI = new List<OfferUI>();
            using (DataReaderBase reader = q.ExecuteReader())
            {
                while (reader.Read())
                {
                    LstOfferUI.Add(new OfferUI
                    {
                        SupplierId = (reader[Offer.Columns.SupplierId] is DBNull ? 0 : (Int64)reader[Offer.Columns.SupplierId]),
                        OfferId = (reader[Offer.Columns.OfferId] is DBNull ? 0 : (Int64)reader[Offer.Columns.OfferId]),
                        Gift = (reader[Offer.Columns.Gift] is DBNull ? "" : (reader[Offer.Columns.Gift]).ToString()),
                        TotalPrice = (reader[Offer.Columns.Price] is DBNull ? 0 : decimal.Parse((reader[Offer.Columns.Price]).ToString())),
                        SupplierName = (reader[AppSupplier.Columns.BusinessName] is DBNull ? "" : (reader[AppSupplier.Columns.BusinessName]).ToString()),
                        IsOrder = (reader[Order.Columns.OrderId] is DBNull ? false : true),
                    });

                }
            }
            return LstOfferUI;

        }
        public static List<OfferUI> GetAllOfferByBidId(Int64 BidId)
        {
            Query q = new Query(Offer.TableSchema);
            q.OrderBy(Offer.Columns.Price, SortDirection.ASC);
            q.Join(JoinType.InnerJoin, Offer.TableSchema, Offer.Columns.SupplierId, Offer.TableSchema.SchemaName,
                AppSupplier.TableSchema, AppSupplier.Columns.SupplierId, AppSupplier.TableSchema.SchemaName);
            q.Join(JoinType.LeftJoin, AppSupplier.TableSchema, AppSupplier.Columns.SupplierId, AppSupplier.TableSchema.SchemaName,
                 Comment.TableSchema, Comment.Columns.SupplierId, Comment.TableSchema.SchemaName);
            q.Where(Offer.TableSchema.SchemaName, Offer.Columns.BidId, WhereComparision.EqualsTo, BidId);
            q.SelectAllTableColumns();
            q.AddSelect(AppSupplier.TableSchema.SchemaName, AppSupplier.Columns.BusinessName, AppSupplier.Columns.BusinessName);
            q.AddSelect(AppSupplier.TableSchema.SchemaName, AppSupplier.Columns.MastercardCode, AppSupplier.Columns.MastercardCode);
            q.AddSelectLiteral(" avg(" + Comment.TableSchema.SchemaName + "." + Comment.Columns.Rate + ") as AvgRate ");
            q.GroupBy(AppSupplier.TableSchema.SchemaName, AppSupplier.Columns.SupplierId);

            List<OfferUI> LstOfferUI = new List<OfferUI>();
            using (DataReaderBase reader = q.ExecuteReader())
            {
                while (reader.Read())
                {
                    LstOfferUI.Add(new OfferUI
                    {
                        SupplierId = (reader[Offer.Columns.SupplierId] != null ? (Int64)reader[Offer.Columns.SupplierId] : 0),
                        OfferId = (reader[Offer.Columns.OfferId] != null ? (Int64)reader[Offer.Columns.OfferId] : 0),
                        Gift = (reader[Offer.Columns.Gift] != null ? (reader[Offer.Columns.Gift]).ToString() : ""),
                        TotalPrice = (reader[Offer.Columns.Price] != null ? decimal.Parse((reader[Offer.Columns.Price]).ToString()) : 0),
                        AvgRate = (reader["AvgRate"] is DBNull ? 0 : double.Parse((reader["AvgRate"]).ToString())),
                        SupplierName = (reader[AppSupplier.Columns.BusinessName] != null ? (reader[AppSupplier.Columns.BusinessName]).ToString() : ""),
                        MastercardCode = (reader[AppSupplier.Columns.MastercardCode] != null ? (reader[AppSupplier.Columns.MastercardCode]).ToString() : "0")

                    });

                }
            }
            return LstOfferUI;

        }

        public static List<OfferUI> GetAllOfferByProductIds(Dictionary<Int64, int> LstProduct, Int64 CityId)
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
                return null;

            Query qry = new Query(AppSupplier.TableSchema);
            qry.SelectAllTableColumns();
            qry.Join(JoinType.InnerJoin, SupplierProduct.TableSchema, SupplierProduct.TableSchema.SchemaName,
                new JoinColumnPair(AppSupplier.TableSchema.SchemaName, AppSupplier.Columns.SupplierId, SupplierProduct.Columns.SupplierId));
            qry.Join(JoinType.LeftJoin, Comment.TableSchema, Comment.TableSchema.SchemaName,
                new JoinColumnPair(AppSupplier.TableSchema.SchemaName, AppSupplier.Columns.SupplierId, Comment.Columns.SupplierId));

            qry.AddSelect(SupplierProduct.TableSchema.SchemaName, SupplierProduct.Columns.ProductId, SupplierProduct.Columns.ProductId);
            qry.AddSelect(SupplierProduct.TableSchema.SchemaName, SupplierProduct.Columns.Price, SupplierProduct.Columns.Price);
            qry.AddSelect(SupplierProduct.TableSchema.SchemaName, SupplierProduct.Columns.Gift, SupplierProduct.Columns.Gift);
            qry.Where(AppSupplier.TableSchema.SchemaName, AppSupplier.Columns.SupplierId, WhereComparision.In, Suppliers);
            qry.AddWhere(AppSupplier.TableSchema.SchemaName, AppSupplier.Columns.SupplierId, WhereComparision.In, CityInnerQuery);
            qry.AddWhere(SupplierProduct.TableSchema.SchemaName, SupplierProduct.Columns.ProductId, WhereComparision.In, LstProduct.Select(r => r.Key).ToList());
            qry.AddWhere(AppSupplier.TableSchema.SchemaName, AppSupplier.Columns.IsDeleted, WhereComparision.NotEqualsTo, true);
            qry.AddWhere(AppSupplier.TableSchema.SchemaName, AppSupplier.Columns.IsLocked, WhereComparision.NotEqualsTo, true);
            qry.AddWhere(AppSupplier.TableSchema.SchemaName, AppSupplier.Columns.Status, WhereComparision.NotEqualsTo, false);
            
            qry.AddSelectLiteral("(SELECT avg(" + Comment.Columns.Rate + ") from " + Comment.TableSchema.SchemaName + " where "
                        + Comment.TableSchema.SchemaName + "." + Comment.Columns.SupplierId + "=" + AppSupplier.TableSchema.SchemaName + "." + AppSupplier.Columns.SupplierId +
                          " AND " + Comment.TableSchema.SchemaName + "." + Comment.Columns.Status + "=" + (int)CommentStatus.Approved + ")", "AvgRate");
            qry.AddSelectLiteral("(SELECT Count(" + Comment.Columns.Rate + ") from " + Comment.TableSchema.SchemaName + " where " +
                Comment.TableSchema.SchemaName + "." + Comment.Columns.SupplierId + "=" + AppSupplier.TableSchema.SchemaName + "." + AppSupplier.Columns.SupplierId +
                 " AND " + Comment.TableSchema.SchemaName + "." + Comment.Columns.Status + "=" + (int)CommentStatus.Approved + ")", "numberOfComments");
            qry.AddSelect(AppSupplier.TableSchema.SchemaName, AppSupplier.Columns.SupplierId, AppSupplier.Columns.SupplierId);
            qry.GroupBy(AppSupplier.Columns.SupplierId);
            qry.GroupBy(SupplierProduct.Columns.ProductId);
            var suppliersSumDic = new Dictionary<Int64, OfferUI>();
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                while (reader.Read())
                {
                    Int64 supplierId = reader[SupplierProduct.Columns.SupplierId] != null ? Convert.ToInt64(reader[SupplierProduct.Columns.SupplierId]) : 0;
                    Int64 productId = reader[SupplierProduct.Columns.ProductId] != null ? Convert.ToInt64(reader[SupplierProduct.Columns.ProductId]) : 0;
                    AppSupplier supplier = AppSupplier.FetchByID(supplierId);
                    if (supplier != null && productId > 0)
                    {
                        decimal price = Convert.ToDecimal(reader["Price"]);
                        string gift = reader["Gift"].ToString();
                        int amount = LstProduct[productId];
                        decimal productPrice =  price * amount;

                        if (!suppliersSumDic.ContainsKey(supplierId))
                            suppliersSumDic[supplierId] = new OfferUI
                            {
                                SupplierName = supplier.BusinessName,
                                SupplierId = supplierId,
                                MastercardCode = supplier.MastercardCode,
                                Products = new List<ProductUI>(),
                                AvgRate = reader["AvgRate"] == DBNull.Value ? 0 : Convert.ToDouble(reader["AvgRate"]),
                                NumberOfComments = reader["numberOfComments"] == DBNull.Value ? 0 : Convert.ToInt32(reader["numberOfComments"])
                            };

                        var product = new ProductUI {
                            ProductId = productId,
                            ProductPrice = productPrice,
                            Amount = amount.ToString(),
                        };

                        suppliersSumDic[supplierId].Products.Add(product);
                        suppliersSumDic[supplierId].TotalPrice += productPrice;
                        suppliersSumDic[supplierId].Gift += gift;
                    }
                }
            }
            return suppliersSumDic.Values.OrderBy(x => x.TotalPrice).ToList();
        }

        public static bool IsOfferStillValid(Dictionary<Int64, int> products, Int64 supplierId, decimal totalPrice)
        {
            Query qry = new Query(SupplierProduct.TableSchema);
            qry.AddSelect(SupplierProduct.Columns.ProductId);
            qry.AddSelect(SupplierProduct.Columns.Price);
            qry.Where(SupplierProduct.Columns.SupplierId, WhereComparision.EqualsTo, supplierId);
            qry.AddWhere(SupplierProduct.Columns.ProductId, WhereComparision.In, products.Select(r => r.Key).ToList());
            
            decimal currentTotalPrice = 0;
            try
            {
                using (DataReaderBase reader = qry.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Int64 productId = reader[SupplierProduct.Columns.ProductId] != null ? Convert.ToInt64(reader[SupplierProduct.Columns.ProductId]) : 0;
                        decimal price = Convert.ToDecimal(reader["Price"]);
                        currentTotalPrice += price * products[productId];
                    }
                }
            }
            catch
            {
                return false;
            }
            if (totalPrice == currentTotalPrice)
                return true;
            return false;
        }
    }
}

