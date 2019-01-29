using System;
using System.Collections.Generic;
using System.Text;
using dg.Sql;
using dg.Sql.Connector;
using dg.Utilities;

/*
 * ProductFilter
 * ProductFilter
 * ProductId:       INT64;
 * FilterId:        INT64;
 * SubFilterId:     INT64;
 * @INDEX:          NAME(pk_ProductFilter);PRIMARYKEY;[ProductId,FilterId,SubFilterId]; 
 * @INDEX:          NAME(ix_ProductFilter_ProductId);[ProductId];
 * @INDEX:          NAME(ix_ProductFilter_FilterId);[FilterId];
 * @INDEX:          NAME(ix_ProductFilter_SubFilterId);[SubFilterId];
 * @FOREIGNKEY:     NAME(fk_ProductFilter_ProductId); FOREIGNTABLE(Product); COLUMNS[ProductId]; FOREIGNCOLUMNS[ProductId]; ONUPDATE(CASCADE); ONDELETE(CASCADE);
 * @FOREIGNKEY:     NAME(fk_ProductFilter_SubFilterId); FOREIGNTABLE(SubFilter); COLUMNS[SubFilterId]; FOREIGNCOLUMNS[SubFilterId]; ONUPDATE(CASCADE); ONDELETE(CASCADE);
 * @FOREIGNKEY:     NAME(fk_ProductFilter_FilterId); FOREIGNTABLE(Filter); COLUMNS[FilterId]; FOREIGNCOLUMNS[FilterId]; ONUPDATE(CASCADE); ONDELETE(CASCADE);
 * */

namespace Snoopi.core.DAL
{

    public partial class ProductFilterCollection : AbstractRecordList<ProductFilter, ProductFilterCollection>
    {
    }

    public partial class ProductFilter : AbstractRecord<ProductFilter>
    {
        #region Table Schema
        private static TableSchema _TableSchema;
        public struct Columns
        {
            public static string ProductId = "ProductId";
            public static string FilterId = "FilterId";
            public static string SubFilterId = "SubFilterId";
        }
        public override TableSchema GetTableSchema()
        {
            if (null == _TableSchema)
            {
                TableSchema schema = new TableSchema();
                schema.SchemaName = @"ProductFilter";
                schema.AddColumn(Columns.ProductId, typeof(Int64), 0, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.FilterId, typeof(Int64), 0, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.SubFilterId, typeof(Int64), 0, 0, 0, false, false, false, null);

                _TableSchema = schema;

                schema.AddIndex("pk_ProductFilter", TableSchema.ClusterMode.None, TableSchema.IndexMode.PrimaryKey, TableSchema.IndexType.None, Columns.ProductId, Columns.FilterId, Columns.SubFilterId);
                schema.AddIndex("ix_ProductFilter_ProductId", TableSchema.ClusterMode.None, TableSchema.IndexMode.None, TableSchema.IndexType.None, Columns.ProductId);
                schema.AddIndex("ix_ProductFilter_FilterId", TableSchema.ClusterMode.None, TableSchema.IndexMode.None, TableSchema.IndexType.None, Columns.FilterId);
                schema.AddIndex("ix_ProductFilter_SubFilterId", TableSchema.ClusterMode.None, TableSchema.IndexMode.None, TableSchema.IndexType.None, Columns.SubFilterId);

                schema.AddForeignKey("fk_ProductFilter_ProductId", ProductFilter.Columns.ProductId, Product.TableSchema.SchemaName, Product.Columns.ProductId, TableSchema.ForeignKeyReference.Cascade, TableSchema.ForeignKeyReference.Cascade);
                schema.AddForeignKey("fk_ProductFilter_SubFilterId", ProductFilter.Columns.SubFilterId, SubFilter.TableSchema.SchemaName, SubFilter.Columns.SubFilterId, TableSchema.ForeignKeyReference.Cascade, TableSchema.ForeignKeyReference.Cascade);
                schema.AddForeignKey("fk_ProductFilter_FilterId", ProductFilter.Columns.FilterId, Filter.TableSchema.SchemaName, Filter.Columns.FilterId, TableSchema.ForeignKeyReference.Cascade, TableSchema.ForeignKeyReference.Cascade);

            }

            return _TableSchema;
        }
        #endregion

        #region Private Members
        internal Int64 _ProductId = 0;
        internal Int64 _FilterId = 0;
        internal Int64 _SubFilterId = 0;
        #endregion

        #region Properties
        public Int64 ProductId
        {
            get { return _ProductId; }
            set { _ProductId = value; }
        }
        public Int64 FilterId
        {
            get { return _FilterId; }
            set { _FilterId = value; }
        }
        public Int64 SubFilterId
        {
            get { return _SubFilterId; }
            set { _SubFilterId = value; }
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
            qry.Insert(Columns.ProductId, ProductId);
            qry.Insert(Columns.FilterId, FilterId);
            qry.Insert(Columns.SubFilterId, SubFilterId);

            object lastInsert = null;
            if (qry.Execute(conn, out lastInsert) > 0)
            {
                MarkOld();
            }
        }
        public override void Update(ConnectorBase conn)
        {
            Query qry = new Query(TableSchema);
            qry.Update(Columns.ProductId, ProductId);
            qry.Update(Columns.FilterId, FilterId);
            qry.Update(Columns.SubFilterId, SubFilterId);
            qry.Where(Columns.ProductId, ProductId);
            qry.AND(Columns.FilterId, FilterId);
            qry.AND(Columns.SubFilterId, SubFilterId);

            qry.Execute(conn);
        }
        public override void Read(DataReaderBase reader)
        {
            ProductId = Convert.ToInt64(reader[Columns.ProductId]);
            FilterId = Convert.ToInt64(reader[Columns.FilterId]);
            SubFilterId = Convert.ToInt64(reader[Columns.SubFilterId]);

            IsThisANewRecord = false;
        }
        #endregion

        #region Helpers
        public static ProductFilter FetchByID(Int64 ProductId, Int64 FilterId, Int64 SubFilterId)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.ProductId, ProductId)
                .AND(Columns.FilterId, FilterId)
                .AND(Columns.SubFilterId, SubFilterId);
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                if (reader.Read())
                {
                    ProductFilter item = new ProductFilter();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }

        public static int Delete(Int64 ProductId, Int64 FilterId, Int64 SubFilterId)
        {
            Query qry = new Query(TableSchema)
                .Delete().Where(Columns.ProductId, ProductId)
                .AND(Columns.FilterId, FilterId)
                .AND(Columns.SubFilterId, SubFilterId);
            return qry.Execute();
        }
        public static ProductFilter FetchByID(ConnectorBase conn, Int64 ProductId, Int64 FilterId, Int64 SubFilterId)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.ProductId, ProductId)
                .AND(Columns.FilterId, FilterId)
                .AND(Columns.SubFilterId, SubFilterId);
            using (DataReaderBase reader = qry.ExecuteReader(conn))
            {
                if (reader.Read())
                {
                    ProductFilter item = new ProductFilter();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }

        public static int Delete(ConnectorBase conn, Int64 ProductId, Int64 FilterId, Int64 SubFilterId)
        {
            Query qry = new Query(TableSchema)
                .Delete().Where(Columns.ProductId, ProductId)
                .AND(Columns.FilterId, FilterId)
                .AND(Columns.SubFilterId, SubFilterId);
            return qry.Execute(conn);
        }
        #endregion
    }

}
