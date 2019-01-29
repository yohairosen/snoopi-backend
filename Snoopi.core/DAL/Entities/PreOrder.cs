using dg.Sql;
using dg.Sql.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snoopi.core.DAL
{
    public partial class PreOrderCollection : AbstractRecordList<PreOrder, PreOrderCollection>
    {
    }

    public partial class PreOrder : AbstractRecord<PreOrder>
    {
        #region Table Schema
        private static TableSchema _TableSchema;
        public struct Columns
        {
            public static string SupplierId = "supplierId";
            public static string UniqueId = "uniqueId";
            public static string TotalPrice = "totalPrice";
            public static string BidId = "bidId";
            public static string TransactionId = "transactionId";
            public static string Created = "created";
            public static string Gifts = "gifts";
        }
        public override TableSchema GetTableSchema()
        {
            if (null == _TableSchema)
            {
                TableSchema schema = new TableSchema();
                schema.SchemaName = @"preorder";
                schema.AddColumn(Columns.UniqueId, typeof(Int64), 0, 0, 0, false, true, false, null);
                schema.AddColumn(Columns.Created, typeof(DateTime), 0, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.TotalPrice, typeof(decimal), 0, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.BidId, typeof(Int64), 0, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.SupplierId, typeof(Int64), 0, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.TransactionId, typeof(bool), 0, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.Gifts, typeof(string), 0, 0, 0, false, false, true, null);

                _TableSchema = schema;
            }

            return _TableSchema;
        }
        #endregion

        #region Private Members
        private Int64 _supplierId;
        private Int64 _uniqueId;
        private decimal _totalPrice;
        private Int64 _bidId;
        private string _transactionId;
        private DateTime _created;
        private string _gifts;

        #endregion

        #region Properties
        public Int64 SupplierId
        {
            get { return _supplierId; }
            set { _supplierId = value; }
        }
        public Int64 UniqueId
        {
            get { return _uniqueId; }
            set { _uniqueId = value; }
        }
        public decimal TotalPrice
        {
            get { return _totalPrice; }
            set { _totalPrice = value; }
        }
        public Int64 BidId
        {
            get { return _bidId; }
            set { _bidId = value; }
        }
        public string TransactionId
        {
            get { return _transactionId; }
            set { _transactionId = value; }
        }
        public DateTime Created
        {
            get { return _created; }
            set { _created = value; }
        }
        public string Gifts
        {
            get { return _gifts; }
            set { _gifts = value; }
        }
        #endregion

        #region AbstractRecord members
        public override object GetPrimaryKeyValue()
        {
            return UniqueId;
        }

        public override void Insert(ConnectorBase conn)
        {
            Query qry = new Query(TableSchema);
            qry.Insert(Columns.TransactionId, TransactionId);
            qry.Insert(Columns.Created, Created);
            qry.Insert(Columns.UniqueId, UniqueId);
            qry.Insert(Columns.BidId, BidId);
            qry.Insert(Columns.TotalPrice, TotalPrice);
            qry.Insert(Columns.SupplierId, SupplierId);
            qry.Insert(Columns.Gifts, Gifts);

            object lastInsert = null;
            if (qry.Execute(conn, out lastInsert) > 0)
            {
                MarkOld();
            }
        }
        public override void Update(ConnectorBase conn)
        {
            Query qry = new Query(TableSchema);
            qry.Update(Columns.TransactionId, TransactionId);
            qry.Update(Columns.Created, Created);
            qry.Update(Columns.BidId, BidId);
            qry.Update(Columns.TotalPrice, TotalPrice);
            qry.Update(Columns.SupplierId, SupplierId);
            qry.Update(Columns.Gifts, Gifts);


            qry.Where(Columns.UniqueId, UniqueId);

            qry.Execute(conn);
        }
        public override void Read(DataReaderBase reader)
        {
            BidId = Convert.ToInt64(reader[Columns.BidId]);
            UniqueId = Convert.ToInt64(reader[Columns.UniqueId]);
            TransactionId = reader[Columns.TransactionId].ToString();
            TotalPrice = Convert.ToDecimal(reader[Columns.TotalPrice]);
            Created = Convert.ToDateTime(reader[Columns.Created]);
            SupplierId = Convert.ToInt64(reader[Columns.SupplierId]);
            Gifts = reader[Columns.Gifts].ToString();

            IsThisANewRecord = false;
        }
        #endregion

        #region Helpers
        public static PreOrder FetchByID(Int64 uniqueId)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.UniqueId, uniqueId);
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                if (reader.Read())
                {
                    var item = new PreOrder();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }

        public static int Delete(Int64 uniqueId)
        {
            Query qry = new Query(TableSchema)
                .Delete().Where(Columns.UniqueId, uniqueId);
            return qry.Execute();
        }
        public static PreOrder FetchByID(ConnectorBase conn, Int64 uniqueId)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.UniqueId, uniqueId);
            using (DataReaderBase reader = qry.ExecuteReader(conn))
            {
                if (reader.Read())
                {
                    var item = new PreOrder();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }

        public static int Delete(ConnectorBase conn, Int64 uniqueId)
        {
            Query qry = new Query(TableSchema)
                .Delete().Where(Columns.UniqueId, uniqueId);
            return qry.Execute(conn);
        }
        #endregion
    }
}
