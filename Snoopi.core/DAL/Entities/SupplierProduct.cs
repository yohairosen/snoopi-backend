using System;
using System.Collections.Generic;
using System.Text;
using dg.Sql;
using dg.Sql.Connector;

/*
 * SupplierProduct
 * SupplierProduct
 * SupplierId:            INT64; The admin AppUser
 * ProductId:             INT64; Group
 * Price:                 DECIMAL; DEFAULT 0;
 * Gift:                  FIXEDSTRING(100); DEFAULT string.Empty;
 * CreateDate:            DATETIME; DEFAULT DateTime.MinValue; Create date/time
 * @INDEX:                NAME(pk_SupplierProduct_SupplierIdProductId);PRIMARYKEY;[SupplierId,ProductId]; 
 * @FOREIGNKEY:           NAME(fk_SupplierProduct_AppSupplier); FOREIGNTABLE(AppSupplier); COLUMNS[SupplierId]; FOREIGNCOLUMNS[SupplierId]; ONUPDATE(CASCADE); ONDELETE(CASCADE);
 * @INDEX:                NAME(ix_SupplierProduct_SupplierId);[SupplierId]; 
 * @FOREIGNKEY:           NAME(fk_SupplierProduct_ProductId); FOREIGNTABLE(Product); COLUMNS[ProductId]; FOREIGNCOLUMNS[ProductId]; ONUPDATE(CASCADE); ONDELETE(CASCADE);
 * @INDEX:                NAME(ix_SupplierProduct_ProductId);[ProductId]; 
 * @INDEX:                NAME(ix_SupplierProduct_CreateDate); [CreateDate ASC];
 * */

namespace Snoopi.core.DAL
{
    public partial class SupplierProductCollection : AbstractRecordList<SupplierProduct, SupplierProductCollection>
    {
    }

    public partial class SupplierProduct : AbstractRecord<SupplierProduct>
    {
        #region Table Schema
        private static TableSchema _TableSchema;
        public struct Columns
        {
            public static string SupplierId = "SupplierId"; // The admin AppUser
            public static string ProductId = "ProductId"; // Group
            public static string Price = "Price";
            public static string Gift = "Gift";
            public static string CreateDate = "CreateDate"; // Create date/time
        }
        public override TableSchema GetTableSchema()
        {
            if (null == _TableSchema)
            {
                TableSchema schema = new TableSchema();
                schema.SchemaName = @"SupplierProduct";
                schema.AddColumn(Columns.SupplierId, typeof(Int64), 0, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.ProductId, typeof(Int64), 0, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.Price, typeof(decimal), 0, 0, 0, false, false, false, 0);
                schema.AddColumn(Columns.Gift, typeof(string), DataType.Char, 100, 0, 0, false, false, false, string.Empty);
                schema.AddColumn(Columns.CreateDate, typeof(DateTime), 0, 0, 0, false, false, false, DateTime.MinValue);

                _TableSchema = schema;

                schema.AddIndex("pk_SupplierProduct_SupplierIdProductId", TableSchema.ClusterMode.None, TableSchema.IndexMode.PrimaryKey, TableSchema.IndexType.None, Columns.SupplierId, Columns.ProductId);
                schema.AddIndex("ix_SupplierProduct_SupplierId", TableSchema.ClusterMode.None, TableSchema.IndexMode.None, TableSchema.IndexType.None, Columns.SupplierId);
                schema.AddIndex("ix_SupplierProduct_ProductId", TableSchema.ClusterMode.None, TableSchema.IndexMode.None, TableSchema.IndexType.None, Columns.ProductId);
                schema.AddIndex("ix_SupplierProduct_CreateDate", TableSchema.ClusterMode.None, TableSchema.IndexMode.None, TableSchema.IndexType.None, Columns.CreateDate, SortDirection.ASC);

                schema.AddForeignKey("fk_SupplierProduct_AppSupplier", SupplierProduct.Columns.SupplierId, AppSupplier.TableSchema.SchemaName, AppSupplier.Columns.SupplierId, TableSchema.ForeignKeyReference.Cascade, TableSchema.ForeignKeyReference.Cascade);
                schema.AddForeignKey("fk_SupplierProduct_ProductId", SupplierProduct.Columns.ProductId, Product.TableSchema.SchemaName, Product.Columns.ProductId, TableSchema.ForeignKeyReference.Cascade, TableSchema.ForeignKeyReference.Cascade);

            }

            return _TableSchema;
        }
        #endregion

