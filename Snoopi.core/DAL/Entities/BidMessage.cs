using System;
using System.Collections.Generic;
using System.Text;
using dg.Sql;
using dg.Sql.Connector;
using dg.Utilities;

namespace Snoopi.core.DAL
{
    class BidMessageCollection : AbstractRecordList<BidMessage, BidMessageCollection>
    {
    }

    public partial class BidMessage : AbstractRecord<BidMessage>
    {
        #region Constructors
        public BidMessage(BidMessage msg)
        {
            BidId = msg.BidId;
            IsActive = msg.IsActive;
            SendingTime = DateTime.Now;
            OriginalSupplierId = msg.OriginalSupplierId;
        }
        public BidMessage() : base()
        {

        }
        #endregion
        #region Table Schema
        private static TableSchema _TableSchema;
        public struct Columns
        {
            public static string BidId = "BidId";
            public static string SupplierId = "SupplierId";
            public static string IsActive = "IsActive";
            public static string ExpirationTime = "ExpirationTime";
            public static string Stage = "Stage";
            public static string MessageId = "MessageId";
            public static string SendingTime = "SendingTime";
            public static string OriginalSupplierId = "OriginalSupplierId";
        }
        public override TableSchema GetTableSchema()
        {
            if (null == _TableSchema)
            {
                TableSchema schema = new TableSchema();
                schema.SchemaName = @"bidmessage";
                schema.AddColumn(Columns.BidId, typeof(Int64), 0, 0, 0, false, true, false, null);
                schema.AddColumn(Columns.SupplierId, typeof(Int64), 0, 0, 0, false, true, false, null);
                schema.AddColumn(Columns.IsActive, typeof(bool), 0, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.ExpirationTime, typeof(DateTime), 0, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.MessageId, typeof(Int64), 0, 0, 0, false, true, false, null);
                schema.AddColumn(Columns.Stage, typeof(string), DataType.Char, 64, 0, 0, false, false, false, string.Empty);
                schema.AddColumn(Columns.SendingTime, typeof(DateTime), 0, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.OriginalSupplierId, typeof(Int64), 0, 0, 0, false, false, false, null);


                schema.AddIndex("pk_MessageId", TableSchema.ClusterMode.None, TableSchema.IndexMode.PrimaryKey, TableSchema.IndexType.None, Columns.SupplierId, Columns.MessageId);
                schema.AddIndex("ix_BidMessage_IsActive", TableSchema.ClusterMode.None, TableSchema.IndexMode.None, TableSchema.IndexType.None, Columns.IsActive);
                schema.AddIndex("ix_BidMessage_ExpirationTime", TableSchema.ClusterMode.None, TableSchema.IndexMode.None, TableSchema.IndexType.None, Columns.ExpirationTime);
                schema.AddIndex("ix_BidMessage_SupplierId", TableSchema.ClusterMode.None, TableSchema.IndexMode.None, TableSchema.IndexType.None, Columns.SupplierId);

                schema.AddForeignKey("fk_Supplier_Message", Columns.SupplierId, AppSupplier.TableSchema.SchemaName, AppSupplier.Columns.SupplierId, TableSchema.ForeignKeyReference.Cascade, TableSchema.ForeignKeyReference.Cascade);
                schema.AddForeignKey("fk_Bid_Message", Columns.MessageId, Bid.TableSchema.SchemaName, Bid.Columns.BidId, TableSchema.ForeignKeyReference.Cascade, TableSchema.ForeignKeyReference.Cascade);

                _TableSchema = schema;
            }

            return _TableSchema;
        }
        #endregion

        #region Private Members
        private string _stage = string.Empty;
        private Int64 _messageId = 0;
        private Int64 _supplierId = 0;
        private Int64 _bidId = 0;
        private DateTime _expirationTime = DateTime.MinValue;
        private DateTime _sendingTime = DateTime.MinValue;
        private bool _isActive = false;
        private Int64 _originalSupplierId = 0;

        #endregion

        #region Properties
       
        public Int64 SupplierId
        {
            get { return _supplierId; }
            set { _supplierId = value; }
        }
        
        public Int64 BidId
        {
            get { return _bidId; }
            set { _bidId = value; }
        }
        
        public string Stage
        {
            get { return _stage; }
            set { _stage = value; }
        }

        public DateTime ExpirationTime
        {
            get { return _expirationTime; }
            set { _expirationTime = value; }
        }
        public DateTime SendingTime
        {
            get { return _sendingTime; }
            set { _sendingTime = value; }
        }

        public bool IsActive
        {
            get { return _isActive; }
            set { _isActive = value; }
        }

        public Int64 MessageId
        {
            get { return _messageId;}
            set { _messageId = value; }
        }

        public Int64 OriginalSupplierId
        {
            get { return _originalSupplierId; }
            set { _originalSupplierId = value; }
        }
        #endregion

        #region AbstractRecord members
       
        public override void Insert(ConnectorBase conn)
        {
            Query qry = new Query(TableSchema);
            qry.Insert(Columns.BidId, BidId);
            qry.Insert(Columns.IsActive, IsActive);
            qry.Insert(Columns.SupplierId, SupplierId);
            qry.Insert(Columns.ExpirationTime, ExpirationTime);
            qry.Insert(Columns.Stage, Stage);
            qry.Insert(Columns.SendingTime, SendingTime);
            qry.Insert(Columns.OriginalSupplierId, OriginalSupplierId);
           
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
            qry.Update(Columns.IsActive, IsActive);
            qry.Update(Columns.SupplierId, SupplierId);
            qry.Update(Columns.ExpirationTime, ExpirationTime);
            qry.Update(Columns.Stage, Stage);
            qry.Update(Columns.SendingTime, SendingTime);
            qry.Update(Columns.OriginalSupplierId, OriginalSupplierId);
            qry.Where(Columns.MessageId, MessageId);

            qry.Execute(conn);
        }
        public override void Read(DataReaderBase reader)
        {
            BidId = Convert.ToInt64(reader[Columns.BidId]);
            SupplierId = Convert.ToInt64(reader[Columns.SupplierId]);
            Stage = (string)reader[Columns.Stage];
            MessageId = Convert.ToInt64(reader[Columns.MessageId]);
            IsActive = Convert.ToBoolean(reader[Columns.IsActive]);
            ExpirationTime = Convert.ToDateTime(reader[Columns.ExpirationTime]);
            SendingTime = Convert.ToDateTime(reader[Columns.SendingTime]);
            OriginalSupplierId = Convert.ToInt64(reader[Columns.OriginalSupplierId]);
            IsThisANewRecord = false;
        }

        public override object GetPrimaryKeyValue()
        {
            return null;
        }

        #endregion

        #region Helpers

        public static BidMessage FetchByID(Int64 messageId)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.MessageId, messageId);
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                if (reader.Read())
                {
                    BidMessage item = new BidMessage();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }

        public static BidMessage FetchByID(ConnectorBase conn, Int64 messageId)
        {
            Query qry = new Query(TableSchema)
              .Where(Columns.MessageId, messageId);
            using (DataReaderBase reader = qry.ExecuteReader(conn))
            {
                if (reader.Read())
                {
                    BidMessage item = new BidMessage();
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

    public partial class BidMessage
    {

      
    }

}

