using System;
using System.Collections.Generic;
using System.Text;
using dg.Sql;
using dg.Sql.Connector;
using dg.Utilities;

/*
 * SubCategoryFilter
 * SubCategoryFilter
 * SubCategoryId:       INT64;
 * FilterId:        INT64;
 * CategoryId:     INT64;
 * @INDEX:          NAME(pk_SubCategoryFilter);PRIMARYKEY;[SubCategoryId,FilterId,CategoryId]; 
 * @INDEX:          NAME(ix_SubCategoryFilter_SubCategoryId);[SubCategoryId];
 * @INDEX:          NAME(ix_SubCategoryFilter_FilterId);[FilterId];
 * @INDEX:          NAME(ix_SubCategoryFilter_CategoryId);[CategoryId];
 * @FOREIGNKEY:     NAME(fk_SubCategoryFilter_SubCategoryId); FOREIGNTABLE(SubCategory); COLUMNS[SubCategoryId]; FOREIGNCOLUMNS[SubCategoryId]; ONUPDATE(CASCADE); ONDELETE(CASCADE);
 * @FOREIGNKEY:     NAME(fk_SubCategoryFilter_CategoryId); FOREIGNTABLE(Category); COLUMNS[CategoryId]; FOREIGNCOLUMNS[CategoryId]; ONUPDATE(CASCADE); ONDELETE(CASCADE);
 * @FOREIGNKEY:     NAME(fk_SubCategoryFilter_FilterId); FOREIGNTABLE(Filter); COLUMNS[FilterId]; FOREIGNCOLUMNS[FilterId]; ONUPDATE(CASCADE); ONDELETE(CASCADE);
 * */

namespace Snoopi.core.DAL
{

    public partial class SubCategoryFilterCollection : AbstractRecordList<SubCategoryFilter, SubCategoryFilterCollection>
    {
    }

    public partial class SubCategoryFilter : AbstractRecord<SubCategoryFilter>
    {
        #region Table Schema
        private static TableSchema _TableSchema;
        public struct Columns
        {
            public static string SubCategoryId = "SubCategoryId";
            public static string CategoryId = "CategoryId";
            public static string FilterId = "FilterId";
            
        }
        public override TableSchema GetTableSchema()
        {
            if (null == _TableSchema)
            {
                TableSchema schema = new TableSchema();
                schema.SchemaName = @"SubCategoryFilter";
                schema.AddColumn(Columns.SubCategoryId, typeof(Int64), 0, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.FilterId, typeof(Int64), 0, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.CategoryId, typeof(Int64), 0, 0, 0, false, false, false, null);

                _TableSchema = schema;

                schema.AddIndex("pk_SubCategoryFilter", TableSchema.ClusterMode.None, TableSchema.IndexMode.PrimaryKey, TableSchema.IndexType.None, Columns.SubCategoryId, Columns.FilterId, Columns.CategoryId);
                schema.AddIndex("ix_SubCategoryFilter_SubCategoryId", TableSchema.ClusterMode.None, TableSchema.IndexMode.None, TableSchema.IndexType.None, Columns.SubCategoryId);
                schema.AddIndex("ix_SubCategoryFilter_FilterId", TableSchema.ClusterMode.None, TableSchema.IndexMode.None, TableSchema.IndexType.None, Columns.FilterId);
                schema.AddIndex("ix_SubCategoryFilter_CategoryId", TableSchema.ClusterMode.None, TableSchema.IndexMode.None, TableSchema.IndexType.None, Columns.CategoryId);

                schema.AddForeignKey("fk_SubCategoryFilter_SubCategoryId", SubCategoryFilter.Columns.SubCategoryId, SubCategory.TableSchema.SchemaName, SubCategory.Columns.SubCategoryId, TableSchema.ForeignKeyReference.Cascade, TableSchema.ForeignKeyReference.Cascade);
                schema.AddForeignKey("fk_SubCategoryFilter_CategoryId", SubCategoryFilter.Columns.CategoryId, Category.TableSchema.SchemaName, Category.Columns.CategoryId, TableSchema.ForeignKeyReference.Cascade, TableSchema.ForeignKeyReference.Cascade);
                schema.AddForeignKey("fk_SubCategoryFilter_FilterId", SubCategoryFilter.Columns.FilterId, Filter.TableSchema.SchemaName, Filter.Columns.FilterId, TableSchema.ForeignKeyReference.Cascade, TableSchema.ForeignKeyReference.Cascade);

            }

            return _TableSchema;
        }
        #endregion

