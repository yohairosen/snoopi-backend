using System;
using System.Collections.Generic;
using System.Text;
using dg.Sql;
using dg.Sql.Connector;
using dg.Utilities;

/*
 * PriceDeviation
 * PriceDeviation
 * PriceDeviationName:    FIXEDSTRING(64);
 * @INDEX:        NAME(ix_PriceDeviation_PriceDeviationId); [PriceDeviationId]; 
 * */

namespace Snoopi.core.DAL
{

    public partial class PriceDeviationCollection : AbstractRecordList<PriceDeviation, PriceDeviationCollection>
    {
    }

    public partial class PriceDeviation : AbstractRecord<PriceDeviation>
    {
        #region Table Schema
        private static TableSchema _TableSchema;
        public struct Columns
        {
            public static string SupplierName = "SupplierName";
            public static string SupplierId = "SupplierId";
            public static string RecommendedPrice = "RecommendedPrice";
            public static string ProductName = "ProductName";
            public static string ProductId = "ProductId";
            public static string DeviationPercentage = "DeviationPercentage";
            public static string ActualPrice = "ActualPrice";
            public static string IsApproved = "IsApproved";
            public static string TimeOfApproval = "TimeOfApproval";
        }
        public override TableSchema GetTableSchema()
        {
            if (null == _TableSchema)
            {
                TableSchema schema = new TableSchema();
                schema.SchemaName = @"pricedeviation";
                schema.AddColumn(Columns.ProductId, typeof(Int64), 0, 0, 0, false, true, false, null);
                schema.AddColumn(Columns.SupplierId, typeof(Int64), 0, 0, 0, false, true, false, null);
                schema.AddColumn(Columns.ProductName, typeof(string), DataType.Char, 64, 0, 0, false, false, false, string.Empty);
                schema.AddColumn(Columns.SupplierName, typeof(string), DataType.Char, 128, 0, 0, false, false, false, string.Empty);
                schema.AddColumn(Columns.RecommendedPrice, typeof(decimal), 0, 64, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.ActualPrice, typeof(decimal), 0, 64, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.DeviationPercentage, typeof(int), 0, 64, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.TimeOfApproval, typeof(DateTime), 0, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.IsApproved, typeof(bool), 0, 0, 0, false, false, false, null);

                schema.AddIndex("pk_SupplierProduct_SupplierIdProductId", TableSchema.ClusterMode.None, TableSchema.IndexMode.PrimaryKey, TableSchema.IndexType.None, Columns.SupplierId, Columns.ProductId);
                schema.AddIndex("ix_SupplierProduct_SupplierId", TableSchema.ClusterMode.None, TableSchema.IndexMode.None, TableSchema.IndexType.None, Columns.SupplierId);
                schema.AddIndex("ix_SupplierProduct_ProductId", TableSchema.ClusterMode.None, TableSchema.IndexMode.None, TableSchema.IndexType.None, Columns.ProductId);
              
                schema.AddForeignKey("fk_SupplierProduct_AppSupplier", SupplierProduct.Columns.SupplierId, AppSupplier.TableSchema.SchemaName, AppSupplier.Columns.SupplierId, TableSchema.ForeignKeyReference.Cascade, TableSchema.ForeignKeyReference.Cascade);
                schema.AddForeignKey("fk_SupplierProduct_ProductId", SupplierProduct.Columns.ProductId, Product.TableSchema.SchemaName, Product.Columns.ProductId, TableSchema.ForeignKeyReference.Cascade, TableSchema.ForeignKeyReference.Cascade);

                _TableSchema = schema;
            }

            return _TableSchema;
        }
        #endregion

        #region Private Members
        internal string _supplierName = string.Empty;
        internal Int64 _supplierId = 0;
        internal decimal _recommendedPrice = 0;
        internal Int64 _productId = 0;
        internal string _productName = string.Empty;
        internal decimal _deviationPercentage = 0;
        internal decimal _actualPrice = 0;
        internal DateTime _timeOfApproval = DateTime.MinValue;
        internal bool _isApproved = false;

        #endregion

        #region Properties
       
