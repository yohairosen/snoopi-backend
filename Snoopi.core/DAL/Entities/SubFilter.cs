using System;
using System.Collections.Generic;
using System.Text;
using dg.Sql;
using dg.Sql.Connector;
using dg.Utilities;

/*
 * SubFilter
 * SubFilter
 * SubFilterId:       PRIMARY KEY; INT64; AUTOINCREMENT;
 * SubFilterName:     FIXEDSTRING(64); SubFilter Name
 * FilterId:          INT64;
 * @FOREIGNKEY:       NAME(fk_SubFilter_FilterId); FOREIGNTABLE(Filter); COLUMNS[FilterId]; FOREIGNCOLUMNS[FilterId]; ONUPDATE(CASCADE); ONDELETE(CASCADE);
 * @INDEX:            NAME(ix_SubFilter_SubFilterName); [SubFilterName];
 * */

//@INDEX:            NAME(ix_SubFilter_SubFilterName); [SubFilterName]; UNIQUE;
namespace Snoopi.core.DAL
{
    public partial class SubFilterCollection : AbstractRecordList<SubFilter, SubFilterCollection>
    {
    }

    public partial class SubFilter : AbstractRecord<SubFilter>
    {
        #region Table Schema
        private static TableSchema _TableSchema;
        public struct Columns
        {
            public const string SubFilterId = "SubFilterId";
            public const string SubFilterName = "SubFilterName"; // SubFilter Name
            public const string FilterId = "FilterId";
        }
        public override TableSchema GetTableSchema()
        {
            if (null == _TableSchema)
            {
                TableSchema schema = new TableSchema();
                schema.SchemaName = @"SubFilter";
                schema.AddColumn(Columns.SubFilterId, typeof(Int64), 0, 0, 0, true, true, false, null);
                schema.AddColumn(Columns.SubFilterName, typeof(string), 64, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.FilterId, typeof(Int64), 0, 0, 0, false, false, false, null);

                _TableSchema = schema;

                schema.AddIndex("ix_SubFilter_SubFilterName", TableSchema.ClusterMode.None, TableSchema.IndexMode.None, TableSchema.IndexType.None, Columns.SubFilterName);

                schema.AddForeignKey("fk_SubFilter_FilterId", SubFilter.Columns.FilterId, Filter.TableSchema.SchemaName, Filter.Columns.FilterId, TableSchema.ForeignKeyReference.Cascade, TableSchema.ForeignKeyReference.Cascade);

            }

            return _TableSchema;
        }
        #endregion

        #region Private Members
        internal Int64 _SubFilterId = 0;
        internal string _SubFilterName = string.Empty;
        internal Int64 _FilterId = 0;
        #endregion

        #region Properties
        public Int64 SubFilterId
        {
            get { return _SubFilterId; }
            set { _SubFilterId = value; }
        }
        public string SubFilterName
        {
            get { return _SubFilterName; }
            set { _SubFilterName = value; }
        }
        public Int64 FilterId
        {
            get { return _FilterId; }
            set { _FilterId = value; }
        }
        #endregion

        #region AbstractRecord members
        public override object GetPrimaryKeyValue()
        {
            return SubFilterId;
        }

        public override void Insert(ConnectorBase conn)
        {
            Query qry = new Query(TableSchema);
            qry.Insert(Columns.SubFilterName, SubFilterName);
            qry.Insert(Columns.FilterId, FilterId);

            object lastInsert = null;
            if (qry.Execute(conn, out lastInsert) > 0)
            {
                SubFilterId = Convert.ToInt64((lastInsert));
                MarkOld();
            }
        }
        public override void Update(ConnectorBase conn)
        {
            Query qry = new Query(TableSchema);
            qry.Update(Columns.SubFilterName, SubFilterName);
            qry.Update(Columns.FilterId, FilterId);
            qry.Where(Columns.SubFilterId, SubFilterId);

            qry.Execute(conn);
        }
        public override void Read(DataReaderBase reader)
        {
            SubFilterId = Convert.ToInt64(reader[Columns.SubFilterId]);
            SubFilterName = (string)reader[Columns.SubFilterName];
            FilterId = Convert.ToInt64(reader[Columns.FilterId]);

            IsThisANewRecord = false;
        }
        #endregion

        #region Helpers
        public static SubFilter FetchByID(Int64 SubFilterId)
        {
            Query qry = new Query(TableSchema)
            .Where(Columns.SubFilterId, SubFilterId);
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                if (reader.Read())
                {
                    SubFilter item = new SubFilter();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }

        public static int Delete(Int64 SubFilterId)
        {
            Query qry = new Query(TableSchema)
            .Delete().Where(Columns.SubFilterId, SubFilterId);
            return qry.Execute();
        }
        public static SubFilter FetchByID(ConnectorBase conn, Int64 SubFilterId)
        {
            Query qry = new Query(TableSchema)
            .Where(Columns.SubFilterId, SubFilterId);
            using (DataReaderBase reader = qry.ExecuteReader(conn))
            {
                if (reader.Read())
                {
                    SubFilter item = new SubFilter();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }

        public static int Delete(ConnectorBase conn, Int64 SubFilterId)
        {
            Query qry = new Query(TableSchema)
            .Delete().Where(Columns.SubFilterId, SubFilterId);
            return qry.Execute(conn);
        }
        #endregion
    }

    public partial class SubFilter 
    {
        public static SubFilter FetchByName(string SubFilterName)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.SubFilterName, SubFilterName);
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                if (reader.Read())
                {
                    SubFilter item = new SubFilter();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }
    }


}