        #region Private Members
        internal Int64 _SubCategoryId = 0;
        internal Int64 _FilterId = 0;
        internal Int64 _CategoryId = 0;
        #endregion

        #region Properties
        public Int64 SubCategoryId
        {
            get { return _SubCategoryId; }
            set { _SubCategoryId = value; }
        }
        public Int64 FilterId
        {
            get { return _FilterId; }
            set { _FilterId = value; }
        }
        public Int64 CategoryId
        {
            get { return _CategoryId; }
            set { _CategoryId = value; }
        }
        #endregion

        #region AbstractRecord members
        public override object GetPrimaryKeyValue()
        {
            return null;
        }

        public override void Insert(ConnectorBase conn)
        {
            Query qry = new Query(TableSchema);
            qry.Insert(Columns.SubCategoryId, SubCategoryId);
            qry.Insert(Columns.FilterId, FilterId);
            qry.Insert(Columns.CategoryId, CategoryId);

            object lastInsert = null;
            if (qry.Execute(conn, out lastInsert) > 0)
            {
                MarkOld();
            }
        }
        public override void Update(ConnectorBase conn)
        {
            Query qry = new Query(TableSchema);
            qry.Update(Columns.SubCategoryId, SubCategoryId);
            qry.Update(Columns.FilterId, FilterId);
            qry.Update(Columns.CategoryId, CategoryId);
            qry.Where(Columns.SubCategoryId, SubCategoryId);
            qry.AND(Columns.FilterId, FilterId);
            qry.AND(Columns.CategoryId, CategoryId);

            qry.Execute(conn);
        }
        public override void Read(DataReaderBase reader)
        {
            SubCategoryId = Convert.ToInt64(reader[Columns.SubCategoryId]);
            FilterId = Convert.ToInt64(reader[Columns.FilterId]);
            CategoryId = Convert.ToInt64(reader[Columns.CategoryId]);

            IsThisANewRecord = false;
        }
        #endregion

        #region Helpers
        public static SubCategoryFilter FetchByID(Int64 SubCategoryId, Int64 FilterId, Int64 CategoryId)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.SubCategoryId, SubCategoryId)
                .AND(Columns.FilterId, FilterId)
                .AND(Columns.CategoryId, CategoryId);
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                if (reader.Read())
                {
                    SubCategoryFilter item = new SubCategoryFilter();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }

        public static int Delete(Int64 SubCategoryId, Int64 FilterId, Int64 CategoryId)
        {
            Query qry = new Query(TableSchema)
                .Delete().Where(Columns.SubCategoryId, SubCategoryId)
                .AND(Columns.FilterId, FilterId)
                .AND(Columns.CategoryId, CategoryId);
            return qry.Execute();
        }
        public static SubCategoryFilter FetchByID(ConnectorBase conn, Int64 SubCategoryId, Int64 FilterId, Int64 CategoryId)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.SubCategoryId, SubCategoryId)
                .AND(Columns.FilterId, FilterId)
                .AND(Columns.CategoryId, CategoryId);
            using (DataReaderBase reader = qry.ExecuteReader(conn))
            {
                if (reader.Read())
                {
                    SubCategoryFilter item = new SubCategoryFilter();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }

        public static int Delete(ConnectorBase conn, Int64 SubCategoryId, Int64 FilterId, Int64 CategoryId)
        {
            Query qry = new Query(TableSchema)
                .Delete().Where(Columns.SubCategoryId, SubCategoryId)
                .AND(Columns.FilterId, FilterId)
                .AND(Columns.CategoryId, CategoryId);
            return qry.Execute(conn);
        }
        #endregion
    }

}
