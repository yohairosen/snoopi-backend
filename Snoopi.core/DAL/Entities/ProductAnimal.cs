using System;
using System.Collections.Generic;
using System.Text;
using dg.Sql;
using dg.Sql.Connector;
using dg.Utilities;

/*
 * ProductAnimal
 * ProductAnimal
 * ProductId:       INT64;
 * AnimalId:        INT64;
 * @INDEX:          NAME(pk_ProductAnimal);PRIMARYKEY;[ProductId,AnimalId]; 
 * @INDEX:          NAME(ix_ProductAnimal_ProductId);[ProductId];
 * @INDEX:          NAME(ix_ProductAnimal_AnimalId);[AnimalId];
 * @FOREIGNKEY:     NAME(fk_ProductAnimal_ProductId); FOREIGNTABLE(Product); COLUMNS[ProductId]; FOREIGNCOLUMNS[ProductId]; ONUPDATE(CASCADE); ONDELETE(CASCADE);
 * @FOREIGNKEY:     NAME(fk_ProductAnimal_AnimalId); FOREIGNTABLE(Animal); COLUMNS[AnimalId]; FOREIGNCOLUMNS[AnimalId]; ONUPDATE(CASCADE); ONDELETE(CASCADE);
 * */

namespace Snoopi.core.DAL
{

    public partial class ProductAnimalCollection : AbstractRecordList<ProductAnimal, ProductAnimalCollection>
    {
    }

    public partial class ProductAnimal : AbstractRecord<ProductAnimal>
    {
        #region Table Schema
        private static TableSchema _TableSchema;
        public struct Columns
        {
            public static string ProductId = "ProductId";
            public static string AnimalId = "AnimalId";
        }
        public override TableSchema GetTableSchema()
        {
            if (null == _TableSchema)
            {
                TableSchema schema = new TableSchema();
                schema.SchemaName = @"ProductAnimal";
                schema.AddColumn(Columns.ProductId, typeof(Int64), 0, 0, 0, false, false, false, null);
                schema.AddColumn(Columns.AnimalId, typeof(Int64), 0, 0, 0, false, false, false, null);

                _TableSchema = schema;

                schema.AddIndex("pk_ProductAnimal", TableSchema.ClusterMode.None, TableSchema.IndexMode.PrimaryKey, TableSchema.IndexType.None, Columns.ProductId, Columns.AnimalId);
                schema.AddIndex("ix_ProductAnimal_ProductId", TableSchema.ClusterMode.None, TableSchema.IndexMode.None, TableSchema.IndexType.None, Columns.ProductId);
                schema.AddIndex("ix_ProductAnimal_AnimalId", TableSchema.ClusterMode.None, TableSchema.IndexMode.None, TableSchema.IndexType.None, Columns.AnimalId);

                schema.AddForeignKey("fk_ProductAnimal_ProductId", ProductAnimal.Columns.ProductId, Product.TableSchema.SchemaName, Product.Columns.ProductId, TableSchema.ForeignKeyReference.Cascade, TableSchema.ForeignKeyReference.Cascade);
                schema.AddForeignKey("fk_ProductAnimal_AnimalId", ProductAnimal.Columns.AnimalId, Animal.TableSchema.SchemaName, Animal.Columns.AnimalId, TableSchema.ForeignKeyReference.Cascade, TableSchema.ForeignKeyReference.Cascade);

            }

            return _TableSchema;
        }
        #endregion

        #region Private Members
        internal Int64 _ProductId = 0;
        internal Int64 _AnimalId = 0;
        #endregion

        #region Properties
        public Int64 ProductId
        {
            get { return _ProductId; }
            set { _ProductId = value; }
        }
        public Int64 AnimalId
        {
            get { return _AnimalId; }
            set { _AnimalId = value; }
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
            qry.Insert(Columns.AnimalId, AnimalId);

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
            qry.Update(Columns.AnimalId, AnimalId);
            qry.Where(Columns.ProductId, ProductId);
            qry.AND(Columns.AnimalId, AnimalId);

            qry.Execute(conn);
        }
        public override void Read(DataReaderBase reader)
        {
            ProductId = Convert.ToInt64(reader[Columns.ProductId]);
            AnimalId = Convert.ToInt64(reader[Columns.AnimalId]);

            IsThisANewRecord = false;
        }
        #endregion

        #region Helpers
        public static ProductAnimal FetchByID(Int64 ProductId, Int64 AnimalId)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.ProductId, ProductId)
                .AND(Columns.AnimalId, AnimalId);
            using (DataReaderBase reader = qry.ExecuteReader())
            {
                if (reader.Read())
                {
                    ProductAnimal item = new ProductAnimal();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }

        public static int Delete(Int64 ProductId, Int64 AnimalId)
        {
            Query qry = new Query(TableSchema)
                .Delete().Where(Columns.ProductId, ProductId)
                .AND(Columns.AnimalId, AnimalId);
            return qry.Execute();
        }
        public static ProductAnimal FetchByID(ConnectorBase conn, Int64 ProductId, Int64 AnimalId)
        {
            Query qry = new Query(TableSchema)
                .Where(Columns.ProductId, ProductId)
                .AND(Columns.AnimalId, AnimalId);
            using (DataReaderBase reader = qry.ExecuteReader(conn))
            {
                if (reader.Read())
                {
                    ProductAnimal item = new ProductAnimal();
                    item.Read(reader);
                    return item;
                }
            }
            return null;
        }

        public static int Delete(ConnectorBase conn, Int64 ProductId, Int64 AnimalId)
        {
            Query qry = new Query(TableSchema)
                .Delete().Where(Columns.ProductId, ProductId)
                .AND(Columns.AnimalId, AnimalId);
            return qry.Execute(conn);
        }
        #endregion
    }

}