        #region Private Members
        internal Int64 _SupplierId = 0;
        internal Int64 _ProductId = 0;
        internal decimal _Price = 0;
        internal string _Gift = string.Empty;
        internal DateTime _CreateDate = DateTime.MinValue;
        #endregion

        #region Properties
        public Int64 SupplierId
        {
            get { return _SupplierId; }
            set { _SupplierId = value; }
        }
        public Int64 ProductId
        {
            get { return _ProductId; }
            set { _ProductId = value; }
        }
        public decimal Price
        {
            get { return _Price; }
            set { _Price = value; }
        }
        public string Gift
        {
            get { return _Gift; }
            set { _Gift = value; }
        }
        public DateTime CreateDate
        {
            get { return _CreateDate; }
            set { _CreateDate = value; }
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
            qry.Insert(Columns.SupplierId, SupplierId);
            qry.Insert(Columns.ProductId, ProductId);
            qry.Insert(Columns.Price, Price);
            qry.Insert(Columns.Gift, Gift);
            qry.Insert(Columns.CreateDate, CreateDate);

            object lastInsert = null;
            if (qry.Execute(conn, out lastInsert) > 0)
            {
                MarkOld();
            }
        }
        public override void Update(ConnectorBase conn)
        {
            Query qry = new Query(TableSchema);
            qry.Update(Columns.SupplierId, SupplierId);
            qry.Update(Columns.ProductId, ProductId);
            qry.Update(Columns.Price, Price);
            qry.Update(Columns.Gift, Gift);
            qry.Update(Columns.CreateDate, CreateDate);
            qry.Where(Columns.SupplierId, SupplierId);
            qry.AND(Columns.ProductId, ProductId);

            qry.Execute(conn);
        }
        public override void Read(DataReaderBase reader)
        {
            SupplierId = Convert.ToInt64(reader[Columns.SupplierId]);
            ProductId = Convert.ToInt64(reader[Columns.ProductId]);
            Price = Convert.ToDecimal(reader[Columns.Price]);
            Gift = (string)reader[Columns.Gift];
            CreateDate = Convert.ToDateTime(reader[Columns.CreateDate]);

            IsThisANewRecord = false;
        }
        #endregion

        #region Helpers
        public static SupplierProduct FetchByID(Int64 SupplierId, Int64 ProductId)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.SupplierId, SupplierId)
                .AND(Columns.ProductId, ProductId);
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                if (reader.Read())
                {
                    SupplierProduct item = new SupplierProduct();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }

        public static int Delete(Int64 SupplierId, Int64 ProductId)
        {
            Query qry = new Query(TableSchema)
                .Delete().Where(Columns.SupplierId, SupplierId)
                .AND(Columns.ProductId, ProductId);
            return qry.Execute();
        }
        public static SupplierProduct FetchByID(ConnectorBase conn, Int64 SupplierId, Int64 ProductId)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.SupplierId, SupplierId)
                .AND(Columns.ProductId, ProductId);
            using (DataReaderBase reader = qry.ExecuteReader(conn))
            {
                if (reader.Read())
                {
                    SupplierProduct item = new SupplierProduct();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }

        public static int Delete(ConnectorBase conn, Int64 SupplierId, Int64 ProductId)
        {
            Query qry = new Query(TableSchema)
                .Delete().Where(Columns.SupplierId, SupplierId)
                .AND(Columns.ProductId, ProductId);
            return qry.Execute(conn);
        }
        #endregion
    }
}
