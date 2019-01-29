using System;
using System.Collections.Generic;
using System.Text;
using dg.Sql;
using dg.Sql.Connector;
using dg.Utilities;

/*
 * BidProduct
 * BidProduct
 * BidId:             INT64;
 * ProductId:         INT64;
 * Amount:            INT32; DEFAULT 1;
 * @INDEX:            NAME(pk_BidProduct_BidId_ProductId);PRIMARYKEY[BidId,ProductId];
 * @INDEX:            NAME(ix_BidProduct_BidId);[BidId];
 * @INDEX:            NAME(ix_BidProduct_ProductId);[ProductId];
 * 
 * */

namespace Snoopi.core.DAL
{
    public partial class BidProductCollection : AbstractRecordList<BidProduct, BidProductCollection>
    {
    }

    public partial class BidProduct : AbstractRecord<BidProduct>
    {
        #region Table Schema
        private static TableSchema _TableSchema;
        public struct Columns
        {
            public static string BidId = "BidId";
            public static string ProductId = "ProductId";
            public static string Amount = "Amount";
            public static string Price = "Price";
        }
        public override TableSchema GetTableSchema()
        {
            if (null == _TableSchema)
            {
                TableSchema schema = new TableSchema();
                schema.SchemaName = @"BidProduct";
                schema.AddColumn(Columns.BidId, typeof(Int64), 0, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.ProductId, typeof(Int64), 0, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.Amount, typeof(Int32), 0, 0, 0, false, false, false, 1);
                schema.AddColumn(Columns.Price, typeof(decimal), 0, 0, 0, false, false, false, null);

                _TableSchema = schema;

                schema.AddIndex("pk_BidProduct_BidId_ProductId", TableSchema.ClusterMode.None, TableSchema.IndexMode.None, TableSchema.IndexType.None);
                schema.AddIndex("ix_BidProduct_BidId", TableSchema.ClusterMode.None, TableSchema.IndexMode.None, TableSchema.IndexType.None, Columns.BidId);
                schema.AddIndex("ix_BidProduct_ProductId", TableSchema.ClusterMode.None, TableSchema.IndexMode.None, TableSchema.IndexType.None, Columns.ProductId);

            }

            return _TableSchema;
        }
        #endregion

        #region Private Members
        internal Int64 _BidId = 0;
        internal Int64 _ProductId = 0;
        internal Int32 _Amount = 1;
        internal decimal _price = 0;
        #endregion

        #region Properties
        public Int64 BidId
        {
            get { return _BidId; }
            set { _BidId = value; }
        }
        public Int64 ProductId
        {
            get { return _ProductId; }
            set { _ProductId = value; }
        }
        public Int32 Amount
        {
            get { return _Amount; }
            set { _Amount = value; }
        }
        public decimal Price
        {
            get { return _price; }
            set { _price = value; }
        }

        #endregion

        #region AbstractRecord members
        public override object GetPrimaryKeyValue()
        {
            return null;
        }

        public override void Insert(ConnectorBase conn)
        {
            Query qry = new Query(TableSchema);
            qry.Insert(Columns.BidId, BidId);
            qry.Insert(Columns.ProductId, ProductId);
            qry.Insert(Columns.Amount, Amount);
            qry.Insert(Columns.Price, Price);

            object lastInsert = null;
            if (qry.Execute(conn, out lastInsert) > 0)
            {
                MarkOld();
            }
        }
        public override void Update(ConnectorBase conn)
        {
            Query qry = new Query(TableSchema);
            qry.Update(Columns.BidId, BidId);
            qry.Update(Columns.ProductId, ProductId);
            qry.Update(Columns.Amount, Amount);
            qry.Update(Columns.Price, Price);

            qry.Execute(conn);
        }
        public override void Read(DataReaderBase reader)
        {
            BidId = Convert.ToInt64(reader[Columns.BidId]);
            ProductId = Convert.ToInt64(reader[Columns.ProductId]);
            Amount = Convert.ToInt32(reader[Columns.Amount]);
            Price = Convert.ToDecimal(reader[Columns.Price]);

            IsThisANewRecord = false;
        }
        #endregion

        #region Helpers
        #endregion
    }

}
