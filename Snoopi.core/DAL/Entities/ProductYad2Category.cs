using System;
using System.Collections.Generic;
using System.Text;
using dg.Sql;
using dg.Sql.Connector;

/*
 * ProductYad2Category
 * ProductYad2Category
 * CategoryYad2Id:          INT64;
 * ProductId:               INT64;
 * @INDEX:                  NAME(pk_ProductYad2Category);PRIMARYKEY;[CategoryYad2Id,ProductId]; 
 * @FOREIGNKEY:             NAME(fk_ProductYad2Category_ProductId); FOREIGNTABLE(ProductYad2); COLUMNS[ProductId]; FOREIGNCOLUMNS[ProductId]; ONUPDATE(CASCADE); ONDELETE(CASCADE);
 * @FOREIGNKEY:             NAME(fk_ProductYad2Category_CategoryYad2Id); FOREIGNTABLE(CategoryYad2); COLUMNS[CategoryYad2Id]; FOREIGNCOLUMNS[CategoryYad2Id]; ONUPDATE(CASCADE); ONDELETE(CASCADE);
 * */

namespace Snoopi.core.DAL
{
    public partial class ProductYad2CategoryCollection : AbstractRecordList<ProductYad2Category, ProductYad2CategoryCollection>
    {
    }

    public partial class ProductYad2Category : AbstractRecord<ProductYad2Category>
    {
        #region Table Schema
        private static TableSchema _TableSchema;
        public struct Columns
        {
            public static string CategoryYad2Id = "CategoryYad2Id";
            public static string ProductId = "ProductId";
        }
        public override TableSchema GetTableSchema()
        {
            if (null == _TableSchema)
            {
                TableSchema schema = new TableSchema();
                schema.SchemaName = @"ProductYad2Category";
                schema.AddColumn(Columns.CategoryYad2Id, typeof(Int64), 0, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.ProductId, typeof(Int64), 0, 0, 0, false, false, false, null);

                _TableSchema = schema;

                schema.AddIndex("pk_ProductYad2Category", TableSchema.ClusterMode.None, TableSchema.IndexMode.PrimaryKey, TableSchema.IndexType.None, Columns.CategoryYad2Id, Columns.ProductId);

                schema.AddForeignKey("fk_ProductYad2Category_ProductId", ProductYad2Category.Columns.ProductId, ProductYad2.TableSchema.SchemaName, ProductYad2.Columns.ProductId, TableSchema.ForeignKeyReference.Cascade, TableSchema.ForeignKeyReference.Cascade);
                schema.AddForeignKey("fk_ProductYad2Category_CategoryYad2Id", ProductYad2Category.Columns.CategoryYad2Id, CategoryYad2.TableSchema.SchemaName, CategoryYad2.Columns.CategoryYad2Id, TableSchema.ForeignKeyReference.Cascade, TableSchema.ForeignKeyReference.Cascade);

            }

            return _TableSchema;
        }
        #endregion

        #region Private Members
        internal Int64 _CategoryYad2Id = 0;
        internal Int64 _ProductId = 0;
        #endregion

        #region Properties
        public Int64 CategoryYad2Id
        {
            get { return _CategoryYad2Id; }
            set { _CategoryYad2Id = value; }
        }
        public Int64 ProductId
        {
            get { return _ProductId; }
            set { _ProductId = value; }
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
            qry.Insert(Columns.CategoryYad2Id, CategoryYad2Id);
            qry.Insert(Columns.ProductId, ProductId);

            object lastInsert = null;
            if (qry.Execute(conn, out lastInsert) > 0)
            {
                MarkOld();
            }
        }
        public override void Update(ConnectorBase conn)
        {
            Query qry = new Query(TableSchema);
            qry.Update(Columns.CategoryYad2Id, CategoryYad2Id);
            qry.Update(Columns.ProductId, ProductId);
            qry.Where(Columns.CategoryYad2Id, CategoryYad2Id);
            qry.AND(Columns.ProductId, ProductId);

            qry.Execute(conn);
        }
        public override void Read(DataReaderBase reader)
        {
            CategoryYad2Id = Convert.ToInt64(reader[Columns.CategoryYad2Id]);
            ProductId = Convert.ToInt64(reader[Columns.ProductId]);

            IsThisANewRecord = false;
        }
        #endregion

        #region Helpers
        public static ProductYad2Category FetchByID(Int64 CategoryYad2Id, Int64 ProductId)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.CategoryYad2Id, CategoryYad2Id)
                .AND(Columns.ProductId, ProductId);
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                if (reader.Read())
                {
                    ProductYad2Category item = new ProductYad2Category();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }

        public static int Delete(Int64 CategoryYad2Id, Int64 ProductId)
        {
            Query qry = new Query(TableSchema)
                .Delete().Where(Columns.CategoryYad2Id, CategoryYad2Id)
                .AND(Columns.ProductId, ProductId);
            return qry.Execute();
        }
        public static ProductYad2Category FetchByID(ConnectorBase conn, Int64 CategoryYad2Id, Int64 ProductId)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.CategoryYad2Id, CategoryYad2Id)
                .AND(Columns.ProductId, ProductId);
            using (DataReaderBase reader = qry.ExecuteReader(conn))
            {
                if (reader.Read())
                {
                    ProductYad2Category item = new ProductYad2Category();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }

        public static int Delete(ConnectorBase conn, Int64 CategoryYad2Id, Int64 ProductId)
        {
            Query qry = new Query(TableSchema)
                .Delete().Where(Columns.CategoryYad2Id, CategoryYad2Id)
                .AND(Columns.ProductId, ProductId);
            return qry.Execute(conn);
        }
        #endregion
    }
}