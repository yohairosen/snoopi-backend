using System;
using System.Collections.Generic;
using System.Text;
using dg.Sql;
using dg.Sql.Connector;
using dg.Utilities;

/*
 * Service
 * Service
 * ServiceId:        PRIMARY KEY; INT64; AUTOINCREMENT;
 * ServiceName:      FIXEDSTRING(64); Service Name
 * ServiceComment:   FIXEDSTRING(255); 
 * IsHomeService:    BOOL; Defaulr false;
 * @INDEX:           NAME(ix_Service_ServiceName); [ServiceName]; UNIQUE;
 * */

namespace Snoopi.core.DAL
{

    public partial class ServiceCollection : AbstractRecordList<Service, ServiceCollection>
    {
    }

    public partial class Service : AbstractRecord<Service>
    {
        #region Table Schema
        private static TableSchema _TableSchema;
        public struct Columns
        {
            public static string ServiceId = "ServiceId";
            public static string ServiceName = "ServiceName"; // Service Name
            public static string ServiceComment = "ServiceComment";
            public static string IsHomeService = "IsHomeService";
        }
        public override TableSchema GetTableSchema()
        {
            if (null == _TableSchema)
            {
                TableSchema schema = new TableSchema();
                schema.SchemaName = @"Service";
                schema.AddColumn(Columns.ServiceId, typeof(Int64), 0, 0, 0, true, true, false, null);
                schema.AddColumn(Columns.ServiceName, typeof(string), DataType.Char, 64, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.ServiceComment, typeof(string), DataType.Char, 255, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.IsHomeService, typeof(bool), 0, 0, 0, false, false, false, null);

                _TableSchema = schema;

                schema.AddIndex("ix_Service_ServiceName", TableSchema.ClusterMode.None, TableSchema.IndexMode.Unique, TableSchema.IndexType.None, Columns.ServiceName);

            }

            return _TableSchema;
        }
        #endregion

        #region Private Members
        internal Int64 _ServiceId = 0;
        internal string _ServiceName = string.Empty;
        internal string _ServiceComment = string.Empty;
        internal bool _IsHomeService = false;
        #endregion

        #region Properties
        public Int64 ServiceId
        {
            get { return _ServiceId; }
            set { _ServiceId = value; }
        }
        public string ServiceName
        {
            get { return _ServiceName; }
            set { _ServiceName = value; }
        }
        public string ServiceComment
        {
            get { return _ServiceComment; }
            set { _ServiceComment = value; }
        }
        public bool IsHomeService
        {
            get { return _IsHomeService; }
            set { _IsHomeService = value; }
        }
        #endregion

        #region AbstractRecord members
        public override object GetPrimaryKeyValue()
        {
            return ServiceId;
        }

        public override void Insert(ConnectorBase conn)
        {
            Query qry = new Query(TableSchema);
            qry.Insert(Columns.ServiceName, ServiceName);
            qry.Insert(Columns.ServiceComment, ServiceComment);
            qry.Insert(Columns.IsHomeService, IsHomeService);

            object lastInsert = null;
            if (qry.Execute(conn, out lastInsert) > 0)
            {
                ServiceId = Convert.ToInt64((lastInsert));
                MarkOld();
            }
        }
        public override void Update(ConnectorBase conn)
        {
            Query qry = new Query(TableSchema);
            qry.Update(Columns.ServiceName, ServiceName);
            qry.Update(Columns.ServiceComment, ServiceComment);
            qry.Update(Columns.IsHomeService, IsHomeService);
            qry.Where(Columns.ServiceId, ServiceId);

            qry.Execute(conn);
        }
        public override void Read(DataReaderBase reader)
        {
            ServiceId = Convert.ToInt64(reader[Columns.ServiceId]);
            ServiceName = (string)reader[Columns.ServiceName];
            ServiceComment = (string)reader[Columns.ServiceComment];
            IsHomeService = Convert.ToBoolean(reader[Columns.IsHomeService]);

            IsThisANewRecord = false;
        }
        #endregion

        #region Helpers
        public static Service FetchByID(Int64 ServiceId)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.ServiceId, ServiceId);
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                if (reader.Read())
                {
                    Service item = new Service();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }

        public static int Delete(Int64 ServiceId)
        {
            Query qry = new Query(TableSchema)
                .Delete().Where(Columns.ServiceId, ServiceId);
            return qry.Execute();
        }
        public static Service FetchByID(ConnectorBase conn, Int64 ServiceId)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.ServiceId, ServiceId);
            using (DataReaderBase reader = qry.ExecuteReader(conn))
            {
                if (reader.Read())
                {
                    Service item = new Service();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }

        public static int Delete(ConnectorBase conn, Int64 ServiceId)
        {
            Query qry = new Query(TableSchema)
                .Delete().Where(Columns.ServiceId, ServiceId);
            return qry.Execute(conn);
        }
        #endregion
    }
    public partial class Service
    {
        static public Int64 FetchHomeService()
        {
            using (DataReaderBase reader = new Query(TableSchema).Where(Columns.IsHomeService, true).ExecuteReader())
            {
                if (reader.Read())
                {
                    Service s = new Service();
                    s.Read(reader);
                    return s.ServiceId;
                }
            }
            return 0;
        }

        static public List<Int64> FetchAllHomeServices()
        {
            List<Int64> HomeServices = new List<Int64>();
            using (DataReaderBase reader = new Query(TableSchema).Where(Columns.IsHomeService, true).ExecuteReader())
            {
                while (reader.Read())
                    {
                    Service s = new Service();
                    s.Read(reader);
                    HomeServices.Add(s.ServiceId);
                }
            }
            return HomeServices;
        }



        static public Service FetchByName(string name)
        {
            using (DataReaderBase reader = new Query(TableSchema).Where(Columns.ServiceName, name).ExecuteReader())
            {
                if (reader.Read())
                {
                    Service s = new Service();
                    s.Read(reader);
                    return s;
                }
            }
            return null;
        }
    }
}