        public string SupplierName
        {
            get { return _supplierName; }
            set { _supplierName = value; }
        }
        public Int64 SupplierId
        {
            get { return _supplierId; }
            set { _supplierId = value; }
        }
        public Int64 ProductId
        {
            get { return _productId; }
            set { _productId = value; }
        }
        public decimal RecommendedPrice
        {
            get { return _recommendedPrice; }
            set { _recommendedPrice = value; }
        }
        public string ProductName
        {
            get { return _productName; }
            set { _productName = value; }
        }
        public decimal DeviationPercentage
        {
            get { return _deviationPercentage; }
            set { _deviationPercentage = value; }
        }
        public decimal ActualPrice
        {
            get { return _actualPrice; }
            set { _actualPrice = value; }
        }
        public DateTime TimeOfApproval
        {
            get { return _timeOfApproval; }
            set { _timeOfApproval = value; }
        }
        public bool IsApproved
        {
            get { return _isApproved; }
            set { _isApproved = value; }
        }
        #endregion

        #region AbstractRecord members
       

        public override void Insert(ConnectorBase conn)
        {
            Query qry = new Query(TableSchema);
            qry.Insert(Columns.SupplierId, SupplierId);
            qry.Insert(Columns.SupplierName, SupplierName);
            qry.Insert(Columns.DeviationPercentage, DeviationPercentage);
            qry.Insert(Columns.ProductId, ProductId);
            qry.Insert(Columns.ProductName, ProductName);
            qry.Insert(Columns.ActualPrice, ActualPrice);
            qry.Insert(Columns.RecommendedPrice, RecommendedPrice);
            qry.Insert(Columns.TimeOfApproval, TimeOfApproval);
            qry.Insert(Columns.IsApproved, IsApproved);

            object lastInsert = null;
            if (qry.Execute(conn, out lastInsert) > 0)
            {
                MarkOld();
            }
        }
        public override void Update(ConnectorBase conn)
        {
            Query qry = new Query(TableSchema);
            qry.Update(Columns.SupplierName, SupplierName);
            qry.Update(Columns.DeviationPercentage, DeviationPercentage);
            qry.Update(Columns.ProductName, ProductName);
            qry.Update(Columns.ActualPrice, ActualPrice);
            qry.Update(Columns.TimeOfApproval, TimeOfApproval);
            qry.Update(Columns.IsApproved, IsApproved);
            qry.Update(Columns.RecommendedPrice, RecommendedPrice);
            qry.Where(Columns.SupplierId, SupplierId);
            qry.AND(Columns.ProductId, ProductId);

            qry.Execute(conn);
        }
        public override void Read(DataReaderBase reader)
        {
            ProductId = Convert.ToInt64(reader[Columns.ProductId]);
            SupplierId = Convert.ToInt64(reader[Columns.SupplierId]);
            ProductName = (string)reader[Columns.ProductName];
            SupplierName = (string)reader[Columns.SupplierName];
            DeviationPercentage = Convert.ToDecimal(reader[Columns.DeviationPercentage]);
            RecommendedPrice = Convert.ToDecimal(reader[Columns.RecommendedPrice]);
            ActualPrice = Convert.ToDecimal(reader[Columns.ActualPrice]);
            IsApproved = Convert.ToBoolean(reader[Columns.IsApproved]);
            TimeOfApproval = Convert.ToDateTime(reader[Columns.TimeOfApproval]);
            IsThisANewRecord = false;
        }

        public override object GetPrimaryKeyValue()
        {
            return null;
        }

        #endregion

        #region Helpers

        public static PriceDeviation FetchByID(Int64 SupplierId, Int64 ProductId)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.SupplierId, SupplierId)
                .AND(Columns.ProductId, ProductId);
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                if (reader.Read())
                {
                    PriceDeviation item = new PriceDeviation();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }

        public static PriceDeviation FetchByID(ConnectorBase conn, Int64 SupplierId, Int64 ProductId)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.SupplierId, SupplierId)
                .AND(Columns.ProductId, ProductId);
            using (DataReaderBase reader = qry.ExecuteReader(conn))
            {
                if (reader.Read())
                {
                    PriceDeviation item = new PriceDeviation();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }

        public static int Delete(Int64 supplierId, Int64 productId)
        {
            Query qry = new Query(TableSchema)
                .Delete().Where(Columns.SupplierId, supplierId).AND(Columns.ProductId, productId); ;
            return qry.Execute();
        }

        public static int Delete(ConnectorBase conn, Int64 supplierId, Int64 productId)
        {
            Query qry = new Query(TableSchema)
                .Delete().Where(Columns.SupplierId, supplierId).AND(Columns.ProductId, productId); ;
            return qry.Execute(conn);
        }
        #endregion
    }

    public partial class PriceDeviation
    {

      
    }

}
