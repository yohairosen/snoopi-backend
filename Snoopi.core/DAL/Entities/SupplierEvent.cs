using dg.Sql;
using dg.Sql.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snoopi.core.DAL.Entities
{
    public partial class SupplierEvent : AbstractRecord<SupplierEvent>
    {
        #region Constructors
        public SupplierEvent(SupplierEvent sup_event)
        {
            Id = sup_event.Id;
            SupplierId = sup_event.SupplierId;
            UserId = sup_event.UserId;
            EventTime = DateTime.Now;
            EventType = sup_event.EventType;
        }
        public SupplierEvent() : base()
        {

        }
        #endregion
        #region Table Schema
        private static TableSchema _TableSchema;
        public struct Columns
        {
            public static string id = "id";
            public static string SupplierId = "supplier_id";
            public static string UserId = "user_id";
            public static string EventTime = "event_time";
            public static string EventType = "event_type";
        }
        public override TableSchema GetTableSchema()
        {
            if (null == _TableSchema)
            {
                TableSchema schema = new TableSchema();
                schema.SchemaName = @"supplierevent";
                schema.AddColumn(Columns.id, typeof(Int64), 0, 0, 0, true, true, false, null);
                schema.AddColumn(Columns.SupplierId, typeof(Int64), 0, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.UserId, typeof(Int64), 0, 0, 0, false, false, true, null);
                schema.AddColumn(Columns.EventTime, typeof(DateTime), 0, 0, 0, false, false, false, DateTime.Now);
                schema.AddColumn(Columns.EventType, typeof(string), 0, 0, 0, false, false, false, null);

                schema.AddIndex("pk_SupplierEventId", TableSchema.ClusterMode.None, TableSchema.IndexMode.PrimaryKey, TableSchema.IndexType.None, Columns.id);
                _TableSchema = schema;
            }

            return _TableSchema;
        }
        #endregion

        #region Private Members
        private Int64 _id = 0;
        private Int64 _supplierId = 0;
        private Int64 _userId = 0;
        private DateTime _eventTime = DateTime.MinValue;
        private string _eventType = string.Empty;


        #endregion

        #region Properties

        public Int64 Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public Int64 SupplierId
        {
            get { return _supplierId; }
            set { _supplierId = value; }
        }
        public Int64 UserId
        {
            get { return _userId; }
            set { _userId = value; }
        }

        public DateTime EventTime
        {
            get { return _eventTime; }
            set { _eventTime = value; }
        }
        public string EventType
        {
            get { return _eventType; }
            set { _eventType = value; }
        }

        #endregion

        #region Public Const
        public const string CLICK = "Click";
        public const string PHONE_CALL = "PhoneCall";
        #endregion

        #region AbstractRecord members

        public override void Insert(ConnectorBase conn)
        {
            Query qry = new Query(TableSchema);
            qry.Insert(Columns.SupplierId, SupplierId);
            qry.Insert(Columns.UserId, UserId);
            qry.Insert(Columns.EventTime, EventTime);
            qry.Insert(Columns.EventType, EventType);

            object lastInsert = null;
            if (qry.Execute(conn, out lastInsert) > 0)
            {
                Id = Convert.ToInt64((lastInsert));
                MarkOld();
            }
        }
        public override void Update(ConnectorBase conn)
        {
            Query qry = new Query(TableSchema);
            qry.Update(Columns.SupplierId, SupplierId);
            qry.Update(Columns.UserId, UserId);
            qry.Update(Columns.EventTime, EventTime);
            qry.Update(Columns.EventType, EventType);
            qry.Where(Columns.id, Id);

            qry.Execute(conn);
        }
        public override void Read(DataReaderBase reader)
        {
            Id = Convert.ToInt64(reader[Columns.id]);
            SupplierId = Convert.ToInt64(reader[Columns.SupplierId]);
            UserId = Convert.ToInt64(reader[Columns.UserId]);
            EventTime = Convert.ToDateTime(reader[Columns.EventTime]);
            EventType = (string)(reader[Columns.EventType]);
        }

        public override object GetPrimaryKeyValue()
        {
            return null;
        }

        #endregion

        #region Helpers

        public static SupplierEvent FetchByID(Int64 supplier_event_Id)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.id, supplier_event_Id);
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                if (reader.Read())
                {
                    SupplierEvent item = new SupplierEvent();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }

        public static SupplierEvent FetchByID(ConnectorBase conn,Int64 supplier_event_Id)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.id, supplier_event_Id);
            using (DataReaderBase reader = qry.ExecuteReader(conn))
            {
                if (reader.Read())
                {
                    SupplierEvent item = new SupplierEvent();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }
        #endregion

    }
}
