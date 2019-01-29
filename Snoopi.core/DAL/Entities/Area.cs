using System;
using System.Collections.Generic;
using System.Text;
using dg.Sql;
using dg.Sql.Connector;
using dg.Utilities;

/*
 * Area
 * Area
 * AreaId:        PRIMARY KEY; INT64; AUTOINCREMENT;
 * AreaName:      FIXEDSTRING(64);
 * @INDEX:        NAME(ix_Area_AreaId); [AreaId]; 
 * */

namespace Snoopi.core.DAL
{

    public partial class AreaCollection : AbstractRecordList<Area, AreaCollection>
    {
    }

    public partial class Area : AbstractRecord<Area>
    {
        #region Table Schema
        private static TableSchema _TableSchema;
        public struct Columns
        {
            public static string AreaId = "AreaId";
            public static string AreaName = "AreaName";
        }
        public override TableSchema GetTableSchema()
        {
            if (null == _TableSchema)
            {
                TableSchema schema = new TableSchema();
                schema.SchemaName = @"Area";
                schema.AddColumn(Columns.AreaId, typeof(Int64), 0, 0, 0, true, true, false, null);
                schema.AddColumn(Columns.AreaName, typeof(string), DataType.Char, 64, 0, 0, false, false, false, null);

                _TableSchema = schema;

                schema.AddIndex("ix_Area_AreaId", TableSchema.ClusterMode.None, TableSchema.IndexMode.None, TableSchema.IndexType.None, Columns.AreaId);

            }

            return _TableSchema;
        }
        #endregion

        #region Private Members
        internal Int64 _AreaId = 0;
        internal string _AreaName = string.Empty;
        #endregion

        #region Properties
        public Int64 AreaId
        {
            get { return _AreaId; }
            set { _AreaId = value; }
        }
        public string AreaName
        {
            get { return _AreaName; }
            set { _AreaName = value; }
        }
        #endregion

        #region AbstractRecord members
        public override object GetPrimaryKeyValue()
        {
            return AreaId;
        }

        public override void Insert(ConnectorBase conn)
        {
            Query qry = new Query(TableSchema);
            qry.Insert(Columns.AreaName, AreaName);

            object lastInsert = null;
            if (qry.Execute(conn, out lastInsert) > 0)
            {
                AreaId = Convert.ToInt64((lastInsert));
                MarkOld();
            }
        }
        public override void Update(ConnectorBase conn)
        {
            Query qry = new Query(TableSchema);
            qry.Update(Columns.AreaName, AreaName);
            qry.Where(Columns.AreaId, AreaId);

            qry.Execute(conn);
        }
        public override void Read(DataReaderBase reader)
        {
            AreaId = Convert.ToInt64(reader[Columns.AreaId]);
            AreaName = (string)reader[Columns.AreaName];

            IsThisANewRecord = false;
        }
        #endregion

        #region Helpers
        public static Area FetchByID(Int64 AreaId)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.AreaId, AreaId);
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                if (reader.Read())
                {
                    Area item = new Area();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }

        public static int Delete(Int64 AreaId)
        {
            Query qry = new Query(TableSchema)
                .Delete().Where(Columns.AreaId, AreaId);
            return qry.Execute();
        }
        public static Area FetchByID(ConnectorBase conn, Int64 AreaId)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.AreaId, AreaId);
            using (DataReaderBase reader = qry.ExecuteReader(conn))
            {
                if (reader.Read())
                {
                    Area item = new Area();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }

        public static int Delete(ConnectorBase conn, Int64 AreaId)
        {
            Query qry = new Query(TableSchema)
                .Delete().Where(Columns.AreaId, AreaId);
            return qry.Execute(conn);
        }
        #endregion
    }

    public partial class Area
    {

        public static Area FetchByName(string name)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.AreaName, name);
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                if (reader.Read())
                {
                    Area item = new Area();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }
    }

}
