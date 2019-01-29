using System;
using System.Collections.Generic;
using System.Text;
using dg.Sql;
using dg.Sql.Connector;
using dg.Utilities;

namespace Snoopi.core.DAL
{
    class CartProductCollection : AbstractRecordList<CartProduct, CartProductCollection>
    {
    }

    public partial class CartProduct : AbstractRecord<CartProduct>
    {
        #region Constructors

        public CartProduct()
            : base()
        {

        }
        #endregion
        #region Table Schema
        private static TableSchema _TableSchema;
        public struct Columns
        {
            public static string CartProductId = "CartProduct_id";
            public static string ProductId = "product_id";
            public static string CartId = "cart_id";
            public static string ProductAmount = "product_amount";
        }

        public override TableSchema GetTableSchema()
        {
            if (null == _TableSchema)
            {
                TableSchema schema = new TableSchema();
                schema.SchemaName = @"CartProduct";
                schema.AddColumn(Columns.CartId, typeof(int), 0, 0, 0, false, true, false, null);
                schema.AddColumn(Columns.ProductId, typeof(Int64), 0, 0, 0,false, true, false, null);
                schema.AddColumn(Columns.CartProductId, typeof(Int64), 0, 0, 0, true, false, false, null);
                schema.AddColumn(Columns.ProductAmount, typeof(int), 0, 0, 0, false, false, false, null);

                schema.AddForeignKey("appuser_id", Columns.ProductId, AppSupplier.TableSchema.SchemaName, Product.Columns.ProductId, TableSchema.ForeignKeyReference.Cascade, TableSchema.ForeignKeyReference.Cascade);
                _TableSchema = schema;
            }

            return _TableSchema;
        }
        #endregion

        #region Private Members
        private int _cartId;
        private Int64 _productId;
        private Int64 _CartProductId;
        private int _productAmount;

        #endregion

        #region Properties

        public Int64 ProductId
        {
            get { return _productId; }
            set { _productId = value; }
        }

        public Int64 CartProductId
        {
            get { return _CartProductId; }
            set { _CartProductId = value; }
        }

        public int CartId
        {
            get { return _cartId; }
            set { _cartId = value; }
        }

        public int ProductAmount
        {
            get { return _productAmount; }
            set { _productAmount = value; }
        }



        #endregion

        #region AbstractRecord members

        public override void Insert(ConnectorBase conn)
        {
            Query qry = new Query(TableSchema);
            qry.Insert(Columns.CartProductId, CartProductId);
            qry.Insert(Columns.ProductId, ProductId);
            qry.Insert(Columns.CartId, CartId);
            qry.Insert(Columns.ProductAmount, ProductAmount);

            object lastInsert = null;
            if (qry.Execute(conn, out lastInsert) > 0)
            {
                MarkOld();
            }
        }
        public override void Update(ConnectorBase conn)
        {
            Query qry = new Query(TableSchema);
            qry.Update(Columns.ProductId, ProductId);
            qry.Update(Columns.CartId, CartId);
            qry.Update(Columns.ProductAmount, ProductAmount);

            qry.Where(Columns.CartProductId, CartProductId);

            qry.Execute(conn);
        }
        public override void Read(DataReaderBase reader)
        {
            CartProductId = Convert.ToInt64(reader[Columns.CartProductId]);
            ProductId = Convert.ToInt64(reader[Columns.ProductId]);
            CartId = (int)reader[Columns.CartId];
            ProductAmount = (int)reader[Columns.CartId];
            IsThisANewRecord = false;
        }

        public override object GetPrimaryKeyValue()
        {
            return null;
        }

        #endregion

        #region Helpers

        public static CartProduct FetchByID(int CartProductId)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.CartProductId, CartProductId);
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                if (reader.Read())
                {
                    CartProduct item = new CartProduct();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }

        public static CartProduct FetchByID(ConnectorBase conn, int CartProductId)
        {
            Query qry = new Query(TableSchema)
              .Where(Columns.CartProductId, CartProductId);
            using (DataReaderBase reader = qry.ExecuteReader(conn))
            {
                if (reader.Read())
                {
                    CartProduct item = new CartProduct();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }

        //public static int Delete(Int64 supplierId, Int64 productId)
        //{
        //    Query qry = new Query(TableSchema)
        //        .Delete().Where(Columns.SupplierId, supplierId).AND(Columns.ProductId, productId); ;
        //    return qry.Execute();
        //}

        //public static int Delete(ConnectorBase conn, Int64 supplierId, Int64 productId)
        //{
        //    Query qry = new Query(TableSchema)
        //        .Delete().Where(Columns.SupplierId, supplierId).AND(Columns.ProductId, productId); ;
        //    return qry.Execute(conn);
        //}
        #endregion
    }

    public partial class CartProduct
    {


    }
}
