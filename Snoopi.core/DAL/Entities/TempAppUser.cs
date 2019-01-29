using System;
using System.Collections.Generic;
using System.Text;
using dg.Sql;
using dg.Sql.Connector;
using dg.Utilities;

/*
 * TempAppUser
 * TempAppUser
 * TempAppUserId:   PRIMARY KEY; INT64; AUTOINCREMENT;
 * CityId:          INT64; City
 * Location:        POINT; DEFAULT null;
 * DeviceUdid:      FIXEDSTRING(255);       
 * @INDEX:          NAME(ix_TempAppUser);[TempAppUserId];
 * */

namespace Snoopi.core.DAL
{

    public partial class TempAppUserCollection : AbstractRecordList<TempAppUser, TempAppUserCollection>
    {
    }

    public partial class TempAppUser : AbstractRecord<TempAppUser>
    {
        #region Table Schema
        private static TableSchema _TableSchema;
        public struct Columns
        {
            public static string TempAppUserId = "TempAppUserId";
            public static string CityId = "CityId"; // City
            public static string Location = "Location";
            public static string DeviceUdid = "DeviceUdid";
        }
        public override TableSchema GetTableSchema()
        {
            if (null == _TableSchema)
            {
                TableSchema schema = new TableSchema();
                schema.SchemaName = @"TempAppUser";
                schema.AddColumn(Columns.TempAppUserId, typeof(Int64), 0, 0, 0, true, true, false, null);
                schema.AddColumn(Columns.CityId, typeof(Int64), 0, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.Location, typeof(Geometry.Point), DataType.Point, 0, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.DeviceUdid, typeof(string), DataType.Char, 255, 0, 0, false, false, false, null);

                _TableSchema = schema;

                schema.AddIndex("ix_TempAppUser", TableSchema.ClusterMode.None, TableSchema.IndexMode.None, TableSchema.IndexType.None, Columns.TempAppUserId);

            }

            return _TableSchema;
        }
        #endregion

        #region Private Members
        internal Int64 _TempAppUserId = 0;
        internal Int64 _CityId = 0;
        internal Geometry.Point _Location = null;
        internal string _DeviceUdid = string.Empty;
        #endregion

        #region Properties
        public Int64 TempAppUserId
        {
            get { return _TempAppUserId; }
            set { _TempAppUserId = value; }
        }
        public Int64 CityId
        {
            get { return _CityId; }
            set { _CityId = value; }
        }
        public Geometry.Point Location
        {
            get { return _Location; }
            set { _Location = value; }
        }
        public string DeviceUdid
        {
            get { return _DeviceUdid; }
            set { _DeviceUdid = value; }
        }
        #endregion

        #region AbstractRecord members
        public override object GetPrimaryKeyValue()
        {
            return TempAppUserId;
        }

        public override void Insert(ConnectorBase conn)
        {
            Query qry = new Query(TableSchema);
            qry.Insert(Columns.CityId, CityId);
            qry.Insert(Columns.Location, Location);
            qry.Insert(Columns.DeviceUdid, DeviceUdid);

            object lastInsert = null;
            if (qry.Execute(conn, out lastInsert) > 0)
            {
                TempAppUserId = Convert.ToInt64((lastInsert));
                MarkOld();
            }
        }
        public override void Update(ConnectorBase conn)
        {
            Query qry = new Query(TableSchema);
            qry.Update(Columns.CityId, CityId);
            qry.Update(Columns.Location, Location);
            qry.Update(Columns.DeviceUdid, DeviceUdid);
            qry.Where(Columns.TempAppUserId, TempAppUserId);

            qry.Execute(conn);
        }
        public override void Read(DataReaderBase reader)
        {
            TempAppUserId = Convert.ToInt64(reader[Columns.TempAppUserId]);
            CityId = Convert.ToInt64(reader[Columns.CityId]);
            Location = reader.GetGeometry(Columns.Location) as Geometry.Point;
            DeviceUdid = (string)reader[Columns.DeviceUdid];

            IsThisANewRecord = false;
        }
        #endregion

        #region Helpers
        public static TempAppUser FetchByID(Int64 TempAppUserId)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.TempAppUserId, TempAppUserId);
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                if (reader.Read())
                {
                    TempAppUser item = new TempAppUser();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }

        public static int Delete(Int64 TempAppUserId)
        {
            Query qry = new Query(TableSchema)
                .Delete().Where(Columns.TempAppUserId, TempAppUserId);
            return qry.Execute();
        }
        public static TempAppUser FetchByID(ConnectorBase conn, Int64 TempAppUserId)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.TempAppUserId, TempAppUserId);
            using (DataReaderBase reader = qry.ExecuteReader(conn))
            {
                if (reader.Read())
                {
                    TempAppUser item = new TempAppUser();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }

        public static int Delete(ConnectorBase conn, Int64 TempAppUserId)
        {
            Query qry = new Query(TableSchema)
                .Delete().Where(Columns.TempAppUserId, TempAppUserId);
            return qry.Execute(conn);
        }
        #endregion
    }

}
