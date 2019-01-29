using dg.Sql;
using dg.Sql.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Snoopi.core.DAL.Entities
{
    public enum enEventType
    {
        click = 1,
        hover = 2,

    }
    public enum enSource
    {
        site_users = 1,
        app_users = 2,

    }
    partial class CompanyEvent:AbstractRecord<CompanyEvent>
    {
        #region Constructors
        public CompanyEvent(CompanyEvent comp_event)
        {
            Id = comp_event.Id;
            CompanyId = comp_event.CompanyId;
            UserId = comp_event.UserId;
            EventTime = DateTime.Now;
            EventType = comp_event.EventType;
        }
        public CompanyEvent() : base()
        {

        }
        #endregion
        #region Table Schema
        private static TableSchema _TableSchema;
        public struct Columns
        {
            public static string id = "id";
            public static string CompanyId = "company_id";
            public static string UserId = "user_id";
            public static string EventTime = "event_time";
            public static string EventType = "event_type"; //enEventType
            public static string Source = "source";//enSource
        }
        public override TableSchema GetTableSchema()
        {
            if (null == _TableSchema)
            {
                TableSchema schema = new TableSchema();
                schema.SchemaName = @"companyevent";
                schema.AddColumn(Columns.id, typeof(Int64), 0, 0, 0, true, true, false, null);
                schema.AddColumn(Columns.CompanyId, typeof(Int64), 0, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.UserId, typeof(Int64), 0, 0, 0, false, false, true, null);
                schema.AddColumn(Columns.EventTime, typeof(DateTime), 0, 0, 0, false, false, false, DateTime.Now);
                schema.AddColumn(Columns.EventType, typeof(enEventType), 0, 0, 0, false, false, false, enEventType.click);
                schema.AddColumn(Columns.Source, typeof(enSource), 0, 0, 0, false, false, false, enSource.app_users);
                _TableSchema = schema;
            }

            return _TableSchema;
        }
        #endregion

        #region Private Members
        private Int64 _id = 0;
        private Int64 _companyId = 0;
        private Int64 _userId = 0;
        private DateTime _eventTime = DateTime.MinValue;
        private enEventType _eventType = enEventType.click;
        private enSource _source = enSource.app_users;



        #endregion

        #region Properties

        public Int64 Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public Int64 CompanyId
        {
            get { return _companyId; }
            set { _companyId = value; }
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
        public enEventType EventType
        {
            get { return _eventType; }
            set { _eventType = value; }
        }
        public enSource Source
        {
            get { return _source; }
            set { _source = value; }
        }
        #endregion

        #region AbstractRecord members

        public override void Insert(ConnectorBase conn)
        {
            Query qry = new Query(TableSchema);
            qry.Insert(Columns.CompanyId, CompanyId);
            qry.Insert(Columns.UserId, UserId);
            qry.Insert(Columns.EventTime, EventTime);
            qry.Insert(Columns.EventType,(int) EventType);
            qry.Insert(Columns.Source,(int) Source);

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
            qry.Update(Columns.CompanyId, CompanyId);
            qry.Update(Columns.UserId, UserId);
            qry.Update(Columns.EventTime, EventTime);
            qry.Update(Columns.EventType,(int) EventType);
            qry.Update(Columns.Source, (int)Source);
            qry.Where(Columns.id, Id);


            qry.Execute(conn);
        }
        public override void Read(DataReaderBase reader)
        {
            Id = Convert.ToInt64(reader[Columns.id]);
            CompanyId = Convert.ToInt64(reader[Columns.CompanyId]);
            UserId = Convert.ToInt64(reader[Columns.UserId]);
            EventTime = Convert.ToDateTime(reader[Columns.EventTime]);
            EventType = (enEventType)Convert.ToInt16(reader[Columns.EventType]);
            Source = (enSource)Convert.ToInt16(reader[Columns.Source]);
        }

        public override object GetPrimaryKeyValue()
        {
            return null;
        }

        #endregion

        #region Helpers

        public static CompanyEvent FetchByID(Int64 supplier_event_Id)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.id, supplier_event_Id);
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                if (reader.Read())
                {
                    CompanyEvent item = new CompanyEvent();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }

        public static CompanyEvent FetchByID(ConnectorBase conn, Int64 company_event_Id)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.id, company_event_Id);
            using (DataReaderBase reader = qry.ExecuteReader(conn))
            {
                if (reader.Read())
                {
                    CompanyEvent item = new CompanyEvent();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }
        #endregion
    }
}
