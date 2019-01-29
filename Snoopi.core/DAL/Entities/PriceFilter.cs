using System;
using System.Collections.Generic;
using System.Text;
using dg.Sql;
using dg.Sql.Connector;

/*
 * PriceFilter
 * PriceFilter
 * PriceId:          PRIMARY KEY; INT64; AUTOINCREMENT; 
 * PriceName:        FIXEDSTRING(128);
 * @INDEX:           NAME(ik_PriceFilter_PriceId); [PriceId];
 * */

namespace Snoopi.core.DAL
{
    public partial class PriceFilterCollection : AbstractRecordList<PriceFilter, PriceFilterCollection>
    {
    }

    public partial class PriceFilter : AbstractRecord<PriceFilter>
    {
        #region Table Schema
        private static TableSchema _TableSchema;
        public struct Columns
        {
            public static string PriceId = "PriceId";
            public static string PriceName = "PriceName";
        }
        public override TableSchema GetTableSchema()
        {
            if (null == _TableSchema)
            {
                TableSchema schema = new TableSchema();
                schema.SchemaName = @"PriceFilter";
                schema.AddColumn(Columns.PriceId, typeof(Int64), 0, 0, 0, true, true, false, null);
                schema.AddColumn(Columns.PriceName, typeof(string), DataType.Char, 128, 0, 0, false, false, false, null);

                _TableSchema = schema;

                schema.AddIndex("ik_PriceFilter_PriceId", TableSchema.ClusterMode.None, TableSchema.IndexMode.None, TableSchema.IndexType.None, Columns.PriceId);

            }

            return _TableSchema;
        }
        #endregion

        #region Private Members
        internal Int64 _PriceId = 0;
        internal string _PriceName = string.Empty;
        #endregion

        #region Properties
        public Int64 PriceId
        {
            get { return _PriceId; }
            set { _PriceId = value; }
        }
        public string PriceName
        {
            get { return _PriceName; }
            set { _PriceName = value; }
        }
        #endregion

        #region AbstractRecord members
        public override object GetPrimaryKeyValue()
        {
            return PriceId;
        }

        public override void Insert(ConnectorBase conn)
        {
            Query qry = new Query(TableSchema);
            qry.Insert(Columns.PriceName, PriceName);

            object lastInsert = null;
            if (qry.Execute(conn, out lastInsert) > 0)
            {
                PriceId = Convert.ToInt64((lastInsert));
                MarkOld();
            }
        }
        public override void Update(ConnectorBase conn)
        {
            Query qry = new Query(TableSchema);
            qry.Update(Columns.PriceName, PriceName);
            qry.Where(Columns.PriceId, PriceId);

            qry.Execute(conn);
        }
        public override void Read(DataReaderBase reader)
        {
            PriceId = Convert.ToInt64(reader[Columns.PriceId]);
            PriceName = (string)reader[Columns.PriceName];

            IsThisANewRecord = false;
        }
        #endregion

        #region Helpers
        public static PriceFilter FetchByID(Int64 PriceId)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.PriceId, PriceId);
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                if (reader.Read())
                {
                    PriceFilter item = new PriceFilter();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }

        public static int Delete(Int64 PriceId)
        {
            Query qry = new Query(TableSchema)
                .Delete().Where(Columns.PriceId, PriceId);
            return qry.Execute();
        }
        public static PriceFilter FetchByID(ConnectorBase conn, Int64 PriceId)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.PriceId, PriceId);
            using (DataReaderBase reader = qry.ExecuteReader(conn))
            {
                if (reader.Read())
                {
                    PriceFilter item = new PriceFilter();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }

        public static int Delete(ConnectorBase conn, Int64 PriceId)
        {
            Query qry = new Query(TableSchema)
                .Delete().Where(Columns.PriceId, PriceId);
            return qry.Execute(conn);
        }
        #endregion
    }

    public partial class PriceFilter 
    {
        public static PriceFilter FetchByName(string PriceName)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.PriceName, PriceName);
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                if (reader.Read())
                {
                    PriceFilter item = new PriceFilter();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }
    }
}