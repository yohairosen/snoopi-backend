using System;
using System.Collections.Generic;
using System.Text;
using dg.Sql;
using dg.Sql.Connector;
using dg.Utilities;

/*
 * Filter
 * Filter
 * FilterId:       PRIMARY KEY; INT64; AUTOINCREMENT;
 * FilterName:     FIXEDSTRING(64); Filter Name
 * @INDEX:           NAME(ix_Filter_FilterName); [FilterName]; UNIQUE;
 * */

namespace Snoopi.core.DAL
{

    public partial class FilterCollection : AbstractRecordList<Filter, FilterCollection>
    {
    }

    public partial class Filter : AbstractRecord<Filter>
    {
        #region Table Schema
        private static TableSchema _TableSchema;
        public struct Columns
        {
            public static string FilterId = "FilterId";
            public static string FilterName = "FilterName"; // Filter Name
        }
        public override TableSchema GetTableSchema()
        {
            if (null == _TableSchema)
            {
                TableSchema schema = new TableSchema();
                schema.SchemaName = @"Filter";
                schema.AddColumn(Columns.FilterId, typeof(Int64), 0, 0, 0, true, true, false, null);
                schema.AddColumn(Columns.FilterName, typeof(string), DataType.Char, 64, 0, 0, false, false, false, null);

                _TableSchema = schema;

                schema.AddIndex("ix_Filter_FilterName", TableSchema.ClusterMode.None, TableSchema.IndexMode.Unique, TableSchema.IndexType.None, Columns.FilterName);

            }

            return _TableSchema;
        }
        #endregion

        #region Private Members
        internal Int64 _FilterId = 0;
        internal string _FilterName = string.Empty;
        #endregion

        #region Properties
        public Int64 FilterId
        {
            get { return _FilterId; }
            set { _FilterId = value; }
        }
        public string FilterName
        {
            get { return _FilterName; }
            set { _FilterName = value; }
        }
        #endregion

        #region AbstractRecord members
        public override object GetPrimaryKeyValue()
        {
            return FilterId;
        }

        public override void Insert(ConnectorBase conn)
        {
            Query qry = new Query(TableSchema);
            qry.Insert(Columns.FilterName, FilterName);

            object lastInsert = null;
            if (qry.Execute(conn, out lastInsert) > 0)
            {
                FilterId = Convert.ToInt64((lastInsert));
                MarkOld();
            }
        }
        public override void Update(ConnectorBase conn)
        {
            Query qry = new Query(TableSchema);
            qry.Update(Columns.FilterName, FilterName);
            qry.Where(Columns.FilterId, FilterId);

            qry.Execute(conn);
        }
        public override void Read(DataReaderBase reader)
        {
            FilterId = Convert.ToInt64(reader[Columns.FilterId]);
            FilterName = (string)reader[Columns.FilterName];

            IsThisANewRecord = false;
        }
        #endregion

        #region Helpers
        public static Filter FetchByID(Int64 FilterId)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.FilterId, FilterId);
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                if (reader.Read())
                {
                    Filter item = new Filter();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }

        public static int Delete(Int64 FilterId)
        {
            Query qry = new Query(TableSchema)
                .Delete().Where(Columns.FilterId, FilterId);
            return qry.Execute();
        }
        public static Filter FetchByID(ConnectorBase conn, Int64 FilterId)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.FilterId, FilterId);
            using (DataReaderBase reader = qry.ExecuteReader(conn))
            {
                if (reader.Read())
                {
                    Filter item = new Filter();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }

        public static int Delete(ConnectorBase conn, Int64 FilterId)
        {
            Query qry = new Query(TableSchema)
                .Delete().Where(Columns.FilterId, FilterId);
            return qry.Execute(conn);
        }
        #endregion
    }
    public partial class Filter 
    {
        public static Filter FetchByName(String FilterName)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.FilterName, FilterName);
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                if (reader.Read())
                {
                    Filter item = new Filter();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }
    
    }

}
