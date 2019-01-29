using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Snoopi.core.DAL;
using dg.Sql;
using dg.Sql.Connector;
using System.IO;
using System;

namespace Snoopi.core.BL
{
    public class PriceDeviationUi
    {   
        public string SupplierName {get; set;}
        public Int64 SupplierId { get; set; }
        public decimal RecommendedPrice { get; set; }
        public Int64 ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal DeviationPercentage { get; set; }
        public decimal ActualPrice { get; set; }
        public DateTime TimeOfApproval { get; set; }
        public bool IsApproved { get; set; }
    }

    public class PriceDeviationController
    {
        public static List<PriceDeviationUi> GetUnapprovedPriceDeviation()
        {
            Query q = new Query(PriceDeviation.TableSchema);
            q.SelectAllTableColumns();
            q.AddSelect(PriceDeviation.TableSchema.SchemaName);
            q.OrderBy(PriceDeviation.Columns.DeviationPercentage, SortDirection.DESC);
            q.Where(PriceDeviation.Columns.IsApproved, false);
            q.SelectAll();

            List<PriceDeviationUi> LstPriceDeviationUi = new List<PriceDeviationUi>();
            using (DataReaderBase reader = q.ExecuteReader())
            {
                while (reader.Read())
                {
                    LstPriceDeviationUi.Add(new PriceDeviationUi
                    {
                        SupplierId = (reader[PriceDeviation.Columns.SupplierId] is DBNull ? 0 : (Int64)reader[PriceDeviation.Columns.SupplierId]),
                        ProductId = (reader[PriceDeviation.Columns.ProductId] is DBNull ? 0 : (Int64)reader[PriceDeviation.Columns.ProductId]),
                        DeviationPercentage = (reader[PriceDeviation.Columns.DeviationPercentage] is DBNull ? 0 : Convert.ToDecimal(reader[PriceDeviation.Columns.DeviationPercentage])),
                        RecommendedPrice = (reader[PriceDeviation.Columns.RecommendedPrice] is DBNull ? 0 : Convert.ToDecimal(reader[PriceDeviation.Columns.RecommendedPrice])),
                        ActualPrice = (reader[PriceDeviation.Columns.ActualPrice] is DBNull ? 0 : Convert.ToDecimal(reader[PriceDeviation.Columns.ActualPrice])),
                       SupplierName = (reader[PriceDeviation.Columns.SupplierName] is DBNull ? "" : (reader[PriceDeviation.Columns.SupplierName]).ToString()),
                       ProductName = (reader[PriceDeviation.Columns.ProductName] is DBNull ? "" : (reader[PriceDeviation.Columns.ProductName]).ToString()),
                        IsApproved = (reader[PriceDeviation.Columns.IsApproved] is DBNull ? false : true),
                        TimeOfApproval = (reader[PriceDeviation.Columns.TimeOfApproval] is DBNull ? DateTime.MinValue : Convert.ToDateTime((reader[PriceDeviation.Columns.TimeOfApproval]))),

                    });

                }
            }
            return LstPriceDeviationUi;
        }
    }

}

