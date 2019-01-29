using System;
using System.Collections.Generic;
using System.Text;
using dg.Sql;
using dg.Sql.Connector;
using dg.Utilities;

namespace Snoopi.core.DAL
{
    class CartCollection : AbstractRecordList<Cart, CartCollection>
    {
    }

    public partial class Cart : AbstractRecord<Cart>
    {
        #region Constructors
       
        public Cart()
            : base()
        {

        }
        #endregion
        #region Table Schema
        private static TableSchema _TableSchema;
        public struct Columns
        {
            public static string CartId = "cart_id";
            public static string UserId = "user_id";
            public static string CreatedDate = "created_date";
            public static string SupplierId = "supplier_id";
            public static string CartTotalPrice = "cart_total_price";
            public static string TempUserId = "temp_user_id";
        }
            
        public override TableSchema GetTableSchema()
        {
            if (null == _TableSchema)
            {
                TableSchema schema = new TableSchema();
                schema.SchemaName = @"cart";
                schema.AddColumn(Columns.CartId, typeof(int), 0, 0, 0, true, true, false, null);
                schema.AddColumn(Columns.UserId, typeof(Int64), 0, 0, 0, false, false, true, null);
                schema.AddColumn(Columns.TempUserId, typeof(Int64), 0, 0, 0, false, false, true, null);
                schema.AddColumn(Columns.CreatedDate, typeof(DateTime), 0, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.SupplierId, typeof(Int64), 0, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.CartTotalPrice, typeof(decimal), 0, 0, 0, false, false, true, null);
                _TableSchema = schema;

            }

            return _TableSchema;
        }
        #endregion

        #region Private Members
        private DateTime _createdDate;
        private Int64? _userId;
        private Int64? _tempUserId;
        private int _cartId;
        private Int64 _supplierId;
        private decimal _cartTotalPrice;
        #endregion

        #region Properties

        public decimal CartTotalPrice
        {
            get { return _cartTotalPrice; }
            set { _cartTotalPrice = value; }
        }
        public Int64? UserId
        {
            get { return _userId; }
            set { _userId = value; }
        }

        public Int64? TempUserId
        {
            get { return _tempUserId; }
            set { _tempUserId = value; }
        }

        public Int64 SupplierId
        {
            get { return _supplierId; }
            set { _supplierId = value; }
        }

        public int CartId
        {
            get { return _cartId; }
            set { _cartId = value; }
        }

        public DateTime CreatedDate
        {
            get { return _createdDate; }
            set { _createdDate = value; }
        }

       
        #endregion

        #region AbstractRecord members

        public override void Insert(ConnectorBase conn)
        {
            Query qry = new Query(TableSchema);
            qry.Insert(Columns.CartId, CartId);
            qry.Insert(Columns.UserId, UserId);
            qry.Insert(Columns.TempUserId, TempUserId);
            qry.Insert(Columns.SupplierId, SupplierId);
            qry.Insert(Columns.CreatedDate, CreatedDate);
            qry.Insert(Columns.CartTotalPrice, CartTotalPrice);
            
            object lastInsert = null;
            if (qry.Execute(conn, out lastInsert) > 0)
            {
                int cartId = 0 ;
                Int32.TryParse(lastInsert.ToString(), out cartId);
                CartId = cartId;
                MarkOld();
            }
        }
        public override void Update(ConnectorBase conn)
        {
            Query qry = new Query(TableSchema);
            qry.Update(Columns.UserId, UserId);
            qry.Update(Columns.SupplierId, SupplierId);
            qry.Update(Columns.TempUserId, TempUserId);
            qry.Update(Columns.CreatedDate, CreatedDate);
            qry.Update(Columns.CartTotalPrice, CartTotalPrice);
            
            qry.Where(Columns.CartId, CartId);

            qry.Execute(conn);
        }
        public override void Read(DataReaderBase reader)
        {
            CartId = Convert.ToInt32(reader[Columns.CartId]);
            UserId = Convert.ToInt64(reader[Columns.UserId]);
            TempUserId = Convert.ToInt64(reader[Columns.TempUserId]);
            SupplierId = Convert.ToInt64(reader[Columns.SupplierId]);
            CreatedDate = (DateTime)reader[Columns.CreatedDate];
            CartTotalPrice = reader[Columns.CartTotalPrice] == null ? 0 : (decimal)reader[Columns.CartTotalPrice];
            IsThisANewRecord = false;
        }

        public override object GetPrimaryKeyValue()
        {
            return null;
        }

        #endregion

        #region Helpers

        public static Cart FetchByID(int cartId)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.CartId, cartId);
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                if (reader.Read())
                {
                    Cart item = new Cart();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }

        public static Cart FetchByID(ConnectorBase conn, int cartId)
        {
            Query qry = new Query(TableSchema)
              .Where(Columns.CartId, cartId);
            using (DataReaderBase reader = qry.ExecuteReader(conn))
            {
                if (reader.Read())
                {
                    Cart item = new Cart();
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

    public partial class Cart
    {


    }

}

